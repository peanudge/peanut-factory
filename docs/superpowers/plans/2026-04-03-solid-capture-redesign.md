# SOLID Capture Pipeline Redesign — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extract `PeanutVision.Capture` library and decompose `AcquisitionManager` + `ImageCaptureService` into single-responsibility components following SOLID principles.

**Architecture:** A new `PeanutVision.Capture` project provides `IFrameWriter`, `IFrameQueue`, and `CaptureScheduler` with no ASP.NET dependency. The Api replaces the monolithic `AcquisitionManager` with `AcquisitionSession`, `ExposureController`, `SnapshotCapture`, and `FrameSavedHandler`, wired together by a `FrameWriterBackgroundService`.

**Tech Stack:** .NET 10, C# 12, xUnit, `System.Threading.Channels`, `SixLabors.ImageSharp` (via MultiCamDriver)

---

## File Map

### New — `src/PeanutVision.Capture/`
| File | Responsibility |
|------|----------------|
| `PeanutVision.Capture.csproj` | Project file, refs MultiCamDriver |
| `OutputFormat.cs` | Enum: Png, Bmp, Raw |
| `FrameWriterOptions.cs` | Record: OutputDirectory, FilenamePrefix, TimestampFormat, Format |
| `FrameSavedEvent.cs` | Record: FilePath, CapturedAt, Width, Height, FileSizeBytes |
| `IFrameWriter.cs` | Interface: Write(ImageData, FrameWriterOptions) → string |
| `ImageFileWriter.cs` | Implements IFrameWriter using ImageWriter from MultiCamDriver |
| `IFrameQueue.cs` | Interface: TryEnqueue, ReadAllAsync |
| `BoundedFrameQueue.cs` | Implements IFrameQueue using Channel\<T\> with DropWrite |
| `CaptureScheduler.cs` | Timer wrapper, fires Tick event at interval |
| `ServiceCollectionExtensions.cs` | AddCaptureServices() helper |

### New — `src/PeanutVision.Capture.Tests/`
| File | Responsibility |
|------|----------------|
| `PeanutVision.Capture.Tests.csproj` | xUnit project |
| `BoundedFrameQueueTests.cs` | Queue capacity, overflow, drain |
| `ImageFileWriterTests.cs` | File creation, path return, format |
| `CaptureSchedulerTests.cs` | Interval firing, Stop prevents ticks |

### New — `src/PeanutVision.Api/Services/`
| File | Responsibility |
|------|----------------|
| `IAcquisitionSession.cs` | Merged interface: channel lifecycle + acquisition state |
| `IExposureSource.cs` | Internal: channel exposure access for ExposureController |
| `AcquisitionSession.cs` | Implements IAcquisitionSession + IExposureSource |
| `IExposureController.cs` | Get/set exposure |
| `ExposureController.cs` | Delegates to IExposureSource |
| `ISnapshotCapture.cs` | Single async capture → file path |
| `SnapshotCapture.cs` | Temp channel, capture, save, DB record |
| `FrameSavedHandler.cs` | DB record + thumbnail after FrameSavedEvent |
| `FrameWriterBackgroundService.cs` | BackgroundService: drains IFrameQueue → IFrameWriter → FrameSavedHandler |
| `ImageSaveSettingsExtensions.cs` | ToWriterOptions(contentRoot) helper |

### Modified
| File | Change |
|------|--------|
| `PeanutVision.Api.csproj` | Add ProjectReference to PeanutVision.Capture |
| `Program.cs` | Replace AcquisitionManager DI with new components |
| `Controllers/AcquisitionController.cs` | Use IAcquisitionSession, IExposureController, ISnapshotCapture |

### Deleted
| File | Reason |
|------|--------|
| `Services/AcquisitionManager.cs` | Replaced by AcquisitionSession + ExposureController + SnapshotCapture |
| `Services/IAcquisitionService.cs` | Replaced by IAcquisitionSession |
| `Services/IChannelService.cs` | Merged into IAcquisitionSession |
| `Services/IExposureControl.cs` | Replaced by IExposureController |
| `Services/ImageCaptureService.cs` | Replaced by FrameWriterBackgroundService + FrameSavedHandler |
| `Services/IImageCaptureService.cs` | Same |
| `Services/FrameSaveTracker.cs` | No longer needed — all frames go through queue |

---

## Task 1: Create `PeanutVision.Capture` project scaffold

**Files:**
- Create: `src/PeanutVision.Capture/PeanutVision.Capture.csproj`
- Create: `src/PeanutVision.Capture/OutputFormat.cs`
- Create: `src/PeanutVision.Capture/FrameWriterOptions.cs`
- Create: `src/PeanutVision.Capture/FrameSavedEvent.cs`

- [ ] **Step 1: Create project file**

```xml
<!-- src/PeanutVision.Capture/PeanutVision.Capture.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <Description>Capture pipeline abstractions — frame queue, file writer, scheduler. No ASP.NET dependency.</Description>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\PeanutVision.MultiCamDriver\PeanutVision.MultiCamDriver.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Create value types**

```csharp
// src/PeanutVision.Capture/OutputFormat.cs
namespace PeanutVision.Capture;

public enum OutputFormat { Png, Bmp, Raw }
```

```csharp
// src/PeanutVision.Capture/FrameWriterOptions.cs
namespace PeanutVision.Capture;

public sealed record FrameWriterOptions
{
    public string OutputDirectory { get; init; } = "CapturedImages";
    public string FilenamePrefix { get; init; } = "frame";
    public string TimestampFormat { get; init; } = "yyyyMMdd_HHmmss_fff";
    public OutputFormat Format { get; init; } = OutputFormat.Png;
}
```

```csharp
// src/PeanutVision.Capture/FrameSavedEvent.cs
namespace PeanutVision.Capture;

public sealed record FrameSavedEvent(
    string FilePath,
    DateTime CapturedAt,
    int Width,
    int Height,
    long FileSizeBytes
);
```

- [ ] **Step 3: Add to solution**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
dotnet sln add src/PeanutVision.Capture/PeanutVision.Capture.csproj
dotnet build src/PeanutVision.Capture
```

