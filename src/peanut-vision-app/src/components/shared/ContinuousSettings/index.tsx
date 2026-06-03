import { useState } from 'react'
import type { ContinuousSubMode } from '@/api/types'
import cx from './cx'

interface Props {
  subMode: ContinuousSubMode
  onSubModeChange: (value: ContinuousSubMode) => void
  frameCount: number | null
  onFrameCountChange: (value: number | null) => void
  intervalMs: number | null
  onIntervalMsChange: (value: number | null) => void
  disabled?: boolean
}

export default function ContinuousSettings({
  subMode,
  onSubModeChange,
  frameCount,
  onFrameCountChange,
  intervalMs,
  onIntervalMsChange,
  disabled,
}: Props) {
  const [infinite, setInfinite] = useState(frameCount === null)

  const handleInfiniteChange = (checked: boolean) => {
    setInfinite(checked)
    onFrameCountChange(checked ? null : 10)
  }

  return (
    <div className={cx('wrap')}>
      <div className={cx('toggleGroup')}>
        {(['auto', 'manual'] as ContinuousSubMode[]).map((v) => (
          <button
            key={v}
            type="button"
            className={cx('toggle', { active: subMode === v })}
            onClick={() => onSubModeChange(v)}
            disabled={disabled}
          >
            {v.charAt(0).toUpperCase() + v.slice(1)}
          </button>
        ))}
      </div>

      <div className={cx('field')}>
        <label>Frame Count</label>
        <input
          type="number"
          min={1}
          value={infinite ? '' : (frameCount ?? '')}
          onChange={(e) => {
            const v = parseInt(e.target.value, 10)
            onFrameCountChange(isNaN(v) || v < 1 ? null : v)
          }}
          disabled={disabled || infinite}
        />
      </div>

      <label className={cx('checkLabel')}>
        <input
          type="checkbox"
          checked={infinite}
          onChange={(e) => handleInfiniteChange(e.target.checked)}
          disabled={disabled}
        />
        Infinite
      </label>

      {subMode === 'auto' && (
        <div className={cx('field')}>
          <label>Interval (ms)</label>
          <input
            type="number"
            min={50}
            value={intervalMs ?? ''}
            onChange={(e) => {
              const v = parseInt(e.target.value, 10)
              onIntervalMsChange(isNaN(v) || v < 0 ? null : v)
            }}
            disabled={disabled}
          />
          <small>최소 50ms</small>
        </div>
      )}
    </div>
  )
}
