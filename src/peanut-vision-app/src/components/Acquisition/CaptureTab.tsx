import { useState } from 'react'
import { Square, Save, Trash2 } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import Modal from '@/components/shared/Modal'
import AcquisitionActionBar from '@/components/shared/AcquisitionActionBar'
import CameraProfileSelector from '@/components/shared/CameraProfileSelector'
import AcquisitionModeSelector from '@/components/shared/AcquisitionModeSelector'
import ContinuousSettings from '@/components/shared/ContinuousSettings'
import StatusChip from '@/components/shared/StatusChip'
import type { AcquisitionConfigPreset } from '@/api/types'
import { getPresets, savePreset, deletePreset } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'
import type { AcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import type { AcquisitionSession } from '@/hooks/useAcquisitionSession'
import type { InputMode } from './index'
import cx from './cx'

interface Props {
  config: AcquisitionConfig
  session: AcquisitionSession
  inputMode: InputMode
  onInputModeChange: (mode: InputMode) => void
  selectedPreset: AcquisitionConfigPreset | null
  onPresetSelect: (preset: AcquisitionConfigPreset | null) => void
}

export default function CaptureTab(props: Props) {
  if (props.session.isActive) {
    return <ActiveView session={props.session} />
  }
  return <IdleView {...props} />
}

// ── 촬영 중: readonly 상태 표시 ──────────────────────────────────────────────

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
        <div className={cx('infoRow')}><dt>Trigger</dt><dd>{s?.triggerMode ?? '—'}</dd></div>
        <div className={cx('infoRow')}>
          <dt>Frame count</dt>
          <dd>{s?.activeFrameCount != null ? s.activeFrameCount : '∞'}</dd>
        </div>
        <div className={cx('infoRow')}>
          <dt>Interval</dt>
          <dd>{s?.activeIntervalMs != null ? `${s.activeIntervalMs} ms` : 'manual'}</dd>
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
        <button type="button" className={cx('triggerBtn')} onClick={session.handleTrigger} disabled={session.busy}>
          Trigger
        </button>
      )}
      <button type="button" className={cx('stopBtn')} onClick={session.handleStop} disabled={session.busy}>
        <Square size={14} /> Stop
      </button>
    </div>
  )
}

// ── 대기 중: 입력 모드 선택 ──────────────────────────────────────────────────

function IdleView({ config, session, inputMode, onInputModeChange, selectedPreset, onPresetSelect }: Props) {
  return (
    <>
      <InputModeToggle mode={inputMode} onChange={onInputModeChange} />
      {inputMode === 'manual'
        ? <ManualForm config={config} session={session} />
        : <PresetForm session={session} selected={selectedPreset} onSelect={onPresetSelect} />
      }
    </>
  )
}

// ── 입력 모드 토글 ─────────────────────────────────────────────────────────

function InputModeToggle({ mode, onChange }: { mode: InputMode; onChange: (m: InputMode) => void }) {
  return (
    <div className={cx('modeToggle')}>
      <button
        type="button"
        className={cx('modeBtn', { active: mode === 'manual' })}
        onClick={() => onChange('manual')}
      >
        Manual
      </button>
      <button
        type="button"
        className={cx('modeBtn', { active: mode === 'preset' })}
        onClick={() => onChange('preset')}
      >
        Preset
      </button>
    </div>
  )
}

// ── Manual 모드: 폼 입력 ───────────────────────────────────────────────────