Expected: Build succeeded, 0 errors.

- [ ] **Step 4: Commit**

```bash
git add src/PeanutVision.Capture/
git commit -m "feat: scaffold PeanutVision.Capture project with value types"
```

---

## Task 2: `IFrameWriter` + `ImageFileWriter`

**Files:**
- Create: `src/PeanutVision.Capture/IFrameWriter.cs`
- Create: `src/PeanutVision.Capture/ImageFileWriter.cs`
- Create: `src/PeanutVision.Capture.Tests/PeanutVision.Capture.Tests.csproj`
- Create: `src/PeanutVision.Capture.Tests/ImageFileWriterTests.cs`

- [ ] **Step 1: Create test project**

```xml
<!-- src/PeanutVision.Capture.Tests/PeanutVision.Capture.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\PeanutVision.Capture\PeanutVision.Capture.csproj" />
    <ProjectReference Include="..\PeanutVision.MultiCamDriver\PeanutVision.MultiCamDriver.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Write failing tests**

```csharp
// src/PeanutVision.Capture.Tests/ImageFileWriterTests.cs
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture.Tests;

public class ImageFileWriterTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    private static ImageData MakeFrame(int w = 4, int h = 4)
        => new(new byte[w * h * 3], w, h, w * 3);

    [Fact]
    public void Write_CreatesFileAtReturnedPath()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, Format = OutputFormat.Png };

        var path = writer.Write(MakeFrame(), opts);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public void Write_ReturnsPathWithCorrectExtension_Png()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, Format = OutputFormat.Png };

        var path = writer.Write(MakeFrame(), opts);

        Assert.EndsWith(".png", path, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Write_ReturnsPathWithCorrectExtension_Bmp()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, Format = OutputFormat.Bmp };

        var path = writer.Write(MakeFrame(), opts);

        Assert.EndsWith(".bmp", path, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Write_CreatesOutputDirectoryIfMissing()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var nested = Path.Combine(_tempDir, "sub", "dir");
        var opts = new FrameWriterOptions { OutputDirectory = nested, Format = OutputFormat.Png };

        writer.Write(MakeFrame(), opts);

        Assert.True(Directory.Exists(nested));
    }

    [Fact]
    public void Write_FilenameContainsPrefix()
    {
        var writer = new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter());
        var opts = new FrameWriterOptions { OutputDirectory = _tempDir, FilenamePrefix = "myprefix", Format = OutputFormat.Png };

        var path = writer.Write(MakeFrame(), opts);

        Assert.Contains("myprefix", Path.GetFileName(path));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }
}
```

- [ ] **Step 3: Add test project to solution and run (should fail)**

```bash
dotnet sln add src/PeanutVision.Capture.Tests/PeanutVision.Capture.Tests.csproj
dotnet test src/PeanutVision.Capture.Tests --no-build 2>&1 | head -5
```

Expected: Build error — `ImageFileWriter` not found.

- [ ] **Step 4: Create interface and implementation**

```csharp
// src/PeanutVision.Capture/IFrameWriter.cs
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

public interface IFrameWriter
{
    /// <summary>Writes the image to disk. Returns the actual file path written.</summary>
    string Write(ImageData image, FrameWriterOptions options);
}
```

```csharp
// src/PeanutVision.Capture/ImageFileWriter.cs
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

public sealed class ImageFileWriter : IFrameWriter
{
    private readonly ImageWriter _writer;

    public ImageFileWriter(ImageWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public string Write(ImageData image, FrameWriterOptions options)
    {
        Directory.CreateDirectory(options.OutputDirectory);

        var ext = options.Format switch
        {
            OutputFormat.Bmp => ".bmp",
            OutputFormat.Raw => ".raw",
            _                => ".png",
        };

        var filename = $"{options.FilenamePrefix}_{DateTime.Now.ToString(options.TimestampFormat)}{ext}";
        var filePath = Path.Combine(options.OutputDirectory, filename);

        _writer.Save(image, filePath);
        return filePath;
    }
}
```

- [ ] **Step 5: Run tests**

```bash
dotnet test src/PeanutVision.Capture.Tests -v minimal
```

Expected: 5 tests passed.

- [ ] **Step 6: Commit**

```bash
git add src/PeanutVision.Capture/ src/PeanutVision.Capture.Tests/
git commit -m "feat: add IFrameWriter and ImageFileWriter with tests"
```

---

## Task 3: `IFrameQueue` + `BoundedFrameQueue`

**Files:**
- Create: `src/PeanutVision.Capture/IFrameQueue.cs`
- Create: `src/PeanutVision.Capture/BoundedFrameQueue.cs`
- Create: `src/PeanutVision.Capture.Tests/BoundedFrameQueueTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
// src/PeanutVision.Capture.Tests/BoundedFrameQueueTests.cs
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture.Tests;

public class BoundedFrameQueueTests
{
    private static ImageData MakeFrame() => new(new byte[12], 2, 2, 6);

    [Fact]
    public void TryEnqueue_BelowCapacity_ReturnsTrue()
    {
        using var queue = new BoundedFrameQueue(capacity: 4);
        Assert.True(queue.TryEnqueue(MakeFrame()));
    }

    [Fact]
    public void TryEnqueue_AtCapacity_ReturnsFalse()
    {
        using var queue = new BoundedFrameQueue(capacity: 1);
        queue.TryEnqueue(MakeFrame()); // fill
        Assert.False(queue.TryEnqueue(MakeFrame())); // overflow → drop
    }

