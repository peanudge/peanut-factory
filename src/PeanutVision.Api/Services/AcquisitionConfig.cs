namespace PeanutVision.Api.Services;

public enum SaveImageFormat { Png, Bmp, Raw }

public sealed record AcquisitionConfig(
    ProfileId ProfileId,
    int? FrameCount = null,
    int? IntervalMs = null,
    string OutputDirectory = "CapturedImages",
    SaveImageFormat Format = SaveImageFormat.Png,
    bool AutoSave = true
);
