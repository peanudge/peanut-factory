# Acquisition OOP Refactoring Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** AcquisitionManager의 God Object를 해소하여 `AcquisitionConfig` 값 객체, `AcquisitionStatus` 스냅샷, 단일 `IAcquisitionSession` 인터페이스로 단순화한다.

**Architecture:** TDD로 진행한다 — 새 타입/메서드의 테스트를 먼저 작성하고, 구현 후 기존 코드를 제거한다. 각 Task는 빌드·테스트가 통과하는 상태로 커밋한다.

**Tech Stack:** .NET 10, ASP.NET Core, C# 12, xUnit

---

## 변경 파일 목록

### 신규
- `src/PeanutVision.Api/Services/AcquisitionConfig.cs`
- `src/PeanutVision.Api/Services/AcquisitionStatus.cs`
- `src/PeanutVision.Api/Services/IAcquisitionSession.cs` (IAcquisitionService 대체)

### 수정
- `src/PeanutVision.Api/Services/AcquisitionManager.cs` — `Start(AcquisitionConfig)`, `GetStatus()` 추가; 기존 산발된 프로퍼티 제거
- `src/PeanutVision.Api/Services/AutoSaveService.cs` — `IAcquisitionSession` 사용, `ActiveProfileId` → `GetStatus().ActiveConfig`
- `src/PeanutVision.Api/Controllers/AcquisitionController.cs` — 새 인터페이스 사용, 컨트롤러 코드 단순화
- `src/PeanutVision.Api/Program.cs` — DI 등록 갱신
- `src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs` — 새 API 기준 테스트 갱신
- `src/PeanutVision.Api.Tests/Unit/AutoSaveServiceTests.cs` — FakeAcquisitionService 갱신
- `src/PeanutVision.Api.Tests/Unit/CalibrationManagerTests.cs` — AcquisitionManager 직접 참조 유지 (concrete class는 유지)

### 삭제
- `src/PeanutVision.Api/Services/IAcquisitionService.cs`
- `src/PeanutVision.Api/Services/IChannelService.cs`

---

## Task 1: `AcquisitionConfig` 값 객체

**Files:**
- Create: `src/PeanutVision.Api/Services/AcquisitionConfig.cs`
- Test: `src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

`AcquisitionManagerTests.cs` 파일 끝 (`}` 직전)에 새 클래스 추가:

```csharp
public class Given_AcquisitionConfig : AcquisitionManagerTests
{
    [Fact]
    public void Config_holds_all_fields()
    {
        var config = new AcquisitionConfig(
            new ProfileId("cam.cam"),
            TriggerMode.Soft,
            FrameCount: 5,
            IntervalMs: 100);

        Assert.Equal("cam.cam", config.ProfileId.Value);
        Assert.Equal(TriggerMode.Soft, config.TriggerMode);
        Assert.Equal(5, config.FrameCount);
        Assert.Equal(100, config.IntervalMs);
    }

    [Fact]
    public void Config_defaults_optional_fields_to_null()
    {
        var config = new AcquisitionConfig(new ProfileId("cam.cam"));

        Assert.Null(config.TriggerMode);
        Assert.Null(config.FrameCount);
        Assert.Null(config.IntervalMs);
    }
}
```

- [ ] **Step 2: 테스트 실패 확인**

```bash
dotnet test peanut-factory.sln --filter "Given_AcquisitionConfig" -v quiet 2>&1 | tail -5
```
Expected: 컴파일 오류 (`AcquisitionConfig` 존재하지 않음)

- [ ] **Step 3: `AcquisitionConfig.cs` 구현**

```csharp
namespace PeanutVision.Api.Services;

public sealed record AcquisitionConfig(
    ProfileId ProfileId,
    TriggerMode? TriggerMode = null,
    int? FrameCount = null,
    int? IntervalMs = null
);
```

- [ ] **Step 4: 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "Given_AcquisitionConfig" -v quiet 2>&1 | tail -5
```
Expected: `Passed: 2`

- [ ] **Step 5: 커밋**

```bash
git add src/PeanutVision.Api/Services/AcquisitionConfig.cs \
        src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs
git commit -m "feat: add AcquisitionConfig value object"
```

---

## Task 2: `AcquisitionManager.Start(AcquisitionConfig)` — 병합된 Start

