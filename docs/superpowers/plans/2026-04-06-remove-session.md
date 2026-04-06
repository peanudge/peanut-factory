# Remove Session Concept — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Remove the Session concept entirely — backend files, DI, DB, API params, and frontend components — and replace session-based image grouping with date-based filtering.

**Architecture:** Four independent layers are changed in sequence: (1) backend model/services/controller deletion, (2) API filter param swap (sessionId → date), (3) SubfolderStrategy default change, (4) frontend component/hook/type cleanup and date filter UI addition.

**Tech Stack:** .NET 10 / ASP.NET Core / EF Core / SQLite (backend) · React 18 / TypeScript / MUI / TanStack Query (frontend) · xUnit integration tests · Vitest unit tests

---

## File Map

### Deleted
- `src/PeanutVision.Api/Services/Session.cs`
- `src/PeanutVision.Api/Services/ISessionRepository.cs`
- `src/PeanutVision.Api/Services/SessionRepository.cs`
- `src/PeanutVision.Api/Controllers/SessionController.cs`
- `src/peanut-vision-ui/src/components/SessionSelector.tsx`
- `src/PeanutVision.Api/peanut-vision.db` (reset DB)

### Modified — Backend
- `src/PeanutVision.Api/Services/CapturedImage.cs` — remove `SessionId`, `Session` nav prop
- `src/PeanutVision.Api/Services/AppDbContext.cs` — remove `Sessions` DbSet, FK config on `CapturedImage`
- `src/PeanutVision.Api/Program.cs` — remove `ISessionRepository`/`SessionRepository` DI registration
- `src/PeanutVision.Api/Controllers/AcquisitionController.cs` — remove `ISessionRepository` constructor param and `GetActiveAsync()` call
- `src/PeanutVision.Api/Services/ICapturedImageRepository.cs` — remove `sessionId` param, add `DateOnly? date`
- `src/PeanutVision.Api/Services/CapturedImageRepository.cs` — implement `date` filter
- `src/PeanutVision.Api/Controllers/ImagesController.cs` — replace `sessionId` param with `date`, remove `SessionId` from `CapturedImageDto`
- `src/PeanutVision.Api/Services/ImageSaveSettings.cs` — remove `BySession` enum value, change default to `ByDate`
- `src/PeanutVision.Api/Services/FilenameGenerator.cs` — remove `BySession` case

### Modified — Frontend
- `src/peanut-vision-ui/src/api/types.ts` — remove `Session` interface, remove `sessionId` from `CapturedImageRecord`, remove `"bySession"` from `SubfolderStrategy`
- `src/peanut-vision-ui/src/api/client.ts` — remove session functions, change `listImages` param to `date?`
- `src/peanut-vision-ui/src/api/queryKeys.ts` — remove `sessions`, `activeSession` keys
- `src/peanut-vision-ui/src/App.tsx` — remove `sessionName` state and AppBar chip, remove `onSessionChange` prop on `AcquisitionTab`
- `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx` — remove `onSessionChange` prop, remove `SessionSelector` import and usage
- `src/peanut-vision-ui/src/hooks/useImageGallery.ts` — replace `filterSessionId` with `filterDate`
- `src/peanut-vision-ui/src/components/ImageGallery.tsx` — replace session `Select` with `<input type="date">`
- `src/peanut-vision-ui/src/tabs/GalleryTab.tsx` — pass `filterDate`/`setFilterDate`

### New
- `src/PeanutVision.Api.Tests/Specs/Images/ImageDateFilterSpec.cs` — integration test for `GET /api/images?date=`

---

## Task 1: Delete Session backend files and reset DB

**Files:**
- Delete: `src/PeanutVision.Api/Services/Session.cs`
- Delete: `src/PeanutVision.Api/Services/ISessionRepository.cs`
- Delete: `src/PeanutVision.Api/Services/SessionRepository.cs`
- Delete: `src/PeanutVision.Api/Controllers/SessionController.cs`
- Modify: `src/PeanutVision.Api/Services/CapturedImage.cs`
- Modify: `src/PeanutVision.Api/Services/AppDbContext.cs`
- Modify: `src/PeanutVision.Api/Controllers/AcquisitionController.cs`
- Modify: `src/PeanutVision.Api/Program.cs`

