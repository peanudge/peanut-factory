using System.Runtime.InteropServices;
using System.Text;

namespace PeanutVision.MultiCamDriver;

#region Type Definitions and Constants

/// <summary>
/// MultiCam status codes (MCSTATUS)
/// </summary>
public enum McStatus
{
	MC_OK = 0,
	MC_ERROR = -1,
	MC_INVALID_HANDLE = -2,
	MC_INVALID_PARAM = -3,
	MC_TIMEOUT = -4,
	MC_NOT_READY = -5,
	MC_NO_MORE_RESOURCES = -6,
	MC_IN_USE = -7,
	MC_BUSY = -8,
	MC_IO_ERROR = -9,
	MC_INTERNAL_ERROR = -10,
}

/// <summary>
/// MultiCam signal types for callback/wait operations
/// </summary>
public enum McSignal
{
	MC_SIG_SURFACE_PROCESSING = 1,
	MC_SIG_SURFACE_FILLED = 2,
	MC_SIG_ACQUISITION_FAILURE = 3,
	MC_SIG_END_CHANNEL_ACTIVITY = 4,
	MC_SIG_UNRECOVERABLE_ERROR = 5,
	MC_SIG_START_OF_FRAME = 6,
	MC_SIG_END_OF_FRAME = 7,
	MC_SIG_FRAMETRIGGER_VIOLATION = 8,
	MC_SIG_START_EXPOSURE = 9,
	MC_SIG_END_EXPOSURE = 10,
	MC_SIG_ANY = -1,
}

/// <summary>
/// Channel state values for MC_ChannelState parameter
/// </summary>
public enum McChannelState
{
	MC_ChannelState_IDLE = 1,
	MC_ChannelState_ACTIVE = 2,
	MC_ChannelState_READY = 3,
	MC_ChannelState_FREE = 4,
	MC_ChannelState_ORPHAN = 5,
}

/// <summary>
/// Trigger mode values
/// </summary>
public enum McTrigMode
{
	MC_TrigMode_INT = 1,
	MC_TrigMode_EXT = 2,
	MC_TrigMode_SOFT = 3,
	MC_TrigMode_EXTRC = 4,
	MC_TrigMode_AUTO = 5,
	MC_TrigMode_IMMEDIATE = 6,
	MC_TrigMode_HARD = 7,
	MC_TrigMode_COMBINED = 8,
	MC_TrigMode_PAUSE = 9,
	MC_TrigMode_MASTER_CHANNEL = 10,
	MC_TrigMode_SLAVE = 11,
}

/// <summary>
/// Acquisition mode values
/// </summary>
public enum McAcquisitionMode
{
	MC_AcquisitionMode_SNAPSHOT = 1,
	MC_AcquisitionMode_HFR = 2,
	MC_AcquisitionMode_PAGE = 3,
	MC_AcquisitionMode_WEB = 4,
	MC_AcquisitionMode_LONGPAGE = 5,
	MC_AcquisitionMode_INVALID = 6,
	MC_AcquisitionMode_VIDEO = 7,
}

/// <summary>
/// Sequence length values for controlling acquisition
/// </summary>
public enum McSeqLength
{
	MC_SeqLength_CM_INFINITE = -1,
	MC_SeqLength_CM_STOP = 0,
}

/// <summary>
/// Surface state values
/// </summary>
public enum McSurfaceState
{
	MC_SurfaceState_FREE = 0,
	MC_SurfaceState_FILLING = 1,
	MC_SurfaceState_FILLED = 2,
	MC_SurfaceState_PROCESSING = 3,
}

/// <summary>
/// Signal information structure returned by wait/callback operations (MCSIGNALINFO)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct McSignalInfo
{
	/// <summary>User-defined context pointer passed to McRegisterCallback</summary>
	public IntPtr Context;
	/// <summary>Instance handle that generated the signal (channel handle)</summary>
	public uint Instance;
	/// <summary>Signal identifier (McSignal value)</summary>
	public int Signal;
	/// <summary>Additional signal-specific information (e.g., surface index for SURFACE_PROCESSING)</summary>
	public uint SignalInfo;
	/// <summary>Signal context information</summary>
	public uint SignalContext;
}

