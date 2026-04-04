# Multi-Camera Actor Architecture Design

**Status:** Approved — pending implementation  
**Date:** 2026-04-04  
**Supersedes:** `2026-04-03-solid-capture-redesign.md` (extends, does not replace)

---

## Goal

현재 단일 카메라 고정 구조(`AcquisitionSession` 싱글톤)를 **N대 카메라 동시 지원** 구조로 전환하고, 싱글톤 공유 상태에서 발생하는 스레드 안전성 버그를 **아키텍처 수준에서 원천 차단**한다.

---

## Background: 현재 구조의 문제

SOLID 리디자인(2026-04-03) 이후에도 다음 싱글톤들이 공유 가변 상태를 가진다:

| 싱글톤 | 문제 |
|--------|------|
| `AcquisitionSession` | HTTP 요청 스레드 + 하드웨어 콜백 스레드 + 백그라운드 서비스가 동시 접근. `_lock`으로 보호하지만 lock 경쟁 상시 존재 |
| `ExposureController` | 같은 싱글톤을 여러 HTTP 요청이 동시 접근. `_lock` 추가로 임시 해결 |
| `FrameSaveTracker` | 여러 HTTP 요청이 동시에 `ShouldSave()` 호출 (Interlocked로 해결했으나 카메라별 추적 불가) |
| `IFrameQueue` | 전역 단일 큐. 멀티카메라 시 프레임 혼용 |
| `FrameWriterBackgroundService` | 단일 큐 소비자. 멀티카메라 불가 |

실제 발생한 버그 사례: 트리거 프레임이 `IFrameQueue`를 통해 백그라운드 서비스에도 전달되어 DB에 이중 저장 → SQLite 잠금 충돌 → 500 에러.

---

## Architecture

### 핵심 원칙

> **카메라 간 공유 상태를 구조적으로 없앤다. 카메라 내부 상태는 단일 루프(actor)만 접근한다.**

### 구성 요소

```
CameraRegistry (singleton)
  ├── CameraActor["cam-1"]
  │     ├── Channel<ICameraCommand>  ← mailbox
  │     ├── Task _loopTask           ← 단일 소비자 루프
  │     └── 상태 필드 (lock 없음)
  │           _channel, _lastFrame, _state,
  │           _desiredExposureUs, _pendingTriggerTcs ...
  ├── CameraActor["cam-2"]
  └── CameraActor["cam-3"]
```

**CameraRegistry** (singleton)
- `ConcurrentDictionary<CameraId, CameraActor>` 로 카메라 목록 관리
- 내부 가변 상태 없음 — 읽기/쓰기 모두 thread-safe 컬렉션에 위임

**CameraActor** (카메라당 1개, DI 외부에서 생성)
- `Channel<ICameraCommand>` mailbox로 모든 외부 접근을 직렬화
- 단일 `Task _loopTask` 가 mailbox를 소비 → 동시 실행 불가
- 상태 필드는 일반 C# 변수 (lock, volatile 불필요)
- 하드웨어 콜백은 `mailbox.TryWrite(new FrameArrivedCmd(image))` 한 줄로 끝

### Actor Loop

```csharp
private async Task RunLoopAsync(CancellationToken ct)
{
    await foreach (var cmd in _mailbox.Reader.ReadAllAsync(ct))
    {
        switch (cmd)
        {
            case StartCmd c:            HandleStart(c);           break;
            case StopCmd c:             HandleStop(c);            break;
            case TriggerCmd c:          HandleTrigger(c);         break;
            case GetLatestFrameCmd c:   HandleGetLatestFrame(c);  break;
            case FrameArrivedCmd c:     HandleFrameArrived(c);    break;
            case AcquisitionErrorCmd c: HandleError(c);           break;
            case GetStatusCmd c:        c.Tcs.SetResult(BuildStatus()); break;
            case GetExposureCmd c:      c.Tcs.SetResult(BuildExposureInfo()); break;
            case SetExposureCmd c:      HandleSetExposure(c);     break;
        }
    }
}
```

### 데이터 흐름

**HTTP 요청 → Actor:**
```
HTTP → CameraController
     → registry.Get(cameraId)
     → actor.TriggerAsync(timeout, ct)
           → new TCS<ImageData>
           → mailbox.TryWrite(new TriggerCmd(tcs, ct))
           → await tcs.Task
     ← ImageData
```

**하드웨어 콜백 → Actor:**
```
MultiCam 드라이버 (콜백 스레드)
  → OnFrameAcquired(image)
      mailbox.TryWrite(new FrameArrivedCmd(image))  ← 즉시 리턴

[actor loop]
  → HandleFrameArrived:
      _lastFrame = image
      if (_pendingTriggerTcs != null)
          _pendingTriggerTcs.SetResult(image)   // HTTP 요청자에게 전달
          return  // triggered frame: 컨트롤러가 저장
      else
          SaveStreamFrameAsync(image)  // 스트림 프레임: 비동기 저장
```

**Snapshot (기존 SnapshotCapture 유지):**
```
POST /api/cameras/{cameraId}/snapshot
  → actor.GetStatusAsync()  // 상태 확인
  → IsActive? → 409
  → SnapshotCapture.CaptureAsync()  // 별도 채널로 독립 처리
```

