namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Service interface for frame grabber operations.
/// Designed for ASP.NET Core Dependency Injection.
/// </summary>
public interface IGrabService : IDisposable
{
    /// <summary>
    /// Whether the driver is initialized and ready.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Gets the number of boards detected in the system.
    /// </summary>
    int BoardCount { get; }

    /// <summary>
    /// Gets the driver version string.
    /// </summary>
    string DriverVersion { get; }

    /// <summary>
    /// Initializes the MultiCam driver.
    /// Must be called before any other operations.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Creates a new grab channel with the specified options.
    /// </summary>
    /// <param name="options">Channel configuration options</param>
    /// <returns>Configured grab channel ready for acquisition</returns>
    GrabChannel CreateChannel(GrabChannelOptions options);

    /// <summary>
    /// Creates a new grab channel with default options and the specified .cam file.
    /// </summary>
    /// <param name="camFilePath">Path to the .cam configuration file</param>
    /// <param name="driverIndex">Board index (default 0)</param>
    /// <returns>Configured grab channel ready for acquisition</returns>
    GrabChannel CreateChannel(string camFilePath, int driverIndex = 0);

    /// <summary>
    /// Gets board information for the specified board index.
    /// </summary>
    BoardInfo GetBoardInfo(int boardIndex);

    /// <summary>
    /// Creates a new grab channel using an embedded camera configuration file.
    /// The cam file is extracted from embedded resources to a temp directory.
    /// </summary>
    /// <param name="embeddedCamFileName">Name of the embedded .cam file (e.g., "TC-A160K-SEM_freerun_RGB8.cam")</param>
    /// <param name="driverIndex">Board index (default 0)</param>
    /// <returns>Configured grab channel ready for acquisition</returns>
    GrabChannel CreateChannelFromEmbeddedCamFile(string embeddedCamFileName, int driverIndex = 0);

    /// <summary>
    /// Creates a new grab channel using the default TC-A160K FreeRun RGB8 configuration.
    /// </summary>
    /// <param name="driverIndex">Board index (default 0)</param>
    /// <returns>Configured grab channel ready for acquisition</returns>
    GrabChannel CreateChannelForTC_A160K(int driverIndex = 0);
}

/// <summary>
/// Information about a detected frame grabber board.
/// </summary>
public readonly struct BoardInfo
{
    /// <summary>Board index (0-based)</summary>
    public int Index { get; init; }

    /// <summary>Board type identifier</summary>
    public string BoardType { get; init; }

    /// <summary>Board name/model</summary>
    public string BoardName { get; init; }

    /// <summary>Serial number</summary>
    public string SerialNumber { get; init; }

    /// <summary>PCI bus position</summary>
    public string PciPosition { get; init; }
}
