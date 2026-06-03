# peanut-vision-app 포팅 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** peanut-vision-ui의 기능(System/Acquisition/Gallery/Latency)을 peanut-vision-app으로 포팅하면서 MUI → SCSS 모듈로 마이그레이션

**Architecture:** routeMap + splat 라우트(`/$`) 패턴 유지. 각 탭이 routeMap의 child route가 됨. React Query를 `__root.tsx`에서 제공.

**Tech Stack:** React 19, TanStack Router, TanStack React Query, SCSS Modules, classnames/bind, lucide-react, recharts

**Spec:** `docs/superpowers/specs/2026-06-03-peanut-vision-port-design.md`

---

## 페이즈 개요

| 페이즈 | 범위 | 완료 기준 |
|--------|------|-----------|
| **P1** | 기반 설정 (deps, API, hooks, providers, routeMap) | `npm run dev` — GNB에 4개 메뉴 표시, 각 페이지 빈 플레이스홀더 렌더링 |
| **P2** | System 페이지 | Board/Camera 테이블 조회 및 렌더링 |
| **P3** | Acquisition 페이지 | 촬영 컨트롤, 라이브 스트림, Exposure/Calibration/Preset/Session |
| **P4** | Gallery 페이지 | 이미지 그리드 + 뷰어 |
| **P5** | Latency 페이지 | 레이턴시 차트 + 테이블 |

각 페이즈는 이전 페이즈 완료 후 실행. 페이즈 시작 전 해당 페이즈 세부 계획 확인.

---

## Phase 1: 기반 설정

**목표:** 앱이 빌드되고, GNB에 4개 메뉴가 나타나며, 각 페이지에 플레이스홀더가 렌더링됨.

### 파일 변경 목록

| 작업 | 파일 |
|------|------|
| 수정 | `package.json` |
| 생성 | `src/constants.ts` |
| 생성 | `src/api/types.ts` |
| 생성 | `src/api/client.ts` |
| 생성 | `src/api/queryKeys.ts` |
| 생성 | `src/utils/formatTimestamp.ts` |
| 생성 | `src/hooks/useResizablePanel.ts` |
| 생성 | `src/contexts/ToastContext.tsx` |
| 생성 | `src/contexts/ToastContainer.tsx` |
| 수정 | `src/routes/__root.tsx` |
| 생성 | `src/components/Acquisition/index.tsx` + `cx.ts` + `index.module.scss` |
| 생성 | `src/components/Gallery/index.tsx` + `cx.ts` + `index.module.scss` |
| 생성 | `src/components/Latency/index.tsx` + `cx.ts` + `index.module.scss` |
| 수정 | `src/routeMap.tsx` |

### Task 1-1: 의존성 설치

- [ ] `package.json`의 `dependencies`에 추가:
  ```json
  "@tanstack/react-query": "latest",
  "recharts": "latest",
  "file-saver": "^2.0.5",
  "jszip": "^3.10.1"
  ```
  `devDependencies`에 추가:
  ```json
  "@types/file-saver": "latest"
  ```
- [ ] 패키지 설치: `npm install`
- [ ] 빌드 확인: `npm run build` (에러 없어야 함)
- [ ] 커밋:
  ```bash
  git add package.json package-lock.json
  git commit -m "feat: add react-query, recharts, file-saver, jszip dependencies"
  ```

### Task 1-2: 상수 및 API 레이어

- [ ] `src/constants.ts` 생성:
  ```ts
  export const API_BASE_URL = 'http://localhost:5000/api'
  export const POLL_INTERVAL_MS = 3000
  export const REFRESH_THROTTLE_MS = 3000
  export const DEFAULT_EXPOSURE_MIN = 100
  export const DEFAULT_EXPOSURE_MAX = 10000
  export const DEFAULT_CONTINUOUS_INTERVAL_MS = 1000
  export const GALLERY_POLL_INTERVAL_MS = 5000
  ```
- [ ] `src/api/types.ts` 생성 — peanut-vision-ui의 `src/api/types.ts` 내용 전체 복사 (수정 없음)
- [ ] `src/api/queryKeys.ts` 생성 — peanut-vision-ui의 `src/api/queryKeys.ts` 전체 복사 (수정 없음)
- [ ] `src/api/client.ts` 생성 — peanut-vision-ui의 `src/api/client.ts` 복사 후 import 경로만 수정:
  ```ts
  // 변경 전: import { API_BASE_URL } from "../constants";
  // 변경 후:
  import { API_BASE_URL } from '@/constants'
  import type { ... } from './types'
  ```
- [ ] `src/utils/formatTimestamp.ts` 생성 — peanut-vision-ui의 `src/utils/formatTimestamp.ts` 전체 복사
- [ ] `src/hooks/useResizablePanel.ts` 생성 — peanut-vision-ui의 `src/hooks/useResizablePanel.ts` 전체 복사
- [ ] 타입 체크: `npx tsc --noEmit`
- [ ] 커밋:
  ```bash
  git add src/constants.ts src/api/ src/utils/ src/hooks/useResizablePanel.ts
  git commit -m "feat: add api layer, constants, utils, useResizablePanel"
  ```

### Task 1-3: Toast 시스템

