namespace PeanutVision.Capture;

public sealed record FrameWriterOptions
{
    public string OutputDirectory { get; init; } = "CapturedImages";
    public string FilenamePrefix { get; init; } = "frame";
    public string TimestampFormat { get; init; } = "yyyyMMdd_HHmmss_fff";
    public OutputFormat Format { get; init; } = OutputFormat.Png;
}
