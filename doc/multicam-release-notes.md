# MultiCam 6.19.4 Release Notes (D401EN)

> Source: Euresys MultiCam 6.19.4 (Doc D401EN, build 4059, 2025-12-15)

## Contents

- [1. Release Benefits](#1-release-benefits)
- [2. Release Specification](#2-release-specification)
  - [2.1. MultiCam Products](#21-multicam-products)
  - [2.2. Supported Operating Systems](#22-supported-operating-systems)
  - [2.3. Memento](#23-memento)
  - [2.4. Development Tools](#24-development-tools)
  - [2.5. Software Tools](#25-software-tools)
  - [2.6. MultiCam Distribution](#26-multicam-distribution)
- [3. Important Notices](#3-important-notices)
  - [3.1. Notices Overview](#31-notices-overview)
  - [3.2. Notices for Windows](#32-notices-for-windows)
  - [3.3. Notices for Linux](#33-notices-for-linux)
  - [3.4. Turning-off Windows Fast Startup](#34-turning-off-windows-fast-startup)
  - [3.5. Buffer Size Limits](#35-buffer-size-limits)
  - [3.6. Memory Allocation](#36-memory-allocation)
  - [3.7. DMA Addressing Capability](#37-dma-addressing-capability)
  - [3.8. PCI Express Compatibility Issue](#38-pci-express-compatibility-issue)
  - [3.9. Low Camera Link Clock Rate](#39-low-camera-link-clock-rate)
  - [3.10. Camera and CamConfig Parameters](#310-camera-and-camconfig-parameters)
  - [3.11. Configuration Switches](#311-configuration-switches)
  - [3.12. PoCL Mode Control](#312-pocl-mode-control)
  - [3.13. BoardTopology Limitations](#313-boardtopology-limitations)
  - [3.14. PCI and PCI Express Compatibility Note](#314-pci-and-pci-express-compatibility-note)
- [4. Release Details](#4-release-details)
  - [4.1. Products Update](#41-products-update)
  - [4.2. Added/Improved Features](#42-addedimproved-features)
  - [4.3. Solved Issues](#43-solved-issues)
  - [4.4. Breaking Changes](#44-breaking-changes)
- [5. Known Issues](#5-known-issues)
- [6. Appendix](#6-appendix)
  - [6.1. Reading PCIe Endpoint Version Number](#61-reading-pcie-endpoint-version-number)

---

## 1. Release Benefits

### Improved security on Microsoft Windows

The MultiCam driver is now compatible with the Windows Core Isolation security feature. Core Isolation provides added protection against malware and other attacks by isolating computer processes from the operating system and device.

### Windows 11 support

The MultiCam driver is now supporting Windows 11, including the server versions, on x86-64 (64-bit) platforms.

### Support of recent Linux distributions & kernels

The MultiCam driver is now supporting the latest Linux distributions and kernels.

### New Python bindings for MultiCam

The new Python bindings for MultiCam allow users to call MultiCam functions and operate Grablink cards from Python scripts. They are compatible with Python 2.7 and Python 3.x under Windows and Linux.

On Windows, the MultiCam installer adds a shortcut in the Windows Start Menu to install the MultiCam Python bindings.

### Important notice

> **NOTE:** Kindly note that this is the last release that supports the development of 32-bit applications and the use of 32-bit operating systems with MultiCam. Of course, developing or running 32-bit applications developed using MultiCam packages that have been released before 31-Dec-2024 will remain possible. All developers are highly encouraged to migrate to 64-bit operating systems and 64-bit applications.

---

## 2. Release Specification

### 2.1. MultiCam Products

#### Grablink main products

| Product | S/N Prefix | Icon |
|---------|-----------|------|
| PC1622 Grablink Full | FM1 | Full |
| PC1623 Grablink DualBase | GDB | DualBase |
| PC1624 Grablink Base | GBA | Base |
| PC1626 Grablink Full XR | FXR | FullXR |

#### Grablink accessories

| Product | S/N Prefix |
|---------|-----------|
| PC1625 DB25F I/O Adapter Cable | DBC |
| PC3304 HD26F I/O Adapter Cable | 3304 |
| PC3305 C2C SyncBus Cable | 3305 |
| PC3306 C2C Quad SyncBus Cable | 3306 |

#### Domino main products

| Product | S/N Prefix | Status |
|---------|-----------|--------|
| PC1167 Domino Melody | DML | EOL |
| PC1601 Domino Symphony PCIe | DSE | EOL |

#### Picolo main products

| Product | S/N Prefix | Status |
|---------|-----------|--------|
| PC1157 Picolo Pro 2 | PP2 | EOL |
| PC1641 Picolo Alert PCIe | PAX | NRND |
| PC1685 Picolo PCIe | PIE | EOL |

### 2.2. Supported Operating Systems

#### Windows

The MultiCam drivers are designed to support all Windows versions from 7 SP1 to 11, including the Server and the IoT Enterprise versions, on x86-64 (64-bit) and x86 (32-bit) platforms.

**Release validation:**

| OS Name & Version | Platform | Notes |
|-------------------|----------|-------|
| Microsoft Windows 7 | x86 (32-bit) | Service Pack 1 with the latest updates |
| Microsoft Windows 10 | x86-64 (64-bit) | Enterprise edition - Version 22H2 |

> **NOTE:**
> - The driver is signed by Microsoft.
> - MultiCam 6.19 is the last release that supports Windows 7, Windows 8.1, Windows Server 2008 R2, and Windows Server 2012 R2.
> - In 2026, the minimum Windows 10 version requirements will change to "Windows 10 version 1809 or later".

> **NOTE:** Kindly note that this is the last release that supports the development of 32-bit applications and the use of 32-bit operating systems with MultiCam.

> **WARNING:**
> - Power saving modes of the operating systems (StandBy, Sleep, Suspend...) are not supported.
> - The PCI Express Active-State Power Management (ASPM) must be disabled!
> - Kernel DMA Protection is not supported.
> - Windows 11 doesn't support x86 (32-bit) platforms!

#### Linux

The MultiCam drivers are designed to be distribution-independent on x86 (32-bit) and x86-64 (64-bit) platforms. They are expected to work with a wide range of distributions.

**Release validation:**

| OS Name & Version | Platform | Notes |
|-------------------|----------|-------|
| Linux Debian Lenny 5.0.1 | x86 (32-bit) | Kernel version 2.6.35 |
| Linux Ubuntu 18.04 LTS | x86-64 (64-bit) | Kernel version 4.15.0 |

> **NOTE:** Kindly note that this is the last release that supports the development of 32-bit applications and the use of 32-bit operating systems with MultiCam.

> **WARNING:** Power saving modes of the operating systems (StandBy, Sleep, Suspend...) are not supported.

**Requirements:**

- The PCI Express Active-State Power Management (ASPM) must be disabled.
- MultiCam tools and libraries require kernel version 2.6.32 or higher.
- MultiCam Studio requires glibc version 2.12 (or higher).
- Other tools and libraries require glibc version 2.4 (or higher).

> **NOTE (2026 changes):**
> - The minimum kernel version requirements will change to Linux kernel version 4.4 or higher.
> - The GNU C library (glibc) requirements will change to GLIBC 2.23 or higher.

### 2.3. Memento

MultiCam (version 6.12 and later) supports Memento (version 4.5 or later).

### 2.4. Development Tools

MultiCam 6.19.4 should be usable with any development tool that supports at least one of the following interfaces.

> **NOTE:** These programming interfaces also cover most of the available development tools used with other languages.

#### C/C++

MultiCam 6.19.4 is supplied as:

- A 32-bit binary library (Windows and Linux) designed to be used with ISO-compliant C/C++ compilers for the development of x86 (32-bit) applications.
- A 64-bit binary library (Windows and Linux) designed to be used with ISO-compliant C/C++ compilers for the development of x86-64 (64-bit) applications.

#### DirectShow filters (Windows and Picolo series only)

- DirectShow 32-bit filters designed to be used with x86 (32-bit) Microsoft Visual C++ compilers for the development of x86 (32-bit) applications.
- DirectShow 64-bit filters designed to be used with x86-64 (64-bit) Microsoft Visual C++ compilers for the development of x86-64 (64-bit) applications.

#### Serial communication DLL (Windows and Linux on Grablink series only)

- 32-bit and 64-bit dynamic libraries (Windows and Linux on Grablink series only) designed to be used for the serial communication with Camera Link cameras.

#### .NET

MultiCam 6.19.4 is delivered with 32-bit and 64-bit .NET `MultiCam_NetApi.dll` assemblies, based on the `MultiCam.cs` interface file provided with the C# sample programs.

They are designed to be used with development environments compatible with .NET frameworks version 2.0 or higher. You can find more information on this API in the sample programs source code.

> **NOTE:** The previously available ActiveX controls library and .NET assembly are now deprecated and have been removed from the MultiCam package. MultiCam can still be used with Microsoft Visual Basic 6 or a .NET language provided that the C API is called directly. Sample programs are available beside the driver in the MultiCam download area of the Euresys website.

#### Python

Python bindings for MultiCam allow users to call MultiCam functions and operate Grablink frame grabbers from Python scripts. They are compatible with Python 2.7 and 3.x under Windows and Linux.

On Windows, the MultiCam installer adds a shortcut in the Windows Start Menu to install the MultiCam Python bindings.

### 2.5. Software Tools

| Tool Name | Description |
|-----------|-------------|
| MultiCam Studio (64-bit) | 64-bit version of the GUI tool giving access to all the MultiCam features including image acquisition and display |
| MultiCam Studio | 32-bit version of MultiCam Studio |
| Camera Link Validation Tool (64-bit) | 64-bit version of the Euresys tool used to validate the operational parameters of a Camera Link installation |
| Camera Link Validation Tool | 32-bit version of the Camera Link Validation Tool |

### 2.6. MultiCam Distribution

MultiCam is distributed on the "Support > Download Area" page of the Euresys web site: <https://www.euresys.com/en/download-area/>

> **NOTE:** The first time access requires a profile creation to obtain a user ID and a password.

The MultiCam panel contains 4 sections:

#### Release Notes section

The "Release Notes" section provides the MultiCam Release Notes in PDF format.

#### Documentation section

The "Documentation" section provides:

- `multicam-win-offline-documentation-<ma.mi.re.bu>.exe`: a Windows installer for the offline MultiCam Documentation
- `multicam-linux-offline-documentation-<ma.mi.re.bu>.tar.gz`: a compressed archive of the MultiCam Documentation

The Windows installer installs locally the MultiCam Documentation and creates shortcuts in the Windows Start menu. The compressed archive can be unpacked anywhere.

#### Setup Files section

The "Setup Files" section provides MultiCam drivers setup files for Windows and Linux:

**MultiCam driver setup files for Windows:**

- For Windows 10 and higher: `multicam-win10-<ma.mi.re.bu>.exe`
- For prior editions of Windows: `multicam-win7-<ma.mi.re.bu>.exe`

> **NOTE:**
> - On x86-64 (64-bit) Host PC's, the Windows installers install the 64-bit MultiCam driver, the 32-bit and 64-bit variants of the Development Tools and Software Tools.
> - On x86 (32-bit) Host PC's, the Windows installers install the 32-bit MultiCam driver, the 32-bit and 64-bit variants of the Development Tools and only the 32-bit variant of the Software Tools.

**MultiCam driver setup files for Linux:**

- For x86-64 (64-bit) processor architecture: `multicam-linux-x86_64-<ma.mi.re.bu>.tar.gz` -- installs also the 64-bit variants of the Development Tools and Software Tools.
- For x86 (32-bit) processor architecture: `multicam-linux-x86-<ma.mi.re.bu>.tar.gz` -- installs also the 32-bit variants of the Development Tools and Software Tools.

#### Sample Programs section

The "Sample Programs" section provides two sample programs packages that demonstrate how to interface a frame grabber, acquire images and activate various functions on Grablink, Domino and Picolo products:

- `multicam-win-sample-programs-<ma.mi.re.bu>.zip` compressed archive for Windows
- `multicam-linux-sample-programs-<ma.mi.re.bu>.tar.gz` compressed archive for Linux

> **NOTE:** For a short description of each program, refer to the D404EN-MultiCam Sample Programs PDF document available on PDF Guides page of the MultiCam Documentation: <https://documentation.euresys.com/Products/MultiCam/MultiCam/Content/00_Home/PDF_Guides.htm>

---

## 3. Important Notices

Important notifications to be read before installing and/or using the product on your PC!

### 3.1. Notices Overview

**Global notices:**

- Power saving modes of the operating systems (StandBy, Sleep, Suspend...) are not supported.
- The maximum buffer size is 2 GB. Refer to [Buffer Size Limits](#35-buffer-size-limits) for more information.
- Despite the support of 64-bit OS, the physical memory addressable by a frame grabber is determined by the hardware capabilities. Refer to [DMA Addressing Capability](#37-dma-addressing-capability) for a list of products supporting 64-bit DMA transfers.
- For boards having only a 32-bit physical addressing capability, it is mandatory to allocate the image buffer into the lowest 4 GB of the physical memory. For systems having more than 4 GB of physical memory, it is mandatory to use the automatic memory allocation mode of MultiCam. For more information, refer to [Memory Allocation](#36-memory-allocation).

See also: [Notices for Windows](#32-notices-for-windows) and [Notices for Linux](#33-notices-for-linux)

**Additional notices for Grablink:**

- 4-lane PCI Express Grablink cards support only the x4 link width. Refer to [PCI Express Compatibility Issue](#38-pci-express-compatibility-issue) for more information.
- To operate latest generation of Grablink boards with a Camera Link clock rate lower than 30 MHz, refer to [Low Camera Link Clock Rate](#39-low-camera-link-clock-rate) for more information.
- Restrictions apply on allowed values for the Camera and CamConfig parameters. Refer to [Camera and CamConfig Parameters](#310-camera-and-camconfig-parameters) for more information.
- There are configuration switches on the latest generation of Grablink products. Refer to [Configuration Switches](#311-configuration-switches) for more information.
- PoCL mode control is only effective when the MultiCam channel is in the Ready state. Refer to [PoCL Mode Control](#312-pocl-mode-control) for more information.
- The value of the BoardTopology parameter cannot be changed while a Camera Link serial port is in use. Refer to [BoardTopology Limitations](#313-boardtopology-limitations) for more information.

**Additional notices for Picolo:**

- Picolo cards based on the Conexant Fusion 878a have specific PCI and PCI Express bus requirements. Refer to [PCI and PCI Express Compatibility Note](#314-pci-and-pci-express-compatibility-note) for more information.

### 3.2. Notices for Windows

Important notifications to be read before installing and/or using the product on your Windows PC.

#### Always trust Euresys code-signing certificate on Windows 7 and 8.1

A Windows Security warning message may occur at driver installation on Microsoft Windows 7 and 8.1 when the Euresys code-signing certificate is missing from the "Trusted Publishers" Windows Certificate store. This happens, for instance, when the Euresys code-signing certificate must be renewed.

Follow the instructions to install the current Euresys code-signing certificate into the "Trusted Publishers" Windows certificate store.

#### Missing time-stamping certificate

A Windows Security warning message may occur at driver installation on Microsoft Windows when the GlobalSign Root CA - R6 certificate is missing from the Windows certificate store.

This issue can be solved by installing this missing certificate which can be downloaded from the GlobalSign website then installed in the Trusted Root Certification Authorities (local computer) certificate store.

#### Support of SHA-256 certificates for Windows 7

Microsoft Windows 7 and Microsoft Windows Server 2008 R2 now require at least SP1 as well as some specific Windows updates in order to support SHA-256 certificates.

The following Windows update is required and must be installed before using Euresys drivers on Microsoft Windows 7 and Microsoft Windows Server 2008 R2:

- **KB3033929** (provides support for SHA-256 certificates which are required by Microsoft): without this one, a "Windows cannot verify the digital signature for the drivers required for this device" (code 52) error will prevent the Euresys drivers from loading.

#### Additional notices for Windows

- MultiCam Service must be started before opening the driver. On Windows operating systems, MultiCam relies on a service named "MultiCam Service". This service is automatically started when the computer boots. Software should only access MultiCam when this service is started. `McOpenDriver` will return `MC_SERVICE_ERROR` if the MultiCam service is not started when called.
- Windows Fast Startup feature is not supported. Refer to [Turning-off Windows Fast Startup](#34-turning-off-windows-fast-startup) for more information.

#### Additional notices for Windows 32-bit

- When PAE (Physical Address Extension) is activated, it is mandatory to use the automatic memory allocation mode of MultiCam.
- The development of 64-bit applications with MultiCam is also possible on a 32-bit Windows installation, providing that the x86-64 (64-bit) development tools are properly installed. Both the 32-bit and 64-bit versions of `MultiCam.lib` import libraries are installed when the import libraries installation option is checked.

### 3.3. Notices for Linux

Important notifications to be read before installing and/or using the product on your Linux PC.

#### Memento installation

Memento must be installed prior to MultiCam.

If the MultiCam package is already installed, proceed as follows:
1. Uninstall MultiCam.
2. Install Memento.
3. Re-install MultiCam.

#### Additional notices for Linux 64-bit

- It is allowed to allocate memory anywhere in the available physical memory addressing space. When memory is allocated beyond the lowest 4 GB of the physical memory addressing space, the OS performs automatically a buffer copy.
- For systems with more than 2 GB of RAM:
  - The kernel reserves swiotlb memory for DMA transfers with devices that do not support 64-bit addressing.
  - Usually, the default is 64MB for the whole system. This may be insufficient for the total amount of memory used by your surfaces. To increase the amount of memory that can be used for acquisitions, you can use the swiotlb boot option `swiotlb=n` where `n` is the desired number of swiotlb slabs (1 slab = 2KB).
  - Refer to [DMA Addressing Capability](#37-dma-addressing-capability) for a list of products supporting 64-bit DMA transfers.

#### Additional notices for Linux 32-bit

- PAE (Physical Address Extension) is not supported. The memory allocated for MultiCam must be below 4 GB in the physical memory addressing space.
- For systems with more than 2 GB of RAM:
  - Devices that do not support 64-bit addressing must use memory below 4 GB for DMA transfers.
  - The `mem` kernel boot option can be used to restrict the amount of memory used by the OS. For example: `mem=3000M`
  - Refer to [DMA Addressing Capability](#37-dma-addressing-capability) for a list of products supporting 64-bit DMA transfers.

### 3.4. Turning-off Windows Fast Startup

The Microsoft Windows Fast Startup feature which is available since Windows 8 is not supported by the MultiCam driver. Please make sure to turn it off before using MultiCam.

**To turn off the Fast Startup feature:**

1. Go to the Control Panel then click on the Power Options icon.
2. Click on the "Choose what the power buttons do" link on the left side.
3. Click on the "Change settings that are currently unavailable" link at the top.
4. If prompted by UAC, then click on "Yes".
5. Under Shutdown settings, uncheck the "Turn on fast startup" checkbox if it is listed, then click on the "Save changes" button.

The Fast Startup feature is now disabled.

> **WARNING:** After a Windows 10 upgrade, the Fast Startup feature can be enabled automatically again. Please, re-apply the procedure after any Windows 10 upgrade.

### 3.5. Buffer Size Limits

The maximum buffer size allowed per MultiCam surface is 2 GB.

If a MultiCam surface exceeds this limit, MultiCam returns `MC_IO_ERROR` at channel activation.

### 3.6. Memory Allocation

The recommended method for allocating memory to the surfaces of MultiCam is the **"Automatic method"** since this is the only method that is always applicable.

The usage of the "manual" memory allocation method is restricted to the following cases:

- On "Windows 32-bit without PAE" systems, without any further restrictions
- On boards having 64-bit DMA addressing capability, without any further restrictions
- On Linux operating systems, without any further restrictions: The Linux kernel provides a buffering system ensuring that the DMA operates always in the lowest 4 GB of physical addressing space.

The "manual" method is **prohibited** when:

- The board has no 64-bit DMA capability **and**
- The system has physical memory beyond the 4 GB address boundary **and**
- The operating system is "Windows x86 (32-bit) with PAE" or "Windows x86-64 (64-bit)"

Since MultiCam 6.5.1, MultiCam returns the `MC_INVALID_SURFACE` error on channel activation if the manual memory allocation method is used in a prohibited case.

### 3.7. DMA Addressing Capability

The following products have a 64-bit DMA engine allowing the MultiCam surfaces to be allocated anywhere in the user memory space:

| Product | S/N Prefix |
|---------|-----------|
| PC1622 Grablink Full | FM1 |
| PC1623 Grablink DualBase | GDB |
| PC1624 Grablink Base | GBA |
| PC1626 Grablink Full XR | FXR |

Other MultiCam products have a 32-bit DMA engine. The MultiCam surfaces must be allocated in the lowest 4 Gigabytes of the memory space.

### 3.8. PCI Express Compatibility Issue

**Applies to:** DualBase, Full, FullXR

Since version 138 (0x8A) of the PCI Express endpoint interface, PC1623 Grablink DualBase, PC1622 Grablink Full and PC1626 Grablink Full XR support exclusively the **x4 link width**.

> **WARNING:** The x1 link width is no longer supported!

> **NOTE:** The version of the PCI Express endpoint interface is given by the `PCIeEndpointRevisionID` board parameter.

### 3.9. Low Camera Link Clock Rate

**Operating latest generation of Grablink boards with a Camera Link clock rate lower than 30 MHz.**

**Applies to: Base, Full**

Since MultiCam 6.7.2.1677, to be able to use cameras with a Camera Link clock rate lower than 30 MHz, an application must set the `BoardTopology` board parameter value to `MONO_SLOW`.

**Applies to: DualBase**

Since MultiCam 6.7.2.1677, to be able to use cameras with a Camera Link clock rate lower than 30 MHz, an application must set the `BoardTopology` board parameter value to `DUO_SLOW`.

> **NOTE:** Both channels are configured for low speed operation. It is not possible to configure only one channel for low-speed operation.

**Applies to: FullXR**

> **WARNING:** PC1626 Grablink Full XR doesn't operate with a Camera Link clock rate lower than 30 MHz!

### 3.10. Camera and CamConfig Parameters

**Applies to:** Base, DualBase, Full, FullXR

The allowed values for the `CamConfig` parameters are `PxxSC`, `PxxRC`, `PxxRG`, `LxxxxSC`, `LxxxxSP`, `LxxxxRC`, `LxxxxRP`, `LxxxxRG` and `LxxxxRG2`.

All CamFiles have been adapted accordingly and can be downloaded from the support pages of the Euresys website: <https://www.euresys.com/Frame-Grabbers-Supported-Cameras>

Other boards are not concerned but it is however recommended to use the latest available CamFiles in each case.

### 3.11. Configuration Switches

**Applies to:** Base, DualBase, Full, FullXR

These boards feature a set of configuration switches. For normal operation, **both switches must be in the ON position**.

Should recovery mode be enabled by error, the Grablink board appears respectively as "GRABLINK Base (Recovery)", "GRABLINK DualBase (Recovery)", "GRABLINK Full (Recovery)" and "GRABLINK Full XR (Recovery)" in Windows Device Manager and is not functional.

To restore normal operation, power off the PC, change the switches to normal position and then reboot.

### 3.12. PoCL Mode Control

**Applies to:** Base, DualBase, FullXR

Any modification of the `PoCL_Mode` parameter is only effective when the MultiCam channel is in the Ready state.

Specifically, to turn off the power of the camera:

1. Set the `PoCL_Mode` parameter to value `OFF`.
2. Set the `ChannelState` parameter to value `READY`.

### 3.13. BoardTopology Limitations

**Applies to:** Base, DualBase, Full, FullXR

The value of the `BoardTopology` parameter can be modified only if no Camera Link serial port is in use.

> **NOTE:** This occurs for instance when a camera manufacturer application uses the Virtual COM port or the Camera Link serial library.

Besides this restriction, in a MultiCam application, `BoardTopology` should always be modified at initialization before doing anything else since it implies hardware reconfiguration.

### 3.14. PCI and PCI Express Compatibility Note

**PCI and PCI Express bus requirements for Picolo cards based on the Conexant Fusion 878a.**

**Applies to:** Picolo-e (EOL), Pro2 (EOL)

> **WARNING:** This notice applies only to Picolo cards based on the Conexant Fusion 878a chip.

These cards do not have a large on-board frame buffer and in order to ensure correct image/video transfer, the response time (also called latency) of the PCI or PCIe bus of the motherboard must be low enough. The latency of the PCI or PCIe bus primarily depends on the architecture of the motherboard. It may also depend on the operating system, the BIOS version and BIOS settings. We have observed that the latency of the PCIe bus of many motherboards using the latest generation CPUs (for example Intel Core CPU 6th and 7th generations) is too long and causes issues with these Picolo cards. If the latency of the bus is too long, randomly distributed black lines may appear in the image acquired. They are caused by the long response time of the PCI or PCIe bus, leading to FIFO overruns. The acquisition may also eventually stop.

The requirements for correct operation depend on the color format and the buffer pitch used. Both are set by the application using the Picolo through MultiCam parameters:

| Color Format | Buffer Pitch | Max Allowed Bus Latency | Notes |
|-------------|-------------|------------------------|-------|
| RGB24 (default) | default | 11 us | Least favorable case |
| RGB24 | 4096 | 17 us | |
| YUV422 (packed) or RGB16 | 4096 | 29 us | |
| YUV411 (packed) | 4096 | 37 us | Most favorable case |

If you experience this problem, try changing these parameters towards a more favorable case. If this is not possible or if this does not solve the problem, this Picolo card may not be compatible with your motherboard. We recommend you use the PC1641 Picolo Alert PCIe instead.

---

## 4. Release Details

### 4.1. Products Update

**Modification(s) to the list of supported products and/or accessories in MultiCam 6.19**

#### No longer supported product

**Applies to:** Value (EOS), Express (EOS), Alpha2 (EOS)

Grablink Value, Grablink Express and Domino Alpha 2 are no longer supported starting from MultiCam 6.19.

### 4.2. Added/Improved Features

#### MultiCam 6.19.4

**Added Python sample program**
*Applies to: Base, DualBase, Full, FullXR*

Added Python terminal sample program that shows how to use the clseremc Camera Link serial library.

**Improved terminal C sample program**
*Applies to: Base, DualBase, Full, FullXR*

- Use `clGetNumBytesAvail` to check the input queue
- Send end of line character to the camera if needed
- Make sure `clSerialClose` is always called when the program is interrupted

**Improved Memento traces for the serial line**
*Applies to: Base, DualBase, Full, FullXR*

Improved Memento traces of data read and written on the Camera Link serial line.

#### MultiCam 6.19.2

**Improved virtual COM port**
*Applies to: Base, DualBase, Full, FullXR, Symphony (EOL)*

Administrator access rights are no longer needed to set the value of the `SerialControl[A,B,C,D]` parameters and use the virtual COM port.

#### MultiCam 6.19.1

**Improved MultiCam Studio**

New Refresh button in the Properties Dialog control panel of MultiCam Studio.

MultiCam Studio property pages are not refreshed automatically. For instance, to update the value of any counter (such as `EncoderTickCount`, `LineTriggerViolation`), it was mandatory to switch back and forth between parameter categories. Now, this can be achieved by clicking the Refresh button.

The refresh button has been implemented as follows (shown for GrabWindow):

1. Change the parameter value
2. Click on the Refresh button (below Save CAM file button)
3. The Properties panel is refreshed with new parameters

#### MultiCam 6.19

**Improved security on Microsoft Windows**

The MultiCam driver is now compatible with the Windows Core Isolation security feature. Core Isolation provides added protection against malware and other attacks by isolating computer processes from the operating system and device.

**Improved installation of driver on Linux**

- Improved installation of drivers on recent Linux distributions
- Improved installation on Linux:
  - List modules in `/etc/modules` to ensure that they are loaded when DKMS is activated
  - Ensure enrollment of keys in `check-install.sh` when "Secure Boot" is enabled
  - Improved installation of drivers on Red Hat 8.8

**Added Python bindings for MultiCam**

The Python bindings for MultiCam are provided in a Python wheel installation package (a `.whl` file) located in the `python` subdirectory of the MultiCam installation directory.

Depending on your Python setup, installation can be as easy as:

```
python -m pip install <PATH_TO_MULTICAM_WHL>
```

**Added Python sample program**
*Applies to: Base, DualBase, Full, FullXR*

Added Python GrablinkSnapshot sample program.

**Added Microsoft Visual C++ sample program**
*Applies to: Base, DualBase, Full, FullXR*

Added Microsoft Visual C++ GrablinkPipelineController sample program.

**Updated sample programs**

Updated TestIo and io_test sample programs to avoid using deprecated parameters.

### 4.3. Solved Issues

#### MultiCam 6.19.4

- Fixed installation on RHEL/CentOS 9.6 (kernel 5.14.0-513)
- *Applies to: Picolo-e (EOL), Pro2 (EOL)* -- Fixed channel activation failure (`MC_IO_ERROR`) that could follow right after changing the value of parameters affecting the cluster (e.g. `ColorFormat`, `GrabField`, `SurfaceCount`...)

#### MultiCam 6.19.3

- *Applies to: Base, DualBase, Full, FullXR* -- Fixed clseremc library that sometimes did not detect every board in the system during initialization
- *Applies to: Base, DualBase, Full, FullXR, Symphony (EOL)* -- Fixed handling of virtual COM ports when `SERIALCOMM` registry key did not exist
- *Applies to: Base, DualBase, Full, FullXR* -- Fixed incrementation of the `OverrunCount` parameter
- *Applies to: Base, DualBase, Full, FullXR* -- Added missing `MC_MetadataLocation_TAP1` value for the `MetadataLocation` parameter
- Fixed installation on Linux kernel 6.15
- Fixed objtool warnings when compiling kernel modules on Linux 6.2 and later

#### MultiCam 6.19.2

- *Applies to: Base, DualBase, Full, FullXR, Symphony (EOL)* -- Fixed `DRIVER_IRQL_NOT_LESS_OR_EQUAL` Blue Screen (bug check 0xD1) that could occur when using the virtual COM port with some Windows 10 applications

#### MultiCam 6.19.1

- Fixed build of MultiCam drivers on Linux openSUSE Leap 15.3
- *Applies to: Alert-e (NRND)* -- Fixed frozen video acquisition after 1 frame
- *Applies to: Picolo-e (EOL), Pro2 (EOL)* -- Fixed card installation and detection on Linux
- MultiCam Studio:
  - Fixed image refresh issue when using scroll bars
  - Fixed unintended dependency on the `libjbig.so.0` library on Linux
  - Fixed layout issue in menu bar's zoom controls on some Linux distributions
  - Fixed image display issue when zooming
  - Fixed swap of red and blue pixel components for RGB24 and RGB32 color formats when `ColorComponentsOrder=RGB`

#### MultiCam 6.19

- Fixed installation on Linux kernels 6.5, 6.6, 6.7 and 6.8
- Fixed installation on RHEL/CentOS 9.1, 9.2 and 9.3
- Fixed build of Microsoft Visual C++ sample programs with Visual Studio 2022
- Fixed build of `multicam_advanced_waitsignal` Linux sample program

### 4.4. Breaking Changes

#### Since MultiCam 6.19.3

The range of values for `StrobeDur` is now `[1, 100]` instead of `[0, 100]`. The only way to set a strobe line to the inactive level is to set `StrobeMode=OFF`.

#### Since MultiCam 6.19

The minimal GLIBC version required for MultiCam Studio Linux binaries is now 2.12.

---

## 5. Known Issues

### Incorrect trigger delays

**Applies to:** Base, DualBase, Full, FullXR

The effective trigger delay is only 80% of the `TrigDelay_us` set value.

**Workaround:** Set `TrigDelay_us` to 125% of the desired value.

### Unsupported padding when using McConvertSurface for pixel format conversion

The `McConvertSurface` conversion does not work as expected when there is padding in the input surface, i.e. when image width is not a multiple of 8 bytes.

### Missing image format filter when saving images with MultiCam Studio

The user has the possibility to select three different file formats for saving images (BMP, TIFF and JPEG). However, none of these formats support all available image formats (e.g.: BMP does not support 16-bit images). The user should always choose an appropriate file format to save images. The user should also not use the "All" option when not all displayed images are compatible with the chosen file format. If the user chooses the wrong format, no image will be saved on the hard disk.

### Invalid "0x00" character sent on Camera Link serial port at FPGA initialization

**Applies to:** Base, DualBase, Full, FullXR

These boards send a spurious `0x00` character on Camera Link serial port at FPGA initialization. Please contact Euresys support when this causes troubles with your camera.

### MultiCam Studio is unable to display 30-bit and 40-bit MultiCam Packed Pixel Formats

**Applies to:** Full, FullXR

MultiCam Studio does not support display of `RGB30P` and `RGBI40P` color formats.

### Acquisition settings restrictions on Picolo Alert

**Applies to:** Alert-e (NRND)

These boards provide 16 channels grouped by 4. Channel 1, 5, 9, 13 belong to the first group; channels 2, 6, 10, 14 belong to the second group and so on.

Within a group, all channels must be configured with the same video standard and the same resolution. Other settings are free.

### Infrequent device start failure after cold boot

**Applies to:** Base, DualBase, Full, FullXR

Some cards may infrequently fail to start properly after power up (PC cold boot) and are not detected by the PC. The card operates properly again, after a power down / power up cycle of the PC.

The cards with the following version numbers may exhibit this issue: v128 (0x80), v129 (0x81), v130 (0x82), v131 (0x83), v132 (0x84), v133 (0x85), v134 (0x86), v135 (0x87), v136 (0x88), v160 (0xA0), v161 (0xA1). Cards with other version numbers do not present this issue.

Refer to [Reading PCIe Endpoint Version Number](#61-reading-pcie-endpoint-version-number).

### MC_SIG_END_ACQUISITION_SEQUENCE sometimes occurs before last MC_SIG_SURFACE_PROCESSING

**Applies to:** Base, DualBase, Full, FullXR

When using `AcquisitionMode = LONGPAGE`, the `MC_SIG_END_ACQUISITION_SEQUENCE` signal is sometimes issued before the last `MC_SIG_SURFACE_PROCESSING` signal of a sequence.

### Invalid image borders when using Cropping with a Bayer CFA camera

**Applies to:** Base, DualBase, Full, FullXR

When using a cropped window with a Bayer CFA camera, the 4 borders of acquired images (i.e. the first and last lines as well as the first and last columns) contain invalid data.

### Line-scan acquisitions with PageLength_Ln = 1 may lead to segmentation fault or kernel panic under Linux

**Applies to:** Base, DualBase, Full, FullXR

When performing line-scan acquisitions with `PageLength_Ln` set to 1, some segmentation fault or kernel panic issues have been observed in rare cases, depending on the Linux distribution used.

**Workaround:** If the problem occurs, setting `PageLength_Ln` to a value greater than 1 will make it disappear.

### MC_SIG_END_ACQUISITION_SEQUENCE signal is generated twice when EndTrigEffect = FOLLOWINGLINE

**Applies to:** Base, DualBase, Full, FullXR

When using the LONGPAGE acquisition mode with `EndTrigMode = HARD` and `EndTrigEffect = FOLLOWINGLINE`, the `MC_SIG_END_ACQUISITION_SEQUENCE` signal is generated twice when enabled.

**Workaround:** Use `EndTrigEffect = PRECEDINGLINE` or handle the `MC_SIG_END_ACQUISITION_SEQUENCE` signal twice.

### Misbehaviour of the trigger decimation unit when using both software and hardware triggers together

**Applies to:** Base, DualBase, Full, FullXR

The trigger decimation unit does not take the occurrence of software triggers into account for the decimation counter. This may lead to misbehavior especially when the first acquisition phase has been software triggered. In this case the trigger decimation unit is still continuing to consider the value of `TrigDelay_Pls` parameter instead of `NextTrigDelay_Pls` parameter. The `NextTrigDelay_Pls` parameter is only taken into account from the second hardware initiated trigger event.

**Workaround:** Avoid using software triggers with this feature or use the same value for both `TrigDelay_Pls` and `NextTrigDelay_Pls` parameters.

### Synchronized acquisition broken on slave channels for specific LONGPAGE configuration

**Applies to:** Base, DualBase, Full, FullXR

Synchronized acquisition using two or more line-scan cameras connected on several boards is broken on slaves when channels are restarted in the following conditions:

- `AcquisitionMode = LONGPAGE`
- `BreakEffect = FINISH`
- `EndTrigMode = HARD`
- The master channel is set to the IDLE state before receiving the hardware end trigger and before setting the slave channels to the IDLE state.

**Workaround:** Set all slave channels to IDLE before setting the master channel to IDLE.

### LineTriggerViolation wrongly incremented at channel (de)activation when using the rate converter

**Applies to:** Base, DualBase, Full, FullXR

The `LineTriggerViolation` parameter is wrongly incremented when a channel is activated or deactivated if `LineRateMode` is set to `CONVERT`.

### The upper limit of Hactive_Px is 65504 instead of 65535

**Applies to:** Base, DualBase, Full, FullXR

The upper limit for the `Hactive_Px` parameter is currently 65504 instead of 65535 (this value depends on the `TapConfiguration` parameter value). When setting a value greater than 65504, MultiCam returns `MC_RANGE_ERROR`.

### Cannot change connector for a camera without creating the channel again

**Applies to:** DualBase

If a channel is first created on the A connector, no acquisition will be performed by just setting the `Connector` parameter to the B value when changing the camera from the A connector to the B connector. In that case, the channel must be created again using the B connector.

### Inoperative timeout for clSerialRead and clSerialWrite functions of the Camera Link serial Linux library

**Applies to:** Base, DualBase, Full, FullXR

Under Linux, the `clSerialRead` and `clSerialWrite` functions of the `libclseremc.so` library do not take the timeout passed as fourth argument into account. These functions simply return `CL_ERR_NO_ERR` immediately instead of `CL_ERR_TIMEOUT` when no data could be read or written within the specified timeout.

### Inoperative StartExposure signal for subsequent images in a sequence

**Applies to:** Base, DualBase, Full, FullXR

When acquiring a sequence of 2 or more images, the `MC_SIG_ACQUISITION_FAILURE` signal is only issued for the first acquired image.

There is no workaround.

### Invalid strobe pulse when using PreStrobe_us parameter

**Applies to:** Melody (EOL)

The pre-strobe function is not functional. There is no workaround.

### MULTIPLE_IRP_COMPLETE_REQUESTS Blue Screen occasionally occurs on some systems

**Applies to:** Picolo-e (EOL), Pro2 (EOL)

On some systems a `MULTIPLE_IRP_COMPLETE_REQUESTS` Blue Screen might occasionally occur.

### ImageSizeX is 702 instead of 704 when Standard = PAL and PixelTiming = BROADCAST

**Applies to:** Picolo-e (EOL), Pro2 (EOL)

When `Standard = PAL` and `PixelTiming = BROADCAST`, `ImageSizeX` is wrongly set to 702 pixels instead of 704.

**Workaround:** Manually set `ImageSizeX` to the correct value.

### Left-over binaries after uninstalling MultiCam from Windows 7

Since Windows 7, some MultiCam driver binaries located in `C:\Windows\system\euresys\multicam` are left on the system after uninstalling MultiCam. Deleting them manually is allowed once MultiCam has been uninstalled.

---

## 6. Appendix

### 6.1. Reading PCIe Endpoint Version Number

There are two methods to read the PCIe endpoint version number of a Grablink card:

1. **Using Windows Device Manager** -- Properties Dialog showing the PCIe endpoint version number in hexadecimal format.
2. **Using MultiCam Studio** -- Board Information Dialog showing the PCIe endpoint version number in decimal format.
