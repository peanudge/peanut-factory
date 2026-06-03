import type { TriggerModeOption } from '@/api/types'
import cx from './cx'

interface Props {
  triggerMode: TriggerModeOption
  onTriggerModeChange: (mode: TriggerModeOption) => void
  disabled?: boolean
}

export default function AcquisitionModeSelector({
  triggerMode,
  onTriggerModeChange,
  disabled,
}: Props) {
  return (
    <div className={cx('AcquisitionModeSelector')}>
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
