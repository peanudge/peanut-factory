namespace PeanutVision.Api.Services;

/// <summary>
/// Application-level latency service: records measurements and exposes
/// both raw records and aggregate statistics to consumers.
/// </summary>
public interface ILatencyService
{
    void Record(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId);
    IReadOnlyList<LatencyRecord> GetRecent(int max = 200);
    LatencyStats? GetStats();
    void Clear();
}