    [Fact]
    public async Task ReadAllAsync_YieldsAllEnqueuedFrames()
    {
        using var queue = new BoundedFrameQueue(capacity: 4);
        var f1 = MakeFrame();
        var f2 = MakeFrame();
        queue.TryEnqueue(f1);
        queue.TryEnqueue(f2);
        queue.Dispose(); // complete the channel so ReadAllAsync terminates

        var results = new List<ImageData>();
        await foreach (var f in queue.ReadAllAsync(CancellationToken.None))
            results.Add(f);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task ReadAllAsync_CompletesOnDispose()
    {
        var queue = new BoundedFrameQueue(capacity: 4);
        var cts = new CancellationTokenSource();

        var readerTask = Task.Run(async () =>
        {
            var count = 0;
            await foreach (var _ in queue.ReadAllAsync(cts.Token))
                count++;
            return count;
        });

        queue.TryEnqueue(MakeFrame());
        await Task.Delay(20);
        queue.Dispose(); // signals completion

        var received = await readerTask.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.Equal(1, received);
    }
}
```

- [ ] **Step 2: Run (expect build failure)**

```bash
dotnet test src/PeanutVision.Capture.Tests --no-build 2>&1 | head -5
```

Expected: Build error — `BoundedFrameQueue` not found.

- [ ] **Step 3: Implement interface and class**

```csharp
// src/PeanutVision.Capture/IFrameQueue.cs
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

public interface IFrameQueue : IDisposable
{
    /// <summary>Enqueues a frame for async saving. Returns false and drops the frame if the queue is full.</summary>
    bool TryEnqueue(ImageData frame);

    /// <summary>Async stream of frames for the background writer to consume.</summary>
    IAsyncEnumerable<ImageData> ReadAllAsync(CancellationToken ct);
}
```

```csharp
// src/PeanutVision.Capture/BoundedFrameQueue.cs
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

public sealed class BoundedFrameQueue : IFrameQueue
{
    private readonly Channel<ImageData> _channel;

    public BoundedFrameQueue(int capacity = 16)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _channel = Channel.CreateBounded<ImageData>(new BoundedChannelOptions(capacity)
        {
            FullMode    = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public bool TryEnqueue(ImageData frame) => _channel.Writer.TryWrite(frame);

    public IAsyncEnumerable<ImageData> ReadAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);

    public void Dispose() => _channel.Writer.TryComplete();
}
```

- [ ] **Step 4: Run tests**

```bash
dotnet test src/PeanutVision.Capture.Tests -v minimal
```

Expected: 9 tests passed (5 from Task 2 + 4 new).

- [ ] **Step 5: Commit**

```bash
git add src/PeanutVision.Capture/
git commit -m "feat: add IFrameQueue and BoundedFrameQueue with tests"
```

---

## Task 4: `CaptureScheduler`

**Files:**
- Create: `src/PeanutVision.Capture/CaptureScheduler.cs`
- Create: `src/PeanutVision.Capture.Tests/CaptureSchedulerTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
// src/PeanutVision.Capture.Tests/CaptureSchedulerTests.cs
using PeanutVision.Capture;

namespace PeanutVision.Capture.Tests;

public class CaptureSchedulerTests
{
    [Fact]
    public async Task Start_FiresTickMultipleTimes()
    {
        var ticks = 0;
        using var scheduler = new CaptureScheduler();
        scheduler.Tick += () => Interlocked.Increment(ref ticks);

        scheduler.Start(TimeSpan.FromMilliseconds(40));
        await Task.Delay(200);
        scheduler.Stop();

        // At 40ms interval over 200ms: expect at least 3 ticks
        Assert.True(ticks >= 3, $"Expected >= 3 ticks, got {ticks}");
    }

    [Fact]
    public async Task Stop_PreventsFurtherTicks()
    {
        var ticks = 0;
        using var scheduler = new CaptureScheduler();
        scheduler.Tick += () => Interlocked.Increment(ref ticks);

        scheduler.Start(TimeSpan.FromMilliseconds(30));
        await Task.Delay(80);
        scheduler.Stop();
        var countAfterStop = ticks;

        await Task.Delay(100); // wait — no more ticks should fire

        Assert.Equal(countAfterStop, ticks);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var scheduler = new CaptureScheduler();
        scheduler.Start(TimeSpan.FromMilliseconds(50));
        var ex = Record.Exception(() => scheduler.Dispose());
        Assert.Null(ex);
    }
}
```

- [ ] **Step 2: Implement**

```csharp
// src/PeanutVision.Capture/CaptureScheduler.cs
namespace PeanutVision.Capture;

/// <summary>Fires Tick at a fixed interval. Single responsibility: timing only.</summary>
public sealed class CaptureScheduler : IDisposable
{
    private Timer? _timer;
    private bool _disposed;

    public event Action? Tick;

    public void Start(TimeSpan interval)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _timer?.Dispose();
        _timer = new Timer(_ => Tick?.Invoke(), null, TimeSpan.Zero, interval);
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public void Dispose()
    {
        _disposed = true;
        Stop();
    }
}
```

- [ ] **Step 3: Run tests**

```bash
dotnet test src/PeanutVision.Capture.Tests -v minimal
```

Expected: 12 tests passed.

- [ ] **Step 4: Commit**

```bash
git add src/PeanutVision.Capture/ src/PeanutVision.Capture.Tests/
git commit -m "feat: add CaptureScheduler with tests"
```

---

## Task 5: `IAcquisitionSession` + `IExposureSource`

**Files:**
- Create: `src/PeanutVision.Api/Services/IAcquisitionSession.cs`
- Create: `src/PeanutVision.Api/Services/IExposureSource.cs`
- Modify: `src/PeanutVision.Api/PeanutVision.Api.csproj`

- [ ] **Step 1: Add Capture project reference**

Edit `src/PeanutVision.Api/PeanutVision.Api.csproj` — add inside `<ItemGroup>`:
```xml
<ProjectReference Include="..\PeanutVision.Capture\PeanutVision.Capture.csproj" />
```

- [ ] **Step 2: Create `IAcquisitionSession`**

```csharp
// src/PeanutVision.Api/Services/IAcquisitionSession.cs
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IAcquisitionSession : IDisposable
{
    bool IsActive { get; }
    bool HasFrame { get; }
    string? LastError { get; }
    ChannelState ChannelState { get; }
    ProfileId? ActiveProfileId { get; }
    IReadOnlySet<ChannelAction> GetAllowedActions();

    /// <summary>Creates a channel for the given profile and starts acquisition.</summary>
    void Start(ProfileId profileId, TriggerMode? triggerMode = null,
               int? frameCount = null, int? intervalMs = null);

    /// <summary>Stops acquisition and releases the channel.</summary>
    void Stop();

    /// <summary>Sends a software trigger. Used by CaptureScheduler.</summary>
    void SendTrigger();

    Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000);
    ImageData? GetLatestFrame();
    AcquisitionStatisticsSnapshot? GetStatistics();
    IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50);

