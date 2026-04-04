using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

public sealed record LatestFrameResult(ImageData? Frame, bool IsNew);
