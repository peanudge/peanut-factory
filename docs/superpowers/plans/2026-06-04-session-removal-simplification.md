# Session 제거 및 기능 단순화 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Session 개념과 불필요한 ImageSaveSettings 필드(FilenamePrefix, TimestampFormat, IncludeSequenceNumber, SubfolderStrategy)를 BE/FE 전체에서 제거하고, outputDirectory에 `{date}`/`{profile}` 토큰을 지원하는 경로 템플릿으로 대체한다.

**Architecture:** Session 관련 파일 삭제 → CapturedImage/AppDbContext 단순화 → FilenameGenerator 재작성 → AutoSaveService 단순화 → API 레이어 정리 → 테스트 업데이트 → FE 동기화. BE와 FE를 한 번에 처리한다.

**Tech Stack:** .NET 10, ASP.NET Core, EF Core (SQLite), React 19, TypeScript

---

## 변경 파일 목록

### 삭제
- `src/PeanutVision.Api/Services/Session.cs`
- `src/PeanutVision.Api/Services/ISessionRepository.cs`
- `src/PeanutVision.Api/Services/SessionRepository.cs`
- `src/PeanutVision.Api/Controllers/SessionController.cs`
- `src/peanut-vision-app/src/components/shared/SessionSelector/` (디렉토리 전체)

### BE 수정
- `src/PeanutVision.Api/Services/CapturedImage.cs`
- `src/PeanutVision.Api/Services/ICapturedImageRepository.cs`
- `src/PeanutVision.Api/Services/CapturedImageRepository.cs`
- `src/PeanutVision.Api/Services/AppDbContext.cs`
- `src/PeanutVision.Api/Services/ImageSaveSettings.cs`
- `src/PeanutVision.Api/Services/FilenameGenerator.cs`
- `src/PeanutVision.Api/Services/AutoSaveService.cs`
- `src/PeanutVision.Api/Controllers/ImagesController.cs`
- `src/PeanutVision.Api/Program.cs`

### 테스트 수정
- `src/PeanutVision.Api.Tests/Unit/AutoSaveServiceTests.cs`
- `src/PeanutVision.Api.Tests/Unit/ImageSaveSettingsServiceTests.cs`

### FE 수정
- `src/peanut-vision-app/src/api/types.ts`
- `src/peanut-vision-app/src/api/client.ts`
- `src/peanut-vision-app/src/api/queryKeys.ts`
- `src/peanut-vision-app/src/hooks/useImageGallery.ts`
- `src/peanut-vision-app/src/components/shared/ImageGallery/index.tsx`
- `src/peanut-vision-app/src/components/shared/ImageSaveSettingsPanel/index.tsx`
- `src/peanut-vision-app/src/components/Acquisition/index.tsx`

---

### Task 1: BE 데이터 레이어 — Session 파일 삭제 및 CapturedImage/AppDbContext 단순화

**Files:**
- Delete: `src/PeanutVision.Api/Services/Session.cs`
- Delete: `src/PeanutVision.Api/Services/ISessionRepository.cs`
- Delete: `src/PeanutVision.Api/Services/SessionRepository.cs`
- Modify: `src/PeanutVision.Api/Services/CapturedImage.cs`
- Modify: `src/PeanutVision.Api/Services/ICapturedImageRepository.cs`
- Modify: `src/PeanutVision.Api/Services/CapturedImageRepository.cs`
- Modify: `src/PeanutVision.Api/Services/AppDbContext.cs`

- [ ] **Step 1: Session 파일 3개 삭제**

```bash
rm src/PeanutVision.Api/Services/Session.cs
rm src/PeanutVision.Api/Services/ISessionRepository.cs
rm src/PeanutVision.Api/Services/SessionRepository.cs
```

- [ ] **Step 2: CapturedImage.cs에서 SessionId/Session 제거**

전체 파일을 다음으로 교체:

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

- [ ] **Step 3: ICapturedImageRepository.cs에서 sessionId 파라미터 제거**

전체 파일을 다음으로 교체:

