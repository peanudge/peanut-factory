# Acquisition Panel Redesign

**Date:** 2026-06-07
**Status:** Approved

---

## Problem

The current Acquisition page has two separate input paths (Manual / Preset) that split the flow and create confusion. The panel is on the left, pushing the image viewer to the right. Form inputs use heavily customized styles that add visual noise without improving usability.

---

## Goal

A single unified flow: fill in settings → choose trigger mode → start. Presets serve as a "quick start" shortcut within that same panel. The image viewer gets the main space; the settings panel moves to the right.

---

## Layout

```
┌──────────────────────────────┬────────────────────┐
│                              │                    │
│        Image Viewer          │   Settings Panel   │
│         (flex: 1)            │     (320px fixed)  │
│                              │                    │
└──────────────────────────────┴────────────────────┘
```

- Image viewer: left, `flex: 1`, takes all remaining space
- Settings panel: right, 320px fixed, `border-left` separator
- Panel is scrollable if content overflows

---

## Panel Structure (Idle State)

Top-to-bottom single flow. No tabs, no mode toggle.

### 1. 빠른 시작 (Quick Start Presets)

- Collapsible section at the top (open by default if presets exist, hidden if none)
- Presets rendered as chips/tags in a wrapping row
- Clicking a preset chip → immediately calls Start with that preset's config
- While starting: chip shows spinner, all chips disabled
- Chip shows preset name only; full config visible on hover via `title` attribute

### 2. 촬영 설정 (Settings Form)

All inputs use **native browser elements** — no custom styling beyond layout spacing and the project's existing CSS variables for color. Specifically:

- **카메라 프로파일**: native `<select>`. While loading: `disabled` + single `<option>` "로딩 중…"
- **저장 경로**: native `<input type="text">` + native `<button>` (📁 browse). Browse button disabled + spinner while file browser is opening
- **포맷**: three native `<input type="radio">` buttons labeled PNG / BMP / RAW, displayed inline
- **프레임 수**: native `<input type="number">` with `placeholder="∞ (제한없음)"`. Empty = unlimited

### 3. 촬영 방식 (Trigger Mode)

Positioned directly above Start — "how to start" rather than a general setting.

- Two native radio options: **자동** / **수동**
- Selecting 자동 reveals an interval input inline below: `<input type="number">` + "초" label
- Minimum interval: 0.05s

### 4. 촬영 시작

- Full-width `<button>` labeled "촬영 시작" — primary color, prominent
- Disabled + spinner (Loader2 icon, rotating) while server responds to Start request
- Below the button: small text link "프리셋으로 저장 +" that opens a modal

### 프리셋 저장 Modal

- Single `<input type="text">` for preset name
- Summary line showing current config (profile · frame count · interval)
- Save button: disabled + spinner while saving; Cancel button always enabled

---

## Active State (During Capture)

When acquisition is running, the settings form is replaced entirely by a status view.

```
┌────────────────────┐
│  ● 촬영 중          │
│  {profile name}    │
├────────────────────┤
│  Frames   {n}      │
│  FPS      {n.n}    │
│  Mode     자동|수동 │
│  Save to  {path}   │
│  Dropped  {n}      │  ← shown only if > 0
│  Error    {msg}    │  ← shown only if present
├────────────────────┤
│  [ Trigger ]       │  ← shown only in 수동 mode
│                    │    disabled + spinner while pending
├────────────────────┤
│  [ ■ 촬영 중지 ]   │
│                    │    disabled + spinner while stopping
└────────────────────┘
```

---

## Loading States Summary

| Operation | Element behavior |
|-----------|-----------------|
| Camera profile list loading | `<select>` disabled, single option "로딩 중…" |
| Preset list loading | Section shows 2–3 skeleton placeholder lines |
| Start | Start button disabled + Loader2 spinner, all preset chips disabled |
| Stop | Stop button disabled + Loader2 spinner |
| Trigger | Trigger button disabled + Loader2 spinner |
| Save preset | Modal Save button disabled + spinner |
| File browser open | Browse (📁) button disabled + spinner |

**Rules:**
- Never block the full panel with an overlay — loading is per-element
- Buttons always go `disabled` during pending requests to prevent duplicate submissions
- Spinner replaces the button label/icon — no separate spinner element next to text

---

## Components Affected

| Component | Change |
|-----------|--------|
| `Acquisition/index.tsx` | Swap sidebar from left to right; remove `inputMode` / `selectedPreset` state |
| `Acquisition/index.module.scss` | Flip flex direction; rename/clean sidebar styles |
| `CaptureTab.tsx` | Remove `IdleView` / `ManualForm` / `PresetForm` / `InputModeToggle` split; merge into single `IdleView` |
| `AcquisitionSettings/index.tsx` | Rewrite: native inputs, trigger mode moved in, presets section added at top |
| `AcquisitionSettings/index.module.scss` | Simplify significantly — mostly spacing only |
| `useAcquisitionConfig.ts` | Remove `inputMode` if present; no other logic change |

---

## Out of Scope

- Preset editing (rename, reorder) — manage via save/delete only
- Keyboard shortcuts
- Mobile/responsive layout
- Any change to the backend API or data model
