namespace PeanutVision.Api.Services;

/// <summary>
/// Flat-field calibration and white balance operations.
/// Exposure control is intentionally excluded — see <see cref="IExposureControl"/>.
/// </summary>
public interface IChannelCalibration
{
    bool IsCalibrationAvailable { get; }
    void PerformBlackCalibration();
    void PerformWhiteCalibration();
    void PerformWhiteBalanceOnce();
    void SetFlatFieldCorrection(bool enable);
}
