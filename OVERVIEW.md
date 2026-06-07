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
        GC["GrabChannel\n(콜백 기반 프레임 수신)"]
        IMG["Imaging\nImageData / ImageWriter\nEncoders (PNG, BMP, Raw)"]
        CAMF["Camera\nCamFileInfo / CamFileService"]
        SURF["SurfaceData"]
        STATS["AcquisitionStatistics"]

        GS --> GC
        GC --> HAL
        HAL --> MCAPI
        GC --> SURF
        GC --> STATS
        GS --> IMG
        GS --> CAMF
    end

    subgraph API["PeanutVision.Api (ASP.NET Core)"]
        ACQC["AcquisitionController\n(start / stop / trigger / latest-frame)"]
        SYSC["SystemController"]
        IMGC["ImagesController"]
        LATC["LatencyController"]
        ACQM["AcquisitionManager\nimplements IAcquisitionSession"]
        AUTOSAVE["AutoSaveService\n(IHostedService)\nFrameAcquired 구독 → 자동 저장"]

        ACQC --> ACQM
        SYSC --> ACQM
        ACQM --> GS
        ACQM -->|"FrameAcquired 이벤트"| AUTOSAVE
        AUTOSAVE -->|"디스크 + DB 기록"| IMGC
    end

    subgraph FakeCam["PeanutVision.FakeCamDriver"]
        FHAL["FakeMultiCamHAL\n(테스트용 HAL 구현)"]
        FGEN["TestPatternGenerator"]
        FHAL --> FGEN
    end

    subgraph UI["peanut-vision-app (React + SCSS)"]
        ROUTER["TanStack Router\n(file-based routing)"]
        ACQPAGE["AcquisitionPage\n(Capture / Settings 2탭)"]
        GALPAGE["GalleryPage"]
        LATPAGE["LatencyPage"]
        SYSPAGE["SystemPage"]
        HOOKS["Focused Hooks\nuseAcquisitionConfig\nuseAcquisitionSession"]

        ROUTER --> ACQPAGE
        ROUTER --> GALPAGE
        ROUTER --> LATPAGE
        ROUTER --> SYSPAGE
        ACQPAGE --> HOOKS
    end

    subgraph Tests["Tests"]
        UT["Unit Tests\nMultiCamDriver.Tests / Api.Tests Unit"]
        IT["Integration Tests\nMultiCamDriver.IntegrationTests"]
        AT["API Spec Tests\nApi.Tests Specs"]
    end

    MCAPI -->|"P/Invoke"| GL
    UI -->|"HTTP REST + SSE"| API
    FakeCam -.->|"테스트 대체"| API
    UT -.->|tests| Driver
    IT -.->|tests| Driver
    AT -.->|tests| API
