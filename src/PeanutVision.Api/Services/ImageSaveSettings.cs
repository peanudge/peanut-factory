namespace PeanutVision.Api.Services;

public enum SaveImageFormat { Png, Bmp, Raw }

public sealed record ImageSaveSettings
{
    public string OutputDirectory { get; init; } = "CapturedImages";
    public SaveImageFormat Format { get; init; } = SaveImageFormat.Png;
    public bool AutoSave { get; init; } = true;
}
