using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;

namespace PeanutVision.MultiCamDriver.Imaging.Encoders;

/// <summary>
/// Encodes images to BMP format (uncompressed bitmap).
/// Fast encoding but produces larger files than PNG.
/// </summary>
public sealed class BmpEncoder : ImageSharpEncoderBase
{
    private readonly BmpBitsPerPixel _bitsPerPixel;

    public override string Extension => ".bmp";
    public override string FormatName => "BMP (Bitmap)";

    public BmpEncoder(BmpBitsPerPixel bitsPerPixel = BmpBitsPerPixel.Pixel24)
    {
        _bitsPerPixel = bitsPerPixel;
    }

    protected override void EncodeToStream(Image<Rgb24> image, Stream stream)
    {
        var encoder = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder
        {
            BitsPerPixel = _bitsPerPixel
        };
        image.Save(stream, encoder);
    }
}
