FUNCTIONAL GUIDE

Grablink

Grablink Functional Guide

PC1622 Grablink Full
PC1623 Grablink DualBase
PC1624 Grablink Base
PC1626 Grablink Full XR

© EURESYS s.a. 2025 - Doc. D411EN-Grablink Functional Guide-6.19.4.4059 built on 2025-12-15

Grablink Grablink Functional Guide

This documentation is provided with Grablink 6.19.4 (doc build 4059).
www.euresys.com

2

Grablink Grablink Functional Guide

Contents

1. About This Document
1.1. Document Scope

2. Camera Link Interface

2.1. Camera Link Configurations
2.2. Enable Signals

Enable Signals Timing Diagrams
Enable Signals Configuration

2.3. Video Data Signals

Bit Assignments

2.4. Upstream Control Lines
2.5. Serial Communication

3. Cameras

3.1. Camera Classes
3.2. Camera Operation Modes
3.3. Camera Configurations
3.4. Camera Tap Properties
TapConfiguration Glossary
Supported Tap Configurations
TapGeometry Glossary
Supported Tap Geometries

3.5. Camera Active Area Properties
3.6. Bayer CFA Color Registration

4. Processing

4.1. Overview
4.2. Image Reconstruction
4.3. Image Cropping
4.4. Image Flipping
4.5. Pixel Data Processing Configurations
Configurations for Monochrome Pixels
Configurations for Bayer CFA Pixels
Configurations for RGB Pixels

4.6. Look-up Table Transformation
4.7. Bayer CFA to RGB Conversion
4.8. White Balance

What Is White Balance?
White Balance Operator
Automatic Calibration Description
Automatic Calibration Requirements
Automatic Calibration Timing
AWB_AREA Settings Description

4.9. Pixel Formatting
4.10. Image Transfer
4.11. Transfer Latency

5. Acquisition

5.1. Grablink Acquisition Modes
5.2. SNAPSHOT Acquisition Mode
5.3. HFR Acquisition Mode
5.4. WEB Acquisition Mode
5.5. PAGE Acquisition Mode
5.6. LONGPAGE Acquisition Mode
5.7. Setting Optimal Page Length

6. Input/Output Ports

3

6
6

7
8
9
11
14
16
16
17
18

19
20
23
25
27
28
29
34
38
40
43

44
45
47
51
53
54
55
57
60
62
65
68
69
70
72
73
75
76
77
79
81

83
84
85
88
91
94
98
101

103

Grablink Grablink Functional Guide

6.1. I/O Ports Overview
6.2. I/O Functions

General-Purpose Inputs
General-Purpose Output
Trigger Input
End Trigger Input
Line Trigger Input
Strobe Output
Isolated I/O SyncBus Driver
Isolated I/O SyncBus Receiver
Event Signaling
Bracket LED Control
6.3. I/O Indices Catalog

7. Triggers

7.1. Hardware Trigger
7.2. Hardware End Trigger

8. Exposure Control

9. Strobe Control

10. Line-Scan Synchronization

10.1. Line Capture Modes
10.2. Line Rate Modes
10.3. Valid Line-Scan Synchronization Settings
10.4. Operating Limits

11. Line Trigger

11.1. Line Trigger Overview
11.2. Source Path Selector
11.3. Rate Divider
11.4. Rate Converter
11.5. Periodic Generator
11.6. Line Trigger Line Selection
11.7. Signal Conditioning
11.8. Quadrature Decoder

12. Advanced Features

12.1. Synchronized Line-scan Acquisition
12.2. Metadata Insertion

Introduction to Metadata Insertion
Metadata Controls
Metadata Content
Metadata Fields
Memory Layouts
1-field 8-bit
1-field 10-bit
1-field 12-bit
1-field 14-bit
1-field 16-bit
1-field 24-bit
1-field 30-bit
1-field 36-bit
1-field 42-bit
1-field 48-bit
2-field 8-bit
2-field 24-bit
3-field 8-bit (TAP1)
3-field 8-bit (TAP10)
3-field 8-bit (LVALRISE)

4

104
109
110
111
112
113
114
115
116
117
118
119
120

126
127
133

140

142

147
148
150
152
153

154
155
156
157
158
159
160
161
162

164
165
169
170
171
173
175
178
179
180
181
182
183
184
185
186
188
190
191
192
194
196
198

Grablink Grablink Functional Guide

3-field 10-bit Packed
3-field 30-bit Packed
3-field 40-bit Packed
12.3. Interleaved Acquisition

Interleaved Area-scan Acquisition Principles
Interleaved Line-scan Acquisition Principles
Reset and Strobe Signals Routing
Interleaved Camera and Illumination Control
Interleaved Area-scan Acquisition Channel Setup
Interleaved Line-scan Acquisition Channel Setup
12.4. Two-line Synchronized Line-scan Acquisition

Two-line Camera Cycles
System Architecture
Line Capture Modes
Camera, Illumination and Acquisition controller
SyncBus Wiring
Camfile Template – Take-All-Lines mode
Camfile Template – Tag-A-Line mode
Camfile Customization
Basler spL4096-70kc Camfile for Tag-A-Line mode

12.5. Machine Pipeline Control
Pipe-lined Machine Description
Pipeline Controller Description

12.6. 8-tap/10-bit Acquisition

Introduction to 8-tap/10-bit Acquisition
8-tap/10-bit MultiCam Tap Configurations
10-bit, 30-bit and 40-bit MultiCam Packed Pixel Formats

12.7. Video Lines Reordering

13. Annex

13.1. Interfacing Camera Link Cameras

200
202
204
206
207
209
211
212
218
223
228
229
231
233
235
237
238
241
244
246
249
250
251
255
256
258
258
259

261
262

5

Grablink Grablink Functional Guide

1. About This Document

1.1. Document Scope

6

1.1. Document Scope

This document describes the functions of all the products of the Grablink series together with
their related products.

Grablink main products

Product

PC1622 Grablink Full

PC1623 Grablink DualBase

PC1624 Grablink Base

PC1626 Grablink Full XR

Grablink accessories

Product

S/N Prefix

Icon

FM1

GDB

GBA

FXR

S/N Prefix

Icon

PC1625 DB25F I/O Adapter Cable

DBC

PC3304 HD26F I/O Adapter Cable

PC3305 C2C SyncBus Cable

PC3306 C2C Quad SyncBus Cable

NOTE
The S/N prefix is a 3-letter string at the beginning of the card serial number.

NOTE
Icons are used in this document for tagging titles of card-specific content.

6

FullDualBaseBaseFullXR1625330433053306Grablink Grablink Functional Guide

2. Camera Link Interface

2.1. Camera Link Configurations

2.2. Enable Signals

2.3. Video Data Signals

2.4. Upstream Control Lines

2.5. Serial Communication

8

9

16

17

18

7

Grablink Grablink Functional Guide

2.1. Camera Link Configurations

Camera Link 2.1 Configurations

Configuration

Channel Link
count (names)

Connectors
count

Bit count

Ports
count(names)

Lite

Base

Medium

Full

72-bit

80-bit

1 (X)

1 (X)

2 (X, Y)

3 (X, Y, Z)

3 (X, Y, Z)

3 (X, Y, Z)

1

1

2

2

2

2

10

24

48

64

72

80

2 (A, B)*

3 (A, B, C)

6 (A to F)

8 (A to H)

9 (A to I)

10 (A to J)

NOTE
(*) Up to 10 bits only

Supported Camera Link Configurations vs. Grablink Product

Configuration

Lite

Base

Medium

Full

72-bit

80-bit

OK

OK

-

-

-

-

OK

OK

-

-

-

-

-

OK

OK

OK

OK

OK

-

OK

OK

OK

OK

OK

8

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

2.2. Enable Signals

The Camera Link standard defines 4 enable signals:

● FVAL (Frame Valid) is defined HIGH for valid lines.

● LVAL (Line Valid) is defined HIGH for valid pixels.

● DVAL (Data Valid) is defined HIGH when data is valid.

● Spare has been defined for future use.

The Camera Link standard requires that enable signals are provided on Channel Link chips as
follows:

Camera Link
Configuration

Lite

Base

FVAL

LVAL

DVAL

Spare

X

X

X

X

-

X

-

X

Medium

X and Y

X and Y

X and Y

X and Y

Full

72-bit

80-bit

X, Y and Z X, Y and Z X, Y and Z X, Y and Z

X, Y and Z X, Y and Z X, Y and Z X, Y and Z

X only

X, Y and Z

-

-

Grablink Usage

On Grablink boards, the enable signals are used differently according to the type of acquisition:

Area-scan Image Acquisition

● The rising edge of the FVAL signal is used as a "Start-of-Frame"

● The rising edge of the LVAL signal is used as a "Start-of-Line"

● The DVAL signal can optionally be used as a clock qualifier

● The Spare signal is unused

Line-scan Image Acquisition

● The FVAL signal is unused

● The rising edge of the LVAL signal is used as a "Start-of-Line"

● The DVAL signal can optionally be used as a clock qualifier

● The Spare signal is unused

Raw Data Acquisition

● The FVAL signal is used as a "Frame cover" signal surrounding the data to acquire

● The LVAL signal is unused

9

Grablink Grablink Functional Guide

● The DVAL signal can optionally be used as a clock qualifier

● The Spare signal is unused

10

Grablink Grablink Functional Guide

Enable Signals Timing Diagrams

FVAL Timing for Area-Scan Acquisition

When the Grablink board is configured for image acquisition from area-scan cameras:

● The rising edge of FVAL is used as a start of frame signal.

● The falling edge of FVAL is ignored.

FVAL timing diagram (area-scan acquisition)

If the acquisition trigger is armed when FVAL raises, the board:

1. Skips a pre-defined amount (VsyncAft_Ln) of video data lines at begin of frame (BOF),

2. Acquires a pre-defined amount (Vactive_Ln) of video data lines,

3. Terminates the image acquisition

The subsequent video data lines are all skipped until a new FVAL rising edge occurs.

Applicable limits on FVAL timing for Area-Scan Acquisition

Parameter

Min

Typ

Max Unit

FVAL high duration

FVAL low duration

FVAL rising edge to LVAL rising edge setup time

Skipped lines at begin of frame (BOF)

Skipped lines at end of frame (EOF)

Lines
with
active
video

Lines
with
blanked
video

-

0

-

64

64

0

0

0

N/A

Clock
cycle

N/A

Clock
cycle

N/A

255

N/A

Clock
cycle

LVAL
cycle

LVAL
cycle

Refer to "Camera Active Area Properties" on page 40 for constraints on the value of Vactive_Ln.

11

Grablink Grablink Functional Guide

FVAL Timing for Raw Data Acquisition

When the Grablink board is configured for raw data acquisition:

● The rising edge of FVAL starts the data acquisition.

● The falling edge of FVAL stops the data acquisition.

FVAL timing diagram (area-scan acquisition mode)

If the acquisition trigger is armed when FVAL rises, the board acquires all data until FVAL goes
down.

You can configure the frame grabber to use the DVAL signal as a clock qualifier but this feature
is rarely used. By default, the configuration ignores the DVAL signal. When DVAL is enabled, the
data acquisition is inhibited for clock cycles where DVAL = 0.

Applicable limits on FVAL timing for Raw Data Acquisition

Parameter

Min Typ Max

Unit

FVAL high duration

FVAL low duration

1

1

-

-

N/A Clock cycle

N/A Clock cycle

LVAL Timing Diagram

When the Grablink board is configured for image acquisition from an area-scan camera or from
a line-scan camera:

● The rising edge of LVAL is actually the reference for the horizontal timing

● The falling edge of LVAL is ignored

LVAL timing diagram

If, at the rising edge of LVAL, the acquisition of the next line is enabled, the board

1. Skips a pre-defined amount (HsyncAft_Tk) of camera clock cycles at begin of line (BOL)

12

Grablink Grablink Functional Guide

2. Acquires a pre-defined amount (Hactive_Px) pixel data

3. Terminates the line acquisition

The subsequent clock cycles are skipped until a new LVAL rising edge occurs.

You can configure the frame grabber to use the DVAL signal as a clock qualifier but this feature
is rarely used. Therefore, the default configuration ignores the DVAL signal.

Applicable limits on LVAL timing for Area-Scan and Line-Scan Acquisitions

Parameter

Min

Typ

LVAL high duration

LVAL low duration

Skipped clocks at begin of line
(BOL)

Skipped clocks at end of line
(EOL)

EOL skipped clocks

1

1

0

0

Others:
0

# clocks with active
video

# clocks with blanked
video

0

-

-

Max

N/A

N/A

255

N/A

N/A

Unit

Qualified clock
cycle

Qualified clock
cycle

Qualified clock
cycle

Qualified clock
cycle

Qualified clock
cycle

Refer to "Camera Active Area Properties" on page 40 for constraints on the value of Hactive_Px.

A qualified clock cycle is defined as:

● Any clock cycle when the frame grabber is configured to ignore DVAL.

● A clock cycle with DVAL = 1 when the frame grabber is configured to use DVAL.

When DVAL is enabled:

● The data acquisition is inhibited for clock cycles where DVAL = 0. Only the data

corresponding to DVAL = 1 are effectively acquired.

● The BOL skipped clock counter is not incremented for clock cycles where DVAL = 0. In other
words, HsyncAft_Tk specifies the amount number of "clock cycles with DVAL = 1" to skip at
begin of line.

13

Grablink Grablink Functional Guide

Enable Signals Configuration

The enumerated parameter FvalMode specifies the usage of the FVAL downstream signal;
possible values are:

Value

Meaning

FA

FN

FC

The rising edge of the FVAL signal is used as a Start of Frame; the falling edge is
irrelevant.

The FVAL signal is ignored by the board.

Raw grabbing; start/stop recording on FVAL rising/falling edges

The enumerated parameter LvalMode specifies the usage of the LVAL downstream signal;
possible values are:

Value

Meaning

LA

LN

The rising edge of the LVAL signal is used as a Start of Line; the falling edge
is irrelevant.

The LVAL signal is ignored by the board.

The enumerated parameter DvalMode specifies the usage of the DVAL downstream signal;
possible values are:

Value

Meaning

DG

DN

The DVAL signal is a clock qualifier,only the data transmitted during clock cycles
with DVAL = 1 are captured by the board.

The DVAL signal is ignored by the board.

The MultiCam channel must be configured for the appropriate camera operation modes by
assigning the appropriate values to the FvalMode, LvalMode, and DvalMode parameters.

Configuring Enable Signals for Area-Scan Cameras

The following combinations of values of FvalMode, LvalMode, and DvalMode parameters are
allowed for image acquisition from area-scan cameras (Imaging = AREA):

● FA-LA-DN and FA-LA-DG are the recommended configurations for capturing fixed size images.
The leading (rising) edges of the FVAL and LVAL signals are used as vertical and horizontal
synchronization events. Their trailing (falling) edges are ignored. The DVAL can optionally be
used as a DATA qualifier.

● FC-LA-DN and FC-LA-DG are the recommended configurations for cameras delivering images
having a variable amount of fixed size lines. The FVAL signal is used as a "Frame Cover"
signal that surrounds the lines to be acquired. The leading (rising) edge of the LVAL signal is
used as a horizontal synchronization event. Its trailing edge is ignored. The DVAL can
optionally be used as a DATA qualifier.

14

Grablink Grablink Functional Guide

Configuring Enable Signals for Line-Scan Cameras

The following combinations of values of FvalMode, LvalMode, and DvalMode parameters are
recommended for image acquisition from line-scan (Imaging = LINE) and TDI line-scan cameras
(Imaging = TDI):

● FN-LA-DN and FN-LA-DG are the recommended configurations for capturing fixed size lines.
The FVAL signal is ignored. The leading (rising) edge of the LVAL signals is used as a start-of-
line synchronization events. The trailing (falling) edge of LVAL is ignored. The DVAL can
optionally be used as a DATA qualifier.

Configuring Enable Signals for Raw Data Acquisition

The following combinations of values of FvalMode, LvalMode, and DvalMode parameters are
recommended for raw data acquisition from digital devices:

● FC-LN-DN and FC-LN-DG are the recommended configurations for digital devices delivering
raw data or camera devices delivering irregularly structured images. The FVAL signal is used
as a "Frame Cover" signal that surrounds the data to be acquired. The LVAL signal is ignored.
The DVAL can optionally be used as a DATA qualifier.

15

Grablink Grablink Functional Guide

2.3. Video Data Signals

Bit Assignments

Bit Assignments

Grablink products comply with the following bit assignment tables of section 4 of the Camera
Link 2.1 Specification:

● Table 4-1: 8-Bit Modes, Base/Medium/Full

● Table 4-2: 8-Bit Modes, 80-bit

● Table 4-3: 10-Bit Modes, Base/Medium/Full

● Table 4-4: 10-Bit Modes, 80-bit

● Table 4-5: 12-Bit Modes, Base/Medium/Full

● Table 4-6: 14-Bit Modes, Base/Medium/Full/72-bit

● Table 4-7: 16-Bit Modes, Base/Medium/Full

● Table 4-8: 16-Bit Mode, 80-bit

● Table 4-9: Lite Modes

16

Grablink Grablink Functional Guide

2.4. Upstream Control Lines

According to the Camera Link standard, four LVDS signals are reserved for general-purpose
camera control.

They are defined as camera inputs and frame grabber outputs. Camera manufacturers can
define these signals to meet their needs for a particular product. The signals are:

● Camera Control 1 (CC1)

● Camera Control 2 (CC2)

● Camera Control 3 (CC3)

● Camera Control 4 (CC4)

Each control line can be configured as:

Reset signal

A transition (either rising or falling edge) on this line resets the camera. This action initiates
either a new exposure/readout, or a readout of a frame for an area-scan camera or a line for a
line-scan camera.

Expose control signal

The leading edge (either rising or falling edge) on this line initiates a new exposure.

The trailing edge terminates the exposure and initiates the readout of a frame for an area-scan
camera or a line for a line-scan camera.

The pulse width is actually the exposure time.

GPIO

Usually, CC1 is used for reset or expose signal for asynchronous reset cameras.

This board does not provide HDRIVE, VDRIVE signals. Usually, Camera Link cameras do not need
genlocking.

17

Grablink Grablink Functional Guide

2.5. Serial Communication

This board supports the asynchronous full-duplex serial communication between the camera
and the frame grabber.

Serial Link for the Lite Camera Link Configuration

For cameras using the Lite configuration, the downstream serial communication link doesn't
use a dedicated line pair but, instead, is embedded in the Channel Link.

Serial Link for the Base, Medium, Full, or 80-bit Camera Link Configurations

For cameras using the Base, Medium, Full, or 80-bit configurations, two differential line pairs of
the first camera cable are dedicated to the serial communication, one for each direction:

● SerTFG— Downstream (camera to frame grabber) serial communication link

● SerTC— Upstream (frame grabber to camera) serial communication link

Serial Link Functionalities

The application software controls the serial communication channel through the standardized
API defined by the Camera Link standard.

Alternatively, it can also be controlled using virtual COM ports. Therefore, the application must
set the appropriate values to the parameters SerialControlA and SerialControlB respectively.

Supported baud rates

600, 1200, 1800, 2400, 3600, 4800, 7200, 9600 (Default), 14400, 19200, 28800, 38400, 57600,
115200 and 230400.

18

Grablink Grablink Functional Guide

3. Cameras

3.1. Camera Classes

3.2. Camera Operation Modes

3.3. Camera Configurations

3.4. Camera Tap Properties

3.5. Camera Active Area Properties

3.6. Bayer CFA Color Registration

20

23

25

27

40

43

19

Grablink Grablink Functional Guide

3.1. Camera Classes

In MultiCam, cameras are classified according to four Channel parameters: Imaging, Spectrum,
DataLink, and ColorMethod.

Imaging Parameter

The enumerated parameter Imaging specifies the geometry of the camera sensor; possible
values are:

Value

Description

AREA

LINE

TDI

The camera is an area-scan model. An area-scan camera is based on a 2D imaging
sensor; it delivers individual image frames composed of a fixed amount of lines,
each containing a fixed amount of pixels.

The camera is a line-scan model. A line-scan camera is based on 1D imaging
sensor; it delivers individual image lines composed of a fixed amount of pixels.

The camera is a TDI line-scan model. A particular type of line-scan camera using
the Time Delayed Integration technology (TDI). It is based on a 2D imaging sensor
having a small amount of lines; like any other line-scan camera, it delivers
individual image lines composed of a fixed amount of pixels.

NOTE
The board's behavior is the same when Imaging is TDI instead of LINE;
however TDI cameras exhibit less operational modes since they lack an
electronic shutter.

Spectrum Parameter

The enumerated parameter Spectrum specifies the spectral sensitivity of the camera; possible
values are:

Value

Description

BW

IR

The camera delivers a monochrome image obtained from an image sensor
operating in the human visible domain of the light spectrum.

The camera delivers a monochrome image obtained from an image sensor
operating in the infra-red domain of the light spectrum.

COLOR

The camera delivers a color image.

NOTE
The board's behavior is exactly the same when Spectrum is IR instead of
BW.

20

Grablink Grablink Functional Guide

DataLink Parameter

The enumerated parameter DataLink specifies the data transfer method used by the camera;
the unique possible value is:

Value

Description

CAMERALINK

The camera delivers a digital video signal complying to the Camera Link
standard.

ColorMethod Parameter

The enumerated parameter ColorMethod specifies the color analysis method used by the
camera; possible values are:

Value

NONE

BAYER

PRISM

RGB

Description

The camera delivers a monochrome image.

The camera uses a single imaging sensor coated with a Bayer Color Filter
Array and delivers the raw Bayer CFA data as a single video data stream
embedding the RGB information.

The camera uses a wavelength-separating prism to feed three distinct
imaging sensors. The color information is available as three R, G, B video data
streams.

The camera uses a coated sensor and an internal processor to reconstruct the
full color information. The color information is available as three R, G, B video
data streams.

TRILINEAR

The camera uses three parallel sensing linear arrays of pixels exhibiting
different wavelength sensitivities. The color information is available as three
R, G, B video data streams.

Notes

□ This board provides a limited support of TRILINEAR cameras since the scan-delay

compensation function is not available.

□ The board's behavior is the same when ColorMethod is TRILINEAR instead of RGB.
□ The board's behavior is exactly the same when ColorMethod is PRISM instead of RGB.

21

Grablink Grablink Functional Guide

Supported Camera Classes

Grablink products support the following combinations of values:

Imaging

Spectrum

ColorMethod

Camera Class

BW

IR

AREA

NONE

NONE

BAYER

Monochrome area-scan cameras – Visible
spectrum

Monochrome area-scan cameras – Infra-red
spectrum

Color area-scan cameras –Raw BAYER data
output

COLOR

RGB

Color area-scan cameras – RGB data output

BW

IR

COLOR

PRISM

NONE

NONE

BAYER

RGB

PRISM

3-CCD Color area-scan cameras – RGB data
output

Monochrome line-scan cameras – Visible
spectrum

Monochrome line-scan cameras – Infra-red
spectrum

Color line-scan cameras –Raw BAYER data
output

Color line-scan cameras – RGB data output

3-CCD Color line-scan cameras – RGB data
output

TRILINEAR

Color line-scan cameras – 1 tri-linear sensor –
RGB data output

BW

IR

NONE

NONE

COLOR

PRISM

Monochrome Time-Delay-Integration line-scan
cameras – Visible spectrum

Monochrome Time-Delay-Integration line-scan
cameras – Infrared spectrum

3-CCD Color Time-Delay-Integration line-scan
cameras – 3 sensors with filters – RGB data
output

LINE

TDI

22

Grablink Grablink Functional Guide

3.2. Camera Operation Modes

The classification of the operation modes of the industrial cameras is based on the following
MultiCam parameters: Expose and Readout.

Expose

The enumerated parameter Expose specifies the camera exposure principle used by the camera;
possible values are:

Value

Description

INTCTL

PLSTRG

WIDTH

The line or frame exposure condition is totally controlled by the camera. The
exposure duration is set through camera configuration settings.

The line or frame exposure condition starts upon receiving a pulse from the
frame grabber.

The duration of a pulse issued by the frame grabber determines the line or
frame exposure condition.

INTPRM

The exposure is permanent.

Readout

The enumerated parameter Readout specifies the camera readout principle used by the camera;
possible values are

Value

Description

INTCTL

The frame read-out condition is totally controlled by the camera. The read-out
duration is set through camera configuration settings.

Supported Area-Scan Camera Operation Modes

Grablink products support the following combinations of values for area-scan cameras (Imaging
= AREA):

CamConfig

Expose

Readout

Description

PxxSC

INTCTL

INTCTL

Synchronous progressive scan, camera-controlled
exposure

PxxRC

PLSTRG

INTCTL

PxxRG

WIDTH

INTCTL

Asynchronous progressive scan, camera-
controlled exposure

Asynchronous progressive scan, grabber-
controlled exposure

Supported Line-Scan Camera Operation Modes

Grablink products support the following combinations of values for (non-TDI) line-scan cameras
(Imaging = LINE):

23

Grablink Grablink Functional Guide

CamConfig

Expose

Readout

Description

LxxxxSP

INTPRM

INTCTL

LxxxxSC

INTCTL

INTCTL

LxxxxRP

INTPRM

PLSTRG

LxxxxRC

PLSTRG

INTCTL

LxxxxRG

WIDTH

INTCTL

LxxxxRG2

PLSTRG

PLSTRG

Free-running or camera-controlled line rate
Permanent exposure or disabled electronic
shutter
No control line

Free-running or camera-controlled line rate
Camera-controlled exposure with electronic
shutter
Single control line

Grabber-controlled camera line rate and exposure
Permanent exposure or disabled electronic
shutter
Single control line

Grabber-controlled camera line rate
camera controlled exposure
Single control line

Grabber-controlled camera rate and exposure
Camera-controlled exposure with electronic
shutter
Single control line

Grabber-controlled camera rate and exposure
Camera-controlled exposure with electronic
shutter
Two control lines

Supported TDI Line-Scan Camera Operation Modes

Grablink products support the following combinations of values for TDI line-scan cameras
(Imaging = TDI):

CamConfig

Expose

Readout

Description

LxxxxSP

INTPRM

INTCTL

Free-running, permanent exposure

LxxxxRP

INTPRM

PLSTRG

Grabber-controlled line-scanning, permanent
exposure

24

Grablink Grablink Functional Guide

3.3. Camera Configurations

MultiCam embeds predefined camera parameters settings for each available camera operation
mode.

The user selects the by assigning a value to the Camera and CamConfig parameters. For all
Grablink products:

● Camera must be set to MyCameraLink

● CamConfig must be set to any of the following value :

CamConfig

Camera
Class

PxxSC

Area-scan

PxxRC

Area-scan

PxxRG

Area-scan

LxxxxSC

Line-scan

LxxxxRC

Line-scan

LxxxxRG

Line-scan

LxxxxSP

(TDI) Line-
scan

LxxxxRP

(TDI) Line-
scan

Camera Operation Mode

CamFile template

Synchronous progressive scan
Camera-controlled exposure

Asynchronous progressive scan
Camera-controlled exposure

Asynchronous progressive scan
Grabber-controlled exposure

MyCameraLink_
PxxSC.cam

MyCameraLink_
PxxRC.cam

MyCameraLink_
PxxRG.cam

Free-running or camera-controlled line
rate
Camera-controlled exposure with
electronic shutter
No control line

N/A

Grabber-controlled camera line rate
Camera controlled exposure
Single control line

Grabber-controlled camera rate and
exposure
Camera-controlled exposure with
electronic shutter
Single control line

Free-running or camera-controlled line
rate
Permanent exposure or disabled
electronic shutter
No control line

Grabber-controlled camera line rate
and exposure
Permanent exposure or disabled
electronic shutter
Single control line

MyCameraLink_
LxxxxRC.cam

MyCameraLink_
LxxxxRG.cam

MyCameraLink_
LxxxxSP.cam

MyCameraLink_
LxxxxRP.cam

CamFile templates are delivered for most of the camera operation modes.

25

Grablink Grablink Functional Guide

NOTE
A CamFile is a text file gathering all the relevant Channel parameters.

26

Grablink Grablink Functional Guide

3.4. Camera Tap Properties

TapConfiguration Parameter

The enumerated parameter TapConfiguration declares the Camera Link tap configuration used
by the camera.

Refer to "Supported Tap Configurations" on page 29 for an exhaustive list of configurations
supported by Grablink products.

TapGeometry Parameter

The tap geometry is a Euresys proprietary taxonomy that describes, with a standardized name,
the geometrical properties characterizing the different taps of a multi-tap camera.

The enumerated parameter TapGeometry declares the Camera Link tap geometry used by the
camera.

Refer to "Supported Tap Geometries" on page 38 for an exhaustive list of valid combinations of
TapConfiguration and TapGeometry values.

Image Reconstruction

For most TapConfiguration and TapGeometry values, the frame grabber is capable to re-arrange
the data in the destination surface.

Refer to "Image Reconstruction" on page 47 for more information.

27

Grablink Grablink Functional Guide

TapConfiguration Glossary

Naming Convention

A tap configuration is designated by:

<Config>_<TapCount>T<BitDepth>(B<TimeSlots>)

<Config>

Designates the Camera Link configuration as follows:

Camera Link Configuration name

<Config> value

Lite

Base

Medium

Full

72-bit

80-bit

<TapCount>

LITE

BASE

MEDIUM

FULL

DECA

DECA

Total number of pixel taps. Values range: 1 to 10.

<BitDepth>

Number of bits per tap. Values list: {8, 10, 12, 14, 16, 24, 30, 36, 42, 48}.

<TimeSlots>

Number of consecutive time slots required to transfer one pixel data. Values list: {2, 3}

The field and the letter B are omitted when a single time slot is sufficient to deliver all the pixel
data.

Examples

BASE_1T8: Base Camera Link configuration, 1 tap, 8-bit pixel data

BASE_1T24: Base Camera Link configuration, 1 tap, 24-bit pixel data (likely RGB)

DECA_8T10: 80-bit Camera Link configuration, 8 taps, 10-bit pixel data

DECA_8T30B3: 80-bit Camera Link configuration, 8 taps, 30-bit pixel data (likely RGB), 3 time
slots

28

Grablink Grablink Functional Guide

Supported Tap Configurations

This topic lists all the Camera Link tap configurations (a.k.a. modes) defined in the section 4 of
version 2.1 of the Camera Link standard.

The tap configurations are grouped by bit-depth then pixel type. Within a table, entries are
sorted by increasing number of taps.

NOTE
Tap Configuration is a Euresys proprietary taxonomy that integrates, the
channel link configuration, the number of taps and the pixel bit depth.

For each entry, it specifies:

1. CL2.1 Name: The name of the configuration as written in the section 4 of version 2.1 of the

Camera Link standard

2. Euresys Name:The Euresys name of the configuration, i.e. the value of the TapConfiguration

parameter.

3. Compatible products: The list of Grablink products supporting that configuration. An empty

cell indicates that the configuration is not supported.

29

Grablink Grablink Functional Guide

8-bit Tap Configurations

Monochrome 8-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Lite

LITE_1T8

Base 1 tap

BASE_1T8

Base 2 taps

BASE_2T8

Base 3 taps

BASE_3T8

Medium 4 taps

MEDIUM_4T8

Medium 5 taps

-

Medium 6 taps

MEDIUM_6T8

Full 7 taps

Full 8 taps

-

FULL_8T8

Full 9 taps

DECA_9T8

80-bit 10 taps

DECA_10T8

-

-

RGB 8-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Base 1 tap

BASE_1T24

Medium 2 taps

MEDIUM_2T24

Full 3 taps

DECA_3T24

80-bit 10 taps

-

-

RGBI 8-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

Full 2 taps

-

-

-

-

30

BaseDualBaseExpressEOSBaseDualBaseFullFullXRExpressEOSBaseDualBaseFullFullXRBaseDualBaseFullFullXRFullFullXRFullFullXRFullFullXRFullFullXRFullFullXRExpressEOSBaseDualBaseFullFullXRFullFullXRFullFullXRGrablink Grablink Functional Guide

10-bit Tap Configurations

Monochrome 10-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Lite

LITE_1T10

Base 1 tap

BASE_1T10

Base 2 taps

BASE_2T10

Medium 3 taps

MEDIUM_3T10

Medium 4 taps

MEDIUM_4T10

Full 5 taps

Full 6 taps

80-bit 7 taps

-

-

-

-

-

-

80-bit 8 taps

DECA_8T10

RGB 10-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

MEDIUM_1T30

Full 2 taps

-

-

80-bit 8 taps (3 slots)

DECA_8T30B3

RGBI 10-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

-

-

Full 2 taps

DECA_2T40

31

BaseDualBaseExpressEOSBaseDualBaseFullFullXRExpressEOSBaseDualBaseFullFullXRFullFullXRFullFullXRFullFullXRFullFullXRFullFullXRFullFullXRGrablink Grablink Functional Guide

12-bit Tap Configurations

Monochrome 12-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Base 1 tap

BASE_1T12

Base 2 taps

BASE_2T12

Medium 3 taps

MEDIUM_3T12

Medium 4 taps

MEDIUM_4T12

Full 5 taps

Full 6 taps

-

-

-

-

RGB 12-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

MEDIUM_1T36

Full 2 taps

-

-

RGBI 12-bit Tap Configuration

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

-

-

14-bit Tap Configurations

Monochrome 14-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Base 1 tap

BASE_1T14

Medium 2 taps

MEDIUM_2T14

Medium 3 taps

MEDIUM_3T14

Full 4 taps

72-bit 5 taps

-

-

-

-

32

ExpressEOSBaseDualBaseFullFullXRExpressEOSBaseDualBaseFullFullXRFullFullXRFullFullXRFullFullXRExpressEOSBaseDualBaseFullFullXRFullFullXRFullFullXRGrablink Grablink Functional Guide

RGB 14-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

MEDIUM_1T42

RGBI 14-bit Tap Configuration

CL2.1 Name

Euresys Name

Compatible products

Full 1 tap

-

-

16-bit Tap Configurations

Monochrome 16-bit Tap Configurations

CL2.1 Name

Euresys Name

Compatible products

Base 1 tap

BASE_1T16

Medium 2 taps

MEDIUM_2T16

Medium 3 taps

MEDIUM_3T16

Full 4 taps

72-bit 5 taps

-

-

-

-

RGB 16-bit Tap Configuration

CL2.1 Name

Euresys Name

Compatible products

Medium 1 tap

MEDIUM_1T48

RGBI 16-bit Tap Configuration

CL2.1 Name

Euresys Name

Compatible products

Full 1 tap

-

-

33

FullFullXRExpressEOSBaseDualBaseFullFullXRFullFullXRFullFullXRFullFullXRGrablink Grablink Functional Guide

TapGeometry Glossary

Definitions

Adjacent taps

Two taps are adjacent when the extracted pixels are adjacent on the same row or on the same
column.

Region

A rectangular area of adjacent pixels that are transferred in a raster-scan order through one or
multiple adjacent taps.

Tap

One pixel stream output port of the camera that delivers one pixel every clock cycle.

Tap Geometrical Properties

A tap is characterized by the following properties:

XStart: X-position of the first extracted pixel of a camera readout cycle

XEnd: X-position of the last extracted pixel of a camera readout cycle

YStart: Y-position of the first extracted pixel of a camera readout cycle

YEnd: Y-position of the last extracted pixel of a camera readout cycle

YStep: the difference of Y-position between consecutive rows of pixels; it is positive when Y-
position values are increasing (top to bottom); it is negative otherwise.

X-Position: the pixel column number in the (non-flipped) image; column 1 is the leftmost
column; column W is the rightmost column of an image having a width of W pixels.

Y-Position: the pixel row number in the (non-flipped) image; row 1 is the topmost row; row H is
the bottommost row of an image having a height of H pixels.

TapGeometry Values Syntax

There are two variants of the syntax:

1. For cameras delivering two or more rows of pixels every camera readout cycle:

<TapGeometryX>_<TapGeometryY>

2. For cameras delivering only one row of pixels every camera, e.g. single line line-scan

cameras:

<TapGeometryX>

34

Grablink Grablink Functional Guide

TapGeometryX Syntax

<TapGeometryX> describes the geometrical organization of the taps along one row of the image.
It is built as follows:

<XRegions>X(<XTaps>)(<ExtX)>)

● <XRegions>: an integer declaring the number of regions encountered across one image row
(= the X-direction or the horizontal direction). Possible values are 1, 2, 3, 4, 6, 8, and 10.

● <XTaps>: an integer declaring the number of consecutive pixels along one region row that

are extracted simultaneously.
Possible values are 1, 2, 3, 4, 8, and 10.
The field is omitted when <XTaps> is 1.

● <ExtX>: a letter declaring the relative location of the pixels extractors across one row of the

image.
□ This field is omitted when all pixel extractors are at the left of each region.
□ Letter E indicates that pixel extractors are at both ends of the image row.
□ Letter M indicates that pixel extractors are at middle of the image row.
□ Letter R indicates that the pixel extractors are all at the right of each region

TapGeometryY Syntax

<TapGeometryY> describes the geometrical organization of the taps along one column of the
image. It is built as follows:

<YRegions>Y(<YTaps>)(<ExtY)>)

<YRegions>: an integer declaring the number of regions encountered across vertical direction.
Possible values are 1 and 2.

<YTaps>: an integer declaring the number of consecutive pixels along one region column that
are extracted simultaneously.
Possible values are 1 and 2. The field is omitted when YTaps is 1.

<ExtY>: a letter declaring the relative location of the pixels extractors across one column of the
image.

□ This field is omitted when all pixel extractors are at the top of each region.
□ Letter E indicates that pixel extractors are at both ends of the image column.

35

Grablink Grablink Functional Guide

TapGeometry Values Examples

1X_1Y designates the tap geometry of a single-tap camera having 1 region across the X-direction
and 1 region across the Y direction.

The pixels are delivered one at a time on a single tap beginning with the leftmost pixel of the
top row, scanning progressively all the rows of the image one by one, and ending with the
rightmost pixel of the bottom row.

1X2_1Y designates the tap geometry of a two-tap camera having 1 region across the X-direction
and 1 region across the Y direction.

The pixels are delivered two at a time on two taps beginning with the two leftmost pixels of
the top row, scanning progressively all the rows of the image one by one, and ending with
the two rightmost pixels of the bottom row.

1X_1Y2 designates the tap geometry of a two-tap camera having 1 region across the X-direction
and 1 region across the Y direction.

The pixels are delivered two at a time on two taps beginning with the two uppermost pixels
of the first column , scanning progressively all the rows of the image two by two, and ending
with the two lowermost pixels of the rightmost column.

2XE_2YE designates the tap geometry of a four-tap camera having 2 regions across the X-
direction and 2 regions across the Y direction.

The pixels are delivered four at a time on four taps. Each region delivers its pixels on a
single-tap using a specific scanning scheme:

The pixels of the upper left quadrant are delivered on tap 1 starting with the upper left
pixel and ending with the lower right pixel of the region.

The pixels of the upper right quadrant are delivered on tap 2 starting with the upper
rightmost pixel and ending with the lower left pixel of the region.

The pixels of the lower left quadrant are delivered on tap 3 starting with the lower left
pixel and ending with the upper right pixel of the region.

The pixels of the lower right quadrant are delivered on tap 4 starting with the lower
rightmost pixel and ending with the upper left pixel of the region.

1X4 designates the tap geometry of a four-tap line-scan camera having 1 region across the X-
direction.

The pixels are delivered four at a time on four taps beginning with the four leftmost pixels
and ending with the four rightmost pixels.

36

121X2_1Y23411X423414X121X_1Y212XE_2YE32411X_1YGrablink Grablink Functional Guide

4X designates the tap geometry of a four-tap line-scan camera having 4 regions across the X-
direction.

The pixels are delivered four at a time on four taps. Each region delivers its pixels on a
single-tap using a common scanning scheme beginning with the leftmost pixel and ending
with the rightmost pixel.

37

Grablink Grablink Functional Guide

Supported Tap Geometries

Tap Geometries for Line-scan Cameras

Number of taps 1 X-region 2 X-regions 3 X-regions 4 X-regions 8 X-regions 10 X-regions

1

2

3

4

8

10

1X

1X2

1X3

1X4

1X8

1X10

-

2X
2XE
2XM
2XR

-

2X2
2X2E
2X2M

2X4

-

-

-

3X

-

-

-

-

-

-

4X
4XE
4XR

4X2
4X2E

-

-

-

-

-

8X
8XR

-

-

-

-

-

-

10X

Tap Geometries for Area-scan Cameras having only one tap along the vertical
direction

Number of
taps

1 X-
region

2 X-
regions

3 X-
regions

4 X-
regions

8 X-
regions

10 X-
regions

1

2

3

4

8

1X_1Y

-

1X2_1Y

2X_1Y
2XE_1Y
2XM_1Y
2XR_1Y

-

-

1X3_1Y

-

3X_1Y

1X4_1Y

2X2_1Y
2X2E_1Y
2X2M_1Y

1X8_1Y

2X4_1Y

-

-

-

-

-

-

4X_1Y
4XE_1Y
4XR_1Y

-

-

-

-

4X2_1Y
4X2E_1Y

8X_1Y
8XR_1Y

-

-

-

-

-

-

-

10X_1Y

10

1X10_1Y

-

Tap Geometries for Area-scan Cameras having Two taps along the vertical
direction

Number of taps 1 X-region 2 X-regions 3 X-regions 4 X-regions 8 X-regions 10 X-regions

2

4

1X_1Y2
1X_2YE

-

-

2XE_2YE

-

-

-

-

-

-

-

-

38

Grablink Grablink Functional Guide

NOTE
Refer to TapGeometry in the Parameters Reference for a description of each
geometry.

39

Grablink Grablink Functional Guide

3.5. Camera Active Area Properties

The Camera Active Area is a rectangular array of pixels containing active video that are
delivered by the camera to the frame grabber.

NOTE
For line-scan cameras, the height of the active area is 1 (or 2 for bilinear line-
scan).

Hactive_Px Parameter

For all cameras, the MultiCam parameter Hactive_Px represents the number of pixels in each
line of the Camera Active Area. The following rules apply:

Rule #1

The width of the Camera Active Area may contain at most 65535 pixels:

Rule #2

Each tap delivers the same amount of pixels every line, Hactive_Px must be a multiple of XTaps:

N is an integer number since each tap delivers the same amount of pixels every line:

XTaps is the number of taps along the X direction. It can be obtained from the value of
TapGeometry by multiplying together the two numbers surrounding the letter "X". For
example, 1X2, 1X2_1Y, 1X2_1Y2, 2X, 2X_1Y have all 2 taps along the X direction.

Rule #3

Each XRegion must contain at least MinBytesPerRegionLine bytes:

MinBytesPerRegionLine = 48

XRegions is the number of geometrical regions in the X direction. This is the number
preceding the letter "X"in the TapGeometry value: For example: 2X, 2X_1Y, 2X2 have all 2
regions along the X direction.

BytesPerPixel is the amount of bytes required to store one pixel into the on-board memory:

1 byte for 8-bit monochrome and Bayer CFA cameras

40

Grablink Grablink Functional Guide

2 bytes for 10-/12-/14- and 16-bit monochrome and Bayer CFA cameras

3 bytes for 24-bit RGB cameras

6 bytes for 30-/36-/42-bit and 48-bit RGB cameras

TapConfiguration

TapGeometry Min. value Multiple of Max. value

BASE_1T8

BASE_1T10, BASE_1T12
BASE_1T14, BASE_1T16

BASE_1T24

MEDIUM_1T30, MEDIUM_1T36
MEDIUM_1T42, MEDIUM_1T48

BASE_2T8

BASE_2T10, BASE_2T12
MEDIUM_2T14, MEDIUM_2T16

MEDIUM_2T24

BASE_3T8

MEDIUM_3T10, MEDIUM_3T12
MEDIUM_3T14, MEDIUM_3T16

MEDIUM_4T8

MEDIUM_4T10, MEDIUM_4T12

FULL_8T8

DECA_10T8

Vactive_Ln Parameter

1X

1X

1X

1X

1X2

2X

1X2

2X

1X2

2X

1X3

3X

1X3

3X

1X4

4X

1X4

4X

1X8

8X

1X10

10X

48

24

16

8

48

96

24

48

16

32

48

144

24

72

48

192

24

96

48

384

50

480

1

1

1

1

2

2

2

2

2

2

3

3

3

3

4

4

4

4

8

8

10

10

65535

65535

65535

65535

65534

65534

65534

65534

65534

65534

65535

65535

65535

65535

65532

65532

65532

65532

65528

65528

65530

65530

For area-scan cameras only, the MultiCam parameter Vactive_Ln represents the number of lines
of the Camera Active Area.

The following rules apply:

Rule #1: The camera active window may contain at most 65535 lines.

Rule #2: Each tap delivers exactly the same amount of pixels, Vactive_Ln must be a multiple of
YTaps.

41

Grablink Grablink Functional Guide

N is an integer number

YTaps is the number of taps along the Y direction. YTaps can be obtained from the value of
TapGeometry by multiplying together the two numbers surrounding the letter "Y". For
example: 1X_1Y2, 1X_2YE have all 2 taps along the Y direction.

42

Grablink Grablink Functional Guide

3.6. Bayer CFA Color Registration

When ColorMethod = BAYER, the enumerated parameter ColorRegistration specifies the
alignment of the color pattern filter over the sensor active area.

Possible values are: GB, BG, RG, GR. The two letters indicate respectively the color of the two
first pixels of the first line.

Value

Description

GB

BG

RG

GR

The first two pixels are green and blue

The first two pixels are blue and green

The first two pixels are red and green

The first two pixels are green and blue

The information is used by MultiCam to automatically configure the Bayer CFA decoder.

43

Grablink Grablink Functional Guide

4. Processing

4.1. Overview

4.2. Image Reconstruction

4.3. Image Cropping

4.4. Image Flipping

4.5. Pixel Data Processing Configurations

4.6. Look-up Table Transformation

4.7. Bayer CFA to RGB Conversion

4.8. White Balance

4.9. Pixel Formatting

4.10. Image Transfer

4.11. Transfer Latency

45

47

51

53

54

62

65

68

77

79

81

44

Grablink Grablink Functional Guide

4.1. Overview

OverviewoftheimagedataprocessingonGrablinkcards

The acquisition channels of Grablink boards performs the following successive operations on
the image data stream:

Image Reconstruction

This operation unscrambles the pixel streams of multi-tap cameras and reconstructs the image
exactly like it was captured on the camera sensor

For more information, refer to "Image Reconstruction" on page 47.

Image Cropping

This operation extracts a rectangular area from the Camera Active Area.

For more information, refer to "Image Cropping" on page 51.

Image Flipping

This operation flips the image around an horizontal and/or a vertical axis.

For more information, refer to "Image Flipping" on page 53.

Lookup Table Transformation

Applies to:

This operation performs lookup table processing on individual pixel components.

For more information and configuration instructions, refer to "Look-up Table Transformation"
on page 62.

Bayer CFA decoding

Applies to:

This operation transforms the raw Bayer CFA data stream issued by the camera into an RGB
color data stream.

For more information and configuration instructions, refer to "Bayer CFA to RGB Conversion" on
page 65.

White Balancing

Applies to:

This operation adjusts the gain and the offset of each color channel.

For more information, refer to "White Balance Operator" on page 70.

45

BaseDualBaseFullFullXRBaseDualBaseFullFullXRBaseDualBaseFullFullXRGrablink Grablink Functional Guide

Pixel Formatting

This stage performs several operations:

● unpacking of 10-bit, 12-bit, and 14-bit pixel components to 8-bit or 16-bit.

● delivery of RGB data in packed or planar formats

For more information and configuration instructions, refer to"Pixel Formatting" on page 77 .

Image line build-up

This operation builds concatenates the components data of all pixels of an image line:

● 8-bit pixel components are aligned to byte boundaries

● 16-bit pixel components (possibly expanded by unpacking or lookup table processing) are
aligned to word (2-byte) boundaries, the 2 bytes are stored according to the little-endian
convention.

Image Transfer

The processed and formatted image data are transferred into a MultiCam Surface over the PCI
Express bus using a DMA engine.

For more information, refer to "Image Transfer" on page 79.

Transfer Latencies

Image data are transferred ASAP to the MultiCam Surface, keeping the time latency as short as
possible.

For more information, refer to"Transfer Latency" on page 81.

46

Grablink Grablink Functional Guide

4.2. Image Reconstruction

Grablink boards unscramble the pixel streams of multi-tap cameras and reconstruct the image
exactly like it was captured on the camera sensor for most of the tap configurations and
geometries.

The following tables list, for each tap configuration, the tap geometries that allows the
reconstruction of the image.

□ Refer to TapGeometry in the Parameters Reference for a description of each geometry.

Tap Geometries for Lite Camera Link Configuration

Applies to:

Tap Configuration

Line-scan

Area-scan,1 YTap

Area-scan, 2 YTap

LITE_1T8
LITE_1T10

1X

1X_1Y

-

47

BaseDualBaseGrablink Grablink Functional Guide

Tap Geometries for Base Camera Link Configuration

Applies to:

Tap Configuration

Line-scan

Area-scan, 1 YTap

Area-scan, 2 YTap

BASE_1T8
BASE_1T10
BASE_1T12
BASE_1T14
BASE_1T16
BASE_1T24

BASE_2T8
BASE_2T10
BASE_2T12

Applies to:

Tap
Configuration

BASE_1T8
BASE_1T10
BASE_1T12
BASE_1T14
BASE_1T16
BASE_1T24

BASE_2T8
BASE_2T10
BASE_2T12

BASE_3T8

1X

1X_1Y

-

1X2
2X 2XE 2XM
2XR

1X2_1Y
2X_1Y 2XE_1Y 2XM_1Y 2XR_1Y

1X_1Y2
1X_2YE

Line-scan

Bilinear
line-
scan

Area-scan, 1 YTap

Area-scan, 2
YTap

1X

-

1X_1Y

-

1X2
2X 2XE 2XM
2XR

1X_1Y2

1X2_1Y
2X_1Y 2XE_1Y 2XM_1Y 2XR_
1Y

1X3 3X

-

1X3_1Y 3X_1Y

1X_1Y2
1X_2YE

-

48

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Tap Geometries for Medium Camera Link Configuration

Applies to:

Tap
Configuration

MEDIUM_1T30
MEDIUM_1T36
MEDIUM_1T42
MEDIUM_1T48

MEDIUM_2T14
MEDIUM_2T16
MEDIUM_2T24

MEDIUM_3T10
MEDIUM_3T12
MEDIUM_3T14
MEDIUM_3T16

MEDIUM_4T8
MEDIUM_4T10
MEDIUM_4T12

Line-scan

Bilinear
line-
scan

Area-scan, 1 YTap

Area-scan, 2 YTap

1X

-

1X_1Y

-

1X2
2X 2XE
2XM 2XR

1X_1Y2

1X2_1Y
2X_1Y 2XE_1Y 2XM_
1Y 2XR_1Y

1X3
3X

-

1X3_1Y
3X_1Y

1X_1Y2
1X_2YE

-

1X4
2X2 2X2E
2X2M
4X 4XE
4XR

1X2_1Y2
2X_1Y2
2XE_1Y2
2XM_1Y2
2XR_1Y2

1X4_1Y
2X2_1Y 2X2E_1Y
2X2M_1Y
4X_1Y 4XE_1Y 4XR_
1Y

1X2_1Y2
2X_1Y2 2XE_1Y2 2XM_1Y2
2XR_1Y2
1X2_2YE 2X_2YE 2XE_2YE
2XM_2YE 2XR_2YE

MEDIUM_6T8

-

1X3_1Y2
3X_1Y2

-

Tap Geometries for Full Camera Link Configuration

1X3_1Y2
1X3_2YE
3X_1Y2
3X_2YE

Applies to:

Tap
Configuration

Line-scan

Bilinear line-scan

Area-scan,
1 YTap

Area-scan, 2 YTap

FULL_8T8

1X8 2X4 4X2
4X2E 8X 8XR

1X4_1Y2
2X2_1Y2 2X2E_
1Y2 2X2M_1Y2
4X_1Y2 4XE_1Y2
4XR_1Y2

1X8_1Y
2X4_1Y
4X2_1Y
4X2E_1Y
8X_1Y
8XR_1Y

1X4_1Y2
2X2_1Y2 2X2E_1Y2 2X2M_
1Y2
4X_1Y2 4XE_1Y2 4XR_1Y2
1X4_2YE
2X2_2YE 2X2E_2YE 2X2M_
2YE
4X_2YE 4XE_2YE 4XR_2YE

49

FullFullXRFullFullXRGrablink Grablink Functional Guide

Tap Geometries for 72-bit Camera Link Configuration

Applies to:

Tap Configuration

Line-scan Trilinear line-scan

Area-scan, 1 YTap

Area-scan, 2 YTap

DECA_3T24

DECA_9T8

1X3 3X

-

-

3X_1Y3

1X3_1Y 3X_1Y

-

-

3X_1Y3

Tap Geometries for 80-bit Camera Link Configuration

Applies to:

Tap Configuration

Line-scan

Area-scan, 1 YTap

Area-scan, 2 YTap

DECA_2T40

DECA_8T10

DECA_8T30B3

-

1X8

1X8

DECA_10T8

1X10 10X

1X2_1Y

1X8_1Y

1X8_1Y

1X10_1Y
10X_1Y

-

-

-

-

50

FullFullXRFullFullXRGrablink Grablink Functional Guide

4.3. Image Cropping

Grablink boards implement an image cropping operator that selects a subset of the pixels
delivered by the camera to build the image delivered to the Host PC.

This subset, named WindowArea, includes:

● For area-scan cameras: a single rectangular region of the 2D image sensor.

● For line-scan cameras: a single segment of the 1D image sensor.

Image Cropping parameters

The operator is controlled through the following Channel Class parameters of the Grabber
Timing category:

● GrabWindow: the main control parameter.

● WindowX_Px, WindowY_Ln: integer parameters defining the size of the WindowArea.

● OffsetX_Px, and OffsetY_Ln: integer parameters defining the position of the WindowArea

within the Camera Active Area.

Window Area Parameters for Area-Scan cameras

Window Area Parameters for Line-Scan cameras

51

Grablink Grablink Functional Guide

NOTE
The position of the Window Area within the Camera Active Window is
expressed as the difference of coordinates between Cw, the center of the
Window Area, and Cw, the center of the Camera Active Area.

NOTE
The range of allowed values of OffsetX_Px and OffsetY_Ln parameters is
automatically adjusted to force the Window Area to stay within the
boundaries of the Camera Active Area.

WindowOrgX_Px reports the X-coordinate in the Camera Active Area of the leftmost pixels of the
Window Area:

For area-scan cameras only, WindowOrgY_Ln reports the Y-coordinate in the Camera Active Area
of the topmost pixels of the Window Area:

Configuring Image Cropping

By default, GrabWindow is set to NOBLACK disabling the ICO: the acquired image includes all
active pixels delivered by the camera without any surrounding weak or blind pixels on the
image edges.

To enable image cropping, proceed as follows:

● Enable cropping by setting GrabWindow to MAN.

● Adjust the width of the Window Area using WindowX_Px. Any integer value ranging from8 up

to Hactive_Px is allowed.

● Forarea-scancamerasonly:Adjust the height of the Window Area using WindowY_Ln. Any

integer value ranging from 1 up to Vactive_Ln is allowed.

● Move horizontally the Window Area using OffsetX_Px. Increasing the value moves the

Window Area towards the right of the Camera Active Area and vice-versa, decreasing the
value moves the Window Area towards the left of the Camera Active Area.

● Forarea-scancamerasonly:Move vertically the Window Area using OffsetY_Ln. Increasing the
value moves the Window Area towards the top of the Camera Active Area and vice-versa,
decreasing the value moves the Window Area towards the bottom of the Camera Active Area.

Conditions of applicability

Cropping is applicable to the following camera classes:

● Monochrome, RGB color, and Bayer CFA color area-scan cameras: any valid combination of

TapConfiguration and TapGeometry is allowed except whenTapGeometry = *_2YE

● Monochrome, and RGB color line-scan cameras.

52

Grablink Grablink Functional Guide

4.4. Image Flipping

Grablink boards implement an image flipping operator that performs the mirroring of the image
delivered to the Host PC:

● For area-scan cameras, it performs the left/right and the top/bottom mirroring.

● For line-scan camera, it is capable to perform the left/right mirroring only.

The operator controlled through the following Channel Class parameters of the Cluster
Category:

● ImageFlipX enables the left/right mirroring

● ImageFlipY enables the top/bottom mirroring.

By default, both operators are OFF disabling any mirroring.

Image Flipping Operations

Conditions of applicability

IFO is applicable to the following camera classes:

● Monochrome, RGB color, and Bayer CFA color area-scancameras

● Monochrome, and RGB color line-scancameras

53

Grablink Grablink Functional Guide

4.5. Pixel Data Processing Configurations

Configurations for Monochrome Pixels

Configurations for Bayer CFA Pixels

Configurations for RGB Pixels

55

57

60

54

Grablink Grablink Functional Guide

Configurations for Monochrome Pixels

Processing elements availability for monochrome pixels vs. products

Capability

LUT transformation

Pixel formatting

OK

OK

OK

OK

OK

OK

OK

OK

Valid configurations when LUT transformation is disabled

Applies to:

The pixel output format is defined by ColorFormat.

Camera PFNC Pixel Format ColorFormat Output PFNC Pixel Format

Mono8

Mono10

Mono12

Mono14

Mono16

Y8

Y8

Y10

Y16

Y8

Y12

Y16

Y8

Y14

Y16

Y8

Y16

Mono8

Mono8

Mono10

Mono16

Mono8

Mono12

Mono16

Mono8

Mono14

Mono16

Mono8

Mono16

55

BaseDualBaseFullFullXRBaseDualBaseFullFullXRGrablink Grablink Functional Guide

Valid configurations when LUT transformation is enabled

Applies to:

The LUT operator operates in the monochrome mode, its input bit depth is the camera pixel bit
depth.

The LUT output bit depth and the pixel output format are defined by ColorFormat.

Camera PFNC Pixel
Format

Lookup Table
Configuration

ColorFormat

Output PFNC Pixel
Format

Mono8

Mono10

Mono12

M_8x8

M_10x8

M_10x10

M_10x16

M_12x8

M_12x12

M_12x16

Y8

Y8

Y10

Y16

Y8

Y12

Y16

Mono8

Mono8

Mono10

Mono16

Mono8

Mono12

Mono16

56

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Configurations for Bayer CFA Pixels

Processing elements availability for Bayer CFA pixels vs. products

Elements

Bayer CFA Decoding

White Balance

LUT Transformation

Pixel Formatting

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

Valid configurations when Bayer CFA decoding is disabled

Applies to:

The pixel output format is defined by ColorFormat.

Camera PFNC Pixel Format ColorFormat Output PFNC Pixel Format

Bayer**8

Bayer**10

Bayer**12

Bayer**14

Bayer**16

BAYER8

BAYER8

BAYER10

BAYER16

BAYER8

BAYER12

BAYER16

BAYER8

BAYER14

BAYER16

BAYER16

Bayer**8

Bayer**8

Bayer**10

Bayer**16

Bayer**8

Bayer**12

Bayer**16

Bayer**8

Bayer**14

Bayer**16

Bayer**16

NOTE
The white balance and the LUT transformation are not available when Bayer
CFA decoding is disabled!

Valid configurations when Bayer CFA decoding is enabled

Applies to:

The pixel processing chain uses the following elements:

● Bayer CFA Decoder

● White Balance operator (Optional)

● Look-Up-table operator (Optional)

57

BaseDualBaseFullFullXRBaseDualBaseFullFullXRBaseDualBaseFullFullXRGrablink Grablink Functional Guide

The Look-Up-table operator is configured for RGB color processing.

The processing chain outputs one RGB pixel for each RAW pixel of the input buffer.

Pixel Processing Chain - Bayer CFA => RGB Configuration

Valid configurations and peak processing rate [Megapixels/sec] for PC1624 Grablink
Base and PC1623 Grablink DualBase

Applies to:

Camera PFNC
Pixel Format

WBO

Lookup Table
Configuration

ColorFormat

Output
PFNC Pixel
Format

Bayer**8

Optional

Bayer**10

Optional

Bayer**12

Optional

8-bit to 8-bit
RGB

10-bit to 8-bit
RGB

12-bit to 8-bit
RGB

Bayer14

Optional

Not available

Bayer16

Optional

Not available

RGB24
RGB32
RGB24PL

RGB24
RGB32
RGB24PL

RGB24
RGB32
RGB24PL

RGB24
RGB32
RGB24PL

RGB24
RGB32
RGB24PL

BGR8
BGRa8
-

BGR8
BGRa8
-

BGR8
BGRa8
-

BGR8
BGRa8
-

BGR8
BGRa8
-

Max.
rate

125

125

125

125

125

58

BaseDualBaseGrablink Grablink Functional Guide

Valid configurations and peak processing rate [Megapixels/sec] for PC1622 Grablink
Full and PC1626 Grablink Full XR

ColorFormat

Output PFNC
Pixel Format

Max.
rate

BGR8
BGRa8
-

BGR8
BGRa8
-

-

-

BGR8
BGRa8
-

-

-

BGR8
BGRa8
-

-

-

BGR8
BGRa8
-

250

250

125

125

250

125

125

250

125

125

250

125

Applies to:

Camera PFNC
Pixel Format

WBO

Bayer8

Optional

Bayer10

Optional

Bayer10

Optional

Bayer10

Optional

Bayer12

Optional

Bayer12

Optional

Bayer12

Optional

Lookup Table
Configuration

8-bit to 8-bit
RGB

10-bit to 8-bit
RGB

10-bit to 10-bit
RGB

10-bit to 16-bit
RGB

12-bit to 8-bit
RGB

12-bit to 12-bit
RGB

12-bit to 16-bit
RGB

Bayer14

Optional

Not available

RGB24
RGB32
RGB24PL

RGB24
RGB32
RGB24PL

RGB30PL

RGB48PL

RGB24
RGB32
RGB24PL

RGB36PL

RGB48PL

RGB24
RGB32
RGB24PL

Bayer14

Bayer14

Optional

Not available

RGB42PL

Optional

Not available

RGB48PL

Bayer16

Optional

Not available

RGB24
RGB32
RGB24PL

Bayer16

Optional

Not available

RGB48PL

-

59

FullFullXRGrablink Grablink Functional Guide

Configurations for RGB Pixels

Processing elements availability for RGB pixels vs. products

Elements

White Balance

LUT Transformation

Pixel Formatting

OK

OK

OK

Valid configurations

OK

OK

OK

OK

OK

OK

OK

OK

OK

The processing chain outputs one RGB pixel for each RGB pixel of the input buffer.

Pixel Processing Chain - RGB => RGB Configuration

Valid configurations for PC1624 Grablink Base and PC1623 Grablink DualBase

Applies to:
Camera
PFNC
Pixel
Format

WBO

Lookup Table
Configuration

ColorFormat

RedBlueSwap

RGB8

Optional

8-bit to 8-bit
RGB

RGB24

RGB32

ENABLE

DISABLE

ENABLE

DISABLE

RGB24PL

-

Output
PFNC
Pixel
Format

BGR8

RGB8

BGRa8

RGBa8

-

60

BaseDualBaseFullFullXRBaseDualBaseGrablink Grablink Functional Guide

Valid configurations for PC1622 Grablink Full and PC1626 Grablink Full XR

Applies to:

Camera PFNC
Pixel Format

WBO

LUT

ColorFormat

RedBlueSwap

Output
PFNC Pixel
Format

BGR8

RGB8

BGRa8

RGBa8

-

BGR8

RGB8

BGRa8

RGBa8

-

-

-

BGR8

RGB8

BGRa8

RGBa8

-

-

-

RGB8

Optional

8-bit to
8-bit RGB

RGB24

RGB32

ENABLE

DISABLE

ENABLE

DISABLE

RGB24PL

-

RGB10

RGB12

Optional

10-bit to
8-bit RGB

Optional

Optional

10-bit to
10-bit
RGB

10-bit to
16-bit
RGB

Optional

12-bit to
8-bit RGB

Optional

Optional

12-bit to
10-bit
RGB

12-bit to
16-bit
RGB

ENABLE

DISABLE

ENABLE

DISABLE

-

-

-

ENABLE

DISABLE

ENABLE

DISABLE

-

-

-

RGB24

RGB32

RGB24PL

RGB30PL

RGB48PL

RGB24

RGB32

RGB24PL

RGB36PL

RGB48PL

61

FullFullXRGrablink Grablink Functional Guide

4.6. Look-up Table Transformation

Applies to:

The look-up table operator enables you to process monochrome or RGB color pixel data
streams.

Storage for four LUT definitions is available in the main memory. They are indexed from 1 to 4.

Selecting the LUT of index 0 disables the LUT operator and establishes a bypass over the Look-
up table operator in the pixel processing stream.

During MultiCam channel activation, the hardware initializes the LUT operator. Therefore it fills
the tables of the LUT operator with the selected LUT definition.

Any further modification of the LUT operator configuration that occurs during the acquisition
sequence is applied without any further delay. For example, this occurs when:

● Changing the LUT_UseIndex parameter.

● Modifying the LUT definition have the same index as the LUT_UseIndex value.

See also: Using Look-Up Tables in the MultiCam User Guide.

Monochrome Operation

When the acquisition channel is configured for acquisition from monochrome cameras, the LUT
operator is modeled as a single very high speed RAM inserted into the pixel data stream.

Available configurations and performance of the LUT operator for monochrome
cameras

Camera

LUT
Input
bit depth

LUT
output
bit depth

8

10

12

14

16

8

10

12

12

12

8

8, 10, 16

8, 12, 16

8, 14, 16

8, 16

Peak pixel rate
[Megapixels/s]

Peak pixel rate
[Megapixels/s]

500

250

250

250

250

1000

500

500

500

500

The input bit depth of the Look-Up-Table is:

● Equal to the camera bit depth for 8-, 10- and 12-bit cameras.

● 12-bit for 14-bit and 16-bit cameras.

62

BaseDualBaseFullFullXRBaseDualBaseFullFullXRGrablink Grablink Functional Guide

NOTE
For 14-bit and 16-bit cameras, when the look-up table operator is enabled,
only 12 most significant bits of the camera pixel data are effectively
considered; the remaining bits are ignored.

The output bit depth of look-up table is equal to the bit depth of the selected output format.
The possible bit depths depends on the pixel depth of the camera:

● For 8-bit cameras: 8-bit

● For cameras delivering more than 8-bit: 8-bit, 16-bit and the same bit depth as the camera.

The LUT operator is designed to sustain the highest pixel rate achievable by the board-
compatible monochrome cameras.

RGB Color Acquisition

When the board is configured for acquisition from RGB color cameras, the LUT operator is
modeled as a triplet of very high speed RAMs inserted into the red, green, and blue pixel
components data streams.

The three color components have the same bit depth. Consequently, the three look-up tables
have the same input bit depth and the same output bit depth.

Available configurations and performance of the LUT operator for RGB color cameras

Camera

LUT
Input
bit depth

LUT
output
bit depth

Peak pixel rate
[Megapixels/s]

Peak pixel rate
[Megapixels/s]

8

10

12

8

10

12

8

8, 10, 16

8, 10, 16

125

-

-

250

125

125

The input bit depth of each look-up table is equal to the camera bit depth of each color
component.

The output bit depth of each look-up table is equal to the bit depth of each color component of
the selected output format. The possible color components bit depths depends on the color
component pixel bit depth of the camera:

● For 3 x 8-bit RGB cameras: 8-bit.

●

bit and the same bit depth as the component bit depth of the camera.

For cameras delivering more than 8-bit per component: 8-bit,16-

The LUT operator can sustain the highest pixel rate achievable by the board-compatible RGB
color cameras.

63

BaseDualBaseFullFullXRFullFullXRGrablink Grablink Functional Guide

Bayer Color Acquisition

When the board is configured for acquisition from Bayer color cameras, the LUT operator is
modeled as a triplet of very high speed RAM inserted into the red, green, and blue pixel
components data streams delivered by the CFA decoder.

Available configurations of the LUT operator for Bayer color cameras when the Bayer
decoder is enabled

Camera

LUT
Input
bit depth

LUT
output
bit depth

8

10

12

14

16

8

10

12

12

12

8

8

10, 16

8

12, 16

8

14, 16

8

16

Peak pixel rate
[Megapixels/s]

Peak pixel rate
[Megapixels/s]

125

-

-

-

-

-

-

-

-

250

250

125

250

125

250

125

250

125

The input bit depth of the look-up table is:

● Equal to the camera bit depth for 8-, 10- and 12-bit cameras.

● 12-bit for 14-bit and 16-bit cameras.

NOTE
For 14-bit and 16-bit cameras, when the look-up table operator is enabled,
only 12 most significant bits of each component delivered by the CFA
decoder are effectively considered; the remaining bits are ignored.

The output bit depth of each of the 3 look-up tables is equal to the bit depth of each color
component of the selected output format. The possible color components bit depths depends
on the pixel bit depth of the camera and board type:

● For 8-bit Bayer cameras: 8-bit per component.

●

bit and the same bit depth as the component bit depth of the camera.

For cameras delivering more than 8-bit per component; 8-bit, 16-

The performance of the RGB LUT operator matches the performance of the Bayer CFA decoder.

64

BaseDualBaseFullFullXRFullFullXRGrablink Grablink Functional Guide

4.7. Bayer CFA to RGB Conversion

Applies to:

The Bayer CFA decoder transforms the raw Bayer CFA data stream issued by the camera into an
RGB color data stream.

The missing pixel components are reconstructed using one of the following interpolation
methods:

● Legacy interpolation method computes the missing color components using exclusively

Mean() functions of the nearest components.

● Advanced interpolation method computes the missing color components using Mean() and
Median() functions of the nearest components. The advanced interpolation eliminates the
"creneling" effect on the highly contrasted sharp transitions in the image.

The CFA decoder requires up to eight surrounding pixels to compute the missed components of
RGB pixels. Surrounding pixels are identified by their geographic location relative to the pixel for
computation.

Definitions

Mean2(a,b) = (a+b)/2
Mean4(a,b,c,d) = (a+b+c+d)/4
Median2Of4(a,b,c,d) = Mean2{ Min [ Max(a,b); Max(c,d) ] ; Max [ Min(a,b); Min(c,d) ] }

Computing Missing Components on 4 Positions of a Bayer CFA Array

65

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

For red pixel locations (case of R22)

G <= Mean4(GN, GS, GE, GW)
B <= Mean4(BNE, BSE, BSW, BNW)

Case of R22

For green pixel locations in lines with blue (case of G23)

R <= Mean4(RN, RS)
B <= Mean4(BE, BW)

Case of G23

For green pixel locations in lines with red (case of G32)

R <= Mean2(RE, RW)
B <= Mean2(BN, BS)

Case of G32

66

Grablink Grablink Functional Guide

For blue pixel locations (case of B33)

G <= Mean4(GN, GS, GE, GW)
R <= Mean4(RNE, RSE, RSW, RNW)

Enabling the Bayer CFA Decoder

Case of B33

The Bayer CFA decoding function is automatically enabled if all the following conditions are
satisfied:

1. The camera is an area-scan camera (Imaging = AREA)

2. The camera is a color camera (Spectrum = COLOR)

3. The camera delivers raw data from a Bayer Color Filter Array sensor (ColorMethod = BAYER)

Registering Bayer CFA

The registration of the BAYER CFA must be correctly set by assigning the appropriate value to
the ColorRegistration parameter.

There are 4 values: GB, BG, RG, GR corresponding to the colors of the 2 first pixels of the first
image line delivered by the camera.

Configuring the Bayer CFA Decoder

The CFA decoder has only one setting to select the interpolation method: the CFD_Mode
parameter.

The default and recommended setting is ADVANCED. The alternate setting is LEGACY.

CFA Decoder Performance

The peak pixel processing rate of the CFA decoder of each acquisition channel is 250
megapixels/s.

The performance level of the Bayer CFA decoder is matching approximately the performance of
the DMA transfer. However, the peak pixel processing rate is significantly lower than the highest
pixel rate achievable by the board-compatible Bayer CFA cameras.

67

Grablink Grablink Functional Guide

4.8. White Balance

Applies to:
DetaileddescriptionoftheWhiteBalanceOperator

What Is White Balance?

White Balance Operator

Automatic Calibration Description

Automatic Calibration Requirements

Automatic Calibration Timing

AWB_AREA Settings Description

69

70

72

73

75

76

68

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

What Is White Balance?

Color image acquisition

A color image acquisition involves the use of three color filters on the camera sensor. Each color
filter restricts the light source to a range of wavelengths of the light spectrum, either red (R),
green (G), or blue (B).

An ideal capture system renders a white object as a white image. A white stimulation should
yield the same signal for R, G and B filters. But practically, there are always unavoidable defects
on the signals that introduce a whiteimbalance.

White imbalance factor

Several factors, due to the camera and to the capture conditions, are responsible for the white
imbalance:

● Object illumination. The color of an object is a combination of its reflectivity and the spectral

contents of the illuminating light.

● Camera optical filters response.

● Sensor sensitivity, which is not the same for the three ranges of wavelength.

● Different gain coefficients applied to each color signal before digitization.

White balance correction

MultiCam can correct the white imbalance of the capture system. The operation is called the
whitebalance:

● The whitebalanceoperatorapplies correcting coefficients (R, G, and B gains) to each color
signal, so, for a white object, the combination of the R, G, and B signals renders a white
image.

● The whitebalancecalibrationis the computation of the three R, G, and B gains. It is

performed on a representative image area, prior to the image capture. It can be automatic or
manual.

69

Grablink Grablink Functional Guide

White Balance Operator

White Balance Operator - Block Diagram

The White Balance Operator is an element of the pixel processing chain. It is composed with 3
identical processing blocks, one for each color component.Each processing block contains 3
elements:

● One register

● One multiplier

● One clipper

The register element holds the gain correction factor. The gain value is registered as a 16-bit
unsigned binary value allowing gain correction factors to be accurately defined.

The multiplier computes the product of the gain correction factor and the color component
value. It is capable to handle components having 8-bit, 10-bit, 12-bit, 14-bit and 16-bit bit depth.

The multiplier output is clipped to the maximum value of the digital output scale. The digital
output scale is in all cases identical to the digital input scale; itself identical to the digital output
scale of the camera. For instance, for a camera delivering 10-bit components, the digital scale is
[0..1023].

70

Grablink Grablink Functional Guide

White Balance Operator - Transfer Function

The above drawing shows 2 transfer functions of one component of the White Balance Operator:

● The blue line corresponds to a gain setting of 1.000; i.e., the minimal allowed gain value.

● The red line corresponds to a gain setting of 2.000. The output remains proportional to the
input until the 100% full-scale output is reached; for greater input values, the output is
clipped to 100% full-scale!

71

Grablink Grablink Functional Guide

Automatic Calibration Description

The color calibration process takes place during the first acquisition phase of a MultiCam
acquisition sequence when the WBO_Mode is set to ONCE.

The color calibrator analyzes a rectangular area (AWB_AREA) of one uncorrected image and
computes a correcting gain factor for each RGB color component.

The correction factor for the color component having the strongest response is always 1; the
correction factors for the weakest color components are greater than 1.

Providing that the requirements of the colorsourceequipment, the calibrationtargetand the
acquisitionchannelsettingsare fulfilled, the calibrator estimates the gain factors with an
accuracy better than 1/1000.

Applying the calculated gain correction factors to the White Balance Operator for subsequent
image acquisitions allows on-the-fly color balancing of the acquired images.

The calibrator returns a NOT_OK status in the following cases:

● Excessive color imbalance.

● Not enough pixels satisfying the calibration target requirements in the AWB_AREA.

72

Grablink Grablink Functional Guide

Automatic Calibration Requirements

This topic describes the requirements that must be fulfilled to obtain optimal calibration
results.

Image Source Equipment Requirements

The image source equipment including: the camera, the lighting and the optical elements, must
exhibit:

● A linear response: The digital value of each color component must be proportional to the

light intensity of the corresponding color.

● A moderate color imbalance: The ratio between the response of the strongest color

component and the weakest color component must be less than 5.

Calibration Target Requirements

The calibration target is a neutral color object located in the field of view of the camera during
the calibration process.

The form of the target can be either:

● Clustered light gray pixels located in a specific area of the camera field of view.

● Non-clustered-light gray pixels located in a specific area of the camera field of view.

● Non-clustered-light gray pixels located anywhere in the camera field of view.

The calibration target can be:

● In the object to inspect.

● A specific object placed in the camera field of view during the calibration phase.

The appearance of the target must be:

● A neutral light gray color.

● The level of the brightest component within 75% to 90% of the full scale.

● The level of the darkest component above 15% of the full scale.

The target must contain at least 256 pixels satisfying the appearance requirements.

Acquisition Channel Settings

The parameter WBO_Mode must be set to ONCE.

The parameters defining the position and the size of the AWB_AREA must be configured such
that:

● It includes at least 256 pixels satisfying the calibration target appearance requirements.

● It contains at least 1 line and 32 columns of pixels.

● It is located entirely within the Camera Active Area.

Specifically onPC1624 Grablink Base, PC1623 Grablink DualBase, PC1622 Grablink Full and
PC1626 Grablink Full XR:

● The LUT Operator must be disabled.

73

Grablink Grablink Functional Guide

● The position and the size of the cropping area must be configured such that it encompasses

the AWB_AREA

74

Grablink Grablink Functional Guide

