# Gallery Tab Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** `AcquisitionTab` 우 패널의 갤러리를 독립적인 최상위 "Gallery" 탭으로 분리한다. 촬영 UI는 라이브 뷰어에만 집중하고, 갤러리 탐색은 전용 탭에서 이루어진다.

**Architecture:** `GalleryTab.tsx`를 신규 생성하여 `useImageGallery` 훅과 기존 `ImageViewer`/`ImageGallery` 컴포넌트를 재사용한다. `App.tsx`에 탭을 추가하고, `AcquisitionTab.tsx`에서 우 패널 전체를 제거한다.

**Tech Stack:** React 19, TypeScript, MUI

---

## File Map

| 파일 | 변경 유형 | 역할 |
|------|----------|------|
| `src/peanut-vision-ui/src/tabs/GalleryTab.tsx` | 신규 생성 | Gallery 전용 탭 컴포넌트 |
| `src/peanut-vision-ui/src/App.tsx` | 수정 | Gallery 탭 추가, Latency 인덱스 조정 |
| `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx` | 수정 | 우 패널 + useImageGallery 제거 |

---

## Task 1: GalleryTab.tsx 생성

**Files:**
- Create: `src/peanut-vision-ui/src/tabs/GalleryTab.tsx`

- [ ] **Step 1: GalleryTab.tsx 생성**

`src/peanut-vision-ui/src/tabs/GalleryTab.tsx`를 아래 내용으로 생성한다:

```tsx
import Box from "@mui/material/Box";
import ImageViewer from "../components/ImageViewer";
import ImageGallery from "../components/ImageGallery";
import { useImageGallery } from "../hooks/useImageGallery";

export default function GalleryTab() {
  const gallery = useImageGallery();

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      {/* 좌: 선택 이미지 뷰어 */}
      <Box
        sx={{
          width: 480,
          flexShrink: 0,
          borderRight: "1px solid",
          borderColor: "divider",
          p: 2,
          display: "flex",
          flexDirection: "column",
        }}
      >
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={
            gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null
          }
          onClose={() => gallery.setSelectedId(null)}
        />
      </Box>

      {/* 우: 갤러리 그리드 */}
      <Box sx={{ flexGrow: 1, overflow: "hidden", p: 2 }}>
        <ImageGallery
          images={gallery.images}
          selectedId={gallery.selectedId}
          onSelect={gallery.setSelectedId}
          onDelete={gallery.handleDelete}
          page={gallery.page}
          totalPages={gallery.totalPages}
          onPageChange={gallery.setPage}
          filterSessionId={gallery.filterSessionId}
          onFilterChange={gallery.setFilterSessionId}
          isLoading={gallery.isLoading}
        />
      </Box>
    </Box>
  );
}
```

- [ ] **Step 2: TypeScript 컴파일 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx tsc --noEmit
```

Expected: 에러 없음. `ImageViewer`의 `onClose` prop이 없으면 타입 오류가 발생하는지 확인 — 없으면 `onClose` prop 이름이 맞는지 `src/peanut-vision-ui/src/components/ImageViewer.tsx`에서 확인한다.

- [ ] **Step 3: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/peanut-vision-ui/src/tabs/GalleryTab.tsx
git commit -m "feat: add GalleryTab component"
```

---

## Task 2: App.tsx에 Gallery 탭 추가

**Files:**
- Modify: `src/peanut-vision-ui/src/App.tsx`

현재 `App.tsx`에는 System(0), Acquisition(1), Latency(2) 탭이 있다. Gallery(2)를 Acquisition과 Latency 사이에 삽입하고, Latency가 인덱스 3으로 밀린다.

- [ ] **Step 1: GalleryTab import 추가**

파일 상단 import 목록에 아래를 추가한다:
```typescript
import GalleryTab from "./tabs/GalleryTab";
```

- [ ] **Step 2: Tabs에 Gallery 탭 추가**

현재 코드:
```tsx
<Tab label="System" />
<Tab label="Acquisition" />
<Tab label="Latency" />
```

아래로 교체한다:
```tsx
<Tab label="System" />
<Tab label="Acquisition" />
<Tab label="Gallery" />
<Tab label="Latency" />
```

- [ ] **Step 3: GalleryTab 렌더링 추가 및 Latency 인덱스 조정**

현재 코드:
```tsx
<Container maxWidth="lg" sx={{ py: 3, flexGrow: 1, display: tab === 0 ? undefined : "none" }}>
  <SystemTab />
</Container>
<Box sx={{ flexGrow: 1, overflow: "hidden", display: tab === 1 ? "flex" : "none" }}>
  <AcquisitionTab onSessionChange={setSessionName} />
</Box>
<Box sx={{ flexGrow: 1, overflow: "auto", display: tab === 2 ? "block" : "none" }}>
  <LatencyTab />
</Box>
```

아래로 교체한다:
```tsx
<Container maxWidth="lg" sx={{ py: 3, flexGrow: 1, display: tab === 0 ? undefined : "none" }}>
  <SystemTab />
</Container>
<Box sx={{ flexGrow: 1, overflow: "hidden", display: tab === 1 ? "flex" : "none" }}>
  <AcquisitionTab onSessionChange={setSessionName} />
</Box>
<Box sx={{ flexGrow: 1, overflow: "hidden", display: tab === 2 ? "flex" : "none" }}>
  <GalleryTab />
</Box>
<Box sx={{ flexGrow: 1, overflow: "auto", display: tab === 3 ? "block" : "none" }}>
  <LatencyTab />
</Box>
```

