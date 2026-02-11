# PeanutVision API Test Suite

BDD-style test suite for the PeanutVision REST API. 63 tests covering all endpoints through integration tests (full HTTP pipeline) and unit tests for `AcquisitionManager` business logic.

## Quick Start

```bash
# Run all API tests
dotnet test src/PeanutVision.Api.Tests/

# Run a specific spec
dotnet test src/PeanutVision.Api.Tests/ --filter "FullyQualifiedName~SystemBoardsSpec"

# Run by category
dotnet test src/PeanutVision.Api.Tests/ --filter "FullyQualifiedName~Specs.System"
dotnet test src/PeanutVision.Api.Tests/ --filter "FullyQualifiedName~Specs.Acquisition"
dotnet test src/PeanutVision.Api.Tests/ --filter "FullyQualifiedName~Specs.Calibration"
dotnet test src/PeanutVision.Api.Tests/ --filter "FullyQualifiedName~Unit"
```

## Project Structure

```
PeanutVision.Api.Tests/
  Infrastructure/
    PeanutVisionApiFactory.cs       # WebApplicationFactory with MockMultiCamHAL
    HttpClientExtensions.cs         # JSON request/response helpers
  Specs/
    System/
      SystemBoardsSpec.cs           # GET /api/system/boards, boards/{index}/status
      SystemCamerasSpec.cs          # GET /api/system/cameras
    Acquisition/
      AcquisitionStartSpec.cs       # POST /api/acquisition/start
      AcquisitionStopSpec.cs        # POST /api/acquisition/stop
      AcquisitionStatusSpec.cs      # GET  /api/acquisition/status
      AcquisitionTriggerSpec.cs     # POST /api/acquisition/trigger
      AcquisitionCaptureSpec.cs     # POST /api/acquisition/capture
      AcquisitionLifecycleSpec.cs   # Multi-step sequences
    Calibration/
      CalibrationOperationsSpec.cs  # POST black, white, white-balance, ffc
      CalibrationExposureSpec.cs    # GET/PUT /api/calibration/exposure
  Unit/
    AcquisitionManagerTests.cs      # Direct unit tests for AcquisitionManager
```

## Architecture

### Two Layers of Testing

The test suite uses two complementary approaches:

**Integration tests** (`Specs/`) send real HTTP requests through the full ASP.NET Core pipeline. They exercise routing, model binding, content negotiation, serialization, controller logic, and service interaction as a single coherent unit. Assertions use `System.Text.Json.JsonDocument` to inspect responses without needing strongly-typed DTOs in the test project.

**Unit tests** (`Unit/`) create `AcquisitionManager` directly with a `MockMultiCamHAL`, bypassing the HTTP layer entirely. They verify business logic such as state transitions, exception semantics, and the pinned-memory frame simulation path that cannot be exercised through HTTP.

### How the Mock HAL Replaces Real Hardware

The production `Program.cs` registers `IGrabService` via `AddGrabService(autoInitialize: true)`, which creates a `GrabService` backed by the real `MultiCamHAL` (native driver). The test factory replaces this:

```
Production:  IGrabService -> GrabService(MultiCamHAL)   -> MultiCam.dll -> Hardware
Tests:       IGrabService -> GrabService(MockMultiCamHAL) -> In-memory simulation
```

`PeanutVisionApiFactory` performs the swap:

```csharp
public class PeanutVisionApiFactory : WebApplicationFactory<Program>
{
    public MockMultiCamHAL MockHal { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGrabService>();
            services.AddSingleton<IGrabService>(_ =>
            {
                var service = new GrabService(MockHal);
                service.Initialize();
                return service;
            });
        });
    }
}
```

The `MockHal` property is exposed so tests can:
- Configure behavior before a request (`MockHal.Configuration`)
- Verify HAL interactions after a request (`MockHal.CallLog`)
- Simulate hardware events (`MockHal.SimulateFrameAcquisition()`)
- Reset tracking state between tests (`ResetMockState()`)

### Test Isolation Pattern

Each spec class uses `IClassFixture<PeanutVisionApiFactory>` to share a single test host instance (fast startup), and `IAsyncLifetime` to clean up after each test:

```csharp
public class AcquisitionStartSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public AcquisitionStartSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        // Stop any active acquisition so tests don't leak state
        await _client.PostAsync("/api/acquisition/stop", null);
    }
}
```

The `DisposeAsync` call to `POST /api/acquisition/stop` is idempotent (the API returns 200 even when idle), ensuring each test starts with a clean acquisition state regardless of what the previous test did.

## Writing Tests

### Integration Test Example