Automatic Calibration Timing

The color calibration process takes place during the first acquisition phase of a MultiCam
acquisition sequence when the WBO_Mode is set to ONCE.

The White Balance Operator is disabled before the sequence starts.

The calibration process begins when the DMA transfer of the first acquisition phase is
completed. The first MC_SIG_SURFACE_PROCESSING signal of the sequence is delayed until
the completion of the calibration process.

● At the completion of a successful calibration process:

● The value of the parameter WBO_Status is set to OK.

● The values of parameters WBO_GainR, WBO_GainG, and WBO_GainB are updated with

the calibration results.

● The White Balance Operator is reconfigured with the new settings.

At the completion of an unsuccessful calibration process:

● The value of the parameter WBO_Status is set to NOT_OK.

● The original values of parameters WBO_GainR, WBO_GainG, and WBO_GainB are

restored.

● The White Balance Operator is reconfigured with the original settings.

Calibration Timing Diagram

75

Grablink Grablink Functional Guide

AWB_AREA Settings Description

AWB_AREA layout

The AWB_AREA is a rectangular area within the Camera Active Window that is analyzed by the
color balancing calibrator.

The size and the position of the AWB_AREA within the Camera Active Area is defined by the
following parameters: WBO_Width, WBO_Height, WBO_OrgX, and WBO_OrgY.

The default size of the AWB_AREA is the whole Camera Active Area.

76

Grablink Grablink Functional Guide

4.9. Pixel Formatting

Pixel Component Unpacking

Grablink boards unpack 10-bit, 12-bit, and 14-bit pixel component data to 16-bit pixel data.

Two unpacking options are available:

● Unpacking to lsb (Default setting)

● Unpacking to msb

Unpacking to lsb

The significant bits of the pixel component data are aligned to the least significant bit of the
data container. Padding '0' bits are put as necessary in the most significant bits to reach the
next 8-bit boundary.

● 10-bit pixels: 0000 00<pp pppp pppp>

● 12-bit pixels: 0000 <pppp pppp pppp>

● 14-bit pixels: 00<pp pppp pppp pppp>

NOTE
Unpacking to lsb doesn't modify the pixel component value.

Unpacking to msb

The significant bits of the pixel component data are aligned to the most significant bit of the
data container. Padding '0' bits are put as necessary in the least significant bits to reach the
next 8-bit boundary.

● 10-bit pixels: <pppp pppp pp>00 0000

● 12-bit pixels: <pppp pppp pppp> 0000

● 14-bit pixels: <pppp pppp pppp pp>00

NOTE
Unpacking 10-bit, 12-bit, and 14-bit pixel components to msb multiplies the
pixel component value by 64, 16, and 4 respectively.

77

Grablink Grablink Functional Guide

NOTE
Unpacking 8-bit and 16-bit pixel components is a neutral operation:

● The size of the data container is unchanged: One byte for 8-bit pixel

components; two bytes for 16-bit pixel components

● The data bits are not modified

NOTE
Unpacking 10-bit, 12-bit, and 14-bit pixel components increases the amount
of data by 160%, 133%, and 114% respectively.

No unpacking

The packed image data transmitted by the camera through the CoaXPress Link is delivered as is
to the output buffer.

Pixel Bit Depth Reduction

Grablink boards are capable to reduce the bit depth of 10-/12-/14- and 16-bit pixel components
to 8-bit by truncation of the least significant bits.

Pixel Format Control

Pixel Component Unpacking and Pixel Bit Depth reduction are controlled by the ColorFormat
parameter.

For instance for monochrome 10-bit pixels:

● Set ColorFormat to Y8 to select the bit depth reduction

● Set ColorFormat to Y16 to select the unpacking to msb

● Keep the default value (or set) ColorFormat to Y10 to select unpacking to lsb

Refer to "Configurations for Monochrome Pixels" on page 55, "Configurations for Bayer CFA
Pixels" on page 57 and "Configurations for RGB Pixels" on page 60 for an exhaustive list of valid
pixel processing configurations.

Refer to D406EN-MultiCamStorageFormatsPDF document for an exhaustive description of
pixel formats.

78

Grablink Grablink Functional Guide

4.10. Image Transfer

The processed and formatted image data are transferred into a surface over the PCI Express bus
using a DMA engine.

NOTE
In the MultiCam driver, a surface is the physical memory space allocated
into the host PC memory for the storage of one image.

The transferred image is stored in the destination surface in a progressive-scan order:

● The first pixel (top-left corner) of the transferred image is stored at the first memory location

of the surface (at 0).

● The first pixels of the subsequent lines are stored at a byte address that is a multiple of the

surface pitch.

Surface Pitch

The MultiCam driver establishes a default surface pitch that corresponds to the amount of bytes
required to store one pixel row of the image data. You may increase this value if the application
requires it.

Surface Size

The MultiCam driver establishes a default surface size that corresponds to the amount of bytes
required to store all the rows of the image data. You may increase this value if its application
requires it.

Extended Addressing Capabilities

Applies to:

These Grablink boards are capable of transferring image data anywhere into the physical
memory of the system.

Automatic DMA Descriptors Loading

Applies to:

These Grablink boards implement an automatic mechanism for the loading of the DMA
descriptors from the host PC memory to the DMA descriptor section of the on-board memory.

The DMA descriptors lists can then be prepared and stored into the host PC memory; the
automatic loader pre-fetches the descriptors such that they are available for the DMA engine
when it needs them.

79

BaseDualBaseFullFullXRBaseDualBaseFullFullXRGrablink Grablink Functional Guide

Notes

□ The transfer of the descriptors is performed with the second DMA engine, it doesn't

require any CPU work.

□ The transfer of the descriptors does not impact the available bandwidth on the PCI
Express link for image transfer since it conveys data on the opposite direction.

Transfer Rate

Applies to:

This board sustains image data transfer over the PCI Express bus:

● Up to 200 megabytes/s for a PCI Express payload size of 256 bytes

● Up to 180 megabytes/s for a PCI Express payload size of 128 bytes.

WARNING
The effective data rate depends on the performance of the PCI Express link.

Transfer Rate

Applies to:

This board sustains image data transfer over the PCI Express bus:

● Up to 833 megabytes/s for a PCI Express payload size of 256 bytes and 64-bit addressing.

● Up to 844 megabytes/s for a PCI Express payload size of 256 bytes and 32-bit addressing.

● Up to 754 megabytes/s for a PCI Express payload size of 128 bytes and 64-bit addressing.

● Up to 780 megabytes/s for a PCI Express payload size of 128 bytes and 32-bit addressing.

WARNING
The effective data rate depends on the performance of the PCI Express link.

80

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

4.11. Transfer Latency

The transfer latency is the time interval between the time when the last camera pixel of the
image enters the frame grabber and the same processed pixel is stored in the host PC memory.

.

Case #1: Camera pixel rate < max pixel processing or pixel delivery rate

Case #2: Camera pixel rate > max pixel processing or pixel delivery rate

81

Grablink Grablink Functional Guide

For both cases

● The camera delivers the image starting at time t0 and ending at time t2. For the simplicity,

the line blanking intervals are not shown.

● The on-board pixel processing and DMA transfer to the host PC memory begins at time t1 and

ends at time t3.

● The pixel processing requires that a minimum of pixel data (P1 pixels) is available into the

buffer. Consequently, the processing of the first pixel is retarded until the minimum amount
of pixel is available. The time interval (t1 - t0) is the "initial latency"; it depends only on the
camera pixel data rate and P1.

● The time interval (t3 - t2) is the "latency".

For case #1

● After t1, the effective pixel processing rate is equal to the camera pixel rate; the processing is

eventually halted at line boundaries until enough data is available from the camera to
resume it.

● When the camera has delivered the last pixel of the image, the processing continues at the
max pixel processing (or the maximum delivery rate) until the last pixel of the image. The
latency depends only on P1 and the maximum processing/delivery rate.

For case #2

● After t1, the effective pixel processing rate is the maximum pixel processing/delivery rate

until the last pixel of the image. The time interval (t3 - t2) is the "latency"; it depends only on
P1 and on the maximum processing/delivery rate.

For the estimation of the latency

The following board characteristics are be considered:

● P1 is the amount of pixel that must be received from the camera before initiating the pixel

processing. This amount is equal to the number of pixels contain into:
□ Two lines of the image if the Bayer CFA decoder is used.
□ One line of the image if the Bayer CFA decoder is not used.

● The maximum processing/delivery pixel rate is the smallest value of :

□ The pixel processing rate.
□ The pixel delivery rate over the PCIe interface.

● For the pixel processing rate, the following processing functions may limit the processing

rate if they are used:
□ For the Bayer CFA decoder performances, refer to Bayer CFA to RGB Conversion for more

details

□ For LUT processor performances, refer to Look-up Table Transformation for more details.

● For the pixel delivery rate, refer to "Image Transfer" on page 79 for an estimation of the data
transfer rate and divide by the number of bytes/pixel to obtain an evaluation in pixels/s.

82

Grablink Grablink Functional Guide

5. Acquisition

5.1. Grablink Acquisition Modes

5.2. SNAPSHOT Acquisition Mode

5.3. HFR Acquisition Mode

5.4. WEB Acquisition Mode

5.5. PAGE Acquisition Mode

5.6. LONGPAGE Acquisition Mode

5.7. Setting Optimal Page Length

84

85

88

91

94

98

101

83

Grablink Grablink Functional Guide

5.1. Grablink Acquisition Modes

Grablink products support the following fundamental acquisition modes for area-scan and line-
scan cameras:

Fundamental acquisition modes for area-scan cameras

Acquisition
Mode

SNAPSHOT

HFR

Short Description

The SNAPSHOT acquisition mode is intended for the acquisition of snapshot
images.

The HFR acquisition mode is intended for the acquisition of snapshotimages
from highframeratecameras.

Fundamental acquisition modes for line-scan cameras

Acquisition
Mode

Short Description

WEB

PAGE

LONGPAGE

The WEB acquisition mode is intended for image acquisition of single
continuousobjectsofanysize.

The PAGE acquisition mode is intended for image acquisition of multiple
discreteobjectshavingafixedsize.
The page size is user-configurable up to 65,535 lines. The acquisition
sequence can be configured to terminate automatically after a predefined
number of objects.

The LONGPAGE acquisition mode is intended for image acquisition of
multiplediscreteobjectshaving,possibly,avariableand/oralargersize.
This mode supports objects up to 2,147,483,648 lines and has the unique
capability to acquire variable size objects as defined by a "Page Cover"
signal.

The user must select the fundamental acquisition mode by assigning the appropriate value to
the AcquisitionMode parameter before any other acquisition control parameter.

84

Grablink Grablink Functional Guide

5.2. SNAPSHOT Acquisition Mode

The SNAPSHOT acquisition mode is intended for the acquisition of snapshotimages.

Description

In the SNAPSHOT acquisition mode, the unique sequence is capable of acquiring SeqLength_Fr
frames within the channel activity period.

The SNAPSHOT acquisition mode is the default mode enforced automatically by MultiCam for
all area-scan cameras; it can also be explicitly invoked by assigning value SNAPSHOT to
AcquisitionMode.

Preparing the Channel for SNAPSHOT acquisition

The first action is to define all MultiCam Channel parameters before channel activation.

More specifically the following acquisition control parameters need to be configured:

Parameter

AcquisitionMode

TrigMode

NextTrigMode

SeqLength_Fr

Value range

SNAPSHOT

IMMEDIATE, HARD, SOFT and COMBINED

SAME, REPEAT, HARD, SOFT and COMBINED

MC_INDETERMINATE, 1 ~ 65534

When invoking the SNAPSHOT acquisition mode:

● ActivityLength is enforced to 1. The channel goes inactive at the completion of the sequence.

● PhaseLength_Fr is enforced to 1. A single frame is acquired during an acquisition phase.

● TrigMode establishes the starting condition of the sequence and consequently the starting

condition of the first phase of the sequence. Possible values are IMMEDIATE, HARD, SOFT and
COMBINED. The default MultiCam setting is IMMEDIATE.

● NextTrigMode establishes the starting condition of the subsequent phases within the

sequence. Possible values are SAME, REPEAT, HARD, SOFT and COMBINED. The default value
is SAME.

● The sequence length is specified by SeqLength_Fr. Assigning a value MC_INDETERMINATE

enforces an indefinite acquisition sequence.

Activating the Channel

Setting ChannelState parameter to ACTIVE activates the channel and arms the trigger circuit.

The SNAPSHOT acquisition sequence will effectively start after the first trigger event occurring
after channel activation when the first acquisition phase is triggered.

85

Grablink Grablink Functional Guide

Starting the first SNAPSHOT acquisition phase

The trigger source is determined by the Start Trigger condition. Usually, the start trigger event is
a hard trigger; however a soft trigger signal can also be selected. Usually, the trigger is derived
from a position detector. The trigger condition is defined by parameter TrigMode.

A programmable time delay can optionally be specified with parameter TrigDelay_us. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

At first trigger event, the image capture starts effectively; a "Start of Sequence" signal is
reported to the user.

Starting the subsequent SNAPSHOT acquisition phases

The trigger source for all subsequent acquisition phases is determined by the Next Start Trigger
condition. Usually, the Next start trigger event is the same as the Start Trigger condition. The
next trigger condition is defined by parameter NextTrigMode.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

A programmable time delay can optionally be specified with parameter TrigDelay_us. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

At subsequent trigger events, the image capture starts effectively; however no "Start of
Sequence" signal is reported to the user.

SNAPSHOT acquisition phase

Once the SNAPSHOT acquisition phase is started, the frame grabber acquires one image frame
and stores image data into one surface.

Stopping a SNAPSHOT acquisition sequence

The SNAPSHOT acquisition mode allows two methods to stop the sequence: Manualstopand
Automaticstop.

If the number of frames to capture is known before starting the sequence and is not higher than
65,535, the automatic stop method can be used to automatically stop the sequence after a
predefined number of frames. This variant of SNAPSHOT acquisition is named FiniteSNAPSHOT
acquisition. In this case, the number of frames to be acquired needs to be specified before
Channel activation.

If the number of frames is unknown or higher than 65,535, the manual stop method is required.
In this case, the parameter SeqLength_Fr should be set to MC_INDETERMINATE, in order to
define an infiniteSNAPSHOTacquisitionsequence.

Manual sequence stop

The SNAPSHOT acquisition sequence stops by setting ChannelState=IDLE.

86

Grablink Grablink Functional Guide

In case of a user break event, the BreakEffect parameter value is irrelevant. The acquisition
terminates ALWAYS at a phase —frame— boundary ensuring the integrity of the frame.

Automatic sequence stop

The sequence terminates automatically after the acquisition of the specified number of frames.
An indefinite acquisition sequence stops when the channel is forced to its inactive state.

Use parameter SeqLength_Fr to specify the total number of frames to be acquired. The
sequence will automatically stop after the last acquired frame.

Monitoring a SNAPSHOT acquisition

● Elapsed_Fr reports the number of acquired frames in the sequence.

● When the sequence length is defined (SeqLength_Fr≠MC_INDETERMINATE), Remaining_Fr

reports the number of remaining frames in the sequence.

● When the sequence contains more than 1 frame (SeqLength_Fr>1), PerSecond_Fr reports the

measured average frame rate.

87

Grablink Grablink Functional Guide

5.3. HFR Acquisition Mode

The HFR acquisition mode is intended for the acquisition of snapshotimagesfrom highframe
ratecameras.

Description

In HFR acquisition mode, the unique sequence is divided into phases, each phase acquiring
PhaseLength_Fr frames into a single destination surface.

The HFR acquisition mode is applicable to area-scan cameras with a frame rate not exceeding
1,275,000 frames per second.

The HFR acquisition mode is explicitly invoked by assigning value HFR to AcquisitionMode.

Preparing the Channel for SNAPSHOT acquisition

The first action is to define all MultiCam Channel parameters before channel activation.

More specifically the following acquisition control parameters need to be configured:

Parameter

Value range

AcquisitionMode

HFR

TrigMode

NextTrigMode

SeqLength_Fr

IMMEDIATE, HARD, SOFT and COMBINED

SAME, REPEAT, HARD, SOFT and COMBINED

MC_INDETERMINATE, 1 ~ (PhaseLength_Fr×65,534)

PhaseLength_Fr

1 ~ 255

When invoking the HFR acquisition mode:

● The ActivityLength parameter is enforced to 1. The channel goes inactive at the completion

of the sequence.

● The number of frames per acquisition phase is specified by PhaseLength_Fr. The minimal

applicable value is the camera frame rate divided by 5000.

● The TrigMode parameter establishes the starting condition of the sequence and

consequently the starting condition of the first slice of the first phase —the first frame— of
the sequence. Possible values are IMMEDIATE, HARD, SOFT and COMBINED. The default
MultiCam setting is IMMEDIATE.

● The NextTrigMode parameter establishes the starting condition of the subsequent slices —

frames— within the sequence. Possible values are SAME, REPEAT, HARD, SOFT and
COMBINED. The default value is SAME.

● The sequence length is specified by SeqLength_Fr. Assigning a value MC_INDETERMINATE

enforces an indefinite acquisition sequence.

88

Grablink Grablink Functional Guide

Starting the first slice —frame— of a HFR acquisition sequence

The trigger source is determined by the Start Trigger condition. Usually, the start trigger event is
a hard trigger; however a soft trigger signal can also be selected. Usually, the trigger is derived
from a position detector. The trigger condition is defined by parameter TrigMode.

A programmable time delay can optionally be specified with parameter TrigDelay_us. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

At first trigger event, the image capture starts effectively; a "Start of Sequence" signal is
reported to the user.

Starting the subsequent slices —frames— of a HFR acquisition sequence

The trigger source for all subsequent acquisition slices is determined by the Next Start Trigger
condition. Usually, the Next start trigger event is the same as the Start Trigger condition. The
next trigger condition is defined by parameter NextTrigMode.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

A programmable time delay can optionally be specified with parameter TrigDelay_us. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

At subsequent trigger events, the image capture starts effectively; however no "Start of
Sequence" signal is reported to the user.

Stopping a HFR acquisition sequence

The HFR acquisition mode allows two methods to stop the sequence: Manualstopand
Automaticstop.

If the number of frames to capture is known before starting the sequence and is not higher than
(PhaseLength_Fr×65,534), the automatic stop method can be used to automatically stop the
sequence after a predefined number of frames. This variant of HFR acquisition is named Finite
HFRacquisition. In this case, the number of frames to be acquired needs to be specified before
Channel activation.

If the number of frames is unknown or higher than (PhaseLength_Fr×65,534), the manual stop
method is required. In this case, the parameter SeqLength_Fr should be set to MC_
INDETERMINATE, in order to define an infiniteHFRacquisitionsequence.

Manual sequence stop

The HFR acquisition sequence stops by setting ChannelState=IDLE.

In case of a user break event, the BreakEffect parameter value is irrelevant. The acquisition
terminates ALWAYS at a phase —frame— boundary ensuring the integrity of the frame.

Automatic sequence stop

The sequence terminates automatically after the acquisition of the specified number of frames.
An indefinite acquisition sequence stops when the channel is forced to its inactive state.

89

Grablink Grablink Functional Guide

Use parameter SeqLength_Fr to specify the total number of frames to be acquired. The
sequence will automatically stop after the last acquired frame.

Activating the Channel

Setting ChannelState parameter to ACTIVE activates the channel and arms the trigger circuit.

The HFR acquisition sequence will effectively start after the first trigger event occurring after
channel activation when the first acquisition phase is triggered.

HFR acquisition phase

Once the HFR acquisition phase is started, the frame grabber acquires PhaseLength_Fr image
frames and stores images data into one surface.

This means that each phase is divided in slices. The interruption rate of the operating system is
then divided by the number of slice in a phase.

Stopping a PAGE acquisition sequence

The HFR acquisition mode allows two methods to stop the sequence: Manualstopand
Automaticstop.

If the number of frames to capture is known before starting the sequence and is not higher than
65,535, the automatic stop method can be used to automatically stop the sequence after a
predefined number of frames. This variant of HFR acquisition is named FiniteHFRacquisition.
In this case, the number of frames to be acquired needs to be specified before Channel
activation.

If the number of frames is unknown or higher than 65,535, the manual stop method is required.
In this case, the parameter SeqLength_Fr should be set to MC_INDETERMINATE, in order to
define an infiniteSNAPSHOTacquisitionsequence.

Manual sequence stop

The HFR acquisition sequence stops by setting ChannelState=IDLE.

In case of a user break event, the BreakEffect parameter value is irrelevant. The acquisition
terminates ALWAYS at a phase —frame— boundary ensuring the integrity of the frame.

Automatic sequence stop

The sequence terminates automatically after the acquisition of the specified number of frames.
An indefinite acquisition sequence stops when the channel is forced to its inactive state.

Use parameter SeqLength_Fr to specify the total number of frames to be acquired. The
sequence will automatically stop after the last acquired frame.

Monitoring a HFRacquisition

● The Elapsed_Fr parameter reports the number of acquired frames in the sequence.

● When the sequence length is defined (SeqLength_Fr≠MC_INDETERMINATE), Remaining_Fr

reports the number of remaining frames in the sequence.

● When the sequence contains more than 1 frame (SeqLength_Fr>1), PerSecond_Fr reports the

measured average frame rate.

90

Grablink Grablink Functional Guide

5.4. WEB Acquisition Mode

The WEB acquisition mode is intended for image acquisition of singlecontinuousobjectsofany
size.

Description

In the WEB acquisition mode, a unique acquisition sequence can be executed within the
Channel activity period.

A WEB acquisition sequence acquires SeqLength_Ln contiguous lines. It is divided in contiguous
phases, each phase acquiring PageLength_Ln lines. When SeqLength_Ln is not a multiple of
PageLength_Ln, the last phase fills partially the surface.

The sequence and the first acquisition phase are initiated according to TrigMode. Subsequent
acquisition phases are automatically initiated without any line loss.

BreakEffect specifies the behavior in case of a user break.

The WEB acquisition mode is the default mode enforced automatically by MultiCam for line-
scan cameras; it can also be explicitly invoked by assigning value WEB to AcquisitionMode.

Preparing the Channel for WEB acquisition

The first action is to define all MultiCam channel parameters before channel activation.

More specifically the following acquisition control parameters need to be configured:

Parameter

Value range

AcquisitionMode

WEB

TrigMode

BreakEffect

IMMEDIATE, HARD, SOFTand COMBINED

FINISH, ABORT

PageLength_Ln

1 ~ 65535

SeqLength_Ln

MC_INDETERMINATE and 1 ~ (PageLength_Ln×65,535)

When invoking the WEB acquisition mode:

● ActivityLength is enforced to 1. The channel automatically goes inactive at the completion of

the sequence.

● PhaseLength_Pg is enforced to 1. A single page is acquired during an acquisition phase.

● PageLength_Ln is automatically set to a working value. However, the page length can be
enforced by setting PageLength_Ln. For more information see "Setting Optimal Page
Length" on page 101.

● TrigMode establishes the starting condition of the sequence and consequently the starting

condition of the first phase of the sequence. Possible values are IMMEDIATE, HARD, SOFT and
COMBINED. The default MultiCam setting is IMMEDIATE.

91

Grablink Grablink Functional Guide

● NextTrigMode is enforced to REPEAT. This ensures that no lines are missed between

subsequent acquisition phases.

● The sequence length is specified by SeqLength_Ln. Assigning a value MC_INDETERMINATE
enforces an indefiniteWEBacquisitionsequence. Assigning any value ≥ 1 enforces a finite
WEB acquisitionsequence.

● EndTrigMode is enforced to AUTO. The sequence terminates automatically after the

acquisition of the specified number of pages. An indefinite acquisition sequence stops when
the channel is forced to its inactive state.

● BreakEffect establishes the effect of a user break on the channel. When set to FINISH, it

ensures the integrity of the last acquired phase —page— even when the user break event
occurs during its execution; this is the default value. When set to ABORT, the effect of the
user break is immediate (at line boundary); the current acquisition might be incomplete; the
portion of image already acquired is available.
When SeqLength_Ln is not a multiple of PageLength_Ln, the last acquired page is partially
filled despite the FINISH setting.

Activating the Channel

Setting ChannelState parameter to ACTIVE activates the channel and arms the trigger circuit.

The WEB acquisition sequence will start after the first trigger event occurring after channel
activation.

Starting a WEB acquisition sequence

The origin of the trigger event is determined by the trigger condition as specified by TrigMode
parameter.

Usually, the trigger event is immediate (TrigMode=IMMEDIATE) in order to start the acquisition
sequence immediately. Alternatively soft trigger (TrigMode=SOFT) or hard trigger signal
(TrigMode=HARD) or both can be selected (TrigMode=COMBINED). When hard trigger is specified,
additional parameters TrigCtl, TrigEdge, TrigFilter and TrigLine define the trigger input.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

WEB acquisition sequence

Once the WEB acquisition sequence is started, the frame grabber acquires data lines
continuously until the acquisition sequence is stopped.

The acquisition sequence is composed of one or more acquisition phases. During an acquisition
phase the frame grabber stores data lines into one destination surface.

Destination surfaces contain an arbitrary number of lines as defined by parameter PageLength_
Ln.

When a surface is filled, the acquisition continues automatically into the next available surface
without any missing lines. This process repeats until a stop condition occurs.

Every time a surface is filled, the "MC_SIG_SURFACE_FILLED" signal is reported to the user.
Parameter LineIndex reflects the number of lines already written in a partially filled surface. If
SeqLength_Ln is not a multiple of PageLength_Ln, the last surface will be partially filled. A new
surface is selected at each begin of acquisition phase.

92

Grablink Grablink Functional Guide

Lines are acquired at a rate defined by the line trigger condition. When object speed is variable,
it is convenient to generate a line trigger derived from a motion encoder.

For more information, see "Line Trigger" on page 154 and "Line-Scan Synchronization" on page
147.

Stopping a WEB acquisition sequence

The WEB acquisition mode allows two methods to stop the sequence: Manualstopand
Automaticstop.

If the scanned object has a finite length, the automatic stop method can be used to
automatically stop the sequence after a predefined number of lines. This variant of WEB
acquisition is named WEBFinite. In this case, the number of lines to be acquired needs to be
specified before Channel activation.

More often the length of the object is unknown. Therefore, the manual stop method is required.
In this case, the parameter SeqLength_Ln should be set to MC_INDETERMINATE, in order to
define an infinite WEB acquisition sequence.

Manual stop

The WEB acquisition sequence stops by setting ChannelState to IDLE.

Two flavors of stop are selectable with parameter BreakEffect:

□ When BreakEffect=FINISH, the WEB acquisition sequence stops after the completion of

the surface filling.

□ When BreakEffect=ABORT, the WEB acquisition sequence stops immediately, interrupting

also the filling of the current surface. The last surface might be partially filled.

Automatic stop

Use parameter SeqLength_Ln to specify the total number of lines to be acquired. The sequence
will automatically stop after the last acquired line. The last surface might be partially filled.

The maximum value of SeqLength_Ln is (PageLength_Ln×65,535).

Monitoring a WEB acquisition

● Elapsed_Ln reports the number of acquired lines in the sequence.

● When the sequence length is defined (SeqLength_Ln≠MC_INDETERMINATE), Remaining_Ln

reports the number of remaining lines in the sequence.

93

Grablink Grablink Functional Guide

5.5. PAGE Acquisition Mode

The PAGE acquisition mode is intended for image acquisition of multiplediscreteobjectshaving
afixedsize.

 The acquisition sequence can be configured to terminate automatically after a predefined
number of objects.

Description

In the PAGE acquisition mode, a unique acquisition sequence can be executed within the
Channel activity period.

The PAGE acquisition sequence is composed of a repetitive sequence of PAGE acquisition
phases. Each PAGE acquisition phase is responsible for the image capture of one object.

Each page is constituted of contiguous lines; the page length, expressed in lines, is specified by
PageLength_Ln.

A single sequence is capable to acquire SeqLength_Pg pages within the channel activity period.

The PAGE acquisition mode is explicitly invoked by assigning value PAGE to AcquisitionMode.

Preparing the Channel for PAGE acquisition

The first action is to define all MultiCam Channel parameters before channel activation.

More specifically the following acquisition control parameters need to be configured:

Parameter

Value range

AcquisitionMode

PAGE

TrigMode

NextTrigMode

BreakEffect

IMMEDIATE, HARD, SOFT and COMBINED

SAME, REPEAT, HARD, SOFT and COMBINED

FINISH, ABORT

PageLength_Ln

1 ~ 65535

SeqLength_Pg

MC_INDETERMINATE, 1 ~ 65535

When invoking the PAGE acquisition mode:

● The ActivityLength parameter is enforced to 1. The channel goes inactive at the completion

of the sequence.

● The PhaseLength_Pg parameter is enforced to 1. A single page is acquired during an

acquisition phase.

● The page length can be enforced by setting PageLength_Ln to any value up to 65,535. For

more information see "Setting Optimal Page Length" on page 101.

● TrigMode establishes the starting condition of the sequence and consequently the starting

condition of the first phase —the first PAGE— of the sequence. Possible values are
IMMEDIATE, HARD, SOFT and COMBINED. The default MultiCam setting is COMBINED.

94

Grablink Grablink Functional Guide

● NextTrigMode establishes the starting condition of the subsequent phases —pages— within
the sequence. Possible values areSAME, REPEAT, HARD, SOFT and COMBINED. The default
value is SAME.

● The sequence length is specified by SeqLength_Pg. Assigning a value MC_INDETERMINATE

enforces an indefinitePAGE acquisitionsequence. Assigning any value ≥ 1 enforces a finite
PAGEacquisitionsequence.

● EndTrigMode is enforced to AUTO. The sequence terminates automatically after the

acquisition of the specified number of pages. An indefinite acquisition sequence stops when
the channel is forced to its inactive state.

● BreakEffect establishes the effect of a user break on the channel. When set to FINISH, it

ensures the integrity of the last acquired phase —page—– even when the user break event
occurs during its execution; this is the default value. When set to ABORT, the effect of the
user break is immediate (at line boundary); the current acquisition might be incomplete; the
portion of image already acquired is available.

Activating the Channel

Setting ChannelState parameter to ACTIVE activates the channel and arms the trigger circuit.

The PAGE acquisition sequence will effectively start after the first trigger event occurring after
channel activation when the first acquisition phase is triggered.

Starting the first PAGE acquisition phase

The trigger source is determined by the Start Trigger condition. Usually, the start trigger event is
a hard trigger; however a soft trigger signal can also be selected. Usually, the trigger is derived
from a position detector. The trigger condition is defined by parameter TrigMode.

A programmable page delay can optionally be specified with parameter PageDelay_Ln. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

At first trigger event, the object scanning starts effectively; a "Start of Sequence" signal is
reported to the user.

Starting the subsequent PAGE acquisition phases

The trigger source for all subsequent acquisition phases is determined by the Next Start Trigger
condition. Usually, the Next start trigger event is the same as the Start Trigger condition. The
next trigger condition is defined by parameter NextTrigMode.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

A programmable page delay can optionally be specified with parameter PageDelay_Ln. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

At subsequent trigger events, the object scanning starts effectively; however no "Start of
Sequence" signal is reported to the user.

95

Grablink Grablink Functional Guide

PAGE acquisition phase

Once the PAGE acquisition phase is started, the frame grabber acquires data lines and stores
them into one surface.

Destination surface receives an arbitrary number of lines as defined by parameter PageLength_
Ln.

Parameter LineIndex reflects the number of lines already written in a partially filled surface. If
SeqLength_Ln is not a multiple of PageLength_Ln, the surface will be partially filled. A new
surface is selected at each begin of Page acquisition phase.

Lines are acquired at a rate defined by the line trigger condition. When object speed is variable,
it is convenient to generate a line trigger derived from a motion encoder.

For more information, see "Line Trigger" on page 154 and "Line-Scan Synchronization" on page
147.

Stopping a PAGE acquisition phase

Within a PAGE acquisition sequence, the acquisition phase stops automatically when the surface
is filled after the acquisition of PageLength_Ln lines.

The PAGE acquisition sequence supports objects up to highest possible value of PageLength_Ln,
namely 65,535 lines. For large objects, consider using the LONGPAGE acquisition mode.

When acquisition phase is stopped, a "Surface filled" signal is reported immediately to the user.
The "Surface filled" signal will be reported as soon as all the acquired data lines are transferred
to the host memory.

Stopping a PAGE acquisition sequence

The PAGE acquisition mode allows two methods to stop the sequence: Manualstopand
Automaticstop.

If the number of scanned objects is known before starting the sequence and is not higher than
65,535, the automatic stop method can be used to automatically stop the sequence after a
predefined number of pages. This variant of PAGE acquisition is named FinitePAGEacquisition.
In this case, the number of pages to be acquired needs to be specified before Channel
activation.

If the number of scanned objects is unknown or higher than 65,535, the manual stop method is
required. In this case, the parameter SeqLength_Pg should be set to MC_INDETERMINATE, in
order to define an infinitePAGEacquisitionsequence.

Manual sequence stop

The PAGE acquisition sequence stops by setting ChannelState=IDLE.

Two flavors are selectable with parameter BreakEffect:

□ When BreakEffect=FINISH, the PAGE acquisition sequence stops after normal completion
of the current PAGE acquisition phase if the object scanning is already started. Otherwise
the PAGE acquisition sequence stops immediately.

96

Grablink Grablink Functional Guide

□ When BreakEffect=ABORT, the PAGE acquisition sequence stops immediately, interrupting

also a PAGE acquisition phase in progress. The last object acquisition is not usable.

Automatic sequence stop

Use parameter SeqLength_Pg to specify the total number of pages to be acquired. The
sequence will automatically stop after the last acquired page.

The maximum value of SeqLength_Pg is 65,535.

Monitoring a PAGE acquisition

● Elapsed_Pg reports the number of acquired pages in the sequence.

● When the sequence length is defined (SeqLength_Pg≠MC_INDETERMINATE), Remaining_Pg

reports the number of remaining pages in the sequence.

97

Grablink Grablink Functional Guide

5.6. LONGPAGE Acquisition Mode

The LONGPAGE acquisition mode is intended for image acquisition of multiplediscreteobjects
having,possibly,avariableand/oralargersize.

Description

In the LONGPAGE acquisition mode, multiple acquisition sequences can be executed within the
Channel activity period.

The ActivityLength parameter specifies the number of sequences within the channel activity
period. Each sequence is capable to acquire SeqLength_Ln contiguous lines.

A sequence is divided in phases, each phase acquiring PageLength_Ln lines.

The LONGPAGE acquisition mode is explicitly invoked by assigning value LONGPAGE to
AcquisitionMode.

Preparing the Channel for LONGPAGE acquisition

The first action is to define all MultiCam channel parameters before channel activation.

More specifically the following acquisition control parameters need to be configured:

Parameter

Value range

AcquisitionMode

LONGPAGE

TrigMode

IMMEDIATE, HARD, SOFT and COMBINED

NextTrigMode

SAME, REPEAT, HARD, SOFT and COMBINED

EndTrigMode

AUTO, HARD

BreakEffect

FINISH, ABORT

PageLength_Ln

1 ~ 65535

SeqLength_Ln

MC_INDETERMINATE and 1 ~ (PageLength_Ln×65,535)

When invoking the LONGPAGE acquisition mode:

● The ActivityLength parameter is enforced to INDETERMINATE. The channel remains active at

the completion of the sequence.

● The PhaseLength_Pg parameter is enforced to 1. A single page is acquired during an

acquisition phase.

● The PageLength_Ln parameter is automatically set to a working value. However, the page
length can be enforced by setting PageLength_Ln. For more information see "Setting
Optimal Page Length" on page 101.

● The TrigMode parameter establishes the starting condition of the sequence and

consequently the starting condition of the first slice of the first phase of the sequence.
Possible values are IMMEDIATE, HARD, SOFT and COMBINED. The default MultiCam setting is
COMBINED.

98

Grablink Grablink Functional Guide

● The NextTrigMode parameter is enforced to REPEAT. This ensures that no lines are missed

between subsequent acquisition phases.

● The EndTrigMode parameter establishes the conditions of a sequence termination. When
EndTrigMode = AUTO, the sequence terminates automatically after the acquisition of the
specified number of frames. When EndTrigMode = HARD, the sequence terminates upon an
external End Trigger event.

● The BreakEffect parameter establishes the effect of a user break on the channel. When set to
FINISH, it ensures the integrity of the last acquired sequence —long page— even when the
user break event occurs during its execution; this is the default value. When set to ABORT,
the effect of the user break is immediate (at line boundary); the current acquisition might be
incomplete; the portion of image already acquired is available.
When SeqLength_Ln is not multiple of PageLength_Ln, the last acquired page is partially
filled despite the FINISH setting.

● The sequence length is specified by SeqLength_Ln. Assigning a value MC_INDETERMINATE

enforces an indefinite acquisition sequence.

Activating the Channel

Setting ChannelState parameter to ACTIVE activates the channel and arms the trigger circuit.

The first LONPAGE acquisition sequence will start after the first trigger event occurring after
channel activation.

Starting a LONGPAGE Acquisition Sequence

The trigger source is determined by the Start Trigger condition. Usually, the start trigger event is
a hard trigger; however a soft trigger signal can also be selected. Usually, the trigger is derived
from a position detector. The trigger condition is defined by parameter TrigMode.

At trigger event, the acquisition sequence starts effectively.

A programmable page delay can optionally be specified with parameter PageDelay_Ln. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

To summarize the usage of trigger parameters, see "Hardware Trigger" on page 127.

LONGPAGE Acquisition Sequence

Once the LONGPAGE acquisition sequence is started, the frame grabber acquires data lines
continuously until the acquisition sequence is stopped.

The acquisition sequence is composed of one or more acquisition phases. During an acquisition
phase the frame grabber stores data lines into one destination surface.

Destination surfaces contain an arbitrary number of lines as defined by parameter PageLength_
Ln.

When a surface is filled, the acquisition continues automatically into the next available surface
without any missing lines. This process repeats until a stop condition occurs.

99

Grablink Grablink Functional Guide

Every time a surface is filled, the "Surface Filled" signal is reported to the user. Parameter
LineIndex reflects the number of lines already written in a partially filled surface. If SeqLength_
Ln is not a multiple of PageLength_Ln, the last surface will be partially filled. A new surface is
selected at each begin of acquisition phase.

Lines are acquired at a rate defined by the line trigger condition. When object speed is variable,
it is convenient to generate a line trigger derived from a motion encoder.

For more information, see "Line Trigger" on page 154 and "Line-Scan Synchronization" on page
147.

Stopping a LONGPAGE Acquisition Sequence

MultiCam provides solutions for both fixed size and variable size objects.

When acquisition sequence is stopped, the "Surface Filled" signal will be reported as soon as all
the acquired data lines are transferred to the host memory. The "MC_SIG_EAS" signal is also
reported to the user.

Fixed size objects

