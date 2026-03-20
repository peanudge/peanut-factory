using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionManager : IAcquisitionService, IChannelCalibration
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly object _lock = new();
    private readonly ChannelEventLog _eventLog = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private bool _disposed;
    private Timer? _triggerTimer;
    private bool _snapshotInProgress;
    private ChannelState _channelState = ChannelState.None;
    private ProfileId? _channelProfileId;
    private TriggerMode? _channelTriggerMode;

    public AcquisitionManager(IGrabService grabService, ICamFileService camFileService)
    {
        _grabService = grabService;
        _camFileService = camFileService;
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
            if (_snapshotInProgress)
                return new HashSet<ChannelAction>();

            return _channelState switch
            {
                ChannelState.None   => new HashSet<ChannelAction> { ChannelAction.Create, ChannelAction.Start, ChannelAction.Snapshot },
                ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Release, ChannelAction.Snapshot },
                ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
                _                   => new HashSet<ChannelAction>(),
            };
        }
    }

    // IChannelCalibration implementation

    public bool IsCalibrationAvailable => IsActive;

    public void PerformBlackCalibration() => GetRequiredActiveChannel().PerformBlackCalibration();

    public void PerformWhiteCalibration() => GetRequiredActiveChannel().PerformWhiteCalibration();

    public void PerformWhiteBalanceOnce() => GetRequiredActiveChannel().PerformWhiteBalanceOnce();

    public void SetFlatFieldCorrection(bool enable) => GetRequiredActiveChannel().SetFlatFieldCorrection(enable);

    public ExposureInfo GetExposure()
    {
        var channel = GetRequiredActiveChannel();
        var range = channel.GetExposureRange();
        return new ExposureInfo
        {
            ExposureUs = channel.GetExposureUs(),
            GainDb = channel.GetGainDb(),
            ExposureRange = new ExposureRangeInfo { Min = range.Min, Max = range.Max },
        };
    }

    public ExposureInfo SetExposure(double? exposureUs, double? gainDb)
    {
        var channel = GetRequiredActiveChannel();
        if (exposureUs.HasValue) channel.SetExposureUs(exposureUs.Value);
        if (gainDb.HasValue) channel.SetGainDb(gainDb.Value);
        var range = channel.GetExposureRange();
        return new ExposureInfo
        {
            ExposureUs = channel.GetExposureUs(),
            GainDb = channel.GetGainDb(),
            ExposureRange = new ExposureRangeInfo { Min = range.Min, Max = range.Max },
        };
    }

    private GrabChannel GetRequiredActiveChannel()
    {
        lock (_lock)
        {
            if (_channelState != ChannelState.Active || _channel == null)
                throw new InvalidOperationException("No active acquisition channel.");
            return _channel;
        }
    }

    internal GrabChannel? Channel
    {
        get { lock (_lock) return _channel; }
    }

    public void CreateChannel(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        lock (_lock)
        {
            if (_snapshotInProgress)
                throw new InvalidOperationException("A snapshot is in progress. Wait for it to complete.");

            if (_channelState != ChannelState.None)
                throw new InvalidOperationException("A channel already exists. Release it first.");

            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode)
                : camFile.ToChannelOptions();

            _channel = _grabService.CreateChannel(options);
            _channelProfileId = profileId;
            _channelTriggerMode = triggerMode;
            _channelState = ChannelState.Idle;
        }
    }

    public void ReleaseChannel()
    {
        GrabChannel? channel;

        lock (_lock)
        {
            if (_channelState == ChannelState.Active)
                throw new InvalidOperationException("Cannot release an active channel. Stop acquisition first.");

            if (_channelState == ChannelState.None)
                return;

            channel = _channel;
            _channel = null;
            _channelProfileId = null;
            _channelTriggerMode = null;
            _channelState = ChannelState.None;
            _lastFrame = null;
            _statistics = null;
        }

        if (channel != null)
            _grabService.ReleaseChannel(channel);
    }

    public void Start(int? frameCount = null, int? intervalMs = null)
    {
        const int minIntervalMs = 50;

        if (intervalMs.HasValue && intervalMs.Value > 0 && intervalMs.Value < minIntervalMs)
            throw new ArgumentException($"intervalMs must be at least {minIntervalMs}ms, got {intervalMs.Value}ms.");

        lock (_lock)
        {
            if (_snapshotInProgress)
                throw new InvalidOperationException("A snapshot is in progress. Wait for it to complete.");

            if (_channelState == ChannelState.None)
                throw new InvalidOperationException("No channel exists. Create a channel first.");

            if (_channelState == ChannelState.Active)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");

            _lastFrame = null;
            _lastError = null;
            _statistics = new AcquisitionStatistics();

            _channel!.FrameAcquired    += OnFrameAcquired;
            _channel.AcquisitionError  += OnAcquisitionError;
            _channel.AcquisitionEnded  += OnAcquisitionEnded;

            _channelState = ChannelState.Active;
            _statistics.Start();
            _channel.StartAcquisition(frameCount ?? -1);

            if (intervalMs.HasValue && intervalMs.Value > 0)
            {
                _triggerTimer = new Timer(_ =>
                {
                    lock (_lock)
                    {
                        if (_channel?.IsActive == true)
                            _channel.SendSoftwareTrigger();
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

    public ImageData Snapshot(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        lock (_lock)
        {
            if (_channelState == ChannelState.Active)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");

            _snapshotInProgress = true;
        }

        try
        {
            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode, useCallback: false)
                : camFile.ToChannelOptions(useCallback: false);

            // Snapshot requires a software-triggerable mode.
            // Throw explicitly rather than silently overriding the caller's intent.
            if (triggerMode.HasValue &&
                options.TriggerMode != McTrigMode.MC_TrigMode_SOFT &&
                options.TriggerMode != McTrigMode.MC_TrigMode_COMBINED)
            {
                throw new ArgumentException(
                    $"Snapshot requires SOFT or COMBINED trigger mode. " +
                    $"The requested mode '{options.TriggerMode}' is not software-triggerable. " +
                    "Use a cam file or trigger mode that supports software triggering.",
                    nameof(triggerMode));
            }

            // If no explicit trigger mode was requested, force SOFT for reliable snapshot behavior.
            if (!triggerMode.HasValue)
            {
                options.TriggerMode = McTrigMode.MC_TrigMode_SOFT;
            }

            var channel = _grabService.CreateChannel(options);
            try
            {
                channel.StartAcquisition(1);
                channel.SendSoftwareTrigger();

                var surface = channel.WaitForFrame(5000)
                    ?? throw new TimeoutException("Snapshot timed out waiting for frame.");

                try
                {
                    return ImageData.FromSurface(surface);
                }
                finally
                {
                    channel.ReleaseSurface(surface);
                }
            }
            finally
            {
                channel.StopAcquisition();
                _grabService.ReleaseChannel(channel);
            }
        }
        finally
        {
            lock (_lock)
            {
                _snapshotInProgress = false;
            }
        }
    }

    /// <summary>
    /// Frame callback — receives already-copied ImageData from GrabChannel's copy thread.
    /// </summary>
    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        ProcessFrame(e.Image);
    }

    /// <summary>
    /// Error callback — called from GrabChannel's native callback thread.
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
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            _lastFrame = image;
            _statistics?.RecordFrame();
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

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
        GrabChannel? channel;
        lock (_lock)
        {
            channel = _channel;
            _channel = null;
            _channelState = ChannelState.None;
            _channelProfileId = null;
            _channelTriggerMode = null;
        }
        if (channel != null)
            _grabService.ReleaseChannel(channel);
    }
}