**Files:**
- Modify: `src/PeanutVision.Api/Services/AcquisitionManager.cs`
- Test: `src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

`AcquisitionManagerTests.cs`에 새 클래스 추가:

```csharp
public class Given_Start_with_config : AcquisitionManagerTests
{
    [Fact]
    public void Then_channel_is_active()
    {
        var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"));
        _manager.Start(config);

        Assert.True(_manager.IsActive);
        Assert.Equal(ChannelState.Active, _manager.ChannelState);
    }

    [Fact]
    public void Then_active_profile_id_matches_config()
    {
        var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"));
        _manager.Start(config);

        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", _manager.ActiveProfileId?.Value);
    }

    [Fact]
    public void Then_auto_releases_idle_channel_before_starting()
    {
        // Put manager into Idle state
        _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
        _mockHal.CallLog.Reset();

        var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"));
        _manager.Start(config);

        // Old channel released, new one created
        Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
        Assert.Equal(1, _mockHal.CallLog.CreateCalls);
        Assert.Equal("crevis-tc-a160k-softtrig-rgb8.cam", _manager.ActiveProfileId?.Value);
    }

    [Fact]
    public void When_already_active_then_throws()
    {
        var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"));
        _manager.Start(config);

        Assert.Throws<InvalidOperationException>(() =>
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"))));
    }

    [Fact]
    public void Then_frameCount_stored_as_ActiveFrameCount()
    {
        var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), FrameCount: 7);
        _manager.Start(config);

        Assert.Equal(7, _manager.ActiveFrameCount);
    }

    [Fact]
    public void Then_intervalMs_stored_as_ActiveIntervalMs()
    {
        var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), IntervalMs: 200);
        _manager.Start(config);

        Assert.Equal(200, _manager.ActiveIntervalMs);
    }
}
```

- [ ] **Step 2: 테스트 실패 확인**

```bash
dotnet test peanut-factory.sln --filter "Given_Start_with_config" -v quiet 2>&1 | tail -5
```
Expected: 컴파일 오류 (`AcquisitionManager.Start(AcquisitionConfig)` 존재하지 않음)

- [ ] **Step 3: `AcquisitionManager`에 `Start(AcquisitionConfig)` 구현**

`AcquisitionManager.cs`에서 `CreateChannel` 메서드 바로 뒤에 다음을 추가한다:

```csharp
public void Start(AcquisitionConfig config)
{
    const int minIntervalMs = 50;
    if (config.IntervalMs is > 0 and < minIntervalMs)
        throw new ArgumentException($"intervalMs must be at least {minIntervalMs}ms.");

    // Auto-release idle channel outside lock (avoids potential deadlock with channel dispose)
    GrabChannel? toRelease = null;
    lock (_lock)
    {
        if (_channelState == ChannelState.Active)
            throw new InvalidOperationException("Acquisition is already running. Stop it first.");

        if (_channelState == ChannelState.Idle)
        {
            toRelease = _channel;
            _channel = null;
            _channelProfileId = null;
            _channelTriggerMode = null;
            _channelState = ChannelState.None;
            _lastFrame = null;
            _statistics = null;
        }
    }
    if (toRelease != null)
        _grabService.ReleaseChannel(toRelease);

    // Create channel and start acquisition under a single lock section
    lock (_lock)
    {
        var camFile = _camFileService.GetByFileName(config.ProfileId.Value);
        var options = config.TriggerMode.HasValue
            ? camFile.ToChannelOptions(config.TriggerMode.Value.Mode)
            : camFile.ToChannelOptions();

        _channel = _grabService.CreateChannel(options);
        _channelProfileId = config.ProfileId;
        _channelTriggerMode = config.TriggerMode;
        _lastFrame = null;
        _lastError = null;
        _statistics = new AcquisitionStatistics();

        _channel.FrameAcquired   += OnFrameAcquired;
        _channel.AcquisitionError += OnAcquisitionError;
        _channel.AcquisitionEnded += OnAcquisitionEnded;

        _targetFrameCount = config.FrameCount;
        _activeIntervalMs = config.IntervalMs is > 0 ? config.IntervalMs : null;
        _channelState = ChannelState.Active;
        _statistics.Start();
        _channel.StartAcquisition(config.FrameCount ?? -1);
        try { _channel.SetExposureUs(_desiredExposureUs); } catch { /* best-effort */ }

        if (config.IntervalMs is > 0)
        {
            var ms = config.IntervalMs.Value;
            _triggerTimer = new Timer(_ =>
            {
                lock (_lock)
                {
                    if (_channel?.IsActive == true)
                    {
                        _triggerTimestamps.Enqueue(DateTimeOffset.UtcNow);
                        _channel.SendSoftwareTrigger();
                    }
                }
            }, null, 0, ms);
        }

        _eventLog.Add(new ChannelEvent(
            DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
            $"Acquisition started with profile '{config.ProfileId.Value}'" +
            (config.FrameCount.HasValue ? $", frameCount={config.FrameCount}" : "") +
            (config.IntervalMs.HasValue ? $", intervalMs={config.IntervalMs}" : "")));
    }

    StatusChanged?.Invoke(this, EventArgs.Empty);
}
```

- [ ] **Step 4: 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "Given_Start_with_config" -v quiet 2>&1 | tail -5
```
Expected: `Passed: 6`

