import { Square } from 'lucide-react'
import AcquisitionActionBar from '@/components/shared/AcquisitionActionBar'
import CameraProfileSelector from '@/components/shared/CameraProfileSelector'
import AcquisitionModeSelector from '@/components/shared/AcquisitionModeSelector'
import ContinuousSettings from '@/components/shared/ContinuousSettings'
import PresetSelector from '@/components/shared/PresetSelector'
import StatusChip from '@/components/shared/StatusChip'
import type { AcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import type { AcquisitionSession } from '@/hooks/useAcquisitionSession'
import cx from './cx'

interface Props {
  config: AcquisitionConfig
  session: AcquisitionSession
}

export default function CaptureTab({ config, session }: Props) {
  if (session.isActive) {
    return <ActiveView session={session} />
  }
  return <ConfigForm config={config} session={session} />
}

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
        <div className={cx('infoRow')}>
          <dt>Profile</dt>
          <dd>{s?.profileId ?? '—'}</dd>
        </div>
        <div className={cx('infoRow')}>
          <dt>Trigger</dt>
          <dd>{s?.triggerMode ?? '—'}</dd>
        </div>
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
            <div className={cx('infoRow')}>
              <dt>Frames</dt>
              <dd>{s.statistics.frameCount}</dd>
            </div>
            <div className={cx('infoRow')}>
              <dt>FPS</dt>
              <dd>{s.statistics.averageFps.toFixed(1)}</dd>
            </div>
            {s.statistics.droppedFrameCount > 0 && (
              <div className={cx('infoRow', 'warn')}>
                <dt>Dropped</dt>
                <dd>{s.statistics.droppedFrameCount}</dd>
              </div>
            )}
          </>
        )}
        {s?.lastError && (
          <div className={cx('infoRow', 'error')}>
            <dt>Error</dt>
            <dd>{s.lastError}</dd>
          </div>
        )}
      </dl>

      {s?.activeIntervalMs == null && (
        <button
          type="button"
          className={cx('triggerBtn')}
          onClick={session.handleTrigger}
          disabled={session.busy}
        >
          Trigger
        </button>
      )}

      <button
        type="button"
        className={cx('stopBtn')}
        onClick={session.handleStop}
        disabled={session.busy}
      >
        <Square size={14} /> Stop
      </button>
    </div>
  )
}

function ConfigForm({ config, session }: Props) {
  return (
    <>
      <PresetSelector
        profileId={config.selectedProfile}
        triggerMode={config.triggerMode}
        frameCount={config.frameCount}
        intervalMs={config.intervalMs}
        onLoadPreset={config.handleLoadPreset}
        disabled={false}
      />
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
    </>
  )
}
