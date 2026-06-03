# peanut-vision-app 포팅 설계

**날짜:** 2026-06-03  
**범위:** peanut-vision-ui → peanut-vision-app 기능 포팅 및 MUI → SCSS 모듈 마이그레이션

---

## 배경

`peanut-vision-ui`는 MUI 기반 탭 내비게이션(App.tsx 단일 파일)으로 구성된 기존 프로젝트다.
`peanut-vision-app`은 TanStack Router 파일기반 라우팅과 SCSS 모듈을 사용하는 신규 프로젝트로, 라우팅 방식과 설계 구조를 개선하기 위해 별도로 생성되었다.

---

## 1. 라우팅 아키텍처

기존 peanut-vision-app의 splat 라우트(`/$`) + `routeMap.tsx` 패턴을 유지한다.

### routeMap 구조

```
root
├── system-state  → SystemState
├── acquisition   → AcquisitionPage
├── gallery       → GalleryPage
└── latency       → LatencyPage
```

- `gnbRootList`가 `root.children`을 순회하므로 GNB는 자동으로 4개 메뉴를 렌더링한다.
- 모든 페이지는 `/$` splat 라우트를 통해 렌더링된다.
- `__root.tsx`에 `QueryClientProvider`를 추가해 React Query를 앱 전체에 공급한다.

### React Query 설정

`__root.tsx`에서 `QueryClient`를 생성하고 `QueryClientProvider`로 감싼다. 기본 `staleTime`은 0, `refetchOnWindowFocus`는 false로 설정한다.

---

## 2. 의존성 추가

`package.json`에 다음 패키지를 추가한다:

| 패키지 | 버전 | 용도 |
|--------|------|------|
| `@tanstack/react-query` | latest | 서버 상태 관리 |
| `recharts` | latest | Latency 차트 |
| `file-saver` | latest | Gallery 이미지 다운로드 |
| `jszip` | latest | Gallery 일괄 다운로드 |
| `@types/file-saver` | latest | 타입 정의 |

---

## 3. 파일 구조

```
src/
├── api/
│   ├── client.ts          # peanut-vision-ui에서 복사 (API_BASE_URL import 수정)
│   ├── types.ts           # 그대로 복사
│   └── queryKeys.ts       # 그대로 복사
├── components/
│   ├── Gnb.tsx            # 기존 유지
│   ├── SystemState/
│   │   ├── index.tsx      # placeholder → 완전 구현 (Board/Camera 테이블)
│   │   ├── index.module.scss
│   │   └── cx.ts
│   ├── Acquisition/
│   │   ├── index.tsx      # AcquisitionTab 포팅
│   │   ├── index.module.scss
│   │   └── cx.ts
│   ├── Gallery/
│   │   ├── index.tsx      # GalleryTab 포팅
│   │   ├── index.module.scss
│   │   └── cx.ts
│   ├── Latency/
│   │   ├── index.tsx      # LatencyTab 포팅
│   │   ├── index.module.scss
│   │   └── cx.ts
│   └── shared/            # 여러 페이지에서 공유되는 서브컴포넌트
│       ├── StatusChip/
│       ├── ImageViewer/
│       ├── ImageGallery/
│       ├── BoardRow/
│       ├── AcquisitionControls/
│       ├── ExposureControl/
│       ├── CalibrationActions/
│       ├── PresetSelector/
│       ├── SessionSelector/
│       ├── ImageSaveSettingsPanel/
│       └── ContinuousSettings/
├── hooks/
│   ├── useAcquisitionActions.ts  # 그대로 복사
│   ├── useImageGallery.ts        # 그대로 복사
│   ├── useLiveStream.ts          # 그대로 복사
│   └── useResizablePanel.ts      # 그대로 복사
├── utils/
│   └── formatTimestamp.ts        # 그대로 복사
├── constants.ts                  # API_BASE_URL 정의
└── routeMap.tsx                  # 4개 라우트로 확장
```

각 컴포넌트 폴더는 `index.tsx + index.module.scss + cx.ts` 패턴을 따른다.

---

## 4. MUI → SCSS 변환 전략

MUI 컴포넌트를 plain HTML 요소 + SCSS 모듈로 교체한다. `sx={{ ... }}` 인라인 스타일은 module.scss 클래스로 이동한다.

### 컴포넌트 매핑

| MUI 컴포넌트 | 대체 |
|-------------|------|
| `Box` | `div` (레이아웃 클래스) |
| `Typography` | `h1`–`h6`, `p`, `span` |
| `Button` | `<button>` |
| `Chip` | `<span class="chip">` |
| `Table/TableRow/TableCell` | `<table>/<tr>/<td>` |
| `CircularProgress` | CSS 키프레임 스피너 |
| `Alert` | `<div class="alert alert--error">` |
| `Paper/Card/CardContent` | `<div class="card">` |
| `Tabs/Tab` | 커스텀 탭 컴포넌트 (CSS 클래스 기반) |
| MUI Icons | lucide-react (이미 설치됨) |
| `Grid` | CSS grid 또는 flex |

### SCSS 설계 원칙

- 컴포넌트별 `.module.scss` 파일에 스타일 격리
- `classnames/bind` 패턴(`cx`)으로 조건부 클래스 적용
- 전역 CSS 변수는 `src/styles/index.scss`에서 정의
- 색상·간격·타이포그래피는 CSS 변수로 관리

---

## 5. 포팅 범위

### 포팅 대상 (4개 탭에서 사용되는 컴포넌트)

**페이지 컴포넌트**
- SystemState (기존 placeholder → 완전 구현)
- AcquisitionPage
- GalleryPage
- LatencyPage

**공유 서브컴포넌트**
- StatusChip
- BoardRow
- ImageViewer
- ImageGallery
- AcquisitionControls (+ 내부: AcquisitionModeSelector, ChannelStateIndicator, CameraProfileSelector, AcquisitionStats, SidebarSection, StatLine)
- ExposureControl
- CalibrationActions
- PresetSelector
- SessionSelector
- ImageSaveSettingsPanel
- ContinuousSettings

**API / 훅 / 유틸**
- `api/client.ts`, `api/types.ts`, `api/queryKeys.ts`
- `useAcquisitionActions`, `useImageGallery`, `useLiveStream`, `useResizablePanel`
- `utils/formatTimestamp.ts`
- `constants.ts`

### 포팅 제외

- `EventLog` — 4개 탭에서 미사용
- `ToastContainer` / `ToastContext` — 4개 탭에서 미사용
- `AcquisitionActionBar` — AcquisitionTab에서 미사용
- `HistogramChart` — 어디서도 import되지 않음, 미사용 확인

---

## 6. 구현 순서

1. 의존성 설치 (`npm install`)
2. `constants.ts` 작성
3. `api/` 폴더 복사·수정
4. `hooks/`, `utils/` 복사
5. `__root.tsx`에 `QueryClientProvider` 추가
6. `routeMap.tsx` 확장 (4개 라우트)
7. 공유 서브컴포넌트 포팅 (bottom-up: leaf 컴포넌트 먼저)
8. 페이지 컴포넌트 포팅 (SystemState → Acquisition → Gallery → Latency)
9. 동작 확인 (`npm run dev`)

---

## 7. 완료 기준

- 4개 페이지가 GNB에서 네비게이션되어 렌더링됨
- API 연결 (board/camera 조회, 캡처, 갤러리, 레이턴시) 동작
- MUI import 없음
- TypeScript 빌드 에러 없음 (`npm run build`)
