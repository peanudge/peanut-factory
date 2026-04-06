# Gallery Tab Design

**Date:** 2026-04-05  
**Status:** Approved

## Goal

현재 `AcquisitionTab` 우 패널에 내장된 갤러리 기능을 독립적인 최상위 탭(Gallery)으로 분리한다. 촬영 UI와 갤러리 탐색 UI가 완전히 분리되어 각자의 역할에 집중할 수 있다.

---

## Architecture

### 변경 전

```
App
├── System 탭
├── Acquisition 탭
│   ├── 좌 사이드바 (촬영 제어)
│   ├── 중앙 (라이브 뷰어)
│   └── 우 패널 (갤러리 + 선택 이미지 뷰어)  ← 제거 대상
└── Latency 탭
```

### 변경 후

```
App
├── System 탭
├── Acquisition 탭
│   ├── 좌 사이드바 (촬영 제어)
│   └── 중앙 (라이브 뷰어)  ← 전체 너비 사용
├── Gallery 탭  ← 신규
│   ├── 좌: 선택 이미지 뷰어 (전체 높이)
│   └── 우: 갤러리 그리드 + 세션 필터 + 페이지네이션
└── Latency 탭
```

---

## Detailed Design

### `App.tsx` 변경

탭 목록에 "Gallery" 추가 (Acquisition과 Latency 사이):

```tsx
<Tab label="System" />
<Tab label="Acquisition" />
<Tab label="Gallery" />    {/* 신규, index 2 */}
<Tab label="Latency" />    {/* 기존 index 2 → 3 */}
```

탭 인덱스 변경에 따라 기존 `tab === 2`(Latency) → `tab === 3`으로 조정.

`GalleryTab` 렌더링 추가:
```tsx
<Box sx={{ flexGrow: 1, overflow: "hidden", display: tab === 2 ? "flex" : "none" }}>
  <GalleryTab />
</Box>
```

### `GalleryTab.tsx` (신규)

**위치:** `src/peanut-vision-ui/src/tabs/GalleryTab.tsx`

**레이아웃:**
```
[좌 패널: 선택 이미지 뷰어 (전체 높이, width 480px)]  |  [우 패널: 갤러리 그리드]
```

**사용 훅:** `useImageGallery()` — 기존 훅 그대로 재사용

**사용 컴포넌트:**
- `ImageViewer` — 좌 패널, 선택된 이미지 표시
- `ImageGallery` — 우 패널, 썸네일 그리드 + 세션 필터 (`filterSessionId` prop)

> `SessionSelector`는 저장 세션 선택용이므로 GalleryTab에 포함하지 않는다. 세션 필터링은 `ImageGallery`의 `filterSessionId`/`onFilterChange` prop으로 처리한다.

**구현 스케치:**
```tsx
export default function GalleryTab() {
  const gallery = useImageGallery();

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      {/* 좌: 선택 이미지 뷰어 */}
      <Box sx={{ width: 480, flexShrink: 0, p: 2, borderRight: "1px solid", borderColor: "divider" }}>
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null}
          onClose={() => gallery.setSelectedId(null)}
        />
      </Box>

      {/* 우: 갤러리 */}
      <Box sx={{ flexGrow: 1, display: "flex", flexDirection: "column", overflow: "hidden", p: 2 }}>
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

### `AcquisitionTab.tsx` 변경

- 우 패널 전체(`RIGHT PANEL DRAG HANDLE` + `RIGHT PANEL` Box) 삭제
- `useImageGallery` import 및 훅 호출 삭제
- `rightPanelRef`, `onRightResizerMouseDown` 관련 `useResizablePanel` 호출 삭제
- `CollapsiblePanel`, `ImageGallery`, `EventLog` import 삭제 (우 패널에서만 사용)
- 중앙 뷰어가 전체 너비를 차지하게 됨

---

## 파일 변경 목록

| 파일 | 변경 유형 |
|------|----------|
| `src/peanut-vision-ui/src/App.tsx` | Gallery 탭 추가, Latency 탭 인덱스 조정 |
| `src/peanut-vision-ui/src/tabs/GalleryTab.tsx` | 신규 생성 |
| `src/peanut-vision-ui/src/tabs/AcquisitionTab.tsx` | 우 패널 제거, useImageGallery 제거 |

---

## Verification

1. Gallery 탭 클릭 → 갤러리 그리드 표시
2. 갤러리 썸네일 클릭 → 좌 패널에 이미지 표시
3. 세션 필터 변경 → 해당 세션 이미지만 표시
4. 이미지 삭제 → 갤러리 즉시 갱신
5. Acquisition 탭 → 우 패널 없이 중앙 뷰어가 전체 너비 사용
6. 연속 촬영 중 Gallery 탭으로 이동 → 갤러리 5초 폴링으로 신규 이미지 자동 추가