When objects have fixed size, it is convenient to specify the number of lines to be acquired
during a LONGPAGE acquisition sequence with parameter SeqLength_Ln.

The acquisition sequence will automatically stop after the last acquired line. The last surface
might be partially filled. All following conditions must be satisfied:

□ SeqLength_Ln > 0
□ SeqLength_Ln < (PageLength×65535)
□ SeqLength_Ln < 231

This method does not require any specific signal or user actions.

Variable size objects

When objects have variable size, it is convenient to use the signal delivered by a position
detector to generate an end trigger condition.

Parameter EndTrigLine specify the input pin. Polarity and electrical style are specified by
parameters EndTrigEdge and EndTrigCtl.

In case a single signal is used to indicate the start and the stop object positions, the EndTrigLine
input pin is the same as the TrigLine input pin with opposite polarities.

A programmable page delay can optionally be specified with parameter EndPageDelay_Ln. It
compensates a trigger advance delivered by a position detector placed away from the camera
field of view.

To summarize the usage of end trigger parameters, see "Hardware End Trigger" on page 133.

Monitoring a LONGPAGE acquisition

● The Elapsed_Ln parameter reports the number of acquired lines in the sequence.

● When the sequence length is defined (SeqLength_Ln≠MC_INDETERMINATE), Remaining_Ln

reports the number of remaining lines in the sequence.

Deactivation of the Channel

The Channel is deactivated by setting ChannelState to IDLE.

100

Grablink Grablink Functional Guide

Two flavors are selectable with parameter BreakEffect:

● When BreakEffect=FINISH, the Channel deactivates after normal completion of the current
LONGPAGE acquisition sequence. If the trigger event has not yet occurred, the Channel
deactivates immediately.

● When BreakEffect=ABORT, the LONGPAGEacquisition sequence stops immediately.

NOTE
An automatic channel deactivation is not available for the
LONGPAGEacquisition mode.

5.7. Setting Optimal Page Length

In line-scan acquisition modes, the PageLength_Ln parameter specifies the number of lines to
acquire in a single surface.

Following rules and recommendation applies to determine the optimal value:

Rule 1

Current hardware implementations limit PageLength_Ln to a 16-bit value.

Rule 2

The maximum surface transition rate should not exceed 1 kHz. With
MaximumLineAcquisitionRatebeing the highest line acquisition rate of the application
expressed in Hertz:

Rule 3

The maximum number of pages per sequence is 65565:

Recommendation

For optimal usage of the on-board buffer, the amount of data in a single surface should be in
the range 1..4 Megabytes.

Therefore:

101

Grablink Grablink Functional Guide

NOTE
This recommendation becomes significant only when the average data rate
on the PCI interface is approaching the board limits.

102

Grablink Grablink Functional Guide

6. Input/Output Ports

6.1. I/O Ports Overview

6.2. I/O Functions

6.3. I/O Indices Catalog

104

109

120

103

Grablink Grablink Functional Guide

6.1. I/O Ports Overview

Applies to:

Every Channel owns a dedicated set of 10 system I/O ports including:

● 4 isolated input ports named IIN1, IIN2, IIN3, IIN4

● 2 high-speed differential input ports named DIN1, DIN2

● 4 isolated output ports named IOUT1, IOUT2, IOUT3, IOUT4

104

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Input ports

Structure

Functions

Input Function

DIN1 DIN2 IIN1 IIN2 IIN3 IIN4

"General-Purpose Inputs" on page 110

"Trigger Input" on page 112

"End Trigger Input" on page 113

"Line Trigger Input" on page 114

OK

OK

OK

OK

"Isolated I/O SyncBus Receiver" on page 117

-

OK

OK

OK

OK

-

OK

OK

OK

OK

-

OK

OK

OK

OK

-

OK

OK

OK

OK

OK

OK

OK

OK

OK

OK

NOTE
The input ports are individually designated by their I/O index. Refer to "I/O
Indices Catalog" on page 120 for a list of I/O indices for each product.

105

Grablink Grablink Functional Guide

Output Ports

Structure

Organic diagram of the output ports of a set of system I/O

The four output ports are based on a uniform structure that includes the following elements:

A programmable event signal generator composed with a set/reset flip-flop and a pair of
configurable multiplexers that selects the set and the reset conditions from a panel of internal
"events" issued by the acquisition and camera controller.

An output multiplexer that selects the signal to be issued on the output port. Possible selections
are :

□ "LOW" to connect any of the 4 output port to the logical level corresponding to the OFF

state of the opto-coupler

106

Grablink Grablink Functional Guide

□ "HIGH" to connect any of the 4 output port logical level corresponding to the OFF state of

the opto-coupler

□ "EVSIGx" to connect any of the four output port to the signal issued by the respective

event signal generator

□ "STROBE" to connect the IOUT1 port to the signal produced by the acquisition and

camera controller

□ "SB1" to connect the IOUT3 port to the first of the two signals produced by the SyncBus

transmitter of the acquisition and camera controller

□ "SB2" to connect the IOUT4 port to the second of the two signals produced by the

SyncBus transmitter of the acquisition and camera controller

● The ISO electrical interface built with an opto-coupler device.

● A readback circuit allowing getting at any time the actual logic state of the output

multiplexer.

Functions

Output Function

IOUT1 IOUT2 IOUT3 IOUT4 LED

LED_
A

LED_
B

General-purpose output

Event signaling

Strobe output

"Isolated I/O SyncBus Driver" on page
116

"Bracket LED Control" on page 119

OK

OK

OK

-

-

OK

OK

-

-

-

OK

OK

-

OK

-

OK

OK

-

OK

-

-

-

-

-

-

-

-

-

-

-

-

-

OK

OK

OK

107

Grablink Grablink Functional Guide

Selecting the output function

The output ports are primarily managed using Board class parameters belonging to the
Input/Output Control Category:

OutputConfig is a set-only collection parameter that must be used by the application software
to configure an output port for a particular usage.

OutputFunction is a get-only collection parameter that reports the actual function assigned to
the output port designated.

The following state diagram shows the 3 states of OutputFunction and all the possible inter-
state transition:

OutputFunction state diagram

The "UNKNOWN" state means that the function of the output port is not known by the
MultiCam Board object. The output port is then free to be used by a MultiCam Channel for
"Strobe Output" on page 115 or "Isolated I/O SyncBus Driver" on page 116 functions. Setting
OutputConfig to FREE forces immediately OutputFunction to the value UNKNOWN. This is the
default state after board startup.

The "SOFT" state means that the output port is directly under control of the application
software for general-purpose usage. Setting OutputConfig to SOFT forces immediately
OutputFunction to the value SOFT. The output port can be used by the MultiCam Board for
"General-Purpose Output" on page 111 function.

The "EVENT" state means that the output port is driven by the respective event signal generator
for the event signaling usage. Setting OutputConfig to EVENT forces immediately
OutputFunction to the value EVENT. The output port can be used by the MultiCam Board for
"Event Signaling" on page 118 function.

NOTE
The output ports are individually designated by their I/O index. Refer to "I/O
Indices Catalog" on page 120 for a list of I/O indices for each product.

108

Grablink Grablink Functional Guide

6.2. I/O Functions

General-Purpose Inputs

General-Purpose Output

Trigger Input

End Trigger Input

Line Trigger Input

Strobe Output

Isolated I/O SyncBus Driver

Isolated I/O SyncBus Receiver

Event Signaling

Bracket LED Control

110

111

112

113

114

115

116

117

118

119

109

Grablink Grablink Functional Guide

General-Purpose Inputs

All the system I/O input port can be used as a general-purpose digital input port.

For that usage, use the MultiCam Board parameters belonging to the Input/Output Control
Category.

Prior to the first attempt to get the state of an input port, it is mandatory to configure the port
for that usage by assigning the value SOFT to the corresponding member of the collection
parameter InputConfig.

When configured for general-purpose input usage, the corresponding member of the get only
InputFunction parameter reports the value SOFT.

The digital state of the input port can then be read at any time by getting the value of the
corresponding member of the InputState parameter.

110

Grablink Grablink Functional Guide

General-Purpose Output

When configured for general-purpose usage, the output multiplexer is restricted to two
positions: LOW and HIGH .

The following state diagram shows the 2 states and all the possible inter-state transition:

Output multiplexer state diagram (OutputFunction = SOFT)

The position of the output multiplexer is controlled by means of OutputState, a Board class
MultiCam parameter belonging to the Input/Output Control Category .

The "LOW" state means that the output multiplexer is in the "LOW" position. Setting
OutputState to LOW forces immediately the output multiplexer to the "LOW state".

The "HIGH" state means that the output multiplexer is in the "HIGH" position. Setting
OutputState to HIGH forces immediately the output multiplexer to the "HIGH state".

Setting OutputState to TOGGLE forces immediately the output multiplexer to change its
position from LOW to HIGH, if it was at the LOW position or vice-versa.

111

Grablink Grablink Functional Guide

Trigger Input

Applies to:

For applications using a hardware acquisition trigger, anyone of the system I/O input ports can
be selected as the source for the trigger control circuit of the acquisition channel.

For that usage, use the parameters belonging to the Channel Trigger Control Category .

The trigger source signal can originate from the following type of devices:

● Through a single high-speed differential input port driven by an RS-422 compatible detector.

● Through a single isolated current-sense input port driven by a detector that is not RS-422

compliant.

The selection of the port is primarily based on the electrical style of the sensor device used as
trigger source.

Possible ports assignments

Sourcing device type

Default port(s) assignment

Alternate port assignment(s)

RS-422 compatible detector High-speed diff. input #2

High-speed diff. input #1

Other detectors

Isolated input #2

Isolated input #1
Isolated input #3
Isolated input #4

NOTE
The default port assignment for both electrical styles is different than the
one of the line trigger (at least when a single signal is used).

See also: "Hardware Trigger" on page 127

112

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

End Trigger Input

Applies to:

For applications using a hardware acquisition end trigger, anyone of the system I/O input ports
can be selected as the source for the end trigger control circuit of the acquisition channel.

For that usage, use the parameters belonging to the Trigger Control Category .

The end trigger source signal can originate from the following type of devices:

● Through a single high-speed differential input port driven by an RS-422 compatible detector.

● Through a single isolated current-sense input port driven by a detector that is not RS-422

compliant.

The selection of the port is primarily based on the electrical style of the sensor device used as
End Trigger source.

Possible ports assignments

Sourcing device type

Default port(s) assignment

Alternate port assignment(s)

RS-422 compatible detector High-speed diff. input #2

High-speed diff. input #1

Other detectors

Isolated input #2

Isolated input #1
Isolated input #3
Isolated input #4

NOTE
Any input port and hence any electrical style can be specified for that
function.

NOTE
The default port assignment is the same as the one for the trigger. This
corresponds to the case where a single signal is used for both functions, one
edge being the trigger, the opposite edge being the end trigger.

See also: "Hardware End Trigger" on page 133

113

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Line Trigger Input

Applies to:

For line-scan applications using a hardware acquisition line trigger, anyone or some pairs of the
system I/O input ports can be selected as the source(s) for the line trigger control circuit of the
acquisition channel.

For that usage, use the parameters belonging to the Encoder Control Category .

The line trigger signal can be elaborated from one or two external signals provided by one of
the following type of devices:

● Through one pair of high-speed differential input ports driven by an RS-422 compatible dual

output phase quadrature incremental motion encoder.

● Through one pair of isolated current-sense input ports driven by a dual output phase

quadrature incremental motion encoder that is not RS-422 compliant.

● Through a single high-speed differential input port driven by an RS-422 compatible single
output incremental motion encoder or a similar device. This can be a dual output type of
incremental motion encoder for which only one output signal is connected.

● Through a single isolated current-sense input port driven by an incremental motion encoder

or a similar device that is not RS-422 compliant.

Possible ports assignments for each case

Sourcing device type

Default port(s)
assignment

Alternate port
assignment(s)

RS-422 compatible dual output phase quadrature
incremental motion encoder

High-speed diff.
inputs #1 and #2

-

Other dual output phase quadrature incremental
motion encoder

Isolated input #1
and #2

Isolated input #3
and #4

RS-422 compatible single output incremental
motion encoder or similar device

High-speed diff.
input #1

High-speed diff.
input #2

Single output incremental motion encoder or
similar device

Isolated input #1

Isolated input #2
Isolated input #3
Isolated input #4

NOTE
Any input port and hence any electrical style can be specified for that
function.

NOTE
The default port assignment for both single signal electrical styles is
different of the one of the trigger.

See also: "Line Trigger" on page 154

114

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Strobe Output

When the Board class OutputFunction parameter is UNKNOWN, the output multiplexer of IOUT1
is under control of the Channel class StrobeMode parameter.

Output multiplexer state diagram (OutputFunction = UNKNOWN)

The output multiplexer is reconfigured at channel activation (SCA event) and deactivation (ECA
event) according to the value of StrobeMode:

● When StrobeMode = AUTO or MAN, the output multiplexer is configured as follows:

□ At channel activation, it is forced to the STROBE position, allowing the STROBE signal to

drive the opto-coupler of the output port.

□ At channel deactivation, it is forced to the LOW position, turning off the opto-coupler and

preventing any strobe pulses to occur while the channel is deactivated.

● When StrobeMode = OFF, the output multiplexer is configured at channel activation to the

LOW position, turning off the opto-coupler and preventing any strobe pulses to occur during
channel activity. The OFF state persists after channel deactivation.

When StrobeMode = NONE, the output multiplexer is left unchanged.

See also: "Strobe Control" on page 142

115

Grablink Grablink Functional Guide

Isolated I/O SyncBus Driver

Applies to:

When the Board class OutputFunction parameter is UNKNOWN, the output multiplexer of IOUT3
and IOUT4 are under control of the Channel class SynchronizedAcquisition parameter.

The output multiplexer is reconfigured at channel activation (SCA event) according to the value
of SynchronizedAcquisition :

● When SynchronizedAcquisition = MASTER or LOCAL_MASTER, it is forced to the SYNCBUS

position, allowing the respective signal of the SyncBus transmitter to drive the output port.

● For other values of SynchronizedAcquisition, the output multiplexer is left unchanged.

At channel deactivation, the output multiplexer remains unchanged.

See also: "Synchronized Line-scan Acquisition" on page 165 and "Two-line Synchronized Line-
scan Acquisition" on page 228

116

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Isolated I/O SyncBus Receiver

Applies to:

For applications using the synchronized acquisition feature, IIN3 and IIN4 ports can be used for
the SyncBus receiver. For that usage, use the SynchronizedAcquisition parameter.

See also: "Synchronized Line-scan Acquisition" on page 165 and "Two-line Synchronized Line-
scan Acquisition" on page 228

117

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Event Signaling

Applies to:

When the Board class OutputFunction parameter is set to EVENT, the output multiplexer selects
the output of the event signal generator.

The event signal generator is configured by means of Board class SetSignal and ResetSignal
parameters.

SetSignal is a collection parameter that configures the multiplexer on the set branch of the SR
flip-flop.

ResetSignal is a collection parameter that configures the multiplexer on the reset branch of the
SR flip-flop.

All the multiplexers of the event signal generator exhibit the same set of events sources.

● Start and end of : channel activity, acquisition phase, acquisition sequence

● Rising and falling edges of Camera Link downstream control signals: FVAL, LVAl, DVAL

● Rising and falling edges of Camera Link upstream control signals: CC1, CC2, CC3, CC4

In addition, the multiplexer can be set to the position NONE ensuring that no more event are
considered.

NOTE
When the set and reset condition are identical, the SR flip/flop toggles at
every event.

118

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Bracket LED Control

Applies to:
The application can turn ON and OFF the bracket LED's to identify a card in a PC using the I/O
control parameters OutputConfig and OutputState of the Board object with the following I/O
indices:
Product

PinName

Index

LED

LED_A

LED_B

LED

LED

25

27

28

25

25

119

BaseDualBaseFullFullXRBaseDualBaseFullFullXRGrablink Grablink Functional Guide

6.3. I/O Indices Catalog

I/O indices for input lines

Index

ConnectorName InputPinName

InputStyle

1

2

3

4

28

29

17

18

21

22

23

24

SYSTEM

Enhanced_I01

SYSTEM

Enhanced_I02

SYSTEM

Enhanced_I03

SYSTEM

Enhanced_I04

TTL

TTL

TTL

TTL

SYSTEM

SYSTEM

SYSTEM

SYSTEM

CAMERA

CAMERA

CAMERA

CAMERA

ISOA1

ISOA2

TRA1

TRA2

LVAL

FVAL

DVAL

ITTL, I12V

TTL, I12V

LVDS

LVDS

CHANNELLINK

CHANNELLINK

CHANNELLINK

SPARE

CHANNELLINK

I/O indices for output lines

Index

ConnectorName OutputPinName OutputStyle

1

2

3

4

28

29

17

21

22

23

24

51

52

SYSTEM

SYSTEM

SYSTEM

SYSTEM

SYSTEM

SYSTEM

SYSTEM

CAMERA

CAMERA

CAMERA

CAMERA

LED

LED

Enhanced_I01

Enhanced_I02

Enhanced_I03

Enhanced_I04

TTL

TTL

TTL

TTL

ISOA1

ISOA2

ITTL, IOC, IOE

ITTL, IOC, IOE

STA

CC1

CC2

CC3

CC4

RED

GREEN

OPTO

LVDS

LVDS

LVDS

LVDS

TTL

TTL

120

ExpressEOSExpressEOSGrablink Grablink Functional Guide

I/O indices for input lines

Index

ConnectorName InputPinName

InputStyle

1

2

3

4

5

6

7

8

9

10

11

23

24

IO

IO

IO

IO

IO

IO

CAMERA

CAMERA

CAMERA

CAMERA

IIN1

IIN2

IIN3

IIN4

DIN1

DIN2

LVAL

FVAL

DVAL

ISO

ISO

ISO

ISO

DIFF

DIFF

CHANNELLINK

CHANNELLINK

CHANNELLINK

SPARE

CHANNELLINK

CAMERA

CK_PRESENT

CHANNELLINK

IO

IO

POWER_5V

POWERSTATE5V

POWER_12V

POWERSTATE12V

I/O indices for output lines

Index

ConnectorName OutputPinName

OutputStyle

1

2

3

4

7

8

9

10

25

IO

IO

IO

IO

CAMERA

CAMERA

CAMERA

CAMERA

BRACKET

IOUT1

IOUT2

IOUT3

IOUT4

CC1

CC2

CC3

CC4

LED

ISO

ISO

ISO

ISO

CHANNELLINK

CHANNELLINK

CHANNELLINK

CHANNELLINK

NA

121

BaseBaseGrablink Grablink Functional Guide

I/O indices for input lines

Index

ConnectorName InputPinName

InputStyle

1

2

3

4

5

6

7

8

9

10

11

12

13

14

15

16

17

18

19

20

21

22

23

24

25

26

IO_A

IO_A

IO_A

IO_A

IO_A

IO_A

CAMERA_A

CAMERA_A

CAMERA_A

IIN1

IIN2

IIN3

IIN4

DIN1

DIN2

LVAL

FVAL

DVAL

ISO

ISO

ISO

ISO

DIFF

DIFF

CHANNELLINK

CHANNELLINK

CHANNELLINK

CAMERA_A

SPARE

CHANNELLINK

CAMERA_A

CK_PRESENT

CHANNELLINK

IO_B

IO_B

IO_B

IO_B

IO_B

IO_B

CAMERA_B

CAMERA_B

CAMERA_B

IIN1

IIN2

IIN3

IIN4

DIN1

DIN2

LVAL

FVAL

DVAL

ISO

ISO

ISO

ISO

DIFF

DIFF

CHANNELLINK

CHANNELLINK

CHANNELLINK

CAMERA_B

SPARE

CHANNELLINK

CAMERA_B

CK_PRESENT

CHANNELLINK

IO_A

IO_A

IO_B

IO_B

POWER_5V

POWERSTATE5V

POWER_12V

POWERSTATE12V

POWER_5V

POWERSTATE5V

POWER_12V

POWERSTATE12V

122

DualBaseGrablink Grablink Functional Guide

I/O indices for output lines

Index

ConnectorName OutputPinName

OutputStyle

1

2

3

4

7

8

9

10

12

13

14

15

18

19

20

21

22

28

IO_A

IO_A

IO_A

IO_A

CAMERA_A

CAMERA_A

CAMERA_A

CAMERA_A

IO_B

IO_B

IO_B

IO_B

CAMERA_B

CAMERA_B

CAMERA_B

CAMERA_B

BRACKET

BRACKET

IOUT1

IOUT2

IOUT3

IOUT4

CC1

CC2

CC3

CC4

IOUT1

IOUT2

IOUT3

IOUT4

CC1

CC2

CC3

CC4

LED_A

LED_B

ISO

ISO

ISO

ISO

CHANNELLINK

CHANNELLINK

CHANNELLINK

CHANNELLINK

ISO

ISO

ISO

ISO

CHANNELLINK

CHANNELLINK

CHANNELLINK

CHANNELLINK

NA

NA

123

DualBaseGrablink Grablink Functional Guide

I/O indices for input lines

Index

ConnectorName

InputPinName

InputStyle

1

2

3

4

5

6

7

8

9

10

11

12

13

14

15

16

17

18

19

20

21

23

24

IO

IO

IO

IO

IO

IO

IIN1

IIN2

IIN3

IIN4

DIN1

DIN2

ISO

ISO

ISO

ISO

DIFF

DIFF

CAMERA

CAMERA

CAMERA

CAMERA

LVAL_X

FVAL_X

DVAL_X

CHANNELLINK

CHANNELLINK

CHANNELLINK

SPARE_X

CHANNELLINK

CAMERA

CK_PRESENT_X

CHANNELLINK

CAMERA

CAMERA

CAMERA

CAMERA

LVAL_Y

FVAL_Y

DVAL_Y

CHANNELLINK

CHANNELLINK

CHANNELLINK

SPARE_Y

CHANNELLINK

CAMERA

CK_PRESENT_Y

CHANNELLINK

CAMERA

CAMERA

CAMERA

CAMERA

LVAL_Z

FVAL_Z

DVAL_Z

CHANNELLINK

CHANNELLINK

CHANNELLINK

SPARE_Z

CHANNELLINK

CAMERA

CK_PRESENT_Z

CHANNELLINK

IO

IO

POWER_5V

POWERSTATE5V

POWER_12V

POWERSTATE12V

NOTE
The I/O indices 0 and 22 have no input-related function.

124

FullFullXRGrablink Grablink Functional Guide

I/O indices for output lines

Index

ConnectorName OutputPinName

OutputStyle

1

2

3

4

7

8

9

10

25

IO

IO

IO

IO

CAMERA

CAMERA

CAMERA

CAMERA

BRACKET

IOUT1

IOUT2

IOUT3

IOUT4

CC1

CC2

CC3

CC4

LED

ISO

ISO

ISO

ISO

CHANNELLINK

CHANNELLINK

CHANNELLINK

CHANNELLINK

NA

NOTE
The I/O indices 0, 5, 6, and {11 24} have no output-related function.

125

FullFullXRGrablink Grablink Functional Guide

7. Triggers

7.1. Hardware Trigger

7.2. Hardware End Trigger

127

133

126

Grablink Grablink Functional Guide

7.1. Hardware Trigger

About hardware trigger event sources

When the frame grabber is configured for area-scan acquisition using the SNAPSHOT or the HFR
acquisition modes, a (frame) trigger is an electrical signal sent by the external system to
instruct the frame grabber to take control over the camera, including exposure control, and to
perform a frame acquisition. This is usually used when an asynchronous capture of a moving
object is involved. The trigger pulse is issued by a position sensor indicating when the observed
object is adequately located in the field of view.

When the frame grabber is configured for line-scan acquisition using the WEB, PAGE or the
LONGPAGE acquisition modes, a (page) trigger is an electrical signal sent by the external system
to instruct the frame grabber to perform the acquisition of a set of several successive lines. This
is usually used when a moving object is about to enter the field of view of the line-scan camera.

Each MultiCam channel elaborates a clean trigger event using a dedicated set of hardware
resources including: source multiplexer, edge detector, noise filter, delay line and decimation
filter.

The hardware trigger input function is available for a restricted set of AcquisitionMode,
TrigMode and NextTrigMode acquisition control parameters. In the following table, a
OK indicates that the hardware trigger input function is effectively used:

Initial Trigger Event

Subsequent Trigger Events

TrigMode

IMMEDIATE

HARD

SOFT

COMBINED

NextTrigMode

HARD

SOFT

COMBINED

SAME

REPEAT

OK

OK

OK

OK

-

OK

-

OK

OK

OK

OK

OK

-

OK

-

OK

-

OK

-

OK

Preparing the Channel for hardware triggering

When hardware trigger is required, the following trigger control parameters need to be
configured:

Parameter

Value range

TrigCtl

TrigEdge

TrigFilter

TrigDelay_us

PageDelay_Ln

See "Source Selection and Electrical Style Control" on page 128

GOHIGH, GOLOW See "Polarity Control" on page 130

See "Filter Control" on page 130

See "Delay Control" on page 131

TrigLine

See "Source Selection and Electrical Style Control" on page 128

127

Grablink Grablink Functional Guide

Parameter

Value range

TrigDelay_Pls

NextTrigDelay_Pls

See "Decimation Control" on page 132

Source Selection and Electrical Style Control

Source Selection and Electrical Style Control

Applies to:

The trigger signal can originate from the following type of devices:

1. A TTL compatible detector attached to any of the four Enhanced I/O ports or any of the two

Isolated I/O ports

2. A 12V CMOS compatible device attached to any of the two Isolated I/O ports

3. A LVDS or RS-422 compatible detector attached to any of the two differential input ports

Sourcing device type TrigCtl

Input Port

TrigLine

Enhanced IO1

NOM or IO1

Enhanced IO2

Enhanced IO3

Enhanced IO4

IO2

IO3

IO4

IsoA1

IsoA2

TRA1

TRA2

NOM or ISOA1

ISOA2

NOM or TRA1

TRA2

TTL

TTL and 12V CMOS

TTL

ITTL
I12V

LVDS or RS-422

LVDS

To select a port:

1. Set the value of the TrigCtl parameter corresponding to the electrical style of the sensor

device used as trigger source.

2. Optionally, set the value of the TrigLine parameter corresponding to the I/O port used to

attach the trigger detector.

NOTE
The default value of TrigLine is NOM.

The hardware trigger input ports are available on the (External) System Connector and on the
Internal System Connector

128

Grablink Grablink Functional Guide

Applies to:

The trigger signal can originate from the following type of devices:

3. An RS-422 compatible detector attached to any of the two high-speed differential input ports

belonging to the channel or ...

4.

... another type of device attached to any of the 4 isolated current-sense input ports
belonging to the channel.

Sourcing device type TrigCtl

Input Port

TrigLine

RS-422

DIFF

Other detectors

ISO

Diff. Input #1

DIN1

Diff. Input #2

NOM or DIN2

Isolated input #1

IIN1

Isolated input #2 NOM or IIN2

Isolated input #3

Isolated input #4

IIN3

IIN4

To select a port:

1. Set the value of the TrigCtl parameter corresponding to the electrical style of the sensor

device used as trigger source.

2. Optionally, set the value of the TrigLine parameter corresponding to the I/O port used to

attach the trigger detector.

NOTE
The default value of TrigLine is NOM.

The hardware trigger input ports are available on Internal IO and External IO connectors:

Product

Camera

Connector(s)

-

A

B

-

Internal I/O
External I/O

Channel A Internal
I/O

External I/O

Channel B Internal
I/O

External I/O

Internal I/O
External I/O

129

Trigger Input
Ports

DIN1, DIN2
IIN1, IIN2,
IIN3, IIN4

DIN1A, DIN2A
IIN1A, IIN2A,
IIN3A, IIN4A

DIN1A, DIN2A
IIN1A, IIN2A

DIN1B, DIN2B
IIN1B, IIN2B,
IIN3B, IIN4B

DIN1B, DIN2B
IIN1B, IIN2B

DIN1, DIN2
IIN1, IIN2,
IIN3, IIN4

BaseDualBaseFullFullXRBaseDualBaseDualBaseFullFullXRGrablink Grablink Functional Guide

Polarity Control

A trigger event is generated on a positive-going or a negative-going transition of the electrical
signal.

To select the transition, set accordingly the value of the TrigEdge parameter:

TrigEdge

Description

GOHIGH

GOLOW

The trigger event is generated at each positive-going transition of the trigger
line

The trigger event is generated at each negative-going transition of the trigger
line

The default value for TrigEdge is GOHIGH.

Filter Control

The hardware signal flows through a digital filter that removes any pulse narrower than its time
constant.

The filter strength is configurable in 3 steps by means of the TrigFilter parameter. Each step
corresponds to a specific filter time constant.

TrigFilter

Time Constant

OFF

100 nanoseconds

ON or MEDIUM 500 nanoseconds

STRONG

2.5 microseconds (Default value)

TIP
To avoid unexpected loss of trigger events, check that the selected time
constant is shorter than the trigger pulse width sent by the detector!

Product specific notes

Product

Description

The value OFF is not allowed for isolated ports IsoA1 and IsoA2

There is no digital filter for isolated inputs.

130

ExpressEOSBaseDualBaseFullFullXRGrablink Grablink Functional Guide

Delay Control

For area-scan cameras operated with the SNAPSHOT or the HFR acquisition modes, the
hardware frame trigger signal can be delayed by a user-programmable time delay.

For (TDI) line-scan cameras operated with the PAGE or the LONGPAGE acquisition modes, the
hardware page trigger signal can be delayed by a user-programmable number of captured lines.

The following table shows the respective control parameters, their value range and the default
value:

AcquisitionMode

Parameter

Value Range

Default Value

SNAPSHOT or HFR

TrigDelay_us 0 to 2,000,000 (2 seconds)

PAGE or LONGPAGE PageDelay_Ln

0 to 65,534 lines

0

0

NOTE
The "number of captured lines" is equal to the "number of camera cycles"
when the frame grabber is configured to capture all lines. When the frame
grabber performs "downweb resampling", the "number of captured lines"
might be different than the "number of camera cycles".

NOTE
When the downweb line rate is linked to the web speed through an encoder,
the delay expressed as a number of captured lines represents a fixed length
on the web!

131

Grablink Grablink Functional Guide

Decimation Control

Applies to:

The trigger decimation feature applies:

● when a hardware frame trigger source is selected for the area-scan SNAPSHOT or HFR

acquisition modes.

● When a hardware page trigger source is selected for the (TDI) line-scan WEB, PAGE or

LONGPAGE acquisition modes.

Trigger decimation discards a configurable number of trigger events after the start of
acquisition sequence and after every start of acquisition phase.

TrigDelay_Pls specifies the number of detected pulses on the hardware trigger line to be
skipped after the acquisition sequence begins.

NextTrigDelay_Pls specifies the number of detected pulses on the hardware trigger or page
trigger line to be skipped between successive acquisition phases.

Parameter

Value Range

Default Value

TrigDelay_Pls

0 to 65,536 pulses

NextTrigDelay_Pls 0 to 65,536 pulses

0

0

Acquisition rate and trigger decimation, with TrigDelay_Pls = 4 and NextTrigDelay_Pls = 3

WARNING
NextTrigDelay_Pls is irrelevant for the LONGPAGE acquisition mode.

132

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

7.2. Hardware End Trigger

About End Trigger Input

When the frame grabber is configured for line-scan acquisition using the LONGPAGE acquisition
mode, a end trigger is an electrical signal sent by the external system to instruct the frame
grabber to stop the acquisition of a set of several successive lines.

Each MultiCam channel elaborates a clean end trigger event using a dedicated set of hardware
resources including: source multiplexer, edge detector, noise filter and delay line.

The hardware end trigger input function is only available when EndTrigMode parameter is set to
HARD.

Preparing the Channel for hardware triggering

When hardware trigger is required, the following trigger control parameters need to be
configured:

Parameter

EndTrigCtl

Value range

See "Source Selection and Electrical Style Control " on page 134

EndTrigEdge

GOHIGH, GOLOW See "Polarity Control" on page 136

EndTrigFilter

See "Filter Control" on page 137

EndTrigEffect

FOLLOWINGLINE, PRECEDINGLINE See "Effect Control" on page 137

EndPageDelay_Ln

See "Delay Control" on page 139

EndTrigLine

See "Source Selection and Electrical Style Control " on page 134

133

Grablink Grablink Functional Guide

Source Selection and Electrical Style Control

Source Selection and Electrical Style Control

Applies to:

The end trigger signal can originate from the following type of devices:

1. A TTL compatible detector attached to any of the four Enhanced I/O ports or any of the two

Isolated I/O ports

2. A 12V CMOS compatible device attached to any of the two Isolated I/O ports

3. A LVDS or RS-422 compatible detector attached to any of the two differential input ports

Sourcing device type EndTrigCtl

Input Port

EndTrigLine

TTL

TTL and 12V CMOS

TTL

ITTL
I12V

LVDS or RS-422

LVDS

To select a port:

Enhanced IO1

NOM or IO1

Enhanced IO2

Enhanced IO3

Enhanced IO4

IO2

IO3

IO4

IsoA1

IsoA2

TRA1

TRA2

NOM or ISOA1

ISOA2

NOM or TRA1

TRA2

1. Set the value of the EndTrigCtl parameter corresponding to the electrical style of the sensor

device used as trigger source.

2. Optionally, set the value of the EndTrigLine parameter corresponding to the I/O port used to

attach the trigger detector.

NOTE
The default value of EndTrigLine is NOM.

The hardware trigger input ports are available on the (External) System Connector and on the
Internal System Connector

134

Grablink Grablink Functional Guide

Source Selection and Electrical Style Control

Applies to:

The end trigger signal can originate from the following type of devices:

1. An RS-422 compatible detector attached to any of the two high-speed differential input ports

belonging to the channel or ...

2.

... another type of device attached to any of the 4 isolated current-sense input ports
belonging to the channel.

Sourcing device type EndTrigCtl

Input Port

EndTrigLine

RS-422 compatible
detector

Diff. Input #2

DIFF

Diff. Input #1

Diff. Input #2

NOM

DIN1

DIN2

Isolated input #2

NOM

Other detectors

ISO

Isolated input #2

Isolated input #1

Isolated input #3

Isolated input #4

IIN1

IIN2

IIN3

IIN4

To select a port:

1. Set the value of the EndTrigCtl parameter corresponding to the electrical style of the sensor

device used as end trigger source.

2. Optionally, set the value of the EndTrigLine parameter corresponding to the I/O port used to

attach the end trigger detector.

NOTE
The default value of EndTrigLine is NOM.

The hardware end trigger input ports are available on Internal IO and External IO connectors:

Product

Camera

Connector(s)

-

A

B

Internal I/O Connector
External I/O Connector

Channel A Internal I/O
Connector

External I/O Connector

Channel B Internal I/O
Connector

End Trigger
Input Ports

DIN1, DIN2
IIN1, IIN2,
IIN3, IIN4

DIN1A, DIN2A
IIN1A, IIN2A,
IIN3A, IIN4A

DIN1A, DIN2A
IIN1A, IIN2A

DIN1B, DIN2B
IIN1B, IIN2B,
IIN3B, IIN4B

External I/O Connector

DIN1B, DIN2B

135

BaseDualBaseFullFullXRBaseDualBaseDualBaseGrablink Grablink Functional Guide

Product

Camera

Connector(s)

-

Internal I/O Connector
External I/O Connector

End Trigger
Input Ports

IIN1B, IIN2B

DIN1, DIN2
IIN1, IIN2,
IIN3, IIN4

Polarity Control

A end trigger event is generated on a positive-going or a negative-going transition of the
electrical signal.

To select the transition, set accordingly the value of the EndTrigEdge parameter:

EndTrigEdge

Description

GOHIGH

GOLOW

The end trigger event is generated at each positive-going transition of the
trigger line

The end trigger event is generated at each negative-going transition of the
trigger line

The default value for EndTrigEdge is GOLOW.

136

FullFullXRGrablink Grablink Functional Guide

Filter Control

Applies to:

The hardware signal issued from differential inputs only flows through a digital filter that
removes any pulse narrower than its time constant.

The filter strength is configurable in 3 steps by means of the EndTrigFilter parameter. Each step
corresponds to a specific filter time constant.

EndTrigFilter

Time Constant

OFF

100 nanoseconds

ON or MEDIUM

500 nanoseconds

STRONG

2.5 microseconds (Default value)

TIP
To avoid unexpected loss of end trigger events, check that the selected time
constant is shorter than the end trigger pulse width sent by the detector!

Product specific notes

Product

Description

The value OFF is not allowed for isolated ports IsoA1 and IsoA2

There is no digital filter for isolated inputs.

Effect Control

For (TDI) line-scan cameras operated with the LONGPAGE acquisition mode, two variants are
selectable with the parameter EndTrigEffect:

● When EndTrigEffect=FOLLOWINGLINE, on reception of an "End Trigger" event, the MultiCam

Acquisition Controller acquires the line following the "End Trigger" event then terminates the
acquisition phase.

137

BaseDualBaseFullFullXRExpressEOSBaseDualBaseFullFullXRGrablink Grablink Functional Guide

Terminates after the following line

● When EndTrigEffect=PRECEDINGLINE, on reception of an "End Trigger" event, the MultiCam

Acquisition Controller acquires the line preceding the "End Trigger" event and terminates the
acquisition phase immediately.

Terminates immediately

NOTE
The PRECEDINGLINE value is not allowed for Bayer bi-linear line-scan
cameras.

138

« End Trigger »  event« Beginning Of Line » eventsEndTrigEffect = FOLLOWINGLINEAcquired Data« End of Acquisition Phase » event « End Trigger »  event« Beginning Of Line » eventsEndTrigEffect = PRECEDINGLINEAcquired Data« End of Acquisition Phase » event Grablink Grablink Functional Guide

Delay Control

For (TDI) line-scan cameras operated with the LONGPAGE acquisition mode, the hardware page
end trigger signal can be delayed by a user-programmable number of captured lines.

The following table shows the control parameter, its value range and its default value:

AcquisitionMode

Parameter

Value Range

Default Value

LONGPAGE

EndPageDelay_Ln 0 to 65,534 lines

0

NOTE
The "number of captured lines" is equal to the "number of camera cycles"
when the frame grabber is configured to capture all lines. When the frame
grabber performs "downweb resampling", the "number of captured lines"
might be different than the "number of camera cycles".

NOTE
When the downweb line rate is linked to the web speed through an encoder,
the delay expressed as a number of captured lines represents a fixed length
on the web.!

139

Grablink Grablink Functional Guide

8. Exposure Control

Grabber-Controlled Exposure

This board supports the following grabber-controlled exposure class of cameras:

Imaging

CamConfig

Camera operation class

AREA

LINE

LINE

LINE

TDI

PxxRG

Asynchronous progressive scan, grabber-controlled exposure

LxxxxRG

Grabber-controlled line-scanning, grabber controlled exposure

