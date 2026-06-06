using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>
/// Controls a single acquisition session. One session at a time.
/// Start(config) auto-releases any existing idle channel before starting.
/// </summary>
public interface IAcquisitionSession : IDisposable
{
    AcquisitionStatus GetStatus();
    void Start(AcquisitionConfig config);
    void Stop();
    void ReleaseChannel();
    Task<ImageData> TriggerAsync(int timeoutMs = 5000);
    ImageData? GetLatestFrame();

    event EventHandler FrameAcquired;
    event EventHandler StatusChanged;
}
