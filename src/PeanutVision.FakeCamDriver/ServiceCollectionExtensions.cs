using Microsoft.Extensions.DependencyInjection;
using PeanutVision.MultiCamDriver;

namespace PeanutVision.FakeCamDriver;

/// <summary>
/// DI extension methods for registering FakeMultiCamHAL-based AcquisitionChannelManager.
/// </summary>
public static class FakeCamServiceCollectionExtensions
{
    /// <summary>
    /// Adds a fake IAcquisitionChannelManager backed by FakeMultiCamHAL with test pattern generation.
    /// </summary>
    public static IServiceCollection AddFakeAcquisitionChannelManager(
        this IServiceCollection services,
        Action<FakeHalConfiguration>? configure = null)
    {
        services.AddSingleton<IAcquisitionChannelManager>(sp =>
        {
            var fakeHal = new FakeMultiCamHAL();
            configure?.Invoke(fakeHal.FakeConfig);

            var service = new AcquisitionChannelManager(fakeHal);
            service.Initialize();
            return service;
        });

        return services;
    }
}