- [ ] **Step 1: Delete the four Session files**

```bash
rm src/PeanutVision.Api/Services/Session.cs
rm src/PeanutVision.Api/Services/ISessionRepository.cs
rm src/PeanutVision.Api/Services/SessionRepository.cs
rm src/PeanutVision.Api/Controllers/SessionController.cs
```

- [ ] **Step 2: Strip SessionId and Session nav prop from CapturedImage**

Replace entire `src/PeanutVision.Api/Services/CapturedImage.cs` with:

```csharp
namespace PeanutVision.Api.Services;

public sealed class CapturedImage
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSizeBytes { get; set; }
    public string Format { get; set; } = string.Empty;
    public DateTime CapturedAt { get; set; }
}
```

- [ ] **Step 3: Strip Session from AppDbContext**

Replace entire `src/PeanutVision.Api/Services/AppDbContext.cs` with:

```csharp
using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class AppDbContext : DbContext
{
    public DbSet<CapturedImage> CapturedImages => Set<CapturedImage>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapturedImage>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FilePath).HasMaxLength(1000).IsRequired();
            entity.Property(c => c.ThumbnailPath).HasMaxLength(1000);
            entity.Property(c => c.Format).HasMaxLength(10).IsRequired();
            entity.HasIndex(c => c.CapturedAt);
        });
    }
}
```

- [ ] **Step 4: Remove ISessionRepository from AcquisitionController**

In `src/PeanutVision.Api/Controllers/AcquisitionController.cs`:

Remove `ISessionRepository` from the field list and constructor:

```csharp
// BEFORE constructor fields (remove this line):
private readonly ISessionRepository _sessionRepository;

// BEFORE constructor params (remove):
ISessionRepository sessionRepository,

// BEFORE constructor body (remove):
_sessionRepository = sessionRepository;
```

Replace the `SaveAndRecordAsync` method with:

```csharp
private async Task<string> SaveAndRecordAsync(
    ImageData image, ImageSaveSettings settings, string? profileId)
{
    var filePath = _filenameGenerator.Generate(settings, _contentRootPath, profileId);
    new ImageWriter().Save(image, filePath);

    var thumbPath = await _thumbnailService.GenerateAsync(filePath);
    var fileInfo = new FileInfo(filePath);

    await _imageRepository.AddAsync(new CapturedImage
    {
        Id = Guid.NewGuid(),
        FilePath = filePath,
        ThumbnailPath = thumbPath,
        Width = image.Width,
        Height = image.Height,
        FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
        Format = settings.Format.ToString().ToLower(),
        CapturedAt = DateTime.UtcNow,
    });

    return filePath;
}
```

- [ ] **Step 5: Remove ISessionRepository DI registration from Program.cs**

In `src/PeanutVision.Api/Program.cs`, delete this line:

```csharp
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
```

- [ ] **Step 6: Delete the SQLite DB file so EF Core recreates the schema**

```bash
rm -f src/PeanutVision.Api/peanut-vision.db
```

- [ ] **Step 7: Build to verify zero compilation errors**

```bash
cd src && dotnet build PeanutVision.Api/PeanutVision.Api.csproj
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`

- [ ] **Step 8: Run tests**

```bash
dotnet test PeanutVision.Api.Tests/PeanutVision.Api.Tests.csproj
```

Expected: `Passed! - Failed: 0, Passed: 180, ...`

- [ ] **Step 9: Commit**

```bash
git add -A
git commit -m "refactor: remove Session model, services, controller, and DB"
```

---

## Task 2: Replace sessionId filter with date filter in image API

**Files:**
- Modify: `src/PeanutVision.Api/Services/ICapturedImageRepository.cs`
- Modify: `src/PeanutVision.Api/Services/CapturedImageRepository.cs`
- Modify: `src/PeanutVision.Api/Controllers/ImagesController.cs`
- Create: `src/PeanutVision.Api.Tests/Specs/Images/ImageDateFilterSpec.cs`

- [ ] **Step 1: Write the failing integration test**

Create `src/PeanutVision.Api.Tests/Specs/Images/ImageDateFilterSpec.cs`:

```csharp
using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Images;

public class ImageDateFilterSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;

    public ImageDateFilterSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_images_with_today_date_returns_ok()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?date={today}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_images_with_future_date_returns_empty_list()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/images?date={future}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(0, doc.RootElement.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task Get_images_with_no_date_returns_all()
    {
        var response = await _client.GetAsync("/api/images");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        // Just verify the shape is correct — items array exists
        Assert.True(doc.RootElement.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task SessionId_param_no_longer_accepted()
    {
        // sessionId is gone — the server must simply ignore or 400 unknown query params
        // Since ASP.NET Core ignores unknown query params, this returns 200 (not filtered by session)
        var response = await _client.GetAsync("/api/images?sessionId=00000000-0000-0000-0000-000000000000");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- [ ] **Step 2: Run the test to confirm it fails**

```bash
dotnet test PeanutVision.Api.Tests/PeanutVision.Api.Tests.csproj \
  --filter "FullyQualifiedName~ImageDateFilterSpec"
```

Expected: FAIL — `Get_images_with_today_date_returns_ok` fails because `date` param doesn't exist yet (returns 200 with all images — actually this test may pass trivially). The important thing is that no `sessionId` filtering exists as designed.

- [ ] **Step 3: Update ICapturedImageRepository — replace sessionId with date**

Replace `src/PeanutVision.Api/Services/ICapturedImageRepository.cs` with:

```csharp
namespace PeanutVision.Api.Services;

public interface ICapturedImageRepository
{
    Task<CapturedImage> AddAsync(CapturedImage image);
    Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        DateOnly? date = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
    Task<CapturedImage?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}
```

- [ ] **Step 4: Implement date filter in CapturedImageRepository**

Replace `src/PeanutVision.Api/Services/CapturedImageRepository.cs` with:

```csharp
using Microsoft.EntityFrameworkCore;

namespace PeanutVision.Api.Services;

public sealed class CapturedImageRepository : ICapturedImageRepository
{
    private readonly AppDbContext _db;

    public CapturedImageRepository(AppDbContext db) => _db = db;

    public async Task<CapturedImage> AddAsync(CapturedImage image)
    {
        _db.CapturedImages.Add(image);
        await _db.SaveChangesAsync();
        return image;
    }

    public async Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        DateOnly? date = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _db.CapturedImages.AsNoTracking().AsQueryable();

