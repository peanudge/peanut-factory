using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionManager : IDisposable
{
    private readonly IGrabService _grabService;
    private readonly object _lock = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private string? _activeProfileId;
    private bool _disposed;

    public AcquisitionManager(IGrabService grabService)
    {
        _grabService = grabService;
    }

    public bool IsActive
    {
        get { lock (_lock) return _channel?.IsActive == true; }
    }

    public string? ActiveProfileId
    {
        get { lock (_lock) return _activeProfileId; }
    }

    public ImageData? LastFrame
    {
        get { lock (_lock) return _lastFrame; }
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

    public GrabChannel? Channel
    {
        get { lock (_lock) return _channel; }
    }

    public void Start(string profileId, McTrigMode? triggerMode = null)
    {
        lock (_lock)
        {
            if (_channel != null)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");

            var profile = _grabService.CameraProfiles.GetProfile(profileId);
            var options = triggerMode.HasValue
                ? profile.ToChannelOptions(triggerMode.Value)
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
        lock (_lock)
        {
            if (_channel == null)
                return;

            _statistics?.Stop();
            _channel.StopAcquisition();
            _channel.FrameAcquired -= OnFrameAcquired;
            _channel.AcquisitionError -= OnAcquisitionError;
            _channel.Dispose();
            _channel = null;
            _activeProfileId = null;
        }
    }

    public void SendTrigger()
    {
        lock (_lock)
        {
            if (_channel == null || !_channel.IsActive)
                throw new InvalidOperationException("No active acquisition. Start acquisition first.");

            _channel.SendSoftwareTrigger();
        }
    }

    public ImageData? CaptureFrame()
    {
        lock (_lock)
        {
            return _lastFrame;
        }
    }

    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        var image = ImageData.FromSurface(e.Surface);

        lock (_lock)
        {
            _lastFrame = image;
            _statistics?.RecordFrame();
        }
    }

    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
    {
        lock (_lock)
        {
            _lastError = e.Message;
            _statistics?.RecordError();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}
