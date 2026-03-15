# MultiCam Error Codes Reference

> Source: Euresys MultiCam SDK Documentation — [MultiCam Error Codes](https://documentation.euresys.com/Products/MultiCam/MultiCam/Content/03_MultiCam/Multicam_User_Guide/multicam-error-codes.htm), [MCSTATUS](https://documentation.euresys.com/Products/MULTICAM/MULTICAM/Content/03_MultiCam/Multicam_API_References/multicam-c-reference/mcstatus.htm), [API Errors](https://documentation.euresys.com/products/multicam/multicam_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/api-errors.htm)

## Contents

- [MCSTATUS Type](#mcstatus-type)
- [Error Code Table](#error-code-table)
- [Error Code Details and Troubleshooting](#error-code-details-and-troubleshooting)
- [Error Handling Modes](#error-handling-modes)
- [Error Handling in C# (.NET)](#error-handling-in-c-net)
- [Best Practices](#best-practices)

---

## MCSTATUS Type

All MultiCam C API functions return a value of type `MCSTATUS`, which is a signed 32-bit integer:

```c
typedef int MCSTATUS;
```

- A return value of `0` (`MC_OK`) indicates success.
- A negative return value indicates an error, with the specific negative value identifying the error type.

---

## Error Code Table

| Return Value | Error Identifier | Description |
|:---:|---|---|
| `0` | `MC_OK` | No error |
| `-1` | `MC_NO_BOARD_FOUND` | No board found |
| `-2` | `MC_BAD_PARAMETER` | Bad parameter |
| `-3` | `MC_IO_ERROR` | I/O error |
| `-4` | `MC_INTERNAL_ERROR` | Internal error |
| `-5` | `MC_NO_MORE_RESOURCES` | No more resources |
| `-6` | `MC_IN_USE` | Object still in use |
| `-7` | `MC_NOT_SUPPORTED` | Operation not supported |
| `-8` | `MC_DATABASE_ERROR` | Parameter database error |
| `-9` | `MC_OUT_OF_BOUND` | Value out of bound |
| `-10` | `MC_INSTANCE_NOT_FOUND` | Object instance not found |
| `-11` | `MC_INVALID_HANDLE` | Invalid handle |
| `-12` | `MC_TIMEOUT` | Timeout |
| `-13` | `MC_INVALID_VALUE` | Invalid value |
| `-14` | `MC_RANGE_ERROR` | Value not in range |
| `-15` | `MC_BAD_HW_CONFIG` | Invalid hardware configuration |
| `-16` | `MC_NO_EVENT` | No event |
| `-17` | `MC_LICENSE_NOT_GRANTED` | License not granted |
| `-18` | `MC_FATAL_ERROR` | Fatal error |
| `-19` | `MC_HW_EVENT_CONFLICT` | Hardware event conflict |
| `-20` | `MC_FILE_NOT_FOUND` | File not found |
| `-21` | `MC_OVERFLOW` | Overflow |
| `-22` | `MC_INVALID_PARAMETER_SETTING` | Parameter inconsistency |
| `-23` | `MC_PARAMETER_ILLEGAL_ACCESS` | Illegal operation |
| `-24` | `MC_CLUSTER_BUSY` | Cluster busy |
| `-25` | `MC_SERVICE_ERROR` | MultiCam service error |
| `-26` | `MC_INVALID_SURFACE` | Invalid surface |

---

## Error Code Details and Troubleshooting

### `MC_OK` (0) — No Error

The function executed successfully. No action required.

---

### `MC_NO_BOARD_FOUND` (-1) — No Board Found

The driver could not detect any compatible Euresys frame grabber board in the system.

**Troubleshooting:**
- Verify the board is physically seated correctly in the PCIe slot.
- Check that the Euresys MultiCam driver is installed and running (`MultiCam` Windows service).
- Run the Euresys board information utility to confirm detection.
- Ensure the board is not disabled in Device Manager.

---

### `MC_BAD_PARAMETER` (-2) — Bad Parameter

The parameter name or ID passed to a `McSetParam` / `McGetParam` function is invalid or does not exist for the target object.

**Troubleshooting:**
- Verify the parameter name string is spelled correctly (parameter names are case-sensitive).
- Confirm the parameter is valid for the object type (e.g., channel vs. board vs. configuration).
- Check that the parameter exists for the loaded `.cam` file and board type.

---

### `MC_IO_ERROR` (-3) — I/O Error

A hardware-level I/O error occurred during communication with the board or camera.

**Troubleshooting:**
- Inspect Camera Link cable connections for loose contacts.
- Check for electromagnetic interference near the frame grabber.
- Verify the board firmware is up to date.
- Restart the MultiCam service and retry.

---

### `MC_INTERNAL_ERROR` (-4) — Internal Error

An unexpected internal error occurred within the MultiCam driver.

**Troubleshooting:**
- Restart the MultiCam service.
- Reboot the system if the error persists.
- Check the Windows Event Log for related driver messages.
- Contact Euresys support if the issue is reproducible.

---

### `MC_NO_MORE_RESOURCES` (-5) — No More Resources

The system has exhausted available resources (memory, DMA channels, handles, etc.).

**Troubleshooting:**
- Ensure all previously created objects (`McCreate`) are properly deleted (`McDelete`).
- Reduce `SurfaceCount` or image resolution if memory is constrained.
- Close other applications that may be consuming DMA or system resources.

---

### `MC_IN_USE` (-6) — Object Still in Use

The object cannot be deleted or modified because it is currently in use (e.g., a channel is `ACTIVE`).

**Troubleshooting:**
- Set `ChannelState` to `IDLE` before deleting a channel.
- Ensure no callback is actively processing a surface before cleanup.
- Wait for all pending signals to complete.

---

### `MC_NOT_SUPPORTED` (-7) — Operation Not Supported

The requested operation or parameter value is not supported by the current board or camera configuration.

**Troubleshooting:**
- Verify the feature is supported by your board model (e.g., Grablink Full PC1622).
- Check that the `.cam` file matches the camera model.
- Consult the board-specific parameter reference.

---

### `MC_DATABASE_ERROR` (-8) — Parameter Database Error

An error occurred in the internal parameter database, typically due to a corrupt or incompatible `.cam` file.

**Troubleshooting:**
- Verify the `.cam` file is valid and not corrupted.
- Reinstall the MultiCam driver to restore default `.cam` files.
- Ensure the `.cam` file version matches the installed driver version.

---

### `MC_OUT_OF_BOUND` (-9) — Value Out of Bound

A parameter value exceeds the allowed range.

**Troubleshooting:**
- Query the parameter's valid range using `McGetParamNmInt` with the corresponding `Min` / `Max` parameter names (e.g., `ExposeMin_us`, `ExposeMax_us`).
- Adjust the value to fall within the permitted range.

---

### `MC_INSTANCE_NOT_FOUND` (-10) — Object Instance Not Found

The specified object instance does not exist in the driver's internal registry.

**Troubleshooting:**
- Verify the handle was obtained from a successful `McCreate` call.
- Ensure the object has not already been deleted with `McDelete`.
- Check for handle corruption or use-after-free conditions.

---

### `MC_INVALID_HANDLE` (-11) — Invalid Handle

The handle passed to the function is not a valid MultiCam object handle.

**Troubleshooting:**
- Confirm the handle variable was initialized by `McCreate`.
- Do not use handles after calling `McDelete` on them.
- Avoid passing `0` or uninitialized values as handles.

---

### `MC_TIMEOUT` (-12) — Timeout

A wait operation (e.g., `McWaitSignal`) exceeded its timeout period without receiving the expected signal.

**Troubleshooting:**
- Verify the channel is in `ACTIVE` state before waiting for signals.
- For `SNAPSHOT` mode, ensure `ForceTrig` has been issued.
- Check that the camera is connected and producing frames.
- Increase the timeout value if the camera has long exposure times.

---

### `MC_INVALID_VALUE` (-13) — Invalid Value

The parameter value is syntactically or semantically invalid.

**Troubleshooting:**
- For string parameters, verify the exact spelling and casing (e.g., `"SNAPSHOT"` not `"Snapshot"`).
- For integer parameters, ensure the value is within the expected enumeration.
- Consult the parameter reference for valid values.

---

### `MC_RANGE_ERROR` (-14) — Value Not in Range

Similar to `MC_OUT_OF_BOUND`, but specifically indicates the value falls outside a defined numeric range.

**Troubleshooting:**
- Check the parameter's valid range in the documentation.
- Ensure dependent parameters are set before the constrained parameter (order of parameter setting can affect valid ranges).

---

### `MC_BAD_HW_CONFIG` (-15) — Invalid Hardware Configuration

The hardware configuration is invalid, often due to incompatible parameter combinations.

**Troubleshooting:**
- Verify the `Connector`, `DriverIndex`, and `CamFile` parameters are consistent.
- Ensure the Camera Link configuration matches the physical wiring (Base, Medium, Full).
- Check that no conflicting channels share the same connector.

---

### `MC_NO_EVENT` (-16) — No Event

No signal event was available when checked (non-blocking signal check).

**Troubleshooting:**
- This is often an expected condition when polling; not necessarily an error.
- Use `McWaitSignal` with a timeout for blocking behavior instead.

---

### `MC_LICENSE_NOT_GRANTED` (-17) — License Not Granted

The board's security key does not authorize the requested feature or the license has expired.

**Troubleshooting:**
- Run the Euresys license management utility.
- Verify the board serial number matches the license key.
- Contact Euresys for license renewal or upgrade.

---

### `MC_FATAL_ERROR` (-18) — Fatal Error

A non-recoverable error occurred. The driver or board may be in an undefined state.

**Troubleshooting:**
- Restart the application and the MultiCam service.
- If recurring, reboot the system.
- Check system logs for hardware failures or driver crashes.
- Contact Euresys support with full diagnostic logs.

---

### `MC_HW_EVENT_CONFLICT` (-19) — Hardware Event Conflict

A hardware event conflict was detected, typically when multiple channels or signals compete for the same hardware resource.

**Troubleshooting:**
- Review signal routing and ensure no two channels share conflicting hardware triggers.
- Reduce the number of simultaneously active channels.

---

### `MC_FILE_NOT_FOUND` (-20) — File Not Found

The specified file (typically a `.cam` file) could not be found.

**Troubleshooting:**
- Verify the `CamFile` path is correct and the file exists on disk.
- Check the MultiCam cam file search directories.
- Ensure the file name includes the correct extension (`.cam`).

---

### `MC_OVERFLOW` (-21) — Overflow

A data overflow occurred, typically in the DMA transfer or internal buffer.

**Troubleshooting:**
- Reduce the frame rate or image resolution.
- Increase `SurfaceCount` to provide more buffering.
- Ensure the CPU is not overloaded during acquisition.
- Check PCIe bandwidth availability.

---

### `MC_INVALID_PARAMETER_SETTING` (-22) — Parameter Inconsistency

The combination of parameter values is internally inconsistent or contradictory.

**Troubleshooting:**
- Review the order in which parameters are set; some parameters constrain others.
- Reset the channel and reconfigure parameters in the recommended order.
- Consult the acquisition mode documentation for valid parameter combinations.

---

### `MC_PARAMETER_ILLEGAL_ACCESS` (-23) — Illegal Operation

Attempted to read a write-only parameter, write a read-only parameter, or access a parameter at an invalid time (e.g., while the channel is `ACTIVE`).

**Troubleshooting:**
- Check the parameter's access mode (read, write, read/write) in the reference documentation.
- Set `ChannelState` to `IDLE` before modifying parameters that cannot be changed during acquisition.

---

### `MC_CLUSTER_BUSY` (-24) — Cluster Busy

The surface cluster is busy (a surface is in `PROCESSING` state) and cannot accept the requested operation.

**Troubleshooting:**
- Ensure callback processing completes promptly (target < 1 ms).
- Release surfaces from `PROCESSING` state by returning from the callback.
- Increase `SurfaceCount` to reduce contention.

---

### `MC_SERVICE_ERROR` (-25) — MultiCam Service Error

The MultiCam Windows service is not running or cannot be contacted. This is commonly encountered during `McOpenDriver`.

**Troubleshooting:**
- Check that the `MultiCam` Windows service is running (`services.msc`).
- Implement a retry loop with delay when calling `McOpenDriver`:
  ```csharp
  int status;
  do {
      status = McOpenDriver(null);
      if (status == -25) Thread.Sleep(500);
  } while (status == -25);
  ```
- Restart the MultiCam service if it is stopped.
- Reinstall the driver if the service fails to start.

---

### `MC_INVALID_SURFACE` (-26) — Invalid Surface

The specified surface handle or index is invalid.

**Troubleshooting:**
- Verify the surface index is within the range `[0, SurfaceCount - 1]`.
- Ensure surfaces have been properly registered to the cluster.
- Do not access surfaces after deleting the parent channel.

---

## Error Handling Modes

MultiCam supports four error handling behaviors controlled by the `ErrorHandling` configuration parameter on the `MC_CONFIGURATION` object:

| Mode | Behavior |
|---|---|
| `NONE` | Returns the `MCSTATUS` error code as an integer. The application must check the return value. |
| `MSGBOX` | Displays a dialog box with error details and three buttons: **OK** (returns error code), **ABORT** (terminates application), **IGNORE** (returns `MC_OK`). Windows only. |
| `EXCEPTION` | Throws a WIN32 structured exception with the `MCSTATUS` code. Windows only. |
| `MSGEXCEPTION` | Combines `MSGBOX` and `EXCEPTION`: displays a dialog, then throws an exception on **OK**. Windows only. |

For headless or service-based applications, use `NONE` and handle errors programmatically.

---

## Error Handling in C# (.NET)

Since this project uses `LibraryImport` (P/Invoke) from .NET, all API calls return raw `MCSTATUS` integers. A recommended helper pattern:

```csharp
public static class McCheck
{
    public static void Status(int status, [CallerMemberName] string? caller = null)
    {
        if (status != 0) // MC_OK
        {
            string name = status switch
            {
                -1  => "MC_NO_BOARD_FOUND",
                -2  => "MC_BAD_PARAMETER",
                -3  => "MC_IO_ERROR",
                -4  => "MC_INTERNAL_ERROR",
                -5  => "MC_NO_MORE_RESOURCES",
                -6  => "MC_IN_USE",
                -7  => "MC_NOT_SUPPORTED",
                -8  => "MC_DATABASE_ERROR",
                -9  => "MC_OUT_OF_BOUND",
                -10 => "MC_INSTANCE_NOT_FOUND",
                -11 => "MC_INVALID_HANDLE",
                -12 => "MC_TIMEOUT",
                -13 => "MC_INVALID_VALUE",
                -14 => "MC_RANGE_ERROR",
                -15 => "MC_BAD_HW_CONFIG",
                -16 => "MC_NO_EVENT",
                -17 => "MC_LICENSE_NOT_GRANTED",
                -18 => "MC_FATAL_ERROR",
                -19 => "MC_HW_EVENT_CONFLICT",
                -20 => "MC_FILE_NOT_FOUND",
                -21 => "MC_OVERFLOW",
                -22 => "MC_INVALID_PARAMETER_SETTING",
                -23 => "MC_PARAMETER_ILLEGAL_ACCESS",
                -24 => "MC_CLUSTER_BUSY",
                -25 => "MC_SERVICE_ERROR",
                -26 => "MC_INVALID_SURFACE",
                _   => $"UNKNOWN_ERROR({status})",
            };
            throw new InvalidOperationException(
                $"MultiCam error in {caller}: {name} ({status})");
        }
    }
}
```

Usage:

```csharp
McCheck.Status(MultiCamNative.McOpenDriver(null));
McCheck.Status(MultiCamNative.McCreate(MC_CHANNEL, out uint channel));
McCheck.Status(MultiCamNative.McSetParamNmStr(channel, "CamFile", "TC-A160K-SEM_freerun_RGB8.cam"));
```

---

## Best Practices

1. **Always check return values.** Every `MultiCamNative` call returns `MCSTATUS`. Never ignore the return value.

2. **Use `NONE` error handling mode** for server/service applications to avoid dialog boxes blocking execution.

3. **Implement retry logic for `MC_SERVICE_ERROR` (-25).** This is expected during `McOpenDriver` when the service is still initializing.

4. **Log all non-zero status codes** with the function name and parameters for post-mortem debugging.

5. **Clean up on error.** If initialization fails partway, ensure all previously created objects are deleted via `McDelete` and `McCloseDriver` is called.

6. **Do not ignore `MC_TIMEOUT`.** A timeout on `McWaitSignal` typically indicates a camera or trigger issue, not a transient condition.

7. **Handle `MC_CLUSTER_BUSY` gracefully.** Optimize callback processing time and increase `SurfaceCount` to minimize this condition.
