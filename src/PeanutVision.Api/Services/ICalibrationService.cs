namespace PeanutVision.Api.Services;

public interface ICalibrationService
{
    bool IsAvailable { get; }
    void PerformBlackCalibration();
    void PerformWhiteCalibration();
    void PerformWhiteBalanceOnce();
    void SetFlatFieldCorrection(bool enable);
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs, double? gainDb);
}

public class ExposureInfo
{
    public double ExposureUs { get; init; }
    public double GainDb { get; init; }
    public ExposureRangeInfo? ExposureRange { get; init; }
}

public class ExposureRangeInfo
{
    public double Min { get; init; }
    public double Max { get; init; }
}
