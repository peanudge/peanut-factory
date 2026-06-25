import { useState, useEffect } from 'react'
import { Loader2, FolderSearch } from 'lucide-react'
import type { AcquisitionFormConfig, SaveImageFormat } from '@/api/types'
import DirectoryBrowser from '@/components/shared/DirectoryBrowser'
import Modal from '@/components/shared/Modal'
import cx from './cx'

interface Props {
  config: AcquisitionFormConfig
  onChange: <K extends keyof AcquisitionFormConfig>(key: K, value: AcquisitionFormConfig[K]) => void
  cameras: string[]
  camerasLoading: boolean
  canStart: boolean
  busy: boolean
  onStart: () => void
  onSavePreset: (name: string) => void
  savingPreset: boolean
}

const FORMATS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

export default function AcquisitionSettings({
  config, onChange,
  cameras, camerasLoading,
  canStart, busy, onStart,
  onSavePreset, savingPreset,
}: Props) {
  const [browserOpen, setBrowserOpen] = useState(false)
  const [saveOpen, setSaveOpen] = useState(false)
  const [presetName, setPresetName] = useState('')
  const [intervalRaw, setIntervalRaw] = useState(
    config.intervalMs != null ? String(config.intervalMs) : ''
  )
  const [intervalError, setIntervalError] = useState('')

  // Sync when intervalMs changes externally (e.g. preset loaded)
  useEffect(() => {
    setIntervalRaw(config.intervalMs != null ? String(config.intervalMs) : '')
    setIntervalError('')
  }, [config.intervalMs])

  const handleIntervalChange = (raw: string) => {
    setIntervalRaw(raw)
    if (raw.trim() === '') {
      setIntervalError('')
      onChange('intervalMs', null)
      return
    }
    const ms = parseInt(raw, 10)
    if (isNaN(ms) || ms < 50) {
      setIntervalError('50ms 이상의 정수를 입력하세요')
    } else {
      setIntervalError('')
      onChange('intervalMs', ms)
    }
  }

  const handleIntervalStep = (delta: number) => {
    const next = Math.max(50, (config.intervalMs ?? 1000) + delta)
    onChange('intervalMs', next)
  }

  const handleSave = () => {
    if (!presetName.trim()) return
    onSavePreset(presetName.trim())
    setPresetName('')
    setSaveOpen(false)
  }

  return (
    <div className={cx('wrap')}>

      {/* ── Settings Form ── */}
      <section className={cx('section')}>
        <span className={cx('sectionLabel')}>촬영 설정</span>

        <label>
          카메라 프로파일
          <select
            value={config.profileId}
            onChange={(e) => onChange('profileId', e.target.value)}
            disabled={camerasLoading || busy}
          >
            {camerasLoading
              ? <option>로딩 중…</option>
              : <>
                  {config.profileId && !cameras.includes(config.profileId) && (
                    <option value={config.profileId}>{config.profileId} (사용 불가)</option>
                  )}
                  {cameras.map((name) => (
                    <option key={name} value={name}>{name}</option>
                  ))}
                </>
            }
          </select>
        </label>

        <label>
          저장 경로
          <div className={cx('dirRow')}>
            <input
              type="text"
              value={config.outputDirectory}
              onChange={(e) => onChange('outputDirectory', e.target.value)}
              disabled={busy}
              placeholder="CapturedImages"
            />
            <button
              type="button"
              onClick={() => setBrowserOpen(true)}
              disabled={busy}
              title="Browse"
            >
              <FolderSearch size={14} />
            </button>
          </div>
          <small>토큰: {'{date}'} → yyyy-MM-dd, {'{profile}'} → cam 파일명</small>
        </label>

        <fieldset>
          <legend>포맷</legend>
          {FORMATS.map(({ value, label }) => (
            <label key={value}>
              <input
                type="radio"
                name="format"
                value={value}
                checked={config.format === value}
                onChange={() => onChange('format', value)}
                disabled={busy}
              />
              {label}
            </label>
          ))}
        </fieldset>

        <label>
          프레임 수
          <input
            type="number"
            min={1}
            value={config.frameCount ?? ''}
            placeholder="∞ (제한없음)"
            onChange={(e) => {
              const v = parseInt(e.target.value, 10)
              onChange('frameCount', isNaN(v) || v < 1 ? null : v)
            }}
            disabled={busy}
          />
        </label>
      </section>

      {/* ── Trigger Mode ── */}
      <section className={cx('section')}>
        <span className={cx('sectionLabel')}>촬영 방식</span>

        <label>
          <input
            type="radio"
            name="acquisitionMode"
            value="auto"
            checked={config.acquisitionMode === 'auto'}
            onChange={() => onChange('acquisitionMode', 'auto')}
            disabled={busy}
          />
          자동 (N초 간격 반복)
        </label>

        <label>
          <input
            type="radio"
            name="acquisitionMode"
            value="manual"
            checked={config.acquisitionMode === 'manual'}
            onChange={() => onChange('acquisitionMode', 'manual')}
            disabled={busy}
          />
          수동 (트리거로 한 장씩)
        </label>

        {config.acquisitionMode === 'auto' && (
          <label>
            간격
            <div className={cx('intervalRow')}>
              <button
                type="button"
                className={cx('intervalBtn')}
                onClick={() => handleIntervalStep(-50)}
                disabled={busy || (config.intervalMs ?? 1000) <= 50}
              >−50</button>
              <input
                type="text"
                inputMode="numeric"
                value={intervalRaw}
                placeholder="1000"
                onChange={(e) => handleIntervalChange(e.target.value)}
                disabled={busy}
                className={intervalError ? cx('inputError') : undefined}
              />
              <span className={cx('intervalUnit')}>ms</span>
              <button
                type="button"
                className={cx('intervalBtn')}
                onClick={() => handleIntervalStep(50)}
                disabled={busy}
              >+50</button>
            </div>
            {intervalError && (
              <span className={cx('errorMsg')}>{intervalError}</span>
            )}
          </label>
        )}
      </section>

      {/* ── Start ── */}
      <section className={cx('section')}>
        <button
          type="button"
          className={cx('startBtn')}
          onClick={onStart}
          disabled={!canStart || busy}
        >
          {busy && <Loader2 size={14} className={cx('spin')} />}
          촬영 시작
        </button>

        <button
          type="button"
          className={cx('saveLinkBtn')}
          onClick={() => setSaveOpen(true)}
          disabled={!config.profileId || busy}
        >
          이 설정 저장하기
        </button>
      </section>

      <DirectoryBrowser
        open={browserOpen}
        currentPath={config.outputDirectory}
        onSelect={(path) => onChange('outputDirectory', path)}
        onClose={() => setBrowserOpen(false)}
      />

      <Modal
        open={saveOpen}
        onClose={() => setSaveOpen(false)}
        title="설정 저장"
        actions={
          <>
            <button type="button" onClick={() => setSaveOpen(false)}>
              취소
            </button>
            <button
              type="button"
              onClick={handleSave}
              disabled={savingPreset || !presetName.trim()}
            >
              {savingPreset && <Loader2 size={13} className={cx('spin')} />}
              저장
            </button>
          </>
        }
      >
        <input
          type="text"
          placeholder="설정 이름"
          value={presetName}
          onChange={(e) => setPresetName(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleSave()}
          autoFocus
        />
        <small>
          {[
            config.profileId || 'none',
            config.frameCount != null ? `${config.frameCount} frames` : null,
            config.intervalMs != null ? `${config.intervalMs / 1000}s` : null,
          ].filter(Boolean).join(' | ')}
        </small>
      </Modal>
    </div>
  )
}
