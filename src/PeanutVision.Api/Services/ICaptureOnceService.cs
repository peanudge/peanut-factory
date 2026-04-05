using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface ICaptureOnceService
{
    ImageData CaptureOnce(ProfileId profileId, TriggerMode? triggerMode = null);
}
