# SSE Live Preview Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 현재 `latestFrame` 1초 폴링과 `acquisitionStatus` 폴링을 SSE push로 교체하여 프레임 촬영 즉시 UI가 반응하도록 만든다.

**Architecture:** 백엔드에 `FrameAcquired`/`StatusChanged` 이벤트를 `IAcquisitionService`에 추가하고, `GET /api/acquisition/events` SSE 엔드포인트로 클라이언트에 push한다. 프론트엔드는 `useLiveStream` 훅이 `EventSource`를 관리하며 `previewUrl`과 `isActive`를 반환한다.

**Tech Stack:** ASP.NET Core (.NET 8), `System.Threading.Channels`, React 19, TypeScript, Vitest

---

## File Map

| 파일 | 변경 유형 | 역할 |
|------|----------|------|
| `src/PeanutVision.Api/Services/IAcquisitionService.cs` | 수정 | FrameAcquired, StatusChanged 이벤트 추가 |
| `src/PeanutVision.Api/Services/AcquisitionManager.cs` | 수정 | 이벤트 발생 로직 추가 |
| `src/PeanutVision.Api/Controllers/AcquisitionController.cs` | 수정 | GET /events SSE 엔드포인트, BuildStatusJson 헬퍼 추가 |
| `src/peanut-vision-ui/src/hooks/useLiveStream.ts` | 신규 생성 | EventSource 관리, previewUrl/isActive 반환 |
| `src/peanut-vision-ui/src/hooks/useLiveStream.test.tsx` | 신규 생성 | useLiveStream 유닛 테스트 |
| `src/peanut-vision-ui/src/hooks/useLivePreview.ts` | 삭제 | SSE로 대체 |
| `src/peanut-vision-ui/src/hooks/useLivePreview.test.tsx` | 삭제 | SSE로 대체 |
| `src/peanut-vision-ui/src/hooks/useAcquisitionActions.ts` | 수정 | acquisitionStatus refetchInterval 제거, latestFrame invalidate 제거 |
| `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx` | 수정 | useLivePreview → useLiveStream |

---

## Task 1: IAcquisitionService에 이벤트 추가

**Files:**
- Modify: `src/PeanutVision.Api/Services/IAcquisitionService.cs`
- Modify: `src/PeanutVision.Api/Services/AcquisitionManager.cs`

- [ ] **Step 1: IAcquisitionService에 이벤트 선언 추가**

`src/PeanutVision.Api/Services/IAcquisitionService.cs`를 아래로 교체한다:

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public interface IAcquisitionService : IChannelService, IDisposable
{
    bool IsActive { get; }
    bool HasFrame { get; }
    string? LastError { get; }
    AcquisitionStatisticsSnapshot? GetStatistics();
    IReadOnlyList<ChannelEvent> GetRecentEvents(int max = 50);
    void Start(int? frameCount = null, int? intervalMs = null);
    void Stop();
    Task<ImageData> TriggerAndWaitAsync(int timeoutMs = 5000);
    ImageData Snapshot(ProfileId profileId, TriggerMode? triggerMode = null);
    ImageData? GetLatestFrame();

    /// <summary>Raised on the driver callback thread when a new frame is ready in <see cref="GetLatestFrame"/>.</summary>
    event EventHandler FrameAcquired;

    /// <summary>Raised after acquisition state changes (start, stop, error).</summary>
    event EventHandler StatusChanged;
}
```

- [ ] **Step 2: AcquisitionManager에 이벤트 선언 추가**

`src/PeanutVision.Api/Services/AcquisitionManager.cs`의 클래스 선언 직후 (기존 필드들 다음, `public ChannelState ChannelState` 프로퍼티 전)에 아래를 추가한다:

```csharp
public event EventHandler? FrameAcquired;
public event EventHandler? StatusChanged;
```

`EventHandler?`로 선언한다 (`event EventHandler FrameAcquired`가 아닌 nullable).

- [ ] **Step 3: ProcessFrame에서 FrameAcquired 발생**

`AcquisitionManager.ProcessFrame()` 메서드 끝에 (tcs?.TrySetResult(image) 다음) 아래를 추가한다:

```csharp
FrameAcquired?.Invoke(this, EventArgs.Empty);
```

- [ ] **Step 4: Start/Stop에서 StatusChanged 발생**

`AcquisitionManager.Start()` 메서드의 `lock (_lock)` 블록이 끝난 직후 (블록 닫는 `}` 다음, 메서드 끝 전)에 추가한다:

```csharp
StatusChanged?.Invoke(this, EventArgs.Empty);
```

`AcquisitionManager.Stop()` 메서드의 `tcs?.TrySetCanceled()` 다음에 추가한다:

```csharp
StatusChanged?.Invoke(this, EventArgs.Empty);
```

- [ ] **Step 5: 빌드 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
dotnet build src/PeanutVision.Api/PeanutVision.Api.csproj
```

