namespace PeanutVision.Capture;

public sealed record FrameSavedEvent(
    string FilePath,
    DateTime CapturedAt,
    int Width,
    int Height,
    long FileSizeBytes
);
