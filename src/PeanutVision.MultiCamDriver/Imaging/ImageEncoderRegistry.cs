using System.Collections.Frozen;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.MultiCamDriver.Imaging;

/// <summary>
/// Registry for image encoders. Supports the Open/Closed Principle by allowing
/// new encoders to be registered without modifying existing code.
/// </summary>
public sealed class ImageEncoderRegistry
{
    private readonly Dictionary<string, IImageEncoder> _encoders = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the default registry instance with standard encoders (PNG, BMP, RAW).
    /// </summary>
    public static ImageEncoderRegistry Default { get; } = CreateDefault();

    /// <summary>
    /// Gets all registered encoders.
    /// </summary>
    public IReadOnlyCollection<IImageEncoder> Encoders => _encoders.Values;

    /// <summary>
    /// Gets all supported file extensions.
    /// </summary>
    public IReadOnlyCollection<string> SupportedExtensions => _encoders.Keys;

    /// <summary>
    /// Registers an encoder for its file extension.
    /// </summary>
    public ImageEncoderRegistry Register(IImageEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(encoder);

        var extension = NormalizeExtension(encoder.Extension);
        _encoders[extension] = encoder;
        return this;
    }

    /// <summary>
    /// Unregisters an encoder by extension.
    /// </summary>
    public ImageEncoderRegistry Unregister(string extension)
    {
        _encoders.Remove(NormalizeExtension(extension));
        return this;
    }

    /// <summary>
    /// Gets an encoder for the specified file extension.
    /// </summary>
    public IImageEncoder GetEncoder(string extension)
    {
        var normalized = NormalizeExtension(extension);

        if (_encoders.TryGetValue(normalized, out var encoder))
        {
            return encoder;
        }

        var supported = string.Join(", ", _encoders.Keys);
        throw new NotSupportedException(
            $"No encoder registered for extension '{extension}'. Supported: {supported}");
    }

    /// <summary>
    /// Gets an encoder for the specified file path based on its extension.
    /// </summary>
    public IImageEncoder GetEncoderForPath(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return GetEncoder(extension);
    }

    /// <summary>
    /// Tries to get an encoder for the specified extension.
    /// </summary>
    public bool TryGetEncoder(string extension, out IImageEncoder? encoder)
    {
        return _encoders.TryGetValue(NormalizeExtension(extension), out encoder);
    }

    /// <summary>
    /// Checks if an encoder is registered for the specified extension.
    /// </summary>
    public bool IsSupported(string extension)
    {
        return _encoders.ContainsKey(NormalizeExtension(extension));
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("Extension cannot be empty", nameof(extension));

        return extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";
    }

    private static ImageEncoderRegistry CreateDefault()
    {
        var registry = new ImageEncoderRegistry();
        registry.Register(new PngEncoder());
        registry.Register(new BmpEncoder());
        registry.Register(new RawEncoder());
        return registry;
    }
}
