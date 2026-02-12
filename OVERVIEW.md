# PeanutVision System Overview

## Architecture Diagram

```mermaid
graph TB
    subgraph HW["Hardware"]
        CAM["Crevis TC-A160K\n(Area-Scan Camera)"]
        GL["Grablink Full PC1622\n(Frame Grabber)"]
        CAM -->|Camera Link| GL
    end

    subgraph Driver["PeanutVision.MultiCamDriver (.NET)"]
        HAL["HAL Layer\nIMultiCamHAL / MultiCamHAL / MockMultiCamHAL"]
        MCAPI["MultiCamApi\n(P/Invoke → MultiCam.dll)"]
        GS["GrabService / IGrabService"]
        GC["GrabChannel"]
        IMG["Imaging\nImageData / ImageWriter\nEncoders (PNG, BMP, Raw)"]
        CAMS["Camera\nCameraProfile / CameraRegistry\nCrevisProfiles"]
        SURF["SurfaceData"]
        STATS["AcquisitionStatistics"]
        CAMF["CamFileResource"]

        GS --> GC
        GC --> HAL
        HAL --> MCAPI
        GC --> SURF
        GC --> STATS
        GS --> IMG
        GS --> CAMS
        GS --> CAMF
    end

    subgraph API["PeanutVision.Api (ASP.NET Core)"]
        ACQC["AcquisitionController"]
        CALC["CalibrationController"]
        SYSC["SystemController"]
        ACQM["AcquisitionManager"]

        ACQC --> ACQM
        CALC --> ACQM
        SYSC --> ACQM
        ACQM --> GS
    end

    subgraph CLI["PeanutVision.Console"]
        CMD["CommandRunner"]
        SOFT["SoftwareTriggerCommand"]
        SAVE["SaveImageCommand"]
        CALIB["CalibrationCommand"]
        CONT["ContinuousAcquisitionCommand"]
        BENCH["BenchmarkCommand"]
        BOARD["BoardStatusCommand"]
        STATUS["SystemStatusCommand"]
        CAMINFO["CamFileInfoCommand"]

        CMD --> SOFT
        CMD --> SAVE
        CMD --> CALIB
        CMD --> CONT
        CMD --> BENCH
        CMD --> BOARD
        CMD --> STATUS
        CMD --> CAMINFO
    end

    subgraph UI["peanut-vision-ui (React + MUI)"]
        APP["App.tsx"]
        ACQT["AcquisitionTab"]
        CALT["CalibrationTab"]
        SYST["SystemTab"]
        COMP["Components\nImageViewer / ExposureControl\nStatusChip / BoardRow / ..."]

        APP --> ACQT
        APP --> CALT
        APP --> SYST
        ACQT --> COMP
        CALT --> COMP
        SYST --> COMP
    end

    subgraph Tests["Tests"]
        UT["Unit Tests\nMultiCamDriver.Tests"]
        IT["Integration Tests\nMultiCamDriver.IntegrationTests"]
        AT["API Tests\nApi.Tests"]
    end

    MCAPI -->|"P/Invoke"| GL
    CLI --> GS
    UI -->|"HTTP REST"| API
    UT -.->|tests| Driver
    IT -.->|tests| Driver
    AT -.->|tests| API
```

## Project Structure