- [ ] **Step 5: 전체 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "FullyQualifiedName!~IntegrationTests" -v quiet 2>&1 | tail -5
```
Expected: `Failed: 0`

- [ ] **Step 6: 커밋**

```bash
git add src/PeanutVision.Api/Services/AcquisitionManager.cs \
        src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs
git commit -m "feat: add AcquisitionManager.Start(AcquisitionConfig) merging CreateChannel+Start"
```

---

## Task 3: `AcquisitionStatus` 스냅샷 및 `GetStatus()`

**Files:**
- Create: `src/PeanutVision.Api/Services/AcquisitionStatus.cs`
- Modify: `src/PeanutVision.Api/Services/AcquisitionManager.cs`
- Test: `src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs`

- [ ] **Step 1: 실패하는 테스트 작성**

`AcquisitionManagerTests.cs`에 추가:

```csharp
public class Given_GetStatus : AcquisitionManagerTests
{
    [Fact]
    public void When_idle_returns_None_state_and_no_config()
    {
        var status = _manager.GetStatus();

        Assert.Equal(ChannelState.None, status.ChannelState);
        Assert.False(status.IsActive);
        Assert.Null(status.ActiveConfig);
        Assert.False(status.HasFrame);
        Assert.Null(status.LastError);
        Assert.Null(status.Statistics);
    }

    [Fact]
    public void When_active_returns_Active_state_with_config()
    {
        var config = new AcquisitionConfig(
            new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
            TriggerMode.Soft,
            FrameCount: 3,
            IntervalMs: 100);
        _manager.Start(config);

        var status = _manager.GetStatus();

        Assert.Equal(ChannelState.Active, status.ChannelState);
        Assert.True(status.IsActive);
        Assert.NotNull(status.ActiveConfig);
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", status.ActiveConfig!.ProfileId.Value);
        Assert.Equal(TriggerMode.Soft, status.ActiveConfig.TriggerMode);
        Assert.Equal(3, status.ActiveConfig.FrameCount);
        Assert.Equal(100, status.ActiveConfig.IntervalMs);
    }

    [Fact]
    public void When_active_AllowedActions_contains_Stop_and_Trigger()
    {
        _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

        var status = _manager.GetStatus();

        Assert.Contains(ChannelAction.Stop, status.AllowedActions);
        Assert.Contains(ChannelAction.Trigger, status.AllowedActions);
        Assert.DoesNotContain(ChannelAction.Start, status.AllowedActions);
    }

    [Fact]
    public void When_idle_AllowedActions_contains_Start()
    {
        var status = _manager.GetStatus();

        Assert.Contains(ChannelAction.Start, status.AllowedActions);
    }

    [Fact]
    public void After_stop_returns_Idle_state_and_null_config()
    {
        _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
        _manager.Stop();

        var status = _manager.GetStatus();

        Assert.Equal(ChannelState.Idle, status.ChannelState);
        Assert.False(status.IsActive);
        Assert.Null(status.ActiveConfig);
    }
}
```

- [ ] **Step 2: 테스트 실패 확인**

```bash
dotnet test peanut-factory.sln --filter "Given_GetStatus" -v quiet 2>&1 | tail -5
```
Expected: 컴파일 오류 (`AcquisitionStatus`, `GetStatus()` 존재하지 않음)

- [ ] **Step 3: `AcquisitionStatus.cs` 구현**

```csharp
using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

