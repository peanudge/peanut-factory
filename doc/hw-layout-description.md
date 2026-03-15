# Peanut Discoloration Inspection Hardware Layout

> Source: Drawing DWG-NO. 19001 Rev 1.0 -- "땅콩 변색 검사 개념도" (Peanut Discoloration Inspection Concept Diagram), www.mv21.kr

## Contents

1. [Purpose](#1-purpose)
2. [Camera System](#2-camera-system)
3. [Lens Configuration](#3-lens-configuration)
4. [Field of View](#4-field-of-view)
5. [Lighting System](#5-lighting-system)
6. [Working Distances](#6-working-distances)
7. [Mechanical Notes](#7-mechanical-notes)
8. [Projection Standard](#8-projection-standard)

---

## 1. Purpose

The system performs automated color and discoloration inspection of peanuts (땅콩 변색 검사). Three area-scan cameras capture overlapping images that are stitched into a single wide field of view covering a 453 mm horizontal conveyor zone.

---

## 2. Camera System

| Parameter | Value |
|-----------|-------|
| Camera model | Crevis TC-A160K-102 |
| Number of cameras | 3 (arranged in a horizontal row) |
| Camera spacing (center-to-center) | 2-150 mm |
| Camera view hole diameter | 55 mm |
| Interface | Camera Link |

The three cameras are mounted side by side, each looking down through a 55 mm diameter viewing aperture.

---

## 3. Lens Configuration

| Parameter | Value |
|-----------|-------|
| Lens model | M0814-MP2 |
| Focal length | 8 mm |
| Aperture | f/1.4 |
| Mount | C-mount (megapixel rated) |

---

## 4. Field of View

| Parameter | Value |
|-----------|-------|
| FOV per camera | 153 (H) x 114 (V) mm |
| Total combined FOV | 453 (H) x 114 (V) mm |
| Camera overlap (adjacent) | 2-3 mm |

The three individual FOVs are arranged horizontally with a 2-3 mm overlap between adjacent camera views, yielding a total inspection width of approximately 453 mm.

---

## 5. Lighting System

| Parameter | Value |
|-----------|-------|
| Type | Strobe (스트로브 타입) |
| Emitting area (horizontal bar) | 160 (V) x 600 (H) mm |
| Side light emitting height | 160 (V) mm |
| Light working distance | 215 +/- 15 mm |

A large horizontal bar light spans the full width of the three-camera array, with side illumination panels to reduce shadow artifacts.

---

## 6. Working Distances

| Measurement | Distance |
|-------------|----------|
| Lens working distance (WD) | 240 +/- 15 mm |
| Light working distance | 215 +/- 15 mm |

The light is mounted closer to the inspection surface than the cameras, positioned in front of the lens plane.

---

## 7. Mechanical Notes

1. **FOV interference concern:** The lighting must be fixed in front of the lens assembly to prevent FOV interference between the cameras and the light fixtures (화각 간섭 우려로 인한 조명 렌즈 앞단에 고정 필요).
2. **Coupled WD adjustment:** When adjusting the lens working distance, the lighting working distance must also be adjusted correspondingly (Lens WD 조정시 조명도 같이 조정 필요).

---

## 8. Projection Standard

The drawing uses 3rd angle projection (제3각법).
