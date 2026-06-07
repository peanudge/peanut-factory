import { Play, Square, Crosshair } from 'lucide-react'
import type { AcquisitionMode } from '@/api/types'
import cx from './cx'

interface Props {
  canStart: boolean
  canStop: boolean
  canTrigger: boolean
  acquisitionMode: AcquisitionMode
  busy: boolean
  onStart: () => void
  onStop: () => void
  onTrigger: () => void
}

export default function AcquisitionActionBar({
  canStart,
  canStop,
  canTrigger,
  acquisitionMode,
  busy,
  onStart,
  onStop,
  onTrigger,
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
        {canTrigger && acquisitionMode === 'manual' && (
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
    </div>
  )
}
