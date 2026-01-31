using System.Collections.Concurrent;

namespace PeanutVision.MultiCamDriver.Hal;

/// <summary>
/// Mock implementation of IMultiCamHal for unit testing.
/// Simulates MultiCam hardware behavior without requiring actual hardware.
/// </summary>
public class MockMultiCamHAL : IMultiCamHAL
{
    private readonly object _lock = new();
    private bool _driverOpen;
    private uint _nextInstanceId = 1;

    // Track created instances
    private readonly ConcurrentDictionary<uint, MockInstance> _instances = new();

    // Configuration for simulating different scenarios
    public MockHalConfiguration Configuration { get; } = new();

    // Tracking for verification in tests
    public MockHalCallLog CallLog { get; } = new();

    /// <summary>
    /// Represents a mock instance (channel, surface, etc.)
    /// </summary>
    private class MockInstance
    {
        public uint Handle { get; init; }
        public uint Model { get; init; }
        public string? ModelName { get; init; }
        public ConcurrentDictionary<string, object> Parameters { get; } = new();
        public bool CallbackRegistered { get; set; }
        public IntPtr CallbackPtr { get; set; }
        public IntPtr CallbackContext { get; set; }
    }

    #region Driver Lifecycle

    public int OpenDriver(string? driverName)
    {
        CallLog.OpenDriverCalls++;

        if (Configuration.OpenDriverFailure.HasValue)
            return Configuration.OpenDriverFailure.Value;

        lock (_lock)
        {
            _driverOpen = true;
        }

        return MultiCamApi.MC_OK;
    }

    public int CloseDriver()
    {
        CallLog.CloseDriverCalls++;

        lock (_lock)
        {
            _driverOpen = false;
        }

        return MultiCamApi.MC_OK;
    }

    #endregion

    #region Instance Management

    public int Create(uint model, out uint instance)
    {
        CallLog.CreateCalls++;

        if (Configuration.CreateFailure.HasValue)
        {
            instance = 0;
            return Configuration.CreateFailure.Value;
        }

        lock (_lock)
        {
            instance = _nextInstanceId++;
            var mockInstance = new MockInstance
            {
                Handle = instance,
                Model = model
            };

            // Set default parameters based on model
            InitializeInstanceDefaults(mockInstance);

            _instances[instance] = mockInstance;
        }

        return MultiCamApi.MC_OK;
    }

    public int CreateNm(string modelName, out uint instance)
    {
        CallLog.CreateNmCalls++;

        if (Configuration.CreateFailure.HasValue)
        {
            instance = 0;
            return Configuration.CreateFailure.Value;
        }

        lock (_lock)
        {
            instance = _nextInstanceId++;
            var mockInstance = new MockInstance
            {
                Handle = instance,
                ModelName = modelName
            };

            InitializeInstanceDefaults(mockInstance);
            _instances[instance] = mockInstance;
        }

        return MultiCamApi.MC_OK;
    }

    public int Delete(uint instance)
    {
        CallLog.DeleteCalls++;

        if (!_instances.TryRemove(instance, out _))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        return MultiCamApi.MC_OK;
    }

    private void InitializeInstanceDefaults(MockInstance instance)
    {
        // Set default image parameters
        instance.Parameters[MultiCamApi.PN_ImageSizeX] = Configuration.DefaultImageWidth;
        instance.Parameters[MultiCamApi.PN_ImageSizeY] = Configuration.DefaultImageHeight;
        instance.Parameters[MultiCamApi.PN_BufferPitch] = Configuration.DefaultImageWidth * 3; // RGB
        instance.Parameters[MultiCamApi.PN_BufferSize] = Configuration.DefaultImageWidth * Configuration.DefaultImageHeight * 3;
        instance.Parameters[MultiCamApi.PN_SurfaceCount] = 4;
        instance.Parameters[MultiCamApi.PN_ChannelState] = MultiCamApi.MC_ChannelState_IDLE_STR;
        instance.Parameters[MultiCamApi.PN_TrigMode] = MultiCamApi.MC_TrigMode_IMMEDIATE_STR;

        // Calibration defaults
        instance.Parameters[MultiCamApi.PN_BalanceRatioRed] = 1.0;
        instance.Parameters[MultiCamApi.PN_BalanceRatioGreen] = 1.0;
        instance.Parameters[MultiCamApi.PN_BalanceRatioBlue] = 1.0;

        // Exposure defaults
        instance.Parameters[MultiCamApi.PN_Expose_us] = 10000.0;
        instance.Parameters[MultiCamApi.PN_ExposeMin_us] = 10.0;
        instance.Parameters[MultiCamApi.PN_ExposeMax_us] = 1000000.0;
        instance.Parameters[MultiCamApi.PN_Gain_dB] = 0.0;
    }

