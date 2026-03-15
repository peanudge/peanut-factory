# Processing

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [Overview](#1-overview)
2. [Image Reconstruction](#2-image-reconstruction)
3. [Image Cropping](#3-image-cropping)
4. [Image Flipping](#4-image-flipping)
5. [Pixel Data Processing Configurations](#5-pixel-data-processing-configurations)
6. [Look-up Table Transformation](#6-look-up-table-transformation)
7. [Bayer CFA to RGB Conversion](#7-bayer-cfa-to-rgb-conversion)
8. [Pixel Formatting](#8-pixel-formatting)
9. [Image Transfer](#9-image-transfer)
10. [Transfer Latency](#10-transfer-latency)

---

## 1. Overview

The acquisition channels of Grablink boards perform the following successive operations on the image data stream:

1. **Image Reconstruction** -- Unscrambles the pixel streams of multi-tap cameras and reconstructs the image exactly like it was captured on the camera sensor.

2. **Image Cropping** -- Extracts a rectangular area from the Camera Active Area.

3. **Image Flipping** -- Flips the image around a horizontal and/or a vertical axis.

4. **Lookup Table Transformation** -- Performs lookup table processing on individual pixel components. *(Applies to: Full, FullXR)*

5. **Bayer CFA Decoding** -- Transforms the raw Bayer CFA data stream into an RGB color data stream. *(Applies to: Full, FullXR)*

6. **White Balancing** -- Adjusts the gain and the offset of each color channel. *(Applies to: Full, FullXR)*

7. **Pixel Formatting** -- Unpacks 10-bit, 12-bit, and 14-bit pixel components to 8-bit or 16-bit; delivers RGB data in packed or planar formats.

8. **Image Line Build-up** -- Concatenates the component data of all pixels of an image line. 8-bit components are byte-aligned; 16-bit components are word-aligned (little-endian).

9. **Image Transfer** -- Processed and formatted image data are transferred into a MultiCam Surface over the PCI Express bus using a DMA engine.

---

## 2. Image Reconstruction

Grablink boards unscramble the pixel streams of multi-tap cameras and reconstruct the image exactly like it was captured on the camera sensor for most of the tap configurations and geometries.

Supported tap geometries are organized by Camera Link configuration (Lite, Base, Medium, Full, 72-bit, 80-bit) and vary by line-scan, area-scan (1 YTap), and area-scan (2 YTap) camera types.

> **NOTE:** Refer to the `TapGeometry` parameter in the Parameters Reference for a description of each geometry.

---

## 3. Image Cropping

Grablink boards implement an image cropping operator that selects a subset of the pixels delivered by the camera to build the image delivered to the Host PC.

This subset, named **WindowArea**, includes:
- For area-scan cameras: a single rectangular region of the 2D image sensor.
- For line-scan cameras: a single segment of the 1D image sensor.

### Image Cropping Parameters

| Parameter | Description |
|-----------|-------------|
| `GrabWindow` | Main control parameter |
| `WindowX_Px`, `WindowY_Ln` | Size of the WindowArea |
| `OffsetX_Px`, `OffsetY_Ln` | Position of the WindowArea within the Camera Active Area |

> **NOTE:** The range of allowed values of `OffsetX_Px` and `OffsetY_Ln` is automatically adjusted to force the Window Area to stay within the boundaries of the Camera Active Area.

### Configuring Image Cropping

By default, `GrabWindow` is set to `NOBLACK` (cropping disabled). To enable:

1. Set `GrabWindow` to `MAN`
2. Adjust `WindowX_Px` (range: 8 to `Hactive_Px`)
3. For area-scan only: adjust `WindowY_Ln` (range: 1 to `Vactive_Ln`)
4. Adjust `OffsetX_Px` to move horizontally
5. For area-scan only: adjust `OffsetY_Ln` to move vertically

**Conditions of applicability:** Cropping is applicable to monochrome, RGB color, and Bayer CFA color area-scan cameras (any valid TapConfiguration/TapGeometry except when `TapGeometry` = `*_2YE`), and to monochrome and RGB color line-scan cameras.

---

## 4. Image Flipping

Grablink boards implement image flipping (mirroring):

- For area-scan cameras: left/right and top/bottom mirroring
- For line-scan cameras: left/right mirroring only

| Parameter | Description |
|-----------|-------------|
| `ImageFlipX` | Enables left/right mirroring |
| `ImageFlipY` | Enables top/bottom mirroring |

By default, both are `OFF`.

---

## 5. Pixel Data Processing Configurations

### Configurations for Monochrome Pixels

When LUT transformation is disabled, the pixel output format is defined by `ColorFormat`:

| Camera PFNC Pixel Format | ColorFormat | Output PFNC Pixel Format |
|--------------------------|-------------|------------------------|
| Mono8 | `Y8` | Mono8 |
| Mono10 | `Y8` / `Y10` / `Y16` | Mono8 / Mono10 / Mono16 |
| Mono12 | `Y8` / `Y12` / `Y16` | Mono8 / Mono12 / Mono16 |
| Mono14 | `Y8` / `Y14` / `Y16` | Mono8 / Mono14 / Mono16 |
| Mono16 | `Y8` / `Y16` | Mono8 / Mono16 |

### Configurations for Bayer CFA Pixels

When Bayer CFA decoding is disabled, pixel output format is defined by `ColorFormat`:

| Camera PFNC Format | ColorFormat | Output PFNC Format |
|--------------------|-------------|-------------------|
| Bayer**8 | `BAYER8` | Bayer**8 |
| Bayer**10 | `BAYER8` / `BAYER10` / `BAYER16` | Bayer**8 / Bayer**10 / Bayer**16 |
| Bayer**12 | `BAYER8` / `BAYER12` / `BAYER16` | Bayer**8 / Bayer**12 / Bayer**16 |

> **NOTE:** The white balance and the LUT transformation are not available when Bayer CFA decoding is disabled.

When Bayer CFA decoding is enabled, the processing chain uses the Bayer CFA Decoder, White Balance operator (optional), and Look-Up-Table operator (optional). The chain outputs one RGB pixel for each RAW pixel.

### Configurations for RGB Pixels

The processing chain outputs one RGB pixel for each RGB pixel of the input buffer. White balance, LUT transformation, and pixel formatting are available. The `RedBlueSwap` parameter controls whether output uses BGR or RGB byte order.

---

## 6. Look-up Table Transformation

*(Applies to: Base, DualBase, Full, FullXR)*

The look-up table operator enables processing of monochrome or RGB color pixel data streams.

Storage for **four LUT definitions** is available in main memory (indexed 1 to 4). Selecting LUT index 0 disables the operator.

During channel activation, the hardware initializes the LUT operator with the selected LUT definition. Modifications during acquisition (changing `LUT_UseIndex` or modifying the definition) are applied without delay.

### Monochrome Operation

The LUT operator is modeled as a single high-speed RAM in the pixel data stream.

| Camera Bit Depth | LUT Input Depth | LUT Output Depth | Peak Rate (Base/DualBase) | Peak Rate (Full/FullXR) |
|-------------------|-----------------|-------------------|--------------------------|------------------------|
| 8 | 8 | 8 | 500 MPx/s | 1000 MPx/s |
| 10 | 10 | 8, 10, 16 | 250 MPx/s | 500 MPx/s |
| 12 | 12 | 8, 12, 16 | 250 MPx/s | 500 MPx/s |
| 14 | 12 | 8, 14, 16 | 250 MPx/s | 500 MPx/s |
| 16 | 12 | 8, 16 | 250 MPx/s | 500 MPx/s |

> **NOTE:** For 14-bit and 16-bit cameras, only 12 most significant bits of the camera pixel data are effectively considered; the remaining bits are ignored.

### RGB Color Operation

When configured for RGB color cameras, the LUT operator is modeled as a triplet of high-speed RAMs inserted into the R, G, B component data streams.

### Bayer Color Operation

When configured for Bayer color cameras (with Bayer decoder enabled), the LUT operates as a triplet of RAMs in the R, G, B data streams delivered by the CFA decoder.

---

## 7. Bayer CFA to RGB Conversion

*(Applies to: Base, DualBase, Full, FullXR)*

The Bayer CFA decoder transforms the raw Bayer CFA data stream into an RGB color data stream.

### Interpolation Methods

- **Legacy interpolation** computes missing color components using exclusively Mean() functions of the nearest components.
- **Advanced interpolation** computes missing color components using Mean() and Median() functions, eliminating the "creneling" effect on highly contrasted sharp transitions.

### Definitions

```
Mean2(a,b) = (a+b)/2
Mean4(a,b,c,d) = (a+b+c+d)/4
Median2Of4(a,b,c,d) = Mean2{ Min[Max(a,b); Max(c,d)] ; Max[Min(a,b); Min(c,d)] }
```

### Enabling the Bayer CFA Decoder

The function is automatically enabled if all conditions are satisfied:
1. The camera is area-scan (`Imaging` = AREA)
2. The camera is color (`Spectrum` = COLOR)
3. The camera delivers raw Bayer data (`ColorMethod` = BAYER)

### Configuring the Bayer CFA Decoder

The `CFD_Mode` parameter selects the interpolation method. Default and recommended: `ADVANCED`. Alternate: `LEGACY`.

**CFA Decoder Performance:** Peak pixel processing rate is **250 megapixels/s** per acquisition channel.

---

## 8. Pixel Formatting

### Pixel Component Unpacking

Grablink boards unpack 10-bit, 12-bit, and 14-bit pixel component data to 16-bit pixel data with two options:

**Unpacking to lsb (Default):** Significant bits aligned to least significant bit. Padding zeros in MSB.
- 10-bit: `0000 00<pppppppppp>`
- 12-bit: `0000 <pppppppppppp>`
- 14-bit: `00<pppppppppppppp>`

**Unpacking to msb:** Significant bits aligned to most significant bit. Padding zeros in LSB.
- 10-bit: `<pppppppppp>000000` (multiplies value by 64)
- 12-bit: `<pppppppppppp>0000` (multiplies value by 16)
- 14-bit: `<pppppppppppppp>00` (multiplies value by 4)

### Pixel Bit Depth Reduction

Grablink boards can reduce 10-/12-/14-/16-bit pixel components to 8-bit by truncation of the least significant bits.

### Pixel Format Control

Controlled by the `ColorFormat` parameter. For monochrome 10-bit pixels:
- Set `ColorFormat` to `Y8` for bit depth reduction
- Set `ColorFormat` to `Y16` for unpacking to msb
- Keep default `ColorFormat` = `Y10` for unpacking to lsb

---

## 9. Image Transfer

Processed and formatted image data are transferred into a **surface** over the PCI Express bus using a DMA engine.

> **NOTE:** A surface is the physical memory space allocated into the host PC memory for the storage of one image.

The transferred image is stored in progressive-scan order:
- First pixel (top-left) stored at address offset 0
- Subsequent lines stored at byte addresses that are multiples of the surface pitch

### Surface Pitch

The MultiCam driver establishes a default surface pitch corresponding to the bytes required for one pixel row. You may increase this value if needed.

### Transfer Rate

**Base, DualBase:**
- Up to 200 MB/s (256-byte PCI Express payload)
- Up to 180 MB/s (128-byte PCI Express payload)

**Full, FullXR:**
- Up to 833 MB/s (256-byte payload, 64-bit addressing)
- Up to 844 MB/s (256-byte payload, 32-bit addressing)
- Up to 754 MB/s (128-byte payload, 64-bit addressing)
- Up to 780 MB/s (128-byte payload, 32-bit addressing)

> **WARNING:** The effective data rate depends on the performance of the PCI Express link.

---

## 10. Transfer Latency

The **transfer latency** is the time interval between when the last camera pixel enters the frame grabber and when the same processed pixel is stored in host PC memory.

Key factors for latency estimation:

- **P1** is the amount of pixel data needed before initiating processing:
  - Two lines if the Bayer CFA decoder is used
  - One line if the Bayer CFA decoder is not used

- **Maximum processing/delivery pixel rate** is the smaller of:
  - The pixel processing rate (may be limited by Bayer CFA decoder or LUT processor)
  - The pixel delivery rate over the PCIe interface
