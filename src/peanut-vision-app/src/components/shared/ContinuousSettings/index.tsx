import type { AcquisitionMode } from '@/api/types'
import cx from './cx'

interface Props {
  acquisitionMode: AcquisitionMode
  onAcquisitionModeChange: (value: AcquisitionMode) => void
  frameCount: number | null
  onFrameCountChange: (value: number | null) => void
  intervalMs: number | null
  onIntervalMsChange: (value: number | null) => void
  disabled?: boolean
}

export default function ContinuousSettings({
  acquisitionMode,
  onAcquisitionModeChange,
  frameCount,
  onFrameCountChange,
  intervalMs,
  onIntervalMsChange,
  disabled,
}: Props) {
  return (
    <div className={cx('wrap')}>
      {/* Mode selector */}
      <div className={cx('modeRow')}>
        <button
          type="button"
          className={cx('modeCard', { active: acquisitionMode === 'auto' })}
          onClick={() => onAcquisitionModeChange('auto')}
          disabled={disabled}
        >
          <span className={cx('modeIcon')}>🔄</span>
          <span className={cx('modeLabel')}>자동 촬영</span>
          <span className={cx('modeDesc')}>N초 간격 반복</span>
        </button>
        <button
          type="button"
          className={cx('modeCard', { active: acquisitionMode === 'manual' })}
          onClick={() => onAcquisitionModeChange('manual')}
          disabled={disabled}
        >
          <span className={cx('modeIcon')}>👆</span>
          <span className={cx('modeLabel')}>수동 촬영</span>
          <span className={cx('modeDesc')}>트리거로 한 장씩</span>
        </button>
      </div>

      {/* Auto settings */}
      {acquisitionMode === 'auto' && (
        <div className={cx('autoFields')}>
          <div className={cx('field')}>
            <label>Interval</label>
            <div className={cx('inputRow')}>
              <input
                type="number"
                min={50}
                placeholder="500"
                value={intervalMs ?? ''}
                onChange={(e) => {
                  const v = parseInt(e.target.value, 10)
                  onIntervalMsChange(isNaN(v) || v < 0 ? null : v)
                }}
                disabled={disabled}
              />
              <span className={cx('unit')}>ms</span>
            </div>
            <small>최소 50ms</small>
          </div>
          <div className={cx('field')}>
            <label>Stop after</label>
            <div className={cx('inputRow')}>
              <input
                type="number"
                min={1}
                placeholder="∞"
                value={frameCount ?? ''}
                onChange={(e) => {
                  const v = parseInt(e.target.value, 10)
                  onFrameCountChange(isNaN(v) || v < 1 ? null : v)
                }}
                disabled={disabled}
              />
              <span className={cx('unit')}>frames</span>
            </div>
            <small>비우면 제한없음</small>
          </div>
        </div>
      )}

      {/* Manual description */}
      {acquisitionMode === 'manual' && (
        <p className={cx('manualDesc')}>
          Start 후 화면 상단 <strong>Trigger</strong> 버튼으로 한 장씩 촬영합니다.
        </p>
      )}
    </div>
  )
}
