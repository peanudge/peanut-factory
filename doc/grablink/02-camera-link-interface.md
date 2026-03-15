# Camera Link Interface

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [Camera Link Configurations](#1-camera-link-configurations)
2. [Enable Signals](#2-enable-signals)
3. [Video Data Signals](#3-video-data-signals)
4. [Upstream Control Lines](#4-upstream-control-lines)

---

## 1. Camera Link Configurations

### Camera Link 2.1 Configurations

| Configuration | Channel Link Count (Names) | Connectors | Bit Count | Ports Count (Names) |
|--------------|---------------------------|------------|-----------|-------------------|
| Lite | 1 (X) | 1 | 10 | 2 (A, B)* |
| Base | 1 (X) | 1 | 24 | 3 (A, B, C) |
| Medium | 2 (X, Y) | 2 | 48 | 6 (A to F) |
| Full | 3 (X, Y, Z) | 2 | 64 | 8 (A to H) |
| 72-bit | 3 (X, Y, Z) | 2 | 72 | 9 (A to I) |
| 80-bit | 3 (X, Y, Z) | 2 | 80 | 10 (A to J) |

> **NOTE:** (*) Up to 10 bits only

### Supported Camera Link Configurations vs. Grablink Product

| Configuration | Base | DualBase | Full | FullXR |
|--------------|------|----------|------|--------|
| Lite | OK | OK | - | - |
| Base | OK | OK | - | - |
| Medium | - | OK | OK | OK |
| Full | - | OK | OK | OK |
| 72-bit | - | - | OK | OK |
| 80-bit | - | - | OK | OK |

---

## 2. Enable Signals

The Camera Link standard defines 4 enable signals:

- **FVAL** (Frame Valid) is defined HIGH for valid lines.
- **LVAL** (Line Valid) is defined HIGH for valid pixels.
- **DVAL** (Data Valid) is defined HIGH when data is valid.
- **Spare** has been defined for future use.

### Grablink Usage

On Grablink boards, the enable signals are used differently according to the type of acquisition:

**Area-scan Image Acquisition:**
- The rising edge of the FVAL signal is used as a "Start-of-Frame"
- The rising edge of the LVAL signal is used as a "Start-of-Line"
- The DVAL signal can optionally be used as a clock qualifier
- The Spare signal is unused

**Line-scan Image Acquisition:**
- The FVAL signal is unused
- The rising edge of the LVAL signal is used as a "Start-of-Line"
- The DVAL signal can optionally be used as a clock qualifier
- The Spare signal is unused

**Raw Data Acquisition:**
- The FVAL signal is used as a "Frame cover" signal surrounding the data to acquire
- The LVAL signal is unused
- The DVAL signal can optionally be used as a clock qualifier
- The Spare signal is unused

### Enable Signals Timing Diagrams

#### FVAL Timing for Area-Scan Acquisition

When the Grablink board is configured for image acquisition from area-scan cameras:

- The rising edge of FVAL is used as a start of frame signal.
- The falling edge of FVAL is ignored.

If the acquisition trigger is armed when FVAL raises, the board:

1. Skips a pre-defined amount (`VsyncAft_Ln`) of video data lines at begin of frame (BOF)
2. Acquires a pre-defined amount (`Vactive_Ln`) of video data lines
3. Terminates the image acquisition

The subsequent video data lines are all skipped until a new FVAL rising edge occurs.

#### FVAL Timing for Raw Data Acquisition

When the Grablink board is configured for raw data acquisition:

- The rising edge of FVAL starts the data acquisition.
- The falling edge of FVAL stops the data acquisition.

You can configure the frame grabber to use the DVAL signal as a clock qualifier but this feature is rarely used. By default, the configuration ignores the DVAL signal. When DVAL is enabled, the data acquisition is inhibited for clock cycles where DVAL = 0.

#### LVAL Timing Diagram

When the Grablink board is configured for image acquisition from an area-scan camera or from a line-scan camera:

- The rising edge of LVAL is the reference for the horizontal timing
- The falling edge of LVAL is ignored

If, at the rising edge of LVAL, the acquisition of the next line is enabled, the board:

1. Skips a pre-defined amount (`HsyncAft_Tk`) of camera clock cycles at begin of line (BOL)
2. Acquires a pre-defined amount (`Hactive_Px`) pixel data
3. Terminates the line acquisition

A **qualified clock cycle** is defined as:
- Any clock cycle when the frame grabber is configured to ignore DVAL.
- A clock cycle with DVAL = 1 when the frame grabber is configured to use DVAL.

### Enable Signals Configuration

The enumerated parameter `FvalMode` specifies the usage of the FVAL downstream signal:

| Value | Meaning |
|-------|---------|
| `FA` | The rising edge of the FVAL signal is used as a Start of Frame; the falling edge is irrelevant. |
| `FN` | The FVAL signal is ignored by the board. |
| `FC` | Raw grabbing; start/stop recording on FVAL rising/falling edges. |

The enumerated parameter `LvalMode` specifies the usage of the LVAL downstream signal:

| Value | Meaning |
|-------|---------|
| `LA` | The rising edge of the LVAL signal is used as a Start of Line; the falling edge is irrelevant. |
| `LN` | The LVAL signal is ignored by the board. |

The enumerated parameter `DvalMode` specifies the usage of the DVAL downstream signal:

| Value | Meaning |
|-------|---------|
| `DG` | The DVAL signal is a clock qualifier; only the data transmitted during clock cycles with DVAL = 1 are captured by the board. |
| `DN` | The DVAL signal is ignored by the board. |

#### Configuring Enable Signals for Area-Scan Cameras

The following combinations are allowed for image acquisition from area-scan cameras (`Imaging` = AREA):

- **FA-LA-DN** and **FA-LA-DG** are the recommended configurations for capturing fixed size images. The leading (rising) edges of the FVAL and LVAL signals are used as vertical and horizontal synchronization events.
- **FC-LA-DN** and **FC-LA-DG** are the recommended configurations for cameras delivering images having a variable amount of fixed size lines. The FVAL signal is used as a "Frame Cover" signal.

#### Configuring Enable Signals for Line-Scan Cameras

- **FN-LA-DN** and **FN-LA-DG** are the recommended configurations for capturing fixed size lines. The FVAL signal is ignored. The leading (rising) edge of the LVAL signals is used as a start-of-line synchronization event.

#### Configuring Enable Signals for Raw Data Acquisition

- **FC-LN-DN** and **FC-LN-DG** are the recommended configurations for digital devices delivering raw data or camera devices delivering irregularly structured images.

---

## 3. Video Data Signals

### Bit Assignments

Grablink products comply with the following bit assignment tables of section 4 of the Camera Link 2.1 Specification:

- Table 4-1: 8-Bit Modes, Base/Medium/Full
- Table 4-2: 8-Bit Modes, 80-bit
- Table 4-3: 10-Bit Modes, Base/Medium/Full
- Table 4-4: 10-Bit Modes, 80-bit
- Table 4-5: 12-Bit Modes, Base/Medium/Full
- Table 4-6: 14-Bit Modes, Base/Medium/Full/72-bit
- Table 4-7: 16-Bit Modes, Base/Medium/Full
- Table 4-8: 16-Bit Mode, 80-bit
- Table 4-9: Lite Modes

---

## 4. Upstream Control Lines

According to the Camera Link standard, four LVDS signals are reserved for general-purpose camera control. They are defined as camera inputs and frame grabber outputs:

- **Camera Control 1 (CC1)**
- **Camera Control 2 (CC2)**
- **Camera Control 3 (CC3)**
- **Camera Control 4 (CC4)**

Each control line can be configured as:

**Reset signal:** A transition (either rising or falling edge) on this line resets the camera. This action initiates either a new exposure/readout, or a readout of a frame for an area-scan camera or a line for a line-scan camera.

**Expose control signal:** The leading edge (either rising or falling edge) on this line initiates a new exposure. The trailing edge terminates the exposure and initiates the readout. The pulse width is actually the exposure time.

**GPIO:** Usually, CC1 is used for reset or expose signal for asynchronous reset cameras.

> **NOTE:** This board does not provide HDRIVE, VDRIVE signals. Usually, Camera Link cameras do not need genlocking.