LxxxxRG2

LxxxxRP

LxxxxRP

Grabber-controlled line-scanning, grabber controlled exposure,
dual signal

Grabber-controlled line-scanning, permanent exposure

Grabber-controlled line-scanning, permanent exposure

If the camera belongs to the grabber-controlled exposure class, you can specify the desired
exposure time through parameters Expose_us and optionally ExposeTrim. This board provides
an exposure time range of 1 µs up to 5 s.

The lower limit of the exposure time range is defined by the camera parameter ExposeMin_us.
The upper limit of usable exposure time range is defined by the camera parameter ExposeMax_
us.

NOTE
Both parameters prevent using the camera outside the permitted range.

The grabber controls the exposure time through the reset line and optionally —in case of
LxxxxRG2 operation mode— through the auxiliary reset line.

Any of the four Camera Link upstream control line can be used for these purposes with
independent polarity control. MultiCam selects automatically the exposure control line(s)
according to the value of camera parameters CC1Usage, CC2Usage, CC3Usage and CC4Usage; it
configures automatically the polarity according to camera parameters ResetEdge and
AuxResetEdge.

140

Grablink Grablink Functional Guide

Uncontrolled Exposure

This board supports the following grabber-controlled exposure class of cameras:

Imaging

CamConfig

Camera operation class

AREA

LINE

PxxSC

PxxRC

LxxxxSP

LxxxxSC

LxxxxRC

Synchronous progressive scan, camera-controlled exposure

Asynchronous progressive scan, camera-controlled exposure

Free-running, permanent exposure

Free-running, camera-controlled exposure

Grabber-controlled line scanning, camera controlled exposure

If the camera belongs to the camera-controlled exposure class, you can specify the actual
exposure time through parameter TrueExp_us. This board provides an exposure time range of
1 microsecond up to 20 s.

141

Grablink Grablink Functional Guide

9. Strobe Control

Each MultiCam acquisition channel embeds one strobe controller capable of delivering one
strobe signal.

Function Availability

The strobe function is only available for these camera operation modes:

Area-scan cameras (Imaging = AREA)

Short name

Camera operation class

PxxRC

PxxRG

Asynchronous progressive scan, camera-controlled exposure

Asynchronous progressive scan, grabber-controlled exposure

Line-scan cameras (Imaging = LINE)

Short name

Camera operation class

LxxxxRC

Grabber-controlled line scanning, camera controlled exposure

LxxxxRG

Grabber-controlled line-scanning, grabber controlled exposure

LxxxxRG2

Grabber-controlled line-scanning, grabber controlled exposure, dual signal

LxxxxRP

Grabber-controlled line-scanning, permanent exposure

TDI line-scan cameras (Imaging = TDI)

Short name

Camera operation class

LxxxxRP

Grabber-controlled line-scanning, permanent exposure

NOTE
The strobe function is inoperative for PxxSC, LxxxxSC, and LxxxxSP camera
operation modes; therefore, the StrobeMode parameter is filtered in
MultiCam Studio for these classes of cameras.

142

Grablink Grablink Functional Guide

Mode Control

The main control parameter is StrobeMode:

StrobeMode

Description

NONE

MAN

AUTO

OFF

The strobe function is disabled. No strobe line is allocated to the channel.

The strobe function is enabled with a manual timing control feature.

The strobe function is enabled with an automatic timing control feature.

The designated strobe line is set to the inactive level; no more strobe pulses
are issued.

The default and allowable values of are depending on the camera camera operation mode:

StrobeMode

Camera operation modes

NONE

MAN

AUTO OFF

Grabber-controlledrateandexposurePxxRG,
LxxxxRG, LxxxxRG2

Grabber-controlledrate,permanentexposure
LxxxxRP

Grabber-controlledrate,cameracontrolled
exposure
PxxRC, LxxxxRC

OK

OK

-

-

Default OK

Default OK

OK

Default

-

OK

143

Grablink Grablink Functional Guide

Duration and Position Controls

Parameter Value Range Default Value

StrobeDur

1 to 100

StrobePos

0 to 100

50

50

The strobe duration is configured as a percentage of the exposure time using the StrobeDur
parameter.

The effective strobe pulsation is:

● Expose_us x StrobeDur when the exposure time is controlled by the frame grabber (RG and

RP modes)

● TrueExp_us x StrobeDur when the exposure time is controlled by the camera (RC modes)

The strobe position is adjustable in 100 steps within the exposure time using the StrobePos
parameter.

A value of 0 establishes the earliest position. The leading edge of the strobe pulse is
simultaneous with the beginning of the exposure.

A value of 100 establishes the latest position. The trailing edge of the strobe pulse is
simultaneous with the end of exposure.

A value of 50 % means that the strobe pulse is located in the middle of the exposure period.

144

Grablink Grablink Functional Guide

Output Selection, Polarity and Electrical Style Controls

Applies to:

Output Line
Type

StrobeCtl StrobeLevel

Input Port

StrobeLine

Opto-isolated

OPTO

PLSLOW

STA

NOM or STA

PLSLOW
or
PLSHIGH

TTL

TTL

Isolated TTL

ITTL

Isolated open-
collector

Isolated open-
emittor

IOC

IOE

To select a strobe output port:

Enhanced IO1

IO1

Enhanced IO2

NOM or IO2

Enhanced IO3

Enhanced IO4

IsoA1

IsoA2

IsoA1

IsoA2

IsoA1

IsoA2

IO3

IO4

ISOA1

NOM or ISOA2

ISOA1

NOM or ISOA2

ISOA1

NOM or ISOA2

1. Set the value of the StrobeCtl parameter corresponding to the electrical style of the strobe

light device.

2. Set the value of the StrobeLevelparameter corresponding to the desired signal polarity:

3. Optionally, set the value of the StrobeLine parameter corresponding to the I/O port used to

attach the strobe light device.

NOTE
The default value of StrobeLine is NOM.

The strobe output ports are available on the System I/O Connector.

145

Grablink Grablink Functional Guide

Output Selection and Electrical Style Control

Applies to:

These Grablink boards only have one dedicated strobe output line per camera.

The strobe output line is available on both the Internal IO and External IO connectors:

Product

Camera

Connector(s)

Pins name

-

A

B

-

Internal I/O Connector
External I/O Connector

Channel A Internal I/O
Connector
External I/O Connector

Channel B Internal I/O
Connector
External I/O Connector

Internal I/O Connector
External I/O Connector

IOUT1+/IOUT1-

IOUT1A+/IOUT1A-

IOUT1B+/IOUT1B-

IOUT1+/IOUT1-

The strobe line drives an optically-isolated pair of pins. The + pin is the collector and the - pin is
the emitter of an uncommitted photo-transistor driven by LED-emitted light. The photo-
transistor remains OFF during the board initialization.

There is no line selection control: the Strobeline parameter is not applicable.

There is no electrical style control. The StrobeCtl parameter is not applicable.

NOTE
The strobe being always enabled by default, the IOUT1 output port is
configured by default as a strobe output and driven by the strobe controller.
To disconnect the IOUT1 output port from the strobe generator, set
StrobeMode to OFF.

146

BaseDualBaseFullFullXRBaseDualBaseDualBaseFullFullXRGrablink Grablink Functional Guide

10. Line-Scan Synchronization

10.1. Line Capture Modes

10.2. Line Rate Modes

10.3. Valid Line-Scan Synchronization Settings

10.4. Operating Limits

148

150

152

153

147

Grablink Grablink Functional Guide

10.1. Line Capture Modes

LineCaptureMode

Description

ALL

PICK

TAG

"Take-All-Line" line capture mode

The board acquires all the lines delivered by the camera providing
that the acquisition channel is active and the trigger conditions are
satisfied.
If the downweb motion speed is varying, the line-scanning process of
the camera would be rate-controlled accordingly.
This is the default line capture mode.

"Pick-A-Line" line capture mode

Each pulse occurring at the downweb line rate determines the
acquisition of the next line delivered by the camera providing that the
acquisition channel is active and the trigger conditions are satisfied.
This downweb resampling method allows the camera to be operated
at a constant line rate while acquiring lines at a variable downweb
line rate.

Tag-A-Line line capture mode

The line-scanning process of the camera is running at a constant rate
determined by Period_us. The down-web line rate is determined by
the pulse rate of A/B signals delivered by an external encoder and
processed by the quadrature decoder and the rate divider. The frame
grabber captures all lines delivered by the camera after having
replaced the first pixel data by a tag indicating that the line was
preceded or not by an hardware event on the divider output.

When LineCaptureMode = ALL, the DownwebLineRateand the CameraLineRateare the same.
The requested resolution and the effective motion speed uni-vocally dictate the DownwebLine
Rate. Then the camera has to be chosen to operate at an exactly matching Camera Line Rate,
even if the speed of motion is varying. This imposes a requirement for a rate-controllable
camera.

Using DownwebResamplingoffers a way to eliminate the requirement for this exact match. The
CameraLineRatemay be chosen at a fixed value, and the acquisition will still acquire lines at
the expected downweb resolution, even when the speed of motion is varying.

The Tag-A-Lineline capture mode is used together with the two-line synchronized line-scan
acquisition advanced feature. This feature enables a line-scan imaging application to acquire, in
a single scanning operation, images from 2 (or more) Basler Sprint bi-linear Bayer CFA color
line-scan cameras with 2 illumination devices turned on alternatively. The Tag-A-Line line
capture mode eliminates the spatial aliasing artifacts in the downweb direction that occurs
when using the Take-All-Linesmethod.

The relevant and the applicable values of LineCaptureMode depend on two prerequisites
settings: Imaging, CamConfig.

148

Grablink Grablink Functional Guide

See also: Refer to "Valid Line-Scan Synchronization Settings" on page 152 for a global view

149

Grablink Grablink Functional Guide

10.2. Line Rate Modes

LineRateModeexpresses how the DownwebLineRateis determined in a line-scan acquisition
system.

The user specifies the LineRateModeby means of MultiCam parameter LineRateMode. Five Line
RateModesare identified in MultiCam:

LineRateMode

Description

CAMERA

Camera – The DownwebLineRateis originated by the camera.

PULSE

CONVERT

PERIOD

EXPOSE

Trigger Pulse – The DownwebLineRateoriginates from a train of pulses
applied on the line trigger input belonging to the grabber.

Rate Converter – The DownwebLineRateoriginates from a train of
pulses applied on the line trigger input and processed by a rate converter
belonging to the grabber.

Periodic – The DownwebLineRateoriginates from an internal periodic
generator belonging to the grabber

Exposure Time – The DownwebLineRateis identical to the camera line
rate and established by the exposure time settings

LineRateMode = CAMERA

This mode is applicable exclusively for free-run permanent exposure – LxxxxSP – class of line
scan cameras when LineCaptureMode = ALL. The grabber does not perform any sampling in the
downweb direction; the DownwebLineRateis equal to the camera line rate. The camera line
rate is entirely under control of the camera. Notice that most of the line scan cameras provide
an internal line rate adjustment.

LineRateMode = PULSE

When the speed of motion is varying, the DownwebLineRateshould be slaved to this motion.
To achieve this, a motion encoder is a good solution.
The motion encoder delivers an electrical pulse each time the moving web advances by a
determined amount of length. The continuous motion results in a train of pulses the frequency
of which is proportional to the web speed.
There exists another way to take knowledge of the web speed. In some applications, the motion
is caused by a stepping motor controlled by pulses. The controlling train of pulses is also a
measure of relative motion.
In both cases, the pulses are called line trigger pulses, and their repetition rate is the Line
Trigger Rate. The line trigger pulses are applied to the frame grabber to determine the
DownwebLineRate.
Each line trigger pulse may result into the generation of one line in the acquired image. This
means that the DownwebLineRateis equal to the Trigger Rate.

150

Grablink Grablink Functional Guide

LineRateMode = CONVERT

Alternatively to the "PULSE" mode, for more flexibility, the Line Trigger Rate may be scaled up
or down to match the required DownwebLineRate. The proportion between the two rates is
freely programmable to any value lower or greater than unity, with high accuracy. This makes
possible to accommodate a variety of mechanical setups, and still maintain a full control over
the downweb resolution. The hardware device responsible for this rate conversion is called the
rate converter. This device is a unique characteristic of Euresys line-scan frame grabbers.

LineRateMode = PERIOD

Other circumstances necessitate the DownwebLineRateto be hardware-generated by a
programmable timer, called the "periodic generator".

LineRateMode = EXPOSE

Applies to:

This mode is applicable exclusively for line rate controlled permanent exposure – LxxxxRP –
class of line scan cameras when LineCaptureMode = ALL. The grabber does not perform any
sampling in the downweb direction; the DownwebLineRateis equal to the camera line rate.
The camera line rate is entirely under control of the grabber through the exposure time settings.

This mode is the default and recommended mode for LxxxxRP class of cameras on PC1621
Grablink Express.

151

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

10.3. Valid Line-Scan Synchronization Settings

The following table shows the valid combinations of parameters values to setup a line-scan
acquisition system:

Imagi
ng

CamCo
nfig

LineCaptureMode

ALL

PICK

TAG

LineRateMode

CAME
RA

PERI
OD

PUL
SE

CONV
ERT

EXPO
SE

PERI
OD

PUL
SE

CONV
ERT

PERI
OD

✓

✓

✓

LxxxxS
P

LxxxxR
P

LxxxxS
C

LxxxxR
C

LxxxxR
G

LxxxxR
G2

LxxxxS
P

LxxxxR
P

LINE

TDI

✓ (*)

✓ (*)

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓ (*)

✓ (*)

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

✓

NOTE
(*) These settings are not recommended since the camera sensitivity is
varying with the line rate.

152

Grablink Grablink Functional Guide

10.4. Operating Limits

When LineCaptureMode is set to PICK or ALL, the downweb line rate may not exceed the
maximum camera line rate.

The maximum effective camera line rate is the highest line rate that the line-scan camera can
achieve in the operating conditions. It can be evaluated using following formula:

● For free-running operation modes: LxxxxSP and LxxxxSC, it is mandatory to set the LineRate_

Hz parameter to a value equal to (or smaller than) the actual camera line rate.

● For grabber-controlled rate operation modes: LxxxxRP, LxxxxRC, LxxxxRG, and LxxxxRG2, it is

mandatory to set the LineRate_Hz parameter to a value equal to (or smaller than) the
maximum rate allowed by the camera.

● For grabber-controlled exposure operation modes: LxxxxRP, LxxxxRG, and LxxxxRG2, the

exposure time is defined by means of exposure control parameters.

● For grabber-controlled rate and camera-controlled exposure operation mode: LxxxxRC, it is
mandatory to set TrueExp_us to a value equal to (or higher than) the actual exposure time.

153

Grablink Grablink Functional Guide

11. Line Trigger

11.1. Line Trigger Overview

11.2. Source Path Selector

11.3. Rate Divider

11.4. Rate Converter

11.5. Periodic Generator

11.6. Line Trigger Line Selection

11.7. Signal Conditioning

11.8. Quadrature Decoder

155

156

157

158

159

160

161

162

154

Grablink Grablink Functional Guide

11.1. Line Trigger Overview

The "line trigger" signal triggers the acquisition of one line of the image.

Block diagram

NOTE
The line trigger is available only for the line-scan acquisition modes of
MultiCam, namely WEB, PAGE, and LONGPAGE. It is inoperative for the area-
scan acquisition mode.

NOTE
The line trigger is effective within a MultiCam acquisition sequence. Any line
trigger applied outside a MultiCam acquisition sequence is lost.

155

Grablink Grablink Functional Guide

11.2. Source Path Selector

The line trigger has five possible source paths. The path is selectable by means of the MultiCam
parameter LineRateMode:

● When LineRateMode is set to PULSE, the line trigger signal is generated by the quadrature

decoder circuit through a 1/N rate divider.

● When LineRateMode is set to CONVERT, the line trigger signal is generated by the quadrature

decoder circuit through a P/Q rate converter.

● When LineRateMode is set to PERIOD, the line trigger signal is generated by a periodic pulse

generator.

● When LineRateMode is set to CAMERA, the line trigger is generated by the camera through
the LVAL signal. The time interval between line triggers is actually driven by the camera.

● When LineRateMode is set to EXPOSE, the line trigger is generated by the exposure control
circuit. The time interval between line triggers is actually driven by the exposure time.

156

Grablink Grablink Functional Guide

11.3. Rate Divider

The rate divider circuit generates a line trigger signal at a frequency that is an integer fraction
1/N of the frequency of the pulses delivered by the quadrature decoder circuit.

For N consecutive incoming pulses issued by the quadrature decoder circuit, the 1/N rate
divider:

● generates one output pulse (one line trigger)

● skips N-1 input pulse

The rate divider is initialized at the beginning of every MultiCam acquisition sequence. The first
output pulse is produced from the first clock input pulse occurring after the sequence trigger
event.

Beginning of an acquisition sequence for a value N of 4

The division factor N is user-programmable. Possible values of N include all integer numbers
ranging from 1 up to 512, the default value is 1.

N is configured by the MultiCam parameter RateDivisionFactor.

Notes

□ The output frequency is lower than (N > 1) or equal to (N = 1) the input frequency. It

cannot be higher.

□ The output pulse is generated immediately after a non-skipped input pulse. The line

trigger pulses are phase-locked to the quadrature decoder output.

□ The rate divider settings may not be modified while acquisition is in progress.

157

Grablink Grablink Functional Guide

11.4. Rate Converter

The rate converter circuit generates a line trigger signal at a frequency that is proportional to
the frequency of the pulses delivered by the quadrature decoder circuit.

Rate conversion ratio

The rate converter is capable to multiply or divide the input rate of encoder ticks by any rational
number ranging from 1/1000 up to 1000/1

The rate conversion ratio RCR is defined as the ratio between the output rate and the input rate:

RCR = EncoderPitch/LinePitch

The possible values of RCR range from 0.001 up to 1000.0. The recommended values range from
0.01 up to 1.

Operating range

The rate converter operates within in a limited range of frequencies:

● The upper limit of the output frequency range is user configurable with parameter

MaxSpeed. By default, it is set to the maximum line rate sustainable by the camera defined
by LineRate_Hz.

● The lower limit of the output frequency is automatically set by the driver and reported to the
application via the MinSpeed parameter. The MaxSpeed/MinSpeed ratio is typically greater
than 100.

When the input rate drops below MinSpeed, the rate converter behaves according to the
OnMinSpeed setting:

● When set to MUTING, it stops delivering line trigger ticks.

● When set to IDLING, it continues delivering line trigger ticks at a constant frequency.

WARNING
To enlarge the usable speed range, it is mandatory to set MaxSpeed at a
value slightly above the actual max camera line rate.

Notices

● The line trigger pulses are NOT phase-locked to the quadrature decoder output.

● The rate converter settings may not be modified while acquisition is in progress.

158

Grablink Grablink Functional Guide

11.5. Periodic Generator

The periodic generator circuit generates a line trigger signal at a constant frequency.

The time interval T between two consecutive pulses is user programmable. Values of T range
from 1 µs up to 5 seconds.

The period T is configured by Period_us and PeriodTrim.

159

Grablink Grablink Functional Guide

11.6. Line Trigger Line Selection

When LineRateMode is PULSE or CONVERT, the line trigger signal originates from an external
motion encoder device. In that case line trigger input port(s) must be selected.

The selection is performed in two steps:

● Determine the electrical style of the input port(s) and set LineTrigCtl accordingly. MultiCam

selects the default input port(s) for the selected style.

● Optionally select one alternate choice for the input ports by setting the LineTrigLine

parameter.

LineTrigCtl

Description

Targeted type of motion encoder
device

DIFF

Single differential high-speed input port
compatible with EIA/TIA-422 signaling

Single output RS-422 compatible
incremental motion encoder

DIFF_PAIRED

Pair of differential high-speed input
ports compatible with EIA/TIA-422
signaling. Default value.

Dual-output RS-422 compatible
phase quadrature incremental
motion encoder

ISO

ISO_PAIRED

Single isolated current loop input port
compatible with TTL, +12V, + 24V
signaling

Pair of isolated current loop input ports
compatible with TTL, +12V, + 24V
signaling

Single output incremental motion
encoder

Dual-output phase quadrature
incremental motion encoder

LineTrigCtl

Default port(s) assignment

Alternate port assignment(s)

DIFF

DIN1

DIFF_PAIRED

DIN1 (A) + DIN2 (B)

ISO

IIN1

DIN2

-

IIN2
IIN3
IIN4

ISO_PAIRED

IIN1 (A) + IIN2 (B)

IIN3 (A) + IIN4 (B)

Notes

□ Dual output devices where only one output is connected are assimilated to single output

devices.

□ Any input port and hence any electrical style can be specified for that function.
□ The default port assignment for both single signal electrical styles is different of the one

of the trigger.

160

Grablink Grablink Functional Guide

11.7. Signal Conditioning

Applies to:

The hardware Trigger and End Trigger signals issued from differential inputs are conditioned
before being applied to the acquisition controller.

Each signal is sampled at a constant frequency of 50 MHz and flows through a digital filter.

With such a filter:

● All the pulses having a duration larger than THIGH[ns] are transmitted to the output.

● All the pulses having a duration smaller than TLOW[ns] are blocked.

● Pulses having duration in between the above mentioned limits can be transmitted or

blocked.

The following table shows for each possible value of the respective TrigFilter and EndTrigFilter
parameters:

● TLOW[ns]: the lower limit of the time constant

● THIGH[ns]: the upper limit of the time constant

TrigFilter
EndTrigFilter

TLOW[ns]

THIGH[ns] Note

OFF

ON

MEDIUM

STRONG

96

496

496

112

512

512

2496

2512

Default value

NOTE
TrigFilter and EndTrigFilter are not relevant for isolated inputs.

161

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

11.8. Quadrature Decoder

The quadrature decoder circuit interfaces directly with dual output phase quadrature
incremental motion encoders. It decodes the encoder A and B signals and generates pulses on
the Q output.

Quadrature decoder block diagram

Edge Selection

The rate of the output pulse can be 1, 2, or 4 times the frequency of the A signal according to
the position of the edge selector.

● When the edge selector is in the rising A position, an output pulse is generated for every

rising edge of the A signal. The falling edge on the A signal and both edges on the B-signal
are ignored.

● When the edge selector is in the falling A position, an output pulse is generated for every

falling edge of the A signal. The rising edge on the A signal and both edges on the B-signal
are ignored.

● When the selector is in the All A position, an output pulse is generated for every rising and

falling edges of the A signal. The B-signal is ignored.

● When the selector is in the All A B position, an output pulse is generated for every rising and

falling edges of the A and B signals.

The edge selector is controlled by the enumerated MultiCam parameter LineTrigEdge.

Direction Selector

The motion direction is determined by the phase relationship of the A and B signals.

162

Grablink Grablink Functional Guide

By construction, the dual-output phase quadrature incremental motion encoder maintains a
phase relationship of about 90 degrees between the two signals. For motion in one direction,
the A signal leads the B signal by about 90 degrees. For a motion in the other direction, the B
signal leads the A signal by about 90 degrees.

The direction selector provides the capability to define which one of the phase relationships is
considered as the forward direction for the application.

The default assignment identifies the case "A Leads B" as the forward direction.

Backward Motion Cancellation

The backward cancellation circuit stops sending line trigger pulses as soon as a backward
motion is detected. If such an event occurs, the acquisition is stopped.

When the backward cancellation control is configured in the filter mode (F-Mode), the line
acquisition resumes when the motion changes to the forward direction. Therefore, the
cancellation circuit filters out all the pulses corresponding to the backward direction.

When the backward cancellation control is configured in the compensation mode (C-Mode), the
line acquisition resumes when the motion changes to the the forward direction at the place it
was interrupted. Therefore, the cancellation circuit filters out not only the pulses corresponding
to the backward direction, but a number of forward pulses equal to the number of skipped
backward pulses.

In C-Mode, the cancellation circuit uses a "backward pulse counter" that:

● Increments by 1 every clock in the backward direction

● Decrements by 1 every clock in the forward direction until it reaches 0

● Resets at the beginning of each MultiCam acquisition sequence. More precisely, at the first
trigger event of the sequence. This trigger is considered as the reference for the position
along the web for the whole acquisition sequence.

In C-Mode, all pulses occurring when the counter value is different of zero are blocked.

The counter has a 16-bit span, backward displacement up to 65535 pulses can be compensated.

The backward cancellation circuit operates exclusively with quadrature motion encoders when
LineRateMode = PULSE.

The backward cancellation mode is configured by BackwardMotionCancellationMode.

163

Grablink Grablink Functional Guide

12. Advanced Features

12.1. Synchronized Line-scan Acquisition

12.2. Metadata Insertion

12.3. Interleaved Acquisition

12.4. Two-line Synchronized Line-scan Acquisition

12.5. Machine Pipeline Control

12.6. 8-tap/10-bit Acquisition

12.7. Video Lines Reordering

165

169

206

228

249

255

259

164

Grablink Grablink Functional Guide

12.1. Synchronized Line-scan Acquisition

Applies to:

Introduction

The "Synchronized Line-scan Acquisition" feature synchronizes accurately two or more line-scan
cameras using a SyncBus.

The SyncBus interconnects one Master MultiCam channel to one or more Slave MultiCam
channel(s).

The SyncBus conveys two synchronization signals:

● a line trigger signal, that, typically, originates from the motion encoder attached to the

Master MultiCam channel

● a start (and optionally end) of acquisition signal, that originates from the acquisition

controller of the Master MultiCam channel.

The synchronization signals are propagated to all the camera and acquisition controllers of all
the participating channels, including the master one.

The SyncBus interconnection is made using one of the following wiring schemes:

□ "Internal SyncBus wiring" on page 165
□ "C2C SyncBus wiring" on page 165
□ "Isolated I/O SyncBus wiring" on page 166

Internal SyncBus wiring

Applies to:

In the specific case of a SyncBus linking only the 2 channels of a PC1623 Grablink DualBase,
the SyncBus doesn't require any physical interconnect wiring; instead, it is routed locally
inside the FPGA.

C2C SyncBus wiring

Applies to:

The SyncBus wiring interconnects the C2C SyncBus Connector of the Master channel to the
C2C SyncBus Connector of one Slave channel using the 3305 C2C SyncBus cable or up to 3
Slave channels using the 3306 C2C Quad SyncBus Cable.

NOTE
This wiring scheme directly interconnects FPGA I/O’s allowing very accurate
synchronization with the fastest line-scan cameras.

This wiring scheme applies only to one pair of cards.

165

BaseDualBaseFullFullXRDualBaseFullFullXRGrablink Grablink Functional Guide

Isolated I/O SyncBus wiring

Applies to:

The SyncBus wiring interconnects the IOUT3/IOUT4 output ports of the Master channel to the
IIN3/IIN4 input ports of all the participating channels.

NOTE
This wiring scheme restricts the camera line rate range to 40 kHz.

Synchronized acquisition using Isolated I/O SyncBus wiring

166

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Requirements

For adequate synchronization, the line rate of the cameras must be controlled by the frame
grabber. Synchronization acquisition is allowed exclusively for the following values of
CamConfig: LxxxxRC, LxxxxRG, LxxxxRG2, and LxxxxRP.

The exposure settings of all cameras must be identical:

● For cameras having the exposure time controlled by the frame grabber, the value of Expose_

us and ExposeTrim must be identical on all channels.

● When the exposure time is controlled by the camera, all cameras must be configured

identically, and the value of TrueExp_us must be identical on all channels.

The line capture controller must be configured to take all lines: LineCaptureMode = ALL.

NOTE
The Pick-A-Line mode LineCaptureMode = PICK is not allowed.

Control Parameters

The SynchronizedAcquisition Channel parameter is the main control:

● when set to OFF (default value), the synchronized acquisition feature is disabled.

● when set to MASTER, LOCAL_MASTER, SLAVE or LOCAL_SLAVE, the synchronized acquisition

feature is enabled.

To configure the synchronized acquisition and the participating channels, proceed as follows:

The SynchronizedAcquisitionBus Channel parameter provides an additional option to select the
SyncBus wring:

● when set to ISO (default value), the SyncBus uses the Isolated I/O SyncBus wiring (or the

internal SyncBus wiring of PC1623 Grablink DualBase).

● when set to C2C, the SyncBus uses the C2C SyncBus wiring avaialble only on On PC1622

Grablink Full and PC1626 Grablink Full XR.

Master channel setup procedure

1. Select a line-scan acquisition mode by setting the value WEB, PAGE or LONGPAGE to

AcquisitionMode.

2. Set the value MASTER (or LOCAL_MASTER if local wiring can be used) to

SynchronizedAcquisition.

3. Set the value C2C to SynchronizedAcquisitionBus if C2C SyncBus wiring is required or leave

the ISO value

4. Configure remaining acquisition control parameters as for a stand-alone channel.

5. Configure the trigger control, and the encoder control parameters as for a stand-alone

channel.

6. Configure the exposure control parameters.

167

Grablink Grablink Functional Guide

Slave channel(s) setup procedure

1. Assign to AcquisitionMode the same value as on the master channel.

2. Set the value SLAVE (or LOCAL_SLAVE if internal SyncBus wiring can be used) to

SynchronizedAcquisition.

3. Set the value C2C to SynchronizedAcquisitionBus if C2C SyncBus wiring is required or leave

the ISO value

4. Configure the remaining acquisition control parameters as follows:

a. TrigMode and NextTrigMode are automatically set by MultiCam to the appropriate values,

they may not be modified.

b. EndTrigMode must be set to SLAVE if it is configured to HARD on the master channel.

Otherwise, it remains at its default value AUTO.

c. BreakEffect, PageLength_Ln, SeqLength_Pg and SeqLength_Ln, when applicable, must be

configured to the same values as on the master channel.

5. All the trigger control parameters are irrelevant and don't need to be configured.

6. The LineRateMode parameter is automatically set to SLAVE; the other parameters of the

encoder control category are irrelevant.

7. Apply to exposure control parameters the same settings as the master channel.

168

Grablink Grablink Functional Guide

12.2. Metadata Insertion

Applies to:

Introduction to Metadata Insertion

Metadata Controls

Metadata Content

Metadata Fields

Memory Layouts

170

171

173

175

178

169

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Introduction to Metadata Insertion

The metadata insertion feature inserts metadata in the Camera Link pixel data stream.

Requirements

Metadata insertion is available only with PC1624 Grablink Base, PC1623 Grablink DualBase,
PC1622 Grablink Full and PC1626 Grablink Full XR for a subset of cameras.

The metadata insertion feature is NOT compatible with the following frame grabber pixel
processing options:

● Look-up table transformation

● On-board Bayer CFA to RGB color conversion

● Pixel bit depth reduction

Capabilities

One, two or three fields of metadata are inserted in the Camera Link data stream according to
the camera configuration. Refer to "Metadata Content" on page 173 for an detailed description.

Refer to "Metadata Fields" on page 175 for an extensive description of each field.

Refer to "Memory Layouts" on page 178 for an extensive description of the metadata layout in
the captured images.

Refer to "Metadata Controls" on page 171 for an exhaustive list of control parameters.

170

Grablink Grablink Functional Guide

Metadata Controls

Main control

The activation of the Metadata Insertion feature is controlled through the MultiCam Channel
parameter MetadataInsertion.

By default, MetadataInsertion is set to DISABLE.

To activate the feature, set the parameter to ENABLE at channel creation. The setting takes
effect at the next channel activation.

NOTE
An error is reported when setting to ENABLE if the camera interface
configuration is not compatible.

Metadata location control

There are three distinct methods to insert metadata in the Camera Link pixel data stream
according to the value of the MetadataLocation parameter: LVALRISE, TAP1 or TAP10.

● When set to LVALRISE, the image data conveyed during the first Camera Link time slot after

an LVAL are replaced by metadata.

● When set to TAP1, the image data conveyed on the Camera Link port A (Tap 1) is replaced by
metadata during the first 10 Camera Link Clock cycles of every line after the LVAL rising edge.

● When set to TAP10, the image data conveyed on the Camera Link port J (Tap 10) is replaced
by metadata during the first 10 Camera Link Clock cycles of every line after the LVAL rising
edge. For the other clock cycles, the frame grabber inserts bytes of '0'.

By default, MetadataLocation is set to LVALRISE. This value is always available.

The TAP1 option is applicable only to Medium 4-tap 8-bit line-scan and TDI line-scan cameras
characterized by TapConfiguration = MEDIUM_4T8 and TapGeometry = 4X.

The TAP10 option is applicable only to 80-bit (10-tap 8-bit) line-scan and TDI line-scan cameras
characterized by TapConfiguration = DECA_10T8 and TapGeometry = 1X10.

171

Grablink Grablink Functional Guide

General Purpose Pulse Counter (GPPC) controls

Applies to:

The GPPC, a 32-bit binary counter, is available only on PC1622 Grablink Full and PC1626
Grablink Full XR when MetadataContent = THREE_FIELD.

The GPPC value is sampled at each rising edge of LVAL is reported in the GPPC metadata field.

Main control

The MetadataGPPCInputLine channel parameter is the main control of the GPPC:

● when set to NONE (default value), the GPPC is disabled,

● when set to IIN1, the GPPC counts the rising edge events of the electrical signal applied to

the IIN1 isolated input line.

GPPC field location control

The MetadataGPPCLocation parameter controls the location of the GPPC field in the metadata:

● when set to NONE (default value), the GPPC metadata is not inserted into the Camera Link

data stream.

● when set to INSTEAD_LVALCNT, the GPPC metadata replaces the LVAL Count metadata in

the Camera Link data stream,

● when set to INSTEAD_QCNT, the GPPC metadata replaces the Q Count metadata in the

Camera Link data stream,

GPPC reset control

The MetadataGPPCResetLine parameter controls the reset feature of the GPPC:

● when set to NONE (default value), the GPPC has no reset input line; the IIN4 input remains

available for another purpose.

● when set to IIN4, the GPPC resets when a high-level is applied to the IIN4 isolated input line.

172

FullFullXRGrablink Grablink Functional Guide

Metadata Content

The get-only MultiCam Channel parameter MetadataContent reports the number of distinct
metadata fields in the metadata content.

MetadataContent

Description

NONE

ONE_FIELD

TWO_FIELD

There is no metadata content.

The metadata content includes one single field: I/O State.

The metadata content includes two fields: I/O State + LVAL Count

THREE_FIELD

The metadata content includes three fields.

When MetadataContent = THREE_FIELD, the composition of the metadata depends on the
MetadataGPPCLocation control value:

MetadataGPPCLocation

Field 1

Field 2

NONE

I/O State

LVAL Count

INSTEAD_LVALCNT

I/O State

GPPC Count

Field 3

Q Count

Q Count

INSTEAD_QCNT

I/O State

LVAL Count

GPPC Count

NOTE
Refer to "Memory Layouts" on page 178 for the bit assignment and memory
layout applicable to each configuration

Camera configurations providing 3 fields

Imaging

Tap
Configuration

Tap
Geometry

Metadata
Location

Memory Layout

MEDIUM_4T8

4X

TAP1

TAP10

LVALRISE

LINE
or
TDI

DECA_10T8

1X10

DECA_8T10

1X8

DECA_8T30B3

1X8

DECA_2T40

1X2

"3-field 8-bit (TAP1)" on page
194

"3-field 8-bit (TAP10)" on page
196

"3-field 8-bit (LVALRISE)" on
page 198

"3-field 10-bit Packed" on
page 200

"3-field 30-bit Packed" on
page 202

"3-field 40-bit Packed" on
page 204

173

Grablink Grablink Functional Guide

Camera configurations providing 2 fields

Imaging

TapConfiguration

TapGeometry

Memory Layout

LINE
or
TDI

MEDIUM_6T8

1X3_1Y2

FULL_8T8

MEDIUM_2T24

1X8

1X2

"2-field 8-bit " on page 191

"2-field 24-bit " on page 192

Camera configurations providing 1 field

Imaging

TapConfiguration

TapGeometry

Memory Layout

AREA,
LINE
or
TDI

AREA,
LINE
or
TDI

*T8

*T10

*T12

*T14

*T16

*T24

*T30

*T36

*T42

*T48

All but1
2XM*,
2XR*,
2X2M*,
4XR*,
8XR*

"1-field 8-bit " on page 179

"1-field 10-bit " on page 180

"1-field 12-bit " on page 181

"1-field 14-bit " on page 182

"1-field 16-bit " on page 183

"1-field 24-bit " on page 184

"1-field 30-bit " on page 185

"1-field 36-bit " on page 186

"1-field 42-bit " on page 188

"1-field 48-bit " on page 190

1 Any tap geometry where the pixel order of the first region is not modified!

174

Grablink Grablink Functional Guide

Metadata Fields

I/O State Field

The I/O State field is a 6-bit field reporting the logical state of all System I/O input lines
belonging to the Channel.

The reported state is the logical state measured right at the input stage of the Grablink card.

WARNING
As the measurement takes place before any glitch removal filters, spurious
state transitions may occur!

The state of System I/O input lines and the values of the counters are sampled at each rising
edge of the Camera Link LVAL signal. The sampling time is not adjustable.

LVAL Count Field

The LVAL Count field is a 32-bit field reporting the current value of the LVAL pulse counter

LVAL pulse counter

The LVAL pulse counter is a 32-bit binary counter that counts the Camera Link LVAL pulses.

The counter is not resettable, it is set to 0 at driver initialization. As soon as the Camera Link de-
serializers are initialized, it is incremented by 1 at every LVAL cycle whatever the acquisition
conditions, i.e. whether the corresponding line data is acquired or not. It wraps around to 0
when it reaches the maximum count 4,294,967,295 (=232 -1).

NOTE
The counter is incremented before its value is inserted as metadata: the first
line cycle is marked 1.

175

Grablink Grablink Functional Guide

GPPC Count Field

The GPPC Count field is a 32-bit field reporting the current value of the General Purpose Pulse
Counter - GPPC.

General purpose pulse counter

The GGPC is a 32-bit binary counter.

The counter is set to 0 at driver initialization and is incremented by 1 at every rising edge event
detected on its input (IIN1). It wraps around to 0 when it reaches the maximum count
4,294,967,295 (=232 -1).

The counter has an optional reset input (IIN4). Applying a high level to the reset input, resets the
count value to 0.

NOTE
The counter is incremented before its value is inserted as metadata: the first
line cycle is marked 1.

176

Grablink Grablink Functional Guide

Q Count Field

The Q Count Field is a 32-bit field reporting the current value of the Motion Encoder Q counter.

This 32-bit binary counter counts the pulses at the Q output of the Quadrature Decoder:

NOTE
Depending on the Quadrature Decoder settings, the counter increments by
0, 1, 2, or 4 units every encoder cycle.

The counter is not resettable:

● It is set to 0 at driver initialization.

● It is incremented by 1 at every Q cycle.

● It wraps around to 0 when it reaches the maximum count 4,294,967,295 (=232 -1).

177

Grablink Grablink Functional Guide

