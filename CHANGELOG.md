# Changelog

All notable changes to PeanutVision are documented here.

---

## [Unreleased] — 2026-04-01

### Image Viewer

- **Zoom & Pan** (#69) — Mouse wheel zoom (1×–10×) and drag-to-pan on the 4K image viewer. Double-click resets to fit view. Zoom level badge shown in the top-right corner. Zoom resets automatically when a new live frame arrives. No additional packages required.

- **RGB Histogram** (#70) — Brightness distribution histogram displayed below the image viewer whenever a saved image is selected. R/G/B channels rendered as overlapping SVG area fills (normalized, 256 bins). Powered by a new `GET /api/images/{id}/histogram` endpoint using SixLabors.ImageSharp. Hidden during live preview.

- **Image Annotations** (#74) — Tags and freeform notes can now be added to any captured image. Tags are shown as MUI chips with add/remove controls. Notes auto-save on blur (500ms debounce) with a subtle "Saved" confirmation. Backed by a new `PATCH /api/images/{id}` endpoint and an EF Core migration (`AddImageAnnotations`) that adds `Tags` (JSON array) and `Notes` columns to the database.

### Image Gallery

- **Date Range & Format Filter** (#72) — Filter the gallery by capture date range (from/to) and file format (PNG / BMP / RAW). Filters are additive; clearing them restores the full catalog. Page resets to 1 on any filter change. Backed by new `from`, `to`, and `format` query parameters on `GET /api/images`.

- **Bulk ZIP Export** (#73) — Select mode in the gallery (checkbox icon in header) allows multi-image selection with "All / None" shortcuts. "Export ZIP" downloads selected images as a streaming ZIP archive via `POST /api/images/export`. Exiting select mode or pressing Escape deselects all. Empty selection exports all images.

### Capture Workflow

- **Keyboard Shortcuts** (#71) — Global hotkeys for the capture workflow:

  | Key | Action |
  |-----|--------|
  | `Space` | Snapshot (when idle) |
  | `Ctrl+R` | Start / stop continuous capture |
  | `Delete` | Delete selected image |
  | `←` / `→` | Previous / next image |
  | `Escape` | Return to live view |
  | `?` | Show keyboard shortcuts help |

  Shortcuts are disabled when focus is inside an input, textarea, or select element. Shortcut hints are shown in button tooltips.

- **Sidebar Tab Persistence** (#75) — The sidebar remembers the last selected tab (Capture / Camera / Settings) across page reloads via `localStorage`.

### Reliability

- **Error Boundaries** (#76) — React error boundaries wrap the image gallery, image viewer, sidebar panel, and event log independently. A component crash in one section no longer takes down the rest of the UI. Each section shows a "Something went wrong" fallback with a retry button.

- **API Request Timeouts & Retry** (#77) — All API calls now time out after 10 seconds (30 seconds for calibration endpoints). Network errors and 5xx responses are retried up to 3 times with exponential backoff (1s → 2s → 4s). 4xx errors fail immediately without retrying.

### Tests

- **Vitest Unit Tests** (#78) — 64 frontend unit tests added and passing, covering:
  - `useImageGallery` — filter state, page reset behaviour
  - `useKeyboardShortcuts` — key dispatch and input-focus guard
  - `useCaptureLog` — FIFO eviction and selection behaviour
  - `ImageGallery` — format dropdown, filter clear button visibility
  - `ErrorBoundary` — normal render and fallback UI on throw

- **Backend API Tests** — 18 new xUnit tests across three new spec files:
  - `ImagesFilterSpec` (9 tests) — date/format filter combinations
  - `ImagesHistogramSpec` (5 tests) — channel structure, normalization, 404
  - `ImagesAnnotationsSpec` (4 tests) — PATCH, 404, clear, GET shape

---

## Prior releases

| Commit | Date | Summary |
|--------|------|---------|
| `748e675` | 2026-03-xx | Captures UX improvements + UseExceptionHandler hotfix (#68) |
| `61d6cbc` | 2026-03-xx | Trigger-to-frame latency analysis backoffice tab (#67) |
| `0a0a798` | 2026-03-xx | Persistent image catalog with thumbnail gallery (#66) |
| `d3c3098` | 2026-03-xx | Show captured image when clicking thumbnail in continuous mode (#65) |
| `b82c1e0` | 2026-03-xx | Image gallery UX — thumbnails, viewer sync, mode badge (#64) |
| `b60c722` | 2026-03-xx | Default interval time and validation for continuous mode (#63) |
| `662bf61` | 2026-03-xx | Capture gallery, live preview, and acquisition stop after frame count (#62) |
