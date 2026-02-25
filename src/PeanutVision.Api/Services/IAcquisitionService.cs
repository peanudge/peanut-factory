using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IAcquisitionService : IDisposable
{
    bool IsActive { get; }
    ProfileId? ActiveProfileId { get; }
    string? LastError { get; }
    bool HasFrame { get; }
    AcquisitionStatisticsSnapshot? GetStatistics();
    void Start(ProfileId profileId, TriggerMode? triggerMode = null);
    void Stop();
    Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000);
    ImageData Snapshot(ProfileId profileId, TriggerMode? triggerMode = null);
}
