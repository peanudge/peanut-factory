# MultiCam API References

> Source: Euresys MultiCam 6.19.4 (Doc D403EN)

## Contents

1. [About This Document](#1-about-this-document)

**Part I: MultiCam C++ Reference**

2. [Using the MultiCam Libraries with C++](#2-using-the-multicam-libraries-with-c)
3. [C++ Classes](#3-c-classes)
   - [Configuration Class](#31-configuration-class)
   - [Board Class](#32-board-class)
   - [BoardList Class](#33-boardlist-class)
   - [Channel Class](#34-channel-class)
   - [Exception Class](#35-exception-class)
   - [MultiCamObject Class](#36-multicamobject-class)
   - [SignalInfo Class](#37-signalinfo-class)
   - [Surface Class](#38-surface-class)
4. [C++ Functions](#4-c-functions)
5. [C++ Appendix](#5-c-appendix)

**Part II: MultiCam C Reference**

6. [Introduction (C API)](#6-introduction-c-api)
7. [C Functions](#7-c-functions)
   - [Driver Connection](#71-driver-connection) -- `McOpenDriver`, `McCloseDriver`
   - [Instance Management](#72-instance-management) -- `McCreate`, `McCreateNm`, `McDelete`
   - [Parameters Management](#73-parameters-management) -- `McGetParam*`, `McSetParam*`
   - [Signaling](#74-signaling) -- `McGetSignalInfo`, `McRegisterCallback`, `McWaitSignal`
   - [Pixel Format Conversion](#75-pixel-format-conversion) -- `McConvertSurface`
8. [Enumerations](#8-enumerations)
9. [Exceptions Management](#9-exceptions-management)
10. [C Predefined Types](#10-c-predefined-types)

---

## 1. About This Document

### Document Revision History

| Date | Version | Description |
|------|---------|-------------|
| 2019-07-24 | 6.17 | MultiCam 6.17 |
| 2018-08-10 | 6.15.1 | First edition in MadCap Flare |

### Document Changes

**MultiCam 6.17** -- New topics:

- `Surface::Convert` Method
- Pixel Format Conversion (`McConvertSurface`)

---

# Part I: MultiCam C++ Reference

---

## 2. Using the MultiCam Libraries with C++

**Supported Platforms:** Refer to the Release Specification section of the MultiCam Release Notes.

**Requested Files:**

The C++ classes and functions can be used if including the requested header files (`.h`) and linking to the import library file (`.lib`) associated to a dynamic-link library (`.dll`).

- C++ headers (`.h` files) can be found in the `Include` folder, under the installation root.
- Import libraries (`.lib` files) can be found in the folder corresponding to the supported compilers.
- Dynamic link libraries (`.dll` files) can be found in the system folder.

---

## 3. C++ Classes

### 3.1. Configuration Class

The **Configuration** object gives access to all MultiCam parameters dedicated to the control of system-wide features.

The system is a set of Euresys boards installed inside a computer. The Configuration object also addresses any hardware or software element of the computer requesting some degree of control for the MultiCam system operation.

The Configuration object exists in one instance per application. The user is not allowed to create a Configuration object. It is natively made available to the application through the `Config` global variable.

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

---

### 3.2. Board Class

The **Board** object gives access to all MultiCam parameters dedicated to the control of board features. It also addresses the access of I/O lines from an application program, implementing the general-purpose I/O functionality.

The Board object exists in one instance for each Euresys board installed inside a computer. The user is not allowed to create Board objects. They are natively made available to the application through the `Boards` global object.

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

---

### 3.3. BoardList Class

The **BoardList** object gives access to the Board objects representing Euresys frame grabbers in the host computer. There is only one `BoardList` instance per application: the global `Boards` variable. The user is not allowed to create a BoardList object.

| Method | Description |
|--------|-------------|
| `GetBoardByBoardIdentifier` | Finds a reference to a Board object using its board identifier |
| `GetBoardByBoardName` | Finds a reference to a Board object using its board name |
| `GetBoardByDriverIndex` | Finds a reference to a Board object using its index |
| `GetBoardByPciPosition` | Finds a reference to a Board object using its position on the PCI bus |
| `GetCount` | Returns the number of elements in the board list |
| `operator[]` | Finds a reference to a Board object using its index |

#### BoardList::GetBoardByBoardIdentifier

```cpp
Board* BoardList::GetBoardByBoardIdentifier(const char* boardIdentifier);
```

| Parameter | Description |
|-----------|-------------|
| `boardIdentifier` | User allocated character string holding the board identifier |

The board identifier is an ASCII character string resulting from the concatenation of the board type and the serial number with an intervening underscore. The serial number is a 6-digit string made of characters 0 to 9. Example: `"GRABLINK_FULL_000123"`.

#### BoardList::GetBoardByBoardName

```cpp
Board* BoardList::GetBoardByBoardName(const char* boardName);
```

| Parameter | Description |
|-----------|-------------|
| `boardName` | User allocated character string holding the board name (max 16 ASCII characters) |

To give a name to a board, use the `NameBoard` board parameter.

#### BoardList::GetBoardByDriverIndex

```cpp
Board* BoardList::GetBoardByDriverIndex(int driverIndex);
```

| Parameter | Description |
|-----------|-------------|
| `driverIndex` | Index of the board in the board list |

The set of MultiCam compliant boards are assigned a set of consecutive integers, starting at zero. The indexing order is system-dependent.

#### BoardList::GetBoardByPciPosition

```cpp
Board* BoardList::GetBoardByPciPosition(int pciPosition);
```

| Parameter | Description |
|-----------|-------------|
| `pciPosition` | Number representing the PCI position |

The designation is based on a number associated to a PCI slot. This number is assigned by the operating system in a non-predictable way, but remains consistent for a given configuration in a given system.

#### BoardList::GetCount

```cpp
int BoardList::GetCount();
```

Returns the number of elements in the board list.

#### BoardList::operator[]

```cpp
Board* BoardList::operator[](int driverIndex);
```

| Parameter | Description |
|-----------|-------------|
| `driverIndex` | Index of the board in the board list |

---

### 3.4. Channel Class

The **Channel** object represents the path from the camera to the surface, that is the image acquired in the computer memory. It performs the image acquisition.

Usually, the application creates one channel per camera. After creation, the channel is configured by setting relevant parameters with the `GetParam` and `SetParam` methods. Once configured, the channel is activated with the `SetActive` method. It is stopped with the `SetIdle` method.

Optionally and before calling `SetActive`, the channel may be prepared with the `Prepare` method. This ensures that all time-consuming configuration operations are done and that the channel will immediately perform image acquisitions upon activation.

When active, the channel performs image acquisition. It reports its activity by executing callback functions previously registered with `RegisterCallback`. As an alternative, the `WaitForSignal` and `GetSignalInfo` methods may be used to synchronize with the channel activity.

| Method | Description |
|--------|-------------|
| `Channel` | Constructs a Channel object |
| `GetSignalInfo` | Retrieves the information associated with a MultiCam signal |
| `Prepare` | Reduces the Channel activation time by previously setting the creation and configuration parameters |
| `RegisterCallback` | Registers the callback function for a given signal |
| `SetActive` | Activates the channel |
| `SetIdle` | Ends the channel acquisition sequence |
| `UnregisterCallback` | Unregisters the callback function for a given signal |
| `WaitForSignal` | Waits for a MultiCam signal and provides the corresponding SignalInfo object |

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

#### Channel::Channel Constructor

```cpp
Channel::Channel(Board* board, int connector);
Channel::Channel(Board* board, const char* connector);
```

| Parameter | Description |
|-----------|-------------|
| `board` | Reference to the Board object |
| `connector` | Identifier (int) or name (string) of the connector |

#### Channel::GetSignalInfo

Retrieves the information associated with a MultiCam signal. Used in combination with `WaitForSignal`.

```cpp
void Channel::GetSignalInfo(MCSIGNAL signal, SignalInfo& info);
```

| Parameter | Description |
|-----------|-------------|
| `signal` | Identifier of the signal |
| `info` | User allocated SignalInfo object |

#### Channel::RegisterCallback

Registers the callback function for a MultiCam signal. The callback method is executed in a dedicated thread.

```cpp
void Channel::RegisterCallback(
    T* owner,
    void (T::*callbackMethod)(Channel&, SignalInfo&),
    MCSIGNAL signal
);
```

| Parameter | Description |
|-----------|-------------|
| `owner` | Reference to the object that contains the callback method |
| `callbackMethod` | Address of the callback method (`ch`: the Channel that caused the signal; `info`: SignalInfo holding signal information) |
| `signal` | Identifier of the signal |

**Example:**

```cpp
class Foo {
    Channel *pChannel;
public:
    void OnSurfaceProcessing(Channel &channel, SignalInfo &info);
};

void Foo::OnSurfaceProcessing(Channel &channel, SignalInfo &info) {
    cout << "Signal " << info.Signal << endl;
    // ...UpdateImageConfig(*info.Surf, someEImage);
}

BOOL Foo::Init() {
    pChannel->RegisterCallback(this, &OnSurfaceProcessing, MC_SIG_SURFACE_PROCESSING);
}
```

#### Channel::Prepare

Channel creation and configuration time-consuming tasks are best performed after all relevant parameters have been set and before channel activation.

```cpp
void Channel.Prepare();
```

The `Prepare` method is optionally called after channel configuration and before calling `SetActive`. When doing so, all time-consuming tasks (firmware upload, memory allocation, etc.) are performed during the Prepare call. If `Prepare` was not called, or if parameters were changed after calling Prepare, the time-consuming tasks will be performed when calling `SetActive`.

#### Channel::SetActive

Activates the channel. When active, the channel performs image acquisition according to its configuration.

```cpp
void Channel::SetActive();
```

#### Channel::SetIdle

Ends the channel acquisition sequence. When idle, the channel does not acquire images nor responds to acquisition triggers.

```cpp
void Channel::SetIdle();
```

#### Channel::UnregisterCallback

Unregisters the callback function for a given signal.

```cpp
void Channel::UnregisterCallback(MCSIGNAL signal);
```

| Parameter | Description |
|-----------|-------------|
| `signal` | Identifier of the signal |

#### Channel::WaitForSignal

Waits for a MultiCam signal and provides, when the signal is issued, the corresponding SignalInfo object.

```cpp
void Channel::WaitForSignal(MCSIGNAL signal, unsigned int timeout, SignalInfo& info);
```

| Parameter | Description |
|-----------|-------------|
| `signal` | Identifier of the signal |
| `timeout` | Time-out value, expressed in milliseconds (ms) |
| `info` | SignalInfo object corresponding to the signal |

---

### 3.5. Exception Class

MultiCam reports errors through the exception system. The **Exception** object stores information about abnormal conditions.

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

#### Exception::What

Returns the description of the exception.

```cpp
const char* Exception::What();
```

---

### 3.6. MultiCamObject Class

The **MultiCamObject** class is the base class for `Configuration`, `Board`, `Channel`, and `Surface`.

| Method | Description |
|--------|-------------|
| `GetParam` | Returns the value of a MultiCam parameter |
| `SetParam` | Sets the value of a MultiCam parameter |

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

#### MultiCamObject::GetParam

Returns the value of a MultiCam parameter. There are two ways to designate the parameter: **by identifier** (`MCPARAMID`) and **by name** (`const char*`). Independently of the parameter type, its value may be returned as `int`, `unsigned int`, `double`, `Surface*`, `char*`, `long long int`, or `void*`.

**By-identifier overloads:**

```cpp
void MultiCamObject::GetParam(MCPARAMID param, int& value);
void MultiCamObject::GetParam(MCPARAMID param, unsigned int& value);
void MultiCamObject::GetParam(MCPARAMID param, double& value);
void MultiCamObject::GetParam(MCPARAMID param, Surface*& value);
void MultiCamObject::GetParam(MCPARAMID param, char* value, int maxLength);
void MultiCamObject::GetParam(MCPARAMID param, long long int& value);
void MultiCamObject::GetParam(MCPARAMID param, void*& value);
```

**By-name overloads:**

```cpp
void MultiCamObject::GetParam(const char* param, int& value);
void MultiCamObject::GetParam(const char* param, unsigned int& value);
void MultiCamObject::GetParam(const char* param, double& value);
void MultiCamObject::GetParam(const char* param, Surface*& value);
void MultiCamObject::GetParam(const char* param, char* value, int maxLength);
void MultiCamObject::GetParam(const char* param, long long int& value);
void MultiCamObject::GetParam(const char* param, void*& value);
```

| Parameter | Description |
|-----------|-------------|
| `param` | MultiCam parameter identifier (`MCPARAMID`) or name (`const char*`) |
| `value` | Output variable receiving the parameter value |
| `maxLength` | Size of the caller-allocated character string (for `char*` overloads only) |

#### MultiCamObject::SetParam

Sets the value of a MultiCam parameter. There are two ways to designate the parameter: **by identifier** (`MCPARAMID`) and **by name** (`const char*`). Independently of the parameter type, the value may be passed as `int`, `unsigned int`, `double`, `Surface&`, `const char*`, `long long int`, or `void*`.

**By-identifier overloads:**

```cpp
void MultiCamObject::SetParam(MCPARAMID param, int value);
void MultiCamObject::SetParam(MCPARAMID param, unsigned int value);
void MultiCamObject::SetParam(MCPARAMID param, double value);
void MultiCamObject::SetParam(MCPARAMID param, Surface& value);
void MultiCamObject::SetParam(MCPARAMID param, const char* value);
void MultiCamObject::SetParam(MCPARAMID param, long long int value);
void MultiCamObject::SetParam(MCPARAMID param, void* value);
```

**By-name overloads:**

```cpp
void MultiCamObject::SetParam(const char* param, int value);
void MultiCamObject::SetParam(const char* param, unsigned int value);
void MultiCamObject::SetParam(const char* param, double value);
void MultiCamObject::SetParam(const char* param, Surface& value);
void MultiCamObject::SetParam(const char* param, const char* value);
void MultiCamObject::SetParam(const char* param, long long int value);
void MultiCamObject::SetParam(const char* param, void* value);
```

| Parameter | Description |
|-----------|-------------|
| `param` | MultiCam parameter identifier (`MCPARAMID`) or name (`const char*`) |
| `value` | Value to assign to the parameter |

---

### 3.7. SignalInfo Class

The **SignalInfo** object conveys information about a MultiCam signal.

| Property | Type | Description |
|----------|------|-------------|
| `Signal` | `MCSIGNAL` | Represents the MCSIGNAL identifier of a MultiCam signal (read-only) |
| `Surf` | `Surface*` | Refers to the Surface object related to the MultiCam signal (read-only) |

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

---

### 3.8. Surface Class

The **Surface** object represents the images acquired by the channel in the computer memory. There is a direct relation between Surface objects and `EImage...` objects. This relation involves no image buffer copy, the image buffers being shared between Surface and `EImage...` objects.

A Channel acquires images in a group of Surface objects. MultiCam provides a protection mechanism to ensure that no new acquisition occurs in a surface during image processing. This mechanism is automatic: during the "surface processing" callback, the surface is protected and no new acquisition will occur in this surface for the duration of the callback method.

In addition, the `Reserve` and `Free` methods allow controlling manually the surface protection.

| Method | Description |
|--------|-------------|
| `Surface` | Constructs an empty Surface object |
| `Reserve` | Excludes the Surface object from the acquisition process |
| `Free` | Reverts the effect of Reserve |
| `Convert` | Handles software conversion between various pixel formats |

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

#### Surface::Surface Constructor

```cpp
void Surface::Surface();
```

Usually, the surface creation is not mandatory. The channel automatically creates Surface objects for image acquisition. Surface construction is useful when the application requires control on the memory allocation process. In this case, the application configures the relevant Surface parameters through `GetParam`/`SetParam`. Surfaces are passed to the Channel through the `Cluster` parameter.

#### Surface::Reserve

Excludes the Surface object from the acquisition process. When this method is called, new image acquisitions are not allowed to target this surface.

```cpp
void Surface::Reserve();
```

#### Surface::Free

Reverts the effect of `Reserve`. When this method is called, the channel is allowed to acquire new images in the surface.

```cpp
void Surface::Free();
```

#### Surface::Convert

Handles software conversion between various pixel formats. A user-created Surface properly configured and passed as an argument will contain the converted image buffer after calling this method.

```cpp
void Surface::Convert(Surface& convertedSurface);
```

To configure the target Surface, the user must allocate a buffer and set required values to the various Surface parameters: `SurfaceAddr`, `SurfaceSize`, `SurfacePitch`, `SurfaceSizeX`, `SurfaceSizeY`, `SurfaceColorFormat`, and `SurfaceColorComponentsOrder`.

---

## 4. C++ Functions

### Initialize

Establishes the communication of the application process with the MultiCam driver.

```cpp
void Initialize();
```

Before using any MultiCam function, the communication between the application process and the MultiCam driver must be established. This is done by calling the `Initialize` function.

If an application calls `Initialize` several times, it must call `Terminate` the same number of times.

**Do not call `Initialize`:**
- Inside the constructor of a global or static object
- Inside the DLL entry point (`DllMain`)

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

### Terminate

Terminates the communication of the application process with the MultiCam driver.

```cpp
void Terminate();
```

Before terminating the application, the user must terminate the communication. If an application calls `Initialize` several times, it must call `Terminate` the same number of times.

**Do not call `Terminate`:**
- Inside the destructor of a global or static object
- Inside the DLL entry point (`DllMain`)

| Requirement | Value |
|-------------|-------|
| Header | `MultiCamCpp.h` |
| Namespace | `Euresys::MultiCam` |

---

## 5. C++ Appendix

### C++ Predefined Types

| Predefined Type | Description |
|-----------------|-------------|
| `MCPARAMID` | Unsigned 32-bit integer. `typedef UINT32 MCPARAMID;` |
| `MCSIGNAL` | Signed 32-bit integer. `typedef int MCSTATUS;` |

---

# Part II: MultiCam C Reference

---

## 6. Introduction (C API)

### Using the MultiCam Libraries with C

**Supported Platforms:** Refer to the Release Specification section of the MultiCam Release Notes.

**Requested Files:**

- C header: `MultiCam.h` file can be found in the `Include` folder, under the installation root.
- Import library: `MultiCam.lib` file can be found in the folder corresponding to the supported compilers.

### Creating and Deleting a MultiCam Object

A MultiCam object (an instance of a MultiCam class) is designated by a handle of the MultiCam-defined type **`MCHANDLE`**. This type is defined in the `MultiCam.h` header. Most MultiCam functions use such a handle as an argument.

Class instances can be created in two different ways: "by handle" or "by name", with the API functions `McCreate` or `McCreateNm`.

When an object is no more needed by the application, it is advisable to release the resources assigned to it with `McDelete`.

### Accessing a Parameter

All parameters are consistently accessed by a unique set of get/set functions. According to the type of the parameter, and to the by-identifier or by-name access way, several versions of these get and set functions are available.

| Parameter Type | Set (by-identifier) | Set (by-name) | Get (by-identifier) | Get (by-name) |
|---------------|---------------------|---------------|---------------------|---------------|
| Integer | `McSetParamInt` | `McSetParamNmInt` | `McGetParamInt` | `McGetParamNmInt` |
| 64-bit integer | `McSetParamInt64` | `McSetParamNmInt64` | `McGetParamInt64` | `McGetParamNmInt64` |
| Floating point | `McSetParamFloat` | `McSetParamNmFloat` | `McGetParamFloat` | `McGetParamNmFloat` |
| String | `McSetParamStr` | `McSetParamNmStr` | `McGetParamStr` | `McGetParamNmStr` |
| Instance | `McSetParamInst` | `McSetParamNmInst` | `McGetParamInst` | `McGetParamNmInst` |
| Pointer | `McSetParamPtr` | `McSetParamNmPtr` | `McGetParamPtr` | `McGetParamNmPtr` |
| Enumerated (by-id) | `McSetParamInt` | `McSetParamNmInt` | `McGetParamInt` | `McGetParamNmInt` |
| Enumerated (by-name) | `McSetParamStr` | `McSetParamNmStr` | `McGetParamStr` | `McGetParamNmStr` |

### Accessing a Collection Element

Some MultiCam parameters exist as a **collection**. This applies to any parameter type.

**By Identifier:** The first element of the collection is referred to with the identifier just like a non-collection parameter. The following elements are referred to with consecutive integer values starting from the identifier value.

**By Name:** The name allowing access to the collection element is the concatenation of: parameter name + colon (`:`) + zero-based index.

| Collection Element | By-identifier Access | By-name Access |
|-------------------|---------------------|---------------|
| First element | `MC_Cluster` | `Cluster` |
| First element (alt) | `MC_Cluster + 0` | `Cluster:0` |
| Second element | `MC_Cluster + 1` | `Cluster:1` |
| Third element | `MC_Cluster + 2` | `Cluster:2` |

---

## 7. C Functions

A MultiCam function is a software item complying to the standard C language syntax. The set of all MultiCam functions constitutes the **MultiCam API** (Application Programming Interface).

The functions are organized in five categories:

- **Driver connection** functions: enable or disable the driver communication
- **Instance management** functions: create or remove object instances
- **Parameters management** functions: set and get MultiCam parameters value
- **Signaling** functions: manage the event flow associated to the acquisition
- **Pixel Format Conversion** functions: handle software conversion between various pixel formats

---

### 7.1. Driver Connection

#### McCloseDriver

Terminates the communication of the application process with the MultiCam driver.

```c
MCSTATUS McCloseDriver();
```

| Item | Details |
|------|---------|
| **Parameters** | None |
| **Return** | `MCSTATUS` -- `MC_OK` (0) on success, negative value on error |

**Remarks:**
- If an application successfully calls `McOpenDriver` several times, it must call `McCloseDriver` the same number of times.
- Do not call `McCloseDriver` inside the destructor of a global or static object, or inside `DllMain`.

---

#### McOpenDriver

Establishes the communication of the application process with the MultiCam driver.

```c
MCSTATUS McOpenDriver(PCCHAR MultiCamName);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `MultiCamName` | `PCCHAR` | Reserved for future functionality. Specify a `NULL` pointer. |

| Return | Description |
|--------|-------------|
| `MC_OK` | Success |
| `MC_SERVICE_ERROR` | MultiCam service is not started |
| Other negative | Error |

**Remarks:**
- If an application successfully calls `McOpenDriver` several times, it must call `McCloseDriver` the same number of times.
- On Windows, MultiCam relies on a service named "MultiCam Service" that starts automatically at boot. `McOpenDriver` will return `MC_SERVICE_ERROR` if the service is not started.
- To check when the service is started, call `McOpenDriver` in a loop until the function returns `MC_OK`.
- Do not call `McOpenDriver` inside the constructor of a global or static object, or inside `DllMain`.

---

### 7.2. Instance Management

#### McCreate

Creates an instance for a MultiCam object according to a model referred to by its handle.

```c
MCSTATUS McCreate(MCHANDLE Model, PMCHANDLE Instance);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Model` | `MCHANDLE` | When creating a Channel, use a handle designating the connector structure. When creating a Surface, use `MC_DEFAULT_SURFACE_HANDLE`. |
| `Instance` | `PMCHANDLE` | Pointer to the newly created instance |

| Return | Description |
|--------|-------------|
| `MC_OK` | Success |

---

#### McCreateNm

Creates an instance for a MultiCam object according to a model referred to by its name.

```c
MCSTATUS McCreateNm(PCHAR ModelName, PMCHANDLE Instance);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `ModelName` | `PCHAR` | When creating a Channel, use a string representing the connector structure. When creating a Surface, use `McCreate`. |
| `Instance` | `PMCHANDLE` | Pointer to the newly created instance |

| Return | Description |
|--------|-------------|
| `MC_OK` | Success |

---

#### McDelete

Deletes the instance of a MultiCam object.

```c
MCSTATUS McDelete(MCHANDLE Instance);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance to delete |

| Return | Description |
|--------|-------------|
| `MC_OK` | Success |

---

### 7.3. Parameters Management

#### McGetParamFloat

Returns the current value of a MultiCam parameter as a floating-point variable. The parameter is referred to **by-identifier**, and is preferably of the floating-point type.

```c
MCSTATUS McGetParamFloat(MCHANDLE Instance, MCPARAMID Param, PFLOAT64 ValueFloat);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `Param` | `MCPARAMID` | Identifier of the parameter to read |
| `ValueFloat` | `PFLOAT64` | Pointer to the floating-point variable that receives the parameter value |

If the MultiCam parameter is not of the floating-point type, a type conversion is performed.

---

#### McGetParamNmFloat

Returns the current value of a MultiCam parameter as a floating-point variable. The parameter is referred to **by-name**, and is preferably of the floating-point type.

```c
MCSTATUS McGetParamNmFloat(MCHANDLE Instance, PCHAR ParamName, PFLOAT64 ValueFloat);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to read |
| `ValueFloat` | `PFLOAT64` | Pointer to the floating-point variable that receives the parameter value |

If the MultiCam parameter is not of the floating-point type, a type conversion is performed.

---

#### McGetParamInst

Returns the current value of a MultiCam parameter as an instance variable. The parameter is referred to **by-identifier**, and is of the instance type.

```c
MCSTATUS McGetParamInst(MCHANDLE Instance, MCPARAMID Param, PMCHANDLE ValueInst);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `Param` | `MCPARAMID` | Identifier of the parameter to read |
| `ValueInst` | `PMCHANDLE` | Pointer to the instance variable that receives the parameter value |

**Example:**

```c
// Declaring a MCHANDLE variable
MCHANDLE MySurface;
// Getting the parameter using the by-ident method
Status = McGetParamInst(ObjectHandle, MC_Cluster, &MySurface);
```

---

#### McGetParamNmInst

Returns the current value of a MultiCam parameter as an instance variable. The parameter is referred to **by-name**, and is of the instance type.

```c
MCSTATUS McGetParamNmInst(MCHANDLE Instance, PCHAR ParamName, PMCHANDLE ValueInst);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to read |
| `ValueInst` | `PMCHANDLE` | Pointer to the instance variable that receives the parameter value |

**Example:**

```c
// Declaring a MCHANDLE variable
MCHANDLE MySurface;
// Getting the parameter using the by-name method
Status = McGetParamNmInst(ObjectHandle, "Cluster", &MySurface);
```

---

#### McGetParamInt

Returns the current value of a MultiCam parameter as an integer variable. The parameter is referred to **by-identifier**, and is preferably of the integer or enumerated type.

```c
MCSTATUS McGetParamInt(MCHANDLE Instance, MCPARAMID Param, PINT32 ValueInt);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `Param` | `MCPARAMID` | Identifier of the parameter to read |
| `ValueInt` | `PINT32` | Pointer to the integer variable that receives the parameter value |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

**Example:**

```c
// Declaring an integer variable
INT32 MyCount;
// Getting the parameter using the by-ident method
Status = McGetParamInt(ObjectHandle, MC_Elapsed_Fr, &MyCount);
```

---

#### McGetParamNmInt

Returns the current value of a MultiCam parameter as an integer variable. The parameter is referred to **by-name**, and is preferably of the integer or enumerated type.

```c
MCSTATUS McGetParamNmInt(MCHANDLE Instance, PCHAR ParamName, PINT32 ValueInt);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to read |
| `ValueInt` | `PINT32` | Pointer to the integer variable that receives the parameter value |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

**Example:**

```c
// Declaring an integer variable
INT32 MyCount;
// Getting the parameter using the by-name method
Status = McGetParamNmInt(ObjectHandle, "Elapsed_Fr", &MyCount);
```

---

#### McGetParamInt64

Returns the current value of a MultiCam parameter as a 64-bit integer variable. The parameter is referred to **by-identifier**.

```c
MCSTATUS McGetParamInt64(MCHANDLE Instance, MCPARAMID Param, PINT64 ValueInt64);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `Param` | `MCPARAMID` | Identifier of the parameter to read |
| `ValueInt64` | `PINT64` | Pointer to the integer variable that receives the parameter value |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

---

#### McGetParamNmInt64

Returns the current value of a MultiCam parameter as a 64-bit integer variable. The parameter is referred to **by-name**.

```c
MCSTATUS McGetParamNmInt64(MCHANDLE Instance, PCHAR ParamName, PINT64 ValueInt64);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to read |
| `ValueInt64` | `PINT64` | Pointer to the integer variable that receives the parameter value |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

---

#### McGetParamPtr

Returns the current value of a MultiCam parameter as a pointer. The parameter is referred to **by-identifier**.

```c
MCSTATUS McGetParamPtr(MCHANDLE Instance, MCPARAMID Param, PVOID* ValuePtr);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `Param` | `MCPARAMID` | Identifier of the parameter to read |
| `ValuePtr` | `PVOID*` | Pointer to the parameter value |

---

#### McGetParamNmPtr

Returns the current value of a MultiCam parameter as a pointer. The parameter is referred to **by-name**.

```c
MCSTATUS McGetParamNmPtr(MCHANDLE Instance, PCHAR ParamName, PVOID* ValuePtr);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to read |
| `ValuePtr` | `PVOID*` | Pointer to the parameter value |

---

#### McGetParamStr

Returns the current value of a MultiCam parameter as a string variable. The parameter is referred to **by-identifier**, and is preferably of the string or enumerated type.

```c
MCSTATUS McGetParamStr(MCHANDLE Instance, MCPARAMID Param, PCHAR ValueStr, UINT32 MaxLength);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `Param` | `MCPARAMID` | Identifier of the parameter to read |
| `ValueStr` | `PCHAR` | Pointer to the string variable that receives the parameter value |
| `MaxLength` | `UINT32` | Maximum number of characters in the read string |

If the MultiCam parameter is not of the string type, a type conversion is performed.

**Example:**

```c
// Declaring a string variable
CHAR MyBoard[32];
// Getting the parameter
Status = McGetParamStr(ObjectHandle, MC_BoardIdentifier, MyBoard, 32);
```

---

#### McGetParamNmStr

Returns the current value of a MultiCam parameter as a string variable. The parameter is referred to **by-name**, and is preferably of the string or enumerated type.

```c
MCSTATUS McGetParamNmStr(MCHANDLE Instance, PCHAR ParamName, PCHAR ValueStr, UINT32 MaxLength);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to read |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to read |
| `ValueStr` | `PCHAR` | Pointer to the string variable that receives the parameter value |
| `MaxLength` | `UINT32` | Maximum number of characters in the read string |

If the MultiCam parameter is not of the string type, a type conversion is performed.

**Example:**

```c
// Declaring a string variable
CHAR MyBoard[32];
// Getting the parameter using the by-name method
Status = McGetParamNmStr(ObjectHandle, "BoardIdentifier", MyBoard, 32);
```

---

#### McSetParamFloat

Assigns a floating-point variable to a MultiCam parameter. The parameter is referred to **by-identifier**, and is preferably of the floating-point type.

```c
MCSTATUS McSetParamFloat(MCHANDLE Instance, MCPARAMID Param, FLOAT64 ValueFloat);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `Param` | `MCPARAMID` | Identifier of the parameter to configure |
| `ValueFloat` | `FLOAT64` | Floating-point value assigned to the parameter |

If the MultiCam parameter is not of the floating-point type, a type conversion is performed.

---

#### McSetParamNmFloat

Assigns a floating-point variable to a MultiCam parameter. The parameter is referred to **by-name**, and is preferably of the floating-point type.

```c
MCSTATUS McSetParamNmFloat(MCHANDLE Instance, PCHAR ParamName, FLOAT64 ValueFloat);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to configure |
| `ValueFloat` | `FLOAT64` | Floating-point value assigned to the parameter |

If the MultiCam parameter is not of the floating-point type, a type conversion is performed.

---

#### McSetParamInst

Assigns an instance variable to a MultiCam parameter. The parameter is referred to **by-identifier**, and is of the instance type.

```c
MCSTATUS McSetParamInst(MCHANDLE Instance, MCPARAMID Param, MCHANDLE ValueInst);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `Param` | `MCPARAMID` | Identifier of the parameter to configure |
| `ValueInst` | `MCHANDLE` | Handle of the instance assigned to the parameter |

**Example:**

```c
// Setting the parameter using the by-ident method
Status = McSetParamInst(ChannelHandle, MC_Cluster, SurfaceHandle);
```

---

#### McSetParamNmInst

Assigns an instance variable to a MultiCam parameter. The parameter is referred to **by-name**, and is of the instance type.

```c
MCSTATUS McSetParamNmInst(MCHANDLE Instance, PCHAR ParamName, MCHANDLE ValueInst);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to configure |
| `ValueInst` | `MCHANDLE` | Handle of the instance assigned to the parameter |

**Example:**

```c
// Setting the parameter using the by-name method
Status = McSetParamNmInst(ChannelHandle, "Cluster", SurfaceHandle);
```

---

#### McSetParamInt

Assigns an integer variable to a MultiCam parameter. The parameter is referred to **by-identifier**, and is preferably of the integer or enumerated type.

```c
MCSTATUS McSetParamInt(MCHANDLE Instance, MCPARAMID Param, INT32 ValueInt);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `Param` | `MCPARAMID` | Identifier of the parameter to configure |
| `ValueInt` | `INT32` | Integer value assigned to the parameter |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

**Example:**

```c
// Setting the parameter using the by-ident method
Status = McSetParamInt(ObjectHandle, MC_SeqLength_Fr, 100);
```

---

#### McSetParamNmInt

Assigns an integer variable to a MultiCam parameter. The parameter is referred to **by-name**, and is preferably of the integer or enumerated type.

```c
MCSTATUS McSetParamNmInt(MCHANDLE Instance, PCHAR ParamName, INT32 ValueInt);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to configure |
| `ValueInt` | `INT32` | Integer value assigned to the parameter |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

**Example:**

```c
// Setting the parameter using the by-name method
Status = McSetParamNmInt(ObjectHandle, "SeqLength_Fr", 100);
```

---

#### McSetParamInt64

Assigns a 64-bit integer variable to a MultiCam parameter. The parameter is referred to **by-identifier**.

```c
MCSTATUS McSetParamInt64(MCHANDLE Instance, MCPARAMID Param, INT64 ValueInt64);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `Param` | `MCPARAMID` | Identifier of the parameter to configure |
| `ValueInt64` | `INT64` | Integer value assigned to the parameter |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

---

#### McSetParamNmInt64

Assigns a 64-bit integer variable to a MultiCam parameter. The parameter is referred to **by-name**.

```c
MCSTATUS McSetParamNmInt64(MCHANDLE Instance, PCHAR ParamName, INT64 ValueInt64);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to configure |
| `ValueInt64` | `INT64` | Integer value assigned to the parameter |

If the MultiCam parameter is not of the integer type, a type conversion is performed.

---

#### McSetParamPtr

Assigns a pointer value to a MultiCam parameter. The parameter is referred to **by-identifier**.

```c
MCSTATUS McSetParamPtr(MCHANDLE Instance, MCPARAMID Param, PVOID ValuePtr);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `Param` | `MCPARAMID` | Identifier of the parameter to configure |
| `ValuePtr` | `PVOID` | Pointer to the parameter |

---

#### McSetParamNmPtr

Assigns a pointer value to a MultiCam parameter. The parameter is referred to **by-name**.

```c
MCSTATUS McSetParamNmPtr(MCHANDLE Instance, PCHAR ParamName, PVOID ValuePtr);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to configure |
| `ValuePtr` | `PVOID` | Pointer to the parameter |

---

#### McSetParamStr

Assigns a string variable to a MultiCam parameter. The parameter is referred to **by-identifier**, and is preferably of the string or enumerated type.

```c
MCSTATUS McSetParamStr(MCHANDLE Instance, MCPARAMID Param, PCHAR ValueStr);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `Param` | `MCPARAMID` | Identifier of the parameter to configure |
| `ValueStr` | `PCHAR` | Pointer to the string assigned to the parameter |

If the MultiCam parameter is not of the string type, a type conversion is performed.

**Example:**

```c
// Setting the parameter using the by-ident method
Status = McSetParamStr(ObjectHandle, MC_CamFile, "CAM2000-I200SA");
```

---

#### McSetParamNmStr

Assigns a string variable to a MultiCam parameter. The parameter is referred to **by-name**, and is preferably of the string or enumerated type.

```c
MCSTATUS McSetParamNmStr(MCHANDLE Instance, PCHAR ParamName, PCHAR ValueStr);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the instance of the parameter to configure |
| `ParamName` | `PCHAR` | Pointer to a string containing the name of the parameter to configure |
| `ValueStr` | `PCHAR` | Pointer to the string assigned to the parameter |

If the MultiCam parameter is not of the string type, a type conversion is performed.

**Example:**

```c
// Setting the parameter using the by-name method
Status = McSetParamNmStr(ObjectHandle, "CamFile", "CAM2000-I200SA");
```

---

### 7.4. Signaling

#### McGetSignalInfo

Returns the information structure associated with the last occurrence of a MultiCam signal.

```c
MCSTATUS McGetSignalInfo(MCHANDLE Instance, MCSIGNAL Signal, PMCSIGNALINFO SignalInformation);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the channel instance that generates the signal |
| `Signal` | `MCSIGNAL` | Identifier of the signal |
| `SignalInformation` | `PMCSIGNALINFO` | Signal information structure pointer. Updated with signal information when the function completes successfully. |

---

#### McRegisterCallback

Registers the callback function to a channel instance.

```c
MCSTATUS McRegisterCallback(MCHANDLE Instance, PMCCALLBACK CallbackFunction, PVOID Context);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the channel instance of the callback function to register |
| `CallbackFunction` | `PMCCALLBACK` | Pointer to the user-supplied callback function. To unregister a function, use `NULL`. |
| `Context` | `PVOID` | Argument to pass to the callback function |

---

#### McWaitSignal

Allows a thread to wait for a specific MultiCam signal occurrence.

```c
MCSTATUS McWaitSignal(MCHANDLE Instance, MCSIGNAL Signal, UINT32 TimeOut, PMCSIGNALINFO SignalInformation);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `Instance` | `MCHANDLE` | Handle of the channel instance that generates the signal |
| `Signal` | `MCSIGNAL` | Identifier of the signal to be waited for |
| `TimeOut` | `UINT32` | Timeout duration expressed in milliseconds. To disable the timeout, use `INFINITE`. |
| `SignalInformation` | `PMCSIGNALINFO` | Signal information structure pointer. Updated with signal information when the function completes successfully. |

---

### 7.5. Pixel Format Conversion

#### McConvertSurface

Handles software conversion between various pixel formats:

- Bayer pixel formats to RGB pixel formats
- Packed pixel formats (e.g. `Y10P`) to unpacked pixel formats
- RGB to BGR or BGR to RGB pixel formats
- Planar to non-planar or non-planar to planar RGB pixel formats
- Image depth conversion

```c
MCSTATUS McConvertSurface(MCHANDLE InputInstance, MCHANDLE OutputInstance);
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `InputInstance` | `MCHANDLE` | Handle of the input surface instance containing the image buffer to convert |
| `OutputInstance` | `MCHANDLE` | Handle of the output surface instance which will hold the converted image buffer |

---

## 8. Enumerations

### MCPARAMID

Refer to the Parameters Reference for the parameter identifiers.

### MCSTATUS

Signed 32-bit integer.

```c
typedef int MCSTATUS;
```

To get the error codes returned by MultiCam functions, see MultiCam Error Codes.

---

## 9. Exceptions Management

### API Errors

MultiCam errors are managed either with a MultiCam error code or a Windows exception, and with or without an error dialog box.

The **`ErrorHandling`** parameter sets the error management behavior and has 4 possible values:

**`NONE`** -- the function returns a MultiCam error code.
- All API functions have the type `MCSTATUS` and return an integer value.
- If no error occurs, the function returns `MC_OK` (0).
- If an error occurs, the function returns a negative value as defined in `MCSTATUS`.

**`MSG`** -- the function displays a dialog box then returns a MultiCam error code. (Windows OS only)
- If no error occurs, the function returns `MC_OK` (0).
- If an error occurs, a dialog box is displayed with 3 buttons:
  - **OK** -- the function returns a negative value as defined in `MCSTATUS`.
  - **ABORT** -- terminates the application. All MultiCam resources are cleaned up but not the application resources.
  - **IGNORE** -- forces the function to return `MC_OK`, allowing the program to ignore the error.

**`EXCEPTION`** -- the function issues a Windows exception. (Windows OS only)
- If an error occurs, the function generates a WIN32 structured exception with an error code as defined in `MCSTATUS`.
- The exception handling is compiler dependent.

**`MSGEXCEPTION`** -- the function displays a dialog box then issues a Windows exception. (Windows OS only)
- If an error occurs, a dialog box is displayed with 3 buttons:
  - **OK** -- the function generates a WIN32 structured exception.
  - **ABORT** -- terminates the application.
  - **IGNORE** -- forces the function to return `MC_OK` without generating an exception.

### Events Signaling

MultiCam driver generates signals representing events issued by a channel. These signals enable interaction between the image acquisition process and the application.

Refer to the signaling documentation in the MultiCam User Guide.

---

## 10. C Predefined Types

The following data types are used in the MultiCam C API. They are predefined using `typedef`.

| Predefined Type | Description | Definition |
|-----------------|-------------|------------|
| `PVOID` | Pointer to a void | `typedef void *PVOID;` |
| `INT32` | Signed 32-bit integer | `typedef signed int INT32;` |
| `PINT32` | Pointer to a signed 32-bit integer | `typedef signed int *PINT32;` |
| `UINT32` | Unsigned 32-bit integer | `typedef unsigned int UINT32;` |
| `INT64` | Signed 64-bit integer | `typedef signed int INT64;` |
| `PINT64` | Pointer to a signed 64-bit integer | `typedef signed int *PINT64;` |
| `FLOAT64` | Double precision 64-bit floating point | `typedef double FLOAT64;` |
| `PFLOAT64` | Pointer to a double precision 64-bit floating point | `typedef double *PFLOAT64;` |
| `PCHAR` | Pointer to a character | `typedef char *PCHAR;` |
| `PCCHAR` | Pointer to a constant character | `typedef const char *PCCHAR;` |
| `MCHANDLE` | Unsigned 32-bit integer | `typedef UINT32 MCHANDLE;` |
| `PMCHANDLE` | Pointer to an unsigned 32-bit integer | `typedef UINT32 *PMCHANDLE;` |
| `MCSTATUS` | Signed 32-bit integer | `typedef int MCSTATUS;` |
| `MCPARAMID` | Unsigned 32-bit integer | `typedef UINT32 MCPARAMID;` |
| `MCSIGNAL` | Signed 32-bit integer | `typedef int MCSIGNAL;` |

### PMCSIGNALINFO

Pointer to a structure containing MultiCam signal information.

```c
typedef struct _MC_CALLBACK_INFO
{
    PVOID    Context;
    MCHANDLE Instance;
    MCSIGNAL Signal;
    UINT32   SignalInfo;
    UINT32   SignalContext;
} MCSIGNALINFO, *PMCSIGNALINFO, *PMCCALLBACKINFO, MCCALLBACKINFO;
```

### PMCCALLBACK

```c
typedef void (MCAPI *PMCCALLBACK)(PMCSIGNALINFO CbInfo);
typedef void (MCAPI *PMCCALLBACKEX)(PVOID Context);
```
