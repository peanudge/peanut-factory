using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Api.Tests.Infrastructure;

public class PeanutVisionApiFactory : WebApplicationFactory<Program>
{
    public MockMultiCamHAL MockHal { get; } = new();
    private IntPtr _surfaceMemory;
    private readonly string _testDbPath = Path.Combine(Path.GetTempPath(), $"pv-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Allocate real native memory so SurfaceData.ToArray() works in tests
        var bufferSize = MockHal.Configuration.DefaultImageWidth
            * MockHal.Configuration.DefaultImageHeight * 3;
        _surfaceMemory = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, _surfaceMemory, bufferSize);
        MockHal.Configuration.SimulatedSurfaceAddress = _surfaceMemory;
        MockHal.Configuration.AutoSimulateFrameOnTrigger = true;

        builder.ConfigureServices(services =>
        {
            // Replace ICamFileService with test cam files
            services.RemoveAll<ICamFileService>();
            services.AddSingleton(TestCamFileHelper.GetOrCreate());

            // Remove the production IAcquisitionChannelManager registration
            services.RemoveAll<IAcquisitionChannelManager>();

            // Register an AcquisitionChannelManager backed by MockMultiCamHAL
            services.AddSingleton<IAcquisitionChannelManager>(_ =>
            {
                var service = new AcquisitionChannelManager(MockHal);
                service.Initialize();
                return service;
            });

            // Replace DbContext with test-specific SQLite file
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={_testDbPath}"));
        });
    }

    public void ResetMockState()
    {
        MockHal.CallLog.Reset();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_surfaceMemory != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_surfaceMemory);
            _surfaceMemory = IntPtr.Zero;
        }
        try { File.Delete(_testDbPath); } catch { /* ignore */ }
    }
}
