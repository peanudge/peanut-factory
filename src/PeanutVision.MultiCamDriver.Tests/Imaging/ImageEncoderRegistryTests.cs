using PeanutVision.MultiCamDriver.Imaging;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.MultiCamDriver.Tests.Imaging;

public class ImageEncoderRegistryTests
{
    [Fact]
    public void Default_ContainsPngBmpRaw()
    {
        var registry = ImageEncoderRegistry.Default;

        Assert.True(registry.IsSupported(".png"));
        Assert.True(registry.IsSupported(".bmp"));
        Assert.True(registry.IsSupported(".raw"));
    }

    [Fact]
    public void IsSupported_CaseInsensitive()
    {
        var registry = ImageEncoderRegistry.Default;

        Assert.True(registry.IsSupported(".PNG"));
        Assert.True(registry.IsSupported(".Png"));
        Assert.True(registry.IsSupported("png")); // without dot
    }

    [Fact]
    public void GetEncoder_ReturnsCorrectEncoder()
    {
        var registry = ImageEncoderRegistry.Default;

        Assert.IsType<PngEncoder>(registry.GetEncoder(".png"));
        Assert.IsType<BmpEncoder>(registry.GetEncoder(".bmp"));
        Assert.IsType<RawEncoder>(registry.GetEncoder(".raw"));
    }

    [Fact]
    public void GetEncoder_UnsupportedExtension_Throws()
    {
        var registry = ImageEncoderRegistry.Default;

        var ex = Assert.Throws<NotSupportedException>(() => registry.GetEncoder(".tiff"));
        Assert.Contains("tiff", ex.Message.ToLower());
    }

    [Fact]
    public void Register_AddsNewEncoder()
    {
        var registry = new ImageEncoderRegistry();
        var encoder = new PngEncoder();

        registry.Register(encoder);

        Assert.True(registry.IsSupported(".png"));
        Assert.Same(encoder, registry.GetEncoder(".png"));
    }

    [Fact]
    public void Register_OverwritesExisting()
    {
        var registry = new ImageEncoderRegistry();
        var encoder1 = new PngEncoder();
        var encoder2 = new PngEncoder();

        registry.Register(encoder1);
        registry.Register(encoder2);

        Assert.Same(encoder2, registry.GetEncoder(".png"));
    }

    [Fact]
    public void Unregister_RemovesEncoder()
    {
        var registry = new ImageEncoderRegistry();
        registry.Register(new PngEncoder());

        registry.Unregister(".png");

        Assert.False(registry.IsSupported(".png"));
    }

    [Fact]
    public void TryGetEncoder_ReturnsTrue_WhenFound()
    {
        var registry = ImageEncoderRegistry.Default;

        var result = registry.TryGetEncoder(".png", out var encoder);

        Assert.True(result);
        Assert.NotNull(encoder);
    }

    [Fact]
    public void TryGetEncoder_ReturnsFalse_WhenNotFound()
    {
        var registry = ImageEncoderRegistry.Default;

        var result = registry.TryGetEncoder(".xyz", out var encoder);

        Assert.False(result);
        Assert.Null(encoder);
    }

    [Fact]
    public void GetEncoderForPath_ExtractsExtension()
    {
        var registry = ImageEncoderRegistry.Default;

        var encoder = registry.GetEncoderForPath("/path/to/image.png");

        Assert.IsType<PngEncoder>(encoder);
    }

    [Fact]
    public void SupportedExtensions_ReturnsAllRegistered()
    {
        var registry = ImageEncoderRegistry.Default;

        var extensions = registry.SupportedExtensions;

        Assert.Contains(".png", extensions, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(".bmp", extensions, StringComparer.OrdinalIgnoreCase);
        Assert.Contains(".raw", extensions, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void Register_FluentApi_AllowsChaining()
    {
        var registry = new ImageEncoderRegistry()
            .Register(new PngEncoder())
            .Register(new BmpEncoder())
            .Register(new RawEncoder());

        Assert.Equal(3, registry.SupportedExtensions.Count);
    }
}
