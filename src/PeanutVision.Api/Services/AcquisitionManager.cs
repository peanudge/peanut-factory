using System.Collections.Concurrent;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionManager : IAcquisitionSession
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly ILatencyService _latencyService;
    private readonly object _lock = new();
    private readonly ChannelEventLog _eventLog = new();
    private readonly ConcurrentQueue<DateTimeOffset> _triggerTimestamps = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private bool _disposed;
    private Timer? _triggerTimer;
    private ChannelState _channelState = ChannelState.None;
    private AcquisitionConfig? _activeConfig;  // full config including OutputDirectory, Format, AutoSave

    public event EventHandler? FrameAcquired;
    public event EventHandler? StatusChanged;

    public AcquisitionManager(IGrabService grabService, ICamFileService camFileService, ILatencyService latencyService)
    {
        _grabService = grabService;
        _camFileService = camFileService;
        _latencyService = latencyService;
    }

    public ImageData? GetLatestFrame()
    {
        lock (_lock) return _lastFrame;
    }

    public AcquisitionStatus GetStatus()
    {
        lock (_lock)
        {
            var statsSnapshot = _statistics?.GetSnapshot();
            AcquisitionStatisticsSnapshot? stats = null;
            if (statsSnapshot.HasValue && _channel != null)
            {
                stats = statsSnapshot.Value with
                {
                    CopyDropCount = _channel.CopyDropCount,
                    ClusterUnavailableCount = _channel.ClusterUnavailableCount,
                };
            }
            else if (statsSnapshot.HasValue)
            {
                stats = statsSnapshot;
            }

            AcquisitionConfig? activeConfig = _channelState == ChannelState.Active
                ? _activeConfig
                : null;

            var allowedActions = _channelState switch
            {
                ChannelState.None   => (IReadOnlySet<ChannelAction>)new HashSet<ChannelAction> { ChannelAction.Start },
                ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start },
                ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
                _                   => new HashSet<ChannelAction>(),
            };

            return new AcquisitionStatus(
                ChannelState: _channelState,
                ActiveConfig: activeConfig,
                HasFrame: _lastFrame != null,
                LastError: _lastError,
                Statistics: stats,
                RecentEvents: _eventLog.GetRecent(50),
                AllowedActions: allowedActions
            );
        }
    }

    private void CreateChannel(ProfileId profileId)
    {
        lock (_lock)
        {
            if (_channelState != ChannelState.None)
                throw new InvalidOperationException("A channel already exists. Release it first.");

            var camFile = _camFileService.GetByFileName(profileId.Value);
            _channel = _grabService.CreateChannel(camFile.ToChannelOptions());
            _activeConfig = null; // will be set in Start()
            _channelState = ChannelState.Idle;
        }
    }

    public void Start(AcquisitionConfig config)
    {
        // Capture idle channel for release OUTSIDE lock to avoid potential deadlock with channel Dispose
        GrabChannel? toRelease = null;
        lock (_lock)
        {
            if (_channelState == ChannelState.Active)
                throw new InvalidOperationException("Acquisition is already running. Stop it first.");

            if (_channelState == ChannelState.Idle)
            {
                toRelease = _channel;
                _channel = null;
                
                
                _channelState = ChannelState.None;
                _lastFrame = null;
                _statistics = null;
            }
        }
        if (toRelease != null)
            _grabService.ReleaseChannel(toRelease);

        lock (_lock)
        {
            var camFile = _camFileService.GetByFileName(config.ProfileId.Value);
            _channel = _grabService.CreateChannel(camFile.ToChannelOptions());
            _activeConfig = config with
            {
                IntervalMs = config.IntervalMs is > 0 ? config.IntervalMs : null,
            };

            _lastFrame = null;
            _lastError = null;
            _statistics = new AcquisitionStatistics();

            _channel.FrameAcquired    += OnFrameAcquired;
            _channel.AcquisitionError += OnAcquisitionError;
            _channel.AcquisitionEnded += OnAcquisitionEnded;

            _channelState = ChannelState.Active;
            _statistics.Start();
            _channel.StartAcquisition(config.FrameCount ?? -1);

            if (config.IntervalMs is > 0)
            {
                var ms = config.IntervalMs.Value;
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
                }, null, 0, ms);
            }

            _eventLog.Add(new ChannelEvent(
                DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
                $"Acquisition started with profile '{config.ProfileId.Value}'" +
                (config.FrameCount.HasValue ? $", frameCount={config.FrameCount}" : "") +
                (config.IntervalMs.HasValue ? $", intervalMs={config.IntervalMs}" : "")));
        }

        StatusChanged?.Invoke(this, EventArgs.Empty);
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
            
            
            _channelState = ChannelState.None;
            _lastFrame = null;
            _statistics = null;
        }

        if (channel != null)
            _grabService.ReleaseChannel(channel);
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            if (_channelState != ChannelState.Active)
                return;

            _channelState = ChannelState.Idle;
            _activeConfig = null;

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
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<ImageData> TriggerAsync(int timeoutMs = 5000)
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
                    $"TriggerAsync requires SOFT or COMBINED trigger mode, but channel is configured for {_channel.TriggerMode}. " +
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
    /// Frame callback — receives already-copied ImageData from GrabChannel's copy thread.
    /// </summary>
    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        ProcessFrame(e.Image);

        int? target;
        long frameCount;
        lock (_lock)
        {
            target = _activeConfig?.FrameCount;
            frameCount = _statistics?.FrameCount ?? 0;
        }

        if (target.HasValue && frameCount >= target.Value)
            Task.Run(Stop);
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
        var frameReceivedAt = DateTimeOffset.UtcNow;

        TaskCompletionSource<ImageData>? tcs;
        long frameIndex;
        string? profileId;

        lock (_lock)
        {
            _lastFrame = image;
            _statistics?.RecordFrame();
            frameIndex = _statistics?.FrameCount ?? 0;
            profileId = _activeConfig?.ProfileId.Value;
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        if (_triggerTimestamps.TryDequeue(out var triggerSentAt))
            _latencyService.Record(triggerSentAt, frameReceivedAt, frameIndex, profileId);

        tcs?.TrySetResult(image);
        FrameAcquired?.Invoke(this, EventArgs.Empty);
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
            
            
        }
        if (channel != null)
            _grabService.ReleaseChannel(channel);
    }
}
