namespace PeanutVision.Api.Services;

public class CalibrationManager : ICalibrationService
{
    private readonly IChannelCalibration _calibration;

    public CalibrationManager(IChannelCalibration calibration)
    {
        _calibration = calibration;
    }

    public bool IsAvailable => _calibration.IsCalibrationAvailable;

    public void PerformBlackCalibration() => _calibration.PerformBlackCalibration();

    public void PerformWhiteCalibration() => _calibration.PerformWhiteCalibration();

    public void PerformWhiteBalanceOnce() => _calibration.PerformWhiteBalanceOnce();

    public void SetFlatFieldCorrection(bool enable) => _calibration.SetFlatFieldCorrection(enable);

    public ExposureInfo GetExposure() => _calibration.GetExposure();

    public ExposureInfo SetExposure(double? exposureUs, double? gainDb) =>
        _calibration.SetExposure(exposureUs, gainDb);
}
