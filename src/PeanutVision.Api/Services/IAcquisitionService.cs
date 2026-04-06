using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IAcquisitionService : IChannelService, IDisposable
{
    bool IsActive { get; }
    bool HasFrame { get; }
    string? LastError { get; }
    AcquisitionStatisticsSnapshot? GetStatistics();
    IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50);
    void Start(int? frameCount = null, int? intervalMs = null);
    void Stop();
    Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000);
    ImageData? GetLatestFrame();
}
