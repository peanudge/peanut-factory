# Design: SOLID Capture Pipeline Redesign

**Date:** 2026-04-03
**Status:** Approved
**Replaces:** `2026-04-03-capture-core-modularization-design.md`
**Scope:** SOLID-based decomposition of `AcquisitionManager` + `ImageCaptureService`; extraction of `PeanutVision.Capture` library

---

## Problem

`AcquisitionManager` violates SRP by combining 7+ responsibilities: channel lifecycle, acquisition control, exposure, snapshot, frame buffering, statistics, and periodic timer. `ImageCaptureService` violates DIP by instantiating `new ImageWriter()` directly and violates SRP by combining filename generation, file writing, thumbnail generation, and DB recording. `IAcquisitionService` violates ISP by exposing a wide interface that consumers use only partially.

---

## SOLID Violations → Fixes

| Principle | Violation | Fix |
|-----------|-----------|-----|
| SRP | `AcquisitionManager` does 7+ things | Split into `AcquisitionSession`, `ExposureController`, `SnapshotCapture`, `FrameSavedHandler` |
| OCP | File format hardcoded in `ImageCaptureService` | `IFrameWriter` abstraction — swap implementation without modifying callers |
| ISP | `IAcquisitionService` too wide | Split into `IAcquisitionSession`, `IExposureController`, `ISnapshotCapture` |
| DIP | `new ImageWriter()` inside `ImageCaptureService` | Inject `IFrameWriter` |
| DIP | `new Timer()` inside `AcquisitionManager` | Extract `CaptureScheduler` with injectable interface |

---

## Layer Structure

```
PeanutVision.MultiCamDriver       ← unchanged
  GrabService, GrabChannel
  ImageWriter, ImageEncoderRegistry, ImageData

PeanutVision.FakeCamDriver        ← unchanged

         ↓ (ProjectReference)

PeanutVision.Capture (NEW)
  IFrameWriter / ImageFileWriter
  IFrameQueue  / BoundedFrameQueue
  CaptureScheduler
  FrameWriterOptions
  FrameSavedEvent

         ↓

PeanutVision.Api (refactored)
  AcquisitionSession
  ExposureController
  SnapshotCapture
  FrameSavedHandler
```

`GrabService` already owns channel create/release at the hardware level — no `ChannelManager` wrapper is needed. `AcquisitionSession` receives `IGrabService` and manages the channel as an internal implementation detail.

---

## `PeanutVision.Capture` — Public API

### `IFrameWriter` (DIP)

```csharp
public interface IFrameWriter
{
    /// <summary>Writes image to disk. Returns the actual file path written.</summary>
    string Write(ImageData image, FrameWriterOptions options);
}

public record FrameWriterOptions(
    string OutputDirectory,
    string FilenamePattern,   // e.g. "frame_{timestamp:yyyyMMdd_HHmmss_fff}"
    ImageFormat Format        // Png | Bmp | Raw
);

/// <summary>Default implementation — delegates to MultiCamDriver's ImageWriter.</summary>
public sealed class ImageFileWriter : IFrameWriter
{
    public ImageFileWriter(ImageWriter writer) { ... }
    public string Write(ImageData image, FrameWriterOptions options) { ... }
}
```

### `IFrameQueue` (DIP)

```csharp
public interface IFrameQueue : IDisposable
{
    /// <summary>Enqueue a frame for async saving. Returns false if queue is full (frame dropped).</summary>
    bool TryEnqueue(ImageData frame);

    /// <summary>Async stream consumed by the background writer task.</summary>
    IAsyncEnumerable<ImageData> ReadAllAsync(CancellationToken ct);
}

/// <summary>Bounded queue backed by System.Threading.Channels. Drops oldest on overflow.</summary>
public sealed class BoundedFrameQueue : IFrameQueue
{
    public BoundedFrameQueue(int capacity = 16) { ... }
}
```

### `CaptureScheduler` (SRP)

```csharp
/// <summary>Fires Tick at a fixed interval. Single responsibility: timing only.</summary>
public sealed class CaptureScheduler : IDisposable
{
    public event Action? Tick;
    public void Start(TimeSpan interval);
    public void Stop();
}
```

### `FrameSavedEvent`

```csharp
public record FrameSavedEvent(
    string FilePath,
    DateTime CapturedAt,
    int Width,
    int Height,
    long FileSizeBytes
);
```

### `PeanutVision.Capture` Dependencies

- `PeanutVision.MultiCamDriver` (`ImageWriter`, `ImageData`, `ImageFormat`)
- `Microsoft.Extensions.Logging.Abstractions`
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- No ASP.NET Core, no EF Core, no SixLabors

---

## `PeanutVision.Api` — Decomposed Components

