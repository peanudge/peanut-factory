# Video Capture

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [Grablink Acquisition Modes](#1-grablink-acquisition-modes)
2. [SNAPSHOT Acquisition Mode](#2-snapshot-acquisition-mode)
3. [HFR Acquisition Mode](#3-hfr-acquisition-mode)
4. [WEB Acquisition Mode](#4-web-acquisition-mode)
5. [PAGE Acquisition Mode](#5-page-acquisition-mode)
6. [LONGPAGE Acquisition Mode](#6-longpage-acquisition-mode)
7. [Setting Optimal Page Length](#7-setting-optimal-page-length)
8. [Hardware Trigger](#8-hardware-trigger)
9. [Hardware End Trigger](#9-hardware-end-trigger)
10. [Exposure Control](#10-exposure-control)
11. [Strobe Control](#11-strobe-control)
12. [Line-Scan Synchronization](#12-line-scan-synchronization)
13. [Line Trigger](#13-line-trigger)

---

## 1. Grablink Acquisition Modes

### Area-Scan Cameras

| Acquisition Mode | Description |
|-----------------|-------------|
| `SNAPSHOT` | Intended for the acquisition of snapshot images. |
| `HFR` | Intended for the acquisition of snapshot images from high frame rate cameras. |

### Line-Scan Cameras

| Acquisition Mode | Description |
|-----------------|-------------|
| `WEB` | Intended for image acquisition of single continuous objects of any size. |
| `PAGE` | Intended for image acquisition of multiple discrete objects having a fixed size. Page size configurable up to 65,535 lines. |
| `LONGPAGE` | Intended for image acquisition of multiple discrete objects having, possibly, a variable and/or a larger size. Supports objects up to 2,147,483,648 lines. |

The user selects the mode by assigning the appropriate value to the `AcquisitionMode` parameter before any other acquisition control parameter.

---

## 2. SNAPSHOT Acquisition Mode

The **SNAPSHOT** acquisition mode is the default mode for all area-scan cameras. A unique sequence acquires `SeqLength_Fr` frames within the channel activity period.

### Configuration Parameters

| Parameter | Value Range |
|-----------|------------|
| `AcquisitionMode` | `SNAPSHOT` |
| `TrigMode` | `IMMEDIATE`, `HARD`, `SOFT`, `COMBINED` |
| `NextTrigMode` | `SAME`, `REPEAT`, `HARD`, `SOFT`, `COMBINED` |
| `SeqLength_Fr` | `MC_INDETERMINATE`, 1 to 65534 |

When invoking SNAPSHOT mode:
- `ActivityLength` is enforced to 1 (channel goes inactive at completion)
- `PhaseLength_Fr` is enforced to 1 (single frame per acquisition phase)
- `TrigMode` establishes the starting condition (default: `IMMEDIATE`)
- `NextTrigMode` establishes subsequent phase starting condition (default: `SAME`)

### Activating the Channel

Set `ChannelState` to `ACTIVE` to activate the channel and arm the trigger circuit. The sequence starts after the first trigger event.

### Stopping

**Manual stop:** Set `ChannelState` = `IDLE`. The acquisition terminates ALWAYS at a frame boundary ensuring integrity.

**Automatic stop:** Set `SeqLength_Fr` to the desired number of frames. The sequence stops automatically after the last frame. An indefinite sequence (MC_INDETERMINATE) stops when the channel is forced to IDLE.

### Monitoring

- `Elapsed_Fr` -- number of acquired frames
- `Remaining_Fr` -- remaining frames (when `SeqLength_Fr` is defined)
- `PerSecond_Fr` -- measured average frame rate (when `SeqLength_Fr` > 1)

---

## 3. HFR Acquisition Mode

The **HFR** (High Frame Rate) mode is for acquiring images from high frame rate cameras (up to 1,275,000 fps). The sequence is divided into phases, each phase acquiring `PhaseLength_Fr` frames into a single destination surface.

### Configuration Parameters

| Parameter | Value Range |
|-----------|------------|
| `AcquisitionMode` | `HFR` |
| `TrigMode` | `IMMEDIATE`, `HARD`, `SOFT`, `COMBINED` |
| `NextTrigMode` | `SAME`, `REPEAT`, `HARD`, `SOFT`, `COMBINED` |
| `SeqLength_Fr` | `MC_INDETERMINATE`, 1 to (`PhaseLength_Fr` x 65,534) |
| `PhaseLength_Fr` | 1 to 255 |

The minimal applicable value of `PhaseLength_Fr` is the camera frame rate divided by 5000.

### Monitoring

- `Elapsed_Fr` -- number of acquired frames in the sequence
- `Remaining_Fr` -- remaining frames (when sequence length is defined)
- `PerSecond_Fr` -- measured average frame rate

---

## 4. WEB Acquisition Mode

The **WEB** mode is the default for line-scan cameras. It acquires `SeqLength_Ln` contiguous lines, divided into phases of `PageLength_Ln` lines each.

### Configuration Parameters

| Parameter | Value Range |
|-----------|------------|
| `AcquisitionMode` | `WEB` |
| `TrigMode` | `IMMEDIATE`, `HARD`, `SOFT`, `COMBINED` |
| `BreakEffect` | `FINISH`, `ABORT` |
| `PageLength_Ln` | 1 to 65535 |
| `SeqLength_Ln` | `MC_INDETERMINATE`, 1 to (`PageLength_Ln` x 65,535) |

Key behaviors:
- `NextTrigMode` is enforced to `REPEAT` (no lines missed between phases)
- `EndTrigMode` is enforced to `AUTO`
- `BreakEffect` = `FINISH` (default) ensures integrity of the last page; `ABORT` stops immediately at line boundary

### Sequence Operation

Once started, the frame grabber acquires data lines continuously. When a surface is filled, acquisition continues automatically into the next available surface without any missing lines. The `MC_SIG_SURFACE_FILLED` signal is reported each time a surface is filled.

`LineIndex` reflects the number of lines already written in a partially filled surface.

### Monitoring

- `Elapsed_Ln` -- number of acquired lines
- `Remaining_Ln` -- remaining lines (when sequence length is defined)

---

## 5. PAGE Acquisition Mode

The **PAGE** mode acquires multiple discrete objects of fixed size. Each phase captures one object (`PageLength_Ln` lines).

### Configuration Parameters

| Parameter | Value Range |
|-----------|------------|
| `AcquisitionMode` | `PAGE` |
| `TrigMode` | `IMMEDIATE`, `HARD`, `SOFT`, `COMBINED` |
| `NextTrigMode` | `SAME`, `REPEAT`, `HARD`, `SOFT`, `COMBINED` |
| `BreakEffect` | `FINISH`, `ABORT` |
| `PageLength_Ln` | 1 to 65535 |
| `SeqLength_Pg` | `MC_INDETERMINATE`, 1 to 65535 |

A programmable page delay (`PageDelay_Ln`) compensates trigger advance from a position detector placed away from the camera field of view.

### Monitoring

- `Elapsed_Pg` -- number of acquired pages
- `Remaining_Pg` -- remaining pages (when sequence length is defined)

---

## 6. LONGPAGE Acquisition Mode

The **LONGPAGE** mode supports multiple discrete objects with possibly variable and/or larger size (up to 2^31 lines).

### Configuration Parameters

| Parameter | Value Range |
|-----------|------------|
| `AcquisitionMode` | `LONGPAGE` |
| `TrigMode` | `IMMEDIATE`, `HARD`, `SOFT`, `COMBINED` |
| `NextTrigMode` | `SAME`, `REPEAT`, `HARD`, `SOFT`, `COMBINED` |
| `EndTrigMode` | `AUTO`, `HARD` |
| `BreakEffect` | `FINISH`, `ABORT` |
| `PageLength_Ln` | 1 to 65535 |
| `SeqLength_Ln` | `MC_INDETERMINATE`, 1 to (`PageLength_Ln` x 65,535) |

Key behaviors:
- `ActivityLength` is enforced to `INDETERMINATE` (channel remains active at completion)
- `EndTrigMode` = `AUTO` terminates after specified frames; `HARD` terminates upon external End Trigger event

### Stopping

**Fixed size objects:** Set `SeqLength_Ln` to the desired number of lines. Conditions: `SeqLength_Ln` > 0, < (`PageLength` x 65535), < 2^31.

**Variable size objects:** Use a position detector to generate an end trigger condition via `EndTrigLine`, `EndTrigEdge`, and `EndTrigCtl` parameters.

### Monitoring

- `Elapsed_Ln` -- number of acquired lines
- `Remaining_Ln` -- remaining lines (when sequence length is defined)

---

## 7. Setting Optimal Page Length

For line-scan acquisition modes, `PageLength_Ln` specifies lines per surface.

**Rule 1:** Hardware limits `PageLength_Ln` to a 16-bit value (max 65535).

**Rule 2:** The maximum surface transition rate should not exceed 1 kHz.

**Rule 3:** The maximum number of pages per sequence is 65,535.

**Recommendation:** For optimal on-board buffer usage, each surface should be 1-4 MB of data.

---

## 8. Hardware Trigger

### About Hardware Trigger Event Sources

**Area-scan (SNAPSHOT/HFR):** A frame trigger instructs the frame grabber to take control over the camera and perform a frame acquisition. Usually issued by a position sensor indicating when the object is in the field of view.

**Line-scan (WEB/PAGE/LONGPAGE):** A page trigger instructs the frame grabber to acquire a set of successive lines. Usually used when a moving object is about to enter the field of view.

Each channel elaborates a clean trigger event using hardware resources: source multiplexer, edge detector, noise filter, delay line, and decimation filter.

### Trigger Control Parameters

| Parameter | Description |
|-----------|-------------|
| `TrigCtl` | Electrical style of trigger source |
| `TrigEdge` | Polarity: `GOHIGH` (default) or `GOLOW` |
| `TrigFilter` | Noise filter: `OFF` (100ns), `ON`/`MEDIUM` (500ns), `STRONG` (2.5us, default) |
| `TrigDelay_us` | Time delay for area-scan (0 to 2,000,000 us) |
| `PageDelay_Ln` | Line delay for line-scan (0 to 65,534 lines) |
| `TrigLine` | Source port selection |
| `TrigDelay_Pls` | Pulse decimation after sequence start (0 to 65,536) |
| `NextTrigDelay_Pls` | Pulse decimation between phases (0 to 65,536) |

> **TIP:** To avoid unexpected loss of trigger events, check that the selected filter time constant is shorter than the trigger pulse width.

---

## 9. Hardware End Trigger

The end trigger is available when `EndTrigMode` = `HARD` for the LONGPAGE acquisition mode.

### End Trigger Control Parameters

| Parameter | Description |
|-----------|-------------|
| `EndTrigCtl` | Electrical style |
| `EndTrigEdge` | Polarity: `GOHIGH` or `GOLOW` (default) |
| `EndTrigFilter` | Noise filter (same options as `TrigFilter`) |
| `EndTrigEffect` | `FOLLOWINGLINE` (acquires next line then stops) or `PRECEDINGLINE` (stops immediately) |
| `EndPageDelay_Ln` | Line delay (0 to 65,534) |
| `EndTrigLine` | Source port selection |

> **NOTE:** The `PRECEDINGLINE` value is not allowed for Bayer bi-linear line-scan cameras.

---

## 10. Exposure Control

### Grabber-Controlled Exposure

Supported camera operation modes:

| Imaging | CamConfig | Description |
|---------|-----------|-------------|
| AREA | `PxxRG` | Asynchronous progressive scan, grabber-controlled exposure |
| LINE | `LxxxxRG` | Grabber-controlled line-scanning and exposure |
| LINE | `LxxxxRG2` | Grabber-controlled, dual signal |
| LINE | `LxxxxRP` | Grabber-controlled line-scanning, permanent exposure |
| TDI | `LxxxxRP` | Grabber-controlled line-scanning, permanent exposure |

Specify the exposure time through `Expose_us` and optionally `ExposeTrim`. Range: 1 us to 5 s. Limits enforced by `ExposeMin_us` and `ExposeMax_us`.

The grabber controls exposure through the reset line (and optionally the auxiliary reset line for `LxxxxRG2`). Any of the four Camera Link upstream control lines (CC1-CC4) can be used, automatically selected via `CC1Usage`-`CC4Usage`.

### Uncontrolled (Camera-Controlled) Exposure

For `PxxSC`, `PxxRC`, `LxxxxSP`, `LxxxxSC`, `LxxxxRC` modes: specify the actual exposure time through `TrueExp_us` (range: 1 us to 20 s).

---

## 11. Strobe Control

Each MultiCam acquisition channel embeds one strobe controller.

### Function Availability

| Camera Mode | Strobe Available |
|-------------|-----------------|
| `PxxRC`, `PxxRG` | Yes |
| `LxxxxRC`, `LxxxxRG`, `LxxxxRG2`, `LxxxxRP` | Yes |
| `PxxSC`, `LxxxxSC`, `LxxxxSP` | No |

### Mode Control

| StrobeMode | Description |
|------------|-------------|
| `NONE` | Disabled; no strobe line allocated |
| `MAN` | Enabled with manual timing control |
| `AUTO` | Enabled with automatic timing control |
| `OFF` | Designated strobe line set to inactive level |

### Duration and Position Controls

| Parameter | Value Range | Default |
|-----------|------------|---------|
| `StrobeDur` | 1 to 100 (% of exposure time) | 50 |
| `StrobePos` | 0 to 100 (position within exposure) | 50 |

- `StrobePos` = 0: leading edge simultaneous with start of exposure
- `StrobePos` = 100: trailing edge simultaneous with end of exposure
- `StrobePos` = 50: strobe pulse centered in exposure period

---

## 12. Line-Scan Synchronization

### Line Capture Modes

| LineCaptureMode | Description |
|-----------------|-------------|
| `ALL` | **Take-All-Lines** (default). Acquires all lines delivered by the camera. Camera line rate must match downweb rate. |
| `PICK` | **Pick-A-Line**. Each pulse at the downweb line rate determines acquisition of the next camera line. Allows constant camera rate with variable downweb rate. |
| `TAG` | **Tag-A-Line**. Camera runs at constant rate. First pixel replaced by a tag indicating whether a hardware event preceded the line. |

### Line Rate Modes

| LineRateMode | Description |
|--------------|-------------|
| `CAMERA` | Downweb line rate originated by the camera |
| `PULSE` | Downweb line rate from line trigger input pulses |
| `CONVERT` | Downweb line rate from line trigger input through a P/Q rate converter |
| `PERIOD` | Downweb line rate from internal periodic generator |
| `EXPOSE` | Downweb line rate established by exposure time settings |

---

## 13. Line Trigger

The **line trigger** signal triggers the acquisition of one line of the image. It is available only for WEB, PAGE, and LONGPAGE acquisition modes.

### Source Path Selector

Controlled by `LineRateMode`:
- `PULSE` -- generated by quadrature decoder through a 1/N rate divider
- `CONVERT` -- generated by quadrature decoder through a P/Q rate converter
- `PERIOD` -- generated by a periodic pulse generator
- `CAMERA` -- generated by the camera through LVAL
- `EXPOSE` -- generated by the exposure control circuit

### Rate Divider

Generates line trigger at frequency 1/N of the quadrature decoder output. `N` is programmable (1 to 512, default 1) via `RateDivisionFactor`.

### Rate Converter

Generates line trigger proportional to quadrature decoder frequency. The rate conversion ratio `RCR = EncoderPitch/LinePitch` ranges from 0.001 to 1000.0 (recommended: 0.01 to 1). Operating range upper limit set by `MaxSpeed`, lower limit reported by `MinSpeed`.

### Periodic Generator

Generates line trigger at constant frequency. Period `T` (1 us to 5 s) configured by `Period_us` and `PeriodTrim`.

### Quadrature Decoder

Interfaces with dual output phase quadrature incremental motion encoders. Features:

- **Edge Selection** via `LineTrigEdge`: rising A, falling A, all A, or all A+B edges
- **Direction Selector**: defines which phase relationship is the forward direction
- **Backward Motion Cancellation**: `BackwardMotionCancellationMode` with filter mode (F-Mode) or compensation mode (C-Mode, 16-bit counter)

### Signal Conditioning

Hardware trigger and end trigger signals from differential inputs are sampled at 50 MHz and filtered:

| TrigFilter / EndTrigFilter | TLOW (ns) | THIGH (ns) |
|---------------------------|-----------|------------|
| `OFF` | 96 | 112 |
| `ON` / `MEDIUM` | 496 | 512 |
| `STRONG` (default) | 2496 | 2512 |

> **NOTE:** `TrigFilter` and `EndTrigFilter` are not relevant for isolated inputs.