Testing that starting acquisition with a valid profile returns 200 and the correct profile ID:

```csharp
[Fact]
public async Task Start_with_valid_profile_returns_ok()
{
    var response = await _client.PostJsonAsync("/api/acquisition/start",
        new { profileId = "crevis-tc-a160k-freerun-rgb8" });

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    using var doc = await response.ReadJsonDocumentAsync();
    Assert.Equal("crevis-tc-a160k-freerun-rgb8",
        doc.RootElement.GetProperty("profileId").GetString());
}
```

### Verifying HAL Interactions

Tests can assert that the API correctly delegates to the hardware layer by inspecting `MockHal.CallLog`:

```csharp
[Fact]
public async Task Start_sets_hal_acquisition_started()
{
    _factory.ResetMockState();

    await _client.PostJsonAsync("/api/acquisition/start",
        new { profileId = "crevis-tc-a160k-freerun-rgb8" });

    Assert.True(_factory.MockHal.CallLog.AcquisitionStarted);
}
```

Available `CallLog` fields for assertions:

| Field | Type | What It Tracks |
|-------|------|----------------|
| `AcquisitionStarted` | `bool` | ChannelState set to ACTIVE |
| `AcquisitionStopped` | `bool` | ChannelState set to IDLE |
| `SoftwareTriggerCount` | `int` | Number of ForceTrig calls |
| `BlackCalibrationPerformed` | `bool` | BlackCalibration set to ON |
| `WhiteCalibrationPerformed` | `bool` | WhiteCalibration set to ON |
| `WhiteBalancePerformed` | `bool` | BalanceWhiteAuto set to ONCE |
| `CreateCalls` | `int` | Number of McCreate calls |
| `DeleteCalls` | `int` | Number of McDelete calls |
| `LastSetParams` | `ConcurrentDictionary` | Last value set for each param |

Call `_factory.ResetMockState()` before the action under test to isolate from setup-phase HAL calls.

### Testing Error Responses

Error paths follow a consistent pattern: assert the HTTP status code, then optionally verify the error body:

```csharp
[Fact]
public async Task Start_with_unknown_profile_returns_404()
{
    var response = await _client.PostJsonAsync("/api/acquisition/start",
        new { profileId = "nonexistent-camera" });

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    using var doc = await response.ReadJsonDocumentAsync();
    Assert.True(doc.RootElement.TryGetProperty("error", out _));
}
```

The API uses these HTTP status codes for errors:

| Code | Meaning | Example |
|------|---------|---------|
| 404 | Resource not found | Unknown profile, invalid board index, no frame |
| 409 | Conflict / invalid state | Already active, no active channel, trigger while idle |

### Unit Test with Simulated Frame Acquisition

The `LastFrame_populated_after_simulated_frame_with_pinned_memory` test shows how to simulate a frame arriving from the hardware. This is the only way to test the frame capture path because `MockMultiCamHAL`'s default `SimulatedSurfaceAddress` (0x10000000) is not valid memory for `Marshal.Copy`:

```csharp
[Fact]
public void LastFrame_populated_after_simulated_frame_with_pinned_memory()
{
    int width = _mockHal.Configuration.DefaultImageWidth;
    int height = _mockHal.Configuration.DefaultImageHeight;
    int pitch = width * 3;  // RGB24
    int size = pitch * height;
    byte[] pixelData = new byte[size];

    // Pin the array so GC won't move it while native code reads it
    var pinnedHandle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
    try
    {
        // Point the mock's surface address to our pinned array
        _mockHal.Configuration.SimulatedSurfaceAddress =
            pinnedHandle.AddrOfPinnedObject();

        _manager.Start("crevis-tc-a160k-freerun-rgb8");

        // Trigger the callback as if hardware delivered a frame
        _mockHal.SimulateFrameAcquisition(_manager.Channel!.Handle);

        var frame = _manager.CaptureFrame();
        Assert.NotNull(frame);
        Assert.Equal(width, frame.Width);
        Assert.Equal(height, frame.Height);
    }
    finally
    {
        pinnedHandle.Free();
    }
}
```

The key steps are:
1. Allocate a byte array matching the expected image dimensions
2. Pin it with `GCHandle.Alloc(..., GCHandleType.Pinned)` to get a stable memory address
3. Set `MockHal.Configuration.SimulatedSurfaceAddress` to the pinned address
4. Start acquisition and call `SimulateFrameAcquisition()` to invoke the callback
5. Assert that `CaptureFrame()` returns the expected `ImageData`

### JSON Response Assertions

