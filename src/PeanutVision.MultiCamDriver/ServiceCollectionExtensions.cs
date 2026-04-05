#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Extension methods for registering AcquisitionChannelManager with ASP.NET Core Dependency Injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the IAcquisitionChannelManager to the service collection as a singleton.
    /// The manager must be initialized by calling Initialize() after resolution.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// // In Program.cs or Startup.cs
    /// builder.Services.AddAcquisitionChannelManager();
    ///
    /// // In your service/controller
    /// public class VisionController : ControllerBase
    /// {
    ///     private readonly IAcquisitionChannelManager _channelManager;
    ///
    ///     public VisionController(IAcquisitionChannelManager channelManager)
    ///     {
    ///         _channelManager = channelManager;
    ///         _channelManager.Initialize(); // Initialize on first use
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddAcquisitionChannelManager(this IServiceCollection services)
    {
        services.TryAddSingleton<IAcquisitionChannelManager, AcquisitionChannelManager>();
        return services;
    }

    /// <summary>
    /// Adds the IAcquisitionChannelManager to the service collection as a singleton with auto-initialization.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="autoInitialize">If true, Initialize() is called immediately upon service creation</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// // In Program.cs
    /// builder.Services.AddAcquisitionChannelManager(autoInitialize: true);
    /// </code>
    /// </example>
    public static IServiceCollection AddAcquisitionChannelManager(this IServiceCollection services, bool autoInitialize)
    {
        if (!autoInitialize)
        {
            return services.AddAcquisitionChannelManager();
        }

        services.TryAddSingleton<IAcquisitionChannelManager>(sp =>
        {
            var manager = new AcquisitionChannelManager();
            manager.Initialize();
            return manager;
        });

        return services;
    }

    /// <summary>
    /// Adds the ICamFileService to the service collection as a singleton.
    /// Scans the specified directory for .cam files and parses their metadata.
    /// </summary>
    public static IServiceCollection AddCamFileService(this IServiceCollection services, string directory)
    {
        services.TryAddSingleton<ICamFileService>(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("CamFileService");
            var service = new CamFileService(directory);

            logger.LogInformation("CamFile directory: {Directory}", service.Directory);

            if (service.CamFiles.Count == 0)
            {
                logger.LogWarning("No .cam files found in {Directory}", service.Directory);
            }
            else
            {
                logger.LogInformation("Loaded {Count} cam file(s):", service.CamFiles.Count);
                foreach (var cam in service.CamFiles)
                {
                    logger.LogInformation("  {FileName} — {Manufacturer} {Model}, {Width}x{Height}, {Spectrum}, {ColorFormat}, TrigMode={TrigMode}",
                        cam.FileName, cam.Manufacturer, cam.CameraModel,
                        cam.Width, cam.Height, cam.Spectrum, cam.ColorFormat, cam.TrigMode);
                }
            }

            return service;
        });
        return services;
    }

    /// <summary>
    /// Adds the IAcquisitionChannelManager with a custom configuration action.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action to run after initialization</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddAcquisitionChannelManager(service =>
    /// {
    ///     Console.WriteLine($"Boards detected: {service.BoardCount}");
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddAcquisitionChannelManager(
        this IServiceCollection services,
        Action<IAcquisitionChannelManager> configure)
    {
        services.TryAddSingleton<IAcquisitionChannelManager>(sp =>
        {
            var manager = new AcquisitionChannelManager();
            manager.Initialize();
            configure(manager);
            return manager;
        });

        return services;
    }
}
#endif
