using PeanutVision.MultiCamDriver.Imaging;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.MultiCamDriver.Tests.Imaging;

public class ImageWriterTests : IDisposable
{
    private readonly string _tempDir;

    public ImageWriterTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImageWriterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    private static ImageData CreateTestImage(int width = 100, int height = 100)
    {
        var pitch = width * 3;
        var pixels = new byte[height * pitch];
        // Fill with gradient
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (byte)(i % 256);
        }
        return new ImageData(pixels, width, height, pitch);
    }

    [Fact]
    public void Constructor_WithDefaultRegistry_Works()
    {
        var writer = new ImageWriter();

        Assert.True(writer.IsSupported(".png"));
        Assert.True(writer.IsSupported(".bmp"));
        Assert.True(writer.IsSupported(".raw"));
    }

    [Fact]
    public void Constructor_WithCustomRegistry_UsesIt()
    {
        var registry = new ImageEncoderRegistry()
            .Register(new PngEncoder());

        var writer = new ImageWriter(registry);

        Assert.True(writer.IsSupported(".png"));
        Assert.False(writer.IsSupported(".bmp"));
    }

    [Fact]
    public void Save_ImageData_CreatesFile()
    {
        var writer = new ImageWriter();
        var image = CreateTestImage();
        var filePath = Path.Combine(_tempDir, "test.png");

        writer.Save(image, filePath);

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Save_ByteArray_CreatesFile()
    {
        var writer = new ImageWriter();
        var pixels = new byte[300];
        var filePath = Path.Combine(_tempDir, "test.png");

        writer.Save(pixels, 100, 1, 300, filePath);

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Save_ToStream_WritesData()
    {
        var writer = new ImageWriter();
        var image = CreateTestImage();
        using var stream = new MemoryStream();

        writer.Save(image, stream, ".png");

        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void Save_WithExplicitEncoder_UsesIt()
    {
        var writer = new ImageWriter();
        var image = CreateTestImage();
        using var stream = new MemoryStream();
        var encoder = new RawEncoder();

        writer.Save(image, stream, encoder);

        // RAW format = exact pixel data size
        Assert.Equal(image.Pixels.Length, stream.Length);
    }

    [Fact]
    public void Save_UnsupportedFormat_Throws()
    {
        var writer = new ImageWriter();
        var image = CreateTestImage();
        var filePath = Path.Combine(_tempDir, "test.xyz");

        Assert.Throws<NotSupportedException>(() => writer.Save(image, filePath));
    }

    [Fact]
    public void Save_CreatesDirectoryIfNeeded()
    {
        var writer = new ImageWriter();
        var image = CreateTestImage();
        var subDir = Path.Combine(_tempDir, "subdir", "nested");
        var filePath = Path.Combine(subDir, "test.png");

        writer.Save(image, filePath);

        Assert.True(File.Exists(filePath));
        Assert.True(Directory.Exists(subDir));
    }

    [Fact]
    public void Registry_Property_ReturnsRegistry()
    {
        var registry = new ImageEncoderRegistry();
        var writer = new ImageWriter(registry);

        Assert.Same(registry, writer.Registry);
    }
}
