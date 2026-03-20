namespace PeanutVision.Api.Services;

public interface IChannelCalibration
{
    bool IsCalibrationAvailable { get; }
    void PerformBlackCalibration();
    void PerformWhiteCalibration();
    void PerformWhiteBalanceOnce();
    void SetFlatFieldCorrection(bool enable);
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs, double? gainDb);
}
