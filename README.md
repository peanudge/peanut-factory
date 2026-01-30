# PeanutVision - MultiCam Driver for .NET

A modern C# wrapper for the Euresys MultiCam SDK, designed for high-speed image acquisition with the Grablink Full frame grabber and Crevis TC-A160K camera.

## Features

- Modern P/Invoke using `LibraryImportAttribute` (.NET 7+ AOT compatible)
- Hardware Abstraction Layer (HAL) for unit testing without hardware
- Thread-safe callback handling with proper GCHandle management
- ASP.NET Core Dependency Injection support
- Embedded camera configuration files
- Comprehensive test coverage (134+ unit tests)

## Project Structure

```
peanut-factory/
├── src/
│   ├── PeanutVision.MultiCamDriver/           # Core driver library
│   │   ├── MultiCamDriver.cs                  # P/Invoke declarations & enums
│   │   ├── GrabService.cs                     # Main service (singleton, DI-ready)
│   │   ├── GrabChannel.cs                     # Channel lifecycle & acquisition
│   │   ├── AcquisitionStatistics.cs           # Performance monitoring
│   │   ├── CamFileResource.cs                 # Embedded resource extraction
│   │   └── Hal/                               # Hardware Abstraction Layer
│   │       ├── IMultiCamHAL.cs               # HAL interface
│   │       ├── MultiCamHAL.cs                # Production implementation
│   │       └── MockMultiCamHAL.cs            # Mock for testing
│   │
│   ├── PeanutVision.Console/                  # Demo console application
│   │   ├── Program.cs                         # Entry point
│   │   ├── CommandContext.cs                  # Shared state
│   │   ├── CommandRunner.cs                   # Menu & execution
│   │   └── Commands/                          # Modular commands
│   │       ├── SystemStatusCommand.cs
│   │       ├── ContinuousAcquisitionCommand.cs
│   │       ├── SoftwareTriggerCommand.cs
│   │       ├── CalibrationCommand.cs
│   │       ├── BenchmarkCommand.cs
│   │       └── CamFileInfoCommand.cs
│   │
│   ├── PeanutVision.MultiCamDriver.Tests/     # Unit tests
│   └── PeanutVision.MultiCamDriver.IntegrationTests/  # Hardware tests
│
├── setup/
│   └── camfiles/                              # Camera configuration files
│       ├── TC-A160K-SEM_freerun_RGB8.cam
│       └── CREVIS_TC-A160K-SEM_FreeRun_1TAP_RGB8.cam
│
└── doc/                                       # Documentation & specs
```

## Prerequisites

### Hardware
- **Frame Grabber**: Euresys Grablink Full (PC1622)
- **Camera**: Crevis TC-A160K (Area-Scan, Camera Link)
- **Cable**: Standard Camera Link cable

### Software
- **Driver**: Euresys MultiCam 6.19.4 or higher
- **Runtime**: .NET 9.0
- **OS**: Windows 10/11 or Linux

## Quick Start

### Running the Demo Console

```bash
# With hardware
cd src/PeanutVision.Console
dotnet run

# Without hardware (mock mode)
dotnet run -- --mock
```

### Basic Usage

```csharp
using PeanutVision.MultiCamDriver;

// Initialize service
using var service = new GrabService();
service.Initialize();

Console.WriteLine($"Driver: {service.DriverVersion}, Boards: {service.BoardCount}");

// Create channel with embedded cam file
using var channel = service.CreateChannelForTC_A160K();

// Subscribe to frame events
channel.FrameAcquired += (sender, args) =>
{
    // Process frame (runs on MultiCam thread - keep fast!)
    byte[] data = args.Surface.ToArray();
};

// Start acquisition
channel.StartAcquisition();
Console.ReadLine();
channel.StopAcquisition();
```

### Software Trigger Mode

```csharp
using var channel = service.CreateChannel(new GrabChannelOptions
{
    CamFilePath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8),
    TriggerMode = McTrigMode.MC_TrigMode_SOFT,
    UseCallback = false
});

channel.StartAcquisition();

channel.SendSoftwareTrigger();
var surface = channel.WaitForFrame(5000);
if (surface.HasValue)
{
    // Process frame
    channel.ReleaseSurface(surface.Value);
}
```

### ASP.NET Core Integration

```csharp
// In Program.cs
builder.Services.AddSingleton<IGrabService, GrabService>();

// In your service
public class VisionService
{
    private readonly IGrabService _grabService;

    public VisionService(IGrabService grabService)
    {
        _grabService = grabService;
        _grabService.Initialize();
    }
}
```

## Architecture Patterns

### 1. Hardware Abstraction Layer (HAL)

The HAL pattern decouples business logic from native P/Invoke calls, enabling comprehensive unit testing without hardware.

```
┌─────────────────┐     ┌──────────────────┐
│   GrabService   │────▶│   IMultiCamHAL   │
│   GrabChannel   │     └────────┬─────────┘
└─────────────────┘              │
                        ┌────────┴─────────┐
                        │                  │
                ┌───────▼──────┐   ┌───────▼───────┐
                │ MultiCamHAL  │   │ MockMultiCamHAL│
                │  (Production)│   │   (Testing)   │
                └───────┬──────┘   └───────────────┘
                        │
                ┌───────▼──────┐
                │ MultiCamNative│
                │  (P/Invoke)  │
                └──────────────┘
```

