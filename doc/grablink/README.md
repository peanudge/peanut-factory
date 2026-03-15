# Grablink Functional Guide

> Source: Euresys Grablink 6.19.4 (Doc D411EN-Grablink Functional Guide-6.19.4.4059)
>
> Products: PC1622 Grablink Full, PC1623 Grablink DualBase, PC1624 Grablink Base, PC1626 Grablink Full XR

## Chapters

| # | File | Title | Summary |
|---|------|-------|---------|
| 1 | [01-about.md](01-about.md) | About This Document | Document scope, product list (PC1622/PC1623/PC1624/PC1626), and accessories catalog. |
| 2 | [02-camera-link-interface.md](02-camera-link-interface.md) | Camera Link Interface | Camera Link configurations (Lite through 80-bit), enable signals (FVAL/LVAL/DVAL) timing and configuration, video data signal bit assignments, and upstream control lines (CC1-CC4). |
| 3 | [03-cameras.md](03-cameras.md) | Cameras | Camera classification (`Imaging`, `Spectrum`, `DataLink`, `ColorMethod`), operation modes (area-scan/line-scan/TDI), CamConfig templates, tap configuration and geometry taxonomy, active area properties (`Hactive_Px`, `Vactive_Ln`), and Bayer CFA color registration. |
| 4 | [04-processing.md](04-processing.md) | Processing | On-board image data processing pipeline: reconstruction, cropping, flipping, LUT transformation, Bayer CFA to RGB conversion, pixel formatting (unpacking/bit-depth reduction), image transfer over PCIe, and transfer latency analysis. |
| 5 | [05-video-capture.md](05-video-capture.md) | Video Capture | Acquisition modes (SNAPSHOT, HFR, WEB, PAGE, LONGPAGE), triggering (hardware trigger, end trigger, trigger decimation/delay), exposure control (grabber-controlled and camera-controlled), strobe control, line-scan synchronization (line capture modes, line rate modes), and line trigger (rate divider, rate converter, periodic generator, quadrature decoder). |
| 6 | [06-white-balance.md](06-white-balance.md) | White Balance & Color Processing | White balance concepts, the White Balance Operator (gain/multiply/clip per channel), automatic calibration (description, requirements, timing), and AWB_AREA configuration parameters. |
| 7 | [07-io-toolbox.md](07-io-toolbox.md) | I/O Toolbox | System I/O ports overview (IIN1-4, DIN1-2, IOUT1-4), I/O functions (general-purpose I/O, trigger/end-trigger/line-trigger input, strobe output, SyncBus driver/receiver, event signaling, bracket LED), and I/O indices catalog per product. |
| 8 | [08-serial-communication.md](08-serial-communication.md) | Serial Communication | Camera Link serial communication (full-duplex async), Lite vs. Base/Medium/Full link types, virtual COM port control, and supported baud rates (600 to 230400). |

## Additional Content

The original document also contains the following advanced topics (covered in chapters 12-13 of the source PDF) which are referenced from the chapters above:

- **Synchronized Line-scan Acquisition** -- SyncBus master/slave setup for multi-camera sync
- **Metadata Insertion** -- I/O state, LVAL count, Q count, GPPC count fields in pixel data
- **Interleaved Acquisition** -- Alternating exposure/strobe programs for area-scan and line-scan
- **Two-line Synchronized Line-scan Acquisition** -- Dual-camera Bayer CFA with alternating illumination
- **Machine Pipeline Control** -- Pipeline controller for multi-stage machine vision
- **8-tap/10-bit Acquisition** -- DECA configurations for high-throughput 10-bit cameras
- **Video Lines Reordering** -- PROGRESSIVE, DUALYEND, and NYTAP reordering schemes
- **Annex: Interfacing Camera Link Cameras** -- CamFile templates and selection guidance for area-scan and line-scan cameras