    #endregion

    #region Parameter Access - Get

    public int GetParamInt(uint instance, string paramName, out int value)
    {
        CallLog.GetParamCalls++;

        // Handle configuration object
        if (instance == MultiCamApi.MC_CONFIGURATION)
        {
            return GetConfigurationParam(paramName, out value);
        }

        // Handle board objects
        if (instance >= MultiCamApi.MC_BOARD && instance < MultiCamApi.MC_BOARD + 100)
        {
            value = 0;
            return MultiCamApi.MC_OK;
        }

        if (!_instances.TryGetValue(instance, out var inst))
        {
            value = 0;
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        if (inst.Parameters.TryGetValue(paramName, out var obj) && obj is int intValue)
        {
            value = intValue;
            return MultiCamApi.MC_OK;
        }

        value = 0;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    public int GetParamInt64(uint instance, string paramName, out long value)
    {
        CallLog.GetParamCalls++;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            value = 0;
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        if (inst.Parameters.TryGetValue(paramName, out var obj) && obj is long longValue)
        {
            value = longValue;
            return MultiCamApi.MC_OK;
        }

        value = 0;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    public int GetParamFloat(uint instance, string paramName, out double value)
    {
        CallLog.GetParamCalls++;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            value = 0;
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        if (inst.Parameters.TryGetValue(paramName, out var obj) && obj is double doubleValue)
        {
            value = doubleValue;
            return MultiCamApi.MC_OK;
        }

        value = 0;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    public int GetParamStr(uint instance, string paramName, out string value)
    {
        CallLog.GetParamCalls++;

        // Handle configuration object
        if (instance == MultiCamApi.MC_CONFIGURATION)
        {
            return GetConfigurationParamStr(paramName, out value);
        }

        // Handle board objects
        if (instance >= MultiCamApi.MC_BOARD && instance < MultiCamApi.MC_BOARD + 100)
        {
            return GetBoardParamStr(instance, paramName, out value);
        }

        if (!_instances.TryGetValue(instance, out var inst))
        {
            value = string.Empty;
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        if (inst.Parameters.TryGetValue(paramName, out var obj) && obj is string strValue)
        {
            value = strValue;
            return MultiCamApi.MC_OK;
        }

        value = string.Empty;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    public int GetParamPtr(uint instance, string paramName, out IntPtr value)
    {
        CallLog.GetParamCalls++;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            value = IntPtr.Zero;
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        // For surface address, return a simulated address
        if (paramName == MultiCamApi.PN_SurfaceAddr)
        {
            value = Configuration.SimulatedSurfaceAddress;
            return MultiCamApi.MC_OK;
        }

        if (inst.Parameters.TryGetValue(paramName, out var obj) && obj is IntPtr ptrValue)
        {
            value = ptrValue;
            return MultiCamApi.MC_OK;
        }

        value = IntPtr.Zero;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    private int GetConfigurationParam(string paramName, out int value)
    {
        if (paramName == MultiCamApi.PN_BoardCount)
        {
            value = Configuration.BoardCount;
            return MultiCamApi.MC_OK;
        }

        value = 0;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    private int GetConfigurationParamStr(string paramName, out string value)
    {
        if (paramName == MultiCamApi.PN_DriverVersion)
        {
            value = Configuration.DriverVersion;
            return MultiCamApi.MC_OK;
        }

        value = string.Empty;
        return (int)McStatus.MC_INVALID_PARAM;
    }

    private int GetBoardParamStr(uint boardHandle, string paramName, out string value)
    {
        int boardIndex = (int)(boardHandle - MultiCamApi.MC_BOARD);

        if (boardIndex < 0 || boardIndex >= Configuration.BoardCount)
        {
            value = string.Empty;
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        value = paramName switch
        {
            MultiCamApi.PN_BoardName => Configuration.BoardNames.ElementAtOrDefault(boardIndex) ?? "MockBoard",
            MultiCamApi.PN_BoardType => Configuration.BoardTypes.ElementAtOrDefault(boardIndex) ?? "MockType",
            MultiCamApi.PN_SerialNumber => $"MOCK{boardIndex:D4}",
            MultiCamApi.PN_PCIPosition => $"0:{boardIndex}:0",
            _ => string.Empty
        };

        return MultiCamApi.MC_OK;
    }

    #endregion

    #region Parameter Access - Set

    public int SetParamInt(uint instance, string paramName, int value)
    {
        CallLog.SetParamCalls++;
        CallLog.LastSetParams[paramName] = value;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        inst.Parameters[paramName] = value;
        return MultiCamApi.MC_OK;
    }

    public int SetParamInt64(uint instance, string paramName, long value)
    {
        CallLog.SetParamCalls++;
        CallLog.LastSetParams[paramName] = value;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        inst.Parameters[paramName] = value;
        return MultiCamApi.MC_OK;
    }

    public int SetParamFloat(uint instance, string paramName, double value)
    {
        CallLog.SetParamCalls++;
        CallLog.LastSetParams[paramName] = value;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        inst.Parameters[paramName] = value;
        return MultiCamApi.MC_OK;
    }

    public int SetParamStr(uint instance, string paramName, string value)
    {
        CallLog.SetParamCalls++;
        CallLog.LastSetParams[paramName] = value;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        // Handle special parameters
        if (paramName == MultiCamApi.PN_ChannelState)
        {
            if (value == MultiCamApi.MC_ChannelState_ACTIVE_STR)
            {
                CallLog.AcquisitionStarted = true;
            }
            else if (value == MultiCamApi.MC_ChannelState_IDLE_STR)
            {
                CallLog.AcquisitionStopped = true;
            }
        }

        if (paramName == MultiCamApi.PN_ForceTrig)
        {
            CallLog.SoftwareTriggerCount++;
        }

        if (paramName == MultiCamApi.PN_BlackCalibration && value == "ON")
        {
            CallLog.BlackCalibrationPerformed = true;
        }

        if (paramName == MultiCamApi.PN_WhiteCalibration && value == "ON")
        {
            CallLog.WhiteCalibrationPerformed = true;
        }

        if (paramName == MultiCamApi.PN_BalanceWhiteAuto && value == "ONCE")
        {
            CallLog.WhiteBalancePerformed = true;
            // Simulate auto white balance adjustment
            inst.Parameters[MultiCamApi.PN_BalanceRatioRed] = 1.05;
            inst.Parameters[MultiCamApi.PN_BalanceRatioGreen] = 1.0;
            inst.Parameters[MultiCamApi.PN_BalanceRatioBlue] = 0.95;
        }

        inst.Parameters[paramName] = value;
        return MultiCamApi.MC_OK;
    }

    public int SetParamPtr(uint instance, string paramName, IntPtr value)
    {
        CallLog.SetParamCalls++;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        inst.Parameters[paramName] = value;
        return MultiCamApi.MC_OK;
    }

    #endregion

    #region Signaling

    public int RegisterCallback(uint instance, IntPtr callbackPtr, IntPtr context)
    {
        CallLog.RegisterCallbackCalls++;

        if (!_instances.TryGetValue(instance, out var inst))
        {
            return (int)McStatus.MC_INVALID_HANDLE;
        }

        inst.CallbackRegistered = true;
        inst.CallbackPtr = callbackPtr;
        inst.CallbackContext = context;

        return MultiCamApi.MC_OK;
    }

    public int WaitSignal(uint instance, int signal, uint timeout, out McSignalInfo info)
    {
        CallLog.WaitSignalCalls++;

        if (Configuration.WaitSignalFailure.HasValue)
        {
            info = default;
            return Configuration.WaitSignalFailure.Value;
        }

        // Simulate a successful signal
        info = new McSignalInfo
        {
            Instance = instance,
            Signal = signal == (int)McSignal.MC_SIG_ANY ? (int)McSignal.MC_SIG_SURFACE_PROCESSING : signal,
            SignalInfo = 0, // Surface index 0
            Context = IntPtr.Zero
        };

        return MultiCamApi.MC_OK;
    }

    #endregion

    #region Test Helpers

    /// <summary>
    /// Simulates a frame acquisition by invoking the registered callback.
    /// Useful for testing callback-based acquisition.
    /// </summary>
    public void SimulateFrameAcquisition(uint channelHandle, int surfaceIndex = 0)
    {
        if (!_instances.TryGetValue(channelHandle, out var inst) || !inst.CallbackRegistered)
            return;

        var signalInfo = new McSignalInfo
        {
            Instance = channelHandle,
            Signal = (int)McSignal.MC_SIG_SURFACE_PROCESSING,
            SignalInfo = (uint)surfaceIndex,
            Context = inst.CallbackContext
        };

        // Invoke the callback
        var callback = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<McCallback>(inst.CallbackPtr);
        callback(ref signalInfo);
    }

    /// <summary>
    /// Simulates an acquisition error.
    /// </summary>
    public void SimulateAcquisitionError(uint channelHandle, McSignal errorSignal)
    {
        if (!_instances.TryGetValue(channelHandle, out var inst) || !inst.CallbackRegistered)
            return;

        var signalInfo = new McSignalInfo
        {
            Instance = channelHandle,
            Signal = (int)errorSignal,
            SignalInfo = 0,
            Context = inst.CallbackContext
        };

        var callback = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<McCallback>(inst.CallbackPtr);
        callback(ref signalInfo);
    }

    /// <summary>
    /// Resets the mock to initial state.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            _driverOpen = false;
            _nextInstanceId = 1;
            _instances.Clear();
            CallLog.Reset();
        }
    }

    #endregion
}

/// <summary>
/// Configuration options for MockMultiCamHal behavior.
/// </summary>
public class MockHalConfiguration
{
    /// <summary>Number of simulated boards.</summary>
    public int BoardCount { get; set; } = 1;

    /// <summary>Simulated driver version.</summary>
    public string DriverVersion { get; set; } = "6.19.4.4059 (Mock)";

    /// <summary>Board names for each simulated board.</summary>
    public List<string> BoardNames { get; set; } = new() { "Grablink Full" };

    /// <summary>Board types for each simulated board.</summary>
    public List<string> BoardTypes { get; set; } = new() { "PC1622" };

    /// <summary>Default image width for created channels.</summary>
    public int DefaultImageWidth { get; set; } = 1920;

    /// <summary>Default image height for created channels.</summary>
    public int DefaultImageHeight { get; set; } = 1080;

    /// <summary>Simulated surface memory address.</summary>
    public IntPtr SimulatedSurfaceAddress { get; set; } = new IntPtr(0x10000000);

    /// <summary>If set, OpenDriver will return this error code.</summary>
    public int? OpenDriverFailure { get; set; }

    /// <summary>If set, Create/CreateNm will return this error code.</summary>
    public int? CreateFailure { get; set; }

    /// <summary>If set, WaitSignal will return this error code.</summary>
    public int? WaitSignalFailure { get; set; }
}

/// <summary>
/// Tracks calls made to the MockMultiCamHal for test verification.
/// </summary>
public class MockHalCallLog
{
    public int OpenDriverCalls { get; set; }
    public int CloseDriverCalls { get; set; }
    public int CreateCalls { get; set; }
    public int CreateNmCalls { get; set; }
    public int DeleteCalls { get; set; }
    public int GetParamCalls { get; set; }
    public int SetParamCalls { get; set; }
    public int RegisterCallbackCalls { get; set; }
    public int WaitSignalCalls { get; set; }

    public bool AcquisitionStarted { get; set; }
    public bool AcquisitionStopped { get; set; }
    public int SoftwareTriggerCount { get; set; }
    public bool BlackCalibrationPerformed { get; set; }
    public bool WhiteCalibrationPerformed { get; set; }
    public bool WhiteBalancePerformed { get; set; }

    public ConcurrentDictionary<string, object> LastSetParams { get; } = new();

    public void Reset()
    {
        OpenDriverCalls = 0;
        CloseDriverCalls = 0;
        CreateCalls = 0;
        CreateNmCalls = 0;
        DeleteCalls = 0;
        GetParamCalls = 0;
        SetParamCalls = 0;
        RegisterCallbackCalls = 0;
        WaitSignalCalls = 0;
        AcquisitionStarted = false;
        AcquisitionStopped = false;
        SoftwareTriggerCount = 0;
        BlackCalibrationPerformed = false;
        WhiteCalibrationPerformed = false;
        WhiteBalancePerformed = false;
        LastSetParams.Clear();
    }
}