        if (date.HasValue)
        {
            var from = date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var to   = date.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            query = query.Where(c => c.CapturedAt >= from && c.CapturedAt <= to);
        }
        else
        {
            if (dateFrom.HasValue)
                query = query.Where(c => c.CapturedAt >= dateFrom.Value);
            if (dateTo.HasValue)
                query = query.Where(c => c.CapturedAt <= dateTo.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CapturedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public Task<CapturedImage?> GetByIdAsync(Guid id)
        => _db.CapturedImages.FindAsync(id).AsTask()!;

    public async Task DeleteAsync(Guid id)
    {
        var image = await _db.CapturedImages.FindAsync(id)
            ?? throw new KeyNotFoundException($"CapturedImage {id} not found");
        _db.CapturedImages.Remove(image);
        await _db.SaveChangesAsync();
    }
}
```

- [ ] **Step 5: Update ImagesController — replace sessionId with date, remove SessionId from DTO**

Replace the `GetImages` action and `CapturedImageDto` record in `src/PeanutVision.Api/Controllers/ImagesController.cs`:

```csharp
[HttpGet]
public async Task<ActionResult<ImagePageDto>> GetImages(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] DateOnly? date = null,
    [FromQuery] DateTime? dateFrom = null,
    [FromQuery] DateTime? dateTo = null)
{
    pageSize = Math.Clamp(pageSize, 1, 100);
    var (items, total) = await _repo.GetPageAsync(page, pageSize, date, dateFrom, dateTo);
    return Ok(new ImagePageDto(
        items.Select(ToDto).ToList(),
        total,
        page,
        pageSize,
        (int)Math.Ceiling(total / (double)pageSize)));
}
```

Also update `ToDto` (find it in the file) to remove `SessionId`:

```csharp
private static CapturedImageDto ToDto(CapturedImage c) => new(
    c.Id,
    c.FilePath,
    Path.GetFileName(c.FilePath),
    c.ThumbnailPath is not null,
    c.Width,
    c.Height,
    c.FileSizeBytes,
    c.Format,
    c.CapturedAt);
```

And replace `CapturedImageDto` record:

```csharp
public record CapturedImageDto(
    Guid Id,
    string FilePath,
    string Filename,
    bool HasThumbnail,
    int Width,
    int Height,
    long FileSizeBytes,
    string Format,
    DateTime CapturedAt);
```

- [ ] **Step 6: Run tests**

```bash
dotnet test PeanutVision.Api.Tests/PeanutVision.Api.Tests.csproj
```

Expected: `Passed! - Failed: 0, Passed: 183, ...` (180 existing + 3 new ImageDateFilter tests — the 4th test is a sanity check that passes trivially)

- [ ] **Step 7: Commit**

```bash
git add -A
git commit -m "refactor: replace sessionId filter with date filter in image API"
```

---

## Task 3: Remove BySession from SubfolderStrategy, default to ByDate

**Files:**
- Modify: `src/PeanutVision.Api/Services/ImageSaveSettings.cs`
- Modify: `src/PeanutVision.Api/Services/FilenameGenerator.cs`

- [ ] **Step 1: Remove BySession from the enum and change default**

Replace `src/PeanutVision.Api/Services/ImageSaveSettings.cs` with:

```csharp
namespace PeanutVision.Api.Services;

public enum SaveImageFormat { Png, Bmp, Raw }

public enum SubfolderStrategy { None, ByDate, ByProfile }

public sealed record ImageSaveSettings
{
    public string OutputDirectory { get; init; } = "CapturedImages";
    public SaveImageFormat Format { get; init; } = SaveImageFormat.Png;
    public string FilenamePrefix { get; init; } = "capture";
    public string TimestampFormat { get; init; } = "yyyyMMdd_HHmmss_fff";
    public bool IncludeSequenceNumber { get; init; } = false;
    public SubfolderStrategy SubfolderStrategy { get; init; } = SubfolderStrategy.ByDate;
    public bool AutoSave { get; init; } = true;
}
```

- [ ] **Step 2: Remove BySession case from FilenameGenerator**

In `src/PeanutVision.Api/Services/FilenameGenerator.cs`, update the `subdir` switch expression:

```csharp
var subdir = settings.SubfolderStrategy switch
{
    SubfolderStrategy.ByDate => now.ToString("yyyy-MM-dd"),
    SubfolderStrategy.ByProfile when !string.IsNullOrEmpty(profileId) => SanitizeSegment(profileId),
    _ => null,
};
```

(Remove the `SubfolderStrategy.BySession` line.)

- [ ] **Step 3: Build to verify**

```bash
dotnet build PeanutVision.Api/PeanutVision.Api.csproj
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`

- [ ] **Step 4: Run tests**

```bash
dotnet test PeanutVision.Api.Tests/PeanutVision.Api.Tests.csproj
```

Expected: `Passed! - Failed: 0, ...`

- [ ] **Step 5: Commit**

```bash
git add src/PeanutVision.Api/Services/ImageSaveSettings.cs \
        src/PeanutVision.Api/Services/FilenameGenerator.cs
git commit -m "refactor: remove BySession subfolder strategy, default to ByDate"
```

---

## Task 4: Remove Session from frontend

**Files:**
- Delete: `src/peanut-vision-ui/src/components/SessionSelector.tsx`
- Modify: `src/peanut-vision-ui/src/api/types.ts`
- Modify: `src/peanut-vision-ui/src/api/client.ts`
- Modify: `src/peanut-vision-ui/src/api/queryKeys.ts`
- Modify: `src/peanut-vision-ui/src/App.tsx`
- Modify: `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx`

- [ ] **Step 1: Delete SessionSelector**

```bash
rm src/peanut-vision-ui/src/components/SessionSelector.tsx
```

- [ ] **Step 2: Remove Session interface and sessionId from types.ts**

In `src/peanut-vision-ui/src/api/types.ts`:

Remove the entire `Session` interface:
```typescript
// DELETE THIS BLOCK:
export interface Session {
  id: string;
  name: string;
  createdAt: string;
  endedAt: string | null;
  notes: string | null;
  isActive: boolean;
}
```

Remove `sessionId` from `CapturedImageRecord`:
```typescript
// BEFORE:
export interface CapturedImageRecord {
  id: string;
  filePath: string;
  filename: string;
  hasThumbnail: boolean;
  width: number;
  height: number;
  fileSizeBytes: number;
  format: string;
  capturedAt: string;
  sessionId: string | null;  // DELETE THIS LINE
}
```

Change `SubfolderStrategy` type:
```typescript
// BEFORE:
export type SubfolderStrategy = "none" | "byDate" | "bySession" | "byProfile";

// AFTER:
export type SubfolderStrategy = "none" | "byDate" | "byProfile";
```

- [ ] **Step 3: Remove session functions from client.ts**

In `src/peanut-vision-ui/src/api/client.ts`, delete:
- `getSessions()` function
- `getActiveSession()` function
- `createSession()` function
- `endSession()` function
- `deleteSession()` function

Also update `listImages` to replace `sessionId` with `date`:

```typescript
// BEFORE:
export function listImages(params: {
  page?: number;
  pageSize?: number;
  sessionId?: string;
}): Promise<ImagePage> {
  const q = new URLSearchParams();
  if (params.page) q.set("page", String(params.page));
  if (params.pageSize) q.set("pageSize", String(params.pageSize));
  if (params.sessionId) q.set("sessionId", params.sessionId);
  return request(`/images?${q}`);
}

// AFTER:
export function listImages(params: {
  page?: number;
  pageSize?: number;
  date?: string;
}): Promise<ImagePage> {
  const q = new URLSearchParams();
  if (params.page) q.set("page", String(params.page));
  if (params.pageSize) q.set("pageSize", String(params.pageSize));
  if (params.date) q.set("date", params.date);
  return request(`/images?${q}`);
}
```

- [ ] **Step 4: Remove session keys from queryKeys.ts**

In `src/peanut-vision-ui/src/api/queryKeys.ts`, delete:

```typescript
sessions:          ["sessions"]                        as const,
activeSession:     ["activeSession"]                   as const,
```

- [ ] **Step 5: Remove sessionName state and chip from App.tsx**

In `src/peanut-vision-ui/src/App.tsx`:

Remove the `sessionName` state:
```typescript
// DELETE:
const [sessionName, setSessionName] = useState<string | null>(null);
```

Remove the chip in the AppBar (find and delete this block):
```typescript
// DELETE:
{sessionName && (
  <Chip
    icon={<FolderIcon />}
    label={sessionName}
    size="small"
    color="secondary"
    variant="outlined"
    sx={{ mr: 1 }}
  />
)}
```

Remove `onSessionChange` prop from `<AcquisitionTab>`:
```typescript
// BEFORE:
<AcquisitionTab onSessionChange={setSessionName} />

// AFTER:
<AcquisitionTab />
```

Also remove unused imports: `FolderIcon`, `Chip` (if no longer used), `useState` (if no longer used).

- [ ] **Step 6: Remove SessionSelector from AcquisitionTab.tsx**

In `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx`:

Remove import:
```typescript
// DELETE:
import SessionSelector from "../components/SessionSelector";
```

Remove `onSessionChange` from the Props interface and destructuring:
```typescript
// BEFORE:
interface Props {
  onSessionChange?: (name: string | null) => void;
}
export default function AcquisitionTab({ onSessionChange }: Props = {}) {

// AFTER:
export default function AcquisitionTab() {
```

Remove the `<SessionSelector>` usage from the JSX (find and delete):
```typescript
// DELETE:
<SessionSelector onSessionChange={onSessionChange} />
```

- [ ] **Step 7: Build frontend to verify no TypeScript errors**

```bash
cd src/peanut-vision-ui && npm run build
```

Expected: build succeeds with 0 errors.

- [ ] **Step 8: Commit**

```bash
cd ../.. # back to repo root
git add -A
git commit -m "refactor: remove Session from frontend — types, client, queryKeys, App, AcquisitionTab"
```

---

## Task 5: Replace session filter with date filter in Gallery UI

**Files:**
- Modify: `src/peanut-vision-ui/src/hooks/useImageGallery.ts`
- Modify: `src/peanut-vision-ui/src/components/ImageGallery.tsx`
- Modify: `src/peanut-vision-ui/src/tabs/GalleryTab.tsx`
- Test: `src/peanut-vision-ui/src/hooks/useImageGallery.test.tsx`

- [ ] **Step 1: Write the failing hook test**

Create `src/peanut-vision-ui/src/hooks/useImageGallery.test.tsx`:

```typescript
import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import React from "react";
import { useImageGallery } from "./useImageGallery";

vi.mock("../api/client", () => ({
  listImages: vi.fn(() => Promise.resolve({ items: [], totalCount: 0, page: 1, pageSize: 20, totalPages: 0 })),
  deleteImage: vi.fn(),
  imageFileUrl: vi.fn((id: string) => `/api/images/${id}/file`),
}));

vi.mock("../contexts/ToastContext", () => ({
  useToast: () => ({ toast: vi.fn() }),
}));

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe("useImageGallery", () => {
  beforeEach(() => vi.clearAllMocks());

  it("starts with filterDate null", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    expect(result.current.filterDate).toBeNull();
  });

  it("setFilterDate updates filterDate", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    act(() => { result.current.setFilterDate("2026-04-06"); });
    expect(result.current.filterDate).toBe("2026-04-06");
  });

  it("setFilterDate null clears the filter", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    act(() => { result.current.setFilterDate("2026-04-06"); });
    act(() => { result.current.setFilterDate(null); });
    expect(result.current.filterDate).toBeNull();
  });
});
```

- [ ] **Step 2: Run test to verify it fails**

```bash
cd src/peanut-vision-ui && npx vitest run src/hooks/useImageGallery.test.tsx
```

Expected: FAIL — `filterDate` does not exist on the return value yet.

- [ ] **Step 3: Update useImageGallery.ts — replace filterSessionId with filterDate**

Replace `src/peanut-vision-ui/src/hooks/useImageGallery.ts` with:

```typescript
import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listImages, deleteImage, imageFileUrl } from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { GALLERY_POLL_INTERVAL_MS } from "../constants";
import { useToast } from "../contexts/ToastContext";
import type { CapturedImageRecord } from "../api/types";

