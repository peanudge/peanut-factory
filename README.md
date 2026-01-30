# PeanutVision - MultiCam Driver for .NET

A modern C# wrapper for the Euresys MultiCam SDK, designed for high-speed image acquisition with the Grablink Full frame grabber and Crevis TC-A160K camera.

## Features

- Modern P/Invoke using `LibraryImportAttribute` (.NET 9+ AOT compatible)
- Hardware Abstraction Layer (HAL) for unit testing without hardware
- **Camera Profile System** for abstract, extensible camera configurations
- **Image Saving** to PNG, BMP, RAW formats (using ImageSharp)
- Thread-safe callback handling with proper GCHandle management
- ASP.NET Core Dependency Injection support
- Embedded camera configuration files
- Comprehensive test coverage (205+ unit tests)

## Project Structure

```
peanut-factory/
├── src/
│   ├── PeanutVision.MultiCamDriver/           # Core driver library
│   │   ├── MultiCamNative.cs                  # P/Invoke declarations & enums
│   │   ├── GrabService.cs                     # Main service (singleton, DI-ready)
│   │   ├── GrabChannel.cs                     # Channel lifecycle & acquisition
│   │   ├── AcquisitionStatistics.cs           # Performance monitoring
│   │   ├── CamFileResource.cs                 # Embedded resource extraction
│   │   ├── ImageSaver.cs                      # Static facade for saving images
│   │   ├── Hal/                               # Hardware Abstraction Layer
│   │   │   ├── IMultiCamHAL.cs               # HAL interface
│   │   │   ├── MultiCamHAL.cs                # Production implementation
│   │   │   └── MockMultiCamHAL.cs            # Mock for testing
│   │   ├── Camera/                            # Camera Profile System
│   │   │   ├── CameraProfile.cs              # Immutable camera configuration
│   │   │   ├── CameraRegistry.cs             # Profile registry
│   │   │   └── Profiles/
│   │   │       └── CrevisProfiles.cs         # Built-in Crevis profiles
│   │   └── Imaging/                           # Image Processing
│   │       ├── ImageData.cs                  # Image value object
│   │       ├── IImageEncoder.cs              # Encoder strategy interface
│   │       ├── ImageEncoderRegistry.cs       # Encoder registry
│   │       ├── ImageWriter.cs                # Main image writer
│   │       └── Encoders/                     # Format encoders
│   │           ├── PngEncoder.cs
│   │           ├── BmpEncoder.cs
│   │           └── RawEncoder.cs
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
│   │       ├── CamFileInfoCommand.cs
│   │       └── SaveImageCommand.cs            # Capture & save to file
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
using PeanutVision.MultiCamDriver.Camera;

// Initialize service
using var service = new GrabService();
service.Initialize();

Console.WriteLine($"Driver: {service.DriverVersion}, Boards: {service.BoardCount}");

// Create channel using camera profile
using var channel = service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

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
// Use software trigger profile
var profile = CrevisProfiles.TC_A160K_SoftwareTrigger_RGB8;
var options = profile.ToChannelOptions(McTrigMode.MC_TrigMode_SOFT, useCallback: false);

using var channel = service.CreateChannel(options);
channel.StartAcquisition();

channel.SendSoftwareTrigger();
var surface = channel.WaitForFrame(5000);
if (surface.HasValue)
{
    // Process frame
    channel.ReleaseSurface(surface.Value);
}
```

### Saving Images

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

// Simple static API
ImageSaver.Save(surfaceData, "capture.png");
ImageSaver.SaveAsPng(surfaceData, "output.png");
ImageSaver.SaveAsBmp(surfaceData, "output.bmp");
ImageSaver.SaveAsRaw(surfaceData, "output.raw");

// OOP API with custom configuration
var writer = new ImageWriter();
var imageData = ImageData.FromSurface(surfaceData);
writer.Save(imageData, "output.png");

// Custom encoder settings
var registry = new ImageEncoderRegistry()
    .Register(new PngEncoder(PngCompressionLevel.BestCompression));