```csharp
namespace PeanutVision.Api.Services;

public interface ICapturedImageRepository
{
    Task<CapturedImage> AddAsync(CapturedImage image);
    Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
        int page, int pageSize,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
    Task<CapturedImage?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}
```

- [ ] **Step 4: CapturedImageRepository.cs에서 sessionId 필터 제거**

전체 파일을 다음으로 교체:

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
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _db.CapturedImages.AsNoTracking().AsQueryable();

        if (dateFrom.HasValue)
            query = query.Where(c => c.CapturedAt >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(c => c.CapturedAt <= dateTo.Value);

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

- [ ] **Step 5: AppDbContext.cs에서 Sessions DbSet 및 관련 설정 제거**

전체 파일을 다음으로 교체:

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

---

### Task 2: BE 서비스 레이어 — ImageSaveSettings 단순화 및 FilenameGenerator 재작성

**Files:**
- Modify: `src/PeanutVision.Api/Services/ImageSaveSettings.cs`
- Modify: `src/PeanutVision.Api/Services/FilenameGenerator.cs`

- [ ] **Step 1: ImageSaveSettings.cs 단순화**

전체 파일을 다음으로 교체:

```csharp
namespace PeanutVision.Api.Services;

public enum SaveImageFormat { Png, Bmp, Raw }

public sealed record ImageSaveSettings
{
    public string OutputDirectory { get; init; } = "CapturedImages";
    public SaveImageFormat Format { get; init; } = SaveImageFormat.Png;
    public bool AutoSave { get; init; } = true;
}
```

- [ ] **Step 2: FilenameGenerator.cs 재작성 (토큰 기반 경로 + 고정 파일명)**

전체 파일을 다음으로 교체:

```csharp
namespace PeanutVision.Api.Services;

public sealed class FilenameGenerator
{
    private int _sequenceCounter;

    /// <summary>
    /// outputDirectory 내 지원 토큰:
    ///   {date}    → yyyy-MM-dd
    ///   {profile} → profileId에서 특수문자를 _로 치환한 값
    /// 파일명 고정: capture_yyyyMMdd_HHmmss_fff_NNNNN.ext
    /// </summary>
    public string Generate(ImageSaveSettings settings, string contentRootPath, string? profileId = null)
    {
        var now = DateTime.Now;
        var dir = ExpandDirectory(settings.OutputDirectory, contentRootPath, now, profileId);
        Directory.CreateDirectory(dir);

        var seq = Interlocked.Increment(ref _sequenceCounter);
        var ext = settings.Format switch
        {
            SaveImageFormat.Bmp => ".bmp",
            SaveImageFormat.Raw => ".raw",
            _ => ".png",
        };
        var name = $"capture_{now:yyyyMMdd_HHmmss_fff}_{seq:D5}{ext}";
        return Path.Combine(dir, name);
    }

    private static string ExpandDirectory(string template, string contentRootPath, DateTime now, string? profileId)
    {
        if (string.IsNullOrWhiteSpace(template))
            return Path.Combine(contentRootPath, "CapturedImages");

        var expanded = template
            .Replace("{date}", now.ToString("yyyy-MM-dd"))
            .Replace("{profile}", SanitizeSegment(profileId ?? "unknown"));

        return Path.IsPathRooted(expanded)
            ? expanded
            : Path.Combine(contentRootPath, expanded);
    }

    private static string SanitizeSegment(string segment) =>
        string.Concat(segment.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
}
```

---

### Task 3: BE 서비스 레이어 — AutoSaveService에서 Session 의존성 제거

**Files:**
- Modify: `src/PeanutVision.Api/Services/AutoSaveService.cs`

- [ ] **Step 1: AutoSaveService.cs에서 ISessionRepository 제거**

전체 파일을 다음으로 교체:

```csharp
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AutoSaveService : IHostedService
{
    private readonly IAcquisitionService _acquisition;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FrameSaveTracker _frameSaveTracker;
    private readonly IThumbnailService _thumbnailService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _environment;

    public AutoSaveService(
        IAcquisitionService acquisition,
        IImageSaveSettingsService saveSettings,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        IThumbnailService thumbnailService,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment)
    {
        _acquisition = acquisition;
        _saveSettings = saveSettings;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
        _thumbnailService = thumbnailService;
        _scopeFactory = scopeFactory;
        _environment = environment;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _acquisition.FrameAcquired += OnFrameAcquired;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _acquisition.FrameAcquired -= OnFrameAcquired;
        return Task.CompletedTask;
    }

    private void OnFrameAcquired(object? sender, EventArgs e)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave)
            return;

        var frame = _acquisition.GetLatestFrame();
        if (frame == null || !_frameSaveTracker.ShouldSave(frame))
            return;

        var profileId = _acquisition.ActiveProfileId?.Value;
        _ = SaveAsync(frame, settings, profileId);
    }

    private async Task SaveAsync(ImageData image, ImageSaveSettings settings, string? profileId)
    {
        try
        {
            var filePath = _filenameGenerator.Generate(settings, _environment.ContentRootPath, profileId);
            new ImageWriter().Save(image, filePath);

            var thumbPath = await _thumbnailService.GenerateAsync(filePath);
            var fileInfo = new FileInfo(filePath);

            using var scope = _scopeFactory.CreateScope();
            var imageRepo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();

            await imageRepo.AddAsync(new CapturedImage
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
        }
        catch
        {
            // Save failures must not propagate to the acquisition pipeline
        }
    }
}
```

---

### Task 4: BE API 레이어 — ImagesController, SessionController 삭제, Program.cs 정리

**Files:**
- Delete: `src/PeanutVision.Api/Controllers/SessionController.cs`
- Modify: `src/PeanutVision.Api/Controllers/ImagesController.cs`
- Modify: `src/PeanutVision.Api/Program.cs`

- [ ] **Step 1: SessionController.cs 삭제**

```bash
rm src/PeanutVision.Api/Controllers/SessionController.cs
```

- [ ] **Step 2: ImagesController.cs에서 sessionId 파라미터 및 DTO 필드 제거**

전체 파일을 다음으로 교체:

```csharp
using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly ICapturedImageRepository _repo;

    public ImagesController(ICapturedImageRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<ImagePageDto>> GetImages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var (items, total) = await _repo.GetPageAsync(page, pageSize, dateFrom, dateTo);
        return Ok(new ImagePageDto(
            items.Select(ToDto).ToList(),
            total,
            page,
            pageSize,
            (int)Math.Ceiling(total / (double)pageSize)));
    }

    [HttpGet("{id:guid}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();
        if (image.ThumbnailPath is null || !System.IO.File.Exists(image.ThumbnailPath))
            return NotFound(new { error = "Thumbnail not available" });
        return PhysicalFile(image.ThumbnailPath, "image/jpeg");
    }

    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();
        if (!System.IO.File.Exists(image.FilePath))
            return NotFound(new { error = "File not found on disk" });

        var mimeType = image.Format switch
        {
            "bmp" => "image/bmp",
            "raw" => "application/octet-stream",
            _ => "image/png",
        };
        return PhysicalFile(image.FilePath, mimeType, Path.GetFileName(image.FilePath));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image is null) return NotFound();

        try { if (System.IO.File.Exists(image.FilePath)) System.IO.File.Delete(image.FilePath); } catch { }
        try
        {
            if (image.ThumbnailPath is not null && System.IO.File.Exists(image.ThumbnailPath))
                System.IO.File.Delete(image.ThumbnailPath);
        }
        catch { }

        await _repo.DeleteAsync(id);
        return NoContent();
    }

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
}

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

public record ImagePageDto(
    IReadOnlyList<CapturedImageDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
```

- [ ] **Step 3: Program.cs에서 Session DI 등록과 Sessions SQL 제거**

`Program.cs`의 `builder.Services.AddScoped<ISessionRepository, SessionRepository>();` 줄을 삭제한다.

`db.Database.ExecuteSqlRaw(...)` 블록 전체를 다음으로 교체한다:

```csharp
db.Database.EnsureCreated();
db.Database.ExecuteSqlRaw("""
    CREATE TABLE IF NOT EXISTS CapturedImages (
        Id TEXT NOT NULL PRIMARY KEY,
        FilePath TEXT NOT NULL,
        ThumbnailPath TEXT,
        Width INTEGER NOT NULL DEFAULT 0,
        Height INTEGER NOT NULL DEFAULT 0,
        FileSizeBytes INTEGER NOT NULL DEFAULT 0,
        Format TEXT NOT NULL DEFAULT '',
        CapturedAt TEXT NOT NULL
    );
    CREATE INDEX IF NOT EXISTS IX_CapturedImages_CapturedAt ON CapturedImages(CapturedAt);
    """);
```

- [ ] **Step 4: dotnet build로 BE 컴파일 확인**

```bash
dotnet build peanut-factory.sln -v quiet 2>&1 | tail -5
```

기대 결과: `Build succeeded. 0 Warning(s) 0 Error(s)`

---

### Task 5: BE 테스트 업데이트

**Files:**
- Modify: `src/PeanutVision.Api.Tests/Unit/AutoSaveServiceTests.cs`
- Modify: `src/PeanutVision.Api.Tests/Unit/ImageSaveSettingsServiceTests.cs`

- [ ] **Step 1: AutoSaveServiceTests.cs에서 FakeSessionRepository 제거**

`FakeSessionRepository` 클래스 전체를 삭제하고, `FakeScopeFactory` 내부에서 `FakeSessionRepository` 등록 줄을 제거한다.

`FakeScopeFactory.CreateScope()` 내부를 다음으로 교체:

```csharp
public IServiceScope CreateScope()
{
    var services = new ServiceCollection();
    services.AddSingleton<ICapturedImageRepository>(FakeImageRepo);
    var provider = services.BuildServiceProvider();
    return new FakeScope(provider);
}
```

`FakeImageRepository.GetPageAsync` 시그니처에서 `Guid? sessionId = null` 파라미터를 제거한다:

```csharp
public Task<(IReadOnlyList<CapturedImage> Items, int TotalCount)> GetPageAsync(
    int page, int pageSize, DateTime? dateFrom = null, DateTime? dateTo = null) =>
    Task.FromResult<(IReadOnlyList<CapturedImage>, int)>(([], 0));
```

- [ ] **Step 2: ImageSaveSettingsServiceTests.cs 업데이트**

제거된 필드(`FilenamePrefix`, `TimestampFormat`, `IncludeSequenceNumber`, `SubfolderStrategy`)를 참조하는 모든 assertion과 설정 객체를 수정한다.

기본값 테스트를 다음으로 교체:

```csharp
[Fact]
public void GetSettings_ReturnsDefaults_WhenNoFile()
{
    using var dir = new TempDirectory();
    var service = new ImageSaveSettingsService(Path.Combine(dir.Path, "settings.json"));

    var settings = service.GetSettings();

    Assert.Equal("CapturedImages", settings.OutputDirectory);
    Assert.Equal(SaveImageFormat.Png, settings.Format);
    Assert.True(settings.AutoSave);
}
```

저장/불러오기 테스트에서 `ImageSaveSettings` 생성 시 제거된 필드를 쓰지 않도록 수정. 예시:

```csharp
var newSettings = new ImageSaveSettings
{
    OutputDirectory = "/custom/path",
    Format = SaveImageFormat.Bmp,
    AutoSave = false,
};
```

- [ ] **Step 3: dotnet test로 전체 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "FullyQualifiedName!~IntegrationTests" -v quiet 2>&1 | tail -10
```

기대 결과: `Failed: 0`

- [ ] **Step 4: 커밋**

```bash
git add src/PeanutVision.Api/ src/PeanutVision.Api.Tests/
git commit -m "refactor(be): remove Session, simplify ImageSaveSettings and FilenameGenerator"
```

---

### Task 6: FE 타입/클라이언트/queryKeys 정리

**Files:**
- Modify: `src/peanut-vision-app/src/api/types.ts`
- Modify: `src/peanut-vision-app/src/api/client.ts`
- Modify: `src/peanut-vision-app/src/api/queryKeys.ts`

- [ ] **Step 1: types.ts에서 Session 관련 타입/필드 제거**

`Session` 인터페이스 전체 삭제.

`CapturedImageRecord`에서 `sessionId: string | null;` 삭제.

`ImageSaveSettings`를 다음으로 교체:

```typescript
export interface ImageSaveSettings {
  outputDirectory: string;
  format: SaveImageFormat;
  autoSave: boolean;
}
```

`SubfolderStrategy` 타입 전체 삭제.

- [ ] **Step 2: client.ts에서 Session API 함수 5개 삭제**

`getSessions`, `getActiveSession`, `createSession`, `endSession`, `deleteSession` 함수 전체를 삭제한다.

- [ ] **Step 3: queryKeys.ts에서 sessions/activeSession 키 삭제**

```typescript
export const queryKeys = {
  cameras:           ["cameras"]                         as const,
  acquisitionStatus: ["acquisitionStatus"]               as const,
  latestFrame:       ["latestFrame"]                     as const,
  boards:            ["boards"]                          as const,
  boardStatus:       (index: number) => ["boardStatus", index] as const,
  presets:           ["presets"]                         as const,
  histogram:         ["histogram"]                       as const,
  imageSaveSettings: ["imageSaveSettings"]               as const,
  exposure:          ["exposure"]                        as const,
  images:            (params?: object) => params ? ["images", params] as const : ["images"] as const,
};
```

---

### Task 7: FE 갤러리 — 세션 필터 → 날짜 범위 필터

**Files:**
- Modify: `src/peanut-vision-app/src/hooks/useImageGallery.ts`
- Modify: `src/peanut-vision-app/src/components/shared/ImageGallery/index.tsx`
- Modify: `src/peanut-vision-app/src/components/Gallery/index.tsx`

- [ ] **Step 1: useImageGallery.ts를 날짜 범위 필터로 교체**

전체 파일을 다음으로 교체:

```typescript
import { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { listImages, deleteImage, imageFileUrl } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { GALLERY_POLL_INTERVAL_MS } from '@/constants'
import { useToast } from '@/contexts/ToastContext'
import type { CapturedImageRecord } from '@/api/types'

const PAGE_SIZE = 20

export function useImageGallery() {
  const queryClient = useQueryClient()
  const { toast } = useToast()
  const [page, setPage] = useState(1)
  const [dateFrom, setDateFrom] = useState<string | null>(null)
  const [dateTo, setDateTo] = useState<string | null>(null)
  const [selectedId, setSelectedId] = useState<string | null>(null)

  const queryParams = {
    page,
    pageSize: PAGE_SIZE,
    ...(dateFrom ? { dateFrom } : {}),
    ...(dateTo ? { dateTo } : {}),
  }

  const { data, isLoading } = useQuery({
    queryKey: queryKeys.images(queryParams),
    queryFn: () => listImages(queryParams),
    refetchInterval: GALLERY_POLL_INTERVAL_MS,
  })

  useEffect(() => {
    if (data?.items.length && selectedId === null) {
      setSelectedId(data.items[0].id)
    }
  }, [data, selectedId])

  const deleteMutation = useMutation({
    mutationFn: deleteImage,
    onSuccess: (_: void, id: string) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.images() })
      if (selectedId === id) setSelectedId(null)
      toast('이미지가 삭제되었습니다', 'info')
    },
    onError: (e: unknown) =>
      toast(e instanceof Error ? e.message : '삭제에 실패했습니다', 'error'),
  })

  const images: CapturedImageRecord[] = data?.items ?? []
  const selectedImage = images.find((i) => i.id === selectedId) ?? null
  const selectedImageUrl = selectedId ? imageFileUrl(selectedId) : null

  return {
    images,
    totalCount: data?.totalCount ?? 0,
    totalPages: data?.totalPages ?? 0,
    page,
    setPage,
    isLoading,
    dateFrom,
    setDateFrom,
    dateTo,
    setDateTo,
    selectedId,
    setSelectedId,
    selectedImage,
    selectedImageUrl,
    handleDelete: (id: string) => deleteMutation.mutate(id),
  }
}
```

- [ ] **Step 2: ImageGallery/index.tsx Props 인터페이스와 필터 UI 교체**

Props 인터페이스를 다음으로 교체:

```typescript
interface Props {
  images: CapturedImageRecord[]
  selectedId: string | null
  onSelect: (id: string) => void
  onDelete: (id: string) => void
  page: number
  totalPages: number
  onPageChange: (p: number) => void
  dateFrom: string | null
  dateTo: string | null
  onDateFromChange: (v: string | null) => void
  onDateToChange: (v: string | null) => void
  isLoading: boolean
}
```

`getSessions` import와 세션 useQuery 블록을 삭제하고, 세션 필터 `<select>` 블록을 날짜 범위 입력으로 교체:

```tsx
{/* Date range filter */}
<div className={cx('filterRow')}>
  <input
    type="date"
    className={cx('dateInput')}
    value={dateFrom ?? ''}
    onChange={(e) => onDateFromChange(e.target.value || null)}
  />
  <span>–</span>
  <input
    type="date"
    className={cx('dateInput')}
    value={dateTo ?? ''}
    onChange={(e) => onDateToChange(e.target.value || null)}
  />
  {(dateFrom || dateTo) && (
    <button
      type="button"
      className={cx('clearBtn')}
      onClick={() => { onDateFromChange(null); onDateToChange(null) }}
    >
      <X size={14} />
    </button>
  )}
