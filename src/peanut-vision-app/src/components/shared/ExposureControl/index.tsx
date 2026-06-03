import type { ExposureInfo } from '@/api/types'
import { DEFAULT_EXPOSURE_MIN, DEFAULT_EXPOSURE_MAX } from '@/constants'
import cx from './cx'

interface Props {
  exposure: ExposureInfo | null
  exposureValue: number
  isActive: boolean
  busy: boolean
  isCalibrationAvailable: boolean
  onExposureChange: (value: number) => void
  onLoad: () => void
  onApply: () => void
}

export default function ExposureControl({
  exposure,
  exposureValue,
  isActive,
  busy,
  isCalibrationAvailable,
  onExposureChange,
  onLoad,
  onApply,
}: Props) {
  const min = exposure?.exposureRange?.min ?? DEFAULT_EXPOSURE_MIN
  const max = exposure?.exposureRange?.max ?? DEFAULT_EXPOSURE_MAX

  return (
    <div className={cx('card')}>
      <div className={cx('header')}>
        <span className={cx('title')}>Exposure</span>
        <span className={cx('badge', isActive ? 'live' : 'pending')}>
          {isActive ? 'Live' : 'Pending'}
        </span>
        <button
          type="button"
          className={cx('textBtn')}
          onClick={onLoad}
          disabled={busy || !isActive}
        >
          Load Current
        </button>
      </div>

      <div className={cx('body')}>
        <label className={cx('sliderLabel')}>
          Exposure ({exposureValue.toFixed(0)} µs)
        </label>
        <input
          type="range"
          min={min}
          max={max}
          step={10}
          value={exposureValue}
          onChange={(e) => onExposureChange(Number(e.target.value))}
          className={cx('slider')}
        />
        {exposure?.exposureRange && (
          <span className={cx('hint')}>
            Range: {min} – {max} µs
          </span>
        )}
        <button
          type="button"
          className={cx('btn', 'primary')}
          onClick={onApply}
          disabled={busy || !isCalibrationAvailable}
        >
          {isActive ? 'Apply Settings' : 'Apply on Start'}
        </button>
      </div>
    </div>
  )
}