    event Action<ImageData>? FrameAcquired;
}
```

- [ ] **Step 3: Create `IExposureSource` (internal — Api project only)**

```csharp
// src/PeanutVision.Api/Services/IExposureSource.cs
namespace PeanutVision.Api.Services;

/// <summary>
/// Internal interface exposing channel exposure access to ExposureController.
/// Not visible outside PeanutVision.Api.
/// </summary>
internal interface IExposureSource
{
    bool HasActiveChannel { get; }
    double GetExposureUs();
    (double Min, double Max) GetExposureRange();
    void SetExposureUs(double us);
}
```

- [ ] **Step 4: Build to verify**

```bash
dotnet build src/PeanutVision.Api
```

Expected: Build succeeded.

- [ ] **Step 5: Commit**

```bash
git add src/PeanutVision.Api/
git commit -m "feat: add IAcquisitionSession and IExposureSource interfaces"
```

---

## Task 6: `AcquisitionSession`

**Files:**
- Create: `src/PeanutVision.Api/Services/AcquisitionSession.cs`

- [ ] **Step 1: Implement**

```csharp
// src/PeanutVision.Api/Services/AcquisitionSession.cs
using System.Collections.Concurrent;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionSession : IAcquisitionSession, IExposureSource
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly IFrameQueue _frameQueue;
    private readonly ILatencyService _latencyService;
    private readonly object _lock = new();
    private readonly ChannelEventLog _eventLog = new();
    private readonly ConcurrentQueue<DateTimeOffset> _triggerTimestamps = new();

    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _triggerTcs;
    private ChannelState _state = ChannelState.None;
    private ProfileId? _activeProfileId;
    private bool _disposed;

    public event Action<ImageData>? FrameAcquired;

    public bool IsActive => _state == ChannelState.Active;
    public bool HasFrame { get { lock (_lock) return _lastFrame != null; } }
    public string? LastError { get { lock (_lock) return _lastError; } }
    public ChannelState ChannelState { get { lock (_lock) return _state; } }
    public ProfileId? ActiveProfileId { get { lock (_lock) return _activeProfileId; } }

    // IExposureSource (internal — accessed by ExposureController via DI)
    bool IExposureSource.HasActiveChannel { get { lock (_lock) return _channel?.IsActive ?? false; } }
    double IExposureSource.GetExposureUs() { lock (_lock) return _channel?.GetExposureUs() ?? 10_000.0; }
    (double Min, double Max) IExposureSource.GetExposureRange() { lock (_lock) return _channel?.GetExposureRange() ?? (1_000, 100_000); }
    void IExposureSource.SetExposureUs(double us) { lock (_lock) _channel?.SetExposureUs(us); }

    public AcquisitionSession(
        IGrabService grabService,
        ICamFileService camFileService,
        IFrameQueue frameQueue,
        ILatencyService latencyService)
    {
        _grabService = grabService;
        _camFileService = camFileService;
        _frameQueue = frameQueue;
        _latencyService = latencyService;
    }

