import type { CamFileInfo } from '@/api/types'
import cx from './cx'

interface Props {
  cameras: CamFileInfo[]
  selectedProfile: string
  onProfileChange: (id: string) => void
  disabled?: boolean
}

export default function CameraProfileSelector({
  cameras,
  selectedProfile,
  onProfileChange,
  disabled,
}: Props) {
  return (
    <div className={cx('CameraProfileSelector')}>
      <label className={cx('label')}>Camera Profile</label>
      <select
        value={selectedProfile}
        onChange={(e) => onProfileChange(e.target.value)}
        disabled={disabled}
        className={cx('select')}
      >
        {cameras.map((c) => (
          <option key={c.fileName} value={c.fileName}>
            {c.fileName}
          </option>
        ))}
      </select>
    </div>
  )
}