public sealed record AcquisitionStatus(
    ChannelState ChannelState,
    AcquisitionConfig? ActiveConfig,
    bool HasFrame,
    string? LastError,
    AcquisitionStatisticsSnapshot? Statistics,
    IReadOnlyList<ChannelEvent> RecentEvents,
    IReadOnlySet<ChannelAction> AllowedActions
)
{
    public bool IsActive => ChannelState == ChannelState.Active;
}
```

- [ ] **Step 4: `AcquisitionManager.GetStatus()` 구현**

`AcquisitionManager.cs`에서 `GetStatistics()` 메서드 바로 뒤에 추가:

```csharp
public AcquisitionStatus GetStatus()
{
    lock (_lock)
    {
        var statsSnapshot = _statistics?.GetSnapshot();
        AcquisitionStatisticsSnapshot? stats = null;
        if (statsSnapshot.HasValue && _channel != null)
        {
            stats = statsSnapshot.Value with
            {
                CopyDropCount = _channel.CopyDropCount,
                ClusterUnavailableCount = _channel.ClusterUnavailableCount,
            };
        }
        else if (statsSnapshot.HasValue)
        {
            stats = statsSnapshot;
        }

        AcquisitionConfig? activeConfig = _channelState == ChannelState.Active
            ? new AcquisitionConfig(_channelProfileId!.Value, _channelTriggerMode, _targetFrameCount, _activeIntervalMs)
            : null;

        var allowedActions = _channelState switch
        {
            ChannelState.None   => (IReadOnlySet<ChannelAction>)new HashSet<ChannelAction> { ChannelAction.Start },
            ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start },
            ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
            _                   => new HashSet<ChannelAction>(),
        };

        return new AcquisitionStatus(
            ChannelState: _channelState,
            ActiveConfig: activeConfig,
            HasFrame: _lastFrame != null,
            LastError: _lastError,
            Statistics: stats,
            RecentEvents: _eventLog.GetRecent(50),
            AllowedActions: allowedActions
        );
    }
}
```

- [ ] **Step 5: 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "Given_GetStatus" -v quiet 2>&1 | tail -5
```
Expected: `Passed: 5`

- [ ] **Step 6: 전체 테스트 확인**

```bash
dotnet test peanut-factory.sln --filter "FullyQualifiedName!~IntegrationTests" -v quiet 2>&1 | tail -5
```
Expected: `Failed: 0`

- [ ] **Step 7: 커밋**

```bash
git add src/PeanutVision.Api/Services/AcquisitionStatus.cs \
        src/PeanutVision.Api/Services/AcquisitionManager.cs \
        src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs
git commit -m "feat: add AcquisitionStatus snapshot and AcquisitionManager.GetStatus()"
```

---

## Task 4: `IAcquisitionSession` — 새 인터페이스로 교체

**Files:**
- Create: `src/PeanutVision.Api/Services/IAcquisitionSession.cs`
- Delete: `src/PeanutVision.Api/Services/IAcquisitionService.cs`
- Delete: `src/PeanutVision.Api/Services/IChannelService.cs`
- Modify: `src/PeanutVision.Api/Services/AcquisitionManager.cs`
- Modify: `src/PeanutVision.Api/Services/AutoSaveService.cs`
- Modify: `src/PeanutVision.Api/Controllers/AcquisitionController.cs`
- Modify: `src/PeanutVision.Api/Program.cs`
- Modify: `src/PeanutVision.Api.Tests/Unit/AutoSaveServiceTests.cs`
- Modify: `src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs`

- [ ] **Step 1: `IAcquisitionSession.cs` 작성**

```csharp
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>
/// Controls a single acquisition session. One session at a time.
/// Start(config) auto-releases any existing idle channel before starting.
/// </summary>
public interface IAcquisitionSession : IDisposable
{
    AcquisitionStatus GetStatus();
    void Start(AcquisitionConfig config);
    void Stop();
    void ReleaseChannel();
    Task<ImageData> TriggerAsync(int timeoutMs = 5000);
    ImageData? GetLatestFrame();

    event EventHandler FrameAcquired;
    event EventHandler StatusChanged;
}
```

- [ ] **Step 2: `AcquisitionManager`가 `IAcquisitionSession`을 구현하도록 변경**

`AcquisitionManager.cs` 첫 줄을 변경:

```csharp
// 변경 전
public sealed class AcquisitionManager : IAcquisitionService, IChannelCalibration, IExposureControl

// 변경 후
public sealed class AcquisitionManager : IAcquisitionSession, IChannelCalibration, IExposureControl
```

`TriggerAndWaitAsync`를 `TriggerAsync`로 rename (기존 메서드 시그니처 변경):

