import { useState } from 'react'
import { FolderSearch } from 'lucide-react'
import type { AcquisitionMode, SaveImageFormat } from '@/api/types'
import DirectoryBrowser from '@/components/shared/DirectoryBrowser'
import cx from './cx'

const FORMAT_OPTIONS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

interface Props {
  acquisitionMode: AcquisitionMode
  onAcquisitionModeChange: (value: AcquisitionMode) => void
  frameCount: number | null
  onFrameCountChange: (value: number | null) => void
  intervalMs: number | null
  onIntervalMsChange: (value: number | null) => void
  outputDirectory: string
  onOutputDirectoryChange: (value: string) => void
  format: SaveImageFormat
  onFormatChange: (value: SaveImageFormat) => void
  autoSave: boolean
  onAutoSaveChange: (value: boolean) => void
  disabled?: boolean
}

export default function AcquisitionSettings({
  acquisitionMode,
  onAcquisitionModeChange,
  frameCount,
  onFrameCountChange,
  intervalMs,
  onIntervalMsChange,
  outputDirectory,
  onOutputDirectoryChange,
  format,
  onFormatChange,
  autoSave,
  onAutoSaveChange,
  disabled,
}: Props) {
  const [browserOpen, setBrowserOpen] = useState(false)

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

      {/* Save settings */}
      <div className={cx('saveSection')}>
        <div className={cx('field')}>
          <label>저장 경로</label>
          <div className={cx('dirRow')}>
            <input
              type="text"
              value={outputDirectory}
              onChange={(e) => onOutputDirectoryChange(e.target.value)}
              disabled={disabled}
              placeholder="CapturedImages"
            />
            <button
              type="button"
              className={cx('browseBtn')}
              onClick={() => setBrowserOpen(true)}
              disabled={disabled}
              title="Browse"
            >
              <FolderSearch size={14} />
            </button>
          </div>
          <small>Tokens: {'{date}'} → yyyy-MM-dd, {'{profile}'} → cam file name</small>
        </div>

        <div className={cx('saveRow')}>
          <div className={cx('field')}>
            <label>포맷</label>
            <select
              value={format}
              onChange={(e) => onFormatChange(e.target.value as SaveImageFormat)}
              disabled={disabled}
            >
              {FORMAT_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
          <label className={cx('checkLabel')}>
            <input
              type="checkbox"
              checked={autoSave}
              onChange={(e) => onAutoSaveChange(e.target.checked)}
              disabled={disabled}
            />
            자동 저장
          </label>
        </div>
      </div>

      <DirectoryBrowser
        open={browserOpen}
        currentPath={outputDirectory}
        onSelect={onOutputDirectoryChange}
        onClose={() => setBrowserOpen(false)}
      />
    </div>
  )
}
