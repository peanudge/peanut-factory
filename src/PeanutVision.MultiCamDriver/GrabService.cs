using System.Collections.Concurrent;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Thread-safe service for managing MultiCam driver and grab channels.
/// Implements IDisposable for proper resource cleanup.
/// Suitable for ASP.NET Core Dependency Injection as a Singleton.
/// </summary>
public sealed class GrabService : IGrabService
{
    private readonly object _lock = new();
    private readonly ConcurrentDictionary<uint, GrabChannel> _channels = new();
    private readonly IMultiCamHAL _hal;
    private bool _initialized;
    private bool _disposed;
    private int _openCount;

    private int _boardCount;

    public bool IsInitialized => _initialized;
    public int BoardCount => _boardCount;

    /// <summary>
    /// Creates a new GrabService using the real HAL.
    /// </summary>
    public GrabService() : this(MultiCamHAL.Instance)
    {
    }

    /// <summary>
    /// Creates a new GrabService with the specified HAL.
    /// Use this constructor for testing with MockMultiCamHal.
    /// </summary>
    public GrabService(IMultiCamHAL hal)
    {
        ArgumentNullException.ThrowIfNull(hal);
        _hal = hal;
    }

    /// <summary>
    /// Gets the HAL instance used by this service.
    /// Useful for testing and simulation scenarios.
    /// </summary>
    public IMultiCamHAL Hal => _hal;

    /// <summary>
    /// Initializes the MultiCam driver.
    /// Thread-safe; can be called multiple times.
    /// </summary>
    public void Initialize()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            if (_initialized)
                return;

            // Open driver with NULL (reserved parameter)
            int status = _hal.OpenDriver(null);
            if (status != MultiCamApi.MC_OK)
            {
                throw new MultiCamException(status, "McOpenDriver",
                    "Failed to initialize MultiCam driver. Ensure the driver is installed and hardware is connected.");
            }

            _openCount = 1;

            // Assume single board at index 0 (MC_CONFIGURATION not reliable)
            // Try to verify board exists by querying board info
            try
            {
                uint boardHandle = MultiCamApi.MC_BOARD + (uint)MultiCamApi.DefaultBoardIndex;
                status = _hal.GetParamStr(boardHandle, MultiCamApi.PN_BoardName, out string boardName);
                if (status == MultiCamApi.MC_OK && !string.IsNullOrEmpty(boardName))
                {
                    _boardCount = 1;
                }
            }
            catch
            {
                // Non-critical - continue even if we can't read info
                _boardCount = 0;
            }

