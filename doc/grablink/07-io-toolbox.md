# I/O Toolbox

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [I/O Ports Overview](#1-io-ports-overview)
2. [I/O Functions](#2-io-functions)
3. [I/O Indices Catalog](#3-io-indices-catalog)

---

## 1. I/O Ports Overview

*(Applies to: Base, DualBase, Full, FullXR)*

Every Channel owns a dedicated set of **10 system I/O ports** including:

- 4 isolated input ports: `IIN1`, `IIN2`, `IIN3`, `IIN4`
- 2 high-speed differential input ports: `DIN1`, `DIN2`
- 4 isolated output ports: `IOUT1`, `IOUT2`, `IOUT3`, `IOUT4`

### Input Ports Functions

| Input Function | DIN1 | DIN2 | IIN1 | IIN2 | IIN3 | IIN4 |
|---------------|------|------|------|------|------|------|
| General-Purpose Inputs | OK | OK | OK | OK | OK | OK |
| Trigger Input | OK | OK | OK | OK | OK | OK |
| End Trigger Input | OK | OK | OK | OK | OK | OK |
| Line Trigger Input | OK | OK | OK | OK | OK | OK |
| Isolated I/O SyncBus Receiver | - | - | - | - | OK | OK |

### Output Ports Functions

| Output Function | IOUT1 | IOUT2 | IOUT3 | IOUT4 | LED |
|----------------|-------|-------|-------|-------|-----|
| General-purpose output | OK | OK | OK | OK | - |
| Event signaling | OK | OK | OK | OK | - |
| Strobe output | OK | - | - | - | - |
| Isolated I/O SyncBus Driver | - | - | OK | OK | - |
| Bracket LED Control | - | - | - | - | OK |

### Output Port Structure

The four output ports are based on a uniform structure including:

1. **Programmable event signal generator** -- a set/reset flip-flop with configurable multiplexers selecting set and reset conditions from internal acquisition events.

2. **Output multiplexer** selecting the signal to be issued:
   - `LOW` -- OFF state of the opto-coupler
   - `HIGH` -- ON state of the opto-coupler
   - `EVSIGx` -- output of the event signal generator
   - `STROBE` -- strobe signal (IOUT1 only)
   - `SB1` -- SyncBus signal 1 (IOUT3 only)
   - `SB2` -- SyncBus signal 2 (IOUT4 only)

3. **ISO electrical interface** -- opto-coupler device

4. **Readback circuit** for reading the actual logic state

### Selecting the Output Function

Output ports are managed using Board class parameters:

- `OutputConfig` -- set-only collection parameter to configure output port usage
- `OutputFunction` -- get-only collection parameter reporting actual assigned function

**OutputFunction states:**

| State | Description |
|-------|-------------|
| `UNKNOWN` | Function not known by Board object; port is free for Channel use (strobe, SyncBus). Default state. |
| `SOFT` | Under direct control of application software for general-purpose usage. |
| `EVENT` | Driven by the event signal generator for event signaling. |

---

## 2. I/O Functions

### General-Purpose Inputs

All system I/O input ports can be used as general-purpose digital input ports.

1. Configure the port by assigning `SOFT` to the corresponding member of `InputConfig`
2. Verify the port is configured: `InputFunction` reports `SOFT`
3. Read the digital state at any time via `InputState`

### General-Purpose Output

When configured for general-purpose usage (`OutputFunction` = `SOFT`), the output multiplexer is restricted to `LOW` and `HIGH` positions.

- `OutputState` = `LOW` forces the LOW position
- `OutputState` = `HIGH` forces the HIGH position
- `OutputState` = `TOGGLE` changes from LOW to HIGH or vice versa

### Trigger Input

For applications using a hardware acquisition trigger, any system I/O input port can be selected as the trigger source.

| Sourcing Device Type | Default Port | Alternate Ports |
|---------------------|-------------|-----------------|
| RS-422 compatible detector | High-speed diff. input #2 | High-speed diff. input #1 |
| Other detectors | Isolated input #2 | Isolated inputs #1, #3, #4 |

> **NOTE:** The default port assignment for trigger is different from the line trigger.

### End Trigger Input

For LONGPAGE mode, any system I/O input port can serve as the end trigger source.

> **NOTE:** The default port assignment is the same as the trigger. This corresponds to using a single signal for both functions (one edge = trigger, opposite edge = end trigger).

### Line Trigger Input

For line-scan applications, one or a pair of input ports can serve as the line trigger source.

| Sourcing Device Type | Default Port(s) | Alternate |
|---------------------|-----------------|-----------|
| RS-422 dual output phase quadrature encoder | DIN1 + DIN2 | - |
| Other dual output phase quadrature encoder | IIN1 + IIN2 | IIN3 + IIN4 |
| RS-422 single output encoder | DIN1 | DIN2 |
| Other single output encoder | IIN1 | IIN2, IIN3, IIN4 |

### Strobe Output

When `OutputFunction` = `UNKNOWN`, the IOUT1 output multiplexer is controlled by the `StrobeMode` parameter.

- `StrobeMode` = `AUTO` or `MAN`: at channel activation, multiplexer is forced to STROBE position; at deactivation, forced to LOW.
- `StrobeMode` = `OFF`: multiplexer forced to LOW at channel activation.
- `StrobeMode` = `NONE`: multiplexer left unchanged.

### Isolated I/O SyncBus Driver

*(Applies to: Base, DualBase, Full, FullXR)*

When `OutputFunction` = `UNKNOWN`, IOUT3 and IOUT4 output multiplexers are controlled by the `SynchronizedAcquisition` parameter.

- `SynchronizedAcquisition` = `MASTER` or `LOCAL_MASTER`: forced to SYNCBUS position at channel activation.
- Other values: multiplexer left unchanged.

### Isolated I/O SyncBus Receiver

*(Applies to: Base, DualBase, Full, FullXR)*

For synchronized acquisition, `IIN3` and `IIN4` ports can serve as SyncBus receivers. Configure via `SynchronizedAcquisition` parameter.

### Event Signaling

*(Applies to: Base, DualBase, Full, FullXR)*

When `OutputFunction` = `EVENT`, the output multiplexer selects the event signal generator output.

The event signal generator is configured via:
- `SetSignal` -- configures the set branch of the SR flip-flop
- `ResetSignal` -- configures the reset branch of the SR flip-flop

Available event sources:
- Start and end of: channel activity, acquisition phase, acquisition sequence
- Rising and falling edges of Camera Link downstream signals: FVAL, LVAL, DVAL
- Rising and falling edges of Camera Link upstream signals: CC1, CC2, CC3, CC4
- `NONE` -- disables further events

> **NOTE:** When the set and reset conditions are identical, the SR flip-flop toggles at every event.

### Bracket LED Control

*(Applies to: Base, DualBase, Full, FullXR)*

The application can turn ON and OFF the bracket LEDs to identify a card in a PC using `OutputConfig` and `OutputState` parameters with the appropriate I/O indices.

---

## 3. I/O Indices Catalog

I/O indices vary by product. The following is a summary of the key indices for each Grablink product.

### Common I/O Structure (Base, DualBase, Full, FullXR)

**Input Lines:**

| Index | Connector | Pin Name | Style |
|-------|-----------|----------|-------|
| 1 | IO | IIN1 | ISO |
| 2 | IO | IIN2 | ISO |
| 3 | IO | IIN3 | ISO |
| 4 | IO | IIN4 | ISO |
| 5 | IO | DIN1 | DIFF |
| 6 | IO | DIN2 | DIFF |
| 7-10 | CAMERA | LVAL, FVAL, DVAL, SPARE | CHANNELLINK |

**Output Lines:**

| Index | Connector | Pin Name | Style |
|-------|-----------|----------|-------|
| 1 | IO | IOUT1 | ISO |
| 2 | IO | IOUT2 | ISO |
| 3 | IO | IOUT3 | ISO |
| 4 | IO | IOUT4 | ISO |
| 7-10 | CAMERA | CC1, CC2, CC3, CC4 | CHANNELLINK |
| 25 | BRACKET | LED | NA |

> **NOTE:** Exact index assignments vary by product model. Refer to the full I/O Indices Catalog in the original document for product-specific details for ExpressEOS, Base, DualBase, Full, and FullXR boards.
