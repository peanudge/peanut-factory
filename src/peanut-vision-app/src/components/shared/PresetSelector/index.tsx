import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Save, FolderOpen, Trash2 } from 'lucide-react'
import Modal from '@/components/shared/Modal'
import type { AcquisitionPreset, TriggerModeOption } from '@/api/types'
import { getPresets, savePreset, deletePreset } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'
import cx from './cx'

interface Props {
  profileId: string
  triggerMode: TriggerModeOption
  frameCount: number | null
  intervalMs: number | null
  onLoadPreset: (preset: AcquisitionPreset) => void
  disabled?: boolean
}

export default function PresetSelector({
  profileId,
  triggerMode,
  frameCount,
  intervalMs,
  onLoadPreset,
  disabled,
}: Props) {
  const queryClient = useQueryClient()
  const { toast } = useToast()
  const [loadOpen, setLoadOpen] = useState(false)
  const [saveOpen, setSaveOpen] = useState(false)
  const [presetName, setPresetName] = useState('')

  const { data: presets = [] } = useQuery({
    queryKey: queryKeys.presets,
    queryFn: getPresets,
  })

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: queryKeys.presets })

  const handleError = (e: unknown) =>
    toast(e instanceof Error ? e.message : '프리셋 작업에 실패했습니다', 'error')

  const saveMutation = useMutation({
    mutationFn: (preset: AcquisitionPreset) => savePreset(preset),
    onSuccess: () => {
      setPresetName('')
      setSaveOpen(false)
      invalidate()
    },
    onError: handleError,
  })

  const deleteMutation = useMutation({
    mutationFn: (name: string) => deletePreset(name),
    onSuccess: invalidate,
    onError: handleError,
  })

  const busy = saveMutation.isPending || deleteMutation.isPending

  const handleSave = () => {
    if (!presetName.trim()) return
    saveMutation.mutate({
      name: presetName.trim(),
      profileId,
      triggerMode,
      frameCount,
      intervalMs,
    })
  }

  const handleLoad = (preset: AcquisitionPreset) => {
    onLoadPreset(preset)
    setLoadOpen(false)
  }

  const metaDesc = [
    profileId || 'none',
    triggerMode,
    frameCount != null ? `${frameCount} frames` : null,
    intervalMs != null ? `${intervalMs}ms` : null,
  ]
    .filter(Boolean)
    .join(' | ')

  return (
    <div className={cx('wrap')}>
      <button
        type="button"
        className={cx('btn')}
        onClick={() => setSaveOpen(true)}
        disabled={disabled ?? !profileId}
      >
        <Save size={13} /> Save Preset
      </button>
      <button
        type="button"
        className={cx('btn')}
        onClick={() => setLoadOpen(true)}
        disabled={disabled}
      >
        <FolderOpen size={13} /> Load Preset
      </button>

      {/* Save Modal */}
      <Modal
        open={saveOpen}
        onClose={() => setSaveOpen(false)}
        title="Save Acquisition Preset"
        actions={
          <>
            <button type="button" className={cx('btn')} onClick={() => setSaveOpen(false)}>
              Cancel
            </button>
            <button
              type="button"
              className={cx('btn', 'primary')}
              onClick={handleSave}
              disabled={busy || !presetName.trim()}
            >
              Save
            </button>
          </>
        }
      >
        <input
          type="text"
          className={cx('field')}
          placeholder="Preset Name"
          value={presetName}
          onChange={(e) => setPresetName(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleSave()}
          autoFocus
        />
        <p className={cx('meta')}>{metaDesc}</p>
      </Modal>

      {/* Load Modal */}
      <Modal
        open={loadOpen}
        onClose={() => setLoadOpen(false)}
        title="Load Preset"
        actions={
          <button type="button" className={cx('btn')} onClick={() => setLoadOpen(false)}>
            Close
          </button>
        }
      >
        {presets.length === 0 ? (
          <p className={cx('empty')}>No presets saved yet</p>
        ) : (
          <ul className={cx('list')}>
            {presets.map((p) => (
              <li key={p.name} className={cx('listItem')} onClick={() => handleLoad(p)}>
                <div>
                  <div className={cx('presetName')}>{p.name}</div>
                  <div className={cx('presetMeta')}>
                    {[
                      p.profileId,
                      p.triggerMode ?? 'soft',
                      p.frameCount != null ? `${p.frameCount} frames` : null,
                      p.intervalMs != null ? `${p.intervalMs}ms` : null,
                    ]
                      .filter(Boolean)
                      .join(' | ')}
                  </div>
                </div>
                <button
                  type="button"
                  className={cx('iconBtn')}
                  onClick={(e) => {
                    e.stopPropagation()
                    deleteMutation.mutate(p.name)
                  }}
                  disabled={busy}
                >
                  <Trash2 size={14} />
                </button>
              </li>
            ))}
          </ul>
        )}
      </Modal>
    </div>
  )
}
