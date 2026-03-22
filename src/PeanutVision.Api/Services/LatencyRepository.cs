using Microsoft.Extensions.Options;

namespace PeanutVision.Api.Services;

/// <summary>
/// Thread-safe in-memory circular buffer for latency records.
/// Responsible only for storage — statistical computation lives in <see cref="LatencyAnalyzer"/>.
/// </summary>
public sealed class LatencyRepository : ILatencyRepository
{
    private readonly int _capacity;
    private readonly object _lock = new();
    private readonly Queue<LatencyRecord> _records;
    private int _nextId = 1;

    public LatencyRepository(IOptions<LatencyRepositoryOptions> options)
    {
        _capacity = options.Value.Capacity;
        _records = new Queue<LatencyRecord>(_capacity + 1);
    }

    public void Add(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId)
    {
        var record = new LatencyRecord
        {
            Id              = _nextId++,
            TriggerSentAt   = triggerSentAt,
            FrameReceivedAt = frameReceivedAt,
            LatencyMs       = Math.Round((frameReceivedAt - triggerSentAt).TotalMilliseconds, 3),
            FrameIndex      = frameIndex,
            ProfileId       = profileId,
        };

        lock (_lock)
        {
            _records.Enqueue(record);
            if (_records.Count > _capacity)
                _records.Dequeue();
        }
    }

    public IReadOnlyList<LatencyRecord> GetRecent(int max = 200)
    {
        lock (_lock)
        {
            return _records.TakeLast(max).ToList();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _records.Clear();
            _nextId = 1;
        }
    }
}
