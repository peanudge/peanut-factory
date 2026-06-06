namespace PeanutVision.Api.Services;

public sealed record AcquisitionConfig(
    ProfileId ProfileId,
    TriggerMode? TriggerMode = null,
    int? FrameCount = null,
    int? IntervalMs = null
);
