namespace PeanutVision.MultiCamDriver.Hal;

/// <summary>
/// Hardware Abstraction Layer interface for MultiCam operations.
/// Enables unit testing without actual hardware by allowing mock implementations.
/// </summary>
public interface IMultiCamHAL
{
    #region Driver Lifecycle

    /// <summary>Opens connection to the MultiCam driver.</summary>
    int OpenDriver(string? driverName);

    /// <summary>Closes connection to the MultiCam driver.</summary>
    int CloseDriver();

    #endregion

    #region Instance Management

    /// <summary>Creates a new MultiCam object instance.</summary>
    int Create(uint model, out uint instance);

    /// <summary>Creates a new MultiCam object instance from a model name.</summary>
    int CreateNm(string modelName, out uint instance);

    /// <summary>Deletes a MultiCam object instance.</summary>
    int Delete(uint instance);

    #endregion

    #region Parameter Access - Get

    int GetParamInt(uint instance, string paramName, out int value);
    int GetParamInt64(uint instance, string paramName, out long value);
    int GetParamFloat(uint instance, string paramName, out double value);
    int GetParamStr(uint instance, string paramName, out string value);
    int GetParamPtr(uint instance, string paramName, out IntPtr value);

    #endregion

    #region Parameter Access - Set (by Name)

    int SetParamInt(uint instance, string paramName, int value);
    int SetParamInt64(uint instance, string paramName, long value);

    #endregion

    #region Parameter Access - Set (by ID)

    /// <summary>
    /// Sets an integer parameter using the parameter ID.
    /// Used for compound parameters like MC_SignalEnable + signal_id.
    /// </summary>
    int SetParamIntById(uint instance, uint paramId, int value);
    int SetParamFloat(uint instance, string paramName, double value);
    int SetParamStr(uint instance, string paramName, string value);
    int SetParamPtr(uint instance, string paramName, IntPtr value);

    #endregion

    #region Signaling

    /// <summary>Registers a callback for signal notifications.</summary>
    int RegisterCallback(uint instance, IntPtr callbackPtr, IntPtr context);

    /// <summary>Waits for a signal from the specified instance.</summary>
    int WaitSignal(uint instance, int signal, uint timeout, out McSignalInfo info);

    #endregion
}

/// <summary>
/// Extension methods for IMultiCamHal to provide convenient typed access.
/// </summary>
public static class MultiCamHalExtensions
{
    /// <summary>
    /// Gets an integer parameter, throwing on error.
    /// </summary>
    public static int GetParamIntOrThrow(this IMultiCamHAL hal, uint instance, string paramName)
    {
        int status = hal.GetParamInt(instance, paramName, out int value);
        if (status != MultiCamApi.MC_OK)
            throw new MultiCamException(status, $"GetParam({paramName})");
        return value;
    }

    /// <summary>
    /// Gets a float parameter, throwing on error.
    /// </summary>
    public static double GetParamFloatOrThrow(this IMultiCamHAL hal, uint instance, string paramName)
    {
        int status = hal.GetParamFloat(instance, paramName, out double value);
        if (status != MultiCamApi.MC_OK)
            throw new MultiCamException(status, $"GetParam({paramName})");
        return value;
    }

    /// <summary>
    /// Gets a string parameter, throwing on error.
    /// </summary>
    public static string GetParamStrOrThrow(this IMultiCamHAL hal, uint instance, string paramName)
    {
        int status = hal.GetParamStr(instance, paramName, out string value);
        if (status != MultiCamApi.MC_OK)
            throw new MultiCamException(status, $"GetParam({paramName})");
        return value;
    }

    /// <summary>
    /// Sets an integer parameter, throwing on error.
    /// </summary>
    public static void SetParamIntOrThrow(this IMultiCamHAL hal, uint instance, string paramName, int value)
    {
        int status = hal.SetParamInt(instance, paramName, value);
        if (status != MultiCamApi.MC_OK)
            throw new MultiCamException(status, $"SetParam({paramName}={value})");
    }

    /// <summary>
    /// Sets a float parameter, throwing on error.
    /// </summary>
    public static void SetParamFloatOrThrow(this IMultiCamHAL hal, uint instance, string paramName, double value)
    {
        int status = hal.SetParamFloat(instance, paramName, value);
        if (status != MultiCamApi.MC_OK)
            throw new MultiCamException(status, $"SetParam({paramName}={value})");
    }

    /// <summary>
    /// Sets a string parameter, throwing on error.
    /// </summary>
    public static void SetParamStrOrThrow(this IMultiCamHAL hal, uint instance, string paramName, string value)
    {
        int status = hal.SetParamStr(instance, paramName, value);
        if (status != MultiCamApi.MC_OK)
            throw new MultiCamException(status, $"SetParam({paramName}={value})");
    }
}
