# White Balance & Color Processing

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [What Is White Balance?](#1-what-is-white-balance)
2. [White Balance Operator](#2-white-balance-operator)
3. [Automatic Calibration Description](#3-automatic-calibration-description)
4. [Automatic Calibration Requirements](#4-automatic-calibration-requirements)
5. [Automatic Calibration Timing](#5-automatic-calibration-timing)
6. [AWB_AREA Settings Description](#6-awb_area-settings-description)

---

## 1. What Is White Balance?

*(Applies to: Base, DualBase, Full, FullXR)*

### Color Image Acquisition

A color image acquisition involves the use of three color filters on the camera sensor. Each color filter restricts the light source to a range of wavelengths: red (R), green (G), or blue (B).

An ideal capture system renders a white object as a white image. A white stimulation should yield the same signal for R, G and B filters. But practically, there are always unavoidable defects that introduce a **white imbalance**.

### White Imbalance Factors

Several factors are responsible for the white imbalance:

- **Object illumination** -- The color of an object is a combination of its reflectivity and the spectral contents of the illuminating light.
- **Camera optical filters response**
- **Sensor sensitivity** -- not the same for the three wavelength ranges.
- **Different gain coefficients** applied to each color signal before digitization.

### White Balance Correction

MultiCam can correct the white imbalance:

- The **white balance operator** applies correcting coefficients (R, G, and B gains) to each color signal, so a white object renders a white image.
- The **white balance calibration** is the computation of the three R, G, and B gains. It can be automatic or manual.

---

## 2. White Balance Operator

The White Balance Operator is an element of the pixel processing chain. It is composed of 3 identical processing blocks, one for each color component. Each block contains:

- One **register** (holds the gain correction factor as a 16-bit unsigned binary value)
- One **multiplier** (handles 8-bit, 10-bit, 12-bit, 14-bit and 16-bit components)
- One **clipper** (clips to the maximum value of the digital output scale)

### Transfer Function

- Gain setting of 1.000 is the minimal allowed gain value (identity transfer).
- Gain setting of 2.000: output remains proportional to input until 100% full-scale is reached; for greater input values, the output is clipped to 100% full-scale.

The digital output scale is identical to the digital input scale. For example, for a camera delivering 10-bit components, the digital scale is [0..1023].

---

## 3. Automatic Calibration Description

The color calibration process takes place during the **first acquisition phase** of a MultiCam acquisition sequence when `WBO_Mode` is set to `ONCE`.

The color calibrator analyzes a rectangular area (**AWB_AREA**) of one uncorrected image and computes a correcting gain factor for each RGB color component.

Key behaviors:
- The correction factor for the component with the **strongest response** is always 1.
- Correction factors for weaker components are greater than 1.
- Accuracy is better than 1/1000 when requirements are fulfilled.

The calibrator returns a `NOT_OK` status when:
- Excessive color imbalance is detected.
- Not enough pixels satisfy the calibration target requirements in the AWB_AREA.

---

## 4. Automatic Calibration Requirements

### Image Source Equipment Requirements

The image source equipment (camera, lighting, optical elements) must exhibit:
- **Linear response:** The digital value of each color component must be proportional to the light intensity.
- **Moderate color imbalance:** The ratio between the strongest and weakest color component must be less than 5.

### Calibration Target Requirements

The calibration target is a neutral color object located in the field of view during the calibration process.

**Target forms:**
- Clustered light gray pixels in a specific area
- Non-clustered light gray pixels in a specific area
- Non-clustered light gray pixels anywhere in the field of view

**Target appearance:**
- Neutral light gray color
- Brightest component level: 75% to 90% of full scale
- Darkest component level: above 15% of full scale
- At least 256 pixels satisfying the appearance requirements

### Acquisition Channel Settings

- `WBO_Mode` must be set to `ONCE`
- The AWB_AREA must include at least 256 qualifying pixels
- The AWB_AREA must contain at least 1 line and 32 columns
- The AWB_AREA must be entirely within the Camera Active Area

For PC1624 Base, PC1623 DualBase, PC1622 Full, and PC1626 Full XR:
- The LUT Operator must be disabled
- The cropping area must encompass the AWB_AREA

---

## 5. Automatic Calibration Timing

The calibration process takes place during the first acquisition phase when `WBO_Mode` = `ONCE`.

1. The White Balance Operator is **disabled** before the sequence starts.
2. Calibration begins when the DMA transfer of the first phase completes.
3. The first `MC_SIG_SURFACE_PROCESSING` signal is **delayed** until calibration completes.

**On successful calibration:**
- `WBO_Status` is set to `OK`
- `WBO_GainR`, `WBO_GainG`, `WBO_GainB` are updated with calibration results
- The White Balance Operator is reconfigured with new settings

**On unsuccessful calibration:**
- `WBO_Status` is set to `NOT_OK`
- Original gain values are restored
- The White Balance Operator is reconfigured with original settings

---

## 6. AWB_AREA Settings Description

The **AWB_AREA** is a rectangular area within the Camera Active Window analyzed by the color balancing calibrator.

Size and position are defined by:

| Parameter | Description |
|-----------|-------------|
| `WBO_Width` | Width of the AWB_AREA |
| `WBO_Height` | Height of the AWB_AREA |
| `WBO_OrgX` | X-origin of the AWB_AREA |
| `WBO_OrgY` | Y-origin of the AWB_AREA |

The default size of the AWB_AREA is the whole Camera Active Area.
