using System.Collections.Concurrent;
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
    private string _driverVersion = string.Empty;

    public bool IsInitialized => _initialized;
    public int BoardCount => _boardCount;
    public string DriverVersion => _driverVersion;

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
            if (status != MultiCamNative.MC_OK)
            {
                throw new MultiCamException(status, "McOpenDriver",
                    "Failed to initialize MultiCam driver. Ensure the driver is installed and hardware is connected.");
            }

            _openCount = 1;

            // Query system information
            try
            {
                status = _hal.GetParamInt(MultiCamNative.MC_CONFIGURATION,
                    MultiCamNative.PN_BoardCount, out _boardCount);

                status = _hal.GetParamStr(MultiCamNative.MC_CONFIGURATION,
                    MultiCamNative.PN_DriverVersion, out _driverVersion);
            }
            catch
            {
                // Non-critical - continue even if we can't read info
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

            uint boardHandle = MultiCamNative.MC_BOARD + (uint)boardIndex;

            _hal.GetParamStr(boardHandle, MultiCamNative.PN_BoardType, out string boardType);
            _hal.GetParamStr(boardHandle, MultiCamNative.PN_BoardName, out string boardName);
            _hal.GetParamStr(boardHandle, MultiCamNative.PN_SerialNumber, out string serialNumber);
            _hal.GetParamStr(boardHandle, MultiCamNative.PN_PciPosition, out string pciPosition);

            return new BoardInfo
            {
                Index = boardIndex,
                BoardType = boardType,
                BoardName = boardName,
                SerialNumber = serialNumber,
                PciPosition = pciPosition
            };
        }
    }

    /// <summary>
    /// Creates a new grab channel using an embedded camera configuration file.
    /// The cam file is extracted from embedded resources to a temp directory.
    /// </summary>
    public GrabChannel CreateChannelFromEmbeddedCamFile(string embeddedCamFileName, int driverIndex = 0)
    {
        string camFilePath = CamFileResource.GetCamFilePath(embeddedCamFileName);
        return CreateChannel(camFilePath, driverIndex);
    }

    /// <summary>
    /// Creates a new grab channel using the default TC-A160K FreeRun RGB8 configuration.
    /// </summary>
    public GrabChannel CreateChannelForTC_A160K(int driverIndex = 0)
    {
        return CreateChannelFromEmbeddedCamFile(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8, driverIndex);
    }

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
