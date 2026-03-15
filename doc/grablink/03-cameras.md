# Cameras

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [Camera Classes](#1-camera-classes)
2. [Camera Operation Modes](#2-camera-operation-modes)
3. [Camera Configurations](#3-camera-configurations)
4. [Camera Tap Properties](#4-camera-tap-properties)
5. [Camera Active Area Properties](#5-camera-active-area-properties)
6. [Bayer CFA Color Registration](#6-bayer-cfa-color-registration)

---

## 1. Camera Classes

In MultiCam, cameras are classified according to four Channel parameters: `Imaging`, `Spectrum`, `DataLink`, and `ColorMethod`.

### Imaging Parameter

The enumerated parameter `Imaging` specifies the geometry of the camera sensor:

| Value | Description |
|-------|-------------|
| `AREA` | The camera is an area-scan model. An area-scan camera is based on a 2D imaging sensor; it delivers individual image frames composed of a fixed amount of lines, each containing a fixed amount of pixels. |
| `LINE` | The camera is a line-scan model. A line-scan camera is based on 1D imaging sensor; it delivers individual image lines composed of a fixed amount of pixels. |
| `TDI` | The camera is a TDI line-scan model. A particular type of line-scan camera using the Time Delayed Integration technology (TDI). |

> **NOTE:** The board's behavior is the same when `Imaging` is TDI instead of LINE; however TDI cameras exhibit less operational modes since they lack an electronic shutter.

### Spectrum Parameter

The enumerated parameter `Spectrum` specifies the spectral sensitivity of the camera:

| Value | Description |
|-------|-------------|
| `BW` | The camera delivers a monochrome image obtained from a sensor operating in the human visible domain. |
| `IR` | The camera delivers a monochrome image obtained from a sensor operating in the infra-red domain. |
| `COLOR` | The camera delivers a color image. |

> **NOTE:** The board's behavior is exactly the same when `Spectrum` is IR instead of BW.

### DataLink Parameter

The enumerated parameter `DataLink` specifies the data transfer method:

| Value | Description |
|-------|-------------|
| `CAMERALINK` | The camera delivers a digital video signal complying to the Camera Link standard. |

### ColorMethod Parameter

The enumerated parameter `ColorMethod` specifies the color analysis method:

| Value | Description |
|-------|-------------|
| `NONE` | The camera delivers a monochrome image. |
| `BAYER` | The camera uses a single imaging sensor coated with a Bayer Color Filter Array and delivers the raw Bayer CFA data. |
| `PRISM` | The camera uses a wavelength-separating prism to feed three distinct imaging sensors. The color information is available as three R, G, B video data streams. |
| `RGB` | The camera uses a coated sensor and an internal processor to reconstruct the full color information as three R, G, B video data streams. |
| `TRILINEAR` | The camera uses three parallel sensing linear arrays of pixels exhibiting different wavelength sensitivities. |

> **NOTE:** This board provides limited support of TRILINEAR cameras since the scan-delay compensation function is not available.

---

## 2. Camera Operation Modes

The classification of the operation modes is based on the following MultiCam parameters: `Expose` and `Readout`.

### Expose Parameter

| Value | Description |
|-------|-------------|
| `INTCTL` | The line or frame exposure condition is totally controlled by the camera. The exposure duration is set through camera configuration settings. |
| `PLSTRG` | The line or frame exposure condition starts upon receiving a pulse from the frame grabber. |
| `WIDTH` | The duration of a pulse issued by the frame grabber determines the line or frame exposure condition. |
| `INTPRM` | The exposure is permanent. |

### Readout Parameter

| Value | Description |
|-------|-------------|
| `INTCTL` | The frame read-out condition is totally controlled by the camera. |

### Supported Area-Scan Camera Operation Modes

| CamConfig | Expose | Readout | Description |
|-----------|--------|---------|-------------|
| `PxxSC` | INTCTL | INTCTL | Synchronous progressive scan, camera-controlled exposure |
| `PxxRC` | PLSTRG | INTCTL | Asynchronous progressive scan, camera-controlled exposure |
| `PxxRG` | WIDTH | INTCTL | Asynchronous progressive scan, grabber-controlled exposure |

### Supported Line-Scan Camera Operation Modes

| CamConfig | Expose | Readout | Description |
|-----------|--------|---------|-------------|
| `LxxxxSP` | INTPRM | INTCTL | Free-running, permanent exposure, no control line |
| `LxxxxSC` | INTCTL | INTCTL | Free-running, camera-controlled exposure, single control line |
| `LxxxxRP` | INTPRM | PLSTRG | Grabber-controlled line rate, permanent exposure, single control line |
| `LxxxxRC` | PLSTRG | INTCTL | Grabber-controlled line rate, camera-controlled exposure, single control line |
| `LxxxxRG` | WIDTH | INTCTL | Grabber-controlled rate and exposure, single control line |
| `LxxxxRG2` | PLSTRG | PLSTRG | Grabber-controlled rate and exposure, two control lines |

### Supported TDI Line-Scan Camera Operation Modes

| CamConfig | Expose | Readout | Description |
|-----------|--------|---------|-------------|
| `LxxxxSP` | INTPRM | INTCTL | Free-running, permanent exposure |
| `LxxxxRP` | INTPRM | PLSTRG | Grabber-controlled line-scanning, permanent exposure |

---

## 3. Camera Configurations

MultiCam embeds predefined camera parameters settings for each available camera operation mode.

The user selects by assigning a value to the `Camera` and `CamConfig` parameters. For all Grablink products:

- `Camera` must be set to `MyCameraLink`
- `CamConfig` must be set to one of the values listed below:

| CamConfig | Camera Class | Camera Operation Mode | CamFile Template |
|-----------|-------------|----------------------|-----------------|
| `PxxSC` | Area-scan | Synchronous progressive scan, camera-controlled exposure | `MyCameraLink_PxxSC.cam` |
| `PxxRC` | Area-scan | Asynchronous progressive scan, camera-controlled exposure | `MyCameraLink_PxxRC.cam` |
| `PxxRG` | Area-scan | Asynchronous progressive scan, grabber-controlled exposure | `MyCameraLink_PxxRG.cam` |
| `LxxxxSC` | Line-scan | Free-running, camera-controlled exposure | N/A |
| `LxxxxRC` | Line-scan | Grabber-controlled camera line rate, camera-controlled exposure | `MyCameraLink_LxxxxRC.cam` |
| `LxxxxRG` | Line-scan | Grabber-controlled camera rate and exposure | `MyCameraLink_LxxxxRG.cam` |
| `LxxxxSP` | (TDI) Line-scan | Free-running, permanent exposure | `MyCameraLink_LxxxxSP.cam` |
| `LxxxxRP` | (TDI) Line-scan | Grabber-controlled camera line rate and exposure | `MyCameraLink_LxxxxRP.cam` |

> **NOTE:** A CamFile is a text file gathering all the relevant Channel parameters.

---

## 4. Camera Tap Properties

### TapConfiguration Parameter

The enumerated parameter `TapConfiguration` declares the Camera Link tap configuration used by the camera. The naming convention is:

```
<Config>_<TapCount>T<BitDepth>(B<TimeSlots>)
```

Where:
- `<Config>` designates the Camera Link configuration: `LITE`, `BASE`, `MEDIUM`, `FULL`, `DECA`
- `<TapCount>` is the total number of pixel taps (1 to 10)
- `<BitDepth>` is the number of bits per tap: 8, 10, 12, 14, 16, 24, 30, 36, 42, 48
- `<TimeSlots>` is the number of consecutive time slots required (2, 3); omitted when 1

**Examples:**
- `BASE_1T8`: Base configuration, 1 tap, 8-bit pixel data
- `BASE_1T24`: Base configuration, 1 tap, 24-bit pixel data (likely RGB)
- `DECA_8T10`: 80-bit configuration, 8 taps, 10-bit pixel data
- `DECA_8T30B3`: 80-bit configuration, 8 taps, 30-bit pixel data (RGB), 3 time slots

### TapGeometry Parameter

The **tap geometry** is a Euresys proprietary taxonomy that describes the geometrical properties characterizing the different taps of a multi-tap camera.

Syntax variants:

1. For cameras delivering two or more rows per readout cycle: `<TapGeometryX>_<TapGeometryY>`
2. For cameras delivering only one row per readout cycle: `<TapGeometryX>`

**TapGeometryX** describes the horizontal organization: `<XRegions>X(<XTaps>)(<ExtX>)`

**TapGeometryY** describes the vertical organization: `<YRegions>Y(<YTaps>)(<ExtY>)`

### Image Reconstruction

For most `TapConfiguration` and `TapGeometry` values, the frame grabber is capable of re-arranging the data in the destination surface to reconstruct the image exactly like it was captured on the camera sensor.

---

## 5. Camera Active Area Properties

The **Camera Active Area** is a rectangular array of pixels containing active video delivered by the camera to the frame grabber.

> **NOTE:** For line-scan cameras, the height of the active area is 1 (or 2 for bilinear line-scan).

### Hactive_Px Parameter

The MultiCam parameter `Hactive_Px` represents the number of pixels in each line of the Camera Active Area. The following rules apply:

- **Rule #1:** The width may contain at most 65535 pixels.
- **Rule #2:** `Hactive_Px` must be a multiple of XTaps (each tap delivers the same amount of pixels every line).
- **Rule #3:** Each XRegion must contain at least `MinBytesPerRegionLine` = 48 bytes.

**BytesPerPixel** depends on camera type:
- 1 byte for 8-bit monochrome and Bayer CFA cameras
- 2 bytes for 10-/12-/14-/16-bit monochrome and Bayer CFA cameras
- 3 bytes for 24-bit RGB cameras
- 6 bytes for 30-/36-/42-/48-bit RGB cameras

### Vactive_Ln Parameter

For area-scan cameras only, `Vactive_Ln` represents the number of lines of the Camera Active Area:

- **Rule #1:** The camera active window may contain at most 65535 lines.
- **Rule #2:** `Vactive_Ln` must be a multiple of YTaps (each tap delivers exactly the same amount of pixels).

---

## 6. Bayer CFA Color Registration

When `ColorMethod` = BAYER, the enumerated parameter `ColorRegistration` specifies the alignment of the color pattern filter over the sensor active area.

Possible values are: `GB`, `BG`, `RG`, `GR`. The two letters indicate respectively the color of the two first pixels of the first line.

| Value | Description |
|-------|-------------|
| `GB` | The first two pixels are green and blue |
| `BG` | The first two pixels are blue and green |
| `RG` | The first two pixels are red and green |
| `GR` | The first two pixels are green and red |

The information is used by MultiCam to automatically configure the Bayer CFA decoder.
