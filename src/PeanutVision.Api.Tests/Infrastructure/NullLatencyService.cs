using PeanutVision.Api.Services;

namespace PeanutVision.Api.Tests.Infrastructure;

internal sealed class NullLatencyService : ILatencyService
{
    public void Record(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId) { }
    public IReadOnlyList<LatencyRecord> GetRecent(int max = 200) => Array.Empty<LatencyRecord>();
    public LatencyStats? GetStats() => null;
    public void Clear() { }
}
