import { Play, Square, Crosshair, RefreshCw } from 'lucide-react'
import StatusChip from '@/components/shared/StatusChip'
import type { ShootingMode } from '@/api/types'
import cx from './cx'

interface Props {
  isActive: boolean
  profileLabel?: string
  canStart: boolean
  canStop: boolean
  canTrigger: boolean
  shootingMode: ShootingMode
  busy: boolean
  onStart: () => void
  onStop: () => void
  onTrigger: () => void
  onRefresh: () => void
  refreshThrottled: boolean
  hasWarnings?: boolean
  hasErrors?: boolean
}

export default function AcquisitionActionBar({
  isActive,
  profileLabel,
  canStart,
  canStop,
  canTrigger,
  shootingMode,
  busy,
  onStart,
  onStop,
  onTrigger,
  onRefresh,
  refreshThrottled,
  hasWarnings,
  hasErrors,
}: Props) {
  return (
    <div className={cx('bar')}>
      <div className={cx('group')}>
        {canStop ? (
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
            disabled={busy || !canStart}
          >
            <Play size={14} /> Start
          </button>
        )}
        {canTrigger && shootingMode === 'manual' && (
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

      <StatusChip
        active={isActive}
        label={isActive ? `Active (${profileLabel ?? ''})` : 'Inactive'}
        hasWarnings={hasWarnings}
        hasErrors={hasErrors}
      />

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