- [ ] **Step 4: TypeScript 컴파일 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx tsc --noEmit
```

Expected: 에러 없음.

- [ ] **Step 5: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/peanut-vision-ui/src/App.tsx
git commit -m "feat: add Gallery top-level tab"
```

---

## Task 3: AcquisitionTab에서 우 패널 제거

**Files:**
- Modify: `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx`

현재 `AcquisitionTab.tsx`에서 제거할 항목:
- `useImageGallery` import 및 훅 호출 (`const gallery = useImageGallery()`)
- `rightPanelRef`, `onRightResizerMouseDown` 관련 `useResizablePanel` 호출
- `CollapsiblePanel`, `ImageGallery`, `EventLog` import
- `RIGHT PANEL DRAG HANDLE` Box 전체
- `RIGHT PANEL` Box 전체 (CollapsiblePanel "Captures" + CollapsiblePanel "Event Log" 포함)

- [ ] **Step 1: import 정리**

파일 상단에서 아래 import들을 삭제한다:
```typescript
import EventLog from "../components/EventLog";
import ImageGallery from "../components/ImageGallery";
import CollapsiblePanel from "../components/CollapsiblePanel";
import { useImageGallery } from "../hooks/useImageGallery";
```

- [ ] **Step 2: gallery 훅 호출 제거**

다음 줄을 삭제한다:
```typescript
const gallery = useImageGallery();
```

- [ ] **Step 3: rightPanel useResizablePanel 호출 제거**

아래 코드를 삭제한다:
```typescript
const { panelRef: rightPanelRef, onResizerMouseDown: onRightResizerMouseDown } = useResizablePanel({
  defaultWidth: 280,
  min: 200,
  max: 560,
});
```

> `useResizablePanel` import 자체는 `sidebarRef`에서 여전히 사용하므로 삭제하지 않는다.

- [ ] **Step 4: RIGHT PANEL DRAG HANDLE 제거**

아래 Box 전체를 삭제한다 (주석 포함):
```tsx
{/* RIGHT PANEL DRAG HANDLE */}
<Box
  onMouseDown={onRightResizerMouseDown}
  sx={{
    width: 4,
    flexShrink: 0,
    cursor: "col-resize",
    bgcolor: "divider",
    transition: "background-color 0.15s",
    "&:hover": { bgcolor: "primary.main" },
  }}
/>
```

- [ ] **Step 5: RIGHT PANEL Box 전체 제거**

아래 Box 전체를 삭제한다 (주석 포함):
```tsx
{/* RIGHT PANEL */}
<Box
  ref={rightPanelRef}
  sx={{
    width: 280,
    flexShrink: 0,
    display: "flex",
    flexDirection: "column",
    overflow: "hidden",
  }}
>
  <CollapsiblePanel label="Captures" count={gallery.totalCount} defaultOpen={true}>
    {gallery.selectedImageUrl && (
      <Box sx={{ mb: 1, maxHeight: 240, overflow: "hidden" }}>
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null}
          onClose={() => gallery.setSelectedId(null)}
        />
      </Box>
    )}
    <ImageGallery
      images={gallery.images}
      selectedId={gallery.selectedId}
      onSelect={gallery.setSelectedId}
      onDelete={gallery.handleDelete}
      page={gallery.page}
      totalPages={gallery.totalPages}
      onPageChange={gallery.setPage}
      filterSessionId={gallery.filterSessionId}
      onFilterChange={gallery.setFilterSessionId}
      isLoading={gallery.isLoading}
    />
  </CollapsiblePanel>
  <CollapsiblePanel label="Event Log" defaultOpen={false}>
    <EventLog events={acq.acquisitionStatus?.recentEvents} />
  </CollapsiblePanel>
</Box>
```

- [ ] **Step 6: TypeScript 컴파일 확인**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx tsc --noEmit
```

Expected: 에러 없음. 에러가 있으면 메시지를 읽고 해당 참조를 추가로 제거한다.

- [ ] **Step 7: 전체 테스트 실행**

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npx vitest run
```

Expected: 모든 테스트 pass.

- [ ] **Step 8: 커밋**

```bash
cd /Users/sonjiho/Workspace/peanut-factory
git add src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx
git commit -m "refactor: remove gallery right panel from AcquisitionTab"
```

---

## Verification

앱 실행 후 확인:

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/peanut-vision-ui
npm run dev
```

1. 상단 탭에 System / Acquisition / **Gallery** / Latency 표시 확인
2. **Gallery 탭** → 좌 패널(선택 이미지 뷰어) + 우 패널(갤러리 그리드) 표시
3. 갤러리 썸네일 클릭 → 좌 패널에 해당 이미지 표시
4. 세션 필터 변경 → 해당 세션 이미지만 표시
5. 이미지 삭제 → 즉시 갱신
6. **Acquisition 탭** → 우 패널 없이 중앙 뷰어가 전체 너비 사용
7. 연속 촬영 중 Gallery 탭 이동 → 5초 폴링으로 신규 이미지 자동 추가