- [ ] `src/contexts/ToastContext.tsx` 생성:
  ```tsx
  import {
    createContext, useCallback, useContext, useState, type ReactNode,
  } from 'react'

  type ToastSeverity = 'success' | 'info' | 'warning' | 'error'

  export interface Toast {
    id: string
    message: string
    severity: ToastSeverity
    duration: number | null
  }

  interface ToastContextValue {
    toasts: Toast[]
    toast: (message: string, severity?: ToastSeverity, duration?: number | null) => void
    dismiss: (id: string) => void
  }

  const DEFAULT_DURATION: Record<ToastSeverity, number | null> = {
    success: 3000, info: 3000, warning: 5000, error: null,
  }

  const ToastContext = createContext<ToastContextValue | null>(null)

  export function ToastProvider({ children }: { children: ReactNode }) {
    const [toasts, setToasts] = useState<Toast[]>([])

    const dismiss = useCallback((id: string) => {
      setToasts((prev) => prev.filter((t) => t.id !== id))
    }, [])

    const toast = useCallback(
      (message: string, severity: ToastSeverity = 'info', duration?: number | null) => {
        const id = crypto.randomUUID()
        const resolved = duration !== undefined ? duration : DEFAULT_DURATION[severity]
        setToasts((prev) => {
          const next = [...prev, { id, message, severity, duration: resolved }]
          return next.length > 5 ? next.slice(next.length - 5) : next
        })
        if (resolved !== null && resolved > 0) {
          setTimeout(() => dismiss(id), resolved)
        }
      },
      [dismiss],
    )

    return (
      <ToastContext.Provider value={{ toasts, toast, dismiss }}>
        {children}
      </ToastContext.Provider>
    )
  }

  export function useToast(): ToastContextValue {
    const ctx = useContext(ToastContext)
    if (!ctx) throw new Error('useToast must be used inside <ToastProvider>')
    return ctx
  }
  ```
- [ ] `src/contexts/ToastContainer.tsx` 생성:
  ```tsx
  import { useToast } from './ToastContext'
  import styles from './ToastContainer.module.scss'
  import classNames from 'classnames/bind'
  const cx = classNames.bind(styles)

  export function ToastContainer() {
    const { toasts, dismiss } = useToast()
    return (
      <div className={cx('ToastContainer')}>
        {toasts.map((t) => (
          <div key={t.id} className={cx('toast', t.severity)}>
            <span>{t.message}</span>
            <button onClick={() => dismiss(t.id)}>✕</button>
          </div>
        ))}
      </div>
    )
  }
  ```
- [ ] `src/contexts/ToastContainer.module.scss` 생성:
  ```scss
  .ToastContainer {
    position: fixed;
    bottom: 24px;
    right: 24px;
    z-index: 9999;
    display: flex;
    flex-direction: column;
    gap: 8px;
    width: 360px;
    max-width: calc(100vw - 48px);
    pointer-events: none;
  }
  .toast {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 10px 14px;
    border-radius: 6px;
    font-size: 0.85rem;
    pointer-events: all;
    color: #fff;
    button {
      background: none;
      border: none;
      color: inherit;
      cursor: pointer;
      padding: 0 0 0 12px;
      font-size: 1rem;
      opacity: 0.7;
      &:hover { opacity: 1; }
    }
    &.success { background: #2e7d32; }
    &.error   { background: #c62828; }
    &.warning { background: #e65100; }
    &.info    { background: #1565c0; }
  }
  ```
- [ ] 커밋:
  ```bash
  git add src/contexts/
  git commit -m "feat: add toast context and container"
  ```

### Task 1-4: QueryClient Provider 설정

- [ ] `src/routes/__root.tsx` 수정:
  ```tsx
  import { Outlet, createRootRoute } from '@tanstack/react-router'
  import { TanStackRouterDevtoolsPanel } from '@tanstack/react-router-devtools'
  import { TanStackDevtools } from '@tanstack/react-devtools'
  import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
  import { Gnb } from '@/components/Gnb'
  import { ToastProvider } from '@/contexts/ToastContext'
  import { ToastContainer } from '@/contexts/ToastContainer'

  const queryClient = new QueryClient({
    defaultOptions: { queries: { refetchOnWindowFocus: false } },
  })

  export const Route = createRootRoute({ component: RootComponent })

  function RootComponent() {
    return (
      <QueryClientProvider client={queryClient}>
        <ToastProvider>
          <Gnb />
          <main>
            <Outlet />
          </main>
          <ToastContainer />
          <TanStackDevtools
            config={{ position: 'bottom-right' }}
            plugins={[{ name: 'TanStack Router', render: <TanStackRouterDevtoolsPanel /> }]}
          />
        </ToastProvider>
      </QueryClientProvider>
    )
  }
  ```
- [ ] 타입 체크: `npx tsc --noEmit`
- [ ] 커밋:
  ```bash
  git add src/routes/__root.tsx
  git commit -m "feat: add QueryClientProvider and ToastProvider to root"
  ```

### Task 1-5: 플레이스홀더 페이지 및 routeMap 확장

- [ ] `src/components/Acquisition/cx.ts` 생성 (SystemState/cx.ts 동일 패턴):
  ```ts
  import classNames from 'classnames/bind'
  import style from './index.module.scss'
  const cx = classNames.bind(style)
  export default cx
  ```
- [ ] `src/components/Acquisition/index.module.scss` 생성:
  ```scss
  .Acquisition { padding: 20px; }
  ```
- [ ] `src/components/Acquisition/index.tsx` 생성:
  ```tsx
  import cx from './cx'
  const Acquisition = () => <div className={cx('Acquisition')}><h2>Acquisition</h2></div>
  export default Acquisition
  ```
