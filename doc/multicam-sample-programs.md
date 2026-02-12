SAMPLE PROGRAMS

MultiCam

MultiCam Sample Programs

© EURESYS s.a. 2025 - Doc. D404EN-MultiCam Sample Programs-6.19.4.4059 built on 2025-12-15

MultiCam MultiCam Sample Programs

This documentation is provided with MultiCam 6.19.4 (doc build 4059).
www.euresys.com

2

MultiCam MultiCam Sample Programs

Contents

1. Grablink Sample Programs

2. Domino Sample programs

3. Picolo Sample Programs

4. General Sample Programs

4

8

9

10

3

MultiCam MultiCam Sample Programs

1. Grablink Sample Programs

SampleprogramsforGrablinkproducts

GrablinkSnapshot

This is a simple application demonstrating the MultiCam SNAPSHOT acquisition mode on a
Grablink board. The program performs continuous image acquisitions and display.

NOTE
The sample code is written for the PC1624 Grablink Base, PC1622 Grablink
Full and the PC1626 Grablink Full XR models. If you want to operate it with a
PC1623 Grablink DualBase model, the MC_Connector MultiCam parameter
has to be changed according to the sample program comments.

GrablinkSnapshotTrigger

This is a simple application demonstrating the MultiCam SNAPSHOT acquisition mode on a
Grablink board. The program performs one frame acquisition and displays it each time a
hardware or software trigger event occurs.

NOTE
The sample code is written for the PC1624 Grablink Base, PC1623 Grablink
DualBase,PC1622 Grablink Full and the PC1626 Grablink Full XRmodels. For
any other Grablink board model, the TrigCtl parameter has to be updated
according to the sample program comments.

4

Windows32-/64-bitC/C++C#VB 6.0VB.NETPythonLinux32-/64-bitPythonWindows32-/64-bitC/C++C#MultiCam MultiCam Sample Programs

grablink_snapshot_trigger

This is a simple SDL based application demonstrating the MultiCam SNAPSHOT acquisition
mode using a Grablink board. The program performs one frame acquisition and displays it each
time a hardware or software trigger event occurs.

NOTE
The sample code is written for the PC1624 Grablink Base, PC1623 Grablink
DualBase,PC1622 Grablink Full and the PC1626 Grablink Full XR models. For
any other Grablink board model, the TrigCtl parameter has to be updated
according to the sample program comments.

GrablinkHfr

This is a simple application demonstrating the MultiCam HFR acquisition mode on a Grablink
board. The program performs continuous image acquisitions and display.

GrablinkHfrTrigger

This is a simple application demonstrating the MultiCam HFR acquisition mode on a Grablink
board. The program performs one frame acquisition and displays it each time a hardware or
software trigger event occurs.

GrablinkWeb

This is a simple application demonstrating the MultiCam WEB acquisition mode on a Grablink
board. The program performs a continuous web acquisition and display.

grablink_web

This is a simple SDL based application demonstrating the MultiCam WEB acquisition mode on a
Grablink board. The program performs continuous web acquisition and display.

GrablinkPageTrigger

This is a simple application demonstrating the MultiCam PAGE acquisition mode on a Grablink
board. The program performs one page acquisition and displays it each time a hardware or
software trigger event occurs.

5

Linux32-/64-bitC/C++SDLWindows32-/64-bitC/C++Windows32-/64-bitC/C++Windows32-/64-bitC/C++Linux32-/64-bitC/C++SDLWindows32-/64-bitC/C++MultiCam MultiCam Sample Programs

GrablinkLongPageTrigger

This is a simple application demonstrating the MultiCam LONGPAGE acquisition mode on a
Grablink board. The program performs one LONGPAGE acquisition and displays it each time a
hardware or software trigger event occurs.

PlanarRGB

This is a simple application showing the planar RGB management. The application performs
image acquisition and display.

GrablinkDualFull

This application demonstrates video acquisition using an AVT Bonito-CL-400B camera
connected to 2 Grablink Full boards.

GrablinkMultiBase

This application demonstrates synchronized line-scan video acquisition with image stitching
from several Camera Link Base line-scan cameras connected to one or several Grablink boards.

GrablinkPipelineController

This application demonstrates the PipelineControllerin PAGE Acquisition Mode on a Grablink
board, with a hardware page trigger connected to the IIN2 input and a motion encoder
connected to the IIN1 input.

GrablinkSerialCommunication

This is a simple application demonstrating Camera Link serial communication through the
clseremc library on a Grablink board.

GrablinkIoPorts

A simple graphical application showing usage of PC1624 Grablink Base, PC1623 Grablink
DualBase, PC1622 Grablink Full and PC1626 Grablink Full XR I/O lines through the MultiCam
Board object.

6