```

## Project Structure

```
peanut-factory/
├── src/
│   ├── PeanutVision.MultiCamDriver/          # 코어 드라이버 라이브러리
│   │   ├── Hal/                               # 하드웨어 추상화 레이어
│   │   │   ├── IMultiCamHAL.cs               #   HAL 인터페이스
│   │   │   ├── MultiCamHAL.cs                #   실제 하드웨어 구현
│   │   │   └── MockMultiCamHAL.cs            #   테스트용 Mock
│   │   ├── Imaging/                           # 이미지 처리 및 인코딩
│   │   │   ├── ImageData.cs                  #   원시 이미지 데이터 모델
│   │   │   ├── ImageWriter.cs                #   파일 출력
│   │   │   ├── IImageEncoder.cs              #   인코더 인터페이스
│   │   │   ├── ImageEncoderRegistry.cs       #   인코더 조회
│   │   │   └── Encoders/                     #   PNG, BMP, Raw 인코더
│   │   ├── Camera/                            # CamFile 파싱 및 관리
│   │   │   ├── CamFileInfo.cs                #   .cam 파일 메타데이터
│   │   │   ├── CamFileParser.cs              #   파서
│   │   │   └── CamFileService.cs             #   서비스
│   │   ├── MultiCamApi.cs                    # P/Invoke 바인딩 (MultiCam.dll)
│   │   ├── GrabService.cs                    # 드라이버 초기화 + 채널 관리
│   │   ├── GrabChannel.cs                    # 채널 수명주기 (콜백 전용)
│   │   ├── SurfaceData.cs                    # 프레임 버퍼 데이터 모델
│   │   ├── AcquisitionStatistics.cs          # 성능 카운터
│   │   └── ServiceCollectionExtensions.cs    # DI 등록
│   │
│   ├── PeanutVision.FakeCamDriver/           # 테스트용 가짜 HAL 구현
│   │   ├── FakeMultiCamHAL.cs               #   MockHAL 기반 프레임 시뮬레이터
│   │   ├── FakeHalConfiguration.cs          #   시뮬레이션 설정
│   │   ├── SurfaceMemoryManager.cs          #   가상 서피스 메모리
│   │   └── FrameGenerators/                 #   테스트 패턴 생성기
│   │
│   ├── PeanutVision.Api/                     # REST API 서버
│   │   ├── Program.cs
│   │   ├── Controllers/
│   │   │   ├── AcquisitionController.cs     #   start / stop / trigger / latest-frame
│   │   │   ├── SystemController.cs          #   보드 & 카메라 정보
│   │   │   ├── ImagesController.cs          #   저장 이미지 목록/조회/삭제
│   │   │   ├── LatencyController.cs         #   레이턴시 분석
│   │   │   ├── SettingsController.cs        #   이미지 저장 설정
│   │   │   └── PresetController.cs          #   촬영 프리셋
│   │   └── Services/
│   │       ├── IAcquisitionSession.cs       #   촬영 세션 인터페이스 (7 members)
│   │       ├── AcquisitionManager.cs        #   IAcquisitionSession 구현, 상태 머신
│   │       ├── AcquisitionConfig.cs         #   촬영 설정 값 객체 (ProfileId, TriggerMode, ...)
│   │       ├── AcquisitionStatus.cs         #   촬영 상태 스냅샷 (단일 GetStatus() 호출)
│   │       ├── AutoSaveService.cs           #   FrameAcquired 구독 → headless 자동 저장
│   │       ├── LatencyService.cs            #   레이턴시 측정 및 기록
│   │       ├── ImageSaveSettingsService.cs  #   저장 설정 영속화
│   │       ├── FilenameGenerator.cs         #   {date}/{profile} 토큰 기반 경로 생성
│   │       ├── FrameSaveTracker.cs          #   중복 저장 방지
│   │       └── ThumbnailService.cs          #   썸네일 생성
│   │
│   ├── peanut-vision-app/                    # React 대시보드 (Vite + TanStack Router + SCSS)
│   │   └── src/
│   │       ├── routes/                       # 파일 기반 라우팅
│   │       ├── components/
│   │       │   ├── Acquisition/             #   촬영 페이지
│   │       │   │   ├── index.tsx            #     레이아웃 (Capture / Settings 2탭)
│   │       │   │   └── CaptureTab.tsx       #     촬영 중 readonly 뷰 / 설정 폼
│   │       │   ├── Gallery/                 #   이미지 갤러리 (날짜 범위 필터)
│   │       │   ├── Latency/                 #   레이턴시 분석
│   │       │   ├── SystemState/             #   시스템 상태
│   │       │   └── shared/                  #   ContinuousSettings, ImageViewer,
│   │       │                                #   ImageSaveSettingsPanel, PresetSelector, ...
│   │       ├── hooks/
│   │       │   ├── useAcquisitionConfig.ts  #   폼 상태 (profile, triggerMode, frameCount)
│   │       │   ├── useAcquisitionSession.ts #   실행 + 상태 (start/stop/trigger, canStart...)
│   │       │   └── useLiveStream.ts         #   SSE 기반 라이브 뷰
│   │       └── api/                         #   REST API 클라이언트 & 타입 정의
│   │
│   ├── PeanutVision.MultiCamDriver.Tests/            # 유닛 테스트
│   ├── PeanutVision.MultiCamDriver.IntegrationTests/ # HW 통합 테스트
│   └── PeanutVision.Api.Tests/                       # API 스펙 테스트
│
├── doc/                                      # SDK 문서 (마크다운)
├── setup/                                    # 카메라 파일 & SDK 헤더
│   ├── camfiles/
│   └── multicam_header_files/
└── peanut-factory.sln
```

## Acquisition Flow (촬영 흐름)

```mermaid
sequenceDiagram
    participant UI as React UI
    participant API as REST API
    participant ACQM as AcquisitionManager
    participant CH as GrabChannel
    participant HAL as MultiCamHAL
    participant HW as Grablink + Camera
    participant SAVE as AutoSaveService

    UI->>API: POST /acquisition/start\n{profileId, triggerMode?, frameCount?, intervalMs?}
    API->>ACQM: Start(AcquisitionConfig)
    ACQM->>CH: StartAcquisition()
    CH->>HAL: McCreate / McSetParam(CamFile) / ChannelState=ACTIVE
    HAL->>HW: Configure & activate\n(cam file 설정 자동 적용)

    Note over UI,SAVE: Continuous 모드 — 소프트 트리거 예시
    UI->>API: POST /acquisition/trigger
    API->>ACQM: TriggerAsync()
    ACQM->>CH: SendSoftwareTrigger()
    CH->>HAL: McSetParam(ForceTrig=TRIG)
    HAL->>HW: Trigger capture

    HW-->>HAL: Frame data (MC_SIG_SURFACE_PROCESSING)
    HAL-->>CH: Native callback (< 1ms)
    CH-->>CH: 내부 큐 → ProcessingLoopAsync
    CH-->>ACQM: FrameAcquired 이벤트
    ACQM-->>API: ImageData 반환 (TriggerAsync 완료)
    API-->>UI: PNG image bytes

    Note over ACQM,SAVE: AutoSave 활성화 시 병렬 저장
    ACQM-->>SAVE: FrameAcquired 이벤트
    SAVE-->>SAVE: SaveAsync() fire-and-forget
    Note right of SAVE: 디스크 저장 + 썸네일 + DB 기록
