using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Api.Tests.Infrastructure;

public class PeanutVisionApiFactory : WebApplicationFactory<Program>
{
    public MockMultiCamHAL MockHal { get; } = new();
    private IntPtr _surfaceMemory;

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
            // Create dummy cam files and register profiles for tests
            var camDir = CamFileResource.GetDirectory();
            foreach (var name in new[]
            {
                "crevis-tc-a160k-freerun-rgb8.cam",
                "crevis-tc-a160k-freerun-1tap-rgb8.cam",
                "crevis-tc-a160k-softtrig-rgb8.cam",
            })
            {
                var path = Path.Combine(camDir, name);
                if (!File.Exists(path)) File.WriteAllText(path, "");
                CameraRegistry.Default.Register(CameraProfile.FromCamFile(name));
            }

            // Remove the production IGrabService registration
            services.RemoveAll<IGrabService>();

            // Register a GrabService backed by MockMultiCamHAL
            services.AddSingleton<IGrabService>(_ =>
            {
                var service = new GrabService(MockHal);
                service.Initialize();
                return service;
            });
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
    }
}
