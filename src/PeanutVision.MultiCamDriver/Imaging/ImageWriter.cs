namespace PeanutVision.MultiCamDriver.Imaging;

/// <summary>
/// High-level API for saving images to files.
/// Uses the Strategy pattern via IImageEncoder implementations.
/// </summary>
/// <example>
/// // Using default registry (PNG, BMP, RAW)
/// var writer = new ImageWriter();
/// writer.Save(imageData, "output.png");
///
/// // With custom registry
/// var registry = new ImageEncoderRegistry()
///     .Register(new PngEncoder(PngCompressionLevel.BestCompression));
/// var writer = new ImageWriter(registry);
/// writer.Save(imageData, "output.png");
/// </example>
public sealed class ImageWriter
{
    private readonly ImageEncoderRegistry _registry;

    /// <summary>
    /// Creates an ImageWriter with the default encoder registry.
    /// </summary>
    public ImageWriter() : this(ImageEncoderRegistry.Default)
    {
    }

    /// <summary>
    /// Creates an ImageWriter with a custom encoder registry.
    /// </summary>
    public ImageWriter(ImageEncoderRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Saves image data to a file. Format is determined by file extension.
    /// </summary>
    public void Save(ImageData image, string filePath)
    {
        var encoder = _registry.GetEncoderForPath(filePath);
        encoder.Encode(image, filePath);
    }

    /// <summary>
    /// Saves image data to a stream using the specified encoder.
    /// </summary>
    public void Save(ImageData image, Stream stream, IImageEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(encoder);
        encoder.Encode(image, stream);
    }

    /// <summary>
    /// Saves image data to a stream. Format is determined by file extension hint.
    /// </summary>
    public void Save(ImageData image, Stream stream, string extensionHint)
    {
        var encoder = _registry.GetEncoder(extensionHint);
        encoder.Encode(image, stream);
    }

    /// <summary>
    /// Saves surface data to a file. Format is determined by file extension.
    /// </summary>
    public void Save(SurfaceData surface, string filePath)
    {
        var image = ImageData.FromSurface(surface);
        Save(image, filePath);
    }

    /// <summary>
    /// Saves raw pixel data to a file. Format is determined by file extension.
    /// </summary>
    public void Save(byte[] pixels, int width, int height, int pitch, string filePath)
    {
        var image = new ImageData(pixels, width, height, pitch);
        Save(image, filePath);
    }

    /// <summary>
    /// Gets the encoder registry used by this writer.
    /// </summary>
    public ImageEncoderRegistry Registry => _registry;

    /// <summary>
    /// Checks if the specified file extension is supported.
    /// </summary>
    public bool IsSupported(string extension) => _registry.IsSupported(extension);
}
