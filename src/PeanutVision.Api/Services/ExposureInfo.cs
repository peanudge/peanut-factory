namespace PeanutVision.Api.Services;

public class ExposureInfo
{
    public double ExposureUs { get; init; }
    public ExposureRangeInfo? ExposureRange { get; init; }
}

public class ExposureRangeInfo
{
    public double Min { get; init; }
    public double Max { get; init; }
}