    public void Start(ProfileId profileId, TriggerMode? triggerMode = null,
                      int? frameCount = null, int? intervalMs = null)
    {
        const int minIntervalMs = 50;
        if (intervalMs.HasValue && intervalMs.Value > 0 && intervalMs.Value < minIntervalMs)
            throw new ArgumentException($"intervalMs must be >= {minIntervalMs}ms, got {intervalMs.Value}ms.");

        lock (_lock)
        {
            if (_state == ChannelState.Active)
                throw new InvalidOperationException("Acquisition is already active. Stop it first.");

            // Release any idle channel from a previous run
            if (_channel != null)
            {
                _grabService.ReleaseChannel(_channel);
                _channel = null;
                _state = ChannelState.None;
            }

            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode)
                : camFile.ToChannelOptions();

            _channel = _grabService.CreateChannel(options);
            _activeProfileId = profileId;
            _lastFrame = null;
            _lastError = null;
            _statistics = new AcquisitionStatistics();

            _channel.FrameAcquired   += OnFrameAcquired;
            _channel.AcquisitionError += OnAcquisitionError;
            _channel.AcquisitionEnded += OnAcquisitionEnded;

            _statistics.Start();
            _channel.StartAcquisition(frameCount ?? -1);
            _state = ChannelState.Active;
        }

        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
            $"Started with profile '{profileId.Value}'" +
            (frameCount.HasValue ? $", frameCount={frameCount}" : "") +
            (intervalMs.HasValue ? $", intervalMs={intervalMs}" : "")));
    }

    public void Stop()
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            if (_state != ChannelState.Active) return;
            _state = ChannelState.Idle;

            tcs = _triggerTcs;
            _triggerTcs = null;

            _statistics?.Stop();
            _channel!.StopAcquisition();
            _channel.FrameAcquired   -= OnFrameAcquired;
            _channel.AcquisitionError -= OnAcquisitionError;
            _channel.AcquisitionEnded -= OnAcquisitionEnded;
            _grabService.ReleaseChannel(_channel);
            _channel = null;
            _activeProfileId = null;
            _state = ChannelState.None;
        }

        tcs?.TrySetCanceled();
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStopped, "Stopped"));
    }

    public void SendTrigger()
    {
        lock (_lock)
        {
            if (_channel?.IsActive == true)
            {
                _triggerTimestamps.Enqueue(DateTimeOffset.UtcNow);
                _channel.SendSoftwareTrigger();
            }
        }
    }

    public async Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000)
    {
        TaskCompletionSource<ImageData> tcs;

        lock (_lock)
        {
            if (_channel == null || !_channel.IsActive)
                throw new InvalidOperationException("No active acquisition. Start acquisition first.");
            if (_triggerTcs != null)
                throw new InvalidOperationException("A trigger is already pending.");
            if (!_channel.SupportsSoftwareTrigger)
                throw new InvalidOperationException(
                    $"TriggerAndWaitAsync requires SOFT or COMBINED trigger mode, " +
                    $"but channel is configured for {_channel.TriggerMode}.");

            tcs = new TaskCompletionSource<ImageData>(TaskCreationOptions.RunContinuationsAsynchronously);
            _triggerTcs = tcs;
            _triggerTimestamps.Enqueue(DateTimeOffset.UtcNow);
            _channel.SendSoftwareTrigger();
        }

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
        if (completed != tcs.Task)
        {
            lock (_lock) { _triggerTcs = null; }
            throw new TimeoutException("Trigger timed out waiting for frame.");
        }

        return await tcs.Task;
    }

    public ImageData? GetLatestFrame() { lock (_lock) return _lastFrame; }

    public AcquisitionStatisticsSnapshot? GetStatistics()
    {
        lock (_lock)
        {
            var snap = _statistics?.GetSnapshot();
            if (snap.HasValue && _channel != null)
                return snap.Value with
                {
                    CopyDropCount          = _channel.CopyDropCount,
                    ClusterUnavailableCount = _channel.ClusterUnavailableCount,
                };
            return snap;
        }
    }

    public IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50) => _eventLog.GetRecent(max);

    public IReadOnlySet<ChannelAction> GetAllowedActions()
    {
        lock (_lock)
        {
            return _state switch
            {
                ChannelState.None   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
                ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
                ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
                _                   => new HashSet<ChannelAction>(),
            };
        }
    }

    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
    {
        var frameAt = DateTimeOffset.UtcNow;
        TaskCompletionSource<ImageData>? tcs;
        long frameIndex;
        string? profileId;

        lock (_lock)
        {
            _lastFrame = e.Image;
            _statistics?.RecordFrame();
            frameIndex = _statistics?.FrameCount ?? 0;
            profileId  = _activeProfileId?.Value;
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        if (_triggerTimestamps.TryDequeue(out var triggerAt))
            _latencyService.Record(triggerAt, frameAt, frameIndex, profileId);

        _frameQueue.TryEnqueue(e.Image);
        FrameAcquired?.Invoke(e.Image);
        tcs?.TrySetResult(e.Image);
    }

    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
    {
        TaskCompletionSource<ImageData>? tcs;

        lock (_lock)
        {
            _lastError = e.Message;
            _statistics?.RecordError();
            if (e.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE) _statistics?.RecordDroppedFrame();
            tcs = _triggerTcs;
            _triggerTcs = null;
        }

        var eventType = e.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE
            ? ChannelEventType.BufferUnavailable
            : ChannelEventType.AcquisitionError;
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, eventType, e.Message));
        tcs?.TrySetException(new InvalidOperationException($"Acquisition error: {e.Message}"));

        if (e.Signal == McSignal.MC_SIG_UNRECOVERABLE_OVERRUN)
            Task.Run(Stop);
    }

    private void OnAcquisitionEnded(object? sender, EventArgs e) => Task.Run(Stop);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Stop();
    }
}
```

- [ ] **Step 2: Build to verify**

```bash
dotnet build src/PeanutVision.Api
```

Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/PeanutVision.Api/Services/AcquisitionSession.cs
git commit -m "feat: implement AcquisitionSession (SRP — replaces AcquisitionManager core)"
```

---

## Task 7: `IExposureController` + `ExposureController`

**Files:**
- Create: `src/PeanutVision.Api/Services/IExposureController.cs`
- Create: `src/PeanutVision.Api/Services/ExposureController.cs`

- [ ] **Step 1: Create interface**

```csharp
// src/PeanutVision.Api/Services/IExposureController.cs
namespace PeanutVision.Api.Services;

public interface IExposureController
{
    ExposureInfo GetExposure();
    ExposureInfo SetExposure(double? exposureUs);
}
```

- [ ] **Step 2: Implement**

```csharp
// src/PeanutVision.Api/Services/ExposureController.cs
namespace PeanutVision.Api.Services;

public sealed class ExposureController : IExposureController
{
    private readonly IExposureSource _source;
    private double _desiredExposureUs = 10_000.0;

    public ExposureController(IExposureSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public ExposureInfo GetExposure()
    {
        if (_source.HasActiveChannel)
        {
            _desiredExposureUs = _source.GetExposureUs();
            var (min, max) = _source.GetExposureRange();
            return new ExposureInfo
            {
                ExposureUs    = _desiredExposureUs,
                ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
            };
        }
        return new ExposureInfo { ExposureUs = _desiredExposureUs };
    }

    public ExposureInfo SetExposure(double? exposureUs)
    {
        if (exposureUs.HasValue)
            _desiredExposureUs = exposureUs.Value;

        if (_source.HasActiveChannel)
        {
            _source.SetExposureUs(_desiredExposureUs);
            var (min, max) = _source.GetExposureRange();
            return new ExposureInfo
            {
                ExposureUs    = _source.GetExposureUs(),
                ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
            };
        }
        return new ExposureInfo { ExposureUs = _desiredExposureUs };
    }
}
```

- [ ] **Step 3: Build**

```bash
dotnet build src/PeanutVision.Api
```

Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add src/PeanutVision.Api/Services/
git commit -m "feat: add IExposureController and ExposureController (ISP)"
```

---

## Task 8: `ISnapshotCapture` + `SnapshotCapture`

**Files:**
- Create: `src/PeanutVision.Api/Services/ISnapshotCapture.cs`
- Create: `src/PeanutVision.Api/Services/SnapshotCapture.cs`

- [ ] **Step 1: Create interface**

```csharp
// src/PeanutVision.Api/Services/ISnapshotCapture.cs
using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

