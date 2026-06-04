import { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Folder } from 'lucide-react'
import type { ImageSaveSettings, SaveImageFormat } from '@/api/types'
import { getImageSaveSettings, updateImageSaveSettings } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import cx from './cx'

const FORMAT_OPTIONS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

const DEFAULT_SETTINGS: ImageSaveSettings = {
  outputDirectory: 'CapturedImages',
  format: 'png',
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
              value={localSettings.outputDirectory}
              onChange={(e) => update('outputDirectory', e.target.value)}
            />
            <small>Tokens: {'{date}'} → yyyy-MM-dd, {'{profile}'} → cam file name</small>
          </div>
          <div className={cx('field')} style={{ width: 110 }}>
            <label>Format</label>
            <select
              value={localSettings.format}
              onChange={(e) => update('format', e.target.value as SaveImageFormat)}
            >
              {FORMAT_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </div>

        <div className={cx('checkRow')}>
          <label className={cx('checkLabel')}>
            <input
              type="checkbox"
              checked={localSettings.autoSave}
              onChange={(e) => update('autoSave', e.target.checked)}
            />
            Auto-save on capture
          </label>
        </div>

        <div className={cx('bottomRow')}>
          <span className={cx('example')}>
            Example: <strong>capture_20260604_143000_123_00001.{localSettings.format}</strong>
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
