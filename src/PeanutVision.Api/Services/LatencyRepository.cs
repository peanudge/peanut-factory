namespace PeanutVision.Api.Services;

/// <summary>
/// In-memory circular buffer that stores the most recent trigger-to-frame latency measurements.
/// Thread-safe. Retains up to <see cref="Capacity"/> records; older entries are evicted automatically.
/// </summary>
public sealed class LatencyRepository
{
    private const int Capacity = 1000;

    private readonly object _lock = new();
    private readonly List<LatencyRecord> _records = new(Capacity + 1);
    private int _nextId = 1;

    public void Add(DateTimeOffset triggerSentAt, DateTimeOffset frameReceivedAt, long frameIndex, string? profileId)
    {
        var record = new LatencyRecord
        {
            Id = _nextId++,
            TriggerSentAt = triggerSentAt,
            FrameReceivedAt = frameReceivedAt,
            LatencyMs = Math.Round((frameReceivedAt - triggerSentAt).TotalMilliseconds, 3),
            FrameIndex = frameIndex,
            ProfileId = profileId,
        };

        lock (_lock)
        {
            _records.Add(record);
            if (_records.Count > Capacity)
                _records.RemoveAt(0);
        }
    }

    public IReadOnlyList<LatencyRecord> GetRecent(int max = 200)
    {
        lock (_lock)
        {
            var skip = Math.Max(0, _records.Count - max);
            return _records.Skip(skip).ToList();
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

    public LatencyStats? GetStats()
    {
        double[] latencies;

        lock (_lock)
        {
            if (_records.Count == 0) return null;
            latencies = _records.Select(r => r.LatencyMs).OrderBy(x => x).ToArray();
        }

        var count = latencies.Length;
        var mean = latencies.Average();
        var variance = latencies.Average(x => (x - mean) * (x - mean));

        return new LatencyStats
        {
            Count = count,
            MinMs = Math.Round(latencies[0], 3),
            MaxMs = Math.Round(latencies[count - 1], 3),
            MeanMs = Math.Round(mean, 3),
            P50Ms = Percentile(latencies, 50),
            P95Ms = Percentile(latencies, 95),
            P99Ms = Percentile(latencies, 99),
            StdDevMs = Math.Round(Math.Sqrt(variance), 3),
        };
    }

    private static double Percentile(double[] sorted, int p)
    {
        if (sorted.Length == 1) return sorted[0];
        double index = (p / 100.0) * (sorted.Length - 1);
        int lower = (int)Math.Floor(index);
        int upper = Math.Min(lower + 1, sorted.Length - 1);
        double fraction = index - lower;
        return Math.Round(sorted[lower] + fraction * (sorted[upper] - sorted[lower]), 3);
    }
}
