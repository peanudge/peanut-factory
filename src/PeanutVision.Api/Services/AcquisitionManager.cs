using System.Threading.Channels;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionManager : IAcquisitionService
{
    private readonly IGrabService _grabService;
    private readonly object _lock = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private ProfileId? _activeProfileId;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private bool _disposed;

    // Queue-based signal processing
    private Channel<AcquisitionSignal>? _signalChannel;
    private Task? _processingTask;
    private CancellationTokenSource? _processingCts;
    private TaskCompletionSource? _signalProcessedTcs;

    public AcquisitionManager(IGrabService grabService)
    {
        _grabService = grabService;
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

    public string? LastError
    {
        get { lock (_lock) return _lastError; }
    }

    public AcquisitionStatisticsSnapshot? GetStatistics()
    {
        lock (_lock)
        {
            return _statistics?.GetSnapshot();
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

            var profile = _grabService.CameraProfiles.GetProfile(profileId.Value);
            var options = triggerMode.HasValue
                ? profile.ToChannelOptions(triggerMode.Value.Mode)
                : profile.ToChannelOptions();

            _channel = _grabService.CreateChannel(options);
            _activeProfileId = profileId;
            _lastFrame = null;
            _lastError = null;

            _statistics = new AcquisitionStatistics();

            // Set up queue-based signal processing
            _signalChannel = System.Threading.Channels.Channel.CreateUnbounded<AcquisitionSignal>(
                new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
            _processingCts = new CancellationTokenSource();
            _processingTask = Task.Run(() => ProcessSignalsAsync(_processingCts.Token));

            _channel.FrameAcquired += OnFrameAcquired;
            _channel.AcquisitionError += OnAcquisitionError;

            _statistics.Start();
            _channel.StartAcquisition();
        }
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;
        Task? processingTask;

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
            _channel.Dispose();
            _channel = null;
            _activeProfileId = null;

            // Signal the processing task to stop
            _signalChannel?.Writer.TryComplete();
            _processingCts?.Cancel();
            processingTask = _processingTask;
            _processingTask = null;
        }

        tcs?.TrySetCanceled();

        // Wait for processing task to finish outside the lock
        if (processingTask != null)
        {
            try { processingTask.GetAwaiter().GetResult(); }
            catch (OperationCanceledException) { }
        }

        _processingCts?.Dispose();
        _processingCts = null;
        _signalChannel = null;
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

        var profile = _grabService.CameraProfiles.GetProfile(profileId.Value);
        var options = triggerMode.HasValue
            ? profile.ToChannelOptions(triggerMode.Value.Mode, useCallback: false)
            : profile.ToChannelOptions(useCallback: false);

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
    /// Callback handler - copies data and enqueues, no lock contention.
    /// Called from MultiCam native thread, must be fast.
    /// </summary>
    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        // Copy image data from surface (must happen before surface is released)
        var image = ImageData.FromSurface(e.Surface);

        // Fire-and-forget enqueue - if channel is closed, just drop
        _signalChannel?.Writer.TryWrite(new AcquisitionSignal.FrameReady(image));
    }

    /// <summary>
    /// Error callback handler - enqueues error signal, no lock contention.
    /// </summary>
    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
    {
        _signalChannel?.Writer.TryWrite(new AcquisitionSignal.Error(e.Message, e.Signal));
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

    /// <summary>
    /// Background consumer that processes queued signals sequentially.
    /// All state mutations happen here - single reader, no contention.
    /// </summary>
    private async Task ProcessSignalsAsync(CancellationToken ct)
    {
        if (_signalChannel == null) return;

        try
        {
            await foreach (var signal in _signalChannel.Reader.ReadAllAsync(ct))
            {
                switch (signal)
                {
                    case AcquisitionSignal.FrameReady frame:
                        ProcessFrame(frame.Image);
                        break;

                    case AcquisitionSignal.Error error:
                        ProcessError(error.Message, error.Signal);
                        break;
                }

                // Notify waiters that a signal was processed
                TaskCompletionSource? processedTcs;
                lock (_lock)
                {
                    processedTcs = _signalProcessedTcs;
                    _signalProcessedTcs = null;
                }
                processedTcs?.TrySetResult();
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
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

        tcs?.TrySetException(new InvalidOperationException($"Acquisition error: {message}"));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}

/// <summary>
/// Discriminated union for signals passed through the acquisition queue.
/// </summary>
internal abstract record AcquisitionSignal
{
    public sealed record FrameReady(ImageData Image) : AcquisitionSignal;
    public sealed record Error(string Message, McSignal Signal) : AcquisitionSignal;
}