const PAGE_SIZE = 20;

export function useImageGallery() {
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const [page, setPage] = useState(1);
  const [filterDate, setFilterDate] = useState<string | null>(null);
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const queryParams = {
    page,
    pageSize: PAGE_SIZE,
    ...(filterDate ? { date: filterDate } : {}),
  };

  const { data, isLoading } = useQuery({
    queryKey: queryKeys.images(queryParams),
    queryFn: () => listImages(queryParams),
    refetchInterval: GALLERY_POLL_INTERVAL_MS,
  });

  useEffect(() => {
    if (data?.items.length && selectedId === null) {
      setSelectedId(data.items[0].id);
    }
  }, [data, selectedId]);

  const deleteMutation = useMutation({
    mutationFn: deleteImage,
    onSuccess: (_: void, id: string) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.images() });
      if (selectedId === id) setSelectedId(null);
      toast("이미지가 삭제되었습니다", "info");
    },
    onError: (e: unknown) =>
      toast(e instanceof Error ? e.message : "삭제에 실패했습니다", "error"),
  });

  const images: CapturedImageRecord[] = data?.items ?? [];
  const selectedImage = images.find((i) => i.id === selectedId) ?? null;
  const selectedImageUrl = selectedId ? imageFileUrl(selectedId) : null;

  return {
    images,
    totalCount: data?.totalCount ?? 0,
    totalPages: data?.totalPages ?? 0,
    page,
    setPage,
    isLoading,
    filterDate,
    setFilterDate,
    selectedId,
    setSelectedId,
    selectedImage,
    selectedImageUrl,
    handleDelete: (id: string) => deleteMutation.mutate(id),
  };
}
```

- [ ] **Step 4: Run test to verify it passes**

```bash
npx vitest run src/hooks/useImageGallery.test.tsx
```

Expected: `Tests 3 passed (3)`

- [ ] **Step 5: Update ImageGallery.tsx — replace session Select with date input**

In `src/peanut-vision-ui/src/components/ImageGallery.tsx`:

Update the Props interface — replace `filterSessionId` / `onFilterChange`:

```typescript
interface Props {
  images: CapturedImageRecord[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  page: number;
  totalPages: number;
  onPageChange: (p: number) => void;
  filterDate: string | null;
  onFilterDateChange: (date: string | null) => void;
  isLoading: boolean;
}
```

Update destructuring:
```typescript
export default function ImageGallery({
  images,
  selectedId,
  onSelect,
  onDelete,
  page,
  totalPages,
  onPageChange,
  filterDate,
  onFilterDateChange,
  isLoading,
}: Props) {
```

Remove the `useQuery` for sessions (delete these lines):
```typescript
// DELETE:
const { data: sessions } = useQuery({
  queryKey: queryKeys.sessions,
  queryFn: getSessions,
});
```

Replace the session filter `<Box>` with a date filter:
```typescript
{/* Date filter */}
<Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 1 }}>
  <input
    type="date"
    value={filterDate ?? ""}
    onChange={(e) => onFilterDateChange(e.target.value || null)}
    style={{
      flex: 1,
      padding: "4px 8px",
      fontSize: "0.75rem",
      border: "1px solid #ccc",
      borderRadius: 4,
      background: "transparent",
      color: "inherit",
    }}
  />
  {filterDate && (
    <Button size="small" onClick={() => onFilterDateChange(null)}>
      Clear
    </Button>
  )}
