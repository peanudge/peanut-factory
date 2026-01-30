using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace PeanutVision.MultiCamDriver.Imaging.Encoders;

/// <summary>
/// Encodes images to PNG format with lossless compression.
/// Recommended for ML datasets due to quality preservation and broad compatibility.
/// </summary>
public sealed class PngEncoder : ImageSharpEncoderBase
{
    private readonly PngCompressionLevel _compressionLevel;

    public override string Extension => ".png";
    public override string FormatName => "PNG (Portable Network Graphics)";

    public PngEncoder(PngCompressionLevel compressionLevel = PngCompressionLevel.DefaultCompression)
    {
        _compressionLevel = compressionLevel;
    }

    protected override void EncodeToStream(Image<Rgb24> image, Stream stream)
    {
        var encoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
        {
            CompressionLevel = _compressionLevel
        };
        image.Save(stream, encoder);
    }
}
