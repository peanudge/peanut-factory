import { Play, Square, Camera, Crosshair, RefreshCw } from 'lucide-react'
import StatusChip from '@/components/shared/StatusChip'
import type {
  AcquisitionAction,
  AcquisitionMode,
  AcquisitionStatus,
  ContinuousSubMode,
} from '@/api/types'
import cx from './cx'

interface Props {
  mode: AcquisitionMode
  continuousSubMode: ContinuousSubMode
  selectedProfile: string
  status: AcquisitionStatus | null
  busy: boolean
  onCapture: () => void
  onStart: () => void
  onStop: () => void
  onTrigger: () => void
  onRefresh: () => void
  refreshThrottled: boolean
  hasWarnings?: boolean
  hasErrors?: boolean
}

export default function AcquisitionActionBar({
  mode,
  continuousSubMode,
  selectedProfile,
  status,
  busy,
  onCapture,
  onStart,
  onStop,
  onTrigger,
  onRefresh,
  refreshThrottled,
  hasWarnings,
  hasErrors,
}: Props) {
  const allowed = (action: AcquisitionAction) =>
    status?.allowedActions?.includes(action) ?? false

  return (
    <div className={cx('bar')}>
      {mode === 'single' ? (
        <button
          type="button"
          className={cx('btn', 'primary')}
          onClick={onCapture}
          disabled={busy || !allowed('start') || !selectedProfile}
        >
          <Camera size={14} /> Capture
        </button>
      ) : (
        <div className={cx('group')}>
          {allowed('stop') ? (
            <button
              type="button"
              className={cx('btn', 'danger')}
              onClick={onStop}
              disabled={busy}
            >
              <Square size={14} /> Stop
            </button>
          ) : (
            <button
              type="button"
              className={cx('btn', 'success')}
              onClick={onStart}
              disabled={busy || !allowed('start') || !selectedProfile}
            >
              <Play size={14} /> Start
            </button>
          )}
          {allowed('trigger') && continuousSubMode === 'manual' && (
            <button
              type="button"
              className={cx('btn')}
              onClick={onTrigger}
              disabled={busy}
            >
              <Crosshair size={14} /> Trigger
            </button>
          )}
        </div>
      )}

      {status && (
        <StatusChip
          active={status.isActive}
          label={status.isActive ? `Active (${status.profileId ?? ''})` : 'Inactive'}
          hasWarnings={hasWarnings}
          hasErrors={hasErrors}
        />
      )}

      <button
        type="button"
        className={cx('iconBtn')}
        onClick={onRefresh}
        disabled={refreshThrottled}
      >
        <RefreshCw size={14} />
      </button>
    </div>
  )
}