- [ ] Gallery, Latency도 동일 패턴으로 생성 (파일명/클래스명만 변경)
- [ ] `src/routeMap.tsx` 수정:
  ```tsx
  import SystemState from './components/SystemState'
  import Acquisition from './components/Acquisition'
  import Gallery from './components/Gallery'
  import Latency from './components/Latency'

  const _routerMap = {
    root:         { name: 'root', children: ['system-state', 'acquisition', 'gallery', 'latency'] },
    'system-state': { name: '시스템 상태', Component: SystemState },
    'acquisition':  { name: '촬영', Component: Acquisition },
    'gallery':      { name: '갤러리', Component: Gallery },
    'latency':      { name: '레이턴시', Component: Latency },
  }

  export type RoutePath = keyof typeof _routerMap
  type BaseRoute = { name: string; link?: RoutePath }
  export type ParentRoute = BaseRoute & { children: RoutePath[] }
  export type ChildRoute = BaseRoute & { Component: React.ComponentType }
  export type Route = ChildRoute | ParentRoute
  export const routeMap = _routerMap as Record<RoutePath, Route>
  export const isParentRoute = (route: Route): route is ParentRoute => 'children' in route
  export const gnbRootList: [RoutePath, Route][] = (routeMap.root as ParentRoute).children.map(
    (r) => [r, routeMap[r]]
  )
  ```
- [ ] `npm run dev` 실행 → GNB에 4개 메뉴 확인, 각 페이지 플레이스홀더 확인
- [ ] `npm run build` — 에러 없어야 함
- [ ] 커밋:
  ```bash
  git add src/components/Acquisition/ src/components/Gallery/ src/components/Latency/ src/routeMap.tsx
  git commit -m "feat(p1): placeholder pages and extend routeMap with 4 routes"
  ```

**Phase 1 완료 기준:** `npm run dev` 실행 후 GNB에 시스템 상태/촬영/갤러리/레이턴시 메뉴가 표시되고 각 페이지에서 제목이 렌더링됨.

---

## Phase 2: System 페이지

**목표:** Board/Camera 테이블이 API에서 데이터를 불러와 렌더링됨.

### 파일 변경 목록

| 작업 | 파일 |
|------|------|
| 생성 | `src/components/shared/StatusChip/index.tsx` + `cx.ts` + `index.module.scss` |
| 생성 | `src/components/shared/BoardRow/index.tsx` + `cx.ts` + `index.module.scss` |
| 수정 | `src/components/SystemState/index.tsx` |
| 수정 | `src/components/SystemState/index.module.scss` |

### Task 2-1: StatusChip

- [ ] `src/components/shared/StatusChip/cx.ts` 생성 (표준 cx.ts 패턴)
- [ ] `src/components/shared/StatusChip/index.module.scss` 생성:
  ```scss
  .chip {
    display: inline-block;
    padding: 2px 8px;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 600;
    border: 1px solid currentColor;
    &.success { color: #4caf50; }
    &.warning { color: #ff9800; }
    &.error   { color: #f44336; }
    &.default { color: #888; }
  }
  ```
- [ ] `src/components/shared/StatusChip/index.tsx` 생성:
  ```tsx
  import cx from './cx'
  interface Props {
    active: boolean
    label?: string
    hasWarnings?: boolean
    hasErrors?: boolean
  }
  export default function StatusChip({ active, label, hasWarnings, hasErrors }: Props) {
    const color = hasErrors ? 'error' : hasWarnings ? 'warning' : active ? 'success' : 'default'
    return (
      <span className={cx('chip', color)}>
        {label ?? (active ? 'Active' : 'Inactive')}
      </span>
    )
  }
  ```

### Task 2-2: BoardRow

- [ ] `src/components/shared/BoardRow/index.module.scss` 생성:
  ```scss
  .toggle { background: none; border: none; cursor: pointer; padding: 4px; color: inherit; }
  .detail { padding: 12px 16px; background: #1a1a1a; }
  .detailTable { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
  .detailTable td { padding: 4px 8px; }
  .detailTable td:first-child { font-weight: 500; width: 220px; color: #aaa; }
  .spinner {
    display: inline-block; width: 16px; height: 16px; border: 2px solid #444;
    border-top-color: #58a; border-radius: 50%; animation: spin 0.8s linear infinite;
  }
  @keyframes spin { to { transform: rotate(360deg); } }
  ```
- [ ] `src/components/shared/BoardRow/index.tsx` 생성:
  ```tsx
  import { useState } from 'react'
  import { useQuery } from '@tanstack/react-query'
  import { ChevronDown, ChevronUp } from 'lucide-react'
  import type { BoardInfo } from '@/api/types'
  import { getBoardStatus } from '@/api/client'
  import { queryKeys } from '@/api/queryKeys'
  import cx from './cx'

  export default function BoardRow({ board }: { board: BoardInfo }) {
    const [open, setOpen] = useState(false)
    const { data: status, isFetching } = useQuery({
      queryKey: queryKeys.boardStatus(board.index),
      queryFn: () => getBoardStatus(board.index),
      enabled: open,
      staleTime: Infinity,
    })
    const rows: [string, string | number][] = status ? [
      ['Input Connector', status.inputConnector],
      ['Input State', status.inputState],
      ['Signal Strength', status.signalStrength],
      ['Output State', status.outputState],
      ['Camera Link', status.cameraLinkStatus],
      ['Sync Errors', status.syncErrors],
      ['Clock Errors', status.clockErrors],
      ['Grabber Errors', status.grabberErrors],
      ['Frame Trigger Violations', status.frameTriggerViolations],
      ['Line Trigger Violations', status.lineTriggerViolations],
      ['PCIe Link', status.pcieLinkInfo],
    ] : []
    return (
      <>
        <tr onClick={() => setOpen((o) => !o)} style={{ cursor: 'pointer' }}>
          <td><button className={cx('toggle')}>{open ? <ChevronUp size={14}/> : <ChevronDown size={14}/>}</button></td>
          <td>{board.index}</td>
          <td>{board.boardName}</td>
          <td>{board.boardType}</td>
          <td>{board.serialNumber}</td>
          <td>{board.pciPosition}</td>
        </tr>
        {open && (
          <tr>
            <td colSpan={6} className={cx('detail')}>
              {isFetching && <span className={cx('spinner')} />}
              {status && (
                <table className={cx('detailTable')}>
                  <tbody>{rows.map(([label, value]) => (
                    <tr key={label}><td>{label}</td><td>{String(value)}</td></tr>
                  ))}</tbody>
                </table>
              )}
            </td>
          </tr>
        )}
      </>
    )
  }
  ```

