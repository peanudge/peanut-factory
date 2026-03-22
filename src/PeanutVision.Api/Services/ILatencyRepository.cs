namespace PeanutVision.Api.Services;

/// <summary>
/// Stores raw trigger-to-frame latency measurements.
/// Responsible only for persistence — no statistical computation.
/// </summary>
public interface ILatencyRepository
{
    void Add(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId);
    IReadOnlyList<LatencyRecord> GetRecent(int max = 200);
    void Clear();
}
