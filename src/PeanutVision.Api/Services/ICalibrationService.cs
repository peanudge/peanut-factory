namespace PeanutVision.Api.Services;

public interface ICalibrationService
{
    bool IsAvailable { get; }
    void PerformBlackCalibration();
    void PerformWhiteCalibration();
    void PerformWhiteBalanceOnce();
    void SetFlatFieldCorrection(bool enable);
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs);
}