The test suite uses `System.Text.Json.JsonDocument` to inspect responses without strongly-typed DTOs. The `HttpClientExtensions.ReadJsonDocumentAsync()` helper handles deserialization:

```csharp
// Check a specific property value
using var doc = await response.ReadJsonDocumentAsync();
Assert.Equal("crevis-tc-a160k-freerun-rgb8",
    doc.RootElement.GetProperty("profileId").GetString());

// Check that a property exists (without caring about the value)
Assert.True(doc.RootElement.TryGetProperty("message", out _));

// Check JSON value kind (null, array, etc.)
Assert.Equal(JsonValueKind.Null,
    doc.RootElement.GetProperty("profileId").ValueKind);

// Inspect nested objects
var range = doc.RootElement.GetProperty("exposureRange");
Assert.True(range.TryGetProperty("min", out _));

// Check array length
Assert.Equal(3, doc.RootElement.GetArrayLength());

// Iterate array elements
foreach (var item in doc.RootElement.EnumerateArray())
{
    Assert.True(item.TryGetProperty("id", out _));
}
```

### HTTP Helper Methods

`HttpClientExtensions` provides convenience methods so tests read naturally:

```csharp
// POST with JSON body
var response = await _client.PostJsonAsync("/api/acquisition/start",
    new { profileId = "crevis-tc-a160k-freerun-rgb8" });

// PUT with JSON body
var response = await _client.PutJsonAsync("/api/calibration/exposure",
    new { exposureUs = 5000.0, gainDb = 1.5 });

// Parse response as JsonDocument
using var doc = await response.ReadJsonDocumentAsync();
```

## Test Inventory

### System Specs (9 tests)

| Test | Endpoint | Asserts |
|------|----------|---------|
| `GetBoards_returns_ok_with_board_list` | `GET /api/system/boards` | 200, JSON array |
| `GetBoards_returns_one_board_from_mock` | `GET /api/system/boards` | Array length = 1 |
| `GetBoards_board_has_expected_fields` | `GET /api/system/boards` | index, boardType, boardName, serialNumber, pciPosition |
| `GetBoardStatus_returns_ok_for_valid_index` | `GET /api/system/boards/0/status` | 200, boardName, inputState, pcIeLinkInfo |
| `GetBoardStatus_returns_404_for_invalid_index` | `GET /api/system/boards/99/status` | 404 |
| `GetCameras_returns_ok` | `GET /api/system/cameras` | 200 |
| `GetCameras_returns_three_crevis_profiles` | `GET /api/system/cameras` | Array length = 3 |
| `GetCameras_each_profile_has_required_fields` | `GET /api/system/cameras` | id, displayName, manufacturer, model, connector, triggerMode, pixelFormat |
| `GetCameras_includes_freerun_rgb8_profile` | `GET /api/system/cameras` | Contains "crevis-tc-a160k-freerun-rgb8" |

### Acquisition Specs (21 tests)

| Test | Endpoint | Asserts |
|------|----------|---------|
| `Start_with_valid_profile_returns_ok` | `POST /start` | 200, profileId in response |
| `Start_with_unknown_profile_returns_404` | `POST /start` | 404, error field |
| `Start_when_already_active_returns_409` | `POST /start` | 409 |
| `Start_sets_hal_acquisition_started` | `POST /start` | CallLog.AcquisitionStarted |
| `Start_with_custom_trigger_mode_returns_ok` | `POST /start` | 200 with MC_TrigMode_SOFT |
| `Start_response_contains_message` | `POST /start` | message field present |
| `Stop_when_idle_returns_ok` | `POST /stop` | 200, message field |
| `Stop_when_active_returns_ok_and_stops` | `POST /stop` | 200, status shows idle |
| `Stop_sets_hal_acquisition_stopped` | `POST /stop` | CallLog.AcquisitionStopped |
| `Status_when_idle_shows_inactive` | `GET /status` | isActive=false, profileId=null |
| `Status_when_active_shows_active_with_profile` | `GET /status` | isActive=true, profileId set |
| `Status_includes_hasFrame_field` | `GET /status` | hasFrame present |
| `Status_when_active_includes_statistics_shape` | `GET /status` | statistics object with frameCount, droppedFrameCount, errorCount, elapsedMs, averageFps |
| `Trigger_when_inactive_returns_409` | `POST /trigger` | 409 |
| `Trigger_when_active_returns_ok` | `POST /trigger` | 200, message field |
| `Trigger_increments_hal_trigger_count` | `POST /trigger` | CallLog.SoftwareTriggerCount = 2 |
| `Capture_when_no_acquisition_returns_404` | `POST /capture` | 404 |
| `Capture_when_active_but_no_frame_returns_404` | `POST /capture` | 404 |
| `Full_lifecycle_start_status_stop_status` | Multiple | Start->active->Stop->idle |
| `Can_restart_after_stop` | Multiple | Stop then start with different profile |
| `Trigger_after_stop_returns_409` | Multiple | Start->Stop->Trigger = 409 |

