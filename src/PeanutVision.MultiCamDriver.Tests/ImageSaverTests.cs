using System.Runtime.InteropServices;

namespace PeanutVision.MultiCamDriver.Tests;

public class ImageSaverTests : IDisposable
{
    private readonly string _tempDir;
    private const int TestWidth = 100;
    private const int TestHeight = 80;
    private const int BytesPerPixel = 3; // RGB8
    private const int TestPitch = TestWidth * BytesPerPixel; // No padding

    public ImageSaverTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ImageSaverTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    #region Test Data Helpers

    /// <summary>
    /// Creates a test RGB8 image with a gradient pattern.
    /// </summary>
    private static byte[] CreateTestImageData(int width, int height, int pitch)
    {
        var data = new byte[height * pitch];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = y * pitch + x * BytesPerPixel;
                data[offset] = (byte)(x * 255 / width);      // R: horizontal gradient
                data[offset + 1] = (byte)(y * 255 / height); // G: vertical gradient
                data[offset + 2] = 128;                       // B: constant
            }
        }

        return data;
    }

    /// <summary>
    /// Creates a solid color test image.
    /// </summary>
    private static byte[] CreateSolidColorImage(int width, int height, int pitch, byte r, byte g, byte b)
    {
        var data = new byte[height * pitch];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = y * pitch + x * BytesPerPixel;
                data[offset] = r;
                data[offset + 1] = g;
                data[offset + 2] = b;
            }
        }

        return data;
    }

    /// <summary>
    /// Creates a SurfaceData struct from a managed byte array for testing.
    /// </summary>
    private static SurfaceData CreateTestSurface(byte[] data, int width, int height, int pitch)
    {
        // Pin the array and get a pointer
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        var ptr = handle.AddrOfPinnedObject();

        return new SurfaceData
        {
            Address = ptr,
            Width = width,
            Height = height,
            Pitch = pitch,
            Size = data.Length,
            SurfaceIndex = 0,
            FrameCount = 1,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    #endregion

    #region Save with Extension Detection

    [Fact]
    public void Save_WithPngExtension_CreatesPngFile()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "test.png");

        ImageSaver.Save(data, TestWidth, TestHeight, TestPitch, filePath);

        Assert.True(File.Exists(filePath));
        Assert.True(new FileInfo(filePath).Length > 0);
    }

    [Fact]
    public void Save_WithBmpExtension_CreatesBmpFile()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "test.bmp");

        ImageSaver.Save(data, TestWidth, TestHeight, TestPitch, filePath);

        Assert.True(File.Exists(filePath));
        Assert.True(new FileInfo(filePath).Length > 0);
    }

    [Fact]
    public void Save_WithRawExtension_CreatesRawFile()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "test.raw");

        ImageSaver.Save(data, TestWidth, TestHeight, TestPitch, filePath);

        Assert.True(File.Exists(filePath));
        Assert.Equal(data.Length, new FileInfo(filePath).Length);
    }

    [Fact]
    public void Save_WithUnsupportedExtension_ThrowsNotSupportedException()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "test.jpg");

        var ex = Assert.Throws<NotSupportedException>(() =>
            ImageSaver.Save(data, TestWidth, TestHeight, TestPitch, filePath));

        Assert.Contains(".jpg", ex.Message);
    }

    [Fact]
    public void Save_WithUpperCaseExtension_Works()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "test.PNG");

        ImageSaver.Save(data, TestWidth, TestHeight, TestPitch, filePath);

        Assert.True(File.Exists(filePath));
    }

    #endregion

    #region PNG Format Tests

    [Fact]
    public void SaveAsPng_CreatesValidPngFile()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "gradient.png");

        ImageSaver.SaveAsPng(data, TestWidth, TestHeight, TestPitch, filePath);

        Assert.True(File.Exists(filePath));

        // Verify PNG signature (first 8 bytes)
        var fileBytes = File.ReadAllBytes(filePath);
        Assert.True(fileBytes.Length >= 8);
        Assert.Equal(0x89, fileBytes[0]); // PNG signature
        Assert.Equal((byte)'P', fileBytes[1]);
        Assert.Equal((byte)'N', fileBytes[2]);
        Assert.Equal((byte)'G', fileBytes[3]);
    }

    [Fact]
    public void SaveAsPng_WithSurfaceData_CreatesFile()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

        try
        {
            var surface = new SurfaceData
            {
                Address = handle.AddrOfPinnedObject(),
                Width = TestWidth,
                Height = TestHeight,
                Pitch = TestPitch,
                Size = data.Length
            };

            var filePath = Path.Combine(_tempDir, "surface.png");
            ImageSaver.SaveAsPng(surface, filePath);

            Assert.True(File.Exists(filePath));
        }
        finally
        {
            handle.Free();
        }
    }

    #endregion

    #region BMP Format Tests

    [Fact]
    public void SaveAsBmp_CreatesValidBmpFile()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "gradient.bmp");

        ImageSaver.SaveAsBmp(data, TestWidth, TestHeight, TestPitch, filePath);

        Assert.True(File.Exists(filePath));

        // Verify BMP signature (first 2 bytes: "BM")
        var fileBytes = File.ReadAllBytes(filePath);
        Assert.True(fileBytes.Length >= 2);
        Assert.Equal((byte)'B', fileBytes[0]);
        Assert.Equal((byte)'M', fileBytes[1]);
    }

    [Fact]
    public void SaveAsBmp_LargerThanPng_DueToNoCompression()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var pngPath = Path.Combine(_tempDir, "compare.png");
        var bmpPath = Path.Combine(_tempDir, "compare.bmp");

        ImageSaver.SaveAsPng(data, TestWidth, TestHeight, TestPitch, pngPath);
        ImageSaver.SaveAsBmp(data, TestWidth, TestHeight, TestPitch, bmpPath);

        var pngSize = new FileInfo(pngPath).Length;
        var bmpSize = new FileInfo(bmpPath).Length;

        // BMP is uncompressed, so it should generally be larger
        Assert.True(bmpSize >= pngSize, $"BMP ({bmpSize}) should be >= PNG ({pngSize})");
    }

    #endregion

    #region RAW Format Tests

    [Fact]
    public void SaveAsRaw_PreservesExactData()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "exact.raw");

        ImageSaver.SaveAsRaw(data, filePath);

        var savedData = File.ReadAllBytes(filePath);
        Assert.Equal(data, savedData);
    }

    [Fact]
    public void SaveAsRaw_FileSizeMatchesDataSize()
    {
        var data = CreateTestImageData(TestWidth, TestHeight, TestPitch);
        var filePath = Path.Combine(_tempDir, "size.raw");

        ImageSaver.SaveAsRaw(data, filePath);

        Assert.Equal(data.Length, new FileInfo(filePath).Length);
    }

    [Fact]
    public void SaveAsRaw_WithSurfaceData_PreservesData()
    {
        var data = CreateSolidColorImage(TestWidth, TestHeight, TestPitch, 255, 128, 64);
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

        try
        {
            var surface = new SurfaceData
            {
                Address = handle.AddrOfPinnedObject(),
                Width = TestWidth,
                Height = TestHeight,
                Pitch = TestPitch,
                Size = data.Length
            };

            var filePath = Path.Combine(_tempDir, "surface.raw");
            ImageSaver.SaveAsRaw(surface, filePath);

            var savedData = File.ReadAllBytes(filePath);
            Assert.Equal(data, savedData);
        }
        finally
        {
            handle.Free();
        }
    }

    #endregion

    #region Pitch Handling Tests

    [Fact]
    public void SaveAsPng_WithPaddedPitch_HandlesCorrectly()
    {
        // Simulate pitch with padding (e.g., 4-byte aligned rows)
        int paddedPitch = ((TestWidth * BytesPerPixel + 3) / 4) * 4 + 4; // Add extra padding
        var data = CreateSolidColorImage(TestWidth, TestHeight, paddedPitch, 100, 150, 200);
        var filePath = Path.Combine(_tempDir, "padded.png");

        ImageSaver.SaveAsPng(data, TestWidth, TestHeight, paddedPitch, filePath);

        Assert.True(File.Exists(filePath));
        Assert.True(new FileInfo(filePath).Length > 0);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void SaveAsPng_SmallImage_1x1()
    {
        var data = new byte[] { 255, 0, 0 }; // Single red pixel
        var filePath = Path.Combine(_tempDir, "tiny.png");

        ImageSaver.SaveAsPng(data, 1, 1, 3, filePath);

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void SaveAsPng_LargeImage_Performance()
    {
        // 4K resolution test
        const int largeWidth = 3840;
        const int largeHeight = 2160;
        const int largePitch = largeWidth * BytesPerPixel;
        var data = new byte[largeHeight * largePitch];

        // Fill with simple pattern
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i % 256);
        }

        var filePath = Path.Combine(_tempDir, "large.png");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        ImageSaver.SaveAsPng(data, largeWidth, largeHeight, largePitch, filePath);
        sw.Stop();

        Assert.True(File.Exists(filePath));
        // Sanity check: should complete in reasonable time (< 10 seconds)
        Assert.True(sw.ElapsedMilliseconds < 10000, $"Took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void Save_OverwritesExistingFile()
    {
        var filePath = Path.Combine(_tempDir, "overwrite.png");

        // Create first file
        var data1 = CreateSolidColorImage(TestWidth, TestHeight, TestPitch, 255, 0, 0);
        ImageSaver.Save(data1, TestWidth, TestHeight, TestPitch, filePath);
        var size1 = new FileInfo(filePath).Length;

        // Overwrite with different data
        var data2 = CreateSolidColorImage(TestWidth * 2, TestHeight * 2, TestWidth * 2 * BytesPerPixel, 0, 255, 0);
        ImageSaver.Save(data2, TestWidth * 2, TestHeight * 2, TestWidth * 2 * BytesPerPixel, filePath);
        var size2 = new FileInfo(filePath).Length;

        // Second file should be different size (larger image)
        Assert.NotEqual(size1, size2);
    }

    #endregion
}