```csharp
// 변경 전
public async Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000)

// 변경 후
public async Task<ImageData> TriggerAsync(int timeoutMs = 5000)
```

- [ ] **Step 3: `IAcquisitionService.cs` 삭제**

```bash
rm src/PeanutVision.Api/Services/IAcquisitionService.cs
```

- [ ] **Step 4: `IChannelService.cs` 삭제**

```bash
rm src/PeanutVision.Api/Services/IChannelService.cs
```

- [ ] **Step 5: `AutoSaveService.cs` 갱신**

```csharp
public sealed class AutoSaveService : IHostedService
{
    private readonly IAcquisitionSession _acquisition;   // IAcquisitionService → IAcquisitionSession
    // ... 나머지 필드 동일

    public AutoSaveService(
        IAcquisitionSession acquisition,                  // 타입 변경
        // ... 나머지 파라미터 동일
    )

    private void OnFrameAcquired(object? sender, EventArgs e)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave)
            return;

        var frame = _acquisition.GetLatestFrame();
        if (frame == null || !_frameSaveTracker.ShouldSave(frame))
            return;

        // 변경: ActiveProfileId?.Value → GetStatus().ActiveConfig?.ProfileId.Value
        var profileId = _acquisition.GetStatus().ActiveConfig?.ProfileId.Value;
        _ = SaveAsync(frame, settings, profileId);
    }
    // ... 나머지 동일
}
```

- [ ] **Step 6: `AcquisitionController.cs` 갱신**

전체 파일을 다음으로 교체한다:

