namespace PeanutVision.Api.Services;

public sealed record ImageSaveSettings
{
    public string OutputDirectory { get; init; } = "CapturedImages";
}
