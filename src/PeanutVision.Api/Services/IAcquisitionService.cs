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

    /// <summary>Raised on the driver callback thread when a new frame is ready in <see cref="GetLatestFrame"/>.</summary>
    event EventHandler FrameAcquired;

    /// <summary>Raised after acquisition state changes (start, stop, error).</summary>
    event EventHandler StatusChanged;
}