---

## API 변경

| 현재 | 변경 후 |
|------|---------|
| `POST /api/acquisition/start` | `POST /api/cameras/{cameraId}/start` |
| `POST /api/acquisition/stop` | `POST /api/cameras/{cameraId}/stop` |
| `POST /api/acquisition/trigger` | `POST /api/cameras/{cameraId}/trigger` |
| `GET /api/acquisition/latest-frame` | `GET /api/cameras/{cameraId}/latest-frame` |
| `GET /api/acquisition/status` | `GET /api/cameras/{cameraId}/status` |
| `GET /api/acquisition/exposure` | `GET /api/cameras/{cameraId}/exposure` |
| `PUT /api/acquisition/exposure` | `PUT /api/cameras/{cameraId}/exposure` |
| `POST /api/acquisition/snapshot` | `POST /api/cameras/{cameraId}/snapshot` |
| _(없음)_ | `GET /api/cameras` |

`{cameraId}`: 설정 파일에서 정의한 카메라 식별자 (예: `"cam-1"`, `"top-camera"`)

---

## 제거되는 컴포넌트

| 제거 | 대체 |
|------|------|
| `AcquisitionSession` | `CameraActor` 내부 상태 |
| `IAcquisitionSession` | `ICameraActor` |
| `ExposureController` / `IExposureController` / `IExposureSource` | actor 내부 필드 |
| `FrameSaveTracker` (싱글톤) | actor 내부 `_lastSavedFrame` 필드 |
| `IFrameQueue` / `BoundedFrameQueue` (싱글톤) | actor 내부 per-camera 처리 |
| `FrameWriterBackgroundService` | actor loop 내 `SaveStreamFrameAsync` |
| `AcquisitionController` | `CameraController` |

## 유지되는 컴포넌트

- `IFrameWriter` / `ImageFileWriter` — 무상태, 공유 안전
- `IThumbnailService` — 무상태, 공유 안전
- `IImageSaveSettingsService` — 전체 카메라 공통 설정
- `FrameSavedHandler` — scoped, 변경 없음
- `IAutoSaveService` / `AutoSaveService` — scoped, 변경 없음
- `ISnapshotCapture` / `SnapshotCapture` — 변경 없음
- `GlobalExceptionHandler` — 변경 없음
- DB / Repository 레이어 전체 — 변경 없음

---

## DI 등록 변화

```csharp
// 제거
builder.Services.AddSingleton<AcquisitionSession>();
builder.Services.AddSingleton<IAcquisitionSession>(...);
builder.Services.AddSingleton<IExposureController, ExposureController>();
builder.Services.AddSingleton<FrameSaveTracker>();
builder.Services.AddSingleton<IFrameQueue>(...);
builder.Services.AddHostedService<FrameWriterBackgroundService>();

// 추가
builder.Services.AddSingleton<CameraRegistry>();
// CameraActor는 CameraRegistry가 직접 생성 (DI 컨테이너 밖)
```

싱글톤 수: **10개 → 5개** (남은 5개는 모두 무상태 또는 thread-safe 컬렉션)

---

## 마이그레이션 순서

각 단계가 독립적으로 배포 가능합니다.

```
단계 1: CameraActor + CameraRegistry 신규 구현
         기존 AcquisitionSession 유지 (병행)

단계 2: CameraController 추가
         /api/cameras/{cameraId}/... 신규 엔드포인트 추가
         기존 /api/acquisition/... 병행 유지

단계 3: 기존 AcquisitionController를 CameraActor 위임으로 내부 교체
         URL은 유지, 구현만 교체

단계 4: 레거시 코드 삭제
```

---

## 테스트 전략

**CameraActor 단위 테스트** — WebApplicationFactory 없이 직접 테스트 가능:

```csharp
var actor = new CameraActor("cam-1", mockGrabService, ...);
await actor.StartAsync(profileId);
actor.SimulateFrameArrived(fakeImage);  // 하드웨어 콜백 시뮬레이션
var result = await actor.TriggerAsync(1000, ct);
Assert.Equal(fakeImage, result);
```

**통합 테스트** — URL만 변경:
```csharp
// 기존
await _client.PostJsonAsync("/api/acquisition/start", ...);
// 변경 후
await _client.PostJsonAsync("/api/cameras/cam-1/start", ...);
```

---

## 코드 리뷰 체크리스트 (구현 시 필수)

- [ ] actor loop 외부 코드에서 `_lastFrame`, `_state` 등 상태 필드 직접 접근 없음
- [ ] 하드웨어 콜백은 반드시 `mailbox.TryWrite(...)` 한 줄로만 처리
- [ ] `_loopTask`는 단 하나 (중복 시작 방어 로직 필요)
- [ ] triggered frame은 actor가 저장하지 않음 (컨트롤러가 직접 저장)
- [ ] stream frame은 actor가 비동기 저장 (fire-and-forget Task)
- [ ] `CameraActor.DisposeAsync()`는 mailbox 완료 후 loop 종료 대기

---

## 참고

- Actor 모델 개요: https://en.wikipedia.org/wiki/Actor_model
- `System.Threading.Channels` 공식 문서: https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
- Akka.NET (참고용, 본 프로젝트에는 미사용): https://getakka.net