### Task 2-3: SystemState 페이지 완성

- [ ] `src/components/SystemState/index.module.scss` 수정:
  ```scss
  .SystemState { padding: 20px; display: flex; flex-direction: column; gap: 32px; }
  .section h3 { margin: 0 0 12px; font-size: 1rem; }
  .tableWrap { border: 1px solid #444; border-radius: 6px; overflow: hidden; }
  table { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
  th, td { padding: 8px 12px; text-align: left; border-bottom: 1px solid #333; }
  th { background: #2a2a2a; font-weight: 600; color: #aaa; }
  tr:last-child td { border-bottom: none; }
  tr:hover td { background: #1e1e1e; }
  .mono { font-family: monospace; font-size: 0.8rem; }
  .loading { padding: 32px; text-align: center; }
  .spinner {
    display: inline-block; width: 24px; height: 24px; border: 3px solid #444;
    border-top-color: #58a; border-radius: 50%; animation: spin 0.8s linear infinite;
  }
  @keyframes spin { to { transform: rotate(360deg); } }
  .error { color: #f44336; padding: 16px; background: #2a1010; border-radius: 6px; }
  ```
- [ ] `src/components/SystemState/index.tsx` 수정:
  ```tsx
  import { useQuery } from '@tanstack/react-query'
  import { getBoards, getCameras } from '@/api/client'
  import { queryKeys } from '@/api/queryKeys'
  import StatusChip from '@/components/shared/StatusChip'
  import BoardRow from '@/components/shared/BoardRow'
  import cx from './cx'

  const SystemState = () => {
    const { data: boards, isLoading: boardsLoading, error: boardsError } = useQuery({
      queryKey: queryKeys.boards, queryFn: getBoards,
    })
    const { data: cameras, isLoading: camsLoading, error: camsError } = useQuery({
      queryKey: queryKeys.cameras, queryFn: getCameras,
    })
    const loading = boardsLoading || camsLoading
    const error = boardsError ?? camsError
    if (loading) return <div className={cx('loading')}><span className={cx('spinner')} /></div>
    if (error) return <div className={cx('error')}>{error instanceof Error ? error.message : 'Failed to load'}</div>
    return (
      <div className={cx('SystemState')}>
        <div className={cx('section')}>
          <h3>Frame Grabber Boards</h3>
          <div className={cx('tableWrap')}>
            <table>
              <thead><tr><th/><th>#</th><th>Name</th><th>Type</th><th>Serial</th><th>PCI Position</th></tr></thead>
              <tbody>
                {!boards?.length
                  ? <tr><td colSpan={6} style={{ textAlign: 'center', padding: 16 }}>No boards detected</td></tr>
                  : boards.map((b) => <BoardRow key={b.index} board={b} />)
                }
              </tbody>
            </table>
          </div>
        </div>
        <div className={cx('section')}>
          <h3>Camera Files</h3>
          <div className={cx('tableWrap')}>
            <table>
              <thead><tr><th>File Name</th><th>Manufacturer</th><th>Model</th><th>Format</th><th>Resolution</th><th>Trigger</th></tr></thead>
              <tbody>
                {!cameras?.length
                  ? <tr><td colSpan={6} style={{ textAlign: 'center', padding: 16 }}>No camera files found</td></tr>
                  : cameras.map((c) => (
                    <tr key={c.fileName}>
                      <td className={cx('mono')}>{c.fileName}</td>
                      <td>{c.manufacturer}</td>
                      <td>{c.cameraModel}</td>
                      <td>{c.colorFormat}</td>
                      <td>{c.width}×{c.height}</td>
                      <td><StatusChip active={c.trigMode === 'IMMEDIATE'} label={c.trigMode} /></td>
                    </tr>
                  ))
                }
              </tbody>
            </table>
          </div>
        </div>
      </div>
    )
  }
  export default SystemState
  ```
- [ ] `npm run dev` → System 페이지에서 Board/Camera 테이블 확인
- [ ] `npm run build`
- [ ] 커밋:
  ```bash
  git add src/components/shared/ src/components/SystemState/
  git commit -m "feat(p2): implement System page with Board and Camera tables"
  ```

**Phase 2 완료 기준:** `/system-state` 에서 Board/Camera 데이터가 테이블에 표시됨 (API 연결 시).

---

## Phase 3: Acquisition 페이지

**목표:** 촬영 컨트롤 패널 + 라이브 이미지 뷰어가 동작함.

### 파일 변경 목록

