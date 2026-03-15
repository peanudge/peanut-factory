# MultiCam User Guide Summary

> Source: Euresys MultiCam SDK Documentation — [Classes](https://documentation.euresys.com/Products/MULTICAM/MULTICAM_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/classes.htm), [The Channel Class](https://documentation.euresys.com/Products/MULTICAM/MULTICAM_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/the-channel-class.htm), [MultiCam Signals](https://documentation.euresys.com/Products/MultiCam/MultiCam_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/multicam-signals.htm), [Callback Signaling](https://documentation.euresys.com/Products/MultiCam/MultiCam_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/callback-signaling.htm), [Cluster State](https://documentation.euresys.com/Products/MultiCam/MultiCam_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/cluster-state.htm), [SurfaceState](https://documentation.euresys.com/Products/MULTICAM/MULTICAM_6_16/Content/03_Using_Grablink/Parameters_Reference/parameters/surfacestate.htm), [API Errors](https://documentation.euresys.com/products/multicam/multicam_6_16/Content/04_Using_MultiCam/Multicam_User_Guide/api-errors.htm)

## Contents

- [MultiCam Object Model](#multicam-object-model)
  - [Configuration Class](#configuration-class)
  - [Board Class](#board-class)
  - [Channel Class](#channel-class)
  - [Surface Class](#surface-class)
- [Driver Lifecycle](#driver-lifecycle)
  - [McOpenDriver](#mcopendriver)
  - [McCloseDriver](#mcclosedriver)
  - [Lifecycle Example](#lifecycle-example)
- [Object Creation and Deletion](#object-creation-and-deletion)
- [Parameter System](#parameter-system)
  - [Parameter Access by Name](#parameter-access-by-name)
  - [Parameter Access by ID](#parameter-access-by-id)
  - [Parameter Types](#parameter-types)
- [Signaling](#signaling)
  - [Signal Types](#signal-types)
  - [Enabling Signals](#enabling-signals)
  - [Callback Signaling](#callback-signaling)
  - [Wait Signaling](#wait-signaling)
  - [Advanced Signaling](#advanced-signaling)
- [Channel States and Transitions](#channel-states-and-transitions)
- [Surface Lifecycle](#surface-lifecycle)
  - [Surface States](#surface-states)
  - [State Transition Flow](#state-transition-flow)
  - [Cluster States](#cluster-states)
  - [Multiple Buffer Management](#multiple-buffer-management)

---

## MultiCam Object Model

MultiCam organizes its functionality into four primary classes. Each class serves as a container for related parameters and provides a specific role within the acquisition pipeline.

```
MC_CONFIGURATION (singleton)
    |
    +-- MC_BOARD (one per installed board, auto-created)
    |       |
    |       +-- MC_CHANNEL (user-created, one per acquisition path)
    |               |
    |               +-- MC_SURFACE (user-created or auto-allocated, image buffers)
    |               +-- MC_SURFACE
    |               +-- ...
    |
    +-- MC_BOARD (additional boards)
```

### Configuration Class

The Configuration class (`MC_CONFIGURATION`) is a system-wide singleton that manages universal MultiCam characteristics. It exists automatically when the driver is opened and cannot be created or deleted by user applications.

**Key parameters:**
- `ErrorHandling` — controls error management behavior (`NONE`, `MSGBOX`, `EXCEPTION`, `MSGEXCEPTION`)
- Driver-wide settings affecting all boards and channels

**Handle constant:** `0x20000000`

### Board Class

The Board class (`MC_BOARD`) represents a physical Euresys frame grabber board installed in the host computer. One Board object exists automatically for each detected board. Board objects cannot be created or deleted by user applications.

**Key parameters:**
- Board identification (serial number, firmware version)
- Board-level I/O configuration
- Security key and license status

**Handle construction:** `0xE0000000 | board_index`

### Channel Class

The Channel class (`MC_CHANNEL`) is the central object for image acquisition. A channel represents the association of an individual grabber connected to a camera, delivering data to a set of surfaces called a **cluster**. Channels are created by user applications and provide an independent set of control parameters for each acquisition path.

A channel manages:
- Camera control (reset, exposure, trigger)
- Physical connections (connectors, Camera Link cables)
- Signal routing within the frame grabber
- Digital receiving and de-serializing (for digital cameras)
- Timing generation and video signal conditioning
- DMA transfers to host memory
- Host memory surface cluster management
- External trigger management

**Handle constant (model):** `0x8000FFFF`

### Surface Class

The Surface class (`MC_SURFACE`) represents an image buffer in host memory. Surfaces are organized into clusters managed by their parent channel. They can be automatically allocated by the driver (via `SurfaceCount`) or manually created and registered.

**Key parameters:**
- `SurfaceAddr` — pointer to the image data buffer
- `SurfacePitch` — bytes per row (stride)
- `SurfaceState` — current state in the lifecycle (`FREE`, `FILLING`, `FILLED`, `PROCESSING`, `RESERVED`)
- `SurfaceContext` — user-defined context value for identification

**Handle constant (model):** `0x4000FFFF`

---

## Driver Lifecycle

### McOpenDriver

`McOpenDriver` initiates communication between the application process and the MultiCam driver. It must be the first MultiCam function called.

```csharp
int status = MultiCamNative.McOpenDriver(null); // Parameter must be NULL
```

**Important behaviors:**
- If the MultiCam service is still starting, `McOpenDriver` returns `MC_SERVICE_ERROR` (-25). Implement a retry loop with a delay.
- If called multiple times from the same process, each call must be matched with a corresponding `McCloseDriver` call.
- The function initializes the internal object registry, detects installed boards, and creates Board objects.

### McCloseDriver

`McCloseDriver` terminates the communication with the MultiCam driver. It must be the last MultiCam function called.

```csharp
MultiCamNative.McCloseDriver();
```

**Important behaviors:**
- Must be called once for each successful `McOpenDriver` call.
- Do not call from within a destructor of a global or static object.
- Do not call from within `DllMain` (Windows DLL entry point).
- All channels and surfaces should be deleted before closing the driver.

### Lifecycle Example

```csharp
// Open driver (with retry for MC_SERVICE_ERROR)
int status;
do {
    status = MultiCamNative.McOpenDriver(null);
    if (status == -25) Thread.Sleep(500);
} while (status == -25);

if (status != 0)
    throw new InvalidOperationException($"McOpenDriver failed: {status}");

try
{
    // Create and use channels...
}
finally
{
    // Clean up all channels first, then close driver
    MultiCamNative.McCloseDriver();
}
```

---

## Object Creation and Deletion

User-creatable objects (channels and surfaces) are managed with `McCreate` and `McDelete`.

### McCreate

Creates a new instance of a class and returns a unique handle.

```csharp
MultiCamNative.McCreate(MC_CHANNEL, out uint channelHandle);
MultiCamNative.McCreate(MC_SURFACE, out uint surfaceHandle);
```

Creating an object establishes:
1. A new class instance
2. A unique handle for designating the instance
3. An associated parameter set owned by that instance

### McDelete

Destroys a previously created object and releases its resources.

```csharp
MultiCamNative.McDelete(channelHandle);
```

**Rules:**
- Set `ChannelState` to `IDLE` before deleting a channel.
- Delete surfaces before deleting their parent channel (if manually created).
- Never use a handle after calling `McDelete` on it.

---

## Parameter System

MultiCam uses a parameter-based configuration model. Every aspect of board, channel, and surface behavior is controlled through named or ID-based parameters.

### Parameter Access by Name

Name-based access uses `McSetParamNmXxx` / `McGetParamNmXxx` functions with string parameter names. This is the preferred approach.

```csharp
// Set string parameter
MultiCamNative.McSetParamNmStr(channel, "CamFile", "TC-A160K-SEM_freerun_RGB8.cam");

// Set integer parameter
MultiCamNative.McSetParamNmInt(channel, "DriverIndex", 0);

// Get integer parameter
MultiCamNative.McGetParamNmInt(channel, "ImageSizeX", out int width);

// Get string parameter
byte[] buffer = new byte[256];
MultiCamNative.McGetParamNmStr(channel, "ColorFormat", buffer, 256);
string format = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

// Get pointer parameter
MultiCamNative.McGetParamNmPtr(channel, "SurfaceAddr", out IntPtr addr);
```

### Parameter Access by ID

ID-based access uses `McSetParamInt` / `McGetParamInt` with numeric parameter identifiers. This is required for compound parameters such as `SignalEnable`.

```csharp
// Enable a signal (compound parameter: base ID + signal constant)
uint paramId = MC_SignalEnable + (uint)McSignal.MC_SIG_SURFACE_PROCESSING;
MultiCamNative.McSetParamInt(channel, paramId, 5); // 5 = ON
```

### Parameter Types

| Function Suffix | C# Type | Use Case |
|---|---|---|
| `NmStr` | `string` | Enumerated values, file paths, names |
| `NmInt` | `int` | Numeric settings, counts, indices |
| `NmPtr` | `IntPtr` | Memory addresses (surface data pointers) |
| `Int` (by ID) | `int` | Compound parameters (e.g., `SignalEnable`) |

**Parameter setting order matters.** Some parameters constrain the valid range of others. A recommended configuration order:

1. `DriverIndex` — select the board
2. `Connector` — select the camera connector
3. `CamFile` — load the camera configuration
4. `AcquisitionMode` — set the acquisition mode
5. `TrigMode` / `NextTrigMode` / `EndTrigMode` — configure triggering
6. `SurfaceCount` — allocate buffers
7. Signal enabling — enable required signals
8. `ChannelState` = `ACTIVE` — start acquisition

---

## Signaling

MultiCam uses a signal-based mechanism to notify applications of acquisition events. Three synchronization approaches are available.

### Signal Types

| Signal Constant | Description |
|---|---|
| `MC_SIG_SURFACE_PROCESSING` | A surface is ready for image processing (primary signal) |
| `MC_SIG_SURFACE_FILLED` | A surface buffer has been filled with image data |
| `MC_SIG_ACQUISITION_FAILURE` | An acquisition error occurred |
| `MC_SIG_END_CHANNEL_ACTIVITY` | Channel acquisition has stopped |
| `MC_SIG_UNRECOVERABLE_ERROR` | A fatal, non-recoverable error occurred |
| `MC_SIG_START_OF_FRAME` | Start of a new frame |
| `MC_SIG_END_OF_FRAME` | End of frame transfer |
| `MC_SIG_FRAMETRIGGER_VIOLATION` | Frame trigger timing violation detected |
| `MC_SIG_START_EXPOSURE` | Camera exposure has started |
| `MC_SIG_END_EXPOSURE` | Camera exposure has ended |
| `MC_SIG_ANY` | Wait for any signal (used with `McWaitSignal`) |

### Signal Details and Troubleshooting

#### `MC_SIG_ACQUISITION_FAILURE` — Acquisition Error

This signal is fired when the grabber detects that a frame acquisition could not complete normally. Common root causes:

| Category | Cause | Mechanism |
|----------|-------|-----------|
| **Camera Link 신호** | 케이블 분리/접촉 불량 | FVAL/LVAL/DVAL 신호 단절로 프레임 취득 불가 |
| **Camera Link 신호** | 카메라 전원 꺼짐/재부팅 | 취득 중 카메라 무응답 |
| **Camera Link 신호** | 신호 강도 부족 | 장거리 케이블 또는 EMI로 인한 신호 열화 (`DetectedSignalStrength`로 확인) |
| **트리거 타이밍** | 트리거 간격 < readout 시간 | 이전 프레임 readout이 끝나기 전에 다음 트리거 도착 (`FrameTriggerViolation` 증가) |
| **트리거 타이밍** | `Expose_us` > 프레임 간격 | 노출 시간이 트리거 주기보다 길어 프레임 충돌 |
| **DMA 전송** | PCIe 대역폭 부족 | 다른 디바이스와 PCIe 버스 경합으로 Surface 쓰기 지연 |
| **DMA 전송** | 시스템 메모리 부족 | DMA 타겟 메모리 할당 실패 |
| **Surface Cluster** | Cluster UNAVAILABLE | 모든 Surface가 `FILLING`/`PROCESSING` 상태로 `FREE` Surface 없음 |
| **Surface Cluster** | 콜백 처리 지연 (>1ms) | Surface 회전이 막혀 새 프레임을 받을 버퍼 없음 |
| **카메라 프레임** | 불완전 프레임 | FVAL 활성화 후 예상 라인 수(`Vactive_Ln`)만큼 LVAL 미도달 |

**진단 방법:**

```csharp
void OnSignal(ref McSignalInfo info)
{
    if (info.Signal == (int)McSignal.MC_SIG_ACQUISITION_FAILURE)
    {
        McGetParamNmInt(channel, "GrabberErrors", out int grabErrors);
        McGetParamNmInt(channel, "FrameTriggerViolation", out int ftv);
        McGetParamNmInt(channel, "LineTriggerViolation", out int ltv);

        Log.Error($"Acquisition failure: GrabberErrors={grabErrors}, "
                + $"FTV={ftv}, LTV={ltv}");
    }
}
```

**프로젝트 환경(TC-A160K + Grablink Full)에서의 우선 점검 순서:**

1. 콜백 처리 지연 → Cluster UNAVAILABLE (이미지 처리가 무거울 때)
2. Camera Link 케이블 접촉 불량 (물리적 환경)
3. 트리거 타이밍 위반 (`Expose_us`가 프레임 간격 대비 너무 길 때)

#### `MC_SIG_UNRECOVERABLE_ERROR` — Fatal Error

복구 불가능한 하드웨어 또는 드라이버 수준의 치명적 에러. 이 시그널을 받으면 채널을 `IDLE`로 전환한 후 `McDelete` → `McCloseDriver` 순으로 정리하고, 드라이버를 재초기화해야 한다.

#### `MC_SIG_FRAMETRIGGER_VIOLATION` — Frame Trigger Timing Violation

카메라가 아직 이전 프레임을 readout하고 있는 동안 새 트리거가 도착했음을 의미한다. `FrameTriggerViolation` 카운터가 증가한다. 반복 발생 시 `Expose_us`를 줄이거나 트리거 주기를 늘려야 한다.

### Enabling Signals

Signals must be explicitly enabled before they can be received. Use the `SignalEnable` compound parameter:

```csharp
// Enable Surface Processing signal
uint paramId = MC_SignalEnable + (uint)McSignal.MC_SIG_SURFACE_PROCESSING;
MultiCamNative.McSetParamInt(channel, paramId, 5); // 5 = ON, 4 = OFF
```

Signals can also be enabled using string-based access:

```csharp
// Using McSetParamStr equivalent (where supported)
// Signal identifier constructed as: MC_SignalEnable + signal_constant
```

### Callback Signaling

Callback signaling provides automatic, event-driven notification. A user-defined function is invoked by the driver whenever an enabled signal occurs.

**Callback function signature (C):**
```c
typedef void (MCAPI *PMCCALLBACK)(PMCSIGNALINFO SignalInfo);
```

**Registration:**
```csharp
MultiCamNative.McRegisterCallback(channel, callbackPtr, context);
```

**Key behaviors:**
- A dedicated thread is created for each registered callback.
- The thread remains idle until a signal occurs, then invokes the callback.
- Only one callback function per object is supported.
- If multiple enabled signals occur simultaneously, the callback is invoked successively for each.
- Register callbacks **before** setting `ChannelState` to `ACTIVE`.

**Signal information structure:**
```csharp
[StructLayout(LayoutKind.Sequential)]
public struct McSignalInfo
{
    public IntPtr Context;       // User-defined context from McRegisterCallback
    public uint   Instance;      // Handle of the signaling object (channel)
    public int    Signal;        // Signal type (e.g., MC_SIG_SURFACE_PROCESSING)
    public uint   SignalInfo;    // Signal-specific data (e.g., surface handle)
    public uint   SignalContext;  // Additional context
}
```

For `MC_SIG_SURFACE_PROCESSING` and `MC_SIG_SURFACE_FILLED`, the `SignalInfo` member contains the surface handle, which can be used to retrieve the `SurfaceAddr` and `SurfaceContext` parameters.

**Best practices:**
- Keep callback execution time under 1 ms.
- Use a `ConcurrentQueue` to hand off data from the callback thread to processing threads.
- Never call `McDelete` or `McCloseDriver` from within a callback.
- Do not mix callback and waiting signaling on the same object.

### Wait Signaling

Wait signaling provides synchronous, blocking notification using `McWaitSignal`.

```csharp
int status = MultiCamNative.McWaitSignal(
    channel,
    (int)McSignal.MC_SIG_SURFACE_PROCESSING,
    5000,           // timeout in milliseconds
    out McSignalInfo info
);
```

**Key behaviors:**
- Blocks the calling thread until the specified signal occurs or the timeout expires.
- Returns `MC_TIMEOUT` (-12) if no signal arrives within the timeout period.
- The `McSignalInfo` structure is populated with signal details on success.
- Suitable for simple, single-threaded acquisition loops.

### Advanced Signaling

Advanced signaling integrates with standard OS wait mechanisms (e.g., Windows `WaitForSingleObject`). This approach is used when the application needs to combine MultiCam signals with other OS-level events in a unified wait loop. This is less commonly used in typical applications.

---

## Channel States and Transitions

A channel operates in two primary states controlled by the `ChannelState` parameter:

| State | Description |
|---|---|
| `IDLE` | Channel is configured but not acquiring. Parameters can be modified. |
| `ACTIVE` | Channel is acquiring images. Most parameters are locked. |

**State transitions:**

```
                  McSetParamNmStr("ChannelState", "ACTIVE")
    IDLE  ─────────────────────────────────────────────────>  ACTIVE
      ^                                                          |
      |    McSetParamNmStr("ChannelState", "IDLE")               |
      <──────────────────────────────────────────────────────────+
      |                                                          |
      |    MC_SIG_END_CHANNEL_ACTIVITY (auto-stop)               |
      <──────────────────────────────────────────────────────────+
```

**Rules:**
- Configure all parameters while the channel is `IDLE`.
- Set `ChannelState` to `ACTIVE` to begin acquisition.
- The channel transitions back to `IDLE` either explicitly (by setting `ChannelState` to `IDLE`) or automatically when acquisition completes (signaled by `MC_SIG_END_CHANNEL_ACTIVITY`).
- Attempting to modify locked parameters while `ACTIVE` returns `MC_PARAMETER_ILLEGAL_ACCESS` (-23).
- Always transition to `IDLE` before calling `McDelete`.

---

## Surface Lifecycle

### Surface States

Each surface in a cluster cycles through the following states:

| State | Description |
|---|---|
| `FREE` | The surface is available to receive image data from the grabber. |
| `FILLING` | The surface is currently receiving (or ready to receive) image data. |
| `FILLED` | The surface has finished receiving data and is ready for processing. |
| `PROCESSING` | The surface is being processed by the host application. |
| `RESERVED` | The surface is removed from the standard state transition cycle. |

### State Transition Flow

```
    +-------+       Grabber selects       +---------+
    |  FREE |  ────────────────────────>  | FILLING |
    +-------+                             +---------+
        ^                                      |
        |                             Transfer complete
        |                                      |
        |                                      v
        |        Callback returns        +---------+
        + <──────────────────────────    |  FILLED |
        |                                +---------+
        |                                      |
        |                          Signal dispatched
        |                                      |
        |                                      v
        |        Callback returns       +------------+
        + <─────────────────────────    | PROCESSING |
                                        +------------+
```

**Detailed flow:**
1. **FREE -> FILLING:** The grabber selects the next `FREE` surface in the cluster for the upcoming acquisition phase.
2. **FILLING -> FILLED:** The DMA transfer from the grabber to host memory completes.
3. **FILLED -> PROCESSING:** The `MC_SIG_SURFACE_PROCESSING` signal is dispatched, and the callback begins execution. The surface is protected from overwrite during processing.
4. **PROCESSING -> FREE:** The callback function returns, releasing the surface back to the `FREE` pool.

The `RESERVED` state is a special state that removes a surface from automatic cycling. Surfaces can be manually set to `RESERVED` to hold them for extended processing outside the callback.

### Cluster States

The cluster (the group of surfaces belonging to a channel) has its own aggregate state:

| Cluster State | Condition |
|---|---|
| `OFF` | Acquisition is not active (channel is `IDLE`). |
| `READY` | Acquisition is active, no surface is `PROCESSING`, and the cluster can accept new acquisitions. |
| `BUSY` | Acquisition is active and one surface is in `PROCESSING` state. |
| `UNAVAILABLE` | Acquisition is active but no `FREE` surface is available. This is an exceptional condition. |

When the cluster is `UNAVAILABLE`, the grabber cannot find a `FREE` surface. It will attempt to recycle the oldest `FILLED` surface. If no surface can be recycled (e.g., one is `PROCESSING`), the acquisition frame is dropped and a `MC_SIG_CLUSTER_UNAVAILABLE` signal may be issued.

### Multiple Buffer Management

Using multiple surfaces (recommended: `SurfaceCount >= 2`) provides pipeline parallelism:

```
Time -->

Surface 0:  [FILLING] [FILLED] [PROCESSING] [FREE]          [FILLING] ...
Surface 1:           [FREE]    [FILLING]     [FILLED] [PROCESSING] [FREE] ...
Surface 2:  [FREE]            [FREE]         [FILLING] [FILLED]    ...
```

With 2+ surfaces, the grabber can fill one surface while the application processes another, avoiding frame drops. The recommended minimum is 2 surfaces; 3-4 provides additional safety margin for variable processing times.

```csharp
// Allocate 4 surfaces for robust buffering
MultiCamNative.McSetParamNmInt(channel, "SurfaceCount", 4);
```
