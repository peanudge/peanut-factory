# Vision System Software Project (Grablink Full & TC-A160K)

A C#-based application for high-speed image acquisition and processing using the Euresys Grablink Full frame grabber and TC-A160K camera.

## Prerequisites

### 1. Hardware Requirements

- **Frame Grabber**: Euresys Grablink Full (PC1622)
  - https://www.euresys.com/ko/products/frame-grabber/grablink-full/
- **Camera**: TC-A160K (Area-Scan, Camera Link interface)
- **Cable**: Standard Camera Link cable
- **Power**: Verify PoCL (Power over Camera Link) support

### 2. Software Requirements

- **Driver**: Euresys MultiCam version 6.19.4 or higher
- **Operating System**: Windows 7/8.1/10 (32/64-bit) or Linux
- **Development Environment**: .NET Framework with C# (Visual Studio, etc.)

### 3. Library Setup

- Add the `Euresys.MultiCam.dll` assembly to your project references
- Include the namespace declaration at the top of your code: `using Euresys.MultiCam;`

## Camera Configuration (CamFile)

This project uses the `TC-A160K-SEM_freerun_RGB8.cam` file. Key parameters are as follows:

| Parameter         | Value                  |
| ----------------- | ---------------------- |
| Imaging           | AREA (Area-Scan)       |
| Resolution        | 1456(H) x 1088(V)      |
| Tap Configuration | BASE_1T24 (24-bit RGB) |
| Color Method      | RGB                    |
| Acquisition Mode  | SNAPSHOT or VIDEO      |

## Documentation

All latest documentation is available on the Euresys support page:

- [MultiCam PDF Guides](https://documentation.euresys.com/Products/MULTICAM/MULTICAM/Content/00_Home/PDF_Guides.htm)
