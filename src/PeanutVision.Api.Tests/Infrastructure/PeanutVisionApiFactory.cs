using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Api.Tests.Infrastructure;

public class PeanutVisionApiFactory : WebApplicationFactory<Program>
{
    public MockMultiCamHAL MockHal { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
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
