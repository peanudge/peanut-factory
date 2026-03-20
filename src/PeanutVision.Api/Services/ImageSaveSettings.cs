namespace PeanutVision.Api.Services;

public enum SaveImageFormat { Png, Bmp, Raw }

public enum SubfolderStrategy { None, ByDate, BySession, ByProfile }

public sealed record ImageSaveSettings
{
    public string OutputDirectory { get; init; } = "CapturedImages";
    public SaveImageFormat Format { get; init; } = SaveImageFormat.Png;
    public string FilenamePrefix { get; init; } = "capture";
    public string TimestampFormat { get; init; } = "yyyyMMdd_HHmmss_fff";
    public bool IncludeSequenceNumber { get; init; } = false;
    public SubfolderStrategy SubfolderStrategy { get; init; } = SubfolderStrategy.None;
    public bool AutoSave { get; init; } = true;
}