public interface ISnapshotCapture
{
    /// <summary>
    /// Creates a temporary channel, captures one frame, saves it to disk, records in DB.
    /// Returns the saved file path.
    /// </summary>
    Task<string> CaptureAsync(ProfileId profileId, TriggerMode? triggerMode = null);
}
```

- [ ] **Step 2: Implement**

```csharp
// src/PeanutVision.Api/Services/SnapshotCapture.cs
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class SnapshotCapture : ISnapshotCapture
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly ILatencyService _latencyService;
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;
    private readonly string _contentRootPath;

    public SnapshotCapture(
        IGrabService grabService,
        ICamFileService camFileService,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        ILatencyService latencyService,
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService,
        IWebHostEnvironment environment)
    {
        _grabService      = grabService;
        _camFileService   = camFileService;
        _frameWriter      = frameWriter;
        _saveSettings     = saveSettings;
        _latencyService   = latencyService;
        _imageRepository  = imageRepository;
        _thumbnailService = thumbnailService;
        _contentRootPath  = environment.ContentRootPath;
    }

    public async Task<string> CaptureAsync(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        var camFile = _camFileService.GetByFileName(profileId.Value);
        var options = triggerMode.HasValue
            ? camFile.ToChannelOptions(triggerMode.Value.Mode, useCallback: false)
            : camFile.ToChannelOptions(useCallback: false);

        if (options.TriggerMode != McTrigMode.MC_TrigMode_SOFT &&
            options.TriggerMode != McTrigMode.MC_TrigMode_COMBINED)
        {
            options.TriggerMode = McTrigMode.MC_TrigMode_SOFT;
        }

        var channel = _grabService.CreateChannel(options);
        try
        {
            channel.StartAcquisition(1);
            var triggerAt = DateTimeOffset.UtcNow;
            channel.SendSoftwareTrigger();

            var surface = channel.WaitForFrame(5000)
                ?? throw new TimeoutException("Snapshot timed out waiting for frame.");

            var frameAt = DateTimeOffset.UtcNow;
            _latencyService.Record(triggerAt, frameAt, 1, profileId.Value);

            ImageData image;
            try   { image = ImageData.FromSurface(surface); }
            finally { channel.ReleaseSurface(surface); }

            var settings   = _saveSettings.GetSettings();
            var writerOpts = settings.ToWriterOptions(_contentRootPath);
            var filePath   = _frameWriter.Write(image, writerOpts);

            var thumbPath = await _thumbnailService.GenerateAsync(filePath);
            var fileInfo  = new FileInfo(filePath);

            await _imageRepository.AddAsync(new CapturedImage
            {
                Id             = Guid.NewGuid(),
                FilePath       = filePath,
                ThumbnailPath  = thumbPath,
                Width          = image.Width,
                Height         = image.Height,
                FileSizeBytes  = fileInfo.Exists ? fileInfo.Length : 0,
                Format         = settings.Format.ToString().ToLower(),
                CapturedAt     = DateTime.UtcNow,
            });

            return filePath;
        }
        finally
        {
            channel.StopAcquisition();
            _grabService.ReleaseChannel(channel);
        }
    }
}
```

- [ ] **Step 3: Create `ImageSaveSettingsExtensions`**

```csharp
// src/PeanutVision.Api/Services/ImageSaveSettingsExtensions.cs
using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

internal static class ImageSaveSettingsExtensions
{
    public static FrameWriterOptions ToWriterOptions(this ImageSaveSettings settings, string contentRootPath)
        => new()
        {
            OutputDirectory = Path.IsPathRooted(settings.OutputDirectory)
                ? settings.OutputDirectory
                : Path.Combine(contentRootPath, settings.OutputDirectory),
            FilenamePrefix   = string.IsNullOrWhiteSpace(settings.FilenamePrefix) ? "frame" : settings.FilenamePrefix,
            TimestampFormat  = settings.TimestampFormat,
            Format           = settings.Format switch
            {
                SaveImageFormat.Bmp => OutputFormat.Bmp,
                SaveImageFormat.Raw => OutputFormat.Raw,
                _                   => OutputFormat.Png,
            },
        };
}
```

- [ ] **Step 4: Build**

```bash
dotnet build src/PeanutVision.Api
```

Expected: Build succeeded.

- [ ] **Step 5: Commit**

```bash
git add src/PeanutVision.Api/Services/
git commit -m "feat: add ISnapshotCapture, SnapshotCapture, ImageSaveSettingsExtensions"
```

---

## Task 9: `FrameSavedHandler` + `FrameWriterBackgroundService`

**Files:**
- Create: `src/PeanutVision.Api/Services/FrameSavedHandler.cs`
- Create: `src/PeanutVision.Api/Services/FrameWriterBackgroundService.cs`

- [ ] **Step 1: Create `FrameSavedHandler`**

```csharp
// src/PeanutVision.Api/Services/FrameSavedHandler.cs
using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

/// <summary>Single responsibility: DB record + thumbnail after a frame is written to disk.</summary>
public sealed class FrameSavedHandler
{
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;
    private readonly IImageSaveSettingsService _saveSettings;

    public FrameSavedHandler(
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService,
        IImageSaveSettingsService saveSettings)
    {
        _imageRepository = imageRepository;
        _thumbnailService = thumbnailService;
        _saveSettings = saveSettings;
    }

    public async Task HandleAsync(FrameSavedEvent e)
    {
        var thumbPath = await _thumbnailService.GenerateAsync(e.FilePath);
        var format    = _saveSettings.GetSettings().Format.ToString().ToLower();

        await _imageRepository.AddAsync(new CapturedImage
        {
            Id            = Guid.NewGuid(),
            FilePath      = e.FilePath,
            ThumbnailPath = thumbPath,
            Width         = e.Width,
            Height        = e.Height,
            FileSizeBytes = e.FileSizeBytes,
            Format        = format,
            CapturedAt    = e.CapturedAt,
        });
    }
}
```

- [ ] **Step 2: Create `FrameWriterBackgroundService`**

```csharp
// src/PeanutVision.Api/Services/FrameWriterBackgroundService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

/// <summary>
/// Drains IFrameQueue → IFrameWriter → FrameSavedHandler.
/// Runs for the lifetime of the application.
/// </summary>
public sealed class FrameWriterBackgroundService : BackgroundService
{
    private readonly IFrameQueue _queue;
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FrameWriterBackgroundService> _logger;

