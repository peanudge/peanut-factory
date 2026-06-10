namespace PeanutVision.MultiCamDriver.Imaging;

public interface IImageWriter
{
    void Save(ImageData image, string filePath);
}