/// <summary>
/// Callback delegate for MultiCam signal notifications.
/// Called from a dedicated MultiCam thread - must be thread-safe.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void McCallback(ref McSignalInfo signalInfo);

#endregion

#region Native P/Invoke Methods

/// <summary>
/// Native P/Invoke declarations for MultiCam library functions.
/// Uses modern LibraryImport with source-generated marshalling for AOT compatibility.
/// </summary>
public static partial class MultiCamApi
{
	private const string LibraryName = "MultiCam";

	#region Constants - System Objects

	// Object class constants (from multicam.h)
	private const uint MC_SURFACE_CLASS = 0x4;
	private const uint MC_CHANNEL_CLASS = 0x8;
	private const uint MC_CONFIG_CLASS = 0x2;
	private const uint MC_BOARD_CLASS = 0xE;

	/// <summary>Configuration object handle (MC_CONFIGURATION)</summary>
	public const uint MC_CONFIGURATION = (MC_CONFIG_CLASS << 28) | 0;  // 0x20000000

	/// <summary>Board object handle base (add board index for specific board)</summary>
	public const uint MC_BOARD = (MC_BOARD_CLASS << 28) | 0;  // 0xE0000000

	/// <summary>Default board index (first board)</summary>
	public const int DefaultBoardIndex = 0;

	/// <summary>Channel model for creating channels</summary>
	public const uint MC_CHANNEL = (MC_CHANNEL_CLASS << 28) | 0x0000FFFF;  // 0x8000FFFF

	/// <summary>Surface model for creating surfaces</summary>
	public const uint MC_SURFACE = (MC_SURFACE_CLASS << 28) | 0x0000FFFF;  // 0x4000FFFF

	/// <summary>Default/automatic surface handle</summary>
	public const uint MC_DEFAULT_SURFACE_HANDLE = (MC_SURFACE_CLASS << 28) | 0x0FFFFFFF;  // 0x4FFFFFFF

	#endregion

	#region Constants - Status Codes

	public const int MC_OK = 0;

	#endregion

	#region Constants - Parameter String Values

	// Channel State string values
	public const string MC_ChannelState_IDLE_STR = "IDLE";
	public const string MC_ChannelState_ACTIVE_STR = "ACTIVE";

	// Trigger mode string values
	public const string MC_TrigMode_IMMEDIATE_STR = "IMMEDIATE";
	public const string MC_TrigMode_HARD_STR = "HARD";
	public const string MC_TrigMode_SOFT_STR = "SOFT";
	public const string MC_TrigMode_COMBINED_STR = "COMBINED";

	// Signal enable string values
	public const string MC_SignalEnable_ON_STR = "ON";
	public const string MC_SignalEnable_OFF_STR = "OFF";

	// Signal enable integer values (for McSetParamInt with compound parameter ID)
	public const int MC_SignalEnable_ON = 5;
	public const int MC_SignalEnable_OFF = 4;

	// Parameter ID for SignalEnable (compound: MC_SignalEnable + signal_id)
	public const uint MC_SignalEnable = (24 << 14);

	// ForceTrig string values
	public const string MC_ForceTrig_STR = "TRIG";

	#endregion

	#region Constants - Common Parameter Names (for McGetParamNm/McSetParamNm)

	// Configuration parameters
	public const string PN_BoardCount = "BoardCount";
	public const string PN_ErrorHandling = "ErrorHandling";
	public const string PN_ErrorLog = "ErrorLog";

	// Board parameters
	public const string PN_BoardType = "BoardType";
	public const string PN_BoardName = "BoardName";
	public const string PN_SerialNumber = "SerialNumber";
	public const string PN_PciPosition = "PciPosition";

	// Channel configuration
	public const string PN_DriverIndex = "DriverIndex";
	public const string PN_Connector = "Connector";
	public const string PN_CamFile = "CamFile";
	public const string PN_ChannelState = "ChannelState";
	public const string PN_AcquisitionMode = "AcquisitionMode";
	public const string PN_TrigMode = "TrigMode";
	public const string PN_NextTrigMode = "NextTrigMode";
	public const string PN_SeqLength_Fr = "SeqLength_Fr";
	public const string PN_ForceTrig = "ForceTrig";

