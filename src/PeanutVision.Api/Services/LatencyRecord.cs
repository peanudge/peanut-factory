namespace PeanutVision.Api.Services;

public sealed class LatencyRecord
{
    public int Id { get; init; }
    public DateTimeOffset TriggerSentAt { get; init; }
    public DateTimeOffset FrameReceivedAt { get; init; }
    public double LatencyMs { get; init; }
    public long FrameIndex { get; init; }
    public string? ProfileId { get; init; }
}

public sealed class LatencyStats
{
    public int Count { get; init; }
    public double MinMs { get; init; }
    public double MaxMs { get; init; }
    public double MeanMs { get; init; }
    public double P50Ms { get; init; }
    public double P95Ms { get; init; }
    public double P99Ms { get; init; }
    public double StdDevMs { get; init; }
}
