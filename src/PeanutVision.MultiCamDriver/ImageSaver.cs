using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Static facade for saving images. Provides backward compatibility
/// while using the new OOP architecture internally.
/// For more control, use <see cref="ImageWriter"/> directly.
/// </summary>
public static class ImageSaver
{
    private static readonly ImageWriter Writer = new();

    /// <summary>
    /// Saves surface data to a file. Format is auto-detected from file extension.
    /// </summary>
    public static void Save(SurfaceData surface, string filePath)
        => Writer.Save(surface, filePath);

    /// <summary>
    /// Saves image data to a file. Format is auto-detected from file extension.
    /// </summary>
    public static void Save(byte[] data, int width, int height, int pitch, string filePath)
        => Writer.Save(data, width, height, pitch, filePath);

    /// <summary>
    /// Saves surface data as PNG.
    /// </summary>
    public static void SaveAsPng(SurfaceData surface, string filePath)
        => Save(surface.ToArray(), surface.Width, surface.Height, surface.Pitch,
            Path.ChangeExtension(filePath, ".png"));

    /// <summary>
    /// Saves image data as PNG.
    /// </summary>
    public static void SaveAsPng(byte[] data, int width, int height, int pitch, string filePath)
        => Writer.Save(new ImageData(data, width, height, pitch),
            Path.ChangeExtension(filePath, ".png"));

    /// <summary>
    /// Saves surface data as BMP.
    /// </summary>
    public static void SaveAsBmp(SurfaceData surface, string filePath)
        => Save(surface.ToArray(), surface.Width, surface.Height, surface.Pitch,
            Path.ChangeExtension(filePath, ".bmp"));

    /// <summary>
    /// Saves image data as BMP.
    /// </summary>
    public static void SaveAsBmp(byte[] data, int width, int height, int pitch, string filePath)
        => Writer.Save(new ImageData(data, width, height, pitch),
            Path.ChangeExtension(filePath, ".bmp"));

    /// <summary>
    /// Saves surface data as RAW binary.
    /// </summary>
    public static void SaveAsRaw(SurfaceData surface, string filePath)
        => SaveAsRaw(surface.ToArray(), filePath);

    /// <summary>
    /// Saves image data as RAW binary.
    /// </summary>
    public static void SaveAsRaw(byte[] data, string filePath)
        => Writer.Save(new ImageData(data, 1, data.Length, data.Length, PixelFormat.Grayscale8),
            Path.ChangeExtension(filePath, ".raw"));
}