Expected: Build succeeded. 에러 없음.

- [ ] **Step 6: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/PeanutVision.Api/Services/IAcquisitionService.cs src/PeanutVision.Api/Services/AcquisitionManager.cs
git commit -m "feat: add FrameAcquired and StatusChanged events to IAcquisitionService"
```

---

## Task 2: AcquisitionController에 SSE 엔드포인트 추가

**Files:**
- Modify: `src/PeanutVision.Api/Controllers/AcquisitionController.cs`

- [ ] **Step 1: using 추가**

파일 최상단 using 목록에 아래 두 줄을 추가한다:

```csharp
using System.Text.Json;
using System.Threading.Channels;
```

- [ ] **Step 2: BuildStatusJson 헬퍼 메서드 추가**

`AcquisitionController`의 `SaveAndRecordAsync` private 메서드 바로 위에 아래 메서드를 추가한다:

```csharp
private string BuildStatusJson()
{
    var stats = _acquisition.GetStatistics();
    var payload = new
    {
        isActive = _acquisition.IsActive,
        channelState = _acquisition.ChannelState.ToString().ToLowerInvariant(),
        profileId = _acquisition.ActiveProfileId?.Value,
        hasFrame = _acquisition.HasFrame,
        lastError = _acquisition.LastError,
        allowedActions = _acquisition.GetAllowedActions()
            .Select(a => a.ToString().ToLowerInvariant()).ToArray(),
        statistics = stats.HasValue
            ? (object)new
            {
                frameCount = stats.Value.FrameCount,
                droppedFrameCount = stats.Value.DroppedFrameCount,
                errorCount = stats.Value.ErrorCount,
                elapsedMs = stats.Value.ElapsedTime.TotalMilliseconds,
                averageFps = Math.Round(stats.Value.AverageFps, 2),
                minFrameIntervalMs = Math.Round(stats.Value.MinFrameIntervalMs, 2),
                maxFrameIntervalMs = Math.Round(stats.Value.MaxFrameIntervalMs, 2),
                averageFrameIntervalMs = Math.Round(stats.Value.AverageFrameIntervalMs, 2),
                copyDropCount = stats.Value.CopyDropCount,
                clusterUnavailableCount = stats.Value.ClusterUnavailableCount,
            }
            : null,
        recentEvents = _acquisition.GetRecentEvents(50).Select(e => new
        {
            timestamp = e.Timestamp,
            type = e.Type.ToString(),
            message = e.Message,
        }),
    };
    return JsonSerializer.Serialize(payload);
}
```

- [ ] **Step 3: GetEvents SSE 엔드포인트 추가**

`AcquisitionController`에 `GetStatus()` 메서드 바로 다음에 아래 메서드를 추가한다:

```csharp
[HttpGet("events")]
public async Task GetEvents(CancellationToken ct)
{
    Response.ContentType = "text/event-stream; charset=utf-8";
    Response.Headers["Cache-Control"] = "no-cache";
    Response.Headers["X-Accel-Buffering"] = "no";

    var channel = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions { SingleReader = true });

    void OnFrameAcquired(object? _, EventArgs __) =>
        channel.Writer.TryWrite("event: frame_ready\ndata: {}\n\n");

    void OnStatusChanged(object? _, EventArgs __) =>
        channel.Writer.TryWrite($"event: status_changed\ndata: {BuildStatusJson()}\n\n");

    _acquisition.FrameAcquired += OnFrameAcquired;
    _acquisition.StatusChanged += OnStatusChanged;

    // Send current status immediately so client has initial state
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
```

- [ ] **Step 4: 빌드 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
dotnet build src/PeanutVision.Api/PeanutVision.Api.csproj
```

