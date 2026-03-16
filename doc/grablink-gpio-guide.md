# Using the I/O Ports of Grablink Base/DualBase/Full/Full XR

> Source: Euresys MultiCam (Grablink GPIO User Guide, Version 1.2, December 17, 2013)

## Contents

- [1. Foreword](#1-foreword)
- [2. Description](#2-description)
  - [2.1. I/O Sets](#21-io-sets)
  - [2.2. I/O Connectors](#22-io-connectors)
  - [2.3. I/O Electrical Characteristics](#23-io-electrical-characteristics)
- [3. Electrical Circuits](#3-electrical-circuits)
  - [3.1. High-Speed Differential Input Ports](#31-high-speed-differential-input-ports)
  - [3.2. Isolated Current-Sense Input Ports](#32-isolated-current-sense-input-ports)
  - [3.3. Isolated Contact Output Ports](#33-isolated-contact-output-ports)
  - [3.4. Interconnecting Grablink Cards Using Isolated I/Os](#34-interconnecting-grablink-cards-using-isolated-ios)
- [4. Grablink System I/O Structure](#4-grablink-system-io-structure)
  - [4.1. Input Ports](#41-input-ports)
  - [4.2. Output Ports](#42-output-ports)
- [5. I/O Usage](#5-io-usage)
  - [5.1. Channel Related Functions](#51-channel-related-functions)
  - [5.2. General Purpose I/O](#52-general-purpose-io)
  - [5.3. Events Outputs](#53-events-outputs)
- [6. Appendix](#6-appendix)
  - [6.1. About Galvanic Isolation](#61-about-galvanic-isolation)
  - [6.2. Electronic Fuse Operation](#62-electronic-fuse-operation)

---

## 1. Foreword

This document targets system designers willing to use the I/O ports of the following Grablink cards: Grablink Full, Grablink Full XR, Grablink DualBase, and Grablink Base.

In this document "Grablink card" designates any of the above mentioned cards.

---

## 2. Description

### 2.1. I/O Sets

Every acquisition channel owns 1 set of I/O ports:

- Grablink Full, Grablink Full XR, and Grablink Base own only **one (1)** set of I/O ports
- Grablink DualBase owns **two (2)** sets of I/O ports

Each I/O set provides **ten (10)** I/O ports:

- 2 high-speed differential input ports
- 4 isolated current-sense input ports
- 4 isolated output ports

> **NOTE:**
> 1. There are no bidirectional I/O ports.
> 2. An acquisition channel has only access to the I/O ports that it owns. Consequently:
>    - There is no I/O resource allocation issue.
>    - An I/O port cannot be shared by multiple acquisition channels even when they belong to the same card (e.g., Grablink DualBase).

### 2.2. I/O Connectors

#### 2.2.1. Internal I/O Connector

On all cards, there is one 26-pin standard-pitch flat-cable header connector for each acquisition channel that provides the connections for all 10 I/O ports, 5V, and 12V power outputs.

> **NOTE:**
> 1. All internal I/O connectors exhibit a uniform pin layout, facilitating migration between cards.
> 2. The internal I/O connector is exclusively accessible from inside the PC chassis.

#### 2.2.2. Grablink Full and Grablink Full XR External I/O Connector

All the I/O ports, the 5V, and the 12V power outputs are available on a connector mounted on the bracket. The connector is a robust High-Density 3-row 26-pin female Sub-D connector equipped with UNC4-40 screw locks.

#### 2.2.3. Grablink DualBase External I/O Connector

A selection of the I/O ports of both acquisition channels, the 5V, and the 12V power outputs are available on a bracket-mounted connector. The connector type is the same as for Grablink Full and Full XR (HD 3-row 26-pin female Sub-D). The I/O ports belonging to acquisition Channel-A and power outputs occupy the same positions as on the Full/Full XR connector.

#### 2.2.4. Grablink Base External I/O Connector

The Grablink Base can be fitted with one of:

- **Standard profile bracket**: 1 camera connector + 1 external I/O connector fitted with a flat cable terminated with a 25-pin connector
- **Low profile bracket**: Only 1 camera connector

With the standard profile bracket, all I/O ports and 5V/12V power outputs can be re-routed to the Sub-D25 connector by plugging the 26-pin flat cable into the internal I/O connector.

### 2.3. I/O Electrical Characteristics

#### 2.3.1. Overview

**Input port types:**
- Isolated current sense input
- High-speed differential input

**Output port type:**
- Isolated contact output

> **NOTE:**
> 1. Each port uses 2 electrical lines for better common mode rejection than ground-referred inputs.
> 2. All ports except "High-speed differential input" ports are individually isolated.
> 3. There are no jumpers or switches; the I/O ports require no hardware configuration.

#### 2.3.2. High-Speed Differential Inputs

Simplified schematic: RS-422/RS-485 receiver with 150 Ohm termination, 1K Ohm pull-up/pull-down resistors to 3.3V, 15 pF filter capacitor.

**Characteristics:**

- Non-isolated ANSI/TIA/EIA-422-B differential line receiver
- -7V / +12V common mode voltage
- 4 kV contact, 8 kV air discharge ESD protection
- Minimum pulse width: 100 nanoseconds
- Maximum 10%-90% rise/fall time: 1 us
- Maximum pulse rate: 5 MHz
- Fixed termination of 120 Ohms
- Guaranteed 'HIGH' input state when unconnected (hardware failsafe circuit)
- HF Noise analog and digital filters

**InputState mapping:**

| Input Voltage | InputState |
|--------------|------------|
| (VIN+ - VIN-) > VThreshold | HIGH |
| (VIN+ - VIN-) < VThreshold | LOW |
| Unconnected input port | HIGH |

#### 2.3.3. Isolated Current-Sense Inputs

Simplified schematic: Opto-coupler with current limiter, comparator with Vref threshold.

**Characteristics:**

- Current-sense input:
  - Input Current Threshold: 1 mA (not adjustable)
  - Input Voltage Threshold: 1.65V; adjustable using an external resistor
- Maximum low-level input voltage: 1.5V (over 0-70 deg C range)
- Minimum high-level input voltage: 1.9V (over 0-70 deg C range)
- Current limitation: max input current of 5 mA @ 30V input voltage
- Polarized input, protected against polarity reversal
- Accepts forward and reverse input voltage up to 30V without damage
- Galvanic isolation:
  - Each input is individually isolated
  - Isolation voltage: 500 VAC RMS
- Minimum pulse width: 10 microseconds
- Maximum pulse rate: 50 kHz
- Guaranteed 'LOW' input state when unconnected

**InputState mapping:**

| Input Current | InputState |
|--------------|------------|
| IIN > 1 mA | HIGH |
| IIN < 1 mA | LOW |
| Unconnected input port | LOW |

#### 2.3.4. Isolated Contact Output

**Characteristics:**

- Isolated contact
- Polarized output
- Galvanic isolation:
  - Each output is individually isolated
  - Isolation voltage: 500 VAC RMS
- Maximum current: 100 mA
- Maximum open state voltage (across pins): +/- 30V
- Maximum closed state voltage:
  - 1V @ 100 mA
  - 0.4V @ 1 mA (same as LVTTL driver)
- Fast switching speed:
  - 5 us (or better) turn-ON and turn-OFF time
  - 100 kHz max frequency

**Typical switching performance @ 25 deg C:**

| Current [mA] | Turn ON time [us] | Turn OFF time [us] |
|--------------|-------------------|-------------------|
| 0.5 | 2.0 | 4.8 |
| 1.0 | 2.0 | 3.9 |
| 4.0 | 2.2 | 3.3 |
| 10 | 2.3 | 2.7 |
| 40 | 2.3 | 2.7 |
| 100 | 2.3 | 2.7 |

- Remains in the OFF-state until the port is under control of the application

**OutputState mapping:**

| OutputState | Output Port State |
|-------------|-------------------|
| HIGH | The contact switch is closed (ON) |
| LOW | The contact switch is open (OFF) |
| Initial state after Power-On | The contact switch is open (OFF) |

#### 2.3.5. Power Supply Output

Non-isolated +5V and +12V power outputs are available on all the I/O connectors.

The power originates from a **4-pin Hard Disk Power Supply connector** and NOT from the PCI Express connector. It is mandatory to connect a Hard Disk power supply cable if an application requires these power outputs.

Power supply outputs are protected by electronic fuses that:
- Protect against damage from overload and short-circuits
- Avoid perturbation on the system power supply during power on sequence

**Current limits:**
- Sum of 5V output load currents: <= 1.0 A (over operating temperature range)
- Sum of 12V output load currents: <= 1.0 A (over operating temperature range)

**Voltage monitor thresholds:**

The presence of +5V and +12V voltages is reported by voltage monitors through input ports `POWER_5V` and `POWER_12V`.

| Power Output | Threshold Voltage |
|-------------|------------------|
| +5V | 3.1V (+/- 1.3V) |
| +12V | 7.3V (+/- 3.1V) |

Input state is HIGH when voltage exceeds threshold, LOW otherwise.

#### 2.3.6. Chassis Ground/Signal Ground Interconnect

The "Chassis ground" electrical net is connected to the "Signal ground" electrical net through a protective network. On Grablink Full, a 100k Ohm resistor bridges the two nets. On Grablink Base and DualBase (which have PoCL), the 100k Ohm resistor is replaced by an inductor that provides an additional path for the DC return current of the camera power supply.

- **Chassis ground** includes the metallic bracket and metallic shell of bracket-mounted connectors
- **Signal ground** is the reference potential for all on-card electric circuits
- **Mandatory:** Firmly attach the bracket to the chassis with the screw to establish a good electrical path

---

## 3. Electrical Circuits

> For all cases shown in this section: for better immunity to electromagnetic noise, it is recommended to use overall shielded cables and connectors that interconnect the "Chassis Ground" of each system device and the Grablink card.

### 3.1. High-Speed Differential Input Ports

#### RS-422 Differential Drivers

Recommended wiring for fast system devices such as motion encoders operating at frequencies up to 5 MHz:

- Connect ANSI/EIA/TIA-422/485 differential line driver (OUT+/OUT-) to Grablink DIN+/DIN-
- Use twisted-pair 120 Ohm transmission line
- Connect Signal Ground wire between system device and Grablink card
- Connect cable shield between Chassis Ground of both devices

| Driver State | InputState |
|-------------|------------|
| VOUT+ > VOUT- | HIGH |
| VOUT+ < VOUT- | LOW |

#### Complementary CMOS or TTL Drivers

Alternate wiring for fast system devices up to 5 MHz:

- Connect complementary CMOS/TTL drivers (OUT+/OUT-) to DIN+/DIN-
- Use twisted-pair 120 Ohm transmission line

| Driver State | InputState |
|-------------|------------|
| OUT+ = High; OUT- = Low | HIGH |
| OUT+ = Low; OUT- = High | LOW |

> **NOTE:** For correct operation of both circuits above, it is mandatory to satisfy the common-mode voltage requirements of the receiver. Practically, add one "Signal Ground" wire between each system device and the Grablink card.

### 3.2. Isolated Current-Sense Input Ports

#### 3.2.1. 3V3 or 5V Logic Drivers

**Totem-pole (or open-collector) LVTTL/TTL/5V CMOS drivers:**

Connect IIN+ to VDD through the driver, IIN- to driver output (OUT). The highest transistor of the totem-pole driver is unused in this configuration.

| Driver State | InputState |
|-------------|------------|
| OUT = Low | HIGH |
| OUT = High | LOW |

**Totem-pole (or open-emitter) LVTTL/TTL/5V CMOS drivers:**

Connect IIN+ to driver output (OUT), IIN- to GND through the driver. The lowest transistor of the totem-pole is unused.

| Driver State | InputState |
|-------------|------------|
| OUT = High | HIGH |
| OUT = Low | LOW |

**Alternate RS-422 differential drivers:**

Use when no more high-speed differential input ports are available and pulse width >= 5 microseconds. Connect IIN+ to OUT+, IIN- to OUT-.

| Driver State | InputState |
|-------------|------------|
| VOUT+ > VOUT- | HIGH |
| VOUT+ < VOUT- | LOW |

#### 3.2.2. 12V or 24V Logic Drivers

**Totem-pole (or open-collector) 12V/24V drivers:**

Same topology as 3V3/5V open-collector variant but with 12V or 24V supply.

| Driver State | InputState |
|-------------|------------|
| OUT = Low | HIGH |
| OUT = High | LOW |

**Totem-pole (or open-emitter) 12V/24V drivers:**

Same topology as 3V3/5V open-emitter variant but with 12V or 24V supply.

| Driver State | InputState |
|-------------|------------|
| OUT = High | HIGH |
| OUT = Low | LOW |

> **NOTE:**
> - Operation without series resistor is allowed thanks to the current limiting function of the input ports.
> - Better noise immunity is obtained by inserting a series resistor. Recommended values: **4.7k Ohm for 12V**, **10k Ohm for 24V**.

#### 3.2.3. Potential-Free Contacts

For system devices using potential-free contacts, the current is supplied by the Grablink card:

- Connect IIN+ to the contact
- Connect IIN- through the contact to the power return
- Power the circuit from the Grablink card's P5V or P12V and PGND pins

### 3.3. Isolated Contact Output Ports

For system devices using the isolated output port, the current is supplied by the system device:

- Connect OUT+ to the positive side of the DC load circuit
- Connect OUT- to the negative/return side

> **NOTE:**
> - The isolated output is **polarized**
> - In case of polarity reversal, the output port acts as a closed contact
> - Capable of up to **100 mA** and switching voltages up to **30V**
> - Exceeding 100 mA or 30V may damage the output port
> - The +5V/+12V power may be delivered by the Grablink card

### 3.4. Interconnecting Grablink Cards Using Isolated I/Os

#### Point-to-Point Connection

Connect one Grablink card's isolated contact output (OUT+/OUT-) to another card's isolated current-sense input (IIN+/IIN-) using a twisted-pair 120 Ohm transmission line. Power the circuit from the driving card's +12V and GND.

#### Star Connection (One Output to Two Inputs)

One isolated contact output can drive two isolated current-sense inputs in parallel, each connected through their own twisted-pair cable. The same +12V/GND from the driving card powers the circuit.

---

## 4. Grablink System I/O Structure

### 4.1. Input Ports

Each I/O set contains 6 input ports with the following I/O indices:

| I/O Index | Type | Pin Name | Notes |
|-----------|------|----------|-------|
| 1 (12) | ISO | IIN1 | Isolated current-sense |
| 2 (13) | ISO | IIN2 | Isolated current-sense |
| 3 (14) | ISO | IIN3 | Isolated current-sense |
| 4 (15) | ISO | IIN4 | Isolated current-sense |
| 5 (16) | DIFF | DIN1 | High-speed differential |
| 6 (17) | DIFF | DIN2 | High-speed differential |

> **NOTE:** I/O indices are:
> - 1 to 6 for: Grablink Base, Channel A of Grablink DualBase, Grablink Full, and Grablink Full XR
> - 12 to 17 for: Channel B of Grablink DualBase

**MultiCam parameters for input ports:**
- `InputState` (get only) -- Current logical state of the input
- `InputPinName` (get only) -- Physical pin name
- `InputConfig` -- Board collection parameter for I/O configuration
- `InputFunction` -- Board collection parameter for I/O function assignment
- `ConnectorName` (get only) -- Connector name

**Input port signal routing includes:**
- Trigger control circuit (with `TrigCtl`, `TrigLine`, `TrigEdge`, `TrigFilter` parameters)
- End Trigger control circuit (with `EndTrigCtl`, `EndTrigLine`, `EndTrigEdge`, `EndTrigFilter` parameters)
- Line Trigger control circuit with quadrature decoder (with `LineTrigCtl`, `LineTrigLine`, `LineTrigEdge`, `LineTrigFilter` parameters)
- SyncBus receiver
- `SynchronizedAcquisition` mux

**Possible usages of input ports:**
- General purpose inputs
- SyncBus receiver
- Trigger source
- End Trigger source
- Line Trigger input

### 4.2. Output Ports

Each I/O set contains 4 output ports with the following I/O indices:

| I/O Index | Type | Pin Name | Special Function |
|-----------|------|----------|-----------------|
| 1 (12) | ISO | IOUT1 | Strobe output + Event Signal 1 (EVSIG1) |
| 2 (13) | ISO | IOUT2 | Event Signal 2 (EVSIG2) |
| 3 (14) | ISO | IOUT3 | Event Signal 3 (EVSIG3) |
| 4 (15) | ISO | IOUT4 | Event Signal 4 (EVSIG4) |

> **NOTE:** I/O indices are:
> - 1 to 4 for: Grablink Base, Channel A of Grablink DualBase, Grablink Full, and Grablink Full XR
> - 12 to 15 for: Channel B of Grablink DualBase

**MultiCam parameters for output ports:**
- `OutputState` -- Current state (HIGH/LOW) and set/get control
- `OutputPinName` (get only) -- Physical pin name
- `OutputStyle` -- Output active level (HIGH/LOW)
- `OutputConfig` -- Board collection parameter for I/O configuration
- `OutputFunction` -- Board collection parameter for I/O function assignment
- `ConnectorName` (get only) -- Connector name

**Output port signal routing includes:**
- Flip-flop (FF) with Set/Reset inputs
- `SetSignal` / `ResetSignal` mux for event-driven control
- Acquisition & Camera Controller events (Strobe, SyncBus Transmitter)
- `StrobeMode` parameter for strobe control
- `SynchronizedAcquisition` for SyncBus output

**Internal signal sources available for output ports:**
- STROBE -- Camera strobe/flash signal
- SB1/SB2 -- SyncBus transmitter signals
- EVSIG1-EVSIG4 -- Event signal outputs

---

## 5. I/O Usage

### 5.1. Channel Related Functions

I/Os can be used by the MultiCam Acquisition Channel for trigger inputs, strobe output, and multi-channel synchronization. Configuration parameters:

| Channel I/O Function | Enable/Disable Parameter | Electrical Style Parameter | Port Assignment Parameter |
|---------------------|-------------------------|--------------------------|--------------------------|
| Line Trigger | `LineRateMode` | `LineTrigCtl` | `LineTrigLine` |
| Trigger | `TrigMode`, `NextTrigMode` | `TrigCtl` | `TrigLine` |
| End Trigger | `EndTrigMode` | `EndTrigCtl` | `EndTrigLine` |
| Strobe | `StrobeMode` | `StrobeCtl` | N/A |
| SyncBus Outputs | `SynchronizedAcquisition` | N/A | N/A |
| SyncBus Inputs | `SynchronizedAcquisition` | N/A | N/A |

**I/O Port Function Capabilities:**

| I/O Port | Line Trigger Input | Line Trigger A/B Inputs | Trigger Input | End Trigger Input | SyncBus Inputs | Strobe Output | SyncBus Outputs |
|----------|-------------------|------------------------|---------------|-------------------|----------------|---------------|-----------------|
| High-speed diff input 1 | Default | Default | Alternate | Alternate | -- | N/A | N/A |
| High-speed diff input 2 | Alternate | -- | Default | Default | -- | N/A | N/A |
| Isolated input 1 | Default | Default | OK | Alternate | -- | N/A | N/A |
| Isolated input 2 | Alternate | -- | Default | Default | -- | N/A | N/A |
| Isolated input 3 | Alternate | Alternate | Alternate | Alternate | Fixed | N/A | N/A |
| Isolated input 4 | Alternate | Alternate | Alternate | -- | -- | N/A | N/A |
| Isolated output 1 | N/A | N/A | N/A | N/A | N/A | Fixed | -- |
| Isolated output 2 | N/A | N/A | N/A | N/A | N/A | -- | -- |
| Isolated output 3 | N/A | N/A | N/A | N/A | N/A | -- | Fixed |
| Isolated output 4 | N/A | N/A | N/A | N/A | N/A | -- | -- |

> **NOTE:**
> - Any of the 6 input ports (both differential and isolated types) can be configured as Line Trigger, Trigger, or End Trigger source.
> - Using two-phase (quadrature) motion encoders for Line Trigger requires two input ports. Only 3 port combinations are allowed (as shown above under "Line Trigger A/B Inputs").
> - Only output port 1 can be configured as strobe output; other output ports can only be used as general purpose outputs.

### 5.2. General Purpose I/O

Every I/O port can be used as a general-purpose card I/O. In that case, it must be configured and managed using the MultiCam Board I/O parameters.

### 5.3. Events Outputs

Every output port can be used to deliver an electrical signal built from a selection of internal events. The I/O ports are configured through the MultiCam Board parameters `SetSignal` and `ResetSignal`.

---

## 6. Appendix

### 6.1. About Galvanic Isolation

Galvanic isolation is the principle of isolating functional sections of electric systems so that charge-carrying particles cannot move from one section to another (no direct electric current flow). Energy and/or information can still be exchanged by other means: capacitance, induction, electromagnetic waves, optical, acoustic, or mechanical means.

Galvanic isolation is used when:
- Two or more electric circuits must communicate but their grounds may be at different potentials
- Breaking ground loops by preventing unwanted current between units sharing a ground conductor
- Safety considerations, preventing accidental current from reaching ground through a person's body

### 6.2. Electronic Fuse Operation

#### Soft Start

When establishing power, the electronic fuse performs a soft start: voltage across the load rises smoothly to nominal level.

| Output | Typical Soft Start Time |
|--------|------------------------|
| +12V | 1 millisecond |
| +5V | 1.4 milliseconds |

#### Current Limiting

- When internal FET is not fully conductive (e.g., during soft-start): if load current exceeds "short-circuit current limit", the fuse limits output current
- When FET is fully conductive: load current may increase beyond "short-circuit current limit" up to "overload current limit"
- If load reaches "overload current limit", the fuse reduces current to "short-circuit current limit" level

| Parameter | Typical Value |
|-----------|--------------|
| Short-circuit current limit | 2.0 A |
| Overload current limit | 3.2 A |

(Values apply to both 5V and 12V electronic fuses.)

#### Overheat Protection

If internal temperature becomes excessive, the electronic fuse turns off completely.

#### Overvoltage Protection

If input voltage exceeds the "voltage limit", the fuse clamps the voltage applied to the load.

| Output | Typical Clamp Voltage |
|--------|----------------------|
| +12V | 15V |
| +5V | 6.65V |

#### Undervoltage Lockout

If input voltage drops below "min. input voltage limit", the fuse turns off completely.

| Output | Typical Min. Input Voltage |
|--------|---------------------------|
| +12V | 8.5V |
| +5V | 3.6V |
