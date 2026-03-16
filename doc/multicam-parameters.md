# MultiCam Parameters Reference (D412EN)
> Source: Euresys MultiCam 6.19.4 (Doc D412EN)

---

## Contents

- [1. About This Document](#1-about-this-document)
- [2. Configuration Class](#2-configuration-class)
  - [2.1. Configuration Category](#21-configuration-category)
- [3. Board Class](#3-board-class)
  - [3.1. Board Information Category](#31-board-information-category)
  - [3.2. Input/Output Control Category](#32-inputoutput-control-category)
- [4. Channel Class](#4-channel-class)
  - [4.1. Camera Specification Category](#41-camera-specification-category)
  - [4.2. Camera Timing Category](#42-camera-timing-category)
  - [4.3. Camera Features Category](#43-camera-features-category)
  - [4.4. Cable Features Category](#44-cable-features-category)
  - [4.5. Acquisition Control Category](#45-acquisition-control-category)
  - [4.6. Trigger Control Category](#46-trigger-control-category)
  - [4.7. Interleaved Acquisition Category](#47-interleaved-acquisition-category)
  - [4.8. Exposure Control Category](#48-exposure-control-category)
  - [4.9. Strobe Control Category](#49-strobe-control-category)
  - [4.10. Encoder Control Category](#410-encoder-control-category)
  - [4.11. Pipeline Control Category](#411-pipeline-control-category)
  - [4.12. Grabber Configuration Category](#412-grabber-configuration-category)
  - [4.13. Grabber Timing Category](#413-grabber-timing-category)
  - [4.14. Grabber Conditioning Category](#414-grabber-conditioning-category)
  - [4.15. White Balance Operator Category](#415-white-balance-operator-category)
  - [4.16. Look-up Tables Category](#416-look-up-tables-category)
  - [4.17. Board Linkage Category](#417-board-linkage-category)
  - [4.18. Cluster Category](#418-cluster-category)
  - [4.19. Channel Management Category](#419-channel-management-category)
  - [4.20. Signaling Category](#420-signaling-category)
  - [4.21. Exception Management Category](#421-exception-management-category)
- [5. Surface Class](#5-surface-class)
  - [5.1. Surface Specification Category](#51-surface-specification-category)
  - [5.2. Surface Dynamics Category](#52-surface-dynamics-category)
- [6. Annex](#6-annex)

---

## 1. About This Document

### 1.1. Scope and Summary

This document is a filtered edition of the MultiCam parameters reference for Grablink products supported by MultiCam driver version 6.19.

**Supported Grablink products:**

| Product | S/N Prefix |
|---------|-----------|
| PC1622 Grablink Full | FM1 |
| PC1623 Grablink DualBase | GDB |
| PC1624 Grablink Base | GBA |
| PC1626 Grablink Full XR | FXR |

Parameters are grouped by MultiCam object class. Classes are listed in the top-down hierarchical order. Within each class, parameters are listed in the natural order and grouped by categories.

The main sections of the document are:
- **Configuration Class**: parameters of the MultiCam Configuration object
- **Board Class**: parameters of the MultiCam Board object
- **Channel Class**: parameters of the MultiCam Channel object
- **Surface Class**: parameters of the MultiCam Surface object
- **Annex**: selection of topics referenced in this document

### 1.2. Document Changes

**MultiCam 6.18:**
- Added: FifoOrderingYTapCount
- Revised: FifoOrdering, TapConfiguration, CC3Usage

## 2. Configuration Class

What Is the Configuration Object? The Configuration object groups all MultiCam parameters dedicated to the control of system wide features. The system should be basically understood as the set of Euresys boards installed inside a host computer. The configuration object also addresses any hardware or software element of the host computer requesting some degree of control for the MultiCam system operation. The configuration object does not belong to a true class, as it is unique within the system. There is no need for the user to instantiate a Configuration class object using the McCreate or McCreateNm function. The Configuration object is natively made available to the application when the MultiCam driver is connected to it.

### 2.1. Configuration Category

*Parameters specifying system wide features*

#### MementoCritical

*Sends Memento trace with a Critical level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `102 << 14` |
| String ID | `MementoCritical` |
| C/C++ ID | `MC_MementoCritical` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Critical" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send a critical message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoCritical + 7, "This is a critical message.");

---

#### MementoError

*Sends Memento trace with an Error level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `103 << 14` |
| String ID | `MementoError` |
| C/C++ ID | `MC_MementoError` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Error" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send an error message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoError + 7, "This is an error message.");

---

#### MementoWarning

*Sends Memento trace with a Warning level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `104 << 14` |
| String ID | `MementoWarning` |
| C/C++ ID | `MC_MementoWarning` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Warning" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send a warning message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoWarning + 7, "This is a warning message.");

---

#### MementoNotice

*Sends Memento trace with a Notice level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `105 << 14` |
| String ID | `MementoNotice` |
| C/C++ ID | `MC_MementoNotice` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Notice" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send a notice message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoNotice + 7, "This is a notice message.");

---

#### MementoInfo

*Sends Memento trace with an Info level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `106 << 14` |
| String ID | `MementoInfo` |
| C/C++ ID | `MC_MementoInfo` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Info" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send an information message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoInfo + 7, "This is an information message.");

---

#### MementoDebug

*Sends Memento trace with a Debug level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `107 << 14` |
| String ID | `MementoDebug` |
| C/C++ ID | `MC_MementoDebug` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Debug" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send a debug message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoDebug + 7, "This is a debug message.");

---

#### MementoVerbose

*Sends Memento trace with a Verbose level from user application*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | SELECT |
| Type | String collection |
| Access | Set Only |
| Num ID | `108 << 14` |
| String ID | `MementoVerbose` |
| C/C++ ID | `MC_MementoVerbose` |

**Description:**

This string collection parameter of 16 elements enables the caller to send a Memento trace with "Verbose" level from a user application.

**Usage:**

The collection element index selects the Memento Kind from User0 to UserF. For instance, to send a verbose message with the User7 kind, the following call will be added in the C/C++ user application: McSetParamStr(MC_CONFIGURATION, MC_MementoVerbose + 7, "This is a verbose message.");

---

#### BoardCount

*Number of MultiCam boards in the system*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `62 << 14` |
| String ID | `BoardCount` |
| C/C++ ID | `MC_BoardCount` |

**Description:**

This parameter provides an immediate way for the application to be informed on the number of peripheral boards recognized as MultiCam compliant boards. See also "Code Example: How to Gather Board Information?" on page 514

---

#### ErrorHandling

*Error handling behavior definition*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `49 << 14` |
| String ID | `ErrorHandling` |
| C/C++ ID | `MC_ErrorHandling` |

**Description:**

This parameter defines the error handling behavior.

**Usage:**

Directive: When operating with Windows, select any of the four available behaviors when an error occurs during the execution of a MultiCam API function. Directive: When operating with Linux,leave the default value.

**Values:**

- **`NONE`**
  - C/C++: `MC_ErrorHandling_NONE`
  - On error, the MultiCam driver returns an error code. Default value.
- **`MSGBOX`**
  - C/C++: `MC_ErrorHandling_MSGBOX`
  - On error, the MultiCam driver displays an error dialog box then returns an error code.
- **`EXCEPTION`**
  - C/C++: `MC_ErrorHandling_EXCEPTION`
  - On error, the MultiCam driver issues a Windows structured exception.
- **`MSGEXCEPTION`**
  - C/C++: `MC_ErrorHandling_MSGEXCEPTION`
  - On error, the MultiCam driver displays an error dialog box then issues a Windows structured exception.

---

#### ErrorLog

*Path and filename of the error log file*

| Property | Value |
|----------|-------|
| Class | Configuration |
| Category | Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `81 << 14` |
| String ID | `ErrorLog` |
| C/C++ ID | `MC_ErrorLog` |

**Description:**

This parameter specifies the path and the filename of the error log file that is created when the application returns a MC_INVALID_PARAMETER_SETTING (-22) error code. The incorrect parameters are reported in the log file, including the wrong value and the possible correct values. When specified, the log file is created and filled during the consistency check. When unspecified, the consistency check does not produce a log file.

---

## 3. Board Class

What Is the Board Object? The Board object groups all MultiCam parameters dedicated to the control of features specific to a board. The Board object MultiCam parameters also address the access of I/O lines from an application program, implementing the general-purpose I/O functionality. The Board object does not belong to a true class, as it is unique for each Euresys board installed inside a host computer. There is no need for the user to instantiate a Board class object using the McCreate or McCreateNm function. The Board objects are natively made available to the application for each installed Euresys board when the MultiCam driver is opened.

### 3.1. Board Information Category

*Parameters providing access to identification, structure or security features of the board*

#### BoardTopology

*Arrangement of the cameras connected to the board and features set*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `59 << 14` |
| String ID | `BoardTopology` |
| C/C++ ID | `MC_BoardTopology` |

**Description:**

This parameter defines the arrangement of cameras that can be potentially connected to the frame grabber. When multiple feature sets are available for a board, it allows to select the appropriate feature set for the application.

**Usage:**

Directive: The application must set this parameter before the first assignation of a MultiCam Channel to this board; it must not be modified while at least one channel is assigned to the board. Directive: The parameter value can be modified only if no Camera Link serial port is in use. Otherwise, the MultiCam driver will return an MC_IO_ERROR status.

**Values:**

- **`MONO`**
  - C/C++: `MC_BoardTopology_MONO`
  - One Camera Link Base or one Camera Link Lite camera attached to the CAMERA connector.
  - One Camera Link Base camera attached to the BASE connector or ... one Camera Link Medium or one Camera Link Full camera attached to the BASE and MEDIUM/FULL connectors.
  > **NOTE:** Do not use for Camera Link clock frequencies below 30 MHz! Default value. MONO_OPT1 Base Full FullXR MC_BoardTopology_MONO_OPT1 Base Description One Camera Link Base or one Camera Link Lite camera attached to the CAMERA connector. Includes the pipeline controller optional feature. Full FullXR Description One Camera Link Base camera attached to the BASE connector or ... one Camera Link Medium or one Camera Link Full camera attached to the BASE and MEDIUM/FULL connectors. Includes the pipeline controller optional feature. NOTE Do not use for Camera Link clock frequencies below 30 MHz! MONO_DECA Full FullXR MC_BoardTopology_MONO_DECA Description One Camera Link 72 Bit camera or one Camera Link 80 Bit camera attached to the BASE and MEDIUM/FULL connectors. MONO_DECA_OPT1 Full FullXR MC_BoardTopology_MONO_DECA_OPT1 Description One Camera Link 72 Bit camera or one Camera Link 80 Bit camera attached to the BASE and MEDIUM/FULL connectors. Includes the pipeline controller optional feature. MONO_SLOW Base Full FullXR MC_BoardTopology_MONO_SLOW Base Description One Camera Link Base or one Camera Link Lite camera attached to the CAMERA connector. The cable deskewing function of the Camera Link interface is turned off. NOTE This setting is mandatory for Camera Link clock frequencies below 30 MHz. Full FullXR Description One Camera Link Base camera attached to the BASE connector or ... one Camera Link Medium or one Camera Link Full camera attached to the BASE and MEDIUM/FULL connectors. The cable deskewing function of the Camera Link interface is turned off. NOTE This setting is mandatory for Camera Link clock frequencies below 30 MHz. DUO DualBase MC_BoardTopology_DUO Description One Camera Link Base or one Camera Link Lite camera attached to the A connector and ... one Camera Link Base or one Camera Link Lite camera attached to the B connector. NOTE Do not use for Camera Link clock frequencies below 30 MHz! Default value. DUO_OPT1 DualBase MC_BoardTopology_DUO_OPT1 Description One Camera Link Base or one Camera Link Lite camera attached to the A connector and ... one Camera Link Base or one Camera Link Lite camera attached to the B connector. Includes the pipeline controller optional feature. NOTE Do not use for Camera Link clock frequencies below 30 MHz! DUO_SLOW DualBase
  - C/C++: `MC_BoardTopology_DUO_SLOW`
  - One Camera Link Base or one Camera Link Lite camera attached to the A connector and ... one Camera Link Base or one Camera Link Lite camera attached to the B connector. The cable deskewing function of the Camera Link interface is turned off.
  > **NOTE:** This setting is mandatory for Camera Link clock frequencies below 30 MHz.

---

#### SerialLinkA

*Serial COM receiver source of Camera connector A or M*

**Boards:** Base, DualBase

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10565 << 14` |
| String ID | `SerialLinkA` |
| C/C++ ID | `MC_SerialLinkA` |

**Description:**

Selects the receiver source of the serial COM of the first Camera connector.

**Usage:**

Directive: Set POCL_LITE when attaching a PoCL-Lite camera.

**Values:**

- **`STANDARD`**
  - C/C++: `MC_SerialLinkA_STANDARD`
  - The camera-to-frame-grabber serial communication link uses a dedicated line of the standard Camera Link cable Default value.
- **`POCL_LITE`**
  - C/C++: `MC_SerialLinkA_POCL_LITE`
  - The camera-to-frame-grabber serial link is embedded in the Channel Link of PoCL-Lite Camera
- **`Link cables`**

---

#### SerialLinkB

*Serial COM receiver source of Camera connector B*

**Boards:** DualBase

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10566 << 14` |
| String ID | `SerialLinkB` |
| C/C++ ID | `MC_SerialLinkB` |

**Description:**

Selects the receiver source of the serial COM of the second Camera connector.

**Usage:**

Directive: Set POCL_LITE when attaching a PoCL-Lite camera.

**Values:**

- **`STANDARD`**
  - C/C++: `MC_SerialLinkB_STANDARD`
  - The camera-to-frame-grabber serial communication link uses a dedicated line of the standard Camera Link cable Default value.
- **`POCL_LITE`**
  - C/C++: `MC_SerialLinkB_POCL_LITE`
  - The camera-to-frame-grabber serial link is embedded in the Channel Link of PoCL-Lite Camera
- **`Link cables`**

---

#### DriverIndex

*Board index in the list of MultiCam compliant boards returned by the driver*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `0 << 14` |
| String ID | `DriverIndex` |
| C/C++ ID | `MC_DriverIndex` |

**Description:**

This parameter gives the index of a particular board in the list returned by the driver. This parameter is used to access the Board object parameters related to the board. The MultiCam compliant boards are assigned consecutive integer numbers starting at 0. The indexing order is system dependent.

---

#### PCIPosition

*Board index in the list of PCI slots*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `1 << 14` |
| String ID | `PCIPosition` |
| C/C++ ID | `MC_PCIPosition` |

**Description:**

This parameter gives the index of the PCI slot associated to a board. This number is assigned by the operating system in a non-predictable way, but remains consistent for a given configuration in a given system.

---

#### BoardName

*Name of the board*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | String |
| Access | Set and Get |
| Num ID | `2 << 14` |
| String ID | `BoardName` |
| C/C++ ID | `MC_BoardName` |

**Description:**

This parameter returns the name of the board. The name is a string of maximum 16 ASCII characters.

---

#### BoardIdentifier

*Identifier of the board, made by the combination of its type and serial number*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | String |
| Access | Get Only |
| Num ID | `3 << 14` |
| String ID | `BoardIdentifier` |
| C/C++ ID | `MC_BoardIdentifier` |

**Description:**

This parameter gives the board type and its serial number, providing a unique way to designate a Euresys board. The board identifier is an ASCII character string, resulting from the concatenation of the board type and the serial number, with an intervening underscore. The serial number is a 6-digit string made of characters 0 to 9;for instance, GRABLINK_FULL_000123. Refer to BoardType for available board types.

---

#### NameBoard

*Naming of the selected board*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | String |
| Access | Set Only |
| Num ID | `4 << 14` |
| String ID | `NameBoard` |
| C/C++ ID | `MC_NameBoard` |

**Description:**

Setting this parameter writes the name to the selected board. This name is stored inside an on- board non-volatile memory. The name is a string of maximum 16 ASCII characters.

---

#### SerialNumber

*Unique serial number of the board*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `5 << 14` |
| String ID | `SerialNumber` |
| C/C++ ID | `MC_SerialNumber` |

**Description:**

This parameter returns the serial number assigned to the selected board. This 6-digit number is unique for a board of a given type.

**Values:**

- `0`: Minimum range value.
- `999999`: Maximum range value. BoardType Type of the board

---

#### BoardType

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `6 << 14` |
| String ID | `BoardType` |
| C/C++ ID | `MC_BoardType` |

**Values:**

- **`GRABLINK_BASE`**
  - C/C++: `MC_BoardType_GRABLINK_BASE`
  - PC1624 Grablink Base
- **`GRABLINK_DUALBASE`**
  - C/C++: `MC_BoardType_GRABLINK_DUALBASE`
  - PC1623 Grablink DualBase
- **`GRABLINK_FULL`**
  - C/C++: `MC_BoardType_GRABLINK_FULL`
  - PC1622 Grablink Full
- **`GRABLINK_FULL_XR`**
  - C/C++: `MC_BoardType_GRABLINK_FULL_XR`
  - PC1626 Grablink Full XR

---

#### SerialControlA

*Creation of a serial link through a virtual COM port*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | String |
| Access | Set and Get |
| Num ID | `70 << 14` |
| String ID | `SerialControlA` |
| C/C++ ID | `MC_SerialControlA` |

**Description:**

This parameter declares which virtual COM port is associated with the serial link associated with camera connector M. A "set" operation on the SerialControlA parameter fails if the virtual COM port name is longer than 235 characters. The returned error code is MC_INVALID_VALUE. Status = McSetParamStr(MC_BOARD+1, MC_SerialControlA, "COM4");

---

#### SerialControlB

*Creation of a serial link through virtual COM port*

**Boards:** DualBase

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | String |
| Access | Set and Get |
| Num ID | `71 << 14` |
| String ID | `SerialControlB` |
| C/C++ ID | `MC_SerialControlB` |

**Description:**

This parameter declares which virtual COM port is associated with the serial link associated with camera connector B. A "set" operation on the SerialControlB parameter fails if the virtual COM port name is longer than 235 characters. The returned error code is MC_INVALID_VALUE. Status = McSetParamStr(MC_BOARD+1, MC_SerialControlB, "COM5");

---

#### PCIeDeviceID

*Identification number assigned to the board on the PCI Express system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `2911 << 14` |
| String ID | `PCIeDeviceID` |
| C/C++ ID | `MC_PCIeDeviceID` |

**Description:**

Getting this parameter returns the board ID on the PCI Express system (when the board is configured in normal mode).

**Values:**

- `778`: PC1622 Grablink Full - Normal mode
- `779`: PC1622 Grablink Full - Recovery mode
- `780`: PC1623 Grablink DualBase - Normal mode
- `781`: PC1623 Grablink DualBase - Recovery mode
- `782`: PC1624 Grablink Base - Normal mode
- `783`: PC1624 Grablink Base - Recovery mode
- `784`: PC1626 Grablink Full XR - Normal mode
- `785`: PC1626 Grablink Full XR - Recovery mode PCIeLinkWidth Negotiated width of the PCI Express link

---

#### PCIeLinkWidth

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `10310 << 14` |
| String ID | `PCIeLinkWidth` |
| C/C++ ID | `MC_PCIeLinkWidth` |

**Values:**

- `1`: 1 lane
- `1`: 1 lane
- `4`: 4 lanes PCIePayloadSize Negotiated payload size of the Transport Layer Packets (TLP)

---

#### PCIePayloadSize

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `10403 << 14` |
| String ID | `PCIePayloadSize` |
| C/C++ ID | `MC_PCIePayloadSize` |

**Values:**

- `128`: 128 bytes
- `256`: 256 bytes
- `512`: 512 bytes
- `1024`: 1024 bytes PCIeEndPointRevisionId Revision number of the PCI Express end point firmware

---

#### PCIeEndPointRevisionId

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `10311 << 14` |
| String ID | `PCIeEndPointRevisionId` |
| C/C++ ID | `MC_PCIeEndPointRevisionId` |

---

#### PoCL_PowerInput

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | EXPERT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `9943 << 14` |
| String ID | `PoCL_PowerInput` |
| C/C++ ID | `MC_PoCL_PowerInput` |

**Values:**

- **`ON`**
  - C/C++: `MC_PoCL_PowerInput_ON`
  - A 12V power supply is connected to the camera power connector.
- **`OFF`**
  - C/C++: `MC_PoCL_PowerInput_OFF`
  - No power supply is connected to the camera power connector.

---

#### OemSafetyLock

*Control for locking and checking the board*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | EXPERT |
| Type | String |
| Access | Set and Get |
| Num ID | `8 << 14` |
| String ID | `OemSafetyLock` |
| C/C++ ID | `MC_OemSafetyLock` |

**Description:**

This parameter, along with OemSafetyKey , provides a method to assign a safety key to the selected board. The key is an 8-byte string of ASCII characters. Any character is allowed. A null character acts as the termination character of the safety key. The value when "set" is an 8-byte string of ASCII characters. The entered key is stored in the non-volatile memory of the board and cannot be read back. The "set" operation fails if the key is longer than 8 characters. In that case, the returned error code is MC_INVALID_VALUE. The value when "get" is the string TRUE or FALSE, that is the validity of the key, which has been previously entered under OemSafetyKey . See also "Board Security Feature" on page 504.

---

#### OemSafetyKey

*Safety key for key checking*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Board Information |
| Level | EXPERT |
| Type | String |
| Access | Set Only |
| Num ID | `9 << 14` |
| String ID | `OemSafetyKey` |
| C/C++ ID | `MC_OemSafetyKey` |

**Description:**

This parameter, along with OemSafetyLock , provides a method to assign a safety key to the selected board. The key is implemented as a an 8-byte string of ASCII characters. Any character is allowed. A null character acts as the termination character of the safety key. The key is stored in the non-volatile memory of the board and cannot be read back. The validity of the key is returned by OemSafetyLock . A "set" operation on the OemSafetyLock parameter fails if the key is longer than 8 characters. The returned error code is MC_INVALID_VALUE. See also "Board Security Feature" on page 504.

---

### 3.2. Input/Output Control Category

*Parameters providing access to input and output digital lines featured by the board*

#### InputConfig

*Setting of the I/O lines used as inputs*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices.

**Values:**

- **`SOFT`**
  - C/C++: `MC_InputConfig_SOFT`
  - Declares that the I/O line is locked for general-purpose software input function.
- **`FREE`**
  - C/C++: `MC_InputConfig_FREE`
  - Declares the I/O line to be used for any allowed function.

---

#### OutputConfig

*Configuration of the I/O lines used as outputs*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices.

**Values:**

- **`SOFT`**
  - C/C++: `MC_OutputConfig_SOFT`
  - Declares that the I/O line is locked for general-purpose software output function.
- **`FREE`**
  - C/C++: `MC_OutputConfig_FREE`
  - Declares the I/O line to be used for any allowed function.
- **`EVENT`**
  - C/C++: `MC_OutputConfig_EVENT`
  - Declares the I/O line to be used to report an event.

---

#### InputFunction

*Report of the I/O lines used as inputs*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. The values are specific to each collection member. For further information, refer to the handbooks.

**Values:**

- **`FREE`**
  - C/C++: `MC_InputFunction_FREE`
  - The input line is free from software and channel use. Default value.
- **`SOFT`**
  - C/C++: `MC_InputFunction_SOFT`
  - The I/O line is used as a general-purpose software-controlled input.
- **`NONE`**
  - C/C++: `MC_InputFunction_NONE`
  - The I/O line does not exist.
- **`UNKNOWN`**
  - C/C++: `MC_InputFunction_UNKNOWN`
  - The functional input usage of the I/O line cannot be determined.
- **`LVAL`**
  - C/C++: `MC_InputFunction_LVAL`
  - The I/O line is used to monitor a channel link LVAL.
- **`FVAL`**
  - C/C++: `MC_InputFunction_FVAL`
  - The I/O line is used to monitor a channel link FVAL.
- **`DVAL`**
  - C/C++: `MC_InputFunction_DVAL`
  - The I/O line is used to monitor a channel link DVAL.
- **`SPARE`**
  - C/C++: `MC_InputFunction_SPARE`
  - The I/O line is used to monitor a channel link SPARE.
- **`CK_PRESENT`**
  - C/C++: `MC_InputFunction_CK_PRESENT`
  - The I/O line is used for channel link clock presence indication.
- **`POWERSTATE5V`**
  - C/C++: `MC_InputFunction_POWERSTATE5V`
  - The I/O line is used for 5V power presence indication.
- **`POWERSTATE12V`**
  - C/C++: `MC_InputFunction_POWERSTATE12V`
  - The I/O line is used for 12V power presence indication.

---

#### OutputFunction

*Report of the I/O lines used as outputs*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. The values are specific to each collection member. For further information, refer to the handbooks.

**Values:**

- **`SOFT`**
  - C/C++: `MC_OutputFunction_SOFT`
  - The I/O line is used as a general-purpose software-controlled output.
- **`FREE`**
  - C/C++: `MC_OutputFunction_FREE`
  - The I/O line is free from software or channel use. Default value.
- **`NONE`**
  - C/C++: `MC_OutputFunction_NONE`
  - The I/O line does not exist.
- **`UNKNOWN`**
  - C/C++: `MC_OutputFunction_UNKNOWN`
  - The functional output usage of the I/O line cannot be determined.

---

#### InputState

*Report of the logic state of I/O lines used as inputs*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. Getting the InputState enumerated parameter delivers the present status of the interrogated input line.
- 
The value NONE is reported when the corresponding InputFunction parameter is UNKNOWN.
- 
A MultiCam error is reported when the corresponding InputFunction parameter is NONE.

**Values:**

- **`LOW`**
  - C/C++: `MC_InputState_LOW`
  - Presently at the low logic state.
  - For isolated current-sense inputs: input current < 1 mA, or unconnected input port For high-speed differential inputs: input voltage (VIN+ - VIN-) < VThreshold
- **`HIGH`**
  - C/C++: `MC_InputState_HIGH`
  - Presently at the high logic state.
  - For isolated current-sense inputs, input current > 1 mA For high-speed differential inputs, input voltage (VIN+ - VIN-) > VThreshold, or unconnected input port

---

#### OutputState

*Logic state of I/O lines used as outputs*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. Getting the OutputState parameter is only allowed when the corresponding OutputFunction parameter is SOFT. The returned value is the one that has been previously set. The value NONE is reported when the corresponding OutputFunction parameter is other than SOFT.

**Values:**

- **`LOW`**
  - C/C++: `MC_OutputState_LOW`
  - The contact switch of isolated outputs is open(OFF). Initial state after Power-On.
- **`HIGH`**
  - C/C++: `MC_OutputState_HIGH`
  - The contact switch is closed (ON).
- **`TOGGLE`**
  - C/C++: `MC_OutputState_TOGGLE`
  - A logic state opposite to the present one is issued.
- **`NONE`**
  - C/C++: `MC_OutputState_NONE`
  - The I/O line is not presently used as an output.

---

#### SetSignal

*Event source selection to set the EVENT register*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

Selects an event source to set the EVENT register driving the EVENT signal of the selected output port.

**Usage:**

Relevance condition(s): Condition: OutputConfig is set to EVENT.

**Values:**

- **`NONE`**
  - C/C++: `MC_SetSignal_NONE`
  - All event sources are disconnected. Default value.
- **`SCA`**
  - C/C++: `MC_SetSignal_SCA`
  - The 'Start Channel Activity' event source is selected.
- **`ECA`**
  - C/C++: `MC_SetSignal_ECA`
  - The 'End Channel Activity' event source is selected.
- **`SAP`**
  - C/C++: `MC_SetSignal_SAP`
  - The 'Start Acquisition Phase' event source is selected.
- **`EAP`**
  - C/C++: `MC_SetSignal_EAP`
  - The 'End Acquisition Phase' event source is selected.
- **`SAS`**
  - C/C++: `MC_SetSignal_SAS`
  - The 'Start Acquisition Sequence' event source is selected.
- **`EAS`**
  - C/C++: `MC_SetSignal_EAS`
  - The 'End Acquisition Sequence' event source is selected.
- **`FVAL_GOHIGH`**
  - C/C++: `MC_SetSignal_FVAL_GOHIGH`
  - The 'FVAL Going High' event source is selected.
- **`FVAL_GOLOW`**
  - C/C++: `MC_SetSignal_FVAL_GOLOW`
  - The 'FVAL Going Low' event source is selected.
- **`LVAL_GOHIGH`**
  - C/C++: `MC_SetSignal_LVAL_GOHIGH`
  - The 'LVAL Going High' event source is selected.
- **`LVAL_GOLOW`**
  - C/C++: `MC_SetSignal_LVAL_GOLOW`
  - The 'LVAL Going Low' event source is selected.
- **`DVAL_GOHIGH`**
  - C/C++: `MC_SetSignal_DVAL_GOHIGH`
  - The 'DVAL Going High' event source is selected.
- **`DVAL_GOLOW`**
  - C/C++: `MC_SetSignal_DVAL_GOLOW`
  - The 'DVAL Going Low' event source is selected.
- **`CC1_GOHIGH`**
  - C/C++: `MC_SetSignal_CC1_GOHIGH`
  - The 'CC1 Going High' event source is selected.
- **`CC1_GOLOW`**
  - C/C++: `MC_SetSignal_CC1_GOLOW`
  - The 'CC1 Going Low' event source is selected.
- **`CC2_GOHIGH`**
  - C/C++: `MC_SetSignal_CC2_GOHIGH`
  - The 'CC2 Going High' event source is selected.
- **`CC2_GOLOW`**
  - C/C++: `MC_SetSignal_CC2_GOLOW`
  - The 'CC2 Going Low' event source is selected.
- **`CC3_GOHIGH`**
  - C/C++: `MC_SetSignal_CC3_GOHIGH`
  - The 'CC3 Going High' event source is selected.
- **`CC3_GOLOW`**
  - C/C++: `MC_SetSignal_CC3_GOLOW`
  - The 'CC3 Going Low' event source is selected.
- **`CC4_GOHIGH`**
  - C/C++: `MC_SetSignal_CC4_GOHIGH`
  - The 'CC4 Going High' event source is selected.
- **`CC4_GOLOW`**
  - C/C++: `MC_SetSignal_CC4_GOLOW`
  - The 'CC4 Going Low' event source is selected.

---

#### ResetSignal

*Event source selection to reset the EVENT register*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

Selects an event source to reset the EVENT register driving the EVENT signal of the selected output port.

**Usage:**

Relevance condition(s): Condition: OutputConfig is set to EVENT.

**Values:**

- **`NONE`**
  - C/C++: `MC_ResetSignal_NONE`
  - All event sources are disconnected. Default value.
- **`SCA`**
  - C/C++: `MC_ResetSignal_SCA`
  - The 'Start Channel Activity' event source is selected.
- **`ECA`**
  - C/C++: `MC_ResetSignal_ECA`
  - The 'End Channel Activity' event source is selected.
- **`SAP`**
  - C/C++: `MC_ResetSignal_SAP`
  - The 'Start Acquisition Phase' event source is selected.
- **`EAP`**
  - C/C++: `MC_ResetSignal_EAP`
  - The 'End Acquisition Phase' event source is selected.
- **`SAS`**
  - C/C++: `MC_ResetSignal_SAS`
  - The 'Start Acquisition Sequence' event source is selected.
- **`EAS`**
  - C/C++: `MC_ResetSignal_EAS`
  - The 'End Acquisition Sequence' event source is selected.
- **`FVAL_GOHIGH`**
  - C/C++: `MC_ResetSignal_FVAL_GOHIGH`
  - The 'FVAL Going High' event source is selected.
- **`FVAL_GOLOW`**
  - C/C++: `MC_ResetSignal_FVAL_GOLOW`
  - The 'FVAL Going Low' event source is selected.
- **`LVAL_GOHIGH`**
  - C/C++: `MC_ResetSignal_LVAL_GOHIGH`
  - The 'LVAL Going High' event source is selected.
- **`LVAL_GOLOW`**
  - C/C++: `MC_ResetSignal_LVAL_GOLOW`
  - The 'LVAL Going Low' event source is selected.
- **`DVAL_GOHIGH`**
  - C/C++: `MC_ResetSignal_DVAL_GOHIGH`
  - The 'DVAL Going High' event source is selected.
- **`DVAL_GOLOW`**
  - C/C++: `MC_ResetSignal_DVAL_GOLOW`
  - The 'DVAL Going Low' event source is selected.
- **`CC1_GOHIGH`**
  - C/C++: `MC_ResetSignal_CC1_GOHIGH`
  - The 'CC1 Going High' event source is selected.
- **`CC1_GOLOW`**
  - C/C++: `MC_ResetSignal_CC1_GOLOW`
  - The 'CC1 Going Low' event source is selected.
- **`CC2_GOHIGH`**
  - C/C++: `MC_ResetSignal_CC2_GOHIGH`
  - The 'CC2 Going High' event source is selected.
- **`CC2_GOLOW`**
  - C/C++: `MC_ResetSignal_CC2_GOLOW`
  - The 'CC2 Going Low' event source is selected.
- **`CC3_GOHIGH`**
  - C/C++: `MC_ResetSignal_CC3_GOHIGH`
  - The 'CC3 Going High' event source is selected.
- **`CC3_GOLOW`**
  - C/C++: `MC_ResetSignal_CC3_GOLOW`
  - The 'CC3 Going Low' event source is selected.
- **`CC4_GOHIGH`**
  - C/C++: `MC_ResetSignal_CC4_GOHIGH`
  - The 'CC4 Going High' event source is selected.
- **`CC4_GOLOW`**
  - C/C++: `MC_ResetSignal_CC4_GOLOW`
  - The 'CC4 Going Low' event source is selected.

---

#### InputStyle

*Electrical style of I/O lines used as inputs*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. Setting InputStyle to a precise value yields better electrical performance, such as better common mode rejection ratio. The values are specific to each collection member. For further information, refer to the handbooks.

**Values:**

- **`CHANNELLINK`**
  - C/C++: `MC_InputStyle_CHANNELLINK`
  - The input line is a signal embedded in Channel Link.
- **`ISO`**
  - C/C++: `MC_InputStyle_ISO`
  - The input line is an isolated current-sense input with wide voltage input range up to 30V, compatible with totem-pole LVTTL, TTL, 5V CMOS drivers, RS-422 differential line drivers, potential free contacts, solid-state relays and opto-couplers.
- **`DIFF`**
  - C/C++: `MC_InputStyle_DIFF`
  - The input line is a high-speed differential input compatible with ANSI/EIA/TIA-422/485 differential line drivers and complementary TTL drivers.
- **`POWERSTATE`**
  - C/C++: `MC_InputStyle_POWERSTATE`
  - The input line reports the state of a power input.

---

#### OutputStyle

*Electrical style of I/O lines used as outputs*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. The values are specific to each collection member. For further information, refer to the handbooks.

**Values:**

- **`OPTO`**
  - C/C++: `MC_OutputStyle_OPTO`
  - Isolated contact outputs compatible with 30V / 100mA loads.
- **`LVDS`**
  - C/C++: `MC_OutputStyle_LVDS`
  - The output line is differential LVDS, RS-422 or RS-485 compatible.

---

#### InputPinName

*Pin name of the I/O line used as input*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. The values are specific to each collection member. For further information, refer to the handbooks.

**Values:**

- **`UNKNOWN`**
  - C/C++: `MC_InputPinName_UNKNOWN`
  - The I/O line does not exist.
- **`FVAL`**
  - C/C++: `MC_InputPinName_FVAL`
  - The I/O line is issued from Camera Link camera connector pins named FVAL.
- **`DVAL`**
  - C/C++: `MC_InputPinName_DVAL`
  - The I/O line is issued from Camera Link camera connector pins named DVAL.
- **`LVAL`**
  - C/C++: `MC_InputPinName_LVAL`
  - The I/O line is issued from Camera Link camera connector pins named LVAL.
- **`SPARE`**
  - C/C++: `MC_InputPinName_SPARE`
  - The I/O line is issued from Camera Link camera connector pins named SPARE.
- **`IIN1`**
  - C/C++: `MC_InputPinName_IIN1`
  - The I/O line is issued from an I/O connector pin named IIN1.
- **`IIN2`**
  - C/C++: `MC_InputPinName_IIN2`
  - The I/O line is issued from an I/O connector pin named IIN2.
- **`IIN3`**
  - C/C++: `MC_InputPinName_IIN3`
  - The I/O line is issued from an I/O connector pin named IIN3.
- **`IIN4`**
  - C/C++: `MC_InputPinName_IIN4`
  - The I/O line is issued from an I/O connector pin named IIN4.
- **`DIN1`**
  - C/C++: `MC_InputPinName_DIN1`
  - The I/O line is issued from an I/O connector pin named DIN1.
- **`DIN2`**
  - C/C++: `MC_InputPinName_DIN2`
  - The I/O line is issued from an I/O connector pin named DIN2.
- **`LVAL_X`**
  - C/C++: `MC_InputPinName_LVAL_X`
  - The I/O line is issued from Camera Link Channel X camera connector pins named LVAL.
- **`FVAL_X`**
  - C/C++: `MC_InputPinName_FVAL_X`
  - The I/O line is issued from Camera Link Channel X camera connector pins named FVAL.
- **`DVAL_X`**
  - C/C++: `MC_InputPinName_DVAL_X`
  - The I/O line is issued from Camera Link Channel X camera connector pins named DVAL.
- **`SPARE_X`**
  - C/C++: `MC_InputPinName_SPARE_X`
  - The I/O line is issued from Camera Link Channel X camera connector pins named SPARE.
- **`CK_PRESENT_X`**
  - C/C++: `MC_InputPinName_CK_PRESENT_X`
  - The I/O line is issued from the Camera Link Channel X clock presence detector.
- **`LVAL_Y`**
  - C/C++: `MC_InputPinName_LVAL_Y`
  - The I/O line is issued from Camera Link Channel Y camera connector pins named LVAL.
- **`FVAL_Y`**
  - C/C++: `MC_InputPinName_FVAL_Y`
  - The I/O line is issued from Camera Link Channel Y camera connector pins named FVAL.
- **`DVAL_Y`**
  - C/C++: `MC_InputPinName_DVAL_Y`
  - The I/O line is issued from Camera Link Channel Y camera connector pins named DVAL.
- **`SPARE_Y`**
  - C/C++: `MC_InputPinName_SPARE_Y`
  - The I/O line is issued from Camera Link Channel Y camera connector pins named SPARE.
- **`CK_PRESENT_Y`**
  - C/C++: `MC_InputPinName_CK_PRESENT_Y`
  - The I/O line is issued from the Camera Link Channel Y clock presence detector.
- **`LVAL_Z`**
  - C/C++: `MC_InputPinName_LVAL_Z`
  - The I/O line is issued from Camera Link Channel Z camera connector pins named LVAL.
- **`FVAL_Z`**
  - C/C++: `MC_InputPinName_FVAL_Z`
  - The I/O line is issued from Camera Link Channel Z camera connector pins named FVAL.
- **`DVAL_Z`**
  - C/C++: `MC_InputPinName_DVAL_Z`
  - The I/O line is issued from Camera Link Channel Z camera connector pins named DVAL.
- **`SPARE_Z`**
  - C/C++: `MC_InputPinName_SPARE_Z`
  - The I/O line is issued from Camera Link Channel Z camera connector pins named SPARE.
- **`CK_PRESENT_Z`**
  - C/C++: `MC_InputPinName_CK_PRESENT_Z`
  - The I/O line is issued from the Camera Link Channel Z clock presence detector.
- **`POWER_5V`**
  - C/C++: `MC_InputPinName_POWER_5V`
  - The I/O line is issued from the voltage monitor of the +5 V power input.
- **`POWER_12V`**
  - C/C++: `MC_InputPinName_POWER_12V`
  - The I/O line is issued from the voltage monitor of the +12 V power input.

---

#### OutputPinName

*Pin name of the I/O line used as the output*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant output designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices. The values are specific to each collection member. For further information, refer to the handbooks.

**Values:**

- **`UNKNOWN`**
  - C/C++: `MC_OutputPinName_UNKNOWN`
  - The I/O line does not exist.
- **`CC1`**
  - C/C++: `MC_OutputPinName_CC1`
  - The I/O line is driving Camera Link connector pin named CC1.
- **`CC2`**
  - C/C++: `MC_OutputPinName_CC2`
  - The I/O line is driving Camera Link connector pin named CC2.
- **`CC3`**
  - C/C++: `MC_OutputPinName_CC3`
  - The I/O line is driving Camera Link connector pin named CC3.
- **`CC4`**
  - C/C++: `MC_OutputPinName_CC4`
  - The I/O line is driving Camera Link connector pin named CC4.
- **`IOUT1`**
  - C/C++: `MC_OutputPinName_IOUT1`
  - The I/O line is driving connector pin named IOUT1.
- **`IOUT2`**
  - C/C++: `MC_OutputPinName_IOUT2`
  - The I/O line is driving connector pin named IOUT2.
- **`IOUT3`**
  - C/C++: `MC_OutputPinName_IOUT3`
  - The I/O line is driving connector pin named IOUT3.
- **`IOUT4`**
  - C/C++: `MC_OutputPinName_IOUT4`
  - The I/O line is driving connector pin named IOUT4.

---

#### ConnectorName

*Connector name of the I/O lines used as input*

| Property | Value |
|----------|-------|
| Class | Board |
| Category | Input/Output Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | collection |

**Description:**

The item number of this collection parameter is used as an index to point the relevant input designator among the set of designators owned by selected board. Refer to "I/O Indices Catalog" on page 498 for a list of I/O indices.

**Values:**

- **`UNKNOWN`**
  - C/C++: `MC_ConnectorName_UNKNOWN`
  - The I/O line does not exist.
- **`IO`**
  - C/C++: `MC_ConnectorName_IO`
  - The I/O lines are available on the connector named I/O.
- **`CAMERA`**
  - C/C++: `MC_ConnectorName_CAMERA`
  - The I/O lines are available on the connector named Camera.
- **`CAMERA_B`**
  - C/C++: `MC_ConnectorName_CAMERA_B`
  - The I/O lines are available on the connector named Camera B.
- **`IO_A`**
  - C/C++: `MC_ConnectorName_IO_A`
  - The I/O lines are available on the connector named I/O A.
- **`IO_B`**
  - C/C++: `MC_ConnectorName_IO_B`
  - The I/O lines are available on the connector named I/O B.

---

## 4. Channel Class

What Is a Channel? The Channel class groups all MultiCam parameters dedicated to the control of image acquisition related features. A Channel object is an instance of the Channel class, represented by a dedicated set of such parameters. Typically, the following items are defined and controlled by the Channel object:
- 
The camera feeding the channel, including reset and exposure control
- 
The connector and cable linking the camera to the frame grabber
- 
The switching structures routing the analog or digital video signal inside the frame grabbe
- 
In case of analog camera, the analog-to-digital converter and the associated signal conditioning devices
- 
In case of digital camera, the digital receiving or de-serializing devices
- 
The timing generator and controller associated to the camera, and the video signal conditioning
- 
All digital devices affecting the signal during acquisition, performing tasks such as lookup tables, byte alignment, data channel merging...
- 
The data buffer receiving the images
- 
The DMA devices extracting images out of the data buffer for transfer into host memory
- 
The destination cluster of host memory surfaces
- 
The hardware resources managing the external system trigger The channel is the association of an individual grabber connected to a camera delivering data to a set of surfaces, called a cluster. The channel is able to transport an image from the camera towards a surface belonging to the cluster and usually located in the host memory.

### 4.1. Camera Specification Category

*Parameters specifying the type and operational mode of the camera feeding the channel*

#### CamFile

*Name of the CAM file*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Specification |
| Level | SELECT |
| Type | String |
| Access | Set and Get |
| Num ID | `11 << 14` |
| String ID | `CamFile` |
| C/C++ ID | `MC_CamFile` |

**Description:**

This parameter specifies a camera configuration file as a character string. The .cam extension may or may not be included. The maximum string length is 1024. Getting this parameter returns the name of the lastly executed CAM file. Refer to CAM Files for CAM file syntax and location. See also "CAM Files" on page 523 in the MultiCam user guide for more information.

---

#### Camera

*Camera model attached to the grabber*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Specification |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `700 << 14` |
| String ID | `Camera` |
| C/C++ ID | `MC_Camera` |

**Description:**

Together with CamConfig, this parameter defines a coherent set of camera properties.

**Values:**

- **`MyCameraLink`**
  - C/C++: `MC_Camera_MyCameraLink`
  - Generic Camera Link camera

---

#### CamConfig

*Configuration of the camera model attached to the grabber*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Specification |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `701 << 14` |
| String ID | `CamConfig` |
| C/C++ ID | `MC_CamConfig` |

**Description:**

Together with Camera, this parameter defines a coherent set of camera properties. For Grablink products, the parameter complies to the following syntax: <Imaging>xx [xx]<CamMode><Exp>where:
- 
Imaging designates the type of imaging device:
- 
L: Line-scan imaging device
- 
P: Progressive area-scan imaging device
- 
CamMode designates the main camera operating mode:
- 
R: Asynchronous Reset operating mode. The camera initiates an exposure/readout sequence when it gets a "Reset" signal from the frame grabber
- 
S: Synchronous operating mode. The camera is free-running and delivers permanently video data
- 
Exp designates the exposure control method:
- 
C: The exposure is controlled by the camera
- 
G: The exposure is controlled by the frame grabber
- 
P: The camera sensor has no exposure control .It is exposed permanently.

**Values:**

- **`LxxxxRC`**
  - C/C++: `MC_CamConfig_LxxxxRC`
  - Grabber-controlled rate, camera-controlled exposure time, line-scan camera. The exposure duration is set through camera switches or serial control. The camera cycles are triggered by a pulse over a "Reset" line issued by the frame grabber.
  - *Condition: Camera is set to MyCameraLink*
- **`LxxxxRG`**
  - C/C++: `MC_CamConfig_LxxxxRG`
  - Grabber-controlled line rate, grabber-controlled exposure, line-scan camera. The exposure duration is defined as the active duration of a pulse over a "Reset" line issued by the frame grabber.
  - *Condition: Camera is set to MyCameraLink*
- **`LxxxxRP`**
  - C/C++: `MC_CamConfig_LxxxxRP`
  - Grabber-controlled rate, permanent exposure, line-scan camera. The camera has no exposure control capability, resulting in permanent exposure. The camera cycles are triggered by a pulse over a "Reset" line issued by the frame grabber.
  - *Condition: Camera is set to MyCameraLink*
- **`LxxxxSC`**
  - C/C++: `MC_CamConfig_LxxxxSC`
  - Free-running, camera-controlled exposure time, line-scan camera. The exposure duration is set through camera switches or serial control. The camera cycles are free-running.
  - *Condition: Camera is set to MyCameraLink*
- **`LxxxxSP`**
  - C/C++: `MC_CamConfig_LxxxxSP`
  - Free-running, permanent exposure, line-scan camera. The camera has no exposure control capability, resulting in permanent exposure. The camera cycles are free-running.
  - *Condition: Camera is set to MyCameraLink*
- **`PxxRC`**
  - C/C++: `MC_CamConfig_PxxRC`
  - Progressive, asynchronous reset operation, camera-controlled exposure, area-scan camera. The exposure duration is set through camera switches or serial control. The camera cycles are triggered by a pulse over a "Reset" line issued by the frame grabber.
  - *Condition: Camera is set to MyCameraLink*
- **`PxxRG`**
  - C/C++: `MC_CamConfig_PxxRG`
  - Progressive asynchronous reset operation, grabber-controlled exposure, area-scan camera. The exposure duration is defined as the active duration of a pulse over a "Reset" line issued by the frame grabber.
  - *Condition: Camera is set to MyCameraLink*
- **`PxxSC`**
  - C/C++: `MC_CamConfig_PxxSC`
  - Progressive-scan, synchronous operation, camera-controlled exposure, area-scan camera. The exposure duration is set through camera switches or serial control. The camera cycles are free- running.
  - *Condition: Camera is set to MyCameraLink*

---

#### Imaging

*Camera imaging basic geometry*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Specification |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1007 << 14` |
| String ID | `Imaging` |
| C/C++ ID | `MC_Imaging` |

**Description:**

This parameter is used to distinguish the basic kind of camera feeding the channel. See also "Camera Imaging Basic Geometry" on page 509.

**Values:**

- **`AREA`**
  - C/C++: `MC_Imaging_AREA`
  - The currently selected camera is an area-scan model.
- **`LINE`**
  - C/C++: `MC_Imaging_LINE`
  - The currently selected camera is a line-scan model.
- **`TDI`**
  - C/C++: `MC_Imaging_TDI`
  - The currently selected camera is a TDI line-scan model.

---

#### Spectrum

*Iimaging spectral sensitivity of the specified camera*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Specification |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1008 << 14` |
| String ID | `Spectrum` |
| C/C++ ID | `MC_Spectrum` |

**Description:**

This parameter is used to distinguish the basic kind of camera feeding the channel. This information only makes sense for frame grabber able to indifferently interface to color or monochrome cameras. See also "Camera Spectral Sensitivity" on page 510. The way the color information is built at the camera's sensor is further described by the ColorMethod parameter belonging to the "Camera Features Category " on page 110. Before assigning a value to this parameter, it is mandatory to set Camera and CamConfig .

**Values:**

- **`BW`**
  - C/C++: `MC_Spectrum_BW`
  - The selected camera delivers a monochrome image.
- **`COLOR`**
  - C/C++: `MC_Spectrum_COLOR`
  - The selected camera delivers a color image.
- **`IR`**
  - C/C++: `MC_Spectrum_IR`
  - The selected camera delivers a monochrome image issued by an infra-red sensor.

---

#### DataLink

*Data transfer method of the current camera*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Specification |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1009 << 14` |
| String ID | `DataLink` |
| C/C++ ID | `MC_DataLink` |

**Description:**

This parameter is used to return some information on the basic connection structure of the camera feeding the channel. See also "Camera Data Transfer Method" on page 508.

**Values:**

- **`CAMERALINK`**
  - C/C++: `MC_DataLink_CAMERALINK`
  - The camera delivers a digital video signal complying with the Camera Link standard.

---

### 4.2. Camera Timing Category

*Parameters setting the video timing attributes of the camera feeding the channel*

#### PixelClkMode

*Camera Link clock signal characteristics*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10574 << 14` |
| String ID | `PixelClkMode` |
| C/C++ ID | `MC_PixelClkMode` |

**Description:**

Defines how the camera delivers the Camera Link clock signal.

**Usage:**

Directive: Set to INTERMITTENT when the camera doesn't permanently deliver the Camera Link clock.

**Values:**

- **`PERMANENT`**
  - C/C++: `MC_PixelClkMode_PERMANENT`
  - The camera delivers permanently the Camera Link clock signal Default value.
- **`INTERMITTENT`**
  - C/C++: `MC_PixelClkMode_INTERMITTENT`
  - The camera delivers intermittently the Camera Link clock signal

---

#### LineRate_Hz

*Camera line repetition rate, expressed in Hertz*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `705 << 14` |
| String ID | `LineRate_Hz` |
| C/C++ ID | `MC_LineRate_Hz` |

**Description:**

This parameter declares the line rate, which is the repetition frequency of the video lines scanned and delivered by the camera feeding the channel. This value is a performance figure often stated as is by the camera manufacturer. For area-scan cameras, the line rate is usually under control of the camera itself. In the special case of an area-scan camera receiving horizontal drive information from the frame grabber, the LineRate_Hz parameter expresses the recommended horizontal frequency to be applied to the camera. For line-scan cameras, the line rate is usually under control of the frame grabber. In that case, the LineRate_Hz parameter declares the maximum line frequency the camera can accept. In the special case of a line-scan camera controlling its own line timing, the LineRate_Hz parameter expresses the actual horizontal frequency set by the camera.

**Values:**

- `10`: 10 Hz Minimum range value.
- `100000`: 100,000 Hz (= 100 kHz) Maximum range value. FrameRate_mHz Camera frame repetition rate, expressed in milliHertz

---

#### FrameRate_mHz

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `2222 << 14` |
| String ID | `FrameRate_mHz` |
| C/C++ ID | `MC_FrameRate_mHz` |

**Description:**

This parameter declares the frame rate, which is the repetition frequency of the video frames scanned and delivered by the camera feeding the channel. This value is a performance figure often stated as is by the camera manufacturer.

**Values:**

- `1000`: 1,000 milliHertz (=1 Hz) Minimum range value.
- `127500000`: 127,500,000 milliHertz (=127,5 kHz) Maximum range value. LineDur_ns Total duration of the video line, expressed in nanoseconds

---

#### LineDur_ns

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `732 << 14` |
| String ID | `LineDur_ns` |
| C/C++ ID | `MC_LineDur_ns` |

**Description:**

The total duration of the video line is the inverse value of the camera line repetition rate declared by the LineRate_Hz parameter. For area-scan cameras, the line duration is the sum of the horizontal blanking period and the active part of the video line. This is a feature of the video standard the camera may comply to. For line-scan cameras, the line duration is the minimum time to scan a single line. It is a practical way to characterize the top performance of the camera.

**Values:**

- `10000`: 10,000 nanoseconds (=10 microseconds) Minimum range value.
- `100000000`: 100,000,000 nanoseconds (=100 milliseconds) Maximum range value. Vactive_Ln Number of active video lines in the frame

---

#### Vactive_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `710 << 14` |
| String ID | `Vactive_Ln` |
| C/C++ ID | `MC_Vactive_Ln` |

**Description:**

An active line is, by definition, a video line where useful visual information can appear. Blanking lines take no part in the count of active lines. In case of interlaced scanning, Vactive_Ln represents the number of active lines for both fields altogether. This is equivalent to the number of active half-lines per field. In some cases of dual-tap structure, Vactive_Ln represents the number of active lines for both channels altogether. This parameter is a measure of the height of the camera active window. It is used to characterize area-scan cameras. It is meaningless for line-scan cameras.

**Values:**

- `1`: 1 line Minimum range value.
- `65535`: 65,535 lines Maximum range value. FrameDur_us

---

#### FrameDur_us

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `2223 << 14` |
| String ID | `FrameDur_us` |
| C/C++ ID | `MC_FrameDur_us` |

**Description:**

This parameter is expressed in microseconds.

**Values:**

- `10000`: 10,000 microseconds (=10 milliseconds) Minimum range value.
- `100000000`: 100,000,000 microseconds (=100 seconds) Maximum range value. Hactive_Px Number of active pixels in the line, expressed as a number of camera sensor pixels

---

#### Hactive_Px

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1021 << 14` |
| String ID | `Hactive_Px` |
| C/C++ ID | `MC_Hactive_Px` |

**Description:**

This parameter is used to characterize digital line-scan or area-scan cameras. It announces the number of horizontal pixels belonging to the sensor that are effectively available at the camera output. This is a measure of the width of the camera active window. The allowed values are depending on several factors: board type, tap configuration and tap geometry. Refer to the Grablink User Guide for an extensive description of all cases.

---

#### VsyncAft_Ln

*Vertical delay between vertical synchronization pulse and camera active window*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `712 << 14` |
| String ID | `VsyncAft_Ln` |
| C/C++ ID | `MC_VsyncAft_Ln` |

**Description:**

The delay is expressed as the number of LVAL leading edges to ignore after the leading edge of the FVAL pulse.

**Values:**

- `0`: Minimum range value. Default value.
- `255`: 255 lines after FVAL Maximum range value. HsyncAft_Tk Horizontal delay between horizontal synchronization pulse and camera active window, expressed in TCU (Timing Clock Unit) from the Camera Link clock

---

#### HsyncAft_Tk

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1111 << 14` |
| String ID | `HsyncAft_Tk` |
| C/C++ ID | `MC_HsyncAft_Tk` |

**Description:**

This parameter applies to Camera Link compliant digital area-scan cameras that deliver a LVAL horizontal synchronization pulse used by the frame grabber to monitor the camera timing. For line-scan cameras, the delay is counted from the leading edge of LVAL delivered by the camera to the beginning of the read-out period.

**Values:**

- `-1`: 1 Camera Link clock before LVAL Minimum range value.
- `1023`: 1023 Camera Link clocks after LVAL Maximum range value. ExposeRecovery_us Minimum delay between successive expose pulses, expressed in microseconds

---

#### ExposeRecovery_us

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1311 << 14` |
| String ID | `ExposeRecovery_us` |
| C/C++ ID | `MC_ExposeRecovery_us` |

**Description:**

This parameter declares the minimum amount of time required by the camera between successive expose pulses. Its value is strictly positive.

**Values:**

- `1`: 1 microsecond Minimum range value.
- `1000000`: 1,000,000 microseconds (=1 second) Maximum range value. ReadoutRecovery_us Minimum delay between successive read-out phases, expressed in microseconds

---

#### ReadoutRecovery_us

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1312 << 14` |
| String ID | `ReadoutRecovery_us` |
| C/C++ ID | `MC_ReadoutRecovery_us` |

**Description:**

This parameter declares the minimum amount of time required by the camera between successive read-out phases. This is applicable to area-scan cameras only. The value is strictly positive.

**Values:**

- `1`: 1 microsecond Minimum range value.
- `1000000`: 1,000,000 microseconds (=1 second) Maximum range value. 4.3. Camera Features Category Parameters setting the hardware interface attributes of the camera feeding the channel TapConfiguration
- `111`: TapGeometry
- `119`: ColorMethod
- `169`: ColorRegistration
- `171`: ColorRegistrationControl
- `174`: ExposeOverlap
- `176`: Expose
- `177`: Readout
- `179`: ResetCtl
- `180`: ResetEdge
- `181`: AuxResetCtl
- `182`: AuxResetEdge
- `183`: ResetDur
- `184`: ExposeMin_us
- `185`: ExposeMax_us
- `186`: FvalMode
- `187`: LvalMode
- `188`: DvalMode
- `189`: CC1Usage
- `190`: CC2Usage
- `192`: CC3Usage
- `194`: CC4Usage
- `196`: TwoLineSynchronization
- `198`: TwoLineSynchronizationParity
- `199`: TapConfiguration Camera Link tap configuration

---

#### TapConfiguration

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `4268 << 14` |
| String ID | `TapConfiguration` |
| C/C++ ID | `MC_TapConfiguration` |

**Description:**

This parameter declares the Camera Link tap configuration used by the camera. Refer to "TapConfiguration Glossary" on page 493 for terms definitions and naming conventions.

**Values:**

- **`BASE_1T8`**
  - C/C++: `MC_TapConfiguration_BASE_1T8`
  - The camera requires the Camera Link Base configuration to deliver 1 8-bit pixel every clock cycle.
- **`BASE_1T10`**
  - C/C++: `MC_TapConfiguration_BASE_1T10`
  - The camera requires the Camera Link Base configuration to deliver 1 10-bit pixel every clock cycle.
- **`BASE_1T12`**
  - C/C++: `MC_TapConfiguration_BASE_1T12`
  - The camera requires the Camera Link Base configuration to deliver 1 12-bit pixel every clock cycle.
- **`BASE_1T14`**
  - C/C++: `MC_TapConfiguration_BASE_1T14`
  - The camera requires the Camera Link Base configuration to deliver 1 14-bit pixel every clock cycle.
- **`BASE_1T16`**
  - C/C++: `MC_TapConfiguration_BASE_1T16`
  - The camera requires the Camera Link Base configuration to deliver 1 16-bit pixel every clock cycle.
- **`BASE_1T24`**
  - C/C++: `MC_TapConfiguration_BASE_1T24`
  - The camera requires the Camera Link Base configuration to deliver 3 8-bit color components for 1 24-bit pixel every clock cycle.
- **`BASE_2T8`**
  - C/C++: `MC_TapConfiguration_BASE_2T8`
  - The camera requires the Camera Link Base configuration to deliver 2 8-bit pixels every clock cycle.
- **`BASE_2T10`**
  - C/C++: `MC_TapConfiguration_BASE_2T10`
  - The camera requires the Camera Link Base configuration to deliver 2 10-bit pixels every clock cycle.
- **`BASE_2T12`**
  - C/C++: `MC_TapConfiguration_BASE_2T12`
  - The camera requires the Camera Link Base configuration to deliver 2 12-bit pixels every clock cycle.
- **`BASE_3T8`**
  - C/C++: `MC_TapConfiguration_BASE_3T8`
  - The camera requires the Camera Link Base configuration to deliver 3 8-bit pixels every clock cycle.
- **`MEDIUM_1T30`**
  - C/C++: `MC_TapConfiguration_MEDIUM_1T30`
  - The camera requires the Camera Link Medium configuration to deliver 3 10-bit color components for 1 30-bit pixel every clock cycle.
- **`MEDIUM_1T36`**
  - C/C++: `MC_TapConfiguration_MEDIUM_1T36`
  - The camera requires the Camera Link Medium configuration to deliver 3 12-bit color components for 1 36-bit pixel every clock cycle.
- **`MEDIUM_1T42`**
  - C/C++: `MC_TapConfiguration_MEDIUM_1T42`
  - The camera requires the Camera Link Medium configuration to deliver 3 14-bit color components for 1 42-bit pixel every clock cycle.
- **`MEDIUM_1T48`**
  - C/C++: `MC_TapConfiguration_MEDIUM_1T48`
  - The camera requires the Camera Link Medium configuration to deliver 3 16-bit color components for 1 48-bit pixel every clock cycle.
- **`MEDIUM_2T14`**
  - C/C++: `MC_TapConfiguration_MEDIUM_2T14`
  - The camera requires the Camera Link Medium configuration to deliver 2 14-bit pixels every clock cycle.
- **`MEDIUM_2T16`**
  - C/C++: `MC_TapConfiguration_MEDIUM_2T16`
  - The camera requires the Camera Link Medium configuration to deliver 2 16-bit pixels every clock cycle.
- **`MEDIUM_2T24`**
  - C/C++: `MC_TapConfiguration_MEDIUM_2T24`
  - The camera requires the Camera Link Medium configuration to deliver 6 8-bit color components for 2 24-bit pixels every clock cycle.
- **`MEDIUM_3T10`**
  - C/C++: `MC_TapConfiguration_MEDIUM_3T10`
  - The camera requires the Camera Link Medium configuration to deliver 3 10-bit pixels every clock cycle.
- **`MEDIUM_3T12`**
  - C/C++: `MC_TapConfiguration_MEDIUM_3T12`
  - The camera requires the Camera Link Medium configuration to deliver 3 12-bit pixels every clock cycle.
- **`MEDIUM_3T14`**
  - C/C++: `MC_TapConfiguration_MEDIUM_3T14`
  - The camera requires the Camera Link Medium configuration to deliver 3 14-bit pixels every clock cycle.
- **`MEDIUM_3T16`**
  - C/C++: `MC_TapConfiguration_MEDIUM_3T16`
  - The camera requires the Camera Link Medium configuration to deliver 3 16-bit pixels every clock cycle.
- **`MEDIUM_4T8`**
  - C/C++: `MC_TapConfiguration_MEDIUM_4T8`
  - The camera requires the Camera Link Medium configuration to deliver 4 8-bit pixels every clock cycle.
- **`MEDIUM_4T10`**
  - C/C++: `MC_TapConfiguration_MEDIUM_4T10`
  - The camera requires the Camera Link Medium configuration to deliver 4 10-bit pixels every clock cycle.
- **`MEDIUM_4T12`**
  - C/C++: `MC_TapConfiguration_MEDIUM_4T12`
  - The camera requires the Camera Link Medium configuration to deliver 4 12-bit pixels every clock cycle.
- **`MEDIUM_6T8`**
  - C/C++: `MC_TapConfiguration_MEDIUM_6T8`
  - The camera requires the Camera Link Medium configuration to deliver 6 8-bit pixels every clock cycle.
- **`FULL_8T8`**
  - C/C++: `MC_TapConfiguration_FULL_8T8`
  - The camera requires the Camera Link Full configuration to deliver 8 8-bit pixels every clock cycle.
- **`DECA_2T40`**
  - C/C++: `MC_TapConfiguration_DECA_2T40`
  - The camera requires the Camera Link 80 Bit (8-tap/10-bit) configuration to deliver 10-bit color components for 2 40-bit pixels every clock cycle.
- **`DECA_3T24`**
  - C/C++: `MC_TapConfiguration_DECA_3T24`
  - The camera requires the Camera Link 72 Bit configuration to deliver 9 8-bit color components for 3 24-bit pixels every clock cycle.
- **`DECA_8T10`**
  - C/C++: `MC_TapConfiguration_DECA_8T10`
  - The camera requires the Camera Link 80 Bit (8-tap/10-bit) configuration to deliver 8 10-bit pixels every clock cycle.
- **`DECA_8T30B3`**
  - C/C++: `MC_TapConfiguration_DECA_8T30B3`
  - The camera requires the Camera Link 80 Bit (8-tap/10-bit) configuration to deliver 24 10-bit color components for 8 30-bit pixels every 3 adjacent clock cycles.
- **`DECA_9T8`**
  - C/C++: `MC_TapConfiguration_DECA_9T8`
  - The camera requires the Camera Link 72 Bit configuration to deliver 9 8-bit pixels every clock cycle.
- **`DECA_10T8`**
  - C/C++: `MC_TapConfiguration_DECA_10T8`
  - The camera requires the Camera Link 80 Bit configuration to deliver 10 8-bit pixels every clock cycle.
- **`LITE_1T8`**
  - C/C++: `MC_TapConfiguration_LITE_1T8`
  - The camera requires the Camera Link Lite configuration to deliver 1 8-bit pixel every clock cycle.
- **`LITE_1T10`**
  - C/C++: `MC_TapConfiguration_LITE_1T10`
  - The camera requires the Camera Link Lite configuration to deliver 1 10-bit pixel every clock cycle.

---

#### TapGeometry

*Camera Link tap geometry*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `4273 << 14` |
| String ID | `TapGeometry` |
| C/C++ ID | `MC_TapGeometry` |

**Description:**

This parameter declares the Camera Link tap geometry used by the camera. Based on this parameter together with TapConfiguration , the frame grabber is able to re- arrange the data in the destination surface. Refer to "TapGeometry Glossary" on page 494 for terms definitions and naming conventions.

**Values:**

  - C/C++: `MC_TapGeometry_1X`
  - One region along X-axis, 1 tap per region, line-scan camera.
- `1`: 1X Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W +1
- `1`: H +1 1X_1Y MC_TapGeometry_1X_1Y
  - One region along X-axis, 1 tap per region, area-scan camera. 1X_1Y
- `1`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W +1
- `1`: H +1 1X_1Y2 MC_TapGeometry_1X_1Y2
  - One region along X-axis, 2 vertically adjacent taps per region, line-scan or area-scan camera. 1X_1Y2
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W +1
- `1`: H-1 +2 Tap#2
- `1`: W +1
- `2`: H +2 1X_2YE MC_TapGeometry_1X_2YE
  - One region along X-axis, 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 1X_2YE
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W +1
- `1`: H/2 +1 Tap#2
- `1`: W +1 H H/2 + 1
- `-1`: 1X2 MC_TapGeometry_1X2
  - One region along X-axis, 2 adjacent taps per region, line-scan camera. 1X2
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W-1 +2
- `1`: H +1 Tap#2
- `2`: W +2
- `1`: H +1 1X2_1Y MC_TapGeometry_1X2_1Y
  - One region along X-axis, 2 adjacent taps per region, area-scan camera. 1X2_1Y 1 2 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W-1 +2
- `1`: H +1 Tap#2
- `2`: W +2
- `1`: H +1 1X2_1Y2 MC_TapGeometry_1X2_1Y2
  - One region along X-axis, 2 horizontally adjacent and 2 vertically adjacent taps per region, line- scan or area-scan camera. 1X2_1Y2 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 1 +2
- `1`: H-1 +2 Tap#2
- `2`: W +2
- `1`: H-1 +2 Tap#3
- `1`: W - 1 +2
- `2`: H +2 Tap#4
- `2`: W +2
- `2`: H +2 1X2_2YE MC_TapGeometry_1X2_2YE
  - One region along X-axis, 2 horizontally adjacent and 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 1X2_2YE 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 1 +2
- `1`: H/2 +1 Tap#2
- `2`: W +2
- `1`: H/2 +1 Tap#3
- `1`: W - 1 +2 H H/2 + 1
- `-1`: Tap#4
- `2`: W +2 H H/2 + 1
- `-1`: 1X3 MC_TapGeometry_1X3
  - One region along X-axis, 3 adjacent taps per region, line-scan camera.
- `1`: 
- `2`: 
- `3`: 1X3 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 2 +3
- `1`: H +1 Tap#2
- `2`: W - 1 +3
- `1`: H +1 Tap#3
- `3`: W +3
- `1`: H +1 1X3_1Y MC_TapGeometry_1X3_1Y
  - One region along X-axis, 3 adjacent taps per region, area-scan camera. 1X3_1Y 1 2 3 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 2 +3
- `1`: H +1 Tap#2
- `2`: W - 1 +3
- `1`: H +1 Tap#3
- `3`: W +3
- `1`: H +1 1X3_1Y2 MC_TapGeometry_1X3_1Y2
  - One region along X-axis, 3 horizontally adjacent and 2 vertically adjacent taps per region, line- scan or area-scan camera. 1X3_1Y2 1 2 3 4 5 6 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 2 +3
- `1`: H-1 +2 Tap#2
- `2`: W - 1 +3
- `1`: H-1 +2 Tap#3
- `3`: W +3
- `1`: H-1 +2 Tap#4
- `1`: W - 2 +3
- `2`: H +2 Tap#5
- `2`: W - 1 +3
- `2`: H +2 Tap#6
- `3`: W +3
- `2`: H +2 1X3_2YE MC_TapGeometry_1X3_2YE
  - One region along X-axis, 3 horizontally adjacent and 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 1X3_2YE 1 2 3 4 5 6 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 2 +3
- `1`: H/2 +1 Tap#2
- `2`: W - 1 +3
- `1`: H/2 +1 Tap#3
- `3`: W +3
- `1`: H/2 +1 Tap#4
- `1`: W - 2 +3 H H/2 + 1
- `-1`: Tap#5
- `2`: W - 1 +3 H H/2 + 1
- `-1`: Tap#6
- `3`: W +3 H H/2 + 1
- `-1`: 1X4 MC_TapGeometry_1X4
  - One region along X-axis, 4 adjacent taps per region, line-scan camera.
- `1`: 
- `2`: 
- `3`: 
- `4`: 1X4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 3 +4
- `1`: H +1 Tap#2
- `2`: W - 2 +4
- `1`: H +1 Tap#3
- `3`: W - 1 +4
- `1`: H +1 Tap#4
- `4`: W +4
- `1`: H +1 1X4_1Y MC_TapGeometry_1X4_1Y
  - One region along X-axis, 4 adjacent taps per region, area-scan camera. 1X4_1Y 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 3 +4
- `1`: H +1 Tap#2
- `2`: W - 2 +4
- `1`: H +1 Tap#3
- `3`: W - 1 +4
- `1`: H +1 Tap#4
- `4`: W +4
- `1`: H +1 1X4_1Y2 MC_TapGeometry_1X4_1Y2
  - One region along X-axis, 4 horizontally adjacent and 2 vertically adjacent taps per region, line- scan or area-scan camera. 1X4_1Y2 5 6 7 8 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 3 +4
- `1`: H-1 +2 Tap#2
- `2`: W - 2 +4
- `1`: H-1 +2 Tap#3
- `3`: W - 1 +4
- `1`: H-1 +2 Tap#4
- `4`: W +4
- `1`: H-1 +2 Tap#5
- `1`: W - 3 +4
- `2`: H +2 Tap#6
- `2`: W - 2 +4
- `2`: H +2 Tap#7
- `3`: W - 1 +4
- `2`: H +2 Tap#8
- `4`: W +4
- `2`: H +2 1X4_2YE MC_TapGeometry_1X4_2YE
  - One region along X-axis, 4 horizontally adjacent and 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 1X4_2YE 1 2 3 4 5 6 7 8 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 3 +4
- `1`: H/2 +1 Tap#2
- `2`: W - 2 +4
- `1`: H/2 +1 Tap#3
- `3`: W - 1 +4
- `1`: H/2 +1 Tap#4
- `4`: W +4
- `1`: H/2 +1 Tap#5
- `1`: W - 3 +4 H H/2 + 1
- `-12`: Tap#6
- `2`: W - 2 +4 H H/2 + 1
- `-12`: Tap#7
- `3`: W - 1 +4 H H/2 + 1
- `-12`: Tap#8
- `4`: W +4 H H/2 + 1
- `-12`: 1X8 MC_TapGeometry_1X8
  - One region along X-axis, 8 adjacent taps per region, line-scan camera. 1X8
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 7 +8
- `1`: H +1 Tap#2
- `2`: W - 6 +8
- `1`: H +1 Tap#3
- `3`: W - 5 +8
- `1`: H +1 Tap#4
- `4`: W - 4 +8
- `1`: H +1 Tap#5
- `5`: W - 3 +8
- `1`: H +1 Tap#6
- `6`: W - 2 +8
- `1`: H +1 Tap#7
- `7`: W - 1 +8
- `1`: H +1 Tap#8
- `8`: W +8
- `1`: H +1 1X8_1Y MC_TapGeometry_1X8_1Y
  - One region along X-axis, 8 adjacent taps per region, area-scan camera. 1X8_1Y 1 2 3 4 5 6 7 8 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 7 +8
- `1`: H +1 Tap#2
- `2`: W - 6 +8
- `1`: H +1 Tap#3
- `3`: W - 5 +8
- `1`: H +1 Tap#4
- `4`: W - 4 +8
- `1`: H +1 Tap#5
- `5`: W - 3 +8
- `1`: H +1 Tap#6
- `6`: W - 2 +8
- `1`: H +1 Tap#7
- `7`: W - 1 +8
- `1`: H +1 Tap#8
- `8`: W +8
- `1`: H +1 1X10 MC_TapGeometry_1X10
  - One region along X-axis, 10 adjacent taps per region, line-scan camera. 1X10
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: 
- `9`: 
- `10`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 9 +10
- `1`: H +1 Tap#2
- `2`: W - 8 +10
- `1`: H +1 Tap#3
- `3`: W - 7 +10
- `1`: H +1 Tap#4
- `4`: W - 6 +10
- `1`: H +1 Tap#5
- `5`: W - 5 +10
- `1`: H +1 Tap#6
- `6`: W - 4 +10
- `1`: H +1 Tap#7
- `7`: W - 3 +10
- `1`: H +1 Tap#8
- `8`: W - 2 +10
- `1`: H +1 Tap#9
- `9`: W - 1 +10
- `1`: H +1 Tap#10
- `10`: W +10
- `1`: H +1 1X10_1Y MC_TapGeometry_1X10_1Y
  - One region along X-axis, 10 adjacent taps per region, area-scan camera. 1X10_1Y 1 2 3 4 5 6 7 8 910 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W - 9 +10
- `1`: H +1 Tap#2
- `2`: W - 8 +10
- `1`: H +1 Tap#3
- `3`: W - 7 +10
- `1`: H +1 Tap#4
- `4`: W - 6 +10
- `1`: H +1 Tap#5
- `5`: W - 5 +10
- `1`: H +1 Tap#6
- `6`: W - 4 +10
- `1`: H +1 Tap#7
- `7`: W - 3 +10
- `1`: H +1 Tap#8
- `8`: W - 2 +10
- `1`: H +1 Tap#9
- `9`: W - 1 +10
- `1`: H +1 Tap#10
- `10`: W +10
- `1`: H +1 2X MC_TapGeometry_2X
  - Two regions along X-axis, 1 tap per region, line-scan camera.
- `1`: 
- `2`: 2X Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H +1 Tap#2 W/2 + 1 W +1
- `1`: H +1 2X_1Y MC_TapGeometry_2X_1Y
  - Two regions along X-axis, 1 tap per region, area-scan camera. 2X_1Y
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H +1 Tap#2 W/2 + 1 W +1
- `1`: H +1 2X_1Y2 MC_TapGeometry_2X_1Y2
  - Two regions along X-axis, 2 vertically adjacent taps per region, line-scan or area-scan camera. 2X_1Y2
- `1`: 
- `4`: 
- `3`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H - 1 +2 Tap#2 W/2 + 1 W +1
- `1`: H - 1 +2 Tap#3
- `1`: W/2 +1
- `2`: H +2 Tap#4 W/2 + 1 W +1
- `1`: H +2 2X_2YE MC_TapGeometry_2X_2YE
  - Two regions along X-axis, 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 2X_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H/2 +1 Tap#2 W/2 + 1 W +1
- `1`: H/2 +1 Tap#3
- `1`: W/2 +1 H H/2 + 1
- `-1`: Tap#4 W/2 + 1 W +1 H H/2 + 1
- `-1`: 2XE MC_TapGeometry_2XE
  - Two regions along X-axis, 1 tap per region, start reading from the left/right edges, line-scan camera.
- `1`: 
- `2`: 2XE Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H +1 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H +1 2XE_1Y MC_TapGeometry_2XE_1Y
  - Two regions along X-axis, 1 tap per region, start reading from the left/right edges, area-scan camera. 2XE_1Y
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H +1 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H +1 2XE_1Y2 MC_TapGeometry_2XE_1Y2
  - Two regions along X-axis, 2 vertically adjacent taps per region, start reading from the left/right edges, line-scan or area-scan camera. 2XE_1Y2
- `1`: 
- `4`: 
- `3`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H - 1 +2 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#3
- `1`: W/2 +1
- `2`: H +2 Tap#4 W W/2 + 1
- `-1`: 
- `2`: H +2 2XE_2YE MC_TapGeometry_2XE_2YE
  - Two regions along X-axis, 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 2XE_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 +1
- `1`: H/2 +1 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H/2 +1 Tap#3
- `1`: W/2 +1 H H/2 + 1
- `-1`: Tap#4 W W/2 + 1
- `-1`: H H/2 + 1
- `-1`: 2XM MC_TapGeometry_2XM
  - Two regions along X-axis, 1 tap per region, start reading from the middle, line-scan camera.
- `1`: 
- `2`: 2XM Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 W/2 + 1 W +1
- `1`: H +1 2XM_1Y MC_TapGeometry_2XM_1Y
  - Two regions along X-axis, 1 tap per region, start reading from the middle, area-scan camera. 2XM_1Y
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 W/2 + 1 W +1
- `1`: H +1 2XM_1Y2 MC_TapGeometry_2XM_1Y2
  - Two regions along X-axis, 2 vertically adjacent taps per region, start reading from the middle, line-scan or area-scan camera. 2XM_1Y2
- `1`: 
- `4`: 
- `3`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H - 1 +2 Tap#2 W/2 + 1 W +1
- `1`: H - 1 +2 Tap#3 W/2
- `1`: 
- `-1`: 
- `2`: H +2 Tap#4 W/2 + 1 W +1
- `2`: H +2 2XM_2YE MC_TapGeometry_2XM_2YE
  - Two regions along X-axis, 2 vertical taps per region, start reading from the middle and from the top/bottom edges, area-scan camera. 2XM_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H/2 +1 Tap#2 W/2 + 1 W +1
- `1`: H/2 +1 Tap#3 W/2
- `1`: 
- `-1`: H H/2 + 1
- `-1`: Tap#4 W/2 + 1 W +1 H H/2 + 1
- `-1`: 2XR MC_TapGeometry_2XR
  - Two regions along X-axis, 1 tap per region, start reading from the right, line-scan camera.
- `1`: 
- `2`: 2XR Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H +1 2XR_1Y MC_TapGeometry_2XR_1Y
  - Two regions along X-axis, 1 tap per region, start reading from the right, area-scan camera. 2XR_1Y
- `1`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H +1 2XR_1Y2 MC_TapGeometry_2XR_1Y2
  - Two regions along X-axis, 2 vertically adjacent taps per region, start reading from the right, line-scan or area-scan camera. 2XR_1Y2
- `1`: 
- `4`: 
- `3`: 
- `2`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H - 1 +2 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#3 W/2
- `1`: 
- `-1`: 
- `2`: H +2 Tap#4 W W/2 + 1
- `-1`: 
- `2`: H +2 2XR_2YE MC_TapGeometry_2XR_2YE
  - Two regions along X-axis, 2 vertical taps per region, start reading from the right and from the top/bottom edges, area-scan camera. 2XR_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2
- `1`: 
- `-1`: 
- `1`: H/2 +1 Tap#2 W W/2 + 1
- `-1`: 
- `1`: H/2 +1 Tap#3 W/2
- `1`: 
- `-1`: H H/2 + 1
- `-1`: Tap#4 W W/2 + 1
- `-1`: H H/2 + 1
- `-1`: 2X2 MC_TapGeometry_2X2
  - Two regions along X-axis, 2 adjacent taps per region, line-scan camera.
- `1`: 
- `2`: 
- `3`: 
- `4`: 2X2 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/2 +2
- `1`: H +1 Tap#3 W/2 + 1 W - 1 +2
- `1`: H +1 Tap#4 W/2 + 2 W +2
- `1`: H +1 2X2_1Y2 MC_TapGeometry_2X2_1Y2
  - Two regions along X-axis, 2 horizontally adjacent and 2 vertically adjacent taps per region, line- scan or area-scan camera. 2X2_1Y2 1 2 3 4 5 6 7 8 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H-1 +2 Tap#2
- `2`: W/2 +2
- `1`: H-1 +2 Tap#3 W/2 + 1 W - 1 +2
- `1`: H-1 +2 Tap#4 W/2 + 2 W +2
- `1`: H-1 +2 Tap#5
- `1`: W/2 - 1 +2
- `2`: H +2 Tap#6
- `2`: W/2 +2
- `2`: H +2 Tap#7 W/2 + 1 W - 1 +2
- `2`: H +2 Tap#8 W/2 + 2 W +2
- `2`: H +2 2X2_2YE MC_TapGeometry_2X2_2YE
  - Two regions along X-axis, 2 horizontally adjacent and 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 2X2_2YE 1 2 3 4 5 6 7 8 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H/2 +1 Tap#2
- `2`: W/2 +2
- `1`: H/2 +1 Tap#3 W/2 + 1 W - 1 +2
- `1`: H/2 +1 Tap#4 W/2 + 2 W +2
- `1`: H/2 +1 Tap#5
- `1`: W/2 - 1 +2 H H/2 + 1
- `-1`: Tap#6
- `2`: W/2 +2 H H/2 + 1
- `-1`: Tap#7 W/2 + 1 W - 1 +2 H H/2 + 1
- `-1`: Tap#8 W/2 + 2 W +2 H H/2 + 1
- `-1`: 2X2E MC_TapGeometry_2X2E
  - Two regions along X-axis, 2 adjacent taps per region, start reading from the left/right edges, line-scan camera.
- `1`: 
- `2`: 
- `3`: 
- `4`: 2X2E Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/2 +2
- `1`: H +1 Tap#3 W - 1 W/2 + 1
- `-2`: 
- `1`: H +1 Tap#4 W W/2 + 2
- `-2`: 
- `1`: H +1 2X2E_1Y MC_TapGeometry_2X2E_1Y
  - Two regions along X-axis, 2 adjacent taps per region, start reading from the left/right edges, area-scan camera. 2X2E_1Y 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/2 +2
- `1`: H +1 Tap#3 W - 1 W/2 + 1
- `-2`: 
- `1`: H +1 Tap#4 W W/2 + 2
- `-2`: 
- `1`: H +1 2X2E_1Y2 MC_TapGeometry_2X2E_1Y2
  - Two regions along X-axis, 2 horizontally adjacent and 2 vertically adjacent taps per region, start reading from the left/right edges, line-scan or area-scan camera. 2X2E_1Y2 1 2 3 4 5 6 7 8 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H-1 +2 Tap#2
- `2`: W/2 +2
- `1`: H-1 +2 Tap#3 W - 1 W/2 + 1
- `-2`: 
- `1`: H-1 +2 Tap#4 W W/2 + 2
- `-2`: 
- `1`: H-1 +2 Tap#5
- `1`: W/2 - 1 +2
- `2`: H +2 Tap#6
- `2`: W/2 +2
- `2`: H +2 Tap#7 W - 1 W/2 + 1
- `-2`: 
- `2`: H +2 Tap#8 W W/2 + 2
- `-2`: 
- `2`: H +2 2X2E_2YE MC_TapGeometry_2X2E_2YE
  - Two regions along X-axis, 2 horizontally adjacent and 2 vertical taps per region, start reading from the left/right and top/bottom edges, area-scan camera. 2X2E_2YE 5 6 7 8 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 1 +2
- `1`: H/2 +1 Tap#2
- `2`: W/2 +2
- `1`: H/2 +1 Tap#3 W - 1 W/2 + 1
- `-2`: 
- `1`: H/2 +1 Tap#4 W W/2 + 2
- `-2`: 
- `1`: H/2 +1 Tap#5
- `1`: W/2 - 1 +2 H H/2 + 1
- `-1`: Tap#6
- `2`: W/2 +2 H H/2 + 1
- `-1`: Tap#7 W - 1 W/2 + 1
- `-2`: H H/2 + 1
- `-1`: Tap#8 W W/2 + 2
- `-2`: H H/2 + 1
- `-1`: 2X2M MC_TapGeometry_2X2M
  - Two regions along X-axis, 2 adjacent taps per region, start reading from the middle, line-scan camera.
- `1`: 
- `2`: 
- `3`: 
- `4`: 2X2M Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2 - 1
- `1`: 
- `-2`: 
- `1`: H +1 Tap#2 W/2
- `2`: 
- `-2`: 
- `1`: H +1 Tap#3 W/2 + 1 W - 1 +2
- `1`: H +1 Tap#4 W/2 + 2 W +2
- `1`: H +1 2X2M_1Y MC_TapGeometry_2X2M_1Y
  - Two regions along X-axis, 2 adjacent taps per region, start reading from the middle, area-scan camera. 2X2M_1Y 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2 - 1
- `1`: 
- `-2`: 
- `1`: H +1 Tap#2 W/2
- `2`: 
- `-2`: 
- `1`: H +1 Tap#3 W/2 + 1 W - 1 +2
- `1`: H +1 Tap#4 W/2 + 2 W +2
- `1`: H +1 2X2M_1Y MC_TapGeometry_2X2M_1Y
  - Two regions along X-axis, 2 adjacent taps per region, start reading from the middle, area-scan camera. 2X2M_1Y 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2 - 1
- `1`: 
- `-2`: 
- `1`: H +1 Tap#2 W/2
- `2`: 
- `-2`: 
- `1`: H +1 Tap#3 W/2 + 1 W - 1 +2
- `1`: H +1 Tap#4 W/2 + 2 W +2
- `1`: H +1 2X2M_1Y2 MC_TapGeometry_2X2M_1Y2
  - Two regions along X-axis, 2 horizontally adjacent and 2 vertically adjacent taps per region, start reading from the middle, line-scan or area-scan camera. 2X2M_1Y2 1 2 3 4 5 6 7 8 Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/2 - 1
- `1`: 
- `-2`: 
- `1`: H-1 +2 Tap#2 W/2
- `2`: 
- `-2`: 
- `1`: H-1 +2 Tap#3 W/2 + 1 W - 1 +2
- `1`: H-1 +2 Tap#4 W/2 + 2 W +2
- `1`: H-1 +2 Tap#5 W/2 - 1
- `1`: 
- `-2`: 
- `2`: H +2 Tap#6 W/2
- `2`: 
- `-2`: 
- `2`: H +2 Tap#7 W/2 + 1 W - 1 +2
- `2`: H +2 Tap#8 W/2 + 2 W +2
- `2`: H +2 2X4 MC_TapGeometry_2X4
  - Two regions along X-axis, 4 adjacent taps per region, line-scan camera. 2X4
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 3 +4
- `1`: H +1 Tap#2
- `2`: W/2 - 2 +4
- `1`: H +1 Tap#3
- `3`: W/2 - 1 +4
- `1`: H +1 Tap#4
- `4`: W/2 +4
- `1`: H +1 Tap#5 W/2 + 1 W - 3 +4
- `1`: H +1 Tap#6 W/2 + 2 W - 2 +4
- `1`: H +1 Tap#7 W/2 + 3 W - 1 +4
- `1`: H +1 Tap#8 W/2 + 4 W +4
- `1`: H +1 2X4_1Y MC_TapGeometry_2X4_1Y
  - Two regions along X-axis, 4 adjacent taps per region, area-scan camera. 2X4_1Y 5 6 7 8 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/2 - 3 +4
- `1`: H +1 Tap#2
- `2`: W/2 - 2 +4
- `1`: H +1 Tap#3
- `3`: W/2 - 1 +4
- `1`: H +1 Tap#4
- `4`: W/2 +4
- `1`: H +1 Tap#5 W/2 + 1 W - 3 +4
- `1`: H +1 Tap#6 W/2 + 2 W - 2 +4
- `1`: H +1 Tap#7 W/2 + 3 W - 1 +4
- `1`: H +1 Tap#8 W/2 + 4 W +4
- `1`: H +1 3X MC_TapGeometry_3X
  - Three regions along X-axis, 1 tap per region, line-scan camera. 3X
- `1`: 
- `2`: 
- `3`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/3 +1
- `1`: H +1 Tap#2 W/3 + 1 2W/3 +1
- `1`: H +1 Tap#3 2W/3 + 1 W +1
- `1`: H +1 3X_1Y MC_TapGeometry_3X_1Y
  - Three regions along X-axis, 1 tap per region, area-scan camera. 3X_1Y
- `1`: 
- `2`: 
- `3`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/3 +1
- `1`: H +1 Tap#2 W/3 + 1 2W/3 +1
- `1`: H +1 Tap#3 2W/3 + 1 W +1
- `1`: H +1 3X_1Y2 MC_TapGeometry_3X_1Y2
  - Three regions along X-axis, 2 vertically adjacent taps per region, line-scan or area-scan camera. 3X_1Y2
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/3 +1
- `1`: H-1 +2 Tap#2 W/3 + 1 2W/3 +1
- `1`: H-1 +2 Tap#3 2W/3 + 1 W +1
- `1`: H-1 +2 Tap#4
- `1`: W/3 +1
- `2`: H +2 Tap#5 W/3 + 1 2W/3 +1
- `2`: H +2 Tap#6 2W/3 + 1 W +1
- `2`: H +2 3X_1Y3 MC_TapGeometry_3X_1Y3
  - Three regions along X-axis, 3 vertically adjacent taps per region, line-scan or area-scan camera. 3X_1Y3
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: 
- `9`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/3 +1
- `1`: H-2 +3 Tap#2
- `1`: W/3 +1
- `2`: H-1 +3 Tap#3
- `1`: W/3 +1
- `3`: H +3 Tap#4 W/3 + 1 2W/3 +1
- `1`: H-2 +3 Tap#5 W/3 + 1 2W/3 +1
- `2`: H-1 +3 Tap#6 W/3 + 1 2W/3 +1
- `3`: H +3 Tap#7 2W/3 + 1 W +1
- `1`: H-2 +3 Tap#8 2W/3 + 1 W +1
- `2`: H-1 +3 Tap#9 2W/3 + 1 W +1
- `3`: H +3 4X MC_TapGeometry_4X
  - Four regions along X-axis, 1 tap per region, line-scan camera. 4X
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H +1 Tap#2 W/4 + 1 W/2 +1
- `1`: H +1 Tap#3 W/2 + 1 3W/4 +1
- `1`: H +1 Tap#4 3W/4 + 1 W +1
- `1`: H +1 4X_1Y MC_TapGeometry_4X_1Y
  - Four regions along X-axis, 1 tap per region, area-scan camera. 4X_1Y
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H +1 Tap#2 W/4 + 1 W/2 +1
- `1`: H +1 Tap#3 W/2 + 1 3W/4 +1
- `1`: H +1 Tap#4 3W/4 + 1 W +1
- `1`: H +1 4X_1Y2 MC_TapGeometry_4X_1Y2
  - Four regions along X-axis, 2 vertically adjacent taps per region, line-scan or area-scan camera. 4X_1Y2
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H - 1 +2 Tap#2 W/4 + 1 W/2 +1
- `1`: H - 1 +2 Tap#3 W/2 + 1 3W/4 +1
- `1`: H - 1 +2 Tap#4 3W/4 + 1 W +1
- `1`: H - 1 +2 Tap#5
- `1`: W/4 +1
- `2`: H +2 Tap#6 W/4 + 1 W/2 +1
- `2`: H +2 Tap#7 W/2 + 1 3W/4 +1
- `2`: H +2 Tap#8 3W/4 + 1 W +1
- `2`: H +2 4X_2YE MC_TapGeometry_4X_2YE
  - Four regions along X-axis, 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 4X_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H/2 +1 Tap#2 W/4 + 1 W/2 +1
- `1`: H/2 +1 Tap#3 W/2 + 1 3W/4 +1
- `1`: H/2 +1 Tap#4 3W/4 + 1 W +1
- `1`: H/2 +1 Tap#5
- `1`: W/4 +1 H H/2 + 1
- `-1`: Tap#6 W/4 + 1 W/2 +1 H H/2 + 1
- `-1`: Tap#7 W/2 + 1 3W/4 +1 H H/2 + 1
- `-1`: Tap#8 3W/4 + 1 W +1 H H/2 + 1
- `-1`: 4XE MC_TapGeometry_4XE
  - Four regions along X-axis, 1 tap per region, start reading from the left/right edges, line-scan camera. 4XE
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H +1 Tap#2 W/4 + 1 W/2 +1
- `1`: H +1 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H +1 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H +1 4XE_1Y MC_TapGeometry_4XE_1Y
  - Four regions along X-axis, 1 tap per region, start reading from the left/right edges, area-scan camera. 4XE_1Y
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H +1 Tap#2 W/4 + 1 W/2 +1
- `1`: H +1 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H +1 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H +1 4XE_1Y2 MC_TapGeometry_4XE_1Y2
  - Four regions along X-axis, 2 vertically adjacent taps per region, start reading from the left/right edges, line-scan or area-scan camera. 4XE_1Y2
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H - 1 +2 Tap#2 W/4 + 1 W/2 +1
- `1`: H - 1 +2 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#5
- `1`: W/4 +1
- `2`: H +2 Tap#6 W/4 + 1 W/2 +1
- `2`: H +2 Tap#7 3W/4 W/2 + 1
- `-1`: 
- `2`: H +2 Tap#8 W 3W/4 + 1
- `-1`: 
- `2`: H +2 4XE_2YE MC_TapGeometry_4XE_2YE
  - Four regions along X-axis, 2 vertical taps per region, start reading from the top/bottom edges, area-scan camera. 4XE_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 +1
- `1`: H/2 +1 Tap#2 W/4 + 1 W/2 +1
- `1`: H/2 +1 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H/2 +1 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H/2 +1 Tap#5
- `1`: W/4 +1 H H/2 + 1
- `-1`: Tap#6 W/4 + 1 W/2 +1 H H/2 + 1
- `-1`: Tap#7 3W/4 W/2 + 1
- `-1`: H H/2 + 1
- `-1`: Tap#8 W 3W/4 + 1
- `-1`: H H/2 + 1
- `-1`: 4XR MC_TapGeometry_4XR
  - Four regions along X-axis, 1 tap per region, start reading from the right, line-scan camera. 4XR
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/4
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 W/2 W/4 + 1
- `-1`: 
- `1`: H +1 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H +1 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H +1 4XR_1Y MC_TapGeometry_4XR_1Y
  - Four regions along X-axis, 1 tap per region, start reading from the right, area-scan camera. 4XR_1Y
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/4
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 W/2 W/4 + 1
- `-1`: 
- `1`: H +1 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H +1 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H +1 4XR_1Y2 MC_TapGeometry_4XR_1Y2
  - Four regions along X-axis, 2 vertically adjacent taps per region, start reading from the right, line-scan or area-scan camera. 4XR_1Y2
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/4
- `1`: 
- `-1`: 
- `1`: H - 1 +2 Tap#2 W/2 W/4 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H - 1 +2 Tap#5 W/4
- `1`: 
- `-1`: 
- `2`: H +2 Tap#6 W/2 W/4 + 1
- `-1`: 
- `2`: H +2 Tap#7 3W/4 W/2 + 1
- `-1`: 
- `2`: H +2 Tap#8 W 3W/4 + 1
- `-1`: 
- `2`: H +2 4XR_2YE MC_TapGeometry_4XR_2YE
  - Four regions along X-axis, 2 vertical taps per region, start reading from the right and from the top/bottom edges, area-scan camera. 4XR_2YE
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/4
- `1`: 
- `-1`: 
- `1`: H/2 +1 Tap#2 W/2 W/4 + 1
- `-1`: 
- `1`: H/2 +1 Tap#3 3W/4 W/2 + 1
- `-1`: 
- `1`: H/2 +1 Tap#4 W 3W/4 + 1
- `-1`: 
- `1`: H/2 +1 Tap#5 W/4
- `1`: 
- `-1`: H H/2 + 1
- `-1`: Tap#6 W/2 W/4 + 1
- `-1`: H H/2 + 1
- `-1`: Tap#7 3W/4 W/2 + 1
- `-1`: H H/2 + 1
- `-1`: Tap#8 W 3W/4 + 1
- `-1`: H H/2 + 1
- `-1`: 4X2 MC_TapGeometry_4X2
  - Four regions along X-axis, 2 adjacent taps per region, line-scan camera. 4X2
- `5`: 
- `6`: 
- `7`: 
- `8`: 
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/4 +2
- `1`: H +1 Tap#3 W/4 + 1 W/2 - 1 +2
- `1`: H +1 Tap#4 W/4 + 2 W/2 +2
- `1`: H +1 Tap#5 W/2 + 1 3W/4 - 1 +2
- `1`: H +1 Tap#6 W/2 + 2 3W/4 +2
- `1`: H +1 Tap#7 3W/4 + 1 W - 1 +2
- `1`: H +1 Tap#8 3W/4 + 2 W +2
- `1`: H +1 4X2_1Y MC_TapGeometry_4X2_1Y
  - Four regions along X-axis, 2 adjacent taps per region, area-scan camera. 4X2_1Y 5 6 7 8 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/4 +2
- `1`: H +1 Tap#3 W/4 + 1 W/2 - 1 +2
- `1`: H +1 Tap#4 W/4 + 2 W/2 +2
- `1`: H +1 Tap#5 W/2 + 1 3W/4 - 1 +2
- `1`: H +1 Tap#6 W/2 + 2 3W/4 +2
- `1`: H +1 Tap#7 3W/4 + 1 W - 1 +2
- `1`: H +1 Tap#8 3W/4 + 2 W +2
- `1`: H +1 4X2E MC_TapGeometry_4X2E
  - Four regions along X-axis, 2 adjacent taps per region, start reading from the left/right edges, line-scan camera. 4X2E
- `5`: 
- `6`: 
- `7`: 
- `8`: 
- `1`: 
- `2`: 
- `3`: 
- `4`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/4 +2
- `1`: H +1 Tap#3 W/4 + 1 W/2 - 1 +2
- `1`: H +1 Tap#4 W/4 + 2 W/2 +2
- `1`: H +1 Tap#5 3W/4 - 1 W/2 + 1
- `-2`: 
- `1`: H +1 Tap#6 3W/4 W/2 + 2
- `-2`: 
- `1`: H +1 Tap#7 W - 1 3W/4 + 1
- `-2`: 
- `1`: H +1 Tap#8 W 3W/4 + 2
- `-2`: 
- `1`: H +1 4X2E_1Y MC_TapGeometry_4X2E_1Y
  - Four regions along X-axis, 2 adjacent taps per region, start reading from the left/right edges, area-scan camera. 4X2_1Y 5 6 7 8 1 2 3 4 Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/4 - 1 +2
- `1`: H +1 Tap#2
- `2`: W/4 +2
- `1`: H +1 Tap#3 W/4 + 1 W/2 - 1 +2
- `1`: H +1 Tap#4 W/4 + 2 W/2 +2
- `1`: H +1 Tap#5 3W/4 - 1 W/2 + 1
- `-2`: 
- `1`: H +1 Tap#6 3W/4 W/2 + 2
- `-2`: 
- `1`: H +1 Tap#7 W - 1 3W/4 + 1
- `-2`: 
- `1`: H +1 Tap#8 W 3W/4 + 2
- `-2`: 
- `1`: H +1 8X MC_TapGeometry_8X
  - Eight regions along X-axis, 1 tap per region, line-scan camera. 8X
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/8 +1
- `1`: H +1 Tap#2 W/8 + 1 2W/8 +1
- `1`: H +1 Tap#3 2W/8 + 1 3W/8 +1
- `1`: H +1 Tap#4 3W/8 + 1 4W/8 +1
- `1`: H +1 Tap#5 4W/8 + 1 5W/8 +1
- `1`: H +1 Tap#6 5W/8 + 1 6W/8 +1
- `1`: H +1 Tap#7 6W/8 + 1 7W/8 +1
- `1`: H +1 Tap#8 7W/8 + 1 W +1
- `1`: H +1 8X_1Y MC_TapGeometry_8X_1Y
  - Eight regions along X-axis, 1 tap per region, area-scan camera. 8X-1Y
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/8 +1
- `1`: H +1 Tap#2 W/8 + 1 2W/8 +1
- `1`: H +1 Tap#3 2W/8 + 1 3W/8 +1
- `1`: H +1 Tap#4 3W/8 + 1 4W/8 +1
- `1`: H +1 Tap#5 4W/8 + 1 5W/8 +1
- `1`: H +1 Tap#6 5W/8 + 1 6W/8 +1
- `1`: H +1 Tap#7 6W/8 + 1 7W/8 +1
- `1`: H +1 Tap#8 7W/8 + 1 W +1
- `1`: H +1 8XR MC_TapGeometry_8XR
  - Eight regions along X-axis, 1 tap per region, start reading from the right, line-scan camera. 8XR
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/8
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 2W/8 W/8 + 1
- `-1`: 
- `1`: H +1 Tap#3 3W/8 2W/8 + 1
- `-1`: 
- `1`: H +1 Tap#4 4W/8 3W/8 + 1
- `-1`: 
- `1`: H +1 Tap#5 5W/8 4W/8 + 1
- `-1`: 
- `1`: H +1 Tap#6 6W/8 5W/8 + 1
- `-1`: 
- `1`: H +1 Tap#7 7W/8 6W/8 + 1
- `-1`: 
- `1`: H +1 Tap#8 W 7W/8 + 1
- `-1`: 
- `1`: H +1 8XR_1Y MC_TapGeometry_8XR_1Y
  - Eight regions along X-axis, 1 tap per region, start reading from the right, area-scan camera. 8XR-1Y
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1 W/8
- `1`: 
- `-1`: 
- `1`: H +1 Tap#2 2W/8 W/8 + 1
- `-1`: 
- `1`: H +1 Tap#3 3W/8 2W/8 + 1
- `-1`: 
- `1`: H +1 Tap#4 4W/8 3W/8 + 1
- `-1`: 
- `1`: H +1 Tap#5 5W/8 4W/8 + 1
- `-1`: 
- `1`: H +1 Tap#6 6W/8 5W/8 + 1
- `-1`: 
- `1`: H +1 Tap#7 7W/8 6W/8 + 1
- `-1`: 
- `1`: H +1 Tap#8 W 7W/8 + 1
- `-1`: 
- `1`: H +1 10X MC_TapGeometry_10X
  - Ten regions along X-axis, 1 tap per region, line-scan camera. 10X
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: 
- `9`: 
- `10`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/10 +1
- `1`: H +1 Tap#2 W/10 + 1 2W/10 +1
- `1`: H +1 Tap#3 2W/10 + 1 3W/10 +1
- `1`: H +1 Tap#4 3W/10 + 1 4W/10 +1
- `1`: H +1 Tap#5 4W/10 + 1 5W/10 +1
- `1`: H +1 Tap#6 5W/10 + 1 6W/10 +1
- `1`: H +1 Tap#7 6W/10 + 1 7W/10 +1
- `1`: H +1 Tap#8 7W/10 + 1 8W/10 +1
- `1`: H +1 Tap#9 8W/10 + 1 9W/10 +1
- `1`: H +1 Tap#10 9W/10 + 1 W +1
- `1`: H +1 10X_1Y MC_TapGeometry_10X_1Y
  - Ten regions along X-axis, 1 tap per region, area-scan camera. 10X-1Y
- `1`: 
- `2`: 
- `3`: 
- `4`: 
- `5`: 
- `6`: 
- `7`: 
- `8`: 
- `9`: 
- `10`: Tap# X Start X End Step X Y Start Y End Step Y Tap#1
- `1`: W/10 +1
- `1`: H +1 Tap#2 W/10 + 1 2W/10 +1
- `1`: H +1 Tap#3 2W/10 + 1 3W/10 +1
- `1`: H +1 Tap#4 3W/10 + 1 4W/10 +1
- `1`: H +1 Tap#5 4W/10 + 1 5W/10 +1
- `1`: H +1 Tap#6 5W/10 + 1 6W/10 +1
- `1`: H +1 Tap#7 6W/10 + 1 7W/10 +1
- `1`: H +1 Tap#8 7W/10 + 1 8W/10 +1
- `1`: H +1 Tap#9 8W/10 + 1 9W/10 +1
- `1`: H +1 Tap#10 9W/10 + 1 W +1
- `1`: H +1 ColorMethod Method used at sensor level to build color information

---

#### ColorMethod

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1010 << 14` |
| String ID | `ColorMethod` |
| C/C++ ID | `MC_ColorMethod` |

**Description:**

This parameter returns the way the color information is built. See also "Camera Color Analysis Method" on page 511.

**Values:**

- **`NONE`**
  - C/C++: `MC_ColorMethod_NONE`
  - The camera is monochrome.
- **`RGB`**
  - C/C++: `MC_ColorMethod_RGB`
  - The camera uses a coated sensor and an internal processor to reconstruct the full color information. The color information is available as three R, G, B video data streams.
- **`PRISM`**
  - C/C++: `MC_ColorMethod_PRISM`
  - The camera uses a wavelength-separating prism to feed three distinct imaging sensors. The color information is available as three R, G, B video data streams.
- **`TRILINEAR`**
  - C/C++: `MC_ColorMethod_TRILINEAR`
  - The camera uses three parallel sensing linear arrays of pixels exhibiting different wavelength sensitivities. The color information is available as three R, G, B video data streams.
- **`BAYER`**
  - C/C++: `MC_ColorMethod_BAYER`
  - The camera uses a single imaging sensor coated with a special wavelength-separating patterned filter. The color information is available as a single video data stream embedding the RGB information.

---

#### ColorRegistration

*Alignment of the color pattern filter over the camera window*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1273 << 14` |
| String ID | `ColorRegistration` |
| C/C++ ID | `MC_ColorRegistration` |

**Description:**

When ColorMethod is BAYER, this parameter indicates how the Bayer pattern filter covers the camera active window. Upper left corner of camera window When ColorMethod is TRILINEAR, this parameter states the order the three sensing lines are arranged on the CCD chip. This parameter is otherwise irrelevant. See also "Camera Color Pattern Filter Alignment" on page 511.

**Values:**

- **`GB`**
  - C/C++: `MC_ColorRegistration_GB`
  - The first two pixels are green and blue.
  - *Condition: ColorMethod is set to BAYER*
- **`BG`**
  - C/C++: `MC_ColorRegistration_BG`
  - The first two pixels are blue and green.
  - *Condition: ColorMethod is set to BAYER*
- **`RG`**
  - C/C++: `MC_ColorRegistration_RG`
  - The first two pixels are red and green.
  - *Condition: ColorMethod is set to BAYER*
- **`GR`**
  - C/C++: `MC_ColorRegistration_GR`
  - The first two pixels are green and red.
  - *Condition: ColorMethod is set to BAYER*
- **`RGB`**
  - C/C++: `MC_ColorRegistration_RGB`
  - The three sensing lines are ordered as red, green and blue.
  - *Condition: ColorMethod is set to TRILINEAR*
- **`GBR`**
  - C/C++: `MC_ColorRegistration_GBR`
  - The three sensing lines are ordered as green, blue and red.
  - *Condition: ColorMethod is set to TRILINEAR*
- **`BRG`**
  - C/C++: `MC_ColorRegistration_BRG`
  - The three sensing lines are ordered as blue, red and green.
  - *Condition: ColorMethod is set to TRILINEAR*

---

#### ColorRegistrationControl

*Controls the color registration*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10850 << 14` |
| String ID | `ColorRegistrationControl` |
| C/C++ ID | `MC_ColorRegistrationControl` |

**Description:**

This enumerated parameter controls the method used for ensuring the correct registration of the colors in the image.

**Usage:**

For the particular case of Bayer CFA bilinear line-scan cameras such as the Basler Sprint color, it is necessary to start the acquisition of a new image at a 2-line boundary to ensure a correct color registration of the captured Bayer CFA image. This is achieved by using the FVAL signal to discriminate between the first and the second line of the Bayer CFA sensor.

**Values:**

- **`FVAL`**
  - C/C++: `MC_ColorRegistrationControl_FVAL`
  - Use the FVAL signal as a qualifier for the first line of an image. The first line of an image always corresponds to the first LVAL after FVAL rising.
  - *Condition: All BoardTopology values but MONO_SLOW and DUO_SLOW*
- **`NONE`**
  - C/C++: `MC_ColorRegistrationControl_NONE`
  - Ignore any signal for qualifying the first line of an image (in line-scan acquisition). Default value.

---

#### ExposeOverlap

*Status of the expose to read-out relationship*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1001 << 14` |
| String ID | `ExposeOverlap` |
| C/C++ ID | `MC_ExposeOverlap` |

**Description:**

This parameter indicates whether the expose condition is allowed to overlap the previous read- out condition. This applies to line-scan and area-scan cameras. ExposeOverlap is always allowed for line-scan cameras.

**Values:**

- **`ALLOW`**
  - C/C++: `MC_ExposeOverlap_ALLOW`
  - The expose condition is allowed to overlap the previous read-out condition.

---

#### Expose

*Camera exposure principle*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1011 << 14` |
| String ID | `Expose` |
| C/C++ ID | `MC_Expose` |

**Description:**

This parameter declares the exposure principle of the camera. The camera exposure principle is the way the light exposure function is handled inside the camera. This equally applies to area- scan and line-scan camera.

**Values:**

- **`PLSTRG`**
  - C/C++: `MC_Expose_PLSTRG`
  - The line or frame exposure condition starts upon receiving a pulse from the frame grabber.
- **`WIDTH`**
  - C/C++: `MC_Expose_WIDTH`
  - The duration of a pulse issued by the frame grabber determines the line or frame exposure condition.
- **`INTCTL`**
  - C/C++: `MC_Expose_INTCTL`
  - The line or frame exposure condition is totally controlled by the camera. The exposure duration is set through camera configuration settings.
- **`INTPRM`**
  - C/C++: `MC_Expose_INTPRM`
  - The exposure is permanent.

---

#### Readout

*Camera read-out principle*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1012 << 14` |
| String ID | `Readout` |
| C/C++ ID | `MC_Readout` |

**Description:**

This parameter declares the read-out principle of the camera. The camera read-out principle is the way the read-out function is handled inside the camera.

**Values:**

- **`PLSTRG`**
  - C/C++: `MC_Readout_PLSTRG`
  - With line-scan cameras, the line read-out condition starts upon receiving a pulse from the frame grabber.
- **`INTCTL`**
  - C/C++: `MC_Readout_INTCTL`
  - With line-scan cameras, the read-out duration is set through camera configuration settings. With area-scan cameras, the line read-out condition is totally controlled by the camera.

---

#### ResetCtl

*Electrical style of main reset control line to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1027 << 14` |
| String ID | `ResetCtl` |
| C/C++ ID | `MC_ResetCtl` |

**Description:**

This parameter, along with ResetEdge, declares the attributes of the main reset signal applied to the camera feeding the channel. In case of area-scan cameras, the main reset signal implements the asynchronous frame reset function, which usually triggers the frame exposure condition inside the camera. In case of line-scan cameras, the main reset signal implements the line reset function, which usually triggers the line exposure condition inside the camera. Some cameras use an additional reset control line to independently control the expose and read-out functions. Refer to AuxResetCtl .

**Values:**

- **`NONE`**
  - C/C++: `MC_ResetCtl_NONE`
  - The camera has no reset control line.
- **`DIFF`**
  - C/C++: `MC_ResetCtl_DIFF`
  - The camera reset control line requires a signal at RS-422 or LVDS differential levels.

---

#### ResetEdge

*Significant edge of main reset control line to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1028 << 14` |
| String ID | `ResetEdge` |
| C/C++ ID | `MC_ResetEdge` |

**Description:**

This parameter, along with ResetCtl , declares the attributes of the main reset signal applied to the camera feeding the channel. In case of area-scan cameras, the main reset signal implements the asynchronous frame reset function, which usually triggers the frame exposure condition inside the camera. In case of line-scan cameras, the main reset signal implements the line reset function, which usually triggers the line exposure condition inside the camera. Some cameras use an additional reset control line to independently control the expose and read-out functions. Refer to the AuxResetEdge parameter. The parameter indicates the logic polarity delivered through the main reset line the camera obeys to.

**Values:**

- **`GOHIGH`**
  - C/C++: `MC_ResetEdge_GOHIGH`
  - The camera reacts to a positive going pulse over the main reset control line.
- **`GOLOW`**
  - C/C++: `MC_ResetEdge_GOLOW`
  - The camera reacts to a negative going pulse over the main reset control line.

---

#### AuxResetCtl

*Electrical style of auxiliary reset control line to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1029 << 14` |
| String ID | `AuxResetCtl` |
| C/C++ ID | `MC_AuxResetCtl` |

**Description:**

This parameter, along with AuxResetEdge , declares the attributes of the auxiliary reset signal applied to the camera feeding the channel. Some cameras (area-scan or line-scan) use two reset control lines to independently control the expose and read-out functions. Refer to the ResetCtl parameter.

**Values:**

- **`NONE`**
  - C/C++: `MC_AuxResetCtl_NONE`
  - The camera has no auxiliary reset control line.
- **`DIFF`**
  - C/C++: `MC_AuxResetCtl_DIFF`
  - The camera auxiliary reset control line requires a signal at RS-422 or LVDS differential levels.

---

#### AuxResetEdge

*Significant edge of auxiliary reset control line to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1030 << 14` |
| String ID | `AuxResetEdge` |
| C/C++ ID | `MC_AuxResetEdge` |

**Description:**

This parameter, along with AuxResetCtl , declares the attributes of the auxiliary reset signal applied to the camera feeding the channel. Some cameras (area-scan or line-scan) use two reset control lines to independently control the expose and read-out functions. Refer to the ResetCtl parameter. The parameter indicates the logic polarity delivered through the auxiliary reset line the camera obeys to.

**Values:**

- **`GOHIGH`**
  - C/C++: `MC_AuxResetEdge_GOHIGH`
  - The camera reacts to a positive going pulse over the auxiliary reset control line.
- **`GOLOW`**
  - C/C++: `MC_AuxResetEdge_GOLOW`
  - The camera reacts to a negative going pulse over the auxiliary reset control line.

---

#### ResetDur

*Required duration of pulse sent through reset control line to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1031 << 14` |
| String ID | `ResetDur` |
| C/C++ ID | `MC_ResetDur` |

**Description:**

This parameter declares the minimum pulse width to be applied to the reset control line for the camera to assume the proper reaction. It characterizes both the main (RESET) and auxiliary reset (AUXRESET) control lines when it exists. With area-scan cameras, ResetDur is expressed as a number of video lines. With line-scan cameras, ResetDur is expressed in nanoseconds. ExposeMin_us Base DualBase Full FullXR Minimum duration of grabber-controlled exposure allowed by camera, expressed in microseconds

---

#### ExposeMin_us

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1033 << 14` |
| String ID | `ExposeMin_us` |
| C/C++ ID | `MC_ExposeMin_us` |

**Description:**

This parameter applies to area-scan camera operated in CTL mode, stating the minimum tolerated duration of the frame exposure duration as specified by the camera manufacturer. It also applies to most the line-scan cameras, stating the minimum tolerated duration of the line exposure duration as specified by the camera manufacturer.

**Values:**

- `1`: 1 microsecond Minimum range value.
- `5000000`: 5,000,000 microseconds (=5 seconds) Maximum range value. ExposeMax_us Maximum duration of grabber-controlled exposure allowed by camera, expressed in microseconds

---

#### ExposeMax_us

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1034 << 14` |
| String ID | `ExposeMax_us` |
| C/C++ ID | `MC_ExposeMax_us` |

**Description:**

This parameter applies to area-scan camera operated in CTL mode, stating the maximum tolerated duration of the frame exposure duration as specified by the camera manufacturer. It also applies to most the line-scan cameras, stating the maximum tolerated duration of the line exposure duration as specified by the camera manufacturer.

**Values:**

- `1`: 1 microsecond Minimum range value.
- `20000000`: 20,000,000 microseconds (=20 seconds) Maximum range value. FvalMode Usage of downstream signal FVAL

---

#### FvalMode

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1035 << 14` |
| String ID | `FvalMode` |
| C/C++ ID | `MC_FvalMode` |

**Description:**

The Camera Link standard specifies a downstream signal aimed at signaling the validity of a video frame issued by the camera. This signal is called FVAL. This parameter expresses the timing rules associated to FVAL.

**Values:**

- **`FN`**
  - C/C++: `MC_FvalMode_FN`
  - Frame None.
- **`FA`**
  - C/C++: `MC_FvalMode_FA`
  - Frame Ante.
- **`FC`**
  - C/C++: `MC_FvalMode_FC`
  - Frame Cover.

---

#### LvalMode

*Usage of downstream signal LVAL*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1036 << 14` |
| String ID | `LvalMode` |
| C/C++ ID | `MC_LvalMode` |

**Description:**

The Camera Link standard specifies a downstream signal aimed at signaling the validity of a video line issued by the camera. This signal is called LVAL. This parameter expresses the timing rules associated to LVAL.

**Values:**

- **`LA`**
  - C/C++: `MC_LvalMode_LA`
  - Line Ante.
- **`LN`**
  - C/C++: `MC_LvalMode_LN`
  - Line None.

---

#### DvalMode

*Usage of downstream signal DVAL*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1037 << 14` |
| String ID | `DvalMode` |
| C/C++ ID | `MC_DvalMode` |

**Description:**

This parameter expresses the timing rules associated to DVAL.

**Values:**

- **`DN`**
  - C/C++: `MC_DvalMode_DN`
  - Data None.
- **`DG`**
  - C/C++: `MC_DvalMode_DG`
  - Data Gate.

---

#### CC1Usage

*Usage of upstream signal CC1*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1216 << 14` |
| String ID | `CC1Usage` |
| C/C++ ID | `MC_CC1Usage` |

**Values:**

- **`LOW`**
  - C/C++: `MC_CC1Usage_LOW`
  - The control line is tied to the low logic state.
- **`HIGH`**
  - C/C++: `MC_CC1Usage_HIGH`
  - The control line is tied to the high logic state.
- **`RESET`**
  - C/C++: `MC_CC1Usage_RESET`
  - The control line implements the reset function.
- **`AUXRESET`**
  - C/C++: `MC_CC1Usage_AUXRESET`
  - The control line implements the auxiliary reset function.
- **`SOFT`**
  - C/C++: `MC_CC1Usage_SOFT`
  - The control line is controlled through the I/O API.
- **`DIN1`**
  - C/C++: `MC_CC1Usage_DIN1`
  - The control line is tied to the DIN1 input port.
- **`IIN1`**
  - C/C++: `MC_CC1Usage_IIN1`
  - The control line is tied to the IIN1 input port.

---

#### CC2Usage

*Usage of upstream signal CC2*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1218 << 14` |
| String ID | `CC2Usage` |
| C/C++ ID | `MC_CC2Usage` |

**Values:**

- **`LOW`**
  - C/C++: `MC_CC2Usage_LOW`
  - The control line is tied to the low logic state.
- **`HIGH`**
  - C/C++: `MC_CC2Usage_HIGH`
  - The control line is tied to the high logic state.
- **`RESET`**
  - C/C++: `MC_CC2Usage_RESET`
  - The control line implements the reset function.
- **`AUXRESET`**
  - C/C++: `MC_CC2Usage_AUXRESET`
  - The control line implements the auxiliary reset function.
- **`SOFT`**
  - C/C++: `MC_CC2Usage_SOFT`
  - The control line is controlled through the I/O API.
- **`DIN2`**
  - C/C++: `MC_CC2Usage_DIN2`
  - The control line is tied to the DIN2 input port.

---

#### CC3Usage

*Usage of upstream signal CC3*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1219 << 14` |
| String ID | `CC3Usage` |
| C/C++ ID | `MC_CC3Usage` |

**Values:**

- **`LOW`**
  - C/C++: `MC_CC3Usage_LOW`
  - The control line is tied to the low logic state.
- **`HIGH`**
  - C/C++: `MC_CC3Usage_HIGH`
  - The control line is tied to the high logic state.
- **`RESET`**
  - C/C++: `MC_CC3Usage_RESET`
  - The control line implements the reset function.
- **`AUXRESET`**
  - C/C++: `MC_CC3Usage_AUXRESET`
  - The control line implements the auxiliary reset function.
- **`SOFT`**
  - C/C++: `MC_CC3Usage_SOFT`
  - The control line is controlled through the I/O API.
- **`IIN1`**
  - C/C++: `MC_CC3Usage_IIN1`
  - The control line is tied to the IIN1 input port.

---

#### CC4Usage

*Usage of upstream signal CC4*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1220 << 14` |
| String ID | `CC4Usage` |
| C/C++ ID | `MC_CC4Usage` |

**Values:**

- **`LOW`**
  - C/C++: `MC_CC4Usage_LOW`
  - The control line is tied to the low logic state.
- **`HIGH`**
  - C/C++: `MC_CC4Usage_HIGH`
  - The control line is tied to the high logic state.
- **`RESET`**
  - C/C++: `MC_CC4Usage_RESET`
  - The control line implements the reset function.
- **`AUXRESET`**
  - C/C++: `MC_CC4Usage_AUXRESET`
  - The control line implements the auxiliary reset function.
- **`SOFT`**
  - C/C++: `MC_CC4Usage_SOFT`
  - The control line is controlled through the I/O API.

---

#### TwoLineSynchronization

*Controls the two-line synchronization mode*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11049 << 14` |
| String ID | `TwoLineSynchronization` |
| C/C++ ID | `MC_TwoLineSynchronization` |

**Description:**

This enumerated parameter controls the 2-line mode of the synchronized line-scan acquisition.

**Usage:**

Relevance condition(s): Condition: Basler Sprint Bilinear line-scan camera (or similar product). Directive: Set this parameter to ENABLE to allow synchronized line-scan acquisition with bilinear linescan cameras such as the Basler Sprint.

**Values:**

- **`ENABLE`**
  - C/C++: `MC_TwoLineSynchronization_ENABLE`
  - The 2-line synchronization mode is enabled.
- **`DISABLE`**
  - C/C++: `MC_TwoLineSynchronization_DISABLE`
  - The 2-line synchronization mode is disabled. Default value.

---

#### TwoLineSynchronizationParity

*Controls the two-line synchronization parity*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Camera Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11050 << 14` |
| String ID | `TwoLineSynchronizationParity` |
| C/C++ ID | `MC_TwoLineSynchronizationParity` |

**Description:**

This enumerated parameter selects the line parity of individual cameras in a 2-line synchronized acquisition system.

**Usage:**

Relevance condition(s): Condition: TwoLineSynchronization = ENABLE

**Values:**

- **`ODD`**
  - C/C++: `MC_TwoLineSynchronizationParity_ODD`
  - The camera cycle begins at an odd line trigger count boundary.
- **`EVEN`**
  - C/C++: `MC_TwoLineSynchronizationParity_EVEN`
  - The camera cycle begins at an even line trigger count boundary. Default value.

---

### 4.4. Cable Features Category

*Parameters setting the hardware attributes of the cable linking the camera to the frame grabber*

#### ResetLine

*Designation of line chosen for transporting main reset to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cable Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1158 << 14` |
| String ID | `ResetLine` |
| C/C++ ID | `MC_ResetLine` |

**Description:**

This parameter declares which line is used inside the camera cable to connect the main reset signal from the frame grabber to the camera. Some cameras use an additional reset control line to independently control the expose and read-out functions. Refer to the AuxResetLine parameter.

**Values:**

- **`CC1`**
  - C/C++: `MC_ResetLine_CC1`
  - The main reset uses the CC1 camera control line.
- **`CC2`**
  - C/C++: `MC_ResetLine_CC2`
  - The main reset uses the CC2 camera control line.
- **`CC3`**
  - C/C++: `MC_ResetLine_CC3`
  - The main reset uses the CC3 camera control line.
- **`CC4`**
  - C/C++: `MC_ResetLine_CC4`
  - The main reset uses the CC4 camera control line.
- **`NC`**
  - C/C++: `MC_ResetLine_NC`
  - The main reset is not used and not connected.

---

#### AuxResetLine

*Designation of line chosen for transporting auxiliary reset to camera*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cable Features |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1159 << 14` |
| String ID | `AuxResetLine` |
| C/C++ ID | `MC_AuxResetLine` |

**Description:**

This parameter declares which line is used inside the camera cable to connect the auxiliary reset signal from the frame grabber to the camera. Some cameras (area-scan or line-scan) use two reset control lines to independently control the expose and read-out functions. Refer to the ResetCtl parameter.

**Values:**

- **`NC`**
  - C/C++: `MC_AuxResetLine_NC`
  - No auxiliary reset line to connect.
- **`CC1`**
  - C/C++: `MC_AuxResetLine_CC1`
  - The auxiliary reset uses the CC1 camera control line.
- **`CC2`**
  - C/C++: `MC_AuxResetLine_CC2`
  - The auxiliary reset uses the CC2 camera control line.
- **`CC3`**
  - C/C++: `MC_AuxResetLine_CC3`
  - The auxiliary reset uses the CC3 camera control line.
- **`CC4`**
  - C/C++: `MC_AuxResetLine_CC4`
  - The auxiliary reset uses the CC4 camera control line.

---

### 4.5. Acquisition Control Category

*Parameters installing the acquisition modes of the channel*

#### AcquisitionMode

*Fundamental acquisition mode*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `3396 << 14` |
| String ID | `AcquisitionMode` |
| C/C++ ID | `MC_AcquisitionMode` |

**Description:**

Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- **`WEB`**
  - C/C++: `MC_AcquisitionMode_WEB`
  - This mode is intended for image acquisition of a continuous object, like a web, from a line-scan camera. A single sequence acquiring SeqLength_Ln contiguous lines is available within the channel activity period. The sequence is divided in contiguous phases, each phase acquiring PageLength_Ln lines. In the case SeqLength_Ln is not a multiple of PageLength_Ln , the surface is partially filled during the last phase. The sequence and the first acquisition phase are initiated according to TrigMode . Subsequent acquisition phases are automatically initiated without any line loss. BreakEffect specifies the behavior in case of a user break. Default value.
- **`PAGE`**
  - C/C++: `MC_AcquisitionMode_PAGE`
  - This mode is intended for image acquisition of discrete objects from a line-scan camera. Each page is constituted of contiguous lines; the page length, expressed in lines, is specified by PageLength_Ln . A single sequence is capable to acquire SeqLength_Pg pages within the channel activity period.
- **`LONGPAGE`**
  - C/C++: `MC_AcquisitionMode_LONGPAGE`
  - This mode is intended for image acquisition of long or variable size discrete objects from a line- scan camera. The parameter ActivityLength specifies the number of sequences within the channel activity period. Each sequence is capable to acquire SeqLength_Lncontiguous lines. A sequence is divided in phases, each phase acquiring PageLength_Ln lines.
- **`HFR`**
  - C/C++: `MC_AcquisitionMode_HFR`
  - This mode is intended for acquisition of snapshot images from high frame rate area-scan cameras. A single sequence is capable to acquire SeqLength_Fr frames within the channel activity period. The sequence is divided into phases, each phase acquiring PhaseLength_Fr frames into a single destination surface.
- **`SNAPSHOT`**
  - C/C++: `MC_AcquisitionMode_SNAPSHOT`
  - This mode is intended for acquisition of snapshot images from area-scan cameras. The unique sequence is capable to acquire SeqLength_Fr frames within the channel activity period.

---

#### SynchronizedAcquisition

*Inter-Channel synchronization mode*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10571 << 14` |
| String ID | `SynchronizedAcquisition` |
| C/C++ ID | `MC_SynchronizedAcquisition` |

**Description:**

Main control of the inter-channel synchronization though the SyncBus.

**Usage:**

Directive: Set to MASTER for one SyncBus contributor and set to SLAVE for all other contributors. Directive: Alternatively, when all contributors belong to the same card, set to LOCAL_MASTER for one SyncBus contributor and set to SLAVE for all other contributors. Directive: Set to LOCAL_SLAVE for only one SyncBus contributor when all contributors belong to the same card.

**Values:**

- **`OFF`**
  - C/C++: `MC_SynchronizedAcquisition_OFF`
  - The inter-channel synchronized acquisition feature is disabled. The MultiCam channel is operating independently from other MultiCam channels. Default value.
- **`MASTER`**
  - C/C++: `MC_SynchronizedAcquisition_MASTER`
  - The MultiCam channel is configured as the SyncBus master agent. Two synchronization signals are delivered on the IOUT3 and IOUT4 output ports of the channel for distribution to all the SyncBus agents using the appropriate wiring. The acquisition controller gets synchronization signals from the SyncBus through the IIN3 and IIN4 input ports of the channel.
- **`SLAVE`**
  - C/C++: `MC_SynchronizedAcquisition_SLAVE`
  - The MultiCam channel is configured as a SyncBus slave agent. The acquisition controller gets synchronization signals from the SyncBus through the IIN3 and IIN4 input ports of the channel.
- **`LOCAL_MASTER`**
  - C/C++: `MC_SynchronizedAcquisition_LOCAL_MASTER`
  - The MultiCam channel is configured as the local SyncBus master agent. Two synchronization signals are delivered on a local SyncBus for distribution to all the local SyncBus agents using an internal wiring. The acquisition controller gets synchronization signals from the local SyncBus.
- **`LOCAL_SLAVE`**
  - C/C++: `MC_SynchronizedAcquisition_LOCAL_SLAVE`
  - The MultiCam channel is configured as a local SyncBus master agent. The acquisition controller gets synchronization signals from the local SyncBus.

---

#### SynchronizedAcquisitionBus

*SyncBus interface selector*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11217 << 14` |
| String ID | `SynchronizedAcquisitionBus` |
| C/C++ ID | `MC_SynchronizedAcquisitionBus` |

**Description:**

Selects the hardware interface used by the SyncBus.

**Usage:**

Directive: Set to C2C when the line rate exceeds 40 kHz.

**Values:**

- **`ISO`**
  - C/C++: `MC_SynchronizedAcquisitionBus_ISO`
  - The SyncBus uses the IIN3/IIN4 isolated input lines and the IOUT3/IOUT4 isolated output lines. Default value.
- **`C2C`**
  - C/C++: `MC_SynchronizedAcquisitionBus_C2C`
  - The SyncBus uses the C2C SyncBus connector.

---

#### SynchronizedPageTrigger

*Page trigger synchronization control*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11221 << 14` |
| String ID | `SynchronizedPageTrigger` |
| C/C++ ID | `MC_SynchronizedPageTrigger` |

**Description:**

Selects the signal on which page triggers are synchronized before being broadcasted on the SyncBus.

**Usage:**

Relevance condition(s): Condition: BoardTopology = MONO_DECA Condition: Synchronized acquisition using SyncBus Condition: Line-scan camera Directive: The LINETRIGGER default value is only applicable to line-scan cameras controlled by the frame grabber (RC, RG or RP camera control methods). Directive: Setting to LVALRISE allows to share page triggers using the SyncBus when the camera is not controlled by the frame grabber (SC cameral control method).

**Values:**

- **`LINETRIGGER`**
  - C/C++: `MC_SynchronizedPageTrigger_LINETRIGGER`
  - Page triggers are synchronized with the next line trigger event. Default value.
- **`LVALRISE`**
  - C/C++: `MC_SynchronizedPageTrigger_LVALRISE`
  - Page triggers are synchronized with the next start of line event (LVAL rising edge).

---

#### PageCaptureMode

*Start-of-page capture control*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11222 << 14` |
| String ID | `PageCaptureMode` |
| C/C++ ID | `MC_PageCaptureMode` |

**Description:**

This parameter controls the conditions applied by the frame grabber to capture the first data line after a page trigger.

**Usage:**

Relevance condition(s): Condition: BoardTopology = MONO_DECA Condition: Line-scan camera

**Values:**

- **`FIRST_LINE`**
  - C/C++: `MC_PageCaptureMode_FIRST_LINE`
  - The first captured line is the first entire data line sent by the camera after the page trigger event. Default value.
- **`FIRST_EXPOSURE`**
  - C/C++: `MC_PageCaptureMode_FIRST_EXPOSURE`
  - The first captured line is the data line resulting from the first entire exposure cycle after the page trigger event.

---

#### TrigMode

*Grabber acquisition sequence triggering mode*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `512 << 14` |
| String ID | `TrigMode` |
| C/C++ ID | `MC_TrigMode` |

**Description:**

The TrigMode parameter establishes the starting conditions of an acquisition sequence. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- **`IMMEDIATE`**
  - C/C++: `MC_TrigMode_IMMEDIATE`
  - The acquisition sequence starts immediately without waiting for a trigger.
- **`HARD`**
  - C/C++: `MC_TrigMode_HARD`
  - The start of the acquisition sequence is delayed until the hardware trigger line senses a valid transition. Parameters TrigLine or TrigLineIndex specify the location of a hardware trigger input line. Parameters TrigCtl , TrigEdge and TrigFilter specify the configuration of the hardware trigger input line. A programmable delay can be inserted with parameter PageDelay_Ln.
- **`SOFT`**
  - C/C++: `MC_TrigMode_SOFT`
  - The start of the acquisition sequence is delayed until the software sets parameter ForceTrig to TRIG.
- **`COMBINED`**
  - C/C++: `MC_TrigMode_COMBINED`
  - The start of the acquisition sequence is delayed until detection of hardware or software trigger.
- **`SLAVE`**
  - C/C++: `MC_TrigMode_SLAVE`
  - The start of the acquisition sequence is originated from the master device.
  - *Condition: SynchronizedAcquisition is set to SLAVE or LOCAL_SLAVE.*

---

#### NextTrigMode

*Grabber subsequent acquisition phases or slices triggering mode*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `663 << 14` |
| String ID | `NextTrigMode` |
| C/C++ ID | `MC_NextTrigMode` |

**Description:**

This parameter establishes the starting conditions of the subsequent acquisition phases or slices. Refer to the "MultiCam Acquisition Principles" on page 492 application note. On Domino boards, the default value is SAME. On Grablink boards, the default value depends on the selected AcquisitionMode :
- 
When WEB or LONGPAGE, the default value is REPEAT.
- 
When SNAPSHOT, HFR or PAGE, the default value is SAME.

**Values:**

- **`COMBINED`**
  - C/C++: `MC_NextTrigMode_COMBINED`
  - Any subsequent acquisition phase or slice is delayed until detection of hardware or software trigger.
- **`HARD`**
  - C/C++: `MC_NextTrigMode_HARD`
  - Any subsequent acquisition phase or slice is delayed until the hardware trigger line senses a valid transition. Parameters TrigLine or TrigLineIndex specifies the location of a hardware trigger input line. Parameters TrigCtl , TrigEdge and TrigFilter specify the configuration of the hardware trigger input line. A programmable delay can be inserted with parameter PageDelay_Ln.
- **`REPEAT`**
  - C/C++: `MC_NextTrigMode_REPEAT`
  - Any subsequent acquisition phase or slice occurs immediately after the preceding one.
- **`SAME`**
  - C/C++: `MC_NextTrigMode_SAME`
  - Any subsequent acquisition phase or slice occurs similarly to the conditions defined by TrigMode.
- **`SOFT`**
  - C/C++: `MC_NextTrigMode_SOFT`
  - Any subsequent acquisition phase or slice is delayed until the software sets parameter ForceTrig to TRIG.
- **`SLAVE`**
  - C/C++: `MC_NextTrigMode_SLAVE`
  - Any subsequent acquisition phase or slice is delayed until a trigger is originated from the master device.
  - *Condition: SynchronizedAcquisition is set to SLAVE or LOCAL_SLAVE.*

---

#### TrigRepeatCount

*Trigger repetition control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `10666 << 14` |
| String ID | `TrigRepeatCount` |
| C/C++ ID | `MC_TrigRepeatCount` |

**Description:**

This parameter controls the trigger repetition, a feature providing the capability to insert additional acquisition phases after each triggered acquisition phase. A value of 0 disables the trigger repetition feature. A positive value enables the trigger repetition feature and specifies the number of additional acquisition phases inserted after every triggered phase.

**Usage:**

Prerequisite action(s): Condition: AcquisitionMode = SNAPSHOT, HFR or PAGE. Condition: The feature applies on triggered acquisition phases only. Refer to "TrigMode" on page 214 and "NextTrigMode" on page 216. Directive: Trigger overlap is allowed during the last repeated acquisition phase but not before!

**Values:**

- `0`: Minimum range value. Default value.
- `1024`: Maximum range value. EndTrigMode Grabber end triggering mode

---

#### EndTrigMode

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `2916 << 14` |
| String ID | `EndTrigMode` |
| C/C++ ID | `MC_EndTrigMode` |

**Description:**

The EndTrigMode parameter establishes the conditions of a sequence termination. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- **`AUTO`**
  - C/C++: `MC_EndTrigMode_AUTO`
  - The acquisition sequence terminates automatically upon expiration of a frame, page or line counter. See Automatic completion conditions vs. AcquisitionMode below.
- **`HARD`**
  - C/C++: `MC_EndTrigMode_HARD`
  - The acquisition sequence terminates upon the detection of a valid transition of the hardware end-trigger line. Parameters EndTrigCtl , EndTrigEdge , EndTrigFilter and EndTrigLine specify the location and the configuration of the hardware end-trigger input line. A programmable delay can be inserted with parameter EndPageDelay_Ln .
  - *Condition: AcquisitionMode is set to LONGPAGE.*
  - *Condition: AcquisitionMode is set to LONGPAGE.*
- **`SLAVE`**
  - C/C++: `MC_EndTrigMode_SLAVE`
  - The end of the acquisition sequence is originated from the master device.
  - *Condition: AcquisitionMode is set to LONGPAGE andSynchronizedAcquisition is set to SLAVE or*
- **`LOCAL_SLAVE.`**

---

#### BreakEffect

*Grabber break effect on the acquisition phase*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `2011 << 14` |
| String ID | `BreakEffect` |
| C/C++ ID | `MC_BreakEffect` |

**Description:**

The BreakEffect parameter establishes the effect of a user break on the channel. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- **`FINISH`**
  - C/C++: `MC_BreakEffect_FINISH`
  - The effect of the user break is postponed until the acquisition sequence reaches a specific boundary. The effect is immediate only when no acquisition has been triggered.
  - *Condition: EndTrigMode is set to AUTO*
  - *Condition: AcquisitionMode is set to VIDEO, SNAPSHOT, or HFR: the channel activity and the*
- **`ABORT`**
  - C/C++: `MC_BreakEffect_ABORT`
  - The effect of the user break is immediate. The current acquisition is incomplete. The portion of image already acquired is available. This value is only available for line-scan acquisition modes, not for HFR.

---

#### ActivityLength

*Acquisition sequences count*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3406 << 14` |
| String ID | `ActivityLength` |
| C/C++ ID | `MC_ActivityLength` |

**Description:**

An activity period of a channel is made of one or several acquisition sequences. This parameter establishes the number of acquisition sequences constituting a channel activity period. MultiCam sets this parameter to 1 when AcquisitionMode is SNAPSHOT, WEB, PAGE or HFR. Setting ActivityLength to MC_INDETERMINATE results in indefinitely repeated acquisition sequences. A user break is required to stop the channel activity. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- `1`: 1 acquisition sequence Condition: AcquisitionMode = SNAPSHOT or HFR or WEB or PAGE MC_INDETERMINATE Undefined number of acquisition sequences Condition: AcquisitionMode = LONGPAGE PageLength_Ln Length of page acquisition, expressed as a number of lines

---

#### PageLength_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1039 << 14` |
| String ID | `PageLength_Ln` |
| C/C++ ID | `MC_PageLength_Ln` |

**Description:**

This parameter is applicable to line-scan acquisition, and declares the number of scanned lines stored into a surface. The user is invited to set this parameter when AcquisitionMode = PAGE, WEB or LONGPAGE. The user is invited to read back the parameter since MultiCam may trim its value to fulfill specific grabber requirements. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- `1`: 1 line Minimum range value.
- `65535`: 65,535 lines Maximum range value. SeqLength_Fr Number of frames in a sequence

---

#### SeqLength_Fr

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3407 << 14` |
| String ID | `SeqLength_Fr` |
| C/C++ ID | `MC_SeqLength_Fr` |

**Description:**

This parameter establishes the number of frames constituting a sequence. The user is invited to set this parameter when EndTrigMode is AUTO and AcquisitionMode has one of the following values: VIDEO, SNAPSHOT or HFR. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- `1`: 1 frame Condition: AcquisitionMode is set to SNAPSHOT or HFR Minimum range value.
- `65534`: 65,534 frames Condition: AcquisitionMode is set to SNAPSHOT Maximum range value.
- `16711170`: 16,711,170 frames for max. value PhaseLenght_Fr otherwise (PhaseLength_Fr × 65,534) Condition: AcquisitionMode is set to HFR Maximum range value. MC_INDETERMINATE The frame acquisition is repeated indefinitely, a user break is required to terminate a sequence SeqLength_Pg Number of pages in a sequence

---

#### SeqLength_Pg

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3408 << 14` |
| String ID | `SeqLength_Pg` |
| C/C++ ID | `MC_SeqLength_Pg` |

**Description:**

The SeqLength_Pg parameter establishes the number of pages constituting a sequence. The user is invited to set this parameter when EndTrigMode is AUTO and AcquisitionMode is PAGE. Setting SeqLength_Pg to MC_INDETERMINATE results in indefinitely repeated pages acquisition. A user break is required to terminate the sequence. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Usage:**

Relevance condition(s): Condition: AcquisitionMode is set to PAGE

**Values:**

- `1`: 1 page Minimum range value.
- `65534`: 65,534 pages Maximum range value. MC_INDETERMINATE The page acquisition is repeated indefinitely, a user break is required to terminate a sequence SeqLength_Ln Number of acquired lines in a sequence

---

#### SeqLength_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3401 << 14` |
| String ID | `SeqLength_Ln` |
| C/C++ ID | `MC_SeqLength_Ln` |

**Description:**

The SeqLength_Ln parameter establishes the number of lines to be acquired in a sequence. The user is invited to set this parameter when EndTrigMode is AUTO and AcquisitionMode has one of the following values: WEB or LONGPAGE. Setting SeqLength_Ln to MC_INDETERMINATE results in indefinitely repeated page acquisition. A user break is required to terminate the sequence. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Usage:**

Relevance condition(s): Condition: AcquisitionMode is set to WEB or LONGPAGE

**Values:**

- `1`: 1 line Minimum range value. Variable (PageLength_Ln * 65,535) frames Maximum range value. MC_INDETERMINATE The line acquisition is repeated indefinitely, a user break is required to terminate a sequence Condition: AcquisitionMode is set to WEB SeqLength_Ph Number of acquisition phases constituting a sequence

---

#### SeqLength_Ph

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `3399 << 14` |
| String ID | `SeqLength_Ph` |
| C/C++ ID | `MC_SeqLength_Ph` |

**Description:**

The user is invited to get the value of this parameter when EndTrigMode is set to AUTO. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

---

#### PhaseLength_Fr

*Number of frames constituting a phase*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3409 << 14` |
| String ID | `PhaseLength_Fr` |
| C/C++ ID | `MC_PhaseLength_Fr` |

**Description:**

The parameter establishes the total number of frames acquired within an acquisition phase. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- `1`: 1 frame Condition: AcquisitionMode is set to SNAPSHOT.
- `1`: 1 frame Condition: AcquisitionMode is set to HFR. Minimum range value.
- `255`: 255 frames Condition: AcquisitionMode is set to HFR. Maximum range value. PhaseLength_Pg Number of pages constituting a phase

---

#### PhaseLength_Pg

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3418 << 14` |
| String ID | `PhaseLength_Pg` |
| C/C++ ID | `MC_PhaseLength_Pg` |

**Description:**

This parameter establishes the total number of pages acquired within an acquisition phase. Refer to the "MultiCam Acquisition Principles" on page 492 application note.

---

#### Elapsed_Fr

*Elapsed number of acquired frames*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3453 << 14` |
| String ID | `Elapsed_Fr` |
| C/C++ ID | `MC_Elapsed_Fr` |

**Description:**

This parameter gives information about the acquisition sequence progress, by reporting the number of completed frame acquisitions in a sequence.

---

#### Remaining_Fr

*Number of remaining frames to acquire*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3454 << 14` |
| String ID | `Remaining_Fr` |
| C/C++ ID | `MC_Remaining_Fr` |

**Description:**

This parameter gives information about the acquisition sequence progress, by reporting the number of remaining frames to acquire in a sequence.

**Usage:**

Relevance condition(s): Condition: Seqlength_Fr is not set to MC_INDETERMINATE

**Values:**

- `0`: Minimum range value. PerSecond_Fr Number of frames acquired during a second

---

#### PerSecond_Fr

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3452 << 14` |
| String ID | `PerSecond_Fr` |
| C/C++ ID | `MC_PerSecond_Fr` |

---

#### Elapsed_Pg

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3455 << 14` |
| String ID | `Elapsed_Pg` |
| C/C++ ID | `MC_Elapsed_Pg` |

**Description:**

This parameter gives information about the acquisition sequence progress, by reporting the number of completed page acquisitions in a sequence.

---

#### Remaining_Pg

*Number of remaining pages to acquire*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3457 << 14` |
| String ID | `Remaining_Pg` |
| C/C++ ID | `MC_Remaining_Pg` |

**Description:**

This parameter gives information about the acquisition sequence progress, by reporting the number of remaining pages to acquire in a sequence.

**Usage:**

Relevance condition(s): Condition: Seqlength_Pg is not set to MC_INDETERMINATE

**Values:**

- `0`: Minimum range value. Elapsed_Ln Elapsed number of acquired lines

---

#### Elapsed_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3456 << 14` |
| String ID | `Elapsed_Ln` |
| C/C++ ID | `MC_Elapsed_Ln` |

**Description:**

This parameter gives information about the acquisition sequence progress, by reporting the number of completed line acquisitions in a sequence.

---

#### Remaining_Ln

*Number of remaining lines to acquire*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Acquisition Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `3458 << 14` |
| String ID | `Remaining_Ln` |
| C/C++ ID | `MC_Remaining_Ln` |

**Description:**

This parameter gives information about the acquisition sequence progress, by reporting the number of remaining lines to acquire in a sequence.

**Usage:**

Relevance condition(s): Condition: Seqlength_Ln is not set to MC_INDETERMINATE

**Values:**

- `0`: Minimum range value. 4.6. Trigger Control Category Parameters controlling the triggering features associated to the channel TrigCtl
- `238`: TrigEdge
- `240`: TrigFilter
- `241`: TrigDelay_us
- `243`: PageDelay_Ln
- `244`: TrigDelay_Pls
- `245`: NextTrigDelay_Pls
- `246`: EndTrigCtl
- `247`: EndTrigEdge
- `249`: EndTrigFilter
- `250`: EndTrigEffect
- `252`: EndPageDelay_Ln
- `254`: ForceTrig
- `255`: TrigLine
- `256`: EndTrigLine
- `259`: TrigCtl Electrical style of the trigger hardware line

---

#### TrigCtl

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `513 << 14` |
| String ID | `TrigCtl` |
| C/C++ ID | `MC_TrigCtl` |

**Description:**

This parameter specifies the electrical style of the GPIO line used as trigger input. Along with TrigEdge and TrigFilter, it declares the grabber attributes of the trigger line sensed by the channel and aimed at generating the trigger event.

**Usage:**

Relevance condition(s): Condition: A hardware line is used as trigger input. TrigMode or NextTrigMode are set to HARD or COMBINED.

**Values:**

- **`DIFF`**
  - C/C++: `MC_TrigCtl_DIFF`
  - Differential high-speed input compatible with EIA/TIA-422 signaling.
- **`ISO`**
  - C/C++: `MC_TrigCtl_ISO`
  - Isolated current loop input compatible with TTL, +12V, +24V signaling. Default value.
- **`CAMERA`**
  - C/C++: `MC_TrigCtl_CAMERA`
  - Camera Link downstream signaling.
  - *Condition: BoardTopology ≠MONO_SLOW*
  - *Condition: BoardTopology ≠DUO_SLOW*

---

#### TrigEdge

*Significant edge of designated trigger hardware line from outside system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `664 << 14` |
| String ID | `TrigEdge` |
| C/C++ ID | `MC_TrigEdge` |

**Description:**

This parameter applies to the hardware line designated by TrigLine or TrigLineIndex . Along with TrigCtl and TrigFilter , it declares the grabber attributes of the trigger line sensed by the channel and aimed at generating the trigger event.

**Values:**

- **`GOHIGH`**
  - C/C++: `MC_TrigEdge_GOHIGH`
  - The trigger event is generated at each positive-going transition of the trigger line.
- **`GOLOW`**
  - C/C++: `MC_TrigEdge_GOLOW`
  - The trigger event is generated at each negative-going transition of the trigger line.

---

#### TrigFilter

*Noise removal on designated trigger hardware line from outside system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `665 << 14` |
| String ID | `TrigFilter` |
| C/C++ ID | `MC_TrigFilter` |

**Description:**

This parameter applies to the hardware line designated by TrigLine or TrigLineIndex . Along with TrigCtl and TrigEdge , it declares the grabber attributes of the trigger line sensed by the channel and aimed at generating the trigger event. The TrigFilter parameter specifies the time constant of the noise reduction filter of the designated hardware line. The time constant of the filter is the amount of time the line should be detected at the same logic state before a logic transition be considered. When insulated I/Os are used, TrigFilter is MEDIUM or STRONG. The value OFF is not allowed.

**Values:**

- **`OFF`**
  - C/C++: `MC_TrigFilter_OFF`
  - The noise removal filter is turned off.
  - Time constant = 100 ns
- **`ON`**
  - C/C++: `MC_TrigFilter_ON`
  - The noise removal filter is turned on.
  - Time constant = 500 ns
- **`MEDIUM`**
  - C/C++: `MC_TrigFilter_MEDIUM`
  - The noise removal filter is turned on with a moderate filtering effect.
  - Time constant = 500 ns
- **`STRONG`**
  - C/C++: `MC_TrigFilter_STRONG`
  - The noise removal filter is turned on with a strong filtering effect.
  - Time constant = 2500 ns

---

#### TrigDelay_us

*Trigger delay before the reset pulse is sent to the camera, expressed in microseconds*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `2153 << 14` |
| String ID | `TrigDelay_us` |
| C/C++ ID | `MC_TrigDelay_us` |

**Description:**

This parameter can be used to insert a delay between the hardware trigger and the reset pulse sent to the camera.
> **NOTE:** This parameter does not affect software triggers. NOTE This parameter is applicable exclusively for area-scan cameras. For line-scan cameras, use instead PageDelay_Ln .

**Values:**

- `0`: No delay Minimum range value.
- `2000000`: 2,000,000 microseconds (= 2 seconds) Maximum range value. PageDelay_Ln Delay from trigger to start the page acquisition, expressed as a number of scanned lines

---

#### PageDelay_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1038 << 14` |
| String ID | `PageDelay_Ln` |
| C/C++ ID | `MC_PageDelay_Ln` |

**Description:**

This parameter can be used to insert a programmable delay between the hardware trigger and the start of the acquisition. It is expressed as a number of scanned lines. It exclusively applies to line-scan cameras when AcquisitionMode is LONGPAGE or PAGE. The waiting phase corresponding to the countdown of the page delay can overlap the previous page acquisition phase.
> **NOTE:** For area-scan cameras, use TrigDelay_us instead.

**Usage:**

Directive: Use this feature to compensate the delay introduced by a position detector placed away from the camera field of view.

**Values:**

- `0`: No delay Minimum range value.
- `65534`: 65,534 lines delay Maximum range value. TrigDelay_Pls Number of hardware trigger pulses to ignore after the start of sequence event

---

#### TrigDelay_Pls

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `10080 << 14` |
| String ID | `TrigDelay_Pls` |
| C/C++ ID | `MC_TrigDelay_Pls` |

**Description:**

This parameter specifies the number of detected pulses on the hardware trigger line to be skipped after the acquisition sequence begins. This parameter applies when acquisition control settings require a hardware trigger or page trigger (all acquisition modes).

**Values:**

- `0`: No delay Minimum range value.
- `65536`: 65,536 ignored pulses Maximum range value. NextTrigDelay_Pls Number of hardware trigger pulses to skip between successive acquisition phases

---

#### NextTrigDelay_Pls

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `10081 << 14` |
| String ID | `NextTrigDelay_Pls` |
| C/C++ ID | `MC_NextTrigDelay_Pls` |

**Description:**

This parameter specifies the number of detected pulses on the hardware trigger line to be skipped between successive acquisition phases. This parameter applies when acquisition control settings require a hardware trigger or page trigger event for subsequent acquisition phases (SNAPSHOT, HFR and PAGE acquisition modes)

**Values:**

- `0`: No delay Minimum range value. Default value.
- `65536`: 65,536 skipped pulses Maximum range value. EndTrigCtl Electrical style of designated end trigger hardware line

---

#### EndTrigCtl

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `3447 << 14` |
| String ID | `EndTrigCtl` |
| C/C++ ID | `MC_EndTrigCtl` |

**Description:**

This parameter specifies the electrical style of the GPIO line used as end trigger input. Along with EndTrigEdge and EndTrigFilter, it declares the grabber attributes of the trigger line sensed by the channel and aimed at generating the end trigger event.

**Usage:**

Prerequisite action(s): Condition: AcquisitionMode = LONGPAGE Condition: EndTrigMode = HARD

**Values:**

- **`DIFF`**
  - C/C++: `MC_EndTrigCtl_DIFF`
  - Differential high-speed input compatible with EIA/TIA-422 signaling.
- **`ISO`**
  - C/C++: `MC_EndTrigCtl_ISO`
  - Isolated current loop input compatible with TTL, +12V, +24V signaling. Default value.
- **`CAMERA`**
  - C/C++: `MC_EndTrigCtl_CAMERA`
  - Isolated current loop input compatible with TTL, +12V, +24V signaling. Default value.
  - *Condition: BoardTopology ≠MONO_SLOW*
  - *Condition: BoardTopology ≠DUO_SLOW*

---

#### EndTrigEdge

*Significant edge of designated end trigger hardware line from outside system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `2905 << 14` |
| String ID | `EndTrigEdge` |
| C/C++ ID | `MC_EndTrigEdge` |

**Description:**

This parameter applies to the hardware line designated by EndTrigLine or EndTrigLineIndex . Along with EndTrigCtl and EndTrigFilter , it declares the grabber attributes of the end trigger line sensed by the channel and aimed at generating the end trigger event. EndTrigEdge determines the significant edge of the end trigger pulse.

**Values:**

- **`GOHIGH`**
  - C/C++: `MC_EndTrigEdge_GOHIGH`
  - The trigger event is generated at each positive-going transition of the trigger line.
- **`GOLOW`**
  - C/C++: `MC_EndTrigEdge_GOLOW`
  - The trigger event is generated at each negative-going transition of the trigger line.

---

#### EndTrigFilter

*Noise removal on designated end trigger hardware line from outside system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `3639 << 14` |
| String ID | `EndTrigFilter` |
| C/C++ ID | `MC_EndTrigFilter` |

**Description:**

This parameter applies to the hardware line designated by EndTrigLine or EndTrigLineIndex . Along with EndTrigCtl , EndTrigFilter declares the grabber attributes of the end trigger line sensed by the channel and aimed at generating the end trigger event. EndTrigFilter specifies the time constant of the noise reduction filter of the designated hardware line. The time constant of the filter is the amount of time the line should be detected at the same logic state before a logic transition be considered. When insulated I/O are used, EndTrigFilter is MEDIUM or STRONG. The value OFF is not allowed.

**Values:**

- **`OFF`**
  - C/C++: `MC_EndTrigFilter_OFF`
  - The filter time constant is approximately 100 ns.
  - *Condition: This value is not allowed when EndTrigFilter is set to RELAY.*
- **`ON`**
  - C/C++: `MC_EndTrigFilter_ON`
  - The filter time constant is approximately 500 ns.
- **`MEDIUM`**
  - C/C++: `MC_EndTrigFilter_MEDIUM`
  - The filter time constant is approximately 500 ns.
- **`STRONG`**
  - C/C++: `MC_EndTrigFilter_STRONG`
  - The filter time constant is approximately 2500 ns. Default value.

---

#### EndTrigEffect

*Effect of the "End Trigger" event*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10672 << 14` |
| String ID | `EndTrigEffect` |
| C/C++ ID | `MC_EndTrigEffect` |

**Description:**

Selects the effect of an "End Trigger" event on the end of the acquisition phase.

**Usage:**

Relevance condition(s): Condition: AcquisitionMode = LONGPAGE Condition: EndTrigMode = HARD

**Values:**

- **`FOLLOWINGLINE`**
  - C/C++: `MC_EndTrigEffect_FOLLOWINGLINE`
  - On reception of an "End Trigger" event, the MultiCam Acquisition Controller acquires the line following the "End Trigger" event then terminates the acquisition phase. « End Trigger »  event « Beginning Of Line » events EndTrigEffect = FOLLOWINGLINE Acquired Data « End of Acquisition Phase » event Terminates after the following line Default value.
- **`PRECEDINGLINE`**
  - C/C++: `MC_EndTrigEffect_PRECEDINGLINE`
  - On reception of an "End Trigger" event, the MultiCam Acquisition Controller acquires the line preceding the "End Trigger" event and terminates the acquisition phase immediately. « End Trigger »  event « Beginning Of Line » events EndTrigEffect = PRECEDINGLINE Acquired Data « End of Acquisition Phase » event
- **`Terminates immediately`**
  > **NOTE:** The PRECEDINGLINE value is not allowed for Bayer bi-linear line-scan cameras.

---

#### EndPageDelay_Ln

*Delay from end trigger to end of page acquisition, expressed as a number of scanned lines*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3723 << 14` |
| String ID | `EndPageDelay_Ln` |
| C/C++ ID | `MC_EndPageDelay_Ln` |

**Description:**

This parameter can be used to insert a programmable delay between the hardware end trigger and the end of the acquisition. It is expressed as a number of scanned lines. It exclusively applies to line-scan cameras when AcquisitionMode is LONGPAGE.

**Usage:**

Directive: Use this feature to compensate the delay introduced by a position detector placed away from the camera field of view.

**Values:**

- `0`: No delay Minimum range value.
- `65534`: 65534 lines delay Maximum range value. ForceTrig Forces an event trigger from the application

---

#### ForceTrig

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set Only |
| Num ID | `50 << 14` |
| String ID | `ForceTrig` |
| C/C++ ID | `MC_ForceTrig` |

**Description:**

Refer to the "MultiCam Acquisition Principles" on page 492 application note.

**Values:**

- **`TRIG`**
  - C/C++: `MC_ForceTrig_TRIG`
  - Forces a soft trigger event.
- **`ENDTRIG`**
  - C/C++: `MC_ForceTrig_ENDTRIG`
  - Forces a soft end trigger event.
  - *Condition: AcquisitionMode is set to LONGPAGE.*

---

#### TrigLine

*Designation of the trigger hardware line*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `666 << 14` |
| String ID | `TrigLine` |
| C/C++ ID | `MC_TrigLine` |

**Description:**

This parameter designates the GPIO line sensed by the channel and aimed at generating the trigger event. Along with "TrigCtl" on page 238, "TrigEdge" on page 240 and "TrigFilter" on page 241, it declares the grabber attributes of the trigger line sensed by the channel and aimed at generating the trigger event.

**Usage:**

Relevance condition(s): Condition: A hardware line is used as trigger input.TrigMode or NextTrigMode are set to HARD or COMBINED. Prerequisite action(s): Condition: TrigCtl is set according to the desired electrical style.

**Values:**

- **`NOM`**
  - C/C++: `MC_TrigLine_NOM`
  - Selects the nominal line corresponding to the pre-selected TrigCtl value: ● DIN2 when TrigCtl = DIFF ● IIN2 when TrigCtl = ISO ● FVAL when TrigCtl = CAMERA Default value.
- **`DIN1`**
  - C/C++: `MC_TrigLine_DIN1`
  - Differential high-speed input lines pair #1.
  - *Condition: TrigCtl = DIFF*
- **`DIN2`**
  - C/C++: `MC_TrigLine_DIN2`
  - Differential high-speed input lines pair #2.
  - *Condition: TrigCtl = DIFF*
- **`IIN1`**
  - C/C++: `MC_TrigLine_IIN1`
  - Isolated current loop input line #1.
  - *Condition: TrigCtl = ISO*
- **`IIN2`**
  - C/C++: `MC_TrigLine_IIN2`
  - Isolated current loop input line #2.
  - *Condition: TrigCtl = ISO*
- **`IIN3`**
  - C/C++: `MC_TrigLine_IIN3`
  - Isolated current loop input line #3.
  - *Condition: TrigCtl = ISO*
- **`IIN4`**
  - C/C++: `MC_TrigLine_IIN4`
  - Isolated current loop input line #4.
  - *Condition: TrigCtl = ISO*

---

#### EndTrigLine

*Designation of the end trigger hardware line*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Trigger Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `2906 << 14` |
| String ID | `EndTrigLine` |
| C/C++ ID | `MC_EndTrigLine` |

**Description:**

This parameter designates the GPIO line sensed by the channel and aimed at generating the end trigger event. Along with "EndTrigCtl" on page 247 and "EndTrigFilter" on page 250, it declares the grabber attributes of the trigger line sensed by the channel and aimed at generating the end trigger event.

**Usage:**

Prerequisite action(s): Condition: AcquisitionMode = LONGPAGE Condition: EndTrigMode = HARD

**Values:**

- **`NOM`**
  - C/C++: `MC_EndTrigLine_NOM`
  - Selects the nominal line corresponding to the pre-selected EndTrigCtl value: ● DIN2 when EndTrigCtl = DIFF ● IIN2 when EndTrigCtl = ISO ● FVAL when TrigCtl = CAMERA Default value.
- **`DIN1`**
  - C/C++: `MC_EndTrigLine_DIN1`
  - Differential high-speed input lines pair #1.
  - *Condition: EndTrigCtl = DIFF*
- **`DIN2`**
  - C/C++: `MC_EndTrigLine_DIN2`
  - Differential high-speed input lines pair #2.
  - *Condition: EndTrigCtl = DIFF*
- **`IIN1`**
  - C/C++: `MC_EndTrigLine_IIN1`
  - Isolated current loop input line #1.
  - *Condition: EndTrigCtl = ISO*
- **`IIN2`**
  - C/C++: `MC_EndTrigLine_IIN2`
  - Isolated current loop input line #2.
  - *Condition: EndTrigCtl = ISO*
- **`IIN3`**
  - C/C++: `MC_EndTrigLine_IIN3`
  - Isolated current loop input line #3.
  - *Condition: EndTrigCtl = ISO*
- **`IIN4`**
  - C/C++: `MC_EndTrigLine_IIN4`
  - Isolated current loop input line #4.
  - *Condition: EndTrigCtl = ISO*

---

### 4.7. Interleaved Acquisition Category

*Parameters controlling the interleaved acquisition feature*

#### InterleavedAcquisition

*Master control switch of the interleaved acquisition feature*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10936 << 14` |
| String ID | `InterleavedAcquisition` |
| C/C++ ID | `MC_InterleavedAcquisition` |

**Description:**

This parameter allows to switch ON or OFF the interleaved acquisition feature. When Interleaved Acquisition is turned ON, the Camera and Illumination Controller is configured with two different programs named P1 and P2. The programs are executed alternatively, starting with P1. For more information, refer to the Interleaved Acquisition section of the Grablink Documentation.

**Usage:**

Relevance condition(s): Condition: Available only for grabber-controlled exposure line-scan and area-scan cameras: CamConfig must be set to PxxRG or LxxxRG

**Values:**

- **`OFF`**
  - C/C++: `MC_InterleavedAcquisition_OFF`
  - Interleaved acquisition is disabled. Default value.
- **`ON`**
  - C/C++: `MC_InterleavedAcquisition_ON`
  - Interleaved acquisition is enabled.

---

#### ExposureTime_P1_us

*Exposure time setting for P1 program cycles*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | ADJUST |
| Type | Float |
| Access | Set and Get |
| Num ID | `10940 << 14` |
| String ID | `ExposureTime_P1_us` |
| C/C++ ID | `MC_ExposureTime_P1_us` |

**Description:**

This parameter allows to specify the time interval between the Start of Exposure (ResetON) and the End of Exposure (ResetOFF) events of a P1 program cycle. MultiCam calculates a default value that is equal to the largest exposure time allowed by the camera when operating at the maximum cycle rate. The maximum cycle rate is defined by LineRate_Hz for line-scan cameras and FrameRate_mHz for area-scan cameras. The effective exposure time is reported by ExposureTime_P1_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user must set the exposure time according to the application needs within the range of values allowed by the camera. The camera exposure time range is defined by ExposeMin_us and ExposeMax_us camera parameters.

**Values:**

- `5000000`: 5,000,000 microseconds (=5 seconds) Maximum range value. ExposureTime_P1_Effective_us Effective exposure time for P1

---

#### ExposureTime_P1_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11005 << 14` |
| String ID | `ExposureTime_P1_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time interval between the Start of Exposure (ResetON) and the End of Exposure (ResetOFF) events of a P1 program cycle.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### ExposureTime_P2_us

*Exposure time setting for P2 program cycles*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | ADJUST |
| Type | Float |
| Access | Set and Get |
| Num ID | `10941 << 14` |
| String ID | `ExposureTime_P2_us` |
| C/C++ ID | `MC_ExposureTime_P2_us` |

**Description:**

This parameter allows to specify the time interval between the Start of Exposure (ResetON) and the End of Exposure (ResetOFF) events of a P2 program cycle. MultiCam calculates a default value that is equal to the largest exposure time allowed by the camera when operating at the maximum cycle rate. The maximum cycle rate is defined by LineRate_Hz for line-scan cameras and FrameRate_mHz for area-scan cameras. The effective exposure time is reported by ExposureTime_P2_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user must set the exposure time according to the application needs within the range of values allowed by the camera. The camera exposure time range is defined by ExposeMin_us and ExposeMax_us camera parameters.

**Values:**

- `5000000`: 5,000,000 microseconds (=5 seconds) Maximum range value. ExposureTime_P2_Effective_us Effective exposure time for P2

---

#### ExposureTime_P2_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11006 << 14` |
| String ID | `ExposureTime_P2_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time interval between the Start of Exposure (ResetON) and the End of Exposure (ResetOFF) events of a P2 program cycle.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### ExposureDelayControl

*Control method of the exposure delay*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Set and Get |
| Num ID | `11045 << 14` |
| String ID | `ExposureDelayControl` |
| C/C++ ID | `MC_ExposureDelayControl` |

**Description:**

This parameter allows to select the method used by MultiCam to calculate the Exposure Delay value for P1 and P2 programs. By default, MultiCam configures P1 and P2 with the smallest possible Exposure Delay value. This setting is satisfactory for the use cases where the exposure time is shorter than the readout time. Optionally, keeping ExposureDelayControl set to MAN, allows to change the minimum exposure delay value of P1 and/or P2 using the ExposureDelay_MAN_P1_us and ExposureDelay_MAN_P2_ us parameters. Alternatively, you may also change ExposureDelayControl to one of the automatic control methods: SAME_START_EXPOSURE or SAME_END_EXPOSURE. With SAME_START_EXPOSURE, the start of exposure is delayed by the same amount of time for both programs: both exposure delay values are equal. With SAME_END_EXPOSURE the end of exposure is delayed by the same amount of time for both programs. The effective exposure delay values, calculated by MultiCam are reported by ExposureDelay_P1_ Effective_us and ExposureDelay_P2_Effective_us parameters.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

**Values:**

- **`MAN`**
  - C/C++: `MC_ExposureDelayControl_MAN`
  - Manual control method. The user may specify the minimum exposure delay value of P1 and/or P2 using the ExposureDelay_MAN_P1_us and ExposureDelay_MAN_P2_us parameters respectively. Default value.
- **`SAME_START_EXPOSURE`**
  - C/C++: `MC_ExposureDelayControl_SAME_START_EXPOSURE`
  - Automatic control method 1. The time interval from the cycle trigger to the start of exposure is identical for both programs.
- **`SAME_START_EXPOSURE`**
  - C/C++: `MC_ExposureDelayControl_SAME_START_EXPOSURE`
  - Automatic control method 2. The time interval from the cycle trigger to the end of exposure is identical for both programs.

---

#### ExposureDelay_MAN_P1_us

*Minimum exposure delay value for P1*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Set and Get |
| Num ID | `11039 << 14` |
| String ID | `ExposureDelay_MAN_P1_us` |
| C/C++ ID | `MC_ExposureDelay_MAN_P1_us` |

**Description:**

When InterleavedAcquisition is set to ON, this parameter allows to specify the minimum time interval to be inserted before the Start of Exposure (ResetON) of P1 program cycles. The effective time interval is reported by ExposureDelay_P1_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Condition: ExposureDelayControl must be set to MAN.

**Values:**

- `0`: Minimum range value.
- `5000000`: 5,000,000 microseconds (=5 seconds) Maximum range value. ExposureDelay_P1_Effective_us Effective exposure delay value for P1 program

---

#### ExposureDelay_P1_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11011 << 14` |
| String ID | `ExposureDelay_P1_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time delay inserted before the Start of Exposure (ResetON) event of P1 program cycles.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### ExposureDelay_MAN_P2_us

*Minimum exposure delay value for P2*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Set and Get |
| Num ID | `11040 << 14` |
| String ID | `ExposureDelay_MAN_P2_us` |
| C/C++ ID | `MC_ExposureDelay_MAN_P2_us` |

**Description:**

When InterleavedAcquisition is set to ON, this parameter allows to specify the minimum time interval to be inserted before the Start of Exposure (ResetON) of P2 program cycles. The effective time interval is reported by ExposureDelay_P2_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Condition: ExposureDelayControl must be set to MAN.

**Values:**

- `0`: Minimum range value.
- `5000000`: 5,000,000 microseconds (=5 seconds) Maximum range value. ExposureDelay_P2_Effective_us Effective exposure delay value for P2 program

---

#### ExposureDelay_P2_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11012 << 14` |
| String ID | `ExposureDelay_P2_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time delay inserted before the Start of Exposure (ResetON) event of P2 program cycles.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### StrobeDuration_P1_us

*Strobe duration setting for P1 program cycles*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | ADJUST |
| Type | Float |
| Access | Set and Get |
| Num ID | `10950 << 14` |
| String ID | `StrobeDuration_P1_us` |
| C/C++ ID | `MC_StrobeDuration_P1_us` |

**Description:**

This parameter allows to specify the time interval between the Start of Illumination (StrobeON) and the End of Illumination (StrobeOFF) events of a P1 program cycle. MultiCam calculates a default value that is equal to 50% of the default exposure time. The effective strobe duration is reported by StrobeDuration_P1_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user must set the strobe duration according to the application needs. Values larger than the exposure time are allowed.

**Values:**

- `5000000`: 5,000,000 microseconds (= 5 seconds) after Maximum range value. StrobeDuration_P1_Effective_us Effective strobe duration for P1

---

#### StrobeDuration_P1_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11007 << 14` |
| String ID | `StrobeDuration_P1_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time interval between the Start of Illumination (StrobeON) and the End of Illumination (StrobeOFF) events of a P1 program cycle.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### StrobeDuration_P2_us

*Strobe duration setting for P2 program cycles*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | ADJUST |
| Type | Float |
| Access | Set and Get |
| Num ID | `10951 << 14` |
| String ID | `StrobeDuration_P2_us` |
| C/C++ ID | `MC_StrobeDuration_P2_us` |

**Description:**

This parameter allows to specify the time interval between the Start of Illumination (StrobeON) and the End of Illumination (StrobeOFF) events of a P2 program cycle. MultiCam calculates a default value that is equal to 50% of the default exposure time. The effective strobe duration is reported by StrobeDuration_P2_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user must set the strobe duration according to the application needs. Values larger than the exposure time are allowed.

**Values:**

- `5000000`: 5,000,000 microseconds (= 5 seconds) after Maximum range value. StrobeDuration_P2_Effective_us Effective strobe duration for P2

---

#### StrobeDuration_P2_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11008 << 14` |
| String ID | `StrobeDuration_P2_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time interval between the Start of Illumination (StrobeON) and the End of Illumination (StrobeOFF) events of a P2 program cycle.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### StrobeDelay_P1_us

*Strobe delay setting for P1*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | ADJUST |
| Type | Float |
| Access | Set and Get |
| Num ID | `10953 << 14` |
| String ID | `StrobeDelay_P1_us` |
| C/C++ ID | `MC_StrobeDelay_P1_us` |

**Description:**

This parameter allows to specify the time interval from the Start of Exposure (ResetON) to the Start of Illumination (StrobeON) events of P1 program cycles. The default value is 0. The effective delay is reported by StrobeDelay_P1_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user must set the strobe delay according to the application needs. Set a positive values to retard the Start of Illumination (StrobeON) event. Set a negative values to advance the Start of Illumination (StrobeON) event.

**Values:**

- `-10000`: 10,000 microseconds (= 10 milliseconds) before Minimum range value.
- `5000000`: 5,000,000 microseconds (= 5 seconds) after Maximum range value. StrobeDelay_P1_Effective_us Effective strobe delay value for P1 program

---

#### StrobeDelay_P1_Effective_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11009 << 14` |
| String ID | `StrobeDelay_P1_Effective_` |
| C/C++ ID | `us` |

**Description:**

This parameter reports the effective time interval from the Start of Exposure (ResetON) to the Start of Illumination (StrobeON) events of P1 program cycles.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### StrobeDelay_P2_us

*Strobe delay setting for P2*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | ADJUST |
| Type | Float |
| Access | Set and Get |
| Num ID | `10954 << 14` |
| String ID | `StrobeDelay_P2_us` |
| C/C++ ID | `MC_StrobeDelay_P2_us` |

**Description:**

This parameter allows to specify the time interval from the Start of Exposure (ResetON) to the Start of Illumination (StrobeON) events of P2 program cycles. The default value is 0. The effective delay is reported by StrobeDelay_P2_Effective_us.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user must set the strobe delay according to the application needs. Set a positive values to retard the Start of Illumination (StrobeON) event. Set a negative values to advance the Start of Illumination (StrobeON) event.

**Values:**

- `-10000`: 10,000 microseconds (= 10 milliseconds) before Minimum range value.
- `5000000`: 5,000,000 microseconds (= 5 seconds) after Maximum range value. StrobeDelay_P2_Effective_us Effective strobe delay value for P2 program

---

#### StrobeDelay_P2_Effective_

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11010 << 14` |
| String ID | `StrobeDelay_P2_Effective_` |
| C/C++ ID | `us` |

**Description:**

This parameter reports the effective time interval from the Start of Exposure (ResetON) to the Start of Illumination (StrobeON) events of P2 program cycles.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### MinTriggerPeriod_P1_Effective_us

*Minimum time interval between cycle triggers for P1*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11035 << 14` |
| String ID | `MinTriggerPeriod_P1_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time interval between a P1 Cycle start trigger and the next cycle start trigger.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### MinTriggerPeriod_P2_Effective_us

*Minimum time interval between cycle triggers for P2*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Float |
| Access | Get Only |
| Num ID | `11036 << 14` |
| String ID | `MinTriggerPeriod_P2_` |
| C/C++ ID | `Effective_us` |

**Description:**

This parameter reports the effective time interval between a P2 Cycle start trigger and the next cycle start trigger.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON.

---

#### StrobeLine_P1

*Strobe output line of P1 program*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10955 << 14` |
| String ID | `StrobeLine_P1` |
| C/C++ ID | `MC_StrobeLine_P1` |

**Description:**

This parameter allows to designate the output line delivering the strobe output of the P1 program. By default, MultiCam assigns the strobe output of the P1 program to the IOUT1 output port.
> **NOTE:** When the P1 and P2 programs are assigned to the same strobe output line,the strobe signals are logically OR-ed.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user may select the alternate strobe line by selecting IOUT2 or disable the strobe output of the P1 program by selecting NONE.

**Values:**

- **`IOUT1`**
  - C/C++: `MC_StrobeLine_P1_IOUT1`
  - The strobe signal of P1 program is assigned to the IOUT 1 output port. Default value.
- **`IOUT2`**
  - C/C++: `MC_StrobeLine_P1_IOUT2`
  - The strobe signal of P1 program is assigned to the IOUT 2 output port.
- **`NONE`**
  - C/C++: `MC_StrobeLine_P1_NONE`
  - The strobe signal of P1 program is not assigned to any output port.

---

#### StrobeLine_P2

*Strobe output line of P2 program*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10956 << 14` |
| String ID | `StrobeLine_P2` |
| C/C++ ID | `MC_StrobeLine_P2` |

**Description:**

This parameter allows to designate the output line delivering the strobe output of the P2 program. By default, MultiCam assigns the strobe output of the P2 program to the IOUT2 output port.
> **NOTE:** When the P1 and P2 programs are assigned to the same strobe output line,the strobe signals are logically OR-ed.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: The user may select the alternate strobe line by selecting IOUT2 or disable the strobe output of the P2 program by selecting NONE.

**Values:**

- **`IOUT1`**
  - C/C++: `MC_StrobeLine_P2_IOUT1`
  - The strobe signal of P2 program is assigned to the IOUT 1 output port.
- **`IOUT2`**
  - C/C++: `MC_StrobeLine_P2_IOUT2`
  - The strobe signal of P2 program is assigned to the IOUT 2 output port. Default value.
- **`NONE`**
  - C/C++: `MC_StrobeLine_P2_NONE`
  - The strobe signal of P2 program is not assigned to any output port.

---

#### StrobeOutput_P1

*Strobe output control for P1*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11020 << 14` |
| String ID | `StrobeOutput_P1` |
| C/C++ ID | `MC_StrobeOutput_P1` |

**Description:**

This parameter allows to enable or disable immediately the strobe output of the P1 program. MultiCam enables automatically the strobe output of P1 program providing that:
- 
InterleavedAcquisition is set to ON,
- 
StrobeLine_P1 is set to IOUT1 or IOUT2. MultiCam disables automatically the output when one of the above condition becomes false.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: If required, the user may override the parameter value at any time.

**Values:**

- **`ENABLE`**
  - C/C++: `MC_StrobeOutput_P1_ENABLE`
  - The Strobe output of P1 program is enabled. Default value.
- **`DISABLE`**
  - C/C++: `MC_StrobeOutput_P1_DISABLE`
  - The Strobe output of P1 program is disabled.

---

#### StrobeOutput_P2

*Strobe output control for P2*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Interleaved Acquisition |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11021 << 14` |
| String ID | `StrobeOutput_P2` |
| C/C++ ID | `MC_StrobeOutput_P2` |

**Description:**

This parameter allows to enable or disable immediately the strobe output of the P2 program. MultiCam enables automatically the strobe output of P2 program providing that:
- 
InterleavedAcquisition is set to ON,
- 
StrobeLine_P2 is set to IOUT1 or IOUT2. MultiCam disables automatically the output when one of the above condition becomes false.

**Usage:**

Prerequisite action(s): Condition: InterleavedAcquisition must be set to ON. Directive: If required, the user may override the parameter value at any time.

**Values:**

- **`ENABLE`**
  - C/C++: `MC_StrobeOutput_P2_ENABLE`
  - The Strobe output of P2 program is enabled. Default value.
- **`DISABLE`**
  - C/C++: `MC_StrobeOutput_P2_DISABLE`
  - The Strobe output of P2 program is disabled.

---

### 4.8. Exposure Control Category

*Parameters controlling the camera exposure related features associated to the channel*

#### Expose_us

*Line or frame exposure duration, expressed in microseconds*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exposure Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `830 << 14` |
| String ID | `Expose_us` |
| C/C++ ID | `MC_Expose_us` |

**Description:**

For area-scan cameras, the Expose_us parameter relates to the duration of the frame exposure period. For line-scan cameras, the Expose_us parameter relates to the duration of the line exposure period. Specifically, several controllable cameras make possible for the frame grabber to take control on the exposure period within the camera. This equally applies to the line exposure (line-scan) or frame exposure (area-scan). If an area-scan camera has this exposure control capability, and if it is configured in such a way that this capability is exercised, the camera is said to assume the grabber-controlled exposure mode. This parameter applies only when camera operates in grabber-controlled exposure mode. Refer to the expert-level parameters of the Camera Features Category Expose and Readout .

**Values:**

- `1`: 1 microsecond Minimum range value.
- `20000000`: 20,000,000 microseconds (=20 seconds) Maximum range value. ExposeTrim Amending value for exposure duration, expressed in decibels

---

#### ExposeTrim

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exposure Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `831 << 14` |
| String ID | `ExposeTrim` |
| C/C++ ID | `MC_ExposeTrim` |

**Description:**

This parameter can be used to refine the value programmed by the Expose_us parameter. The following chart helps to understand this logarithmic control process.

**Values:**

- `-6`: - 6 dB: The effective trimmed exposure is Expose_us x 0.5
- `-3`: -3 dB: The effective trimmed exposure is Expose_us x 0.7
- `0`: 0 dB: The effective trimmed exposure is Expose_us Default value.
- `3`: +3 dB: The effective trimmed exposure is Expose_us x 1.4
- `6`: +6 dB: The effective trimmed exposure is Expose_us x 2.0
- `9`: +9 dB: The effective trimmed exposure is Expose_us x 2.8
- `12`: +12 dB: The effective trimmed exposure is Expose_us x 4.0 TrueExp_us Exact exposure duration, expressed in microseconds

---

#### TrueExp_us

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exposure Control |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `832 << 14` |
| String ID | `TrueExp_us` |
| C/C++ ID | `MC_TrueExp_us` |

**Description:**

This parameter returns the effective duration of the exposure period, merging the values of Expose_us and ExposeTrim . Some camera and/or frame grabber limitation can be such that the effective exposure duration may slightly differ from the requested exposure duration. Setting this parameter is required when the strobe function is involved while the grabber does not positively control the exposure function. See StrobeMode .

**Values:**

- `1`: 1 microsecond Minimum range value.
- `20000000`: 20,000,000 microseconds (=20 seconds) Maximum range value. 4.9. Strobe Control Category Parameters controlling the illumination features associated to the channel StrobeMode
- `298`: StrobeDur
- `300`: StrobePos
- `301`: StrobeCtl
- `302`: PreStrobe_us
- `303`: StrobeMode Method for generating strobe pulse to illumination system

---

#### StrobeMode

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Strobe Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1197 << 14` |
| String ID | `StrobeMode` |
| C/C++ ID | `MC_StrobeMode` |

**Description:**

This parameter establishes the method according to which the illumination control pulse is generated. For area-scan cameras, this parameter relates to the illumination during the frame exposure period. For line-scan cameras, this parameter relates to the illumination during the line exposure period. The default value is set automatically to MAN or AUTO by the exposure controller of MultiCam.

**Values:**

- **`NONE`**
  - C/C++: `MC_StrobeMode_NONE`
  - The strobe function is disabled. No strobe line is allocated to the channel. The hardware line dedicated to issuing the strobe pulse is available for general-purpose usage.
- **`AUTO`**
  - C/C++: `MC_StrobeMode_AUTO`
  - The strobe function is enabled with an automatic timing control feature. StrobeDur and StrobePos parameters define the strobe pulse, using the exposure duration declared by the Expose_us parameter.
  - *Condition: Grabber controlled exposure.*
- **`MAN`**
  - C/C++: `MC_StrobeMode_MAN`
  - The strobe function is enabled with a manual timing control feature. StrobeDur and StrobePos parameters define the strobe pulse, using the exposure duration declared by the TrueExp_us parameter.
  - *Condition: Camera controlled exposure.*
- **`OFF`**
  - C/C++: `MC_StrobeMode_OFF`
  - The designed StrobeLine is set to the inactive level; no more strobe pulses are issued.

---

#### StrobeDur

*Duration of strobe pulse to illumination system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Strobe Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `834 << 14` |
| String ID | `StrobeDur` |
| C/C++ ID | `MC_StrobeDur` |

**Description:**

This parameter is expressed as a percentage of the current exposure duration as returned by TrueExp_us . A value of 50 % means that the duration of the strobe pulse is half the duration of the exposure period.
- 
For area-scan cameras, the StrobeDur parameter relates to the illumination during the frame exposure period.
- 
For line-scan cameras, the StrobeDur parameter relates to the illumination during the line exposure period. Strobe duration formula

---

#### StrobePos

*Position of strobe pulse to illumination system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Strobe Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `835 << 14` |
| String ID | `StrobePos` |
| C/C++ ID | `MC_StrobePos` |

**Description:**

This parameter is expressed as a percentage of the allowed position range of the strobe pulse for its current duration. A value of 0 % establishes the earliest position. A value of 100 % establishes the latest position. A value of 50 % means that the strobe pulse is located in the middle of the exposure period. Strobe Position vs. StrobePos value
- 
For area-scan cameras, the StrobePos parameter relates to the illumination during the frame exposure period.
- 
For line-scan cameras, the StrobePos parameter relates to the illumination during the line exposure period. Strobe position formula The StrobePos refers to the middle of the strobe pulse.

---

#### StrobeCtl

*Electrical style of designated strobe pulse to illumination system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Strobe Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `836 << 14` |
| String ID | `StrobeCtl` |
| C/C++ ID | `MC_StrobeCtl` |

**Description:**

This parameter declares the attributes of the strobe line designated by StrobeLine sent by the channel and aimed at generating an illumination pulse.

**Values:**

- **`OPTO`**
  - C/C++: `MC_StrobeCtl_OPTO`
  - The strobe line is issued on an opto-isolated pair of pins. The + pin is the collector and the - pin is the emitter of an uncommitted photo-transistor driven by LED-emitted light.

---

#### PreStrobe_us

*Time delay, expressed in microseconds, before the pulse defined by StrobePos*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Strobe Control |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `2190 << 14` |
| String ID | `PreStrobe_us` |
| C/C++ ID | `MC_PreStrobe_us` |

**Description:**

This parameter declares the delay of the beginning of the strobe pulse before the normal beginning defined by StrobePos . If long enough, it creates a "pre-exposure" phase before actual "start of exposure phase" (SAP).
- 
For area-scan cameras, this parameter relates to the illumination during the frame exposure period.
- 
For line-scan cameras, this parameter is irrelevant.

**Values:**

- `0`: Minimum range value.
- `10000`: 10,000 microseconds (=10 milliseconds) Maximum range value. 4.10. Encoder Control Category Parameters controlling the motion encoder rate conversion device embedded in the line-scan capable frame grabbers LineCaptureMode
- `305`: LineRateMode
- `307`: Period_us
- `309`: PeriodTrim
- `310`: LinePitch
- `311`: EncoderPitch
- `312`: LineTrigCtl
- `313`: LineTrigEdge
- `315`: LineTrigFilter
- `317`: BackwardMotionCancellationMode
- `320`: ForwardDirection
- `322`: RateDivisionFactor
- `323`: LineTrigLine
- `324`: EncoderTickCount
- `327`: BMCRestart
- `328`: RateDividerRestart
- `329`: MaxSpeed
- `330`: MaxSpeedEffective
- `331`: MinSpeed
- `332`: OnMinSpeed
- `333`: CrossPitch
- `334`: SynchronizedPeriodicGenerator
- `335`: LineCaptureMode Fundamental line capturing mode

---

#### LineCaptureMode

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `3001 << 14` |
| String ID | `LineCaptureMode` |
| C/C++ ID | `MC_LineCaptureMode` |

**Description:**

In line-scan system, this parameter declares the fundamental line capturing mode.

**Usage:**

Relevance condition(s): Condition: Any line-scan acquisition mode.

**Values:**

- **`ALL`**
  - C/C++: `MC_LineCaptureMode_ALL`
  - Take-All-Lines capture mode. Each delivered camera line results into a line acquisition. This is the traditional operating mode. If the down-web motion speed is varying, the line-scanning process of the camera would be rate-controlled accordingly. Default value.
- **`PICK`**
  - C/C++: `MC_LineCaptureMode_PICK`
  - Pick-A-Line line capture mode. The line-scanning process of the camera is running at a constant rate. Each pulse occurring at the down-web line rate determines the acquisition of the next line delivered by the camera.
- **`TAG`**
  - C/C++: `MC_LineCaptureMode_TAG`
  - Tag-A-Line capture mode. The line-scanning process of the camera is running at a constant rate determined by Period_us. The down-web line rate is determined by the pulse rate of A/B signals delivered by an external encoder and processed by the quadrature decoder and the rate divider. The frame grabber captures all lines delivered by the camera after having replaced the first pixel data by a tag indicating that the line was preceded or not by an hardware event on the divider output.

---

#### LineRateMode

*Line rate generation method*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1328 << 14` |
| String ID | `LineRateMode` |
| C/C++ ID | `MC_LineRateMode` |

**Description:**

In line-scan system, this parameter declares the device responsible for line rate generation. For more information, refer to "Line Rate Modes" on page 518. When LineRateMode is set to PERIOD, the downweb line rate is controlled by the Period_us parameter. When LineRateMode is set to EXPOSE, the downweb line rate is controlled by the Expose_us parameter. When LineRateMode is set to PULSE or CONVERT, the downweb line rate is directed by a pulse signal applied to line trigger hardware line selected by the LineTrigLine parameter. The applicable line rate modes are depending on the selected LineCaptureMode and on the camera line-scanning mode. The camera line-scanning mode is determined by the Expose and Readout parameters ("Camera Features Category " on page 110). Two classes of camera line- scanning mode are considered in this case:
- 
Free-running cameras
- 
Controlled line rate cameras

**Values:**

- **`CONVERT`**
  - C/C++: `MC_LineRateMode_CONVERT`
  - Rate Converter. The downweb line rate is derived from a train of trigger pulses processed by a rate converter belonging to the grabber.
- **`PULSE`**
  - C/C++: `MC_LineRateMode_PULSE`
  - Trigger Pulse. The downweb line rate is directly derived from trigger pulses applied to the grabber.
- **`PERIOD`**
  - C/C++: `MC_LineRateMode_PERIOD`
  - Periodic. The downweb line rate is internally generated by a periodic generator.
- **`CAMERA`**
  - C/C++: `MC_LineRateMode_CAMERA`
  - Camera. The downweb line rate is originated from the camera.
- **`EXPOSE`**
  - C/C++: `MC_LineRateMode_EXPOSE`
  - Exposure Time. The downweb line rate is identical to the camera line rate, and established by the exposure time settings.
- **`SLAVE`**
  - C/C++: `MC_LineRateMode_SLAVE`
  - Slave. The downweb line rate is originated from the master device. LineRateMode is automatically set to this value set when SynchronizedAcquisition = SLAVE or LOCAL_SLAVE.

---

#### Period_us

*Programmable line-scan period, expressed in microseconds*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1329 << 14` |
| String ID | `Period_us` |
| C/C++ ID | `MC_Period_us` |

**Description:**

This parameter allows for programming the periodic generator issuing the downweb line rate in line-scan systems.

**Values:**

- `1`: 1 microsecond Minimum range value.
- `1000000`: 1,000,000 microseconds (=1 second) Maximum range value. PeriodTrim Amending value for line-scan period duration

---

#### PeriodTrim

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1330 << 14` |
| String ID | `PeriodTrim` |
| C/C++ ID | `MC_PeriodTrim` |

**Description:**

This parameter can be used to refine the value programmed by the Period_us parameter.

**Values:**

- `-6`: -6 dB: The effective trimmed period is Period_us x 0.5
- `-3`: -3 dB: The effective trimmed period is Period_us x 0.7
- `0`: 0 dB: The effective trimmed period is Period_us Default value.
- `3`: +3 dB: The effective trimmed period is Period_us x 1.4
- `6`: +6 dB: The effective trimmed period is Period_us x 2.0
- `9`: +9 dB: The effective trimmed period is Period_us x 2.8
- `12`: +12 dB: The effective trimmed period is Period_us x 4.0 LinePitch Line pitch for rate converter programming

---

#### LinePitch

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `595 << 14` |
| String ID | `LinePitch` |
| C/C++ ID | `MC_LinePitch` |

**Description:**

This parameter applies when the motion encoder is in use with rate conversion. The parameter declares in an arbitrary length unit the distance between two successively scanned lines on the observed moving web. Along with EncoderPitch , it allows for programming the rate converter issuing the line rate in line-scan systems. The EncoderPitch parameter should be expressed in the same length unit. The programmed rate conversion ratio is: RateConversionRatio = EncoderPitch/LinePitch. The resulting downweb line rate is: DownwebLineRate = EncoderRate x RateConversioRatio The encoder rate at a given time is the frequency of the pulses delivered by the motion encoder while the observed web is moving.

**Values:**

- `1`: 1 length unit Minimum range value.
- `10000`: 10,000 length units Maximum range value. EncoderPitch Encoder pitch for rate converter programming

---

#### EncoderPitch

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `592 << 14` |
| String ID | `EncoderPitch` |
| C/C++ ID | `MC_EncoderPitch` |

**Description:**

This parameter applies when the motion encoder is in use with rate conversion. The parameter declares in an arbitrary length unit the distance traveled between two successive pulses issued by the motion encoder. Along with LinePitch, it allows for programming the rate converter issuing the line rate in line- scan systems. The LinePitch parameter should be expressed in the same length unit. The programmed rate conversion ratio is: RateConversionRatio = EncoderPitch/LinePitch. The resulting downweb line rate is: DownwebLineRate = EncoderRate x RateConversioRatio The encoder rate at a given time is the frequency of the pulses delivered by the motion encoder while the observed web is moving.

**Values:**

- `1`: 1 length unit Minimum range value.
- `10000`: 10,000 length units Maximum range value. LineTrigCtl Electrical style of designated line trigger hardware line from outside system

---

#### LineTrigCtl

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1331 << 14` |
| String ID | `LineTrigCtl` |
| C/C++ ID | `MC_LineTrigCtl` |

**Description:**

This parameter applies to the hardware line designated by LineTrigLine when LineRateMode is set to PULSE or CONVERT. Along with LineTrigEdge and LineTrigFilter, it declares the grabber attributes of the terminal sensing the line trigger pulse. The line trigger pulse is processed by the grabber and transferred to the camera as the line reset pulse. The LineTrigCtl parameter determines the electrical style of the line trigger pulse usually issued by a motion encoder.

**Values:**

- **`DIFF`**
  - C/C++: `MC_LineTrigCtl_DIFF`
  - Differential high-speed input compatible with EIA/TIA-422 signaling.
- **`DIFF_PAIRED`**
  - C/C++: `MC_LineTrigCtl_DIFF_PAIRED`
  - Dual differential high-speed input compatible with EIA/TIA-422 signaling. Default value.
- **`ISO`**
  - C/C++: `MC_LineTrigCtl_ISO`
  - Isolated current loop input compatible with TTL, +12V, +24V signaling.
- **`ISO_PAIRED`**
  - C/C++: `MC_LineTrigCtl_ISO_PAIRED`
  - Dual isolated current loop input compatible with TTL, +12V, +24V signaling.

---

#### LineTrigEdge

*Significant edge of designated line trigger hardware line from outside system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1332 << 14` |
| String ID | `LineTrigEdge` |
| C/C++ ID | `MC_LineTrigEdge` |

**Description:**

This parameter applies to the hardware line designated by LineTrigLine when LineRateMode is set to PULSE or CONVERT. Along with LineTrigCtl and LineTrigFilter, it declares the grabber attributes of the terminal sensing the line trigger pulse. The line trigger pulse is processed by the grabber and transferred to the camera as the line reset pulse. The LineTrigEdge parameter determines the significant edge of the line trigger pulse usually issued by a motion encoder.

**Values:**

- **`GOHIGH`**
  - C/C++: `MC_LineTrigEdge_GOHIGH`
  - Value equivalent to RISING_A.
- **`GOLOW`**
  - C/C++: `MC_LineTrigEdge_GOLOW`
  - Value equivalent to FALLING_A.
- **`RISING_A`**
  - C/C++: `MC_LineTrigEdge_RISING_A`
  - An output pulse is generated for every rising edge of the A signal. The falling edge on the A signal and both edges on the B-signal are ignored.
- **`FALLING_A`**
  - C/C++: `MC_LineTrigEdge_FALLING_A`
  - An output pulse is generated for every falling edge of the A signal. The rising edge on the A signal and both edges on the B-signal are ignored.
- **`ALL_A`**
  - C/C++: `MC_LineTrigEdge_ALL_A`
  - An output pulse is generated for every rising and falling edges of the A signal. The B-signal is ignored.
  - *Condition: LineTrigCtl is set to DIFF or ISO.*
- **`ALL_A_B`**
  - C/C++: `MC_LineTrigEdge_ALL_A_B`
  - An output pulse is generated for every rising and falling edges of the A and B signals.
  - *Condition: LineTrigCtl is set to DIFF_PAIRED or ISO_PAIRED.*

---

#### LineTrigFilter

*Noise removal on designated line trigger hardware line from outside system*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1333 << 14` |
| String ID | `LineTrigFilter` |
| C/C++ ID | `MC_LineTrigFilter` |

**Description:**

This parameter applies to the hardware line designated by LineTrigLine when LineRateMode is set to PULSE or CONVERT. Along with LineTrigCtl and LineTrigEdge, it declares the grabber attributes of the terminal sensing the line trigger pulse. The line trigger pulse is processed by the grabber and transferred to the camera as the line reset pulse. The LineTrigFilter parameter reduces the noise sensitivity over the line trigger pulse usually issued by a motion encoder. The time constant of the filter is the amount of time the line should be detected at the same logic state before a logic transition be considered. When LineTrigLine is an insulated I/O, LineTrigFilter is forced to STRONG.

**Values:**

- **`OFF`**
  - C/C++: `MC_LineTrigFilter_OFF`
  - The filter time constant is approximately 40 ns.
- **`MEDIUM`**
  - C/C++: `MC_LineTrigFilter_MEDIUM`
  - The filter time constant is approximately 500 ns. Default value.
- **`STRONG`**
  - C/C++: `MC_LineTrigFilter_STRONG`
  - The filter time constant is approximately 5 µs. Filter_40ns
  - C/C++: `MC_LineTrigFilter_Filter_40ns`
  - The filter time constant is approximately 40 ns. Filter_100ns
  - C/C++: `MC_LineTrigFilter_Filter_100ns`
  - The filter time constant is approximately 100 ns. Filter_200ns
  - C/C++: `MC_LineTrigFilter_Filter_200ns`
  - The filter time constant is approximately 200 ns. Filter_500ns
  - C/C++: `MC_LineTrigFilter_Filter_500ns`
  - The filter time constant is approximately 500 ns. Filter_1us
  - C/C++: `MC_LineTrigFilter_Filter_1us`
  - The filter time constant is approximately 1 us. Filter_5us
  - C/C++: `MC_LineTrigFilter_Filter_5us`
  - The filter time constant is approximately 5 us. Filter_10us
  - C/C++: `MC_LineTrigFilter_Filter_10us`
  - The filter time constant is approximately 10 us.

---

#### BackwardMotionCancellationMode

*Operational mode of the Backward Motion Cancellation circuit*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10352 << 14` |
| String ID | `BackwardMotionCancellationMode` |
| C/C++ ID | `MC_BackwardMotionCancellationMode` |

**Description:**

The backward cancellation circuit stops sending line trigger pulses as soon as a backward motion is detected. If such an event occurs, the acquisition is stopped. When the backward cancellation control is configured in the FILTERED mode (F-mode), the line acquisition resumes as soon as the motion is again in the forward direction. Therefore, the cancellation circuit filters out all the pulses corresponding to the backward direction. When the backward cancellation control is configured in the COMPENSATE mode (C-mode), the line acquisition resumes when the motion is again in the forward direction at the place it was interrupted. Therefore, the cancellation circuit filters out not only the pulses corresponding to the backward direction, but a number of forward pulses equal to the number of skipped backward pulses. In C-Mode, the cancellation circuit uses a "backward pulse counter" that:
- 
Increments by 1 every clock in the backward direction
- 
Decrements by 1 every clock in the forward direction until it reaches 0
- 
Resets at the beginning of each MultiCam acquisition sequence, more precisely, at the first trigger event of the sequence. This trigger is considered as the reference for the position along the web for the whole acquisition sequence. In C-Mode, all pulses occurring when the counter value is different of zero are blocked. The counter has a 16-bit span; backward displacement up to 65535 pulses can be compensated.

**Usage:**

Relevance condition(s): Condition: The line trigger originates from a quadrature motion encoder. Condition: The rate converter circuit is unused.

**Values:**

- **`OFF`**
  - C/C++: `MC_BackwardMotionCancellationMode_OFF`
  - The backward motion cancellation circuit is disabled. Default value.
- **`FILTERED`**
  - C/C++: `MC_BackwardMotionCancellationMode_FILTERED`
  - The backward motion cancellation circuit is enabled and configured for the filter mode.
  - *Condition: LineRateMode is set to PULSE*
  - *Condition: LineTrigCtl is set to DIFF_PAIRED or ISO_PAIRED*
- **`COMPENSATE`**
  - C/C++: `MC_BackwardMotionCancellationMode_COMPENSATE`
  - The backward motion cancellation circuit is enabled and configured for the compensation mode.
  - *Condition: LineRateMode is set to PULSE*
  - *Condition: LineTrigCtl is set to DIFF_PAIRED or ISO_PAIRED*

---

#### ForwardDirection

*Motion direction, determined by the phase relationship of the A and B signals*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10350 << 14` |
| String ID | `ForwardDirection` |
| C/C++ ID | `MC_ForwardDirection` |

**Description:**

The motion direction is determined by the phase relationship of the A and B signals. By construction, the dual-output phase quadrature incremental motion encoder maintains a phase relationship of about 90 degrees between the two signals. For motion in one direction, the A signal leads the B signal by about 90 degrees; for a motion in the other direction, the B signal leads the A signal by about 90 degrees. The direction selector provides the capability to define which one of the phase relationships is considered as the forward direction for the application.

**Values:**

- **`A_LEADS_B`**
  - C/C++: `MC_ForwardDirection_A_LEADS_B`
  - The A signal leads the B signal by about 90 degrees. Default value.
- **`B_LEADS_A`**
  - C/C++: `MC_ForwardDirection_B_LEADS_A`
  - The B signal leads the A signal by about 90 degrees.

---

#### RateDivisionFactor

*Division factor of the line trigger rate divider*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `10553 << 14` |
| String ID | `RateDivisionFactor` |
| C/C++ ID | `MC_RateDivisionFactor` |

**Description:**

The rate divider circuit generates a line trigger signal at a frequency that is an integer fraction 1/N of the frequency of the pulses delivered by the quadrature decoder circuit. For N consecutive incoming pulses issued by the quadrature decoder circuit, the 1/N rate divider:
- 
Generates one output pulse (one line trigger)
- 
Skips N-1 input pulse The rate divider is initialized at the beginning of every MultiCam acquisition sequence. The first output pulse is produced from the first clock input pulse occurring after the sequence trigger event. Notice that:
- 
The output frequency is lower than (N > 1) or equal to (N = 1) the input frequency. It cannot be higher.
- 
The output pulse is generated with a small fixed delay after a non-skipped input pulse. The line trigger pulses are phase-locked to the quadrature decoder output.
- 
The rate divider settings may not be modified while acquisition is in progress.

**Values:**

- `1`: No division Minimum range value. Default value.
- `512`: Divide by 512 Maximum range value. LineTrigLine Designation of line trigger hardware line from outside system

---

#### LineTrigLine

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1334 << 14` |
| String ID | `LineTrigLine` |
| C/C++ ID | `MC_LineTrigLine` |

**Description:**

This parameter designates the terminal sensing the line trigger pulse. The line trigger pulse is processed by the grabber and transferred to the camera as the line reset pulse. Usually, this line trigger signal is generated by a motion encoder. The LineTrigLine parameter designates where the line trigger pulse usually issued by a motion encoder should be applied.

**Values:**

- **`NOM`**
  - C/C++: `MC_LineTrigLine_NOM`
  - t designates the DIN1 line when LineTrigCtl =DIFF, the DIN1 and DIN2 pair of differential lines when LineTrigCtl =DIFF_PAIRED, the IIN1 line when LineTrigCtl =ISO, the IIN1 and IIN2 pair of differential lines when LineTrigCtl =ISO_PAIRED. Default value.
  - *Condition: LineTrigCtl is set to DIFF, DIFF_PAIRED, ISO, or ISO_PAIRED.*
- **`DIN1`**
  - C/C++: `MC_LineTrigLine_DIN1`
  - *Condition: LineTrigCtl is set to DIFF.*
- **`DIN2`**
  - C/C++: `MC_LineTrigLine_DIN2`
  - *Condition: LineTrigCtl is set to DIFF.*
- **`DIN1_DIN2`**
  - C/C++: `MC_LineTrigLine_DIN1_DIN2`
  - The pair of differential input lines DIN1 and DIN2.
  - *Condition: LineTrigCtl is set to DIFF_PAIRED.*
- **`IIN1`**
  - C/C++: `MC_LineTrigLine_IIN1`
  - *Condition: LineTrigCtl is set to ISO.*
- **`IIN2`**
  - C/C++: `MC_LineTrigLine_IIN2`
  - *Condition: LineTrigCtl is set to ISO.*
- **`IIN3`**
  - C/C++: `MC_LineTrigLine_IIN3`
  - *Condition: LineTrigCtl is set to ISO.*
- **`IIN4`**
  - C/C++: `MC_LineTrigLine_IIN4`
  - *Condition: LineTrigCtl is set to ISO.*
- **`IIN1_IIN2`**
  - C/C++: `MC_LineTrigLine_IIN1_IIN2`
  - The pair of differential input lines IIN1 and IIN2.
  - *Condition: LineTrigCtl is set to ISO_PAIRED.*
- **`IIN3_IIN4`**
  - C/C++: `MC_LineTrigLine_IIN3_IIN4`
  - The pair of differential input lines IIN3 and IIN4.
  - *Condition: LineTrigCtl is set to ISO_PAIRED.*

---

#### EncoderTickCount

*Encoder tick counter*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `11189 << 14` |
| String ID | `EncoderTickCount` |
| C/C++ ID | `MC_EncoderTickCount` |

**Description:**

This parameter applies when a motion encoder is used. The encoder tick counter is a 32-bit binary up/down counter that counts encoder ticks delivered by the quadrature decoder. When the quadrature decoder is configured for 2 signals, namely when LineTrigCtl is set to DIFF_PAIRED or ISO_PAIRED, the counter is incremented or decremented according to the detected motion direction. The forward direction is defined by ForwardDirection. When the quadrature decoder is configured for 1 signal, namely when LineTrigCtl is set to DIFF or ISO, the counter is incremented only. The number of ticks per encoder signal(s) cycle can be 1, 2 or 4 according to the value of LineTrigEdge. The counter cannot be disabled. Reading EncoderTickCount reports the current counter value. Setting EncoderTickCount to 0 resets the counter. The counter is automatically reset at channel activation.

**Values:**

  - C/C++: `MC_MIN_INT32`
  - C/C++: `MC_MAX_INT32`

---

#### BMCRestart

*Restart condition of the backward motion cancellation circuit*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11187 << 14` |
| String ID | `BMCRestart` |
| C/C++ ID | `MC_BMCRestart` |

**Description:**

This parameter defines when the backward motion cancellation circuit restarts. On a restart, the backward motion cancellation circuit forgets any motion history.

**Values:**

- **`NEVER`**
  - C/C++: `MC_BMCRestart_NEVER`
  - The backward motion cancellation circuit never restarts. Default value.
- **`START_OF_SCAN`**
  - C/C++: `MC_BMCRestart_START_OF_SCAN`
  - The backward motion cancellation circuit restarts at each start-of-scan i.e. at each page in PAGE acquisition mode and at each sequence in WEB and LONGPAGE acquisition modes.

---

#### RateDividerRestart

*Restart condition of the rate divider*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11046 << 14` |
| String ID | `RateDividerRestart` |
| C/C++ ID | `MC_RateDividerRestart` |

**Description:**

This parameter defines when the rate divider circuit restarts.

**Values:**

- **`NEVER`**
  - C/C++: `MC_RateDividerRestart_NEVER`
  - The rate divider is never reinitialized.
  > **NOTE:** Default value in MultiCam 6.9.6 and older. START_OF_SCAN Base DualBase Full FullXR
  - C/C++: `MC_RateDividerRestart_START_OF_SCAN`
  - The rate divider is reinitialized at each start-of-scan i.e. at each page in PAGE acquisition mode and at each sequence in WEB and LONGPAGE acquisition modes. Default value.

---

#### MaxSpeed

*Maximum operating speed of the line-scan system, expressed in Hertz*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1336 << 14` |
| String ID | `MaxSpeed` |
| C/C++ ID | `MC_MaxSpeed` |

**Description:**

This parameter applies when the motion encoder is in use with rate conversion. The MaxSpeed parameter declares the maximum speed at which the line-scan system will be used, that is the upper limit of the rate converter operating range. The downweb line rate has been chosen as a measurement of the system operating speed. After programming the rate converter, this parameter is automatically set to the highest possible downweb line rate. If desired, the parameter can be set to a lower value to reflect the actual maximum operating speed. When LineCaptureMode = ADR, the highest possible downweb line rate can exceed the highest possible camera rate. When LineCaptureMode = PICK or ALL, the highest possible downweb line rate is determined by the highest possible camera line rate, as indicated by LineRate_Hz . The lower limit of the rate converter operating range is returned by the MinSpeed parameter. The effective upper limit of the rate converter operating range is returned by the MaxSpeedEffective parameter.

**Values:**

- `10`: 10 Hz Minimum range value.
- `100000`: 100,000 Hz (=100 kHz) Maximum range value. MaxSpeedEffective Effective upper limit of the rate converter output frequency, expressed in Hertz.

---

#### MaxSpeedEffective

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `1421 << 14` |
| String ID | `MaxSpeedEffective` |
| C/C++ ID | `MC_MaxSpeedEffective` |

---

#### MinSpeed

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `1337 << 14` |
| String ID | `MinSpeed` |
| C/C++ ID | `MC_MinSpeed` |

**Description:**

This parameter applies when the motion encoder is in use with rate conversion. This parameter returns the lower limit of the line-scan system operating range. The downweb line rate has been chosen as a measurement of the system operating speed. The MinSpeed parameter declares the minimum downweb line rate the converter is able to support. The OnMinSpeed parameter declares the behavior of the rate converter when it reaches the bottom speed limit. The maximum speed at which the line-scan camera will be used has to be previously declared with MaxSpeed , that sets the upper limit of the rate converter operating range.

---

#### OnMinSpeed

*Rate converter behavior below minimum speed limit*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `3374 << 14` |
| String ID | `OnMinSpeed` |
| C/C++ ID | `MC_OnMinSpeed` |

**Description:**

This parameter declares the behavior of the rate converter when it reaches the bottom speed limit of the incoming line trigger rate.

**Values:**

- **`IDLING`**
  - C/C++: `MC_OnMinSpeed_IDLING`
  - The rate converter outputs trigger pulse at a frequency specified by MinSpeed when the incoming line trigger rate is below the input range. Default value.
- **`MUTING`**
  - C/C++: `MC_OnMinSpeed_MUTING`
  - The rate converter does not output trigger pulse when the incoming line trigger rate is below the input range.

---

#### CrossPitch

*Distance between two locations focusing on adjacent pixels on the CCD sensor*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3004 << 14` |
| String ID | `CrossPitch` |
| C/C++ ID | `MC_CrossPitch` |

**Description:**

This parameter states the crossweb resolution. The LinePitch parameter expresses the downweb resolution in arbitrary length units. The "cross pitch" is defined as the crossweb resolution in the same units. This is the distance between two locations focusing on adjacent pixels on the CCD sensor. The ratio of LinePitch and CrossPitch is nothing else than the pixel aspect ratio of the rendered image. Each time the LinePitch parameter is set, the CrossPitch parameter will be set to the same value. This will encourage the 1-to-1 aspect ratio. If the user expects non-square pixels, he will adjust the cross pitch after LinePitch has been set.

---

#### SynchronizedPeriodicGenerator

*Periodic Generator timer synchronization control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Encoder Control |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11224 << 14` |
| String ID | `SynchronizedPeriodicGenerator` |
| C/C++ ID | `MC_SynchronizedPeriodicGenerator` |

**Description:**

This parameter controls the synchronization source of the Periodic Generator timer.

**Usage:**

Prerequisite action(s): Condition: LineRateMode = Period

**Values:**

- **`OFF`**
  - C/C++: `MC_SynchronizedPeriodicGenerator_OFF`
  - The Periodic Generator timer delivers pulses continuously while the channel is in the ACTIVE state. Default value.
- **`PAGETRIGGER`**
  - C/C++: `MC_SynchronizedPeriodicGenerator_PAGETRIGGER`
  - The Periodic Generator timer is synchronized on page triggers. It starts delivering pulses when receiving a page trigger and stops delivering pulses after acquiring the last line of the corresponding page.

---

### 4.11. Pipeline Control Category

*Parameters controlling the line-scan pipeline controller*

#### Pipeline_Control

*Master control switch of the pipeline controller feature*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11067 << 14` |
| String ID | `Pipeline_Control` |
| C/C++ ID | `MC_Pipeline_Control` |

**Description:**

This parameter allows to enable or disable the pipeline controller feature. When enabled, for each object passing in front of the detector, the pipeline controller:
- 
asserts a start-of-scan trigger after Pipeline_StartOfScan_Position encoder ticks,
- 
only when the application requests an ACTIVE output action, asserts a pulse on the selected pipeline output line after Pipeline_Output_Position encoder ticks.

**Usage:**

Relevance condition(s): Condition: Available only when the board class parameter BoardTopology is set to MONO_OPT1, DUO_OPT1 or MONO_DECA_OPT1.

**Values:**

- **`ENABLE`**
  - C/C++: `MC_Pipeline_Control_ENABLE`
  - The pipeline controller is enabled.
- **`DISABLE`**
  - C/C++: `MC_Pipeline_Control_DISABLE`
  - The pipeline controller is disabled. Default value.

---

#### Pipeline_StartOfScan_Position

*Start-of-scan position offset*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `11078 << 14` |
| String ID | `Pipeline_StartOfScan_` |
| C/C++ ID | `Position` |

**Description:**

Position the FOV (Field of View) relative to the trigger sensor position, expressed as an integer number of encoder ticks.

**Usage:**

Relevance condition(s): Condition: Pipeline_Control = ENABLE.

**Values:**

- `0`: Minimum range value.
- `1073741823`: 1,073,741,823 encoder ticks Maximum range value. Pipeline_Output_Position Output pulse position offset

---

#### Pipeline_Output_Position

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Integer collection |
| Access | Set and Get |
| Num ID | `11079 << 14` |
| String ID | `Pipeline_Output_Position` |
| C/C++ ID | `MC_Pipeline_Output_Position` |

**Description:**

Position the output action pulse relative to the trigger sensor position, expressed as an integer number of encoder ticks.

**Usage:**

Relevance condition(s): Condition: Pipeline_Control = ENABLE.

**Values:**

- `0`: Minimum range value.
- `1073741823`: 1,073,741,823 encoder ticks Maximum range value. Pipeline_Output_PulseWidth Pipeline controller output pulse width

---

#### Pipeline_Output_PulseWidth

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

Time duration of the pipeline output action pulse.

**Values:**

  - C/C++: `MC_Pipeline_Output_PulseWidth_100us`
  - 100 microseconds. 200us
  - C/C++: `MC_Pipeline_Output_PulseWidth_200us`
  - 200 microseconds. 500us
  - C/C++: `MC_Pipeline_Output_PulseWidth_500us`
  - 500 microseconds. 1ms
  - C/C++: `MC_Pipeline_Output_PulseWidth_1ms`
  - 1 millisecond. 2ms
  - C/C++: `MC_Pipeline_Output_PulseWidth_2ms`
  - 2 milliseconds. 5ms
  - C/C++: `MC_Pipeline_Output_PulseWidth_5ms`
  - 5 milliseconds. Default value. EncTicks
  - C/C++: `MC_Pipeline_Output_PulseWidth_EncTicks`
  - The output pulse width is determined by the value of the Pipeline_Output_PulseWidth_
- **`EncTicks parameter`**

---

#### Pipeline_Output_PulseWidth_EncTicks

*Pipeline controller output pulse width, expressed in encoder ticks*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Integer collection |
| Access | Set and Get |
| Num ID | `11220 << 14` |
| String ID | `Pipeline_Output_` |
| C/C++ ID | `PulseWidth_EncTicks` |

**Description:**

Time duration of the pipeline output action pulse, expressed in encoder ticks.

**Usage:**

Prerequisite action(s): Condition: Pipeline_Output_PulseWidth = EncTicks

---

#### Pipeline_Output_Action

*Action to execute on the selected pipelined output*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

Defines the action that the pipeline controller must execute when the object has travelled Pipeline_Output_Position encoder ticks since the detector position.

**Usage:**

Directive: For each acquired image scan when Pipeline_Control = ENABLE, the application has to define the action to perform on the pipeline output line according to the result of the image analysis.

**Values:**

- **`ACTIVE`**
  - C/C++: `MC_Pipeline_Output_Action_ACTIVE`
  - Assert a pulse when the object has travelled Pipeline_Output_Position encoder ticks since the detector position.
- **`INACTIVE`**
  - C/C++: `MC_Pipeline_Output_Action_INACTIVE`
  - Don't assert a pulse when the object has travelled Pipeline_Output_Position encoder ticks since the detector position.
- **`NONE`**
  - C/C++: `MC_Pipeline_Output_Action_NONE`
  - Default value.

---

#### Pipeline_Output_Line

*GPIO output line used for pipeline control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

Selects the GPIO output line used by the pipeline controller to execute actions. This is automatically connected to IOUT2 when the pipeline controller is enabled.

**Usage:**

Directive: MultiCam automatically enforces the IOUT2 value when the application sets Pipeline_ Control to ENABLE.

**Values:**

- **`IOUT2`**
  - C/C++: `MC_Pipeline_Output_Line_IOUT2`
  - Isolated Output 2 of the MultiCam Channel.
- **`NONE`**
  - C/C++: `MC_Pipeline_Output_Line_NONE`
  - No GPIO line used for pipeline control output action. Default value.

---

#### Pipeline_Fifo_Overflow

*Count of FIFO overflow errors*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `11073 << 14` |
| String ID | `Pipeline_Fifo_Overflow` |
| C/C++ ID | `MC_Pipeline_Fifo_Overflow` |

**Description:**

Reports the number of FIFO overflow errors encountered by the pipeline controller. An overflow occurs when too many triggers have been received or too many actions have been posted by the application software and the corresponding objects have not yet reached the output.
> **NOTE:** The pipeline controller manages up to 32 objects in the machine pipeline. NOTE This counter is never reset during the lifetime of the acquisition channel.

**Usage:**

Directive: To recover from this error, it is required to terminate the current acquisition sequence and restart a new one.

**Values:**

- `0`: No occurence Minimum range value. Default value. Pipeline_Fifo_Underflow Count of FIFO underflow errors

---

#### Pipeline_Fifo_Underflow

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Pipeline Control |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `11074 << 14` |
| String ID | `Pipeline_Fifo_Underflow` |
| C/C++ ID | `MC_Pipeline_Fifo_Underflow` |

**Description:**

Reports the number of FIFO underflow errors encountered by the pipeline controller. An underflow occurs when an object arrives at the end pipeline and the application software has not yet posted the output action.
> **NOTE:** This counter is never reset during the lifetime of the acquisition channel.

**Usage:**

Directive: To recover from this error, it is required to terminate the current acquisition sequence and restart a new one.

**Values:**

- `0`: No occurence Minimum range value. Default value. 4.12. Grabber Configuration Category Parameters controlling the hardware resources specific to the grabber used by the channel Connector
- `350`: ConnectLoc
- `352`: EqualizationLevel
- `353`: PoCL_Mode
- `355`: ECCO_PLLResetControl
- `357`: ECCO_SkewCompensation
- `359`: FvalMin_Tk
- `361`: LvalMin_Tk
- `362`: PoCL_Status
- `363`: MetadataInsertion
- `365`: MetadataContent
- `366`: MetadataLocation
- `368`: MetadataGPPCInputLine
- `370`: MetadataGPPCLocation
- `371`: MetadataGPPCResetLine
- `372`: MetadataSampleTime
- `373`: Connector Connector used by the channel

---

#### Connector

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `682 << 14` |
| String ID | `Connector` |
| C/C++ ID | `MC_Connector` |

**Description:**

The value of this parameter is entered at the channel creation by means of the Connector argument. The consistency of this parameter should be maintained channel-wide.

**Values:**

  - C/C++: `MC_Connector_A`
  - B
  - C/C++: `MC_Connector_B`
  - M
  - C/C++: `MC_Connector_M`
  - The channel is linked to a Camera Link Medium, Full, or 10-tap configuration camera at both the Camera connector #1 and #2 or to a Camera Link Base configuration camera at the Camera connector #1.

---

#### ConnectLoc

*Connector location on the bracket where the relevant camera is connected*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | SELECT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `694 << 14` |
| String ID | `ConnectLoc` |
| C/C++ ID | `MC_ConnectLoc` |

**Description:**

ConnectLoc is an informational parameter reflecting the argument entered by the application at the channel creation.

**Values:**

- **`UPPER`**
  - C/C++: `MC_ConnectLoc_UPPER`
  - The channel uses a camera connected to the upper bracket connector.
- **`LOWER`**
  - C/C++: `MC_ConnectLoc_LOWER`
  - The channel uses a camera connected to the connector at the lower bracket position.
- **`BOTH`**
  - C/C++: `MC_ConnectLoc_BOTH`
  - The channel uses a camera connected to both (upper and lower) bracket connectors.

---

#### EqualizationLevel

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10667 << 14` |
| String ID | `EqualizationLevel` |
| C/C++ ID | `MC_EqualizationLevel` |

**Description:**

The boards featuring ECCO+ are equipped with cable equalizers on the Camera Link receivers. The equalizers compensate for the attenuation of the highest frequencies of the signal along the data lines of the Camera Link cable. The equalizers provide four selectable equalization levels:
- 
OFF: 0 dB - The equalizers are turned off.
- 
LOW: The equalizers compensate for a cable attenuation of 4 dB at 1 GHz.
- 
MEDIUM: The equalizers compensate for a cable attenuation of 8 dB at 1 GHz.
- 
HIGH: The equalizers compensate for a cable attenuation of 16 dB at 1 GHz. The cable attenuation is proportional to the cable length and to the cable attenuation characteristic. It can be estimated as follows: attenuation [dB] = cable_length [m] x cable_attenuation_characteristic [dB/m]
> **NOTE:** An AWG28 twisted pair, commonly found in Camera Link cable assemblies exhibits a typical cable_attenuation_characteristic of 1.4 dB/m

**Usage:**

Relevance condition(s): Condition: ECCO+ feature is available Directive: Select the equalization level according to the actual cable attenuation. Directive: For AWG28 cables of up to 1 meter, select OFF. Directive: For AWG28 cables of 1 up to 4 meters, select LOW. Directive: For AWG28 cables of 4 up to 8 meters, select MEDIUM. Directive: For AWG28 cables of 8 up to 20 meters, select HIGH.

**Values:**

- **`OFF`**
  - C/C++: `MC_EqualizationLevel_OFF`
  - Equalizers are turned OFF
- **`LOW`**
  - C/C++: `MC_EqualizationLevel_LOW`
  - Equalizers are turned ON with a low gain
- **`MEDIUM`**
  - C/C++: `MC_EqualizationLevel_MEDIUM`
  - Equalizers are turned ON with a medium gain
- **`HIGH`**
  - C/C++: `MC_EqualizationLevel_HIGH`
  - Equalizers are turned ON with a high gain Default value.

---

#### PoCL_Mode

*PoCL control*

**Boards:** Base, DualBase, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `9784 << 14` |
| String ID | `PoCL_Mode` |
| C/C++ ID | `MC_PoCL_Mode` |

**Description:**

Enables/inhibits the PoCL controller to automatically detect a PoCL camera and activate the powering of the camera through the Camera Link cable.
> **NOTE:** Any modification of this parameter is only effective after setting the MultiCam channel in the ready state.

**Usage:**

Directive: Set the ChannelState parameter to value READY after any modification of PoCL_Mode. Directive: To avoid unexpected activation of PoCL when the camera is not powered trough the Camera Link cable, set to OFF.

**Values:**

- **`AUTO`**
  - C/C++: `MC_PoCL_Mode_AUTO`
  - The PoCL controller(s) identifie(s) automatically the type of camera, and configures the camera power distribution accordingly. Default value.
- **`OFF`**
  - C/C++: `MC_PoCL_Mode_OFF`
  - The camera detection is inhibited, and, if power is already applied, the PoCL controller turns off the power.

---

#### ECCO_PLLResetControl

*ECCO PLL reset control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10570 << 14` |
| String ID | `ECCO_PLLResetControl` |
| C/C++ ID | `MC_ECCO_PLLResetControl` |

**Description:**

Selects the method to reset the Phase Locked Loop (PLL) of the ECCO Camera Link receivers owned by the channel.

**Usage:**

Relevance condition(s): Condition: The ECCO feature is used. Namely, when BoardTopology is not equal to MONO_ SLOW nor DUO_SLOW. Directive: Euresys recommends the AUTOMATIC setting.

**Values:**

- **`AUTOMATIC`**
  - C/C++: `MC_ECCO_PLLResetControl_AUTOMATIC`
  - The reset of the PLL is automatically managed by the ECCO circuit. Default value.
- **`CHANNEL_ACTIVATION`**
  - C/C++: `MC_ECCO_PLLResetControl_CHANNEL_ACTIVATION`
  - The reset of the PLL is enforced at every channel activation.
  - *Condition: BoardTopology is not equal to MONO_SLOW nor DUO_SLOW*

---

#### ECCO_SkewCompensation

*ECCO skew compensation control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10568 << 14` |
| String ID | `ECCO_SkewCompensation` |
| C/C++ ID | `MC_ECCO_SkewCompensation` |

**Description:**

Enable/disable the skew compensation function of the ECCO Camera Link receivers owned by the channel.

**Usage:**

Relevance condition(s): Condition: The ECCO feature is used. Namely, when BoardTopology is not equal to MONO_ SLOW nor DUO_SLOW. Directive: Euresys recommends to keep the de-skew function enabled. Disabling the de-skew function should be used for test purpose exclusively.

**Values:**

- **`ECCO_SkewCompensation_ON`**
  - C/C++: `MC_ECCO_SkewCompensation_ECCO_SkewCompensation_ON`
  - The skew compensation function is enabled Default value. ECCO_SkewCompensation_OFF
  - C/C++: `MC_ECCO_SkewCompensation_ECCO_SkewCompensation_OFF`
  - The skew compensation function is disabled
  - *Condition: BoardTopology is not equal to MONO_SLOW nor DUO_SLOW*

---

#### FvalMin_Tk

*Camera Link FVAL digital filter configuration*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `10628 << 14` |
| String ID | `FvalMin_Tk` |
| C/C++ ID | `MC_FvalMin_Tk` |

**Description:**

Configures the digital filter of the Camera Link FVAL input signal owned by the channel.

**Usage:**

Directive: Euresys recommends keeping the filter setting to its default value.

**Values:**

- `1`: MC_FvalMin_Tk_1
  - Does not filter FVAL high pulses; FVAL pulses as narrow as 1 clock period are considered as valid
- `3`: MC_FvalMin_Tk_3
  - Filter out FVAL high pulses narrower than 3 clock periods Default value.

---

#### LvalMin_Tk

*LVAL digital filter configuration*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `10629 << 14` |
| String ID | `LvalMin_Tk` |
| C/C++ ID | `MC_LvalMin_Tk` |

**Description:**

Configures the digital filter of the Camera Link LVAL input signal(s) owned by the channel.

**Usage:**

Directive: Euresys recommends keeping the filter setting to its default value.

**Values:**

- `1`: MC_LvalMin_Tk_1
  - Does not filter LVAL high pulses; LVAL pulses as narrow as 1 clock period are considered as valid
- `2`: MC_LvalMin_Tk_2
  - Filter out LVAL high pulses narrower than 2 clock periods Default value.

---

#### PoCL_Status

*PoCL controller status*

**Boards:** Base, DualBase, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `9941 << 14` |
| String ID | `PoCL_Status` |
| C/C++ ID | `MC_PoCL_Status` |

**Description:**

Reports the status of the PoCL controller.

**Usage:**

Directive: Check the status to troubleshoot inoperative PoCL. Camera presence detection To be detected, the attached camera must deliver a clock signal on Channel_Link_X. Conventional camera detection Possible causes explaining why the detected camera is identified as a non_PoCL camera are: l The camera is not PoCL compliant l The cable is not PoCL compliant PoCL camera detection To be detected as a PoCL camera, the camera and the cable must be PoCL compliant.For dual cable configurations (MEDIUM, FULL, 80-bit), the camera is declared PoCL compliant if at least one of the two PoCL controllers identifies a PoCL-compliant camera/cable combination.

**Values:**

- **`NO_CAMERA`**
  - C/C++: `MC_PoCL_Status_NO_CAMERA`
  - No camera detected.
- **`CONVENTIONAL_CAMERA`**
  - C/C++: `MC_PoCL_Status_CONVENTIONAL_CAMERA`
  - Conventional non-PoCL camera detected. PoCL_CAMERA
  - C/C++: `MC_PoCL_Status_PoCL_CAMERA`
  - PoCL camera detected.

---

#### MetadataInsertion

*Controls metadata insertion into the image*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10842 << 14` |
| String ID | `MetadataInsertion` |
| C/C++ ID | `MC_MetadataInsertion` |

**Description:**

This enumerated parameter controls the insertion of metadata into the image.

**Usage:**

The setting takes effect at the first channel activation.

**Values:**

- **`ENABLE`**
  - C/C++: `MC_MetadataInsertion_ENABLE`
  - Enable insertion of metadata into the image.
- **`DISABLE`**
  - C/C++: `MC_MetadataInsertion_DISABLE`
  - Disable insertion of metadata into the image. Default value.

---

#### MetadataContent

*Reports the metadata content configuration*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `10849 << 14` |
| String ID | `MetadataContent` |
| C/C++ ID | `MC_MetadataContent` |

**Description:**

This enumerated parameter reports the configuration of the metadata content.

**Usage:**

The setting takes effect at the first channel activation.

**Values:**

- **`NONE`**
  - C/C++: `MC_MetadataContent_NONE`
  - There are no metadata content. This occurs when MetadataInsertion = DISABLE or when the camera interface configuration doesn't allow metadata insertion.
- **`ONE_FIELD`**
  - C/C++: `MC_MetadataContent_ONE_FIELD`
  - The metadata content includes one single field: the I/O state.
- **`TWO_FIELD`**
  - C/C++: `MC_MetadataContent_TWO_FIELD`
  - The metadata content includes two fields: the I/O state and the LVAL count.
- **`THREE_FIELD`**
  - C/C++: `MC_MetadataContent_THREE_FIELD`
  - The metadata content includes three fields: I/O state, LVAL count and encoder pulse count.

---

#### MetadataLocation

*Defines metadata location in the Camera Link data stream*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11055 << 14` |
| String ID | `MetadataLocation` |
| C/C++ ID | `MC_MetadataLocation` |

**Description:**

This enumerated parameter controls the location of metadata in the Camera Link data stream.

**Usage:**

Relevance condition(s): Condition: The parameter is relevant only when the insertion of metadata is enabled: MetadataInsertion = ENABLE.

**Values:**

- **`LVALRISE`**
  - C/C++: `MC_MetadataLocation_LVALRISE`
  - The metadata content is located into all taps of the first Camera Link time slot following the rising edge of the Camera Link LVAL signal.
- **`TAP1`**
  - C/C++: `MC_MetadataLocation_TAP1`
  - The metadata content is inserted into the first tap during 10 consecutive Camera Link time slots following the rising edge of the Camera Link LVAL signal.
  - *Condition: Imaging = LINE or TDI: The camera is a line-scan or line-scan TDI camera.*
  - *Condition: TapConfiguration = MEDIUM_4T8: The camera uses an Medium Camera Link*
- **`Condition: TapGeometry = 4X: The camera uses a four X-regions tap geometry.`**
- **`TAP10`**
  - C/C++: `MC_MetadataLocation_TAP10`
  - The metadata content is inserted into the 10th tap during 10 consecutive Camera Link time slots following the rising edge of the Camera Link LVAL signal.
  - *Condition: Imaging = LINE or TDI: The camera is a line-scan or line-scan TDI camera.*
  - *Condition: TapConfiguration = DECA_10T8: The camera uses an 80-bit (10 taps of 8-bit) Camera*
- **`Link configuration.`**
- **`Condition: TapGeometry = 1X10: The camera uses a single X-region tap geometry.`**

---

#### MetadataGPPCInputLine

*GPPC main control*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11211 << 14` |
| String ID | `MetadataGPPCInputLine` |
| C/C++ ID | `MC_MetadataGPPCInputLine` |

**Description:**

This enumerated parameter is the main control of the general purpose pulse counter.

**Usage:**

Directive: Set to IIN1 to enable the general purpose pulse counter.

**Values:**

- **`NONE`**
  - C/C++: `MC_MetadataGPPCInputLine_NONE`
  - The GGPC is disabled. The counter has no input line! Default value.
- **`IIN1`**
  - C/C++: `MC_MetadataGPPCInputLine_IIN1`
  - The GGPC counts the rising edge events applied to the IIN1 isolated input line.

---

#### MetadataGPPCLocation

*GPPC metadata location in the Camera Link data stream*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11213 << 14` |
| String ID | `MetadataGPPCLocation` |
| C/C++ ID | `MC_MetadataGPPCLocation` |

**Values:**

- **`NONE`**
  - C/C++: `MC_MetadataGPPCLocation_NONE`
  - The GPPC metadata is not inserted in the Camera Link data stream.
- **`INSTEAD_LVALCNT`**
  - C/C++: `MC_MetadataGPPCLocation_INSTEAD_LVALCNT`
  - The GPPC metadata replaces the LVAL Count metadata in the Camera Link data stream.
- **`INSTEAD_QCNT`**
  - C/C++: `MC_MetadataGPPCLocation_INSTEAD_QCNT`
  - The GPPC metadata replaces the Q Count metadata in the Camera Link data stream.

---

#### MetadataGPPCResetLine

*GPPC reset line control*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11214 << 14` |
| String ID | `MetadataGPPCResetLine` |
| C/C++ ID | `MC_MetadataGPPCResetLine` |

**Values:**

- **`NONE`**
  - C/C++: `MC_MetadataGPPCResetLine_NONE`
  - The GPPC has no reset input line.
- **`IIN4`**
  - C/C++: `MC_MetadataGPPCResetLine_IIN4`
  - The GPPC resets when a high-level is applied to the IIN4 isolated input line.

---

#### MetadataSampleTime

*Metadata sample time selector*

**Boards:** Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Configuration |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11223 << 14` |
| String ID | `MetadataSampleTime` |
| C/C++ ID | `MC_MetadataSampleTime` |

**Description:**

Defines the metadata sample time.

**Usage:**

Prerequisite action(s): Condition: BoardTopology = MONO_DECA

**Values:**

- **`LVALRISE`**
  - C/C++: `MC_MetadataSampleTime_LVALRISE`
  - Metadata are sampled on each rising edge of LVAL. Default value.
- **`EXPOSURE`**
  - C/C++: `MC_MetadataSampleTime_EXPOSURE`
  - Metadata are sampled on each "start of exposure" event.

---

### 4.13. Grabber Timing Category

*Parameters controlling the hardware resources specific to the grabber used by the channel*

#### GrabWindow

*Method to define the grabbing window area*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `683 << 14` |
| String ID | `GrabWindow` |
| C/C++ ID | `MC_GrabWindow` |

**Description:**

This enumerated parameter selects the method defining the grabbing window area within the camera active area. For area-scan cameras, the grabbing window area is inferred from the camera active rectangular window. For line-scan cameras, the width of the grabbing window area is inferred from the camera active linear window.

**Usage:**

By default, the grabbing window is the largest area achievable by the camera sensor. Alternatively, using the MAN setting, the grabbing window can be reduced to a single rectangular area located anywhere in the camera active area.

**Values:**

- **`MAN`**
  - C/C++: `MC_GrabWindow_MAN`
  - For area-scan cameras, the grabbing window area and location are defined by separate parameters: ● Grabbing window width is defined by WindowX_Px. ● Grabbing window height is defined by WindowY_Ln. ● Grabbing window X-position offset is defined by OffsetX_Px. ● Grabbing window Y-position offset is defined by OffsetY_Ln. For digital line-scan cameras, the grabbing window width and position are defined by separate parameters: ● Grabbing window width is defined by WindowX_Px. ● Grabbing window X-position offset is defined by OffsetX_Px.
  - *Condition: Line-scan or TDI line-scan cameras*
  - *Condition: Area-scan cameras having a single region along the Y direction. For instance, the*

---

#### WindowX_Px

*Width of the grabbing window area*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `826 << 14` |
| String ID | `WindowX_Px` |
| C/C++ ID | `MC_WindowX_Px` |

**Description:**

This integer parameter reflects the width of the grabbing window area, expressed as a number of digitized pixels. The "get" value exactly reflects the actual window width. It may differ from the "set" value established by the user since MultiCam automatically corrects invalid values.

**Usage:**

Relevance condition(s): Condition: Manually defined grabbing window area (GrabWindow = MAN) Prerequisite action(s): Condition: Grabbing window definition method already selected through GrabWindow Directive: Assigning a value smaller than Hactive_Px enables the image cropping feature. Directive: The grabbing window area must be included entirely within the camera active area.

**Values:**

- `8`: 8 pixels Minimum range value. Variable Hactive_Px pixels Maximum range value. WindowY_Ln Height of the grabbing window area

---

#### WindowY_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `827 << 14` |
| String ID | `WindowY_Ln` |
| C/C++ ID | `MC_WindowY_Ln` |

**Description:**

This integer parameter reflects the height of the grabbing window area, expressed as a number of lines. The "get" value exactly reflects the actual window height. It may differ from the "set" value established by the user since MultiCam automatically corrects invalid values. The parameter is available on all MultiCam products supporting area-scan cameras. The parameter can be set when GrabWindow is set to MAN.

**Usage:**

Relevance condition(s): Condition: Manually defined grabbing window area (GrabWindow = MAN) Condition: Area-scan camera (Imaging = AREA) having a single region along the Y direction (TapGeometry ≠*_2YE) Prerequisite action(s): Condition: Grabbing window definition method already selected through GrabWindow Directive: Assigning a value smaller than Vactive_Ln enables the image cropping feature. Directive: The grabbing window area must be included entirely within the camera active area.

**Values:**

- `1`: 1 line Minimum range value. Variable Vactive_Ln lines Maximum range value. OffsetX_Px Horizontal position offset of the grabbing window area in the camera active area

---

#### OffsetX_Px

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `825 << 14` |
| String ID | `OffsetX_Px` |
| C/C++ ID | `MC_OffsetX_Px` |

**Description:**

This integer parameter reflects the horizontal position offset of the center of the grabbing window area relative to the center of the camera active area. The "get" value exactly reflects the shifted amount. It may differ from the "set" value established by the user since MultiCam automatically corrects invalid values.

**Usage:**

Relevance condition(s): Condition: Manually defined grabbing window area (GrabWindow = MAN) Prerequisite action(s): Condition: Grabbing window definition method already selected through GrabWindow Condition: Grabbing window height already set through WindowY_Ln Directive: A value of zero means that the grabbing window area is horizontally centered on the Camera Active Area. Increasing the value shifts the grabbing window area in the right direction. Decreasing the value shifts the grabbing window area in the left direction. Directive: The grabbing window area must be included entirely within the camera active area.

**Values:**

- **`Variable`**
- **`Variable`**
- `0`: The grabbing window area is horizontally centered on the grabbing window area Default value. OffsetY_Ln Vertical position offset of the grabbing window area in the camera active area.

---

#### OffsetY_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `686 << 14` |
| String ID | `OffsetY_Ln` |
| C/C++ ID | `MC_OffsetY_Ln` |

**Description:**

This integer parameter reflects the vertical position offset of the center of the Window Area relative to the center of the camera active area. The "get" value exactly reflects the shifted amount. It may differ from the "set" value established by the user since MultiCam automatically corrects invalid values.

**Usage:**

Relevance condition(s): Condition: Manually defined grabbing window area (GrabWindow = MAN) Condition: Area-scan camera (Imaging = AREA) having a single region along the Y direction (TapGeometry ≠*_2YE) Prerequisite action(s): Condition: Grabbing window definition method already selected through GrabWindow Condition: Grabbing window height already set through WindowY_Ln Directive: Assigning a value of zero means that the grabbing window area is vertically centered on the Camera Active Area. Increasing the value shifts the grabbing window area in the downward direction. Decreasing the value shifts the grabbing window area in the upward direction. Directive: The grabbing window area must be included entirely within the camera active area.

**Values:**

- `0`: The grabbing window area is vertically centered on the grabbing window area. Default value. WindowOrgX_Px X-coordinate of the upper-left corner of the grabbing window area

---

#### WindowOrgX_Px

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `8761 << 14` |
| String ID | `WindowOrgX_Px` |
| C/C++ ID | `MC_WindowOrgX_Px` |

**Description:**

This integer parameter reports the X-coordinate, expressed as a number of pixels, of the upper left corner of the grabbing window area.

**Usage:**

Relevance condition(s): Condition: Manually defined grabbing window area (GrabWindow = MAN)

**Values:**

- `0`: X-coordinate of the first column of the grabbing window area Minimum range value. Variable (Hactive_Px - WindowX_Px) Maximum range value. WindowOrgY_Ln Y-coordinate of the upper-left corner of the grabbing window area

---

#### WindowOrgY_Ln

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Timing |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `8765 << 14` |
| String ID | `WindowOrgY_Ln` |
| C/C++ ID | `MC_WindowOrgY_Ln` |

**Description:**

This integer parameter reports the Y-coordinate, expressed as a number of lines, of the upper left corner of the grabbing window area.

**Usage:**

Relevance condition(s): Condition: Manually defined grabbing window area (GrabWindow = MAN) Condition: Area-scan camera (Imaging = AREA) having a single region along the Y direction (TapGeometry ≠*_2YE)

**Values:**

- `0`: Y-coordinate of the first row of the grabbing window area Minimum range value. Variable (Vactive_Ln - WindowY_Ln) Maximum range value. 4.14. Grabber Conditioning Category Parameters controlling the analog or digital conditioning features applied to the video signal processed by the grabber used by the channel CFD_Mode
- `385`: CFD_Mode Bayer decoding algorithm

---

#### CFD_Mode

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Grabber Conditioning |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `4645 << 14` |
| String ID | `CFD_Mode` |
| C/C++ ID | `MC_CFD_Mode` |

**Values:**

- **`ADVANCED`**
  - C/C++: `MC_CFD_Mode_ADVANCED`
  - Bayer decoding algorithm using a 3x3 interpolation and a median filter.
- **`LEGACY`**
  - C/C++: `MC_CFD_Mode_LEGACY`
  - Bayer decoding algorithm using a 3x3 interpolation identical to eVision Bayer decoding function.

---

### 4.15. White Balance Operator Category

*Parameters controlling the white balance operator used by the channel*

#### WBO_Mode

*Operating mode of the white balance operator*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `4715 << 14` |
| String ID | `WBO_Mode` |
| C/C++ ID | `MC_WBO_Mode` |

**Description:**

This enumerated parameter determines the operating mode of the White Balance Operator within a MultiCam acquisition sequence.

**Usage:**

Relevance condition(s): Condition: The camera is a color camera (Spectrum=COLOR). Condition: The acquisition channel delivers Y and/or RGB pixel data (ColorFormat ≠ BAYER*)

**Values:**

- **`NONE`**
  - C/C++: `MC_WBO_Mode_NONE`
  - When WBO_Mode is set to NONE, the White Balance Operator is disabled; the gain corrections are not applied. Default value.
- **`ONCE`**
  - C/C++: `MC_WBO_Mode_ONCE`
  - When WBO_Mode is set to ONCE, the image color balancing gains are automatically computed during the initial acquisition phase of every MultiCam acquisition sequence within the AWB_ AREA defined by parameters WBO_OrgX, WBO_OrgY, WBO_Width, and WBO_Height. The parameters WBO_GainR, WBO_GainG, and WBO_GainR are automatically set to the respective computed gain values.
  - The White Balance Operator is disabled at the begin of the sequence and remains disabled until the occurrence of the first MC_SIG_SURFACE_PROCESSING signal. The first delivered image is never color balanced; subsequent images remain partially or entirely unbalance until the White Balance Operator is configured.
- **`MANUAL`**
  - C/C++: `MC_WBO_Mode_MANUAL`
  - When WBO_Mode is set to MANUAL, the image color balance is performed with gains specified by parameters WBO_GainR, WBO_GainG and WBO_GainB.

---

#### WBO_GainR

*White balance correction factor for the red color component*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `4717 << 14` |
| String ID | `WBO_GainR` |
| C/C++ ID | `MC_WBO_GainR` |

**Description:**

This integer parameter represents the correction factor applied by the White Balance Operator to the red color component. The parameter values are expressed in 1/1000th. For instance a value of 1234 corresponds to a correction factor of 1.234.

**Usage:**

Relevance condition(s): Condition: Manually defined WBO gains (WBO_Mode = MANUAL) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode

**Values:**

- `1000`: The gain correction factor is 1 Minimum range value. Default value.
- `10000`: The gain correction factor is 10 Maximum range value. WBO_GainG White balance correction factor for the green color component

---

#### WBO_GainG

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `4719 << 14` |
| String ID | `WBO_GainG` |
| C/C++ ID | `MC_WBO_GainG` |

**Description:**

This integer parameter represents the correction factor applied by the White Balance Operator to the green color component. The parameter values are expressed in 1/1000th. For instance a value of 1234 corresponds to a correction factor of 1.234.

**Usage:**

Relevance condition(s): Condition: Manually defined WBO gains (WBO_Mode = MANUAL) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode

**Values:**

- `1000`: The gain correction factor is 1 Minimum range value. Default value.
- `10000`: The gain correction factor is 10 Maximum range value. WBO_GainB White balance correction factor for the blue color component

---

#### WBO_GainB

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `4720 << 14` |
| String ID | `WBO_GainB` |
| C/C++ ID | `MC_WBO_GainB` |

**Description:**

This integer parameter represents the correction factor applied by the White Balance Operator to the blue color component. The parameter values are expressed in 1/1000th. For instance a value of 1234 corresponds to a correction factor of 1.234.

**Usage:**

Relevance condition(s): Condition: Manually defined WBO gains (WBO_Mode = MANUAL) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode

**Values:**

- `1000`: The gain correction factor is 1 Minimum range value. Default value.
- `10000`: The gain correction factor is 10 Maximum range value. WBO_Width Width of the Automatic White Balance Area

---

#### WBO_Width

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `5456 << 14` |
| String ID | `WBO_Width` |
| C/C++ ID | `MC_WBO_Width` |

**Description:**

This integer parameter represents the width, expressed as a number of pixels, of the rectangular region within the camera active area that is used by the Automatic White Balance feature to compute the white balance correction factors.

**Usage:**

Relevance condition(s): Condition: Automatically defined WBO gains (WBO_Mode = ONCE or CONTINUOUS) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode Directive: The AWB_AREA must include at least 256 pixels. Directive: The AWB_AREA must include at least 32 columns of pixels. Directive: The AWB_AREA must be included entirely within the camera active area. Directive: The AWB_AREA must be included entirely within the grabbing window area.

**Values:**

- `32`: 32 pixels Minimum range value. Variable (Hactive_Px - WBO_OrgX) Maximum range value. Default value. WBO_Height Height of the Automatic White Balance Area

---

#### WBO_Height

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `5459 << 14` |
| String ID | `WBO_Height` |
| C/C++ ID | `MC_WBO_Height` |

**Description:**

This integer parameter represents the height, expressed as a number of lines, of the rectangular region within the camera active area that is used by the Automatic White Balance feature to compute the white balance correction factors.

**Usage:**

Relevance condition(s): Condition: Automatically defined WBO gains (WBO_Mode = ONCE or CONTINUOUS) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode Directive: The AWB_AREA must include at least 256 pixels. Directive: The AWB_AREA must include at least 1 line of pixels. Directive: The AWB_AREA must be included entirely within the camera active area. Directive: The AWB_AREA must be included entirely within the grabbing window area.

**Values:**

- `1`: 1 line Minimum range value. Variable (Vactive_Ln - WBO_OrgY) Maximum range value. Default value. WBO_OrgX X-coordinate of the upper-left corner of the AWB_AREA

---

#### WBO_OrgX

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `5449 << 14` |
| String ID | `WBO_OrgX` |
| C/C++ ID | `MC_WBO_OrgX` |

**Description:**

This integer parameter represents the X-coordinate, expressed as a number of pixels, of the upper left corner of arectangular region within the camera active area that is used by the Automatic White Balance feature to compute the white balance correction factors.

**Usage:**

Relevance condition(s): Condition: Automatically defined WBO gains (WBO_Mode = ONCE or CONTINUOUS) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode Directive: The AWB_AREA must include at least 256 pixels. Directive: The AWB_AREA must include at least 32 columns of pixels. Directive: The AWB_AREA must be included entirely within the camera active area. Directive: The AWB_AREA must be included entirely within the grabbing window area.

**Values:**

- `0`: Leftmost column of the grabbing window area Minimum range value. Variable Hactive_Px - 32 Maximum range value. WBO_OrgY Y-coordinate of the upper-left corner of the AWB_AREA

---

#### WBO_OrgY

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `5452 << 14` |
| String ID | `WBO_OrgY` |
| C/C++ ID | `MC_WBO_OrgY` |

**Description:**

This integer parameter represents the Y-coordinate, expressed as a number of lines, of the upper left corner of a rectangular region within the camera active area that is used by the Automatic White Balance feature to compute the white balance correction factors.

**Usage:**

Relevance condition(s): Condition: Automatically defined WBO gains (WBO_Mode = ONCE or CONTINUOUS) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode Directive: The AWB_AREA must include at least 256 pixels. Directive: The AWB_AREA must include at least 1 line of pixels. Directive: The AWB_AREA must be included entirely within the camera active area. Directive: The AWB_AREA must be included entirely within the grabbing window area.

**Values:**

- `0`: Uppermost row of the grabbing window area Minimum range value. Variable (Vactive_Ln - 1) Maximum range value. WBO_Status Status of the automatic white balance learning block

---

#### WBO_Status

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | White Balance Operator |
| Level | EXPERT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `8940 << 14` |
| String ID | `WBO_Status` |
| C/C++ ID | `MC_WBO_Status` |

**Description:**

This enumerated parameter shows the result status of the automatic white balance computation.

**Usage:**

Relevance condition(s): Condition: Automatically defined WBO gains (WBO_Mode = ONCE or CONTINUOUS) Prerequisite action(s): Condition: The WBO operation mode is already selected through WBO_Mode Condition: At least one acquiistion phase already completed.

**Values:**

- **`OK`**
  - C/C++: `MC_WBO_Status_OK`
  - The automatic white balance learning block succeeds to balance the color. The white balance color gain settings are updated.
- **`NOT_OK`**
  - C/C++: `MC_WBO_Status_NOT_OK`
  - The automatic white balance learning block fails to balance the color. The white balance color gain settings are not updated.

---

### 4.16. Look-up Tables Category

*Parameters controlling the look-up-table operator used by the channel*

#### LUT_Method

*LUT construction method*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `4969 << 14` |
| String ID | `LUT_Method` |
| C/C++ ID | `MC_LUT_Method` |

**Description:**

Once the LUT has been defined through any of the construction methods, the values table can not be read back.

**Values:**

- **`RESPONSE_CONTROL`**
  - C/C++: `MC_LUT_Method_RESPONSE_CONTROL`
  - The LUT relevant control parameters are LUT_Contrast , LUT_Visibility , LUT_Brightness and LUT_Negative .
- **`EMPHASIS`**
  - C/C++: `MC_LUT_Method_EMPHASIS`
  - The LUT relevant control parameters are LUT_Emphasis and LUT_Negative .
- **`THRESHOLD`**
  - C/C++: `MC_LUT_Method_THRESHOLD`
  - The LUT relevant control parameters are LUT_SlicingLevel , LUT_SlicingBand , LUT_ LightResponse , LUT_BandResponse and LUT_DarkResponse .
- **`TABLE`**
  - C/C++: `MC_LUT_Method_TABLE`
  - The LUT table is defined through the LUT_Table parameter.

---

#### LUT_StoreIndex

*Index in the board memory of the LUT to store.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `2957 << 14` |
| String ID | `LUT_StoreIndex` |
| C/C++ ID | `MC_LUT_StoreIndex` |

**Description:**

Setting this parameter stores a pre-defined LUT in the board memory. Multiple LUTs can be stored together, and the index defines the LUT place inside the memory.

---

#### LUT_UseIndex

*Index in the board memory of the LUT to activate.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `2956 << 14` |
| String ID | `LUT_UseIndex` |
| C/C++ ID | `MC_LUT_UseIndex` |

**Description:**

Setting this parameter activates immediately the defined LUT stored in the board memory.

---

#### LUT_Contrast

*Contrast factor for a LUT defined through the Response Control method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `2952 << 14` |
| String ID | `LUT_Contrast` |
| C/C++ ID | `MC_LUT_Contrast` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_Brightness

*Brightness factor for a LUT defined through the Response Control method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `2953 << 14` |
| String ID | `LUT_Brightness` |
| C/C++ ID | `MC_LUT_Brightness` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_Visibility

*Visibility factor for a LUT defined through the Response Control method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `2954 << 14` |
| String ID | `LUT_Visibility` |
| C/C++ ID | `MC_LUT_Visibility` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_Negative

*Visibility factor for a LUT defined through the Response Control or the Emphasis method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Enumerated |
| Access | collection |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

**Values:**

---

#### LUT_Emphasis

*Emphasis factor for a LUT defined through the Emphasis method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `4970 << 14` |
| String ID | `LUT_Emphasis` |
| C/C++ ID | `MC_LUT_Emphasis` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_SlicingLevel

*Chooses the level of slicing for a LUT defined through the Threshold method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `4971 << 14` |
| String ID | `LUT_SlicingLevel` |
| C/C++ ID | `MC_LUT_SlicingLevel` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_SlicingBand

*Band width of slicing for a LUT defined through the Threshold method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `4972 << 14` |
| String ID | `LUT_SlicingBand` |
| C/C++ ID | `MC_LUT_SlicingBand` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_LightResponse

*Response in the light part for a LUT defined through the Threshold method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `4973 << 14` |
| String ID | `LUT_LightResponse` |
| C/C++ ID | `MC_LUT_LightResponse` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_BandResponse

*Response in the middle part for a LUT defined through the Threshold method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `4974 << 14` |
| String ID | `LUT_BandResponse` |
| C/C++ ID | `MC_LUT_BandResponse` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_DarkResponse

*Response in the dark part for a LUT defined through the Threshold method.*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Float collection |
| Access | Set and Get |
| Num ID | `4975 << 14` |
| String ID | `LUT_DarkResponse` |
| C/C++ ID | `MC_LUT_DarkResponse` |

**Description:**

If the application is managing monochrome formats, this parameter is a collection of 4 elements with:
- 
Element 0 not relevant.
- 
Element 1 not relevant.
- 
Element 2 not relevant.
- 
Element 3 associated with the image. If the application is managing planar or packed color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 not relevant. If the application is managing combined planar color formats, this parameter is a collection of 4 elements with:
- 
Element 0 associated with the R or red component of the image.
- 
Element 1 associated with the G or green component of the image.
- 
Element 2 associated with the B or blue component of the image.
- 
Element 3 associated with the Y or luminance (gray level) component of the image.

---

#### LUT_InDataWidth

*Digital data width of the LUT input*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3017 << 14` |
| String ID | `LUT_InDataWidth` |
| C/C++ ID | `MC_LUT_InDataWidth` |

**Description:**

Getting this parameter returns the number of significant data bits applied at the input of every LUT transformer.

**Values:**

- `16`: 16 bits LUT_OutDataWidth Digital data width of the LUT output

---

#### LUT_OutDataWidth

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `3018 << 14` |
| String ID | `LUT_OutDataWidth` |
| C/C++ ID | `MC_LUT_OutDataWidth` |

**Description:**

Getting this parameter returns the number of significant data bits delivered by every LUT transformer.

**Values:**

- `16`: 16 bits LUT_Table Manually specifies a LUT defined through the Table method.

---

#### LUT_Table

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Look-up Tables |
| Level | EXPERT |
| Type | Instance |
| Access | Set and Get |
| Num ID | `2951 << 14` |
| String ID | `LUT_Table` |
| C/C++ ID | `MC_LUT_Table` |

**Description:**

Once the LUT has been defined through any method, the values table can not be read back.

---

### 4.17. Board Linkage Category

*Parameters providing several methods to designate one of the frame grabber inside the system as the channel host*

#### BoardName

*Name of the board linked to the channel*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Board Linkage |
| Level | SELECT |
| Type | String |
| Access | Set and Get |
| Num ID | `2 << 14` |
| String ID | `BoardName` |
| C/C++ ID | `MC_BoardName` |

**Description:**

This parameter provides a method to designate a particular board where the channel should find its grabber resources. The designation is based on the name given to a board. The name is a string of maximum 16 ASCII characters.

---

#### DriverIndex

*Board locator in the list returned by the driver*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Board Linkage |
| Level | SELECT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `0 << 14` |
| String ID | `DriverIndex` |
| C/C++ ID | `MC_DriverIndex` |

**Description:**

This parameter provides a method to designate a particular board where the channel should find its grabber resources. The designation is based on the board location in the list returned by the driver. The set of MultiCam compliant boards are assigned a set of consecutive integer numbers starting at 0. The indexing order is system dependent. Setting this parameter links the board having the specified driver index to the channel. Setting the parameter to an index larger than or equal to the number of MultiCam boards results in the MC_NO_BOARD_FOUND error.

---

#### PCIPosition

*Board locator in the list of PCI slots*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Board Linkage |
| Level | SELECT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `1 << 14` |
| String ID | `PCIPosition` |
| C/C++ ID | `MC_PCIPosition` |

**Description:**

This parameter provides a method to designate a particular board where the channel should find its grabber resources. Setting this parameter links the board inserted in the specified PCI slot to the channel The designation is based on the number associated to a PCI slot. This number is assigned by the operating system in a non-predictable way, but remains consistent for a given configuration in a given system. BoardIdentifier Identifier of the board linked to the channel, made by the combination of its type and serial number

---

#### BoardIdentifier

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Board Linkage |
| Level | SELECT |
| Type | String |
| Access | Set and Get |
| Num ID | `3 << 14` |
| String ID | `BoardIdentifier` |
| C/C++ ID | `MC_BoardIdentifier` |

**Description:**

This parameter provides a method to designate a particular board where the channel should find its grabber resources. The designation is based on the board type and its serial number, providing a unique way to designate a Euresys product. The board identifier is an ASCII character string resulting from the concatenation of the board type and the serial number with an intervening underscore. The serial number is a 6-digit string made of characters 0 to 9.

---

### 4.18. Cluster Category

*Parameters defining the destination surface cluster owned by the channel*

#### Cluster

*Set of surfaces associated to a channel*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | SELECT |
| Type | Instance |
| Access | collection |

**Description:**

This parameter gives access to the list of handles of the surfaces belonging to the destination cluster. A cluster is a set of surfaces having compatible characteristics, but different locations. All surfaces belonging to a cluster should be able to accept images coming from the same source through a given channel. The idea behind the clusters is the capability to easily implement advanced destination structures such as double, triple or rotating image buffers. Surface to Cluster Assignment A surface can be assigned to several clusters provided that:
- 
The clusters belong to channels defined within the same application.
- 
The channels address the same board. The maximum number of surfaces assigned to a channel is 4096, and the maximum number of surfaces instantiated within an application is 4096. Currently, the number of surfaces that can be handled by a board may be less than the maximum, depending on the hardware capabilities and characteristics of the acquisition surface.

---

#### ImageSizeX

*Horizontal size of the transferred images, expressed as a number of columns*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Integer |
| Access | Set and Get |
| Num ID | `523 << 14` |
| String ID | `ImageSizeX` |
| C/C++ ID | `MC_ImageSizeX` |

**Description:**

This parameter can be set only with Picolo boards. It exposes the result of any condition adjustment that could affect the image width during the acquisition process. The surface in the destination cluster will receive an image, the width of which is that number of columns. In case of area-scan cameras, the size of the destination surface matches the size of the acquired frame. In case of line-scan cameras, the size of the destination surface matches the size of the acquired page. The horizontal size of the image is scaled to the defined ImageSizeX number of pixels per line.

---

#### ImageSizeY

*Vertical size of the transferred images, expressed as a number of lines*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `524 << 14` |
| String ID | `ImageSizeY` |
| C/C++ ID | `MC_ImageSizeY` |

**Description:**

This parameter can be set only with Picolo boards. It exposes the result of any condition adjustment that could affect the image height during the acquisition process. The surface in the destination cluster will receive an image the height of which is that number of lines. In case of area-scan cameras, the size of the destination surface matches the size of the acquired frame. In case of line-scan cameras, the size of the destination surface matches the size of the acquired page. The vertical size of the image is scaled to the defined ImageSizeY number of lines.

---

#### ImageFlipX

*Horizontal mirroring effect*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1340 << 14` |
| String ID | `ImageFlipX` |
| C/C++ ID | `MC_ImageFlipX` |

**Description:**

The horizontal mirroring effect can be thought as turning the image around a vertical axis (first column becomes last column).

**Values:**

- **`OFF`**
  - C/C++: `MC_ImageFlipX_OFF`
  - No horizontal mirroring effect.
- **`ON`**
  - C/C++: `MC_ImageFlipX_ON`
  - Horizontal mirror applied.

---

#### ImageFlipY

*Vertical mirroring effect*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `525 << 14` |
| String ID | `ImageFlipY` |
| C/C++ ID | `MC_ImageFlipY` |

**Description:**

The vertical mirroring effect can be thought as turning the image around a horizontal axis (first line becomes last line).

**Values:**

- **`OFF`**
  - C/C++: `MC_ImageFlipY_OFF`
  - No vertical mirroring effect.
- **`ON`**
  - C/C++: `MC_ImageFlipY_ON`
  - Vertical mirror applied.

---

#### ColorFormat

*Color format*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `2224 << 14` |
| String ID | `ColorFormat` |
| C/C++ ID | `MC_ColorFormat` |

**Description:**

This parameter summarizes all the properties describing how the frame grabber stores pixel data in the destination surface. For a complete description of pixel storage formats, see "MultiCam Storage Formats" on page 520.

**Values:**

- **`Y8`**
  - C/C++: `MC_ColorFormat_Y8`
  - 8-bit monochrome pixel data; aligned to byte boundaries
- **`Y10`**
  - C/C++: `MC_ColorFormat_Y10`
  - 10-bit monochrome pixel data justified to lsb; 6 padding bits of'0' for alignment to 16-bit
- **`Y10P`**
  - C/C++: `MC_ColorFormat_Y10P`
  - 10-bit monochrome pixel data; no padding bits; packed storage (8 pixels are stored in 10 bytes)
- **`Y12`**
  - C/C++: `MC_ColorFormat_Y12`
  - 12-bit monochrome pixel data justified to lsb; 4 padding bits of'0' for alignment to 16-bit
- **`Y14`**
  - C/C++: `MC_ColorFormat_Y14`
  - 14-bit monochrome pixel data justified to lsb; 2 padding bits of'0' for alignment to 16-bit
- **`Y16`**
  - C/C++: `MC_ColorFormat_Y16`
  - 16-bit monochrome pixel data; aligned to 16-bit boundaries
- **`BAYER8`**
  - C/C++: `MC_ColorFormat_BAYER8`
  - 8-bit BAYER component data; aligned to byte boundaries
- **`BAYER10`**
  - C/C++: `MC_ColorFormat_BAYER10`
  - 10-bit BAYER component data justified to lsb; 6 padding bits of'0' for alignment to 16-bit
- **`BAYER12`**
  - C/C++: `MC_ColorFormat_BAYER12`
  - 12-bit BAYER component data justified to lsb; 4 padding bits of'0' for alignment to 16-bit
- **`BAYER14`**
  - C/C++: `MC_ColorFormat_BAYER14`
  - 14-bit BAYER component data justified to lsb; 2 padding bits of'0' for alignment to 16-bit
- **`BAYER16`**
  - C/C++: `MC_ColorFormat_BAYER16`
  - 16-bit BAYER component data; aligned to 16-bit boundaries
- **`RGB24`**
  - C/C++: `MC_ColorFormat_RGB24`
  - 3x 8-bit packed color components data; each component is aligned to byte boundaries
- **`RGB32`**
  - C/C++: `MC_ColorFormat_RGB32`
  - 4x 8-bit packed color components data; each component is aligned to byte boundaries
- **`ARGB32`**
  - C/C++: `MC_ColorFormat_ARGB32`
  - 4x 8-bit packed color components data; each component is aligned to byte boundaries
- **`RGB30P`**
  - C/C++: `MC_ColorFormat_RGB30P`
  - 3x 10-bit packed color components data; no padding bits; packed storage (8 pixels are stored in 30 bytes)
- **`RGBI40P`**
  - C/C++: `MC_ColorFormat_RGBI40P`
  - 4x 10-bit packed color components data; no padding bits; packed storage (8 pixels are stored in 30 bytes)
- **`RGB24PL`**
  - C/C++: `MC_ColorFormat_RGB24PL`
  - 3 planes of 8-bit color components data; each component is aligned to byte boundaries
  - Each pixel color is stored using RGB24PL system.
- **`RGB30PL`**
  - C/C++: `MC_ColorFormat_RGB30PL`
  - 3 planes of 10-bit color components data; each component is justified to lsb and padded with 6 bits of '0' for alignment to 16-bit
- **`RGB36PL`**
  - C/C++: `MC_ColorFormat_RGB36PL`
  - 3 planes of 12-bit color components data; each component is justified to lsb and padded with 4 bits of '0' for alignment to 16-bit
- **`RGB48PL`**
  - C/C++: `MC_ColorFormat_RGB48PL`
  - Each pixel color is stored using RGB48PL system. In this storage format, the least 6 significant bits of the pixel value are 0.

---

#### RedBlueSwap

*Controls the swapping of the red and blue color components*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `11052 << 14` |
| String ID | `RedBlueSwap` |
| C/C++ ID | `MC_RedBlueSwap` |

**Description:**

This enumerated parameter controls the swapping of the red and blue color components when acquiring color packed image data from RGB color cameras.

**Usage:**

Relevance condition(s): Condition: Parallel RGB camera (delivers 8-bit, 10-bit or 12-bit R, G, and B color components in parallel). Condition: RGB color packed pixel format. Prerequisite action(s): Condition: Spectrum must be set to Color. Condition: ColorMethod must be set RGB. Condition: TapConfiguration must be set to BASE_1T24, MEDIUM_1T30, MEDIUM_1T36, MEDIUM_2T24 or DECA_3T24. Condition: ColorFormat must be set to RGB24 or RGB32.

**Values:**

- **`ENABLE`**
  - C/C++: `MC_RedBlueSwap_ENABLE`
  - The frame grabber swaps the Red and Blue components of the Camera Link RGB pixel data.
  > **NOTE:** This corresponds to the behaviour of MultiCam prior to Release 6.9.8. Default value. DISABLE Base DualBase
  - C/C++: `MC_RedBlueSwap_DISABLE`
  - The frame grabber keeps the pixel component order of the Camera Link RGB pixel data.

---

#### ColorComponentsOrder

*Color components order*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `11054 << 14` |
| String ID | `ColorComponentsOrder` |
| C/C++ ID | `MC_ColorComponentsOrder` |

**Description:**

This enumerated parameter reports the color components order of RGB packed pixel formats.

**Usage:**

Relevance condition(s): Condition: Parallel RGB camera (delivers R, G, and B color components in parallel). Condition: RGB color packed pixel format. Prerequisite action(s): Condition: Spectrum must be set to Color. Condition: ColorMethod must be set RGB. Condition: TapConfiguration must be set to BASE_1T24, MEDIUM_1T30, MEDIUM_1T36, MEDIUM_2T24 or DECA_3T24. Condition: ColorFormat must be set to RGB24 or RGB32.

**Values:**

- **`RGB`**
  - C/C++: `MC_ColorComponentsOrder_RGB`
  - The color components order is RGB.
- **`BGR`**
  - C/C++: `MC_ColorComponentsOrder_BGR`
  - The color components order is BGR.

---

#### ImagePlaneCount

*Number of image planes*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Integer |
| Access | Get Only |
| Num ID | `1718 << 14` |
| String ID | `ImagePlaneCount` |
| C/C++ ID | `MC_ImagePlaneCount` |

**Description:**

MultiCam creates the surfaces and automatically allocates the memory buffers, if not done by the application. The following channel parameters configure the automatic allocation: BufferSize , BufferPitch , ImagePlaneCount and SurfaceCount . MultiCam decides the adequate number of surfaces for the selected acquisition mode. This parameter indicates the number of planes required by the frame grabber to store the pixel data. The channel cannot be activated if all surfaces do not meet this requirement.

**Values:**

- `1`: Single-plane surface
- `3`: Three-plane surface BufferSize Recommended size (in bytes) for the image buffer(s)

---

#### BufferSize

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | ADJUST |
| Type | Integer collection |
| Access | Get Only |
| Num ID | `3333 << 14` |
| String ID | `BufferSize` |
| C/C++ ID | `MC_BufferSize` |

**Description:**

MultiCam creates the surfaces and automatically allocates the memory buffers, if not done by the application. The following channel parameters configure the automatic allocation: BufferSize , BufferPitch , ImagePlaneCount and SurfaceCount . MultiCam decides the adequate number of surfaces for the selected acquisition mode. This parameter is expressed as a number of bytes. It provides the buffer size needed to contain one image produced by the channel. If ImagePlaneCount> 1, the channel produces a "multi-plane" image. In this case, one must allocate ImagePlaneCount buffers. Each buffer size is given in the BufferSize collection members. For instance, if ImagePlaneCount = 3, allocate 3 buffers.
- 
Buffer 1 size is indicated by BufferSize [0].
- 
Buffer 2 size is indicated by BufferSize [1].
- 
Buffer 3 size is indicated by BufferSize [2]. For more information about access to integer collections, refer to Parameters.

---

#### SurfaceIndex

*Index of the next acquisition surface to fill*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `17 << 14` |
| String ID | `SurfaceIndex` |
| C/C++ ID | `MC_SurfaceIndex` |

**Description:**

Getting this parameter gives access to the index of the lastly or currently written surface. This surface is in the FILLING state, as defined by SurfaceState , or got most recently the FILLED state. Setting this parameter allows the selection of a surface to be used by the next acquisition phase. The target surface must be in the FREE state. The value is the zero-based index of the surface in the cluster. This parameter selects the strategy to be exercised by the capture controller.

---

#### SurfaceCount

*Number of surfaces in the channel*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `82 << 14` |
| String ID | `SurfaceCount` |
| C/C++ ID | `MC_SurfaceCount` |

**Description:**

MultiCam creates the surfaces and automatically allocates the memory buffers, if not done by the application. The following channel parameters configure the automatic allocation: BufferSize , BufferPitch , ImagePlaneCount and SurfaceCount . MultiCam decides the adequate number of surfaces for the selected acquisition mode. Getting SurfaceCount indicates the number of surfaces in the channel. The user may change SurfaceCount to another value before channel activation.

---

#### LineIndex

*Index of the written line*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `16 << 14` |
| String ID | `LineIndex` |
| C/C++ ID | `MC_LineIndex` |

**Description:**

This parameter gives access to the index of the line currently written into the FILLING surface, as defined by SurfaceState .

**Values:**

- `0`: Firstly written line Minimum range value. ImageColorRegistration Alignment of Bayer pattern filter over acquired surface

---

#### ImageColorRegistration

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `1274 << 14` |
| String ID | `ImageColorRegistration` |
| C/C++ ID | `MC_ImageColorRegistration` |

**Description:**

This parameter indicates how the Bayer pattern filter covers the image acquired in the destination surface. It applies when Spectrum is COLOR and ColorMethod is BAYER. It is automatically set according to the value of ColorRegistration and according to the setting of the grabbing window. Upper left corner of destination surface

**Values:**

- **`GB`**
  - C/C++: `MC_ImageColorRegistration_GB`
  - The first two pixels are green and blue.
- **`BG`**
  - C/C++: `MC_ImageColorRegistration_BG`
  - The first two pixels are blue and green.
- **`RG`**
  - C/C++: `MC_ImageColorRegistration_RG`
  - The first two pixels are red and green.
- **`GR`**
  - C/C++: `MC_ImageColorRegistration_GR`
  - The first two pixels are green and red.

---

#### SurfacePlaneName

*Image component type stored for each plane*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Enumerated |
| Access | collection |

**Description:**

For a complete description of pixel storage formats, see "MultiCam Storage Formats" on page 520.

**Values:**

- **`UNUSED`**
  - C/C++: `MC_SurfacePlaneName_UNUSED`
  - The plane does not exist. Y
  - C/C++: `MC_SurfacePlaneName_Y`
  - The plane holds the luminance component of the image.
- **`YUV`**
  - C/C++: `MC_SurfacePlaneName_YUV`
  - The plane holds the image in a YUV color packed format. R
  - C/C++: `MC_SurfacePlaneName_R`
  - The plane holds the red component of the image in a color planar format. G
  - C/C++: `MC_SurfacePlaneName_G`
  - The plane holds the green component of the image in a color planar format. B
  - C/C++: `MC_SurfacePlaneName_B`
  - The plane holds the blue component of the image in a color planar format.
- **`RGB`**
  - C/C++: `MC_SurfacePlaneName_RGB`
  - The plane holds the image in a RGB color packed format.
- **`YRGB`**
  - C/C++: `MC_SurfacePlaneName_YRGB`
  - The plane holds the image in a combined luminance and RGB color packed format.

---

#### MinBufferPitch

*Minimum size to contain one line of the image plane, expressed as a number of bytes.*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer collection |
| Access | Get Only |
| Num ID | `3335 << 14` |
| String ID | `MinBufferPitch` |
| C/C++ ID | `MC_MinBufferPitch` |

**Description:**

This parameter indicates the minimal size required to contain one line of the image plane produced by the channel. The channel cannot be activated if all surfaces do not meet this requirement. The line pitch size is defined by parameter BufferPitch . The dimension of this collection parameter is specified by ImagePlaneCount . The assignment of the planes is returned by SurfacePlaneName . For a complete description of pixel storage formats, see "MultiCam Storage Formats" on page 520.

**Values:**

- `4`: 4 bytes Minimum range value.
- `32768`: 32,768 bytes Maximum range value. BufferPitch Size required to contain one line of the plane, expressed in bytes

---

#### BufferPitch

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer collection |
| Access | Set and Get |
| Num ID | `3336 << 14` |
| String ID | `BufferPitch` |
| C/C++ ID | `MC_BufferPitch` |

**Description:**

MultiCam creates the surfaces and automatically allocates the memory buffers, if not done by the application. The following channel parameters configure the automatic allocation: BufferSize , BufferPitch , ImagePlaneCount and SurfaceCount . MultiCam decides the adequate number of surfaces for the selected acquisition mode. Getting this parameter gives the minimum size (in bytes) required to contain one line of the plane produced by the channel. Setting this parameter defines the desired line pitch. If allowed, this value will be used in the computation of other "Cluster Category " on page 421 The minimum value is reported by parameter MinBufferPitch . The dimension of this collection parameter is specified by ImagePlaneCount . The assignment of the planes is returned by SurfacePlaneName . For a complete description of pixel storage formats, see "MultiCam Storage Formats" on page 520.

---

#### MinBufferSize

*Minimal size required to contain the image plane, expressed as a number of bytes.*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer collection |
| Access | Get Only |
| Num ID | `3334 << 14` |
| String ID | `MinBufferSize` |
| C/C++ ID | `MC_MinBufferSize` |

**Description:**

This parameter indicates the absolute minimal buffer size accepted by the channel. If the size of one or more surface buffers is below the corresponding MinBufferSize , the channel will report an error at activation and image acquisition will not be possible. The dimension of this collection parameter is specified by ImagePlaneCount . The assignment of the planes is returned by SurfacePlaneName . For a complete description of pixel storage formats, see "MultiCam Storage Formats" on page 520.

---

#### SurfaceAllocation

*Memory allocation method of MultiCam surfaces*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10092 << 14` |
| String ID | `SurfaceAllocation` |
| C/C++ ID | `MC_SurfaceAllocation` |

**Description:**

MultiCam sets automatically this parameter to the right value, so there should be no need to modify it.

**Values:**

---

#### MaxFillingSurfaces

*Filling surfaces control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `10712 << 14` |
| String ID | `MaxFillingSurfaces` |
| C/C++ ID | `MC_MaxFillingSurfaces` |

**Description:**

This parameter specifies the operation of the cluster mechanism regarding the number of surfaces it is allowed to put in the FILLING state.

**Usage:**

Prerequisite action(s): Condition: The parameter must be set prior to the channel activation, i.e. when ChannelState = IDLE Directive: Allocate a sufficient amount of surfaces and manage the surfaces such that the cluster mechanism maintains a sufficient amount of surfaces in the MC_SurfaceState_FILLING state to cover the largest system interrupt latencies.

**Values:**

- **`MINIMUM`**
  - C/C++: `MC_MaxFillingSurfaces_MINIMUM`
  - The cluster mechanism is allowed to put only one surface in the FILLING state at a time.
- **`MAXIMUM`**
  - C/C++: `MC_MaxFillingSurfaces_MAXIMUM`
  - The cluster mechanism is allowed to put up to 512 surfaces in the FILLING state at a time. Default value.

---

#### FifoOrdering

*Video lines reordering control*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1719 << 14` |
| String ID | `FifoOrdering` |
| C/C++ ID | `MC_FifoOrdering` |

**Description:**

This parameter controls the reordering of video lines in the acquisition buffer. For more information, refer to Video Lines Reordering in the Grablink Functional Guide.

**Usage:**

Directive: For area-scan cameras having a *_2YE TapGeometry value, MultiCam automatically sets the parameter value to DUALYEND. Directive: For other cameras, MultiCam automatically sets the parameter value to PROGRESSIVE. Directive: For multi-color multi-spectral line-scan cameras delivering video data lines by block of N lines, the user may set the parameter value to NYTAP to group lines by color planes.

**Values:**

- **`PROGRESSIVE`**
  - C/C++: `MC_FifoOrdering_PROGRESSIVE`
  - The frame grabber doesn't reorder video lines. This is the default value except for area-scan cameras having a *_2YE TapGeometry value.
- **`DUALYEND`**
  - C/C++: `MC_FifoOrdering_DUALYEND`
  - The frame grabber re-orders the video-lines according to the DUALYEND reordering scheme. This is the default value for area-scan cameras having a *_2YE TapGeometry value.
- **`NYTAP`**
  - C/C++: `MC_FifoOrdering_NYTAP`
  - The frame grabber re-orders the video-lines according to the NYTAP reordering scheme.
- **`PENTAYTAP`**
  - C/C++: `MC_FifoOrdering_PENTAYTAP`
  - The frame grabber re-orders the video-lines according to the PENTAYTAP reordering scheme.
  - This value is kept for backward compatibility.

---

#### FifoOrderingYTapCount

*Number of taps in the Y-direction for video lines reordering*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Cluster |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `11225 << 14` |
| String ID | `FifoOrderingYTapCount` |
| C/C++ ID | `MC_FifoOrderingYTapCount` |

**Description:**

This parameter allows choosing the number of wanted Y taps (planes) when FifoOrdering=NYTAP. For other values of the FifoOrdering parameter, it takes an appropriate default value and changing it has no effect.

**Values:**

- `1`: Single tap Condition: FifoOrdering=NYTAP Minimum range value. Default value. Variable ImageSizeY taps Condition: FifoOrdering=NYTAP Maximum range value.
- `1`: Single tap Condition: FifoOrdering=PROGRESSIVE Default value.
- `2`: Two taps Condition: FifoOrdering=DUALYEND Default value.
- `5`: Five taps Condition: FifoOrdering=PENTAYTAP Default value. 4.19. Channel Management Category Parameters controlling state information of the channel ChannelState
- `454`: CallbackPriority
- `456`: ChannelState State of the channel

---

#### ChannelState

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Channel Management |
| Level | SELECT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `15 << 14` |
| String ID | `ChannelState` |
| C/C++ ID | `MC_ChannelState` |

**Description:**

Refer to "Automatic Switching" on page 503.

**Values:**

- **`IDLE`**
  - C/C++: `MC_ChannelState_IDLE`
  - The channel owns the grabber at this moment but does not lock it.
  - Sets the channel's state to IDLE or READY.
- **`ACTIVE`**
  - C/C++: `MC_ChannelState_ACTIVE`
  - The channel uses the grabber.
- **`ORPHAN`**
  - C/C++: `MC_ChannelState_ORPHAN`
  - The channel has no grabber.
- **`READY`**
  - C/C++: `MC_ChannelState_READY`
  - The channel locks the grabber and is ready to start an acquisition sequence.
- **`FREE`**
  - C/C++: `MC_ChannelState_FREE`
  - Try to set the channel's state to ORPHAN.

---

#### CallbackPriority

*Priority of the callback thread*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Channel Management |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `100 << 14` |
| String ID | `CallbackPriority` |
| C/C++ ID | `MC_CallbackPriority` |

**Description:**

Registering a callback function results into the creation in the application process of a separate thread dedicated to the callback function. This thread is maintained idle until a signal occurs. This parameter can be used to select the priority of this callback thread. Refer to Registration of Callback Function and "Callback Signaling" on page 505 for information on MultiCam callbacks.

**Values:**

- **`LOWEST`**
  - C/C++: `MC_CallbackPriority_LOWEST`
- **`BELOW_NORMAL`**
  - C/C++: `MC_CallbackPriority_BELOW_NORMAL`
- **`NORMAL`**
  - C/C++: `MC_CallbackPriority_NORMAL`
- **`ABOVE_NORMAL`**
  - C/C++: `MC_CallbackPriority_ABOVE_NORMAL`
- **`HIGHEST`**
  - C/C++: `MC_CallbackPriority_HIGHEST`
- **`TIME_CRITICAL`**
  - C/C++: `MC_CallbackPriority_TIME_CRITICAL`

---

### 4.20. Signaling Category

*Parameters controlling signaling information of the channel*

#### SignalEnable

*Selection of callback or waiting signals*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Signaling |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `24 << 14` |
| String ID | `SignalEnable` |
| C/C++ ID | `MC_SignalEnable` |

**Description:**

This collection parameter selects the MultiCam signals able to call a callback function or to trigger a waiting function. For more information, refer to "Enabling Signals" on page 515.

**Values:**

- **`ON`**
  - C/C++: `MC_SignalEnable_ON`
  - The signal is included in the selection.
- **`OFF`**
  - C/C++: `MC_SignalEnable_OFF`
  - The signal is not included in the selection.
- **`AFTER_EAS`**
  - C/C++: `MC_SignalEnable_AFTER_EAS`
  - The signal is disabled until the end of acquisition sequence.

---

#### SignalEvent

*Operating system events associated with a MultiCam signals*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Signaling |
| Level | EXPERT |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `25 << 14` |
| String ID | `SignalEvent` |
| C/C++ ID | `MC_SignalEvent` |

**Description:**

This collection parameter holds operating system handles to event objects that are signaled when MultiCam signals occur.

---

#### SignalHandling

*Signaling method to use when MultiCam signal appears*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Signaling |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `74 << 14` |
| String ID | `SignalHandling` |
| C/C++ ID | `MC_SignalHandling` |

**Description:**

This parameter selects which signaling method is used when a particular MultiCam signal appears. If an application needs to use one method for signals of a particular type and another method for other signals, it must define this parameter for all concerned signals. If only one signaling method is used for all types of signals, this parameter does not have to be set. If the setting of SignalHandling :s is CALLBACK_SIGNALING, each signal of type s will cause the callback function to be called. The MultiCam wait function McWaitSignal called for signal s will not be released upon occurrence of a signal of type s. Likewise, the OS event linked to signal s ( SignalEvent :s) will not be signaled. If the setting of SignalHandling :s is WAITING_SIGNALING, each signal of type s will release the MultiCam wait function McWaitSignal called for signal s. The callback function will not be called upon occurrence of a signal of type s. Likewise, the OS event linked to signal s (  SignalEvent :s) will not be signaled. If the setting of SignalHandling :s is OS_EVENT_SIGNALING, each signal of type s will cause the corresponding OS event (  SignalEvent :s) to be signaled. The callback function will not be called upon occurrence of a signal of type s. Likewise, the MultiCam wait function McWaitSignal called for signal s will not be released.

**Values:**

- **`ANY`**
  - C/C++: `MC_SignalHandling_ANY`
  - No signaling method has been selected.
- **`CALLBACK_SIGNALING`**
  - C/C++: `MC_SignalHandling_CALLBACK_SIGNALING`
  - The callback signaling method is used.
- **`WAITING_SIGNALING`**
  - C/C++: `MC_SignalHandling_WAITING_SIGNALING`
  - The waiting signaling method is used.
- **`OS_EVENT_SIGNALING`**
  - C/C++: `MC_SignalHandling_OS_EVENT_SIGNALING`
  - The OS event signaling method is used.

---

#### GenerateSignal

*Signal generation mode*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Signaling |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `73 << 14` |
| String ID | `GenerateSignal` |
| C/C++ ID | `MC_GenerateSignal` |

**Description:**

This parameter is used to choose between two possible modes of signal generation. By default, each MultiCam event produces a signal (if the corresponding signal is enabled). If a signal cannot be generated when the event occurs, the signal is queued. In the other signal generation mode, the signals are not queued. MultiCam only keeps information about the latest event of each type.

**Values:**

- **`EACH_EVENT`**
  - C/C++: `MC_GenerateSignal_EACH_EVENT`
  - Each MultiCam event produces a signal. If necessary, the signals are queued by MultiCam. Default value.
- **`LATEST_EVENT`**
  - C/C++: `MC_GenerateSignal_LATEST_EVENT`
  - The signals are not queued by MultiCam.

---

### 4.21. Exception Management Category

*Parameters controlling the exception situations encountered by the channel*

#### AcquisitionCleanup

*Filtering of spoiled images*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exception Management |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `3024 << 14` |
| String ID | `AcquisitionCleanup` |
| C/C++ ID | `MC_AcquisitionCleanup` |

**Description:**

Some acquired images may be spoiled, due to FIFO overruns for example. This parameter allows not to transfer these images to the application.

**Values:**

- **`ENABLED`**
  - C/C++: `MC_AcquisitionCleanup_ENABLED`
  - The spoiled images are not signalled. The corresponding surfaces (  SurfaceState ) are immediately set to FREE.
- **`DISABLED`**
  - C/C++: `MC_AcquisitionCleanup_DISABLED`
  - The spoiled images are managed the same way as accurate images.

---

#### AcqTimeout_ms

*Configuration of the acquisition timeout*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exception Management |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `56 << 14` |
| String ID | `Timeout` |
| C/C++ ID | `MC_AcqTimeout_ms` |

**Description:**

This parameter controls the acquisition timeout:
- 
The timeout duration can be configured in steps of 1 millisecond.
- 
The timeout function can be disabled.
> **NOTE:** The string identifier differs from the parameter name for backward compatibility reasons.

**Usage:**

Directive: The parameter must be set prior to the activation of the channel.

**Values:**

- `10`: 10 milliseconds timeout duration Minimum range value.
- `10000`: 10,000 milliseconds (= 10 seconds) timeout duration Default value.
- `1000000`: 1,000,000 milliseconds (= 16 minutes and 40 seconds) timeout duration Maximum range value. MC_INDETERMINATE (-1) Disabled timeout function (= infinite duration) OverrunCount Counter of overrun occurrences

---

#### OverrunCount

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exception Management |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `46 << 14` |
| String ID | `OverrunCount` |
| C/C++ ID | `MC_OverrunCount` |

**Description:**

This parameter returns the number of overrun occurrences since the creation of the channel. It is incremented each time a transfer overrun occurs. It may be initialized at any time by setting its value. An overrun is an exception condition occurring when the data transfer between the frame grabber and the host computer saturates the PCI bus.

---

#### TriggerSkipHold

*Protection method of trigger*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exception Management |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `1309 << 14` |
| String ID | `TriggerSkipHold` |
| C/C++ ID | `MC_TriggerSkipHold` |

**Description:**

When the trigger frequency is faster than allowable, the requested trigger is either skipped or held until the end of the current acquisition phase. When TriggerSkipHold is set to HOLD, only the last trigger is maintained in the queue, even if many triggers appeared.

**Values:**

- **`SKIP`**
  - C/C++: `MC_TriggerSkipHold_SKIP`
  - A trigger event is ignored if occurring while a previous trigger is already being treated.
- **`HOLD`**
  - C/C++: `MC_TriggerSkipHold_HOLD`
  - A trigger event is hold if occurring when a previous trigger is already being treated. The end of the current trigger event will be chained with the "hold" trigger event.

---

#### LineTriggerViolation

*Counter of line trigger violation occurrences*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exception Management |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `57 << 14` |
| String ID | `LineTriggerViolation` |
| C/C++ ID | `MC_LineTriggerViolation` |

**Description:**

Getting this parameter reports the current value of the line trigger violation counter. Setting this parameter sets an initial value for the counter. The counter increments when one of the following events occurs:
- 
Line trigger occurs too quickly. (When the camera and illumination controller is not yet able to start a new cycle.)
- 
An excessive backward motion distance. (Occurs only when BackwardMotionCancellation is set to COMPENSATE.)
- 
An excessive period of time between consecutive line triggers. (Occurs only when LineRateMode is set to CONVERT.)

**Usage:**

Relevance condition(s): Condition: Line-scan cameras.

---

#### FrameTriggerViolation

*Counter of frame trigger violation occurrences*

| Property | Value |
|----------|-------|
| Class | Channel |
| Category | Exception Management |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `58 << 14` |
| String ID | `FrameTriggerViolation` |
| C/C++ ID | `MC_FrameTriggerViolation` |

**Description:**

This parameter increases when frame trigger violations occur. It is not incremented each time a frame trigger violation occurs. At least, it will be incremented once per page (line-scan) or per frame (area-scan). Setting this parameter sets an initial value for the counter. In case of area-scan operation, a frame trigger violation occurs when the frame trigger pulses occur too quickly. In case of line-scan operation, the rule applies to the page trigger pulses.

---

## 5. Surface Class

What Is a Surface? The surface is a container where a 2D image can be stored. In most situations, the surface is a buffer in the host memory. Other types of surfaces may be defined, such as the hardware frame buffer located inside a frame grabber. In the particular case of a line-scan camera, the surface can be used as a circular buffer. This implies that, although the surface is 2D-limited, the incoming data flow is continuous and virtually unlimited. Regarding the acquisition process, the surface is the destination where the grabbed images from the cameras are recorded. The overall goal of the MultiCam driver is to provide flexible channels to route images coming from a camera towards a specified surface. Surface creation The Surface class groups all MultiCam parameters dedicated to the definition of memory buffers for image or data storage. A Surface object is an instance of the Surface class represented by a dedicated set of such parameters that uniquely describe the surface. Several surfaces can exist simultaneously. A process called "surface creation" is applied to define a new surface. A created surface is entirely characterized by a corresponding instance of the Surface class in the MultiCam environment. Surfaces can be deleted by their owning application with an appropriate API function.

### 5.1. Surface Specification Category

*Parameters specifying the static attributes of the surface*

#### SurfaceSize

*Size of the surface for one plane, expressed in bytes*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | ADJUST |
| Type | Integer collection |
| Access | Set and Get |
| Num ID | `27 << 14` |
| String ID | `SurfaceSize` |
| C/C++ ID | `MC_SurfaceSize` |

**Description:**

This parameter should be defined large enough to hold the intended image in the adequate format. For backward compatibility, when it is used as an integer, it gives access to the first plane.

---

#### SurfaceAddr

*Address of the surface for one plane, or list of addresses of the surface planes*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | ADJUST |
| Type | Pointer collection |
| Access | Set and Get |
| Num ID | `28 << 14` |
| String ID | `SurfaceAddr` |
| C/C++ ID | `MC_SurfaceAddr` |

**Description:**

If PlaneCount > 1, this parameter is a collection of the starting addresses for every plane constituting the surface.
> **NOTE:** For backward compatibility, it is still possible to use an integer collection instead of a pointer collection. When it is used as an integer, it gives access to the first plane.

---

#### SurfacePitch

*Pitch of the surfaces, expressed in bytes*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | ADJUST |
| Type | Integer collection |
| Access | Set and Get |
| Num ID | `29 << 14` |
| String ID | `SurfacePitch` |
| C/C++ ID | `MC_SurfacePitch` |

**Description:**

This parameter declares the pitch between vertically adjacent pixels of the surface for one plane. For backward compatibility, when it is used as an integer, it gives access to the first plane.

---

#### PlaneCount

*Number of planes in the surface*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `30 << 14` |
| String ID | `PlaneCount` |
| C/C++ ID | `MC_PlaneCount` |

**Description:**

Usually, the number of planes in the surface is 1. But some image formats, such as planar representation of RGB data, require more than one plane. PlaneCount is changed automatically when the collection parameter SurfaceAddr is set. All planes constituting the surface have a similar structure, but the starting address of each plane is different.

---

#### SurfaceContext

*Placeholder for a pointer-precision user-defined value associated with this surface*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Pointer |
| Access | Set and Get |
| Num ID | `32 << 14` |
| String ID | `SurfaceContext` |
| C/C++ ID | `MC_SurfaceContext` |

**Description:**

This parameter provides a convenient way of declaring a user-defined context associated with a MultiCam surface using a pointer value. This context can be easily retrieved from the surface handle in a callback or waiting function. For backward compatibility, it is still possible to use a 32-bit integer value instead of a pointer.

---

#### SurfaceSizeX

*Horizontal image size in pixels*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `84 << 14` |
| String ID | `SurfaceSizeX` |
| C/C++ ID | `MC_SurfaceSizeX` |

**Description:**

This parameter holds the horizontal image size expressed in pixels. It is deduced from the ImageSizeX of the channel.
> **NOTE:** This parameter access is "Get Only" when the surface belongs to the cluster of surfaces associated with a channel in the ACTIVE state.

---

#### SurfaceSizeY

*Vertical image size in pixels*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Integer |
| Access | Set and Get |
| Num ID | `85 << 14` |
| String ID | `SurfaceSizeY` |
| C/C++ ID | `MC_SurfaceSizeY` |

**Description:**

This parameter holds the vertical image size expressed in pixels. It is deduced from the ImageSizeY of the channel.
> **NOTE:** This parameter access is "Get Only" when the surface belongs to the cluster of surfaces associated with a channel in the ACTIVE state.

---

#### SurfaceColorFormat

*Internal organization of pixels of the surface*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `86 << 14` |
| String ID | `SurfaceColorFormat` |
| C/C++ ID | `MC_SurfaceColorFormat` |

**Description:**

This parameter holds the internal pixel organization. It is deduced from the ColorFormat of the channel.
> **NOTE:** This parameter access is "Get Only" when the surface belongs to the cluster of surfaces associated with a channel in the ACTIVE state.

**Values:**

---

#### SurfaceColorRegistration

*Alignment of the color pattern filter over the camera window*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `93 << 14` |
| String ID | `SurfaceColorRegistration` |
| C/C++ ID | `MC_SurfaceColorRegistration` |

**Description:**

This parameter indicates how the Bayer pattern filter covers the camera active window. It is deduced from the ColorRegistration of the channel.
> **NOTE:** This parameter access is "Get Only" when the surface belongs to the cluster of surfaces associated with a channel in the ACTIVE state.

**Values:**

---

#### SurfaceColorComponentsOrder

*Color components order of RGB packed pixel formats*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Specification |
| Level | EXPERT |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `94 << 14` |
| String ID | `SurfaceColorComponentsOrder` |
| C/C++ ID | `MC_SurfaceColorComponentsOrder` |

**Description:**

This parameter reports the color components order of RGB packed pixel formats. It is deduced from the ColorComponentsOrder of the channel.
> **NOTE:** This parameter access is "Get Only" when the surface belongs to the cluster of surfaces associated with a channel in the ACTIVE state.

**Values:**

---

### 5.2. Surface Dynamics Category

*Parameters specifying the dynamic attributes of the surface*

#### SurfaceState

*State of the surface*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Dynamics |
| Level | ADJUST |
| Type | Enumerated |
| Access | Set and Get |
| Num ID | `31 << 14` |
| String ID | `SurfaceState` |
| C/C++ ID | `MC_SurfaceState` |

**Description:**

Get or set the current state of the surface.

**Values:**

- **`FREE`**
  - C/C++: `MC_SurfaceState_FREE`
  - The surface is able to receive image data from the grabber.
- **`FILLING`**
  - C/C++: `MC_SurfaceState_FILLING`
  - The surface is currently receiving or ready to receive image data from the grabber.
- **`FILLED`**
  - C/C++: `MC_SurfaceState_FILLED`
  - The surface has finished receiving image data from the grabber, and thus is ready for processing
- **`PROCESSING`**
  - C/C++: `MC_SurfaceState_PROCESSING`
  - The surface is being processed by the host processor.
- **`RESERVED`**
  - C/C++: `MC_SurfaceState_RESERVED`
  - The surface is removed from the standard state transition.

---

#### LastInSequence

*Last acquired surface in an acquisition sequence*

**Boards:** Base, DualBase, Full, FullXR

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Dynamics |
| Level | ADJUST |
| Type | Enumerated |
| Access | Get Only |
| Num ID | `92 << 14` |
| String ID | `LastInSequence` |
| C/C++ ID | `MC_LastInSequence` |

**Description:**

This parameter indicates whether a surface is the last one in an acquisition sequence.

**Values:**

- **`TRUE`**
  - C/C++: `MC_LastInSequence_TRUE`
  - The surface is the last one of an acquisition sequence.
- **`FALSE`**
  - C/C++: `MC_LastInSequence_FALSE`
  - The surface is not the last one of an acquisition sequence.

---

#### FillCount

*Number of bytes written by the acquisition*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Dynamics |
| Level | EXPERT |
| Type | Integer collection |
| Access | Get Only |
| Num ID | `43 << 14` |
| String ID | `FillCount` |
| C/C++ ID | `MC_FillCount` |

**Description:**

This parameter holds the number of bytes actually written into the surface by the acquisition in this plane.

---

#### TimeCode

*Internal numbering of surface during acquisition sequence*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Dynamics |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `72 << 14` |
| String ID | `TimeCode` |
| C/C++ ID | `MC_TimeCode` |

**Description:**

The timecode is the order in a sequence. The first acquisition of a sequence is numbered 0, the second 1, and so on. The last acquired surface has the number SeqLength_Ph-1. If an acquisition happens but is not signaled to the application (for example, when no surface is available: cluster unavailable), the timecode is still incremented. The timecode is reset to 0 at each new sequence (channel state -> ACTIVE).

---

#### TimeAnsi

*ANSI time of surface filled event*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Dynamics |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `76 << 14` |
| String ID | `TimeAnsi` |
| C/C++ ID | `MC_TimeAnsi` |

**Description:**

This parameter represents the number of seconds elapsed since midnight (00:00:00), January 1, 1970, coordinated universal time (UTC), according to the system clock when the surface is filled.

---

#### TimeStamp_us

*Time of surface filled event*

| Property | Value |
|----------|-------|
| Class | Surface |
| Category | Surface Dynamics |
| Level | EXPERT |
| Type | Integer |
| Access | Get Only |
| Num ID | `77 << 14` |
| String ID | `TimeStamp_us` |
| C/C++ ID | `MC_TimeStamp_us` |

**Description:**

This parameter represents the number of microseconds elapsed since midnight (00:00:00), January 1, 1970, coordinated universal time (UTC), according to the system clock when the surface is filled. This parameter is a 64-bit integer.
> **NOTE:** For backward compatibility, this parameter may still be a collection of two 32-bit integers; one for the low part and one for the high part.

---

## 6. Annex

### 6.1. MultiCam Acquisition Principles

*Refer to D405EN MultiCam Acquisition Principles PDF document*

### 6.2. TapConfiguration Glossary

*Naming Convention*

A tap configuration is designated by:

<Config>_<TapCount>T<BitDepth>(B<TimeSlots>)

<Config>

Designates the Camera Link configuration as follows:

Camera Link Configuration name

<Config> value

Lite

LITE

Base

BASE

Medium

MEDIUM

Full

FULL

72-bit

DECA

80-bit

DECA

<TapCount>

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

### 6.3. TapGeometry Glossary

*Definitions*

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

## 1. For cameras delivering two or more rows of pixels every camera readout cycle:

<TapGeometryX>_<TapGeometryY>

## 2. For cameras delivering only one row of pixels every camera, e.g. single line line-scan

cameras: <TapGeometryX> TapGeometryX Syntax <TapGeometryX> describes the geometrical organization of the taps along one row of the image. It is built as follows: <XRegions>X(<XTaps>)(<ExtX)>)
- 
<XRegions>: an integer declaring the number of regions encountered across one image row (= the X-direction or the horizontal direction). Possible values are 1, 2, 3, 4, 6, 8, and 10.
- 
<XTaps>: an integer declaring the number of consecutive pixels along one region row that are extracted simultaneously. Possible values are 1, 2, 3, 4, 8, and 10. The field is omitted when <XTaps> is 1.
- 
<ExtX>: a letter declaring the relative location of the pixels extractors across one row of the image. □ This field is omitted when all pixel extractors are at the left of each region. □ Letter E indicates that pixel extractors are at both ends of the image row. □ Letter M indicates that pixel extractors are at middle of the image row. □ Letter R indicates that the pixel extractors are all at the right of each region TapGeometryY Syntax <TapGeometryY> describes the geometrical organization of the taps along one column of the image. It is built as follows: <YRegions>Y(<YTaps>)(<ExtY)>) <YRegions>: an integer declaring the number of regions encountered across vertical direction. Possible values are 1 and 2. <YTaps>: an integer declaring the number of consecutive pixels along one region column that are extracted simultaneously. Possible values are 1 and 2. The field is omitted when YTaps is 1. <ExtY>: a letter declaring the relative location of the pixels extractors across one column of the image. □ This field is omitted when all pixel extractors are at the top of each region. □ Letter E indicates that pixel extractors are at both ends of the image column. TapGeometry Values Examples 1 2

1X_1Y

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

4X designates the tap geometry of a four-tap line-scan camera having 4 regions across the X-

direction.

The pixels are delivered four at a time on four taps. Each region delivers its pixels on a

single-tap using a common scanning scheme beginning with the leftmost pixel and ending

with the rightmost pixel.

### 6.4. I/O Indices Catalog

*I/O indices for input lines*

Base

Index

ConnectorName

InputPinName

IO

IIN1

IO

IIN2

IO

IIN3

IO

IIN4

IO

DIN1

IO

DIN2

CAMERA

LVAL

CAMERA

FVAL

CAMERA

DVAL

CAMERA

SPARE

CAMERA

CK_PRESENT

IO

POWER_5V

IO

POWER_12V

POWERSTATE12V

I/O indices for output lines

Base

Index

ConnectorName

OutputPinName

IO

IOUT1

IO

IOUT2

IO

IOUT3

IO

IOUT4

CAMERA

CC1

CAMERA

CC2

CAMERA

CC3

CAMERA

CC4

BRACKET

LED

NA

I/O indices for input lines

DualBase

Index

ConnectorName

InputPinName

IO_A

IIN1

IO_A

IIN2

IO_A

IIN3

IO_A

IIN4

IO_A

DIN1

IO_A

DIN2

CAMERA_A

LVAL

CAMERA_A

FVAL

CAMERA_A

DVAL

CAMERA_A

SPARE

CAMERA_A

CK_PRESENT

IO_B

IIN1

IO_B

IIN2

IO_B

IIN3

IO_B

IIN4

IO_B

DIN1

IO_B

DIN2

CAMERA_B

LVAL

CAMERA_B

FVAL

CAMERA_B

DVAL

CAMERA_B

SPARE

CAMERA_B

CK_PRESENT

IO_A

POWER_5V

IO_A

POWER_12V

IO_B

POWER_5V

IO_B

POWER_12V

POWERSTATE12V

I/O indices for output lines

DualBase

Index

ConnectorName

OutputPinName

IO_A

IOUT1

IO_A

IOUT2

IO_A

IOUT3

IO_A

IOUT4

CAMERA_A

CC1

CAMERA_A

CC2

CAMERA_A

CC3

CAMERA_A

CC4

IO_B

IOUT1

IO_B

IOUT2

IO_B

IOUT3

IO_B

IOUT4

CAMERA_B

CC1

CAMERA_B

CC2

CAMERA_B

CC3

CAMERA_B

CC4

BRACKET

LED_A

BRACKET

LED_B

NA

I/O indices for input lines

Full

FullXR

Index

ConnectorName

InputPinName

IO

IIN1

IO

IIN2

IO

IIN3

IO

IIN4

IO

DIN1

IO

DIN2

CAMERA

LVAL_X

CAMERA

FVAL_X

CAMERA

DVAL_X

CAMERA

SPARE_X

CAMERA

CK_PRESENT_X

CAMERA

LVAL_Y

CAMERA

FVAL_Y

CAMERA

DVAL_Y

CAMERA

SPARE_Y

CAMERA

CK_PRESENT_Y

CAMERA

LVAL_Z

CAMERA

FVAL_Z

CAMERA

DVAL_Z

CAMERA

SPARE_Z

CAMERA

CK_PRESENT_Z

IO

POWER_5V

IO

POWER_12V

POWERSTATE12V

NOTE

The I/O indices 0 and 22 have no input-related function.

I/O indices for output lines

Full

FullXR

Index

ConnectorName

OutputPinName

IO

IOUT1

IO

IOUT2

IO

IOUT3

IO

IOUT4

CAMERA

CC1

CAMERA

CC2

CAMERA

CC3

CAMERA

CC4

BRACKET

LED

NA

NOTE

The I/O indices 0, 5, 6, and {11 24} have no output-related function.

### 6.5. Automatic Switching

*Refer to the "Automatic Switching" on page 503 section in D402EN-MultiCam User Guide PDF document.*

### 6.6. Board Security Feature

*A security feature is incorporated in all MultiCam-compliant boards.*

The general idea is that the OEM application programmer is able to engrave in the board a

secret proprietary key.

The security key is an 8-bytes string of ASCII characters. Any character is allowed. A null

character acts as the termination character of the key.

The security key is stored in the non-volatile memory of the board and cannot be read back.

There is no way to obtain this security key number back from the board. However, it is possible

to verify that a given board currently holds a security key equal to a given one.

Using this simple mechanism, it is easy to lock an application to a board or to a set of boards.

### 6.7. Callback Signaling

*Callback Signaling Mechanism*

The callback mechanism implies an event driven behavior. The following description uses the

Surface Processing signal as an example of callback generating event.

The Surface Processing signal occurs when a transfer phase terminates. It is issued by a channel

to indicate that the destination memory surface has been filled with an image coming from the

source camera, and that this surface is available for image processing (see SurfaceState).

The image processing task is performed on this event by a special function called the callback

function.

Callback mechanism

The callback function is called by the MultiCam driver, not by the user application. This ensures

that the image-processing task is realized at the ideal instant, exactly when the surface

becomes ready for processing.

MultiCam benefits from several built-in features to ease the implementation of the callback

function.

□

A dedicated thread is created for the callback function execution.

□

The callback function prototype is declared in the MultiCam system C header file.

□

Means are provided to designate the channel and the signal(s) issuing the callback

function calls.

□

The callback function argument provides all relevant information to the user-written

code.

The MultiCam function to register a callback function to a channel is McRegisterCallback.

Callback Signaling Information

Callback Function Prototype

The callback function prototype is declared in the MultiCam system's MultiCam.h header file as

follows:

typedef void (MCAPI *PMCCALLBACK)(PMCSIGNALINFO SignalInfo);

Item

Type

Description

Function

PMCCALLBACK

Callback function

SignalInfo

PMCSIGNALINFO

Argument providing the signal information structure.

The user should define the callback function in the application code in accordance with this

prototype.

The callback function is called by the MultiCam driver when a channel issues a pre-defined

signal.

The pre-defined signal should be enabled with the SignalEnable parameter. It is allowed to

enable several signals.

If more than one enabled signals are issued simultaneously from an object, the callback

function is successively called for each signal occurrence.

When the signal occurs, the callback dedicated thread is released, and the callback function is

automatically invoked. The thread is restored to an idle condition when the callback function is

exited.

The function has a single argument, which is a structure passing information on the signal that

caused the callback function. This structure has the signal information type.

If the callback signaling mechanism is used, the waiting and advanced signaling mechanisms

cannot be used.

Registration of Callback Function

A callback function should be registered to a channel object before use. Only one callback

function per object is supported.

Registering the callback function results into the creation in the application process of a

separate thread dedicated to the callback function. This thread is maintained in a idle state

until a signal occurs. There can be only one dedicated thread per channel object.

A dedicated MultiCam function is provided -for callback registration: McRegisterCallback.

Context

Context is an argument of the callback registration function as well as a member of the signal

information structure available to the callback function.

The user is free to use this item at the registration time to hold any identifying information he

may find useful.

When the callback function is executed, the user gets back the context information as it was

passed to the registration function.

Code Example of Callback Mechanism

The following code uses the callback mechanism to process images grabbed during an

acquisition sequence. One or several surfaces have to be created and assigned to the cluster

owned by the channel. At the end of each acquisition phase, the surface is filled and made

available to the callback function. The Status variable can be used for error checking.

[C]

void MyApplication()

{

//Application level initializing code

MCSTATUS Status = McOpenDriver(NULL);

//Application level initializing code

MCHANDLE MyChannel;

Status = McCreateNm("CHANNEL", &MyChannel);

Status = McSetParamInt(MyChannel, MC_DriverIndex, 0);

Status = McSetParamInt(MyChannel, MC_Connector, MC_Connector_M);

//Assign grabber and camera to channel

//Configure channel including triggering mode

//Assign to channel a destination cluster of surfaces

//Registering the callback function

Status = McRegisterCallback(MyChannel, MyFunction, NULL);

//Activating acquisition sequence

Status = McSetParamInt(MyChannel, MC_ChannelState, MC_ChannelState_ACTIVE);

//Acquisition sequence is now active

//A callback is automatically generated after each acquisition phase

//Deleting the channels

Status = McDelete(MyChannel);

//Disconnecting from driver

Status = McCloseDriver();

}

void MCAPI MyFunction(PMCSIGNALINFO SignalInfo){

//...

//Image processing code

//Image to be processed is available in the destination cluster of surfaces

//...

}

### 6.8. Camera Data Transfer Method

*The DataLink parameter declares the data transfer method of the camera feeding the channel.*

MultiCam supports three data transfer methods:

●

COMPOSITE: The "composite video" cameras deliver the video data as an analog composite

video signal. The signal can be:

□

CVBS including Color, Video, Blanking, and Sync

□

VBS including Video, Blanking, and Sync

●

ANALOG: The "analog industrial" cameras deliver the video data as an analog video signal.

The signal can be:

□

Single lane VBS including Video, Blanking, and Sync

□

Single lane VB including Video, Blanking

□

Three lane analog RGB with Sync on Green

●

CAMERALINK: The "Camera Link" cameras deliver digital video data complying with the

Camera Link standard.

NOTE

There is a 1-to-1 match between the values of DataLink and the Euresys

frame grabber series: COMPOSITE for Picolo series, ANALOG for Domino

series and CAMERALINK for Grablink series.

### 6.9. Camera Imaging Basic Geometry

*The Imaging parameter declares the basic geometry of the camera feeding the channel.*

MultiCam supports three basic geometries:

●

AREA: The area-scan cameras are based on 2D imager(s) and deliver 2D data frames

●

LINE: The non-TDI line-scan cameras are based on 1D imager(s) and deliver 1D data lines

●

TDI: The TDI line-scan cameras are based on 2D imager(s) and deliver 1D data lines

TDI stands for Time Delay Integration. TDI line-scan cameras exhibit an increased sensitivity

since the light integration spans over multiple line periods.

MultiCam distinguishes TDI and non-TDI line-scan cameras since TDI line-scan cameras have

specific requirements for their control. However, both are line-scan cameras and share a

common set of acquisition modes.

### 6.10. Camera Spectral Sensitivity

*The Spectrum parameter declares the spectral sensitivity of the camera feeding the channel.*

MultiCam supports three spectral sensitivities:

●

BW: The black/white cameras are delivering a monochrome video signal built from an imager

having a spectral response covering the visible light spectrum

●

IR: The infrared cameras are delivering a monochrome video signal built from an imager

having a spectral response covering the infra-red light spectrum

●

COLOR: The color cameras are delivering a multi-component video signal built from either a

single imager having Color Filter Arrays or from multiple imagers having different spectral

responses

For the frame grabber point of view, BW and IR are equivalent. The wording "monochrome

cameras" designates both classes of cameras.

The class of color cameras is further divided into several sub-classes. See Color Camera

Specification.

### 6.11. Color Camera Specification

*Camera Color Analysis Method*

The ColorMethod parameter declares the color analysis method of the camera feeding the

channel. MultiCam supports the following color analysis methods:

●

NONE: The "monochrome" cameras have no color analysis method.

●

RGB: The "RGB" cameras deliver the video data as three separate color components

respectively named Red, Green, Blue.

●

BAYER: The "Bayer CFA" cameras deliver the raw video obtained from a Bayer CFA imager.

●

PRISM: The "PRISM " cameras are a sub-class of RGB cameras using a 3-CCD prism assembly

ensuring a perfect registration of all color components of a pixel.

●

TRILINEAR: The "trilinear" cameras are a sub-class of non-TDI line-scan RGB color cameras

using a triple line-array imager and delivering un-registered color components.

Camera Color Pattern Filter Alignment

The ColorRegistration parameter declares the alignment of the color pattern filter of the camera

feeding the channel.

MultiCam supports the following filter alignments for Bayer CFA cameras: GB, BG, RG, GR.

MultiCam supports the following filter alignments for trilinear cameras: RGB, GBR, BRG.

Color Gap

The ColorGap parameter declares the gap between adjacent sensing lines of the trilinear

camera feeding the channel.

This gap is expressed as a number of pixel pitches along the line. It is an unchangeable

geometrical feature of the trilinear sensor.

### 6.12. Channel Creation

*To create a channel, go through the following three steps.*

## 1. Create a channel instance.

## 2. Associate the channel to a board.

## 3. Select the connector.

Channel Instance Creation The channel is created with the McCreate or McCreateNm function. The By-Ident Method McCreate(MC_CHANNEL, &m_Channel); The By-Name Method McCreateNm("CHANNEL", &m_Channel); Maximum number of Channels □ At any time, up to 2048 MultiCam channels can exist in a single process. □ At any time, up to 64 MultiCam channels can exist on a Domino or Grablink board. □ At any time, up to 256 MultiCam channels can exist on a Picolo board. Channel-Board Association The targeted board is identified by one of the 4 channel parameters: DriverIndex , PciPosition, BoardName or BoardIdentifier. Example McSetParamInt(m_Channel, MC_DriverIndex, 0); Connector Selection After associating the channel with a board, it is required to set the Connector channel parameter. NOTE For boards having multiple topologies, it is required to define the BoardTopology before the first channel creation on this board. Example McSetParamInt(m_Channel, MC_Connector, MC_Connector_VID1);

### 6.13. Code Example: How to Gather Board

*Information?*

The following code scans all installed MultiCam-compliant boards, and builds a database

containing their information relative to name, serial number and type.

MC_CONFIGURATION is the C identifier used as a handle to the configuration object. This

object has not to be explicitly instantiated.

MC_BOARD is the C identifier used as a handle to the board object. This object has not to be

explicitly instantiated.

The Status variable can be used for error checking.

[C]

//Defining the database structure type

typedef struct

{  char BoardName[17];

INT32 SerialNumber;

INT32 BoardType;

} MULTICAM_BOARDINFO;

//Variables declaration

MULTICAM_BOARDINFO BoardInfo[10];

INT32 BoardCount;

INT32 i;

MCSTATUS Status;

//Connecting to driver

Status = McOpenDriver(NULL);

//Getting number of boards

Status = McGetParamInt(MC_CONFIGURATION, MC_BoardCount, &BoardCount);

//Scanning across MultiCam boards

for (i=0; i<BoardCount; i++)

{

//Fetching the board name (String MultiCam parameter)

Status = McGetParamStr(

MC_BOARD+i,

MC_BoardName,

BoardInfo[i].BoardName,

17);

//Fetching the board serial number (Integer MultiCam parameter)

Status = McGetParamInt(

MC_BOARD+i,

MC_SerialNumber,

&BoardInfo[i].SerialNumber);

//Fetching the board type (Enumerated MultiCam parameter)

Status = McGetParamInt(

MC_BOARD+i,

MC_BoardType,

&BoardInfo[i].BoardType);

}

//Disconnecting from driver

Status = McCloseDriver();

### 6.14. Enabling Signals

*To designate one or several signals as responsible for signaling operation, the MultiCam system provides an adjust-level parameter called SignalEnable.*

One such parameter exists for the channel class. It has the MultiCam type "enumerated

collection".

Each item of the collection allows for enabling or disabling a specific signal. The value of the

item is ON or OFF.

The set of all ON signals constitute the selection of signals enabling the relevant channel to

perform one of the following:

□

Calling a callback function

□

Releasing a waiting thread

□

Causing a Windows event

To address a specific signal, the by-ident parameter access method is used with the

SignalEnable parameter belonging to the desired channel object. The parameter setting

function McSetParamInt or McSetParamStr is used with a parameter identifier established as

follows:

To reach signal...

Use parameter identifier...

Frame Trigger Violation

MC_SignalEnable + MC_SIG_FRAME_TRIGGER_VIOLATION

Start Exposure

MC_SignalEnable + MC_SIG_START_EXPOSURE

End Exposure

MC_SignalEnable + MC_SIG_END_EXPOSURE

Release (*)

MC_SignalEnable + MC_SIG_RELEASE

Surface Filled

MC_SignalEnable + MC_SIG_SURFACE_FILLED

Surface Processing

MC_SignalEnable + MC_SIG_SURFACE_PROCESSING

Cluster Unavailable

MC_SignalEnable + MC_SIG_CLUSTER_UNAVAILABLE

Acquisition failure

MC_SignalEnable + MC_SIG_ACQUISITION_FAILURE

End of acquisition

MC_SignalEnable + MC_SIG_END_ACQUISITION_SEQUENCE

Start of acquisition

MC_SignalEnable + MC_SIG_START_ACQUISITION_SEQUENCE

End of channel activity

MC_SignalEnable + MC_SIG_END_CHANNEL_ACTIVITY

(*) This signal is generated only with Domino boards.

Example

The following code enables the "Surface Filled" signal with the channel designated by my_

Channel:

Status = McSetParamInt (

my_Channel,

MC_SignalEnable + MC_SIG_SURFACE_FILLED,

MC_SignalEnable_ON

);

The Status variable can be used for error checking.

### 6.15. MultiCam Error Codes

*Error codes returned by MultiCam functions*

Return value

Error identifier

MC_OK

No Error

-1

MC_NO_BOARD_FOUND

No Board Found

-2

MC_BAD_PARAMETER

Bad Parameter

-3

MC_IO_ERROR

I/O Error

-4

MC_INTERNAL_ERROR

Internal Error

-5

MC_NO_MORE_RESOURCES

No More Resources

-6

MC_IN_USE

Object still in use

-7

MC_NOT_SUPPORTED

Operation not supported

-8

MC_DATABASE_ERROR

Parameter database error

-9

MC_OUT_OF_BOUND

Value out of bound

-10

MC_INSTANCE_NOT_FOUND

Object instance not found

-11

MC_INVALID_HANDLE

Invalid Handle

-12

MC_TIMEOUT

Timeout

-13

MC_INVALID_VALUE

Invalid Value

-14

MC_RANGE_ERROR

Value not in range

-15

MC_BAD_HW_CONFIG

Invalid hardware configuration

-16

MC_NO_EVENT

No Event

-17

MC_LICENSE_NOT_GRANTED

License not granted

-18

MC_FATAL_ERROR

Fatal error

-19

MC_HW_EVENT_CONFLICT

Hardware event conflict

-20

MC_FILE_NOT_FOUND

File not found

-21

MC_OVERFLOW

Overflow

-22

MC_INVALID_PARAMETER_SETTING

Parameter inconsistency

-23

MC_PARAMETER_ILLEGAL_ACCESS

Illegal operation

-24

MC_CLUSTER_BUSY

Cluster busy

-25

MC_SERVICE_ERROR

MultiCam service error

-26

MC_INVALID_SURFACE

Invalid surface

### 6.16. Line Rate Modes

*Line Rate Mode expresses how the Downweb Line Rate is determined in a line-scan acquisition system.*

The user specifies the Line Rate Mode by means of MultiCam parameter LineRateMode. Five Line

Rate Modes are identified in MultiCam:

LineRateMode

Description

CAMERA

Camera – The Downweb Line Rate is originated by the camera.

PULSE

Trigger Pulse – The Downweb Line Rate originates from a train of pulses

applied on the line trigger input belonging to the grabber.

CONVERT

Rate Converter – The Downweb Line Rate originates from a train of

pulses applied on the line trigger input and processed by a rate converter

belonging to the grabber.

PERIOD

Periodic – The Downweb Line Rate originates from an internal periodic

generator belonging to the grabber

EXPOSE

Exposure Time – The Downweb Line Rate is identical to the camera line

rate and established by the exposure time settings

LineRateMode = CAMERA

This mode is applicable exclusively for free-run permanent exposure – LxxxxSP – class of line

scan cameras when LineCaptureMode = ALL. The grabber does not perform any sampling in the

downweb direction; the Downweb Line Rate is equal to the camera line rate. The camera line

rate is entirely under control of the camera. Notice that most of the line scan cameras provide

an internal line rate adjustment.

LineRateMode = PULSE

When the speed of motion is varying, the Downweb Line Rate should be slaved to this motion.

To achieve this, a motion encoder is a good solution.

The motion encoder delivers an electrical pulse each time the moving web advances by a

determined amount of length. The continuous motion results in a train of pulses the frequency

of which is proportional to the web speed.

There exists another way to take knowledge of the web speed. In some applications, the motion

is caused by a stepping motor controlled by pulses. The controlling train of pulses is also a

measure of relative motion.

In both cases, the pulses are called line trigger pulses, and their repetition rate is the Line

Trigger Rate. The line trigger pulses are applied to the frame grabber to determine the

Downweb Line Rate .

Each line trigger pulse may result into the generation of one line in the acquired image. This

means that the Downweb Line Rate is equal to the Trigger Rate.

LineRateMode = CONVERT

Alternatively to the "PULSE" mode, for more flexibility, the Line Trigger Rate may be scaled up

or down to match the required Downweb Line Rate. The proportion between the two rates is

freely programmable to any value lower or greater than unity, with high accuracy. This makes

possible to accommodate a variety of mechanical setups, and still maintain a full control over

the downweb resolution. The hardware device responsible for this rate conversion is called the

rate converter. This device is a unique characteristic of Euresys line-scan frame grabbers.

LineRateMode = PERIOD

Other circumstances necessitate the Downweb Line Rate to be hardware-generated by a

programmable timer, called the "periodic generator".

LineRateMode = EXPOSE

Applies to:

Base

DualBase

Full

FullXR

This mode is applicable exclusively for line rate controlled permanent exposure – LxxxxRP –

class of line scan cameras when LineCaptureMode = ALL. The grabber does not perform any

sampling in the downweb direction; the Downweb Line Rate is equal to the camera line rate.

The camera line rate is entirely under control of the grabber through the exposure time settings.

### 6.17. MultiCam Storage Formats

*Refer to D406EN MultiCam Storage Formats PDF document*

### 6.18. MultiCam Tap Geometries

*Refer to:*

●

"TapGeometry Glossary" on page 494 and Supported Tap Geometries topics in D411EN

Grablink Functional Guide PDF document.

●

"TapGeometry" on page 119 topic in Grablink Parameters Reference PDF document.

### 6.19. Using Look-Up Tables

*6.20. CAM Files*

What Is a CamFile?

The CamFile can be seen as a script of MultiCam setting functions that are played when the

CamFile parameter is written to. After the CamFile is played, the channel is ready to operate

according to the parameter settings specified in the file. Generally speaking, it means that the

channel is ready to start an acquisition for a specified camera in a specified fundamental mode.

"Cam" stands for Camera. In the computer file system, the CamFile exhibits the .cam extension.

A CamFile is a readable ASCII file having the following structure:

●

An "CamFile Identification Header" on page 523 (optional)

●

A pair of "CamFile Parameter Assignments " on page 524 for the Camera and CamConfig

parameters (mandatory)

●

A list of "CamFile Parameter Assignments " on page 524 for all relevant MultiCam Channel

parameters (optional)

WARNING

A CamFile exclusively contains Channel parameters!

CamFile Identification Header

The identification header is an optional section that includes MultiCam Studio directives.

Example of a CamFile header

;***************************************************************

; Camera Manufacturer: My Cameras

; Camera Model: ProgressiveFR

; Camera Configuration: Progressive Free-Run Scanning, Analog synchronization

;***************************************************************

The MultiCam Studio CamFile directives have the simple format:

; <DirectiveName>: <DirectiveValue> <EOL>

All values are string of characters terminated by an end of line.

Directive

name

Value meaning

Board

Restricts the visibility of the camera in the camera selection wizard of

MultiCam Studio. When value is Domino, the CamFile is listed only when

the channel is created on a a Domino board.When value is Grablink, the

CamFile is listed only when the channel is created on a a Grablink board.

Other values are simply ignored. If more than one board directive is

present, only the first one is considered

Camera

Manufacturer

Declare the manufacturer name to display in the camera selection wizard of

MultiCam Studio

Camera Model

Declare the camera model name to display in the camera selection wizard

of MultiCam Studio

Revision

Declare the revision number and/or date of the CamFile

CamFile Parameter Assignments

A parameter assignment line has the following format:

<ParameterName> = <ParameterValue> [;<Comment>] <EOL>

where:

## 1. ParameterName is a valid MultiCam Channel parameter name for the targeted board.

## 2. ParameterValue is a valid value for the MultiCam parameter.

## 3. An optional comment can be appended to the assignment; it must be preceded by a semi-

column.

## 4. A valid End-Of-Line: a CR or a pair of CR and LF characters.

Example of parameters assignment lines Camera = ProgressiveFR CamConfig = PxxSA ; Gain=1000 TargetFrameRate_Hz = 0.5; 1 frame every two seconds Example of comment lines ; Camera Specification category ;------------------------------ ; Gain=1000 NOTE Only one parameter assignment per line is allowed. Every line containing a parameter assignment must be terminated by Spaces or Tab characters can be freely inserted anywhere. Empty lines, lines containing only comments, are allowed. WARNING Considering built-in dependencies between MultiCam parameters, it is recommended to assign values to Channel parameters starting from the parent. Practical rules for Cam Files:
- 
Keep the statements order of CamFile templates.
- 
When a parameter statement is added in a CamFile, follow the same order as in the Channel Class section of the Parameters Reference manual. Loading the CamFile The loading of a CamFile into a MultiCam channel is a matter of setting the CamFile parameter of a MultiCam channel to the value of the CamFile name (without the .cam extension) When a CamFile is loaded, it is simply interpreted by the MultiCam driver as a series of "set parameter" function calls. Examples The following lines of code implement possible CamFile parameter assignment to a MultiCam channel defined in a Domino board (depends of the camera). MCSTATUS Status = McSetParamStr(MyChannel, MC_CamFile, "VCC-870A_P15RA"); MCSTATUS Status = McSetParamStr(MyChannel, MC_CamFile, "KP-F3_I60SM"); MCSTATUS Status = McSetParamStr(MyChannel, MC_CamFile, "XC-ES30CE_I50SM_R"); The following lines of code implement possible camera assignment to a MultiCam channel defined in a Grablink board (depends of the camera). MCSTATUS Status = McSetParamStr(MyChannel, MC_CamFile, "4000m_P16RG"); MCSTATUS Status = McSetParamStr(MyChannel, MC_CamFile, "Colibri2048CL_L2048RG"); CamFile libraries CamFile Templates A CamFile template is a Camfile intended to be customized by the MultiCam user willing to interface a particular camera with a Domino or a Grablink board. The MultiCam driver is delivered with a collection of templates. The MultiCam driver installation tool installs the CamFile templates as follows: The CamFile templates applicable to the the Grablink boards are stored in the <InstallDir>\Cameras\_TEMPLATES\Grablink\ directory. Refer to Interfacing Camera Link Cameras for additional information about CamFile templates for Grablink boards. Camera Interface Packages Library A Camera Interface Package is a set of files that contains all the information needed by a MultiCam user to configure a MultiCam channel for a particular camera model. A Camera Interface Package is a ZIP file that includes:
- 
Ready-to-use CamFiles with the exhaustive set of relevant parameters. One for each of the recommended operating modes
- 
A documentation explaining how to use this particular camera model with Euresys frame grabbers When unzipped on the target machine, the CamFiles and the documentation are extracted in the <InstallDir>\Cameras\<Manufacturer>\ folder. The library of Camera Interface Packages contains a large amount of packages for both analog and Camera Link digital camera models. Furthermore, this library is regularly updated with new packages and constantly growing. There are 2 ways to access the library:

## 1. Automatic update with MultiCam Studio

MultiCam Studio provides a convenient way to download and update all the available CamFiles. MultiCam Studio automatically downloads and installs on the MultiCam install directory, from the website, a ZIP file containing the CamFiles and the associated PDF documentation files.

## 2. Free downloads from the Euresys website

The library directory is available online on https://www.euresys.com/Support/Supported- cameras. The directory can be easily browsed using interactive filters. Each entry in the directory provides the following fields: □ Camera manufacturer name □ Camera model name □ Compatible Euresys boards □ Link to the Camera Interface Package ZIP file
