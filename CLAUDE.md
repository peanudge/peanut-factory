This guide is optimized for controlling the **Grablink Full (PC1622)** board and **Crevis TC-A160K** camera via direct native driver access through **P/Invoke (`LibraryImport`)**, without using any legacy `.NET Framework` libraries.

---

# CLAUDE.md: Vision System Project (Grablink & Crevis)

## Project Overview

- **Frame Grabber:** Euresys Grablink Full (PC1622)
- **Camera:** Crevis TC-A160K (Area-Scan, Camera Link, 4160x3120, 1.4um pixel)
- **Runtime:** .NET 8 (C# 12)
- **SDK Strategy:** Native C API Interop via `LibraryImport` (P/Invoke)
- **Core Library:** `MultiCam.dll` (System level driver)
- **Header References:** `MultiCam.h`, `McParams.h`

## Documentation References

Converted Markdown docs in `doc/` contain the full Euresys MultiCam SDK documentation.
**When working on MultiCam driver code, P/Invoke bindings, acquisition logic, or camera control, you MUST read the relevant doc files below before writing code:**

| When you need... | Read this file |
|------------------|---------------|
| Channel setup, acquisition flow, trigger modes, start/stop control | `doc/multicam-acquisition-principles.md` |
| Pixel formats, memory layouts, Bayer patterns, buffer pitch/stride | `doc/multicam-storage-formats.md` |
| Grablink board config, Camera Link, I/O, serial comms, full API | `doc/grablink-functional-guide.md` |
| Sample code patterns and program descriptions | `doc/multicam-sample-programs.md` |

---

## MultiCam Object Model Constants

```csharp
public const uint MC_CONFIGURATION = 0x20000000;  // System configuration
public const uint MC_BOARD         = 0xE0000000;  // Board object (+ index)
public const uint MC_CHANNEL       = 0x8000FFFF;  // Channel model
public const uint MC_SURFACE       = 0x4000FFFF;  // Surface model
public const int  MC_OK            = 0;            // Success status
```

## Native C API Functions (LibraryImport)

```csharp
public static partial class MultiCamNative
{
    private const string LibraryName = "MultiCam.dll";

    // Driver lifecycle
    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McOpenDriver(string? multiCamName);  // Must pass NULL

    [LibraryImport(LibraryName)]
    public static partial int McCloseDriver();

    // Instance management
    [LibraryImport(LibraryName)]
    public static partial int McCreate(uint model, out uint instance);

    [LibraryImport(LibraryName)]
    public static partial int McDelete(uint instance);

    // Parameter access (by name — preferred)
    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McSetParamNmStr(uint instance, string paramName, string value);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McSetParamNmInt(uint instance, string paramName, int value);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McGetParamNmInt(uint instance, string paramName, out int value);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McGetParamNmStr(uint instance, string paramName, byte[] value, uint maxLen);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McGetParamNmPtr(uint instance, string paramName, out IntPtr valuePtr);

    // Parameter access (by integer ID — for compound params like SignalEnable)
    [LibraryImport(LibraryName)]
    public static partial int McSetParamInt(uint instance, uint paramId, int value);

    // Signaling
    [LibraryImport(LibraryName)]
    public static partial int McRegisterCallback(uint instance, IntPtr callbackFn, IntPtr context);

    [LibraryImport(LibraryName)]
    public static partial int McWaitSignal(uint instance, int signal, uint timeout, out McSignalInfo info);
}
```

## Signal Constants & Callback

```csharp
public enum McSignal
{
    MC_SIG_SURFACE_PROCESSING   = 1,   // Frame ready for processing (primary)
    MC_SIG_SURFACE_FILLED       = 2,   // Surface buffer filled
    MC_SIG_ACQUISITION_FAILURE  = 3,   // Acquisition error
    MC_SIG_END_CHANNEL_ACTIVITY = 4,   // Acquisition stopped
    MC_SIG_UNRECOVERABLE_ERROR  = 5,   // Fatal error
    MC_SIG_START_OF_FRAME       = 6,
    MC_SIG_END_OF_FRAME         = 7,
    MC_SIG_FRAMETRIGGER_VIOLATION = 8,
    MC_SIG_START_EXPOSURE       = 9,
    MC_SIG_END_EXPOSURE         = 10,
    MC_SIG_ANY                  = -1,  // Wait for any signal
}

[StructLayout(LayoutKind.Sequential)]
public struct McSignalInfo
{
    public IntPtr Context;
    public uint   Instance;
    public int    Signal;
    public uint   SignalInfo;     // e.g., surface index for SURFACE_PROCESSING
    public uint   SignalContext;
}

// Enable signal: compound parameter
// uint paramId = MC_SignalEnable + (uint)McSignal.MC_SIG_SURFACE_PROCESSING;
// McSetParamInt(channel, paramId, 5);  // 5=ON, 4=OFF
```

## Channel Parameter Names (string, for McSetParamNmXxx)

### Board & Connector
| Parameter | Example | Description |
|-----------|---------|-------------|
| `DriverIndex` | `0` | Board index (PC1622 = 0) |
| `Connector` | `"M"` | Camera connector (M=Medium on PC1622) |
| `CamFile` | `"TC-A160K-SEM_freerun_RGB8.cam"` | Camera configuration file path |

### Acquisition Control
| Parameter | Values | Description |
|-----------|--------|-------------|
| `AcquisitionMode` | `SNAPSHOT`, `VIDEO`, `HFR`, `PAGE`, `WEB`, `LONGPAGE` | Acquisition mode |
| `ChannelState` | `IDLE`, `ACTIVE` | Start/stop acquisition |
| `TrigMode` | `IMMEDIATE`, `SOFT`, `HARD`, `COMBINED` | Sequence start trigger |
| `NextTrigMode` | `REPEAT`, `COMBINED`, `PERIODIC` | Subsequent phase trigger |
| `EndTrigMode` | `AUTO`, `HARD` | Sequence termination |
| `ForceTrig` | `TRIG` | Fire software trigger |
| `SeqLength_Fr` | integer or `-1` (infinite) | Frames per sequence |
| `ActivityLength` | integer | Sequences per activity |
| `BreakEffect` | string | Stop behavior on user break |

### Surface/Cluster
| Parameter | Type | Description |
|-----------|------|-------------|
| `SurfaceCount` | int | Number of frame buffers (recommend >= 2) |
| `SurfaceAddr` | ptr | Surface memory address |
| `SurfacePitch` | int | Bytes per row (stride) |
| `SurfaceState` | string | `FREE` / `FILLING` / `FILLED` / `PROCESSING` |
| `ImageSizeX` | int | Image width in pixels |
| `ImageSizeY` | int | Image height in pixels |
| `BufferPitch` | int | Buffer stride in bytes |
| `ColorFormat` | string | Pixel format (see below) |

### Trigger Hardware
| Parameter | Description |
|-----------|-------------|
| `TrigCtl` | Electrical style of trigger line |
| `TrigEdge` | `GOLOW` / `GOHIGH` |
| `TrigFilter` | Noise removal |
| `TrigLine` | Hardware trigger line |
| `TrigDelay_us` | Trigger-to-reset delay (us) |
| `TargetFrameRate_Hz` | Periodic trigger rate |

### Calibration & Exposure (Crevis TC-A160K)
| Parameter | Values | Description |
|-----------|--------|-------------|
| `BlackCalibration` | `Execute` | Run black FFC (cover lens!) |
| `WhiteCalibration` | `Execute` | Run white FFC (~200DN illumination) |
| `FlatFieldCorrection` | `ON` / `OFF` | Enable/disable FFC |
| `BalanceWhiteAuto` | `ONCE` / `CONTINUOUS` / `OFF` | Auto white balance |
| `BalanceRatioRed` | float | Red channel gain |
| `BalanceRatioGreen` | float | Green channel gain |
| `BalanceRatioBlue` | float | Blue channel gain |
| `Expose_us` | int | Exposure time (microseconds) |
| `ExposeMin_us` | int (read) | Min exposure |
| `ExposeMax_us` | int (read) | Max exposure |
| `Gain_dB` | float | Gain in decibels |

### Diagnostics (read-only)
| Parameter | Description |
|-----------|-------------|
| `GrabberErrors` | Error counter |
| `FrameTriggerViolation` | Frame trigger violation count |
| `LineTriggerViolation` | Line trigger violation count |
| `DetectedSignalStrength` | Signal strength indicator |

---

## Acquisition Modes (Area-Scan: TC-A160K)

| Mode | Use Case | TrigMode | SeqLength_Fr |
|------|----------|----------|-------------|
| **SNAPSHOT** | Single frame per trigger | `SOFT` or `HARD` | 1+ |
| **VIDEO** | Continuous freerun | `IMMEDIATE` | `-1` (infinite) |
| **HFR** | High frame rate (multi-frame/surface) | varies | 1-256 |

PAGE, WEB, LONGPAGE are for **line-scan cameras only** — do not use with TC-A160K.

## Acquisition Hierarchy

```
Activity (channel ACTIVE → IDLE)
  └── Sequence (started by TrigMode trigger)
       └── Phase (= one surface fill)
            └── Slice (= one frame capture)
```

Key events: `SCA` → `SAS` → `SAP` → `SASL` → `XPC` → `ROC` → `EAP` → `EAS`

---

## Pixel Formats (ColorFormat values)

### Monochrome
| Format | Bits | Bytes/px | Notes |
|--------|------|----------|-------|
| `Y8` | 8 | 1 | Mono8 |
| `Y10` | 10 | 2 | Upper bits zero |
| `Y12` | 12 | 2 | |
| `Y16` | 16 | 2 | |

### Bayer CFA
| Format | Bits | Patterns |
|--------|------|----------|
| `BAYER8` | 8 | BayerBG8, BayerGB8, BayerGR8, BayerRG8 |
| `BAYER10` | 10 | Same 4 patterns |
| `BAYER12` | 12 | Same 4 patterns |

### RGB Packed
| Format | Bits | Bytes/px | Memory Order |
|--------|------|----------|-------------|
| `RGB24` / `RGB8` | 8 | 3 | B-G-R (BGR8) |
| `RGB32` | 8+a | 4 | B-G-R-A (BGRa8) |
| `RGBa8` | 8+a | 4 | R-G-B-A |

### RGB Planar
`RGB24PL` (8-bit), `RGB30PL` (10-bit), `RGB36PL` (12-bit), `RGB48PL` (16-bit)
— 3 separate planes: R, G, B each with own pitch.

### Pixel Data Access
```csharp
// Y8: pixel at (x,y)
byte value = buffer[y * pitch + x];

// RGB24 (BGR8): pixel at (x,y)
byte b = buffer[y * pitch + x * 3 + 0];
byte g = buffer[y * pitch + x * 3 + 1];
byte r = buffer[y * pitch + x * 3 + 2];

// RGB32 (BGRa8): pixel at (x,y)
byte b = buffer[y * pitch + x * 4 + 0];
byte g = buffer[y * pitch + x * 4 + 1];
byte r = buffer[y * pitch + x * 4 + 2];
```

---

## Initialization Sequence

```csharp
// 1. Open driver
McOpenDriver(null);  // retry on MC_SERVICE_ERROR(-25)

// 2. Create channel
McCreate(MC_CHANNEL, out uint channel);

// 3. Configure
McSetParamNmInt(channel, "DriverIndex", 0);
McSetParamNmStr(channel, "Connector", "M");
McSetParamNmStr(channel, "CamFile", "TC-A160K-SEM_freerun_RGB8.cam");

// 4. Acquisition mode
McSetParamNmStr(channel, "AcquisitionMode", "SNAPSHOT");
McSetParamNmStr(channel, "TrigMode", "SOFT");

// 5. Buffers
McSetParamNmInt(channel, "SurfaceCount", 4);

// 6. Enable signals
uint sigParam = MC_SignalEnable + (uint)MC_SIG_SURFACE_PROCESSING;
McSetParamInt(channel, sigParam, 5);  // ON

// 7. Register callback (or use McWaitSignal)
McRegisterCallback(channel, callbackPtr, IntPtr.Zero);

// 8. Start
McSetParamNmStr(channel, "ChannelState", "ACTIVE");

// 9. Software trigger
McSetParamNmStr(channel, "ForceTrig", "TRIG");

// 10. Cleanup
McSetParamNmStr(channel, "ChannelState", "IDLE");
McDelete(channel);
McCloseDriver();
```

## Crevis TC-A160K Camera Files

| CamFile | Mode | Format |
|---------|------|--------|
| `TC-A160K-SEM_freerun_RGB8.cam` | Freerun | RGB 8-bit |
| `TC-A160K-SEM_freerun_Mono8.cam` | Freerun | Mono 8-bit |
| `TC-A160K-SEM_software_RGB8.cam` | Software trigger | RGB 8-bit |
| `TC-A160K-SEM_software_Mono8.cam` | Software trigger | Mono 8-bit |

## Hardware Control (Crevis TC-A160K)

- **Load CamFile:** `TC-A160K-SEM_freerun_RGB8.cam`
- **White Balance:** `BalanceWhiteAuto = ONCE` — must capture a white target (~200DN)
- **FFC Calibration:**
  - **Black:** Cover the lens, then `BlackCalibration = Execute`
  - **White:** Under uniform illumination (~200DN), `WhiteCalibration = Execute`
- **Trigger:** Software trigger via `ForceTrig = TRIG`

---

## Development Rules

1. **Error Handling:** Every API call returns `MCSTATUS` — throw or log if not `0 (MC_OK)`.
2. **Resource Cleanup:** Use `IDisposable` pattern to always call `McDelete` + `McCloseDriver`.
3. **Thread Safety:** Callbacks run on a dedicated driver thread — use `Invoke` for UI updates.
4. **Driver Polling:** On `MC_SERVICE_ERROR (-25)` from `McOpenDriver`, retry in a polling loop until success.
5. **Surface Lifecycle:** Respect the `FREE → FILLING → FILLED → PROCESSING → FREE` cycle.
6. **Callback Performance:** Keep callback handler < 1ms; use `ConcurrentQueue` for data handoff.
7. **API Layer Abstraction:** The API layer must be user-friendly and hide hardware mechanism details. Expose simple, intuitive endpoints (e.g., "snapshot", "start", "stop") — never leak MultiCam constants, surface lifecycle, signal handling, or driver internals to API consumers.

## Error/Status Codes

| Code | Name | Description |
|------|------|-------------|
| `0` | `MC_OK` | Success |
| `-1` | `MC_ERROR` | Generic error |
| `-2` | `MC_INVALID_HANDLE` | Invalid instance handle |
| `-3` | `MC_INVALID_PARAM` | Invalid parameter |
| `-4` | `MC_TIMEOUT` | Operation timeout |
| `-5` | `MC_NOT_READY` | Device not ready |
| `-6` | `MC_NO_MORE_RESOURCES` | Resource exhaustion |
| `-7` | `MC_IN_USE` | Resource in use |
| `-8` | `MC_BUSY` | Device busy |
| `-9` | `MC_IO_ERROR` | I/O error |
| `-10` | `MC_INTERNAL_ERROR` | Internal error |
| `-25` | `MC_SERVICE_ERROR` | Driver not initialized (retry) |

## Project Structure

- `/Native`: `MultiCamNative.cs` (LibraryImport definitions and constants)
- `/Services`: `VisionService.cs` (Channel control and image acquisition logic)
- `/Calibration`: `CrevisController.cs` (FFC and white balance operations)
- `/Models`: Surface data and signal info structs