```
peanut-factory/
├── src/
│   ├── PeanutVision.MultiCamDriver/     # Core driver library
│   │   ├── Hal/                          # Hardware abstraction layer
│   │   │   ├── IMultiCamHAL.cs           #   HAL interface
│   │   │   ├── MultiCamHAL.cs            #   Real hardware implementation
│   │   │   └── MockMultiCamHAL.cs        #   Mock for testing
│   │   ├── Imaging/                      # Image processing & encoding
│   │   │   ├── ImageData.cs              #   Raw image data model
│   │   │   ├── ImageWriter.cs            #   File output
│   │   │   ├── IImageEncoder.cs          #   Encoder interface
│   │   │   ├── ImageEncoderRegistry.cs   #   Encoder lookup
│   │   │   └── Encoders/                 #   PNG, BMP, Raw encoders
│   │   ├── Camera/                       # Camera profiles
│   │   │   ├── CameraProfile.cs          #   Profile model
│   │   │   ├── CameraRegistry.cs         #   Profile registry
│   │   │   └── Profiles/CrevisProfiles.cs
│   │   ├── MultiCamApi.cs                # P/Invoke bindings to MultiCam.dll
│   │   ├── GrabService.cs                # High-level acquisition service
│   │   ├── IGrabService.cs               # Service interface
│   │   ├── GrabChannel.cs                # Channel lifecycle management
│   │   ├── SurfaceData.cs                # Frame buffer data model
│   │   ├── AcquisitionStatistics.cs      # Performance counters
│   │   ├── CamFileResource.cs            # .cam file management
│   │   ├── ImageSaver.cs                 # Image save helper
│   │   └── ServiceCollectionExtensions.cs # DI registration
│   │
│   ├── PeanutVision.Api/                 # REST API server
│   │   ├── Program.cs
│   │   ├── Services/AcquisitionManager.cs
│   │   └── Controllers/
│   │       ├── AcquisitionController.cs  # Start/stop/trigger/capture
│   │       ├── CalibrationController.cs  # FFC & white balance
│   │       └── SystemController.cs       # Board & camera info
│   │
│   ├── PeanutVision.Console/             # CLI tool
│   │   ├── Program.cs
│   │   ├── CommandRunner.cs
│   │   ├── ICommand.cs
│   │   ├── CommandContext.cs
│   │   └── Commands/
│   │       ├── SoftwareTriggerCommand.cs
│   │       ├── SaveImageCommand.cs
│   │       ├── CalibrationCommand.cs
│   │       ├── ContinuousAcquisitionCommand.cs
│   │       ├── BenchmarkCommand.cs
│   │       ├── BoardStatusCommand.cs
│   │       ├── SystemStatusCommand.cs
│   │       └── CamFileInfoCommand.cs
│   │
│   ├── peanut-vision-ui/                 # React dashboard (Vite + MUI)
│   │   └── src/
│   │       ├── App.tsx
│   │       ├── api/                      # API client & types
│   │       ├── tabs/                     # Acquisition, Calibration, System
│   │       ├── components/               # Shared UI components
│   │       ├── hooks/                    # useApiData, usePolling, ...
│   │       └── theme.ts                  # MUI dark theme
│   │
│   ├── PeanutVision.StrobeLightConsole/  # Strobe light control utility
│   │
│   ├── PeanutVision.MultiCamDriver.Tests/           # Unit tests
│   ├── PeanutVision.MultiCamDriver.IntegrationTests/ # HW integration tests
│   └── PeanutVision.Api.Tests/                      # API spec tests
│
├── doc/                                  # SDK documentation (markdown + PDF)
├── setup/                                # Camera files & SDK headers
│   ├── camfiles/
│   └── multicam_header_files/
└── peanut-factory.sln                    # Solution file
```

## Data Flow

```mermaid
sequenceDiagram
    participant UI as React UI
    participant API as REST API
    participant SVC as GrabService
    participant CH as GrabChannel
    participant HAL as MultiCamHAL
    participant DLL as MultiCam.dll
    participant HW as Grablink + Camera

    UI->>API: POST /acquisition/start
    API->>SVC: StartAcquisition()
    SVC->>CH: Open channel
    CH->>HAL: McOpenDriver / McCreate
    HAL->>DLL: P/Invoke
    DLL->>HW: Configure & activate

    UI->>API: POST /acquisition/trigger
    API->>SVC: SoftwareTrigger()
    SVC->>CH: ForceTrig
    CH->>HAL: McSetParamNmStr("ForceTrig", "TRIG")
    HAL->>DLL: P/Invoke
    DLL->>HW: Trigger capture

    HW-->>DLL: Frame data (signal)
    DLL-->>HAL: MC_SIG_SURFACE_PROCESSING
    HAL-->>CH: Callback / WaitSignal
    CH-->>SVC: SurfaceData

    UI->>API: GET /acquisition/capture
    API->>SVC: GetLastImage()
    SVC-->>API: ImageData (PNG/BMP)
    API-->>UI: Image bytes
```

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Hardware | Crevis TC-A160K (Camera Link) + Grablink Full PC1622 |
| Native Driver | MultiCam.dll (Euresys SDK) |
| Core Library | .NET 9/10, C# 12, LibraryImport (P/Invoke) |
| REST API | ASP.NET Core Minimal/Controllers |
| Frontend | React 18, TypeScript, Vite, MUI (Material UI) |
| Testing | xUnit, WebApplicationFactory |
