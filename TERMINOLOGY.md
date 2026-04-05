# PeanutVision Terminology

This document defines the canonical vocabulary for PeanutVision.
Use these terms consistently in code, comments, commits, and documentation.

---

## SDK Layer — Euresys MultiCam Concepts

| Term | Definition |
|------|------------|
| **Board** | Physical frame-grabber board (Grablink Full PC1622). Identified by `MC_BOARD` handle. |
| **Channel** (SDK) | The complete end-to-end acquisition path: camera → Camera Link cable → board input → DMA engine → surface cluster in host memory. Modeled as `MC_CHANNEL` in the MultiCam API. One camera = one channel. |
| **Surface** | A single frame buffer inside the surface cluster. Follows the lifecycle `FREE → FILLING → FILLED → PROCESSING → FREE`. |
| **Cluster** | The pool of surfaces allocated for one channel. Controlled by `SurfaceCount`. |
| **Activity** | The outermost acquisition lifecycle unit. Starts when `ChannelState = ACTIVE`; ends on `MC_SIG_END_CHANNEL_ACTIVITY`. |
| **Sequence** | One trigger event → one set of frames. A channel activity contains one or more sequences. |
| **TrigMode** | How a sequence is initiated: `IMMEDIATE` (free-run), `SOFT` (software trigger), `HARD` (hardware signal), `COMBINED`. |
| **AcquisitionMode** | `SNAPSHOT` (trigger-per-frame, used with SOFT/HARD/COMBINED) or `VIDEO` (continuous free-run). |
| **ForceTrig** | MultiCam parameter that fires a software trigger (`McSetParamNmStr(channel, "ForceTrig", "TRIG")`). |

---

## Driver Layer — `PeanutVision.MultiCamDriver`

| Term | Type | Definition |
|------|------|------------|
| **AcquisitionChannel** | `class AcquisitionChannel` | C# wrapper for a single `MC_CHANNEL` handle. Manages its lifecycle (create → configure → start → stop → delete). Exposes `StartAcquisition`, `StopAcquisition`, `SendSoftwareTrigger`, `WaitForFrame`, exposure, and calibration APIs. |
| **AcquisitionChannelOptions** | `class AcquisitionChannelOptions` | Configuration snapshot used to create one `AcquisitionChannel`: cam file path, connector, trigger mode, surface count, callback flag. |
| **AcquisitionChannelManager** | `class AcquisitionChannelManager : IAcquisitionChannelManager` | Factory and driver lifecycle owner. Calls `McOpenDriver` / `McCloseDriver`. Creates and tracks all `AcquisitionChannel` instances. One per process. |
| **IAcquisitionChannelManager** | `interface IAcquisitionChannelManager` | Public contract for channel factory + board inspection. |
| **CamFileService** | `class CamFileService : ICamFileService` | Scans a directory for `.cam` files and parses their metadata into `CamFileInfo`. |
| **CamFileInfo** | `record CamFileInfo` | Parsed metadata from a `.cam` file (model, resolution, spectrum, trigger mode). Used to build `AcquisitionChannelOptions`. |
| **ProfileId** | `record ProfileId(string Value)` | Strongly-typed cam file name used as an acquisition profile identifier. |

---

## API Layer — `PeanutVision.Api.Services`

| Term | Type | Definition |
|------|------|------------|
| **AcquisitionService** | `class AcquisitionService : IAcquisitionService` | Stateful service that owns a persistent `AcquisitionChannel` and manages the **continuous acquisition state machine** (NotAllocated → Idle → Active). Handles frame buffering, statistics, event log, latency measurement, exposure, and calibration delegation. |
| **ChannelState** | `enum ChannelState` | State of the persistent channel in `AcquisitionService`: `NotAllocated` (no channel), `Idle` (channel exists but not acquiring), `Active` (acquiring). |
| **ChannelAction** | `enum ChannelAction` | Actions currently allowed given the channel state: `Start`, `Stop`, `Trigger`, `CaptureOnce`. |
| **CaptureOnceService** | `class CaptureOnceService : ICaptureOnceService` | Performs a **single-shot synchronous capture** using a dedicated temporary channel. Completely independent of `AcquisitionService`'s persistent channel. Cannot run concurrently with itself or while `AcquisitionService` is in `Active` state. |
| **AcquisitionOperationGate** | `class AcquisitionOperationGate` | Thread-safe `Interlocked`-based gate that coordinates mutual exclusion between `CaptureOnceService` and `AcquisitionService`. `CaptureOnceService` acquires the gate during a capture; `AcquisitionService` checks `IsInProgress` before allowing `Start` or `CreateChannel`. |
| **CalibrationManager** | `class CalibrationManager : ICalibrationService` | Delegates flat-field correction (FFC), white balance, and exposure operations to the active `AcquisitionChannel` via `IChannelCalibration` and `IExposureControl`. |
| **TriggerMode** | `record TriggerMode` | API-level value object wrapping `McTrigMode`. Parsed from string (`"soft"`, `"hard"`, `"combined"`, `"immediate"`). |

---

## API Endpoint Vocabulary

| HTTP Route | Meaning |
|------------|---------|
| `POST /api/acquisition/start` | Create a persistent channel and begin continuous acquisition. |
| `POST /api/acquisition/stop` | Stop acquisition (channel remains allocated). |
| `DELETE /api/acquisition` | Release the persistent channel entirely. |
| `POST /api/acquisition/trigger` | Send a software trigger and wait for the next frame (requires Active + SOFT/COMBINED mode). |
| `GET /api/acquisition/latest-frame` | Return the most recently acquired frame as PNG. |
| `POST /api/acquisition/capture-once` | Single-shot capture using a temporary channel (does not affect the persistent channel state). |

---

## Key Design Rules

1. **`AcquisitionChannel` = SDK channel path** — maps 1:1 to a `MC_CHANNEL` handle; never share across threads without locking.
2. **`AcquisitionService` owns one persistent channel** — creates it on `CreateChannel()`, releases it on `ReleaseChannel()` or `Dispose()`.
3. **`CaptureOnceService` is self-contained** — creates its own temporary channel, captures one frame, and immediately destroys the channel. It never touches `AcquisitionService`'s channel.
4. **`AcquisitionOperationGate` prevents races** — ensures `CaptureOnce` and `AcquisitionService.Start` cannot overlap.
5. **`ChannelState.NotAllocated`** — the initial and post-release state; means no driver-level channel exists.
