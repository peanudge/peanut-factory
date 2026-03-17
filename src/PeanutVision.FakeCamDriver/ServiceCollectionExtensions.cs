using Microsoft.Extensions.DependencyInjection;
using PeanutVision.MultiCamDriver;

namespace PeanutVision.FakeCamDriver;

/// <summary>
/// DI extension methods for registering FakeMultiCamHAL-based GrabService.
/// </summary>
public static class FakeCamServiceCollectionExtensions
{
    /// <summary>
    /// Adds a fake IGrabService backed by FakeMultiCamHAL with test pattern generation.
    /// </summary>
    public static IServiceCollection AddFakeGrabService(
        this IServiceCollection services,
        Action<FakeHalConfiguration>? configure = null)
    {
        services.AddSingleton<IGrabService>(sp =>
        {
            var fakeHal = new FakeMultiCamHAL();
            configure?.Invoke(fakeHal.FakeConfig);

            var service = new GrabService(fakeHal);
            service.Initialize();
            return service;
        });

        return services;
    }
}