**Usage:**
```csharp
// Production (default)
using var service = new GrabService();

// Testing with mock
var mockHal = new MockMultiCamHAL();
mockHal.Configuration.BoardCount = 2;
using var service = new GrabService(mockHal);
```

### 2. Command Pattern (Console App)

The console application uses the Command pattern for modular, extensible menu operations.

```
┌─────────────┐     ┌───────────────┐     ┌──────────────┐
│   Program   │────▶│ CommandRunner │────▶│  ICommand    │
└─────────────┘     └───────────────┘     └──────┬───────┘
                            │                    │
                    ┌───────▼───────┐    ┌───────┴────────────┐
                    │CommandContext │    │                    │
                    │ (shared state)│    ▼                    ▼
                    └───────────────┘  SystemStatus    Calibration
                                       Acquisition     Benchmark
                                       ...
```

**Adding a new command:**
```csharp
public sealed class MyCommand : CommandBase
{
    public override string Name => "My Command";
    public override string Description => "Does something";
    public override char Key => '7';

    public override void Execute(CommandContext context)
    {
        PrintHeader("MY COMMAND");
        // Implementation...
        PrintFooter();
        WaitForKey();
    }
}

// Register in CommandRunner
Register(new MyCommand());
```

### 3. Thread-Safe Callback Handling

Native callbacks require careful GCHandle management to prevent garbage collection.

```csharp
// Pin delegate and 'this' reference
_nativeCallback = OnNativeCallback;
_callbackHandle = GCHandle.Alloc(_nativeCallback);
_thisHandle = GCHandle.Alloc(this);

// Get function pointer
IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(_nativeCallback);
IntPtr contextPtr = GCHandle.ToIntPtr(_thisHandle);

// Register with native API
_hal.RegisterCallback(_channelHandle, callbackPtr, contextPtr);

// In static callback, recover 'this'
private static void OnNativeCallback(ref McSignalInfo info)
{
    var channel = GCHandle.FromIntPtr(info.Context).Target as GrabChannel;
    channel?.ProcessSignal(ref info);
}

// Free handles in Dispose()
if (_callbackHandle.IsAllocated) _callbackHandle.Free();
if (_thisHandle.IsAllocated) _thisHandle.Free();
```

### 4. Embedded Resource Pattern

Camera configuration files are embedded in the assembly and extracted on-demand.

```xml
<!-- In .csproj -->
<ItemGroup>
  <EmbeddedResource Include="..\..\setup\camfiles\*.cam">
    <LogicalName>CamFiles.%(Filename)%(Extension)</LogicalName>
  </EmbeddedResource>
</ItemGroup>
```

```csharp
// Extract and get path
string camPath = CamFileResource.GetCamFilePath("TC-A160K-SEM_freerun_RGB8.cam");

// Or use known constants
string camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);

// List available files
foreach (var file in CamFileResource.GetAvailableCamFiles())
    Console.WriteLine(file);
```

## Camera Calibration

### Flat Field Correction (FFC)

```csharp
// 1. Black calibration (cover lens)
channel.PerformBlackCalibration();

// 2. White calibration (uniform white surface, ~200 DN)
channel.PerformWhiteCalibration();

// 3. Enable FFC
channel.SetFlatFieldCorrection(true);
```

### White Balance

```csharp
// Auto white balance
channel.PerformWhiteBalanceOnce();

// Manual adjustment
channel.SetWhiteBalanceRatios(red: 1.2, green: 1.0, blue: 0.9);

// Read current ratios
var (r, g, b) = channel.GetWhiteBalanceRatios();
```

### Exposure Control

```csharp
var (minExp, maxExp) = channel.GetExposureRange();
channel.SetExposureUs(1000); // 1ms
double current = channel.GetExposureUs();
```

## Testing

```bash
# Run unit tests (no hardware required)
dotnet test --filter "Category!=Hardware"

# Run hardware integration tests
dotnet test --filter "Category=Hardware"

# Run all tests
dotnet test
```

### Test Coverage
- **GrabServiceTests** - Service lifecycle, initialization
- **GrabChannelTests** - Channel operations, callbacks, calibration
- **MockMultiCamHalTests** - Mock behavior verification
- **CamFileResourceTests** - Embedded resource extraction
- **AcquisitionStatisticsTests** - Performance monitoring

## Performance Monitoring

```csharp
var stats = new AcquisitionStatistics();

channel.FrameAcquired += (s, e) => stats.RecordFrame();
channel.AcquisitionError += (s, e) => stats.RecordError();

stats.Start();
channel.StartAcquisition();
// ...
channel.StopAcquisition();
stats.Stop();

var snapshot = stats.GetSnapshot();
Console.WriteLine($"Frames: {snapshot.FrameCount}");
Console.WriteLine($"FPS: {snapshot.AverageFps:F1}");
Console.WriteLine($"Errors: {snapshot.ErrorCount}");
Console.WriteLine($"Interval: {snapshot.MinFrameIntervalMs:F1}-{snapshot.MaxFrameIntervalMs:F1}ms");
```

## Documentation

- [MultiCam API Reference](https://documentation.euresys.com/Products/MULTICAM/)
- [Grablink Full Product Page](https://www.euresys.com/ko/products/frame-grabber/grablink-full/)

## License

Proprietary - Internal use only.
