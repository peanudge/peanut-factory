using System.Runtime.InteropServices;
using System.Threading.Channels;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;

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

    /// <summary>Acquisition mode (SNAPSHOT, VIDEO, HFR). Must match the camera type — never use PAGE/WEB/LONGPAGE with TC-A160K area-scan cameras.</summary>
    public McAcquisitionMode AcquisitionMode { get; set; } = McAcquisitionMode.MC_AcquisitionMode_SNAPSHOT;
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
    private volatile bool _disposing;
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
    private int _clusterUnavailableCount;
    private McTrigMode _triggerMode;

    // Internal signal processing thread (only active when UseCallback=true)
    // All signals from the native callback are enqueued here — no event is ever
    // fired directly on the MultiCam thread, guaranteeing < 1ms callback time.
    private volatile Channel<CallbackSignal>? _signalQueue;
    private Task? _processingTask;
    private CancellationTokenSource? _processingCts;
    private int _copyDropCount;

    /// <summary>Number of times cluster unavailable signal was received (frame drops)</summary>
    public int ClusterUnavailableCount => _clusterUnavailableCount;

    /// <summary>Number of frames dropped because the internal copy queue was full</summary>
    public int CopyDropCount => _copyDropCount;

    /// <summary>Channel handle for direct native API access if needed</summary>
    public uint Handle => _channelHandle;

    /// <summary>Whether the channel is currently acquiring</summary>
    public bool IsActive => _isActive;

    /// <summary>Number of surfaces in the cluster</summary>
    public int SurfaceCount => _surfaceCount;

    /// <summary>Image width in pixels</summary>
    public int ImageWidth => _imageWidth;

    /// <summary>Image height in pixels</summary>
    public int ImageHeight => _imageHeight;

    /// <summary>Configured trigger mode for this channel</summary>
    public McTrigMode TriggerMode => _triggerMode;

    /// <summary>
    /// Whether this channel's trigger mode supports software triggering via SendSoftwareTrigger().
    /// True for SOFT and COMBINED modes; false for IMMEDIATE and HARD.
    /// </summary>
    public bool SupportsSoftwareTrigger =>
        _triggerMode == McTrigMode.MC_TrigMode_SOFT ||
        _triggerMode == McTrigMode.MC_TrigMode_COMBINED;

    /// <summary>
    /// Fired when a frame has been copied and is ready for processing.
    /// Called from the internal copy thread — the surface is already released.
    /// </summary>
    public event EventHandler<FrameAcquiredEventArgs>? FrameAcquired;

    /// <summary>
    /// Fired when an acquisition error occurs.
    /// Called from the internal processing thread, never from the native callback.
    /// </summary>
    public event EventHandler<AcquisitionErrorEventArgs>? AcquisitionError;

    /// <summary>
    /// Fired when acquisition ends (MC_SIG_END_CHANNEL_ACTIVITY).
    /// Called from the internal processing thread, never from the native callback.
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
        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        ThrowOnError(status, "McCreate(MC_CHANNEL)");

        try
        {
            // Set driver index (board selection)
            status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, options.DriverIndex);
            ThrowOnError(status, $"SetParam(DriverIndex={options.DriverIndex})");

            // Set connector
            status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, options.Connector);
            ThrowOnError(status, $"SetParam(Connector={options.Connector})");

            // Load .cam file if specified
            if (!string.IsNullOrEmpty(options.CamFilePath))
            {
                status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, options.CamFilePath);
                ThrowOnError(status, $"SetParam(CamFile={options.CamFilePath})");
            }

            // Configure surface cluster
            status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, options.SurfaceCount);
            ThrowOnError(status, $"SetParam(SurfaceCount={options.SurfaceCount})");

            _surfaceCount = options.SurfaceCount;

            // Set trigger mode
            string trigModeStr = options.TriggerMode switch
            {
                McTrigMode.MC_TrigMode_IMMEDIATE => MultiCamApi.MC_TrigMode_IMMEDIATE_STR,
                McTrigMode.MC_TrigMode_HARD => MultiCamApi.MC_TrigMode_HARD_STR,
                McTrigMode.MC_TrigMode_SOFT => MultiCamApi.MC_TrigMode_SOFT_STR,
                McTrigMode.MC_TrigMode_COMBINED => MultiCamApi.MC_TrigMode_COMBINED_STR,
                _ => MultiCamApi.MC_TrigMode_IMMEDIATE_STR
            };
            status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_TrigMode, trigModeStr);
            ThrowOnError(status, $"SetParam(TrigMode={trigModeStr})");
            _triggerMode = options.TriggerMode;

            // Set acquisition mode explicitly — never rely on cam file defaults
            string acqModeStr = options.AcquisitionMode switch
            {
                McAcquisitionMode.MC_AcquisitionMode_SNAPSHOT => MultiCamApi.MC_AcquisitionMode_SNAPSHOT_STR,
                McAcquisitionMode.MC_AcquisitionMode_VIDEO => MultiCamApi.MC_AcquisitionMode_VIDEO_STR,
                McAcquisitionMode.MC_AcquisitionMode_HFR => MultiCamApi.MC_AcquisitionMode_HFR_STR,
                _ => throw new ArgumentException($"Unsupported acquisition mode for area-scan camera: {options.AcquisitionMode}")
            };
            status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_AcquisitionMode, acqModeStr);
            ThrowOnError(status, $"SetParam(AcquisitionMode={acqModeStr})");

            // Enable surface processing signal
            SetSignalEnable(McSignal.MC_SIG_SURFACE_PROCESSING, true);
            SetSignalEnable(McSignal.MC_SIG_ACQUISITION_FAILURE, true);
            SetSignalEnable(McSignal.MC_SIG_END_CHANNEL_ACTIVITY, true);
            SetSignalEnable(McSignal.MC_SIG_UNRECOVERABLE_OVERRUN, true);
            SetSignalEnable(McSignal.MC_SIG_CLUSTER_UNAVAILABLE, true);

            // Register callback if requested
            if (options.UseCallback)
            {
                RegisterCallback();
                InitializeProcessingThread();
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

        GrabChannel? channel;
        try
        {
            GCHandle handle = GCHandle.FromIntPtr(info.Context);
            channel = handle.Target as GrabChannel;
        }
        catch (InvalidOperationException)
        {
            // GCHandle was freed during disposal — expected, not a bug
            return;
        }

        // Check for disposal BEFORE processing — volatile read ensures visibility
        if (channel == null || channel._disposing)
            return;

        channel.ProcessSignal(ref info);
    }

    /// <summary>
    /// Routes all signals to the background queue. Never fires events directly.
    /// Called from the MultiCam native thread — must complete in &lt; 1ms.
    /// </summary>
    internal void ProcessSignal(ref McSignalInfo info)
    {
        McSignal signal = (McSignal)info.Signal;

        switch (signal)
        {
            case McSignal.MC_SIG_SURFACE_PROCESSING:
                EnqueueSurface(info.SignalInfo);
                break;

            case McSignal.MC_SIG_UNRECOVERABLE_OVERRUN:
                _isActive = false;
                EnqueueSignal(new CallbackSignal.Error(
                    signal, info.Instance, info.SignalInfo, "Unrecoverable error - acquisition stopped"));
                break;

            case McSignal.MC_SIG_CLUSTER_UNAVAILABLE:
                Interlocked.Increment(ref _clusterUnavailableCount);
                EnqueueSignal(new CallbackSignal.Error(
                    signal, info.Instance, info.SignalInfo,
                    "Surface cluster unavailable - all surfaces busy, frame dropped"));
                break;

            case McSignal.MC_SIG_ACQUISITION_FAILURE:
                EnqueueSignal(new CallbackSignal.Error(
                    signal, info.Instance, info.SignalInfo, "Acquisition failure detected"));
                break;

            case McSignal.MC_SIG_END_CHANNEL_ACTIVITY:
                _isActive = false;
                EnqueueSignal(CallbackSignal.EndActivity.Instance);
                break;
        }
    }

    private void EnqueueSurface(uint surfaceHandle)
    {
        if (_signalQueue != null &&
            _signalQueue.Writer.TryWrite(new CallbackSignal.SurfaceReady(surfaceHandle)))
        {
            return;
        }

        Interlocked.Increment(ref _copyDropCount);
        ReleaseSurface(surfaceHandle);
    }

    private void EnqueueSignal(CallbackSignal signal)
    {
        _signalQueue?.Writer.TryWrite(signal);
    }

    private void InitializeProcessingThread()
    {
        _signalQueue = Channel.CreateBounded<CallbackSignal>(
            new BoundedChannelOptions(_surfaceCount + 8) // extra room for error/end signals
            {
                SingleReader = true,
                SingleWriter = false, // native callback can deliver multiple signal types concurrently
            });
        _processingCts = new CancellationTokenSource();
        _processingTask = Task.Run(() => ProcessingLoopAsync(_processingCts.Token));
    }

    /// <summary>
    /// Background loop: processes all signals from the native callback thread.
    /// Surface signals: copy data → release surface → fire FrameAcquired.
    /// Error/end signals: fire AcquisitionError or AcquisitionEnded.
    /// </summary>
    private async Task ProcessingLoopAsync(CancellationToken ct)
    {
        if (_signalQueue == null) return;

        try
        {
            await foreach (var signal in _signalQueue.Reader.ReadAllAsync(ct))
            {
                try
                {
                    switch (signal)
                    {
                        case CallbackSignal.SurfaceReady surface:
                            ProcessSurfaceReady(surface.SurfaceHandle);
                            break;

                        case CallbackSignal.Error error:
                            AcquisitionError?.Invoke(this, new AcquisitionErrorEventArgs(
                                error.Signal, error.Instance, error.SignalInfo, error.Message));
                            break;

                        case CallbackSignal.EndActivity:
                            AcquisitionEnded?.Invoke(this, EventArgs.Empty);
                            break;
                    }
                }
                catch
                {
                    // Don't let subscriber exceptions kill the processing loop
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
    }

    private void ProcessSurfaceReady(uint surfaceHandle)
    {
        ImageData? image = null;
        try
        {
            var surface = GetSurfaceData(surfaceHandle);
            image = ImageData.FromSurface(surface);
        }
        catch
        {
            // Copy failed — still must release surface
        }
        finally
        {
            ReleaseSurface(surfaceHandle);
        }

        if (image != null)
        {
            FrameAcquired?.Invoke(this, new FrameAcquiredEventArgs(
                image, _channelHandle, McSignal.MC_SIG_SURFACE_PROCESSING));
        }
    }

    private SurfaceData GetSurfaceData(uint surfaceHandle)
    {
        // Read SurfaceAddr directly from the surface handle
        // (per Euresys official sample: MC.GetParam(surfaceHandle, "SurfaceAddr", out addr))
        int status = _hal.GetParamPtr(surfaceHandle, MultiCamApi.PN_SurfaceAddr, out IntPtr address);
        if (status != MultiCamApi.MC_OK)
        {
            address = IntPtr.Zero;
        }

        return new SurfaceData
        {
            SurfaceHandle = surfaceHandle,
            Address = address,
            Pitch = _bufferPitch,
            Size = _bufferSize,
            Width = _imageWidth,
            Height = _imageHeight,
        };
    }

    private void ReleaseSurface(uint surfaceHandle)
    {
        // Guard: if the channel has been deleted during Dispose, _channelHandle is 0 —
        // skip the HAL call to avoid use-after-free on the native handle.
        if (_channelHandle == 0) return;

        // Set SurfaceState back to FREE on the surface handle
        _hal.SetParamInt(surfaceHandle, MultiCamApi.PN_SurfaceState, (int)McSurfaceState.MC_SurfaceState_FREE);
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

            _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeX, out _imageWidth);
            _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeY, out _imageHeight);
            _hal.GetParamInt(_channelHandle, MultiCamApi.PN_BufferPitch, out _bufferPitch);
            _hal.GetParamInt(_channelHandle, MultiCamApi.PN_BufferSize, out _bufferSize);
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
    /// Starts acquisition with the specified frame count.
    /// May be called on a freshly created channel or on a channel that was previously stopped
    /// with <see cref="StopAcquisition"/> (channel reuse scenario).
    /// </summary>
    /// <param name="frameCount">Number of frames to acquire, or -1 for infinite</param>
    public void StartAcquisition(int frameCount)
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            if (_isActive)
                return;

            // Re-initialize processing thread if it was shut down (reuse scenario)
            if (_signalQueue == null && _nativeCallback != null)
            {
                InitializeProcessingThread();
            }

            // Set sequence length
            int status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SeqLength_Fr, frameCount);
            ThrowOnError(status, $"SetParam(SeqLength_Fr={frameCount})");

            // Check current channel state before activation to avoid hanging
            status = _hal.GetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, out string currentState);
            ThrowOnError(status, "GetParam(ChannelState)");

            if (currentState == MultiCamApi.MC_ChannelState_ACTIVE_STR)
            {
                throw new InvalidOperationException(
                    "Channel is already ACTIVE. Another application (e.g., MultiCam Studio) may be using this channel.");
            }

            // Activate channel
            status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState,
                MultiCamApi.MC_ChannelState_ACTIVE_STR);
            ThrowOnError(status, "SetParam(ChannelState=ACTIVE)");

            _isActive = true;
        }
    }

    /// <summary>
    /// Stops the current acquisition sequence but keeps the channel alive for reuse.
    /// After this call, <see cref="StartAcquisition(int)"/> may be called again with the same channel.
    /// To fully release resources, call <see cref="Dispose"/>.
    /// </summary>
    public void StopAcquisition()
    {
        lock (_lock)
        {
            if (!_isActive || _disposed)
                return;

            // Set sequence length to 0 to stop
            _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SeqLength_Fr, 0);

            // Set channel state to IDLE
            _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState,
                MultiCamApi.MC_ChannelState_IDLE_STR);

            _isActive = false;
        }

        // Shut down the processing thread so it can be re-initialized on the next StartAcquisition call.
        // This must be done outside the lock to avoid deadlocking with the processing loop.
        ShutdownProcessingThread();
    }

    /// <summary>
    /// Sends a software trigger (when TrigMode is SOFT or COMBINED).
    /// </summary>
    public void SendSoftwareTrigger()
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            int status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ForceTrig,
                MultiCamApi.MC_ForceTrig_STR);
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

        uint surfaceHandle = info.SignalInfo;
        var surface = GetSurfaceData(surfaceHandle);

        // Note: Caller is responsible for calling ReleaseSurface when done
        return surface;
    }

    /// <summary>
    /// Releases a surface back to the acquisition cluster.
    /// Must be called after processing a frame obtained via WaitForFrame.
    /// </summary>
    public void ReleaseSurface(SurfaceData surface)
    {
        ReleaseSurface(surface.SurfaceHandle);
    }

    #endregion

    #region Signal Configuration

    private void SetSignalEnable(McSignal signal, bool enable)
    {
        // MultiCam uses compound parameter IDs: MC_SignalEnable + signal_id
        uint compoundParamId = MultiCamApi.MC_SignalEnable + (uint)signal;
        int value = enable ? MultiCamApi.MC_SignalEnable_ON : MultiCamApi.MC_SignalEnable_OFF;

        _hal.SetParamIntById(_channelHandle, compoundParamId, value);
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

            int status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_BlackCalibration, MultiCamApi.MC_Calibration_Execute);
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

            int status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_WhiteCalibration, MultiCamApi.MC_Calibration_Execute);
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

            string value = enable ? MultiCamApi.MC_FlatFieldCorrection_ON : MultiCamApi.MC_FlatFieldCorrection_OFF;
            int status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_FlatFieldCorrection, value);
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

            int status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_BalanceWhiteAuto, "ONCE");
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

            _hal.GetParamFloat(_channelHandle, MultiCamApi.PN_BalanceRatioRed, out double red);
            _hal.GetParamFloat(_channelHandle, MultiCamApi.PN_BalanceRatioGreen, out double green);
            _hal.GetParamFloat(_channelHandle, MultiCamApi.PN_BalanceRatioBlue, out double blue);

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

            _hal.SetParamFloat(_channelHandle, MultiCamApi.PN_BalanceRatioRed, red);
            _hal.SetParamFloat(_channelHandle, MultiCamApi.PN_BalanceRatioGreen, green);
            _hal.SetParamFloat(_channelHandle, MultiCamApi.PN_BalanceRatioBlue, blue);
        }
    }

    #endregion

    #region Exposure Control

    /// <summary>
    /// Gets the current exposure time in microseconds.
    /// </summary>
    public double GetExposureUs()
    {
        return GetParamFloat(MultiCamApi.PN_Expose_us);
    }

    /// <summary>
    /// Sets the exposure time in microseconds.
    /// </summary>
    public void SetExposureUs(double exposureUs)
    {
        SetParamFloat(MultiCamApi.PN_Expose_us, exposureUs);
    }

    /// <summary>
    /// Gets the exposure range in microseconds.
    /// </summary>
    public (double Min, double Max) GetExposureRange()
    {
        double min = GetParamFloat(MultiCamApi.PN_ExposeMin_us);
        double max = GetParamFloat(MultiCamApi.PN_ExposeMax_us);
        return (min, max);
    }

    #endregion

    #region Error Handling

    private void ThrowOnError(int status, string operation)
    {
        if (status != MultiCamApi.MC_OK)
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

        // Signal callbacks to stop processing — set before acquiring lock
        // so any in-flight callback sees it immediately (volatile write)
        _disposing = true;

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
                    _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SeqLength_Fr, 0);
                    _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState,
                        MultiCamApi.MC_ChannelState_IDLE_STR);
                }
                catch { /* Ignore cleanup errors */ }
                _isActive = false;
            }
        }

        // Atomically capture and zero the channel handle while still under the previous lock's
        // visibility guarantee.  After this point, ReleaseSurface will see _channelHandle == 0
        // and skip any HAL call, eliminating the TOCTOU use-after-free window.
        uint capturedHandle;
        lock (_lock)
        {
            capturedHandle = _channelHandle;
            _channelHandle = 0; // zero BEFORE ShutdownProcessingThread drains the queue
        }

        // Shut down processing thread (outside lock to avoid deadlock).
        // ReleaseSurface calls during drain are now guarded by the _channelHandle == 0 check.
        ShutdownProcessingThread();

        // Delete the native channel using the captured handle.
        if (capturedHandle != 0)
        {
            _hal.Delete(capturedHandle);
        }

        lock (_lock)
        {
            // Free GC handles
            if (_callbackHandle.IsAllocated)
                _callbackHandle.Free();

            if (_thisHandle.IsAllocated)
                _thisHandle.Free();

            _nativeCallback = null;
        }
    }

    private void ShutdownProcessingThread()
    {
        _signalQueue?.Writer.TryComplete();
        _processingCts?.Cancel();

        if (_processingTask != null)
        {
            try { _processingTask.Wait(TimeSpan.FromSeconds(2)); }
            catch { /* Ignore timeout/cancellation */ }
        }

        // Drain any remaining signals — release surfaces, discard others
        if (_signalQueue != null)
        {
            while (_signalQueue.Reader.TryRead(out var signal))
            {
                if (signal is CallbackSignal.SurfaceReady surface)
                    ReleaseSurface(surface.SurfaceHandle);
            }
        }

        _processingCts?.Dispose();
        _processingCts = null;
        _signalQueue = null;
        _processingTask = null;
    }

    #endregion
}

/// <summary>
/// Internal signal types routed through the background processing thread.
/// All native callback signals are enqueued as one of these — no event is ever
/// fired directly on the MultiCam callback thread.
/// </summary>
internal abstract record CallbackSignal
{
    public sealed record SurfaceReady(uint SurfaceHandle) : CallbackSignal;
    public sealed record Error(McSignal Signal, uint Instance, uint SignalInfo, string Message) : CallbackSignal;

    public sealed record EndActivity : CallbackSignal
    {
        public static readonly EndActivity Instance = new();
    }
}
