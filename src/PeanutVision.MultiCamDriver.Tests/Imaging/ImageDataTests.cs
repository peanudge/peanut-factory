using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.MultiCamDriver.Tests.Imaging;

public class ImageDataTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var pixels = new byte[300];
        var image = new ImageData(pixels, 100, 1, 300);

        Assert.Same(pixels, image.Pixels);
        Assert.Equal(100, image.Width);
        Assert.Equal(1, image.Height);
        Assert.Equal(300, image.Pitch);
        Assert.Equal(PixelFormat.Rgb24, image.Format);
    }

    [Fact]
    public void Constructor_WithFormat_SetsFormat()
    {
        var pixels = new byte[100];
        var image = new ImageData(pixels, 100, 1, 100, PixelFormat.Grayscale8);

        Assert.Equal(PixelFormat.Grayscale8, image.Format);
    }

    [Fact]
    public void Constructor_NullPixels_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ImageData(null!, 100, 100, 300));
    }

    [Fact]
    public void Constructor_ZeroWidth_Throws()
    {
        var pixels = new byte[300];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ImageData(pixels, 0, 100, 300));
    }

    [Fact]
    public void Constructor_NegativeWidth_Throws()
    {
        var pixels = new byte[300];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ImageData(pixels, -1, 100, 300));
    }

    [Fact]
    public void Constructor_ZeroHeight_Throws()
    {
        var pixels = new byte[300];
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ImageData(pixels, 100, 0, 300));
    }

    [Fact]
    public void Constructor_PitchTooSmall_Throws()
    {
        var pixels = new byte[100];
        // Width 100 * 3 bytes = 300, but pitch is only 100
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ImageData(pixels, 100, 1, 100));
    }

    [Fact]
    public void Constructor_PitchValidForGrayscale()
    {
        var pixels = new byte[100];
        // Width 100 * 1 byte = 100, pitch is exactly 100
        var image = new ImageData(pixels, 100, 1, 100, PixelFormat.Grayscale8);

        Assert.Equal(100, image.Pitch);
    }

    [Fact]
    public void PixelFormat_Rgb24_Has3BytesPerPixel()
    {
        Assert.Equal(3, PixelFormat.Rgb24.BytesPerPixel);
        Assert.Equal("RGB24", PixelFormat.Rgb24.Name);
    }

    [Fact]
    public void PixelFormat_Rgba32_Has4BytesPerPixel()
    {
        Assert.Equal(4, PixelFormat.Rgba32.BytesPerPixel);
        Assert.Equal("RGBA32", PixelFormat.Rgba32.Name);
    }

    [Fact]
    public void PixelFormat_Grayscale8_Has1BytePerPixel()
    {
        Assert.Equal(1, PixelFormat.Grayscale8.BytesPerPixel);
        Assert.Equal("Gray8", PixelFormat.Grayscale8.Name);
    }

    [Fact]
    public void PixelFormat_ToString_ReturnsName()
    {
        Assert.Equal("RGB24", PixelFormat.Rgb24.ToString());
    }
}
