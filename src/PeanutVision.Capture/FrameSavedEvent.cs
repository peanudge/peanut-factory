namespace PeanutVision.Capture;

public sealed record FrameSavedEvent(
    string FilePath,
    DateTimeOffset CapturedAt,
    int Width,
    int Height,
    long FileSizeBytes
);
