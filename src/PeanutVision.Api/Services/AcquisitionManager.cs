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

            _channel.FrameAcquired += OnFrameAcquired;
            _channel.AcquisitionError += OnAcquisitionError;

            _statistics.Start();
            _channel.StartAcquisition();
        }
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;

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
        }

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

    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        var image = ImageData.FromSurface(e.Surface);
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

    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            _lastError = e.Message;
            _statistics?.RecordError();
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        tcs?.TrySetException(new InvalidOperationException($"Acquisition error: {e.Message}"));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}
