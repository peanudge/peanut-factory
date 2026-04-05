# SSE Live Preview Design

**Date:** 2026-04-05  
**Status:** Approved

## Goal

현재 1초 폴링 방식의 `latestFrame` 조회와 `acquisitionStatus` 폴링을 Server-Sent Events(SSE) 기반 push로 교체하여 최대 반응성을 확보한다.

---

## Architecture

### 변경 전

```
Frontend                          Backend
--------                          -------
latestFrame polling (1s)  →  GET /acquisition/latest-frame
acquisitionStatus polling  →  GET /acquisition/status
```

### 변경 후

```
Frontend                          Backend
--------                          -------
EventSource                  ←  SSE /acquisition/events
  frame_ready event         →   GET /acquisition/latest-frame (image only)
  status_changed event           (no additional request needed)
```

---

## Backend Design

### SSE Endpoint

`GET /api/acquisition/events`

- Response: `Content-Type: text/event-stream`
- 연결 유지 (long-lived HTTP connection)
- 클라이언트 연결/해제는 `CancellationToken`으로 감지

### 이벤트 형식

**frame_ready** — `FrameAcquired` 이벤트 발생 시 전송:
```
event: frame_ready
data: {"timestamp":"2026-04-05T12:00:00.123Z"}

```

**status_changed** — acquisitionStatus 변경 시 전송 (start/stop/trigger/snapshot 후):
```
event: status_changed
data: {"isActive":true,"channelState":"active","hasFrame":true,"statistics":{...},"recentEvents":[...]}

```

### 구현 방식

`AcquisitionManager`에 SSE 클라이언트 컬렉션을 관리:
- `ConcurrentDictionary<string, ChannelWriter<SseEvent>>` (clientId → channel)
- `FrameAcquired` 이벤트 핸들러에서 모든 클라이언트에 `frame_ready` 전송
- `StartAcquisition`, `StopAcquisition`, `TriggerAndCapture`, `Snapshot` 호출 후 `status_changed` 전송
- 클라이언트 해제 시(`CancellationToken` 취소) 컬렉션에서 제거

`AcquisitionController`에 SSE 엔드포인트 추가:
- `Response.Headers["Cache-Control"] = "no-cache"` 등 SSE 필수 헤더 설정
- `Channel<SseEvent>` 생성 후 `AcquisitionManager`에 등록
- `CancellationToken` 취소까지 채널 읽으며 `Response.Body`에 write

---

## Frontend Design

### `useLiveStream` 훅 (신규)

**위치:** `src/peanut-vision-ui/src/hooks/useLiveStream.ts`

**책임:**
- `EventSource` 연결 관리 (항상 열림, 앱 마운트 시부터)
- `frame_ready` → previewUrl 업데이트 (이미지 자체는 타임스탬프 쿼리파라미터로 캐시 무효화)
- `status_changed` → `queryClient.setQueryData(queryKeys.acquisitionStatus, ...)` 직접 업데이트

**반환값:**
```typescript
{
  previewUrl: string | null;
  isActive: boolean;
}
```

**구현 스케치:**
```typescript
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

    // EventSource는 에러 시 자동 재연결. 별도 처리 불필요.

    return () => es.close();
  }, [queryClient]);

  return { previewUrl, isActive };
}
```

### `useAcquisitionActions` 변경

- `acquisitionStatus` useQuery의 `refetchInterval` 제거 (SSE가 push하므로 polling 불필요)
- 쿼리 자체는 초기 로드용으로 유지 (SSE 연결 전 초기 상태 조회)
- `triggerMutation`, `snapshotMutation`의 `queryClient.invalidateQueries(latestFrame)` 제거 (SSE `frame_ready`가 대신 처리)

### `AcquisitionTab` 변경

- `useLivePreview` 제거
- `useLiveStream` 사용
- `live.previewUrl`, `live.isActive` 연결

---

## 파일 변경 목록

| 파일 | 변경 유형 |
|------|----------|
| `src/PeanutVision.Api/Controllers/AcquisitionController.cs` | SSE 엔드포인트 추가 |
| `src/PeanutVision.Api/Services/AcquisitionManager.cs` | SSE 클라이언트 관리, push 로직 추가 |
| `src/peanut-vision-ui/src/hooks/useLiveStream.ts` | 신규 생성 |
| `src/peanut-vision-ui/src/hooks/useLiveStream.test.tsx` | 신규 생성 |
| `src/peanut-vision-ui/src/hooks/useLivePreview.ts` | 삭제 |
| `src/peanut-vision-ui/src/hooks/useLivePreview.test.tsx` | 삭제 |
| `src/peanut-vision-ui/src/hooks/useAcquisitionActions.ts` | acquisitionStatus polling 제거, latestFrame invalidate 제거 |
| `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx` | useLivePreview → useLiveStream |

---

## Verification

1. 연속 촬영 시작 → 프레임 촬영 즉시 중앙 뷰어 업데이트 (SSE push)
2. 촬영 시작/중지 → UI 상태 즉시 반영 (polling 지연 없음)
3. 네트워크 일시 끊김 후 재연결 → EventSource 자동 재연결, 정상 동작 재개
4. 브라우저 탭 닫기 → 서버 SSE 연결 정리 확인 (로그)