Expected: Build succeeded.

- [ ] **Step 5: 수동 SSE 테스트**

앱을 실행하고 curl로 SSE 스트림을 확인한다:

```bash
curl -N http://localhost:5000/api/acquisition/events
```

Expected: `event: status_changed\ndata: {...}\n\n` 형태의 응답이 즉시 출력됨. Ctrl+C로 종료.

- [ ] **Step 6: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/PeanutVision.Api/Controllers/AcquisitionController.cs
git commit -m "feat: add GET /api/acquisition/events SSE endpoint"
```

---

## Task 3: useLiveStream 훅 생성 (Frontend)

**Files:**
- Create: `src/peanut-vision-ui/src/hooks/useLiveStream.ts`
- Create: `src/peanut-vision-ui/src/hooks/useLiveStream.test.tsx`

- [ ] **Step 1: 테스트 파일 작성**

`src/peanut-vision-ui/src/hooks/useLiveStream.test.tsx`를 아래 내용으로 생성한다:

```tsx
import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createElement } from "react";
import type { ReactNode } from "react";
import { useLiveStream } from "./useLiveStream";
import type { AcquisitionStatus } from "../api/types";

// Mock EventSource
const mockEs = {
  addEventListener: vi.fn(),
  close: vi.fn(),
};
vi.stubGlobal("EventSource", vi.fn(() => mockEs));

vi.mock("../constants", () => ({
  API_BASE_URL: "http://localhost:5000/api",
}));

function wrapper({ children }: { children: ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return createElement(QueryClientProvider, { client: queryClient }, children);
}

describe("useLiveStream", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockEs.addEventListener.mockReset();
    mockEs.close.mockReset();
  });

  it("creates EventSource with correct URL", () => {
    renderHook(() => useLiveStream(), { wrapper });
    expect(EventSource).toHaveBeenCalledWith(
      "http://localhost:5000/api/acquisition/events"
    );
  });

  it("closes EventSource on unmount", () => {
    const { unmount } = renderHook(() => useLiveStream(), { wrapper });
    unmount();
    expect(mockEs.close).toHaveBeenCalled();
  });

  it("previewUrl is null initially", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });
    expect(result.current.previewUrl).toBeNull();
  });

  it("isActive is false initially", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });
    expect(result.current.isActive).toBe(false);
  });

  it("updates previewUrl when frame_ready event is received", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });

    const frameReadyCall = mockEs.addEventListener.mock.calls.find(
      ([event]: [string]) => event === "frame_ready"
    );
    expect(frameReadyCall).toBeDefined();
    const frameReadyHandler = frameReadyCall![1] as () => void;

    act(() => {
      frameReadyHandler();
    });

    expect(result.current.previewUrl).toMatch(/\/acquisition\/latest-frame\?_t=\d+/);
  });

  it("updates isActive when status_changed event is received", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });

    const statusCall = mockEs.addEventListener.mock.calls.find(
      ([event]: [string]) => event === "status_changed"
    );
    expect(statusCall).toBeDefined();
    const statusHandler = statusCall![1] as (e: { data: string }) => void;

    const status: Partial<AcquisitionStatus> = { isActive: true };
    act(() => {
      statusHandler({ data: JSON.stringify(status) });
    });

    expect(result.current.isActive).toBe(true);
  });
});
```

- [ ] **Step 2: 테스트 실행하여 실패 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx vitest run src/hooks/useLiveStream.test.tsx
```

Expected: `Cannot find module './useLiveStream'` 오류로 실패.

- [ ] **Step 3: useLiveStream 훅 구현**

