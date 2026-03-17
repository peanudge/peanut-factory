using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionManager : IAcquisitionService
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly object _lock = new();
    private readonly ChannelEventLog _eventLog = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private ProfileId? _activeProfileId;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private bool _disposed;

    // Test synchronization
    private TaskCompletionSource? _signalProcessedTcs;

    public AcquisitionManager(IGrabService grabService, ICamFileService camFileService)
    {
        _grabService = grabService;
        _camFileService = camFileService;
    }

    public bool IsActive
    {
        get { lock (_lock) return _channel?.IsActive == true; }
    }

    public ProfileId? ActiveProfileId
    {
        get { lock (_lock) return _activeProfileId; }
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

    public IReadOnlySet<string> GetAllowedActions()
    {
        lock (_lock)
        {
            var actions = new HashSet<string>();
            if (_channel == null || !_channel.IsActive)
            {
                actions.Add("start");
                actions.Add("snapshot");
            }
            else
            {
                actions.Add("stop");
                actions.Add("trigger");
            }
            return actions;
        }
    }

    internal GrabChannel? Channel
    {
        get { lock (_lock) return _channel; }
    }

    public void Start(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        lock (_lock)
        {
            if (_channel != null)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");

            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode)
                : camFile.ToChannelOptions();

            _channel = _grabService.CreateChannel(options);
            _activeProfileId = profileId;
            _lastFrame = null;
            _lastError = null;

            _statistics = new AcquisitionStatistics();

            _channel.FrameAcquired += OnFrameAcquired;
            _channel.AcquisitionError += OnAcquisitionError;

            _statistics.Start();
            _channel.StartAcquisition();

            _eventLog.Add(new ChannelEvent(
                DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
                $"Acquisition started with profile '{profileId.Value}'"));
        }
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;
        GrabChannel? channelToDispose;

        lock (_lock)
        {
            if (_channel == null)
                return;

            tcs = _triggerTcs;
            _triggerTcs = null;

            _statistics?.Stop();
            _channel.StopAcquisition();
            _channel.FrameAcquired -= OnFrameAcquired;
            _channel.AcquisitionError -= OnAcquisitionError;

            channelToDispose = _channel;
            _channel = null;
            _activeProfileId = null;
        }

        _eventLog.Add(new ChannelEvent(
            DateTime.UtcNow, ChannelEventType.AcquisitionStopped,
            "Acquisition stopped"));

        tcs?.TrySetCanceled();
        channelToDispose?.Dispose();
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
            if (_channel != null)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");
        }

        var camFile = _camFileService.GetByFileName(profileId.Value);
        var options = triggerMode.HasValue
            ? camFile.ToChannelOptions(triggerMode.Value.Mode, useCallback: false)
            : camFile.ToChannelOptions(useCallback: false);

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
            channel.Dispose();
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
    }

    /// <summary>
    /// Prepares a waiter for the next signal processing completion.
    /// Call this BEFORE the action that triggers the signal, then await the returned task.
    /// Internal for test use only.
    /// </summary>
    internal Task PrepareSignalWaiter()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_lock)
        {
            _signalProcessedTcs = tcs;
        }
        return tcs.Task;
    }

    private void ProcessFrame(ImageData image)
    {
        TaskCompletionSource<ImageData>? tcs;
        TaskCompletionSource? processedTcs;

        lock (_lock)
        {
            _lastFrame = image;
            _statistics?.RecordFrame();
            tcs = _triggerTcs;
            _triggerTcs = null;
            processedTcs = _signalProcessedTcs;
            _signalProcessedTcs = null;
        }

        tcs?.TrySetResult(image);
        processedTcs?.TrySetResult();
    }

    private void ProcessError(string message, McSignal signal)
    {
        TaskCompletionSource<ImageData>? tcs;
        TaskCompletionSource? processedTcs;

        lock (_lock)
        {
            _lastError = message;
            _statistics?.RecordError();

            if (signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE)
                _statistics?.RecordDroppedFrame();

            tcs = _triggerTcs;
            _triggerTcs = null;
            processedTcs = _signalProcessedTcs;
            _signalProcessedTcs = null;
        }

        var eventType = signal switch
        {
            McSignal.MC_SIG_CLUSTER_UNAVAILABLE => ChannelEventType.BufferUnavailable,
            _ => ChannelEventType.AcquisitionError,
        };
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, eventType, message));

        tcs?.TrySetException(new InvalidOperationException($"Acquisition error: {message}"));
        processedTcs?.TrySetResult();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}
