namespace PeanutVision.Api.Services;

public sealed record AcquisitionPreset
{
    public string Name { get; init; } = string.Empty;
    public string ProfileId { get; init; } = string.Empty;
    public string? TriggerMode { get; init; }
    public int? FrameCount { get; init; }
    public int? IntervalMs { get; init; }
}
