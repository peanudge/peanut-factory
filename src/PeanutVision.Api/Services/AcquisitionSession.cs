using System.Collections.Concurrent;
using PeanutVision.Api.Exceptions;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionSession : IAcquisitionSession, IExposureSource
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly IFrameQueue _frameQueue;
    private readonly ILatencyService _latencyService;
    private readonly object _lock = new();
    private readonly ChannelEventLog _eventLog = new();
    private readonly ConcurrentQueue<DateTimeOffset> _triggerTimestamps = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private ChannelState _state = ChannelState.None;
    private ProfileId? _activeProfileId;
    private bool _disposed;

    public event Action<ImageData>? FrameAcquired;

    public bool IsActive => _state == ChannelState.Active;
    public bool HasFrame { get { lock (_lock) return _lastFrame != null; } }
    public string? LastError { get { lock (_lock) return _lastError; } }
    public ChannelState ChannelState { get { lock (_lock) return _state; } }
    public ProfileId? ActiveProfileId { get { lock (_lock) return _activeProfileId; } }

    // IExposureSource (internal — accessed by ExposureController via DI)
    bool IExposureSource.HasActiveChannel { get { lock (_lock) return _channel?.IsActive ?? false; } }
    double IExposureSource.GetExposureUs() { lock (_lock) return _channel?.GetExposureUs() ?? 10_000.0; }
    (double Min, double Max) IExposureSource.GetExposureRange() { lock (_lock) return _channel?.GetExposureRange() ?? (1_000, 100_000); }
    void IExposureSource.SetExposureUs(double us) { lock (_lock) _channel?.SetExposureUs(us); }

    public AcquisitionSession(
        IGrabService grabService,
        ICamFileService camFileService,
        IFrameQueue frameQueue,
        ILatencyService latencyService)
    {
        _grabService = grabService;
        _camFileService = camFileService;
        _frameQueue = frameQueue;
        _latencyService = latencyService;
    }

    public void Start(ProfileId profileId, TriggerMode? triggerMode = null,
                      int? frameCount = null, int? intervalMs = null)
    {
        const int minIntervalMs = 50;
        if (intervalMs.HasValue && intervalMs.Value > 0 && intervalMs.Value < minIntervalMs)
            throw new InvalidParameterException($"intervalMs must be >= {minIntervalMs}ms, got {intervalMs.Value}ms.");

        lock (_lock)
        {
            if (_state == ChannelState.Active)
                throw new AcquisitionConflictException("Acquisition is already active. Stop it first.");

            // Release any idle channel from a previous run
            if (_channel != null)
            {
                _grabService.ReleaseChannel(_channel);
                _channel = null;
                _state = ChannelState.None;
            }

            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode)
                : camFile.ToChannelOptions();

            _channel = _grabService.CreateChannel(options);
            _activeProfileId = profileId;
            _lastFrame = null;
            _lastError = null;
            _statistics = new AcquisitionStatistics();

            _channel.FrameAcquired   += OnFrameAcquired;
            _channel.AcquisitionError += OnAcquisitionError;
            _channel.AcquisitionEnded += OnAcquisitionEnded;

            _statistics.Start();
            _channel.StartAcquisition(frameCount ?? -1);
            _state = ChannelState.Active;
        }

        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
            $"Started with profile '{profileId.Value}'" +
            (frameCount.HasValue ? $", frameCount={frameCount}" : "") +
            (intervalMs.HasValue ? $", intervalMs={intervalMs}" : "")));
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            if (_state != ChannelState.Active) return;
            _state = ChannelState.Idle;

            tcs = _triggerTcs;
            _triggerTcs = null;

            _statistics?.Stop();
            _channel!.StopAcquisition();
            _channel.FrameAcquired   -= OnFrameAcquired;
            _channel.AcquisitionError -= OnAcquisitionError;
            _channel.AcquisitionEnded -= OnAcquisitionEnded;
            _grabService.ReleaseChannel(_channel);
            _channel = null;
            _activeProfileId = null;
            _state = ChannelState.None;
        }

        tcs?.TrySetCanceled();
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStopped, "Stopped"));
    }

    public void SendTrigger()
    {
        lock (_lock)
        {
            if (_channel?.IsActive == true)
            {
                _triggerTimestamps.Enqueue(DateTimeOffset.UtcNow);
                _channel.SendSoftwareTrigger();
            }
        }
    }

    public async Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000)
    {
        TaskCompletionSource<ImageData> tcs;

        lock (_lock)
        {
            if (_channel == null || !_channel.IsActive)
                throw new ChannelNotAvailableException();
            if (_triggerTcs != null)
                throw new AcquisitionConflictException("A trigger is already pending. Wait for it to complete.");
            if (!_channel.SupportsSoftwareTrigger)
                throw new InvalidParameterException(
                    $"TriggerAndWaitAsync requires SOFT or COMBINED trigger mode, " +
                    $"but channel is configured for {_channel.TriggerMode}.");

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

    public ImageData? GetLatestFrame() { lock (_lock) return _lastFrame; }

    public AcquisitionStatisticsSnapshot? GetStatistics()
    {
        lock (_lock)
        {
            var snap = _statistics?.GetSnapshot();
            if (snap.HasValue && _channel != null)
                return snap.Value with
                {
                    CopyDropCount          = _channel.CopyDropCount,
                    ClusterUnavailableCount = _channel.ClusterUnavailableCount,
                };
            return snap;
        }
    }

    public IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50) => _eventLog.GetRecent(max);

    public IReadOnlySet<ChannelAction> GetAllowedActions()
    {
        lock (_lock)
        {
            return _state switch
            {
                ChannelState.None   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
                ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
                ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
                _                   => new HashSet<ChannelAction>(),
            };
        }
    }

    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        var frameAt = DateTimeOffset.UtcNow;
        TaskCompletionSource<ImageData>? tcs;
        long frameIndex;
        string? profileId;

        lock (_lock)
        {
            _lastFrame = e.Image;
            _statistics?.RecordFrame();
            frameIndex = _statistics?.FrameCount ?? 0;
            profileId  = _activeProfileId?.Value;
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        if (_triggerTimestamps.TryDequeue(out var triggerAt))
            _latencyService.Record(triggerAt, frameAt, frameIndex, profileId);

        _frameQueue.TryEnqueue(e.Image);
        FrameAcquired?.Invoke(e.Image);
        tcs?.TrySetResult(e.Image);
    }

    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            _lastError = e.Message;
            _statistics?.RecordError();
            if (e.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE) _statistics?.RecordDroppedFrame();
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        var eventType = e.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE
            ? ChannelEventType.BufferUnavailable
            : ChannelEventType.AcquisitionError;
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, eventType, e.Message));
        tcs?.TrySetException(new InvalidOperationException($"Acquisition error: {e.Message}"));

        if (e.Signal == McSignal.MC_SIG_UNRECOVERABLE_OVERRUN)
            Task.Run(Stop);
    }

    private void OnAcquisitionEnded(object? sender, EventArgs e) => Task.Run(Stop);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}
