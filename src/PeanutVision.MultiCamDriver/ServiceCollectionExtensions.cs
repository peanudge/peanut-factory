#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Extension methods for registering GrabService with ASP.NET Core Dependency Injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the IGrabService to the service collection as a singleton.
    /// The service must be initialized by calling Initialize() after resolution.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// // In Program.cs or Startup.cs
    /// builder.Services.AddGrabService();
    ///
    /// // In your service/controller
    /// public class VisionController : ControllerBase
    /// {
    ///     private readonly IGrabService _grabService;
    ///
    ///     public VisionController(IGrabService grabService)
    ///     {
    ///         _grabService = grabService;
    ///         _grabService.Initialize(); // Initialize on first use
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddGrabService(this IServiceCollection services)
    {
        services.TryAddSingleton<IGrabService, GrabService>();
        return services;
    }

    /// <summary>
    /// Adds the IGrabService to the service collection as a singleton with auto-initialization.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="autoInitialize">If true, Initialize() is called immediately upon service creation</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// // In Program.cs
    /// builder.Services.AddGrabService(autoInitialize: true);
    /// </code>
    /// </example>
    public static IServiceCollection AddGrabService(this IServiceCollection services, bool autoInitialize)
    {
        if (!autoInitialize)
        {
            return services.AddGrabService();
        }

        services.TryAddSingleton<IGrabService>(sp =>
        {
            var service = new GrabService();
            service.Initialize();
            return service;
        });

        return services;
    }

    /// <summary>
    /// Adds the IGrabService with a custom configuration action.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action to run after initialization</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddGrabService(service =>
    /// {
    ///     Console.WriteLine($"Boards detected: {service.BoardCount}");
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddGrabService(
        this IServiceCollection services,
        Action<IGrabService> configure)
    {
        services.TryAddSingleton<IGrabService>(sp =>
        {
            var service = new GrabService();
            service.Initialize();
            configure(service);
            return service;
        });

        return services;
    }
}
#endif
