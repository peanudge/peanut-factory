using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;
using System.Runtime.InteropServices;

namespace PeanutVision.Api.Tests.Unit;

public class CameraActorErrorTests : IAsyncDisposable
{
    private readonly MockMultiCamHAL _hal;
    private readonly GrabService _grabService;
    private readonly CameraActor _actor;
    private static readonly ProfileId FreerunProfile = new("crevis-tc-a160k-freerun-rgb8.cam");
    private static readonly ProfileId SoftTrigProfile = new("crevis-tc-a160k-softtrig-rgb8.cam");

    public CameraActorErrorTests()
    {
        _hal = new MockMultiCamHAL();

        var bufferSize = _hal.Configuration.DefaultImageWidth
            * _hal.Configuration.DefaultImageHeight * 3;
        var mem = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, mem, bufferSize);
        _hal.Configuration.SimulatedSurfaceAddress = mem;
        _hal.Configuration.AutoSimulateFrameOnTrigger = true;

        _grabService = new GrabService(_hal);
        _grabService.Initialize();

        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        var saveSettings = new ImageSaveSettingsService(
            Path.Combine(Path.GetTempPath(), $"actor-error-test-settings-{Guid.NewGuid():N}.json"));

        _actor = new CameraActor(
            cameraId:        "cam-err-1",
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
    public async Task Trigger_when_channel_not_active_throws_ChannelNotAvailableException()
    {
        await Assert.ThrowsAsync<ChannelNotAvailableException>(
            () => _actor.TriggerAsync(1000, default));
    }

    [Fact]
    public async Task Trigger_while_trigger_pending_throws_AcquisitionConflictException()
    {
        _hal.Configuration.AutoSimulateFrameOnTrigger = false;
        try
        {
            await _actor.StartAsync(SoftTrigProfile);

            var trigger1 = _actor.TriggerAsync(5000, default);
            await Task.Delay(100);

            await Assert.ThrowsAsync<AcquisitionConflictException>(
                () => _actor.TriggerAsync(1000, default));

            await _actor.StopAsync();
        }
        finally
        {
            _hal.Configuration.AutoSimulateFrameOnTrigger = true;
        }
    }

    [Fact]
    public async Task Trigger_when_freerun_mode_throws_InvalidParameterException()
    {
        await _actor.StartAsync(FreerunProfile);

        await Assert.ThrowsAsync<InvalidParameterException>(
            () => _actor.TriggerAsync(1000, default));

        await _actor.StopAsync();
    }

    [Fact]
    public async Task AcquisitionError_while_trigger_pending_rejects_trigger_with_exception()
    {
        _hal.Configuration.AutoSimulateFrameOnTrigger = false;
        await _actor.StartAsync(SoftTrigProfile);

        var triggerTask = _actor.TriggerAsync(5000, default);
        await Task.Delay(100);

        // The channel handle is the first instance created by the mock HAL (handle = 1)
        uint channelHandle = 1;
        _hal.SimulateAcquisitionError(channelHandle, McSignal.MC_SIG_ACQUISITION_FAILURE);
        await Task.Delay(100);

        await Assert.ThrowsAsync<InvalidOperationException>(() => triggerTask);

        await _actor.StopAsync();
    }

    [Fact]
    public async Task SetExposure_when_active_returns_exposure_range()
    {
        await _actor.StartAsync(FreerunProfile);

        var info = await _actor.SetExposureAsync(20_000.0);

        Assert.Equal(20_000.0, info.ExposureUs, precision: 0);
        Assert.NotNull(info.ExposureRange);
        Assert.True(info.ExposureRange!.Min > 0);
        Assert.True(info.ExposureRange.Max > info.ExposureRange.Min);

        await _actor.StopAsync();
    }

    [Fact]
    public async Task SetExposure_null_preserves_cached_value()
    {
        await _actor.SetExposureAsync(25_000.0);
        await _actor.SetExposureAsync(null);

        var info = await _actor.GetExposureAsync();
        Assert.Equal(25_000.0, info.ExposureUs);
    }

    [Fact]
    public async Task DisposeAsync_stops_active_acquisition_gracefully()
    {
        await _actor.StartAsync(FreerunProfile);
        Assert.True((await _actor.GetStatusAsync()).IsActive);

        await _actor.DisposeAsync();

        Assert.True(_hal.CallLog.AcquisitionStopped);
    }

    [Fact]
    public async Task Start_with_intervalMs_below_minimum_throws_InvalidParameterException()
    {
        var ex = await Assert.ThrowsAsync<InvalidParameterException>(
            () => _actor.StartAsync(FreerunProfile, intervalMs: 10));

        Assert.Contains("50ms", ex.Message);
    }

    public async ValueTask DisposeAsync()
    {
        await _actor.DisposeAsync();
        _grabService.Dispose();
    }
}
