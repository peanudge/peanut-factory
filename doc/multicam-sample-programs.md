# MultiCam Sample Programs

> Source: Euresys MultiCam 6.19.4 (Doc D404EN)

## Contents

1. [Grablink Sample Programs](#1-grablink-sample-programs)
2. [Domino Sample Programs](#2-domino-sample-programs)
3. [Picolo Sample Programs](#3-picolo-sample-programs)
4. [General Sample Programs](#4-general-sample-programs)

---

## 1. Grablink Sample Programs

Sample programs for Grablink products.

### GrablinkSnapshot

This is a simple application demonstrating the MultiCam SNAPSHOT acquisition mode on a Grablink board. The program performs continuous image acquisitions and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C#, VB 6.0, VB.NET, Python |
| Linux 32-/64-bit | Python |

> **Note:** The sample code is written for the PC1624 Grablink Base, PC1622 Grablink Full and the PC1626 Grablink Full XR models. If you want to operate it with a PC1623 Grablink DualBase model, the `MC_Connector` MultiCam parameter has to be changed according to the sample program comments.

### GrablinkSnapshotTrigger

This is a simple application demonstrating the MultiCam SNAPSHOT acquisition mode on a Grablink board. The program performs one frame acquisition and displays it each time a hardware or software trigger event occurs.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

> **Note:** The sample code is written for the PC1624 Grablink Base, PC1623 Grablink DualBase, PC1622 Grablink Full and the PC1626 Grablink Full XR models. For any other Grablink board model, the `TrigCtl` parameter has to be updated according to the sample program comments.

### grablink_snapshot_trigger

This is a simple SDL based application demonstrating the MultiCam SNAPSHOT acquisition mode using a Grablink board. The program performs one frame acquisition and displays it each time a hardware or software trigger event occurs.

| Platform | Language |
|----------|----------|
| Linux 32-/64-bit | C/C++, SDL |

> **Note:** The sample code is written for the PC1624 Grablink Base, PC1623 Grablink DualBase, PC1622 Grablink Full and the PC1626 Grablink Full XR models. For any other Grablink board model, the `TrigCtl` parameter has to be updated according to the sample program comments.

### GrablinkHfr

This is a simple application demonstrating the MultiCam HFR acquisition mode on a Grablink board. The program performs continuous image acquisitions and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### GrablinkHfrTrigger

This is a simple application demonstrating the MultiCam HFR acquisition mode on a Grablink board. The program performs one frame acquisition and displays it each time a hardware or software trigger event occurs.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### GrablinkWeb

This is a simple application demonstrating the MultiCam WEB acquisition mode on a Grablink board. The program performs a continuous web acquisition and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### grablink_web

This is a simple SDL based application demonstrating the MultiCam WEB acquisition mode on a Grablink board. The program performs continuous web acquisition and display.

| Platform | Language |
|----------|----------|
| Linux 32-/64-bit | C/C++, SDL |

### GrablinkPageTrigger

This is a simple application demonstrating the MultiCam PAGE acquisition mode on a Grablink board. The program performs one page acquisition and displays it each time a hardware or software trigger event occurs.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### GrablinkLongPageTrigger

This is a simple application demonstrating the MultiCam LONGPAGE acquisition mode on a Grablink board. The program performs one LONGPAGE acquisition and displays it each time a hardware or software trigger event occurs.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### PlanarRGB

This is a simple application showing the planar RGB management. The application performs image acquisition and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### GrablinkDualFull

This application demonstrates video acquisition using an AVT Bonito-CL-400B camera connected to 2 Grablink Full boards.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++ |

### GrablinkMultiBase

This application demonstrates synchronized line-scan video acquisition with image stitching from several Camera Link Base line-scan cameras connected to one or several Grablink boards.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

### GrablinkPipelineController

This application demonstrates the Pipeline Controller in PAGE Acquisition Mode on a Grablink board, with a hardware page trigger connected to the IIN2 input and a motion encoder connected to the IIN1 input.

| Platform | Language |
|----------|----------|
| Windows | C/C++ 32-/64-bit |

### GrablinkSerialCommunication

This is a simple application demonstrating Camera Link serial communication through the clseremc library on a Grablink board.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

### GrablinkIoPorts

A simple graphical application showing usage of PC1624 Grablink Base, PC1623 Grablink DualBase, PC1622 Grablink Full and PC1626 Grablink Full XR I/O lines through the MultiCam Board object.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C# |

### terminal

This is a console application demonstrating the serial communication with Grablink boards using the clseremc Camera Link library.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | Python |
| Linux 32-/64-bit | C/C++, Python |

### grablink-cuda

This sample illustrates interoperability of a Grablink board, OpenGL and CUDA:

- Acquisition of 8-bit monochrome buffers using a Grablink board
- Basic processing with CUDA (Inverse 8-bit luminance)
- Rendering with OpenGL (Rotate with click and drag)

See README.txt in the sample program folder for more details.

| Platform | Language |
|----------|----------|
| Windows 64-bit | C/C++, OpenGL, nVidia CUDA |
| Linux 64-bit | C/C++, OpenGL, nVidia CUDA |

---

## 2. Domino Sample Programs

Sample programs for Domino products.

### DominoSnapshot

This is a simple application demonstrating the MultiCam Snapshot acquisition mode using a Domino board. The program performs continuous image acquisitions and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C#, VB 6.0, VB.NET |

### DominoSnapshotTrigger

This is a simple application demonstrating the MultiCam Snapshot acquisition mode using a Domino board. Each time a hardware or software trigger event occurs, the program performs one frame acquisition and displays the image.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

### domino_snapshot_trigger

This is a simple SDL-based application demonstrating the MultiCam Snapshot acquisition mode using a Domino board. The program performs one frame acquisition and displays it each time a hardware or software trigger event occurs.

| Platform | Language |
|----------|----------|
| Linux 32-/64-bit | C/C++, SDL |

---

## 3. Picolo Sample Programs

Sample programs for Picolo products.

### PicoloVideo

This is a simple application demonstrating the MultiCam Video acquisition mode using a Picolo board. The program performs continuous image acquisition and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C#, VB 6.0, VB.NET |

### picolo_video

This is a simple SDL based application that acquires and displays live images using the first Picolo board detected by MultiCam.

| Platform | Language |
|----------|----------|
| Linux 32-/64-bit | C/C++, SDL |

### PicoloVideoTrigger

This is a simple application demonstrating the MultiCam Video acquisition mode using a Picolo board. Each time a hardware or software trigger event occurs, the program performs an image sequence acquisition and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

### PicoloDirectShow

This is a simple application demonstrating video acquisition and recording using MultiCam Microsoft DirectShow filters. The program performs continuous image acquisition and display while images are recorded in an AVI file. In order to compile this sample program, the Microsoft DirectShow SDK needs to be installed and the strmbase.lib (Microsoft DirectShow library) needs to be built. Please refer to Microsoft's website and the product documentation for further information on the Microsoft DirectShow SDK.

The ffdshow codec needs to be installed to perform any image recording. This sample has been tested with the ffdshow codec versions 2033 and 3476.

| Platform | Language |
|----------|----------|
| Windows 32-bit | C/C++ |

---

## 4. General Sample Programs

Sample programs for all MultiCam products.

### MulticamAdvancedWaitEvent

A simple application demonstrating the MultiCam Snapshot acquisition mode using event driven signaling instead of callback signaling. The program performs a continuous image acquisition and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

> **Note:** This sample was written for Grablink series but can easily be converted to be used with any other board series.

### MulticamAdvancedWaitSignal

A simple application demonstrating the MultiCam Snapshot acquisition mode using the `McWaitSignal` function instead of the callback signaling method. The program performs a continuous image acquisition and display.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

> **Note:** This sample was written for Grablink series but can easily be converted to be used with any other board series.

### multicam_advanced_waitsignal

This is a simple SDL based application demonstrating the MultiCam Snapshot acquisition mode using the `McWaitSignal` function instead of the callback signaling method. The program performs a continuous image acquisition and display.

| Platform | Language |
|----------|----------|
| Linux 32-/64-bit | C/C++, SDL |

> **Note:** This sample was written for Grablink series but can easily be converted to be used with any other board series.

### TestIo

A simple console application making usage of I/O lines through the MultiCam Board object.

| Platform | Language |
|----------|----------|
| Windows 32-/64-bit | C/C++, C# |

> **Note:** The C# version of this sample program only supports PC1624 Grablink Base, PC1623 Grablink DualBase, PC1622 Grablink Full and PC1626 Grablink Full XR but the code can be easily adapted to other boards.

### io_test

A simple console application demonstrating the usage of I/O lines with the MultiCam Board object.

| Platform | Language |
|----------|----------|
| Linux 32-/64-bit | C/C++ |
