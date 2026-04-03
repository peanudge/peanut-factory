# Design: Capture Core Modularization

**Date:** 2026-04-03  
**Status:** Approved  
**Scope:** Extract `PeanutVision.Capture` class library; refactor API to use it; enable future Console/MCP hosts

---

## Problem

`PeanutVision.Api` mixes the core capture pipeline (MultiCam driver → frame grab → file save) with high-level features (gallery, annotations, histogram, latency, thumbnails, DB). This makes it impossible to reuse the capture pipeline from a simple Console or MCP server without pulling in the entire ASP.NET Core stack.

---

## Goal

- Extract a `PeanutVision.Capture` class library with no ASP.NET Core dependency
- Core responsibility: periodic/on-demand capture, async fire-and-forget file save, `FrameSaved` event
- API refactored to use this library — existing REST surface and UI unchanged
- Console or MCP host can be built by referencing only `PeanutVision.Capture`

---

## Project Structure

```
PeanutVision.MultiCamDriver    ← unchanged (HAL, GrabService, GrabChannel, ImageSaver)
PeanutVision.FakeCamDriver     ← unchanged (fake HAL for tests)
        │
        ▼
PeanutVision.Capture  ← NEW class library
        │
   ┌────┴────┐
   ▼         ▼
PeanutVision.Api    PeanutVision.Console (future)
(refactored)        PeanutVision.Mcp    (future)
```

### `PeanutVision.Capture` dependencies
- `PeanutVision.MultiCamDriver`
- `Microsoft.Extensions.Logging.Abstractions`
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- No ASP.NET Core, no EF Core, no SixLabors

---

## `PeanutVision.Capture` Public API

### `CaptureOptions`

```csharp
public record CaptureOptions(
    string OutputDirectory,
    string FilenamePattern,   // e.g. "frame_{timestamp:yyyyMMdd_HHmmss_fff}"
    ImageFormat Format,        // Png | Bmp | Raw
    int QueueCapacity = 16    // max frames buffered before drop
);
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

### `CaptureEngine`

```csharp
public sealed class CaptureEngine : IDisposable
{
    public event Action<FrameSavedEvent>? FrameSaved;

    // Periodic capture: fires every `interval`, saves async
    public void StartPeriodic(TimeSpan interval);

    // Single capture: enqueues one frame immediately
    public void TriggerOnce();

    // Graceful stop: waits for queued frames to drain
    public Task StopAsync();
}
```

---

## Async Save Pipeline

```
[Timer / TriggerOnce()]
        │
        ▼
  GrabChannel.Trigger()        ← hardware capture, < 1ms
        │
        ▼
  Channel<RawFrame>.Write()    ← lock-free queue (System.Threading.Channels)
        │                         capture thread returns immediately
        ▼
  [background writer Task]
        ├── ImageSaver.Save()  ← file I/O (slow, blocking OK here)
        └── FrameSaved event   ← subscribers notified after write
```

**Queue overflow policy:** when the queue is full (`QueueCapacity` exceeded), the incoming frame is dropped and a warning is logged. Capture timing is never blocked.

---

## API Refactoring

Only the internal wiring changes. The REST API surface, DB schema, and UI are untouched.

| File | Change |
|------|--------|
| `AcquisitionManager.cs` | Replace direct `GrabService`/`GrabChannel` orchestration with `CaptureEngine` |
| `ImageCaptureService.cs` | Subscribe to `CaptureEngine.FrameSaved`; write DB record + generate thumbnail |
| `Program.cs` | Register `CaptureEngine` as singleton; wire `CaptureOptions` from config |
| `PeanutVision.Api.csproj` | Add `<ProjectReference>` to `PeanutVision.Capture` |

**Unchanged:**
- `ImagesController`, `LatencyController`, `SettingsController`, `PresetController`
- All DB migrations and schema
- `peanut-vision-ui` — zero changes
- All existing REST contracts

---

## Future Console / MCP Host Pattern

A minimal host referencing only `PeanutVision.Capture`:

```csharp
// Program.cs of PeanutVision.Console (future)
var engine = new CaptureEngine(grabService, options, logger);
engine.FrameSaved += e => Console.WriteLine(e.FilePath);
engine.StartPeriodic(TimeSpan.FromSeconds(2));

await foreach (var line in ReadStdinAsync())
{
    if (line == "stop")   { await engine.StopAsync(); break; }
    if (line == "capture") engine.TriggerOnce();
}
```

No ASP.NET Core, no DB, no HTTP — just the capture pipeline.

---

## Testing Strategy

- `PeanutVision.Capture` unit tests: use `FakeCamDriver`'s `FakeMultiCamHAL` to exercise `CaptureEngine` without hardware
- Verify: periodic triggers fire at expected interval, queue overflow drops frames, `FrameSaved` event fires after save, `StopAsync` drains queue before returning
- API integration tests: existing xUnit specs must continue to pass unchanged

---

## Out of Scope

- Console or MCP host implementation (design only, not built in this iteration)
- Changes to gallery, annotations, histogram, latency features
- DB schema changes
- UI changes
