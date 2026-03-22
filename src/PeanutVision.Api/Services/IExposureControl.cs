namespace PeanutVision.Api.Services;

/// <summary>
/// Controls camera exposure time.
/// Separated from <see cref="IChannelCalibration"/> so consumers that only
/// need exposure adjustment don't depend on calibration operations.
/// </summary>
public interface IExposureControl
{
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs);
}
