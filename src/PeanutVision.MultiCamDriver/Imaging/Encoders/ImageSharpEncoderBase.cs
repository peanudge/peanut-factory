using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PeanutVision.MultiCamDriver.Imaging.Encoders;

/// <summary>
/// Base class for encoders that use ImageSharp for encoding.
/// Provides common image creation logic from raw pixel data.
/// </summary>
public abstract class ImageSharpEncoderBase : IImageEncoder
{
    public abstract string Extension { get; }
    public abstract string FormatName { get; }

    public void Encode(ImageData image, string filePath)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = File.Create(filePath);
        Encode(image, fileStream);
    }

    public void Encode(ImageData image, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(stream);

        using var sharpImage = CreateSharpImage(image);
        EncodeToStream(sharpImage, stream);
    }

    /// <summary>
    /// Encodes the ImageSharp image to the output stream.
    /// Override in derived classes to use specific format encoding.
    /// </summary>
    protected abstract void EncodeToStream(Image<Rgb24> image, Stream stream);

    /// <summary>
    /// Creates an ImageSharp Image from raw pixel data, handling pitch correctly.
    /// </summary>
    private static Image<Rgb24> CreateSharpImage(ImageData image)
    {
        if (image.Format != PixelFormat.Rgb24)
        {
            throw new NotSupportedException($"Pixel format '{image.Format}' is not yet supported. Only RGB24 is currently implemented.");
        }

        var sharpImage = new Image<Rgb24>(image.Width, image.Height);
        var bytesPerPixel = image.Format.BytesPerPixel;

        sharpImage.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                var rowSpan = accessor.GetRowSpan(y);
                var srcOffset = y * image.Pitch;

                for (int x = 0; x < image.Width; x++)
                {
                    int i = srcOffset + x * bytesPerPixel;
                    if (i + 2 < image.Pixels.Length)
                    {
                        // MultiCam RGB24 stores pixels in BGR order (B-G-R)
                        rowSpan[x] = new Rgb24(
                            image.Pixels[i + 2],  // R
                            image.Pixels[i + 1],  // G
                            image.Pixels[i]        // B
                        );
                    }
                }
            }
        });

        return sharpImage;
    }
}
