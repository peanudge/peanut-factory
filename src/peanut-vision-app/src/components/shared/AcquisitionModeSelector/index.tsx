import type { AcquisitionMode, TriggerModeOption } from '@/api/types'
import cx from './cx'

interface Props {
  mode: AcquisitionMode
  onModeChange: (mode: AcquisitionMode) => void
  triggerMode: TriggerModeOption
  onTriggerModeChange: (mode: TriggerModeOption) => void
  disabled?: boolean
}

export default function AcquisitionModeSelector({
  mode,
  onModeChange,
  triggerMode,
  onTriggerModeChange,
  disabled,
}: Props) {
  return (
    <div className={cx('AcquisitionModeSelector')}>
      <div className={cx('toggleGroup')}>
        {(['single', 'continuous'] as AcquisitionMode[]).map((m) => (
          <button
            key={m}
            type="button"
            className={cx('toggle', { active: mode === m })}
            onClick={() => !disabled && onModeChange(m)}
            disabled={disabled}
          >
            {m === 'single' ? 'Single' : 'Continuous'}
          </button>
        ))}
      </div>

      <select
        value={triggerMode}
        onChange={(e) => onTriggerModeChange(e.target.value as TriggerModeOption)}
        disabled={disabled}
        className={cx('select')}
      >
        <option value="soft">Soft</option>
        <option value="hard">Hard</option>
        <option value="combined">Combined</option>
      </select>
    </div>
  )
}
