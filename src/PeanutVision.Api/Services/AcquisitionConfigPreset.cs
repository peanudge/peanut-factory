namespace PeanutVision.Api.Services;

/// <summary>
/// A named snapshot of AcquisitionConfig for reuse across acquisition sessions.
/// Includes both acquisition parameters and image save settings.
/// </summary>
public sealed record AcquisitionConfigPreset
{
    public string Name { get; init; } = string.Empty;
    public string ProfileId { get; init; } = string.Empty;
    public int? FrameCount { get; init; }
    public int? IntervalMs { get; init; }
    public string OutputDirectory { get; init; } = "CapturedImages";
    public SaveImageFormat Format { get; init; } = SaveImageFormat.Png;
    public bool AutoSave { get; init; } = true;
}