`src/peanut-vision-ui/src/hooks/useLiveStream.ts`를 아래 내용으로 생성한다:

```typescript
import { useEffect, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../api/queryKeys";
import { API_BASE_URL } from "../constants";
import type { AcquisitionStatus } from "../api/types";

export function useLiveStream() {
  const queryClient = useQueryClient();
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [isActive, setIsActive] = useState(false);

  useEffect(() => {
    const es = new EventSource(`${API_BASE_URL}/acquisition/events`);

    es.addEventListener("frame_ready", () => {
      setPreviewUrl(`${API_BASE_URL}/acquisition/latest-frame?_t=${Date.now()}`);
    });

    es.addEventListener("status_changed", (e: MessageEvent) => {
      const status = JSON.parse(e.data) as AcquisitionStatus;
      queryClient.setQueryData(queryKeys.acquisitionStatus, status);
      setIsActive(status.isActive);
    });

    return () => es.close();
  }, [queryClient]);

  return { previewUrl, isActive };
}
```

- [ ] **Step 4: 테스트 실행하여 통과 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx vitest run src/hooks/useLiveStream.test.tsx
```

Expected: 6 tests pass.

- [ ] **Step 5: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/peanut-vision-ui/src/hooks/useLiveStream.ts src/peanut-vision-ui/src/hooks/useLiveStream.test.tsx
git commit -m "feat: add useLiveStream hook for SSE-based live preview"
```

---

## Task 4: useAcquisitionActions에서 폴링 제거

**Files:**
- Modify: `src/peanut-vision-ui/src/hooks/useAcquisitionActions.ts`

- [ ] **Step 1: acquisitionStatus 쿼리에서 refetchInterval 제거**

현재 코드 (L68-73):
```typescript
const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
  queryKey: queryKeys.acquisitionStatus,
  queryFn: getAcquisitionStatus,
  refetchInterval: (query) =>
    query.state.data?.isActive ? POLL_INTERVAL_ACTIVE_MS : POLL_INTERVAL_IDLE_MS,
});
```

아래로 교체한다:
```typescript
const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
  queryKey: queryKeys.acquisitionStatus,
  queryFn: getAcquisitionStatus,
});
```

- [ ] **Step 2: triggerMutation에서 latestFrame invalidate 제거**

`triggerMutation.onSuccess` (현재 L108-113)에서 `queryClient.invalidateQueries({ queryKey: queryKeys.latestFrame })` 한 줄을 삭제한다.

변경 전:
```typescript
onSuccess: () => {
  queryClient.invalidateQueries({ queryKey: queryKeys.latestFrame });
  invalidateStatus();
  toast("프레임이 촬영되었습니다", "success");
},
```

변경 후:
```typescript
onSuccess: () => {
  invalidateStatus();
  toast("프레임이 촬영되었습니다", "success");
},
```

- [ ] **Step 3: snapshotMutation에서 latestFrame invalidate 제거**

`snapshotMutation.onSuccess` (현재 L118-123)에서 `queryClient.invalidateQueries({ queryKey: queryKeys.latestFrame })` 한 줄을 삭제한다.

변경 전:
```typescript
onSuccess: () => {
  queryClient.invalidateQueries({ queryKey: queryKeys.latestFrame });
  invalidateStatus();
  toast("스냅샷이 촬영되었습니다", "success");
},
```

변경 후:
```typescript
onSuccess: () => {
  invalidateStatus();
  toast("스냅샷이 촬영되었습니다", "success");
},
```

- [ ] **Step 4: 사용하지 않는 상수 import 정리**

파일 상단 import에서 `POLL_INTERVAL_ACTIVE_MS`, `POLL_INTERVAL_IDLE_MS`가 더 이상 사용되지 않으므로 제거한다:

```typescript
// 변경 전
import { DEFAULT_CONTINUOUS_INTERVAL_MS, POLL_INTERVAL_ACTIVE_MS, POLL_INTERVAL_IDLE_MS } from "../constants";

// 변경 후
import { DEFAULT_CONTINUOUS_INTERVAL_MS } from "../constants";
```

