using System.Collections.Concurrent;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionService : IAcquisitionService, IChannelCalibration, IExposureControl
{
    private readonly IAcquisitionChannelManager _channelManager;
    private readonly ICamFileService _camFileService;
    private readonly ILatencyService _latencyService;
    private readonly object _lock = new();
    private readonly ChannelEventLog _eventLog = new();
    private readonly ConcurrentQueue<DateTimeOffset> _triggerTimestamps = new();

    private AcquisitionChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private bool _disposed;
    private Timer? _triggerTimer;
    private ChannelState _channelState = ChannelState.NotAllocated;
    private ProfileId? _channelProfileId;
    private TriggerMode? _channelTriggerMode;
    private double _desiredExposureUs = 10000.0;
    private int? _targetFrameCount;

    public AcquisitionService(IAcquisitionChannelManager channelManager, ICamFileService camFileService, ILatencyService latencyService)
    {
        _channelManager = channelManager;
        _camFileService = camFileService;
        _latencyService = latencyService;
    }

    public ChannelState ChannelState
    {
        get { lock (_lock) return _channelState; }
    }

    public bool IsActive
    {
        get { lock (_lock) return _channelState == ChannelState.Active; }
    }

    public ProfileId? ActiveProfileId
    {
        get { lock (_lock) return _channelProfileId; }
    }

    public TriggerMode? ChannelTriggerMode
    {
        get { lock (_lock) return _channelTriggerMode; }
    }

    internal ImageData? LastFrame
    {
        get { lock (_lock) return _lastFrame; }
    }

    public bool HasFrame
    {
        get { lock (_lock) return _lastFrame != null; }
    }

    public ImageData? GetLatestFrame()
    {
        lock (_lock) return _lastFrame;
    }

    public string? LastError
    {
        get { lock (_lock) return _lastError; }
    }

    public AcquisitionStatisticsSnapshot? GetStatistics()
    {
        lock (_lock)
        {
            var snapshot = _statistics?.GetSnapshot();
            if (snapshot.HasValue && _channel != null)
            {
                return snapshot.Value with
                {
                    CopyDropCount = _channel.CopyDropCount,
                    ClusterUnavailableCount = _channel.ClusterUnavailableCount,
                };
            }
            return snapshot;
        }
    }

    public IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50)
    {
        return _eventLog.GetRecent(max);
    }

    public IReadOnlySet<ChannelAction> GetAllowedActions()
    {
        lock (_lock)
        {
            return _channelState switch
            {
                ChannelState.NotAllocated => new HashSet<ChannelAction> { ChannelAction.Start },
                ChannelState.Idle         => new HashSet<ChannelAction> { ChannelAction.Start },
                ChannelState.Active       => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
                _                         => new HashSet<ChannelAction>(),
            };
        }
    }

    // IChannelCalibration implementation

    public bool IsCalibrationAvailable =>
        ChannelState == ChannelState.Idle || ChannelState == ChannelState.Active;

    public void PerformBlackCalibration() => GetRequiredChannel().PerformBlackCalibration();

    public void PerformWhiteCalibration() => GetRequiredChannel().PerformWhiteCalibration();

    public void PerformWhiteBalanceOnce() => GetRequiredChannel().PerformWhiteBalanceOnce();

    public void SetFlatFieldCorrection(bool enable) => GetRequiredChannel().SetFlatFieldCorrection(enable);

    public ExposureInfo GetExposure()
    {
        lock (_lock)
        {
            if (_channelState == ChannelState.Active && _channel != null)
            {
                _desiredExposureUs = _channel.GetExposureUs();
                var range = _channel.GetExposureRange();
                return new ExposureInfo
                {
                    ExposureUs = _desiredExposureUs,
                    ExposureRange = new ExposureRangeInfo { Min = range.Min, Max = range.Max },
                };
            }
            return new ExposureInfo { ExposureUs = _desiredExposureUs };
        }
    }

    public ExposureInfo SetExposure(double? exposureUs)
    {
        lock (_lock)
        {
            if (exposureUs.HasValue)
                _desiredExposureUs = exposureUs.Value;

            if (_channelState == ChannelState.Active && _channel != null)
            {
                _channel.SetExposureUs(_desiredExposureUs);
                var range = _channel.GetExposureRange();
                return new ExposureInfo
                {
                    ExposureUs = _channel.GetExposureUs(),
                    ExposureRange = new ExposureRangeInfo { Min = range.Min, Max = range.Max },
                };
            }
            return new ExposureInfo { ExposureUs = _desiredExposureUs };
        }
    }

    private AcquisitionChannel GetRequiredChannel()
    {
        lock (_lock)
        {
            if ((_channelState != ChannelState.Idle && _channelState != ChannelState.Active) || _channel == null)
                throw new InvalidOperationException("No channel exists. Create a channel first.");
            return _channel;
        }
    }

    internal AcquisitionChannel? Channel
    {
        get { lock (_lock) return _channel; }
    }

    public void CreateChannel(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        lock (_lock)
        {
            if (_channelState != ChannelState.NotAllocated)
                throw new InvalidOperationException("A channel already exists. Release it first.");

            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode)
                : camFile.ToChannelOptions();

            _channel = _channelManager.CreateChannel(options);
            _channelProfileId = profileId;
            _channelTriggerMode = triggerMode;
            _channelState = ChannelState.Idle;
        }
    }

    public void ReleaseChannel()
    {
        AcquisitionChannel? channel;

        lock (_lock)
        {
            if (_channelState == ChannelState.Active)
                throw new InvalidOperationException("Cannot release an active channel. Stop acquisition first.");

            if (_channelState == ChannelState.NotAllocated)
                return;

            channel = _channel;
            _channel = null;
            _channelProfileId = null;
            _channelTriggerMode = null;
            _channelState = ChannelState.NotAllocated;
            _lastFrame = null;
            _statistics = null;
        }

        if (channel != null)
            _channelManager.ReleaseChannel(channel);
    }

    public void Start(int? frameCount = null, int? intervalMs = null)
    {
        const int minIntervalMs = 50;

        if (intervalMs.HasValue && intervalMs.Value > 0 && intervalMs.Value < minIntervalMs)
            throw new ArgumentException($"intervalMs must be at least {minIntervalMs}ms, got {intervalMs.Value}ms.");

        lock (_lock)
        {
            if (_channelState == ChannelState.NotAllocated)
                throw new InvalidOperationException("No channel exists. Create a channel first.");

            if (_channelState == ChannelState.Active)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");

            _lastFrame = null;
            _lastError = null;
            _statistics = new AcquisitionStatistics();

            _channel!.FrameAcquired    += OnFrameAcquired;
            _channel.AcquisitionError  += OnAcquisitionError;
            _channel.AcquisitionEnded  += OnAcquisitionEnded;

            _targetFrameCount = frameCount;
            _channelState = ChannelState.Active;
            _statistics.Start();
            _channel.StartAcquisition(frameCount ?? -1);
            try { _channel.SetExposureUs(_desiredExposureUs); } catch { /* best-effort */ }

            if (intervalMs.HasValue && intervalMs.Value > 0)
            {
                _triggerTimer = new Timer(_ =>
                {
                    lock (_lock)
                    {
                        if (_channel?.IsActive == true)
                        {
                            _triggerTimestamps.Enqueue(DateTimeOffset.UtcNow);
                            _channel.SendSoftwareTrigger();
                        }
                    }
                }, null, 0, intervalMs.Value);
            }

            _eventLog.Add(new ChannelEvent(
                DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
                $"Acquisition started with profile '{_channelProfileId?.Value}'" +
                (frameCount.HasValue ? $", frameCount={frameCount}" : "") +
                (intervalMs.HasValue ? $", intervalMs={intervalMs}" : "")));
        }
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            if (_channelState != ChannelState.Active)
                return;

            _channelState = ChannelState.Idle;
            _targetFrameCount = null;

            tcs = _triggerTcs;
            _triggerTcs = null;

            _triggerTimer?.Dispose();
            _triggerTimer = null;

            _statistics?.Stop();
            _channel!.StopAcquisition();
            _channel.FrameAcquired    -= OnFrameAcquired;
            _channel.AcquisitionError -= OnAcquisitionError;
            _channel.AcquisitionEnded -= OnAcquisitionEnded;
        }

        _eventLog.Add(new ChannelEvent(
            DateTime.UtcNow, ChannelEventType.AcquisitionStopped,
            "Acquisition stopped"));

        tcs?.TrySetCanceled();
    }

    public async Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000)
    {
        TaskCompletionSource<ImageData> tcs;

        lock (_lock)
        {
            if (_channel == null || !_channel.IsActive)
                throw new InvalidOperationException("No active acquisition. Start acquisition first.");

            if (_triggerTcs != null)
                throw new InvalidOperationException("A trigger is already pending. Wait for it to complete.");

            // Validate trigger mode — software trigger only works in SOFT or COMBINED mode
            if (!_channel.SupportsSoftwareTrigger)
            {
                throw new InvalidOperationException(
                    $"TriggerAndWaitAsync requires SOFT or COMBINED trigger mode, but channel is configured for {_channel.TriggerMode}. " +
                    "Use Start() with the correct trigger mode, or use the frame event for IMMEDIATE/HARD trigger modes.");
            }

            tcs = new TaskCompletionSource<ImageData>(TaskCreationOptions.RunContinuationsAsynchronously);
            _triggerTcs = tcs;
            _triggerTimestamps.Enqueue(DateTimeOffset.UtcNow);
            _channel.SendSoftwareTrigger();
        }

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));

        if (completed != tcs.Task)
        {
            lock (_lock) { _triggerTcs = null; }
            throw new TimeoutException("Trigger timed out waiting for frame.");
        }

        return await tcs.Task;
    }

    /// <summary>
    /// Frame callback — receives already-copied ImageData from AcquisitionChannel's copy thread.
    /// </summary>
    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        ProcessFrame(e.Image);

        int? target;
        long frameCount;
        lock (_lock)
        {
            target = _targetFrameCount;
            frameCount = _statistics?.FrameCount ?? 0;
        }

        if (target.HasValue && frameCount >= target.Value)
            Task.Run(Stop);
    }

    /// <summary>
    /// Error callback — called from AcquisitionChannel's native callback thread.
    /// </summary>
    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
    {
        ProcessError(e.Message, e.Signal);
        if (e.Signal == McSignal.MC_SIG_UNRECOVERABLE_OVERRUN)
            Stop();
    }

    /// <summary>
    /// Called when the acquisition sequence ends (MC_SIG_END_CHANNEL_ACTIVITY).
    /// Automatically cleans up so Start() can be called again without requiring an explicit Stop().
    /// </summary>
    private void OnAcquisitionEnded(object? sender, EventArgs e)
    {
        Stop();
    }

    private void ProcessFrame(ImageData image)
    {
        var frameReceivedAt = DateTimeOffset.UtcNow;

        TaskCompletionSource<ImageData>? tcs;
        long frameIndex;
        string? profileId;

        lock (_lock)
        {
            _lastFrame = image;
            _statistics?.RecordFrame();
            frameIndex = _statistics?.FrameCount ?? 0;
            profileId = _channelProfileId?.Value;
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        if (_triggerTimestamps.TryDequeue(out var triggerSentAt))
            _latencyService.Record(triggerSentAt, frameReceivedAt, frameIndex, profileId);

        tcs?.TrySetResult(image);
    }

    private void ProcessError(string message, McSignal signal)
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            _lastError = message;
            _statistics?.RecordError();

            if (signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE)
                _statistics?.RecordDroppedFrame();

            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        var eventType = signal switch
        {
            McSignal.MC_SIG_CLUSTER_UNAVAILABLE => ChannelEventType.BufferUnavailable,
            _ => ChannelEventType.AcquisitionError,
        };
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, eventType, message));

        tcs?.TrySetException(new InvalidOperationException($"Acquisition error: {message}"));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
        AcquisitionChannel? channel;
        lock (_lock)
        {
            channel = _channel;
            _channel = null;
            _channelState = ChannelState.NotAllocated;
            _channelProfileId = null;
            _channelTriggerMode = null;
        }
        if (channel != null)
            _channelManager.ReleaseChannel(channel);
    }
}