Memory Layouts

MemorylayoutsofthemetadataasdeliveredintheMultiCamsurface

Introduction

This section provides one topic for each combination of number of metadata fields and pixel bit
count.

Each topic provides memory layouts of metadata for applicable combinations of ColorFormat,
MetadataLocation and ImageFlipX.

All image lines contain metadata. However, there are exceptions:

1. When TapGeometry is set to any *_1Y2 value, only one line out of two contains metadata.

2. When TapGeometry is set to any *_2YE value, only the lines belonging to the upper (when
ImageFlipY = OFF) or the lower (when ImageFlipY = ON) half region contain metadata.

179

180

181

182

183

184

185

186

188

190

191

192

194

196

198

200

202

204

1-field 8-bit

1-field 10-bit

1-field 12-bit

1-field 14-bit

1-field 16-bit

1-field 24-bit

1-field 30-bit

1-field 36-bit

1-field 42-bit

1-field 48-bit

2-field 8-bit

2-field 24-bit

3-field 8-bit (TAP1)

3-field 8-bit (TAP10)

3-field 8-bit (LVALRISE)

3-field 10-bit Packed

3-field 30-bit Packed

3-field 40-bit Packed

178

Grablink Grablink Functional Guide

1-field 8-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = OFF

Byte #

0

7

0

6

0

5

DIN2

4

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = ON

Byte #

last

7

0

6

0

5

DIN2

4

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

179

Grablink Grablink Functional Guide

1-field 10-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y10 or BAYER10; ImageFlipX = OFF

Byte #

0

1

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = OFF

Byte #

0

1

7

IIN2

0

6

IIN1

0

5

0

0

4

0

0

3

0

2

IIN3

0

2

0

1

IIN2

0

0

IIN1

0

1

0

0

0

DIN2

DIN1

IIN4

IIN3

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y10 or BAYER10; ImageFlipX = ON

Byte #

last - 1

last

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

2

IIN3

0

1

IIN2

0

0

IIN1

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = ON

Byte #

last - 1

last

7

IIN2

0

6

IIN1

0

5

0

0

4

0

0

3

0

2

0

1

0

0

0

DIN2

DIN1

IIN4

IIN3

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

180

Grablink Grablink Functional Guide

1-field 12-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y12 or BAYER12; ImageFlipX = OFF

Byte #

0

1

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

2

IIN3

0

1

IIN2

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = OFF

Byte #

0

1

7

IIN4

0

6

IIN3

0

5

IIN2

0

4

IIN1

0

3

0

0

2

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y12 or BAYER12; ImageFlipX = ON

0

IIN1

0

0

0

1

0

DIN2

DIN1

Byte #

last - 1

last

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

2

IIN3

0

1

IIN2

0

0

IIN1

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = ON

Byte #

last - 1

last

7

IIN4

0

6

IIN3

0

5

IIN2

0

4

IIN1

0

3

0

0

2

0

0

1

0

0

0

DIN2

DIN1

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

181

Grablink Grablink Functional Guide

1-field 14-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y14 or BAYER14; ImageFlipX = OFF

Byte #

0

1

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

2

IIN3

0

1

IIN2

0

0

IIN1

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = OFF

Byte #

0

1

7

DIN2

0

6

DIN1

0

5

IIN4

0

4

IIN3

0

3

IIN2

0

2

IIN1

0

1

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y14 or BAYER14; ImageFlipX = ON

Byte #

last - 1

last

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

2

IIN3

0

1

IIN2

0

0

IIN1

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = ON

Byte #

last - 1

last

7

DIN2

0

6

DIN1

0

5

IIN4

0

4

IIN3

0

3

IIN2

0

2

IIN1

0

1

0

0

0

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

182

Grablink Grablink Functional Guide

1-field 16-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = OFF

Byte #

0

1

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = Y16 or BAYER16; ImageFlipX = ON

Byte #

last - 1

last

7

0

0

6

0

0

5

DIN2

0

4

DIN1

0

3

IIN4

0

2

IIN3

0

2

IIN3

0

1

IIN2

0

1

IIN2

0

0

IIN1

0

0

IIN1

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

183

Grablink Grablink Functional Guide

1-field 24-bit

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24; ImageFlipX = OFF

Byte #

0

1

2

7

0

0

0

6

0

0

0

5

DIN2

0

0

4

DIN1

0

0

3

IIN4

0

0

2

IIN3

0

0

1

IIN2

0

0

TapConfiguration = MEDIUM_2T24; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24PL; ImageFlipX = OFF

Plane Byte #

B 0

G 0

R 0

7

0

0

0

6

0

0

0

5

4

DIN2

DIN1

0

0

0

0

3

IIN4

0

0

2

IIN3

0

0

1

IIN2

0

0

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24; ImageFlipX = ON

Byte #

last - 2

last - 1

last

7

0

0

0

6

0

0

0

5

DIN2

0

0

4

DIN1

0

0

3

IIN4

0

0

2

IIN3

0

0

1

IIN2

0

0

0

IIN1

0

0

0

IIN1

0

0

0

IIN1

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

TapConfiguration = MEDIUM_2T24; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24PL; ImageFlipX = ON

Plane Byte #

B last

G last

R last

7

0

0

0

6

0

0

0

5

4

DIN2

DIN1

0

0

0

0

3

IIN4

0

0

2

IIN3

0

0

1

IIN2

0

0

0

IIN1

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

184

Grablink Grablink Functional Guide

1-field 30-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB30PL; ImageFlipX = OFF

Plane Byte #

B 0

B 1

G 0

G 1

R 0

R 1

7

0

0

0

0

0

0

6

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = OFF

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

0

0

0

0

0

2

0

0

0

0

0

0

1

0

0

0

0

0

0

0

0

3

0

DIN2

DIN1

IIN4

IIN3

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

Plane Byte #

B 0

B 1

G 0

G 1

R 0

R 1

7

IIN2

6

IIN1

0

0

0

0

0

0

0

0

0

0

5

0

0

0

0

0

0

4

0

0

0

0

0

0

185

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

Grablink Grablink Functional Guide

1-field 36-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB36PL; ImageFlipX = OFF

Plane Byte #

B 0

B 1

G 0

G 1

R 0

R 1

7

0

0

0

0

0

0

6

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = OFF

Plane Byte #

B 0

B 1

G 0

G 1

R 0

R 1

7

IIN4

6

IIN3

5

IIN2

4

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

3

0

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB36PL; ImageFlipX = ON

0

0

0

0

0

2

0

0

0

0

0

0

0

0

0

0

0

1

0

0

0

0

0

0

0

0

DIN2

DIN1

0

0

0

0

0

0

0

0

Plane Byte #

B last - 1

B last

G last - 1

G last

R last - 1

R last

7

0

0

0

0

0

0

6

0

0

0

0

0

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

186

Grablink Grablink Functional Guide

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = ON

Plane Byte #

B last - 1

B last

G last - 1

G last

R last - 1

R last

7

IIN4

6

IIN3

5

IIN2

4

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

3

0

0

0

0

0

0

2

0

0

0

0

0

0

1

0

0

0

DIN2

DIN1

0

0

0

0

0

0

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

187

Grablink Grablink Functional Guide

1-field 42-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB42PL; ImageFlipX = OFF

Plane Byte #

B 0

B 1

G 0

G 1

R 0

R 1

7

0

0

0

0

0

0

6

0

0

0

0

0

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = OFF

Plane Byte #

7

6

B 0

B 1

G 0

G 1

R 0

R 1

DIN2

DIN1

0

0

0

0

0

0

0

0

0

0

5

IIN4

4

IIN3

3

2

IIN2

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

1

0

0

0

0

0

0

0

0

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB42PL; ImageFlipX = ON

Plane Byte #

B last - 1

B last

G last - 1

G last

R last - 1

R last

7

0

0

0

0

0

0

6

0

0

0

0

0

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

188

Grablink Grablink Functional Guide

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = ON

Plane Byte #

7

6

B last - 1

B last

G last - 1

G last

R last - 1

R last

DIN2

DIN1

0

0

0

0

0

0

0

0

0

0

5

IIN4

4

IIN3

3

2

IIN2

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

1

0

0

0

0

0

0

0

0

0

0

0

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

189

Grablink Grablink Functional Guide

1-field 48-bit

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = OFF

Plane Byte #

B 0

B 1

G 0

G 1

R 0

R 1

7

0

0

0

0

0

0

6

0

0

0

0

0

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

MetadataLocation = LVALRISE; MetadataContent = ONE_FIELD

ColorFormat = RGB48PL; ImageFlipX = ON

Plane Byte #

B last - 1

B last

G last - 1

G last

R last - 1

R last

7

0

0

0

0

0

0

6

0

0

0

0

0

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

1

IIN2

0

IIN1

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

190

Grablink Grablink Functional Guide

2-field 8-bit

TapConfiguration = MEDIUM_6T8; TapGeometry = 1X3_1Y2

TapConfiguration = FULL_8T8; TapGeometry = 1X8

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

7

0

0

LV7

LV15

LV23

LV31

6

0

0

LV6

LV14

LV22

LV30

5

4

DIN2

DIN1

0

LV5

LV13

LV21

LV29

0

LV4

LV12

LV20

LV28

3

IIN4

0

LV3

LV11

LV19

LV27

TapConfiguration = MEDIUM_6T8; TapGeometry = 1X3_1Y2

TapConfiguration = FULL_8T8; TapGeometry = 1X8

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = ON

Byte #

last - 5

last - 4

last - 3

last - 2

last - 1

last

7

LV31

LV23

LV15

LV7

0

0

6

LV30

LV22

LV14

LV6

0

0

5

LV29

LV21

LV13

LV5

0

4

LV28

LV20

LV12

LV4

0

DIN2

DIN1

3

LV27

LV19

LV11

LV3

0

IIN4

2

IIN3

0

LV2

LV10

LV18

LV26

2

LV26

LV18

LV10

LV2

0

IIN3

1

IIN2

0

LV1

LV9

LV17

LV25

1

LV25

LV17

LV9

LV1

0

IIN2

0

IIN1

0

LV0

LV8

LV16

LV24

0

LV24

LV16

LV8

LV0

0

IIN1

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

191

Grablink Grablink Functional Guide

2-field 24-bit

TapConfiguration = MEDIUM_2T24; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

7

0

0

LV7

LV15

LV23

LV31

6

0

0

LV6

LV14

LV22

LV30

5

4

DIN2

DIN1

0

LV5

LV13

LV21

LV29

0

LV4

LV12

LV20

LV28

3

IIN4

0

LV3

LV11

LV19

LV27

2

IIN3

0

LV2

LV10

LV18

LV26

1

IIN2

0

LV1

LV9

LV17

LV25

TapConfiguration = MEDIUM_2T24; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24PL; ImageFlipX = OFF

Plane Byte #

B 0

G 0

R 0

B 1

G 1

R 1

7

0

0

LV7

LV15

LV23

LV31

6

0

0

LV6

LV14

LV22

LV30

5

4

3

2

DIN2

DIN1

IIN4

IIN3

0

LV5

LV13

LV21

LV29

0

LV4

LV12

LV20

LV28

0

LV3

LV11

LV19

LV27

0

LV2

LV10

LV18

LV26

1

IIN2

0

LV1

LV9

LV17

LV25

TapConfiguration = MEDIUM_2T24; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24; ImageFlipX = ON

Byte #

last - 5

last - 4

last - 3

last - 2

last - 1

last

7

LV15

LV23

LV31

0

0

6

LV14

LV22

LV30

0

0

LV7

LV6

5

LV13

LV21

LV29

DIN2

0

LV5

4

LV12

LV20

LV28

DIN1

0

LV4

3

LV11

LV19

LV27

IIN4

0

LV3

2

LV10

LV18

LV26

IIN3

0

LV2

1

LV9

LV17

LV25

IIN2

0

LV1

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

0

IIN1

0

LV0

LV8

LV16

LV24

0

IIN1

0

LV0

LV8

LV16

LV24

0

LV8

LV16

LV24

IIN1

0

LV0

192

Grablink Grablink Functional Guide

TapConfiguration = MEDIUM_2T24; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataContent = TWO_FIELD

ColorFormat = RGB24PL; ImageFlipX = ON

Plane Byte #

7

3

4

5

6

7

8

B last - 1

G last - 1

R last - 1

B last

G last

R last

6

LV14

LV22

LV30

0

0

LV15

LV23

LV31

0

0

LV7

LV6

5

LV13

LV21

LV29

DIN2

0

LV5

4

LV12

LV20

LV28

DIN1

0

LV4

3

LV11

LV19

LV27

IIN4

0

LV3

2

LV10

LV18

LV26

IIN3

0

LV2

1

LV9

LV17

LV25

IIN2

0

LV1

0

LV8

LV16

LV24

IIN1

0

LV0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

193

Grablink Grablink Functional Guide

3-field 8-bit (TAP1)

TapConfiguration = MEDIUM4_T8; TapGeometry = 4X

MetadataLocation = TAP1; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

6

7

8

9

7

0

0

QC7

QC15

QC23

QC31

LV7

LV15

LV23

LV31

NOTE

6

0

0

QC6

QC14

QC22

QC30

LV6

LV14

LV22

LV30

5

4

DIN2

DIN1

0

QC5

QC13

QC21

QC29

LV5

LV13

LV21

LV29

0

QC4

QC12

QC20

QC28

LV4

LV12

LV20

LV28

3

IIN4

0

QC3

QC11

QC19

QC27

LV3

LV11

LV19

LV27

2

IIN3

0

QC2

QC10

QC18

QC26

LV2

LV10

LV18

LV26

1

IIN2

0

QC1

QC9

QC17

QC25

LV1

LV9

LV17

LV25

0

IIN1

0

QC0

QC8

QC16

QC24

LV0

LV8

LV16

LV24

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

194

Grablink Grablink Functional Guide

TapConfiguration = MEDIUM4_T8; TapGeometry = 4X

MetadataLocation = TAP1; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = ON

Byte #

last - 9

last - 8

last - 7

last - 6

last - 5

last - 4

last - 3

last - 2

last - 1

last - 0

7

LV31

LV23

LV15

LV7

QC31

QC23

QC15

QC7

0

0

6

LV30

LV22

LV14

LV6

QC30

QC22

QC14

QC6

0

0

5

LV29

LV21

LV13

LV5

QC29

QC21

QC13

QC5

0

4

LV28

LV20

LV12

LV4

QC28

QC20

QC12

QC4

0

DIN2

DIN1

3

LV27

LV19

LV11

LV3

QC27

QC19

QC11

QC3

0

IIN4

2

LV26

LV18

LV10

LV2

QC26

QC18

QC10

QC2

0

IIN3

1

LV25

LV17

LV9

LV1

QC25

QC17

QC9

QC1

0

IIN2

0

LV24

LV16

LV8

LV0

QC24

QC16

QC8

QC0

0

IIN1

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

NOTE

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

195

Grablink Grablink Functional Guide

3-field 8-bit (TAP10)

TapConfiguration = DECA_10T8; TapGeometry = 1X10

MetadataLocation = TAP10; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = OFF

Byte #

9

19

29

39

49

59

69

79

89

99

7

0

0

QC7

QC15

QC23

QC31

LV7

LV15

LV23

LV31

NOTE

6

0

0

QC6

QC14

QC22

QC30

LV6

LV14

LV22

LV30

5

0

DIN2

QC5

QC13

QC21

QC29

LV5

LV13

LV21

LV29

4

0

DIN1

QC4

QC12

QC20

QC28

LV4

LV12

LV20

LV28

3

0

IIN4

QC3

QC11

QC19

QC27

LV3

LV11

LV19

LV27

2

0

IIN3

QC2

QC10

QC18

QC26

LV2

LV10

LV18

LV26

1

0

IIN2

QC1

QC9

QC17

QC25

LV1

LV9

LV17

LV25

0

0

IIN1

QC0

QC8

QC16

QC24

LV0

LV8

LV16

LV24

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

196

Grablink Grablink Functional Guide

TapConfiguration = DECA_10T8; TapGeometry = 1X10

MetadataLocation = TAP10; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = ON

Byte #

last - 99

last - 89

last - 79

last - 69

last - 59

last - 49

last - 39

last - 29

last - 19

last - 9

7

LV31

LV23

LV15

LV7

QC31

QC23

QC15

QC7

0

0

6

LV30

LV22

LV14

LV6

QC30

QC22

QC14

QC6

0

0

5

LV29

LV21

LV13

LV5

QC29

QC21

QC13

QC5

DIN2

0

4

LV28

LV20

LV12

LV4

QC28

QC20

QC12

QC4

DIN1

0

3

LV27

LV19

LV11

LV3

QC27

QC19

QC11

QC3

IIN4

0

2

LV26

LV18

LV10

LV2

QC26

QC18

QC10

QC2

IIN3

0

1

LV25

LV17

LV9

LV1

QC25

QC17

QC9

QC1

IIN2

0

0

LV24

LV16

LV8

LV0

QC24

QC16

QC8

QC0

IIN1

0

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

NOTE

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

197

Grablink Grablink Functional Guide

3-field 8-bit (LVALRISE)

TapConfiguration = DECA_10T8; TapGeometry = 1X10

MetadataLocation = LVALRISE; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

6

7

8

9

7

0

0

QC7

QC15

QC23

QC31

LV7

LV15

LV23

LV31

NOTE

6

0

0

QC6

QC14

QC22

QC30

LV6

LV14

LV22

LV30

5

4

DIN2

DIN1

0

QC5

QC13

QC21

QC29

LV5

LV13

LV21

LV29

0

QC4

QC12

QC20

QC28

LV4

LV12

LV20

LV28

3

IIN4

0

QC3

QC11

QC19

QC27

LV3

LV11

LV19

LV27

2

IIN3

0

QC2

QC10

QC18

QC26

LV2

LV10

LV18

LV26

1

IIN2

0

QC1

QC9

QC17

QC25

LV1

LV9

LV17

LV25

0

IIN1

0

QC0

QC8

QC16

QC24

LV0

LV8

LV16

LV24

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

198

Grablink Grablink Functional Guide

TapConfiguration = DECA_10T8; TapGeometry = 1X10

MetadataLocation = LVALRISE; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y8 or BAYER8; ImageFlipX = ON

Byte #

last - 9

last - 8

last - 7

last - 6

last - 5

last - 4

last - 3

last - 2

last - 1

last - 0

7

LV31

LV23

LV15

LV7

QC31

QC23

QC15

QC7

0

0

6

LV30

LV22

LV14

LV6

QC30

QC22

QC14

QC6

0

0

5

LV29

LV21

LV13

LV5

QC29

QC21

QC13

QC5

0

4

LV28

LV20

LV12

LV4

QC28

QC20

QC12

QC4

0

DIN2

DIN1

3

LV27

LV19

LV11

LV3

QC27

QC19

QC11

QC3

0

IIN4

2

LV26

LV18

LV10

LV2

QC26

QC18

QC10

QC2

0

IIN3

1

LV25

LV17

LV9

LV1

QC25

QC17

QC9

QC1

0

IIN2

0

LV24

LV16

LV8

LV0

QC24

QC16

QC8

QC0

0

IIN1

NOTE
last = address offset of the last byte of the rightmost active pixel of the line

NOTE

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

199

Grablink Grablink Functional Guide

3-field 10-bit Packed

TapConfiguration = DECA_8T10; TapGeometry = 1X8

MetadataLocation = LVALRISE; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = Y10P; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

6

7

8

9

7

0

QC7

QC15

QC23

QC31

0

LV7

LV15

LV23

LV31

6

0

QC6

QC14

QC22

QC30

0

LV6

LV14

LV22

LV30

5

DIN2

QC5

QC13

QC21

QC29

0

LV5

LV13

LV21

LV29

4

DIN1

QC4

QC12

QC20

QC28

0

LV4

LV12

LV20

LV28

3

IIN4

QC3

QC11

QC19

QC27

0

LV3

LV11

LV19

LV27

2

IIN3

QC2

QC10

QC18

QC26

0

LV2

LV10

LV18

LV26

1

IIN2

QC1

QC9

QC17

QC25

0

LV1

LV9

LV17

LV25

0

IIN1

QC0

QC8

QC16

QC24

0

LV0

LV8

LV16

LV24

NOTE
ImageFlipX = ON is not available when ColorFormat = Y10P

NOTE

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

200

Grablink Grablink Functional Guide

After unpacking to 16-bit with justification to lsb

Byte #

0

1

2

3

4

5

6

7

8

9

10

11

12

13

14

15

7

0

6

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

QC9

QC8

QC7

QC6

QC5

QC4

QC19

QC18

QC17

QC16

QC15

QC14

QC29

QC28

QC27

QC26

QC25

QC24

0

0

0

0

0

0

LV9

LV8

LV7

LV6

LV5

LV4

LV19

LV18

LV17

LV16

LV15

LV14

LV29

LV28

LV27

LV26

LV25

LV24

1

IIN2

QC1

QC3

QC11

QC13

QC21

QC23

QC31

0

LV1

LV3

LV11

LV13

LV21

LV23

LV31

0

IIN1

QC0

QC2

QC10

QC12

QC20

QC22

QC30

0

LV0

LV2

LV10

LV12

LV20

LV22

LV30

201

Grablink Grablink Functional Guide

3-field 30-bit Packed

TapConfiguration = DECA_8T30B3; TapGeometry = 1X8

MetadataLocation = LVALRISE; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = RGB30P; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

6

7

8

9

7

0

QC7

QC15

QC23

QC31

0

LV7

LV15

LV23

LV31

6

0

QC6

QC14

QC22

QC30

0

LV6

LV14

LV22

LV30

5

DIN2

QC5

QC13

QC21

QC29

0

LV5

LV13

LV21

LV29

4

DIN1

QC4

QC12

QC20

QC28

0

LV4

LV12

LV20

LV28

3

IIN4

QC3

QC11

QC19

QC27

0

LV3

LV11

LV19

LV27

2

IIN3

QC2

QC10

QC18

QC26

0

LV2

LV10

LV18

LV26

1

IIN2

QC1

QC9

QC17

QC25

0

LV1

LV9

LV17

LV25

0

IIN1

QC0

QC8

QC16

QC24

0

LV0

LV8

LV16

LV24

NOTE
ImageFlipX = ON is not available when ColorFormat = RGB30P

NOTE

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

202

Grablink Grablink Functional Guide

After unpacking to 16-bit with justification to lsb

Byte #

0

1

2

3

4

5

6

7

8

9

10

11

12

13

14

15

7

0

6

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

QC9

QC8

QC7

QC6

QC5

QC4

QC19

QC18

QC17

QC16

QC15

QC14

QC29

QC28

QC27

QC26

QC25

QC24

0

0

0

0

0

0

LV9

LV8

LV7

LV6

LV5

LV4

LV19

LV18

LV17

LV16

LV15

LV14

LV29

LV28

LV27

LV26

LV25

LV24

1

IIN2

QC1

QC3

QC11

QC13

QC21

QC23

QC31

0

LV1

LV3

LV11

LV13

LV21

LV23

LV31

0

IIN1

QC0

QC2

QC10

QC12

QC20

QC22

QC30

0

LV0

LV2

LV10

LV12

LV20

LV22

LV30

203

Grablink Grablink Functional Guide

3-field 40-bit Packed

TapConfiguration = DECA_2T40; TapGeometry = 1X2

MetadataLocation = LVALRISE; MetadataGPPCLocation = NONE

MetadataContent = THREE_FIELD

ColorFormat = RGBI40P; ImageFlipX = OFF

Byte #

0

1

2

3

4

5

6

7

8

9

7

0

QC7

QC15

QC23

QC31

0

LV7

LV15

LV23

LV31

6

0

QC6

QC14

QC22

QC30

0

LV6

LV14

LV22

LV30

5

DIN2

QC5

QC13

QC21

QC29

0

LV5

LV13

LV21

LV29

4

DIN1

QC4

QC12

QC20

QC28

0

LV4

LV12

LV20

LV28

3

IIN4

QC3

QC11

QC19

QC27

0

LV3

LV11

LV19

LV27

2

IIN3

QC2

QC10

QC18

QC26

0

LV2

LV10

LV18

LV26

1

IIN2

QC1

QC9

QC17

QC25

0

LV1

LV9

LV17

LV25

0

IIN1

QC0

QC8

QC16

QC24

0

LV0

LV8

LV16

LV24

NOTE
ImageFlipX = ON is not available when ColorFormat = RGBI40P

NOTE

□ When MetadataGPPCLocation = INSTEAD_LVALCNT replace LV* by

GPPC*

□ When MetadataGPPCLocation = INSTEAD_QCNT replace QC* by GPPC*

204

Grablink Grablink Functional Guide

After unpacking to 16-bit with justification to lsb

Byte #

0

1

2

3

4

5

6

7

8

9

10

11

12

13

14

15

7

0

6

0

5

4

DIN2

DIN1

3

IIN4

2

IIN3

QC9

QC8

QC7

QC6

QC5

QC4

QC19

QC18

QC17

QC16

QC15

QC14

QC29

QC28

QC27

QC26

QC25

QC24

0

0

0

0

0

0

LV9

LV8

LV7

LV6

LV5

LV4

LV19

LV18

LV17

LV16

LV15

LV14

LV29

LV28

LV27

LV26

LV25

LV24

1

IIN2

QC1

QC3

QC11

QC13

QC21

QC23

QC31

0

LV1

LV3

LV11

LV13

LV21

LV23

LV31

0

IIN1

QC0

QC2

QC10

QC12

QC20

QC22

QC30

0

LV0

LV2

LV10

LV12

LV20

LV22

LV30

205

Grablink Grablink Functional Guide

12.3. Interleaved Acquisition

Applies to:
Imageacquisitionfromgrabber-controlledexposureasynchronousresetcamerasdriven
alternativelybytwodifferentcameracycleprograms

Introduction

When Interleaved Acquisition is enabled, the Camera and Illumination Controller is configured
with two different programs named P1 and P2.

Each program defines entirely a cameraandilluminationcycleincluding:

● One Reset pulse controlling the start-of-exposure and the end-of-exposure of the camera.

● One Strobe pulse on any of the 2 strobe outputs.

The programs are executed alternatively, starting with P1.

This feature is available for line-scancamerassince MultiCam 6.9.7. For more information, refer
to "Interleaved Line-scan Acquisition Principles" on page 209.

This feature is available for area-scancamerassince MultiCam 6.13. For more information, refer
to "Interleaved Area-scan Acquisition Principles" on page 207.

Interleaved Area-scan Acquisition Principles

Interleaved Line-scan Acquisition Principles

Reset and Strobe Signals Routing

Interleaved Camera and Illumination Control

Interleaved Area-scan Acquisition Channel Setup

Interleaved Line-scan Acquisition Channel Setup

207

209

211

212

218

223

206

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Interleaved Area-scan Acquisition Principles

System Description

Main elements of an area-scan acquisition system configured for Interleaved Area-scan
Acquisition

The system is composed of:

● One asynchronous reset grabber-controlled exposure area-scan camera.

● One acquisition channel of a compatible Grablink frame grabber configured for Interleaved

Area-scan acquisition.

● Two illumination devices, each being controlled by a specific strobe output of the frame

grabber.

Usually, the Trigger event is delivered by a presence detector.

SNAPSHOT and HFR area-scan acquisition modes are compatible with Interleaved Area-scan
Acquisition.

207

GrablinkResetStrobe1Strobe2FVAL, LVALArea-scan CameraDataAcquired DataPresence DetectorCamera & Illumination ControllerCycle TriggerTriggerHost PC MemoryBufferAcquireAcquisition  ControllerGrablink Grablink Functional Guide

Operation

The Interleaved Acquisition feature allows to capture, with a time-optimized sequence of two
camera cycles, two images with different exposure time, strobe duration, strobe delay and
strobe output settings.

The first cycle of the sequence uses the settings defined by P1, the second cycle uses the
settings defined by P2.

Acquisition sequence of two overlapping cycles ( Exposure < Readout for P1 & P2)

The above sequence is time-optimized. Assuming that the exposure time is smaller than the
readout time for both cycles:

● The exposure of the second cycle overlaps the readout of the first cycle.

● The exposure of the second cycle terminates exactly when the readout of the first cycle

terminates.

Double Exposure Mode Emulation

The following drawing shows a particular Interleaved Area-scan Acquisition sequence of two
overlapping cycles where the second exposure time matches the readout time:

Acquisition sequence of two overlapping cycles (Exposure < Readout for P1; Exposure =
Readout for P2)

The exposure time of the second cycle is increased to become equal to the readout time. This
allows the second strobe to be issued immediately after the first strobe.

This emulates the doubleexposuremode.

208

P2 ExposureStrobe 1FVALTriggerResetStrobe 2P1 ExposureP1 StrobeP2 StrobeP1 ReadoutCamera readoutP2 ReadoutCamera exposureP2 ExposureStrobe 1FVALTriggerResetStrobe 2P1 ExposureP1 StrobeP2 StrobeP1 ReadoutCamera readoutP2 ReadoutCamera exposureGrablink Grablink Functional Guide

Interleaved Line-scan Acquisition Principles

System Description

Main elements of an line-scan acquisition system configured for Interleaved Area-scan
Acquisition

The system is composed of:

● One asynchronous reset grabber-controlled exposure line-scan camera.

● One acquisition channel of a compatible frame grabber configured for Interleaved Line-scan

acquisition.

● Two illumination devices, each being controlled by a specific strobe output of the frame

grabber.

Usually, the Line Trigger event is obtained by processing signals delivered by a motion encoder.
As for any line-scan imaging systems, it can be processed by the rate converter or the rate
divider.

Usually, the Trigger and the End Trigger events are delivered by a presence detector.

WEB, PAGE and LONGPAGE line-scan acquisition modes are compatible with Interleaved Line
Acquisition.

Operation

The Interleaved Acquisition feature allows to capture, in a single scanning operation, a
composite image where the odd and even lines are captured with different exposure time,
strobe duration, strobe delay and strobe output settings.

209

GrablinkResetStrobe1Strobe2LVALLine-scan CameraDataAcquired DataCamera & Illumination ControllerCycle TriggerHost PC MemoryBufferAcquireAcquisition  ControllerPresence DetectorTriggerPresence DetectorEnd TriggerEncoderLine TriggerGrablink Grablink Functional Guide

As shown on the following diagram, the Camera and Illumination Controller executes both
programs alternatively: P1 then P2 then P1 … :

Acquisition sequence showing the alternating P1 and P2 cycles

NOTE
In this example, the line trigger interval is larger than the minimum allowed.

The toggling program sequence is reset at every start-of-scan to ensure that the first captured
image line of a scanned object is always built using P1.

In WEB acquisition mode, a reset occurs only once at the beginning of the acquisition sequence.

In PAGE acquisition mode, a reset occurs at the beginning of every acquisition phase.

In LONGPAGE acquisition mode, a reset occurs at the beginning of the first acquisition phase of
every acquisition sequence. No reset occurs at the beginning of the subsequent phases of the
same sequence.

210

P1 StrobeStrobe 1LVALLine TriggerResetStrobe 2Camera ReadoutCamera ExposureP2ProgramEndP1 CycleP2 CycleP1 CycleP1 ExposureP1 ReadoutP2P2 ExposureGrablink Grablink Functional Guide

Reset and Strobe Signals Routing

The reset pulses of both programs are merged into a common Reset signal; the Reset signal can
be sent to one or more of the 4 Camera Link Control lines CC1 … CC4.

The strobe pulse of each program can be routed individually to IOUT1, to IOUT2 or left unused.

When routed to the same output line, the two pulses are merged!

211

Grablink Grablink Functional Guide

Interleaved Camera and Illumination Control

This section describes the operation of the Camera and Illumination Controller – CIC – when
Interleaved Acquisition is enabled.

NOTE
This section applies to line-scan and area-scan interleaved acquisition.

Cycle Trigger designates the event that initiates a CIC cycle: a Line Trigger event in case of line-
scan cameras or a Trigger event in case of area-scan cameras.

CIC Cycle Programs

When Interleaved Acquisition is enabled, the Camera and Illumination Controller is configured
with two, usually different, camera and illumination cycle programs. These programs are named
P1 and P2 respectively.

Each camera and illumination cycle program defines five events on a timeline beginning with a
Cycle Trigger event.

1. ResetON: turn ON time of the Reset pulse and Start of Exposure

2. ResetOFF: turn OFF time of the Reset pulse and End of Exposure

3. StrobeON: turn ON time of the Strobe pulse and Start Of Illumination

4. StrobeOFF: turn OFF time of the Strobe pulse and End Of Illumination

5. AllowNextTrigger : the last event of a program indicating that a new cycle may be initiated.

CIC Program Cycle events and timing definitions

Each program defines two pulses: one Reset pulse and one Strobe pulse. Their timing is user
configurable:

● Exposure Time is the time interval between the ResetON and the ResetOFF events.

● Strobe Duration is the time interval between the StrobeON and the StrobeOFF events.

● Exposure Delay is the time interval between the Cycle Trigger and ResetON events.

212

MinTriggerPeriodStrobeResetExposure TimeStrobe DurationStrobe DelayExposure DelayCycle TriggerResetONStrobeONStrobeOFFResetOFFAllowNextTriggerPostExposureDelayTime0Grablink Grablink Functional Guide

● Strobe Delay is the time interval between the ResetON and StrobeON events. This value can
be positive, null, or negative allowing the Strobe pulse to be positioned anywhere relatively
to the start of exposure.

The following restrictions apply on the position order of the events on the timeline:

0≤ResetON<ResetOFF≤AllowNextTrigger

0≤StrobeON<StrobeOFF≤AllowNextTrigger

MultiCam Camera Trigger Overrun Protection Principle

At acquisition channel configuration time:

● MultiCam checks if the exposure time user setting can be achieved by the camera. If the user
setting of the exposure time is out of bounds, MultiCam corrects its value. The effective
exposure time will be set to the nearest boundary.

● MultiCam calculates the position on the timeline of the AllowNextTrigger event of P1 and P2
programs. This calculation takes into account the camera operating limits and the user-
defined exposure and strobe timing settings for P1 and P2 programs.

At acquisition channel run time, MultiCam reports a "trigger violation” error if a Cycle Trigger
event is issued before the AllowNextTrigger event during the execution of a program.

Camera Operating Limits

The following camera operating limits are considered:

● Exposure time range

● Minimum time interval between two consecutive Exposure

● Maximum line rate

In MultiCam, the following parameters describe the operating limits of a camera:

● ExposeMin_us: declares the minimum exposure time, expressed in microseconds (i.e. the

minimum duration of a Reset pulse).

● ExposeMax_us: declares the maximum exposure time, expressed in microseconds (i.e. the

maximum duration of a Reset pulse).

● ResetDur: for line-scan cameras only, declares the minimum time interval between two

consecutives Reset pulses.

● ExposeRecovery_us: for area-scan cameras only, declares the minimum time interval

between two consecutive Reset pulses.

● LineRate_Hz: for line-scan cameras only, declares the highest line rate supported by the

camera (i.e. the reciprocal of the readout time)

● FrameRate_mHz: for area-scan cameras only, declares the highest frame rate supported by

the camera (i.e. the reciprocal of the readout time)

● ExposeOverlap: declares that the camera allows or forbids the next exposure to begin before

the completion of the current readout.

Allow Next Trigger Rules

MultiCam applies the following rules when it calculates the position of the AllowNextTrigger
event.

213

Grablink Grablink Functional Guide

RULE 1a – Readout time limitation (Expose Overlapping forbidden)

This rule applies only when ExposeOverlap = FORBID.

The start of exposure of the next cycle may not occur before the end of the current camera
readout.

NOTE
If there is any exposure delay (a) in the next cycle, the AllowNextTrigger
event may be generated earlier.

RULE 1b - Readout time limitation (Expose Overlapping allowed)

This rule applies only when ExposeOverlap = ALLOW.

The end of exposure of the next cycle must not occur before the end of the current camera
readout.

NOTE
If there is any exposure delay (a) in the next cycle, the AllowNextTrigger
event may be generated earlier.

214

Cycle TriggerResetCamera ReadoutCamera ExposureAllowNextTriggerNext CycleCurent  CycleaReadout timeCycle TriggerResetCamera ReadoutCamera ExposureAllowNextTriggerNext CycleCurent  CycleaReadout timeGrablink Grablink Functional Guide

RULE 2 – Reset interval limitation

The time interval (r) between consecutive Reset pulses may not be shorter than the value
specified by ResetDur.

NOTE
If there is any exposure delay (a) in the next cycle, the AllowNextTrigger
event may be generated earlier.

RULE 3 – Next Cycle

NOTE
This rule applies only to line-scan interleaved acquisition!

The next cycle after P1 is undetermined, it can be either P2 or P1. The next cycle after P2 is
always P1.

For the calculation of the position of the AllowNextTrigger event of P1, MultiCam evaluates both
hypothesis (P1 and P2) and keeps the worst case.

For the calculation of the position of the AllowNextTrigger event of P2, MultiCam assumes that
the next cycle is P1.

215

Cycle TriggerResetCamera ExposureAllowNextTriggerNext CycleCurent  CyclearGrablink Grablink Functional Guide

Exposure Delay

By default, MultiCam configures P1 and P2 with the smallest possible value:

● 0 when StrobeDelay_P<1,2>_us ≥ 0

● (-StrobeDelay_P<1,2>_us) when StrobeDelay_P<1,2>_us < 0

If required, the exposure delay can be configured using any of the following methods:

Exposure Delay - Manual Method

This method is invoked when ExposureDelayControl is set to MANUAL. This is the default
method.

With this method, the user may specify the exposure delay for P1 and P2 with:

ExposureDelay_MAN_P1_us and ExposureDelay_MAN_P2_us.

By default, these parameters are set to 0.

MultiCam calculates the smallest possible value for each program separately as follows:

● ExposureDelay_MAN_P<1,2>_us whenStrobeDelay_P<1,2>_us ≥ (- ExposureDelay_MAN_

P<1,2>_us)

● (-StrobeDelay_P<1,2>_us) when StrobeDelay_P<1,2>_us < (- ExposureDelay_MAN_P<1,2>_us)

Exposure Delay - Automatic method 1 (Same Start of Exposure)

Select this method by setting ExposureDelayControl to SAME_START_EXPOSURE. MultiCam
calculates, the smallest value ensuring that the start of exposure occurs at the same position on
the program timeline.

Exposure Delay - Automatic method 2 (Same Endof Exposure)

Select this method by setting ExposureDelayControl to SAME_END_EXPOSURE.

MultiCam calculates the smallest values ensuring that the end of exposure occurs at the same
position on the program timeline.

Effective Exposure Time

The effective exposure time values are reported by ExposureTime_P1_Effective_us and
ExposureTime_P2_Effective_us.

NOTE
In general, the effective values are very close to the user settings, the slight
differences can be explained by rounding issues to the nearest timer tick
period.

NOTE
Large differences can be observed in the case of an out-of-bound user
setting.

216

