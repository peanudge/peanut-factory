# Multi-Camera Actor Architecture Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Migrate from singleton `AcquisitionSession` to per-camera `CameraActor` using `System.Threading.Channels` as a mailbox, eliminating shared mutable state and enabling N-camera support.

**Architecture:** Each camera gets a `CameraActor` that exclusively owns its state — no locks. All external access goes through a `Channel<ICameraCommand>` mailbox consumed by a single `Task _loopTask`. `CameraRegistry` (singleton, stateless) maps camera IDs to actors. Migration is 4 stages: (1) add actor types alongside existing code, (2) add `/api/cameras/{cameraId}/...` endpoints, (3) rewrite `AcquisitionController` to delegate to actor, (4) delete all legacy code.

**Tech Stack:** .NET 10, C# 12, `System.Threading.Channels`, ASP.NET Core, xUnit, `MockMultiCamHAL` for tests

---

## File Structure

**New files (Tasks 1–4):**
```
src/PeanutVision.Api/Services/Camera/
  ICameraCommand.cs          — marker interface + all command records
  ICameraActor.cs            — actor interface
  CameraActorStatus.cs       — status DTO returned by GetStatusAsync
  LatestFrameResult.cs       — (Frame, IsNew) pair for latest-frame dedup
  CameraActor.cs             — full actor implementation
  CameraRegistry.cs          — ConcurrentDictionary<string, ICameraActor>

src/PeanutVision.Api/Controllers/
  CameraController.cs        — /api/cameras/{cameraId}/... endpoints

src/PeanutVision.Api.Tests/Unit/
  CameraActorTests.cs        — actor unit tests (no WebApplicationFactory)

src/PeanutVision.Api.Tests/Specs/Cameras/
  CameraListSpec.cs
  CameraStartSpec.cs
  CameraStopSpec.cs
  CameraStatusSpec.cs
  CameraTriggerSpec.cs
  CameraLatestFrameSpec.cs
  CameraExposureSpec.cs
  CameraSnapshotSpec.cs
  CameraLifecycleSpec.cs
  CameraCaptureFlowSpec.cs
```

**Modified files:**
- `src/PeanutVision.Api/Program.cs` — add `CameraRegistry` singleton (Task 4), remove legacy registrations (Task 7)
- `src/PeanutVision.Api/Controllers/AcquisitionController.cs` — delegate to actor (Task 6), deleted (Task 7)
- `src/PeanutVision.Api/Services/IAutoSaveService.cs` — remove `TrySaveNewAsync` (Task 7)
- `src/PeanutVision.Api/Services/AutoSaveService.cs` — remove `FrameSaveTracker` dep (Task 7)

**Deleted in Task 7:**
```
src/PeanutVision.Api/Services/AcquisitionSession.cs
src/PeanutVision.Api/Services/IAcquisitionSession.cs
src/PeanutVision.Api/Services/ExposureController.cs
src/PeanutVision.Api/Services/IExposureController.cs
src/PeanutVision.Api/Services/IExposureSource.cs
src/PeanutVision.Api/Services/FrameSaveTracker.cs
src/PeanutVision.Api/Services/FrameWriterBackgroundService.cs
src/PeanutVision.Api/Controllers/AcquisitionController.cs
src/PeanutVision.Capture/IFrameQueue.cs
src/PeanutVision.Capture/BoundedFrameQueue.cs
src/PeanutVision.Api.Tests/Specs/Acquisition/*.cs  (all 8 files)
```

---

## Task 1: ICameraCommand, ICameraActor, DTOs

**Files:**
- Create: `src/PeanutVision.Api/Services/Camera/ICameraCommand.cs`
- Create: `src/PeanutVision.Api/Services/Camera/ICameraActor.cs`
- Create: `src/PeanutVision.Api/Services/Camera/CameraActorStatus.cs`
- Create: `src/PeanutVision.Api/Services/Camera/LatestFrameResult.cs`

- [ ] **Step 1: Create `ICameraCommand.cs`**

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

/// <summary>Marker interface for all commands sent to a CameraActor mailbox.</summary>
public interface ICameraCommand { }

public sealed record StartCmd(
    ProfileId ProfileId,
    TriggerMode? TriggerMode,
    int? FrameCount,
    int? IntervalMs,
    TaskCompletionSource<bool> Tcs) : ICameraCommand;

public sealed record StopCmd(
    TaskCompletionSource<bool> Tcs) : ICameraCommand;

public sealed record TriggerCmd(
    TaskCompletionSource<ImageData> Tcs,
    CancellationToken Ct) : ICameraCommand;

public sealed record CancelTriggerCmd(
    TaskCompletionSource<ImageData> Tcs) : ICameraCommand;

public sealed record GetLatestFrameCmd(
    TaskCompletionSource<LatestFrameResult> Tcs) : ICameraCommand;

public sealed record FrameArrivedCmd(ImageData Image) : ICameraCommand;

public sealed record AcquisitionErrorCmd(string Message, McSignal Signal) : ICameraCommand;

public sealed record GetStatusCmd(
    TaskCompletionSource<CameraActorStatus> Tcs) : ICameraCommand;

public sealed record GetExposureCmd(
    TaskCompletionSource<ExposureInfo> Tcs) : ICameraCommand;

public sealed record SetExposureCmd(
    double? ExposureUs,
    TaskCompletionSource<ExposureInfo> Tcs) : ICameraCommand;
```

- [ ] **Step 2: Create `CameraActorStatus.cs`**

```csharp
namespace PeanutVision.Api.Services.Camera;

public sealed record CameraActorStatus(
    bool IsActive,
    bool HasFrame,
    string? LastError,
    ChannelState ChannelState,
    string? ActiveProfileId,
    IReadOnlySet<ChannelAction> AllowedActions,
    AcquisitionStatisticsSnapshot? Statistics,
    IReadOnlyList<ChannelEvent> RecentEvents);
```

- [ ] **Step 3: Create `LatestFrameResult.cs`**

```csharp
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

/// <param name="Frame">Null if no frame has been acquired yet.</param>
/// <param name="IsNew">True if this frame has not been returned by GetLatestFrameAsync before (dedup for auto-save).</param>
public sealed record LatestFrameResult(
    ImageData? Frame,
    bool IsNew);
```

- [ ] **Step 4: Create `ICameraActor.cs`**

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

public interface ICameraActor : IAsyncDisposable
{
    string CameraId { get; }

    Task StartAsync(ProfileId profileId, TriggerMode? triggerMode = null,
                    int? frameCount = null, int? intervalMs = null,
                    CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);

    Task<ImageData> TriggerAsync(int timeoutMs = 5000, CancellationToken ct = default);

    Task<LatestFrameResult> GetLatestFrameAsync(CancellationToken ct = default);

    Task<CameraActorStatus> GetStatusAsync(CancellationToken ct = default);

    Task<ExposureInfo> GetExposureAsync(CancellationToken ct = default);

    Task<ExposureInfo> SetExposureAsync(double? exposureUs, CancellationToken ct = default);
}
```

- [ ] **Step 5: Build to confirm no errors**

```bash
dotnet build src/PeanutVision.Api/ 2>&1 | tail -5
```

Expected: Build succeeded.

- [ ] **Step 6: Commit**

```bash
git add src/PeanutVision.Api/Services/Camera/
git commit -m "feat: add CameraActor command types, interface, and DTOs"
```

---

## Task 2: CameraActor implementation

**Files:**
- Create: `src/PeanutVision.Api/Services/Camera/CameraActor.cs`

Read spec first: `docs/superpowers/specs/2026-04-04-multi-camera-actor-design.md`

- [ ] **Step 1: Write `CameraActor.cs`**

The actor owns all state in private fields. No `lock` anywhere. All state mutation is only in the `RunLoopAsync` method.

