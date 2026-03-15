# Euresys Grablink Full (PC1622) Specifications

> Source: Euresys official product documentation, Grablink Functional Guide (D411EN-6.19.4.4059), and hardware manual references from documentation.euresys.com and ManualsLib.

## Contents

1. [General Information](#1-general-information)
2. [PCI Express Interface](#2-pci-express-interface)
3. [Camera Link Interface](#3-camera-link-interface)
4. [Data Transfer Performance](#4-data-transfer-performance)
5. [Input/Output Ports](#5-inputoutput-ports)
6. [Supported Pixel Formats](#6-supported-pixel-formats)
7. [On-Board Processing](#7-on-board-processing)
8. [Power Consumption](#8-power-consumption)
9. [Mechanical Specifications](#9-mechanical-specifications)
10. [Connectors](#10-connectors)
11. [Software Support](#11-software-support)

---

## 1. General Information

| Parameter | Value |
|-----------|-------|
| Manufacturer | Euresys s.a. |
| Model number | PC1622 |
| Product name | Grablink Full |
| Product family | Grablink series |
| Type | Camera Link frame grabber |
| Cooling | Air-cooled, fanless |
| Form factor | Standard profile, half-length PCIe card |

---

## 2. PCI Express Interface

| Parameter | Value |
|-----------|-------|
| Bus standard | PCI Express Rev 1.1 |
| Lane width | x4 |
| Addressing | 32-bit and 64-bit supported |

---

## 3. Camera Link Interface

| Parameter | Value |
|-----------|-------|
| Supported configurations | Base, Medium, Full, 72-bit, 80-bit |
| Number of cameras | 1 |
| Pixel clock range | 20-85 MHz |
| Camera Link cables | SDR (Shrunk Delta Ribbon) 26-pin |
| ECCO technology | Yes (Extended Camera Link Cable Operation for extended cable reach) |
| Serial communication | Camera Link serial (CC1-CC4 upstream control lines) |

The board supports the full range of Camera Link configurations from Base (1 cable, up to 28 bits per clock) through Full (2 cables, up to 64 bits per clock), as well as extended 72-bit and 80-bit modes.

---

## 4. Data Transfer Performance

Transfer rates depend on PCI Express payload size and addressing mode:

| Payload Size | Addressing | Max Transfer Rate |
|-------------|------------|-------------------|
| 256 bytes | 64-bit | Up to 833 MB/s |
| 256 bytes | 32-bit | Up to 844 MB/s |
| 128 bytes | 64-bit | Up to 754 MB/s |
| 128 bytes | 32-bit | Up to 780 MB/s |

The effective data rate depends on the performance of the PCI Express link in the host system.

---

## 5. Input/Output Ports

Each channel has a dedicated set of 10 system I/O ports:

### Input Ports (6 total)

| Port | Type | Supported Functions |
|------|------|---------------------|
| IIN1 | Isolated opto-coupled | General-purpose input, Trigger, End trigger, Line trigger, SyncBus receiver |
| IIN2 | Isolated opto-coupled | General-purpose input, Trigger, End trigger, Line trigger, SyncBus receiver |
| IIN3 | Isolated opto-coupled | General-purpose input, Trigger, End trigger, Line trigger, SyncBus receiver |
| IIN4 | Isolated opto-coupled | General-purpose input, Trigger, End trigger, Line trigger, SyncBus receiver |
| DIN1 | High-speed differential | General-purpose input, Trigger, End trigger, Line trigger |
| DIN2 | High-speed differential | General-purpose input, Trigger, End trigger, Line trigger |

### Output Ports (4 total)

| Port | Type | Supported Functions |
|------|------|---------------------|
| IOUT1 | Isolated opto-coupled | General-purpose output, Strobe, SyncBus driver |
| IOUT2 | Isolated opto-coupled | General-purpose output, Strobe, SyncBus driver |
| IOUT3 | Isolated opto-coupled | General-purpose output, Strobe, SyncBus driver |
| IOUT4 | Isolated opto-coupled | General-purpose output, Strobe, SyncBus driver |

Trigger signals are sampled at a constant 50 MHz frequency and pass through a configurable digital filter (OFF, ON, MEDIUM, STRONG).

---

## 6. Supported Pixel Formats

### Monochrome

| ColorFormat | Bit Depth | Output PFNC |
|-------------|-----------|-------------|
| Y8 | 8-bit | Mono8 |
| Y10 | 10-bit | Mono10 |
| Y12 | 12-bit | Mono12 |
| Y16 | 16-bit | Mono16 |

### Bayer CFA

| ColorFormat | Bit Depth | Patterns |
|-------------|-----------|----------|
| BAYER8 | 8-bit | BayerBG8, BayerGB8, BayerGR8, BayerRG8 |
| BAYER10 | 10-bit | BayerBG10, BayerGB10, BayerGR10, BayerRG10 |
| BAYER12 | 12-bit | BayerBG12, BayerGB12, BayerGR12, BayerRG12 |

### RGB Packed

| ColorFormat | Bit Depth | Bytes/Pixel | Output PFNC |
|-------------|-----------|-------------|-------------|
| RGB24 / RGB8 | 8-bit | 3 | BGR8 |
| RGB32 / RGBa8 | 8-bit + alpha | 4 | BGRa8 |

### RGB Planar

| ColorFormat | Bit Depth | Notes |
|-------------|-----------|-------|
| RGB24PL | 8-bit | 3 separate planes (R, G, B) |
| RGB30PL | 10-bit | 3 separate planes |
| RGB36PL | 12-bit | 3 separate planes |
| RGB48PL | 16-bit | 3 separate planes |

---

## 7. On-Board Processing

The PC1622 includes hardware processing features in its pixel pipeline:

- **Look-Up Table (LUT) transformation** -- programmable per-channel LUTs
- **Bayer CFA to RGB conversion** -- hardware debayering (Advanced and Legacy modes)
- **White balance** -- automatic and manual white balance calibration
- **Image cropping** -- hardware region-of-interest selection
- **Image flipping** -- horizontal and vertical flip (ImageFlipX, ImageFlipY)
- **Pixel formatting** -- bit depth conversion and packing

---

## 8. Power Consumption

| Parameter | Value |
|-----------|-------|
| Maximum power | 6.9 W |
| Typical power | 5.7 W |
| Current draw (+3.3V rail) | 0.48 A |
| Current draw (+12V rail) | 0.34 A |

---

## 9. Mechanical Specifications

| Parameter | Value |
|-----------|-------|
| Board length | 167.5 mm (6.6 in) |
| Board height | 111.15 mm (4.38 in) |
| Weight | 133 g (4.69 oz) |
| Bracket | Standard profile |
| Cooling | Fanless (passive air cooling) |

---

## 10. Connectors

### External (Bracket-Mounted)

| Connector | Type | Function |
|-----------|------|----------|
| Camera Link Base | 26-pin SDR socket | Base configuration camera cable |
| Camera Link Medium/Full | 26-pin SDR socket | Medium/Full configuration second cable |
| External I/O | DB25F or HD26F (via adapter cable) | Trigger inputs, strobe outputs, GPIO |

### Internal (On-Board)

| Connector | Function |
|-----------|----------|
| Power input | DC power for GPIO output drivers |
| C2C SyncBus | Camera-to-camera synchronization (via PC3305/PC3306 cables) |

### Bracket Indicators

2 LED indicators on the bracket for status monitoring.

### Adapter Cables (Accessories)

| Part Number | Description |
|-------------|-------------|
| PC1625 | DB25F I/O adapter cable |
| PC3304 | HD26F I/O adapter cable |
| PC3305 | C2C SyncBus cable |
| PC3306 | C2C Quad SyncBus cable |

---

## 11. Software Support

| Component | Details |
|-----------|---------|
| Driver | MultiCam SDK (native C API via MultiCam.dll) |
| API | MultiCam parameter-based API (McSetParamNmStr, McSetParamNmInt, etc.) |
| Acquisition modes | SNAPSHOT, VIDEO, HFR (area-scan); WEB, PAGE, LONGPAGE (line-scan) |
| Signal handling | Callback-based or polling (McWaitSignal) |
| Configuration | .cam files for camera-specific settings |
| Supported OS | Windows |
| Documentation version | MultiCam 6.19.4 |