    public FrameWriterBackgroundService(
        IFrameQueue queue,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment,
        ILogger<FrameWriterBackgroundService> logger)
    {
        _queue       = queue;
        _frameWriter = frameWriter;
        _saveSettings = saveSettings;
        _scopeFactory = scopeFactory;
        _environment  = environment;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var frame in _queue.ReadAllAsync(ct))
        {
            var settings = _saveSettings.GetSettings();
            if (!settings.AutoSave) continue;

            try
            {
                var opts     = settings.ToWriterOptions(_environment.ContentRootPath);
                var filePath = _frameWriter.Write(frame, opts);
                var fileInfo = new FileInfo(filePath);

                var evt = new FrameSavedEvent(
                    FilePath:    filePath,
                    CapturedAt:  DateTime.UtcNow,
                    Width:       frame.Width,
                    Height:      frame.Height,
                    FileSizeBytes: fileInfo.Exists ? fileInfo.Length : 0);

                // FrameSavedHandler uses scoped DbContext — create a scope per frame
                await using var scope   = _scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<FrameSavedHandler>();
                await handler.HandleAsync(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save or record frame");
            }
        }
    }
}
```

- [ ] **Step 3: Build**

```bash
dotnet build src/PeanutVision.Api
```

Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add src/PeanutVision.Api/Services/
git commit -m "feat: add FrameSavedHandler and FrameWriterBackgroundService"
```

---

## Task 10: Update `Program.cs` and `AcquisitionController`

**Files:**
- Modify: `src/PeanutVision.Api/Program.cs`
- Modify: `src/PeanutVision.Api/Controllers/AcquisitionController.cs`

- [ ] **Step 1: Replace DI registrations in `Program.cs`**

Replace the block:
```csharp
builder.Services.AddSingleton<AcquisitionManager>();
builder.Services.AddSingleton<IAcquisitionService>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<IExposureControl>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddScoped<IImageCaptureService, ImageCaptureService>();
```

With:
```csharp
builder.Services.AddSingleton<IFrameQueue>(_ => new PeanutVision.Capture.BoundedFrameQueue(capacity: 32));
builder.Services.AddSingleton<IFrameWriter>(_ => new PeanutVision.Capture.ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter()));

builder.Services.AddSingleton<AcquisitionSession>();
builder.Services.AddSingleton<IAcquisitionSession>(sp => sp.GetRequiredService<AcquisitionSession>());
builder.Services.AddSingleton<IExposureSource>(sp => sp.GetRequiredService<AcquisitionSession>());
builder.Services.AddSingleton<IExposureController, ExposureController>();
builder.Services.AddScoped<ISnapshotCapture, SnapshotCapture>();
builder.Services.AddScoped<FrameSavedHandler>();
builder.Services.AddHostedService<FrameWriterBackgroundService>();
```

Also remove the `FrameSaveTracker` and `FilenameGenerator` registrations:
```csharp
// Remove these two lines:
builder.Services.AddSingleton<FilenameGenerator>();
builder.Services.AddSingleton<FrameSaveTracker>();
```

- [ ] **Step 2: Rewrite `AcquisitionController`**

```csharp
// src/PeanutVision.Api/Controllers/AcquisitionController.cs
using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    private readonly IAcquisitionSession _session;
    private readonly IExposureController _exposure;
    private readonly ISnapshotCapture    _snapshot;

    public AcquisitionController(
        IAcquisitionSession session,
        IExposureController exposure,
        ISnapshotCapture snapshot)
    {
        _session  = session;
        _exposure = exposure;
        _snapshot = snapshot;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;
        _session.Start(profileId, triggerMode, request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    [HttpPost("stop")]
    public ActionResult Stop()
    {
        _session.Stop();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpDelete]
    public ActionResult ReleaseChannel()
    {
        _session.Stop();
        return Ok(new { message = "Channel released" });
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var stats = _session.GetStatistics();
        return Ok(new
        {
            isActive     = _session.IsActive,
            channelState = _session.ChannelState.ToString().ToLowerInvariant(),
            profileId    = _session.ActiveProfileId?.Value,
            hasFrame     = _session.HasFrame,
            lastError    = _session.LastError,
            allowedActions = _session.GetAllowedActions(),
            statistics   = stats.HasValue ? new
            {
                frameCount              = stats.Value.FrameCount,
                droppedFrameCount       = stats.Value.DroppedFrameCount,
                errorCount              = stats.Value.ErrorCount,
                elapsedMs               = stats.Value.ElapsedTime.TotalMilliseconds,
                averageFps              = Math.Round(stats.Value.AverageFps, 2),
                minFrameIntervalMs      = Math.Round(stats.Value.MinFrameIntervalMs, 2),
                maxFrameIntervalMs      = Math.Round(stats.Value.MaxFrameIntervalMs, 2),
                averageFrameIntervalMs  = Math.Round(stats.Value.AverageFrameIntervalMs, 2),
                copyDropCount           = stats.Value.CopyDropCount,
                clusterUnavailableCount = stats.Value.ClusterUnavailableCount,
            } : null,
            recentEvents = _session.GetRecentEvents(50).Select(e => new
            {
                timestamp = e.Timestamp,
                type      = e.Type.ToString(),
                message   = e.Message,
            }),
        });
    }

    [HttpPost("trigger")]
    public async Task<ActionResult> Trigger()
    {
        var image = await _session.TriggerAndWaitAsync(5000);
        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;
        return File(stream, "image/png", "trigger.png");
    }

    [HttpGet("latest-frame")]
    public ActionResult GetLatestFrame()
    {
        var frame = _session.GetLatestFrame();
        if (frame is null)
            return NoContent();

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(frame, stream);
        stream.Position = 0;
        return File(stream, "image/png", "latest.png");
    }

    [HttpGet("latest-frame/histogram")]
    public ActionResult GetHistogram()
    {
        var frame = _session.GetLatestFrame();
        if (frame is null) return NoContent();

        var histogram = HistogramService.Compute(frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    [HttpPost("snapshot")]
    public async Task<ActionResult> Snapshot([FromBody] SnapshotRequest request)
    {
        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;

        string filePath;
        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            // Custom output path: bypass DB recording
            var image = await CaptureRawAsync(profileId, triggerMode);
            new ImageWriter().Save(image, request.OutputPath);
            filePath = request.OutputPath;
        }
        else
        {
            filePath = await _snapshot.CaptureAsync(profileId, triggerMode);
        }

        Response.Headers["X-Image-Path"] = filePath;

        // Re-read the saved file to return PNG preview
        var savedImage = LoadImageFromFile(filePath);
        var encoder    = new PngEncoder();
        var stream     = new MemoryStream();
        encoder.Encode(savedImage, stream);
        stream.Position = 0;
        return File(stream, "image/png", "snapshot.png");
    }

    [HttpGet("exposure")]
    public ActionResult GetExposure() => Ok(_exposure.GetExposure());

    [HttpPut("exposure")]
    public ActionResult SetExposure([FromBody] SetExposureRequest request)
        => Ok(_exposure.SetExposure(request.ExposureUs));

    // --- helpers ---

    private async Task<ImageData> CaptureRawAsync(ProfileId profileId, TriggerMode? triggerMode)
    {
        // Reuse SnapshotCapture channel logic but return the ImageData directly.
        // For the custom OutputPath case, we use TriggerAndWaitAsync on a fresh session.
        // Simpler: just delegate to snapshot without DB recording.
        // This edge case keeps the existing behaviour without over-engineering.
        _session.Start(profileId, triggerMode, frameCount: 1);
        try { return await _session.TriggerAndWaitAsync(5000); }
        finally { _session.Stop(); }
    }

    private static ImageData LoadImageFromFile(string filePath)
    {
        // Load saved file back as ImageData for PNG preview
        using var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath);
        var pixels = new byte[img.Width * img.Height * 3];
        img.CopyPixelDataTo(pixels);
        return new ImageData(pixels, img.Width, img.Height, img.Width * 3);
    }
}

public class StartAcquisitionRequest
{
    public required string ProfileId  { get; set; }
    public string? TriggerMode        { get; set; }
    public int?    FrameCount         { get; set; }
    public int?    IntervalMs         { get; set; }
}

public class SnapshotRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode       { get; set; }
    public string? OutputPath        { get; set; }
}

public class SetExposureRequest
{
    public double? ExposureUs { get; set; }
}
```

