using System.Runtime.InteropServices;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Configuration options for creating a GrabChannel
/// </summary>
public class GrabChannelOptions
{
    /// <summary>Driver index (board index), typically 0 for single-board systems</summary>
    public int DriverIndex { get; set; } = 0;

    /// <summary>Connector on the board (e.g., "M" for medium, "A" for first connector)</summary>
    public string Connector { get; set; } = "M";

    /// <summary>Full path to the .cam configuration file</summary>
    public string? CamFilePath { get; set; }

    /// <summary>Number of surfaces in the cluster (frame buffers)</summary>
    public int SurfaceCount { get; set; } = 4;

    /// <summary>Enable callback-based acquisition (recommended for high performance)</summary>
    public bool UseCallback { get; set; } = true;

    /// <summary>Trigger mode for acquisition</summary>
    public McTrigMode TriggerMode { get; set; } = McTrigMode.MC_TrigMode_IMMEDIATE;
}

/// <summary>
/// Represents a MultiCam acquisition channel with thread-safe callback handling.
/// Manages the complete lifecycle of a frame grabber channel.
/// </summary>
public sealed class GrabChannel : IDisposable
{
    private readonly object _lock = new();
    private readonly IMultiCamHAL _hal;
    private uint _channelHandle;
    private bool _disposed;
    private bool _isActive;

    // Callback handling - prevent GC collection
    private McCallback? _nativeCallback;
    private GCHandle _callbackHandle;
    private GCHandle _thisHandle;

    // Image properties (cached after channel configuration)
    private int _imageWidth;
    private int _imageHeight;
    private int _bufferPitch;
    private int _bufferSize;
    private int _surfaceCount;

    /// <summary>Channel handle for direct native API access if needed</summary>
    public uint Handle => _channelHandle;

    /// <summary>Whether the channel is currently acquiring</summary>
    public bool IsActive => _isActive;

    /// <summary>Image width in pixels</summary>
    public int ImageWidth => _imageWidth;

    /// <summary>Image height in pixels</summary>
    public int ImageHeight => _imageHeight;

    /// <summary>
    /// Fired when a frame is ready for processing.
    /// WARNING: Called from MultiCam thread - keep handler fast and thread-safe.
    /// </summary>
    public event EventHandler<FrameAcquiredEventArgs>? FrameAcquired;

    /// <summary>
    /// Fired when an acquisition error occurs.
    /// </summary>
    public event EventHandler<AcquisitionErrorEventArgs>? AcquisitionError;

    /// <summary>
    /// Fired when acquisition ends (MC_SIG_END_CHANNEL_ACTIVITY).
    /// </summary>
    public event EventHandler? AcquisitionEnded;

    /// <summary>
    /// Creates a new GrabChannel with the specified options using the real HAL.
    /// </summary>
    public GrabChannel(GrabChannelOptions options)
        : this(options, MultiCamHAL.Instance)
    {
    }

    /// <summary>
    /// Creates a new GrabChannel with the specified options and HAL.
    /// Use this constructor for testing with MockMultiCamHal.
    /// </summary>
    public GrabChannel(GrabChannelOptions options, IMultiCamHAL hal)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(hal);

