namespace PeanutVision.MultiCamDriver.Imaging.Encoders;

/// <summary>
/// Encodes images as raw binary data without any header or compression.
/// Fastest encoding, but requires external knowledge of dimensions and format to read.
/// </summary>
public sealed class RawEncoder : IImageEncoder
{
    public string Extension => ".raw";
    public string FormatName => "RAW (Binary)";

    public void Encode(ImageData image, string filePath)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllBytes(filePath, image.Pixels);
    }

    public void Encode(ImageData image, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(stream);

        stream.Write(image.Pixels, 0, image.Pixels.Length);
    }
}