```csharp
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

public sealed class CameraActor : ICameraActor
{
    // --- identity & external services ---
    private readonly string _cameraId;
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly ILatencyService _latencyService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly string _contentRootPath;
    private readonly ILogger<CameraActor> _logger;

    // --- mailbox & loop ---
    private readonly Channel<ICameraCommand> _mailbox =
        Channel.CreateUnbounded<ICameraCommand>(new UnboundedChannelOptions { SingleReader = true });
    private readonly Task _loopTask;
    private readonly CancellationTokenSource _cts = new();

    // --- state (only accessed inside RunLoopAsync) ---
    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private ImageData? _lastSavedFrame;   // for IsNew dedup
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _pendingTriggerTcs;
    private DateTimeOffset? _triggerTimestamp;
    private ChannelState _state = ChannelState.None;
    private ProfileId? _activeProfileId;
    private readonly ChannelEventLog _eventLog = new();
    private double _desiredExposureUs = 10_000.0;

    public string CameraId => _cameraId;

    public CameraActor(
        string cameraId,
        IGrabService grabService,
        ICamFileService camFileService,
        ILatencyService latencyService,
        IServiceScopeFactory scopeFactory,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        string contentRootPath,
        ILogger<CameraActor> logger)
    {
        _cameraId        = cameraId;
        _grabService     = grabService;
        _camFileService  = camFileService;
        _latencyService  = latencyService;
        _scopeFactory    = scopeFactory;
        _frameWriter     = frameWriter;
        _saveSettings    = saveSettings;
        _contentRootPath = contentRootPath;
        _logger          = logger;
        _loopTask        = Task.Run(() => RunLoopAsync(_cts.Token));
    }

    // ---- public API — send commands to mailbox ----

    public Task StartAsync(ProfileId profileId, TriggerMode? triggerMode = null,
                           int? frameCount = null, int? intervalMs = null,
                           CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new StartCmd(profileId, triggerMode, frameCount, intervalMs, tcs));
        return tcs.Task;
    }

    public Task StopAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new StopCmd(tcs));
        return tcs.Task;
    }

    public async Task<ImageData> TriggerAsync(int timeoutMs = 5000, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<ImageData>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new TriggerCmd(tcs, ct));

        var timeout = Task.Delay(timeoutMs, ct);
        var completed = await Task.WhenAny(tcs.Task, timeout).ConfigureAwait(false);
        if (completed == timeout)
        {
            _mailbox.Writer.TryWrite(new CancelTriggerCmd(tcs));
            ct.ThrowIfCancellationRequested();
            throw new TimeoutException("Trigger timed out waiting for frame.");
        }
        return await tcs.Task.ConfigureAwait(false);
    }

    public Task<LatestFrameResult> GetLatestFrameAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<LatestFrameResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new GetLatestFrameCmd(tcs));
        return tcs.Task;
    }

    public Task<CameraActorStatus> GetStatusAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<CameraActorStatus>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new GetStatusCmd(tcs));
        return tcs.Task;
    }

    public Task<ExposureInfo> GetExposureAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<ExposureInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new GetExposureCmd(tcs));
        return tcs.Task;
    }

    public Task<ExposureInfo> SetExposureAsync(double? exposureUs, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<ExposureInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new SetExposureCmd(exposureUs, tcs));
        return tcs.Task;
    }

    // ---- actor loop ----

    private async Task RunLoopAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var cmd in _mailbox.Reader.ReadAllAsync(ct).ConfigureAwait(false))
            {
                try
                {
                    switch (cmd)
                    {
                        case StartCmd c:            HandleStart(c);            break;
                        case StopCmd c:             HandleStop(c);             break;
                        case TriggerCmd c:          HandleTrigger(c);          break;
                        case CancelTriggerCmd c:    HandleCancelTrigger(c);    break;
                        case GetLatestFrameCmd c:   HandleGetLatestFrame(c);   break;
                        case FrameArrivedCmd c:     HandleFrameArrived(c);     break;
                        case AcquisitionErrorCmd c: HandleAcquisitionError(c); break;
                        case GetStatusCmd c:        c.Tcs.TrySetResult(BuildStatus()); break;
                        case GetExposureCmd c:      c.Tcs.TrySetResult(BuildExposureInfo()); break;
                        case SetExposureCmd c:      HandleSetExposure(c);      break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in CameraActor loop for {CameraId}", _cameraId);
                    // Set exception on any pending TCS in the command
                    if (cmd is StartCmd sc) sc.Tcs.TrySetException(ex);
                    else if (cmd is StopCmd stc) stc.Tcs.TrySetException(ex);
                    else if (cmd is TriggerCmd tc) tc.Tcs.TrySetException(ex);
                    else if (cmd is GetStatusCmd gsc) gsc.Tcs.TrySetException(ex);
                    else if (cmd is GetExposureCmd gec) gec.Tcs.TrySetException(ex);
                    else if (cmd is SetExposureCmd sec) sec.Tcs.TrySetException(ex);
                    else if (cmd is GetLatestFrameCmd glfc) glfc.Tcs.TrySetException(ex);
                }
            }
        }
        catch (OperationCanceledException) { /* expected on shutdown */ }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CameraActor loop crashed for {CameraId}", _cameraId);
        }
    }

    // ---- command handlers ----

    private void HandleStart(StartCmd c)
    {
        const int minIntervalMs = 50;
        if (c.IntervalMs.HasValue && c.IntervalMs.Value > 0 && c.IntervalMs.Value < minIntervalMs)
        {
            c.Tcs.TrySetException(new Exceptions.InvalidParameterException(
                $"intervalMs must be >= {minIntervalMs}ms, got {c.IntervalMs.Value}ms."));
            return;
        }

        if (_state == ChannelState.Active)
        {
            c.Tcs.TrySetException(new Exceptions.AcquisitionConflictException(
                "Acquisition is already active. Stop it first."));
            return;
        }

        // Release any idle channel from a previous run
        if (_channel != null)
        {
            _grabService.ReleaseChannel(_channel);
            _channel = null;
            _state = ChannelState.None;
        }

        var camFile = _camFileService.GetByFileName(c.ProfileId.Value);
        var options = c.TriggerMode.HasValue
            ? camFile.ToChannelOptions(c.TriggerMode.Value.Mode)
            : camFile.ToChannelOptions();

        _channel = _grabService.CreateChannel(options);
        _activeProfileId = c.ProfileId;
        _lastFrame = null;
        _lastError = null;
        _statistics = new AcquisitionStatistics();

        _channel.FrameAcquired    += OnFrameAcquired;
        _channel.AcquisitionError += OnAcquisitionError;
        _channel.AcquisitionEnded += OnAcquisitionEnded;

        _statistics.Start();
        _channel.StartAcquisition(c.FrameCount ?? -1);
        _state = ChannelState.Active;

        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
            $"Started with profile '{c.ProfileId.Value}'" +
            (c.FrameCount.HasValue ? $", frameCount={c.FrameCount}" : "") +
            (c.IntervalMs.HasValue ? $", intervalMs={c.IntervalMs}" : "")));

        c.Tcs.TrySetResult(true);
    }

    private void HandleStop(StopCmd c)
    {
        if (_state != ChannelState.Active)
        {
            c.Tcs.TrySetResult(true);
            return;
        }
        _state = ChannelState.Idle;

        var pendingTcs = _pendingTriggerTcs;
        _pendingTriggerTcs = null;
        pendingTcs?.TrySetCanceled();

        _statistics?.Stop();
        _channel!.StopAcquisition();
        _channel.FrameAcquired    -= OnFrameAcquired;
        _channel.AcquisitionError -= OnAcquisitionError;
        _channel.AcquisitionEnded -= OnAcquisitionEnded;
        _grabService.ReleaseChannel(_channel);
        _channel = null;
        _activeProfileId = null;
        _state = ChannelState.None;

        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStopped, "Stopped"));
        c.Tcs.TrySetResult(true);
    }

    private void HandleTrigger(TriggerCmd c)
    {
        if (_channel == null || !_channel.IsActive)
        {
            c.Tcs.TrySetException(new Exceptions.ChannelNotAvailableException());
            return;
        }
        if (_pendingTriggerTcs != null)
        {
            c.Tcs.TrySetException(new Exceptions.AcquisitionConflictException(
                "A trigger is already pending. Wait for it to complete."));
            return;
        }
        if (!_channel.SupportsSoftwareTrigger)
        {
            c.Tcs.TrySetException(new Exceptions.InvalidParameterException(
                $"TriggerAsync requires SOFT or COMBINED trigger mode, but channel is {_channel.TriggerMode}."));
            return;
        }

        _pendingTriggerTcs = c.Tcs;
        _triggerTimestamp  = DateTimeOffset.UtcNow;
        _channel.SendSoftwareTrigger();
    }

    private void HandleCancelTrigger(CancelTriggerCmd c)
    {
        if (ReferenceEquals(_pendingTriggerTcs, c.Tcs))
            _pendingTriggerTcs = null;
    }

    private void HandleGetLatestFrame(GetLatestFrameCmd c)
    {
        var isNew = _lastFrame is not null && !ReferenceEquals(_lastFrame, _lastSavedFrame);
        if (isNew) _lastSavedFrame = _lastFrame;
        c.Tcs.TrySetResult(new LatestFrameResult(_lastFrame, isNew));
    }

    private void HandleFrameArrived(FrameArrivedCmd c)
    {
        var frameAt = DateTimeOffset.UtcNow;
        _lastFrame = c.Image;
        _statistics?.RecordFrame();
        var frameIndex = _statistics?.FrameCount ?? 0;

        if (_triggerTimestamp.HasValue)
        {
            _latencyService.Record(_triggerTimestamp.Value, frameAt, frameIndex, _activeProfileId?.Value);
            _triggerTimestamp = null;
        }

        if (_pendingTriggerTcs is not null)
        {
            var tcs = _pendingTriggerTcs;
            _pendingTriggerTcs = null;
            tcs.TrySetResult(c.Image);
            // triggered frames are saved by the controller — do NOT save here
            return;
        }

        // Stream frame — save asynchronously (fire-and-forget)
        _ = SaveStreamFrameAsync(c.Image);
    }

    private void HandleAcquisitionError(AcquisitionErrorCmd c)
    {
        _lastError = c.Message;
        _statistics?.RecordError();
        if (c.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE) _statistics?.RecordDroppedFrame();

        var pendingTcs = _pendingTriggerTcs;
        _pendingTriggerTcs = null;
        pendingTcs?.TrySetException(new InvalidOperationException($"Acquisition error: {c.Message}"));

        var eventType = c.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE
            ? ChannelEventType.BufferUnavailable
            : ChannelEventType.AcquisitionError;
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, eventType, c.Message));

        if (c.Signal == McSignal.MC_SIG_UNRECOVERABLE_OVERRUN)
            _ = Task.Run(() => StopAsync());
    }

    private void HandleSetExposure(SetExposureCmd c)
    {
        if (c.ExposureUs.HasValue)
            _desiredExposureUs = c.ExposureUs.Value;

        if (_channel?.IsActive == true)
        {
            _channel.SetExposureUs(_desiredExposureUs);
            var (min, max) = _channel.GetExposureRange();
            c.Tcs.TrySetResult(new ExposureInfo
            {
                ExposureUs    = _channel.GetExposureUs(),
                ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
            });
        }
        else
        {
            c.Tcs.TrySetResult(new ExposureInfo { ExposureUs = _desiredExposureUs });
        }
    }

    // ---- hardware callbacks (called from driver thread — write to mailbox only) ----

    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
        => _mailbox.Writer.TryWrite(new FrameArrivedCmd(e.Image));

    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
        => _mailbox.Writer.TryWrite(new AcquisitionErrorCmd(e.Message, e.Signal));

    private void OnAcquisitionEnded(object? sender, EventArgs e)
        => _mailbox.Writer.TryWrite(new StopCmd(new TaskCompletionSource<bool>()));

    // ---- helpers ----

    private ExposureInfo BuildExposureInfo()
    {
        if (_channel?.IsActive == true)
        {
            _desiredExposureUs = _channel.GetExposureUs();
            var (min, max) = _channel.GetExposureRange();
            return new ExposureInfo
            {
                ExposureUs    = _desiredExposureUs,
                ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
            };
        }
        return new ExposureInfo { ExposureUs = _desiredExposureUs };
    }

    private CameraActorStatus BuildStatus()
    {
        var snap = _statistics?.GetSnapshot();
        AcquisitionStatisticsSnapshot? statsWithDrv = null;
        if (snap.HasValue && _channel != null)
            statsWithDrv = snap.Value with
            {
                CopyDropCount           = _channel.CopyDropCount,
                ClusterUnavailableCount = _channel.ClusterUnavailableCount,
            };
        else if (snap.HasValue)
            statsWithDrv = snap;

        var allowedActions = _state switch
        {
            ChannelState.None   => (IReadOnlySet<ChannelAction>)new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
            ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
            ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
            _                   => new HashSet<ChannelAction>(),
        };

        return new CameraActorStatus(
            IsActive:       _state == ChannelState.Active,
            HasFrame:       _lastFrame is not null,
            LastError:      _lastError,
            ChannelState:   _state,
            ActiveProfileId: _activeProfileId?.Value,
            AllowedActions: allowedActions,
            Statistics:     statsWithDrv,
            RecentEvents:   _eventLog.GetRecent(50));
    }

    private async Task SaveStreamFrameAsync(ImageData image)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave) return;

        try
        {
            var opts     = settings.ToWriterOptions(_contentRootPath);
            var filePath = _frameWriter.Write(image, opts);
            var fileInfo = new FileInfo(filePath);

            var evt = new FrameSavedEvent(
                FilePath:      filePath,
                CapturedAt:    DateTimeOffset.UtcNow,
                Width:         image.Width,
                Height:        image.Height,
                FileSizeBytes: fileInfo.Exists ? fileInfo.Length : 0,
                Format:        settings.Format.ToString().ToLower());

            await using var scope   = _scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<FrameSavedHandler>();
            await handler.HandleAsync(evt).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save stream frame for camera {CameraId}", _cameraId);
        }
    }

    // ---- dispose ----

    public async ValueTask DisposeAsync()
    {
        // If still active, stop first
        if (_state == ChannelState.Active)
            await StopAsync().ConfigureAwait(false);

        _mailbox.Writer.TryComplete();
        _cts.Cancel();
        try { await _loopTask.ConfigureAwait(false); } catch { /* loop already exited */ }
        _cts.Dispose();
    }
}
```

