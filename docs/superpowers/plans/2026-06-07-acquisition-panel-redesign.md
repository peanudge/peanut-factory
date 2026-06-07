# Acquisition Panel Redesign Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Redesign the Acquisition page to a right-side settings panel with a single top-to-bottom form flow (presets → settings → trigger mode → start), replacing the current split Manual/Preset paths.

**Architecture:** `Acquisition/index.tsx` renders `<canvas | sidebar>` (image left, panel right). `CaptureTab` owns preset data-fetching and switches between `ActiveView` (during capture) and `AcquisitionSettings` (idle). `AcquisitionSettings` is the complete idle form: presets quick-start chips, native input fields, trigger mode selector, and start button.

**Tech Stack:** React 18, TanStack Query, lucide-react (Loader2 spinner), CSS Modules, Vitest + Testing Library

---

## File Map

| File | Action | Responsibility after change |
|------|--------|-----------------------------|
| `src/peanut-vision-app/src/hooks/useAcquisitionConfig.ts` | Modify | Expose `camerasLoading` |
| `src/peanut-vision-app/src/hooks/useAcquisitionSession.ts` | Modify | Add `handleStartWithConfig` |
| `src/peanut-vision-app/src/components/Acquisition/index.tsx` | Modify | Right panel layout, remove inputMode/selectedPreset state |
| `src/peanut-vision-app/src/components/Acquisition/index.module.scss` | Modify | border-left, spin animation |
| `src/peanut-vision-app/src/components/Acquisition/CaptureTab.tsx` | Rewrite | Single IdleView → AcquisitionSettings; ActiveView with spinners |
| `src/peanut-vision-app/src/components/shared/AcquisitionSettings/index.tsx` | Rewrite | Full idle form: presets + settings + trigger + start |
| `src/peanut-vision-app/src/components/shared/AcquisitionSettings/index.module.scss` | Rewrite | Minimal layout-only styles |
| `src/peanut-vision-app/src/test/AcquisitionSettings.test.tsx` | Rewrite | Tests for new component API |
| `src/peanut-vision-app/src/components/shared/AcquisitionActionBar/` | Delete | Replaced by Start button in AcquisitionSettings |
| `src/peanut-vision-app/src/components/shared/CameraProfileSelector/` | Delete | Replaced by native `<select>` in AcquisitionSettings |
| `src/peanut-vision-app/src/test/AcquisitionActionBar.test.tsx` | Delete | Component removed |

---

## Task 1: Expose `camerasLoading` from `useAcquisitionConfig`

**Files:**
- Modify: `src/peanut-vision-app/src/hooks/useAcquisitionConfig.ts`

- [ ] **Step 1: Add camerasLoading to the return value**

Open `useAcquisitionConfig.ts`. Find the cameras query and add `isLoading` to the destructuring. Then add `camerasLoading` to the return object.

The cameras query currently looks like:
```typescript
const { data: cameras = [] } = useQuery({ queryKey: queryKeys.cameras, queryFn: getCamFiles })
```

Change to:
```typescript
const { data: cameras = [], isLoading: camerasLoading } = useQuery({
  queryKey: queryKeys.cameras,
  queryFn: getCamFiles,
})
```

And in the return statement, add `camerasLoading`:
```typescript
return { cameras, camerasLoading, config, updateConfig, loadPreset }
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd src/peanut-vision-app && npx tsc --noEmit
```
Expected: no errors (consumers use spread types, so adding a field is non-breaking).

- [ ] **Step 3: Commit**

```bash
git add src/peanut-vision-app/src/hooks/useAcquisitionConfig.ts
git commit -m "feat: expose camerasLoading from useAcquisitionConfig"
```

---

## Task 2: Add `handleStartWithConfig` to `useAcquisitionSession`

**Files:**
- Modify: `src/peanut-vision-app/src/hooks/useAcquisitionSession.ts`

- [ ] **Step 1: Refactor startMutation to accept config as parameter**

Currently `mutationFn: () => startAcquisition({...config...})` captures config from closure. Change it to accept config explicitly so both `handleStart` and `handleStartWithConfig` share the same mutation (and thus the same `isPending` → `busy`):

```typescript
const startMutation = useMutation({
  mutationFn: (cfg: AcquisitionFormConfig) =>
    startAcquisition({
      profileId: cfg.profileId,
      frameCount: cfg.frameCount,
      intervalMs: cfg.acquisitionMode === 'auto' ? cfg.intervalMs : null,
      outputDirectory: cfg.outputDirectory,
      format: cfg.format,
    }),
  onSuccess: () => {
    invalidateStatus()
    toast('촬영이 시작되었습니다', 'success')
  },
  onError: handleError,
})
```

