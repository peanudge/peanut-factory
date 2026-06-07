import { useState } from 'react'
import { FolderSearch } from 'lucide-react'
import type { AcquisitionFormConfig, SaveImageFormat } from '@/api/types'
import DirectoryBrowser from '@/components/shared/DirectoryBrowser'
import cx from './cx'

const FORMAT_OPTIONS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

interface Props {
  config: AcquisitionFormConfig
  onChange: <K extends keyof AcquisitionFormConfig>(key: K, value: AcquisitionFormConfig[K]) => void
  disabled?: boolean
}

export default function AcquisitionSettings({ config, onChange, disabled }: Props) {
  const [browserOpen, setBrowserOpen] = useState(false)

  return (
    <div className={cx('wrap')}>
      {/* Mode selector */}
      <div className={cx('modeRow')}>
        <button
          type="button"
          className={cx('modeCard', { active: config.acquisitionMode === 'auto' })}
          onClick={() => onChange('acquisitionMode', 'auto')}
          disabled={disabled}
        >
          <span className={cx('modeIcon')}>🔄</span>
          <span className={cx('modeLabel')}>자동 촬영</span>
          <span className={cx('modeDesc')}>N초 간격 반복</span>
        </button>
        <button
          type="button"
          className={cx('modeCard', { active: config.acquisitionMode === 'manual' })}
          onClick={() => onChange('acquisitionMode', 'manual')}
          disabled={disabled}
        >
          <span className={cx('modeIcon')}>👆</span>
          <span className={cx('modeLabel')}>수동 촬영</span>
          <span className={cx('modeDesc')}>트리거로 한 장씩</span>
        </button>
      </div>

      {/* Auto settings */}
      {config.acquisitionMode === 'auto' && (
        <div className={cx('autoFields')}>
          <div className={cx('field')}>
            <label>Interval</label>
            <div className={cx('inputRow')}>
              <input
                type="number"
                min={0.05}
                step={0.1}
                placeholder="1"
                value={config.intervalMs != null ? config.intervalMs / 1000 : ''}
                onChange={(e) => {
                  const secs = parseFloat(e.target.value)
                  onChange('intervalMs', isNaN(secs) || secs <= 0 ? null : Math.round(secs * 1000))
                }}
                disabled={disabled}
              />
              <span className={cx('unit')}>s</span>
            </div>
            {config.intervalMs != null && (
              <small>{config.intervalMs}ms</small>
            )}
            {config.intervalMs == null && (
              <small>최소 0.05s (50ms)</small>
            )}
          </div>
          <div className={cx('field')}>
            <label>Stop after</label>
            <div className={cx('inputRow')}>
              <input
                type="number"
                min={1}
                placeholder="∞"
                value={config.frameCount ?? ''}
                onChange={(e) => {
                  const v = parseInt(e.target.value, 10)
                  onChange('frameCount', isNaN(v) || v < 1 ? null : v)
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
      {config.acquisitionMode === 'manual' && (
        <p className={cx('manualDesc')}>
          Start 후 화면 상단 <strong>Trigger</strong> 버튼으로 한 장씩 촬영합니다.
        </p>
      )}

      {/* Save settings */}
      <div className={cx('saveSection')}>
        <span className={cx('saveSectionLabel')}>저장 설정</span>
        <div className={cx('field')}>
          <label>저장 경로</label>
          <div className={cx('dirRow')}>
            <input
              type="text"
              value={config.outputDirectory}
              onChange={(e) => onChange('outputDirectory', e.target.value)}
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
              value={config.format}
              onChange={(e) => onChange('format', e.target.value as SaveImageFormat)}
              disabled={disabled}
            >
              {FORMAT_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <DirectoryBrowser
        open={browserOpen}
        currentPath={config.outputDirectory}
        onSelect={(path) => onChange('outputDirectory', path)}
        onClose={() => setBrowserOpen(false)}
      />
    </div>
  )
}