### `IAcquisitionSession` (ISP — acquisition concerns only)

```csharp
public interface IAcquisitionSession
{
    bool IsActive { get; }
    bool HasFrame { get; }
    string? LastError { get; }
    void Start(int? frameCount = null);
    void Stop();
    Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000);
    ImageData? GetLatestFrame();
    AcquisitionStatisticsSnapshot? GetStatistics();
    IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50);
    event Action<ImageData> FrameAcquired;
    void SendTrigger();
}

/// <summary>
/// Manages one GrabChannel internally. Creates it on Start(), releases on Stop().
/// Subscribes to GrabChannel frame events and enqueues frames into IFrameQueue.
/// </summary>
public sealed class AcquisitionSession : IAcquisitionSession
{
    public AcquisitionSession(
        IGrabService grabService,
        IFrameQueue frameQueue,
        ILatencyService latencyService) { ... }
}
```

### `IExposureController` (ISP — exposure concerns only)

```csharp
public interface IExposureController
{
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs);
}

/// <summary>Reads/writes exposure on the active channel via IAcquisitionSession.</summary>
public sealed class ExposureController : IExposureController
{
    public ExposureController(IAcquisitionSession session) { ... }
}
```

### `ISnapshotCapture` (ISP — single-frame capture only)

```csharp
public interface ISnapshotCapture
{
    /// <summary>Creates a temporary channel, captures one frame, saves it, returns file path.</summary>
    Task<string> CaptureAsync(ProfileId profileId, TriggerMode? triggerMode = null);
}

public sealed class SnapshotCapture : ISnapshotCapture
{
    public SnapshotCapture(
        IGrabService grabService,
        IFrameWriter frameWriter,
        ILatencyService latencyService,
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService) { ... }
}
```

### `FrameSavedHandler` (SRP — post-save side effects only)

```csharp
/// <summary>
/// Subscribes to IFrameQueue drain completion.
/// Responsibility: DB record + thumbnail generation after a frame is written to disk.
/// </summary>
public sealed class FrameSavedHandler
{
    public FrameSavedHandler(
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService) { ... }

    public Task HandleAsync(FrameSavedEvent e) { ... }
}
```

---

## Data Flow

```
CaptureScheduler.Tick
        ↓
AcquisitionSession.SendTrigger()
        ↓  [GrabChannel callback — < 1ms]
AcquisitionSession raises FrameAcquired(ImageData)
        ↓
IFrameQueue.TryEnqueue(frame)        ← capture thread returns immediately
        ↓  [background writer Task]
IFrameWriter.Write()                 ← slow file I/O, OK to block here
        ↓
FrameSavedEvent published
        ↓
FrameSavedHandler.HandleAsync()      ← DB record + thumbnail
```

**Queue overflow:** frame is dropped and a warning is logged. Capture timing is never blocked.

---

## Console / MCP Host Pattern

A host using only `PeanutVision.Capture` and `PeanutVision.MultiCamDriver`:

```csharp
var grabService = new GrabService();
grabService.Initialize();

var queue = new BoundedFrameQueue(capacity: 16);
var writer = new ImageFileWriter(new ImageWriter());
var scheduler = new CaptureScheduler();

// Wire: scheduler tick → session trigger
var session = new AcquisitionSession(grabService, queue, NullLatencyService.Instance);
scheduler.Tick += () => session.SendTrigger();

// Wire: queue drain → file write → stdout
_ = Task.Run(async () =>
{
    await foreach (var frame in queue.ReadAllAsync(cts.Token))
    {
        var path = writer.Write(frame, options);
        Console.WriteLine(path);
    }
});

scheduler.Start(TimeSpan.FromSeconds(2));

// Camera status — directly via GrabService (no wrapper needed)
var status = grabService.GetBoardStatus(0);

await foreach (var line in ReadStdinAsync())
{
    if (line == "stop")    { scheduler.Stop(); break; }
    if (line == "capture")   session.SendTrigger();
    if (line == "status")    Console.WriteLine(grabService.GetBoardStatus(0));
}
```

---

## What Does NOT Change

- `PeanutVision.MultiCamDriver` — zero changes
- `PeanutVision.FakeCamDriver` — zero changes
- All REST controller routes and response contracts
- DB schema and migrations
- `peanut-vision-ui` — zero changes
- Existing API integration tests — must continue to pass

---

## Testing Strategy

- `AcquisitionSession`, `ExposureController`, `SnapshotCapture` — unit tested with `FakeMultiCamHAL`
- `BoundedFrameQueue` — unit tested: overflow drops frame, drain completes on dispose
- `ImageFileWriter` — unit tested with temp directory
- `FrameSavedHandler` — unit tested with mock repository
- Existing xUnit API integration specs — regression pass required