var customWriter = new ImageWriter(registry);
```

### ASP.NET Core Integration

```csharp
// In Program.cs
builder.Services.AddSingleton<IGrabService, GrabService>();
builder.Services.AddSingleton(CameraRegistry.Default);
builder.Services.AddTransient<ImageWriter>();

// In your service
public class VisionService
{
    private readonly IGrabService _grabService;
    private readonly ImageWriter _imageWriter;

    public VisionService(IGrabService grabService, ImageWriter imageWriter)
    {
        _grabService = grabService;
        _imageWriter = imageWriter;
        _grabService.Initialize();
    }
}
```

## Camera Profile System

The Camera Profile System abstracts camera-specific details into reusable, extensible configurations.

### Available Profiles

| Profile ID | Description |
|------------|-------------|
| `crevis-tc-a160k-freerun-rgb8` | Standard continuous acquisition |
| `crevis-tc-a160k-freerun-1tap-rgb8` | Single-tap configuration |
| `crevis-tc-a160k-softtrig-rgb8` | Software-triggered capture |

### Using Profiles

```csharp
// Use predefined profile
using var channel = service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

// Lookup by ID
var profile = service.CameraProfiles.GetProfile("crevis-tc-a160k-freerun-rgb8");

// List all available profiles
foreach (var p in service.CameraProfiles.Profiles)
{
    Console.WriteLine($"{p.Id}: {p.DisplayName}");
}

// Filter by manufacturer
var crevisProfiles = service.CameraProfiles.GetByManufacturer("Crevis");
```

### Creating Custom Profiles

```csharp
var customProfile = new CameraProfile.Builder()
    .WithId("my-camera-v1")
    .WithDisplayName("My Custom Camera")
    .WithManufacturer("MyBrand")
    .WithModel("CAM-500")
    .WithCamFile("mybrand_cam500.cam")
    .WithConnector("A")
    .WithTriggerMode(McTrigMode.MC_TrigMode_IMMEDIATE)
    .WithSurfaceCount(8)
    .WithPixelFormat(PixelFormat.Rgb24)
    .WithResolution(2048, 1536)
    .WithDescription("High-speed industrial camera")
    .Build();

// Register globally
CameraRegistry.Default.Register(customProfile);

// Or use custom registry
var myRegistry = new CameraRegistry()
    .Register(customProfile)
    .Register(anotherProfile);
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

### 2. Strategy Pattern (Image Encoders)

Image encoding uses the Strategy pattern for extensible format support.

```
┌─────────────┐     ┌────────────────────┐
│ ImageWriter │────▶│ ImageEncoderRegistry│
└─────────────┘     └─────────┬──────────┘
                              │
              ┌───────────────┼───────────────┐
              │               │               │
       ┌──────▼─────┐  ┌──────▼─────┐  ┌──────▼─────┐
       │ PngEncoder │  │ BmpEncoder │  │ RawEncoder │
       └────────────┘  └────────────┘  └────────────┘
```

### 3. Builder Pattern (Camera Profiles)

Camera profiles use the Builder pattern for fluent, type-safe construction.

```csharp
var profile = new CameraProfile.Builder()
    .WithId("my-camera")
    .WithManufacturer("Acme")
    .WithCamFile("acme.cam")
    .Build();
```

### 4. Registry Pattern (Open/Closed Principle)

Both `CameraRegistry` and `ImageEncoderRegistry` support adding new items without modifying existing code.

```csharp
// Add new camera without changing library code
CameraRegistry.Default.Register(myNewCameraProfile);

// Add new image format without changing library code
ImageEncoderRegistry.Default.Register(new TiffEncoder());
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

| Module | Tests |
|--------|-------|
| GrabService | Service lifecycle, initialization |
| GrabChannel | Channel operations, callbacks, calibration |
| MockMultiCamHAL | Mock behavior verification |
| CamFileResource | Embedded resource extraction |
| AcquisitionStatistics | Performance monitoring |
| CameraProfile | Profile builder, options conversion |
| CameraRegistry | Profile lookup, filtering |
| ImageSaver | Format detection, file saving |
| ImageWriter | OOP image writing |
| ImageEncoderRegistry | Encoder registration |

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