- [ ] **Step 2: Build to confirm no compilation errors**

```bash
dotnet build src/PeanutVision.Api/ 2>&1 | tail -8
```

Expected: Build succeeded.

- [ ] **Step 3: Write `CameraActorTests.cs`**

These tests use `MockMultiCamHAL` and real `GrabService` — no `WebApplicationFactory`.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;
using System.Runtime.InteropServices;

namespace PeanutVision.Api.Tests.Unit;

public class CameraActorTests : IAsyncDisposable
{
    private readonly MockMultiCamHAL _hal;
    private readonly GrabService _grabService;
    private readonly CameraActor _actor;
    private static readonly ProfileId FreerunProfile = new("crevis-tc-a160k-freerun-rgb8.cam");
    private static readonly ProfileId SoftTrigProfile = new("crevis-tc-a160k-softtrig-rgb8.cam");

    public CameraActorTests()
    {
        _hal = new MockMultiCamHAL();

        // Allocate real native memory for image buffer
        var bufferSize = _hal.Configuration.DefaultImageWidth
            * _hal.Configuration.DefaultImageHeight * 3;
        var mem = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, mem, bufferSize);
        _hal.Configuration.SimulatedSurfaceAddress = mem;
        _hal.Configuration.AutoSimulateFrameOnTrigger = true;

        _grabService = new GrabService(_hal);
        _grabService.Initialize();

        // Minimal no-op scope factory (stream save not tested here)
        var services = new ServiceCollection();
        services.AddLogging();
        var sp = services.BuildServiceProvider();
        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

        // No-op save settings (AutoSave=false for unit tests)
        var saveSettings = new ImageSaveSettingsService(
            Path.Combine(Path.GetTempPath(), $"actor-test-settings-{Guid.NewGuid():N}.json"));

