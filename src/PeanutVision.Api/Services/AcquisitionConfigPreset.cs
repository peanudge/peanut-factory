namespace PeanutVision.Api.Services;

/// <summary>
/// A named snapshot of AcquisitionConfig for reuse across acquisition sessions.
/// </summary>
public sealed record AcquisitionConfigPreset
{
    public string Name { get; init; } = string.Empty;
    public string ProfileId { get; init; } = string.Empty;
    public string? TriggerMode { get; init; }
    public int? FrameCount { get; init; }
    public int? IntervalMs { get; init; }
}
