using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;
using System.Runtime.InteropServices;

namespace PeanutVision.Api.Tests.Unit;

public class CameraActorTests : IAsyncDisposable
{
    private readonly MockMultiCamHAL _hal;
    private readonly GrabService _grabService;
    private readonly CameraActor _actor;
    private static readonly ProfileId FreerunProfile = new("crevis-tc-a160k-freerun-rgb8.cam");
    private static readonly ProfileId SoftTrigProfile = new("crevis-tc-a160k-softtrig-rgb8.cam");

    public CameraActorTests()
    {
        _hal = new MockMultiCamHAL();

        // Allocate real native memory for image buffer
        var bufferSize = _hal.Configuration.DefaultImageWidth
            * _hal.Configuration.DefaultImageHeight * 3;
        var mem = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, mem, bufferSize);
        _hal.Configuration.SimulatedSurfaceAddress = mem;
        _hal.Configuration.AutoSimulateFrameOnTrigger = true;

        _grabService = new GrabService(_hal);
        _grabService.Initialize();

        // Minimal no-op scope factory (stream save not tested here)
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        // No-op save settings (AutoSave=false for unit tests)
        var saveSettings = new ImageSaveSettingsService(
            Path.Combine(Path.GetTempPath(), $"actor-test-settings-{Guid.NewGuid():N}.json"));

        _actor = new CameraActor(
            cameraId:        "cam-1",
            grabService:     _grabService,
            camFileService:  TestCamFileHelper.GetOrCreate(),
            latencyService:  new LatencyService(new LatencyRepository(
                                 Options.Create(new LatencyRepositoryOptions()))),
            scopeFactory:    scopeFactory,
            frameWriter:     new ImageFileWriter(new ImageWriter()),
            saveSettings:    saveSettings,
            contentRootPath: Path.GetTempPath(),
            logger:          NullLogger<CameraActor>.Instance);
    }

    [Fact]
    public async Task Start_sets_state_to_active()
    {
        await _actor.StartAsync(FreerunProfile);

        var status = await _actor.GetStatusAsync();
        Assert.True(status.IsActive);
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", status.ActiveProfileId);

        await _actor.StopAsync();
    }

    [Fact]
    public async Task Start_twice_throws_conflict()
    {
        await _actor.StartAsync(FreerunProfile);

        await Assert.ThrowsAsync<Exceptions.AcquisitionConflictException>(
            () => _actor.StartAsync(FreerunProfile));

        await _actor.StopAsync();
    }

    [Fact]
    public async Task Stop_when_idle_does_not_throw()
    {
        // Should complete without exception
        await _actor.StopAsync();
    }

    [Fact]
    public async Task Trigger_returns_image_data()
    {
        await _actor.StartAsync(SoftTrigProfile);

        var image = await _actor.TriggerAsync(5000, default);

        Assert.NotNull(image);
        Assert.True(image.Width > 0 && image.Height > 0);

        await _actor.StopAsync();
    }

    [Fact]
    public async Task GetLatestFrame_returns_null_before_any_frame()
    {
        var result = await _actor.GetLatestFrameAsync();

        Assert.Null(result.Frame);
        Assert.False(result.IsNew);
    }

    [Fact]
    public async Task GetLatestFrame_IsNew_true_on_first_poll_false_on_second()
    {
        await _actor.StartAsync(SoftTrigProfile);
        await _actor.TriggerAsync(5000, default);

        var first  = await _actor.GetLatestFrameAsync();
        var second = await _actor.GetLatestFrameAsync();

        Assert.True(first.IsNew,   "First poll should be new");
        Assert.False(second.IsNew, "Second poll of same frame should not be new");

        await _actor.StopAsync();
    }

    [Fact]
    public async Task GetExposure_returns_default_when_not_active()
    {
        var info = await _actor.GetExposureAsync();

        Assert.True(info.ExposureUs > 0);
        Assert.Null(info.ExposureRange);
    }

    [Fact]
    public async Task SetExposure_caches_value_when_not_active()
    {
        await _actor.SetExposureAsync(20_000.0);
        var info = await _actor.GetExposureAsync();

        Assert.Equal(20_000.0, info.ExposureUs);
    }

    [Fact]
    public async Task Status_allowed_actions_change_with_state()
    {
        var idle = await _actor.GetStatusAsync();
        Assert.Contains(ChannelAction.Start, idle.AllowedActions);

        await _actor.StartAsync(FreerunProfile);
        var active = await _actor.GetStatusAsync();
        Assert.Contains(ChannelAction.Stop, active.AllowedActions);
        Assert.DoesNotContain(ChannelAction.Start, active.AllowedActions);

        await _actor.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _actor.DisposeAsync();
        _grabService.Dispose();
    }
}
