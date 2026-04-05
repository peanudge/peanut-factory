# Remove Session Concept — Design Spec

## Goal

Remove the over-engineered Session concept entirely. Images become a flat list sorted by date. Gallery gets a date-based filter to replace session-based filtering.

## Motivation

Session added full lifecycle management (create, end, delete, history, notes, active tracking) to solve a simple grouping problem. The same result is achieved with date-based organization at a fraction of the complexity.

---

## What Gets Deleted

### Backend
| File | Action |
|------|--------|
| `Services/Session.cs` | Delete |
| `Services/ISessionRepository.cs` | Delete |
| `Services/SessionRepository.cs` | Delete |
| `Controllers/SessionController.cs` | Delete (7 endpoints) |

### Backend — Modified
| File | Change |
|------|--------|
| `Services/CapturedImage.cs` | Remove `SessionId` (nullable Guid) and `Session` navigation property |
| `Services/AppDbContext.cs` | Remove `Sessions` DbSet and FK relationship on `CapturedImage` |
| `Controllers/AcquisitionController.cs` | Remove `ISessionRepository` constructor param and `GetActiveAsync()` call in `SaveAndRecordAsync` |
| `Program.cs` | Remove `ISessionRepository`/`SessionRepository` DI registration |

### Frontend
| File | Action |
|------|--------|
| `components/SessionSelector.tsx` | Delete |
| `api/types.ts` | Remove `Session` interface; remove `sessionId` from `CapturedImageRecord` |
| `api/client.ts` | Remove `getSessions`, `getActiveSession`, `createSession`, `endSession`, `deleteSession` |

### Database
Delete the SQLite DB file (`peanut_vision.db` or equivalent). EF Core recreates it from the updated schema on next startup.

---

## What Changes

### SubfolderStrategy
- Remove `BySession` option
- Add `ByDate` — images saved to `{ImageOutputDirectory}/{YYYY-MM-DD}/{filename}.png`
- Default becomes `ByDate`

### Image Repository
`ICapturedImageRepository.GetAllAsync()` and `CapturedImageRepository`:
- Remove `sessionId` parameter
- Add `date` parameter (`DateOnly?`) — filters images where `DATE(capturedAt) = date`
- All images remain sorted by `capturedAt` descending

### API
- `GET /api/images?sessionId={guid}` → `GET /api/images?date=2026-04-06`
- Response shape unchanged (minus `sessionId` field on each record)

### Frontend — `useImageGallery` hook
- `filterSessionId: string | null` → `filterDate: string | null` (ISO date `"YYYY-MM-DD"`)
- `setFilterSessionId` → `setFilterDate`
- `listImages({ sessionId })` → `listImages({ date })`

---

## What Gets Added

### Gallery Date Filter UI
- Native `<input type="date">` in the Gallery toolbar (no new npm dependency)
- Selecting a date sets `filterDate` and refetches images for that day
- Clear button resets `filterDate` to `null` and shows all images
- `SessionSelector` component removed from wherever it currently renders

---

## Out of Scope
- Date grouping / dividers within the gallery list (not requested)
- Keeping any session-related code "just in case"
- EF Core migration file — DB is reset, not migrated

---

## Success Criteria
- Zero references to `Session`, `ISessionRepository`, `SessionSelector` in the codebase
- `/api/sessions/*` endpoints no longer exist
- Images save to date-based subfolders automatically
- Gallery date filter shows only images from the selected date
- All existing tests pass; new date-filter behavior covered by tests