        _actor = new CameraActor(
            cameraId:        "cam-1",
            grabService:     _grabService,
            camFileService:  TestCamFileHelper.GetOrCreate(),
            latencyService:  new LatencyService(new LatencyRepository(
                                 Microsoft.Extensions.Options.Options.Create(
                                     new LatencyRepositoryOptions()))),
            scopeFactory:    scopeFactory,
            frameWriter:     new ImageFileWriter(new PeanutVision.MultiCamDriver.Imaging.ImageWriter()),
            saveSettings:    saveSettings,
            contentRootPath: Path.GetTempPath(),
            logger:          NullLogger<CameraActor>.Instance);
    }

    [Fact]
    public async Task Start_sets_state_to_active()
    {
        await _actor.StartAsync(FreerunProfile);

        var status = await _actor.GetStatusAsync();
        Assert.True(status.IsActive);
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", status.ActiveProfileId);

        await _actor.StopAsync();
    }

    [Fact]
    public async Task Start_twice_throws_conflict()
    {
        await _actor.StartAsync(FreerunProfile);

        await Assert.ThrowsAsync<Exceptions.AcquisitionConflictException>(
            () => _actor.StartAsync(FreerunProfile));

        await _actor.StopAsync();
    }

    [Fact]
    public async Task Stop_when_idle_does_not_throw()
    {
        // Should complete without exception
        await _actor.StopAsync();
    }

    [Fact]
    public async Task Trigger_returns_image_data()
    {
        await _actor.StartAsync(SoftTrigProfile);

        var image = await _actor.TriggerAsync(5000, default);

        Assert.NotNull(image);
        Assert.True(image.Width > 0 && image.Height > 0);

        await _actor.StopAsync();
    }

    [Fact]
    public async Task GetLatestFrame_returns_null_before_any_frame()
    {
        var result = await _actor.GetLatestFrameAsync();

        Assert.Null(result.Frame);
        Assert.False(result.IsNew);
    }

    [Fact]
    public async Task GetLatestFrame_IsNew_true_on_first_poll_false_on_second()
    {
        await _actor.StartAsync(SoftTrigProfile);
        await _actor.TriggerAsync(5000, default);

        var first  = await _actor.GetLatestFrameAsync();
        var second = await _actor.GetLatestFrameAsync();

        Assert.True(first.IsNew,   "First poll should be new");
        Assert.False(second.IsNew, "Second poll of same frame should not be new");

        await _actor.StopAsync();
    }

    [Fact]
    public async Task GetExposure_returns_default_when_not_active()
    {
        var info = await _actor.GetExposureAsync();

        Assert.True(info.ExposureUs > 0);
        Assert.Null(info.ExposureRange); // no active channel
    }

    [Fact]
    public async Task SetExposure_caches_value_when_not_active()
    {
        await _actor.SetExposureAsync(20_000.0);
        var info = await _actor.GetExposureAsync();

        Assert.Equal(20_000.0, info.ExposureUs);
    }

    [Fact]
    public async Task Status_allowed_actions_change_with_state()
    {
        var idle = await _actor.GetStatusAsync();
        Assert.Contains(ChannelAction.Start, idle.AllowedActions);

        await _actor.StartAsync(FreerunProfile);
        var active = await _actor.GetStatusAsync();
        Assert.Contains(ChannelAction.Stop, active.AllowedActions);
        Assert.DoesNotContain(ChannelAction.Start, active.AllowedActions);

        await _actor.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _actor.DisposeAsync();
        _grabService.Dispose();
    }
}
```

- [ ] **Step 4: Run unit tests (will fail — CameraActor not yet known to test project)**

```bash
dotnet test src/PeanutVision.Api.Tests/ --filter "CameraActorTests" 2>&1 | tail -10
```

Expected: Build succeeds, tests pass.

If `LatencyService`, `LatencyRepository`, `ImageSaveSettingsService`, or `ImageFileWriter` don't resolve properly in the unit test constructor, adjust by using concrete constructors that are visible from the test project. If a type is `internal`, use `InternalsVisibleTo` or restructure.

- [ ] **Step 5: Fix any compilation errors and get tests green**

```bash
dotnet test src/PeanutVision.Api.Tests/ --filter "CameraActorTests" 2>&1 | tail -5
```

Expected: Passed! - Failed: 0, Passed: 8 (or however many unit tests).

- [ ] **Step 6: Run full test suite to confirm no regressions**

```bash
dotnet test src/PeanutVision.Api.Tests/ 2>&1 | tail -3
```

Expected: Passed! - Failed: 0, Passed: 88.

- [ ] **Step 7: Commit**

```bash
git add src/PeanutVision.Api/Services/Camera/CameraActor.cs \
        src/PeanutVision.Api.Tests/Unit/CameraActorTests.cs
git commit -m "feat: implement CameraActor with System.Threading.Channels mailbox"
```

---

## Task 3: CameraRegistry

**Files:**
- Create: `src/PeanutVision.Api/Services/Camera/CameraRegistry.cs`

- [ ] **Step 1: Write `CameraRegistry.cs`**

```csharp
using System.Collections.Concurrent;

namespace PeanutVision.Api.Services.Camera;

/// <summary>
/// Singleton registry mapping camera IDs to CameraActors.
/// Thread-safe via ConcurrentDictionary; contains no mutable state of its own.
/// </summary>
public sealed class CameraRegistry
{
    private readonly ConcurrentDictionary<string, ICameraActor> _actors = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Registers an actor. Throws if a camera with the same ID already exists.</summary>
    public void Register(ICameraActor actor)
    {
        if (!_actors.TryAdd(actor.CameraId, actor))
            throw new InvalidOperationException($"Camera '{actor.CameraId}' is already registered.");
    }

    /// <summary>Returns the actor for <paramref name="cameraId"/>, or null if not found.</summary>
    public ICameraActor? TryGet(string cameraId)
        => _actors.TryGetValue(cameraId, out var actor) ? actor : null;

    /// <summary>Returns all registered camera IDs.</summary>
    public IReadOnlyList<string> GetAllIds()
        => _actors.Keys.ToList();
}
```

- [ ] **Step 2: Build**

```bash
dotnet build src/PeanutVision.Api/ 2>&1 | tail -3
```

Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/PeanutVision.Api/Services/Camera/CameraRegistry.cs
git commit -m "feat: add CameraRegistry singleton"
```

---

## Task 4: CameraController + Program.cs wiring

**Files:**
- Create: `src/PeanutVision.Api/Controllers/CameraController.cs`
- Modify: `src/PeanutVision.Api/Program.cs`

- [ ] **Step 1: Write `CameraController.cs`**

The controller looks up the actor from `CameraRegistry`. Returns 404 if camera ID is not found. For `snapshot`, delegates to `ISnapshotCapture` (unchanged). For `trigger` and `latest-frame`, uses `IAutoSaveService`.