Grablink Grablink Functional Guide

Effective Strobe Duration and Strobe Delay

The effective strobe duration and strobe delay values are reported by StrobeDuration_P1_
Effective_us, StrobeDuration_P2_Effective_us, StrobeDelay_P1_Effective_us and, StrobeDelay_
P2_Effective_us.

NOTE
The effective values are, in any case, very close to the user settings, the
slight differences can be explained by rounding issues to the nearest timer
tick period.

Effective Exposure Delay

The effective exposure delay values are reported by ExposureDelay_P1_Effective_us and
ExposureDelay_P2_Effective_us.

NOTE
When ExposureDelayControl = MANUAL, the effective values are very close to
the user settings, the slight differences can be explained by rounding issues
to the nearest timer tick period.

NOTE
Large differences can be observed in the case of negative strobe delay
values.

Effective Minimum Trigger Period

The run time of each program is reported by MinTriggerPeriod_P1_us and MinTriggerPeriod_
P2_us.

NOTE
The values can be different. Considering that programs are executed
alternatively, the user should only consider the larger value as the minimum
time interval between line triggers.

217

Grablink Grablink Functional Guide

Interleaved Area-scan Acquisition Channel Setup

PxxRG_IA CAM File Template

;*******************************************************************************************
**
; Camera Manufacturer: Templates
; Camera Model: MyCameraLink
; Camera Configuration: Interleaved Area-Scan Acquisition, Asynchronous Reset, Grabber-
Controlled Exposure
; Board: Grablink

- SNAPSHOT and HFR Acquisition Modes
- Interleaved Acquisition

- Progressive area-scan camera
- Asynchronous Reset
- Pulse-Width grabber-controlled exposure

;*******************************************************************************************
**
; This CAM file template is suitable for the following camera configuration:
;
;
;
; This CAM file template is suitable for the following system configuration:
;
;
;
; ********************************************************************************
**
; ** CAUTION:
; ** This file is a template, it can be further customized!
**
; ** The lines that can be edited are marked with an arrow followed by the most **
; ** popular alternate values for that parameter.
**
; ** For a complete list of possible values; refer to MultiCam Studio and/or to **
; ** the MultiCam Reference documentation.
**
; ********************************************************************************
;

;*******************************************************************************************
**
; ==Begin of "Camera properties Section"==
;
; -Camera Specification category-

Camera =
CamConfig =
Imaging =
Spectrum =

MyCameraLink;
PxxRG;
AREA;
BW;

;
; -Camera Features category-

TapConfiguration =
TapGeometry =
Expose =
Readout =
ColorMethod =
ColorRegistration = BG;
ExposeOverlap =

BASE_1T8;
1X_1Y;
WIDTH;
INTCTL;
NONE;

FORBID;

;
; --Downstream signals--

FvalMode =
LvalMode =
DvalMode =

;
; --Upstream signals--

ResetCtl =
ResetEdge =
CC1Usage =
CC2Usage =

FA;
LA;
DN;

DIFF;
GOHIGH;
RESET;
LOW;

<== BW COLOR ...

<== BASE_1T8 BASE_1T10 BASE_1T24 ...
<== 1X_1Y 1X2_1Y 2X_1Y ...

<== NONE PRISM BAYER RGB
<== GB BG RG GR (when ColorMethod=BAYER)
<== FORBID ALLOW

<== DN DG

<== GOHIGH GOLOW
<== LOW HIGH RESET SOFT DIN1 IIN1
<== LOW HIGH RESET SOFT DIN2

218

Grablink Grablink Functional Guide

CC3Usage =
CC4Usage =

LOW;
LOW;

;
; -Camera Timing category-

<== LOW HIGH RESET SOFT IIN1
<== LOW HIGH RESET SOFT

640;
480;
0;
0;
30000;

Hactive_Px =
Vactive_Ln =
HSyncAft_Tk =
VSyncAft_Ln =
FrameRate_mHz =
ExposeRecovery_us = 10;
ReadoutRecovery_us = 10;
ExposeMin_us =
ExposeMax_us =

10;
1000000;

<==
<==
<==
<==
<==
<==
<==
<==
<==

;
; ==End of "Camera properties Section"==

;*******************************************************************************************
**
; ==Begin of "System properties Section"==
;
; -Acquisition Control category-

AcquisitionMode =
TrigMode =
NextTrigMode =
ActivityLength =
SeqLength_Fr =
PhaseLength_Fr =

SNAPSHOT;
IMMEDIATE;
SAME;
1;
2;
1;

;
;
; -Trigger Control category-
ISO;
GOHIGH;
MEDIUM;
0;
NOM;

TrigCtl =
TrigEdge =
TrigFilter =
TrigDelay_us =
TrigLine =

<== SNAPSHOT HFR
<== IMMEDIATE HARD SOFT COMBINED
<== SAME HARD SOFT COMBINED REPEAT
<== 1
<== -1 1..65534
<== 1 (when AcquisitionMode = SNAPSHOT)
<== 1..255 (when AcquisitionMode = HFR)

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<==
<== NOM ...

;

The following 2 parameters are controlling the Trigger Decimation circuit:

TrigDelay_Pls =
0;
NextTrigDelay_Pls = 0;

<== 0..65536
<== 0..65536

;
; -Interleaved Acquisition category-

;

;

InterleavedAcquisition =

ON;

<== Enable interleaved acquisition

Define the exposure time for P1 and P2 (= RESET signal pulse width)

ExposureTime_P1_us =
ExposureTime_P2_us =

7000.0;
35000.0;

<== Float (0.16 up to 5000000)
<== Float (0.16 up to 5000000)

Define the strobe duration for P1 and P2 (= STROBE1 and STROBE2 signals pulse width)

StrobeDuration_P1_us =
StrobeDuration_P2_us =

7000.0;
10000.0;

<== Float (0.16 up to 5000000)
<== Float (0.16 up to 5000000)

Define the strobe delay for P1 and P2 (relative time offset from RESET going ON to

The time offset can be positive, null or negative

StrobeDelay_P1_us =
StrobeDelay_P2_us =

0.0;
0.0;

<== Float (-10000 up to 5000000)
<== Float (-10000 up to 5000000)

Select the Exposure delay control method

ExposureDelayControl =

MANUAL;

<== MANUAL SAME_END_EXPOSURE SAME_START_EXPOSURE

When ExposureDelayControl is MANUAL, select the minimum delay from the trigger
to the start of exposure (RESET signal going on)

;

;
;

ExposureDelay_MAN_P1_us =
ExposureDelay_MAN_P2_us =
StrobeLine_P1 =
StrobeLine_P2 =
StrobeOutput_P1 =
StrobeOutput_P2 =

0;
0;
IOUT1;
IOUT2;
ENABLE;
ENABLE;

<== Float (0 up to 5000000)
<== Float (0 up to 5000000)
<== IOUT1 IOUT2 NONE
<== IOUT1 IOUT2 NONE
<== ENABLE DISABLE
<== ENABLE DISABLE

;
; ==End of "System properties Section"==

219

;
STROBEx going ON)
;

;
; -Look-Up Tables category-
;
;
; -Cluster category-
ColorFormat =
ImageFlipX =
ImageFlipY =

Y8;
OFF;
OFF;

Grablink Grablink Functional Guide

;*******************************************************************************************
**
; ==Begin of "Grabber properties Section"==
;
; -Grabber Configuration, Timing & Conditioning categories-
<== NOBLACK MAN

GrabWindow =

NOBLACK;

;

;

The following 4 parameters are relevant only when GrabWindow = MAN:

WindowX_Px =
WindowY_Ln =
OffsetX_Px =
OffsetY_Ln =

640;
480;
0;
0;

<==
<==
<==
<==

The following parameter configures the Bayer CFA Decoder:

CFD_Mode =

ADVANCED;

<== ADVANCED, LEGACY

LUT configuration parameters can be inserted here if required by the application

<== Y8 Y10 RGB24 RGB24PL ...
<== OFF ON
<== OFF ON

;
; End of "Grabber properties Section"

;*******************************************************************************************
**
; End of File
;=============

Customizing Camera Parameters

The following camera parameters must be set according to the selected camera model:

Spectrum, TapConfiguration, TapGeometry, ColorMethod, DvalMode, ResetEdge, CC1Usage,
CC2Usage, CC3Usage, CC4Usage, Hactive_Px, Vactive_Ln , HSyncAft_Tk and, VSyncAft_Ln.

For correct operation of the camera trigger overrun protection mechanism it is mandatory to
carefully set the following parameters: FrameRate_mHz, ExposeMin_us, ExposeMax_us and,
ExposeRecovery_us.

Customizing Acquisition Control Parameters

AcquisitionMode can optionally be set to HFR.

In that case PhaseLength_Fr can be set to any value in 1 … 255 range.

The other parameters are not customizable.

Customizing Trigger Control Parameters

The following trigger parameters must be set according to the application needs: TrigCtl,
TrigEdge, TrigFilter, TrigLine.

The trigger decimation circuit can optionally be activated using TrigDelay_Pls and
NextTrigDelay_Pls.

Customizing Interleaved Acquisition parameters

Enable Interleaved Acquisition by assigning the value ON to InterleavedAcquisition.

220

Grablink Grablink Functional Guide

Customizing Interleaved Acquisition – Exposure and Strobe Timing Parameters

When Interleaved Acquisition is enabled, the following exposure and strobe parameters are
irrelevant:

Expose_us, ExposeTrim, StrobeMode, StrobeDur and, PreStrobe_us.

Instead, the exposure and strobe timings must be defined for P1 and P2 using the following
parameter set:

ExposureTime_P1_us, ExposureTime_P2_us, StrobeDuration_P1_us, StrobeDuration_P2_us,
StrobeDelay_P1_us and, StrobeDelay_P2_us.

Customizing Interleaved Acquisition – Exposure Delay Parameters

By default, MultiCam configures P1 and P2 with the smallest possible Exposure Delay value. This
setting is satisfactory for the use cases where the exposure time is shorter than the readout
time.

Optionally, keeping ExposureDelayControl set to MANUAL, you may manually change the
minimum exposure delay value of P1 and/or P2 using the ExposureDelay_MAN_P1_us and
ExposureDelay_MAN_P2_us parameters.

Alternatively, you may also change ExposureDelayControl to one of the automatic control
methods: SAME_START_EXPOSURE or SAME_END_EXPOSURE.

With SAME_START_EXPOSURE, the start of exposure is delayed by the same amount of time for
both programs: both exposure delay values are equal.

With SAME_END_EXPOSURE the end of exposure is delayed by the same amount of time for
both programs.

In case of asymmetric exposure times, when at least one exposure time is greater than the
readout time, the minimal line trigger period can be achieved when:

● Assigning the longest exposure time to P2

● Inserting an exposure delay prior to the lowest one

Customizing Interleaved Acquisition – Strobe Control Parameters

The StrobeLine_P1 and StrobeLine_P2 parameters designate the I/O lines used as strobe
outputs for P1 and P2 respectively. The default values are IOUT1 for P1 and IOUT2 for P2.

Setting StrobeLine_P2 to IOUT1 or NONE disconnects the IOUT2 output from the P2 Strobe and
makes it available for another usage (Software controlled I/O).

Setting StrobeLine_P1 and StrobeLine_P2to the same output IOUT1 merges the two strobe
pulses .

The StrobeOutput_P1 and StrobeOutput_P2 parameters control the delivery of the strobe pulse
for P1 and P2 respectively. The delivery is enabled by default. Assigning the DISABLE value,
inhibits the delivery of the strobe pulse.

Customizing Grabber Timing Parameters

As for any are-scan application, the following grabber configuration, timing and conditioning
parameters must be set according to the application needs: GrabWindow, WindowX_Px,
WindowY_Ln, OffsetX_Px and OffsetY_Ln.

221

Grablink Grablink Functional Guide

Customizing Cluster Parameters

As for any area-scan application, the following cluster parameters must be set according to the
application needs: ColorFormat, ImageFlipX and, ImageFlipY.

222

Grablink Grablink Functional Guide

Interleaved Line-scan Acquisition Channel Setup

LxxxxRG_IA CAM File Template

;*******************************************************************************************
**
; Camera Manufacturer: Templates
; Camera Model: MyCameraLink
; Camera Configuration: Interleaved Line-Scan Acquisition, Grabber-Controlled Rate and
Exposure
; Board: Grablink

- Line-scan camera
- Grabber-controlled rate
- Pulse-Width grabber-controlled exposure

- WEB, PAGE, or LONGPAGE Acquisition Modes
- Take all lines
- Interleaved Acquisition

;*******************************************************************************************
**
; This CAM file template is suitable for the following camera configuration:
;
;
;
; This CAM file template is suitable for the following system configuration:
;
;
;
;
; ********************************************************************************
**
; ** CAUTION:
; ** This file is a template, it can be further customized!
**
; ** The lines that can be edited are marked with an arrow followed by the most **
**
; ** popular alternate values for that parameter.
; ** For a complete list of possible values; refer to MultiCam Studio and/or to **
; ** the MultiCam Reference documentation.
**
; ********************************************************************************
;

;*******************************************************************************************
**
; ==Begin of "Camera properties Section"==
;
; -Camera Specification category-

Camera =
CamConfig =
Imaging =
Spectrum =

MyCameraLink;
LxxxxRG;
LINE;
BW;

<== BW COLOR ...

;
; -Camera Features category-

TapConfiguration =
TapGeometry =
Expose =
Readout =
ColorMethod =

BASE_1T8;
1X;
WIDTH;
INTCTL;
NONE;

;
; --Downstream signals--

FvalMode =
LvalMode =
DvalMode =

;
; --Upstream signals--

ResetCtl =
ResetEdge =
CC1Usage =
CC2Usage =
CC3Usage =

FN;
LA;
DN;

DIFF;
GOHIGH;
RESET;
LOW;
LOW;

<== BASE_1T8 BASE_1T10 BASE_1T24 ...
<== 1X 1X2 2X ...

<== NONE PRISM TRILINEAR RGB

<== DN DG

<== GOHIGH GOLOW
<== LOW HIGH RESET SOFT DIN1 IIN1
<== LOW HIGH RESET SOFT DIN2
<== LOW HIGH RESET SOFT IIN1

223

Grablink Grablink Functional Guide

CC4Usage =

LOW;

<== LOW HIGH RESET SOFT

;
; -Camera Timing category-

Hactive_Px
=
HSyncAft_Tk =
LineRate_Hz =

4096;
0;
5000;

duration)

<==
<==
<== Max. line rate (= reciprocal of readout

ExposeMin_us =

1;

<== Min. exposure time (= RESET signal pulse

width)

ExposeMax_us =

10000;

<== Max. exposure time (= RESET signal pulse

width)

ResetDur =

3000;

<== Min. time interval, in ns, between

consecutive RESET pulses
;
; ==End of "Camera properties Section"==

;*******************************************************************************************
**
; ==Begin of "System properties Section"==
;
; -Acquisition Control category-

AcquisitionMode =
TrigMode =
NextTrigMode =

WEB;
IMMEDIATE;
REPEAT;

<== WEB PAGE LONGPAGE
<== IMMEDIATE HARD SOFT COMBINED
<== REPEAT (when AcquisitionMode = WEB or

LONGPAGE)
;
AcquisitionMode = PAGE)

EndTrigMode =

AUTO;

;

BreakEffect =
SeqLength_Pg =
SeqLength_Ln =

FINISH;
-1;
-1;

LONGPAGE)

<== SAME REPEAT HARD SOFT COMBINED (when

<== AUTO HARD (when AcquisitionMode = LONGPAGE)
<== AUTO (when AcquisitionMode = WEB or PAGE)
<== FINISH ABORT
<== -1 1 .. 65534 (when AcquisitionMode = PAGE)
<== -1 1 .. 65534 (when AcquisitionMode = WEB or

PageLength_Ln =

500;

<== 1 .. 65535

;
; -Trigger Control category-
ISO;
GOHIGH;
MEDIUM;
NOM;

TrigCtl =
TrigEdge =
TrigFilter =
TrigLine =

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;

The following 4 parameters are relevant only when EndTrigMode = HARD!

EndTrigCtl =
EndTrigEdge =
EndTrigFilter =
EndTrigLine =

ISO;
GOLOW;
MEDIUM;
NOM;

;
; -Interleaved Acquisition category-

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;

;

InterleavedAcquisition =

ON;

<== Enable interleaved acquisition

Define the exposure time for P1 and P2 (= RESET signal pulse width)

ExposureTime_P1_us =
ExposureTime_P2_us =

64.0;
64.0;

<== Float (0.16 up to 5000000)
<== Float (0.16 up to 5000000)

Define the strobe duration for P1 and P2 (= STROBE1 and STROBE2 signals pulse width)

StrobeDuration_P1_us =
StrobeDuration_P2_us =

32.0;
32.0;

<== Float (0.16 up to 5000000)
<== Float (0.16 up to 5000000)

Define the strobe delay for P1 and P2 (relative time offset from RESET going ON to

The time offset can be positive, null or negative

StrobeDelay_P1_us =
StrobeDelay_P2_us =

16.0;
16.0;

<== Float (-10000 up to 5000000)
<== Float (-10000 up to 5000000)

Select the Exposure delay control method

ExposureDelayControl =

MANUAL;

<== MANUAL SAME_END_EXPOSURE SAME_START_EXPOSURE

When ExposureDelayControl is MANUAL, select the minimum delay from the trigger
to the start of exposure (RESET signal going on)

ExposureDelay_MAN_P1_us =

0;

<== Float (0 up to 5000000)

;

;
;

224

;
STROBEx going ON)
;

Grablink Grablink Functional Guide

ExposureDelay_MAN_P2_us =
StrobeLine_P1 =
StrobeLine_P2 =
StrobeOutput_P1 =
StrobeOutput_P2 =

0;
IOUT1;
IOUT2;
ENABLE;
ENABLE;

<== Float (0 up to 5000000)
<== IOUT1
<== IOUT2 NONE
<== ENABLE DISABLE
<== ENABLE DISABLE

;
; -Encoder Control category-
ALL;
PERIOD;

LineCaptureMode =
LineRateMode =

<== PERIOD PULSE CONVERT

;

;

;

The following 2 parameters are relevant only when LineRateMode = PERIOD:

Period_us =
PeriodTrim =

1000;
0;

<==
<==

The following 4 parameters are relevant only when LineRateMode = CONVERT:

LinePitch =
EncoderPitch =
ConverterTrim =
OnMinSpeed =

100;
100;
0;
IDLING;

<==
<==
<==
<== IDLING MUTING

The following 4 parameters are relevant only when LineRateMode = PULSE or CONVERT:

LineTrigCtl =
LineTrigEdge =

DIFF_PAIRED;
ALL_A_B;

<== ISO DIFF ISO_PAIRED DIFF_PAIRED
<== RISING_A FALLING_A ALL_A (when LineTrigCtl =

ISO or DIFF)
;
DIFF_PAIRED)

<== ALL_A_B (when LineTrigCtl = ISO_PAIRED or

LineTrigFilter =
LineTrigLine =

MEDIUM;
NOM;

<== OFF MEDIUM STRONG ...
<== NOM ...

;
LineRateMode = PULSE:

The following parameter controls the Rate divider circuit that is available when

RateDivisionFactor = 1;

<== 1..512
The following 2 parameters are controlling the Backward Motion Cancellation circuit

when LineTrigCtl = ISO_PAIRED or DIFF_PAIRED:

ForwardDirection =
BackwardMotionCancellationMode = OFF;

A_LEADS_B;

<== A_LEADS_B B_LEADS_A
<== OFF FILTERED COMPENSATE

;
; ==End of "System properties Section"==

;*******************************************************************************************
**
; ==Begin of "Grabber properties Section"==
;
; -Grabber Configuration, Timing & Conditioning categories-
<== NOBLACK MAN

GrabWindow =

NOBLACK;

;

The following 2 parameters are relevant only when GrabWindow = MAN:

WindowX_Px =
OffsetX_Px =

2048;
0;

<==
<==

;
that is available
;

;
; -Look-Up Tables category-
;
;
; -Cluster category-
ColorFormat =
ImageFlipX =

Y8;
OFF;

LUT configuration parameters can be inserted here if required by the application

<== Y8 Y10 RGB24 RGB24PL ...
<== OFF ON

;
; End of "Grabber properties Section"

;*******************************************************************************************
**
; End of File
;=============

225

Grablink Grablink Functional Guide

Customizing Camera Parameters

As for any line-scan camera, the following camera parameters must be set according to the
selected camera model:

Spectrum, TapConfiguration, TapGeometry, ColorMethod, DvalMode, ResetEdge, CC1Usage,
CC2Usage, CC3Usage, CC4Usage, Hactive_Px and, HSyncAft_Tk.

For correct operation of the camera trigger overrun protection mechanism it is essential to
carefully set the following parameters:

LineRate_Hz, ExposeMin_us, ExposeMax_us and, ResetDur.

Customizing Acquisition Control Parameters

As for any line-scan application, the following acquisition control parameters must be set
according to the application needs: AcquisitionMode, TrigMode, NextTrigMode, EndTrigMode,
BreakEffect, SeqLength_Pg, SeqLength_Ln and, PageLength_Ln.

Customizing Trigger Control Parameters

As for any line-scan application, the following trigger and end trigger control parameters must
be set according to the application needs: TrigCtl, TrigEdge, TrigFilter, TrigLine, EndTrigCtl,
EndTrigEdge, EndTrigFilter and, EndTrigLine.

Customizing Interleaved Acquisition parameters

Enable Interleaved Line-scan Acquisition by assigning the value ON to InterleavedAcquisition.

Customizing Interleaved Acquisition – Exposure and Strobe Timing Parameters

When Interleaved Line-scan Acquisition is enabled, the following exposure and strobe
parameters are irrelevant:

Expose_us, ExposeTrim, StrobeMode, StrobeDur and, PreStrobe_us.

Instead, the exposure and strobe timings must be defined for P1 and P2 using the following
parameter set:

ExposureTime_P1_us, ExposureTime_P2_us, StrobeDuration_P1_us, StrobeDuration_P2_us,
StrobeDelay_P1_us and, StrobeDelay_P2_us.

226

Grablink Grablink Functional Guide

Customizing Interleaved Acquisition – Exposure Delay Parameters

By default, MultiCam configures P1 and P2 with the smallest possible Exposure Delay value. This
setting is satisfactory for the use cases where the exposure time is shorter than the readout
time.

Optionally, keeping ExposureDelayControl set to MANUAL, you may manually change the
minimum exposure delay value of P1 and/or P2 using the ExposureDelay_MAN_P1_us and
ExposureDelay_MAN_P2_us parameters.

Alternatively, you may also change ExposureDelayControl to one of the automatic control
methods: SAME_START_EXPOSURE or SAME_END_EXPOSURE.

With SAME_START_EXPOSURE, the start of exposure is delayed by the same amount of time for
both programs: both exposure delay values are equal.

With SAME_END_EXPOSURE the end of exposure is delayed by the same amount of time for
both programs.

In case of asymmetric exposure times, when at least one exposure time is greater than the
readout time, the minimal line trigger period can be achieved when:

● Assigning the longest exposure time to P2

● Inserting an exposure delay prior to the lowest one

Customizing Encoder Control Parameters

As for any line-scan application, the following encoder control parameters must be set
according to the application needs: LineCaptureMode, LineRateMode, Period_us, PeriodTrim,
LinePitch, EncoderPitch, ConverterTrim, OnMinSpeed, LineTrigCtl, LineTrigEdge, LineTrigFilter,
LineTrigLine, RateDivisionFactor, ForwardDirection and, BackwardMotionCancellationMode.

Customizing Interleaved Acquisition – Strobe Control Parameters

The StrobeLine_P1 and StrobeLine_P2 parameters designate the I/O lines used as strobe
outputs for P1 and P2 respectively. The default values are IOUT1 for P1 and IOUT2 for P2.

Setting StrobeLine_P2 to NONE disconnects the IOUT2 output from the P2 Strobe and makes it
available for another usage (Software controlled I/O).

The StrobeOutput_P1 and StrobeOutput_P2 parameters control the delivery of the strobe pulse
for P1 and P2 respectively. The delivery is enabled by default. Assigning the DISABLE value,
inhibits the delivery of the strobe pulse.

Customizing Grabber Timing Parameters

As for any line-scan application, the following grabber configuration, timing and conditioning
parameters must be set according to the application needs: GrabWindow, WindowX_Px and,
OffsetX_Px.

Customizing Cluster Parameters

As for any line-scan application, the following cluster parameters must be set according to the
application needs: ColorFormat and, ImageFlipX.

227

Grablink Grablink Functional Guide

12.4. Two-line Synchronized Line-scan Acquisition

Applies to:

Introduction

The Two-lineSynchronizedLine-scanAcquisitionfeature takes advantage of a specificity of the
BASLER Sprint bilinear CMOS camera that, when operating in the so-called "Exsync controlled
operation – Level controlled Mode – Enhanced Raw Line A" exposes light once every two Exsync
cycles. For a full description of such camera cycle, refer to "Two-line Camera Cycles" on page
229.

This feature extends the capability of Grablink cards to synchronize multiple line-scan
acquisition channels using the 2-signal SyncBus. For an architectural description, refer to
"System Architecture" on page 231. For a description of the hardware layer and the SyncBus
wiring, refer to "SyncBus Wiring" on page 237.

This feature supports two line capture modes:

● LineCaptureMode = ALL: Take-All-Lines

● LineCaptureMode = TAG: Tag-A-Line

The "two-line synchronized acquisition" feature is available since MultiCam 6.9.8. The Tag-A-
Line mode is available since MultiCam 6.12.

Two-line Camera Cycles

System Architecture

Line Capture Modes

Camera, Illumination and Acquisition controller

SyncBus Wiring

Camfile Template – Take-All-Lines mode

Camfile Template – Tag-A-Line mode

Camfile Customization

Basler spL4096-70kc Camfile for Tag-A-Line mode

229

231

233

235

237

238

241

244

246

228

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Two-line Camera Cycles

Basler Sprint Camera Cycle

A single camera cycle of a Basler Sprint CMOS bilinear line-scan camera operating in the "Exsync
controlled operation – Level controlled Mode – Raw Line A" requires two consecutive Exsync
pulses to be completed:

Basler Sprint camera cycle

The leading (=falling) edge of the first Exsync pulse initiates a new exposure.

The trailing (=rising) edge of the first Exsync terminates the exposure and initiates the readout
of the first line (line A) of the sensor.

The leading (=falling) edge of the second Exsync pulse has no function.

The trailing (=rising) edge of the second Exsync pulse initiates the readout of the second line
(line B) of the sensor.

The sensor integrates light for all pixels simultaneously during the time interval between the
leading and the trailing edge of the first Exsync pulse. The strobe light must be fired during that
time interval.

The sensor doesn't integrate light during the low period of the second Exsync pulse. Firing the
strobe during that interval has no effect on the acquired data.

The camera qualifies each line of image data by the LVAL signal.

The camera delivers also an FVAL pulse surrounding the two LVAL pulses belonging to the same
camera cycle. This allows the frame grabber to unambiguously identify the "line parity"(A or B).

NOTE
The FVAL Length CSR parameter of the Basler Sprint camera must be set to
2.

229

No ExposureLegendStrobeReadout Line AReadout Line BExposureStrobeExsyncLVALFVALBAExposureGrablink Grablink Functional Guide

Phase-shifted Camera Cycles

Phase-shifted camera cycles of two Basler Sprint cameras.

The drawing shows the camera cycles of two Basler Sprint cameras where the Exsync periods
are synchronized with a phase shift of one period of the Exsync signal.

Notice that:

● The 2 cameras are never exposing simultaneously!

● Firing the illumination during the exposure time interval of a camera will not affect the other

camera.

230

Legend1StrobeReadout Line AReadout Line BExposureStrobeExsyncLVALFVAL2StrobeExsyncLVALFVALGrablink Grablink Functional Guide

System Architecture

Two-line synchronized line-scan acquisition system

A two-line synchronized line-scan acquisition system is composed of at least 2 MultiCam
acquisition channels:

● One "Master Channel"

● One or more "Slave Channels"

Each MultiCam acquisition channel includes:

● 1 Basler Sprint bilinear color line-scan camera

● 1 Strobed illumination device

● 1 Camera, Illumination and Acquisition controller (CIAC).

The Master channel includes:

● 1 Camera Trigger Source circuit that generates the SyncBus: Camera Trigger signal.

● 1 Acquisition Enable source circuit that generates the SyncBus: Acquisition Enable signal.

The SyncBus distributes the two signals to the Camera, Illumination and Acquisition Controller
of all participating channels.

The leading edge of the SyncBus: Camera Trigger signal triggers simultaneously all camera and
illumination controllers. Each controller sends an Exsync pulse (MultiCam reset signal) having a
specified width to the camera. It generates also a strobe pulse once every two Exsync.

The leading edge of the SyncBus: Acquisition Enable signal initiates the image data capture on
all channels. The image data capture effectively begins on the next occurrence of a line A
ensuring that the image data capture begins always on a boundary of the 2 x 2 Bayer CFA
pattern.

231

Slave ChannelLVAL, FVAL, DATAIllumination DeviceExsyncCameraStrobeCamera Illumination & Acquisition ControllerMaster ChannelLVAL, FVAL, DATAIllumination DeviceExsyncCameraStrobeCamera Illumination & Acquisition ControllerCamera TriggerAcquisition EnableSync BusAcquisition Enable source circuitCamera Trigger source circuitGrablink Grablink Functional Guide

The falling edge of the SyncBus: Acquisition Enable signal terminates the image data capture
on all channels. The image data capture effectively terminates after the next occurrence of a
line B ensuring that the image data capture terminates always on a boundary of the 2 x 2 Bayer
CFA pattern.

232

Grablink Grablink Functional Guide

Line Capture Modes

Two line capture modes are available: Take-All-Lines and Tag-A-Line.

The following table summarizes the characteristics:

Characteristics

Take-All-Lines

Tag-A-Line

Camera line rate

Captured image
data

Variable, linked to web
speed

Fixed, defined by the frame grabber

All lines

All lines

Line tagging

No

Yes, linked to web speed

Image re-
sampling

Not required

Required to be done by the application using
tag metadata

Take-All-Lines Line Capture Mode

The camera line rate is proportional to the web speed to maintain a fixed pitch along the down-
web web direction. Therefore, the Master Channel elaborates a Camera Trigger signal from the
A/B signals using the Quadrature Decoder and, when necessary, the Rate Divider or the Rate
Converter.

All lines are captured by the frame grabber when the acquisition is enabled. Therefore, the
Master channel elaborates the Acquisition Enable signal from both edges of the position
detector signal.

233

Camera Trigger source circuit – Take-All-Lines mode.Quadrature DecoderA/BOptional Rate Divider or Rate ConverterQMotion EncoderCamera TriggerAcquisition Enable source circuitPresence DetectorDelayDelayTriggerEndTriggerAcquisition EnableGrablink Grablink Functional Guide

Tag-A-Line Line Capture Mode

The camera line rate is fixed. Therefore, the Master Channel elaborates a Camera Trigger signal
using the Periodic Timer.

The down-web line rate is proportional to the web speed to obtain, after re-sampling by the
application , a fixed pitch along the down-web web direction. The Master channel elaborates
also a Tag Trigger signal from the A/B signals using the Quadrature Decoder and the Rate
Divider. This signal will be used by all acquisition controllers to tag the lines of image data lines
to be kept during the down-web re-sampling process.

The Master channel combines both Tag Trigger and Camera Trigger signals for transmission on
the SyncBus.

All lines are tagged and captured by the frame grabber when the acquisition is enabled.
Therefore, the Master channel elaborates the Acquisition Enable signal from both edges of the
position detector signal in the same way as for the Take-All-Lines mode.

The RGB components data of the first pixel of each image line are replaced by a tag indicating if
the line was preceded by a Tag Trigger or not.

All data bits of R,G and B components are set to 1 when a Tag Trigger occurred during the
preceding line interval.

All data bits of R,G and B components are set to 0 when no Tag Trigger occurred during the
preceding line interval.

NOTE
When ColorFormat is set to RGB32, the alpha component is not tagged!

All image data lines are delivered to the application. The application has to perform down-web
re-sampling using tags to obtain an undistorted image with a constant down-web line pitch.

234

Camera Trigger source circuit – Tag-A-Line mode.Periodic TimerCamera Trigger @Camera Line RateQuadrature DecoderA/BRate DividerQMotion EncoderPWMTag Trigger  @Downweb line rateGrablink Grablink Functional Guide

Camera, Illumination and Acquisition controller

Camera and Illumination Controller

Camera and Illumination Controller block diagram

On every Camera Trigger event, the Camera and Illumination Controller generates:

● One single Exsync pulse, having a duration set by the MultiCam parameter Expose_us

● One single Strobe pulse, having a duration and a position set by the MultiCam parameters

StrobeDur and StrobePos.

The width of the Exsync pulse determines the exposure time of the camera. The strobe pulse
duration is entirely located within the exposure time interval. Its position, and its duration are
defined as a percentage of the exposure time.

Two-line Synchronization Mode

The two-line synchronization mode of the CIAC must be enabled by setting the value ENABLE to
the MultiCam Parameter TwoLineSynchronization.

This mode of synchronization ensures that the acquisition gate opens and closes at a line-pair
boundary. It provides also the capability to control the "Line Parity" of the camera by means of
the TwoLineSynchronizationParity parameter:

When set to EVEN, the camera line parity of the local camera is such that the camera cycle
begins at an even line trigger count boundary.

When set to ODD, the camera line parity of the local camera is such that the camera cycle
begins at an odd line trigger count boundary.

235

Strobe GateStrobeAcquisition GateCamera TriggerCamera  & Illumination ControlParity CheckerExposure Time Strobe Position & DurationLine ParityFVAL,  LVALDataAcquired DataAcquisition EnableSync to Line ABLVALLegendSyncBus signalCamera signalIllumination signalConfiguration Setting2-line-synchronizationExsync GateExsyncParity DetectorGrablink Grablink Functional Guide

Line Parity Control

The line parity control is composed of three function blocks:

● Parity Detector

● Parity Checker

● Exsync Gate

The Parity Detector function block analyzes the FVAL and the LVAL signal of the camera and
generates the Camera Line Parity signal. This signal identifies unambiguously the row A and the
row B of the image sensor.

The Parity Checker function block checks whether the Camera Line Parity signal is as expected
according to the TwoLineSynchronizationParity settings.

If the Camera Line Parity is incorrect, the Exsync Gate removes the next Exsync pulse. This
action restores the appropriate line parity.

Strobe Gating

The Strobe Gate removes one strobe pulse every two. It keeps only, the strobe corresponding
to the Exsync cycle where the local camera exposes.

Acquisition Gating

The acquisition gate opens and closes at line-pair boundaries to ensure that buffers always start
with a line A and ends with a line B.

236

LegendReadout Line AReadout Line BLVALFVALLine ParityStrobeExsyncStrobeExposureLegendReadout Line AReadout Line BLVALFVALAcq. GateStrobeExsyncStrobeExposureAcquisition EnableStartStopLine ParityGrablink Grablink Functional Guide

SyncBus Wiring

Isolated I/O SyncBus Wiring Scheme

The Isolated I/O SyncBus is implemented with a custom made wiring interconnecting a selected
set of I/O pins of the internal I/O connector of each MultiCam Channel.

The following diagram shows the interconnections for a 4-channel SyncBus:

Isolated I/O SyncBus wiring diagram

237

Slave ChannelSlave ChannelSlave ChannelMaster Channel+12V25GND26DCIIN3+11IIN3-12IIN3+11IIN3-12IIN3+11IIN3-12IIN3+11IIN3-12IOUT3+19IOUT3-20IIN4+13IIN4-14IIN4+13IIN4-14IIN4+13IIN4-14IIN4+13IIN4-14IOUT4+21IOUT4-22Grablink Grablink Functional Guide

Camfile Template –Take-All-Lines mode

The following section highlights the additions to the generic MyCameraLink_LxxxxRG.cam
camfile for configuring the Master MultiCam Channel of a two-line synchronized line-scan
acquisition system using the Take-All-Lines line capture mode.

;*******************************************************************************************
**
; Camera Manufacturer: Templates
; Camera Model: MyCameraLink
; Camera Configuration: Line-Scan, Grabber-Controlled Rate and Exposure
; Board: Grablink

- WEB, PAGE, or LONGPAGE Acquisition Modes
- Take all lines

- Line-scan camera
- Grabber-controlled rate
- Pulse-Width grabber-controlled exposure

;*******************************************************************************************
**
; This CAM file template is suitable for the following camera configuration:
;
;
;
; This CAM file template is suitable for the following system configuration:
;
;
;
; ********************************************************************************
**
; ** CAUTION:
; ** This file is a template, it can be further customized!
**
; ** The lines that can be edited are marked with an arrow followed by the most **
; ** popular alternate values for that parameter.
**
; ** For a complete list of possible values; refer to MultiCam Studio and/or to **
; ** the MultiCam Reference documentation.
**
; ********************************************************************************
;

;*******************************************************************************************
**
; ==Begin of "Camera properties Section"==
;
; -Camera Specification category-

Camera =
CamConfig =
Imaging =
Spectrum =

MyCameraLink;
LxxxxRG;
LINE;
BW;

;
; -Camera Features category-

TapConfiguration =
TapGeometry =
Expose =
Readout =
ColorMethod =
TwoLineSynchronization = ENABLE;
TwoLineSynchronizationParity = EVEN;

BASE_1T8;
1X;
WIDTH;
INTCTL;
NONE;

;
; --Downstream signals--

FvalMode =
LvalMode =
DvalMode =

FN;
LA;
DN;

;
; --Upstream signals--

ResetCtl =
ResetEdge =
CC1Usage =

DIFF;
GOHIGH;
RESET;

<== BW COLOR ...

<== BASE_1T8 BASE_1T10 BASE_1T24 ...
<== 1X 1X2 2X ...

<== NONE PRISM TRILINEAR RGB

<== EVEN ODD

<== DN DG

<== GOHIGH GOLOW
<== LOW HIGH RESET SOFT DIN1 IIN1

238

Grablink Grablink Functional Guide

CC2Usage =
CC3Usage =
CC4Usage =

LOW;
LOW;
LOW;

;
; -Camera Timing category-

=
Hactive_Px
HSyncAft_Tk =
LineRate_Hz =

4096;
0;
5000;

duration)

<== LOW HIGH RESET SOFT DIN2
<== LOW HIGH RESET SOFT IIN1
<== LOW HIGH RESET SOFT

<==
<==
<== Max. line rate (= reciprocal of readout

ExposeMin_us =

1;

<== Min. exposure time (= RESET signal pulse

width)

ExposeMax_us =

10000;

<== Max. exposure time (= RESET signal pulse

width)

ResetDur =

3000;

<== Min. time interval, in ns, between

consecutive RESET pulses
;
; ==End of "Camera properties Section"==

;*******************************************************************************************
**
; ==Begin of "System properties Section"==
;
; -Acquisition Control category-

SynchronizedAcquisition = MASTER;
AcquisitionMode =
TrigMode =
NextTrigMode =

WEB;
IMMEDIATE;
REPEAT;

<== MASTER, SLAVE, LOCAL_MASTER, LOCAL_SLAVE
<== WEB PAGE LONGPAGE
<== IMMEDIATE HARD SOFT COMBINED
<== REPEAT (when AcquisitionMode = WEB or

LONGPAGE)
;
AcquisitionMode = PAGE)

EndTrigMode =

AUTO;

;

BreakEffect =
SeqLength_Pg =
SeqLength_Ln =

FINISH;
-1;
-1;

LONGPAGE)