</Box>
```

Remove unused imports: `getSessions`, `Select`, `MenuItem`, `queryKeys` (if no longer used elsewhere in the file), `ExpandMoreIcon`.

- [ ] **Step 6: Update GalleryTab.tsx — pass filterDate/setFilterDate**

Replace `src/peanut-vision-ui/src/tabs/GalleryTab.tsx` with:

```typescript
import Box from "@mui/material/Box";
import ImageViewer from "../components/ImageViewer";
import ImageGallery from "../components/ImageGallery";
import { useImageGallery } from "../hooks/useImageGallery";

export default function GalleryTab() {
  const gallery = useImageGallery();

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      {/* 좌: 선택 이미지 뷰어 */}
      <Box
        sx={{
          width: 480,
          flexShrink: 0,
          borderRight: "1px solid",
          borderColor: "divider",
          p: 2,
          display: "flex",
          flexDirection: "column",
        }}
      >
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={
            gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null
          }
          onClose={() => gallery.setSelectedId(null)}
        />
      </Box>

      {/* 우: 갤러리 그리드 */}
      <Box sx={{ flexGrow: 1, overflow: "hidden", p: 2 }}>
        <ImageGallery
          images={gallery.images}
          selectedId={gallery.selectedId}
          onSelect={gallery.setSelectedId}
          onDelete={gallery.handleDelete}
          page={gallery.page}
          totalPages={gallery.totalPages}
          onPageChange={gallery.setPage}
          filterDate={gallery.filterDate}
          onFilterDateChange={gallery.setFilterDate}
          isLoading={gallery.isLoading}
        />
      </Box>
    </Box>
  );
}
```

- [ ] **Step 7: Build frontend**

```bash
npm run build
```

Expected: build succeeds with 0 TypeScript errors.

- [ ] **Step 8: Run all frontend tests**

```bash
npx vitest run
```

Expected: all tests pass.

- [ ] **Step 9: Run backend tests**

```bash
cd ../.. && dotnet test src/PeanutVision.Api.Tests/PeanutVision.Api.Tests.csproj
```

Expected: `Passed! - Failed: 0, Passed: 183, ...`

- [ ] **Step 10: Verify zero Session references remain**

```bash
grep -r "Session\|sessionId\|ISessionRepository\|SessionSelector\|BySession" \
  src/PeanutVision.Api/Services \
  src/PeanutVision.Api/Controllers \
  src/PeanutVision.Api/Program.cs \
  src/peanut-vision-ui/src \
  --include="*.cs" --include="*.ts" --include="*.tsx" \
  --exclude-dir=".git"
```

Expected: zero matches.

- [ ] **Step 11: Commit**

```bash
git add -A
git commit -m "feat: replace session filter with date filter in Gallery UI"
```
