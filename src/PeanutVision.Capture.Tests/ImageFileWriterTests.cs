using PeanutVision.Capture;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture.Tests;

public class ImageFileWriterTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    private static ImageData MakeFrame(int w = 4, int h = 4)
        => new(new byte[w * h * 3], w, h, w * 3);

    [Fact]
    public void Write_CreatesFileAtReturnedPath()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, Format = OutputFormat.Png };

        var path = writer.Write(MakeFrame(), opts);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public void Write_ReturnsPathWithCorrectExtension_Png()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, Format = OutputFormat.Png };

        var path = writer.Write(MakeFrame(), opts);

        Assert.EndsWith(".png", path, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Write_ReturnsPathWithCorrectExtension_Bmp()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, Format = OutputFormat.Bmp };

        var path = writer.Write(MakeFrame(), opts);

        Assert.EndsWith(".bmp", path, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Write_CreatesOutputDirectoryIfMissing()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var nested = Path.Combine(_tempDir, "sub", "dir");
        var opts = new FrameWriterOptions { OutputDirectory = nested, Format = OutputFormat.Png };

        writer.Write(MakeFrame(), opts);

        Assert.True(Directory.Exists(nested));
    }

    [Fact]
    public void Write_FilenameContainsPrefix()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, FilenamePrefix = "myprefix", Format = OutputFormat.Png };

        var path = writer.Write(MakeFrame(), opts);

        Assert.Contains("myprefix", Path.GetFileName(path));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}