### Calibration Specs (16 tests)

| Test | Endpoint | Asserts |
|------|----------|---------|
| `BlackCalibration_without_active_channel_returns_409` | `POST /black` | 409 |
| `WhiteCalibration_without_active_channel_returns_409` | `POST /white` | 409 |
| `WhiteBalance_without_active_channel_returns_409` | `POST /white-balance` | 409 |
| `Ffc_without_active_channel_returns_409` | `POST /ffc` | 409 |
| `BlackCalibration_with_active_channel_returns_ok` | `POST /black` | 200, CallLog.BlackCalibrationPerformed |
| `WhiteCalibration_with_active_channel_returns_ok` | `POST /white` | 200, CallLog.WhiteCalibrationPerformed |
| `WhiteBalance_with_active_channel_returns_ok` | `POST /white-balance` | 200, CallLog.WhiteBalancePerformed |
| `Ffc_enable_with_active_channel_returns_ok` | `POST /ffc` | 200, "enabled" in message |
| `Ffc_disable_with_active_channel_returns_ok` | `POST /ffc` | 200, "disabled" in message |
| `GetExposure_without_active_channel_returns_409` | `GET /exposure` | 409 |
| `SetExposure_without_active_channel_returns_409` | `PUT /exposure` | 409 |
| `GetExposure_returns_exposure_and_gain` | `GET /exposure` | exposureUs, gainDb |
| `GetExposure_includes_range` | `GET /exposure` | exposureRange.min, exposureRange.max |
| `SetExposure_updates_exposure_value` | `PUT /exposure` | exposureUs = 5000.0 |
| `SetGain_updates_gain_value` | `PUT /exposure` | gainDb = 3.5 |
| `SetExposure_and_gain_together` | `PUT /exposure` | Both values updated |

### Unit Tests (17 tests)

| Test | What It Verifies |
|------|------------------|
| `Initially_is_not_active` | Default state: inactive, no profile, no channel |
| `Start_activates_acquisition` | IsActive, ActiveProfileId, Channel all set |
| `Start_sets_hal_to_active` | CallLog.AcquisitionStarted = true |
| `Start_with_unknown_profile_throws_KeyNotFoundException` | Exception type |
| `Start_when_already_active_throws_InvalidOperationException` | Exception type |
| `Start_with_custom_trigger_mode` | McTrigMode.MC_TrigMode_SOFT accepted |
| `Stop_deactivates_acquisition` | State reset to initial |
| `Stop_when_idle_does_nothing` | No exception, stays inactive |
| `Can_restart_after_stop` | Stop then start with different profile |
| `SendTrigger_when_inactive_throws` | InvalidOperationException |
| `SendTrigger_when_active_increments_hal_count` | SoftwareTriggerCount = 2 after 2 calls |
| `CaptureFrame_returns_null_when_no_frame` | Returns null before start |
| `CaptureFrame_returns_null_after_start_with_no_callback` | Returns null without simulated frame |
| `LastFrame_populated_after_simulated_frame_with_pinned_memory` | Full frame capture path with GCHandle |
| `GetStatistics_returns_null_when_idle` | No stats before start |
| `GetStatistics_returns_snapshot_when_active` | Stats with FrameCount = 0 |
| `Dispose_stops_active_acquisition` | IsActive = false after dispose |

## Adding a New Test

1. Choose the right spec file (or create a new one for a new feature area)
2. Follow the existing pattern:
   - Inject `PeanutVisionApiFactory` via `IClassFixture`
   - Implement `IAsyncLifetime` with stop-acquisition cleanup
   - Name tests descriptively: `{Action}_{Condition}_{Expected}`
3. Use `PostJsonAsync`/`PutJsonAsync` for requests with bodies
4. Use `ReadJsonDocumentAsync()` for JSON assertions
5. Use `_factory.MockHal.CallLog` to verify HAL interactions
6. Call `_factory.ResetMockState()` before the action if you need to isolate from setup-phase calls

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.9.2 | Test framework |
| Microsoft.NET.Test.Sdk | 17.12.0 | Test host |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.3 | WebApplicationFactory |
| xunit.runner.visualstudio | 2.8.2 | IDE test runner |
| coverlet.collector | 6.0.2 | Code coverage |
