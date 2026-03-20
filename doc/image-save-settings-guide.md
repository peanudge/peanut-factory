# Image Save Settings Guide

This guide explains how to configure where and how captured images are saved to the filesystem in PeanutVision.

---

## Overview

Every time you capture a frame (snapshot or trigger), PeanutVision can automatically save the image file to disk. You control the save location, file format, filename pattern, and folder organisation through **Image Save Settings**.

Settings are persisted in `image-save-settings.json` (next to `appsettings.json` in the app root) and take effect on the next capture — no restart required.

---

## Accessing the Settings

Open the **Acquisition** tab and expand the **Image Save Settings** accordion near the top of the page.

```
┌─ Image Save Settings ─────────────────────────────────────────────────┐
│  Output Directory  [CapturedImages                    ]               │
│  Format            [PNG ▼]                                            │
│  Filename Prefix   [capture        ]                                  │
│  Timestamp Format  [yyyyMMdd_HHmmss_fff               ]               │
│  Subfolder         [None ▼]                                           │
│                                                                       │
│  ✓ Auto-save on capture     □ Include sequence number                 │
│                                                                       │
│  Example: capture_20260320_143000_123.png   [Save Settings]           │
└───────────────────────────────────────────────────────────────────────┘
```

The **Example** line updates live as you edit fields, showing exactly what the next saved filename will look like. Click **Save Settings** to persist your changes.

---

## Settings Reference

### Output Directory

Where image files are written.

| Input | Resolved path |
|---|---|
| `CapturedImages` | `{app root}/CapturedImages/` |
| `D:/Vision/Images` | `D:/Vision/Images/` (absolute) |
| `../../output` | Relative to app root |

The directory is created automatically if it does not exist.

---

### Format

The file format used when saving.

| Value | Extension | Notes |
|---|---|---|
| **PNG** | `.png` | Lossless, good default for inspection images |
| **BMP** | `.bmp` | Uncompressed, larger files, fastest write |
| **RAW** | `.raw` | Raw pixel bytes, no header — for custom processing pipelines |

> The HTTP response (for UI display) is always PNG regardless of this setting.

---

### Filename Prefix

The leading text of every saved filename. Defaults to `capture`.

```
capture_20260320_143000_123.png   ← prefix is "capture"
peanut_20260320_143000_123.png    ← prefix is "peanut"
snap_20260320_143000_123.png      ← prefix is "snap"
```

