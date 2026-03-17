using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.MultiCamDriver.Tests.Camera;

public class CamFileServiceTests : IDisposable
{
    private readonly string _testDir;

    public CamFileServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"PeanutVision.CamFileServiceTest.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, recursive: true);
    }

    private void WriteTestCamFile(string fileName, string content = "")
    {
        File.WriteAllText(Path.Combine(_testDir, fileName), content);
    }

    [Fact]
    public void Constructor_ScansDirectory_FindsCamFiles()
    {
        WriteTestCamFile("camera1.cam");
        WriteTestCamFile("camera2.cam");

        var service = new CamFileService(_testDir);

        Assert.Equal(2, service.CamFiles.Count);
    }

    [Fact]
    public void Constructor_EmptyDirectory_ReturnsEmptyList()
    {
        var service = new CamFileService(_testDir);

        Assert.Empty(service.CamFiles);
    }

    [Fact]
    public void Constructor_CreatesDirectoryIfNotExists()
    {
        var newDir = Path.Combine(_testDir, "subdir");
        var service = new CamFileService(newDir);

        Assert.True(Directory.Exists(newDir));
        Assert.Empty(service.CamFiles);
    }

    [Fact]
    public void Directory_ReturnsFullPath()
    {
        var service = new CamFileService(_testDir);

        Assert.Equal(Path.GetFullPath(_testDir), service.Directory);
    }

    [Fact]
    public void GetByFileName_ReturnsCorrectInfo()
    {
        WriteTestCamFile("test-camera.cam", "Hactive_Px = 1920\nVactive_Ln = 1080\nSpectrum = COLOR");

        var service = new CamFileService(_testDir);
        var info = service.GetByFileName("test-camera.cam");

        Assert.Equal("test-camera.cam", info.FileName);
        Assert.Equal(1920, info.Width);
        Assert.Equal(1080, info.Height);
        Assert.Equal("COLOR", info.Spectrum);
    }

    [Fact]
    public void GetByFileName_CaseInsensitive()
    {
        WriteTestCamFile("Test-Camera.cam");

        var service = new CamFileService(_testDir);
        var info = service.GetByFileName("test-camera.cam");

        Assert.Equal("Test-Camera.cam", info.FileName);
    }

    [Fact]
    public void GetByFileName_NotFound_Throws()
    {
        var service = new CamFileService(_testDir);

        var ex = Assert.Throws<KeyNotFoundException>(() =>
            service.GetByFileName("nonexistent.cam"));

        Assert.Contains("nonexistent.cam", ex.Message);
    }

    [Fact]
    public void TryGetByFileName_Found_ReturnsTrue()
    {
        WriteTestCamFile("found.cam");

        var service = new CamFileService(_testDir);
        var found = service.TryGetByFileName("found.cam", out var info);

        Assert.True(found);
        Assert.NotNull(info);
        Assert.Equal("found.cam", info!.FileName);
    }

    [Fact]
    public void TryGetByFileName_NotFound_ReturnsFalse()
    {
        var service = new CamFileService(_testDir);
        var found = service.TryGetByFileName("missing.cam", out var info);

        Assert.False(found);
        Assert.Null(info);
    }

    [Fact]
    public void CamFiles_IgnoresNonCamFiles()
    {
        WriteTestCamFile("camera.cam");
        File.WriteAllText(Path.Combine(_testDir, "readme.txt"), "");
        File.WriteAllText(Path.Combine(_testDir, "config.json"), "");

        var service = new CamFileService(_testDir);

        Assert.Single(service.CamFiles);
    }

    [Fact]
    public void ToChannelOptions_UsesFilePath()
    {
        WriteTestCamFile("test.cam", "TrigMode = SOFT");

        var service = new CamFileService(_testDir);
        var info = service.GetByFileName("test.cam");
        var options = info.ToChannelOptions();

        Assert.Equal(info.FilePath, options.CamFilePath);
        Assert.Equal(McTrigMode.MC_TrigMode_SOFT, options.TriggerMode);
    }

    [Fact]
    public void ToChannelOptions_WithOverride_UsesOverriddenTriggerMode()
    {
        WriteTestCamFile("test.cam", "TrigMode = IMMEDIATE");

        var service = new CamFileService(_testDir);
        var info = service.GetByFileName("test.cam");
        var options = info.ToChannelOptions(McTrigMode.MC_TrigMode_HARD);

        Assert.Equal(McTrigMode.MC_TrigMode_HARD, options.TriggerMode);
    }

    [Fact]
    public void ToChannelOptions_DefaultParameters()
    {
        WriteTestCamFile("test.cam");

        var service = new CamFileService(_testDir);
        var info = service.GetByFileName("test.cam");
        var options = info.ToChannelOptions();

        Assert.Equal(0, options.DriverIndex);
        Assert.Equal("M", options.Connector);
        Assert.Equal(4, options.SurfaceCount);
        Assert.True(options.UseCallback);
    }
}
