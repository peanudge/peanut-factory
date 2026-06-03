import { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Folder } from 'lucide-react'
import type { ImageSaveSettings, SaveImageFormat, SubfolderStrategy } from '@/api/types'
import { getImageSaveSettings, updateImageSaveSettings } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import cx from './cx'

const FORMAT_OPTIONS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

const SUBFOLDER_OPTIONS: { value: SubfolderStrategy; label: string }[] = [
  { value: 'none', label: 'None' },
  { value: 'byDate', label: 'By Date (YYYY-MM-DD)' },
  { value: 'bySession', label: 'By Session' },
  { value: 'byProfile', label: 'By Profile' },
]

const DEFAULT_SETTINGS: ImageSaveSettings = {
  outputDirectory: 'CapturedImages',
  format: 'png',
  filenamePrefix: 'capture',
  timestampFormat: 'yyyyMMdd_HHmmss_fff',
  includeSequenceNumber: false,
  subfolderStrategy: 'none',
  autoSave: true,
}

export default function ImageSaveSettingsPanel() {
  const queryClient = useQueryClient()
  const [localSettings, setLocalSettings] = useState<ImageSaveSettings>(DEFAULT_SETTINGS)
  const [saved, setSaved] = useState(false)

  const { data: serverSettings } = useQuery({
    queryKey: queryKeys.imageSaveSettings,
    queryFn: getImageSaveSettings,
  })

  useEffect(() => {
    if (serverSettings) setLocalSettings(serverSettings)
  }, [serverSettings])

  const saveMutation = useMutation({
    mutationFn: (settings: ImageSaveSettings) => updateImageSaveSettings(settings),
    onSuccess: (updated) => {
      queryClient.setQueryData(queryKeys.imageSaveSettings, updated)
      setLocalSettings(updated)
      setSaved(true)
      setTimeout(() => setSaved(false), 3000)
    },
  })

  const update = <K extends keyof ImageSaveSettings>(key: K, value: ImageSaveSettings[K]) =>
    setLocalSettings((prev) => ({ ...prev, [key]: value }))

  const settings = localSettings

  const exampleFilename =
    [
      settings.filenamePrefix || 'capture',
      '20260320_143000_123',
      settings.includeSequenceNumber ? '00001' : null,
    ]
      .filter(Boolean)
      .join('_') + `.${settings.format}`

  return (
    <details className={cx('accordion')}>
      <summary>
        <Folder size={15} />
        Image Save Settings
      </summary>
      <div className={cx('body')}>
        <div className={cx('row')}>
          <div className={cx('field')} style={{ flexGrow: 1, minWidth: 220 }}>
            <label>Output Directory</label>
            <input
              type="text"
              value={settings.outputDirectory}
              onChange={(e) => update('outputDirectory', e.target.value)}
            />
            <small>Relative to app root, or absolute path</small>
          </div>
          <div className={cx('field')} style={{ width: 110 }}>
            <label>Format</label>
            <select
              value={settings.format}
              onChange={(e) => update('format', e.target.value as SaveImageFormat)}
            >
              {FORMAT_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </div>

        <div className={cx('row')}>
          <div className={cx('field')} style={{ width: 160 }}>
            <label>Filename Prefix</label>
            <input
              type="text"
              value={settings.filenamePrefix}
              onChange={(e) => update('filenamePrefix', e.target.value)}
            />
          </div>
          <div className={cx('field')} style={{ width: 200 }}>
            <label>Timestamp Format</label>
            <input
              type="text"
              value={settings.timestampFormat}
              onChange={(e) => update('timestampFormat', e.target.value)}
            />
            <small>.NET DateTime format</small>
          </div>
          <div className={cx('field')} style={{ width: 200 }}>
            <label>Subfolder</label>
            <select
              value={settings.subfolderStrategy}
              onChange={(e) => update('subfolderStrategy', e.target.value as SubfolderStrategy)}
            >
              {SUBFOLDER_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </div>

        <div className={cx('checkRow')}>
          <label className={cx('checkLabel')}>
            <input
              type="checkbox"
              checked={settings.autoSave}
              onChange={(e) => update('autoSave', e.target.checked)}
            />
            Auto-save on capture
          </label>
          <label className={cx('checkLabel')}>
            <input
              type="checkbox"
              checked={settings.includeSequenceNumber}
              onChange={(e) => update('includeSequenceNumber', e.target.checked)}
            />
            Include sequence number
          </label>
        </div>

        <div className={cx('bottomRow')}>
          <span className={cx('example')}>
            Example: <strong>{exampleFilename}</strong>
          </span>
          <button
            type="button"
            className={cx('btn')}
            onClick={() => saveMutation.mutate(localSettings)}
            disabled={saveMutation.isPending}
          >
            Save Settings
          </button>
        </div>

        {saveMutation.isError && (
          <div className={cx('alert', 'error')}>
            {saveMutation.error instanceof Error
              ? saveMutation.error.message
              : 'Failed to save settings'}
          </div>
        )}
        {saved && <div className={cx('alert', 'success')}>Settings saved</div>}
      </div>
    </details>
  )
}