function ManualForm({ config, session }: { config: AcquisitionConfig; session: AcquisitionSession }) {
  const { toast } = useToast()
  const queryClient = useQueryClient()
  const [saveOpen, setSaveOpen] = useState(false)
  const [presetName, setPresetName] = useState('')

  const saveMutation = useMutation({
    mutationFn: (preset: AcquisitionConfigPreset) => savePreset(preset),
    onSuccess: () => {
      setPresetName('')
      setSaveOpen(false)
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      toast('프리셋이 저장되었습니다', 'success')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '저장 실패', 'error'),
  })

  const handleSave = () => {
    if (!presetName.trim()) return
    saveMutation.mutate({
      name: presetName.trim(),
      profileId: config.selectedProfile,
      triggerMode: config.triggerMode,
      frameCount: config.frameCount,
      intervalMs: config.intervalMs,
    })
  }

  return (
    <>
      <CameraProfileSelector
        cameras={config.cameras}
        selectedProfile={config.selectedProfile}
        onProfileChange={config.setSelectedProfile}
        disabled={false}
      />
      <AcquisitionModeSelector
        triggerMode={config.triggerMode}
        onTriggerModeChange={config.setTriggerMode}
        disabled={false}
      />
      <AcquisitionActionBar
        isActive={false}
        profileLabel={config.selectedProfile}
        canStart={session.canStart}
        canStop={session.canStop}
        canTrigger={session.canTrigger}
        continuousSubMode={config.continuousSubMode}
        busy={session.busy}
        onStart={session.handleStart}
        onStop={session.handleStop}
        onTrigger={session.handleTrigger}
        onRefresh={session.refresh}
        refreshThrottled={false}
        hasWarnings={session.hasWarnings}
        hasErrors={session.hasErrors}
      />
      <ContinuousSettings
        subMode={config.continuousSubMode}
        onSubModeChange={config.setContinuousSubMode}
        frameCount={config.frameCount}
        onFrameCountChange={config.setFrameCount}
        intervalMs={config.intervalMs}
        onIntervalMsChange={config.setIntervalMs}
        disabled={false}
      />
      <button
        type="button"
        className={cx('savePresetBtn')}
        onClick={() => setSaveOpen(true)}
        disabled={!config.selectedProfile}
      >
        <Save size={13} /> Save as Preset
      </button>

      <Modal
        open={saveOpen}
        onClose={() => setSaveOpen(false)}
        title="Save Acquisition Preset"
        actions={
          <>
            <button type="button" className={cx('btn')} onClick={() => setSaveOpen(false)}>Cancel</button>
            <button
              type="button"
              className={cx('btn', 'primary')}
              onClick={handleSave}
              disabled={saveMutation.isPending || !presetName.trim()}
            >
              Save
            </button>
          </>
        }
      >
        <input
          type="text"
          className={cx('field')}
          placeholder="Preset name"
          value={presetName}
          onChange={(e) => setPresetName(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleSave()}
          autoFocus
        />
        <p className={cx('meta')}>
          {[
            config.selectedProfile || 'none',
            config.triggerMode,
            config.frameCount != null ? `${config.frameCount} frames` : null,
            config.intervalMs != null ? `${config.intervalMs}ms` : null,
          ].filter(Boolean).join(' | ')}
        </p>
      </Modal>
    </>
  )
}

// ── Preset 모드: 프리셋 선택 ────────────────────────────────────────────────

function PresetForm({
  session,
  selected,
  onSelect,
}: {
  session: AcquisitionSession
  selected: AcquisitionConfigPreset | null
  onSelect: (p: AcquisitionConfigPreset | null) => void
}) {
  const { toast } = useToast()
  const queryClient = useQueryClient()

  const { data: presets = [] } = useQuery({
    queryKey: queryKeys.presets,
    queryFn: getPresets,
  })

  const deleteMutation = useMutation({
    mutationFn: (name: string) => deletePreset(name),
    onSuccess: (_, name) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      if (selected?.name === name) onSelect(null)
      toast('프리셋이 삭제되었습니다', 'info')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '삭제 실패', 'error'),
  })

  const canStart = !!selected && session.canStart

  return (
    <>
      {presets.length === 0 ? (
        <p className={cx('empty')}>저장된 프리셋이 없습니다. Manual 모드에서 저장하세요.</p>
      ) : (
        <ul className={cx('presetList')}>
          {presets.map((p) => (
            <li
              key={p.name}
              className={cx('presetItem', { selected: selected?.name === p.name })}
              onClick={() => onSelect(p)}
            >
              <div className={cx('presetInfo')}>
                <span className={cx('presetName')}>{p.name}</span>
                <span className={cx('presetMeta')}>
                  {[
                    p.profileId,
                    p.triggerMode ?? 'soft',
                    p.frameCount != null ? `${p.frameCount} frames` : '∞',
                    p.intervalMs != null ? `${p.intervalMs}ms` : null,
                  ].filter(Boolean).join(' · ')}
                </span>
              </div>
              <button
                type="button"
                className={cx('deleteBtn')}
                onClick={(e) => { e.stopPropagation(); deleteMutation.mutate(p.name) }}
                disabled={deleteMutation.isPending}
                title="Delete preset"
              >
                <Trash2 size={13} />
              </button>
            </li>
          ))}
        </ul>
      )}

      <button
        type="button"
        className={cx('btn', 'success', 'startBtn')}
        onClick={session.handleStart}
        disabled={session.busy || !canStart}
      >
        Start
      </button>
    </>
  )
}