```

## Acquisition Mode

촬영은 **Continuous 모드** 하나만 존재합니다.

| 파라미터 | 설명 |
|---------|------|
| `profileId` | 사용할 cam 파일 이름 |
| `triggerMode` | `soft` / `hard` / `combined` |
| `frameCount` | 캡처할 프레임 수 (`null` = 무한, `1` = 단발 촬영) |
| `intervalMs` | 자동 트리거 간격 (auto submode) |

단발 촬영(`frameCount=1`)은 프레임 수신 후 채널이 자동으로 Idle 상태로 돌아갑니다.

## Camera Profile (.cam 파일) 설계 원칙

캘리브레이션은 서비스 UI에서 제거되었습니다. 대신 **cam 파일이 이미 보정된 설정을 포함**한다고 가정합니다.

**근거:**
- 땅콩 검사 시스템은 조명·카메라 위치가 고정된 통제된 환경
- 초기 설치 시 1회 캘리브레이션 후 결과를 cam 파일에 저장
- 이후 동일 환경에서 재보정 불필요

**cam 파일에 포함되는 설정:**
```
BalanceRatioRed   = 1.25    ← 화이트 밸런스 (초기 캘리브레이션 결과)
BalanceRatioGreen = 1.00
BalanceRatioBlue  = 1.18
FlatFieldCorrection = ON    ← FFC 활성화
Expose_us = 8000            ← 최적 노출값
```

**재캘리브레이션이 필요한 경우** (카메라 교체, 조명 변경 등):
MultiCam Studio 또는 별도 초기화 CLI 툴을 통해 처리합니다. 서비스 UI 역할이 아닙니다.

## AutoSave 동작

`AutoSaveService`(`IHostedService`)가 `FrameAcquired` 이벤트를 구독하여 AutoSave 설정이 켜져 있으면 HTTP 요청 없이 자동으로 저장합니다.

```
FrameAcquired 이벤트
  → AutoSave 설정 확인
  → FrameSaveTracker (중복 방지)
  → ImageWriter.Save() → 디스크 ({date}/{profile} 토큰 기반 경로)
  → ThumbnailService.GenerateAsync()
  → ICapturedImageRepository.AddAsync() → DB
```

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Hardware | Crevis TC-A160K (Camera Link) + Grablink Full PC1622 |
| Native Driver | MultiCam.dll (Euresys SDK) |
| Core Library | .NET 10, C# 12, LibraryImport (P/Invoke) |
| REST API | ASP.NET Core (Controllers) |
| Background Service | IHostedService (AutoSaveService) |
| Database | SQLite (EF Core) |
| Frontend | React 19, TypeScript, Vite, TanStack Router, SCSS Modules |
| Testing | xUnit, WebApplicationFactory |
