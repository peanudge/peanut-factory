# Serial Communication

> Source: Euresys Grablink 6.19.4 (Doc D411EN)

## Contents

1. [Serial Link Overview](#1-serial-link-overview)
2. [Serial Link for Lite Configuration](#2-serial-link-for-lite-configuration)
3. [Serial Link for Base/Medium/Full/80-bit Configurations](#3-serial-link-for-basemediumfull80-bit-configurations)
4. [Serial Link Functionalities](#4-serial-link-functionalities)
5. [Supported Baud Rates](#5-supported-baud-rates)

---

## 1. Serial Link Overview

Grablink boards support **asynchronous full-duplex serial communication** between the camera and the frame grabber as defined by the Camera Link standard.

---

## 2. Serial Link for Lite Configuration

For cameras using the **Lite** configuration, the downstream serial communication link does not use a dedicated line pair but, instead, is embedded in the Channel Link.

---

## 3. Serial Link for Base/Medium/Full/80-bit Configurations

For cameras using the **Base**, **Medium**, **Full**, or **80-bit** configurations, two differential line pairs of the first camera cable are dedicated to the serial communication, one for each direction:

- **SerTFG** -- Downstream (camera to frame grabber) serial communication link
- **SerTC** -- Upstream (frame grabber to camera) serial communication link

---

## 4. Serial Link Functionalities

The application software controls the serial communication channel through the standardized API defined by the Camera Link standard.

Alternatively, it can also be controlled using **virtual COM ports**. The application must set the appropriate values to the parameters `SerialControlA` and `SerialControlB` respectively.

---

## 5. Supported Baud Rates

The following baud rates are supported:

600, 1200, 1800, 2400, 3600, 4800, 7200, **9600** (Default), 14400, 19200, 28800, 38400, 57600, 115200, and 230400.