- [ ] **Step 2: Update handleStart and add handleStartWithConfig**

```typescript
const handleStart = () => {
  if (config.acquisitionMode === 'auto' && config.intervalMs === null) {
    toast('Please input interval time', 'warning')
    return
  }
  startMutation.mutate(config)
}

const handleStartWithConfig = (cfg: AcquisitionFormConfig) => {
  if (cfg.acquisitionMode === 'auto' && cfg.intervalMs === null) {
    toast('Please input interval time', 'warning')
    return
  }
  startMutation.mutate(cfg)
}
```

Add `handleStartWithConfig` to the return object:
```typescript
return {
  status, isActive, busy, canStart, canStop, canTrigger,
  hasWarnings, hasErrors,
  handleStart, handleStartWithConfig,
  handleStop: () => stopMutation.mutate(),
  handleTrigger: () => triggerMutation.mutate(),
}
```

- [ ] **Step 3: Verify TypeScript compiles**

```bash
cd src/peanut-vision-app && npx tsc --noEmit
```
Expected: no errors.

- [ ] **Step 4: Commit**

```bash
git add src/peanut-vision-app/src/hooks/useAcquisitionSession.ts
git commit -m "feat: add handleStartWithConfig to useAcquisitionSession"
```

---

## Task 3: Flip layout — panel moves to the right

**Files:**
- Modify: `src/peanut-vision-app/src/components/Acquisition/index.tsx`
- Modify: `src/peanut-vision-app/src/components/Acquisition/index.module.scss`

- [ ] **Step 1: Remove inputMode / selectedPreset state from index.tsx**

Remove these lines entirely:
```typescript
// DELETE these:
const [inputMode, setInputMode] = useState<InputMode>('manual')
const [selectedPreset, setSelectedPreset] = useState<AcquisitionConfigPreset | null>(null)

const activeFormConfig = inputMode === 'preset' && selectedPreset
  ? presetToFormConfig(selectedPreset)
  : acqConfig.config
```

Change `useAcquisitionSession(activeFormConfig)` → `useAcquisitionSession(acqConfig.config)`.

Also delete the `presetToFormConfig` function — it will move to `CaptureTab.tsx` in Task 4.

- [ ] **Step 2: Simplify the JSX — canvas first, sidebar second**

Replace the Acquisition return with:
```tsx
return (
  <div className={cx('Acquisition')}>
    <div className={cx('canvas')}>
      <div className={cx('imageArea')}>
        <ImageViewer
          url={live.previewUrl}
          errorMessage={session.status?.lastError}
          isLive={live.isActive}
          capturedAt={null}
          onClose={() => {}}
        />
      </div>
    </div>

    <div className={cx('sidebar')}>
      <CaptureTab acqConfig={acqConfig} session={session} />
    </div>
  </div>
)
```

Remove the now-unused `InputMode` type export and `AcquisitionConfigPreset` import if no longer used here.

- [ ] **Step 3: Update index.module.scss — border-left, add spin animation**

In `.sidebar`, change `border-right` → `border-left`:
```scss
.sidebar {
  width: 320px;
  flex-shrink: 0;
  border-left: 1px solid var(--border-subtle);  // was border-right
  overflow-y: auto;
  overflow-x: hidden;
  padding: 20px 18px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  position: relative;
  background: var(--bg-surface);
}
```