Windows32-/64-bitC/C++Windows32-/64-bitC/C++Windows32-/64-bitC/C++Windows32-/64-bitC/C++C#WindowsC/C++32-/64-bitWindows32-/64-bitC/C++C#Windows32-/64-bitC#MultiCam MultiCam Sample Programs

terminal

This is a console application demonstrating the serial communication with Grablink boards
using the clseremc Camera Link library.

grablink-cuda

This sample illustrates interoperability of a Grablink board, OpenGL and CUDA:

● Acquisition of 8-bit monochrome buffers using a Grablink board

● Basic processing with CUDA (Inverse 8-bit luminance)

● Rendering with OpenGL (Rotate with click and drag)

See README.txt in the sample program folder for more details.

7

Windows32-/64-bitPythonLinux32-/64-bitC/C++PythonWindows64-bitC/C++OpenGLnVidia CUDALinux64-bitC/C++OpenGLnVidia CUDAMultiCam MultiCam Sample Programs

2. Domino Sample programs

SampleprogramsforDominoproducts

DominoSnapshot

This is a simple application demonstrating the MultiCam Snapshotacquisitionmodeusing a
Domino board. The program performs continuous image acquisitions and display.

DominoSnapshotTrigger

This is a simple application demonstrating the MultiCam Snapshotacquisitionmodeusing a
Domino board. Each time a hardware or software trigger event occurs, the program performs
one frame acquisition and displays the image.

domino_snapshot _trigger

This is a simple SDL-based application demonstrating the MultiCam Snapshot acquisition mode
using a Domino board. The program performs one frame acquisition and displays it each time a
hardware or software trigger event occurs.

8

Windows32-/64-bitC/C++C#VB 6.0VB.NETWindows32-/64-bitC/C++C#Linux32-/64-bitC/C++SDLMultiCam MultiCam Sample Programs

3. Picolo Sample Programs

SampleprogramsforPicoloproducts

PicoloVideo

This is a simple application demonstrating the MultiCam Videoacquisitionmodeusing a Picolo
board. The program performs continuous image acquisition and display.

picolo_video

This is a simple SDL based application that acquires and displays live images using the first
Picolo board detected by MultiCam.

PicoloVideoTrigger

This is a simple application demonstrating the MultiCam Videoacquisitionmodeusing a Picolo
board. Each time a hardware or software trigger event occurs, the program performs an image
sequence acquisition and display.

PicoloDirectShow

This is a simple application demonstrating video acquisition and recording using MultiCam
Microsoft DirectShow filters. The program performs continuous image acquisition and display
while images are recorded in an AVI file. In order to compile this sample program, the Microsoft
DirectShow SDK needs to be installed and the strmbase.lib (Microsoft DirectShow library) needs
to be build. Please refer to Microsoft's website and the product documentation for further
information on the Microsoft DirectShow SDK.

The ffdshow codec needs to be installed to perform any image recording. This sample has been
tested with the ffdshow code versions 2033 and 3476.

9

Windows32-/64-bitC/C++C#VB 6.0VB.NETLinux32-/64-bitC/C++SDLWindows32-/64-bitC/C++C#Windows32-bitC/C++MultiCam MultiCam Sample Programs

4. General Sample Programs

SampleprogramsforallMultiCamproducts

MulticamAdvancedWaitEvent

A simple application demonstrating the MultiCam Snapshotacquisitionmodeusing event
driven signaling instead of callback signaling. The program performs a continuous image
acquisition and display.

NOTE
This sample was written for Grablink series but can easily be converted to be
used with any other board series.

MulticamAdvancedWaitSignal

A simple application demonstrating the MultiCam Snapshotacquisitionmodeusing the
McWaitSignal function instead of the callback signaling method. The program performs a
continuous image acquisition and display.

NOTE
This sample was written for Grablink series but can easily be converted to be
used with any other board series.

multicam_advanced_waitsignal

This is a simple SDL based application demonstrating the MultiCam Snapshot acquisition mode
using the McWaitSignal function instead of the callback signaling method. The program
performs a continuous image acquisition and display.

NOTE
This sample was written for Grablink series but can easily be converted to be
used with any other board series.

10

Windows32-/64-bitC/C++C#Windows32-/64-bitC/C++C#Linux32-/64-bitC/C++SDLMultiCam MultiCam Sample Programs

TestIo

A simple console application making usage of I/O lines through the MultiCam Board object.

NOTE
The C# version of this sample program only supports PC1624 Grablink Base,
PC1623 Grablink DualBase, PC1622 Grablink Full and PC1626 Grablink Full
XR but the code can be easily adapted to other boards.

io_test

A simple console application demonstrating the usage of I/O lines with the MultiCam Board
object.

11

Windows32-/64-bitC/C++C#Linux32-/64-bitC/C++