</div>
```

`getSessions` import 줄도 제거한다.

- [ ] **Step 3: Gallery/index.tsx에서 ImageGallery props 업데이트**

`filterSessionId`/`onFilterChange` → `dateFrom`/`dateTo`/`onDateFromChange`/`onDateToChange`로 교체:

```tsx
<ImageGallery
  images={gallery.images}
  selectedId={gallery.selectedId}
  onSelect={gallery.setSelectedId}
  onDelete={gallery.handleDelete}
  page={gallery.page}
  totalPages={gallery.totalPages}
  onPageChange={gallery.setPage}
  dateFrom={gallery.dateFrom}
  dateTo={gallery.dateTo}
  onDateFromChange={gallery.setDateFrom}
  onDateToChange={gallery.setDateTo}
  isLoading={gallery.isLoading}
/>
```

---

### Task 8: FE 설정 패널 단순화 및 SessionSelector 제거

**Files:**
- Delete: `src/peanut-vision-app/src/components/shared/SessionSelector/` (디렉토리 전체)
- Modify: `src/peanut-vision-app/src/components/shared/ImageSaveSettingsPanel/index.tsx`
- Modify: `src/peanut-vision-app/src/components/Acquisition/index.tsx`

- [ ] **Step 1: SessionSelector 디렉토리 삭제**

```bash
rm -rf src/peanut-vision-app/src/components/shared/SessionSelector
```

- [ ] **Step 2: ImageSaveSettingsPanel/index.tsx 단순화**

전체 파일을 다음으로 교체:

```tsx
import { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Folder } from 'lucide-react'
import type { ImageSaveSettings, SaveImageFormat } from '@/api/types'
import { getImageSaveSettings, updateImageSaveSettings } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import cx from './cx'