Add spin keyframe at the end of the file (needed by CaptureTab's Active view):
```scss
.spin {
  animation: spin-kf 0.8s linear infinite;
}

@keyframes spin-kf { to { transform: rotate(360deg); } }
```

- [ ] **Step 4: Verify TypeScript compiles**

```bash
cd src/peanut-vision-app && npx tsc --noEmit
```

- [ ] **Step 5: Commit**

```bash
git add src/peanut-vision-app/src/components/Acquisition/
git commit -m "refactor: move acquisition panel to right side, remove inputMode state"
```

---

## Task 4: Rewrite CaptureTab — single IdleView

**Files:**
- Rewrite: `src/peanut-vision-app/src/components/Acquisition/CaptureTab.tsx`

- [ ] **Step 1: Write the new CaptureTab.tsx**

Replace the entire file content:

```tsx
import { Loader2, Square } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import StatusChip from '@/components/shared/StatusChip'
import type { AcquisitionConfigPreset, AcquisitionFormConfig } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'
import { getPresets, savePreset } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'
import type { UseAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import type { AcquisitionSession } from '@/hooks/useAcquisitionSession'
import cx from './cx'

function presetToFormConfig(preset: AcquisitionConfigPreset): AcquisitionFormConfig {
  return {
    ...DEFAULT_ACQUISITION_FORM_CONFIG,
    profileId: preset.profileId,
    frameCount: preset.frameCount ?? null,
    intervalMs: preset.intervalMs ?? null,
    acquisitionMode: preset.intervalMs != null ? 'auto' : 'manual',
    outputDirectory: preset.outputDirectory ?? DEFAULT_ACQUISITION_FORM_CONFIG.outputDirectory,
    format: preset.format ?? DEFAULT_ACQUISITION_FORM_CONFIG.format,
  }
}

interface Props {
  acqConfig: UseAcquisitionConfig
  session: AcquisitionSession
}

export default function CaptureTab({ acqConfig, session }: Props) {
  if (session.isActive) {
    return <ActiveView session={session} />
  }
  return <IdleView acqConfig={acqConfig} session={session} />
}

// ── Active: status + stop/trigger ────────────────────────────────────────────

function ActiveView({ session }: { session: AcquisitionSession }) {
  const s = session.status
  return (
    <div className={cx('activeView')}>
      <div className={cx('activeHeader')}>
        <StatusChip
          active
          label={`Active — ${s?.profileId ?? ''}`}
          hasWarnings={session.hasWarnings}
          hasErrors={session.hasErrors}
        />
      </div>
      <dl className={cx('activeInfo')}>
        <div className={cx('infoRow')}><dt>Profile</dt><dd>{s?.profileId ?? '—'}</dd></div>
        <div className={cx('infoRow')}>
          <dt>Frame count</dt>
          <dd>{s?.activeFrameCount != null ? s.activeFrameCount : '∞'}</dd>
        </div>
        <div className={cx('infoRow')}>
          <dt>Interval</dt>
          <dd>{s?.activeIntervalMs != null ? `${s.activeIntervalMs / 1000}s` : 'manual'}</dd>
        </div>
        <div className={cx('infoRow')}>
          <dt>Save to</dt>
          <dd title={s?.outputDirectory}>{s?.outputDirectory ?? '—'}</dd>
        </div>
        {s?.statistics && (
          <>
            <div className={cx('infoRow')}><dt>Frames</dt><dd>{s.statistics.frameCount}</dd></div>
            <div className={cx('infoRow')}><dt>FPS</dt><dd>{s.statistics.averageFps.toFixed(1)}</dd></div>
            {s.statistics.droppedFrameCount > 0 && (
              <div className={cx('infoRow', 'warn')}><dt>Dropped</dt><dd>{s.statistics.droppedFrameCount}</dd></div>
            )}
          </>
        )}
        {s?.lastError && (
          <div className={cx('infoRow', 'error')}><dt>Error</dt><dd>{s.lastError}</dd></div>
        )}
      </dl>

      {s?.activeIntervalMs == null && (
        <button
          type="button"
          className={cx('triggerBtn')}
          onClick={session.handleTrigger}
          disabled={session.busy}
        >
          {session.busy && <Loader2 size={14} className={cx('spin')} />}
          Trigger
        </button>
      )}

      <button
        type="button"
        className={cx('stopBtn')}
        onClick={session.handleStop}
        disabled={session.busy}
      >
        {session.busy
          ? <Loader2 size={14} className={cx('spin')} />
          : <Square size={14} />}
        Stop
      </button>
    </div>
  )
}

// ── Idle: full acquisition form ───────────────────────────────────────────────

function IdleView({ acqConfig, session }: Props) {
  const { toast } = useToast()
  const queryClient = useQueryClient()

  const { data: presets = [], isLoading: presetsLoading } = useQuery({
    queryKey: queryKeys.presets,
    queryFn: getPresets,
  })

  const saveMutation = useMutation({
    mutationFn: savePreset,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      toast('프리셋이 저장되었습니다', 'success')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '저장 실패', 'error'),
  })

  return (
    <AcquisitionSettings
      config={acqConfig.config}
      onChange={acqConfig.updateConfig}
      cameras={acqConfig.cameras}
      camerasLoading={acqConfig.camerasLoading}
      presets={presets}
      presetsLoading={presetsLoading}
      onQuickStart={(preset) => session.handleStartWithConfig(presetToFormConfig(preset))}
      canStart={session.canStart}
      busy={session.busy}
      onStart={session.handleStart}
      onSavePreset={(name) =>
        saveMutation.mutate({
          name,
          profileId: acqConfig.config.profileId,
          frameCount: acqConfig.config.frameCount,
          intervalMs: acqConfig.config.intervalMs,
          outputDirectory: acqConfig.config.outputDirectory,
          format: acqConfig.config.format,
        })
      }
      savingPreset={saveMutation.isPending}
    />
  )
}
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd src/peanut-vision-app && npx tsc --noEmit
```
Expected: errors about AcquisitionSettings props not matching — that's correct, Task 5 fixes them.

- [ ] **Step 3: Commit (even with compile errors as WIP)**

```bash
git add src/peanut-vision-app/src/components/Acquisition/CaptureTab.tsx
git commit -m "refactor: simplify CaptureTab to single IdleView, add preset quick-start"
```

---

## Task 5: Rewrite AcquisitionSettings component

**Files:**
- Rewrite: `src/peanut-vision-app/src/components/shared/AcquisitionSettings/index.tsx`

- [ ] **Step 1: Write the new AcquisitionSettings/index.tsx**

Replace the entire file:

```tsx
import { useState } from 'react'
import { Loader2, FolderSearch } from 'lucide-react'
import type { AcquisitionFormConfig, SaveImageFormat, AcquisitionConfigPreset, CamFileInfo } from '@/api/types'
import DirectoryBrowser from '@/components/shared/DirectoryBrowser'
import Modal from '@/components/shared/Modal'
import cx from './cx'

interface Props {
  config: AcquisitionFormConfig
  onChange: <K extends keyof AcquisitionFormConfig>(key: K, value: AcquisitionFormConfig[K]) => void
  cameras: CamFileInfo[]
  camerasLoading: boolean
  presets: AcquisitionConfigPreset[]
  presetsLoading: boolean
  onQuickStart: (preset: AcquisitionConfigPreset) => void
  canStart: boolean
  busy: boolean
  onStart: () => void
  onSavePreset: (name: string) => void
  savingPreset: boolean
}

const FORMATS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

export default function AcquisitionSettings({
  config, onChange,
  cameras, camerasLoading,
  presets, presetsLoading,
  onQuickStart,
  canStart, busy, onStart,
  onSavePreset, savingPreset,
}: Props) {
  const [browserOpen, setBrowserOpen] = useState(false)
  const [saveOpen, setSaveOpen] = useState(false)
  const [presetName, setPresetName] = useState('')

  const handleSave = () => {
    if (!presetName.trim()) return
    onSavePreset(presetName.trim())
    setPresetName('')
    setSaveOpen(false)
  }

  const showPresetsSection = presetsLoading || presets.length > 0

  return (
    <div className={cx('wrap')}>

      {/* ── Quick Start ── */}
      {showPresetsSection && (
        <section className={cx('section')}>
          <span className={cx('sectionLabel')}>빠른 시작</span>
          {presetsLoading ? (
            <div className={cx('skeletons')}>
              <div className={cx('skeleton')} />
              <div className={cx('skeleton')} />
            </div>
          ) : (
            <div className={cx('chips')}>
              {presets.map((p) => (
                <button
                  key={p.name}
                  type="button"
                  className={cx('chip')}
                  onClick={() => onQuickStart(p)}
                  disabled={busy}
                  title={[
                    p.profileId,
                    p.frameCount != null ? `${p.frameCount}f` : null,
                    p.intervalMs != null ? `${p.intervalMs / 1000}s` : null,
                  ].filter(Boolean).join(' · ')}
                >
                  {busy && <Loader2 size={11} className={cx('spin')} />}
                  {p.name}
                </button>
              ))}
            </div>
          )}
        </section>
      )}

      {/* ── Settings Form ── */}
      <section className={cx('section')}>
        <span className={cx('sectionLabel')}>촬영 설정</span>

        <label>
          카메라 프로파일
          <select
            value={config.profileId}
            onChange={(e) => onChange('profileId', e.target.value)}
            disabled={camerasLoading || busy}
          >
            {camerasLoading
              ? <option>로딩 중…</option>
              : cameras.map((c) => (
                  <option key={c.fileName} value={c.fileName}>{c.fileName}</option>
                ))
            }
          </select>
        </label>

        <label>
          저장 경로
          <div className={cx('dirRow')}>
            <input
              type="text"
              value={config.outputDirectory}
              onChange={(e) => onChange('outputDirectory', e.target.value)}
              disabled={busy}
              placeholder="CapturedImages"
            />
            <button
              type="button"
              onClick={() => setBrowserOpen(true)}
              disabled={busy}
              title="Browse"
            >
              <FolderSearch size={14} />
            </button>
          </div>
          <small>토큰: {'{date}'} → yyyy-MM-dd, {'{profile}'} → cam 파일명</small>
        </label>

        <fieldset>
          <legend>포맷</legend>
          {FORMATS.map(({ value, label }) => (
            <label key={value}>
              <input
                type="radio"
                name="format"
                value={value}
                checked={config.format === value}
                onChange={() => onChange('format', value)}
                disabled={busy}
              />
              {label}
            </label>
          ))}
        </fieldset>

        <label>
          프레임 수
          <input
            type="number"
            min={1}
            value={config.frameCount ?? ''}
            placeholder="∞ (제한없음)"
            onChange={(e) => {
              const v = parseInt(e.target.value, 10)
              onChange('frameCount', isNaN(v) || v < 1 ? null : v)
            }}
            disabled={busy}
          />
        </label>
      </section>

      {/* ── Trigger Mode ── */}
      <section className={cx('section')}>
        <span className={cx('sectionLabel')}>촬영 방식</span>

        <label>
          <input
            type="radio"
            name="acquisitionMode"
            value="auto"
            checked={config.acquisitionMode === 'auto'}
            onChange={() => onChange('acquisitionMode', 'auto')}
            disabled={busy}
          />
          자동 (N초 간격 반복)
        </label>

        <label>
          <input
            type="radio"
            name="acquisitionMode"
            value="manual"
            checked={config.acquisitionMode === 'manual'}
            onChange={() => onChange('acquisitionMode', 'manual')}
            disabled={busy}
          />
          수동 (트리거로 한 장씩)
        </label>

        {config.acquisitionMode === 'auto' && (
          <label>
            간격
            <input
              type="number"
              min={0.05}
              step={0.1}
              value={config.intervalMs != null ? config.intervalMs / 1000 : ''}
              placeholder="1"
              onChange={(e) => {
                const secs = parseFloat(e.target.value)
                onChange('intervalMs', isNaN(secs) || secs <= 0 ? null : Math.round(secs * 1000))
              }}
              disabled={busy}
            />
            초
          </label>
        )}
      </section>

      {/* ── Start ── */}
      <section className={cx('section')}>
        <button
          type="button"
          className={cx('startBtn')}
          onClick={onStart}
          disabled={!canStart || busy}
        >
          {busy && <Loader2 size={14} className={cx('spin')} />}
          촬영 시작
        </button>

        <button
          type="button"
          className={cx('saveLinkBtn')}
          onClick={() => setSaveOpen(true)}
          disabled={!config.profileId || busy}
        >
          + 프리셋으로 저장
        </button>
      </section>

      <DirectoryBrowser
        open={browserOpen}
        currentPath={config.outputDirectory}
        onSelect={(path) => onChange('outputDirectory', path)}
        onClose={() => setBrowserOpen(false)}
      />

      <Modal
        open={saveOpen}
        onClose={() => setSaveOpen(false)}
        title="프리셋 저장"
        actions={
          <>
            <button type="button" onClick={() => setSaveOpen(false)}>
              취소
            </button>
            <button
              type="button"
              onClick={handleSave}
              disabled={savingPreset || !presetName.trim()}
            >
              {savingPreset && <Loader2 size={13} className={cx('spin')} />}
              저장
            </button>
          </>
        }
      >
        <input
          type="text"
          placeholder="프리셋 이름"
          value={presetName}
          onChange={(e) => setPresetName(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleSave()}
          autoFocus
        />
        <small>
          {[
            config.profileId || 'none',
            config.frameCount != null ? `${config.frameCount} frames` : null,
            config.intervalMs != null ? `${config.intervalMs / 1000}s` : null,
          ].filter(Boolean).join(' | ')}
        </small>
      </Modal>
    </div>
  )
}
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd src/peanut-vision-app && npx tsc --noEmit
```
Expected: no errors.

- [ ] **Step 3: Commit**

```bash
git add src/peanut-vision-app/src/components/shared/AcquisitionSettings/index.tsx
git commit -m "feat: rewrite AcquisitionSettings — presets, native inputs, trigger mode, start"
```

---

## Task 6: Rewrite AcquisitionSettings SCSS

**Files:**
- Rewrite: `src/peanut-vision-app/src/components/shared/AcquisitionSettings/index.module.scss`

- [ ] **Step 1: Replace the stylesheet**

```scss
.wrap {
  display: flex;
  flex-direction: column;
  gap: 0;
}

// ── Sections ──

.section {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 16px 0;
  border-top: 1px solid var(--border-subtle);

  &:first-child {
    padding-top: 0;
    border-top: none;
  }
}

.sectionLabel {
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

// ── Presets ──

.chips {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.chip {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 10px;
  font-size: 0.78rem;
  border-radius: 4px;
  cursor: pointer;
  border: 1px solid var(--border-default);
  background: var(--bg-surface);
  color: var(--text-primary);

  &:hover:not(:disabled) { background: var(--bg-hover); }
  &:disabled { opacity: 0.5; cursor: not-allowed; }
}

.skeletons {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.skeleton {
  height: 26px;
  border-radius: 4px;
  background: var(--bg-hover);
  animation: pulse 1.4s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}

// ── Form labels ──

.section label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 0.8rem;
  color: var(--text-secondary);
}

.section fieldset {
  border: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;

  legend {
    font-size: 0.8rem;
    color: var(--text-secondary);
    padding: 0;
    margin-bottom: 4px;
    float: left;
    width: 100%;
  }

  label {
    flex-direction: row;
    align-items: center;
    gap: 6px;
  }
}

// ── Dir row ──

.dirRow {
  display: flex;
  gap: 4px;

  input { flex: 1; min-width: 0; }
}

// ── Start button ──

.startBtn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  width: 100%;
  padding: 10px;
  font-size: 0.88rem;
  font-weight: 600;
  cursor: pointer;
  border: none;
  border-radius: 6px;
  background: var(--color-accent);
  color: #fff;

  &:hover:not(:disabled) { opacity: 0.9; }
  &:disabled { opacity: 0.4; cursor: not-allowed; }
}

.saveLinkBtn {
  background: none;
  border: none;
  font-size: 0.75rem;
  color: var(--text-muted);
  cursor: pointer;
  align-self: flex-start;
  padding: 0;

  &:hover:not(:disabled) { color: var(--text-primary); }
  &:disabled { opacity: 0.4; cursor: not-allowed; }
}

// ── Spinner ──

.spin {
  animation: spin-kf 0.8s linear infinite;
  flex-shrink: 0;
}

@keyframes spin-kf { to { transform: rotate(360deg); } }
```

- [ ] **Step 2: Verify no style regressions**

```bash
cd src/peanut-vision-app && npx tsc --noEmit
```

- [ ] **Step 3: Commit**

```bash
git add src/peanut-vision-app/src/components/shared/AcquisitionSettings/index.module.scss
git commit -m "style: simplify AcquisitionSettings stylesheet to minimal layout styles"
```

---

## Task 7: Rewrite AcquisitionSettings tests

**Files:**
- Rewrite: `src/peanut-vision-app/src/test/AcquisitionSettings.test.tsx`

- [ ] **Step 1: Write the new test file**

```tsx
import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import type { AcquisitionFormConfig, AcquisitionConfigPreset, CamFileInfo } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'

vi.mock('@/components/shared/AcquisitionSettings/cx', () => ({
  default: (...args: unknown[]) => args.filter(Boolean).join(' '),
}))
vi.mock('@/components/shared/DirectoryBrowser', () => ({ default: () => null }))
vi.mock('@/components/shared/Modal', () => ({
  default: ({ open, children, actions }: {
    open: boolean; children: React.ReactNode; actions: React.ReactNode
  }) => open ? <div role="dialog">{children}{actions}</div> : null,
}))

const BASE_CONFIG: AcquisitionFormConfig = {
  ...DEFAULT_ACQUISITION_FORM_CONFIG,
  profileId: 'cam1.cam',
  acquisitionMode: 'manual',
}

const CAMERAS: CamFileInfo[] = [
  { fileName: 'cam1.cam', manufacturer: '', cameraModel: '', width: 1920, height: 1080,
    spectrum: '', colorFormat: '', trigMode: '', acquisitionMode: '', tapConfiguration: '' },
  { fileName: 'cam2.cam', manufacturer: '', cameraModel: '', width: 4160, height: 3120,
    spectrum: '', colorFormat: '', trigMode: '', acquisitionMode: '', tapConfiguration: '' },
]

const PRESETS: AcquisitionConfigPreset[] = [
  { name: '프리셋A', profileId: 'cam1.cam', format: 'png' },
  { name: '프리셋B', profileId: 'cam2.cam', format: 'bmp', frameCount: 10 },
]

function renderSettings(overrides: Partial<Parameters<typeof AcquisitionSettings>[0]> = {}) {
  const defaults = {
    config: BASE_CONFIG,
    onChange: vi.fn(),
    cameras: CAMERAS,
    camerasLoading: false,
    presets: [],
    presetsLoading: false,
    onQuickStart: vi.fn(),
    canStart: true,
    busy: false,
    onStart: vi.fn(),
    onSavePreset: vi.fn(),
    savingPreset: false,
  }
  return render(<AcquisitionSettings {...defaults} {...overrides} />)
}

// ── Presets section ──

describe('Presets quick-start section', () => {
  it('does not render section when presets empty and not loading', () => {
    renderSettings({ presets: [], presetsLoading: false })
    expect(screen.queryByText('빠른 시작')).not.toBeInTheDocument()
  })

  it('renders preset chips when presets provided', () => {
    renderSettings({ presets: PRESETS })
    expect(screen.getByText('프리셋A')).toBeInTheDocument()
    expect(screen.getByText('프리셋B')).toBeInTheDocument()
  })

  it('calls onQuickStart with correct preset when chip clicked', () => {
    const onQuickStart = vi.fn()
    renderSettings({ presets: PRESETS, onQuickStart })
    fireEvent.click(screen.getByText('프리셋A'))
    expect(onQuickStart).toHaveBeenCalledWith(PRESETS[0])
  })

  it('disables all chips when busy', () => {
    renderSettings({ presets: PRESETS, busy: true })
    const buttons = screen.getAllByRole('button', { name: /프리셋/ })
    buttons.forEach(btn => expect(btn).toBeDisabled())
  })

  it('shows skeleton placeholders when presetsLoading', () => {
    const { container } = renderSettings({ presetsLoading: true })
    expect(container.querySelectorAll('.skeleton').length).toBeGreaterThan(0)
    expect(screen.queryByText('프리셋A')).not.toBeInTheDocument()
  })
})

// ── Camera select ──

describe('Camera profile select', () => {
  it('renders all camera options', () => {
    renderSettings({ cameras: CAMERAS })
    expect(screen.getByRole('option', { name: 'cam1.cam' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'cam2.cam' })).toBeInTheDocument()
  })

  it('shows loading option and is disabled when camerasLoading', () => {
    renderSettings({ camerasLoading: true })
    expect(screen.getByRole('option', { name: '로딩 중…' })).toBeInTheDocument()
    expect(screen.getByRole('combobox')).toBeDisabled()
  })

  it('calls onChange with selected value', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.change(screen.getByRole('combobox'), { target: { value: 'cam2.cam' } })
    expect(onChange).toHaveBeenCalledWith('profileId', 'cam2.cam')
  })
})

// ── Format radio ──

describe('Format selection', () => {
  it('renders PNG, BMP, RAW radio buttons', () => {
    renderSettings()
    expect(screen.getByRole('radio', { name: 'PNG' })).toBeInTheDocument()
    expect(screen.getByRole('radio', { name: 'BMP' })).toBeInTheDocument()
    expect(screen.getByRole('radio', { name: 'RAW' })).toBeInTheDocument()
  })

  it('calls onChange("format", "bmp") when BMP selected', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.click(screen.getByRole('radio', { name: 'BMP' }))
    expect(onChange).toHaveBeenCalledWith('format', 'bmp')
  })

  it('reflects current format from config', () => {
    renderSettings({ config: { ...BASE_CONFIG, format: 'bmp' } })
    expect(screen.getByRole('radio', { name: 'BMP' })).toBeChecked()
    expect(screen.getByRole('radio', { name: 'PNG' })).not.toBeChecked()
  })
})

// ── Trigger mode ──

describe('Trigger mode', () => {
  it('renders 자동 and 수동 radio buttons', () => {
    renderSettings()
    expect(screen.getByRole('radio', { name: /자동/ })).toBeInTheDocument()
    expect(screen.getByRole('radio', { name: /수동/ })).toBeInTheDocument()
  })

  it('shows interval input when 자동 selected', () => {
    renderSettings({ config: { ...BASE_CONFIG, acquisitionMode: 'auto' } })
    expect(screen.getByPlaceholderText('1')).toBeInTheDocument()
  })

  it('hides interval input when 수동 selected', () => {
    renderSettings({ config: { ...BASE_CONFIG, acquisitionMode: 'manual' } })
    expect(screen.queryByPlaceholderText('1')).not.toBeInTheDocument()
  })

  it('calls onChange("acquisitionMode", "auto") when 자동 clicked', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.click(screen.getByRole('radio', { name: /자동/ }))
    expect(onChange).toHaveBeenCalledWith('acquisitionMode', 'auto')
  })

  it('calls onChange("intervalMs", 500) when interval set to 0.5', () => {
    const onChange = vi.fn()
    renderSettings({
      onChange,
      config: { ...BASE_CONFIG, acquisitionMode: 'auto' },
    })
    fireEvent.change(screen.getByPlaceholderText('1'), { target: { value: '0.5' } })
    expect(onChange).toHaveBeenCalledWith('intervalMs', 500)
  })
})

// ── Start button ──

describe('Start button', () => {
  it('calls onStart when clicked and canStart is true', () => {
    const onStart = vi.fn()
    renderSettings({ onStart, canStart: true })
    fireEvent.click(screen.getByRole('button', { name: /촬영 시작/ }))
    expect(onStart).toHaveBeenCalledTimes(1)
  })

  it('is disabled when canStart is false', () => {
    renderSettings({ canStart: false })
    expect(screen.getByRole('button', { name: /촬영 시작/ })).toBeDisabled()
  })

  it('is disabled when busy is true', () => {
    renderSettings({ busy: true, canStart: true })
    expect(screen.getByRole('button', { name: /촬영 시작/ })).toBeDisabled()
  })
})

// ── Save preset modal ──

describe('Save preset modal', () => {
  it('opens modal when 프리셋으로 저장 clicked', () => {
    renderSettings()
    fireEvent.click(screen.getByRole('button', { name: /프리셋으로 저장/ }))
    expect(screen.getByRole('dialog')).toBeInTheDocument()
  })

  it('calls onSavePreset with trimmed name on 저장 click', () => {
    const onSavePreset = vi.fn()
    renderSettings({ onSavePreset })
    fireEvent.click(screen.getByRole('button', { name: /프리셋으로 저장/ }))
    fireEvent.change(screen.getByPlaceholderText('프리셋 이름'), { target: { value: '  내 프리셋  ' } })
    fireEvent.click(screen.getByRole('button', { name: /^저장/ }))
    expect(onSavePreset).toHaveBeenCalledWith('내 프리셋')
  })

  it('disables 저장 button when savingPreset is true', () => {
    renderSettings({ savingPreset: true })
    fireEvent.click(screen.getByRole('button', { name: /프리셋으로 저장/ }))
    expect(screen.getByRole('button', { name: /^저장/ })).toBeDisabled()
  })
})
```

- [ ] **Step 2: Run tests**

```bash
cd src/peanut-vision-app && npx vitest run src/test/AcquisitionSettings.test.tsx
```
Expected: all tests pass.

- [ ] **Step 3: Commit**

```bash
git add src/peanut-vision-app/src/test/AcquisitionSettings.test.tsx
git commit -m "test: rewrite AcquisitionSettings tests for new API"
```

---

## Task 8: Cleanup — delete AcquisitionActionBar and CameraProfileSelector

**Files:**
- Delete: `src/peanut-vision-app/src/components/shared/AcquisitionActionBar/` (entire directory)
- Delete: `src/peanut-vision-app/src/components/shared/CameraProfileSelector/` (entire directory)
- Delete: `src/peanut-vision-app/src/test/AcquisitionActionBar.test.tsx`

- [ ] **Step 1: Check for remaining imports**

```bash
grep -r "AcquisitionActionBar\|CameraProfileSelector" src/peanut-vision-app/src --include="*.tsx" --include="*.ts"
```
Expected: no results (both were used only in CaptureTab and AcquisitionSettings which are now rewritten).

- [ ] **Step 2: Delete the files**

```bash
rm -rf src/peanut-vision-app/src/components/shared/AcquisitionActionBar
rm -rf src/peanut-vision-app/src/components/shared/CameraProfileSelector
rm src/peanut-vision-app/src/test/AcquisitionActionBar.test.tsx
```

- [ ] **Step 3: Verify TypeScript and tests pass**

```bash
cd src/peanut-vision-app && npx tsc --noEmit && npx vitest run
```
Expected: no errors, all remaining tests pass.

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "refactor: remove AcquisitionActionBar and CameraProfileSelector — replaced by inline implementation"
```

---

## Self-Review Checklist

**Spec coverage:**
- [x] Right-side panel — Task 3
- [x] Image viewer fills remaining space — Task 3
- [x] Quick-start presets at top — Task 5
- [x] Preset chip → immediate start — Task 4 (onQuickStart → handleStartWithConfig)
- [x] Settings form with native inputs — Task 5
- [x] Trigger mode between settings and Start — Task 5
- [x] Loading states: camera select, presets skeleton, Start/Stop/Trigger spinners — Tasks 4, 5
- [x] Single Start button — Task 5
- [x] Save preset link + modal — Task 5
- [x] Active view: status + Trigger/Stop with spinners — Task 4

**No placeholders found.**

**Type consistency:** `AcquisitionSession.handleStartWithConfig(AcquisitionFormConfig)` defined in Task 2, used in Task 4 `onQuickStart`. Props interface in Task 5 matches what Task 4 passes. ✓
