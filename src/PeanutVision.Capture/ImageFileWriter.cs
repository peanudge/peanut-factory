using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

/// <summary>Writes a frame to disk as an image file using the MultiCam ImageWriter.</summary>
public sealed class ImageFileWriter : IFrameWriter
{
    private readonly ImageWriter _writer;

    public ImageFileWriter(ImageWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public string Write(ImageData image, FrameWriterOptions options)
    {
        Directory.CreateDirectory(options.OutputDirectory);

        var extension = options.Format switch
        {
            OutputFormat.Bmp => ".bmp",
            OutputFormat.Raw => ".raw",
            _                => ".png",
        };

        var filename = $"{options.FilenamePrefix}_{DateTime.Now.ToString(options.TimestampFormat)}{extension}";
        var filePath = Path.Combine(options.OutputDirectory, filename);

        _writer.Save(image, filePath);
        return filePath;
    }
}
