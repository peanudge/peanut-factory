namespace PeanutVision.Api.Services;

/// <summary>
/// Pure, stateless computation of latency statistics from a snapshot of records.
/// Separated from storage so the statistical contract can evolve independently.
/// </summary>
public static class LatencyAnalyzer
{
    public static LatencyStats? Compute(IReadOnlyList<LatencyRecord> records)
    {
        if (records.Count == 0) return null;

        var latencies = records.Select(r => r.LatencyMs).OrderBy(x => x).ToArray();
        var count = latencies.Length;
        var mean = latencies.Average();
        var variance = latencies.Average(x => (x - mean) * (x - mean));

        return new LatencyStats
        {
            Count    = count,
            MinMs    = Math.Round(latencies[0], 3),
            MaxMs    = Math.Round(latencies[count - 1], 3),
            MeanMs   = Math.Round(mean, 3),
            P50Ms    = Percentile(latencies, 50),
            P95Ms    = Percentile(latencies, 95),
            P99Ms    = Percentile(latencies, 99),
            StdDevMs = Math.Round(Math.Sqrt(variance), 3),
        };
    }

    private static double Percentile(double[] sorted, int p)
    {
        if (sorted.Length == 1) return sorted[0];
        double index    = (p / 100.0) * (sorted.Length - 1);
        int    lower    = (int)Math.Floor(index);
        int    upper    = Math.Min(lower + 1, sorted.Length - 1);
        double fraction = index - lower;
        return Math.Round(sorted[lower] + fraction * (sorted[upper] - sorted[lower]), 3);
    }
}
