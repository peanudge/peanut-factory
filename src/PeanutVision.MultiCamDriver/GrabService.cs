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
                uint boardHandle = MultiCamApi.MC_BOARD; // Board index 0
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
    /// Creates a new grab channel with default options and the specified .cam file.
    /// </summary>
    public GrabChannel CreateChannel(string camFilePath, int driverIndex = 0)
    {
        return CreateChannel(new GrabChannelOptions
        {
            CamFilePath = camFilePath,
            DriverIndex = driverIndex,
            UseCallback = true,
            SurfaceCount = 4,
            TriggerMode = McTrigMode.MC_TrigMode_IMMEDIATE
        });
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
            _hal.GetParamStr(boardHandle, MultiCamApi.PN_PCIPosition, out string pciPosition);

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
    /// Creates a new grab channel using a camera profile.
    /// </summary>
    public GrabChannel CreateChannel(CameraProfile profile, int driverIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(profile);
        var options = profile.ToChannelOptions(driverIndex);
        return CreateChannel(options);
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