```csharp
using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver.Imaging.Encoders;
using System.Text.Json;
using System.Threading.Channels;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcquisitionController : ControllerBase
{
    private readonly IAcquisitionSession _acquisition;

    public AcquisitionController(IAcquisitionSession acquisition)
    {
        _acquisition = acquisition;
    }

    [HttpPost("start")]
    public ActionResult Start([FromBody] StartAcquisitionRequest request)
    {
        var config = new AcquisitionConfig(
            new ProfileId(request.ProfileId),
            request.TriggerMode is not null ? TriggerMode.Parse(request.TriggerMode) : null,
            request.FrameCount,
            request.IntervalMs);

        _acquisition.Start(config);
        return Ok(new { message = "Acquisition started", profileId = config.ProfileId.Value });
    }

    [HttpPost("stop")]
    public ActionResult Stop()
    {
        _acquisition.Stop();
        return Ok(new { message = "Acquisition stopped" });
    }

    [HttpDelete]
    public ActionResult ReleaseChannel()
    {
        _acquisition.ReleaseChannel();
        return Ok(new { message = "Channel released" });
    }

    [HttpGet("status")]
    public ActionResult GetStatus()
    {
        var s = _acquisition.GetStatus();
        return Ok(new
        {
            isActive = s.IsActive,
            channelState = s.ChannelState.ToString().ToLowerInvariant(),
            profileId = s.ActiveConfig?.ProfileId.Value,
            triggerMode = s.ActiveConfig?.TriggerMode?.ToString().ToLowerInvariant(),
            activeFrameCount = s.IsActive ? s.ActiveConfig?.FrameCount : null,
            activeIntervalMs = s.IsActive ? s.ActiveConfig?.IntervalMs : null,
            hasFrame = s.HasFrame,
            lastError = s.LastError,
            allowedActions = s.AllowedActions,
            statistics = s.Statistics.HasValue
                ? new
                {
                    frameCount = s.Statistics.Value.FrameCount,
                    droppedFrameCount = s.Statistics.Value.DroppedFrameCount,
                    errorCount = s.Statistics.Value.ErrorCount,
                    elapsedMs = s.Statistics.Value.ElapsedTime.TotalMilliseconds,
                    averageFps = Math.Round(s.Statistics.Value.AverageFps, 2),
                    minFrameIntervalMs = Math.Round(s.Statistics.Value.MinFrameIntervalMs, 2),
                    maxFrameIntervalMs = Math.Round(s.Statistics.Value.MaxFrameIntervalMs, 2),
                    averageFrameIntervalMs = Math.Round(s.Statistics.Value.AverageFrameIntervalMs, 2),
                    copyDropCount = s.Statistics.Value.CopyDropCount,
                    clusterUnavailableCount = s.Statistics.Value.ClusterUnavailableCount,
                }
                : null,
            recentEvents = s.RecentEvents.Select(e => new
            {
                timestamp = e.Timestamp,
                type = e.Type.ToString(),
                message = e.Message,
            }),
        });
    }

    [HttpGet("events")]
    public async Task GetEvents(CancellationToken ct)
    {
        Response.ContentType = "text/event-stream; charset=utf-8";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        var channel = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions { SingleReader = true });

        void OnFrameAcquired(object? _, EventArgs __) =>
            channel.Writer.TryWrite($"event: frame_ready\ndata: {{\"timestamp\":\"{DateTimeOffset.UtcNow:O}\"}}\n\n");

        void OnStatusChanged(object? _, EventArgs __) =>
            channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

        _acquisition.FrameAcquired += OnFrameAcquired;
        _acquisition.StatusChanged += OnStatusChanged;

        channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

        try
        {
            await foreach (var text in channel.Reader.ReadAllAsync(ct))
            {
                await Response.WriteAsync(text, ct);
                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _acquisition.FrameAcquired -= OnFrameAcquired;
            _acquisition.StatusChanged -= OnStatusChanged;
            channel.Writer.TryComplete();
        }
    }

    [HttpPost("trigger")]
    public async Task<ActionResult> Trigger()
    {
        var image = await _acquisition.TriggerAsync(5000);
        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(image, stream);
        stream.Position = 0;
        return File(stream, "image/png", "trigger.png");
    }

    [HttpGet("latest-frame")]
    public ActionResult GetLatestFrame()
    {
        var frame = _acquisition.GetLatestFrame();
        if (frame is null)
            return NoContent();
        var encoder = new PngEncoder();
        var stream = new MemoryStream();
        encoder.Encode(frame, stream);
        stream.Position = 0;
        return File(stream, "image/png", "latest.png");
    }

    [HttpGet("latest-frame/histogram")]
    public ActionResult GetHistogram()
    {
        var frame = _acquisition.GetLatestFrame();
        if (frame is null)
            return NoContent();
        var histogram = HistogramService.Compute(frame);
        return Ok(new { red = histogram.Red, green = histogram.Green, blue = histogram.Blue, bins = 256 });
    }

    private string BuildStatusJson()
    {
        var s = _acquisition.GetStatus();
        var payload = new
        {
            isActive = s.IsActive,
            channelState = s.ChannelState.ToString().ToLowerInvariant(),
            profileId = s.ActiveConfig?.ProfileId.Value,
            triggerMode = s.ActiveConfig?.TriggerMode?.ToString().ToLowerInvariant(),
            activeFrameCount = s.IsActive ? s.ActiveConfig?.FrameCount : (int?)null,
            activeIntervalMs = s.IsActive ? s.ActiveConfig?.IntervalMs : (int?)null,
            hasFrame = s.HasFrame,
            lastError = s.LastError,
            allowedActions = s.AllowedActions.Select(a => a.ToString().ToLowerInvariant()).ToArray(),
            statistics = s.Statistics.HasValue
                ? (object)new
                {
                    frameCount = s.Statistics.Value.FrameCount,
                    droppedFrameCount = s.Statistics.Value.DroppedFrameCount,
                    errorCount = s.Statistics.Value.ErrorCount,
                    elapsedMs = s.Statistics.Value.ElapsedTime.TotalMilliseconds,
                    averageFps = Math.Round(s.Statistics.Value.AverageFps, 2),
                    minFrameIntervalMs = Math.Round(s.Statistics.Value.MinFrameIntervalMs, 2),
                    maxFrameIntervalMs = Math.Round(s.Statistics.Value.MaxFrameIntervalMs, 2),
                    averageFrameIntervalMs = Math.Round(s.Statistics.Value.AverageFrameIntervalMs, 2),
                    copyDropCount = s.Statistics.Value.CopyDropCount,
                    clusterUnavailableCount = s.Statistics.Value.ClusterUnavailableCount,
                }
                : null,
            recentEvents = s.RecentEvents.Select(e => new
            {
                timestamp = e.Timestamp,
                type = e.Type.ToString(),
                message = e.Message,
            }),
        };
        return JsonSerializer.Serialize(payload);
    }
}

public class StartAcquisitionRequest
{
    public required string ProfileId { get; set; }
    public string? TriggerMode { get; set; }
    public int? FrameCount { get; set; }
    public int? IntervalMs { get; set; }
}
```

- [ ] **Step 7: `Program.cs` DI 등록 갱신**

