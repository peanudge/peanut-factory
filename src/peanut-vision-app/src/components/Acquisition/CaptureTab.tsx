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
      onQuickStart={(preset: AcquisitionConfigPreset) => session.handleStartWithConfig(presetToFormConfig(preset))}
      canStart={session.canStart}
      busy={session.busy}
      onStart={session.handleStart}
      onSavePreset={(name: string) =>
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