```csharp
using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/cameras")]
public class CameraController : ControllerBase
{
    private readonly CameraRegistry  _registry;
    private readonly ISnapshotCapture _snapshot;
    private readonly IAutoSaveService _autoSave;

    public CameraController(
        CameraRegistry registry,
        ISnapshotCapture snapshot,
        IAutoSaveService autoSave)
    {
        _registry = registry;
        _snapshot = snapshot;
        _autoSave = autoSave;
    }

    // GET /api/cameras
    [HttpGet]
    public ActionResult ListCameras()
        => Ok(new { cameras = _registry.GetAllIds().Select(id => new { id }) });

    // POST /api/cameras/{cameraId}/start
    [HttpPost("{cameraId}/start")]
    public async Task<ActionResult> Start(string cameraId, [FromBody] StartAcquisitionRequest request)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;
        await actor.StartAsync(profileId, triggerMode, request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    // POST /api/cameras/{cameraId}/stop
    [HttpPost("{cameraId}/stop")]
    public async Task<ActionResult> Stop(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        await actor.StopAsync();
        return Ok(new { message = "Acquisition stopped" });
    }

    // GET /api/cameras/{cameraId}/status
    [HttpGet("{cameraId}/status")]
    public async Task<ActionResult> GetStatus(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var s = await actor.GetStatusAsync();
        var stats = s.Statistics;
        return Ok(new
        {
            isActive     = s.IsActive,
            channelState = s.ChannelState.ToString().ToLowerInvariant(),
            profileId    = s.ActiveProfileId,
            hasFrame     = s.HasFrame,
            lastError    = s.LastError,
            allowedActions = s.AllowedActions,
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
            recentEvents = s.RecentEvents.Select(e => new
            {
                timestamp = e.Timestamp,
                type      = e.Type.ToString(),
                message   = e.Message,
            }),
        });
    }

    // POST /api/cameras/{cameraId}/trigger
    [HttpPost("{cameraId}/trigger")]
    public async Task<ActionResult> Trigger(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var image = await actor.TriggerAsync(5000, HttpContext.RequestAborted);
        var path  = await _autoSave.TrySaveAsync(image);
        if (path is not null)
            Response.Headers["X-Image-Path"] = path;

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;
        return File(stream, "image/png", "trigger.png");
    }

    // GET /api/cameras/{cameraId}/latest-frame
    [HttpGet("{cameraId}/latest-frame")]
    public async Task<ActionResult> GetLatestFrame(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var result = await actor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        if (result.IsNew)
        {
            var path = await _autoSave.TrySaveAsync(result.Frame);
            if (path is not null)
                Response.Headers["X-Image-Path"] = path;
        }

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(result.Frame, stream);
        stream.Position = 0;
        return File(stream, "image/png", "latest.png");
    }

    // GET /api/cameras/{cameraId}/latest-frame/histogram
    [HttpGet("{cameraId}/latest-frame/histogram")]
    public async Task<ActionResult> GetHistogram(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var result = await actor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        var histogram = HistogramService.Compute(result.Frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    // POST /api/cameras/{cameraId}/snapshot
    [HttpPost("{cameraId}/snapshot")]
    public async Task<ActionResult> Snapshot(string cameraId, [FromBody] SnapshotRequest request)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });

        var status = await actor.GetStatusAsync();
        if (status.IsActive)
            throw new AcquisitionConflictException("Cannot snapshot while acquisition is active.");

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;

        string filePath;
        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            // Custom path: bypass DB recording, use actor directly
            await actor.StartAsync(profileId, triggerMode, frameCount: 1);
            ImageData rawImage;
            try { rawImage = await actor.TriggerAsync(5000, HttpContext.RequestAborted); }
            finally { await actor.StopAsync(); }
            new PeanutVision.MultiCamDriver.Imaging.ImageWriter().Save(rawImage, request.OutputPath);
            filePath = request.OutputPath;
        }
        else
        {
            filePath = await _snapshot.CaptureAsync(profileId, triggerMode);
        }

        Response.Headers["X-Image-Path"] = filePath;

        var savedImage = LoadImageFromFile(filePath);
        var encoder    = new PngEncoder();
        var stream     = new MemoryStream();
        encoder.Encode(savedImage, stream);
        stream.Position = 0;
        return File(stream, "image/png", "snapshot.png");
    }

    // GET /api/cameras/{cameraId}/exposure
    [HttpGet("{cameraId}/exposure")]
    public async Task<ActionResult> GetExposure(string cameraId)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });
        return Ok(await actor.GetExposureAsync());
    }

    // PUT /api/cameras/{cameraId}/exposure
    [HttpPut("{cameraId}/exposure")]
    public async Task<ActionResult> SetExposure(string cameraId, [FromBody] SetExposureRequest request)
    {
        var actor = GetActorOrNotFound(cameraId);
        if (actor is null) return NotFound(new { error = $"Camera '{cameraId}' not found." });
        return Ok(await actor.SetExposureAsync(request.ExposureUs));
    }

    private ICameraActor? GetActorOrNotFound(string cameraId)
        => _registry.TryGet(cameraId);

    private static PeanutVision.MultiCamDriver.Imaging.ImageData LoadImageFromFile(string filePath)
    {
        using var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath);
        var pixels = new byte[img.Width * img.Height * 3];
        img.CopyPixelDataTo(pixels);
        return new PeanutVision.MultiCamDriver.Imaging.ImageData(pixels, img.Width, img.Height, img.Width * 3);
    }
}
```

- [ ] **Step 2: Add `CameraRegistry` singleton to `Program.cs`**