```csharp
// 변경 전
builder.Services.AddSingleton<AcquisitionManager>();
builder.Services.AddSingleton<IAcquisitionService>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<IChannelCalibration>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<IExposureControl>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<ICalibrationService, CalibrationManager>();

// 변경 후
builder.Services.AddSingleton<AcquisitionManager>();
builder.Services.AddSingleton<IAcquisitionSession>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<IChannelCalibration>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<IExposureControl>(sp => sp.GetRequiredService<AcquisitionManager>());
builder.Services.AddSingleton<ICalibrationService, CalibrationManager>();
```

- [ ] **Step 8: `AutoSaveServiceTests.cs`의 `FakeAcquisitionService` 갱신**

`FakeAcquisitionService` 클래스가 `IAcquisitionSession`을 구현하도록 변경. 다음 멤버들을 조정한다:

```csharp
internal sealed class FakeAcquisitionService : IAcquisitionSession
{
    private ImageData? _frame;

    public event EventHandler? FrameAcquired;
    public event EventHandler? StatusChanged { add { } remove { } }

    public void SimulateFrame(ImageData? frame)
    {
        _frame = frame;
        FrameAcquired?.Invoke(this, EventArgs.Empty);
    }

    public ImageData? GetLatestFrame() => _frame;

    public AcquisitionStatus GetStatus() => new(
        ChannelState: ChannelState.None,
        ActiveConfig: null,
        HasFrame: _frame != null,
        LastError: null,
        Statistics: null,
        RecentEvents: [],
        AllowedActions: new HashSet<ChannelAction> { ChannelAction.Start }
    );

    // Unused stubs
    public void Start(AcquisitionConfig config) { }
    public void Stop() { }
    public void ReleaseChannel() { }
    public Task<ImageData> TriggerAsync(int timeoutMs = 5000) =>
        Task.FromResult(new ImageData(new byte[3], 1, 1, 3));
    public void Dispose() { }
}
```

제거 대상 (더 이상 인터페이스에 없음):
- `ActiveFrameCount`, `ActiveIntervalMs` 프로퍼티
- `IsActive`, `HasFrame`, `ChannelState`, `ActiveProfileId`, `ChannelTriggerMode`
- `GetStatistics()`, `GetRecentEvents()`, `GetAllowedActions()`
- `CreateChannel()`, `TriggerAndWaitAsync()`

- [ ] **Step 9: `AcquisitionManagerTests.cs` 갱신**

`_manager.CreateChannel(...)` → `_manager.Start(new AcquisitionConfig(...))` 로 대체.
이미 `Given_Start_with_config` 테스트에서 검증되므로, 기존 `Given_channel_created` 클래스 내의 `CreateChannel` 호출만 수정한다:

```csharp
public class Given_channel_created : AcquisitionManagerTests
{
    public Given_channel_created()
    {
        _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
        _manager.Stop(); // → Idle 상태로 이동
    }
    // ... 이하 테스트들 동일
}
```

`_manager.Start()` (파라미터 없는 버전) 호출 → `_manager.Start(new AcquisitionConfig(new ProfileId("...")))` 로 전환.

`TriggerAndWaitAsync` → `TriggerAsync` 로 rename.

- [ ] **Step 10: 빌드 확인**

```bash
dotnet build peanut-factory.sln -v quiet 2>&1 | tail -5
```
Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`

- [ ] **Step 11: 전체 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "FullyQualifiedName!~IntegrationTests" -v quiet 2>&1 | tail -8
```
Expected: `Failed: 0`

- [ ] **Step 12: 커밋**

```bash
git add src/PeanutVision.Api/ src/PeanutVision.Api.Tests/
git commit -m "refactor: replace IAcquisitionService with IAcquisitionSession

- IAcquisitionSession: 7 members (vs 18), no IChannelService inheritance
- AcquisitionController uses GetStatus() single call instead of 8 properties
- Start(AcquisitionConfig) in controller: 3 lines (vs 8)
- AutoSaveService uses GetStatus().ActiveConfig?.ProfileId
- Delete IAcquisitionService.cs, IChannelService.cs"
```

---

## Task 5: 기존 중복 코드 제거

기존 `CreateChannel`, `Start(int?, int?)`, 산발된 프로퍼티들이 `AcquisitionManager`에 남아 있지만 외부에서 쓰이지 않는다. `CalibrationManagerTests`가 `AcquisitionManager` concrete를 직접 사용하므로 `CreateChannel`은 `private`으로 변경하고 test를 `Start(AcquisitionConfig)` 기준으로 업데이트한다.

