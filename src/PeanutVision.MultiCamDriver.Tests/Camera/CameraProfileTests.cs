using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.MultiCamDriver.Tests.Camera;

public class CameraProfileTests : IDisposable
{
    private readonly string _testDir;

    public CameraProfileTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"PeanutVision.CameraProfileTests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
        // Create known cam files so GetCamFilePath succeeds
        File.WriteAllText(Path.Combine(_testDir, CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8), "");
        CamFileResource.SetDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, recursive: true);
    }

    [Fact]
    public void Builder_CreatesProfile_WithRequiredFields()
    {
        var profile = new CameraProfile.Builder()
            .WithId("test-camera")
            .WithCamFile("test.cam")
            .Build();

        Assert.Equal("test-camera", profile.Id);
        Assert.Equal("test.cam", profile.CamFileName);
    }

    [Fact]
    public void Builder_SetsAllProperties()
    {
        var profile = new CameraProfile.Builder()
            .WithId("my-camera")
            .WithDisplayName("My Camera")
            .WithManufacturer("Acme")
            .WithModel("CAM-100")
            .WithCamFile("cam100.cam")
            .WithConnector("A")
            .WithTriggerMode(McTrigMode.MC_TrigMode_SOFT)
            .WithSurfaceCount(8)
            .WithPixelFormat(PixelFormat.Rgba32)
            .WithResolution(1920, 1080)
            .WithDescription("Test camera")
            .Build();

        Assert.Equal("my-camera", profile.Id);
        Assert.Equal("My Camera", profile.DisplayName);
        Assert.Equal("Acme", profile.Manufacturer);
        Assert.Equal("CAM-100", profile.Model);
        Assert.Equal("cam100.cam", profile.CamFileName);
        Assert.Equal("A", profile.Connector);
        Assert.Equal(McTrigMode.MC_TrigMode_SOFT, profile.TriggerMode);
        Assert.Equal(8, profile.SurfaceCount);
        Assert.Equal(PixelFormat.Rgba32, profile.PixelFormat);
        Assert.Equal(1920, profile.ExpectedWidth);
        Assert.Equal(1080, profile.ExpectedHeight);
        Assert.Equal("Test camera", profile.Description);
    }

    [Fact]
    public void Builder_UsesDefaults_WhenNotSet()
    {
        var profile = new CameraProfile.Builder()
            .WithId("minimal")
            .WithCamFile("minimal.cam")
            .Build();

        Assert.Equal("M", profile.Connector);
        Assert.Equal(McTrigMode.MC_TrigMode_IMMEDIATE, profile.TriggerMode);
        Assert.Equal(4, profile.SurfaceCount);
        Assert.Equal(PixelFormat.Rgb24, profile.PixelFormat);
        Assert.Equal("Unknown", profile.Manufacturer);
    }

    [Fact]
    public void Builder_WithoutId_Throws()
    {
        var builder = new CameraProfile.Builder()
            .WithCamFile("test.cam");

        Assert.Throws<ArgumentNullException>(() => builder.Build());
    }

    [Fact]
    public void Builder_WithoutCamFile_Throws()
    {
        var builder = new CameraProfile.Builder()
            .WithId("test");

        Assert.Throws<ArgumentNullException>(() => builder.Build());
    }

    [Fact]
    public void ToChannelOptions_CreatesValidOptions()
    {
        var profile = new CameraProfile.Builder()
            .WithId("test")
            .WithCamFile(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8)
            .WithConnector("A")
            .WithTriggerMode(McTrigMode.MC_TrigMode_IMMEDIATE)
            .WithSurfaceCount(6)
            .Build();

        var options = profile.ToChannelOptions(driverIndex: 1, useCallback: false);

        Assert.Equal(1, options.DriverIndex);
        Assert.Equal("A", options.Connector);
        Assert.Equal(6, options.SurfaceCount);
        Assert.Equal(McTrigMode.MC_TrigMode_IMMEDIATE, options.TriggerMode);
        Assert.False(options.UseCallback);
    }

    [Fact]
    public void ToChannelOptions_WithCustomTrigger_OverridesDefault()
    {
        var profile = new CameraProfile.Builder()
            .WithId("test")
            .WithCamFile(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8)
            .WithTriggerMode(McTrigMode.MC_TrigMode_IMMEDIATE)
            .Build();

        var options = profile.ToChannelOptions(McTrigMode.MC_TrigMode_SOFT);

        Assert.Equal(McTrigMode.MC_TrigMode_SOFT, options.TriggerMode);
    }

    [Fact]
    public void ToString_ReturnsDisplayNameAndId()
    {
        var profile = new CameraProfile.Builder()
            .WithId("test-id")
            .WithDisplayName("Test Camera")
            .WithCamFile("test.cam")
            .Build();

        Assert.Equal("Test Camera (test-id)", profile.ToString());
    }
}