- [ ] **Step 3: Build**

```bash
dotnet build src/PeanutVision.Api
```

Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add src/PeanutVision.Api/
git commit -m "feat: wire new SOLID components in Program.cs and AcquisitionController"
```

---

## Task 11: Delete old classes + full regression

**Files to delete:**
- `src/PeanutVision.Api/Services/AcquisitionManager.cs`
- `src/PeanutVision.Api/Services/IAcquisitionService.cs`
- `src/PeanutVision.Api/Services/IChannelService.cs`
- `src/PeanutVision.Api/Services/IExposureControl.cs`
- `src/PeanutVision.Api/Services/ImageCaptureService.cs`
- `src/PeanutVision.Api/Services/IImageCaptureService.cs`
- `src/PeanutVision.Api/Services/FrameSaveTracker.cs`
- `src/PeanutVision.Api/Services/FilenameGenerator.cs`

- [ ] **Step 1: Delete files**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git rm src/PeanutVision.Api/Services/AcquisitionManager.cs \
        src/PeanutVision.Api/Services/IAcquisitionService.cs \
        src/PeanutVision.Api/Services/IChannelService.cs \
        src/PeanutVision.Api/Services/IExposureControl.cs \
        src/PeanutVision.Api/Services/ImageCaptureService.cs \
        src/PeanutVision.Api/Services/IImageCaptureService.cs \
        src/PeanutVision.Api/Services/FrameSaveTracker.cs \
        src/PeanutVision.Api/Services/FilenameGenerator.cs
```

- [ ] **Step 2: Build (fix any remaining references)**

```bash
dotnet build src/PeanutVision.Api
```

If there are remaining references to deleted types, fix them before proceeding.

- [ ] **Step 3: Run all unit tests**

```bash
dotnet test src/PeanutVision.Capture.Tests -v minimal
dotnet test src/PeanutVision.MultiCamDriver.Tests -v minimal
```

Expected: All pass.

- [ ] **Step 4: Run API integration tests**

```bash
dotnet test src/PeanutVision.Api.Tests -v minimal
```

Expected: All pass. If any spec fails, investigate and fix — do not skip.

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "refactor: remove AcquisitionManager, ImageCaptureService and related legacy types

SOLID decomposition complete:
- AcquisitionManager → AcquisitionSession + ExposureController + SnapshotCapture
- ImageCaptureService → FrameWriterBackgroundService + FrameSavedHandler
- IFrameQueue + IFrameWriter abstractions extracted to PeanutVision.Capture"
```

---

## Self-Review Checklist

- [x] **Spec coverage:**
  - `IFrameWriter` / `ImageFileWriter` → Task 2 ✓
  - `IFrameQueue` / `BoundedFrameQueue` → Task 3 ✓
  - `CaptureScheduler` → Task 4 ✓
  - `IAcquisitionSession` / `AcquisitionSession` → Tasks 5–6 ✓
  - `IExposureController` / `ExposureController` → Task 7 ✓
  - `ISnapshotCapture` / `SnapshotCapture` → Task 8 ✓
  - `FrameSavedHandler` / `FrameWriterBackgroundService` → Task 9 ✓
  - DI wiring + controller → Task 10 ✓
  - Deletion + regression → Task 11 ✓
- [x] **No placeholders** — all steps have complete code
- [x] **Type consistency** — `FrameWriterOptions`, `FrameSavedEvent`, `IFrameQueue`, `IFrameWriter`, `IAcquisitionSession`, `IExposureSource`, `IExposureController`, `ISnapshotCapture` all defined before use
- [x] **`IExposureSource` is internal** — `ExposureController` resolved via `IExposureSource`, registered as `AcquisitionSession` cast in DI
