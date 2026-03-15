# Crevis TC-A160K Camera Reference

> Source: Project CLAUDE.md specifications, MultiCam .cam file configurations, hardware layout drawing DWG-NO. 19001, and Crevis Camera Link product references (www.crevis.co.kr, github.com/CREVIS/Camera).

## Contents

1. [General Information](#1-general-information)
2. [Sensor Specifications](#2-sensor-specifications)
3. [Camera Link Interface](#3-camera-link-interface)
4. [Supported Operating Modes](#4-supported-operating-modes)
5. [Cam File Configurations](#5-cam-file-configurations)
6. [Exposure and Gain Control](#6-exposure-and-gain-control)
7. [Calibration](#7-calibration)
8. [White Balance](#8-white-balance)
9. [Key MultiCam Parameters](#9-key-multicam-parameters)
10. [Optical Setup (Project-Specific)](#10-optical-setup-project-specific)

---

## 1. General Information

| Parameter | Value |
|-----------|-------|
| Manufacturer | Crevis Co., Ltd. (www.crevis.co.kr) |
| Model | TC-A160K |
| Variant used in project | TC-A160K-102 (hardware), TC-A160K-SEM (cam file designation) |
| Camera type | Area-scan, progressive |
| Interface | Camera Link (Base configuration) |
| Mount | C-mount |
| Application | Industrial machine vision (peanut discoloration inspection) |

---

## 2. Sensor Specifications

| Parameter | Value |
|-----------|-------|
| Sensor type | CMOS |
| Max resolution | 4160 x 3120 pixels |
| Megapixels | ~13 MP |
| Pixel size | 1.4 um x 1.4 um |
| Scan type | Area scan, progressive |
| Color capability | Color (RGB) and Monochrome modes |
| Bit depth | 8-bit (standard), 10-bit (supported) |

Note: The cam files used in this project configure the camera at a working resolution of 1456 x 1088 pixels in Camera Link Base 1-tap RGB8 mode. The full 4160 x 3120 native resolution requires higher Camera Link bandwidth configurations.

---

## 3. Camera Link Interface

| Parameter | Value |
|-----------|-------|
| Configuration | Base (single cable) |
| Tap configuration | BASE_1T8 (mono) or BASE_1T24 (RGB 8-bit) |
| Tap geometry | 1X_1Y (single tap, single direction) |
| Downstream signals | FVAL (frame valid), LVAL (line valid), DVAL (data valid) |
| Upstream control | CC1-CC4 (camera control lines) |
| Serial communication | Camera Link serial interface |
| Synchronization | Synchronous operation (PxxSC / PxxRG cam config) |

---

## 4. Supported Operating Modes

| Mode | Cam Config | Trigger | Description |
|------|------------|---------|-------------|
| Free-run | PxxSC | IMMEDIATE | Continuous acquisition at camera frame rate |
| Software trigger | PxxRG | SOFT | Single frame per software ForceTrig command |
| Hardware trigger | PxxRG | HARD | Single frame per external trigger signal |
| Combined trigger | PxxRG | COMBINED | Software or hardware trigger |

### Acquisition Modes (MultiCam)

| Acquisition Mode | Suitable | Notes |
|------------------|----------|-------|
| SNAPSHOT | Yes | Single frame per trigger (primary mode for this project) |
| VIDEO | Yes | Continuous free-run capture |
| HFR | Yes | High frame rate, multi-frame per surface |
| PAGE / WEB / LONGPAGE | No | Line-scan only, not applicable |

---

## 5. Cam File Configurations

The following .cam files are available for the TC-A160K-SEM:

| Cam File | Mode | Color Format | Resolution |
|----------|------|-------------|------------|
| TC-A160K-SEM_freerun_RGB8.cam | Free-run | RGB 8-bit (BASE_1T24) | 1456 x 1088 |
| TC-A160K-SEM_freerun_Mono8.cam | Free-run | Mono 8-bit (BASE_1T8) | 1456 x 1088 |
| TC-A160K-SEM_software_RGB8.cam | Software trigger | RGB 8-bit | 1456 x 1088 |
| TC-A160K-SEM_software_Mono8.cam | Software trigger | Mono 8-bit | 1456 x 1088 |
| CREVIS_TC-A160K-SEM_FreeRun_1TAP_RGB8.cam | Free-run | RGB 8-bit (BASE_1T24) | 1456 x 1088 |

### Cam File Key Parameters

```
Imaging       = AREA
Spectrum      = COLOR
TapConfiguration = BASE_1T24   (for RGB8)
TapGeometry   = 1X_1Y
ColorMethod   = RGB
Expose        = INTCTL         (internal exposure control)
Readout       = INTCTL         (internal readout control)
Hactive_Px    = 1456
Vactive_Ln    = 1088
ColorFormat   = Y8 or RGB24
```

---

## 6. Exposure and Gain Control

| Parameter | Type | Description |
|-----------|------|-------------|
| Expose_us | Read/Write | Exposure time in microseconds |
| ExposeMin_us | Read-only | Minimum supported exposure time |
| ExposeMax_us | Read-only | Maximum supported exposure time |
| Gain_dB | Read/Write | Analog gain in decibels |
| ExposeOverlap | Config | FORBID (default) or ALLOW exposure/readout overlap |

Exposure is internally controlled by the camera (INTCTL mode). The frame grabber sends timing via CC1 (reset/expose signal) to the camera.

---

## 7. Calibration

### Flat Field Correction (FFC)

| Operation | Parameter | Procedure |
|-----------|-----------|-----------|
| Black calibration | BlackCalibration = Execute | Cover the lens completely, then execute |
| White calibration | WhiteCalibration = Execute | Illuminate sensor uniformly to ~200 DN, then execute |
| Enable/disable FFC | FlatFieldCorrection = ON / OFF | Toggle correction on or off |

FFC corrects per-pixel gain and offset variations in the sensor. Black calibration establishes the dark reference, and white calibration establishes the illuminated reference. Both must be performed for effective correction.

---

## 8. White Balance

| Parameter | Values | Description |
|-----------|--------|-------------|
| BalanceWhiteAuto | ONCE / CONTINUOUS / OFF | Automatic white balance mode |
| BalanceRatioRed | float | Red channel gain multiplier |
| BalanceRatioGreen | float | Green channel gain multiplier |
| BalanceRatioBlue | float | Blue channel gain multiplier |

For ONCE mode, capture an image of a white reference target (~200 DN average) and the camera will compute channel ratios automatically.

---

## 9. Key MultiCam Parameters

### Board and Connector Setup

```
DriverIndex = 0          (board index for PC1622)
Connector   = "M"        (Medium connector on PC1622)
CamFile     = "TC-A160K-SEM_freerun_RGB8.cam"
```

### Acquisition Control

```
AcquisitionMode = SNAPSHOT
TrigMode        = SOFT
ChannelState    = ACTIVE / IDLE
ForceTrig       = TRIG       (fire software trigger)
SeqLength_Fr    = 1          (frames per sequence)
SurfaceCount    = 4          (recommended buffer count)
```

### Strobe Control

```
StrobeMode = AUTO / NONE
```

When StrobeMode is set to AUTO, the frame grabber automatically generates a strobe output pulse synchronized to the camera exposure. This is used to control the strobe lighting in the inspection system.

---

## 10. Optical Setup (Project-Specific)

This section describes the optical configuration used in the peanut discoloration inspection system (DWG-NO. 19001).

| Parameter | Value |
|-----------|-------|
| Lens | M0814-MP2 (8 mm f/1.4, megapixel) |
| Working distance | 240 +/- 15 mm |
| FOV per camera | 153 (H) x 114 (V) mm |
| Number of cameras | 3 (TC-A160K-102 units) |
| Total FOV | 453 (H) x 114 (V) mm |
| Camera overlap | 2-3 mm between adjacent cameras |
| Lighting | Strobe type, 160 x 600 mm bar light |
| Light WD | 215 +/- 15 mm |
