using PeanutVision.MultiCamDriver.Camera;

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
    /// Creates a new grab channel using a camera profile.
    /// </summary>
    /// <param name="profile">Camera profile with configuration settings</param>
    /// <param name="driverIndex">Board index (default 0)</param>
    /// <returns>Configured grab channel ready for acquisition</returns>
    GrabChannel CreateChannel(CameraProfile profile, int driverIndex = 0);

    /// <summary>
    /// Gets the default camera profile (first available from registry).
    /// </summary>
    CameraProfile? DefaultCameraProfile { get; }

    /// <summary>
    /// Gets the camera profile registry.
    /// </summary>
    CameraRegistry CameraProfiles { get; }
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
    public string PCIPosition { get; init; }
}
