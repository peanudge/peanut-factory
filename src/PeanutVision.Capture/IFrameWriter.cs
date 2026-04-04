using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

/// <summary>Persists a captured frame to storage and returns the written path.</summary>
public interface IFrameWriter
{
    string Write(ImageData image, FrameWriterOptions options);
}