- [ ] **Step 5: TypeScript 컴파일 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx tsc --noEmit
```

Expected: 에러 없음.

- [ ] **Step 6: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/peanut-vision-ui/src/hooks/useAcquisitionActions.ts
git commit -m "refactor: remove acquisitionStatus polling and latestFrame invalidation (SSE handles both)"
```

---

## Task 5: AcquisitionTab에서 useLivePreview → useLiveStream 교체

**Files:**
- Modify: `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx`

- [ ] **Step 1: import 교체**

파일 상단에서:
```typescript
import { useLivePreview } from "../hooks/useLivePreview";
```
를:
```typescript
import { useLiveStream } from "../hooks/useLiveStream";
```
로 교체한다.

- [ ] **Step 2: 훅 호출 교체**

현재 코드:
```typescript
const live = useLivePreview(acq.acquisitionStatus);
```

아래로 교체한다:
```typescript
const live = useLiveStream();
```

- [ ] **Step 3: TypeScript 컴파일 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx tsc --noEmit
```

Expected: 에러 없음.

- [ ] **Step 4: 전체 테스트 실행**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx vitest run
```

Expected: 모든 테스트 pass.

- [ ] **Step 5: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx
git commit -m "refactor: replace useLivePreview with useLiveStream in AcquisitionTab"
```

---

## Task 6: useLivePreview 파일 삭제 및 상수 정리

**Files:**
- Delete: `src/peanut-vision-ui/src/hooks/useLivePreview.ts`
- Delete: `src/peanut-vision-ui/src/hooks/useLivePreview.test.tsx`
- Modify: `src/peanut-vision-ui/src/constants.ts`

- [ ] **Step 1: useLivePreview 파일 삭제**

```bash
rm /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui/src/hooks/useLivePreview.ts
rm /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui/src/hooks/useLivePreview.test.tsx
```

- [ ] **Step 2: 사용하지 않는 상수 제거**

`src/peanut-vision-ui/src/constants.ts`에서 `LIVE_PREVIEW_POLL_INTERVAL_MS` 한 줄을 삭제한다.

삭제 대상:
```typescript
export const LIVE_PREVIEW_POLL_INTERVAL_MS = 1000;
```

`POLL_INTERVAL_ACTIVE_MS`, `POLL_INTERVAL_IDLE_MS`도 더 이상 사용하지 않으면 삭제한다. 먼저 grep으로 확인:
```bash
grep -r "POLL_INTERVAL_ACTIVE_MS\|POLL_INTERVAL_IDLE_MS\|LIVE_PREVIEW_POLL_INTERVAL_MS" /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui/src --include="*.ts" --include="*.tsx"
```

사용하는 파일이 없으면 `constants.ts`에서 해당 줄들을 삭제한다.

- [ ] **Step 3: TypeScript 컴파일 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx tsc --noEmit
```

Expected: 에러 없음.

- [ ] **Step 4: 전체 테스트 실행**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx vitest run
```

Expected: 모든 테스트 pass.

- [ ] **Step 5: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add -A
git commit -m "chore: delete useLivePreview hook and remove unused polling constants"
```

---

## Verification

앱 실행 후 확인:

```bash
# 백엔드
cd /Users/sonjiho/Workspace/peanut-factory
dotnet run --project src/PeanutVision.Api/PeanutVision.Api.csproj

# 프론트엔드 (별도 터미널)
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npm run dev
```

1. 브라우저 개발자 도구 Network 탭 → `events` 요청이 `text/event-stream` 타입으로 열려 있음 확인
2. 연속 촬영 시작 → 중앙 뷰어에 즉시 이미지 표시 (1초 지연 없음)
3. 촬영 시작/중지 버튼 클릭 → UI 상태 즉시 반영
4. 브라우저 탭 닫기 후 서버 로그에 연결 해제 로그 없는지 확인 (에러 없이 정리됨)
5. Network 탭에서 `acquisitionStatus` 반복 GET 요청이 사라졌는지 확인
