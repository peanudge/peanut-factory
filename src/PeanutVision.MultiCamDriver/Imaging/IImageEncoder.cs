namespace PeanutVision.MultiCamDriver.Imaging;

/// <summary>
/// Strategy interface for encoding image data to a specific format.
/// Implement this interface to add support for new image formats.
/// </summary>
public interface IImageEncoder
{
    /// <summary>
    /// File extension this encoder handles (e.g., ".png", ".bmp").
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// Human-readable format name for display purposes.
    /// </summary>
    string FormatName { get; }

    /// <summary>
    /// Encodes image data and writes to the specified file path.
    /// </summary>
    void Encode(ImageData image, string filePath);

    /// <summary>
    /// Encodes image data and writes to a stream.
    /// </summary>
    void Encode(ImageData image, Stream stream);
}