	// Surface/Cluster configuration
	public const string PN_SurfaceCount = "SurfaceCount";
	public const string PN_ClusterCount = "ClusterCount";
	public const string PN_SurfaceAddr = "SurfaceAddr";
	public const string PN_SurfacePitch = "SurfacePitch";
	public const string PN_SurfaceSize = "SurfaceSize";
	public const string PN_SurfaceState = "SurfaceState";
	public const string PN_SurfaceContext = "SurfaceContext";
	public const string PN_SurfaceIndex = "SurfaceIndex";

	// Image parameters
	public const string PN_ImageSizeX = "ImageSizeX";
	public const string PN_ImageSizeY = "ImageSizeY";
	public const string PN_ImageFlipX = "ImageFlipX";
	public const string PN_ImageFlipY = "ImageFlipY";
	public const string PN_ColorFormat = "ColorFormat";
	public const string PN_BufferPitch = "BufferPitch";
	public const string PN_BufferSize = "BufferSize";

	// Signal enable parameters
	public const string PN_SignalEnable = "SignalEnable";

	// Calibration parameters (Flat Field Correction)
	public const string PN_BlackCalibration = "BlackCalibration";
	public const string PN_WhiteCalibration = "WhiteCalibration";
	public const string PN_FlatFieldCorrection = "FlatFieldCorrection";

	// White balance parameters
	public const string PN_BalanceWhiteAuto = "BalanceWhiteAuto";
	public const string PN_BalanceRatioRed = "BalanceRatioRed";
	public const string PN_BalanceRatioGreen = "BalanceRatioGreen";
	public const string PN_BalanceRatioBlue = "BalanceRatioBlue";

	// Exposure/Gain parameters
	public const string PN_Expose_us = "Expose_us";
	public const string PN_ExposeMin_us = "ExposeMin_us";
	public const string PN_ExposeMax_us = "ExposeMax_us";
	public const string PN_Gain_dB = "Gain_dB";

	// Camera Link specific
	public const string PN_CamConfig = "CamConfig";
	public const string PN_TapConfiguration = "TapConfiguration";

	// Line rate
	public const string PN_LineRate_Hz = "LineRate_Hz";
	public const string PN_FrameRate_mHz = "FrameRate_mHz";

	// Input/Output status
	public const string PN_InputState = "InputState";
	public const string PN_OutputState = "OutputState";
	public const string PN_InputConnectorName = "InputConnectorName";

	// Diagnostics
	public const string PN_GrabberErrors = "GrabberErrors";
	public const string PN_ChannelLinkSyncErrors_X = "ChannelLinkSyncErrors_X";
	public const string PN_ChannelLinkClockErrors_X = "ChannelLinkClockErrors_X";
	public const string PN_LineTriggerViolation = "LineTriggerViolation";
	public const string PN_FrameTriggerViolation = "FrameTriggerViolation";
	public const string PN_DetectedSignalStrength = "DetectedSignalStrength";

	// PCIe
	public const string PN_PCIeLinkInfo = "PCIeLinkInfo";

	// Camera Link frequency range
	public const string PN_CameraLinkFrequencyRange = "CameraLinkFrequencyRange";

	#endregion

	#region Driver Connection

