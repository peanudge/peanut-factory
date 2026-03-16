# PRD-005: 콜백 스레드 이미지 복사 오프로드

| 항목 | 내용 |
|------|------|
| **Issue** | [#5 - Large image copy on MultiCam callback thread exceeds 1ms limit](https://github.com/sonjiho/peanut-factory/issues/5) |
| **상태** | Draft |
| **우선순위** | HIGH |
| **레이블** | performance, architecture |
| **작성일** | 2026-03-17 |

---

## 1. 문제 정의

MultiCam 드라이버가 `MC_SIG_SURFACE_PROCESSING` 시그널을 발생시키면, 네이티브 콜백 스레드에서 `AcquisitionManager.OnFrameAcquired()` 이벤트 핸들러가 동기적으로 실행된다. 이 핸들러는 `ImageData.FromSurface()` → `SurfaceData.ToArray()`를 호출하여 **전체 Surface 버퍼(~39MB)**를 복사한다.

**CLAUDE.md Development Rule #6:** 콜백 핸들러는 1ms 미만으로 완료해야 하며, `ConcurrentQueue`를 사용하여 데이터를 전달해야 한다.

39MB 메모리 복사는 1ms를 크게 초과하며, 다음 문제를 유발할 수 있다:

- **프레임 드롭**: 콜백 스레드 지연으로 다음 프레임 처리 불가
- **`MC_SIG_UNRECOVERABLE_OVERRUN`**: DMA 버퍼가 제때 반환되지 않아 발생
- **`MC_SIG_CLUSTER_UNAVAILABLE`**: 모든 Surface가 PROCESSING 상태에 고착

---

## 2. 현재 구조 분석

### 2.1 콜백 실행 경로

```
[MultiCam 드라이버 스레드]
  GrabChannel.OnNativeCallback()           ← 네이티브 콜백 진입
    → GrabChannel.ProcessSurfaceSignal()
      → GetSurfaceData(surfaceHandle)      ← Surface 메타데이터만 읽음 (빠름)
      → FrameAcquired?.Invoke(...)         ← 이벤트 구독자 동기 호출
        → AcquisitionManager.OnFrameAcquired()
          → ImageData.FromSurface()        ← ⚠ 39MB 복사 (느림)
            → SurfaceData.ToArray()
      → ReleaseSurface(surfaceHandle)      ← 복사 완료까지 Surface 점유
```

### 2.2 핵심 코드

**`AcquisitionManager.cs:179-193`** — 콜백 스레드에서 39MB 복사:
```csharp
private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
{
    var image = ImageData.FromSurface(e.Surface);  // ← 39MB copy on callback thread
    TaskCompletionSource<ImageData>? tcs;

    lock (_lock)
    {
        _lastFrame = image;
        _statistics?.RecordFrame();
        tcs = _triggerTcs;
        _triggerTcs = null;
    }

    tcs?.TrySetResult(image);
}
```

**`GrabChannel.cs:236-255`** — Surface는 이벤트 핸들러 완료 후에야 해제:
```csharp
private void ProcessSurfaceSignal(ref McSignalInfo info)
{
    uint surfaceHandle = info.SignalInfo;
    try
    {
        var surface = GetSurfaceData(surfaceHandle);
        FrameAcquired?.Invoke(this, new FrameAcquiredEventArgs(
            surface, _channelHandle, McSignal.MC_SIG_SURFACE_PROCESSING));
    }
    finally
    {
        ReleaseSurface(surfaceHandle);  // ← 복사 끝날 때까지 대기
    }
}
```

### 2.3 Surface 라이프사이클 영향

현재 `ProcessSurfaceSignal`의 `finally` 블록에서 `ReleaseSurface`를 호출하므로, 이벤트 핸들러(`OnFrameAcquired`)의 39MB 복사가 완료될 때까지 Surface가 PROCESSING 상태로 점유된다. `SurfaceCount=4`일 때, 연속 프레임 수신 시 모든 Surface가 빠르게 소진될 수 있다.

---

## 3. 솔루션 설계

### 3.1 선택: Option A — ConcurrentQueue 기반 지연 복사

SDK 가이드라인과 정확히 일치하는 Producer-Consumer 패턴을 적용한다.

### 3.2 아키텍처

```
[MultiCam 드라이버 스레드]                    [전용 처리 스레드]
  OnNativeCallback()                          ProcessingLoop()
    → ProcessSurfaceSignal()                    ← BlockingCollection.Take()
      → GetSurfaceData() (메타데이터)             → ImageData.FromSurface() (39MB 복사)
      → _frameQueue.Add(workItem)  ──enqueue──→  → ReleaseSurface()
      → return (< 1ms)                           → _lastFrame = image
                                                  → tcs?.TrySetResult(image)
```

### 3.3 변경 사항

#### 3.3.1 `GrabChannel.cs` — Surface 해제 책임 이전

현재 `ProcessSurfaceSignal`의 `finally` 블록에서 자동으로 `ReleaseSurface`를 호출하는 구조를 변경한다. 콜백 모드에서는 이벤트 구독자가 Surface 해제 책임을 가지도록 한다.

- `FrameAcquiredEventArgs`에 `SurfaceHandle` 필드 추가 (이미 `SurfaceData.SurfaceHandle`로 존재)
- `ProcessSurfaceSignal`에서 콜백 모드일 때 `finally`에서 `ReleaseSurface`를 호출하지 않음
- `ReleaseSurface(SurfaceData)` public 메서드는 이미 존재하므로 추가 작업 불필요

#### 3.3.2 `AcquisitionManager.cs` — Producer-Consumer 패턴 도입

```csharp
// 새로 추가되는 필드
private BlockingCollection<FrameWorkItem>? _frameQueue;
private Thread? _processingThread;

// WorkItem 구조체
private readonly record struct FrameWorkItem(
    SurfaceData Surface,
    GrabChannel Channel);
```

**콜백 핸들러 (Producer):**
```csharp
private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
{
    // 복사 없이 메타데이터만 큐에 넣기 (< 1ms)
    _frameQueue?.TryAdd(new FrameWorkItem(e.Surface, _channel!));
}
```

**처리 스레드 (Consumer):**
```csharp
private void ProcessingLoop()
{
    foreach (var item in _frameQueue!.GetConsumingEnumerable())
    {
        try
        {
            var image = ImageData.FromSurface(item.Surface);  // 39MB 복사 (별도 스레드)

            TaskCompletionSource<ImageData>? tcs;
            lock (_lock)
            {
                _lastFrame = image;
                _statistics?.RecordFrame();
                tcs = _triggerTcs;
                _triggerTcs = null;
            }

            tcs?.TrySetResult(image);
        }
        finally
        {
            item.Channel.ReleaseSurface(item.Surface);  // Surface 반환
        }
    }
}
```

#### 3.3.3 라이프사이클 관리

- `Start()`: `BlockingCollection` 생성 + 처리 스레드 시작
- `Stop()`: `BlockingCollection.CompleteAdding()` → 처리 스레드 Join → 잔여 Surface 전부 해제
- `Dispose()`: `Stop()` 호출로 안전한 정리

---

## 4. 수정 대상 파일

| 파일 | 변경 내용 |
|------|-----------|
| `src/PeanutVision.MultiCamDriver/GrabChannel.cs` | `ProcessSurfaceSignal`에서 콜백 모드 시 자동 Surface 해제 제거 |
| `src/PeanutVision.Api/Services/AcquisitionManager.cs` | `BlockingCollection` + 전용 처리 스레드 도입, `OnFrameAcquired`를 경량 Producer로 전환 |

---

## 5. 리스크 및 완화

| 리스크 | 영향 | 완화 방안 |
|--------|------|-----------|
| 처리 스레드가 복사보다 느려 큐가 무한 증가 | 메모리 고갈 | `BlockingCollection`에 `boundedCapacity` 설정 (예: SurfaceCount와 동일). 큐 가득 차면 `TryAdd` 실패 → 해당 프레임 드롭 + Surface 즉시 해제 |
| Surface 해제 누락 | Surface 고갈 → `CLUSTER_UNAVAILABLE` | `finally` 블록에서 반드시 `ReleaseSurface` 호출. `Stop()` 시 큐 잔여 항목 전부 해제 |
| 처리 스레드 예외 | 이후 프레임 처리 중단 | `foreach` 루프 내부에서 개별 try-catch. 예외 시 해당 프레임만 드롭하고 계속 처리 |
| 기존 `Snapshot()` 동기 경로 영향 | 없음 | `Snapshot()`은 `WaitForFrame` (폴링) 방식을 사용하며 콜백을 사용하지 않으므로 영향 없음 |

---

## 6. 수용 기준

- [ ] 콜백 핸들러가 1ms 이내에 완료 (버퍼 복사 없음)
- [ ] Surface 데이터는 전용 처리 스레드에서 복사
- [ ] 복사 완료 후 Surface가 MultiCam에 반환 (`FREE` 상태)
- [ ] `MC_SIG_UNRECOVERABLE_OVERRUN` / `MC_SIG_CLUSTER_UNAVAILABLE` 시그널 증가 없음
- [ ] 프레임 처리 처리량이 기존 구현 이상
- [ ] `Stop()` 호출 시 큐 잔여 Surface 전부 안전하게 해제
- [ ] 기존 `Snapshot()` 동기 경로 동작에 영향 없음

---

## 7. 미채택 대안

### Option B: 사전 할당 Pinned 버퍼 + Unsafe.CopyBlock

콜백 스레드에서의 복사 시간을 줄이지만, 39MB 복사 자체는 여전히 콜백 스레드에서 실행된다. 1ms 제한을 충족할 가능성이 낮다.

### Option C: Zero-Copy Surface Pinning

Surface 버퍼를 `Memory<byte>`로 감싸 복사 자체를 회피한다. 가장 빠르지만, Surface가 소비자 처리 완료까지 점유되어 Surface 고갈 위험이 높고, 라이프타임 관리가 복잡하다.
