using System.Text;

namespace PeanutVision.MultiCamDriver.Hal;

/// <summary>
/// Real implementation of IMultiCamHal that wraps the native MultiCam P/Invoke calls.
/// This is the production implementation used when actual hardware is present.
/// </summary>
public sealed class MultiCamHAL : IMultiCamHAL
{
    /// <summary>
    /// Singleton instance for convenience.
    /// Thread-safe due to static initialization.
    /// </summary>
    public static readonly MultiCamHAL Instance = new();

    private const int MaxStringLength = 512;

    #region Driver Lifecycle

    public int OpenDriver(string? driverName)
    {
        return MultiCamNative.McOpenDriver(driverName);
    }

    public int CloseDriver()
    {
        return MultiCamNative.McCloseDriver();
    }

    #endregion

    #region Instance Management

    public int Create(uint model, out uint instance)
    {
        return MultiCamNative.McCreate(model, out instance);
    }

    public int CreateNm(string modelName, out uint instance)
    {
        return MultiCamNative.McCreateNm(modelName, out instance);
    }

    public int Delete(uint instance)
    {
        return MultiCamNative.McDelete(instance);
    }

    #endregion

    #region Parameter Access - Get

    public int GetParamInt(uint instance, string paramName, out int value)
    {
        return MultiCamNative.McGetParamNmInt(instance, paramName, out value);
    }

    public int GetParamInt64(uint instance, string paramName, out long value)
    {
        return MultiCamNative.McGetParamNmInt64(instance, paramName, out value);
    }

    public int GetParamFloat(uint instance, string paramName, out double value)
    {
        return MultiCamNative.McGetParamNmFloat(instance, paramName, out value);
    }

    public int GetParamStr(uint instance, string paramName, out string value)
    {
        byte[] buffer = new byte[MaxStringLength];
        int status = MultiCamNative.McGetParamNmStr(instance, paramName, buffer, MaxStringLength);

        if (status == MultiCamNative.MC_OK)
        {
            int length = Array.IndexOf(buffer, (byte)0);
            if (length < 0) length = MaxStringLength;
            value = Encoding.UTF8.GetString(buffer, 0, length);
        }
        else
        {
            value = string.Empty;
        }

        return status;
    }

    public int GetParamPtr(uint instance, string paramName, out IntPtr value)
    {
        return MultiCamNative.McGetParamNmPtr(instance, paramName, out value);
    }

    #endregion

    #region Parameter Access - Set

    public int SetParamInt(uint instance, string paramName, int value)
    {
        return MultiCamNative.McSetParamNmInt(instance, paramName, value);
    }

    public int SetParamInt64(uint instance, string paramName, long value)
    {
        return MultiCamNative.McSetParamNmInt64(instance, paramName, value);
    }

    public int SetParamFloat(uint instance, string paramName, double value)
    {
        return MultiCamNative.McSetParamNmFloat(instance, paramName, value);
    }

    public int SetParamStr(uint instance, string paramName, string value)
    {
        return MultiCamNative.McSetParamNmStr(instance, paramName, value);
    }

    public int SetParamPtr(uint instance, string paramName, IntPtr value)
    {
        return MultiCamNative.McSetParamNmPtr(instance, paramName, value);
    }

    #endregion

    #region Signaling

    public int RegisterCallback(uint instance, IntPtr callbackPtr, IntPtr context)
    {
        return MultiCamNative.McRegisterCallback(instance, callbackPtr, context);
    }

    public int WaitSignal(uint instance, int signal, uint timeout, out McSignalInfo info)
    {
        return MultiCamNative.McWaitSignal(instance, signal, timeout, out info);
    }

    #endregion
}
