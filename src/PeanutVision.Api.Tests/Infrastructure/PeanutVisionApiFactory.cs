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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Create dummy cam files so GetCamFilePath succeeds in tests
            // (must run inside ConfigureServices, after Program.cs sets the directory)
            var camDir = CamFileResource.GetDirectory();
            foreach (var name in new[]
            {
                CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8,
                CamFileResource.KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8,
            })
            {
                var path = Path.Combine(camDir, name);
                if (!File.Exists(path)) File.WriteAllText(path, "");
            }

            // Register built-in profiles for tests
            foreach (var profile in CrevisProfiles.All)
                CameraRegistry.Default.Register(profile);

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
}
