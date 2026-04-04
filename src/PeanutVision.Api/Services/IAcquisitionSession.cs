using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IAcquisitionSession : IDisposable
{
    bool IsActive { get; }
    bool HasFrame { get; }
    string? LastError { get; }
    ChannelState ChannelState { get; }
    ProfileId? ActiveProfileId { get; }
    IReadOnlySet<ChannelAction> GetAllowedActions();

    /// <summary>Creates a channel for the given profile and starts acquisition.</summary>
    void Start(ProfileId profileId, TriggerMode? triggerMode = null,
               int? frameCount = null, int? intervalMs = null);

    /// <summary>Stops acquisition and releases the channel.</summary>
    void Stop();

    /// <summary>Sends a software trigger. Used by CaptureScheduler.</summary>
    void SendTrigger();

    Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000);
    ImageData? GetLatestFrame();
    AcquisitionStatisticsSnapshot? GetStatistics();
    IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50);

    event Action<ImageData>? FrameAcquired;
}
