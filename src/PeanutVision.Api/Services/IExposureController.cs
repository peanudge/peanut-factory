namespace PeanutVision.Api.Services;

public interface IExposureController
{
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs);
}
