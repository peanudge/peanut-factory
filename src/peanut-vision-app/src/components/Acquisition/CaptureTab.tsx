import { useState } from 'react'
import { Loader2, Square, Trash2 } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import StatusChip from '@/components/shared/StatusChip'
import Modal from '@/components/shared/Modal'
import type { AcquisitionConfigPreset, AcquisitionFormConfig } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'
import { getPresets, savePreset, deletePreset } from '@/api/client'
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

// ── Idle: tabbed layout ───────────────────────────────────────────────────────

type Tab = 'settings' | 'presets'

function IdleView({ acqConfig, session }: Props) {
  const [activeTab, setActiveTab] = useState<Tab>('settings')
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
      toast('설정이 저장되었습니다', 'success')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '저장 실패', 'error'),
  })

  const deleteMutation = useMutation({
    mutationFn: deletePreset,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      toast('설정이 삭제되었습니다', 'info')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '삭제 실패', 'error'),
  })

  return (
    <div className={cx('idleWrap')}>
      <div className={cx('tabBar')}>
        <button
          type="button"
          className={cx('tabBtn', { tabActive: activeTab === 'settings' })}
          onClick={() => setActiveTab('settings')}
        >
          설정
        </button>
        <button
          type="button"
          className={cx('tabBtn', { tabActive: activeTab === 'presets' })}
          onClick={() => setActiveTab('presets')}
        >
          저장된 설정
          {presets.length > 0 && (
            <span className={cx('tabBadge')}>{presets.length}</span>
          )}
        </button>
      </div>

      <div className={cx('tabContent')}>
        {activeTab === 'settings' ? (
          <AcquisitionSettings
            config={acqConfig.config}
            onChange={acqConfig.updateConfig}
            cameras={acqConfig.cameras}
            camerasLoading={acqConfig.camerasLoading}
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
        ) : (
          <PresetTab
            presets={presets}
            presetsLoading={presetsLoading}
            busy={session.busy}
            onStart={(preset) => session.handleStartWithConfig(presetToFormConfig(preset))}
            onDelete={(name) => deleteMutation.mutate(name)}
            deleting={deleteMutation.isPending}
          />
        )}
      </div>
    </div>
  )
}

// ── Preset tab ────────────────────────────────────────────────────────────────

interface PresetTabProps {
  presets: AcquisitionConfigPreset[]
  presetsLoading: boolean
  busy: boolean
  onStart: (preset: AcquisitionConfigPreset) => void
  onDelete: (name: string) => void
  deleting: boolean
}

function PresetTab({ presets, presetsLoading, busy, onStart, onDelete, deleting }: PresetTabProps) {
  const [confirmPreset, setConfirmPreset] = useState<AcquisitionConfigPreset | null>(null)

  if (presetsLoading) {
    return (
      <div className={cx('presetSkeletons')}>
        <div className={cx('presetSkeleton')} />
        <div className={cx('presetSkeleton')} />
      </div>
    )
  }

  if (presets.length === 0) {
    return (
      <p className={cx('presetsEmpty')}>
        저장된 설정이 없습니다.<br />
        설정 탭에서 현재 설정을 저장해보세요.
      </p>
    )
  }

  return (
    <>
      <ul className={cx('presetList')}>
        {presets.map((p) => (
          <li key={p.name} className={cx('presetItem')}>
            <button
              type="button"
              className={cx('presetNameBtn')}
              onClick={() => setConfirmPreset(p)}
              disabled={busy || deleting}
            >
              <span className={cx('presetItemName')}>{p.name}</span>
              <span className={cx('presetItemMeta')}>
                {[
                  p.profileId,
                  p.intervalMs != null ? `${p.intervalMs / 1000}s 간격` : '수동',
                  p.frameCount != null ? `${p.frameCount}장` : null,
                ].filter(Boolean).join(' · ')}
              </span>
            </button>
            <button
              type="button"
              className={cx('presetDeleteBtn')}
              onClick={() => onDelete(p.name)}
              disabled={deleting || busy}
              title="삭제"
            >
              {deleting
                ? <Loader2 size={13} className={cx('spin')} />
                : <Trash2 size={13} />}
            </button>
          </li>
        ))}
      </ul>

      <Modal
        open={confirmPreset !== null}
        onClose={() => setConfirmPreset(null)}
        title={confirmPreset?.name ?? ''}
        actions={
          <>
            <button type="button" onClick={() => setConfirmPreset(null)}>
              취소
            </button>
            <button
              type="button"
              onClick={() => {
                if (!confirmPreset) return
                onStart(confirmPreset)
                setConfirmPreset(null)
              }}
              disabled={busy}
            >
              {busy && <Loader2 size={13} className={cx('spin')} />}
              이 설정으로 촬영 시작
            </button>
          </>
        }
      >
        {confirmPreset && (
          <dl className={cx('presetDetail')}>
            <div><dt>카메라</dt><dd>{confirmPreset.profileId}</dd></div>
            <div><dt>포맷</dt><dd>{(confirmPreset.format ?? 'png').toUpperCase()}</dd></div>
            <div>
              <dt>촬영 방식</dt>
              <dd>
                {confirmPreset.intervalMs != null
                  ? `자동 (${confirmPreset.intervalMs / 1000}초 간격)`
                  : '수동'}
              </dd>
            </div>
            <div>
              <dt>프레임 수</dt>
              <dd>{confirmPreset.frameCount != null ? `${confirmPreset.frameCount}장` : '제한 없음'}</dd>
            </div>
            {confirmPreset.outputDirectory && (
              <div><dt>저장 경로</dt><dd>{confirmPreset.outputDirectory}</dd></div>
            )}
          </dl>
        )}
      </Modal>
    </>
  )
}
