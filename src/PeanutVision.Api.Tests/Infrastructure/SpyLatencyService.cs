using PeanutVision.Api.Services;

namespace PeanutVision.Api.Tests.Infrastructure;

/// <summary>
/// Latency service that captures all Record() calls for test verification.
/// </summary>
public sealed class SpyLatencyService : ILatencyService
{
    private readonly List<LatencyRecord> _records = [];
    private readonly object _lock = new();
    private int _nextId = 1;

    public IReadOnlyList<LatencyRecord> Recorded
    {
        get { lock (_lock) return _records.ToList(); }
    }

    public void Record(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId)
    {
        lock (_lock)
        {
            _records.Add(new LatencyRecord
            {
                Id             = _nextId++,
                TriggerSentAt  = triggerSentAt,
                FrameReceivedAt = frameReceivedAt,
                LatencyMs      = Math.Round((frameReceivedAt - triggerSentAt).TotalMilliseconds, 3),
                FrameIndex     = frameIndex,
                ProfileId      = profileId,
            });
        }
    }

    public IReadOnlyList<LatencyRecord> GetRecent(int max = 200) => Recorded;
    public LatencyStats? GetStats() => null;
    public void Clear() { lock (_lock) _records.Clear(); }
}