| 작업 | 파일 |
|------|------|
| 생성 | `src/hooks/useAcquisitionActions.ts` |
| 생성 | `src/hooks/useLiveStream.ts` |
| 생성 | `src/components/shared/CameraProfileSelector/` |
| 생성 | `src/components/shared/AcquisitionModeSelector/` |
| 생성 | `src/components/shared/AcquisitionActionBar/` |
| 생성 | `src/components/shared/ExposureControl/` |
| 생성 | `src/components/shared/CalibrationActions/` |
| 생성 | `src/components/shared/PresetSelector/` |
| 생성 | `src/components/shared/SessionSelector/` |
| 생성 | `src/components/shared/ContinuousSettings/` |
| 생성 | `src/components/shared/ImageSaveSettingsPanel/` |
| 생성 | `src/components/shared/ImageViewer/` |
| 수정 | `src/components/Acquisition/index.tsx` + `index.module.scss` |

### Task 3-1: 훅 포팅

- [ ] `src/hooks/useLiveStream.ts` 생성 — peanut-vision-ui의 `hooks/useLiveStream.ts` 복사 후 import 경로 수정:
  ```ts
  // 변경 전: import { queryKeys } from "../api/queryKeys"
  // 변경 후:
  import { queryKeys } from '@/api/queryKeys'
  import { API_BASE_URL } from '@/constants'
  import type { AcquisitionStatus } from '@/api/types'
  ```
- [ ] `src/hooks/useAcquisitionActions.ts` 생성 — peanut-vision-ui의 `hooks/useAcquisitionActions.ts` 복사 후 import 경로 수정:
  ```ts
  // ../api/types → @/api/types
  // ../api/client → @/api/client
  // ../api/queryKeys → @/api/queryKeys
  // ../contexts/ToastContext → @/contexts/ToastContext
  // ../constants → @/constants
  ```
- [ ] `npx tsc --noEmit` — 에러 없어야 함

### Task 3-2: 공통 UI 컴포넌트 — CameraProfileSelector, AcquisitionModeSelector

각 컴포넌트 폴더에 `cx.ts`(표준 패턴), `index.module.scss`, `index.tsx` 생성.

**CameraProfileSelector:**
- [ ] `index.tsx`: MUI `Select/MenuItem` → `<select>/<option>` 변환:
  ```tsx
  import type { CamFileInfo } from '@/api/types'
  import cx from './cx'
  interface Props {
    cameras: CamFileInfo[]
    selectedProfile: string
    onProfileChange: (id: string) => void
    disabled?: boolean
  }
  export default function CameraProfileSelector({ cameras, selectedProfile, onProfileChange, disabled }: Props) {
    return (
      <div className={cx('CameraProfileSelector')}>
        <label className={cx('label')}>Camera Profile</label>
        <select value={selectedProfile} onChange={(e) => onProfileChange(e.target.value)} disabled={disabled} className={cx('select')}>
          {cameras.map((c) => <option key={c.fileName} value={c.fileName}>{c.fileName}</option>)}
        </select>
      </div>
    )
  }
  ```
