using Microsoft.Extensions.Logging.Abstractions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Api.Tests.Unit;

/// <summary>
/// Regression tests: AcquisitionManager.GetStatus().ActiveConfig must reflect
/// the FULL config passed to Start(), including OutputDirectory and Format.
///
/// Bug: GetStatus() was reconstructing ActiveConfig with only ProfileId/FrameCount/IntervalMs,
/// so OutputDirectory/Format always fell back to defaults.
/// </summary>
public class AcquisitionConfigPersistenceTests : IDisposable
{
    private readonly AcquisitionManager _manager;

    public AcquisitionConfigPersistenceTests()
    {
        var mockHal = new MockMultiCamHAL();
        var grabService = new GrabService(mockHal);
        grabService.Initialize();
        _manager = new AcquisitionManager(grabService, TestCamFileHelper.GetOrCreate(), new NullLatencyService(), NullLogger<AcquisitionManager>.Instance);
    }

    public void Dispose() => _manager.Dispose();

    [Fact]
    public void GetStatus_ActiveConfig_reflects_OutputDirectory_from_Start()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            OutputDirectory: "/custom/path/captures");

        _manager.Start(config);

        var activeConfig = _manager.GetStatus().ActiveConfig;
        Assert.NotNull(activeConfig);
        Assert.Equal("/custom/path/captures", activeConfig!.OutputDirectory);
    }

    [Fact]
    public void GetStatus_ActiveConfig_reflects_Format_from_Start()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            Format: SaveImageFormat.Bmp);

        _manager.Start(config);

        var activeConfig = _manager.GetStatus().ActiveConfig;
        Assert.NotNull(activeConfig);
        Assert.Equal(SaveImageFormat.Bmp, activeConfig!.Format);
    }

    [Fact]
    public void GetStatus_ActiveConfig_reflects_all_fields_together()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            FrameCount: 5,
            IntervalMs: 200,
            OutputDirectory: "/data/peanut",
            Format: SaveImageFormat.Raw);

        _manager.Start(config);

        var activeConfig = _manager.GetStatus().ActiveConfig;
        Assert.NotNull(activeConfig);
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", activeConfig!.ProfileId.Value);
        Assert.Equal(5, activeConfig.FrameCount);
        Assert.Equal(200, activeConfig.IntervalMs);
        Assert.Equal("/data/peanut", activeConfig.OutputDirectory);
        Assert.Equal(SaveImageFormat.Raw, activeConfig.Format);
    }

    [Fact]
    public void GetStatus_ActiveConfig_is_null_after_stop()
    {
        _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
        _manager.Stop();

        Assert.Null(_manager.GetStatus().ActiveConfig);
    }

    [Fact]
    public void GetStatus_ActiveConfig_defaults_are_not_used_when_explicit_values_given()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            OutputDirectory: "MyCustomDirectory");

        _manager.Start(config);

        var activeConfig = _manager.GetStatus().ActiveConfig;
        Assert.NotEqual("CapturedImages", activeConfig!.OutputDirectory);
        Assert.Equal("MyCustomDirectory", activeConfig.OutputDirectory);
    }
}