Add after the existing `AcquisitionSession` registrations (do NOT remove them yet — that's Task 7):

```csharp
// Multi-camera actor system (alongside legacy AcquisitionSession for now)
builder.Services.AddSingleton<CameraRegistry>(sp =>
{
    var registry = new CameraRegistry();
    registry.Register(new CameraActor(
        cameraId:        "cam-1",
        grabService:     sp.GetRequiredService<IGrabService>(),
        camFileService:  sp.GetRequiredService<ICamFileService>(),
        latencyService:  sp.GetRequiredService<ILatencyService>(),
        scopeFactory:    sp.GetRequiredService<IServiceScopeFactory>(),
        frameWriter:     sp.GetRequiredService<IFrameWriter>(),
        saveSettings:    sp.GetRequiredService<IImageSaveSettingsService>(),
        contentRootPath: builder.Environment.ContentRootPath,
        logger:          sp.GetRequiredService<ILogger<CameraActor>>()));
    return registry;
});
```

Add `using PeanutVision.Api.Services.Camera;` at the top of `Program.cs`.

- [ ] **Step 3: Build**

```bash
dotnet build src/PeanutVision.Api/ 2>&1 | tail -5
```

Expected: Build succeeded.

- [ ] **Step 4: Run all existing tests — must still pass**

```bash
dotnet test src/PeanutVision.Api.Tests/ 2>&1 | tail -3
```

Expected: Passed! - Failed: 0, Passed: 88.

- [ ] **Step 5: Commit**

```bash
git add src/PeanutVision.Api/Controllers/CameraController.cs \
        src/PeanutVision.Api/Program.cs
git commit -m "feat: add CameraController and register CameraRegistry in DI"
```

---

## Task 5: Integration tests for /api/cameras/{cameraId}

**Files:** Create all files under `src/PeanutVision.Api.Tests/Specs/Cameras/`

The camera ID for tests is `"cam-1"` (registered in `Program.cs`). All tests use the existing `PeanutVisionApiFactory` — no changes needed to the factory.

- [ ] **Step 1: Write `CameraListSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraListSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;
    public CameraListSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task List_returns_ok_with_cameras_array()
    {
        var response = await _client.GetAsync("/api/cameras");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var cameras = doc.RootElement.GetProperty("cameras").EnumerateArray().ToList();
        Assert.NotEmpty(cameras);
        Assert.Equal("cam-1", cameras[0].GetProperty("id").GetString());
    }
}
```

- [ ] **Step 2: Write `CameraStartSpec.cs`**

Mirror `AcquisitionStartSpec` but using `/api/cameras/cam-1/start`:

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraStartSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraStartSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
    }

    [Fact]
    public async Task Start_with_valid_profile_returns_ok()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Start_with_unknown_profile_returns_404()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "nonexistent-camera" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Start_when_already_active_returns_409()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Start_with_unknown_camera_id_returns_404()
    {
        var response = await _client.PostJsonAsync("/api/cameras/nonexistent-cam/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Start_sets_hal_acquisition_started()
    {
        _factory.ResetMockState();
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        Assert.True(_factory.MockHal.CallLog.AcquisitionStarted);
    }

    [Fact]
    public async Task Start_with_intervalMs_below_minimum_returns_bad_request()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam", intervalMs = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
```

- [ ] **Step 3: Write `CameraStopSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraStopSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraStopSpec(PeanutVisionApiFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Stop_when_idle_returns_ok()
    {
        var response = await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("message", out _));
    }

    [Fact]
    public async Task Stop_when_active_stops_acquisition()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });

        var response = await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var status = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await status.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Stop_sets_hal_acquisition_stopped()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        _factory.ResetMockState();

        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

        Assert.True(_factory.MockHal.CallLog.AcquisitionStopped);
    }
}
```

- [ ] **Step 4: Write `CameraStatusSpec.cs`**

```csharp
using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraStatusSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraStatusSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync()
    {
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        await _client.DeleteAsync("/api/acquisition");
    }

    [Fact]
    public async Task Status_when_idle_shows_inactive()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
        Assert.Equal(JsonValueKind.Null, doc.RootElement.GetProperty("profileId").ValueKind);
    }

    [Fact]
    public async Task Status_when_active_shows_active_with_profile()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.GetProperty("isActive").GetBoolean());
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Status_includes_hasFrame_field()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("hasFrame", out _));
    }

    [Fact]
    public async Task Status_includes_lastError_field()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("lastError", out _));
    }

    [Fact]
    public async Task Status_when_idle_allowed_actions_contains_start_and_snapshot()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        var actions = doc.RootElement.GetProperty("allowedActions")
            .EnumerateArray().Select(e => e.GetString()).ToHashSet();
        Assert.Contains("start", actions);
        Assert.Contains("snapshot", actions);
    }

    [Fact]
    public async Task Status_when_active_allowed_actions_contains_stop_and_trigger()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        var actions = doc.RootElement.GetProperty("allowedActions")
            .EnumerateArray().Select(e => e.GetString()).ToHashSet();
        Assert.Contains("stop", actions);
        Assert.Contains("trigger", actions);
        Assert.DoesNotContain("start", actions);
        Assert.DoesNotContain("snapshot", actions);
    }

    [Fact]
    public async Task Status_includes_recentEvents_array()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("recentEvents", out var events));
        Assert.Equal(JsonValueKind.Array, events.ValueKind);
    }

    [Fact]
    public async Task Status_after_start_stop_has_at_least_two_events()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.GetProperty("recentEvents").GetArrayLength() >= 2);
    }

    [Fact]
    public async Task Status_when_active_includes_statistics_shape()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var response = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await response.ReadJsonDocumentAsync();
        var stats = doc.RootElement.GetProperty("statistics");
        Assert.NotEqual(JsonValueKind.Null, stats.ValueKind);
        Assert.True(stats.TryGetProperty("frameCount", out _));
        Assert.True(stats.TryGetProperty("copyDropCount", out _));
        Assert.True(stats.TryGetProperty("clusterUnavailableCount", out _));
    }
}
```

- [ ] **Step 5: Write `CameraTriggerSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraTriggerSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraTriggerSpec(PeanutVisionApiFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Trigger_when_inactive_returns_409()
    {
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Trigger_when_active_returns_png_image()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Trigger_response_contains_valid_png_bytes()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, bytes[..8]);
    }

    [Fact]
    public async Task Trigger_increments_hal_trigger_count()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        _factory.ResetMockState();
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(2, _factory.MockHal.CallLog.SoftwareTriggerCount);
    }

    [Fact]
    public async Task Trigger_when_active_saves_image_to_output_directory()
    {
        var outputDir = Path.Combine(Path.GetTempPath(), $"peanut_test_{Guid.NewGuid():N}");
        try
        {
            using var customFactory = _factory.WithWebHostBuilder(b => b.UseSetting("ImageOutputDirectory", outputDir));
            using var client = customFactory.CreateClient();
            await client.PostJsonAsync($"/api/cameras/{CamId}/start",
                new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
            var response = await client.PostAsync($"/api/cameras/{CamId}/trigger", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Image-Path"));
            var savedPath = response.Headers.GetValues("X-Image-Path").First();
            Assert.True(File.Exists(savedPath));
            await client.PostAsync($"/api/cameras/{CamId}/stop", null);
        }
        finally
        {
            if (Directory.Exists(outputDir)) Directory.Delete(outputDir, recursive: true);
        }
    }
}
```

- [ ] **Step 6: Write `CameraLatestFrameSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraLatestFrameSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraLatestFrameSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task LatestFrame_when_no_frame_returns_no_content()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task LatestFrame_after_trigger_returns_png()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var response = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public async Task LatestFrame_with_autosave_sets_image_path_header_on_first_poll()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var first  = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");
        var second = await _client.GetAsync($"/api/cameras/{CamId}/latest-frame");

        Assert.True(first.Headers.Contains("X-Image-Path"),
            "First poll of a new frame should set X-Image-Path");
        Assert.False(second.Headers.Contains("X-Image-Path"),
            "Second poll of the same frame must not set X-Image-Path");
    }
}
```

- [ ] **Step 7: Write `CameraExposureSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraExposureSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraExposureSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task GetExposure_returns_ok_with_exposureUs()
    {
        var response = await _client.GetAsync($"/api/cameras/{CamId}/exposure");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("exposureUs", out _));
    }

    [Fact]
    public async Task SetExposure_updates_value()
    {
        await _client.PutJsonAsync($"/api/cameras/{CamId}/exposure", new { exposureUs = 15000.0 });
        var response = await _client.GetAsync($"/api/cameras/{CamId}/exposure");
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(15000.0, doc.RootElement.GetProperty("exposureUs").GetDouble(), precision: 1);
    }
}
```

If `PutJsonAsync` is not in `HttpClientExtensions.cs`, add it — it follows the same pattern as `PostJsonAsync`.

- [ ] **Step 8: Write `CameraSnapshotSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraSnapshotSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraSnapshotSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Snapshot_when_idle_returns_png_image()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Snapshot_response_contains_valid_png_bytes()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 8);
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'N', bytes[2]);
        Assert.Equal((byte)'G', bytes[3]);
    }

    [Fact]
    public async Task Snapshot_response_has_content_disposition()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var disposition = response.Content.Headers.ContentDisposition;
        Assert.NotNull(disposition);
        Assert.Equal("snapshot.png", disposition.FileName);
    }

    [Fact]
    public async Task Snapshot_when_active_returns_conflict()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_with_unknown_profile_returns_not_found()
    {
        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "nonexistent-profile" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Snapshot_can_be_called_sequentially()
    {
        var r1 = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var r2 = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);
    }

    [Fact]
    public async Task Snapshot_does_not_affect_acquisition_status()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        var statusResponse = await _client.GetAsync($"/api/cameras/{CamId}/status");
        using var doc = await statusResponse.ReadJsonDocumentAsync();
        Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }
}
```

- [ ] **Step 9: Write `CameraLifecycleSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraLifecycleSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraLifecycleSpec(PeanutVisionApiFactory factory) => _client = factory.CreateClient();
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task Full_lifecycle_start_status_stop_status()
    {
        var startResponse = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        using (var doc = await (await _client.GetAsync($"/api/cameras/{CamId}/status")).ReadJsonDocumentAsync())
            Assert.True(doc.RootElement.GetProperty("isActive").GetBoolean());

        var stopResponse = await _client.PostAsync($"/api/cameras/{CamId}/stop", null);
        Assert.Equal(HttpStatusCode.OK, stopResponse.StatusCode);

        using (var doc = await (await _client.GetAsync($"/api/cameras/{CamId}/status")).ReadJsonDocumentAsync())
            Assert.False(doc.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Can_restart_after_stop()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

        var response = await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal("crevis-tc-a160k-softtrig-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
    }

    [Fact]
    public async Task Trigger_after_stop_returns_409()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

        var response = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
```

- [ ] **Step 10: Write `CameraCaptureFlowSpec.cs`**

```csharp
using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Cameras;

public class CameraCaptureFlowSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;
    private const string CamId = "cam-1";

    public CameraCaptureFlowSpec(PeanutVisionApiFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _client.PostAsync($"/api/cameras/{CamId}/stop", null);

    [Fact]
    public async Task snapshot_creates_image_record_in_catalog()
    {
        var before = await GetTotalImageCount();

        var snapResponse = await _client.PostJsonAsync($"/api/cameras/{CamId}/snapshot",
            new { profileId = "crevis-tc-a160k-freerun-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, snapResponse.StatusCode);

        var after = await GetTotalImageCount();
        Assert.True(after > before, $"Expected image count to increase from {before} but got {after}");
    }

    [Fact]
    public async Task trigger_and_wait_saves_image_when_autosave_enabled()
    {
        var countBefore = await GetTotalImageCount();

        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        var triggerResponse = await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);
        Assert.Equal(HttpStatusCode.OK, triggerResponse.StatusCode);

        var countAfter = await GetTotalImageCount();
        Assert.True(countAfter > countBefore, "Trigger with auto-save should create a new image record");

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var listDoc = await listResponse.ReadJsonDocumentAsync();
        var latest = listDoc.RootElement.GetProperty("items").EnumerateArray().First();
        Assert.False(string.IsNullOrEmpty(latest.GetProperty("filePath").GetString()));
    }

    [Fact]
    public async Task captured_image_has_correct_metadata()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        Assert.Equal(HttpStatusCode.OK, (await _client.PostAsync($"/api/cameras/{CamId}/trigger", null)).StatusCode);

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var item = doc.RootElement.GetProperty("items").EnumerateArray().First();

        Assert.True(Guid.TryParse(item.GetProperty("id").GetString(), out var id) && id != Guid.Empty);
        _ = item.GetProperty("capturedAt").GetDateTime();
        Assert.True(item.GetProperty("width").GetInt32() > 0);
        Assert.True(item.GetProperty("height").GetInt32() > 0);
        Assert.True(item.GetProperty("fileSizeBytes").GetInt64() > 0);
        Assert.False(string.IsNullOrEmpty(item.GetProperty("format").GetString()));
    }

    [Fact]
    public async Task captured_image_file_is_accessible()
    {
        await _client.PostJsonAsync($"/api/cameras/{CamId}/start",
            new { profileId = "crevis-tc-a160k-softtrig-rgb8.cam" });
        await _client.PostAsync($"/api/cameras/{CamId}/trigger", null);

        var listResponse = await _client.GetAsync("/api/images?pageSize=1");
        using var doc = await listResponse.ReadJsonDocumentAsync();
        var idStr = doc.RootElement.GetProperty("items").EnumerateArray().First().GetProperty("id").GetString()!;
        var fileResponse = await _client.GetAsync($"/api/images/{idStr}/file");
        Assert.Equal(HttpStatusCode.OK, fileResponse.StatusCode);
        var bytes = await fileResponse.Content.ReadAsByteArrayAsync();
        Assert.True(bytes.Length > 0);
    }

    private async Task<int> GetTotalImageCount()
    {
        var response = await _client.GetAsync("/api/images?pageSize=1");
        response.EnsureSuccessStatusCode();
        using var doc = await response.ReadJsonDocumentAsync();
        return doc.RootElement.GetProperty("totalCount").GetInt32();
    }
}
```

- [ ] **Step 11: Run all new camera tests**

```bash
dotnet test src/PeanutVision.Api.Tests/ --filter "Cameras" 2>&1 | tail -5
```

Expected: All camera tests pass.

- [ ] **Step 12: Run full test suite — old tests must still pass**

```bash
dotnet test src/PeanutVision.Api.Tests/ 2>&1 | tail -3
```

Expected: Passed! - Failed: 0, Passed: ~130+ (88 old + new camera tests).

- [ ] **Step 13: Commit**

```bash
git add src/PeanutVision.Api.Tests/Specs/Cameras/
git commit -m "test: add integration tests for /api/cameras/{cameraId} endpoints"
```

---

## Task 6: Rewrite AcquisitionController to delegate to CameraActor (Stage 3)

**Files:**
- Modify: `src/PeanutVision.Api/Controllers/AcquisitionController.cs`

The controller no longer injects `IAcquisitionSession` or `IExposureController`. It delegates to `registry.TryGet("cam-1")`. All existing tests must pass (URLs unchanged).

- [ ] **Step 1: Rewrite `AcquisitionController.cs`**

Replace the entire file with this delegation-based version:

```csharp
using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging.Encoders;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    // The legacy /api/acquisition/... endpoints delegate to the "cam-1" actor.
    // This controller is intentionally temporary — it will be deleted in Stage 4.
    private const string DefaultCameraId = "cam-1";

    private readonly CameraRegistry  _registry;
    private readonly ISnapshotCapture _snapshot;
    private readonly IAutoSaveService _autoSave;

    public AcquisitionController(
        CameraRegistry registry,
        ISnapshotCapture snapshot,
        IAutoSaveService autoSave)
    {
        _registry = registry;
        _snapshot = snapshot;
        _autoSave = autoSave;
    }

    private ICameraActor DefaultActor =>
        _registry.TryGet(DefaultCameraId)
        ?? throw new InvalidOperationException($"Default camera '{DefaultCameraId}' is not registered.");

    [HttpPost("start")]
    public async Task<ActionResult> Start([FromBody] StartAcquisitionRequest request)
    {
        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;
        await DefaultActor.StartAsync(profileId, triggerMode, request.FrameCount, request.IntervalMs);
        return Ok(new { message = "Acquisition started", profileId = profileId.Value });
    }

    [HttpPost("stop")]
    public async Task<ActionResult> Stop()
    {
        await DefaultActor.StopAsync();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpDelete]
    public async Task<ActionResult> ReleaseChannel()
    {
        await DefaultActor.StopAsync();
        return Ok(new { message = "Channel released" });
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetStatus()
    {
        var s     = await DefaultActor.GetStatusAsync();
        var stats = s.Statistics;
        return Ok(new
        {
            isActive       = s.IsActive,
            channelState   = s.ChannelState.ToString().ToLowerInvariant(),
            profileId      = s.ActiveProfileId,
            hasFrame       = s.HasFrame,
            lastError      = s.LastError,
            allowedActions = s.AllowedActions,
            statistics     = stats.HasValue ? new
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
            recentEvents = s.RecentEvents.Select(e => new
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
        var image = await DefaultActor.TriggerAsync(5000, HttpContext.RequestAborted);
        var path  = await _autoSave.TrySaveAsync(image);
        if (path is not null)
            Response.Headers["X-Image-Path"] = path;

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;
        return File(stream, "image/png", "trigger.png");
    }

    [HttpGet("latest-frame")]
    public async Task<ActionResult> GetLatestFrame()
    {
        var result = await DefaultActor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        if (result.IsNew)
        {
            var path = await _autoSave.TrySaveAsync(result.Frame);
            if (path is not null)
                Response.Headers["X-Image-Path"] = path;
        }

        var encoder = new PngEncoder();
        var stream  = new MemoryStream();
        encoder.Encode(result.Frame, stream);
        stream.Position = 0;
        return File(stream, "image/png", "latest.png");
    }

    [HttpGet("latest-frame/histogram")]
    public async Task<ActionResult> GetHistogram()
    {
        var result = await DefaultActor.GetLatestFrameAsync();
        if (result.Frame is null) return NoContent();

        var histogram = HistogramService.Compute(result.Frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    [HttpPost("snapshot")]
    public async Task<ActionResult> Snapshot([FromBody] SnapshotRequest request)
    {
        var status = await DefaultActor.GetStatusAsync();
        if (status.IsActive)
            throw new AcquisitionConflictException("Cannot snapshot while acquisition is active.");

        var profileId   = new ProfileId(request.ProfileId);
        var triggerMode = request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : (TriggerMode?)null;

        string filePath;
        if (!string.IsNullOrWhiteSpace(request.OutputPath))
        {
            await DefaultActor.StartAsync(profileId, triggerMode, frameCount: 1);
            PeanutVision.MultiCamDriver.Imaging.ImageData rawImage;
            try { rawImage = await DefaultActor.TriggerAsync(5000, HttpContext.RequestAborted); }
            finally { await DefaultActor.StopAsync(); }
            new PeanutVision.MultiCamDriver.Imaging.ImageWriter().Save(rawImage, request.OutputPath);
            filePath = request.OutputPath;
        }
        else
        {
            filePath = await _snapshot.CaptureAsync(profileId, triggerMode);
        }

        Response.Headers["X-Image-Path"] = filePath;

        var savedImage = LoadImageFromFile(filePath);
        var encoder    = new PngEncoder();
        var stream     = new MemoryStream();
        encoder.Encode(savedImage, stream);
        stream.Position = 0;
        return File(stream, "image/png", "snapshot.png");
    }

    [HttpGet("exposure")]
    public async Task<ActionResult> GetExposure()
        => Ok(await DefaultActor.GetExposureAsync());

    [HttpPut("exposure")]
    public async Task<ActionResult> SetExposure([FromBody] SetExposureRequest request)
        => Ok(await DefaultActor.SetExposureAsync(request.ExposureUs));

    private static PeanutVision.MultiCamDriver.Imaging.ImageData LoadImageFromFile(string filePath)
    {
        using var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath);
        var pixels = new byte[img.Width * img.Height * 3];
        img.CopyPixelDataTo(pixels);
        return new PeanutVision.MultiCamDriver.Imaging.ImageData(pixels, img.Width, img.Height, img.Width * 3);
    }
}

// Request DTOs remain here (same as before)
public class StartAcquisitionRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode       { get; set; }
    public int?    FrameCount        { get; set; }
    public int?    IntervalMs        { get; set; }
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

- [ ] **Step 2: Build**

```bash
dotnet build src/PeanutVision.Api/ 2>&1 | tail -5
```

Expected: Build succeeded.

- [ ] **Step 3: Run full regression — all 88 old tests + all new camera tests must pass**

```bash
dotnet test src/PeanutVision.Api.Tests/ 2>&1 | tail -3
```

Expected: Passed! - Failed: 0.

If any old test fails, it means the delegation mapping is wrong. Compare the old `AcquisitionController` behavior with the new delegation.

- [ ] **Step 4: Commit**

```bash
git add src/PeanutVision.Api/Controllers/AcquisitionController.cs
git commit -m "refactor: AcquisitionController now delegates to CameraActor (Stage 3)"
```

---

## Task 7: Delete legacy code and old tests (Stage 4)

After this task, only the new `CameraController` and `CameraActor`-based system remains.

- [ ] **Step 1: Update `IAutoSaveService.cs` — remove `TrySaveNewAsync`**

```csharp
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IAutoSaveService
{
    Task<string?> TrySaveAsync(ImageData image);
    // TrySaveNewAsync removed — dedup is now handled by CameraActor._lastSavedFrame
}
```

- [ ] **Step 2: Update `AutoSaveService.cs` — remove `FrameSaveTracker` dependency**

Replace the class with a simplified version:

```csharp
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AutoSaveService : IAutoSaveService
{
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly IFrameWriter              _frameWriter;
    private readonly FrameSavedHandler         _savedHandler;
    private readonly string                    _contentRootPath;

    public AutoSaveService(
        IImageSaveSettingsService saveSettings,
        IFrameWriter frameWriter,
        FrameSavedHandler savedHandler,
        IWebHostEnvironment environment)
    {
        _saveSettings    = saveSettings;
        _frameWriter     = frameWriter;
        _savedHandler    = savedHandler;
        _contentRootPath = environment.ContentRootPath;
    }

    public async Task<string?> TrySaveAsync(ImageData image)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave) return null;

        var opts     = settings.ToWriterOptions(_contentRootPath);
        var filePath = _frameWriter.Write(image, opts);
        var fileInfo = new FileInfo(filePath);

        var evt = new FrameSavedEvent(
            FilePath:      filePath,
            CapturedAt:    DateTimeOffset.UtcNow,
            Width:         image.Width,
            Height:        image.Height,
            FileSizeBytes: fileInfo.Exists ? fileInfo.Length : 0,
            Format:        settings.Format.ToString().ToLower());

        await _savedHandler.HandleAsync(evt);
        return filePath;
    }
}
```

- [ ] **Step 3: Update `Program.cs` — remove all legacy DI registrations**

Remove these lines from `Program.cs`:

```csharp
// REMOVE:
builder.Services.AddSingleton<IFrameQueue>(_ => new PeanutVision.Capture.BoundedFrameQueue(capacity: 32));
builder.Services.AddSingleton<AcquisitionSession>();
builder.Services.AddSingleton<IAcquisitionSession>(sp => sp.GetRequiredService<AcquisitionSession>());
builder.Services.AddSingleton<IExposureSource>(sp => sp.GetRequiredService<AcquisitionSession>());
builder.Services.AddSingleton<IExposureController, ExposureController>();
builder.Services.AddSingleton<FrameSaveTracker>();
builder.Services.AddHostedService<FrameWriterBackgroundService>();
```

Also remove `AutoSaveService`'s `FrameSaveTracker` constructor arg in the DI registration (the new simplified `AutoSaveService` no longer takes it).

The remaining DI registrations for `IAutoSaveService` stay:
```csharp
builder.Services.AddScoped<IAutoSaveService, AutoSaveService>();
```

Remove unused `using` statements as needed.

- [ ] **Step 4: Delete legacy service files**

```bash
git rm src/PeanutVision.Api/Services/AcquisitionSession.cs \
       src/PeanutVision.Api/Services/IAcquisitionSession.cs \
       src/PeanutVision.Api/Services/ExposureController.cs \
       src/PeanutVision.Api/Services/IExposureController.cs \
       src/PeanutVision.Api/Services/IExposureSource.cs \
       src/PeanutVision.Api/Services/FrameSaveTracker.cs \
       src/PeanutVision.Api/Services/FrameWriterBackgroundService.cs \
       src/PeanutVision.Api/Controllers/AcquisitionController.cs \
       src/PeanutVision.Capture/IFrameQueue.cs \
       src/PeanutVision.Capture/BoundedFrameQueue.cs
```

- [ ] **Step 5: Delete old acquisition test specs**

```bash
git rm src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionStartSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionStopSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionTriggerSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionStatusSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionSnapshotSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionLatestFrameSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionLifecycleSpec.cs \
       src/PeanutVision.Api.Tests/Specs/Acquisition/AcquisitionCaptureFlowSpec.cs
```

- [ ] **Step 6: Build to confirm no compilation errors**

```bash
dotnet build src/ 2>&1 | grep -E "error|warning" | grep -v "MSB3492" | head -20
```

Fix any dangling references to deleted types (`BoundedFrameQueue`, `AcquisitionSession`, etc.).

If `PeanutVision.Capture.Tests` references `BoundedFrameQueue`, update those tests too:
- `BoundedFrameQueueTests.cs` tests `IFrameQueue`/`BoundedFrameQueue` — these can be deleted since those types are removed.
- Or keep the tests and remove only the specific `BoundedFrameQueue` test file.

Check `src/PeanutVision.Capture.Tests/BoundedFrameQueueTests.cs` — if it tests deleted types, delete it:
```bash
git rm src/PeanutVision.Capture.Tests/BoundedFrameQueueTests.cs
```

- [ ] **Step 7: Run full test suite — only camera tests should remain**

```bash
dotnet test src/PeanutVision.Api.Tests/ src/PeanutVision.Capture.Tests/ 2>&1 | tail -5
```

Expected: Passed! - Failed: 0. The remaining tests are the new camera specs, image specs, system specs, and unit tests.

- [ ] **Step 8: Commit**

```bash
git add src/PeanutVision.Api/Services/IAutoSaveService.cs \
        src/PeanutVision.Api/Services/AutoSaveService.cs \
        src/PeanutVision.Api/Program.cs
git commit -m "feat: delete legacy acquisition session, exposure controller, frame queue, and background service (Stage 4)"
```

---

## Singleton Count Verification

After Task 7, run this check to confirm the singleton reduction from the design spec:

```bash
grep -n "AddSingleton" src/PeanutVision.Api/Program.cs
```

Expected result — 5 singletons, all stateless or thread-safe:
1. `IImageSaveSettingsService` — file-backed, immutable reads
2. `IFrameWriter` — stateless
3. `IThumbnailService` — stateless
4. `ILatencyService` / `ILatencyRepository` — thread-safe internal state
5. `CameraRegistry` — `ConcurrentDictionary` only

Any other singleton is a regression.
