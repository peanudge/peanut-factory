namespace PeanutVision.Api.Services;

public sealed record AcquisitionConfig(
    ProfileId ProfileId,
    int? FrameCount = null,
    int? IntervalMs = null
);
