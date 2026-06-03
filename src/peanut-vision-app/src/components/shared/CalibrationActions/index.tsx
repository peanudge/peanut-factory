import cx from './cx'

interface Props {
  busy: boolean
  isCalibrationAvailable: boolean
  ffcEnabled: boolean
  onBlack: () => void
  onWhite: () => void
  onWhiteBalance: () => void
  onFfcToggle: (_: unknown, checked: boolean) => void
}

export default function CalibrationActions({
  busy,
  isCalibrationAvailable,
  ffcEnabled,
  onBlack,
  onWhite,
  onWhiteBalance,
  onFfcToggle,
}: Props) {
  const dis = busy || !isCalibrationAvailable

  return (
    <div className={cx('card')}>
      <h4 className={cx('title')}>Calibration Actions</h4>
      <div className={cx('stack')}>
        <div className={cx('item')}>
          <button type="button" className={cx('btn')} disabled={dis} onClick={onBlack}>
            Black Calibration
          </button>
          <small className={cx('hint')}>Cover the lens before executing</small>
        </div>
        <div className={cx('item')}>
          <button type="button" className={cx('btn')} disabled={dis} onClick={onWhite}>
            White Calibration
          </button>
          <small className={cx('hint')}>Ensure uniform ~200DN illumination</small>
        </div>
        <div className={cx('item')}>
          <button type="button" className={cx('btn')} disabled={dis} onClick={onWhiteBalance}>
            White Balance (Once)
          </button>
          <small className={cx('hint')}>Point lens at white target (~200DN)</small>
        </div>
        <label className={cx('checkLabel')}>
          <input
            type="checkbox"
            checked={ffcEnabled}
            onChange={(e) => onFfcToggle(null, e.target.checked)}
            disabled={dis}
          />
          Flat Field Correction (FFC)
        </label>
      </div>
    </div>
  )
}