**Files:**
- Modify: `src/PeanutVision.Api/Services/AcquisitionManager.cs`
- Modify: `src/PeanutVision.Api.Tests/Unit/CalibrationManagerTests.cs`
- Modify: `src/PeanutVision.Api.Tests/Unit/AcquisitionManagerTests.cs`

- [ ] **Step 1: `AcquisitionManager`에서 더 이상 인터페이스에 없는 public 멤버를 제거/private화**

`AcquisitionManager.cs`에서 다음을 `private`으로 변경하거나 제거:
- `public void CreateChannel(ProfileId, TriggerMode?)` → `private void CreateChannel(...)` (내부에서만 사용)
- `public void Start(int? frameCount = null, int? intervalMs = null)` → 삭제 (대체됨)
- `public ChannelState ChannelState` → 삭제 (GetStatus()에 포함)
- `public ProfileId? ActiveProfileId` → 삭제 (GetStatus()에 포함)
- `public TriggerMode? ChannelTriggerMode` → 삭제 (GetStatus()에 포함)
- `public bool IsActive` → 삭제 (GetStatus()에 포함)
- `public bool HasFrame` → 삭제 (GetStatus()에 포함)
- `public string? LastError` → 삭제 (GetStatus()에 포함)
- `public int? ActiveFrameCount` → 삭제 (GetStatus()에 포함)
- `public int? ActiveIntervalMs` → 삭제 (GetStatus()에 포함)
- `public AcquisitionStatisticsSnapshot? GetStatistics()` → 삭제 (GetStatus()에 포함)
- `public IReadOnlyList<ChannelEvent> GetRecentEvents(int)` → 삭제 (GetStatus()에 포함)
- `public IReadOnlySet<ChannelAction> GetAllowedActions()` → 삭제 (GetStatus()에 포함)
- `internal GrabChannel? Channel` → 삭제 (테스트에서만 쓰이던 내부 노출)

`CreateChannel`, `ReleaseChannel` (내부용)은 private으로 유지.

- [ ] **Step 2: `CalibrationManagerTests.cs` 갱신**

`CreateChannel` 호출을 `Start(AcquisitionConfig)` + `Stop()` 조합으로 변경:

```csharp
// 변경 전
_acquisitionManager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");

// 변경 후 (Idle 상태 필요 시 — 채널 생성 후 Stop)
_acquisitionManager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
_acquisitionManager.Stop();
```

Active 상태 필요 시:
```csharp
// 변경 전
_acquisitionManager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
_acquisitionManager.Start();

// 변경 후
_acquisitionManager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
```

- [ ] **Step 3: `AcquisitionManagerTests.cs` 잔여 `CreateChannel`/`Start()` 호출 전부 갱신**

파일에서 `_manager.CreateChannel(` 검색 → 모두 `_manager.Start(new AcquisitionConfig(...))` 로 교체.
파일에서 `_manager.Start()` (인자 없는 호출) 검색 → `_manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")))` 로 교체.

- [ ] **Step 4: 빌드 확인**

```bash
dotnet build peanut-factory.sln -v quiet 2>&1 | tail -5
```
Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`

- [ ] **Step 5: 전체 테스트 통과 확인**

```bash
dotnet test peanut-factory.sln --filter "FullyQualifiedName!~IntegrationTests" -v quiet 2>&1 | tail -8
```
Expected: `Failed: 0`

- [ ] **Step 6: grep 검증 — 삭제된 멤버 잔재 없음**

```bash
grep -rn "IAcquisitionService\|IChannelService\|TriggerAndWaitAsync\|\.ActiveProfileId\|\.ChannelTriggerMode\|\.IsActive\b\|\.HasFrame\b\|GetRecentEvents\|GetAllowedActions\|GetStatistics()" \
  src/PeanutVision.Api src/PeanutVision.Api.Tests --include="*.cs" | grep -v "/bin/" | grep -v "/obj/"
```
Expected: 결과 없음

- [ ] **Step 7: 최종 커밋 및 push**

```bash
git add src/PeanutVision.Api/ src/PeanutVision.Api.Tests/
git commit -m "refactor: remove deprecated members from AcquisitionManager

AcquisitionManager now exposes only IAcquisitionSession + IChannelCalibration + IExposureControl.
Scattered public properties replaced by GetStatus() snapshot.
CreateChannel internalized — callers use Start(AcquisitionConfig)."
git push
```