<== SAME REPEAT HARD SOFT COMBINED (when

<== AUTO HARD (when AcquisitionMode = LONGPAGE)
<== AUTO (when AcquisitionMode = WEB or PAGE)
<== FINISH ABORT
<== -1 1 .. 65534 (when AcquisitionMode = PAGE)
<== -1 1 .. 65534 (when AcquisitionMode = WEB or

PageLength_Ln =

500;

<== 1 .. 65535

;
; -Trigger Control category-
ISO;
GOHIGH;
MEDIUM;
NOM;

TrigCtl =
TrigEdge =
TrigFilter =
TrigLine =

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;

The following 4 parameters are relevant only when EndTrigMode = HARD!

EndTrigCtl =
EndTrigEdge =
EndTrigFilter =
EndTrigLine =

ISO;
GOLOW;
MEDIUM;
NOM;

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;
; -Exposure & Strobe Control categories-

Expose_us =
ExposeTrim =
StrobeMode =

90;
0;
NONE;

;
; -Encoder Control category-
ALL;
PERIOD;

LineCaptureMode =
LineRateMode =

<==
<==
<== To free the Strobe Output IO port

<== PERIOD PULSE CONVERT

;

;

The following 2 parameters are relevant only when LineRateMode = PERIOD:

Period_us =
PeriodTrim =

1000;
0;

<==
<==

The following 4 parameters are relevant only when LineRateMode = CONVERT:

LinePitch =
EncoderPitch =
ConverterTrim =

100;
100;
0;

<==
<==
<==

239

Grablink Grablink Functional Guide

OnMinSpeed =

IDLING;

<== IDLING MUTING

;

The following 4 parameters are relevant only when LineRateMode = PULSE or CONVERT:

LineTrigCtl =
LineTrigEdge =

DIFF_PAIRED;
ALL_A_B;

<== ISO DIFF ISO_PAIRED DIFF_PAIRED
<== RISING_A FALLING_A ALL_A (when LineTrigCtl =

ISO or DIFF)
;
DIFF_PAIRED)

<== ALL_A_B (when LineTrigCtl = ISO_PAIRED or

LineTrigFilter =
LineTrigLine =

MEDIUM;
NOM;

<== OFF MEDIUM STRONG ...
<== NOM ...

;
LineRateMode = PULSE:

The following parameter controls the Rate divider circuit that is available when

RateDivisionFactor = 1;

<== 1..512
The following 2 parameters are controlling the Backward Motion Cancellation circuit

when LineTrigCtl = ISO_PAIRED or DIFF_PAIRED:

ForwardDirection =
BackwardMotionCancellationMode = OFF;

A_LEADS_B;

<== A_LEADS_B B_LEADS_A
<== OFF FILTERED COMPENSATE

;
; ==End of "System properties Section"==

;*******************************************************************************************
**
; ==Begin of "Grabber properties Section"==
;
; -Grabber Configuration, Timing & Conditioning categories-
<== NOBLACK MAN

GrabWindow =

NOBLACK;

;

The following 2 parameters are relevant only when GrabWindow = MAN:

WindowX_Px =
OffsetX_Px =

2048;
0;

<==
<==

;
that is available
;

;
; -Look-Up Tables category-
;
;
; -Cluster category-
ColorFormat =
ImageFlipX =
ImageFlipY =

Y8;
OFF;
OFF;

LUT configuration parameters can be inserted here if required by the application

<== Y8 Y10 RGB24 RGB24PL ...
<== OFF ON
<== OFF ON

;
; End of "Grabber properties Section"

;*******************************************************************************************
**
; End of File
;=============

240

Grablink Grablink Functional Guide

Camfile Template –Tag-A-Line mode

The following section highlights the additions to the generic MyCameraLink_LxxxxRG.cam
camfile for configuring the Master MultiCam Channel of a two-line synchronized line-scan
acquisition using the Tag-A-Line line capture mode.

;*******************************************************************************************
**
; Camera Manufacturer: Templates
; Camera Model: MyCameraLink
; Camera Configuration: Line-Scan, Grabber-Controlled Rate and Exposure
; Board: Grablink

- WEB, PAGE, or LONGPAGE Acquisition Modes
- Take all lines

- Line-scan camera
- Grabber-controlled rate
- Pulse-Width grabber-controlled exposure

;*******************************************************************************************
**
; This CAM file template is suitable for the following camera configuration:
;
;
;
; This CAM file template is suitable for the following system configuration:
;
;
;
; ********************************************************************************
**
; ** CAUTION:
; ** This file is a template, it can be further customized!
**
; ** The lines that can be edited are marked with an arrow followed by the most **
; ** popular alternate values for that parameter.
**
; ** For a complete list of possible values; refer to MultiCam Studio and/or to **
; ** the MultiCam Reference documentation.
**
; ********************************************************************************
;

;*******************************************************************************************
**
; ==Begin of "Camera properties Section"==
;
; -Camera Specification category-

Camera =
CamConfig =
Imaging =
Spectrum =

MyCameraLink;
LxxxxRG;
LINE;
BW;

;
; -Camera Features category-

TapConfiguration =
TapGeometry =
Expose =
Readout =
ColorMethod =
TwoLineSynchronization = ENABLE;
TwoLineSynchronizationParity = EVEN;

BASE_1T8;
1X;
WIDTH;
INTCTL;
NONE;

;
; --Downstream signals--

FvalMode =
LvalMode =
DvalMode =

FN;
LA;
DN;

;
; --Upstream signals--

ResetCtl =
ResetEdge =
CC1Usage =

DIFF;
GOHIGH;
RESET;

<== BW COLOR ...

<== BASE_1T8 BASE_1T10 BASE_1T24 ...
<== 1X 1X2 2X ...

<== NONE PRISM TRILINEAR RGB

<== EVEN ODD

<== DN DG

<== GOHIGH GOLOW
<== LOW HIGH RESET SOFT DIN1 IIN1

241

Grablink Grablink Functional Guide

CC2Usage =
CC3Usage =
CC4Usage =

LOW;
LOW;
LOW;

;
; -Camera Timing category-

=
Hactive_Px
HSyncAft_Tk =
LineRate_Hz =

4096;
0;
5000;

duration)

<== LOW HIGH RESET SOFT DIN2
<== LOW HIGH RESET SOFT IIN1
<== LOW HIGH RESET SOFT

<==
<==
<== Max. line rate (= reciprocal of readout

ExposeMin_us =

1;

<== Min. exposure time (= RESET signal pulse

width)

ExposeMax_us =

10000;

<== Max. exposure time (= RESET signal pulse

width)

ResetDur =

3000;

<== Min. time interval, in ns, between

consecutive RESET pulses
;
; ==End of "Camera properties Section"==

;*******************************************************************************************
**
; ==Begin of "System properties Section"==
;
; -Acquisition Control category-

SynchronizedAcquisition = MASTER;
AcquisitionMode =
TrigMode =
NextTrigMode =

WEB;
IMMEDIATE;
REPEAT;

<== MASTER, SLAVE, LOCAL_MASTER, LOCAL_SLAVE
<== WEB PAGE LONGPAGE
<== IMMEDIATE HARD SOFT COMBINED
<== REPEAT (when AcquisitionMode = WEB or

LONGPAGE)
;
AcquisitionMode = PAGE)

EndTrigMode =

AUTO;

;

BreakEffect =
SeqLength_Pg =
SeqLength_Ln =

FINISH;
-1;
-1;

LONGPAGE)

<== SAME REPEAT HARD SOFT COMBINED (when

<== AUTO HARD (when AcquisitionMode = LONGPAGE)
<== AUTO (when AcquisitionMode = WEB or PAGE)
<== FINISH ABORT
<== -1 1 .. 65534 (when AcquisitionMode = PAGE)
<== -1 1 .. 65534 (when AcquisitionMode = WEB or

PageLength_Ln =

500;

<== 1 .. 65535

;
; -Trigger Control category-
ISO;
GOHIGH;
MEDIUM;
NOM;

TrigCtl =
TrigEdge =
TrigFilter =
TrigLine =

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;

The following 4 parameters are relevant only when EndTrigMode = HARD!

EndTrigCtl =
EndTrigEdge =
EndTrigFilter =
EndTrigLine =

ISO;
GOLOW;
MEDIUM;
NOM;

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;
; -Exposure & Strobe Control categories-

Expose_us =
ExposeTrim =
StrobeMode =

90;
0;
NONE;

;
; -Encoder Control category-
TAG;
PERIOD;

LineCaptureMode =
LineRateMode =

<==
<==
<== To free the Strobe Output IO port

<= PERIOD

;

;

The following 2 parameters are relevant when LineCaptureMode = TAG:

Period_us =
PeriodTrim =

1000;
0;

<==
<==

The following 4 parameters are relevant only when LineCaptureMode = TAG:

LinePitch =
EncoderPitch =
ConverterTrim =

100;
100;
0;

<==
<==
<==

242

Grablink Grablink Functional Guide

OnMinSpeed =

IDLING;

<== IDLING MUTING

;

The following 4 parameters are relevant only when LineCaptureMode = TAG:

LineTrigCtl =
LineTrigEdge =

DIFF_PAIRED;
ALL_A_B;

<== ISO DIFF ISO_PAIRED DIFF_PAIRED
<== RISING_A FALLING_A ALL_A (when LineTrigCtl =

ISO or DIFF)
;
DIFF_PAIRED)

<== ALL_A_B (when LineTrigCtl = ISO_PAIRED or

LineTrigFilter =
LineTrigLine =

MEDIUM;
NOM;

<== OFF MEDIUM STRONG ...
<== NOM ...

;
LineCaptureMode = TAG:

The following parameter controls the Rate divider circuit that is available when

RateDivisionFactor = 1;

<== 1..512
The following 2 parameters are controlling the Backward Motion Cancellation circuit

when LineTrigCtl = ISO_PAIRED or DIFF_PAIRED:

ForwardDirection =
BackwardMotionCancellationMode = OFF;

A_LEADS_B;

<== A_LEADS_B B_LEADS_A
<== OFF FILTERED COMPENSATE

;
; ==End of "System properties Section"==

;*******************************************************************************************
**
; ==Begin of "Grabber properties Section"==
;
; -Grabber Configuration, Timing & Conditioning categories-
<== NOBLACK MAN

GrabWindow =

NOBLACK;

;

The following 2 parameters are relevant only when GrabWindow = MAN:

WindowX_Px =
OffsetX_Px =

2048;
0;

<==
<==

;
that is available
;

;
; -Look-Up Tables category-
;
;
; -Cluster category-
ColorFormat =
ImageFlipX =
ImageFlipY =

Y8;
OFF;
OFF;

LUT configuration parameters can be inserted here if required by the application

<== Y8 Y10 RGB24 RGB24PL ...
<== OFF ON
<== OFF ON

;
; End of "Grabber properties Section"

;*******************************************************************************************
**
; End of File
;=============

243

Grablink Grablink Functional Guide

Camfile Customization

Camera Parameters

As for any line-scan camera, the following camera parameters must be set according to the
selected camera model:

Spectrum, TapConfiguration, TapGeometry, ColorMethod, DvalMode, ResetEdge, CC1Usage,
CC2Usage, CC3Usage, CC4Usage, Hactive_Px and, HSyncAft_Tk.

For correct operation of the camera trigger overrun protection mechanism it is essential to
carefully set the following parameters:

LineRate_Hz, ExposeMin_us, ExposeMax_us and, ResetDur.

To operate with bilinear line-scan cameras:

● The 2-line synchronization mode must be enabled by setting TwoLineSynchronization to

ENABLE.

● The 2-line synchronization parity must be selected by setting TwoLineSynchronizationParity

to ODD or EVEN.

There is a phase-shift of 1 Exsync cycle between cameras set to ODD and cameras set EVEN.

Acquisition Control Parameters

As for any line-scan application, the following acquisition control parameters must be set
according to the application needs: AcquisitionMode, TrigMode, NextTrigMode, EndTrigMode,
BreakEffect, SeqLength_Pg, SeqLength_Ln and, PageLength_Ln.

The synchronized acquisition feature must be enabled on all synchronized channels. Refer to
"Synchronized Line-scan Acquisition" on page 165.

Trigger Control Parameters

As for any line-scan application, the following trigger and end trigger control parameters must
be set according to the application needs: TrigCtl, TrigEdge, TrigFilter, TrigLine, EndTrigCtl,
EndTrigEdge, EndTrigFilter and, EndTrigLine.

Exposure and Strobe Timing Parameters

As for any line-scan application, the following exposure and strobe control parameters must be
set according to the application needs:

Expose_us, ExposeTrim, StrobeMode, StrobeDur and, PreStrobe_us.

Encoder Control Parameters

As for any line-scan application, the following encoder control parameters must be set
according to the application needs: LineCaptureMode, LineRateMode, Period_us, PeriodTrim,
LinePitch, EncoderPitch, ConverterTrim, OnMinSpeed, LineTrigCtl, LineTrigEdge, LineTrigFilter,
LineTrigLine, RateDivisionFactor, ForwardDirection and, BackwardMotionCancellationMode.

244

Grablink Grablink Functional Guide

Grabber Timing Parameters

As for any line-scan application, the following grabber configuration, timing and conditioning
parameters must be set according to the application needs: GrabWindow, WindowX_Px and,
OffsetX_Px.

Cluster Parameters

As for any line-scan application, the following cluster parameters must be set according to the
application needs: ColorFormat and, ImageFlipX.

245

Grablink Grablink Functional Guide

Basler spL4096-70kc Camfile for Tag-A-Line mode

This topic a customization of the generic MyCameraLink_LxxxxRG.cam camfile for a bilinear
color BaslerspL4096-70kccamera attached to the Master MultiCam Channel of a two-line
synchronized line-scan acquisition using the Tag-A-Line line capture mode.

;*******************************************************************************************
**
; Camera Manufacturer: BASLER
; Camera Model: spL4096-70kc
; Camera Configuration: RAW Dual Line, 2048 pixels, Grabber-Controlled rate and exposure
; Board: Grablink
; Minimum MultiCam Version: 6.5
; Last update: 25 Sept 2017

;*******************************************************************************************
**
;Disclaimer:
;
;These CAM-files are provided to you free of charge and "as is".
;You should not assume that these CAM-files are error-free or
;suitable for any purpose whatsoever.
;Nor should you assume that all functional modes of the camera are
;covered by these CAM files or that the associated documentation is complete.
;EURESYS does not give any representation or warranty that these CAM-files are
;free of any defect or error or suitable for any purpose.
;EURESYS shall not be liable, in contract, in torts or otherwise,
;for any damages, loss, costs, expenses or other claims for compensation,
;including those asserted by third parties, arising out of or in connection
;with the use of these CAM-files.

;*******************************************************************************************
**
;
; ==Begin of "Camera properties Section"==
;
; -Camera Specification category-

Camera =
CamConfig =
Imaging =
Spectrum =

MyCameraLink;
LxxxxRG;
LINE;
COLOR;

;
; -Camera Features category-

TapConfiguration =
TapGeometry =
Expose =
Readout =
ColorMethod =
ColorRegistration= RG;
ColorRegistrationControl= FVAL;

BASE_2T8;
1X2;
WIDTH;
INTCTL;
BAYER;

;
; --Downstream signals--

FvalMode =
LvalMode =
DvalMode =

;
; --Upstream signals--

ResetCtl =
ResetEdge =
CC1Usage =
CC2Usage =

FN;
LA;
DN;

DIFF;
GOLOW;
RESET;
LOW;

<== BW COLOR ...

<== BASE_1T8 BASE_1T10 BASE_1T24 ...
<== 1X 1X2 2X ...

<== NONE PRISM TRILINEAR RGB

<== DN DG

<== GOHIGH GOLOW
<== LOW HIGH RESET SOFT DIN1 IIN1
<== LOW HIGH RESET SOFT DIN2

246

Grablink Grablink Functional Guide

CC3Usage =
CC4Usage =

LOW;
LOW;

;
; -Camera Timing category-

Hactive_Px =
HSyncAft_Tk =
LineRate_Hz =

2048;
0;
50000;

duration)

<== LOW HIGH RESET SOFT IIN1
<== LOW HIGH RESET SOFT

<==
<==
<== Max. line rate (= reciprocal of readout

ExposeMin_us =

2;

<== Min. exposure time (= RESET signal pulse

width)

ExposeMax_us =

10000;

<== Max. exposure time (= RESET signal pulse

width)

ResetDur =

3000;

<== Min. time interval, in ns, between

consecutive RESET pulses

TwoLineSynchronization= ENABLE;
TwoLineSynchronizationParity= EVEN;

;
; ==End of "Camera properties Section"==

;*******************************************************************************************
**
; ==Begin of "System properties Section"==
;
; -Acquisition Control category-

AcquisitionMode =

LONGPAGE;

<== WEB PAGE LONGPAGE

SynchronizedAcquisition= MASTER;

<== MASTER, SLAVE, LOCAL_MASTER, LOCAL_SLAVE

TrigMode =
NextTrigMode =

HARD;
REPEAT;

<== IMMEDIATE HARD SOFT COMBINED
<== REPEAT (when AcquisitionMode = WEB or

LONGPAGE)
;
AcquisitionMode = PAGE)

EndTrigMode =

HARD;

;

BreakEffect =
SeqLength_Pg =
SeqLength_Ln =

ABORT;
-1:
-1;

LONGPAGE)

<== SAME REPEAT HARD SOFT COMBINED (when

<== AUTO HARD (when AcquisitionMode = LONGPAGE)
<== AUTO (when AcquisitionMode = WEB or PAGE)
<== FINISH ABORT
<== -1 1 .. 65534 (when AcquisitionMode = PAGE)
<== -1 1 .. 65534 (when AcquisitionMode = WEB or

PageLength_Ln =

128;

<== 1 .. 65535

;
; -Trigger Control category-
ISO;
GOHIGH;
MEDIUM;
IIN1;

TrigCtl =
TrigEdge =
TrigFilter =
TrigLine =

<== ISO DIFF ...
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;

The following 4 parameters are relevant only when EndTrigMode = HARD!

EndTrigCtl =
EndTrigEdge =
EndTrigFilter =
EndTrigLine =

ISO;
GOLOW;
MEDIUM;
IIN1;

<== ISO DIFF
<== GOHIGH GOLOW
<== OFF ON MEDIUM STRONG
<== NOM ...

;
; -Exposure & Strobe Control categories-

Expose_us =
ExposeTrim =
StrobeMode =

245;
0;
AUTO;

;
; -Encoder Control category-
TAG;
PERIOD;

LineCaptureMode =
LineRateMode =

<==
<==
<== To free the Strobe Output IO port

<== PERIOD PULSE CONVERT

;

The following 2 parameters are relevant only when LineRateMode = PERIOD:

Period_us =
PeriodTrim =

250;
0;

<==
<==

247

Grablink Grablink Functional Guide

;

;

The following 4 parameters are relevant only when LineRateMode = CONVERT:

LinePitch =
EncoderPitch =
ConverterTrim =
OnMinSpeed =

100;
100;
0;
IDLING;

<==
<==
<==
<== IDLING MUTING

The following 4 parameters are relevant only when LineRateMode = PULSE or CONVERT:

LineTrigCtl =
LineTrigEdge =

ISO;
RISING_A;

<== ISO DIFF ISO_PAIRED DIFF_PAIRED
<== RISING_A FALLING_A ALL_A (when LineTrigCtl =

ISO or DIFF)
;
DIFF_PAIRED)

<== ALL_A_B (when LineTrigCtl = ISO_PAIRED or

LineTrigFilter =
LineTrigLine =

MEDIUM;
IIN2;

<== OFF MEDIUM STRONG ...
<== NOM ...

;
LineRateMode = PULSE:

The following parameter controls the Rate divider circuit that is available when

RateDivisionFactor = 4;

<== 1..512
The following 2 parameters are controlling the Backward Motion Cancellation circuit

when LineTrigCtl = ISO_PAIRED or DIFF_PAIRED:

ForwardDirection =
BackwardMotionCancellationMode = OFF;

A_LEADS_B;

<== A_LEADS_B B_LEADS_A
<== OFF FILTERED COMPENSATE

;
; ==End of "System properties Section"==

;*******************************************************************************************
**
; ==Begin of "Grabber properties Section"==
;
; -Grabber Configuration, Timing & Conditioning categories-
<== NOBLACK MAN

GrabWindow =

NOBLACK;

;

The following 2 parameters are relevant only when GrabWindow = MAN:

WindowX_Px =
OffsetX_Px =

4096;
0;

<==
<==

;
that is available
;

LUT configuration parameters can be inserted here if required by the application

;
; -Look-Up Tables category-
;
;
; -Cluster category-
ColorFormat =
ImageFlipX =
ImageFlipY =

RGB24PL;
OFF;
OFF;

<== Y8 Y10 RGB24 RGB24PL ...
<== OFF ON
<== OFF ON

;
; End of "Grabber properties Section"

;*******************************************************************************************
**
; End of File
;=============

248

Grablink Grablink Functional Guide

12.5. Machine Pipeline Control

Applies to:

Pipe-lined Machine Description

Pipeline Controller Description

250

251

249

BaseDualBaseFullFullXRGrablink Grablink Functional Guide

Pipe-lined Machine Description

Mechanical setup

The objects to inspect, for instance labels, are mounted on a tape. The tape is moving
continuously from the input roll to the output roll at a variable speed. A motion encoder,
typically a rotary quadrature motion encoder, is installed on a machine axis.

Every object successively passes under a presence detector, then under the camera and finally
under a marking stage before exiting the machine.

The presence detector is installed upstream from the camera (distance d1).

The camera is a line-scan camera controlled by the frame grabber. It captures lines of image
data at a rate proportional to the motion speed to avoid geometric distortions.

The marking stage is installed downstream from the camera (distance d2). It applies a "not
good" mark on invalid objects. During the d2 travel time, the application analyzes the object
image and determines the action to be performed when it reaches the marking stage.

Normal operation mode

In normal operation mode, the machine executes the following operations for each object:

An object to inspect comes in from the input roll.

The presence detector delivers triggerpulsefor each object

The Grablink frame grabber starts acquiring the first image line when the label reaches the
camera field-of-view, i.e. after a position offset equal to d1.

Grablink stops acquiring after having captured a specified number of lines and wakes-up the
application using the call-back mechanism when the image buffer are filled with the object
image.

The application software analyzes the captured image and asserts a "set action" command
specifying the action to execute when the object reaches the marking stage.

Grablink activates the marking stage, when an invalid object reaches the action stage, i.e. after
a position offset equal to d1 + d2 .

The above operations are pipe-lined to optimize the machine throughput.

250

CameraPresence DetectorMarking StageMotion EncoderInput rollOutput rolld1d2Grablink Grablink Functional Guide

Pipeline Controller Description

DescriptionoftheGrablinkimplementationofthepipelinedmachinecontroller

Functional block diagram

System diagram for machine pipeline control

Camera controller

The camera controller is configured for line-scan grabber-controlled exposure mode: parameter
CamConfig = LxxxxRG.

For every line trigger tick, the controller delivers a RESET pulse having a width equal to the
exposure time set by parameter Expose_us. The RESET pulse is sent to the camera though the
Camera Link CC1 line.

Two options are applicable to select the line trigger source with the parameter LineRateMode:

● When set to PULSE, the encoder ticks are used directly as line triggers.

● When set to CONVERT, the rate converter output ticks are used as line triggers.

251

Application SoftwareGrablinkImage DataMotion EncoderQuadrature DecoderA/BOptionalRate ConverterCamera ControllerLine  TriggerReset CC1 Encoder TicksPresence DetectorTriggerAcquisition ControllerStart-Of-ScanAcquisition GateCaptureBuffersMarking StageActionSet ActionGPIOGPIOPCI ExpressGPIOPipeline controller Camera LinkCameraGrablink Grablink Functional Guide

Rate converter

The rate converter is capable to multiply or divide the input rate of encoder ticks by a ratio of 2
integers defined by parameters LinePitch and EncoderPitch.

It operates within in a limited range of frequencies:

● The upper limit of the output frequency range is user configurable with parameter

MaxSpeed. By default, it is set to the maximum line rate sustainable by the camera defined
by LineRate_Hz.

● The lower limit of the output frequency takes is automatically set by the driver and reported
to the application via the MinSpeed parameter. The MaxSpeed/MinSpeed ratio is typically
greater than 100.

When the input rate drops below MinSpeed, the rate converter behaves according to the
OnMinSpeed settings:

● When set to MUTING, it stops delivering line trigger ticks

● When set to IDLING, it continues delivering line trigger ticks at a constant frequency

To enlarge the usable speed range, it is mandatory to set MaxSpeed at a value slightly above the
actual max camera line rate.

252

Grablink Grablink Functional Guide

Pipeline controller

The pipeline controller is a piece of hardware responsible for the generation of two time-critical
events:

● A Start-of-Scan trigger that initiates the image acquisition of one object when it reaches the

camera.

● An Action pulse on a GPIO output port that triggers the marking stage.

 The pipeline controller monitors the machine with two external sensors:

● A motion encoder that delivers encoder ticks at a rate proportional to the motion speed.

● A presence detector that delivers a trigger pulse for each object.

To generate the Start-of-Scan trigger, the pipeline controller delays the incoming Trigger by a
configurable count of encoder ticks corresponding to the position offset between the detector
and the camera field-of-view (d1).

Similarly, to generate the Action pulse, the pipeline controller delays the incoming Trigger by a
configurable count of encoder ticks corresponding to the position offset between the detector
and the marking stage (d1 + d2). The generation of the Action pulse is conditioned to the Set
action command issued by the application software after image analysis.

The pipeline controller is capable of managing up to 32 objects in the pipeline between the
detector and the marking stage.

253

Encoder TicksTriggerStart-Of-Scan12ActionGoodNot GoodGoodd1d2Image AnalysisImage Acquisition123Result123Set actionINACTIVEACTIVEINACTIVE3123123Grablink Grablink Functional Guide

Acquisition controller & acquisition gate

The acquisition controller is configured for PAGE or LONGPAGE acquisition modes using the
AcquisitionMode parameter :

● When set to PAGE, one MultiCam surface is filled after each start-of-scan trigger.

● When set to LONGPAGE, several MultiCam surfaces are filled after each start-of-scan trigger.

In both cases, the acquisition controller is configured to start acquisition when the hardware
trigger line senses a valid transition or on software command. Parameter TrigModeis set to
COMBINED and parameter NextTrigModeis set to COMBINED).

Parameters TrigCtl, TrigEdge and TrigFilter specify the configuration of the hardware trigger
input line. Parameter TrigLine specifies the location of a hardware trigger input line.

In PAGE acquisition mode, the acquisition controller stops after having acquired a number of
lines specified by PageLength_Ln

In LONGPAGE acquisition mode, the acquisition controller is configured to stop acquisition
automatically by setting parameter EndTrigMode to AUTO. Acquisition stops after having
acquired a fixed number of lines specified by parameter SeqLength_Ln. The parameter
PageLength_Ln specifies the maximum number of lines that can be stored in a buffer. The last
buffer might be incompletely filled. The image data acquisition of an object starts always with a
new buffer.

The acquisition gate opens and close at line boundaries according to the settings of the
acquisition controller.

Packed RGB data acquired from the Camera Link interface are internally buffered and
transmitted as soon as possible, by a DMA engine, to the application buffers in the Host PC
memory via the PCI Express interface.

To minimize latencies, ensure that the PCI Express is capable of sustaining the image data rate
with a comfortable margin.

254

Grablink Grablink Functional Guide

12.6. 8-tap/10-bit Acquisition

Applies to:

Introduction to 8-tap/10-bit Acquisition

8-tap/10-bit MultiCam Tap Configurations

10-bit, 30-bit and 40-bit MultiCam Packed Pixel Formats

256

258

258

255

FullFullXRGrablink Grablink Functional Guide

Introduction to 8-tap/10-bit Acquisition

PC1622 Grablink Full and PC1626 Grablink Full XR acquire images from cameras using the 8-
tap/10-bit variant of the Camera Link 80-bit Configuration.

Three classes of cameras are supported:

● Monochrome cameras delivering 8 consecutive 10-bit pixels for every Camera Link clock

cycle.

● 3-component RGB color cameras delivering 10-bit color components for 8 consecutive pixels

along 3 successive Camera Link clock cycles.

● 4-component RGBI color cameras delivering 10-bit color components for 2 consecutive pixels

for every Camera Link clock cycle.

These classes are identified by new values of the TapConfiguration Channel parameter: DECA_
8T10, DECA_8T30B3 and DECA_2T40.

For such cameras, the frame grabber doesn't unpack 10-bit components to 16-bit. Instead, it
stores the 10-bit pixel components one after the other without inserting any padding bits for
alignment to 16-bit word boundaries. Pixel components are stored using the little-endian
convention: the lsb is stored first.

The following table shows how the first two pixel components are stored in a MultiCam surface:

Byte address

Bit7

Bit6

Bit5

Bit4

+0

+1

+2

PC1 bit7 PC1 bit6 PC1 bit5 PC1 bit4

PC2 bit5 PC2 bit4 PC2 bit3 PC2 bit2

...

...

...

...

Bit3

PC1
bit3

PC2
bit1

PC2
bit9

Bit2

Bit1

Bit0

PC1 bit2 PC1 bit1 PC1 bit0

PC2 bit0 PC1 bit9 PC1 bit8

PC2 bit8 PC2 bit7 PC2 bit6

The storage formats are identified by three new values of the ColorFormat Channel parameter:

● Y10P for 10-bit packed monochrome pixels.

● RGB30P for 30-bit packed 3-component (RGB) color pixels.

● RGBI40P for 40-bit packed 4-component (RGBI) color pixels.

Packing the pixel components reduces the image data size:

● 8 monochrome pixels are stored in 10 bytes.

● 8 RGB color pixels (= 24 components) are stored in 30 bytes.

● 8 RGBI color pixels (= 32 components) are stored in 40 bytes.

NOTE
Reducing the data size allows the frame grabber to deliver the image data
without significant latencies caused by limitations of the available PCI
Express bandwith.

The order of components defined by the camera is preserved in the MultiCam surface:

256

Grablink Grablink Functional Guide

● R (first), G, B (last) for RGB cameras.

● R (first), G, B, I(last) for RGBI cameras.

Insertion of metadata is possible with such cameras:

● One-field metadata insertion is available for area-scan cameras.

● Three-field metadata insertion is available for line-scan cameras.

257

Grablink Grablink Functional Guide

8-tap/10-bit MultiCam Tap Configurations

The following values of the TapConfiguration Channel parameter apply to Camera Link 8-
tap/10-bit cameras:

● DECA_8T10 for monochrome cameras delivering 8-consecutive pixels of 10 bits every Camera

Link clock cycle.

● DECA_8T30B3 for 3-component (RGB) color cameras delivering 10-bit pixel components for 8

consecutive pixels along 3 successive Camera Link clock cycles.

● DECA_2T40 for 4-component (RGBI) color cameras delivering 10-bit pixel components for 2

consecutive pixels every Camera Link clock cycle.

The above values are available only when BoardTopology is set to MONO_DECA or MONO_
DECA_OPT1.

WARNING
The DECA_8T10, DECA_8T30B3 TapConfiguration values authorize only the
1X8 and the 1X8_1Y TapGeometry values.

WARNING
The DECA_2T40 TapConfiguration value authorizes only the 1X2 and the
1X2_1Y TapGeometry values.

WARNING
On-board pixel processing (e.g. look up tables, white balance, image
flipping, image cropping) is unavailable.

10-bit, 30-bit and 40-bit MultiCam Packed Pixel Formats

The following values of the ColorFormat Channel parameter apply to Camera Link 8-tap/10-bit
cameras:

● Y10P for 10-bit packed monochrome pixels.

● RGB30P for 30-bit packed 3-component (RGB) color pixels.

● RGBI40P for 40-bit packed 4-component (RGBI) color pixels.

WARNING
The pixel unpacking is not performed by the Grablink card! It must be done
by the application.

258

Grablink Grablink Functional Guide

12.7. Video Lines Reordering

Applies to:

Grablink frame grabbers are capable to reorder video lines during the transfer from internal
FIFO buffer to the MultiCam buffer.

The reordeing is controlled by two Channel parameters of the Cluster category: FifoOrdering
and FifoOrderingYTapCount.

The following reordering schemes are supported:

PROGRESSIVE

When FifoOrdering is set to PROGRESSIVE, the video data lines delivered by the camera are not
reordered: the line order in the MultiCam buffer corresponds to the line order at the Camera
output and to the line order on the sensor.

This is the default setting automatically invoked by MultiCam.

FifoOrderingYTapCount is irrelevant.

DUALYEND

When FifoOrdering is set to DUALYEND, the video data lines delivered by the camera using the
*_2YE tap geometry are reordered in the progressive order. After re-ordering, the line order in
the MultiCam buffer corresponds to the line order on the sensor.

This setting is enforced by MultiCam for area-scan cameras having a TapGeometry value
terminated by _2YE. Refer to "Image Reconstruction" on page 47.

FifoOrderingYTapCount is forced to 2 and cannot be changed.

259

BaseDualBaseFullFullXRSensorLine 1Line 2Line N-1Line NCamera outputLine 1Line 2Line N-1Line NMultiCam bufferLine 1Line 2Line N-1Line NtimeaddressY-positionSensorLine 1Line 2Line N/2Line N/2 + 1Line N-1Line NCamera outputLine 1Line NLine N/2Line N/2 + 1MultiCam bufferLine 1Line 2Line N-1Line NLine N/2Line N/2 + 1timeaddressY-positionGrablink Grablink Functional Guide

NYTAP

When FifoOrdering is set to NYTAP and FifoOrderingYTapCount is set to any integer value N, the
video data lines delivered by the camera in block of N lines are stored into N separate color
planes in the MultiCam surface.

The above drawing shows an example with FifoOrderingYTapCount = 5.

NOTE
The PENTAYTAP value is kept for backward compatibility.

260

Color 5Color 4Color 3Color 2Color 1Camera outputMultiCam buffertimeaddressLine 1 Color 5Line 1 Color 4Line 1 Color 3Line 1 Color 2Line 1 Color 1Line 2 Color 5Line 2 Color 4Line 2 Color 3Line 2 Color 2Line 2 Color 1Line N Color 5Line N Color 4Line N Color 3Line N Color 2Line N Color 1SensorLine 1 Color 5Line 1 Color 4Line 1 Color 3Line 1 Color 2Line 1 Color 1Line 2 Color 5Line 2 Color 4Line 2 Color 3Line 2 Color 2Line 2 Color 1Line N Color 5Line N Color 4Line N Color 3Line N Color 2Line N Color 1Grablink Grablink Functional Guide

13. Annex

13.1. Interfacing Camera Link Cameras

262

261

Grablink Grablink Functional Guide

13.1. Interfacing Camera Link Cameras

CamFile templates for Camera Link area-scan cameras

Camera description

Progressive-scan, asynchronous reset operation, grabber-controlled
exposure, area-scan camera

CamFile name

MyCameraLink_
PxxRG.cam

MyCameraLink_
PxxRG_IA.cam

Progressive-scan, asynchronous reset operation, camera-controlled
exposure, area-scan camera

MyCameraLink_
PxxRC.cam

Progressive-scan, synchronous operation, camera-controlled
exposure, area-scan camera

MyCameraLink_
PxxSC.cam

Selecting a template for Camera Link area-scan cameras

The majority of cameras used for industrial applications are progressive-scan cameras operating
in the asynchronousresetmode. There are 3 templates for such cameras:

1. When the exposure time of the camera can be controlled by the frame grabber using the
pulse width of the "Reset" signal; it is recommended to select a PxxRG or a PxxRG_IA
templates:

a. With PxxRG, a single exposure and strobe timing program can be configured and

executed repeatedly.

b. With PxxRG_IA, the "Interleaved Acquisition" on page 206 is enabled; two exposure and

strobe timing programs can be configured and executed alternatively.

2.

If using the PxxRG or PxxRG_IA templates is not possible, select the PxxRC template. In that
case, the exposure time needs to be defined by a camera setting. Furthermore, if a strobe
output is also needed, it is necessary to copy the value of the exposure setting of the camera
into the Channel parameter TrueExp_us in order to have the appropriate timing for the
Strobe pulse produced by the Grablink board.

For free-runningcameras, select the PxxSC template.

NOTE
All the templates select a monochrome camera by default. However, they
can be customized in order to support RGB color or Bayer color cameras.

262

Grablink Grablink Functional Guide

CamFile templates for Camera Link line-scan cameras

Camera description

CamFile name

Camera-controlled exposure, triggered readout, line-scan
camera

Grabber-controlled exposure, triggered readout, line-scan
camera

Grabber-controlled rate, permanent exposure, line-scan
camera

Free-running, permanent exposure, line-scan camera

MyCameraLink_LxxxxRC.cam

MyCameraLink_LxxxxRG.cam

MyCameraLink_LxxxxRG_
IA.cam

MyCameraLink_LxxxxRP.cam

MyCameraLink_LxxxxRP_
DR.cam

MyCameraLink_LxxxxSP.cam

MyCameraLink_LxxxxSP_
DR.cam

Selecting a template for Camera Link line-scan cameras

For cameras having noelectronicshutter, it is mandatory to use one of the following templates:
LxxxxSP, LxxxxSP_DR, LxxxxRP and LxxxRP_DR . If the camera allows grabber controlled line
rate, LxxxxRP and LxxxRP_DR offer the capability to control the exposure time using Channel
parameters of the Exposure Control category. LxxxxSP_DR and LxxxxRP_DR templates enable
the DownWebResamplingto control also the vertical resolution.

For camera having an electronicshutter, it is required to use LxxxxRC, LxxxxRG or LxxxxRG_IA
templates. These templates allow a separate control of the camera line rate providing that the
line period remains greater than the exposure time. In addition:

● With LxxxxRG, a single exposure and strobe timing program can be configured and executed

repeatedly.

● With LxxxRG_IA, the "Interleaved Acquisition" on page 206 is enabled; two exposure and

strobe timing programs can be configured and executed alternatively.

Camera Interfaces

TIP
Camera interfaces are available for download from the Supported Cameras
page of the Euresys Web Site.

263


