using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IImageCaptureService
{
    Task<string> SaveAndRecordAsync(ImageData image, string? profileId);
}