	/// <summary>
	/// Opens a connection to the MultiCam driver.
	/// Must be called before any other MultiCam function.
	/// </summary>
	/// <param name="multiCamName">Reserved, must be NULL</param>
	/// <returns>MC_OK on success</returns>
	[LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McOpenDriver(string? multiCamName);

	/// <summary>
	/// Closes the connection to the MultiCam driver.
	/// Must be called the same number of times as McOpenDriver.
	/// </summary>
	[LibraryImport(LibraryName)]
	public static partial int McCloseDriver();

	#endregion

	#region Instance Management

	/// <summary>
	/// Creates a new MultiCam object instance.
	/// </summary>
	/// <param name="model">Model type (MC_CHANNEL or MC_SURFACE)</param>
	/// <param name="instance">Output: Created instance handle</param>
	[LibraryImport(LibraryName)]
	public static partial int McCreate(uint model, out uint instance);

	/// <summary>
	/// Creates a new MultiCam object instance from a model name string.
	/// </summary>
	[LibraryImport(LibraryName, EntryPoint = "McCreateNm", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McCreateNm(string modelName, out uint instance);

	/// <summary>
	/// Deletes a MultiCam object instance and releases associated resources.
	/// </summary>
	[LibraryImport(LibraryName)]
	public static partial int McDelete(uint instance);

	#endregion

	#region Parameters - Get Methods (by Name)

	[LibraryImport(LibraryName, EntryPoint = "McGetParamNmInt", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McGetParamNmInt(uint instance, string paramName, out int value);

	[LibraryImport(LibraryName, EntryPoint = "McGetParamNmInt64", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McGetParamNmInt64(uint instance, string paramName, out long value);

	[LibraryImport(LibraryName, EntryPoint = "McGetParamNmFloat", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McGetParamNmFloat(uint instance, string paramName, out double value);

	[LibraryImport(LibraryName, EntryPoint = "McGetParamNmStr", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McGetParamNmStr(uint instance, string paramName, [Out] byte[] value, uint maxLength);

	[LibraryImport(LibraryName, EntryPoint = "McGetParamNmInst", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McGetParamNmInst(uint instance, string paramName, out uint valueInst);

	[LibraryImport(LibraryName, EntryPoint = "McGetParamNmPtr", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McGetParamNmPtr(uint instance, string paramName, out IntPtr valuePtr);

	#endregion

	#region Parameters - Set Methods (by ID)

	/// <summary>
	/// Sets an integer parameter using the parameter ID (not name).
	/// Used for compound parameters like MC_SignalEnable + MC_SIG_SURFACE_PROCESSING.
	/// </summary>
	[LibraryImport(LibraryName, EntryPoint = "McSetParamInt")]
	public static partial int McSetParamInt(uint instance, uint paramId, int value);

	#endregion

	#region Parameters - Set Methods (by Name)

	[LibraryImport(LibraryName, EntryPoint = "McSetParamNmInt", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McSetParamNmInt(uint instance, string paramName, int value);

	[LibraryImport(LibraryName, EntryPoint = "McSetParamNmInt64", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McSetParamNmInt64(uint instance, string paramName, long value);

	[LibraryImport(LibraryName, EntryPoint = "McSetParamNmFloat", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McSetParamNmFloat(uint instance, string paramName, double value);

	[LibraryImport(LibraryName, EntryPoint = "McSetParamNmStr", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McSetParamNmStr(uint instance, string paramName, string value);

	[LibraryImport(LibraryName, EntryPoint = "McSetParamNmInst", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McSetParamNmInst(uint instance, string paramName, uint valueInst);

	[LibraryImport(LibraryName, EntryPoint = "McSetParamNmPtr", StringMarshalling = StringMarshalling.Utf8)]
	public static partial int McSetParamNmPtr(uint instance, string paramName, IntPtr valuePtr);

	#endregion

	#region Signaling

	/// <summary>
	/// Registers a callback function for signal notifications.
	/// The callback is invoked from a dedicated MultiCam thread.
	/// </summary>
	[LibraryImport(LibraryName)]
	public static partial int McRegisterCallback(uint instance, IntPtr callbackFn, IntPtr context);

	/// <summary>
	/// Waits for a signal from the specified instance.
	/// </summary>
	/// <param name="instance">Channel instance handle</param>
	/// <param name="signal">Signal to wait for (cast McSignal to int)</param>
	/// <param name="timeout">Timeout in milliseconds (use 0xFFFFFFFF for infinite)</param>
	/// <param name="info">Output: Signal information</param>
	[LibraryImport(LibraryName)]
	public static partial int McWaitSignal(uint instance, int signal, uint timeout, out McSignalInfo info);

	#endregion
}

#endregion

#region Exceptions

/// <summary>
/// Exception thrown when a MultiCam operation fails
/// </summary>
public class MultiCamException : Exception
{
	public int StatusCode { get; }
	public string? Operation { get; }

	public MultiCamException(int statusCode, string operation)
		: base($"MultiCam {operation} failed with status: {(McStatus)statusCode} ({statusCode})")
	{
		StatusCode = statusCode;
		Operation = operation;
	}

	public MultiCamException(int statusCode, string operation, string details)
		: base($"MultiCam {operation} failed: {details} [Status: {(McStatus)statusCode}]")
	{
		StatusCode = statusCode;
		Operation = operation;
	}
}

#endregion
