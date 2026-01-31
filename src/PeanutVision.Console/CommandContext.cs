using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Console;

/// <summary>
/// Shared context for all commands containing service and configuration state.
/// </summary>
public sealed class CommandContext : IDisposable
{
    public IGrabService Service { get; }
    public bool UseMockHal { get; }

    private CommandContext(IGrabService service, bool useMockHal)
    {
        Service = service;
        UseMockHal = useMockHal;
    }

    /// <summary>
    /// Creates and initializes a command context.
    /// </summary>
    public static CommandContext Create(bool useMockHal)
    {
        IGrabService service;

        if (useMockHal)
        {
            var mockHal = new MockMultiCamHAL();
            mockHal.Configuration.BoardCount = 1;
            mockHal.Configuration.DefaultImageWidth = 4096;
            mockHal.Configuration.DefaultImageHeight = 3072;
            service = new GrabService(mockHal);
        }
        else
        {
            service = new GrabService();
        }

        service.Initialize();
        return new CommandContext(service, useMockHal);
    }

    /// <summary>
    /// Gets the MockMultiCamHAL if running in mock mode, otherwise null.
    /// </summary>
    public MockMultiCamHAL? GetMockHal()
    {
        if (UseMockHal && Service is GrabService grabService && grabService.Hal is MockMultiCamHAL mockHal)
        {
            return mockHal;
        }
        return null;
    }

    /// <summary>
    /// Checks if hardware is available (or mock mode is enabled).
    /// </summary>
    public bool IsHardwareAvailable => Service.BoardCount > 0 || UseMockHal;

    public void Dispose()
    {
        Service.Dispose();
    }
}
