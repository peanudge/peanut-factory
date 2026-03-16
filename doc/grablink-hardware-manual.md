# Grablink Hardware Manual (D413EN)

> Source: Euresys MultiCam 6.19.4 (Doc D413EN)
> Products: PC1622 Grablink Full, PC1623 Grablink DualBase, PC1624 Grablink Base, PC1626 Grablink Full XR

## Contents

- [1. About This Document](#1-about-this-document)
  - [1.1. Document Scope](#11-document-scope)
  - [1.2. Document Structure](#12-document-structure)
  - [1.3. Document Changes](#13-document-changes)
- [2. Mechanical Specification](#2-mechanical-specification)
  - [2.1. Product Pictures](#21-product-pictures)
  - [2.2. Physical Characteristics](#22-physical-characteristics)
  - [2.3. Board and Bracket Layouts](#23-board-and-bracket-layouts)
  - [2.4. Connectors](#24-connectors)
  - [2.5. LED Indicators](#25-led-indicators)
- [3. Electrical Specification](#3-electrical-specification)
  - [3.1. Power Supply Requirements](#31-power-supply-requirements)
  - [3.2. PCI Interface](#32-pci-interface)
  - [3.3. Camera Link Interface](#33-camera-link-interface)
  - [3.4. Power over Camera Link](#34-power-over-camera-link)
  - [3.5. DIN* High-Speed Differential Input Ports](#35-din-high-speed-differential-input-ports)
  - [3.6. IIN* Isolated Current-Sense Input Ports](#36-iin-isolated-current-sense-input-ports)
  - [3.7. IOUT* Isolated Contact Output Ports](#37-iout-isolated-contact-output-ports)
  - [3.8. Power Supply Outputs](#38-power-supply-outputs)
  - [3.9. Chassis Ground/Signal Ground Interconnect](#39-chassis-groundsignal-ground-interconnect)
- [4. Environmental Specification](#4-environmental-specification)
  - [4.1. Environmental Conditions](#41-environmental-conditions)
  - [4.2. Compliance Statements](#42-compliance-statements)
- [5. Related Products & Accessories](#5-related-products--accessories)
  - [5.1. PC1625 DB25F I/O Adapter Cable](#51-pc1625-db25f-io-adapter-cable)
  - [5.2. PC3304 HD26F I/O Adapter Cable](#52-pc3304-hd26f-io-adapter-cable)
  - [5.3. 3305 C2C SyncBus Cable](#53-3305-c2c-syncbus-cable)
  - [5.4. 3306 C2C Quad SyncBus Cable](#54-3306-c2c-quad-syncbus-cable)
  - [5.5. Camera Link Cables](#55-camera-link-cables)

---

## 1. About This Document

### 1.1. Document Scope

This document describes the hardware specifications of all the products of the Grablink series together with their related products.

**Grablink main products:**

| Product | S/N Prefix | Icon |
|---------|-----------|------|
| PC1622 Grablink Full | FM1 | Full |
| PC1623 Grablink DualBase | GDB | DualBase |
| PC1624 Grablink Base | GBA | Base |
| PC1626 Grablink Full XR | FXR | FullXR |

**Grablink accessories:**

| Product | S/N Prefix |
|---------|-----------|
| PC1625 DB25F I/O Adapter Cable | DBC |
| PC3304 HD26F I/O Adapter Cable | 3304 |
| PC3305 C2C SyncBus Cable | 3305 |
| PC3306 C2C Quad SyncBus Cable | 3306 |

> **NOTE:** The S/N prefix is a 3-letter string at the beginning of the card serial number.

### 1.2. Document Structure

This document is composed of 4 main sections:

- **Mechanical Specification** - Product pictures, physical dimensions, connectors description and pin assignments, lamps description
- **Electrical Specification** - Electrical characteristics of all input/output ports, description of the power distribution, power requirements
- **Environmental Specification** - Climatic requirements and CE/FCC/RoHS/WEEE compliance statements
- **Related Products & Accessories** - Description of related products and accessories such as adapters, cables

### 1.3. Document Changes

**MultiCam 6.18:** The following topic was revised:
- IIN* Isolated Current-Sense Input Ports

---

## 2. Mechanical Specification

### 2.1. Product Pictures

The Grablink series includes four board variants with different form factors and capabilities. Refer to product images in the original documentation.

### 2.2. Physical Characteristics

**Dimensions and Weight:**

| Product | Length | Height | Weight |
|---------|--------|--------|--------|
| PC1622 Grablink Full | 167.5 mm (6.6 in) | 111.15 mm (4.38 in) | 133 g (4.69 oz) |
| PC1623 Grablink DualBase | 167.5 mm (6.6 in) | 111.15 mm (4.38 in) | 137 g (4.83 oz) |
| PC1624 Grablink Base | 167.5 mm (6.6 in) | 68.9 mm (2.71 in) | 98 g (3.46 oz) |
| PC1626 Grablink Full XR | 167.5 mm (6.6 in) | 111.15 mm (4.38 in) | 136 g (4.80 oz) |

### 2.3. Board and Bracket Layouts

#### PC1624 Grablink Base

**Factory default assembly for standard-profile chassis:**

The PC1624 Grablink Base board is fitted at factory with a standard profile bracket having 2 connectors and 2 LED indicators:

- **Upper connector** (External I/O Connector): For system connection such as external trigger, illumination control or motion encoder.
- **Lower connector** (Camera Link Connector): For the connection of a Camera Link Base camera. Complies with Camera Link standard Shrunk Delta Ribbon (SDR) connector and supports PoCL.
- **LED Indicators**: Report board and acquisition status.

Internal connectors:
- **Internal I/O Connector**: For system connection such as external trigger, illumination control or motion encoder. Connected to External I/O Connector with a flat cable.
- **Power Input Connector**: For powering the camera through PoCL and powering system devices through the I/O connectors.

**Alternate assembly for low-profile chassis:**

PC1624 Grablink Base comes with two parts: a standard-profile board assembly and an extra low-profile bracket. The standard-profile board assembly can be converted into a low-profile board assembly by exchanging the original bracket with the low-profile bracket.

The low-profile bracket has 1 Camera Link connector and 2 LED indicators.

#### PC1623 Grablink DualBase

The PC1623 Grablink DualBase board is fitted with a standard profile bracket having 2 LED indicators and 3 connectors:

- **LED Indicators**: Report board and acquisition status for Channel A and Channel B respectively.
- **Upper connector** (External I/O Connector): For system connection.
- **Center connector** (Camera Link A Connector): For connection of a single-cable Base camera feeding Channel A. SDR connector with PoCL support.
- **Lower connector** (Camera Link B Connector): For connection of another single-cable Base camera feeding Channel B. SDR connector with PoCL support.

Internal connectors:
- **Channel A Internal I/O Connector**: System I/O for Channel A.
- **Channel B Internal I/O Connector**: System I/O for Channel B.
- **Power Input Connector**: For powering cameras through PoCL and system devices through I/O connectors.

#### PC1622 Grablink Full

The PC1622 Grablink Full board is fitted with a standard profile bracket having 2 LED indicators and 3 connectors:

- **Upper connector** (External I/O Connector): For system connection.
- **Center connector** (Base Camera Link Connector): For connection of a single-cable Base camera and the first cable of a Medium-, Full- or 80-bit camera. SDR connector.
- **Lower connector** (Medium/Full Camera Link Connector): For connection of the second cable of a Medium-, Full- or 80-bit camera. SDR connector.

Internal connectors:
- **Internal I/O Connector**: System I/O.
- **Power Input Connector**: For powering system devices through I/O connectors.
- **C2C SyncBus Connector**: For synchronization of four PC1622 Grablink Full or PC1626 Grablink Full XR cards.

#### PC1626 Grablink Full XR

The PC1626 Grablink Full XR board is fitted with a standard profile bracket having 2 LED indicators and 3 connectors:

- **Upper connector** (External I/O Connector): For system connection.
- **Center connector** (Base Camera Link Connector): For connection of a single-cable Base camera and the first cable of a Medium-, Full- or 80-bit camera. SDR connector with PoCL support.
- **Lower connector** (Medium/Full Camera Link Connector): For connection of the second cable of a Medium-, Full- or 80-bit camera. SDR connector with PoCL support.

Internal connectors:
- **Internal I/O Connector**: System I/O.
- **Power Input Connector**: For powering cameras through PoCL and system devices through I/O connectors.
- **C2C SyncBus Connector**: For synchronization of four PC1622/PC1626 cards.

### 2.4. Connectors

#### Camera Link Connector (Base)

| Property | Value |
|----------|-------|
| Name | Camera Link |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | PoCL Camera Link camera input |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | PoCL | PoCL #1 +12V Power output |
| 2 | CC4- | Camera Control 4 -- Negative pole |
| 3 | CC3+ | Camera Control 3 -- Positive pole |
| 4 | CC2- | Camera Control 2 -- Negative pole |
| 5 | CC1+ | Camera Control 1 -- Positive pole |
| 6 | SERTFG+ | Serial COM to Frame Grabber -- Positive pole |
| 7 | SERTC- | Serial COM to Camera -- Negative pole |
| 8 | X3+ | Channel Link X -- Pair 3 -- Positive pole |
| 9 | XCLK+ | Channel Link X -- Clock -- Positive pole |
| 10 | X2+ | Channel Link X -- Pair 2 -- Positive pole |
| 11 | X1+ | Channel Link X -- Pair 1 -- Positive pole |
| 12 | X0+ | Channel Link X -- Pair 0 -- Positive pole |
| 13 | GND | Ground |
| 14 | GND | Ground |
| 15 | CC4+ | Camera Control 4 -- Positive pole |
| 16 | CC3- | Camera Control 3 -- Negative pole |
| 17 | CC2+ | Camera Control 2 -- Positive pole |
| 18 | CC1- | Camera Control 1 -- Negative pole |
| 19 | SERTFG- | Serial COM to Frame Grabber -- Negative pole |
| 20 | SERTC+ | Serial COM to Camera -- Positive pole |
| 21 | X3- | Channel Link X -- Pair 3 -- Negative pole |
| 22 | XCLK- | Channel Link X -- Clock -- Negative pole |
| 23 | X2- | Channel Link X -- Pair 2 -- Negative pole |
| 24 | X1- | Channel Link X -- Pair 1 -- Negative pole |
| 25 | X0- | Channel Link X -- Pair 0 -- Negative pole |
| 26(2) | PoCL | PoCL #1 +12V Power output |

#### Camera Link A Connector (DualBase)

| Property | Value |
|----------|-------|
| Name | Camera Link A |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | Channel A PoCL Camera Link camera input |

Pin assignments are identical to the Base Camera Link Connector above (including PoCL on pins 1 and 26).

#### Camera Link B Connector (DualBase)

| Property | Value |
|----------|-------|
| Name | Camera Link B |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | Channel B PoCL Camera Link camera input |

Pin assignments are identical to the Base Camera Link Connector above (including PoCL on pins 1 and 26).

#### Base Camera Link Connector (Full)

| Property | Value |
|----------|-------|
| Name | Base Camera Link |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | Camera Link camera input (First cable) |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | CC4- | Camera Control 4 -- Negative pole |
| 3 | CC3+ | Camera Control 3 -- Positive pole |
| 4 | CC2- | Camera Control 2 -- Negative pole |
| 5 | CC1+ | Camera Control 1 -- Positive pole |
| 6 | SERTFG+ | Serial COM to Frame Grabber -- Positive pole |
| 7 | SERTC- | Serial COM to Camera -- Negative pole |
| 8 | X3+ | Channel Link X -- Pair 3 -- Positive pole |
| 9 | XCLK+ | Channel Link X -- Clock -- Positive pole |
| 10 | X2+ | Channel Link X -- Pair 2 -- Positive pole |
| 11 | X1+ | Channel Link X -- Pair 1 -- Positive pole |
| 12 | X0+ | Channel Link X -- Pair 0 -- Positive pole |
| 13 | GND | Ground |
| 14 | GND | Ground |
| 15 | CC4+ | Camera Control 4 -- Positive pole |
| 16 | CC3- | Camera Control 3 -- Negative pole |
| 17 | CC2+ | Camera Control 2 -- Positive pole |
| 18 | CC1- | Camera Control 1 -- Negative pole |
| 19 | SERTFG- | Serial COM to Frame Grabber -- Negative pole |
| 20 | SERTC+ | Serial COM to Camera -- Positive pole |
| 21 | X3- | Channel Link X -- Pair 3 -- Negative pole |
| 22 | XCLK- | Channel Link X -- Clock -- Negative pole |
| 23 | X2- | Channel Link X -- Pair 2 -- Negative pole |
| 24 | X1- | Channel Link X -- Pair 1 -- Negative pole |
| 25 | X0- | Channel Link X -- Pair 0 -- Negative pole |
| 26(2) | GND | Ground |

> **NOTE:** Pin 1 is GND (not PoCL) on the Full -- PoCL is not supported on PC1622 Grablink Full.

#### Medium/Full Camera Link Connector (Full)

| Property | Value |
|----------|-------|
| Name | Medium/Full Camera Link |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | Camera Link camera input (Second cable) |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | Z3+ | Channel Link Z -- Pair 3 -- Positive pole |
| 3 | ZCLK+ | Channel Link Z -- Clock -- Positive pole |
| 4 | Z2+ | Channel Link Z -- Pair 2 -- Positive pole |
| 5 | Z1+ | Channel Link Z -- Pair 1 -- Positive pole |
| 6 | Z0+ | Channel Link Z -- Pair 0 -- Positive pole |
| 7 | TERM- | Unused but terminated pair |
| 8 | Y3+ | Channel Link Y -- Pair 3 -- Positive pole |
| 9 | YCLK+ | Channel Link Y -- Clock -- Positive pole |
| 10 | Y2+ | Channel Link Y -- Pair 2 -- Positive pole |
| 11 | Y1+ | Channel Link Y -- Pair 1 -- Positive pole |
| 12 | Y0+ | Channel Link Y -- Pair 0 -- Positive pole |
| 13 | GND | Ground |
| 14 | GND | Ground |
| 15 | Z3- | Channel Link Z -- Pair 3 -- Negative pole |
| 16 | ZCLK- | Channel Link Z -- Clock -- Negative pole |
| 17 | Z2- | Channel Link Z -- Pair 2 -- Negative pole |
| 18 | Z1- | Channel Link Z -- Pair 1 -- Negative pole |
| 19 | Z0- | Channel Link Z -- Pair 0 -- Negative pole |
| 20 | TERM+ | Unused but terminated pair |
| 21 | Y3- | Channel Link Y -- Pair 3 -- Negative pole |
| 22 | YCLK- | Channel Link Y -- Clock -- Negative pole |
| 23 | Y2- | Channel Link Y -- Pair 2 -- Negative pole |
| 24 | Y1- | Channel Link Y -- Pair 1 -- Negative pole |
| 25 | Y0- | Channel Link Y -- Pair 0 -- Negative pole |
| 26(2) | GND | Ground |

#### Base Camera Link Connector (Full XR)

| Property | Value |
|----------|-------|
| Name | Base Camera Link |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | PoCL Camera Link camera input (First cable) |

Pin assignments are identical to the Base Camera Link Connector, except:
- Pin 1: PoCL #1 +12V Power output
- Pin 26(2): PoCL #1 +12V Power output

#### Medium/Full Camera Link Connector (Full XR)

| Property | Value |
|----------|-------|
| Name | Medium/Full Camera Link |
| Type | 26-position Shrunk Delta Ribbon socket |
| Location | Card bracket |
| Usage | PoCL Camera Link camera input (Second cable) |

Pin assignments are identical to the Full Medium/Full Camera Link Connector, except:
- Pin 1: PoCL #2 +12V Power output
- Pin 26(2): PoCL #1 +12V Power output

#### External I/O Connector (Base)

| Property | Value |
|----------|-------|
| Name | External I/O |
| Type | 25-pin female sub-D connector |
| Location | Card bracket |
| Usage | General purpose I/O and power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | DIN1- | High-speed differential input #1 -- Negative pole |
| 3 | DIN2- | High-speed differential input #2 -- Negative pole |
| 4 | IIN1- | Isolated input #1 -- Negative pole |
| 5 | IIN2- | Isolated input #2 -- Negative pole |
| 6 | IIN3- | Isolated input #3 -- Negative pole |
| 7 | IIN4- | Isolated input #4 -- Negative pole |
| 8 | IOUT1- | Isolated contact output #1 -- Negative pole |
| 9 | IOUT2- | Isolated contact output #2 -- Negative pole |
| 10 | IOUT3- | Isolated contact output #3 -- Negative pole |
| 11 | IOUT4- | Isolated contact output #4 -- Negative pole |
| 12 | GND | Ground |
| 13 | GND | Ground |
| 14 | DIN1+ | High-speed differential input #1 -- Positive pole |
| 15 | DIN2+ | High-speed differential input #2 -- Positive pole |
| 16 | IIN1+ | Isolated input #1 -- Positive pole |
| 17 | IIN2+ | Isolated input #2 -- Positive pole |
| 18 | IIN3+ | Isolated input #3 -- Positive pole |
| 19 | IIN4+ | Isolated input #4 -- Positive pole |
| 20 | IOUT1+ | Isolated contact output #1 -- Positive pole |
| 21 | IOUT2+ | Isolated contact output #2 -- Positive pole |
| 22 | IOUT3+ | Isolated contact output #3 -- Positive pole |
| 23 | IOUT4+ | Isolated contact output #4 -- Positive pole |
| 24 | +5V | +5V Power output |
| 25 | +12V | +12V Power output |
| -- | +12V_RTN | Ground |

#### External I/O Connector (DualBase)

| Property | Value |
|----------|-------|
| Name | External I/O |
| Type | 26-pin 3-row high-density female sub-D connector |
| Location | Card bracket |
| Usage | General purpose I/O and power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | DIN2A+ | Channel A - High-speed differential input #2 -- Positive pole |
| 3 | IIN1A+ | Channel A - Isolated input #1 -- Positive pole |
| 4 | DIN1B- | Channel B - High-speed differential input #1 -- Negative pole |
| 5 | DIN2B- | Channel B - High-speed differential input #2 -- Negative pole |
| 6 | IIN1B- | Channel B - Isolated input #1 -- Negative pole |
| 7 | IOUT1B- | Channel B - Isolated contact output #1 -- Negative pole |
| 8 | +5V | +5V Power output |
| 9 | GND | Ground |
| 10 | GND | Ground |
| 11 | DIN2A- | Channel A - High-speed differential input #2 -- Negative pole |
| 12 | IIN1A- | Channel A - Isolated input #1 -- Negative pole |
| 13 | IIN2A+ | Channel A - Isolated input #2 -- Positive pole |
| 14 | DIN1B+ | Channel B - High-speed differential input #1 -- Positive pole |
| 15 | DIN2B+ | Channel B - High-speed differential input #2 -- Positive pole |
| 16 | IIN1B+ | Channel B - Isolated input #1 -- Positive pole |
| 17 | IOUT1B+ | Channel B - Isolated contact output #1 -- Positive pole |
| 18 | GND | Ground |
| 19 | DIN1A- | Channel A - High-speed differential input #1 -- Negative pole |
| 20 | DIN1A+ | Channel A - High-speed differential input #1 -- Positive pole |
| 21 | IIN2A- | Channel A - Isolated input #2 -- Negative pole |
| 22 | IOUT1A- | Channel A - Isolated contact output #1 -- Negative pole |
| 23 | IOUT1A+ | Channel A - Isolated contact output #1 -- Positive pole |
| 24 | IIN2B- | Channel B - Isolated input #2 -- Negative pole |
| 25 | IIN2B+ | Channel B - Isolated input #2 -- Positive pole |
| 26 | +12V | +12V Power output |

#### External I/O Connector (Full / Full XR)

| Property | Value |
|----------|-------|
| Name | External I/O |
| Type | 26-pin 3-row high-density female sub-D connector |
| Location | Card bracket |
| Usage | General purpose I/O and power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | DIN2+ | High-speed differential input #2 -- Positive pole |
| 3 | IIN1+ | Isolated input #1 -- Positive pole |
| 4 | IIN3- | Isolated input #3 -- Negative pole |
| 5 | IIN4- | Isolated input #4 -- Negative pole |
| 6 | IOUT2- | Isolated contact output #2 -- Negative pole |
| 7 | IOUT4- | Isolated contact output #4 -- Negative pole |
| 8 | +5V | +5V Power output |
| 9 | GND | Ground |
| 10 | GND | Ground |
| 11 | DIN2- | High-speed differential input #2 -- Negative pole |
| 12 | IIN1- | Isolated input #1 -- Negative pole |
| 13 | IIN2+ | Isolated input #2 -- Positive pole |
| 14 | IIN3+ | Isolated input #3 -- Positive pole |
| 15 | IIN4+ | Isolated input #4 -- Positive pole |
| 16 | IOUT2+ | Isolated contact output #2 -- Positive pole |
| 17 | IOUT4+ | Isolated contact output #4 -- Positive pole |
| 18 | GND | Ground |
| 19 | DIN1- | High-speed differential input #1 -- Negative pole |
| 20 | DIN1+ | High-speed differential input #1 -- Positive pole |
| 21 | IIN2- | Isolated input #2 -- Negative pole |
| 22 | IOUT1- | Isolated contact output #1 -- Negative pole |
| 23 | IOUT1+ | Isolated contact output #1 -- Positive pole |
| 24 | IOUT3- | Isolated contact output #3 -- Negative pole |
| 25 | IOUT3+ | Isolated contact output #3 -- Positive pole |
| 26 | +12V | +12V Power output |

#### Internal I/O Connector (Base / Full / Full XR)

| Property | Value |
|----------|-------|
| Name | Internal I/O |
| Type | 26-pin dual-row 0.1" pitch pin header with shrouding |
| Location | Printed circuit board |
| Usage | General purpose I/O and power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | GND | Ground |
| 3 | DIN1+ | High-speed differential input #1 -- Positive pole |
| 4 | DIN1- | High-speed differential input #1 -- Negative pole |
| 5 | DIN2+ | High-speed differential input #2 -- Positive pole |
| 6 | DIN2- | High-speed differential input #2 -- Negative pole |
| 7 | IIN1+ | Isolated input #1 -- Positive pole |
| 8 | IIN1- | Isolated input #1 -- Negative pole |
| 9 | IIN2+ | Isolated input #2 -- Positive pole |
| 10 | IIN2- | Isolated input #2 -- Negative pole |
| 11 | IIN3+ | Isolated input #3 -- Positive pole |
| 12 | IIN3- | Isolated input #3 -- Negative pole |
| 13 | IIN4+ | Isolated input #4 -- Positive pole |
| 14 | IIN4- | Isolated input #4 -- Negative pole |
| 15 | IOUT1+ | Isolated contact output #1 -- Positive pole |
| 16 | IOUT1- | Isolated contact output #1 -- Negative pole |
| 17 | IOUT2+ | Isolated contact output #2 -- Positive pole |
| 18 | IOUT2- | Isolated contact output #2 -- Negative pole |
| 19 | IOUT3+ | Isolated contact output #3 -- Positive pole |
| 20 | IOUT3- | Isolated contact output #3 -- Negative pole |
| 21 | IOUT4+ | Isolated contact output #4 -- Positive pole |
| 22 | IOUT4- | Isolated contact output #4 -- Negative pole |
| 23 | +5V | +5V Power output |
| 24 | GND | Ground |
| 25 | +12V | +12V Power output |
| 26 | +12V_RTN | Ground |

#### Channel A Internal I/O Connector (DualBase)

| Property | Value |
|----------|-------|
| Name | Channel A Internal I/O |
| Type | 26-pin dual-row 0.1" pitch pin header with shrouding |
| Location | Printed circuit board |
| Usage | General purpose I/O and power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | GND | Ground |
| 3 | DIN1A+ | Channel A - High-speed differential input #1 -- Positive pole |
| 4 | DIN1A- | Channel A - High-speed differential input #1 -- Negative pole |
| 5 | DIN2A+ | Channel A - High-speed differential input #2 -- Positive pole |
| 6 | DIN2A- | Channel A - High-speed differential input #2 -- Negative pole |
| 7 | IIN1A+ | Channel A - Isolated input #1 -- Positive pole |
| 8 | IIN1A- | Channel A - Isolated input #1 -- Negative pole |
| 9 | IIN2A+ | Channel A - Isolated input #2 -- Positive pole |
| 10 | IIN2A- | Channel A - Isolated input #2 -- Negative pole |
| 11 | IIN3A+ | Channel A - Isolated input #3 -- Positive pole |
| 12 | IIN3A- | Channel A - Isolated input #3 -- Negative pole |
| 13 | IIN4A+ | Channel A - Isolated input #4 -- Positive pole |
| 14 | IIN4A- | Channel A - Isolated input #4 -- Negative pole |
| 15 | IOUT1A+ | Channel A - Isolated contact output #1 -- Positive pole |
| 16 | IOUT1A- | Channel A - Isolated contact output #1 -- Negative pole |
| 17 | IOUT2A+ | Channel A - Isolated contact output #2 -- Positive pole |
| 18 | IOUT2A- | Channel A - Isolated contact output #2 -- Negative pole |
| 19 | IOUT3A+ | Channel A - Isolated contact output #3 -- Positive pole |
| 20 | IOUT3A- | Channel A - Isolated contact output #3 -- Negative pole |
| 21 | IOUT4A+ | Channel A - Isolated contact output #4 -- Positive pole |
| 22 | IOUT4A- | Channel A - Isolated contact output #4 -- Negative pole |
| 23 | +5V | +5V Power output |
| 24 | GND | Ground |
| 25 | +12V | +12V Power output |
| 26 | +12V_RTN | Ground |

#### Channel B Internal I/O Connector (DualBase)

| Property | Value |
|----------|-------|
| Name | Channel B Internal I/O |
| Type | 26-pin dual-row 0.1" pitch pin header with shrouding |
| Location | Printed circuit board |
| Usage | General purpose I/O and power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | GND | Ground |
| 2 | GND | Ground |
| 3 | DIN1B+ | Channel B - High-speed differential input #1 -- Positive pole |
| 4 | DIN1B- | Channel B - High-speed differential input #1 -- Negative pole |
| 5 | DIN2B+ | Channel B - High-speed differential input #2 -- Positive pole |
| 6 | DIN2B- | Channel B - High-speed differential input #2 -- Negative pole |
| 7 | IIN1B+ | Channel B - Isolated input #1 -- Positive pole |
| 8 | IIN1B- | Channel B - Isolated input #1 -- Negative pole |
| 9 | IIN2B+ | Channel B - Isolated input #2 -- Positive pole |
| 10 | IIN2B- | Channel B - Isolated input #2 -- Negative pole |
| 11 | IIN3B+ | Channel B - Isolated input #3 -- Positive pole |
| 12 | IIN3B- | Channel B - Isolated input #3 -- Negative pole |
| 13 | IIN4B+ | Channel B - Isolated input #4 -- Positive pole |
| 14 | IIN4B- | Channel B - Isolated input #4 -- Negative pole |
| 15 | IOUT1B+ | Channel B - Isolated contact output #1 -- Positive pole |
| 16 | IOUT1B- | Channel B - Isolated contact output #1 -- Negative pole |
| 17 | IOUT2B+ | Channel B - Isolated contact output #2 -- Positive pole |
| 18 | IOUT2B- | Channel B - Isolated contact output #2 -- Negative pole |
| 19 | IOUT3B+ | Channel B - Isolated contact output #3 -- Positive pole |
| 20 | IOUT3B- | Channel B - Isolated contact output #3 -- Negative pole |
| 21 | IOUT4B+ | Channel B - Isolated contact output #4 -- Positive pole |
| 22 | IOUT4B- | Channel B - Isolated contact output #4 -- Negative pole |
| 23 | +5V | +5V Power output |
| 24 | GND | Ground |
| 25 | +12V | +12V Power output |
| 26 | +12V_RTN | Ground |

#### Power Input Connector (Base / DualBase / Full XR)

| Property | Value |
|----------|-------|
| Name | Power Input |
| Type | 4-pin Molex disk drive power male connector socket |
| Location | Printed circuit board |
| Usage | DC power input for PoCL and GPIO power output |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 1 | +12VIN | +12V power input |
| 2 | GND | Ground |
| 3 | GND | Ground |
| 4 | +5VIN | +5V power input |

#### Power Input Connector (Full)

| Property | Value |
|----------|-------|
| Name | Power Input |
| Type | 4-pin Molex disk drive power male connector socket |
| Location | Printed circuit board |
| Usage | DC power input for GPIO power output |

Pin assignments are identical to the Base/DualBase/Full XR Power Input Connector.

#### C2C SyncBus Connector (Full / Full XR)

| Property | Value |
|----------|-------|
| Name | C2C SyncBus |
| Type | 20-pin dual-row 0.050" pitch pin header with shrouding |
| Location | Printed circuit board |
| Usage | Card-to-card SyncBus |

**Pin assignments:**

| Pin | Signal | Usage |
|-----|--------|-------|
| 3 | GND | Ground |
| 6 | GND | Ground |
| 7 | GND | Ground |
| 10 | GND | Ground |
| 17 | C2C_1 | Frame Trigger |
| 19 | C2C_2 | Line Trigger |

### 2.5. LED Indicators

Applies to: Base, DualBase, Full, Full XR

The bracket is fitted with two green LEDs mounted on a right-angled holder.

On PC1623 Grablink DualBase, LED A is related to acquisition channel A whereas LED B is related to acquisition channel B. On PC1624 Grablink Base, PC1622 Grablink Full and PC1626 Grablink Full XR, the two LEDs are related to the same acquisition channel.

**LED states:**

| State | Description | Possible Causes |
|-------|-------------|-----------------|
| OFF | The board is not working | Board not powered; MultiCam driver not loaded; FPGA not yet configured; Board defective; No Camera Link clock received; Camera not connected/powered/initialized |
| ON | Camera Link clock received but LVAL/FVAL missing, or board not acquiring | Camera not initialized yet; Frame grabber not delivering Reset/Expose signals correctly; MultiCam Channel not in READY or ACTIVE state; Trigger conditions not satisfied |
| BLINKING | The board is acquiring data | Normal operation |

---

## 3. Electrical Specification

### 3.1. Power Supply Requirements

#### PC1624 Grablink Base -- Board Consumption

PC1624 Grablink Base draws power exclusively from both the +3.3V and +12V rails of the PCI Express connector.

| Parameter | Min | Typ. | Max | Units |
|-----------|-----|------|-----|-------|
| PCI Express +3.3V supply voltage | 3.0 | 3.3 | 3.6 | V |
| PCI Express +3.3V supply current | | 0.34 | | A |
| PCI Express +12V supply voltage | 11.0 | 12 | 13.0 | V |
| PCI Express +12V supply current | | 0.22 | | A |
| PCI Express power rail requirement | | 3.8 | 4.5 | W |

Notes:
- Typical supply current values measured during normal board operation at 25 deg C ambient temperature and nominal supply voltages.
- Maximum current on each supply rail is below limits allowed for a 10W slot.
- Power consumption is below the 10W maximum power dissipation allowed for a low profile x1 PCI Express add-in card.

**System-dependent consumption:** PC1624 draws power from the Power Input Connector when PoCL power is delivered to the camera or when +5V and +12V power is delivered to external loads through System I/O connectors. No additional power is drawn from the PCI Express connector.

#### PC1623 Grablink DualBase -- Board Consumption

| Parameter | Min | Typ. | Max | Units |
|-----------|-----|------|-----|-------|
| PCI Express +3.3V supply voltage | 3.0 | 3.3 | 3.6 | V |
| PCI Express +3.3V supply current | | 0.47 | | A |
| PCI Express +12V supply voltage | 11.0 | 12 | 13.0 | V |
| PCI Express +12V supply current | | 0.37 | | A |
| PCI Express power rail requirement | | 6.0 | 7.2 | W |

Same notes as Base. System-dependent consumption from Power Input Connector for PoCL and I/O power.

#### PC1622 Grablink Full -- Board Consumption

| Parameter | Min | Typ. | Max | Units |
|-----------|-----|------|-----|-------|
| PCI Express +3.3V supply voltage | 3.0 | 3.3 | 3.6 | V |
| PCI Express +3.3V supply current | | 0.48 | | A |
| PCI Express +12V supply voltage | 11.0 | 12 | 13.0 | V |
| PCI Express +12V supply current | | 0.34 | | A |
| PCI Express power rail requirement | | 5.7 | 6.9 | W |

Power consumption is below 25W maximum for a x4 PCI Express add-in card. System-dependent consumption from Power Input Connector for I/O power.

#### PC1626 Grablink Full XR -- Board Consumption

| Parameter | Min | Typ. | Max | Units |
|-----------|-----|------|-----|-------|
| PCI Express +3.3V supply voltage | 3.0 | 3.3 | 3.6 | V |
| PCI Express +3.3V supply current | | 1.0 | | A |
| PCI Express +12V supply voltage | 11.0 | 12 | 13.0 | V |
| PCI Express +12V supply current | | 0.41 | | A |
| PCI Express power rail requirement | | 8.2 | 9.9 | W |

Power consumption is below 25W maximum for a x4 PCI Express add-in card. System-dependent consumption from Power Input Connector for PoCL and I/O power.

### 3.2. PCI Interface

#### PCI Identification

| | Base | DualBase | Full | FullXR |
|---|------|----------|------|--------|
| PCI Vendor ID | 0x1805 | 0x1805 | 0x1805 | 0x1805 |
| PCI Device ID | 0x030E (782) | 0x030C (780) | 0x030A (778) | 0x0310 (784) |
| PCI Device ID (Recovery) | 0x030F (783) | 0x030D (781) | 0x030B (779) | 0x0311 (785) |
| PCI Sub-Vendor ID | 0x0000 | 0x0000 | 0x0000 | 0x0000 |
| PCI Sub-Device ID | 0x0001 | 0x0001 | 0x0001 | 0x0001 |

#### PCI Interface Type per Product

| Product | Interface Type |
|---------|---------------|
| PC1624 Grablink Base | 1-lane Rev 1.1 PCI Express End-point |
| PC1623 Grablink DualBase | 4-lane Rev 1.1 PCI Express End-point |
| PC1622 Grablink Full | 4-lane Rev 1.1 PCI Express End-point |
| PC1626 Grablink Full XR | 4-lane Rev 1.1 PCI Express End-point |

#### 1-lane Rev 1.1 PCI Express End-point (Base)

- Complies with revision 1.1 of the PCI Express Card Electromechanical specification
- Has a 1-lane wide connector
- Operates at 2.5 GHz
- Supports payload size up to 1024 bytes
- Supports 64-bit addressing for bus master access

> **NOTE:** The card can be used in any 1-lane, 4-lane or 8-lane PCIe slot. It can also be used in a 16-lane PCIe slot that is not reserved for a graphical board.

#### 4-lane Rev 1.1 PCI Express End-point (DualBase / Full / Full XR)

- Complies with revision 1.1 of the PCI Express Card Electromechanical specification
- Has a 4-lane wide connector
- Operates at 2.5 GHz
- Supports payload size up to 1024 bytes
- Supports 64-bit addressing for bus master access
- Supports 1-lane and 4-lane link widths
- Allows for logical reversal of lane numbers
- Offers optimal performance when configured for 4-lane operation

**Lanes Configuration:**

| Configuration | Physical Lane 3 | Physical Lane 2 | Physical Lane 1 | Physical Lane 0 |
|---------------|----------------|----------------|----------------|----------------|
| Four non-reversed lanes | Logical 3 | Logical 2 | Logical 1 | Logical 0 |
| Four reversed lanes (starting on lane 3) | Logical 0 | Logical 1 | Logical 2 | Logical 3 |
| One non-reversed lane | - | - | - | Logical 0 |
| One non-reversed lane starting on lane 3 | Logical 0 | - | - | - |

**Payload Size:** Negotiated during PCI Express fabric configuration. Maximum 1024 bytes. Reported through `PCIePayloadSize` parameter. Possible values: 128, 256, 512, 1024.

**PCIe Endpoint Interface Revision Number:** Reported through `PCIeEndpointRevisionID` parameter.

**PCIe data transfer performance:**

| Parameter | Conditions | Min. | Typ. | Max. | Unit |
|-----------|-----------|------|------|------|------|
| Sustainable output data rate | 4-lane @ 2.5 GT/s | | | 800 | MB/s |

### 3.3. Camera Link Interface

#### Features Overview per Product

| Feature | Base | DualBase | Full | FullXR |
|---------|------|----------|------|--------|
| Camera # | 1 | 2 | 1 | 1 |
| Channel Link # (per camera) | 1 | 1 | 3 | 3 |
| Clock range [MHz] | 20-85 | 20-85 | 20-85 / 30-85* | 20-85 / 30-85* |
| Configurations | Lite, Base | Lite, Base | Base, Medium, Full, 80-bit | Base, Medium, Full, 80-bit |
| Connector type | SDR | SDR | SDR | SDR |
| Interface type | ECCO | ECCO | ECCO | ECCO+ |
| PoCL | OK | OK | - | OK |

(*) Camera Link clock range is restricted to 30-85 MHz for 80-bit configurations.

Abbreviations:
- **SDR:** Shrunk Delta Ribbon connector
- **ECCO:** Extended Camera Link Cable Operation
- **ECCO+:** Extended Camera Link Cable Operation with equalization

#### Channel Link

Each channel link is composed of 5 LVDS pairs:
- One LVDS pair transmits the clock to the frame grabber
- Four LVDS pairs transmit the payload at a bit rate equal to clock frequency x 7

#### Clock Line Requirements

- **Clock frequency range:** 20-85 MHz (30-85 MHz for 80-bit configurations)
- Clock may not be switched off during normal operation
- Clock jitter must be as low as possible; recommended to use X-Tal oscillators. May not exceed 1 ns or 20% of the clock period
- Clock duty cycle must be better than 25%/75%
- For PoCL, clock must be applied within 500 ms after power on

#### Data Lines Requirements

For each channel link individually, the electrical signal of the 4 data lines must conform to Camera Link requirements.

#### Cable Requirements

Camera cables must be Camera Link compliant and terminated with a Mini Delta Ribbon (MDR) or Shrunk Delta Ribbon (SDR) connector. Both PoCL and non-PoCL cables can be used.

> **WARNING:** The usage of poor quality Camera Link cables may cause malfunction. This becomes critical for systems with a high clock rate or long cable length. Use Camera Link cables certified by the manufacturer for the length vs. clock rate combination.

#### Multiple Channel Links Requirements (Full / Full XR)

- Camera Link standard specifies all four enable signals are supplied to all channel links using Medium and Full configurations. For 80-bit, only LVAL needs to be distributed.
- Same LVAL signal must be applied on all channel links involved
- At least one valid LVAL pulse must be sent before acquisition can start
- Skew across LVAL signals may not exceed 2 ns (~40 cm of cable). Use identical cables in terms of length and propagation delay.

### 3.4. Power over Camera Link

Applies to: Base, DualBase, Full XR

#### PoCL Features Overview

| Feature | Base | DualBase | Full | FullXR |
|---------|------|----------|------|--------|
| Switchable PoCL SafePower frame grabber | OK | OK | - | OK |
| Single-cable PoCL Base camera # | 1 | 2 | - | 1 |
| Two-cable PoCL Medium/Full/80-bit camera # | - | - | - | 1 |
| PoCL controller # | 1 | 2 | - | 2 |

> **NOTE:** PC1623 Grablink DualBase and PC1626 Grablink Full XR are fitted with two independent PoCL controllers, one per Camera Link connector.

#### PoCL Controller

The PoCL controllers fulfill the requirements of the Power over Camera Link (PoCL) as specified in Camera Link Standard v2.1, Annex E, section 4:

- SafePower Switchable power output for safe operation with PoCL and non-PoCL cameras
- Automatic detection of PoCL cameras
- Up to 4W of power per Camera Link connector
- Over-current protection (OCP)
- AUTO/OFF control through `PoCL_Mode` parameter
- Status through `PoCL_Status` parameter

**PoCL Controller Unit Specification:**

| Parameter | Min. | Typ. | Max. | Unit |
|-----------|------|------|------|------|
| DC input voltage requirement | 11 | 12 | 13 | V |
| DC output voltage @ON-state | 11 | 12 | 13 | V |
| PoCL power output | | | 4 | W |
| PoCL current rating | | | 400 | mA |

#### Power Source Requirements

Power originates from an external power source attached to the Power Input Connector. The external power source must be a regulated 12V DC supply capable of sustaining the current required by the camera(s).

#### PoCL Operation on PC1626 Grablink Full XR

When a PoCL Camera Link Medium, Full or 80-bit camera is attached, power can be delivered through one or both cables based on PoCL device detection results.

- Cameras drawing less than 4W may use a single connector (usually Base Camera Link)
- Cameras drawing more than 4W must use both connectors with requirements:
  - Camera power supply shall not draw more than 4W per cable
  - Camera power supply shall isolate the two Camera Link connectors
  - Camera shall implement voltage detection enabling power supply only when power is present on both cables

#### Overcurrent Protection (OCP)

Each PoCL controller embeds OCP based on:
- dV/dt limiter (controls output voltage rising slope)
- Short-circuit current limiter
- Overload current limiter
- Thermal protection

**OCP Characteristics:**

| Parameter | Min | Typ | Max | Units |
|-----------|-----|-----|-----|-------|
| Output voltage slew rate | | 0.15 | | V/ms |
| Short circuit current limit | 0.6 | 1 | 1.8 | A |
| Overload current limit | 3.1 | 3.2 | 3.5 | A |

**Overvoltage Protection (OVP):**

| Parameter | Min | Typ | Max | Units |
|-----------|-----|-----|-----|-------|
| Output clamping voltage | 13 | 15 | 17 | V |

#### PoCL Device Detector

Each PoCL controller measures the input impedance of the camera power input circuit at off state.

**PoCL Device Detector Characteristics:**

| Parameter | Min | Typ | Max | Units |
|-----------|-----|-----|-----|-------|
| Sense current on 10K impedance load | 30 | 52 | 75 | uA |
| Detector thresholds | 0.3 | | 1.2 | V |
| Allowable impedance range | 8 | 10 | 12 | kOhm |
| PoCL camera detector sense time delay | | | 500 | ms |
| PoCL camera detector sense OK time | | | 500 | ms |
| Initial clock turn-on delay | | | 500 | ms |
| Turn-off delay after clock-loss | | | 500 | ms |

#### PoCL Cable Requirements

**Current Capacity:** Camera Link Standard (v2.0) requires power and drain wires within a cable assembly shall be capable of handling 1.0 A camera current under fault conditions. Power and drain wires shall be at least AWG 28 or larger.

**Conductor Resistance:** DC resistance of any power wire or drain wire shall not exceed 2.5 Ohm for the length of the cable assembly. AWG 28 conductors have typical resistance of 0.21 Ohm/meter, satisfying requirements for cables up to 11 meters.

#### Disabling PoCL Automatic Activation

To avoid unexpected PoCL activation:
1. Before Channel activation: set `PoCL_Mode` to `OFF`
2. Set `ChannelState` to `READY`

### 3.5. DIN* High-Speed Differential Input Ports

Applies to: Base, DualBase, Full, Full XR

#### Characteristics

- Non-isolated ANSI/TIA/EIA-422-B differential line receiver
- -7V / +12V common mode
- 4 kV contact, 8 kV air discharge ESD protection
- Minimum pulse width: 100 nanoseconds (or better)
- Maximum 10%-90% rise/fall time: 1 us
- Maximum pulse rate: up to 5 MHz
- Fixed termination (not removable)
- Guaranteed 'High' input state when unconnected (hardware failsafe circuit)
- HF noise filtering

The state of the port is reported through the `InputState` MultiCam parameter.

#### Compatible Drivers

- ANSI/EIA/TIA-422/485 differential line drivers
- Complementary TTL drivers

#### Electrical Circuit Examples

**RS-422 differential drivers:** Recommended for fast system devices such as motion encoders operating at frequencies up to 5 MHz. Use twisted-pair 120 Ohm transmission lines with a Signal Ground wire between each system device and the board.

**Complementary CMOS or TTL drivers:** Alternate solution for fast system devices up to 5 MHz. Same grounding requirement applies.

> **NOTE:** For correct operation, it is mandatory to satisfy the common-mode voltage requirements of the receiver. This can be achieved by adding one "Signal Ground" wire between each system device and the board.

### 3.6. IIN* Isolated Current-Sense Input Ports

Applies to: Base, DualBase, Full, Full XR

Isolated current-sense input with wide voltage input range up to 30V, compatible with totem-pole LVTTL, TTL, 5V CMOS drivers, RS-422 differential line drivers, potential free contacts, solid-state relays and opto-couplers.

#### DC Characteristics

| Parameter | Conditions | Min. | Typ. | Max. | Units |
|-----------|-----------|------|------|------|-------|
| Differential voltage | | -30 | | +30 | V |
| Input current threshold | | | 1 | | mA |
| Differential voltage @1 mA | | 1.5 | 1.65 | 1.9 | V |
| Input current | @(VIN+ - VIN-) < 1 V | | | 10 | uA |
| Input current | @(VIN+ - VIN-) = 1.65 V | | 1 | | mA |
| Input current | @(VIN+ - VIN-) = 2.5 V | | 2 | | mA |
| Input current | @(VIN+ - VIN-) = 5 V | | 2.3 | | mA |
| Input current | @(VIN+ - VIN-) = 12 V | | 3 | | mA |
| Input current | @(VIN+ - VIN-) = 30 V | | 5 | | mA |

#### AC Characteristics

| Parameter | Conditions | Min. | Typ. | Max. | Units |
|-----------|-----------|------|------|------|-------|
| Positive pulse width | | 10 | | | us |
| Negative pulse width | | 10 | | | us |
| Pulse rate | | 0 | | 50 | kHz |
| Turn-ON delay | 30 deg C; 50 kHz; 2V square wave; TrigFilter=ON (500 ns) | | 2.1 | | us |
| Turn-OFF delay | (same conditions) | | 4.5 | | us |

> **NOTE:** Turn-ON delay is the time between a transition at the input that turns ON the opto-coupler and the subsequent transition in the FPGA. Turn-OFF delay is for the opposite transition. These delays include the delay from the digital line filter controlled by TrigFilter, LineTrigFilter or EndTrigFilter parameters.

#### Isolation Characteristics

| Parameter | Value |
|-----------|-------|
| Isolation grade | Functional |
| Max. DC voltage | 250 V |
| Max. AC voltage | 170 VRMS |

> **NOTE:** The functional isolation is only for circuit technical protection. It does not provide isolation that can protect a human being from electrical shock.

#### Logical Map

| Input Current | Logical State |
|--------------|---------------|
| IIN > 1 mA | HIGH |
| IIN < 1 mA | LOW |
| Unconnected | LOW |

#### Compatible Drivers

- Totem-pole LVTTL, TTL, 5V CMOS drivers
- RS-422 Differential line drivers
- Potential free contact, solid-state relay, or opto-isolators
- 12V and 24V signaling voltages are also accepted

> **NOTE:** The +12V power supply on the I/O connector(s) can be used for powering drivers. No external resistors are required, but for best noise immunity with 12V and 24V signaling, recommended series resistors: 4.7k Ohm for 12V, 10k Ohm for 24V.

#### Electrical Circuit Examples

**3V3 or 5V totem-pole (open-collector) drivers:** Connect IIN+ to VDD through the driver, IIN- to the driver output. Use twisted-pair 120 Ohm transmission line.

**3V3 or 5V totem-pole (open-emitter) drivers:** Connect IIN+ to the driver output, IIN- to GND through the driver.

**12V or 24V drivers:** Same topologies as above. Optional series resistor recommended (4.7k Ohm for 12V, 10k Ohm for 24V).

**RS-422 differential drivers:** Connect IIN+ to OUT+, IIN- to OUT-. Use when no high-speed differential input ports are available and pulse width exceeds 5 us.

**Potential-free contacts:** Connect IIN+ to P12V (or P5V) power output from the board, IIN- through the contact to PGND.

### 3.7. IOUT* Isolated Contact Output Ports

Applies to: Base, DualBase, Full, Full XR

The output port implements an isolated contact output.

#### DC Characteristics

| Parameter | Conditions | Min. | Typ. | Max. | Units |
|-----------|-----------|------|------|------|-------|
| Current | | | | 100 | mA |
| Differential voltage | Open state | -30 | | 30 | V |
| Differential voltage | Closed state @ 1 mA | | | 0.4 | V |
| Differential voltage | Closed state @ 100 mA | | | 1.0 | V |

> **NOTE:** The output port in closed state has no current limiter -- the user circuit must be designed to avoid excessive currents. The output port remains in OFF-state until under application control.

#### AC Characteristics

| Parameter | Min. | Typ. | Max. | Units |
|-----------|------|------|------|-------|
| Pulse rate | 0 | | 100 | kHz |
| Turn-on time | | 5 | | us |
| Turn-off time | | 5 | | us |

**Typical switching performance @ 25 deg C:**

| Current [mA] | Turn ON time [us] | Turn OFF time [us] |
|--------------|-------------------|-------------------|
| 0.5 | 2.0 | 4.8 |
| 1.0 | 2.0 | 3.9 |
| 4.0 | 2.2 | 3.3 |
| 10 | 2.3 | 2.7 |
| 40 | 2.3 | 2.7 |
| 100 | 2.3 | 2.7 |

#### Isolation Characteristics

| Parameter | Value |
|-----------|-------|
| Isolation grade | Functional |
| Max. DC voltage | 250 V |
| Max. AC voltage | 170 VRMS |

> **NOTE:** The functional isolation is only for circuit technical protection. It does not provide isolation that can protect a human being from electrical shock.

#### Compatible Loads

Any load within the 30V / 100 mA envelope is accepted. Power may originate from an external source or from the 12V and GND pins of the I/O connectors.

#### Notes

- The isolated output is polarized
- In case of polarity reversal, the output port acts as a closed contact
- The isolated output can deliver up to 100 mA and switch voltages up to 30V
- Exceeding 100 mA or 30V may damage the output port
- The +5V/+12V power may be delivered by the board

### 3.8. Power Supply Outputs

Applies to: Base, DualBase, Full, Full XR

Non-isolated +5V and +12V power outputs are available on every I/O connector.

**Power output specification:**

| Parameter | Conditions | Min. | Typ. | Max. | Units |
|-----------|-----------|------|------|------|-------|
| Aggregated +5V output current | Operating temp range | | | 1.0 | A |
| Aggregated +12V output current | Operating temp range | | | 1.0 | A |
| Voltage drop across electronic fuse | Max. output current | | | 0.2 | V |

Power originates from external +5V and +12V supplies attached to the Power Input Connector.

> **NOTE:** It is mandatory to connect a hard disk power supply cable to the power input connector if an application requires one or both of these power outputs.

Each power input line is fitted with an electronic fuse and voltage monitor.

**Electronic fuse protections:**
- Limits inrush current during power on sequence
- Protects against overload
- Protects against short-circuits

> **WARNING:** Sum of load currents drawn from 5V outputs must be <= 1.0 A. Sum of load currents drawn from 12V outputs must be <= 1.0 A.

**Voltage monitor thresholds:**

| Characteristics | Min. | Typ. | Max. | Units |
|----------------|------|------|------|-------|
| +5V voltage monitor threshold | 0.9 | 3.1 | 4.4 | V |
| +12V voltage monitor threshold | 4.2 | 7.3 | 10.4 | V |

Result reported as logical state of POWERSTATE5V and POWERSTATE12V input lines:
- Below threshold: LOW
- Above threshold: HIGH

### 3.9. Chassis Ground/Signal Ground Interconnect

Applies to: Base, DualBase, Full, Full XR

The "Chassis ground" electrical net is connected to the "Signal ground" electrical net through a protective network. This prevents significant voltages from developing between the two nets. Together with the cable shield and resistor, it provides an additional path for the DC return current of the camera power supply.

Notes:
- The "Chassis ground" net includes the metallic bracket and metallic shell of the connectors
- The "Signal ground" net is the reference potential for all on-board electric circuits
- It is mandatory to firmly attach the bracket on the chassis by means of the screw to establish a good electrical path

---

## 4. Environmental Specification

### 4.1. Environmental Conditions

**Storage Conditions:**

| Parameter | Conditions | Min | Max | Units |
|-----------|-----------|-----|-----|-------|
| Ambient air temperature | | -20 (-4) | 70 (158) | deg C (deg F) |
| Ambient air humidity | Non-condensing | 10 | 90 | % RH |

**Operating Conditions:**

| Parameter | Conditions | Min | Max | Units |
|-----------|-----------|-----|-----|-------|
| Ambient air temperature* | | 0 (32) | 50 (122) | deg C (deg F) |
| Ambient air humidity | Non-condensing | 10 | 90 | % RH |

(*) Ambient air temperature is measured in the close vicinity of the heatsink, at a distance of about 10 mm above the PCB.

### 4.2. Compliance Statements

**CE/UKCA Compliance:**
- Europe: Conformity with Council Directive 2014/30/EU
- Great Britain: Conformity with Electromagnetic Compatibility Regulations 2016
- Tested to comply with Class B EN55022/CISPR22 electromagnetic emission requirements and EN55024/CISPR24 electromagnetic susceptibility
- Shielded cables must be used to connect peripherals

**FCC Compliance (USA):**
- Complies with limits for Class B digital device, pursuant to Part 15 of FCC Rules
- Provides reasonable protection against harmful interference in residential installations

**KC Compliance (Korea):**

| Product | KC Registration Number |
|---------|----------------------|
| PC1623 Grablink DualBase | MSIP-REM-EUr-PC1623 |
| PC1626 Grablink Full XR | MSIP-REM-EUr-PC1626 |
| PC1622 Grablink Full | MSIP-REM-EUr-PC1622 |
| PC1624 Grablink Base | MSIP-REM-EUr-PC1624 |

**RoHS Compliance:** Conformity with EU RoHS 2015/863 (ROHS3) Directive.

**WEEE Statement:** Product must be disposed of separately from normal household waste per Directive 2012/19/EU.

---

## 5. Related Products & Accessories

### 5.1. PC1625 DB25F I/O Adapter Cable

Applies to: DualBase, Full, Full XR

- Cable length: 200 mm
- B connector: 2x 13-pin 2.54 mm pitch IDC plug
- A connector: 25-pin 2-row Sub-D female connector with UNC 4-40 screw nuts

The PC1625 DB25F I/O Adapter Cable connects all pins (except pin 1) of a 26-pin dual-row 0.1" pitch connector to a 25-pin female SubD connector fitted into a standard-profile PC bracket.

**Usage with Internal I/O Connector (Full / Full XR):**

| Wire # | IDC Pin # | SubD Pin # | Signal | Description |
|--------|----------|-----------|--------|-------------|
| 1 | 1 | -- | GND | Ground |
| 2 | 2 | 1 | GND | Ground |
| 3 | 3 | 14 | DIN1+ | High-speed differential input #1 -- Positive |
| 4 | 4 | 2 | DIN1- | High-speed differential input #1 -- Negative |
| 5 | 5 | 15 | DIN2+ | High-speed differential input #2 -- Positive |
| 6 | 6 | 3 | DIN2- | High-speed differential input #2 -- Negative |
| 7 | 7 | 16 | IIN1+ | Isolated input #1 -- Positive |
| 8 | 8 | 4 | IIN1- | Isolated input #1 -- Negative |
| 9 | 9 | 17 | IIN2+ | Isolated input #2 -- Positive |
| 10 | 10 | 5 | IIN2- | Isolated input #2 -- Negative |
| 11 | 11 | 18 | IIN3+ | Isolated input #3 -- Positive |
| 12 | 12 | 6 | IIN3- | Isolated input #3 -- Negative |
| 13 | 13 | 19 | IIN4+ | Isolated input #4 -- Positive |
| 14 | 14 | 7 | IIN4- | Isolated input #4 -- Negative |
| 15 | 15 | 20 | IOUT1+ | Isolated contact output #1 -- Positive |
| 16 | 16 | 8 | IOUT1- | Isolated contact output #1 -- Negative |
| 17 | 17 | 21 | IOUT2+ | Isolated contact output #2 -- Positive |
| 18 | 18 | 9 | IOUT2- | Isolated contact output #2 -- Negative |
| 19 | 19 | 22 | IOUT3+ | Isolated contact output #3 -- Positive |
| 20 | 20 | 10 | IOUT3- | Isolated contact output #3 -- Negative |
| 21 | 21 | 23 | IOUT4+ | Isolated contact output #4 -- Positive |
| 22 | 22 | 11 | IOUT4- | Isolated contact output #4 -- Negative |
| 23 | 23 | 24 | +5V | +5V Power output |
| 24 | 24 | 12 | GND | Ground |
| 25 | 25 | 25 | +12V | +12V Power output |
| 26 | 26 | 13 | +12V_RTN | Ground |

The same wiring applies for DualBase Channel A Internal I/O Connector (with "A" suffix on signal names) and Channel B Internal I/O Connector (with "B" suffix on signal names).

### 5.2. PC3304 HD26F I/O Adapter Cable

- Cable length: 200 mm
- B connector: 2x 13-pin 2.54 mm pitch IDC plug
- A connector: 26-pin 3-row high-density Sub-D female connector with UNC 4-40 screw nuts

The PC3304 HD26F I/O Adapter Cable interconnects a 26-pin dual-row 0.1" pitch connector to a 26-pin 3-row female High-density SubD connector fitted into a standard-profile PC bracket.

**Usage with Internal I/O Connector (Full / Full XR):**

| Wire # | IDC Pin # | SubD Pin # | Signal | Description |
|--------|----------|-----------|--------|-------------|
| 1 | 1 | 1 | GND | Ground |
| 2 | 2 | 10 | GND | Ground |
| 3 | 3 | 20 | DIN1+ | High-speed differential input #1 -- Positive |
| 4 | 4 | 19 | DIN1- | High-speed differential input #1 -- Negative |
| 5 | 5 | 13 | DIN2+ | High-speed differential input #2 -- Positive |
| 6 | 6 | 11 | DIN2- | High-speed differential input #2 -- Negative |
| 7 | 7 | 3 | IIN1+ | Isolated input #1 -- Positive |
| 8 | 8 | 12 | IIN1- | Isolated input #1 -- Negative |
| 9 | 9 | 13 | IIN2+ | Isolated input #2 -- Positive |
| 10 | 10 | 21 | IIN2- | Isolated input #2 -- Negative |
| 11 | 11 | 14 | IIN3+ | Isolated input #3 -- Positive |
| 12 | 12 | 4 | IIN3- | Isolated input #3 -- Negative |
| 13 | 13 | 15 | IIN4+ | Isolated input #4 -- Positive |
| 14 | 14 | 5 | IIN4- | Isolated input #4 -- Negative |
| 15 | 15 | 23 | IOUT1+ | Isolated contact output #1 -- Positive |
| 16 | 16 | 22 | IOUT1- | Isolated contact output #1 -- Negative |
| 17 | 17 | 16 | IOUT2+ | Isolated contact output #2 -- Positive |
| 18 | 18 | 6 | IOUT2- | Isolated contact output #2 -- Negative |
| 19 | 19 | 25 | IOUT3+ | Isolated contact output #3 -- Positive |
| 20 | 20 | 24 | IOUT3- | Isolated contact output #3 -- Negative |
| 21 | 21 | 17 | IOUT4+ | Isolated contact output #4 -- Positive |
| 22 | 22 | 7 | IOUT4- | Isolated contact output #4 -- Negative |
| 23 | 23 | 8 | +5V | +5V Power output |
| 24 | 24 | 9 | GND | Ground |
| 25 | 25 | 26 | +12V | +12V Power output |
| 26 | 26 | 18 | +12V_RTN | Ground |

The same wiring applies for DualBase Channel A and Channel B Internal I/O Connectors (with "A" or "B" suffix on signal names).

### 5.3. 3305 C2C SyncBus Cable

Applies to: Full, Full XR

- Connectors: 2x 10-pin 1.27 mm pitch IDC plug
- Cable length: 500 mm

The 3305 C2C SyncBus cable interconnects the C2C SyncBus Connectors of 2 Grablink Full or Grablink Full XR cards located in the same PC. It enables synchronization of 2 cameras.

### 5.4. 3306 C2C Quad SyncBus Cable

Applies to: Full, Full XR

- Connectors: 4x 10-pin 1.27 mm pitch IDC plug
- Cable segment length: 76 mm each

The 3306 C2C Quad SyncBus cable interconnects the C2C SyncBus Connectors of up to 4 Grablink Full or Grablink Full XR cards in the same PC. It enables synchronization of up to 4 cameras.

### 5.5. Camera Link Cables

#### Universal Camera Link Cable

**Cable Connectors:**

| Designator | Connector Family | Description |
|-----------|-----------------|-------------|
| MDR | Mini Delta Ribbon | 1.27 mm-pitch blade 26-blade 2-row male plug |
| SDR | Shrunk Delta Ribbon | 0.8 mm-pitch blade 26-blade 2-row male plug |

Any combination of MDR and SDR connectors are valid.

**Cable composition:**

| Structure | Impedance | Suggested Gauge |
|-----------|-----------|----------------|
| 10 High-quality twisted pairs | 100 ohms differential | AWG 28 (0.08 mm2) |
| 1 Twisted pair | N/A | |
| 4 wires | N/A | |
| Overall shield | 80% coverage | |

> **WARNING:** Twisted pairs must be individually shielded.

This cable carries signals for two Channel Links and is universal -- usable as both first and second cable in any configuration. Supports PoCL and non-PoCL cameras.

**Wiring (Second cable / Full & 80-bit configurations):**

| Conductor | Signal | Camera Pin | Frame Grabber Pin | Function |
|-----------|--------|-----------|-------------------|----------|
| Shield | SGND | Shell clamp | Shell clamp | EMC shield |
| HQ pair | Yclk+/Yclk- | 18/5 | 9/22 | Channel Link Y clock |
| HQ pair | Y0+/Y0- | 15/2 | 12/25 | Channel Link Y data 0 |
| HQ pair | Y1+/Y1- | 16/3 | 11/24 | Channel Link Y data 1 |
| HQ pair | Y2+/Y2- | 17/4 | 10/23 | Channel Link Y data 2 |
| HQ pair | Y3+/Y3- | 19/6 | 8/21 | Channel Link Y data 3 |
| HQ pair | Zclk+/Zclk- | 24/11 | 3/16 | Channel Link Z clock |
| HQ pair | Z0+/Z0- | 21/8 | 6/19 | Channel Link Z data 0 |
| HQ pair | Z1+/Z1- | 22/9 | 5/18 | Channel Link Z data 1 |
| HQ pair | Z2+/Z2- | 23/10 | 4/17 | Channel Link Z data 2 |
| HQ pair | Z3+/Z3- | 25/12 | 2/15 | Channel Link Z data 3 |
| Pair | TERM | 7/20 | 20/7 | Terminated |
| Wire | Power | 1 | 1 | Power (nominal 12V DC) |
| Wire | GND | 14 | 14 | Power Return |
| Wire | GND | 13 | 13 | Power Return |
| Wire | Power | 26 | 26 | Power (nominal 12V DC) |

> **NOTE:** This wiring does not implement a straightforward pin-to-pin connection. However, the pin assignment is such that the cable can be installed in any direction.

#### Base/Medium Camera Link Cable

**Cable composition:**

| Structure | Impedance | Suggested Gauge |
|-----------|-----------|----------------|
| 5 High-quality twisted pairs | 100 ohms differential | AWG 28 (0.08 mm2) |
| 6 Twisted pairs | N/A | |
| 4 wires | N/A | |
| Overall shield | 80% coverage | |

> **WARNING:** Twisted pairs must be individually shielded.

> **WARNING:** This cable has only 5 high-quality pairs for a single Channel Link. It cannot be used as the second cable in Full and 80-bit configurations.

Supports PoCL and non-PoCL cameras.

**Wiring (Base configuration / first cable):**

| Conductor | Signal | Camera Pin | Frame Grabber Pin | Function |
|-----------|--------|-----------|-------------------|----------|
| Shield | SGND | Shell clamp | Shell clamp | EMC shield |
| HQ pair | Xclk+/Xclk- | 18/5 | 9/22 | Channel Link clock |
| HQ pair | X0+/X0- | 15/2 | 12/25 | Channel Link data 0 |
| HQ pair | X1+/X1- | 16/3 | 11/24 | Channel Link data 1 |
| HQ pair | X2+/X2- | 17/4 | 10/23 | Channel Link data 2 |
| HQ pair | X3+/X3- | 19/6 | 8/21 | Channel Link data 3 |
| Pair | CC1+/CC1- | 22/9 | 5/18 | Camera Control 1 |
| Pair | CC2+/CC2- | 10/23 | 17/4 | Camera Control 2 |
| Pair | CC3+/CC3- | 24/11 | 3/16 | Camera Control 3 |
| Pair | CC4+/CC4- | 12/25 | 15/2 | Camera Control 4 |
| Pair | SerTC+/SerTC- | 7/20 | 20/7 | Serial to Camera |
| Pair | SerTFG+/SerTFG- | 21/8 | 6/19 | Serial to Grabber |
| Wire | Power | 1 | 1 | Power (nominal 12V DC) |
| Wire | GND | 14 | 14 | Power Return |
| Wire | GND | 13 | 13 | Power Return |
| Wire | Power | 26 | 26 | Power (nominal 12V DC) |

> **NOTE:** This wiring does not implement a straightforward pin-to-pin connection. The cable can be installed in any direction.