- [ ] `index.module.scss`: label + select 스타일 (폭 100%, 높이 32px, border: 1px solid #444, background: #1e1e1e)

**AcquisitionModeSelector:**
- [ ] `index.tsx`: MUI `ToggleButtonGroup` → 버튼 그룹, `Select` → `<select>`:
  ```tsx
  import type { AcquisitionMode, TriggerModeOption } from '@/api/types'
  import cx from './cx'
  interface Props {
    mode: AcquisitionMode
    onModeChange: (mode: AcquisitionMode) => void
    triggerMode: TriggerModeOption
    onTriggerModeChange: (mode: TriggerModeOption) => void
    disabled?: boolean
  }
  export default function AcquisitionModeSelector({ mode, onModeChange, triggerMode, onTriggerModeChange, disabled }: Props) {
    return (
      <div className={cx('AcquisitionModeSelector')}>
        <div className={cx('toggleGroup')}>
          {(['single', 'continuous'] as AcquisitionMode[]).map((m) => (
            <button key={m} type="button" className={cx('toggle', { active: mode === m })}
              onClick={() => !disabled && onModeChange(m)} disabled={disabled}>
              {m === 'single' ? 'Single' : 'Continuous'}
            </button>
          ))}
        </div>
        <select value={triggerMode} onChange={(e) => onTriggerModeChange(e.target.value as TriggerModeOption)}
          disabled={disabled} className={cx('select')}>
          <option value="soft">Soft</option>
          <option value="hard">Hard</option>
          <option value="combined">Combined</option>
        </select>
      </div>
    )
  }
  ```
- [ ] `index.module.scss`: toggleGroup (flex, gap 0), toggle 버튼 스타일 (.active = 강조색)

### Task 3-3: AcquisitionActionBar

- [ ] `index.tsx`: MUI `Button/ButtonGroup/IconButton/Tooltip` → 일반 버튼으로 변환 (StatusChip은 shared에서 import):
  ```tsx
  import { Play, Square, Camera, Crosshair, RefreshCw } from 'lucide-react'
  import StatusChip from '@/components/shared/StatusChip'
  import type { AcquisitionAction, AcquisitionMode, AcquisitionStatus, ContinuousSubMode } from '@/api/types'
  import cx from './cx'
  interface Props {
    mode: AcquisitionMode; continuousSubMode: ContinuousSubMode; selectedProfile: string
    status: AcquisitionStatus | null; busy: boolean
    onCapture: () => void; onStart: () => void; onStop: () => void
    onTrigger: () => void; onRefresh: () => void; refreshThrottled: boolean
    hasWarnings?: boolean; hasErrors?: boolean
  }
  export default function AcquisitionActionBar(props: Props) {
    const { mode, continuousSubMode, selectedProfile, status, busy,
      onCapture, onStart, onStop, onTrigger, onRefresh, refreshThrottled, hasWarnings, hasErrors } = props
    const allowed = (a: AcquisitionAction) => status?.allowedActions?.includes(a) ?? false
    return (
      <div className={cx('bar')}>
        {mode === 'single' ? (
          <button type="button" className={cx('btn', 'primary')} onClick={onCapture}
            disabled={busy || !allowed('snapshot') || !selectedProfile}>
            <Camera size={14} /> Capture
          </button>
        ) : (
          <div className={cx('group')}>
            {allowed('stop') ? (
              <button type="button" className={cx('btn', 'danger')} onClick={onStop} disabled={busy}>
                <Square size={14} /> Stop
              </button>
            ) : (
              <button type="button" className={cx('btn', 'success')} onClick={onStart}
                disabled={busy || !allowed('start') || !selectedProfile}>
                <Play size={14} /> Start
              </button>
            )}
            {allowed('trigger') && continuousSubMode === 'manual' && (
              <button type="button" className={cx('btn')} onClick={onTrigger} disabled={busy}>
                <Crosshair size={14} /> Trigger
              </button>
            )}
          </div>
        )}
        {status && <StatusChip active={status.isActive}
          label={status.isActive ? `Active (${status.profileId ?? ''})` : 'Inactive'}
          hasWarnings={hasWarnings} hasErrors={hasErrors} />}
        <button type="button" className={cx('iconBtn')} onClick={onRefresh} disabled={refreshThrottled}>
          <RefreshCw size={14} />
        </button>
      </div>
    )
  }
  ```
- [ ] `index.module.scss`: flex row, gap 8px; `.btn` 기본 버튼 스타일, `.primary/.danger/.success` 색상 변형

### Task 3-4: ExposureControl

- [ ] `index.tsx`: MUI `Card/Slider/Button` → `div.card` + `input[type=range]` + `button`:
  ```tsx
  import type { ExposureInfo } from '@/api/types'
  import { DEFAULT_EXPOSURE_MIN, DEFAULT_EXPOSURE_MAX } from '@/constants'
  import cx from './cx'
  interface Props {
    exposure: ExposureInfo | null; exposureValue: number; isActive: boolean
    busy: boolean; isCalibrationAvailable: boolean
    onExposureChange: (v: number) => void; onLoad: () => void; onApply: () => void
  }
  export default function ExposureControl({ exposure, exposureValue, isActive, busy, isCalibrationAvailable, onExposureChange, onLoad, onApply }: Props) {
    const min = exposure?.exposureRange?.min ?? DEFAULT_EXPOSURE_MIN
    const max = exposure?.exposureRange?.max ?? DEFAULT_EXPOSURE_MAX
    return (
      <div className={cx('card')}>
        <div className={cx('header')}>
          <span className={cx('title')}>Exposure</span>
          <span className={cx('badge', isActive ? 'live' : 'pending')}>{isActive ? 'Live' : 'Pending'}</span>
          <button type="button" className={cx('textBtn')} onClick={onLoad} disabled={busy || !isActive}>Load Current</button>
        </div>
        <div className={cx('body')}>
          <label className={cx('sliderLabel')}>Exposure ({exposureValue.toFixed(0)} µs)</label>
          <input type="range" min={min} max={max} step={10} value={exposureValue}
            onChange={(e) => onExposureChange(Number(e.target.value))} className={cx('slider')} />
          {exposure?.exposureRange && (
            <span className={cx('hint')}>Range: {min} – {max} µs</span>
          )}
          <button type="button" className={cx('btn', 'primary')} onClick={onApply} disabled={busy || !isCalibrationAvailable}>
            {isActive ? 'Apply Settings' : 'Apply on Start'}
          </button>
        </div>
      </div>
    )
  }
  ```
- [ ] `index.module.scss`: card 스타일 (border, padding, border-radius), slider 전폭, badge 색상

### Task 3-5: CalibrationActions

- [ ] `index.tsx`: MUI `Card/Button/Switch/FormControlLabel` → `div.card` + `button` + `input[type=checkbox]`:
  ```tsx
  import cx from './cx'
  interface Props {
    busy: boolean; isCalibrationAvailable: boolean; ffcEnabled: boolean
    onBlack: () => void; onWhite: () => void; onWhiteBalance: () => void
    onFfcToggle: (_: unknown, checked: boolean) => void
  }
  export default function CalibrationActions({ busy, isCalibrationAvailable, ffcEnabled, onBlack, onWhite, onWhiteBalance, onFfcToggle }: Props) {
    const dis = busy || !isCalibrationAvailable
    return (
      <div className={cx('card')}>
        <h4 className={cx('title')}>Calibration Actions</h4>
        <div className={cx('stack')}>
          <div>
            <button type="button" className={cx('btn', 'outline', 'full')} disabled={dis} onClick={onBlack}>Black Calibration</button>
            <small className={cx('hint')}>Cover the lens before executing</small>
          </div>
          <div>
            <button type="button" className={cx('btn', 'outline', 'full')} disabled={dis} onClick={onWhite}>White Calibration</button>
            <small className={cx('hint')}>Ensure uniform ~200DN illumination</small>
          </div>
          <div>
            <button type="button" className={cx('btn', 'outline', 'full')} disabled={dis} onClick={onWhiteBalance}>White Balance (Once)</button>
            <small className={cx('hint')}>Point lens at white target (~200DN)</small>
          </div>
          <label className={cx('checkLabel')}>
            <input type="checkbox" checked={ffcEnabled} onChange={(e) => onFfcToggle(null, e.target.checked)} disabled={dis} />
            Flat Field Correction (FFC)
          </label>
        </div>
      </div>
    )
  }
  ```

### Task 3-6: PresetSelector (with Modal)

- [ ] `src/components/shared/Modal/index.tsx` 생성 (재사용 모달 컴포넌트):
  ```tsx
  import type { ReactNode } from 'react'
  import styles from './index.module.scss'
  import classNames from 'classnames/bind'
  const cx = classNames.bind(styles)
  interface Props { open: boolean; onClose: () => void; title: string; children: ReactNode; actions?: ReactNode }
  export default function Modal({ open, onClose, title, children, actions }: Props) {
    if (!open) return null
    return (
      <div className={cx('overlay')} onClick={onClose}>
        <div className={cx('dialog')} onClick={(e) => e.stopPropagation()}>
          <div className={cx('header')}><h3>{title}</h3><button onClick={onClose}>✕</button></div>
          <div className={cx('body')}>{children}</div>
          {actions && <div className={cx('footer')}>{actions}</div>}
        </div>
      </div>
    )
  }
  ```
- [ ] `src/components/shared/Modal/index.module.scss` 생성:
  ```scss
  .overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); z-index: 1000; display: flex; align-items: center; justify-content: center; }
  .dialog { background: #222; border: 1px solid #444; border-radius: 8px; min-width: 360px; max-width: 560px; width: 100%; }
  .header { display: flex; justify-content: space-between; align-items: center; padding: 16px 20px; border-bottom: 1px solid #333;
    h3 { margin: 0; font-size: 1rem; } button { background: none; border: none; color: #888; cursor: pointer; font-size: 1.2rem; }
  }
  .body { padding: 16px 20px; }
  .footer { display: flex; justify-content: flex-end; gap: 8px; padding: 12px 20px; border-top: 1px solid #333; }
  ```
- [ ] `PresetSelector/index.tsx` 생성: peanut-vision-ui의 `PresetSelector.tsx` 를 기반으로 MUI Dialog → Modal, MUI Button/TextField/List → 순수 HTML 변환, import 경로 `@/`로 수정
  - MUI `Dialog` → `Modal` (위에서 생성)
  - `TextField` → `<input type="text">`
  - `List/ListItemButton` → `<ul>/<li>/<button>`
  - MUI icon → lucide-react (Save, FolderOpen, Trash2)
  - `useToast` import: `@/contexts/ToastContext`

### Task 3-7: SessionSelector

- [ ] `SessionSelector/index.tsx` 생성: peanut-vision-ui의 `SessionSelector.tsx` 를 기반으로 MUI → HTML 변환
  - MUI `Chip` → `<span class="chip">`
  - MUI `Dialog` → `Modal`
  - MUI icon → lucide-react (Plus, Square, History, Trash2)
  - `useToast` → `@/contexts/ToastContext`
  - `onSessionChange` prop 제거 (탭 기반 앱에서 더 이상 App 레벨 상태 불필요)

### Task 3-8: ContinuousSettings, ImageSaveSettingsPanel

**ContinuousSettings:**
- [ ] `index.tsx`: MUI `ToggleButtonGroup` → 버튼 그룹, `TextField[type=number]` → `<input type="number">`, `Checkbox` → `<input type="checkbox">`

**ImageSaveSettingsPanel:**
- [ ] `index.tsx`: MUI `Accordion` → `<details>/<summary>`, `TextField[select]` → `<select>`, `TextField` → `<input>`, `Checkbox` → `<input type="checkbox">`

### Task 3-9: ImageViewer

- [ ] `src/components/shared/ImageActionBar/index.tsx` 생성:
  ```tsx
  import { Download, Save } from 'lucide-react'
  import cx from './cx'
  interface Props { url: string; filename?: string; savedPath?: string }
  export default function ImageActionBar({ url, filename, savedPath }: Props) {
    return (
      <div className={cx('bar')}>
        <a className={cx('btn')} href={url} download={filename ?? `capture.png`}>
          <Download size={14} /> Download
        </a>
        {savedPath && (
          <span className={cx('savedPath')} title={savedPath}>
            <Save size={12} /> {savedPath}
          </span>
        )}
      </div>
    )
  }
  ```
- [ ] `src/components/shared/ImageViewer/index.tsx` 생성: peanut-vision-ui의 `ImageViewer.tsx` 기반으로 MUI → HTML 변환
  - `Box(dashed border, empty)` → `<div class="empty">`
  - `Chip` (error overlay) → `<span class="errorChip">`
  - `Chip` (LIVE) → `<span class="liveChip">`
  - `Chip` (timestamp) → `<span class="timestampChip">`

### Task 3-10: Acquisition 페이지 조립

- [ ] `src/components/Acquisition/index.tsx` 수정 — AcquisitionTab 로직 포팅:
  - 사이드바(Capture/Camera/Settings 탭) + 드래그 핸들 + 이미지 뷰어 레이아웃
  - `useAcquisitionActions`, `useLiveStream`, `useResizablePanel` 사용
  - 모든 MUI `Box/Tabs/Tab` → `div`/CSS + 상태 기반 탭 전환
- [ ] `src/components/Acquisition/index.module.scss` 수정: flex 레이아웃, 사이드바, 드래그 핸들, 이미지 뷰어 영역 스타일
- [ ] `npm run dev` → Acquisition 페이지에서 촬영 컨트롤 확인
- [ ] `npm run build`
- [ ] 커밋:
  ```bash
  git add src/hooks/ src/components/shared/ src/components/Acquisition/
  git commit -m "feat(p3): implement Acquisition page with all controls"
  ```

**Phase 3 완료 기준:** Acquisition 페이지에서 사이드바 탭 전환, 카메라 프로파일 선택, 촬영 버튼, Exposure/Calibration/Preset/Session 패널이 동작함.

---

## Phase 4: Gallery 페이지

**목표:** 이미지 썸네일 그리드 + 선택 이미지 뷰어가 동작함.

### 파일 변경 목록

| 작업 | 파일 |
|------|------|
| 생성 | `src/hooks/useImageGallery.ts` |
| 생성 | `src/components/shared/ImageGallery/` |
| 수정 | `src/components/Gallery/index.tsx` + `index.module.scss` |

### Task 4-1: useImageGallery 훅

- [ ] `src/hooks/useImageGallery.ts` 생성 — peanut-vision-ui의 `hooks/useImageGallery.ts` 복사 후 import 경로 수정:
  ```ts
  // ../api/client → @/api/client
  // ../api/queryKeys → @/api/queryKeys
  // ../constants → @/constants
  // ../contexts/ToastContext → @/contexts/ToastContext
  ```

### Task 4-2: ImageGallery 컴포넌트

- [ ] `src/components/shared/ImageGallery/index.tsx` 생성: peanut-vision-ui의 `ImageGallery.tsx` 기반으로 MUI → HTML 변환
  - MUI `Select/MenuItem` → `<select>/<option>`
  - MUI `CircularProgress` → CSS 스피너
  - MUI `Box/Tooltip` → `div` + `title` 속성 (또는 CSS tooltip)
  - MUI `IconButton` → `<button>` with lucide-react `X` icon
  - MUI `Button(Load more)` → `<button>`
  - MUI `Button(Clear All)` → `<button class="danger">`
- [ ] `index.module.scss`: 썸네일 그리드 (auto-fill, minmax 72px), 선택 상태 outline, hover 시 삭제 버튼 표시

### Task 4-3: Gallery 페이지 조립

- [ ] `src/components/Gallery/index.tsx` 수정:
  - `useImageGallery` 훅 사용
  - 좌: `ImageViewer` (480px 고정) + 우: `ImageGallery` (flex-grow)
  - MUI `Box` → `div.galleryLayout`, 스타일은 SCSS
- [ ] `npm run dev` → Gallery 페이지 확인
- [ ] `npm run build`
- [ ] 커밋:
  ```bash
  git add src/hooks/useImageGallery.ts src/components/shared/ImageGallery/ src/components/Gallery/
  git commit -m "feat(p4): implement Gallery page with thumbnail grid and viewer"
  ```

**Phase 4 완료 기준:** Gallery 페이지에서 썸네일 그리드가 렌더링되고, 클릭 시 좌측 뷰어에 원본 이미지가 표시됨.

---

## Phase 5: Latency 페이지

**목표:** 레이턴시 통계 카드 + 차트 + 테이블이 동작함.

### 파일 변경 목록

| 작업 | 파일 |
|------|------|
| 수정 | `src/components/Latency/index.tsx` + `index.module.scss` |

### Task 5-1: Latency 페이지 구현

LatencyTab에는 외부 공유 컴포넌트 의존성이 없음. 모든 sub-component(StatCard, LatencyChart, RecordsTable, LatencyChip)를 `index.tsx` 내부에 작성.

- [ ] `src/components/Latency/index.tsx` 수정 — peanut-vision-ui의 `tabs/LatencyTab.tsx` 기반으로 변환:
  - MUI `Card/CardContent` → `div.card`
  - MUI `Grid` → CSS grid
  - MUI `Paper` → `div.paper`
  - MUI `Table/*` → `<table>/<thead>/<tbody>/<tr>/<th>/<td>`
  - MUI `Button` → `<button>`
  - MUI `Chip(LatencyChip)` → `<span class="chip success/warning/error">`
  - MUI `CircularProgress` → CSS 스피너
  - MUI `Typography` → 시맨틱 태그
  - recharts는 **그대로 사용** (MUI가 아님)
  - import 경로 수정: `@tanstack/react-query`, `@/api/client`, `lucide-react`
- [ ] `src/components/Latency/index.module.scss` 수정: card, paper, stats grid, table 스타일
- [ ] `npm run dev` → Latency 페이지에서 차트/테이블 확인
- [ ] `npm run build`
- [ ] 커밋:
  ```bash
  git add src/components/Latency/
  git commit -m "feat(p5): implement Latency page with chart and records table"
  ```

**Phase 5 완료 기준:** Latency 페이지에서 통계 카드, 레이턴시 차트, 레코드 테이블이 렌더링됨.

---

## 전체 완료 기준

- [ ] `npm run build` — TypeScript 에러 없음
- [ ] MUI import가 전체 코드베이스에 없음: `grep -r "@mui" src/` 결과 없음
- [ ] 4개 GNB 메뉴에서 각 페이지 네비게이션 동작
- [ ] API 연결 시 System/Acquisition/Gallery/Latency 기능 동작
