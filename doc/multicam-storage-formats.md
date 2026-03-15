# MultiCam Storage Formats

> Source: Euresys MultiCam 6.19.4 (Doc D406EN)

## Contents

1. [Introduction](#1-introduction)
2. [Monochrome Pixel Formats](#2-monochrome-pixel-formats)
3. [Bayer CFA Pixel Formats](#3-bayer-cfa-pixel-formats)
4. [RGB Color Pixel Formats](#4-rgb-color-pixel-formats)
5. [RGB Color Planar Pixel Formats](#5-rgb-color-planar-pixel-formats)
6. [YUV Color Pixel Formats](#6-yuv-color-pixel-formats)
7. [YUV Color Planar Pixel Formats](#7-yuv-color-planar-pixel-formats)
8. [Raw Data Formats](#8-raw-data-formats)

---

## 1. Introduction

MultiCam frame grabbers store image pixel data into the user buffer according to a format designated by the Channel parameter **`ColorFormat`**.

This document provides for each format:

- The **MultiCam name**, i.e. the `ColorFormat` value
- The **PFNC name** as specified in the Pixel Format Naming Convention of GenICam
- The **storage type**:
  - **PACKED** when all the components of a pixel are stored consecutively
  - **PLANAR** when each component of a pixel is stored separately in different planes
- The **memory layout** describing how the first pixels of the first line(s) are stored in the user buffer

---

## 2. Monochrome Pixel Formats

| MultiCam Name | PFNC Name | Bits/Pixel | Bytes/Pixel | Storage Type | Description |
|---------------|-----------|------------|-------------|--------------|-------------|
| `Y8` | Mono8 | 8 | 1 | N/A | [8-bit Monochrome](#21-8-bit-monochrome) |
| `Y10` | Mono10 | 10 | 2 | N/A | [10-bit Monochrome](#22-10-bit-monochrome) |
| `Y10P` | Mono10p | 10 | 1.25 | N/A | [10-bit Monochrome lsb Packed](#23-10-bit-monochrome-lsb-packed) |
| N/A | Mono10pmsb | 10 | 1.25 | N/A | [10-bit Monochrome msb Packed](#24-10-bit-monochrome-msb-packed) |
| `Y12` | Mono12 | 12 | 2 | N/A | [12-bit Monochrome](#25-12-bit-monochrome) |
| `Y14` | Mono14 | 14 | 2 | N/A | [14-bit Monochrome](#26-14-bit-monochrome) |
| `Y16` | Mono16 | 16 | 2 | N/A | [16-bit Monochrome](#27-16-bit-monochrome) |

---

### 2.1. 8-bit Monochrome

| Property | Value |
|----------|-------|
| MultiCam Name | `Y8` |
| PFNC Name | Mono8 |
| Storage Type | N/A |
| Storage Requirement | 1 Byte/pixel |

**Memory Layout**

Each pixel occupies 1 byte. Pixels are stored consecutively in memory:

```
Byte offset:    +3          +2          +1          +0
              Pixel[3,0]  Pixel[2,0]  Pixel[1,0]  Pixel[0,0]
```

---

### 2.2. 10-bit Monochrome

| Property | Value |
|----------|-------|
| MultiCam Name | `Y10` |
| PFNC Name | Mono10 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is stored in 2 bytes (16 bits), with the upper 6 bits set to zero:

```
Byte offset:    +3          +2          +1          +0
              000000 Pixel[1,0]  000000 Pixel[0,0]
```

---

### 2.3. 10-bit Monochrome lsb Packed

| Property | Value |
|----------|-------|
| MultiCam Name | `Y10P` |
| PFNC Name | Mono10p |
| Storage Type | N/A |
| Storage Requirement | 1.25 Bytes/pixel |

**Memory Layout**

10-bit pixel values are packed with the least significant bits first. Groups of 4 pixels are packed into 5 bytes.

> **Note:** A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (20 bytes).

---

### 2.4. 10-bit Monochrome msb Packed

| Property | Value |
|----------|-------|
| MultiCam Name | N/A |
| PFNC Name | Mono10pmsb |
| Storage Type | N/A |
| Storage Requirement | 1.25 Bytes/pixel |

**Memory Layout**

10-bit pixel values are packed with the most significant bits first. Groups of 4 pixels are packed into 5 bytes.

> **Note:** A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (20 bytes).

---

### 2.5. 12-bit Monochrome

| Property | Value |
|----------|-------|
| MultiCam Name | `Y12` |
| PFNC Name | Mono12 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is stored in 2 bytes (16 bits), with the upper 4 bits set to zero:

```
Byte offset:    +3          +2          +1          +0
              0000 Pixel[1,0]   0000 Pixel[0,0]
```

---

### 2.6. 14-bit Monochrome

| Property | Value |
|----------|-------|
| MultiCam Name | `Y14` |
| PFNC Name | Mono14 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is stored in 2 bytes (16 bits), with the upper 2 bits set to zero:

```
Byte offset:    +3          +2          +1          +0
              00 Pixel[1,0]     00 Pixel[0,0]
```

---

### 2.7. 16-bit Monochrome

| Property | Value |
|----------|-------|
| MultiCam Name | `Y16` |
| PFNC Name | Mono16 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel occupies a full 16-bit word:

```
Byte offset:    +3          +2          +1          +0
              Pixel[1,0]             Pixel[0,0]
```

---

## 3. Bayer CFA Pixel Formats

| MultiCam Name | PFNC Names | Bits/Pixel | Bytes/Pixel | Description |
|---------------|------------|------------|-------------|-------------|
| `BAYER8` | BayerBG8, BayerGB8, BayerGR8, BayerRG8 | 8 | 1 | [8-bit Bayer CFA](#31-8-bit-bayer-cfa) |
| `BAYER10` | BayerBG10, BayerGB10, BayerGR10, BayerRG10 | 10 | 2 | [10-bit Bayer CFA](#32-10-bit-bayer-cfa) |
| `BAYER12` | BayerBG12, BayerGB12, BayerGR12, BayerRG12 | 12 | 2 | [12-bit Bayer CFA](#33-12-bit-bayer-cfa) |
| `BAYER14` | BayerBG14, BayerGB14, BayerGR14, BayerRG14 | 14 | 2 | [14-bit Bayer CFA](#34-14-bit-bayer-cfa) |
| `BAYER16` | BayerBG16, BayerGB16, BayerGR16, BayerRG16 | 16 | 2 | [16-bit Bayer CFA](#35-16-bit-bayer-cfa) |

---

### 3.1. 8-bit Bayer CFA

| Property | Value |
|----------|-------|
| MultiCam Name | `BAYER8` |
| PFNC Names | BayerBG8, BayerGB8, BayerGR8, BayerRG8 |
| Storage Requirement | 1 Byte/pixel |

**Memory Layout (BG CFA example)**

Each pixel occupies 1 byte. The color filter array pattern determines the color component at each position. H = buffer pitch (in bytes).

```
Row 0:  +3            +2            +1            +0
        Pixel[3,0]:G  Pixel[2,0]:B  Pixel[1,0]:G  Pixel[0,0]:B

Row 1:  +3            +2            +1            +0
        Pixel[3,1]:R  Pixel[2,1]:G  Pixel[1,1]:R  Pixel[0,1]:G
```

> **Note:** H = buffer pitch (in bytes).

---

### 3.2. 10-bit Bayer CFA

| Property | Value |
|----------|-------|
| MultiCam Name | `BAYER10` |
| PFNC Names | BayerBG10, BayerGB10, BayerGR10, BayerRG10 |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout (BG CFA example)**

Each pixel is stored in 2 bytes with the upper 6 bits set to zero. H = buffer pitch (in bytes).

```
Row 0:  +3               +2               +1               +0
        000000 Pixel[1,0]:G               000000 Pixel[0,0]:B

Row 1:  +3               +2               +1               +0
        000000 Pixel[1,1]:R               000000 Pixel[0,1]:G
```

> **Note:** H = buffer pitch (in bytes).

---

### 3.3. 12-bit Bayer CFA

| Property | Value |
|----------|-------|
| MultiCam Name | `BAYER12` |
| PFNC Names | BayerBG12, BayerGB12, BayerGR12, BayerRG12 |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout (BG CFA example)**

Each pixel is stored in 2 bytes with the upper 4 bits set to zero. H = buffer pitch (in bytes).

```
Row 0:  +3             +2             +1             +0
        0000 Pixel[1,0]:G             0000 Pixel[0,0]:B

Row 1:  +3             +2             +1             +0
        0000 Pixel[1,1]:R             0000 Pixel[0,1]:G
```

> **Note:** H = buffer pitch (in bytes).

---

### 3.4. 14-bit Bayer CFA

| Property | Value |
|----------|-------|
| MultiCam Name | `BAYER14` |
| PFNC Names | BayerBG14, BayerGB14, BayerGR14, BayerRG14 |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout (BG CFA example)**

Each pixel is stored in 2 bytes with the upper 2 bits set to zero. H = buffer pitch (in bytes).

```
Row 0:  +3           +2           +1           +0
        00 Pixel[1,0]:G           00 Pixel[0,0]:B

Row 1:  +3           +2           +1           +0
        00 Pixel[1,1]:R           00 Pixel[0,1]:G
```

> **Note:** H = buffer pitch (in bytes).

---

### 3.5. 16-bit Bayer CFA

| Property | Value |
|----------|-------|
| MultiCam Name | `BAYER16` |
| PFNC Names | BayerBG16, BayerGB16, BayerGR16, BayerRG16 |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout (BG CFA example)**

Each pixel occupies a full 16-bit word. H = buffer pitch (in bytes).

```
Row 0:  +3             +2      +1             +0
        Pixel[1,0]:G           Pixel[0,0]:B

Row 1:  +3             +2      +1             +0
        Pixel[1,1]:R           Pixel[0,1]:G
```

> **Note:** H = buffer pitch (in bytes).

---

## 4. RGB Color Pixel Formats

| MultiCam Name | PFNC Name | Bits/Pixel | Bytes/Pixel | Storage Type | Description |
|---------------|-----------|------------|-------------|--------------|-------------|
| `RGB15` | BGR555 | 15 (+1 unused) | 2 | PACKED | [5-5-5-bit BGR](#41-5-5-5-bit-bgr) |
| `RGB16` | BGR565 | 16 | 2 | PACKED | [5-6-5-bit BGR](#42-5-6-5-bit-bgr) |
| `RGB24` | BGR8 | 24 | 3 | PACKED | [8-bit BGR](#43-8-bit-bgr) |
| N/A | RGB8 | 24 | 3 | PACKED | [8-bit RGB](#44-8-bit-rgb) |
| `RGB32` | BGRa8 | 32 | 4 | PACKED | [8-bit BGRa](#45-8-bit-bgra) |
| N/A | RGBa8 | 32 | 4 | PACKED | [8-bit RGBa](#46-8-bit-rgba) |
| `RGB30P` | BGR10p | 30 | 3.75 | PACKED | [10-bit BGR lsb Packed](#47-10-bit-bgr-lsb-packed) |
| N/A | BGR10pmsb | 30 | 3.75 | PACKED | [10-bit BGR msb Packed](#48-10-bit-bgr-msb-packed) |
| `RGBI40P` | BGRa10p | 40 | 5 | PACKED | [10-bit BGRa lsb Packed](#49-10-bit-bgra-lsb-packed) |

---

### 4.1. 5-5-5-bit BGR

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB15` |
| PFNC Name | BGR555 |
| Storage Type | PACKED |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is packed into 2 bytes: 1 unused bit, then 5 bits each for R, G, B.

```
Byte offset:    +3          +2          +1          +0
              [0|R|G|B]  Pixel[1,0]  [0|R|G|B]  Pixel[0,0]
              (5-5-5)                (5-5-5)
```

---

### 4.2. 5-6-5-bit BGR

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB16` |
| PFNC Name | BGR565 |
| Storage Type | PACKED |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is packed into 2 bytes: 5 bits R, 6 bits G, 5 bits B.

```
Byte offset:    +3          +2          +1          +0
              [R|G|B]    Pixel[1,0]  [R|G|B]    Pixel[0,0]
              (5-6-5)                (5-6-5)
```

---

### 4.3. 8-bit BGR

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB24` |
| PFNC Name | BGR8 |
| Storage Type | PACKED |
| Storage Requirement | 3 Bytes/pixel |

**Memory Layout**

Pixels are stored in B-G-R byte order. Since 3 bytes per pixel does not align to 4-byte boundaries, successive pixels wrap across 32-bit words:

```
Word @+0:   Pixel[1,0]:B  Pixel[0,0]:R  Pixel[0,0]:G  Pixel[0,0]:B
Word @+4:   Pixel[2,0]:G  Pixel[2,0]:B  Pixel[1,0]:R  Pixel[1,0]:G
Word @+8:   Pixel[3,0]:R  Pixel[3,0]:G  Pixel[3,0]:B  Pixel[2,0]:R
```

---

### 4.4. 8-bit RGB

| Property | Value |
|----------|-------|
| MultiCam Name | N/A |
| PFNC Name | RGB8 |
| Storage Type | PACKED |
| Storage Requirement | 3 Bytes/pixel |

**Memory Layout**

Pixels are stored in R-G-B byte order. Since 3 bytes per pixel does not align to 4-byte boundaries, successive pixels wrap across 32-bit words:

```
Word @+0:   Pixel[1,0]:R  Pixel[0,0]:B  Pixel[0,0]:G  Pixel[0,0]:R
Word @+4:   Pixel[2,0]:G  Pixel[2,0]:R  Pixel[1,0]:B  Pixel[1,0]:G
Word @+8:   Pixel[3,0]:B  Pixel[3,0]:G  Pixel[3,0]:R  Pixel[2,0]:B
```

---

### 4.5. 8-bit BGRa

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB32` |
| PFNC Name | BGRa8 |
| Storage Type | PACKED |
| Storage Requirement | 4 Bytes/pixel |

**Memory Layout**

Each pixel occupies exactly one 32-bit word in B-G-R-a byte order:

```
Byte offset:    +3          +2          +1          +0
              Pixel[0,0]:a  Pixel[0,0]:R  Pixel[0,0]:G  Pixel[0,0]:B
```

---

### 4.6. 8-bit RGBa

| Property | Value |
|----------|-------|
| MultiCam Name | N/A |
| PFNC Name | RGBa8 |
| Storage Type | PACKED |
| Storage Requirement | 4 Bytes/pixel |

**Memory Layout**

Each pixel occupies exactly one 32-bit word in R-G-B-a byte order:

```
Byte offset:    +3          +2          +1          +0
              Pixel[0,0]:a  Pixel[0,0]:B  Pixel[0,0]:G  Pixel[0,0]:R
```

---

### 4.7. 10-bit BGR lsb Packed

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB30P` |
| PFNC Name | BGR10p |
| Storage Type | PACKED |
| Storage Requirement | 3.75 Bytes/pixel |

**Memory Layout**

Three 10-bit color components (B, G, R) are packed per pixel with least significant bits first. The first 32-bit word contains Pixel[0,0] with B, G, R packed from bit 0 upward, with 2 unused bits.

> **Note:** A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (60 bytes).

---

### 4.8. 10-bit BGR msb Packed

| Property | Value |
|----------|-------|
| MultiCam Name | N/A |
| PFNC Name | BGR10pmsb |
| Storage Type | PACKED |
| Storage Requirement | 3.75 Bytes/pixel |

**Memory Layout**

Three 10-bit color components (B, G, R) are packed per pixel with most significant bits first. The first 32-bit word contains Pixel[0,0] with B, G, R packed from the most significant bit downward, with 2 unused bits.

> **Note:** A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (60 bytes).

---

### 4.9. 10-bit BGRa lsb Packed

| Property | Value |
|----------|-------|
| MultiCam Name | `RGBI40P` |
| PFNC Name | BGRa10p |
| Storage Type | PACKED |
| Storage Requirement | 5 Bytes/pixel |

**Memory Layout**

Four 10-bit components (B, G, R, a) are packed per pixel with least significant bits first, totaling 40 bits (5 bytes) per pixel.

> **Note:** A pixel boundary is aligned to a 32-bit word boundary every 4 pixels (20 bytes).

---

## 5. RGB Color Planar Pixel Formats

| MultiCam Name | PFNC Name | Bits/Pixel | Bytes/Pixel | Storage Type | Description |
|---------------|-----------|------------|-------------|--------------|-------------|
| `RGB24PL` | N/A | 24 | 3 | PLANAR | [8-bit RGB Planar](#51-8-bit-rgb-planar) |
| `RGB30PL` | N/A | 30 | 6 | PLANAR | [10-bit RGB Planar](#52-10-bit-rgb-planar) |
| `RGB36PL` | N/A | 36 | 6 | PLANAR | [12-bit RGB Planar](#53-12-bit-rgb-planar) |
| `RGB42PL` | N/A | 42 | 6 | PLANAR | [14-bit RGB Planar](#54-14-bit-rgb-planar) |
| `RGB48PL` | N/A | 48 | 6 | PLANAR | [16-bit RGB Planar](#55-16-bit-rgb-planar) |

---

### 5.1. 8-bit RGB Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB24PL` |
| PFNC Name | N/A |
| Storage Type | PLANAR |
| Storage Requirement | 3 Bytes/pixel |

**Memory Layout**

Three separate planes, each storing 1 byte per pixel:

- **Plane 0 (R):** `Pixel[3,0]:R  Pixel[2,0]:R  Pixel[1,0]:R  Pixel[0,0]:R`
- **Plane 1 (G):** `Pixel[3,0]:G  Pixel[2,0]:G  Pixel[1,0]:G  Pixel[0,0]:G`
- **Plane 2 (B):** `Pixel[3,0]:B  Pixel[2,0]:B  Pixel[1,0]:B  Pixel[0,0]:B`

---

### 5.2. 10-bit RGB Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB30PL` |
| PFNC Name | N/A |
| Storage Type | PLANAR |
| Storage Requirement | 6 Bytes/pixel |

**Memory Layout**

Three separate planes, each storing 2 bytes per pixel (upper 6 bits zero):

- **Plane 0 (R):** `000000 Pixel[1,0]:R  |  000000 Pixel[0,0]:R`
- **Plane 1 (G):** `000000 Pixel[1,0]:G  |  000000 Pixel[0,0]:G`
- **Plane 2 (B):** `000000 Pixel[1,0]:B  |  000000 Pixel[0,0]:B`

---

### 5.3. 12-bit RGB Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB36PL` |
| PFNC Name | N/A |
| Storage Type | PLANAR |
| Storage Requirement | 6 Bytes/pixel |

**Memory Layout**

Three separate planes, each storing 2 bytes per pixel (upper 4 bits zero):

- **Plane 0 (R):** `0000 Pixel[1,0]:R  |  0000 Pixel[0,0]:R`
- **Plane 1 (G):** `0000 Pixel[1,0]:G  |  0000 Pixel[0,0]:G`
- **Plane 2 (B):** `0000 Pixel[1,0]:B  |  0000 Pixel[0,0]:B`

---

### 5.4. 14-bit RGB Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB42PL` |
| PFNC Name | N/A |
| Storage Type | PLANAR |
| Storage Requirement | 6 Bytes/pixel |

**Memory Layout**

Three separate planes, each storing 2 bytes per pixel (upper 2 bits zero):

- **Plane 0 (R):** `00 Pixel[1,0]:R  |  00 Pixel[0,0]:R`
- **Plane 1 (G):** `00 Pixel[1,0]:G  |  00 Pixel[0,0]:G`
- **Plane 2 (B):** `00 Pixel[1,0]:B  |  00 Pixel[0,0]:B`

---

### 5.5. 16-bit RGB Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `RGB48PL` |
| PFNC Name | N/A |
| Storage Type | PLANAR |
| Storage Requirement | 6 Bytes/pixel |

**Memory Layout**

Three separate planes, each storing 2 bytes per pixel (full 16-bit values):

- **Plane 0 (R):** `Pixel[1,0]:R  |  Pixel[0,0]:R`
- **Plane 1 (G):** `Pixel[1,0]:G  |  Pixel[0,0]:G`
- **Plane 2 (B):** `Pixel[1,0]:B  |  Pixel[0,0]:B`

---

## 6. YUV Color Pixel Formats

| MultiCam Name | Alias | PFNC Name | Bytes/Pixel | Storage Type | Description |
|---------------|-------|-----------|-------------|--------------|-------------|
| `YUV411` | Y41P | YUV411_8_UYVYUYVYYYYY | 1.5 | PACKED | [8-bit YUV 4:1:1](#61-8-bit-yuv-411) |
| `YUV422` | Y42P | YUV422_8_YUYV | 2 | PACKED | [8-bit YUV 4:2:2](#62-8-bit-yuv-422) |
| `YUV444` | IYU2 | YUV444_8_UYV | 3 | PACKED | [8-bit YUV 4:4:4](#63-8-bit-yuv-444) |

---

### 6.1. 8-bit YUV 4:1:1

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV411` / Y41P |
| PFNC Name | YUV411_8_UYVYUYVYYYYY |
| Storage Type | PACKED |
| Storage Requirement | 1.5 Bytes/pixel |

**Memory Layout**

8 pixels are packed into 12 bytes. Only 1 U(Cb) and 1 V(Cr) sample are captured every 4 pixels.

```
Word @+0:   Pixel[1,0]:Y   Pixel[0,0]:V(Cr)  Pixel[0,0]:Y   Pixel[0,0]:U(Cb)
Word @+4:   Pixel[3,0]:Y   Pixel[4,0]:V(Cr)  Pixel[2,0]:Y   Pixel[4,0]:U(Cb)
Word @+8:   Pixel[7,0]:Y   Pixel[6,0]:Y      Pixel[5,0]:Y   Pixel[4,0]:Y
```

> **Note:** Only 1 U(Cb) and 1 V(Cr) samples are captured every 4 pixels.

---

### 6.2. 8-bit YUV 4:2:2

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV422` / Y42P |
| PFNC Name | YUV422_8_YUYV |
| Storage Type | PACKED |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

2 pixels are packed into 4 bytes:

```
Byte offset:    +3              +2            +1              +0
              Pixel[0,0]:V(Cr)  Pixel[1,0]:Y  Pixel[0,0]:U(Cb)  Pixel[0,0]:Y
```

> **Note:** Only 1 U(Cb) and 1 V(Cr) samples are captured every 2 pixels.

---

### 6.3. 8-bit YUV 4:4:4

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV444` / IYU2 |
| PFNC Name | YUV444_8_UYV |
| Storage Type | PACKED |
| Storage Requirement | 3 Bytes/pixel |

**Memory Layout**

Each pixel has its own U, Y, V components stored in 3 bytes. Since 3 bytes per pixel does not align to 4-byte boundaries, successive pixels wrap across 32-bit words:

```
Word @+0:   Pixel[1,0]:U(Cb)  Pixel[0,0]:V(Cr)  Pixel[0,0]:Y      Pixel[0,0]:U(Cb)
Word @+4:   Pixel[2,0]:Y      Pixel[2,0]:U(Cb)   Pixel[1,0]:V(Cr)  Pixel[1,0]:Y
Word @+8:   Pixel[3,0]:V(Cr)  Pixel[3,0]:Y       Pixel[3,0]:U(Cb)  Pixel[2,0]:V(Cr)
```

---

## 7. YUV Color Planar Pixel Formats

| MultiCam Name | Alias | PFNC Name | Bytes/Pixel | Storage Type | Description |
|---------------|-------|-----------|-------------|--------------|-------------|
| `YUV411PL_Dec` | YUV9, YVU9 | YUV410_8_Planar | 1.125 | PLANAR | [8-bit YUV 4:1:0 Planar](#71-8-bit-yuv-410-planar) |
| `YUV411PL` | Y41B | YUV411_8_Planar | 1.5 | PLANAR | [8-bit YUV 4:1:1 Planar](#72-8-bit-yuv-411-planar) |
| `YUV422PL_Dec` | I420, IYUV, YV12 | YUV420_8_Planar | 1.5 | PLANAR | [8-bit YUV 4:2:0 Planar](#73-8-bit-yuv-420-planar) |
| `YUV422PL` | Y42B | YUV422_8_Planar | 2 | PLANAR | [8-bit YUV 4:2:2 Planar](#74-8-bit-yuv-422-planar) |
| `YUV444PL` | -- | YUV444_8_Planar | 3 | PLANAR | [8-bit YUV 4:4:4 Planar](#75-8-bit-yuv-444-planar) |

---

### 7.1. 8-bit YUV 4:1:0 Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV411PL_Dec` / YUV9 / YVU9 |
| PFNC Name | YUV410_8_Planar |
| Storage Type | PLANAR |
| Storage Requirement | 1.125 Bytes/pixel |

**Memory Layout**

- **Plane 0 (Y):** 1 byte per pixel, all pixels stored.
  ```
  Pixel[3,0]:Y  Pixel[2,0]:Y  Pixel[1,0]:Y  Pixel[0,0]:Y
  ```

- **Plane 1 (U/Cb):** 1 U(Cb) sample every 4 pixels in 1 line every 4 lines. H = buffer pitch.
  ```
  Row 0:  Pixel[12,0]:U(Cb)  Pixel[8,0]:U(Cb)  Pixel[4,0]:U(Cb)  Pixel[0,0]:U(Cb)
  Row 4:  Pixel[12,4]:U(Cb)  Pixel[8,4]:U(Cb)  Pixel[4,4]:U(Cb)  Pixel[0,4]:U(Cb)
  ```

- **Plane 2 (V/Cr):** 1 V(Cr) sample every 4 pixels in 1 line every 4 lines.
  ```
  Row 0:  Pixel[12,0]:V(Cr)  Pixel[8,0]:V(Cr)  Pixel[4,0]:V(Cr)  Pixel[0,0]:V(Cr)
  Row 4:  Pixel[12,4]:V(Cr)  Pixel[8,4]:V(Cr)  Pixel[4,4]:V(Cr)  Pixel[0,4]:V(Cr)
  ```

> **Note:** Only 1 U(Cb) and 1 V(Cr) sample is captured every 4 pixels in 1 line every 4 lines.

---

### 7.2. 8-bit YUV 4:1:1 Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV411PL` / Y41B |
| PFNC Name | YUV411_8_Planar |
| Storage Type | PLANAR |
| Storage Requirement | 1.5 Bytes/pixel |

**Memory Layout**

- **Plane 0 (Y):** 1 byte per pixel, all pixels stored.

- **Plane 1 (U/Cb):** 1 U(Cb) sample every 4 pixels.
  ```
  Pixel[12,0]:U(Cb)  Pixel[8,0]:U(Cb)  Pixel[4,0]:U(Cb)  Pixel[0,0]:U(Cb)
  ```

- **Plane 2 (V/Cr):** 1 V(Cr) sample every 4 pixels.
  ```
  Pixel[12,0]:V(Cr)  Pixel[8,0]:V(Cr)  Pixel[4,0]:V(Cr)  Pixel[0,0]:V(Cr)
  ```

> **Note:** Only 1 U(Cb) and 1 V(Cr) sample is captured every 4 pixels.

---

### 7.3. 8-bit YUV 4:2:0 Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV422PL_Dec` / I420 / IYUV / YV12 |
| PFNC Name | YUV420_8_Planar |
| Storage Type | PLANAR |
| Storage Requirement | 1.5 Bytes/pixel |

**Memory Layout**

- **Plane 0 (Y):** 1 byte per pixel, all pixels stored.
  ```
  Pixel[3,0]:Y  Pixel[2,0]:Y  Pixel[1,0]:Y  Pixel[0,0]:Y
  ```

- **Plane 1 (U/Cb):** 1 U(Cb) sample every 2 pixels in 1 line every 2 lines. H = buffer pitch.
  ```
  Row 0:  Pixel[6,0]:U(Cb)  Pixel[4,0]:U(Cb)  Pixel[2,0]:U(Cb)  Pixel[0,0]:U(Cb)
  Row 2:  Pixel[6,2]:U(Cb)  Pixel[4,2]:U(Cb)  Pixel[2,2]:U(Cb)  Pixel[0,2]:U(Cb)
  ```

- **Plane 2 (V/Cr):** 1 V(Cr) sample every 2 pixels in 1 line every 2 lines.
  ```
  Row 0:  Pixel[6,0]:V(Cr)  Pixel[4,0]:V(Cr)  Pixel[2,0]:V(Cr)  Pixel[0,0]:V(Cr)
  Row 2:  Pixel[6,2]:V(Cr)  Pixel[4,2]:V(Cr)  Pixel[2,2]:V(Cr)  Pixel[0,2]:V(Cr)
  ```

> **Note:** Only 1 U(Cb) and 1 V(Cr) sample is captured every 2 pixels in 1 line every 2 lines.

---

### 7.4. 8-bit YUV 4:2:2 Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV422PL` / Y42B |
| PFNC Name | YUV422_8_Planar |
| Storage Type | PLANAR |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

- **Plane 0 (Y):** 1 byte per pixel, all pixels stored.

- **Plane 1 (U/Cb):** 1 U(Cb) sample every 2 pixels.
  ```
  Pixel[6,0]:U(Cb)  Pixel[4,0]:U(Cb)  Pixel[2,0]:U(Cb)  Pixel[0,0]:U(Cb)
  ```

- **Plane 2 (V/Cr):** 1 V(Cr) sample every 2 pixels.
  ```
  Pixel[6,0]:V(Cr)  Pixel[4,0]:V(Cr)  Pixel[2,0]:V(Cr)  Pixel[0,0]:V(Cr)
  ```

> **Note:** Only 1 U(Cb) and 1 V(Cr) sample is captured every 2 pixels.

---

### 7.5. 8-bit YUV 4:4:4 Planar

| Property | Value |
|----------|-------|
| MultiCam Name | `YUV444PL` |
| PFNC Name | YUV444_8_Planar |
| Storage Type | PLANAR |
| Storage Requirement | 3 Bytes/pixel |

**Memory Layout**

Three separate planes, each storing 1 byte per pixel:

- **Plane 0 (Y):** `Pixel[3,0]:Y  Pixel[2,0]:Y  Pixel[1,0]:Y  Pixel[0,0]:Y`
- **Plane 1 (U/Cb):** `Pixel[3,0]:U(Cb)  Pixel[2,0]:U(Cb)  Pixel[1,0]:U(Cb)  Pixel[0,0]:U(Cb)`
- **Plane 2 (V/Cr):** `Pixel[3,0]:V(Cr)  Pixel[2,0]:V(Cr)  Pixel[1,0]:V(Cr)  Pixel[0,0]:V(Cr)`

---

## 8. Raw Data Formats

| MultiCam Name | PFNC Name | Bits/Pixel | Bytes/Pixel | Storage Type | Description |
|---------------|-----------|------------|-------------|--------------|-------------|
| `RAW8` | Raw8 | 8 | 1 | N/A | [8-bit Raw Data](#81-8-bit-raw-data) |
| `RAW10` | Raw10 | 10 | 2 | N/A | [10-bit Raw Data](#82-10-bit-raw-data) |
| `RAW12` | Raw12 | 12 | 2 | N/A | [12-bit Raw Data](#83-12-bit-raw-data) |
| `RAW14` | Raw14 | 14 | 2 | N/A | [14-bit Raw Data](#84-14-bit-raw-data) |
| `RAW16` | Raw16 | 16 | 2 | N/A | [16-bit Raw Data](#85-16-bit-raw-data) |

---

### 8.1. 8-bit Raw Data

| Property | Value |
|----------|-------|
| MultiCam Name | `RAW8` |
| PFNC Name | Raw8 |
| Storage Type | N/A |
| Storage Requirement | 1 Byte/pixel |

**Memory Layout**

Each pixel occupies 1 byte:

```
Byte offset:    +3          +2          +1          +0
              Pixel[3,0]  Pixel[2,0]  Pixel[1,0]  Pixel[0,0]
```

---

### 8.2. 10-bit Raw Data

| Property | Value |
|----------|-------|
| MultiCam Name | `RAW10` |
| PFNC Name | Raw10 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is stored in 2 bytes (16 bits), with the upper 6 bits set to zero:

```
Byte offset:    +3          +2          +1          +0
              000000 Pixel[1,0]  000000 Pixel[0,0]
```

---

### 8.3. 12-bit Raw Data

| Property | Value |
|----------|-------|
| MultiCam Name | `RAW12` |
| PFNC Name | Raw12 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is stored in 2 bytes (16 bits), with the upper 4 bits set to zero:

```
Byte offset:    +3          +2          +1          +0
              0000 Pixel[1,0]   0000 Pixel[0,0]
```

---

### 8.4. 14-bit Raw Data

| Property | Value |
|----------|-------|
| MultiCam Name | `RAW14` |
| PFNC Name | Raw14 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel is stored in 2 bytes (16 bits), with the upper 2 bits set to zero:

```
Byte offset:    +3          +2          +1          +0
              00 Pixel[1,0]     00 Pixel[0,0]
```

---

### 8.5. 16-bit Raw Data

| Property | Value |
|----------|-------|
| MultiCam Name | `RAW16` |
| PFNC Name | Raw16 |
| Storage Type | N/A |
| Storage Requirement | 2 Bytes/pixel |

**Memory Layout**

Each pixel occupies a full 16-bit word:

```
Byte offset:    +3          +2          +1          +0
              Pixel[1,0]             Pixel[0,0]
```