const FORMAT_OPTIONS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

const DEFAULT_SETTINGS: ImageSaveSettings = {
  outputDirectory: 'CapturedImages',
  format: 'png',
  autoSave: true,
}

export default function ImageSaveSettingsPanel() {
  const queryClient = useQueryClient()
  const [localSettings, setLocalSettings] = useState<ImageSaveSettings>(DEFAULT_SETTINGS)
  const [saved, setSaved] = useState(false)

  const { data: serverSettings } = useQuery({
    queryKey: queryKeys.imageSaveSettings,
    queryFn: getImageSaveSettings,
  })

  useEffect(() => {
    if (serverSettings) setLocalSettings(serverSettings)
  }, [serverSettings])

  const saveMutation = useMutation({
    mutationFn: (settings: ImageSaveSettings) => updateImageSaveSettings(settings),
    onSuccess: (updated) => {
      queryClient.setQueryData(queryKeys.imageSaveSettings, updated)
      setLocalSettings(updated)
      setSaved(true)
      setTimeout(() => setSaved(false), 3000)
    },
  })

  const update = <K extends keyof ImageSaveSettings>(key: K, value: ImageSaveSettings[K]) =>
    setLocalSettings((prev) => ({ ...prev, [key]: value }))

  return (
    <details className={cx('accordion')}>
      <summary>
        <Folder size={15} />
        Image Save Settings
      </summary>
      <div className={cx('body')}>
        <div className={cx('row')}>
          <div className={cx('field')} style={{ flexGrow: 1, minWidth: 220 }}>
            <label>Output Directory</label>
            <input
              type="text"
              value={localSettings.outputDirectory}
              onChange={(e) => update('outputDirectory', e.target.value)}
            />
            <small>Tokens: {'{date}'} → yyyy-MM-dd, {'{profile}'} → cam file name</small>
          </div>
          <div className={cx('field')} style={{ width: 110 }}>
            <label>Format</label>
            <select
              value={localSettings.format}
              onChange={(e) => update('format', e.target.value as SaveImageFormat)}
            >
              {FORMAT_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </div>

        <div className={cx('checkRow')}>
          <label className={cx('checkLabel')}>
            <input
              type="checkbox"
              checked={localSettings.autoSave}
              onChange={(e) => update('autoSave', e.target.checked)}
            />
            Auto-save on capture
          </label>
        </div>

        <div className={cx('bottomRow')}>
          <span className={cx('example')}>
            Example: <strong>capture_20260604_143000_123_00001.{localSettings.format}</strong>
          </span>
          <button
            type="button"
            className={cx('btn')}
            onClick={() => saveMutation.mutate(localSettings)}
            disabled={saveMutation.isPending}
          >
            Save Settings
          </button>
        </div>

        {saveMutation.isError && (
          <div className={cx('alert', 'error')}>
            {saveMutation.error instanceof Error
              ? saveMutation.error.message
              : 'Failed to save settings'}
          </div>
        )}
        {saved && <div className={cx('alert', 'success')}>Settings saved</div>}
      </div>
    </details>
  )
}
```

- [ ] **Step 3: Acquisition/index.tsx에서 SessionSelector 제거**

`import SessionSelector from '@/components/shared/SessionSelector'` 줄을 삭제한다.

Settings 탭 내부의 `<SessionSelector />` 렌더링 줄을 삭제한다.

---

### Task 9: 최종 검증 및 커밋

- [ ] **Step 1: BE grep 검증**

```bash
grep -rn "Session\|session" src/PeanutVision.Api src/PeanutVision.Api.Tests --include="*.cs" | grep -v "AcquisitionStatisticsSnapshot\|GetSnapshot\|sessions_persist\|events_persist"
```

기대 결과: 출력 없음 (또는 AcquisitionStatisticsSnapshot·GetSnapshot·"events_persist_across_sessions" 메서드명만)

- [ ] **Step 2: FE grep 검증**

```bash
grep -rn "session\|Session" src/peanut-vision-app/src --include="*.ts" --include="*.tsx"
```

기대 결과: 출력 없음

- [ ] **Step 3: dotnet build 경고 0개 확인**

```bash
dotnet build peanut-factory.sln -v quiet 2>&1 | tail -5
```

기대 결과: `Build succeeded. 0 Warning(s) 0 Error(s)`

- [ ] **Step 4: 전체 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "FullyQualifiedName!~IntegrationTests" -v quiet 2>&1 | tail -10
```

기대 결과: `Failed: 0`

- [ ] **Step 5: TypeScript 타입 체크**

```bash
cd src/peanut-vision-app && npx tsc --noEmit 2>&1
```

기대 결과: 출력 없음 (에러 없음)

- [ ] **Step 6: 최종 커밋**

```bash
git add src/peanut-vision-app/ docs/
git commit -m "refactor(fe): remove Session, simplify gallery filter and save settings panel"
git push
```