Invalid filename characters (e.g. `/`, `\`, `:`, `*`) are not allowed.

---

### Timestamp Format

A [.NET DateTime format string](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings) embedded between the prefix and the extension.

| Format string | Example output |
|---|---|
| `yyyyMMdd_HHmmss_fff` | `20260320_143000_123` (default) |
| `yyyy-MM-dd_HH-mm-ss` | `2026-03-20_14-30-00` |
| `HHmmss` | `143000` |
| `yyyyMMddHHmmssfff` | `20260320143000123` |

The server validates this field — an invalid format string returns a `400` error before saving.

---

### Subfolder Strategy

Automatically organise files into subdirectories under the Output Directory.

| Value | Subfolder created | Example path |
|---|---|---|
| **None** | — | `CapturedImages/capture_20260320_143000_123.png` |
| **By Date** | `YYYY-MM-DD` | `CapturedImages/2026-03-20/capture_20260320_143000_123.png` |
| **By Session** | `session_{startup timestamp}` | `CapturedImages/session_20260320_142500/capture_20260320_143000_123.png` |
| **By Profile** | Camera profile filename | `CapturedImages/TC-A160K-SEM_freerun_RGB8.cam/capture_20260320_143000_123.png` |

**By Session** is useful when you run multiple test batches in a day — each app startup gets its own folder.
**By Profile** is useful when switching between camera configurations and keeping their images separated.

---

### Auto-save on Capture

When **enabled** (default), every snapshot and trigger automatically writes a file to disk.

When **disabled**, no files are written. The image is still returned to the UI for viewing and manual download, but nothing is saved to the filesystem. Useful when you only want to save selected frames manually.

---

### Include Sequence Number

When **enabled**, a zero-padded 5-digit counter is appended to each filename. The counter resets when the app restarts.

```
capture_20260320_143000_123_00001.png
capture_20260320_143001_456_00002.png
capture_20260320_143002_789_00003.png
```

Useful when multiple captures happen within the same millisecond (e.g. rapid burst triggering), or when you want a guaranteed-unique ordering independent of the timestamp.

---

## Saved Path in the UI

After each capture, the path of the saved file is shown beneath the image viewer:

```
[Download]  💾 CapturedImages/2026-03-20/capture_20260320_143000_123.png
```

Hover over the path to see the full absolute path in a tooltip if it is truncated.

If **Auto-save** is disabled, this label does not appear.

---

## Common Recipes

### Daily inspection run — separate folder per day

```
Output Directory:   InspectionImages
Format:             PNG
Prefix:             inspection
Timestamp Format:   yyyyMMdd_HHmmss_fff
Subfolder:          By Date
Auto-save:          ✓
Sequence number:    □
```

Result:
```
InspectionImages/
  2026-03-20/
    inspection_20260320_090001_000.png
    inspection_20260320_090045_123.png
  2026-03-21/
    inspection_20260320_083012_456.png
```

---

### ML dataset collection — numbered files, no timestamp noise

```
Output Directory:   D:/Datasets/peanut-v2
Format:             PNG
Prefix:             sample
Timestamp Format:   yyyyMMdd
Subfolder:          By Profile
Auto-save:          ✓
Sequence number:    ✓
```

Result:
```
D:/Datasets/peanut-v2/
  TC-A160K-SEM_freerun_RGB8.cam/
    sample_20260320_00001.png
    sample_20260320_00002.png
    sample_20260320_00003.png
```

---

### Raw pipeline output — no compression

```
Output Directory:   RawOutput
Format:             RAW
Prefix:             frame
Timestamp Format:   HHmmss_fff
Subfolder:          By Session
Auto-save:          ✓
Sequence number:    □
```

Result:
```
RawOutput/
  session_20260320_090000/
    frame_090001_000.raw
    frame_090045_123.raw
```

---

## API Reference

Settings can also be read and updated programmatically.

### Read current settings

```http
GET /api/settings/image-save
```

```json
{
  "outputDirectory": "CapturedImages",
  "format": "png",
  "filenamePrefix": "capture",
  "timestampFormat": "yyyyMMdd_HHmmss_fff",
  "includeSequenceNumber": false,
  "subfolderStrategy": "none",
  "autoSave": true
}
```

### Update settings

```http
PUT /api/settings/image-save
Content-Type: application/json

{
  "outputDirectory": "D:/Images",
  "format": "bmp",
  "filenamePrefix": "peanut",
  "timestampFormat": "yyyyMMdd_HHmmss_fff",
  "includeSequenceNumber": true,
  "subfolderStrategy": "byDate",
  "autoSave": true
}
```

**Enum values:**

| Field | Accepted values |
|---|---|
| `format` | `png`, `bmp`, `raw` |
| `subfolderStrategy` | `none`, `byDate`, `bySession`, `byProfile` |

**Validation errors** return `400`:

```json
{
  "errors": ["FilenamePrefix contains invalid filename characters"]
}
```

### Saved path header

After a successful trigger or snapshot, the response includes:

```
X-Image-Path: C:\app\CapturedImages\2026-03-20\capture_20260320_143000_123.png
```

This header is absent if `autoSave` is `false`.

---

## Settings File Location

`image-save-settings.json` is stored alongside `appsettings.json` in the application root:

```
PeanutVision.Api/
  appsettings.json
  image-save-settings.json   ← created on first Save Settings
  CamFiles/
  CapturedImages/
```

The file is created the first time you click **Save Settings** in the UI or call `PUT /api/settings/image-save`. Before that, all defaults are used. You can edit it directly in a text editor; the new values are loaded on the next app startup.

---

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Files not appearing on disk | Auto-save is disabled | Enable **Auto-save on capture** and save |
| `400` on Save Settings | Invalid prefix or timestamp format | Check the error message; remove special characters from prefix, or use a valid .NET format string |
| Files saved to wrong location | Relative path resolved from wrong root | Use an absolute path, or check `ContentRootPath` in the API logs |
| Two captures get the same filename | Sub-millisecond captures with no sequence number | Enable **Include sequence number** |
| RAW file unreadable | RAW has no header — dimensions and format must be known | Record image dimensions from the API status; width × height × bytes-per-pixel = expected file size |
