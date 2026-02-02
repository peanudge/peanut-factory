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
    /// Gets board information for the specified board index.
    /// </summary>
    BoardInfo GetBoardInfo(int boardIndex);

    /// <summary>
    /// Gets detailed status for the specified board including diagnostics.
    /// </summary>
    BoardStatus GetBoardStatus(int boardIndex);

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

/// <summary>
/// Detailed board status information including diagnostics.
/// </summary>
public readonly struct BoardStatus
{
    // Basic Info
    /// <summary>Board index (0-based)</summary>
    public int Index { get; init; }

    /// <summary>Board name/model</summary>
    public string BoardName { get; init; }

    /// <summary>Board type identifier</summary>
    public string BoardType { get; init; }

    /// <summary>Serial number</summary>
    public string SerialNumber { get; init; }

    /// <summary>PCI bus position</summary>
    public string PCIPosition { get; init; }

    // Input Status
    /// <summary>Input connector identifier</summary>
    public string InputConnector { get; init; }

    /// <summary>Current input state</summary>
    public string InputState { get; init; }

    /// <summary>Detected signal strength</summary>
    public string SignalStrength { get; init; }

    // Output Status
    /// <summary>Current output state</summary>
    public string OutputState { get; init; }

    // Link Status
    /// <summary>Camera Link connection status</summary>
    public string CameraLinkStatus { get; init; }

    /// <summary>Number of link synchronization errors</summary>
    public int SyncErrors { get; init; }

    /// <summary>Number of link clock errors</summary>
    public int ClockErrors { get; init; }

    // Diagnostics
    /// <summary>Number of grabber errors</summary>
    public int GrabberErrors { get; init; }

    /// <summary>Number of frame trigger violations</summary>
    public int FrameTriggerViolations { get; init; }

    /// <summary>Number of line trigger violations</summary>
    public int LineTriggerViolations { get; init; }

    // PCIe
    /// <summary>PCIe link information</summary>
    public string PCIeLinkInfo { get; init; }
}
