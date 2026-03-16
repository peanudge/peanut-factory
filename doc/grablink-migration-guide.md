# Migration Guide for Grablink Series

> Source: Euresys Grablink Migration Guide

## Contents

- [Introduction](#introduction)
- [About the Grablink Boards Differences](#about-the-grablink-boards-differences)
  - [1. Note About the Three Generations of Grablink Boards](#1-note-about-the-three-generations-of-grablink-boards)
  - [2. Features Summary](#2-features-summary)
  - [3. Features Commonality](#3-features-commonality)
  - [4. Features Differences Overview](#4-features-differences-overview)
  - [5. MultiCam Parameters Differences](#5-multicam-parameters-differences)
- [Migration Paths](#migration-paths)
  - [1. General-Purpose Frame Grabbers](#1-general-purpose-frame-grabbers)
  - [2. Grablink Quickpack Frame Grabbers](#2-grablink-quickpack-frame-grabbers)

---

## Introduction

This document targets system designers having some experience with the first generations of Grablink boards, and willing to use Grablink Full, Grablink DualBase or Grablink Base.

Grablink Full, Grablink DualBase and Grablink Base are designed to facilitate the migration of existing system designs using Grablink boards from previous generations. However some migration issues should be considered.

This document provides:

- An overview of the entire Grablink series
- A highlight of the differences and potential migration issues
- An enumeration of the possible migration paths

---

## About the Grablink Boards Differences

### 1. Note About the Three Generations of Grablink Boards

Technologically speaking, the Grablink series embraces three successive generations of boards:

**1st Generation:**
Includes Grablink Value, Grablink Expert 2, and Grablink Quickpack ColorScan.
All these boards use 66 MHz Channel Link receivers from National Semiconductor, Single Data Rate memories, a conventional PCI 32-bit or 64-bit interface, and Xilinx Spartan II FPGA technology.
The Camera Link clock rate is limited at 50 or 60 MHz.

**2nd Generation:**
Includes Grablink Avenue, Grablink Express, Grablink Quickpack CFA, and Grablink Quickpack CFA PCIe.
All these boards use 85 MHz Channel Link receivers from National Semiconductor, DDR-200 memories, a conventional PCI or PCI Express interface, and Xilinx Spartan 3 FPGA technology.

**3rd Generation:**
Includes Grablink Full, Grablink DualBase, and Grablink Base.
All these boards use 85 MHz enhanced Channel Link receivers, DDR2-667 memories, a PCI Express interface, and Xilinx Spartan 6 FPGA technology.

### 2. Features Summary

The following table provides an overview of the distinctive features of all Grablink boards.

#### Host Interface

| Feature | Grablink Value | Grablink Expert 2 | Grablink QP ColorScan | Grablink Avenue | Grablink Express | Grablink QP CFA | Grablink QP CFA PCIe | Grablink Full | Grablink DualBase | Grablink Base |
|---------|---------------|-------------------|----------------------|-----------------|-----------------|-----------------|----------------------|---------------|-------------------|---------------|
| Interface type | PCI | PCI | PCI | PCI | PCIe | PCI | PCIe | PCIe | PCIe | PCIe |
| Bus width/lanes | 32 | 64 | 64 | 64 | x1 | 64 | x4 | x4 | x4 | x1 |
| Frequency/Rate | 33 MHz | 66 MHz | 66 MHz | 66 MHz | 2.5 GHz | 66 MHz | 2.5 GHz | 2.5 GHz | 2.5 GHz | 2.5 GHz |
| PCI throughput (MB/s)\* | 90 | 240 | 240 | 240 | 180 | 240 | 500 | 850 | 850 | 200 |

\* Application- and motherboard-dependent.

#### Memory and Camera Link

| Feature | Grablink Value | Grablink Expert 2 | Grablink QP ColorScan | Grablink Avenue | Grablink Express | Grablink QP CFA | Grablink QP CFA PCIe | Grablink Full | Grablink DualBase | Grablink Base |
|---------|---------------|-------------------|----------------------|-----------------|-----------------|-----------------|----------------------|---------------|-------------------|---------------|
| On-board memory (MB) | 8 | 16 | 32 | 32 | 32 | 128 | 128 | 128 | 64 (x2) | 64 |
| Number of cameras | 1 | 2/1\*1 | 1 | 1 | 1 | 1 | 1 | 1 | 2 | 1 |
| CL BASE config | Y | Y | Y | Y | Y | Y | Y | Y | Y | Y |
| CL MEDIUM config | - | Y | - | - | - | - | - | Y | - | - |
| CL FULL config | - | - | - | - | - | - | - | Y | - | - |
| 10-tap config | - | - | - | - | - | - | - | Y | - | - |
| Multiplexed taps | - | Y | - | Y | Y | Y | Y | - | - | - |
| CL max data rate (MHz) | 60 | 60 | 60\*8 | 85 | 85 | 85 | 85 | 85 | 85 | 85 |
| Connector type\*2 | MDR | MDR | MDR | MDR | MDR | MDR | MDR | SDR | SDR | SDR |
| PoCL SafePower | - | - | - | - | Y | - | Y | - | Y | Y |

#### Acquisition Features

| Feature | Grablink Value | Grablink Expert 2 | Grablink QP ColorScan | Grablink Avenue | Grablink Express | Grablink QP CFA | Grablink QP CFA PCIe | Grablink Full | Grablink DualBase | Grablink Base |
|---------|---------------|-------------------|----------------------|-----------------|-----------------|-----------------|----------------------|---------------|-------------------|---------------|
| Line-scan acquisition | Y | Y | Y\*4 | Y | Y | - | - | Y | Y | Y |
| Single-output encoder | Y | Y | Y | Y | Y | - | - | Y | Y | Y |
| Quadrature encoder | - | - | - | - | - | - | - | Y | Y | Y |
| ADR | - | - | - | Y | Y | - | - | - | - | - |
| Shading correction | - | - | Y | - | - | - | - | - | - | - |
| Scan-delay compensation | - | - | Y | - | - | - | - | - | - | - |
| Area-scan acquisition | Y | Y | - | Y | Y | Y | Y | Y | Y | Y |
| Bayer CFA decoder | - | - | - | - | - | Y | Y | Y | Y | Y |
| Automatic white balance | - | - | - | - | - | Y | Y | - | - | - |
| Luminance blender | - | - | - | - | - | Y | - | - | - | - |
| Multiple WOI support | - | Y | - | - | - | - | - | - | - | - |

#### LUT Operators

| Feature | Grablink Value | Grablink Expert 2 | Grablink QP ColorScan | Grablink Avenue | Grablink Express | Grablink QP CFA | Grablink QP CFA PCIe | Grablink Full | Grablink DualBase | Grablink Base |
|---------|---------------|-------------------|----------------------|-----------------|-----------------|-----------------|----------------------|---------------|-------------------|---------------|
| Number of LUT operators\*6,7 | 3 | - | 3 | - | - | 4 | 4 | 3 | 3 (x2) | 3 |
| Max LUT input bit depth | 8 | - | 8 | - | - | 16 | 16 | 12 | 12 | 12 |
| Max LUT output bit depth | 8 | - | 8 | - | - | 16 | 16 | 16 | 16 | 16 |

#### I/O Ports

| Feature | Grablink Value | Grablink Expert 2 | Grablink QP ColorScan | Grablink Avenue | Grablink Express | Grablink QP CFA | Grablink QP CFA PCIe | Grablink Full | Grablink DualBase | Grablink Base |
|---------|---------------|-------------------|----------------------|-----------------|-----------------|-----------------|----------------------|---------------|-------------------|---------------|
| Differential inputs | - | - | - | - | - | - | - | 2 | 2 (x2) | 2 |
| Isolated input | - | - | - | 2 | 2 | 2 | 2 | 4 | 4 (x2) | 4 |
| Isolated output | - | - | - | - | - | - | - | 4 | 4 (x2) | 4 |
| Isolated bidir. I/O | 2 | 4 | 2 | 1 | 1 | 1 | 1 | - | - | - |
| Non-isolated input | 1 | 1 | - | 2 | 2 | 2 | 2 | - | - | - |
| Non-isolated output | 1 | 1 | - | - | - | - | - | - | - | - |
| Non-isolated bidir. I/O | - | 16\*5 | - | 4 | 4 | 4 | 4 | - | - | - |

#### Notes

- \*1 Grablink Expert 2 accepts 1 Medium or 2 Base cameras.
- \*2 MDR = Standard Camera Link connector; SDR = Miniature Camera Link connector, aka HDR.
- \*3 Grablink Full supports all standard Camera Link configurations, and, in addition, the DECA (10-tap) non-standard configuration.
- \*4 Grablink Quickpack ColorScan supports exclusively color line-scan cameras.
- \*5 Available on the Grablink Expert 2 I/O extension board.
- \*6 LUT operation on RGB pixels requires three LUT operators, one per component.
- \*7 The fourth LUT operator allows LUT transformation on the Y component issued by the luminance blender.
- \*8 The maximum processing rate is 50 MPixels/s.
- \*9 Application- and motherboard-dependent.

### 3. Features Commonality

#### 3.1 MultiCam Driver

All Grablink boards share a common driver: MultiCam. Version 6.7 of MultiCam supports all the above listed Grablink boards. The unified driver greatly facilitates the migration of existing application software.

#### 3.2 CamFiles

Thanks to the large commonality of MultiCam parameters and parameter values between all boards from the Grablink series, the migration of existing CamFiles developed for boards of the 1st or 2nd generation can be easily accomplished. Only a few parameters settings are required to perform an update. These settings are listed later in this document: see [MultiCam Parameters Differences](#5-multicam-parameters-differences).

MultiCam is delivered with CamFiles templates that cover all usage cases for both line-scan and area-scan cameras. These templates are compatible with all Grablink boards. They can further be customized for a particular camera and/or board-specific features.

Furthermore, Euresys provides a library of CamFiles to interface a large number (> 450) of Camera Link cameras with Grablink boards. Typically, the same CamFile is applicable to all Grablink boards. The CamFiles are available for download from the Euresys web site: <http://www.euresys.com/CamFiles/CamFile.asp>

The library of CamFiles also contains all the camera interfaces that were previously available as "built-in" CamFiles.

### 4. Features Differences Overview

This section summarizes the functional differences between Grablink Full, Grablink DualBase and Grablink Base, and previous Grablink boards.

#### 4.1 New Features

This section lists the new, upgraded, and synchronized features brought about Grablink Full, Grablink DualBase and Grablink Base.

**Camera Link Interface:**

- **Improved Channel Link receivers** -- Cost reduction, better performance in terms of maximum cable length.
- **Miniature Camera Link SDR connectors** -- Leave enough room on the bracket for a robust System I/O connector.
- **Camera Link Full configuration including 10-tap** -- The full range of Camera Link configurations is now covered.
- **Additional tap geometries** -- Extends the coverage of the image reconstruction engine allowing support for more camera types.

**I/O Ports:**

- **Individually isolated inputs and outputs** -- Greatly facilitates system interconnection.
- **High-speed RS-422 differential inputs** -- Designed for motion encoders including the fastest ones.
- **Standardized Internal I/O connector** -- Facilitates the migration between Grablink Full, Grablink DualBase and Grablink Base.
- **On bracket External I/O connector** -- Greatly facilitates the system interconnections using robust SubD connectors.
- **5V@1A and 12V@1A power outputs with electronic fuse protection** -- Capability to safely provide power to external devices such as encoders, sensors...

**Processing Functions:**

- **Bayer CFA decoder** -- This significantly off-loads the host CPU.
- **12-bit look-up table operator** -- Performs real-time look-up table transformation on both monochrome and RGB color pixels having a bit depth of 8-, 10-, or 12 bits.

**Area-Scan Acquisition Control:**

- **Trigger decimation** -- Provides the capability to keep 1-out-of-N edge transitions on the trigger source signal.

**Line-Scan Acquisition Control:**

- **Rate divider** -- Provides the capability to keep 1-out-of-N edge transitions on the line trigger source signal.
- **Support for quadrature incremental motion encoders** -- Provides the capability to detect the direction of motion and to reach faster line trigger rates.
- **Backward motion cancellation** -- Provides the capability to discard camera lines that correspond to a transitional backward motion.

**PCI Interface:**

- **PCI Express** -- Ensures compatibility with the latest PC motherboards and provides better throughput performance.
- **DMA with 64-bit addressing** -- Capability to transfer image data over the whole physical address space of 64-bit systems.

#### 4.2 Discarded Features

This section lists the discarded features for Grablink Full, Grablink DualBase and Grablink Base.

**Built-In Camera Interfaces:**

- **Built-in camera interfaces (using explicit camera names)** -- They are to be replaced by their equivalent obtained from the Download Area of the Euresys web site.

> **Note:** On Grablink Base, Grablink DualBase, and Grablink Full, the allowed value for the `Camera` parameter is `MyCameraLink` and the allowed values for the `CamConfig` parameter are `PxxSC`, `PxxRC`, `PxxRG`, `LxxxxSC`, `LxxxxSP`, `LxxxxRC`, `LxxxxRG`, and `LxxxxRG2`. All CamFiles have been adapted accordingly and can be downloaded from the Euresys web site: <http://www.euresys.com/CamFiles/CamFile.asp>. Other boards are not concerned but it is however recommended to use the latest available CamFile in each case.

**Camera Link Interface:**

- **The Camera Link MDR connectors** -- The camera cable has to be replaced by a model that is terminated at the frame grabber side by a Miniature Camera Link 26-pin connector, aka SDR (Shrunk Delta Ribbon) or HDR (Honda Delta Ribbon) connectors. As usual, a careful selection of the cable needs to be done, particularly in case of high clock rates (> 40 MHz) and long cables (> 3 meters).
- **The multiplexed tap configurations** (introduced with the second generation of Grablink boards) -- There is no significant drawback since such configurations are not effectively used by industrial cameras.

**I/O Ports:**

- **TTL, ITTL, I12V, LVDS electrical styles and corresponding MultiCam parameter values** -- A redesign of the I/O connections may be required to fit the new ISO and DIFF electrical styles. An update of the CamFiles of existing camera interfaces is also required.
- **Isolated power outputs on System I/O connector** -- This is a minor drawback since two more powerful and protected but not isolated power outputs are now available.
- **System I/O connector type and layout** -- The cabling of the devices attached to the I/O ports has to be redesigned.

**Area-Scan Acquisition Control:**

- **Multiple Windows Of Interest (WOIs) on Grablink Expert 2** -- This is a minor drawback since this feature is used by only a couple of cameras.

**Line-Scan Acquisition Control:**

- **Advanced Downweb Resampling (ADR) on Grablink Avenue and Grablink Express** -- The "Pick-A-Line" downweb resampling method remains available.

**Processing Functions:**

- **The white balance operator, and the automatic white balance** (previously available on Grablink Quickpack CFA PCIe) -- The white balance correction can be done using the parametric RGB look-up table operator; the appropriate look-up table parameters have to be determined by the application software either empirically or by analysis of a calibration image.
- **Shading corrector** (previously available on Grablink Quickpack ColorScan) -- This is not a significant drawback since most of the line-scan cameras now feature a built-in shading corrector.
- **High-Dynamics 16-bit interpolated LUT** (previously available on Grablink Avenue, Grablink Express, Grablink Quickpack CFA, and Grablink Quickpack CFA PCIe) -- This is a minor drawback since the non-interpolating LUT operator supports input bit depth up to 12-bit.

**Output Formats:**

- **RGB15, RGB16, RGB10_10_10, RGB10_12_10, RGB48, and RGB64** (previously available on Grablink Quickpack CFA PCIe) -- This is a minor drawback since these formats are not really popular.
- **4-component RGBY formats** (previously available on Grablink Quickpack CFA PCIe) -- This is a minor drawback since these formats are not really popular.

**PCI Interface:**

- **Conventional PCI interfaces** -- This is no significant drawback since the PCI Express technology has superseded PCI technology on PC motherboards.

### 5. MultiCam Parameters Differences

#### 5.1 Board Parameters

This section lists migration issues related to MultiCam parameters that belong to the Board class.

| Parameter | Modifications | Migration Issue |
|-----------|--------------|-----------------|
| `BoardTopology` | New values | Applies only to Grablink Full. When interfacing 10-tap cameras, it is mandatory to assign the value `MONO_DECA` to the parameter prior the assignation of the first MultiCam channel to the board. In other cases, leave the default value as is. |

Refer to the board-specific table of I/O indices to select the appropriate index for the I/O.

#### 5.2 Channel Parameters

This section lists the migration issues related to MultiCam parameters that belong to the Channel class.

| Parameter | Modifications | Migration Issue |
|-----------|--------------|-----------------|
| `Manufacturer` | Deleted | This parameter is deleted. Its usage is prohibited. |
| `Camera` | Single value allowed | This parameter has only one possible value. Using any other value is prohibited. For applications that used built-in camera interfaces, the CamFile must be upgraded. |
| `CamConfig` | Eight values allowed | This parameter has only 8 possible values corresponding to the 8 fundamental use cases. Using any other value is prohibited. For applications that used built-in camera interfaces, it is mandatory to upgrade the CamFile. |
| `TapStructure`, `CameraDataWidth`, `ChannelTopology` | Deleted | These parameters are deleted. Their usage is prohibited. They are superseded by `TapConfiguration` and `TapGeometry`. |
| `PixelClk_Hz`, `DataClk_Hz`, `HTotal_Px`, `VTotal_Ln` | Deleted | These parameters are deleted. Their usage is prohibited. |
| `TapConfiguration` | Removed values | Grablink Full, DualBase and Base do not support multiplexed taps: `BASE_1T*B2`, `BASE_2T*B2`, `BASE_1T*B3`, `BASE_2T*B3`. There is no workaround. |
| `ColorRegistration` | Irrelevant | *Applies only when migrating from Grablink Quickpack ColorScan.* Grablink Full, DualBase and Base do not implement scan delay compensation for 3-CCD RGB line-scan cameras. Setting this parameter to `RGB`, `GBR`, or `BRG` has absolutely no effect. |
| `ColorGap` | Deleted | *Applies only when migrating from Grablink Quickpack ColorScan.* This parameter is deleted. Its usage is prohibited. |
| `CrossPitch` | Irrelevant | *Applies only when migrating from Grablink Quickpack ColorScan.* Setting this parameter has absolutely no effect. This parameter is still visible in MultiCam Studio. Documented for Grablink Quickpack ColorScan exclusively. |
| `ParityDetection` | Deleted | Grablink Full, DualBase and Base do not support interlaced-scan cameras. This parameter is deleted. Its usage is prohibited. |
| `CameraWoiBankSelectCmd`, `CameraWoiBankSetupCmd`, `WoiWidth`, `WoiHeight`, `WoiOrgX`, `WoiOrgY` | Irrelevant | *Applies only when migrating from Grablink Expert 2.* Grablink Full, DualBase and Base do not support multiple Windows Of Interest. Setting these parameters has absolutely no effect. Not visible in MultiCam Studio. Documented exclusively for Grablink Expert 2. |
| `Cable`, `CableName` | Deleted | These parameters are deleted. Their usage is prohibited. |
| `TrigCtl`, `EndTrigCtl` | New values (exclusive) | There are 2 new electrical styles for input ports: `DIFF`, `ISO`. Since there are no common values with previous generations of Grablink boards, it is mandatory to update these parameters if they are in use. |
| `TrigLine`, `EndTrigLine` | New values | There are new specific input line names: `DIN1`, `DIN2`, `IIN1`, `IIN2`, `IIN3`, `IIN4`. The value `NOM` is common with previous generations and is the default. It is recommended that the line corresponding to the selected electrical style be used. |
| `StrobeCtl` | New value (single) | Grablink Full, DualBase and Base support only one possible parameter value: `OPTO`; this is also the default. For migration, it is recommended to remove any setting for `StrobeCtl` to adopt the default value of MultiCam: `AUTO`. |
| `StrobeLevel` | Irrelevant | Grablink Full, DualBase and Base do not provide a polarity control for the strobe output. Setting this parameter to `PLSHIGH` or `PLSLOW` has absolutely no effect. Not visible in MultiCam Studio; not documented for 3rd gen boards. |
| `StrobeLine` | Irrelevant | Grablink Full, DualBase and Base support only one possible value for the strobe output: `IOUT1`. Setting this parameter to `NOM` or `IOUT1` has no effect. Not visible in MultiCam Studio; not documented for 3rd gen boards. |
| `LineTrigCtl` | New values (exclusive) | There are 2 new electrical styles for input ports: `DIFF`, `ISO`. Since there are no common values with previous generations, these parameters must be updated if they are in use. |
| `WBO_Mode`, `WBO_Gain<R,G,B>`, `WBO_Width`, `WBO_Height`, `WBO_Org<X,Y>`, `WBO_Status` | Deleted | *Applies only when migrating from Grablink Quickpack CFA.* Grablink Full, DualBase and Base do not have a white balance operator. These parameters are deleted. Their usage is prohibited. |
| `ColorFormat` | Removed values | Grablink Full, DualBase and Base do not support byte-unaligned packed formats (`RGB15`, `RGB16`, `RGB10_10_10`, `RGB10_12_10`), nor 4-component formats of Grablink Quickpack CFA. There is no workaround. |

---

## Migration Paths

This document shows specific migration scenarios and lists all the specific differences that must be taken into account.

In the migration paths:

- **Gray arrows** are migration paths from the 1st to the 2nd generation of Grablink boards.
- **Green arrows** are migration paths from the 1st or the 2nd generations to the 3rd generation of Grablink boards.
- **Solid arrows** are migration paths without significant differences.
- **Dashed arrows** are migration paths with significant differences.

### 1. General-Purpose Frame Grabbers

#### 1.1 Migration from Grablink Value to Grablink Base

**Significant Benefits:**

- Smaller form factor
- Higher PCI throughput
- Bayer CFA decoder
- More powerful LUT operator
- Support for PoCL cameras
- Support for cameras beyond 60 MHz

**Significant Differences:**

- Requires different Camera Link cables, I/O cables, and PCI slot

#### 1.2 Migration from Grablink Expert 2 to Grablink DualBase

**Significant Benefits:**

- Considerably higher PCI throughput
- Bayer CFA decoder
- LUT operator
- Support for PoCL cameras
- Support for cameras beyond 60 MHz

**Significant Differences:**

- Requires different Camera Link cables, I/O cables, and PCI slot
- No support for multiple-Windows Of Interest cameras
- No support for Medium configuration (use Grablink Full instead)

#### 1.3 Migration from Grablink Expert 2 to Grablink Full

**Significant Benefits:**

- Considerably higher PCI throughput
- Bayer CFA decoder
- LUT operator
- Support for cameras beyond 60 MHz
- Support for all Camera Link configurations including Full
- Support for 10-tap 8-bit configuration

**Significant Differences:**

- Requires different Camera Link cables, I/O cables, and PCI slot
- No support for multiple-Windows Of Interest cameras
- No support for two BASE cameras (use Grablink DualBase instead)

#### 1.4 Migration from Grablink Avenue/Grablink Express to Grablink Base

**Significant Benefits:**

- Smaller form factor
- Higher PCI throughput (migration from Grablink Express only)
- Bayer CFA decoder
- LUT operator
- Support for PoCL cameras (migration from Grablink Avenue only)

**Significant Differences:**

- Requires different Camera Link cables, I/O cables, and PCI slot
- ADR is not available
- Slightly lower PCI throughput (migration from Grablink Avenue only)
- No support for multiplexed tap configurations

### 2. Grablink Quickpack Frame Grabbers

#### 2.1 Migration from Grablink Quickpack ColorScan to Grablink Base

**Significant Benefits:**

- Smaller form factor
- Support for PoCL cameras
- Support for cameras beyond 50 MHz

**Significant Differences:**

- Requires different Camera Link cables, I/O cables, and PCI slot
- No scan-delay compensation (useful only for trilinear cameras)
- No shading correction

#### 2.2 Migration from Grablink Quickpack CFA (PCIe) to Grablink Full

**Significant Benefits:**

- Considerably higher PCI throughput

**Significant Differences:**

- Requires different Camera Link cables, I/O cables, and PCI slot
- No support for PoCL cameras. If this is an issue, consider the migration towards a Grablink DualBase possibly with a single camera.
- No automatic white balance
- No luminance blender
- Reduced set of available output formats
- No support for multiplexed tap configurations