            _initialized = true;
        }
    }

    /// <summary>
    /// Creates a new grab channel with the specified options.
    /// </summary>
    public GrabChannel CreateChannel(GrabChannelOptions options)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            EnsureInitialized();

            var channel = new GrabChannel(options, _hal);
            _channels.TryAdd(channel.Handle, channel);

            return channel;
        }
    }

    /// <summary>
    /// Gets board information for the specified board index.
    /// </summary>
    public BoardInfo GetBoardInfo(int boardIndex)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            EnsureInitialized();

            if (boardIndex < 0 || boardIndex >= _boardCount)
            {
                throw new ArgumentOutOfRangeException(nameof(boardIndex),
                    $"Board index must be between 0 and {_boardCount - 1}");
            }

            uint boardHandle = MultiCamApi.MC_BOARD + (uint)boardIndex;

            _hal.GetParamStr(boardHandle, MultiCamApi.PN_BoardType, out string boardType);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_BoardName, out string boardName);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_SerialNumber, out string serialNumber);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_PciPosition, out string pciPosition);

            return new BoardInfo
            {
                Index = boardIndex,
                BoardType = boardType,
                BoardName = boardName,
                SerialNumber = serialNumber,
                PCIPosition = pciPosition
            };
        }
    }

    /// <summary>
    /// Gets detailed status for the specified board including diagnostics.
    /// Uses default connector "M" for Camera Link Full.
    /// </summary>
    public BoardStatus GetBoardStatus(int boardIndex) => GetBoardStatus(boardIndex, "M");

    /// <summary>
    /// Gets detailed status for the specified board including diagnostics.
    /// </summary>
    /// <param name="boardIndex">Board index (0-based)</param>
    /// <param name="connector">Connector to probe for camera status</param>
    public BoardStatus GetBoardStatus(int boardIndex, string connector)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            EnsureInitialized();

            if (boardIndex < 0 || boardIndex >= _boardCount)
            {
                throw new ArgumentOutOfRangeException(nameof(boardIndex),
                    $"Board index must be between 0 and {_boardCount - 1}");
            }

            uint boardHandle = MultiCamApi.MC_BOARD + (uint)boardIndex;

            // Query board-level parameters
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_BoardType, out string boardType);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_BoardName, out string boardName);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_SerialNumber, out string serial);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_PciPosition, out string pci);
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_PCIeLinkInfo, out string pcieInfo);

            // Channel-level parameters (require temporary channel)
            string inputConnector = "N/A";
            string inputState = "N/A";
            string outputState = "N/A";
            string signal = "N/A";
            string cameraLinkStatus = "N/A";
            int syncErrors = 0;
            int clockErrors = 0;
            int grabberErrors = 0;
            int lineViolations = 0;
            int frameViolations = 0;

            // Create temporary channel to query camera/input status
            int status = _hal.Create(MultiCamApi.MC_CHANNEL, out uint tempChannel);
            if (status == MultiCamApi.MC_OK)
            {
                try
                {
                    // Configure minimal channel settings
                    _hal.SetParamInt(tempChannel, MultiCamApi.PN_DriverIndex, boardIndex);
                    _hal.SetParamStr(tempChannel, MultiCamApi.PN_Connector, connector);

                    // Query channel-level input/output status
                    _hal.GetParamStr(tempChannel, MultiCamApi.PN_InputConnectorName, out inputConnector);
                    _hal.GetParamStr(tempChannel, MultiCamApi.PN_InputState, out inputState);
                    _hal.GetParamStr(tempChannel, MultiCamApi.PN_OutputState, out outputState);
                    _hal.GetParamStr(tempChannel, MultiCamApi.PN_DetectedSignalStrength, out signal);

                    // Query link status
                    _hal.GetParamStr(tempChannel, MultiCamApi.PN_CameraLinkFrequencyRange, out cameraLinkStatus);
                    _hal.GetParamInt(tempChannel, MultiCamApi.PN_ChannelLinkSyncErrors_X, out syncErrors);
                    _hal.GetParamInt(tempChannel, MultiCamApi.PN_ChannelLinkClockErrors_X, out clockErrors);

                    // Query diagnostics
                    _hal.GetParamInt(tempChannel, MultiCamApi.PN_GrabberErrors, out grabberErrors);
                    _hal.GetParamInt(tempChannel, MultiCamApi.PN_LineTriggerViolation, out lineViolations);
                    _hal.GetParamInt(tempChannel, MultiCamApi.PN_FrameTriggerViolation, out frameViolations);
                }
                finally
                {
                    // Always delete the temporary channel
                    _hal.Delete(tempChannel);
                }
            }

            return new BoardStatus
            {
                Index = boardIndex,
                BoardName = boardName ?? "Unknown",
                BoardType = boardType ?? "Unknown",
                SerialNumber = serial ?? "Unknown",
                PCIPosition = pci ?? "Unknown",
                InputConnector = inputConnector ?? "N/A",
                InputState = inputState ?? "N/A",
                OutputState = outputState ?? "N/A",
                SignalStrength = signal ?? "N/A",
                CameraLinkStatus = cameraLinkStatus ?? "N/A",
                GrabberErrors = grabberErrors,
                SyncErrors = syncErrors,
                ClockErrors = clockErrors,
                LineTriggerViolations = lineViolations,
                FrameTriggerViolations = frameViolations,
                PCIeLinkInfo = pcieInfo ?? "N/A"
            };
        }
    }

    /// <summary>
    /// Gets the default camera profile from the registry.
    /// </summary>
    public CameraProfile? DefaultCameraProfile => CameraRegistry.Default.DefaultProfile;

    /// <summary>
    /// Gets the camera profile registry.
    /// </summary>
    public CameraRegistry CameraProfiles => CameraRegistry.Default;

    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException(
                "GrabService is not initialized. Call Initialize() first.");
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(GrabService));
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;

            // Dispose all channels
            foreach (var channel in _channels.Values)
            {
                try
                {
                    channel.Dispose();
                }
                catch { /* Ignore cleanup errors */ }
            }
            _channels.Clear();

            // Close driver
            if (_openCount > 0)
            {
                for (int i = 0; i < _openCount; i++)
                {
                    _hal.CloseDriver();
                }
                _openCount = 0;
            }

            _initialized = false;
        }
    }
}