        _hal = hal;
        CreateChannel(options);
    }

    private void CreateChannel(GrabChannelOptions options)
    {
        // Create channel instance
        int status = _hal.Create(MultiCamNative.MC_CHANNEL, out _channelHandle);
        ThrowOnError(status, "McCreate(MC_CHANNEL)");

        try
        {
            // Set driver index (board selection)
            status = _hal.SetParamInt(_channelHandle, MultiCamNative.PN_DriverIndex, options.DriverIndex);
            ThrowOnError(status, $"SetParam(DriverIndex={options.DriverIndex})");

            // Set connector
            status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_Connector, options.Connector);
            ThrowOnError(status, $"SetParam(Connector={options.Connector})");

            // Load .cam file if specified
            if (!string.IsNullOrEmpty(options.CamFilePath))
            {
                status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_CamFile, options.CamFilePath);
                ThrowOnError(status, $"SetParam(CamFile={options.CamFilePath})");
            }

            // Configure surface cluster
            status = _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SurfaceCount, options.SurfaceCount);
            ThrowOnError(status, $"SetParam(SurfaceCount={options.SurfaceCount})");

            _surfaceCount = options.SurfaceCount;

            // Set trigger mode
            string trigModeStr = options.TriggerMode switch
            {
                McTrigMode.MC_TrigMode_IMMEDIATE => MultiCamNative.MC_TrigMode_IMMEDIATE_STR,
                McTrigMode.MC_TrigMode_HARD => MultiCamNative.MC_TrigMode_HARD_STR,
                McTrigMode.MC_TrigMode_SOFT => MultiCamNative.MC_TrigMode_SOFT_STR,
                McTrigMode.MC_TrigMode_COMBINED => MultiCamNative.MC_TrigMode_COMBINED_STR,
                _ => MultiCamNative.MC_TrigMode_IMMEDIATE_STR
            };
            status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_TrigMode, trigModeStr);
            ThrowOnError(status, $"SetParam(TrigMode={trigModeStr})");

            // Enable surface processing signal
            SetSignalEnable(McSignal.MC_SIG_SURFACE_PROCESSING, true);
            SetSignalEnable(McSignal.MC_SIG_ACQUISITION_FAILURE, true);
            SetSignalEnable(McSignal.MC_SIG_END_CHANNEL_ACTIVITY, true);
            SetSignalEnable(McSignal.MC_SIG_UNRECOVERABLE_ERROR, true);

            // Register callback if requested
            if (options.UseCallback)
            {
                RegisterCallback();
            }

            // Read back image parameters
            RefreshImageParameters();
        }
        catch
        {
            // Cleanup on failure
            _hal.Delete(_channelHandle);
            _channelHandle = 0;
            throw;
        }
    }

    private void RegisterCallback()
    {
        // Create delegate and prevent GC
        _nativeCallback = OnNativeCallback;
        _callbackHandle = GCHandle.Alloc(_nativeCallback);

        // Pass 'this' as context so we can route callbacks
        _thisHandle = GCHandle.Alloc(this);

        IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(_nativeCallback);
        IntPtr contextPtr = GCHandle.ToIntPtr(_thisHandle);

        int status = _hal.RegisterCallback(_channelHandle, callbackPtr, contextPtr);
        ThrowOnError(status, "McRegisterCallback");
    }

    /// <summary>
    /// Native callback invoked by MultiCam from its own thread.
    /// Routes to the appropriate event handler.
    /// </summary>
    private static void OnNativeCallback(ref McSignalInfo info)
    {
        // Recover 'this' from context
        if (info.Context == IntPtr.Zero)
            return;

        GrabChannel? channel = null;
        try
        {
            GCHandle handle = GCHandle.FromIntPtr(info.Context);
            channel = handle.Target as GrabChannel;
        }
        catch
        {
            return; // Invalid context, ignore
        }

        channel?.ProcessSignal(ref info);
    }

    internal void ProcessSignal(ref McSignalInfo info)
    {
        McSignal signal = (McSignal)info.Signal;

        switch (signal)
        {
            case McSignal.MC_SIG_SURFACE_PROCESSING:
                ProcessSurfaceSignal(ref info);
                break;

            case McSignal.MC_SIG_ACQUISITION_FAILURE:
                AcquisitionError?.Invoke(this, new AcquisitionErrorEventArgs(
                    signal, info.Instance, info.SignalInfo, "Acquisition failure detected"));
                break;

            case McSignal.MC_SIG_UNRECOVERABLE_ERROR:
                _isActive = false;
                AcquisitionError?.Invoke(this, new AcquisitionErrorEventArgs(
                    signal, info.Instance, info.SignalInfo, "Unrecoverable error - acquisition stopped"));
                break;

            case McSignal.MC_SIG_END_CHANNEL_ACTIVITY:
                _isActive = false;
                AcquisitionEnded?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    private void ProcessSurfaceSignal(ref McSignalInfo info)
    {
        // SignalInfo contains the surface index
        int surfaceIndex = (int)info.SignalInfo;

        try
        {
            // Get surface data
            var surface = GetSurfaceData(surfaceIndex);

            // Fire event
            FrameAcquired?.Invoke(this, new FrameAcquiredEventArgs(
                surface, _channelHandle, McSignal.MC_SIG_SURFACE_PROCESSING));
        }
        finally
        {
            // CRITICAL: Release surface back to MultiCam for next acquisition
            ReleaseSurface(surfaceIndex);
        }
    }

    private SurfaceData GetSurfaceData(int surfaceIndex)
    {
        // Set surface index for parameter access
        int status = _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SurfaceIndex, surfaceIndex);
        if (status != MultiCamNative.MC_OK)
        {
            return new SurfaceData { SurfaceIndex = surfaceIndex };
        }

        // Get surface address
        status = _hal.GetParamPtr(_channelHandle, MultiCamNative.PN_SurfaceAddr, out IntPtr address);
        if (status != MultiCamNative.MC_OK)
            address = IntPtr.Zero;

        return new SurfaceData
        {
            SurfaceIndex = surfaceIndex,
            Address = address,
            Pitch = _bufferPitch,
            Size = _bufferSize,
            Width = _imageWidth,
            Height = _imageHeight,
        };
    }

    private void ReleaseSurface(int surfaceIndex)
    {
        // Set surface state back to FREE
        int status = _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SurfaceIndex, surfaceIndex);
        if (status == MultiCamNative.MC_OK)
        {
            _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SurfaceState, (int)McSurfaceState.MC_SurfaceState_FREE);
        }
    }

    /// <summary>
    /// Refreshes cached image parameters from the channel.
    /// Call after changing image-related settings.
    /// </summary>
    public void RefreshImageParameters()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            _hal.GetParamInt(_channelHandle, MultiCamNative.PN_ImageSizeX, out _imageWidth);
            _hal.GetParamInt(_channelHandle, MultiCamNative.PN_ImageSizeY, out _imageHeight);
            _hal.GetParamInt(_channelHandle, MultiCamNative.PN_BufferPitch, out _bufferPitch);
            _hal.GetParamInt(_channelHandle, MultiCamNative.PN_BufferSize, out _bufferSize);
        }
    }

    #region Acquisition Control

    /// <summary>
    /// Starts continuous acquisition (infinite sequence length).
    /// </summary>
    public void StartAcquisition()
    {
        StartAcquisition(-1); // MC_SeqLength_CM_INFINITE
    }

    /// <summary>
    /// Starts acquisition for a specific number of frames.
    /// </summary>
    /// <param name="frameCount">Number of frames to acquire, or -1 for infinite</param>
    public void StartAcquisition(int frameCount)
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            if (_isActive)
                return;

            // Set sequence length
            int status = _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SeqLength_Fr, frameCount);
            ThrowOnError(status, $"SetParam(SeqLength_Fr={frameCount})");

            // Activate channel
            status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_ChannelState,
                MultiCamNative.MC_ChannelState_ACTIVE_STR);
            ThrowOnError(status, "SetParam(ChannelState=ACTIVE)");

            _isActive = true;
        }
    }

    /// <summary>
    /// Stops acquisition gracefully.
    /// </summary>
    public void StopAcquisition()
    {
        lock (_lock)
        {
            if (!_isActive || _disposed)
                return;

            // Set sequence length to 0 to stop
            _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SeqLength_Fr, 0);

            // Set channel state to IDLE
            _hal.SetParamStr(_channelHandle, MultiCamNative.PN_ChannelState,
                MultiCamNative.MC_ChannelState_IDLE_STR);

            _isActive = false;
        }
    }

    /// <summary>
    /// Sends a software trigger (when TrigMode is SOFT or COMBINED).
    /// </summary>
    public void SendSoftwareTrigger()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            int status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_ForceTrig,
                MultiCamNative.MC_ForceTrig_STR);
            ThrowOnError(status, "SetParam(ForceTrig=TRIG)");
        }
    }

    /// <summary>
    /// Waits for the next frame using polling (alternative to callback).
    /// </summary>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <returns>Surface data, or null if timeout</returns>
    public SurfaceData? WaitForFrame(uint timeoutMs = 5000)
    {
        ThrowIfDisposed();

        int status = _hal.WaitSignal(_channelHandle,
            (int)McSignal.MC_SIG_SURFACE_PROCESSING, timeoutMs, out McSignalInfo info);

        if (status == (int)McStatus.MC_TIMEOUT)
            return null;

        ThrowOnError(status, "McWaitSignal");

        int surfaceIndex = (int)info.SignalInfo;
        var surface = GetSurfaceData(surfaceIndex);

        // Note: Caller is responsible for calling ReleaseSurface when done
        return surface;
    }

    /// <summary>
    /// Releases a surface back to the acquisition cluster.
    /// Must be called after processing a frame obtained via WaitForFrame.
    /// </summary>
    public void ReleaseSurface(SurfaceData surface)
    {
        ReleaseSurface(surface.SurfaceIndex);
    }

    #endregion

    #region Signal Configuration

    private void SetSignalEnable(McSignal signal, bool enable)
    {
        string paramName = $"{signal}+{MultiCamNative.PN_SignalEnable}";
        string value = enable ? MultiCamNative.MC_SignalEnable_ON_STR : MultiCamNative.MC_SignalEnable_OFF_STR;

        _hal.SetParamStr(_channelHandle, paramName, value);
        // Don't throw - some signals may not be available on all boards
    }

    #endregion

    #region Parameter Access

    /// <summary>
    /// Gets an integer parameter value.
    /// </summary>
    public int GetParamInt(string paramName)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            int status = _hal.GetParamInt(_channelHandle, paramName, out int value);
            ThrowOnError(status, $"GetParam({paramName})");
            return value;
        }
    }

    /// <summary>
    /// Gets a floating-point parameter value.
    /// </summary>
    public double GetParamFloat(string paramName)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            int status = _hal.GetParamFloat(_channelHandle, paramName, out double value);
            ThrowOnError(status, $"GetParam({paramName})");
            return value;
        }
    }

    /// <summary>
    /// Gets a string parameter value.
    /// </summary>
    public string GetParamStr(string paramName)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            int status = _hal.GetParamStr(_channelHandle, paramName, out string value);
            ThrowOnError(status, $"GetParam({paramName})");
            return value;
        }
    }

    /// <summary>
    /// Sets an integer parameter value.
    /// </summary>
    public void SetParamInt(string paramName, int value)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            int status = _hal.SetParamInt(_channelHandle, paramName, value);
            ThrowOnError(status, $"SetParam({paramName}={value})");
        }
    }

    /// <summary>
    /// Sets a floating-point parameter value.
    /// </summary>
    public void SetParamFloat(string paramName, double value)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            int status = _hal.SetParamFloat(_channelHandle, paramName, value);
            ThrowOnError(status, $"SetParam({paramName}={value})");
        }
    }

    /// <summary>
    /// Sets a string parameter value.
    /// </summary>
    public void SetParamStr(string paramName, string value)
    {
        lock (_lock)
        {
            ThrowIfDisposed();
            int status = _hal.SetParamStr(_channelHandle, paramName, value);
            ThrowOnError(status, $"SetParam({paramName}={value})");
        }
    }

    #endregion

    #region Calibration

    /// <summary>
    /// Performs black calibration (dark frame correction).
    /// IMPORTANT: Cover the lens or ensure no light reaches the sensor before calling.
    /// </summary>
    public void PerformBlackCalibration()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            int status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_BlackCalibration, "ON");
            ThrowOnError(status, "BlackCalibration");
        }
    }

    /// <summary>
    /// Performs white calibration (gain correction).
    /// IMPORTANT: Illuminate the sensor with uniform ~200DN lighting before calling.
    /// </summary>
    public void PerformWhiteCalibration()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            int status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_WhiteCalibration, "ON");
            ThrowOnError(status, "WhiteCalibration");
        }
    }

    /// <summary>
    /// Enables or disables flat field correction (requires prior black/white calibration).
    /// </summary>
    public void SetFlatFieldCorrection(bool enable)
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            string value = enable ? "ON" : "OFF";
            int status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_FlatFieldCorrection, value);
            ThrowOnError(status, $"SetFlatFieldCorrection({value})");
        }
    }

    /// <summary>
    /// Performs automatic white balance once.
    /// </summary>
    public void PerformWhiteBalanceOnce()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            int status = _hal.SetParamStr(_channelHandle, MultiCamNative.PN_BalanceWhiteAuto, "ONCE");
            ThrowOnError(status, "BalanceWhiteAuto=ONCE");
        }
    }

    /// <summary>
    /// Gets the current white balance ratios.
    /// </summary>
    public (double Red, double Green, double Blue) GetWhiteBalanceRatios()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            _hal.GetParamFloat(_channelHandle, MultiCamNative.PN_BalanceRatioRed, out double red);
            _hal.GetParamFloat(_channelHandle, MultiCamNative.PN_BalanceRatioGreen, out double green);
            _hal.GetParamFloat(_channelHandle, MultiCamNative.PN_BalanceRatioBlue, out double blue);

            return (red, green, blue);
        }
    }

    /// <summary>
    /// Sets the white balance ratios manually.
    /// </summary>
    public void SetWhiteBalanceRatios(double red, double green, double blue)
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            _hal.SetParamFloat(_channelHandle, MultiCamNative.PN_BalanceRatioRed, red);
            _hal.SetParamFloat(_channelHandle, MultiCamNative.PN_BalanceRatioGreen, green);
            _hal.SetParamFloat(_channelHandle, MultiCamNative.PN_BalanceRatioBlue, blue);
        }
    }

    #endregion

    #region Exposure Control

    /// <summary>
    /// Gets the current exposure time in microseconds.
    /// </summary>
    public double GetExposureUs()
    {
        return GetParamFloat(MultiCamNative.PN_Expose_us);
    }

    /// <summary>
    /// Sets the exposure time in microseconds.
    /// </summary>
    public void SetExposureUs(double exposureUs)
    {
        SetParamFloat(MultiCamNative.PN_Expose_us, exposureUs);
    }

    /// <summary>
    /// Gets the exposure range in microseconds.
    /// </summary>
    public (double Min, double Max) GetExposureRange()
    {
        double min = GetParamFloat(MultiCamNative.PN_ExposeMin_us);
        double max = GetParamFloat(MultiCamNative.PN_ExposeMax_us);
        return (min, max);
    }

    /// <summary>
    /// Gets the current gain in dB.
    /// </summary>
    public double GetGainDb()
    {
        return GetParamFloat(MultiCamNative.PN_Gain_dB);
    }

    /// <summary>
    /// Sets the gain in dB.
    /// </summary>
    public void SetGainDb(double gainDb)
    {
        SetParamFloat(MultiCamNative.PN_Gain_dB, gainDb);
    }

    #endregion

    #region Error Handling

    private void ThrowOnError(int status, string operation)
    {
        if (status != MultiCamNative.MC_OK)
        {
            throw new MultiCamException(status, operation);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(GrabChannel));
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;

            // Stop acquisition if active
            if (_isActive)
            {
                try
                {
                    _hal.SetParamInt(_channelHandle, MultiCamNative.PN_SeqLength_Fr, 0);
                    _hal.SetParamStr(_channelHandle, MultiCamNative.PN_ChannelState,
                        MultiCamNative.MC_ChannelState_IDLE_STR);
                }
                catch { /* Ignore cleanup errors */ }
                _isActive = false;
            }

            // Delete channel
            if (_channelHandle != 0)
            {
                _hal.Delete(_channelHandle);
                _channelHandle = 0;
            }

            // Free GC handles
            if (_callbackHandle.IsAllocated)
                _callbackHandle.Free();

            if (_thisHandle.IsAllocated)
                _thisHandle.Free();

            _nativeCallback = null;
        }
    }

    #endregion
}
