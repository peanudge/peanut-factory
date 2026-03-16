# Grablink Installation Guide (D421EN)

> Source: Euresys MultiCam 6.19.4 (Doc D421EN, build 4059, 2025-12-15)

## Contents

- [1. Declarations](#1-declarations)
- [2. Precautions for Use of Board Products](#2-precautions-for-use-of-board-products)
- [3. PCI Express Card Slot Requirements](#3-pci-express-card-slot-requirements)
- [4. PCI Express Card Installation Procedure](#4-pci-express-card-installation-procedure)
- [5. Low-Profile Bracket Installation](#5-low-profile-bracket-installation)
- [6. Software Setup Procedure](#6-software-setup-procedure)

---

## 1. Declarations

### Notice for Europe

This product is in conformity with the Council Directive 2014/30/EU.

### Notice for Great Britain

This product is in conformity with Electromagnetic Compatibility Regulations 2016.

This equipment has been tested and found to comply with:

- Class B EN55022/CISPR22 electromagnetic emission requirements for information technology equipment
- EN55024/CISPR24 electromagnetic immunity requirements for information technology equipment

This product has been tested in a typical class B compliant host system. It is assumed that this product will also achieve compliance in any class B compliant unit.

To meet EC and Great Britain requirements, shielded cables must be used to connect a peripheral to the card.

### Notice for USA

**Compliance Information Statement (Declaration of Conformity Procedure) DoC FCC Part 15**

This equipment has been tested and found to comply with the limits for a Class B digital device, pursuant to Part 15 of the FCC Rules.

These limits are designed to provide reasonable protection against harmful interference in a residential installation or when the equipment is operated in a commercial environment.

This equipment generates, uses and can radiate radio frequency energy and, if not installed and used in accordance with the instructions, may cause harmful interference to radio communications. However, there is no guarantee that interference will not occur in a particular installation.

If this equipment does cause harmful interference to radio or television reception, which can be determined by turning the equipment off and on, the user is encouraged to try to correct the interference by one or more of the following measures:

- Reorient or relocate the receiving antenna.
- Increase the separation between the equipment and receiver.
- Connect the equipment into an outlet on a circuit different from that to which the receiver is connected.
- Consult the dealer or an experienced radio/TV technician for help.

### Notice for Korea

The following products have been registered under the Clause 3, Article 58-2 of Radio Waves Act:

| Product | KC Registration Number |
|---------|----------------------|
| PC1623 Grablink DualBase | MSIP-REM-EUr-PC1623 |
| PC1622 Grablink Full | MSIP-REM-EUr-PC1622 |
| PC1626 Grablink Full XR | MSIP-REM-EUr-PC1626 |
| PC1624 Grablink Base | MSIP-REM-EUr-PC1624 |

### Environmental and Safety Compliance

- This product is in conformity with the European Union 2015/863 (RoHS3) Directive, that stands for "the restriction of the use of certain hazardous substances in electrical and electronic equipment".
- This product is in conformity with the European Union 1907/2006 (REACH) regulation.
- According to the European directive 2012/19/EU, the product must be disposed of separately from normal household waste. It must be recycled according to the local regulations.

### Camera Link Compliance

- PC1624 Grablink Base, PC1623 Grablink DualBase, PC1622 Grablink Full and PC1626 Grablink Full XR fully conform to the "Specifications of the Camera Link Interface Standard for Digital Cameras and Frame Grabbers" version 2.0.
- PC1624 Grablink Base, PC1623 Grablink DualBase and PC1626 Grablink Full XR are "switchable PoCL frame grabbers" implementing the "SafePower" feature, as defined in the Annex E of the Camera Link standard version 1.2.

---

## 2. Precautions for Use of Board Products

**Electrostatic Sensitive Device** -- Boards may be damaged by electrostatic discharges. Follow the procedure hereby described and apply any general procedure aimed at reducing the risk associated with electrostatic discharge. Damage caused by improper handling is not covered by the manufacturer's warranty.

**Electromagnetic Compatibility** -- Euresys boards are compliant with electromagnetic compatibility regulatory requirements. To ensure this compliance, the card bracket must be secured with the relevant screw in accordance with the procedure described herein.

**Risk of Electrical Shock** -- Do not operate the computer with any enclosure cover removed. During the hardware installation, ensure the AC power cord is unplugged before touching any internal part of the computer.

**Risk of Burn** -- Do not touch an operating board. Allow board to cool before handling.

**Heating Device** -- It is normal for a board to dissipate some heat during operation. All enclosure covers, including blank brackets, must be fitted correctly to ensure that the fan cools the computer adequately.

**Hot Plugging Forbidden** -- Uncontrolled plugging and unplugging of equipment may damage a board. Always switch off the computer and any relevant system device when connecting or disconnecting a cable at the frame grabber or auxiliary board bracket. Failure to do so may damage the card and will void the warranty.

**Poor Grounding Protection** -- The computer and the camera can be located in distant areas with individual ground connections. Poor ground interconnection, ground loop or ground fault may induce unwanted voltage between equipment, causing excessive current in the interconnecting cables. This faulty situation can damage the frame grabber or the camera electrical interface. The user must follow proper equipment grounding practices at all ends of the interconnecting cables. In addition, the use of cable assemblies with overall shield solidly connected to the conductive shell of all connectors is recommended. Besides the beneficial effect of cable shielding on electromagnetic compatibility, the shield connection can increase the protection level against grounding problems by temporarily absorbing unwanted fault current.

---

## 3. PCI Express Card Slot Requirements

For optimal performance:

- **PC1623 Grablink DualBase, PC1622 Grablink Full and PC1626 Grablink Full XR** must be plugged into a x4, x8 or x16 PCI Express Gen 1 (or higher) card connector providing at least four active lanes.
- **PC1624 Grablink Base** must be plugged into a x1, x4, x8 or x16 PCI Express Gen 1 (or higher) card connector.

To guarantee proper operation and longer board reliability, ensure an adequate cooling of the card. The cooling is improved by a higher air flow circulating around the board. This air flow is increased, for example, by using computer case fans. In addition, avoid placing a Grablink card next to other heat dissipating boards.

---

## 4. PCI Express Card Installation Procedure

1. Switch off the computer and all connected peripherals (monitor, printer...).
2. Discharge any static electricity that could be accumulated by your body. You can achieve this by touching an unpainted metal part of the enclosure of your computer with a bare hand. Make sure that the computer is linked to the AC power outlet with proper earth connection.
3. Disconnect all cables from your computer, including AC power.
4. Open the computer enclosure, according to the manufacturer instructions, to gain access to the PCI Express slots. Locate an available and adequate PCI Express slot.
5. Remove the blank bracket associated with this location. To achieve this, remove the securing screw and keep it aside for later use in the procedure. Keep the blank bracket in a known place for possible re-use.
6. Unwrap the card packing, take the board and carefully hold it. Avoid any contact of the board with unnecessary items, including your clothes.
7. Gently insert the card into the selected PCI Express slot, taking care to push it down fully into the slot. If you experience some resistance, remove the board and repeat the operation. You should attempt to make a perfect board-to-slot mechanical alignment for best results. Ensure that the lower part of the bracket is inserted into the corresponding enclosure fastening.
8. **Optional.** When the camera(s) is (are) powered through the Camera Link cable or when the +12 V power output is required on any System I/O connector, connect a 12 V power source to the Auxiliary Power Input connector using a 4-pin MOLEX cable.
9. **Optional.** Establish the connections with the Internal GPIO connector(s) as required by the application.
10. **Optional.** When synchronized acquisition is required for cameras attached to different cards, establish the card-to-card link interconnections.
11. Secure the board with the saved screw.
12. Close the computer enclosure according to the manufacturer instructions.
13. Establish the camera(s) connection(s).

---

## 5. Low-Profile Bracket Installation

PC1624 Grablink Base is delivered with two brackets: a full-height bracket and a low-profile bracket.

To install PC1624 Grablink Base in a low-profile computer, replace the existing full-height standard bracket with the low-profile bracket included in your kit:

1. Unplug the flat cable from the Internal I/O connector.
2. Remove the original standard-profile bracket by unscrewing the screw locks of the Camera Link connector; keep the screw locks.
3. Install the low-profile bracket; secure it on the board with the Camera Link connector screw locks.

---

## 6. Software Setup Procedure

Prior to use the board, it is necessary to install the driver.

- The MultiCam driver is available in the Grablink section of the download area of the Euresys website: <https://www.euresys.com/Support/Download-area>
- Detailed instructions for driver installation are available in the Getting Started > Software Setup section of the MultiCam on-line documentation: <https://documentation.euresys.com/Products/MULTICAM/MULTICAM/Default.htm>
