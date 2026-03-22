namespace PeanutVision.Api.Services;

public class CalibrationManager : ICalibrationService
{
    private readonly IChannelCalibration _calibration;
    private readonly IExposureControl _exposure;

    public CalibrationManager(IChannelCalibration calibration, IExposureControl exposure)
    {
        _calibration = calibration;
        _exposure    = exposure;
    }

    public bool IsAvailable => _calibration.IsCalibrationAvailable;

    public void PerformBlackCalibration()           => _calibration.PerformBlackCalibration();
    public void PerformWhiteCalibration()           => _calibration.PerformWhiteCalibration();
    public void PerformWhiteBalanceOnce()           => _calibration.PerformWhiteBalanceOnce();
    public void SetFlatFieldCorrection(bool enable) => _calibration.SetFlatFieldCorrection(enable);

    public ExposureInfo GetExposure()                      => _exposure.GetExposure();
    public ExposureInfo SetExposure(double? exposureUs)    => _exposure.SetExposure(exposureUs);
}
