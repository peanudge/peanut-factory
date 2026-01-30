namespace PeanutVision.MultiCamDriver.Imaging;

/// <summary>
/// Immutable value object representing image data with its dimensions and format.
/// </summary>
public sealed class ImageData
{
    public byte[] Pixels { get; }
    public int Width { get; }
    public int Height { get; }
    public int Pitch { get; }
    public PixelFormat Format { get; }

    /// <summary>
    /// Creates ImageData with RGB24 pixel format (default).
    /// </summary>
    public ImageData(byte[] pixels, int width, int height, int pitch)
        : this(pixels, width, height, pitch, PixelFormat.Rgb24)
    {
    }

    /// <summary>
    /// Creates ImageData with specified pixel format.
    /// </summary>
    public ImageData(byte[] pixels, int width, int height, int pitch, PixelFormat format)
    {
        ArgumentNullException.ThrowIfNull(pixels);
        ArgumentNullException.ThrowIfNull(format);
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive");
        if (pitch < width * format.BytesPerPixel)
            throw new ArgumentOutOfRangeException(nameof(pitch), "Pitch must be at least width * bytes per pixel");

        Pixels = pixels;
        Width = width;
        Height = height;
        Pitch = pitch;
        Format = format;
    }

    /// <summary>
    /// Creates ImageData from a SurfaceData struct.
    /// </summary>
    public static ImageData FromSurface(SurfaceData surface, PixelFormat? format = null)
    {
        return new ImageData(
            surface.ToArray(),
            surface.Width,
            surface.Height,
            surface.Pitch,
            format ?? PixelFormat.Rgb24
        );
    }
}

/// <summary>
/// Supported pixel formats for image data.
/// </summary>
public sealed class PixelFormat
{
    public string Name { get; }
    public int BytesPerPixel { get; }

    private PixelFormat(string name, int bytesPerPixel)
    {
        Name = name;
        BytesPerPixel = bytesPerPixel;
    }

    public static readonly PixelFormat Rgb24 = new("RGB24", 3);
    public static readonly PixelFormat Rgba32 = new("RGBA32", 4);
    public static readonly PixelFormat Grayscale8 = new("Gray8", 1);

    public override string ToString() => Name;
}
