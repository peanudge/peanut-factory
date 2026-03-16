# MultiCam User Guide (D402EN)

> Source: Euresys MultiCam 6.19.4 (Doc D402EN-MultiCam User Guide-6.19.4.4059)

## Contents

- [PART I: General Topics](#part-i-general-topics)
  - [1. Camera Topics](#1-camera-topics)
  - [2. Frame Grabber Topics](#2-frame-grabber-topics)
  - [3. Line-Scan Inspection Topics](#3-line-scan-inspection-topics)
- [PART II: MultiCam Basics](#part-ii-multicam-basics)
  - [1. MultiCam as a Driver](#1-multicam-as-a-driver)
  - [2. Parameters](#2-parameters)
  - [3. Classes](#3-classes)
  - [4. Code Examples for Objects Management](#4-code-examples-for-objects-management)
  - [5. Acquisition](#5-acquisition)
  - [6. Signaling](#6-signaling)
  - [7. Pixel Format Conversion](#7-pixel-format-conversion)
  - [8. Exceptions](#8-exceptions)
  - [9. CAM Files](#9-cam-files)
- [PART III: Configuration Object](#part-iii-configuration-object)
- [PART IV: Board Object](#part-iv-board-object)
  - [1. The Board Object](#1-the-board-object)
  - [2. Board Information Parameters](#2-board-information-parameters)
  - [3. Input/Output Control Parameters](#3-inputoutput-control-parameters)
- [PART V: Channel Class](#part-v-channel-class)
  - [1. The Channel Class](#1-the-channel-class)
  - [2. Introduction to MultiCam Channels](#2-introduction-to-multicam-channels)
  - [3. Configuring a Channel](#3-configuring-a-channel)
  - [4. Understanding MultiCam Acquisition Phases](#4-understanding-multicam-acquisition-phases)
  - [5. MultiCam Triggering](#5-multicam-triggering)
  - [6. Understanding MultiCam Acquisition Sequences](#6-understanding-multicam-acquisition-sequences)
  - [7. Understanding Automatic Switching](#7-understanding-automatic-switching)
  - [8. Understanding Camera Specification](#8-understanding-camera-specification)
  - [9. Understanding Grabber Specification](#9-understanding-grabber-specification)
  - [10. Using Look-Up Tables](#10-using-look-up-tables)
  - [11. Understanding the Rate Converter](#11-understanding-the-rate-converter)
  - [12. Programming the Exposure Time](#12-programming-the-exposure-time)
  - [13. Channel Parameters User Notes](#13-channel-parameters-user-notes)
- [PART VI: Surface Class](#part-vi-surface-class)
- [PART VII: Appendix](#part-vii-appendix)

---

# PART I: General Topics

## 1. Camera Topics

### 1.1. Common Camera Topics

#### What is Exposure?

In order to build an electrical signal representing the light intensity at a given point of a scene, the CCD sensor photosite should be exposed to the light during a certain amount of time. This amount of time is known as the **exposure time**, also called the **integration time**.

The quantity of electrical charge built during the exposure process is proportional to the incoming light intensity and to the exposure time.

In the CCD sensors that can be used for industrial imaging, all the pixels experience simultaneously the exposure condition. This means that the exposure starting and stopping instants are common to all photosites.

The accurate control of the exposure time is a feature that applies to all line-scan sensors, and to most area-scan sensors. The area-scan CCD sensor type that performs best in this respect is the interline transfer CCD sensor.

#### Analog vs. Digital

**Analog cameras**

An analog camera delivers the signal representing the observed image in the form of an analog signal, named the video signal. The analog signal incorporates several features aimed at providing timing information for the frame grabber to synchronize on it.

Domino series products are able to interface to industrial analog cameras, while Picolo series products are able to interface to standard analog cameras.

**Digital cameras**

A digital camera delivers the signal representing the observed image in the form of a digital signal. The luminance signal can be coded on 8 to 12 bits, and transmitted to the frame grabber through a digital data link.

Grablink series products are able to interface to industrial digital cameras based on the Camera Link standard.

#### Fundamental Synchronization Modes

| Synchronization mode | Description |
|---|---|
| **ANALOG** | The only timing information available from the camera is the composite video signal. |
| **DIGITAL** | The camera delivers the timing information through a set of digital lines. |
| **MASTER** | The camera is due to receive its timing information from the board. The board is the timing master of the camera. |

**On Grablink series:** The digital cameras on Grablink series use exclusively the DIGITAL fundamental synchronization mode.

**On Domino series:** The three fundamental synchronization modes are applicable to the analog cameras attached to a board of the Domino series.

**On Picolo series:** The analog cameras on Picolo series use exclusively the ANALOG fundamental synchronization mode.

### 1.2. Area-Scan Camera Topics

#### Area-scan Exposure and Readout

Let us consider the case of an area-scan camera equipped with a CCD sensor of the interline transfer or frame transfer type. It is useful to consider two successive conditions: frame exposure, followed by frame readout.

Additionally, the area-scan CCD sensor can be temporarily set in a reset condition. In this condition, the light has no effect on the sensitive area of the sensor. Any electrical charge that could be contained in the photosites is cleared.

At some instant, the sensor leaves the reset condition to enter into the exposure condition (also known as integration). During this time, every photosite builds an electrical charge growing at a rate proportional to the light intensity it receives. The longer the exposure time, the larger the electrical charge.

Consequently, increasing the exposure time is a means to increase the light sensitivity of the camera.

The end of the exposure time is marked by a special event usually referred to as the "transfer gate". At this instant, the individual electrical charges built by the photosites of the entire frame are set aside and made ready for transport towards the CCD sensor output.

Simultaneously, the photosites get emptied of any electrical charge. This sets the photosites in the same state as the reset feature does.

The transfer gate duration is short, virtually instantaneous for interline transfer type area-scan CCD sensors.

After the transfer gate, the CCD sensor enters the readout period. This takes a fixed amount of time to extract the individual electrical charges of the entire frame set aside at the transfer gate instant.

#### Exposure Control and Asynchronous Reset

Many industrial grade cameras allow for asynchronous reset while enabling exposure control. This means that, at some asynchronous instant, the camera is forced to enter into an "Expose-Readout" sequence, called an acquisition phase.

The image seen by the camera during the exposure condition is sent out of the camera during the readout condition. It is often advisable to keep the exposure duration short to remove any blurring effect if the observed scene is in movement.

Quite often, it is not possible to overlap the exposure condition of a given cycle with the readout condition of the previous cycle.

#### Exposure Control and Synchronous Scanning

Non-industrial grade cameras do not support the asynchronous reset feature. This means that the frame readout condition repeats itself periodically at some permanent frequency called the "frame rate". This is called the synchronous scanning mode.

This does not preclude some exposure control capability. Most synchronous cameras make possible to overlap the exposure and readout conditions. This feature is often referred to as "electronic shutter".

#### Permanent Exposure

The permanent exposure feature allows for the greatest sensitivity of a synchronously scanned area-scan camera. It is made possible because the transfer gate removes all charge out of the sensor pixels, establishing the right situation for a new exposure condition to take place.

### 1.3. Line-Scan Camera Topics

#### Line-Scan Exposure and Readout

Let us consider the case of a camera equipped with a line-scan CCD sensor. It is useful to consider two successive conditions: line exposure, followed by line readout.

Additionally, the line-scan CCD sensor can be temporarily set in a reset condition. In this condition, the light has no effect on the sensitive area of the sensor. Any electrical charge that could be contained in the photosites is cleared.

At some instant, the sensor leaves the reset condition to enter into the exposure condition (also known as integration). During this time, every photosite builds an electrical charge growing at a rate proportional to the light intensity it receives.

The end of the exposure time is marked by a special event usually referred to as the "transfer gate". At this instant, the individual electrical charges built by the photosites of the entire line are set aside and made ready for transport towards the CCD sensor output.

#### Controlled Exposure

In the controlled exposure situation, the line-scan CCD sensor is brought into the reset condition before entering the exposure condition. It is often said that an "electronic shutter" is used.

The line-scan camera hosting the sensor experiences a periodical cycle paced by successive instants referred to as "line reset". The line reset pulses occur at the line frequency. Each line reset causes the following sequence:

1. Exposure condition of some known duration.
2. Transfer gate event.
3. Readout process.
4. Return to reset condition.

It can be seen that the exposure duration is not equal to the line period. It can be made independent of the line rate.

#### Permanent Exposure

In the permanent exposure situation, the line-scan CCD sensor is never brought into the reset condition.

The line-scan camera hosting the sensor experiences a periodical cycle paced by successive instants referred to as "line reset". The line reset pulses occur at the line frequency. Each line reset causes a transfer gate event followed by a readout process.

Between two successive line reset pulses, the sensor integrates the light. The sensitivity of the camera depends on the line period separating the pulses. The lower the line frequency, the higher the response to a given scene.

#### Camera Operating Modes

There are six operating modes, each identified by a symbol:

**First letter:**
- **R** (Reset): the line-scanning process starts upon receiving a signal from the frame grabber.
- **S** (Synchronous): the line-scanning process is free-running.

**Second letter:**
- **G** (Grabber): the exposure control is exercised from the frame grabber.
- **C** (Camera): the exposure control is exercised from within the camera.
- **P** (Permanent): there is no exposure control, i.e. the exposure is permanent.

> **NOTE:** Some cameras may be configured to behave according to more than one mode. However, in a given application, only one mode is in use.

**Controlled cameras:**

| Symbol | Meaning |
|---|---|
| **RG** | Grabber-controlled line-scanning, grabber-controlled exposure, single signal. Exposure duration defined as the active duration of a pulse over a single line issued by the frame grabber. |
| **RG2** | Grabber-controlled line-scanning, grabber-controlled exposure, dual signal. Exposure duration defined as the active duration of a pulse over a dual line issued by the frame grabber. |
| **RC** | Grabber-controlled line-scanning, camera-controlled exposure. Exposure duration set through camera switches or serial control. Line-scanning is triggered by a pulse over a line issued by the frame grabber. |
| **RP** | Grabber-controlled line-scanning, permanent exposure. No exposure control capability, resulting in permanent exposure. Line-scanning is triggered by a pulse over a line issued by the frame grabber. |

**Free-running cameras:**

| Symbol | Meaning |
|---|---|
| **SC** | Free-running, camera-controlled exposure. Exposure duration set through camera switches or serial control. Line-scanning is free-running. |
| **SP** | Free-running, permanent exposure. No exposure control capability, resulting in permanent exposure. Line-scanning is free-running. |

---

## 2. Frame Grabber Topics

### 2.1. Common Frame Grabber Topics

#### What is a Frame Grabber?

A frame grabber is the usual name given to an electronic board to be installed into a computer and aimed at interfacing a video camera to this computer.

The frame grabber can be categorized in several kinds, according to the internal storage structure:

- Frame buffer-based frame grabbers
- FIFO-based frame grabbers

Another kind of frame grabber is the frame processor, which includes some image processing means.

#### What is a Grabber?

A grabber is a set of hardware resources owned by a frame grabber.

Many Euresys frame grabbers are able to incorporate several grabbers operating simultaneously. This effectively results in several independent frame grabbers within a single board.

When several grabbers cannot be operated simultaneously, it is still possible to handle several cameras. The grabber (or set of grabbers) is used in a time-multiplexed fashion. This is called "grabber switching".

#### Frame Buffer-Based Frame Grabber

The grabber is a set of hardware resources taking in charge all timing and control tasks required by the camera, and conditioning the video data (analog or digital) provided by the camera.

The frame buffer is an internal storage area large enough to hold a full frame image issued by the camera. In case of line-scan operation, the frame is actually made of a set of contiguous lines, and this set is called a page.

The DMA controller is a device able to transfer the stored image from the frame buffer into the host computer memory in a DMA (Direct Memory Access) fashion.

The destination area in the on-board or host memory is called a "surface". A surface is a memory container able to store a bi-dimensional image corresponding to a frame (area-scan) or a page (line-scan).

In the frame buffer-based frame grabber, the camera-to-buffer transfer can be decoupled from the buffer-to-surface transfer.

#### FIFO-Based Frame Grabber

The FIFO buffer is an internal storage area able to hold a part of the image issued by the camera, usually a few video lines. FIFO means "First In, First Out".

In the FIFO-based frame grabber, the camera-to-buffer transfer cannot be decoupled from the buffer-to-surface transfer. The goal of the grabber is to directly feed the host memory.

#### Concurrent Acquisition Modes

The names DuoCam and TrioCam are given to the concurrent acquisition modes.

All hardware resources involved in the image acquisition process exist in two instances within the frame grabber. This allows for full independent and simultaneous operation of two acquisition channels.

The TrioCam mode is similar to this, with three acquisition channels instead of two.

#### Switched Acquisition Mode

One frame at a time is extracted out of an individual camera, and all cameras are sequentially scanned according to controlled rules.

The switched acquisition mode is in no manner limited to two cameras.

### 2.2. Area-Scan Frame Grabber Topics

#### Trigger, Reset and Strobe in Area-Scan

- A **trigger pulse** (or frame trigger pulse) is an electrical signal sent by the external system to instruct a frame grabber to take control over the camera, including exposure control, and to perform a frame acquisition.
- A **reset pulse** (or frame reset pulse) is an electrical signal sent by the frame grabber to instruct an area-scan camera to start its frame acquisition cycle.
- A **strobe pulse** is an electrical signal sent by the frame grabber to control an external illumination device.

#### Area-Scan Camera and System Relationship

The external system determines that a scene is to be captured. It sends a trigger pulse to the frame grabber. As a reaction, the frame grabber has the following mission:

- To adequately instruct the camera to capture the image as quickly as possible.
- To retrieve the image produced by the camera.
- To deposit the image in digital format into a memory location inside the computer hosting the frame grabber.

In response to the external trigger, the frame grabber sends an asynchronous reset command to the camera, and enters a waiting phase. Simultaneously, the camera enters the frame exposure condition.

#### Area-Scan Operational Modes

Four fundamental camera modes are applicable to the area-scan camera and grabber association.

| Operational mode | Expose | Readout | Camera mode description |
|---|---|---|---|
| **SC** | INTCTL | INTCTL | The camera operates in the "synchronous scanning" modality. The light exposure duration is set by the camera. |
| **SP** | INTPRM | INTCTL | The camera operates in the "synchronous scanning" modality. The exposure is permanent. |
| **RC** | PLSTRG | INTCTL | The camera operates in the "asynchronous reset" modality. The light exposure duration is set by the camera. |
| **RG** | WIDTH | INTCTL | The camera operates in the "asynchronous reset" modality. The frame grabber positively controls the camera light exposure duration through `Expose_us`. |

#### Area-Scan Acquisition Modes

| Acquisition Mode | Short Description |
|---|---|
| **VIDEO** | Intended for the acquisition of several video sequences from a standard area-scan camera. |
| **SNAPSHOT** | Intended for the acquisition of snapshot images. |
| **HFR** | Intended for the acquisition of snapshot images from high frame rate cameras. |

### 2.3. Line-Scan Frame Grabber Topics

*Applies to: Grablink*

#### Trigger, Reset and Strobe in Line-Scan

- A **trigger pulse** is an electrical signal sent by the external system to synchronize the line rate of the camera to some external reference.
- A **page trigger pulse** is an electrical signal sent by the external system to instruct a frame grabber to perform the acquisition of a set of several successive lines.
- A **reset pulse** is an electrical signal sent by the frame grabber to instruct a line-scan camera to start its line acquisition cycle.

In the case of line-scan, an adjacent set of scanned lines is called a **page**.

#### Line-Scan Camera and System Relationship

At any time, the frame grabber controls the line sequencing of the camera. To achieve this, a succession of line-reset pulses is sent to the camera at a frequency proportional to the repetition frequency of trigger pulses sent by a motion encoder. This is the effect of the built-in rate converter.

The external system determines that a moving object is about to reach the camera field of view. A page trigger pulse is issued. As a reaction, the frame grabber enters a programmable delay, called the page delay (expressed as a number of scan lines).

#### Line-Scan Operational Modes

Six fundamental operational modes are applicable to the line-scan camera and grabber association.

| Operational mode | Expose | Readout | Camera mode description |
|---|---|---|---|
| **SC** | INTCTL | INTCTL | Free-running, camera-controlled exposure. |
| **SP** | INTPRM | INTCTL | Free-running, permanent exposure. |
| **RP** | INTPRM | PLSTRG | Asynchronous reset, permanent exposure. |
| **RC** | PLSTRG | INTCTL | Asynchronous reset, camera-controlled exposure. |
| **RG2** | PLSTRG | PLSTRG | Asynchronous reset, grabber-controlled exposure, dual signal. |
| **RG** | WIDTH | INTCTL | Asynchronous reset, grabber-controlled exposure via `Expose_us`. |

#### Line-Scan Acquisition Modes

| Acquisition Mode | Short Description |
|---|---|
| **WEB** | Intended for image acquisition of single continuous objects of any size. |
| **PAGE** | Intended for image acquisition of multiple discrete objects having a fixed size (up to 65,535 lines). |
| **LONGPAGE** | Intended for image acquisition of multiple discrete objects having a variable and/or larger size (up to 2,147,483,648 lines). Supports "Page Cover" signal. |

#### Line Capture Modes

| LineCaptureMode | Description |
|---|---|
| **ALL** | "Take-All-Line" mode. The board acquires all lines delivered by the camera. This is the default. |
| **PICK** | "Pick-A-Line" mode. Each pulse at the downweb line rate determines acquisition of the next line. Allows the camera to be operated at a constant line rate while acquiring lines at a variable downweb line rate. |
| **TAG** | "Tag-A-Line" mode. The camera runs at a constant rate determined by `Period_us`. The frame grabber captures all lines and replaces the first pixel data by a tag indicating a hardware event. |

#### Line Rate Modes

| LineRateMode | Description |
|---|---|
| **CAMERA** | The Downweb Line Rate is originated by the camera. Applicable exclusively for free-run permanent exposure (SP) class when `LineCaptureMode = ALL`. |
| **PULSE** | The Downweb Line Rate originates from a train of pulses applied on the line trigger input. |
| **CONVERT** | The Downweb Line Rate originates from pulses processed by a rate converter belonging to the grabber. |
| **PERIOD** | The Downweb Line Rate originates from an internal periodic generator. |
| **EXPOSE** | The Downweb Line Rate is established by the exposure time settings. Applicable for RP class when `LineCaptureMode = ALL`. |

---

## 3. Line-Scan Inspection Topics

### 3.1. A Simplified View of a Typical Setup

A line-scan camera transforms the light intensity along a line into a time varying video signal.

A line-scan based system usually observes a continuous material or set of objects exhibiting a regular shifting movement (the "inspected web").

Two directional references are introduced:

- The **downweb direction**: the motion direction of the inspected web (also called the axial direction).
- The **crossweb direction**: the axis of the observed line (also called the transverse direction).

### 3.2. The Video Line

The observed line is divided into a set of aligned pixels. In the case of a digital line-scan camera, the produced video signal is a set of digital values corresponding to the light intensity measured for each consecutive pixel.

The observed line is repetitively scanned pixel after pixel from left to right, and the corresponding light intensity is translated into a set of digital numbers output at the pixel frequency.

A fixed number of pixels is output for each line. The operation of outputting a full line is called the readout process, which is periodically repeated at the line frequency.

### 3.3. The CCD Sensor

A line CCD sensor is a set of light sensitive elements aligned on a small piece of silicon. Each element corresponds to a pixel. The CCD pixel crossweb dimension is also called the CCD pitch.

Usually, the CCD pixel has the same crossweb and downweb dimension (square sensitive area). A typical size is in the order of magnitude of 10 x 10 um.

The number of pixels for currently available CCD sensors and line-scan cameras ranges from 200 to 12,000.

### 3.4. The Optical Setup

A lens is used to project the image of the observed line over the sensitive part of the CCD sensor. The most prevalent attribute of the optical setup is the magnification ratio.

### 3.5. Basic Resolution Issues

**Transverse Resolution:** The crossweb resolution is determined by the distance between two adjacent pixels on the inspected web in the crossweb direction ("crossweb pitch"). It depends only on the CCD sensor geometry and the optical setup.

**Axial Resolution:** The downweb resolution is determined by the distance between two adjacent pixels in the downweb direction ("line pitch"). Contrary to the crossweb resolution, it is NOT dependent on CCD sensor geometry nor optical setup. Instead it depends on the line frequency and web speed:

```
line_pitch = web_speed / line_frequency
```

**Aspect Ratio:**

In general, the line pitch is not equal to the crossweb pitch, so the digital image may not be geometrically exact.

```
aspect_ratio = crossweb_pitch / line_pitch
```

If the web speed and the camera line frequency are such that the line pitch is larger than the crossweb pitch, the resulting image will appear compressed in the downweb direction.

### 3.6. Exposure Issues

**Electronic Shutter:** The CCD sensor can be temporarily set in a "pixel reset" condition, then enters exposure/integration. The end of exposure is marked by a "transfer gate" event, after which the readout period begins.

**Permanent Exposure:** The sensor is never brought into the pixel reset condition. Sensitivity depends on the line period. Lower line frequency = higher response.

**Controlled Exposure:** The sensor IS brought into pixel reset before exposure. The exposure duration can be made independent of the line frequency. This is the fundamental reason why the industrial vision integrator should specify the electronic shutter feature when selecting a line-scan camera.

### 3.7. Line Frequency Limits

**Duration of the Readout Process:** Depends on the pixel frequency and the number of pixels. Example: 2048 pixels at 4 MHz = 512 us readout duration.

**Line Period for Permanent Exposure:** The maximum line frequency is dictated by the readout duration. Successive readout processes cannot overlap.

**Line Period for Controlled Exposure:**
- **Short exposure time:** Maximum line frequency is dictated by the camera (readout process duration).
- **Long exposure time:** If exposure is longer than readout duration, then maximum line frequency is dictated by the exposure time.

### 3.8. Triggering a Line-Scan Camera

**Internal Line Triggering** (`LineRateMode = PERIOD`): The trigger pulse sequence is issued by the Grablink frame grabber internally at a pre-defined rate. Use `Period_us` to set the desired rate. The `PeriodTrim` parameter allows modification using a logarithmic scale from -6 dB to +12 dB.

| Setting | Effective trimmed period |
|---|---|
| -6 | `Period_us` x 0.5 |
| -3 | `Period_us` x 0.7 |
| 0 | `Period_us` |
| +3 | `Period_us` x 1.4 |
| +6 | `Period_us` x 2 |
| +9 | `Period_us` x 2.8 |
| +12 | `Period_us` x 4 |

**External Line Triggering** (`LineRateMode = PULSE`): The trigger pulse generated by the frame grabber is a copy of a system trigger pulse applied through a dedicated hardware line. Use `LineTrigCtl` for signal style and `LineTrigFilter` for noise removal.

**Rate Converter-Based Triggering** (`LineRateMode = CONVERT`): The rate (frequency) of system pulses is converted into a pulse train towards the camera at a different rate. The ratio is freely programmable. See [Understanding the Rate Converter](#11-understanding-the-rate-converter).

---

# PART II: MultiCam Basics

## 1. MultiCam as a Driver

### Multiple Grabber Servicing

A grabber is a set of hardware resources able to handle the task of grabbing a frame from a camera. A board can incorporate one or several simultaneous grabbers.

Up to eight boards can be used simultaneously in MultiCam. Only one MultiCam driver must be installed to give an application control over all the grabbers.

A **channel** is a set of hardware and software resources able to condition and transport the image from the camera into a PC memory surface. Once created, the channel is perceived by the user as a uniform image acquisition chain controlled by its own set of MultiCam parameters.

### Multiple Application Servicing

Several applications can be launched, each having the ability to interact with the common MultiCam driver. Any application can create as many channels as needed, using:

- Any MultiCam compliant board within the system
- Any grabber within the selected board
- Any camera connected to the selected grabber
- Any memory surface in the computer memory

Memory surfaces can be shared by several channels, and cameras can feed several channels.

### Connecting and Disconnecting the Driver

Before using any MultiCam function, communication must be established with `McOpenDriver`. Before terminating, disconnect with `McCloseDriver`.

```c
// Connecting to driver
MCSTATUS Status = McOpenDriver(NULL);

// ... Application code ...

// Disconnecting from driver
Status = McCloseDriver();
```

---

## 2. Parameters

### 2.1. Parameter Types

| Type | Description |
|---|---|
| **Integer** | Coded as a 32-bit signed integer value. |
| **Floating-point** | Coded as a 64-bit floating-point value. |
| **String** | An ordered set of ASCII characters. |
| **Enumerated** | Holds a particular value chosen among a set of known possibilities. |
| **Instance** | Hosts a handle designating the instance of a class object. |

Some parameters exist as a **collection** -- a container hosting several values of the same type, accessed via a single parameter name.

### 2.2. Parameter Name and Parameter Identifiers

Each MultiCam parameter has a **name** (alphanumeric string, case-insensitive) and **identifiers** defined in the header file `McParams.h` (included by `MultiCam.h`).

Each parameter also has a **numerical identifier** (integer). For example, for `BoardTopology`:

| Language | Identifier |
|---|---|
| C, C++ | `MC_BoardTopology` |
| .NET | `MC.BoardTopology` |
| Numerical | `59` |

Enumerated values also have identifiers. For `BoardTopology`:

| Value | Identifier |
|---|---|
| MONO | `MC_BoardTopology_MONO` |
| DUO | `MC_BoardTopology_DUO` |

### 2.3. By-Name vs. By-Identifier Access

Two ways to refer to parameters:

| | By-ident access | By-name access |
|---|---|---|
| Method | Static -- identifiers prepared offline via header file | Dynamic -- reference established at runtime |
| Syntax checking | Compiler can check identifier spelling | No compile-time checking |
| Performance | Faster -- direct access | Slower -- requires string interpretation |

### 2.4. Parameter Levels

| Level | Definition |
|---|---|
| **Select** | Addresses a fundamental feature of hardware or software behavior. |
| **Adjust** | Addresses an operative mode for the fundamental feature. |
| **Expert** | Can be used by experienced users to modify operative mode details. |

An application can change any parameter regardless of its level.

### 2.5. Inference Rules

Channel class parameters exhibit a powerful dependency behavior. The value of any channel parameter is made dependent on one or several higher-level channel parameters. Each time a parameter changes, all dependent parameters are automatically updated.

- **Entry parameter:** Has no inference rule; must receive a value from deliberate setting.
- **Ruled parameter:** Automatically updated according to inference rules, but can still receive deliberate settings.

### 2.6. Code Examples for Parameters Management

**How to Set an Enumerated Parameter?**

```c
// By-ident parameter and by-ident enumerated method
Status = McSetParamInt(ObjectHandle, MC_Camera, MC_Camera_CAM2000);

// By-name parameter and by-ident enumerated method
Status = McSetParamNmInt(ObjectHandle, "Camera", MC_Camera_CAM2000);

// By-ident parameter and by-name enumerated method
Status = McSetParamStr(ObjectHandle, MC_Camera, "CAM2000");

// By-name parameter and by-name enumerated method
Status = McSetParamNmStr(ObjectHandle, "Camera", "CAM2000");
```

**How to Get a Collection Parameter?**

```c
INT32 MyEvent;

// By-ident method
Status = McGetParamInt(ObjectHandle, MC_SignalEvent+1, &MyEvent);

// By-name method
Status = McGetParamNmInt(ObjectHandle, "SignalEvent:1", &MyEvent);
```

---

## 3. Classes

### 3.1. What Is a Class?

A MultiCam class is a container for MultiCam parameters. Each class is divided into categories containing sets of parameters serving a common goal.

| Class | Definition |
|---|---|
| **Configuration** | Controls the common features of the MultiCam system. |
| **Board** | Controls the common features of each board. |
| **Channel** | Defines and controls every individual acquisition path. |
| **Surface** | Defines every image container defined inside the MultiCam system. |

Creating an object = instantiating a class. Effects:

- A new object belonging to the class exists.
- This object has its own designating handle.
- An additional set of parameters owned by the object is created.

**Board** and **Configuration** objects cannot be created by a user application -- they natively exist when the application connects to the MultiCam driver. **Channel** and **Surface** classes can be instantiated as many times as needed.

---

## 4. Code Examples for Objects Management

### 4.1. How to Create and Delete Channels

```c
// Connecting to driver
MCSTATUS Status = McOpenDriver(NULL);

// Instantiating first channel (by-handle method)
MCHANDLE MyChannel1;
Status = McCreate(MC_CHANNEL, &MyChannel1);
Status = McSetParamInt(MyChannel1, MC_DriverIndex, 0);
Status = McSetParamInt(MyChannel1, MC_Connector, MC_Connector_A);

// Instantiating second channel (by-name method)
MCHANDLE MyChannel2;
Status = McCreateNm("CHANNEL", &MyChannel2);
Status = McSetParamNmInt(MyChannel2, "DriverIndex", 0);
Status = McSetParamNmStr(MyChannel2, "Connector", "B");

// ... Application code ...

// Deleting the channels
Status = McDelete(MyChannel1);
Status = McDelete(MyChannel2);

// Disconnecting from driver
Status = McCloseDriver();
```

### 4.2. How to Create and Delete Surfaces

```c
// Connecting to driver
MCSTATUS Status = McOpenDriver(NULL);

// Instantiating surfaces
MCHANDLE MySurface1, MySurface2, MySurface3;
Status = McCreate(MC_DEFAULT_SURFACE_HANDLE, &MySurface1);
Status = McCreate(MC_DEFAULT_SURFACE_HANDLE, &MySurface2);
Status = McCreate(MC_DEFAULT_SURFACE_HANDLE, &MySurface3);

// ... Application code ...

// Deleting all surfaces
Status = McDelete(MySurface1);
Status = McDelete(MySurface2);
Status = McDelete(MySurface3);

// Disconnecting from driver
Status = McCloseDriver();
```

---

## 5. Acquisition

### 5.1. Overview of a Simplified Acquisition Model

A grabber can be sourced by a set of cameras, among which only one is active at a time. The source selector is a programmable device.

The MultiCam system provides a specific object called the **surface** (an instance of the Surface class) to represent a memory buffer. Surfaces are grouped into a **cluster of surfaces**.

A **channel** is the temporary association of a grabber connected to a camera delivering data to a destination cluster. The channel transports an image from the camera towards one of the surfaces ("filling surface").

### 5.2. Acquisition Phase

An acquisition phase is the constitutive element of the acquisition sequence. Its purpose is to fill one surface of the destination cluster.

- **Area-scan (normal speed):** The image data is a frame.
- **Area-scan (high-speed):** Multiple frames stored in a single surface.
- **Line-scan:** The image data is a page (number of lines chosen by `PageLength_Ln`).

An acquisition phase starts at **SAP** (Start of Acquisition Phase) and ends at **EAP** (End of Acquisition Phase).

### 5.3. Acquisition Sequence

An acquisition sequence is a succession of acquisition phases. Intervening gaps of various duration can be present between phases.

Starts at **SAS** (Start of Acquisition Sequence) and ends at **EAS** (End of Acquisition Sequence).

**Speeding-Up the Start of Acquisition Sequence:**

Normally, starting takes less than one millisecond. However, it can be longer on first activation or after format-related parameter changes.

Setting `ChannelState` to `READY` (from IDLE) prepares everything so the next activation to ACTIVE is immediate (< 1ms).

### 5.4. Acquisition Method

Each channel is controlled individually. For switched acquisitions, an automatic switching mechanism handles optimized time sharing of grabber resources.

### 5.5. Multiple Acquisition Buffer Management

#### Cluster of Surfaces

A cluster of surfaces is associated to any channel object. The cluster mechanism unifies control for single, double, triple buffer, and image sequence acquisition.

#### Single Buffer Acquisition

One surface -- written to at each acquisition phase regardless of processing state. Risk of **surface alteration** (processing reads data from multiple acquisition phases).

#### Double Buffer Acquisition

Two surfaces -- useful as long as processing is shorter than the period between acquisition phases. Still a risk of alteration with long processing tasks.

#### Triple Buffer Acquisition

Three surfaces -- the radical way to guarantee an unaltered surface:

- One surface is being processed
- Another surface is experiencing acquisition
- The third is ready for processing

Surface alteration phenomenon is suppressed.

#### Image Sequence Acquisition

Create as many surfaces as wanted and include them in the destination cluster. The cluster mechanism sequentially stores successive images.

#### Registering Surfaces to the Destination Cluster

Use the `Cluster` instance collection parameter: `MC_Cluster`, `MC_Cluster+1`, `MC_Cluster+2`, etc. Each item is loaded with the handle of the constitutive surface.

#### Surface Index

Registering surfaces implicitly assigns a zero-based **surface index**. Available through the `SurfaceIndex` parameter.

### 5.6. Cluster Mechanism

#### Surface States

| SurfaceState | Description |
|---|---|
| **FREE** | Unconditionally able to receive image data from the grabber. |
| **FILLING** | Presently receiving or ready to receive image data. |
| **FILLED** | Has finished receiving image data; ready for processing. |
| **PROCESSING** | Being processed by the host processor. |
| **RESERVED** | Removed from the standard state transition mechanism. |

The state is unique -- a surface belonging to several clusters is perceived consistently by all associated channels.

#### Cluster State

| Cluster state | Meaning |
|---|---|
| **OFF** | Acquisition is not alive. |
| **READY** | Acquisition is alive, no surface is PROCESSING, can accept new acquisition. |
| **BUSY** | Acquisition is alive and one surface is PROCESSING. |
| **UNAVAILABLE** | Acquisition is alive but cannot accept new acquisition (exceptional). |

#### Surface Allocation Rules

**MultiCam Limitations (Grablink):**
- Maximum 4096 surfaces instantiated within an application
- Maximum 4096 surfaces assigned to a channel
- Maximum 4096 surfaces per board

**Operating System Limitations:**
- Maximum 2 GB per MultiCam surface (Windows and Linux)

**Dynamic DMA (Grablink boards):**
- PC1622 Grablink Full / PC1626 Full XR: max ~4,000,000 descriptors per surface
- PC1623 Grablink DualBase (per channel): max ~2,000,000 descriptors per surface
- PC1624 Grablink Base: max ~2,000,000 descriptors per surface

#### Controlling Events

| Event | Meaning |
|---|---|
| Creation | Issued when a surface is instantiated. |
| SAS | Beginning of acquisition sequence. |
| EAP | End of acquisition phase. |
| Exit Callback | Generated when callback function returns. |
| Cluster Ready | Determines surface state transition towards PROCESSING. |
| Software Action | Deliberate user action on `SurfaceState` parameter. |

#### Sourced Signals

| Signal | Meaning |
|---|---|
| Surface Processing | Surface enters state PROCESSING. |
| Surface Filled | Surface enters state FILLED. |
| Cluster Unavailable | Acquisition phase terminates without a destination surface. |

#### State Diagram

Simplified state transitions:

```
Creation --> FREE
FREE --> FILLING           (on SAS or End of Transfer Phase)
FILLING --> FILLED         (on End of Transfer Phase)
FILLED --> PROCESSING      (on McGetSignalInfo, McWaitSignal release, or callback entry)
PROCESSING --> FREE        (on SurfaceState set to FREE, or callback exit)
FILLED/RESERVED --> FREE   (on SurfaceState set to FREE)
FILLED/PROCESSING --> RESERVED  (on SurfaceState set to RESERVED)
```

For a cluster of N surfaces:
- 0 to N surfaces can be FREE
- 0 or 1 surface can be FILLING (or up to 512 when `MaxFillingSurfaces = MAXIMUM`)
- 0 to N surfaces can be FILLED
- **At most 1 surface can be PROCESSING per cluster**
- At least 2 surfaces should be left outside RESERVED state

#### Filling Index and Next Index Evaluation

The filling index designates the current or next FILLING surface. After each acquisition, MultiCam looks at each surface starting from current filling index, wrapping around, and selects the first FREE surface.

If no FREE surface found, the oldest FILLED surface is recycled. If no surface available at all, the **Cluster Unavailable** signal is issued.

### 5.7. Acquisition Code Sample

```c
// Connecting to driver
MCSTATUS Status = McOpenDriver(NULL);

// Instantiating first channel
MCHANDLE MyChannel1;
Status = McCreateNm("CHANNEL", &MyChannel1);
Status = McSetParamInt(MyChannel1, MC_DriverIndex, 0);
Status = McSetParamInt(MyChannel1, MC_Connector, MC_Connector_A);
// Configure first channel...

// Instantiating second channel
MCHANDLE MyChannel2;
Status = McCreateNm("CHANNEL", &MyChannel2);
Status = McSetParamInt(MyChannel2, MC_DriverIndex, 0);
Status = McSetParamInt(MyChannel2, MC_Connector, MC_Connector_B);
// Configure second channel...

// Activating acquisition sequence for first channel
Status = McSetParamInt(MyChannel1, MC_ChannelState, MC_ChannelState_ACTIVE);

// ... Application code ...

// Activating acquisition sequence for second channel
Status = McSetParamInt(MyChannel2, MC_ChannelState, MC_ChannelState_ACTIVE);

// ... Application code ...

// Deleting the channels
Status = McDelete(MyChannel1);
Status = McDelete(MyChannel2);

// Disconnecting from driver
Status = McCloseDriver();
```

---

## 6. Signaling

### 6.1. MultiCam Signals

#### What Is a MultiCam Signal?

A signal is an entity representing a particular event issued from a channel and able to interact with the application. Three synchronization mechanisms are provided:

| Mechanism | Description |
|---|---|
| **Callback signaling** | A user-written function automatically called when a pre-defined signal occurs. |
| **Waiting signaling** | A thread waits for the occurrence of a pre-defined signal. |
| **Advanced signaling** | User-defined mechanism involving standard Windows wait functions. |

#### Signal Identifier

Signals are referred to by identifiers defined in `MultiCam.h`. The identifier is built from the descriptive text prefixed with `MC_SIG_`, with words separated by underscores.

Example: Signal "Surface Processing" = `MC_SIG_SURFACE_PROCESSING`

#### List of MultiCam Signals Issued by Channels

| Signal identifier | Description |
|---|---|
| `MC_SIG_FRAMETRIGGER_VIOLATION` | Frame trigger detected too early for correct hardware handling. |
| `MC_SIG_START_EXPOSURE` | Beginning of the frame exposure condition. |
| `MC_SIG_END_EXPOSURE` | End of the frame exposure condition. |
| `MC_SIG_RELEASE` | Object may be moved away from the camera at end of exposure. |
| `MC_SIG_SURFACE_FILLED` | A surface of the destination cluster enters state FILLED. |
| `MC_SIG_SURFACE_PROCESSING` | A surface of the destination cluster enters state PROCESSING. |
| `MC_SIG_CLUSTER_UNAVAILABLE` | Destination cluster cannot receive the acquired data. |
| `MC_SIG_ACQUISITION_FAILURE` | Channel acquisition timeout timer expires before end of acquisition phase. |
| `MC_SIG_START_ACQUISITION_SEQUENCE` | Acquisition sequence begins. |
| `MC_SIG_END_ACQUISITION_SEQUENCE` | Acquisition sequence terminates. |
| `MC_SIG_END_CHANNEL_ACTIVITY` | Channel leaves the active state. |

#### Enabling Signals

Use the `SignalEnable` collection parameter. Each item enables/disables a specific signal (ON or OFF).

```c
// Enable the "Surface Filled" signal
Status = McSetParamInt(
    my_Channel,
    MC_SignalEnable + MC_SIG_SURFACE_FILLED,
    MC_SignalEnable_ON
);
```

Compound parameter identifiers for each signal:

| Signal | Parameter identifier |
|---|---|
| Frame Trigger Violation | `MC_SignalEnable + MC_SIG_FRAME_TRIGGER_VIOLATION` |
| Start Exposure | `MC_SignalEnable + MC_SIG_START_EXPOSURE` |
| End Exposure | `MC_SignalEnable + MC_SIG_END_EXPOSURE` |
| Release | `MC_SignalEnable + MC_SIG_RELEASE` |
| Surface Filled | `MC_SignalEnable + MC_SIG_SURFACE_FILLED` |
| Surface Processing | `MC_SignalEnable + MC_SIG_SURFACE_PROCESSING` |
| Cluster Unavailable | `MC_SignalEnable + MC_SIG_CLUSTER_UNAVAILABLE` |
| Acquisition Failure | `MC_SignalEnable + MC_SIG_ACQUISITION_FAILURE` |
| End of Acquisition | `MC_SignalEnable + MC_SIG_END_ACQUISITION_SEQUENCE` |
| Start of Acquisition | `MC_SignalEnable + MC_SIG_START_ACQUISITION_SEQUENCE` |
| End of Channel Activity | `MC_SignalEnable + MC_SIG_END_CHANNEL_ACTIVITY` |

#### Signal Information Structure

The `PMCSIGNALINFO` structure provides information on a specific signal. Three usages:
- With the callback function (argument)
- With the waiting function (return value)
- With `McGetSignalInfo` (retrieval)

**SignalInfo:** For Surface Processing or Surface Filled signals, contains the handle of the surface that experienced the state transition.

**SurfaceContext:** A user-convenience integer parameter (not written or read by the driver). Typically used for custom indexing information.

### 6.2. Callback Signaling

#### Callback Signaling Mechanism

The callback function is called by the MultiCam driver (not the user application) when a surface becomes ready for processing. Built-in features:

- A dedicated thread is created for callback function execution.
- The callback function prototype is declared in the MultiCam header file.
- Means are provided to designate the channel and signal(s) issuing the callback.
- The callback function argument provides all relevant information.

Registration function: `McRegisterCallback`

#### Callback Function Prototype

```c
typedef void (MCAPI *PMCCALLBACK)(PMCSIGNALINFO SignalInfo);
```

Only one callback function per object is supported. If multiple enabled signals are issued simultaneously, the callback function is called successively for each.

> **Note:** If the callback signaling mechanism is used, the waiting and advanced signaling mechanisms cannot be used.

#### Code Example of Callback Mechanism

```c
void MyApplication()
{
    MCSTATUS Status = McOpenDriver(NULL);

    MCHANDLE MyChannel;
    Status = McCreateNm("CHANNEL", &MyChannel);
    Status = McSetParamInt(MyChannel, MC_DriverIndex, 0);
    Status = McSetParamInt(MyChannel, MC_Connector, MC_Connector_M);
    // Configure channel, assign destination cluster of surfaces...

    // Registering the callback function
    Status = McRegisterCallback(MyChannel, MyFunction, NULL);

    // Activating acquisition sequence
    Status = McSetParamInt(MyChannel, MC_ChannelState, MC_ChannelState_ACTIVE);

    // ... Callback is automatically generated after each acquisition phase ...

    Status = McDelete(MyChannel);
    Status = McCloseDriver();
}

void MCAPI MyFunction(PMCSIGNALINFO SignalInfo)
{
    // Image processing code
    // Image is available in the destination cluster of surfaces
}
```

### 6.3. Waiting Signaling

The main application thread waits for a specific MultiCam signal using `McWaitSignal`. The signal should be enabled with `SignalEnable`.

Only one signal can be waited for in a given thread at a given time. If the expected signal does not occur within the specified timeout, the function returns `MC_TIMEOUT`.

> **Important:** When waiting for the Surface Processing signal, the application must reset `SurfaceState` to FREE when done. Failure to do so prevents the surface from being reused.

> **Note:** If the waiting signaling mechanism is used, callback signaling cannot be used.

### 6.4. Advanced Signaling

For highest control over event flow, a linkage to regular Windows events is provided. The user can use:

- `WaitForSingleObject`
- `WaitForMultipleObject`
- `MsgWaitForMultipleObjects`

The `SignalEvent` collection parameter provides handles to events corresponding to all MultiCam signals.

```c
HANDLE MyHandle;
McSetParamInt(hChannel, MC_SignalEnable + MC_SIG_SURFACE_FILLED, MC_SignalEnable_ON);
McGetParamInt(hChannel, MC_SignalEvent + MC_SIG_SURFACE_FILLED, (int*)&MyHandle);
WaitForSingleObject(MyHandle, INFINITE);
```

Use `McGetSignalInfo` to retrieve signal information for one or several signals at any time.

> **Note:** If the advanced signaling mechanism is used, callback signaling cannot be used. However, the waiting signaling mechanism can be used for non-conflicting events.

---

## 7. Pixel Format Conversion

To use software pixel format conversion, create a MultiCam surface and configure its parameters.

**Example: Convert BAYER8 to RGB24:**

```c
// Get surfaceIn properties
INT32 width, height, pitchIn;
McGetParamInt(surfaceIn, MC_SurfaceSizeX, &width);
McGetParamInt(surfaceIn, MC_SurfaceSizeY, &height);
McGetParamInt(surfaceIn, MC_SurfacePitch, &pitchIn);

// Create surfaceOut and allocate buffer
MCHANDLE surfaceOut = 0;
McCreate(MC_DEFAULT_SURFACE_HANDLE, &surfaceOut);
INT32 pitchOut = pitchIn * 3;
INT32 sizeOut = pitchOut * height;
unsigned char *buffer = new unsigned char[sizeOut];

// Configure surfaceOut for RGB24
McSetParamPtr(surfaceOut, MC_SurfaceAddr + 0, buffer);
McSetParamInt(surfaceOut, MC_SurfaceSize + 0, sizeOut);
McSetParamInt(surfaceOut, MC_SurfacePitch + 0, pitchOut);
McSetParamInt(surfaceOut, MC_SurfaceSizeX, width);
McSetParamInt(surfaceOut, MC_SurfaceSizeY, height);
McSetParamInt(surfaceOut, MC_SurfaceColorFormat, MC_ColorFormat_RGB24);
McSetParamInt(surfaceOut, MC_SurfaceColorComponentsOrder, MC_ColorComponentsOrder_BGR);

// Convert
McConvertSurface(surfaceIn, surfaceOut);

// Cleanup
if (surfaceOut != 0) McDelete(surfaceOut);
if (buffer != NULL) delete[] buffer;
```

---

## 8. Exceptions

### 8.1. API Errors

#### Error Reporting Behaviors

| ErrorHandling | Behavior | Effect |
|---|---|---|
| **NONE** | Return | The function returns the error code. |
| **MSGBOX** | Message | A dialog box is displayed (Windows only). |
| **EXCEPTION** | Exception | A Windows structured exception is issued (Windows only). |
| **MSGEXCEPTION** | Message + Exception | Dialog box displayed, allowing for exception (Windows only). |

**Return Behavior:** All API functions return `MCSTATUS`. Normally `MC_OK` (0). On error, a negative error code.

**Message Behavior:** Dialog with OK (pass error), ABORT (terminate application), IGNORE (return MC_OK).

**Exception Behavior:** Throws WIN32 structured exception with MultiCam error code.

### 8.2. MultiCam Error Codes

| Return value | Error identifier | Description |
|---|---|---|
| 0 | `MC_OK` | No Error |
| -1 | `MC_NO_BOARD_FOUND` | No Board Found |
| -2 | `MC_BAD_PARAMETER` | Bad Parameter |
| -3 | `MC_IO_ERROR` | I/O Error |
| -4 | `MC_INTERNAL_ERROR` | Internal Error |
| -5 | `MC_NO_MORE_RESOURCES` | No More Resources |
| -6 | `MC_IN_USE` | Object still in use |
| -7 | `MC_NOT_SUPPORTED` | Operation not supported |
| -8 | `MC_DATABASE_ERROR` | Parameter database error |
| -9 | `MC_OUT_OF_BOUND` | Value out of bound |
| -10 | `MC_INSTANCE_NOT_FOUND` | Object instance not found |
| -11 | `MC_INVALID_HANDLE` | Invalid Handle |
| -12 | `MC_TIMEOUT` | Timeout |
| -13 | `MC_INVALID_VALUE` | Invalid Value |
| -14 | `MC_RANGE_ERROR` | Value not in range |
| -15 | `MC_BAD_HW_CONFIG` | Invalid hardware configuration |
| -16 | `MC_NO_EVENT` | No Event |
| -17 | `MC_LICENSE_NOT_GRANTED` | License not granted |
| -18 | `MC_FATAL_ERROR` | Fatal error |
| -19 | `MC_HW_EVENT_CONFLICT` | Hardware event conflict |
| -20 | `MC_FILE_NOT_FOUND` | File not found |
| -21 | `MC_OVERFLOW` | Overflow |
| -22 | `MC_INVALID_PARAMETER_SETTING` | Parameter inconsistency |
| -23 | `MC_PARAMETER_ILLEGAL_ACCESS` | Illegal operation |
| -24 | `MC_CLUSTER_BUSY` | Cluster busy |
| -25 | `MC_SERVICE_ERROR` | MultiCam service error |
| -26 | `MC_INVALID_SURFACE` | Invalid surface |

### 8.3. Operational Exceptions

**Frame Trigger Violation** (`MC_SIG_FRAMETRIGGER_VIOLATION`): Issued when a frame or page trigger has been received which could not be handled because an acquisition phase was still in progress. The trigger is lost.

**Cluster Unavailable** (`MC_SIG_CLUSTER_UNAVAILABLE`): Issued when the cluster mechanism has not been able to designate a surface as the destination of acquisition.

**Acquisition Failure** (`MC_SIG_ACQUISITION_FAILURE`): Issued when the channel acquisition timeout timer expires before the end of the acquisition. The channel is disabled and must be deleted.

---

## 9. CAM Files

### What Is a CamFile?

The CamFile is a script of MultiCam setting functions played when the `CamFile` parameter is written to. After playing, the channel is ready to operate according to the specified parameter settings.

A CamFile is a readable ASCII file with:
- An optional identification header
- A mandatory pair of `Camera` and `CamConfig` parameter assignments
- An optional list of all relevant MultiCam Channel parameter assignments

> **WARNING:** A CamFile exclusively contains Channel parameters!

### CamFile Identification Header

Optional section with MultiCam Studio directives:

```
;***************************************************************
; Camera Manufacturer: My Cameras
; Camera Model: ProgressiveFR
; Camera Configuration: Progressive Free-Run Scanning, Analog synchronization
;***************************************************************
```

### CamFile Parameter Assignments

Format: `<ParameterName> = <ParameterValue> [;<Comment>] <EOL>`

```
Camera = ProgressiveFR
CamConfig = PxxSA
;
Gain=1000
TargetFrameRate_Hz = 0.5; 1 frame every two seconds
```

> **WARNING:** Considering built-in dependencies between MultiCam parameters, assign values starting from the parent. Keep the statements order of CamFile templates.

### Loading the CamFile

Set the `CamFile` parameter to the file name (without the `.cam` extension):

```c
Status = McSetParamStr(MyChannel, MC_CamFile, "VCC-870A_P15RA");
```

**Search rules:**
1. If the file name is a full pathname, no other location is searched.
2. Otherwise: current working folder, then MultiCam "Camera" folder, then all subfolders.

### CamFile Libraries

**CamFile Templates:** Delivered with MultiCam, stored in `<InstallDir>\Cameras\_TEMPLATES\Grablink\`.

**Camera Interface Packages:** ZIP files with ready-to-use CamFiles and documentation. Available via MultiCam Studio automatic update or free download from Euresys website.

---

# PART III: Configuration Object

## 1. The Configuration Object

The configuration object groups all MultiCam parameters dedicated to the control of system-wide features.

The system should be understood as the set of Euresys boards installed inside a host computer. The configuration object also addresses any hardware or software element requesting control for MultiCam system operation.

The configuration object is unique within the system and does not need to be instantiated. It is natively available when the MultiCam driver is connected.

---

# PART IV: Board Object

## 1. The Board Object

The Board object groups all MultiCam parameters dedicated to the control of features specific to a board, including access to I/O lines for general-purpose I/O functionality.

The board object is unique for each installed Euresys board and does not need to be instantiated. One board object is natively available for each installed board when the MultiCam driver is opened.

---

## 2. Board Information Parameters

### 2.1. Board Identification: Addressing a Board

Four ways to address a particular board:

**Index-Addressing** (`DriverIndex`): The driver establishes a list starting at index 0. Set `DriverIndex` to a value between 0 and `BoardCount-1`.

**PCI-Addressing** (`PciPosition`): Each PCI slot has a unique number (PC-specific but consistent for a given model/BIOS).

**Name-Addressing** (`BoardName`): A string of maximum 16 ASCII characters stored in non-volatile board memory. Can be altered by the user.

**Identifier-Addressing** (`BoardIdentifier`): Combination of board type and serial number. Example: `GRABLINK_FULL_000123`.

#### Code Example: How to Gather Board Information?

```c
typedef struct {
    char BoardName[17];
    INT32 SerialNumber;
    INT32 BoardType;
} MULTICAM_BOARDINFO;

MULTICAM_BOARDINFO BoardInfo[10];
INT32 BoardCount, i;
MCSTATUS Status;

Status = McOpenDriver(NULL);
Status = McGetParamInt(MC_CONFIGURATION, MC_BoardCount, &BoardCount);

for (i=0; i<BoardCount; i++) {
    Status = McGetParamStr(MC_BOARD+i, MC_BoardName, BoardInfo[i].BoardName, 17);
    Status = McGetParamInt(MC_BOARD+i, MC_SerialNumber, &BoardInfo[i].SerialNumber);
    Status = McGetParamInt(MC_BOARD+i, MC_BoardType, &BoardInfo[i].BoardType);
}

Status = McCloseDriver();
```

#### Code Example: How to Name a MultiCam Board?

```c
Status = McOpenDriver(NULL);
Status = McSetParamStr(MC_BOARD, MC_NameBoard+5, "MYBOARD");
Status = McCloseDriver();
```

### 2.2. Board Security Feature

A security key (8-byte ASCII string) can be engraved in the board's non-volatile memory. It cannot be read back, but can be verified.

**Writing a key:**
```c
Status = McSetParamStr(MC_BOARD + 1, MC_OemSafetyLock, "MY_NUM18");
```

**Checking a key:**
```c
Status = McSetParamStr(MC_BOARD + 1, MC_OemSafetyKey, "MY_NUM18");
McGetParamStr(MC_BOARD + 1, MC_OemSafetyLock, &Match, 6);
// Match will be "TRUE" or "FALSE"
```

### 2.3. Board Topology

The `BoardTopology` parameter declares how cameras are wired to the frame grabber. This must be entered before creating any channel for the targeted board.

---

## 3. Input/Output Control Parameters

### 3.1. How to Work With Input/Output Lines?

Many I/O lines are shared items that can be activated from:
- A MultiCam channel (camera-specific, function-oriented parameters)
- An application program (general-purpose I/O functionality via board-object parameters)

Before using I/O parameters, select a board via the Board Information category parameters.

---

# PART V: Channel Class

## 1. The Channel Class

The Channel class groups all MultiCam parameters dedicated to the control of image acquisition. A Channel object controls:

- The camera feeding the channel, including reset and exposure control
- The connector and cable linking the camera to the frame grabber
- The switching structures routing video signal inside the frame grabber
- The analog-to-digital converter or digital receiving devices
- The timing generator and controller
- Digital devices (LUTs, byte alignment, data channel merging)
- The data buffer receiving images
- The DMA devices extracting images for transfer into host memory
- The destination cluster of host memory surfaces
- Hardware resources managing the external system trigger

---

## 2. Introduction to MultiCam Channels

### 2.1. The Channel Concept

A MultiCam channel is an acquisition path between a defined camera and a defined cluster of surfaces. The application author sees the channel as a set of adjustable parameters controlling every resource in the acquisition process.

Usually, one channel is created per camera. However, multiple channels sharing the same camera can differ by destination cluster or other aspects.

### 2.2. Grabber and Camera Association

When performing acquisition, a channel appropriates a grabber for an **acquisition phase** -- the tenure of time needed to acquire a frame (area-scan) or page (line-scan).

### 2.3. Concurrent Acquisition

For boards with multiple grabbers, each channel appropriates one grabber permanently. No resource competition -- total independence of channels.

### 2.4. Switched Acquisition

When cameras outnumber available grabbers, channels compete for a common grabber. MultiCam handles resource allocation automatically via automatic switching.

### 2.5. A Pictorial View of a MultiCam System

Example with three channels: Channel #1 uses grabber #1 exclusively (concurrent). Channels #2 and #3 share grabber #2 (switched).

---

## 3. Configuring a Channel

### 3.1. Declaring a Topology

Use `BoardTopology` board-class parameter to declare camera wiring before channel creation.

### 3.2. Creating the Channel

Use `McCreate` with a creation model specific to the frame grabber. The designation of the camera connector location is entered at creation.

### 3.3. Assigning a Board to a Channel

Must be performed immediately after channel creation, before any other parameter setting. Four addressing methods available: `DriverIndex`, `PciPosition`, `BoardName`, `BoardIdentifier`.

### 3.4. Assigning a Camera to a Channel

Set the `CamFile` parameter to a released CAM file name. When set, the file is "played" -- all parameter settings are realized.

### 3.5. Code Example to Configure a Channel

```c
Status = McOpenDriver(NULL);

// Declaring the topology
Status = McSetParamStr(MC_BOARD + 1, MC_BoardTopology, "MONO_DECA");

// Creating a channel
MCHANDLE MyChannel;
Status = McCreate(MC_CHANNEL, &MyChannel);

// Assigning the board
Status = McSetParamInt(MyChannel, MC_DriverIndex, 1);
Status = McSetParamStr(MyChannel, MC_Connector, "M");

// Playing the CAM file
Status = McSetParamStr(MyChannel, MC_CamFile, "P4-CC-04K07T_L12240RG");

// ... Application code ...

Status = McDelete(MyChannel);
Status = McCloseDriver();
```

---

## 4. Understanding MultiCam Acquisition Phases

### 4.1. Acquisition Phase

A non-interruptible amount of time during which the grabber deposits an image into a destination surface.

- **Area-scan (VCAM or CTL mode):** Succession of "frame exposure" and "frame readout" sub-phases.
- **Line-scan (PAGE mode):** Succession of "page delay" and "page scanning" sub-phases.

Events: **SAP** (Start of Acquisition Phase), **EAP** (End of Acquisition Phase).

### 4.2. Vanishing Initial Sub-Phase

Under certain circumstances, the subdivision into sub-phases does not apply:
- Area-scan in SYNC mode: Camera delivers frames continuously, so acquisition phase = frame readout.
- Line-scan in WEB mode: Elementary pages are issued without gap, so page delay is irrelevant.

### 4.3. Waiting Phase

The first sub-phase is sometimes called the **waiting phase**. Events: **SWP** (Start of Waiting Phase), **EWP** (End of Waiting Phase).

The EWP event unconditionally provokes the SAP event. The waiting phase concept is needed for phase overlapping.

### 4.4. Phase Overlapping in Area-Scan

Allowed when:
- `Expose` = PLSTRG or WIDTH (asynchronous reset modality)
- `Readout` = INTCTL
- `ExposeOverlap` = ALLOW

This allows triggering the optical image capture while the previous image is read out, achieving the highest frame rate in "on-the-fly" grabbing. The waiting phase duration is set by `Expose_us`.

### 4.5. Phase Overlapping in Line-Scan

Allowed when `AcquisitionMode = PAGE`. A page trigger can occur before the end of the previous page scanning. The waiting phase duration is set by `PageDelay_Ln`.

---

## 5. MultiCam Triggering

### 5.1. Trigger Event (TE)

The trigger event is the instant when the channel decides to request a grabber for an acquisition phase. As a rule, when TE occurs, an acquisition phase immediately starts (with exceptions for internal circumstances).

### 5.2. Grabber Triggering Mode

Three origins for TE:
- A specified logic transition on a hardware line managed by the grabber
- A special software control
- Another event

The first acquisition phase is the **initial** one (caused by initial TE). Subsequent phases are caused by subsequent TE. Parameters: `TrigMode` and `NextTrigMode`.

### 5.3. Trigger Event Sources

| TrigMode | Initial TE | NextTrigMode | Subsequent TE |
|---|---|---|---|
| IMMEDIATE | SAS | REPEAT | Previous EAP |
| HARD | Hardware line edge detection | HARD | Hardware line edge detection |
| SOFT | `ForceTrig` set to TRIG | SOFT | `ForceTrig` set to TRIG |
| COMBINED | HARD or SOFT (whichever first) | COMBINED | HARD or SOFT (whichever first) |

The channel is **armed** when: the acquisition sequence is running, the previous phase is completed, and the next phase is due at next TE.

### 5.4. Grabber Triggering Examples

#### Triggering Example 1: Immediate single-phase, area-scan, synchronous scanning

| Parameter | Value |
|---|---|
| TrigMode | IMMEDIATE |
| SeqLength_Fr | 1 |

Simplest software-controlled way to grab from a synchronous camera. TE is directly caused by the SAS event.

#### Triggering Example 2: Immediate single-phase, area-scan, asynchronous reset

| Parameter | Value |
|---|---|
| ExposeOverlap | FORBID |
| TrigMode | IMMEDIATE |
| SeqLength_Fr | 1 |

#### Triggering Example 3: Armed single-phase, area-scan, asynchronous reset

| Parameter | Value |
|---|---|
| ExposeOverlap | FORBID |
| TrigMode | HARD, SOFT or COMBINED |
| SeqLength_Fr | 1 |

TE sourced by hardware or software event. No substantial delay between source and SAP. Use `TrigMode = SOFT` for software-controlled grab with tight timing.

#### Triggering Example 4: Armed single-phase, line-scan, page mode with phase overlapping

| Parameter | Value |
|---|---|
| TrigMode | HARD, SOFT or COMBINED |
| SeqLength_Fr | 1 |

Set `PageDelay_Ln = 0` when page delay is not necessary.

#### Triggering Example 5: Immediate multiple-phase, area-scan, asynchronous reset

| Parameter | Value |
|---|---|
| ExposeOverlap | FORBID |
| TrigMode | IMMEDIATE |
| NextTrigMode | HARD, SOFT or COMBINED |
| SeqLength_Fr | >1 |

Set `SeqLength_Fr = MC_INDETERMINATE` for undefined number of images (stop by returning channel to idle).

#### Triggering Example 6: Immediate multiple-phase, area-scan, with phase overlapping

| Parameter | Value |
|---|---|
| ExposeOverlap | ALLOW |
| TrigMode | IMMEDIATE |
| NextTrigMode | HARD, SOFT or COMBINED |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

#### Triggering Example 7: Armed multiple-phase, area-scan, without phase overlapping

| Parameter | Value |
|---|---|
| ExposeOverlap | FORBID |
| TrigMode | HARD, SOFT or COMBINED |
| NextTrigMode | HARD, SOFT or COMBINED |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

#### Triggering Example 8: Armed multiple-phase, line-scan, page mode

| Parameter | Value |
|---|---|
| TrigMode | HARD, SOFT or COMBINED |
| NextTrigMode | HARD, SOFT or COMBINED |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

#### Triggering Example 9: Immediate sustained, area-scan, synchronous scanning

| Parameter | Value |
|---|---|
| TrigMode | IMMEDIATE |
| NextTrigMode | REPEAT |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

Each subsequent TE sourced by end of preceding acquisition phase. Often called "live acquisition".

#### Triggering Example 10: Immediate sustained, line-scan, WEB mode

| Parameter | Value |
|---|---|
| TrigMode | IMMEDIATE |
| NextTrigMode | REPEAT |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

Software-controlled way to grab an arbitrarily long image of a continuous web.

#### Triggering Example 11: Armed sustained, area-scan, asynchronous reset

| Parameter | Value |
|---|---|
| ExposeOverlap | FORBID |
| TrigMode | HARD, SOFT or COMBINED |
| NextTrigMode | REPEAT |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

Single initial external trigger, then sustained acquisition.

#### Triggering Example 12: Armed sustained, line-scan, WEB mode

| Parameter | Value |
|---|---|
| TrigMode | HARD, SOFT or COMBINED |
| NextTrigMode | REPEAT |
| SeqLength_Fr | MC_INDETERMINATE or >1 |

### 5.5. Frame Trigger Violation

A trigger pulse cannot be immediately served when:
- Phase overlapping not allowed: violation when trigger occurs during acquisition phase
- Phase overlapping allowed: violation when trigger occurs too early for waiting phase to complete

When a violation occurs, `MC_SIG_FRAMETRIGGER_VIOLATION` is issued and the `FrameTriggerViolation` counter is incremented.

---

## 6. Understanding MultiCam Acquisition Sequences

### 6.1. Acquisition Sequence

A succession of acquisition phases. Each channel can activate one acquisition sequence. If two sequences are needed for the same camera, create two channels.

### 6.2. Activating the Acquisition Sequence

Activated at **SAS** by setting `ChannelState = ACTIVE`.

### 6.3. Deactivating the Acquisition Sequence

Set `ChannelState = IDLE` to stop. The sequence remains alive until the current acquisition phase completes, except:
- If the stopping condition is an exception signal (immediate stop)
- If in line-scan WEB mode (immediate stop)

### 6.4. Single-Phase Acquisition Sequence

SAP occurrence directed by triggering mode and grabber availability.

### 6.5. Multiple-Phase Acquisition Sequence

Same as single-phase, with multiple successive phases.

### 6.6. Sustained Acquisition Sequence

SAP of each phase derived from EAP of the previous one. No gap between phases. Used for live acquisition (area-scan) or continuous scanning (line-scan).

### 6.7. Concurrent Acquisition Mode

Enough board resources for multiple cameras simultaneously (DuoCam, TrioCam). No resource competition -- no switching needed.

### 6.8. Switched Acquisition Mode

Insufficient board resources for all cameras simultaneously. MultiCam embeds an automatic switching mechanism. Channels may need to wait their turn.

---

## 7. Understanding Automatic Switching

### 7.1. What is Automatic Switching?

Available for all Domino and Grablink frame grabbers. Allows:
- Creation of more than one channel with different settings per camera
- Creation of more than one channel per grabber

MultiCam handles resource allocation automatically.

### 7.2. Getting ChannelState

| State | Description |
|---|---|
| **ORPHAN** | No grabber associated. Parameters can be freely set/get. |
| **IDLE** | Grabber associated. Acquisition possible. MultiCam may reassign resources automatically. |
| **READY** | Grabber associated. Acquisition possible. MultiCam cannot reassign resources automatically. |
| **ACTIVE** | Grabber associated. Performing acquisition sequence. Resources locked. |

### 7.3. ChannelState Transitions

**Initial State:** At channel creation, `ChannelState = ORPHAN`.

**From ORPHAN:**

- **Set to IDLE:** MultiCam associates grabber resources. May take time. Selects automatic switching.
- **Set to READY:** Associates resources if available; otherwise remains ORPHAN. Short completion time. Selects manual switching.
- **Set to ACTIVE:** Associates resources and starts acquisition. May take time. Selects automatic switching.

**Manual Switching (from READY):**

- **Set to ACTIVE:** Starts acquisition immediately.
- **Set to FREE:** Releases grabber resources, returns to ORPHAN.

**From ACTIVE (manual):**
- **Automatically:** EAS after `SeqLength_Fr` phases -> READY.
- **Set to IDLE:** Forces EAS, last phase finishes -> READY.

**Automatic Switching (from IDLE):**
- **Automatically:** IDLE -> ORPHAN when another channel needs resources.
- **Set to ACTIVE:** Immediately starts acquisition -> ACTIVE.

**From ACTIVE (automatic):**
- **Automatically:** EAS after `SeqLength_Fr` phases -> IDLE.
- **Set to IDLE:** Forces EAS -> IDLE.

---

## 8. Understanding Camera Specification

### 8.1. Camera Class Specification

**Camera Imaging Basic Geometry** (`Imaging` parameter):
- **AREA:** Area-scan cameras with 2D imagers delivering 2D data frames
- **LINE:** Non-TDI line-scan cameras with 1D imagers delivering 1D data lines
- **TDI:** TDI line-scan cameras with 2D imagers delivering 1D data lines (increased sensitivity)

**Camera Spectral Sensitivity** (`Spectrum` parameter):
- **BW:** Black/white monochrome cameras
- **IR:** Infrared monochrome cameras
- **COLOR:** Color cameras with CFA or multiple imagers

**Camera Data Transfer Method** (`DataLink` parameter):
- **COMPOSITE:** Composite video cameras (CVBS, VBS) -> Picolo series
- **ANALOG:** Analog industrial cameras -> Domino series
- **CAMERALINK:** Camera Link digital cameras -> Grablink series

### 8.2. Color Camera Specification

**Color Analysis Method** (`ColorMethod`):
- **NONE:** Monochrome
- **RGB:** Three separate R, G, B components
- **BAYER:** Raw video from Bayer CFA imager
- **PRISM:** 3-CCD prism assembly with perfect pixel registration
- **TRILINEAR:** Triple line-array imager with un-registered color components

**Color Pattern Filter Alignment** (`ColorRegistration`):
- Bayer CFA: GB, BG, RG, GR
- Trilinear: RGB, GBR, BRG

**Color Gap** (`ColorGap`): Gap between adjacent sensing lines of trilinear cameras (in pixel pitches).

### 8.3. Camera Timing Specification

The `Camera` parameter declares the camera name. Expert-level parameters in the Camera Timing category are automatically adjusted when `Camera` is updated.

These define the temporal size and location of the **Camera Active Window** inside the scanning format.

For area-scan analog, digital, and line-scan digital cameras, the active window is specified by controlling parameters including: `LineRate_Hz`, `Vtotal_Ln`, `HCsyncAft_ns`, `VCsyncAft_Ln`, `Hactive_Px`, `Vactive_Ln`, `PixelClk_Hz`, and others.

### 8.4. Camera Upstream Specification

Two distinct camera functions (**Expose** and **Readout**) controlled by one or two upstream lines (Reset and AuxReset).

**Line-Scan combinations:**

| Expose | Readout | Description |
|---|---|---|
| INTCTL | INTCTL | Free-running, camera controlled exposure time |
| INTPRM | INTCTL | Free-running, permanent exposure |
| PLSTRG | INTCTL | Grabber-controlled rate, camera-controlled exposure |
| PLSTRG | PLSTRG | Grabber-controlled rate, grabber-controlled exposure, dual signal |
| WIDTH | INTCTL | Grabber-controlled rate, grabber-controlled exposure, single signal |
| INTPRM | PLSTRG | Grabber-controlled rate, permanent exposure |

**Area-Scan combinations:**

| Expose | Readout | Description |
|---|---|---|
| INTCTL | INTCTL | Free-running, camera-controlled exposure |
| PLSTRG | INTCTL | Grabber-controlled rate, camera-controlled exposure |
| WIDTH | INTCTL | Grabber-controlled rate, grabber-controlled exposure |

### 8.5. Camera Output Structure with Grablink

**TapConfiguration Parameter:** Declares the Camera Link tap configuration. Naming convention:

```
<Config>_<TapCount>T<BitDepth>(B<TimeSlots>)
```

Where Config = LITE, BASE, MEDIUM, FULL, or DECA.

Examples:
- `BASE_1T8`: Base, 1 tap, 8-bit
- `BASE_1T24`: Base, 1 tap, 24-bit (RGB)
- `DECA_8T10`: 80-bit, 8 taps, 10-bit
- `DECA_8T30B3`: 80-bit, 8 taps, 30-bit (RGB), 3 time slots

**Supported Monochrome 8-bit Tap Configurations:**

| CL2.1 Name | Euresys Name | Base/DualBase | Full/FullXR |
|---|---|---|---|
| Lite | LITE_1T8 | Yes | - |
| Base 1 tap | BASE_1T8 | Yes | Yes |
| Base 2 taps | BASE_2T8 | Yes | Yes |
| Base 3 taps | BASE_3T8 | Yes | Yes |
| Medium 4 taps | MEDIUM_4T8 | - | Yes |
| Medium 6 taps | MEDIUM_6T8 | - | Yes |
| Full 8 taps | FULL_8T8 | - | Yes |
| Full 9 taps | DECA_9T8 | - | Yes |
| 80-bit 10 taps | DECA_10T8 | - | Yes |

**Supported RGB 8-bit Tap Configurations:**

| CL2.1 Name | Euresys Name | Base/DualBase | Full/FullXR |
|---|---|---|---|
| Base 1 tap | BASE_1T24 | Yes | Yes |
| Medium 2 taps | MEDIUM_2T24 | - | Yes |
| Full 3 taps | DECA_3T24 | - | Yes |

**TapGeometry Parameter:** Describes geometrical properties of multi-tap cameras.

Syntax: `<TapGeometryX>_<TapGeometryY>` (or just `<TapGeometryX>` for single-row cameras)

Where `<TapGeometryX>` = `<XRegions>X(<XTaps>)(<ExtX>)` and `<TapGeometryY>` = `<YRegions>Y(<YTaps>)(<ExtY>)`

Examples:
- `1X_1Y`: Single-tap area-scan, raster-scan order
- `1X2_1Y`: Two-tap area-scan, 2 pixels at a time
- `2XE_2YE`: Four-tap area-scan, 4 quadrants
- `1X4`: Four-tap line-scan, 4 pixels at a time
- `4X`: Four-tap line-scan, 4 regions

---

## 9. Understanding Grabber Specification

### 9.1. How to Define the Grabbing Window?

The `GrabWindow` parameter provides methods to derive the grabbing window from the camera active window.

| Method | Description |
|---|---|
| **NOBLACK** | (Recommended) Only pixels bearing luminance information are acquired. Grabber restrictions taken into account. |
| **NOLOSS** | Smallest size encompassing all camera active pixels. |
| **STD** | Size rounded to standard values (320, 512, 640, 768, 1024, 1280 for width; 240, 288, 480, 576, 600, 1000, 1024 for height). |
| **MAN** | Manual adjustment. Use `WindowX_Px`, `WindowY_Ln`, `OffsetX_Px`, `OffsetY_Ln`. |

For all methods, the user can readjust position with `OffsetX_Px` and `OffsetY_Ln`. Internal protection maintains the grabbing window inside the camera active window.

### 9.2. How to Control the Analog Gain on Domino Boards?

**Parameters:** `GainCtl` (mode), `Gain` (all channels), `GainTrim1/2/3` (per channel).

**Nominal Gain:** Full-scale response at 0.7V luminance. Set `GainCtl = 0DB` and all trims to zero.

**Logarithmic Gain:** Set `GainCtl` to one of: +3dB, +2dB, +1dB, 0dB, -1dB, -2dB, -3dB.

| GainCtl value | Linear expression |
|---|---|
| +3 dB | Nominal gain x 1.413 |
| +2 dB | Nominal gain x 1.259 |
| +1 dB | Nominal gain x 1.122 |
| 0 dB | Nominal gain |
| -1 dB | Nominal gain x 0.891 |
| -2 dB | Nominal gain x 0.794 |
| -3 dB | Nominal gain x 0.708 |

**Linear Gain:** Set `GainCtl = LIN` and use `Gain` parameter (1000 = nominal). Range approximately 700 to 1400.

### 9.3. How to Control the Analog Offset on Domino Boards?

**Parameters:** `Offset` (all channels), `OffsetTrim1/2/3` (per channel).

**Nominal Offset:** Blanking level digitized to zero. Set `Offset = 0` and all trims to zero.

Increasing offset = brighter image. An offset adjustment of 500 = 50% of full digitizing span shift.

---

## 10. Using Look-Up Tables

### 10.1. Definitions

- **LUT device:** Transforms input vector to output vector using a look-up table.
- **LUT dimension:** Input vector width x output vector width.
- **LUT (device) set:** Group of LUT devices operating concurrently in a channel.
- **LUT image:** Data contents of one LUT device.
- **LUT set image:** Data contents of a LUT device set.
- **LUT buffer:** Frame grabber memory cache for one LUT image.

### 10.2. LUT Characteristics per Board

| Board | LUT devices | LUT dimension | Devices per set | LUT buffers |
|---|---|---|---|---|
| Domino Melody | 1 | 8x8 or 10x10 | 1 | 5 |
| Domino Symphony | 4 | 8x8 or 10x10 | 1 | 5 |
| Grablink Base/DualBase/Full/FullXR | 3 | 12x16 | 3 | 4 |

### 10.3. LUT APIs

| Feature | Classic API | Advanced API |
|---|---|---|
| Parametric LUT definition | N/A | Yes |
| User-defined LUT data | Yes | Yes |

Grablink boards: Advanced API only. Domino boards: Both APIs.

### 10.4. How to Operate LUTs?

Three steps:

1. **LUT Definition:** Select a method (parametric or table) and define contents.
2. **LUT Loading:** Transfer to frame grabber memory using `LUT_StoreIndex` (advanced) or `LutIndex` + `InputLut` (classic).
3. **LUT Activation:** Activate with `LUT_UseIndex` (advanced) or `LutIndex` (classic, at next SAS event).

### 10.5. Parametric LUT Definition Methods

Three methods selected via `LUT_Method`:

| LUT_Method | Parameters |
|---|---|
| **RESPONSE_CONTROL** | `LUT_Contrast`, `LUT_Brightness`, `LUT_Visibility`, `LUT_Negative` |
| **EMPHASIS** | `LUT_Emphasis`, `LUT_Negative` |
| **THRESHOLD** | `LUT_SlicingLevel`, `LUT_SlicingBand`, `LUT_LightResponse`, `LUT_BandResponse`, `LUT_DarkResponse` |

**LUT_Contrast:** Controls the slope (gain) of the transformation law. Range 0.00 to 2.00 (default 1.00 = unity gain). Gain = 0.01 at 0.00, Gain = 100 at 2.00.

**LUT_Brightness:** Controls light/dark shift. Range -1.00 to +1.00 (default 0.00). At 0.00, mid-level 0.5 maps to 0.5 output.

**LUT_Visibility:** Reveals hidden image parts (clipped to black/white by contrast/brightness). Range 0.00 to 1.00 (default 0.00 = piecewise linear).

**LUT_Negative:** Mirrors the transformation table (swaps black and white). Default: FALSE.

**LUT_Emphasis:** Power-law transformation with gamma exponent. Range -1.00 to +1.00 (default 0.00 = linear, gamma=1).

**Threshold:** Five parameters define a double threshold transformation law. All range 0.00 to 1.00.

| Parameter | Default |
|---|---|
| `LUT_SlicingLevel` | 0.50 |
| `LUT_SlicingBand` | 0.50 |
| `LUT_LightResponse` | 0.75 |
| `LUT_BandResponse` | 0.50 |
| `LUT_DarkResponse` | 0.25 |

### 10.6. Table LUT Definition Method

Set `LUT_Method = TABLE`. Upload LUT data into a MultiCam surface (LUT surface).

**Surface Size per plane:**

| LUT_InDataWidth | LUT_OutDataWidth | Advanced API | Classic API |
|---|---|---|---|
| 8 bit | 8 bit | 1,024 bytes | 256 bytes |
| 10 bit | 10 bit | 4,096 bytes | 2,048 bytes |
| 12 bit | 16 bit | 16,384 bytes | N/A |
| 16 bit | 16 bit | 262,144 bytes | N/A |

With the advanced API, each entry is 32-bit wide regardless of `LUT_OutDataWidth`. Significant bits must be right-aligned.

---

## 11. Understanding the Rate Converter

### 11.1. How to Use an Encoder?

**Encoder Characteristics:** An electro-mechanical device delivering measurement of web speed via a pulse train. The **encoder pitch** is the distance traveled between successive pulses (independent of speed).

```
encoder_rate = web_speed / encoder_pitch
```

**Encoder and Line Triggering:** To maintain constant aspect ratio:
```
line_pitch = web_speed / line_frequency
```

If encoder pitch equals desired line pitch, simply use external line triggering (`LineRateMode = PULSE`).

**Encoder and Rate Conversion:** If encoder pitch differs from desired line pitch, use the rate converter (`LineRateMode = CONVERT`) to multiply frequency by a non-integer ratio with high accuracy.

```
rate_conversion_ratio = (aspect_ratio * encoder_pitch * num_pixels) / observed_width
```

### 11.2. Programming the Rate Converter

**Setting the Rate Conversion Ratio:**

Use integer parameters `LinePitch` and `EncoderPitch` (range 1 to 10,000):

```
RateConversionRatio = EncoderPitch / LinePitch
```

Example: encoder pitch = 0.75 mm, desired line pitch = 0.391 mm:
```
LinePitch = 391
EncoderPitch = 750
RateConversionRatio = 1.918
```

**Operating Range:**

The rate converter operates within a frequency range. Limits:
- Input too small (< 10 Hz): insufficient encoder precision
- Input too high (> 100 kHz): impractical for encoders
- Output too small (< 10 Hz): camera receives too few pulses
- Output too high (> 100 kHz): camera cannot follow

Operating range ratio is usually above 200.

**MultiCam Parameters:**
- `PixelClk_Hz`: Camera pixel frequency (inherent attribute)
- `MaxSpeed`: Maximum line frequency (default = camera speed limit)
- `MinSpeed`: Read-only, computed minimum line frequency

---

## 12. Programming the Exposure Time

### 12.1. Choosing the Controlled Exposure Mode

Any line-scan camera with electronic shutter is by default controlled through exposure control parameters. Permanent exposure can be forced through hardware line parameters.

### 12.2. Control Parameters

**`Expose_us`:** Basic exposure duration control in microseconds.

**`ExposeTrim`:** Trimming parameter on a logarithmic scale (-12 dB to +6 dB):
- -12 dB: exposure x 0.25
- 0 dB: no change
- +6 dB: exposure x 2

### 12.3. Feedback Parameter

**`TrueExp_us`:** Returns the effective exposure duration resulting from `Expose_us` and `ExposeTrim`.

> **Caution:** If exposure time exceeds the readout duration, the gap between successive trigger pulses cannot be shorter than the exposure duration. External triggering at too-high rates will activate line protection mechanisms.

---

## 13. Channel Parameters User Notes

### 13.1. Interactivity of Parameters

As a general rule, channel parameter modifications take effect at next **SAS**. However, some parameters take effect at next **SAP**:

**Grablink series (effect at SAP):**

| Category | Parameters |
|---|---|
| Exposure control | `Expose_us`, `ExposeTrim` |
| Strobe control | `StrobeDur`, `StrobePos` |
| Grabber conditioning | `Period_us`, `PeriodTrim`, `LinePitch`, `EncoderPitch`, `ConverterTrim`, `MaxSpeed`, `LineTrigCtl`, `LineTrigEdge`, `LineTrigLine` |

**Domino series (effect at SAP):**

| Category | Parameters |
|---|---|
| Exposure control | `Expose_us`, `ExposeTrim` |
| Strobe control | `StrobeDur`, `StrobePos` |
| Grabber conditioning | `VideoFilter`, `GainCtl`, `Gain`, `GainTrim1/2/3`, `Offset`, `OffsetTrim1/2/3`, `InputLut`, `LutIndex`, `ColorFormat` |

### 13.2. Camera Specification

**CamFile search rules:**
1. If full pathname, no other location searched.
2. Otherwise: current working folder, then MultiCam "Camera" folder, then subfolders.

**CamConfig:** Enumerated parameter summarizing essential camera features. The combination of `Camera` and `CamConfig` fully describes the camera sourcing the channel.

### 13.3. Strobe Control

**StrobeMode:**

| Value | Meaning |
|---|---|
| NONE | Strobe function disabled. Hardware line available for general use. |
| AUTO | Enabled with automatic timing (RG mode only). Uses `StrobeDur` and `StrobePos` with `Expose_us`. |
| MAN | Enabled with manual timing (RC mode only). Uses `StrobeDur` and `StrobePos` with `TrueExp_us`. |
| OFF | StrobeLine set to inactive level; no pulses (Grablink only). |

**StrobeDur:** Expressed as percentage of expose width pulse.

**StrobePos:** Expressed as percentage of expose width pulse. 0% = earliest (leading edge at start of exposure). 100% = latest (trailing edge at end of exposure). 50% = middle.

**PreStrobe_us:** Expressed in microseconds. Advances the leading edge of strobe pulse. Can cause strobe to begin before exposure starts.

**StrobeLine:** The NOM option selects nominal output strobe lines (STX, STY, STZ) at the TR-ST connector.

### 13.4. Grabber Configuration

**JumperCK:** Indicates CK jumper block configuration (CKDPOS, CKDNEG, CKSPOS, CKSNEG, ZLANE, EMPTY).

**JumperH:** Indicates H jumper block configuration (TTL, DPOS, DNEG).

**JumperV:** Indicates V jumper block configuration (TTL, DPOS, DNEG).

**SyncMode:**

| Mode | Description |
|---|---|
| **ANALOG** | Timing extracted from composite video signal via phase lock loop. |
| **DIGITAL** | Timing from digital signals (pixel clock, horizontal/vertical reference). |
| **MASTER** | Board is timing master; delivers horizontal/vertical drive to camera. |

### 13.5. Grabber Timing

**SampleClk_Hz:** The sample clock according to which pixels are sampled. In digital synchronization, it aligns to the camera pixel clock. In analog synchronization, it is built internally via phase locking.

The timing clock period is often called the **TCU** (Timing Clock Unit).

### 13.6. Selecting the Pixel Data Output Format

The `ColorFormat` parameter entirely characterizes the pixel data output format.

**On Grablink series (available values by camera class):**

| Camera class | Base/DualBase | Full/FullXR |
|---|---|---|
| Mono 8-bit | Y8 | Y8 |
| Mono 10-bit | Y8, Y10, Y16 | Y8, Y10, Y16 |
| Mono 12-bit | Y8, Y12, Y16 | Y8, Y12, Y16 |
| Mono 14-bit | Y8, Y14, Y16 | Y8, Y14, Y16 |
| Mono 16-bit | Y8, Y16 | Y8, Y16 |
| RGB 24-bit | RGB24, RGB32, RGB24PL | RGB24, RGB32, RGB24PL |
| RGB 30-bit | - | RGB24, RGB32, RGB24PL, RGB30PL, RGB48PL |
| RGB 36-bit | - | RGB24, RGB32, RGB24PL, RGB36PL, RGB48PL |
| Bayer CFA 8-bit | BAYER8, RGB24, RGB32, RGB24PL | BAYER8, RGB24, RGB32, RGB24PL, RGB36PL, RGB48PL |
| Bayer CFA 10-bit | BAYER8/10/16, RGB24, RGB32, RGB24PL | BAYER8/10/16, RGB24, RGB32, RGB24PL, RGB30PL, RGB48PL |
| Bayer CFA 12-bit | BAYER8/12/16, RGB24, RGB32, RGB24PL | BAYER8/12/16, RGB24, RGB32, RGB24PL, RGB36PL, RGB48PL |

**On Domino series:** Y8, Y10, Y16 for analog monochrome.

**On Picolo series:** Pixel data output format is selectable independently from camera type.

### 13.7. SignalEnable Parameter

Designate collection items to enable specific signals. Only enabled signals trigger callbacks or satisfy waiting functions.

For backward compatibility, element #0 accepts legacy values: NONE, PROCESSING, FILLED.

### 13.8. AcqTimeOut_ms Parameter

When an acquisition does not complete within the specified timeout duration, the Acquisition Failure signal is fired and the channel is disabled. The channel must be deleted and created again.

---

# PART VI: Surface Class

## 1. The Surface Class

### 1.1. What is a Surface?

A surface is a container where a 2D image can be stored. In most situations, it is a buffer in host memory. Other types include the hardware frame buffer inside a frame grabber.

For line-scan cameras, the surface can be used as a **circular buffer** -- although 2D-limited, the incoming data flow is continuous and virtually unlimited.

The surface is the destination where grabbed images are recorded. The goal of the MultiCam driver is to provide flexible channels to route images from camera to surface.

### 1.2. Surface Creation

The Surface class groups parameters dedicated to defining memory buffers for image or data storage. Several surfaces can exist simultaneously. Created via API function, deleted when no longer needed.

---

# PART VII: Appendix

## 1. Command-Line Installation Procedure

MultiCam setup can be called in command-line mode for automated installation.

### The Response File

**Step 1: Record a response file**
```
setup.exe /r /f1"path\Setup.iss"
```

**Step 2: Replay the response file (silent mode)**
```
setup.exe /s /f1"path\Setup.iss"
```

Use `/z"ForceInstall"` to force removal of an existing version before setup.

> **NOTE:** No space between flag and argument. `/f1"Setup.iss"` works; `/f1 "Setup.iss"` does not.

### Installation Removal

```
setup.exe /removeonly /s
```

### Reboot during Installation

If reboot was accepted during recording, it will occur automatically during replay. Otherwise, check the registry:
```
[HKEY_LOCAL_MACHINE\SOFTWARE\Euresys\Common] "RebootNeeded"
```
If set to 1, replace with 0 and reboot.

### Error Reporting

After installation, check:
```
[HKEY_LOCAL_MACHINE\SOFTWARE\Euresys\Common\LastInstallError]
```

| ErrorCode | Description |
|---|---|
| 0 | No error |
| 1 | User is not administrator |
| 2 | Not enough space on target disk |
| 3 | Invalid command line |
| 4 | Newer version already installed |

Additional fields: `Cause` (error wording), `Source` (installer ID), `ErrorTime` (time and date).
