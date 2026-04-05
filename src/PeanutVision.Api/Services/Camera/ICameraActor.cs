using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

public interface ICameraActor : IAsyncDisposable
{
    string CameraId { get; }

    Task StartAsync(
        ProfileId profileId,
        TriggerMode? triggerMode = null,
        int? frameCount = null,
        int? intervalMs = null,
        CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);

    Task<ImageData> TriggerAsync(int timeoutMs = 5000, CancellationToken ct = default);

    Task<LatestFrameResult> GetLatestFrameAsync(CancellationToken ct = default);

    Task<CameraActorStatus> GetStatusAsync(CancellationToken ct = default);

    Task<ExposureInfo> GetExposureAsync(CancellationToken ct = default);

    Task<ExposureInfo> SetExposureAsync(double? exposureUs, CancellationToken ct = default);
}
