import { useState } from 'react'
import { useLiveStream } from '@/hooks/useLiveStream'
import { useAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import { useAcquisitionSession } from '@/hooks/useAcquisitionSession'
import type { AcquisitionConfigPreset, AcquisitionFormConfig } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'
import CaptureTab from './CaptureTab'
import ImageViewer from '@/components/shared/ImageViewer'
import cx from './cx'

export type InputMode = 'manual' | 'preset'

function presetToFormConfig(preset: AcquisitionConfigPreset): AcquisitionFormConfig {
  return {
    ...DEFAULT_ACQUISITION_FORM_CONFIG,
    profileId: preset.profileId,
    frameCount: preset.frameCount ?? null,
    intervalMs: preset.intervalMs ?? null,
    acquisitionMode: preset.intervalMs != null ? 'auto' : 'manual',
    outputDirectory: preset.outputDirectory ?? DEFAULT_ACQUISITION_FORM_CONFIG.outputDirectory,
    format: preset.format ?? DEFAULT_ACQUISITION_FORM_CONFIG.format,
  }
}

export default function Acquisition() {
  const acqConfig = useAcquisitionConfig()
  const [inputMode, setInputMode] = useState<InputMode>('manual')
  const [selectedPreset, setSelectedPreset] = useState<AcquisitionConfigPreset | null>(null)

  const activeFormConfig = inputMode === 'preset' && selectedPreset
    ? presetToFormConfig(selectedPreset)
    : acqConfig.config

  const session = useAcquisitionSession(activeFormConfig)
  const live = useLiveStream()

  return (
    <div className={cx('Acquisition')}>
      <div className={cx('sidebar')}>
        <CaptureTab
          acqConfig={acqConfig}
          session={session}
          inputMode={inputMode}
          onInputModeChange={setInputMode}
          selectedPreset={selectedPreset}
          onPresetSelect={setSelectedPreset}
        />
      </div>

      <div className={cx('canvas')}>
        <div className={cx('imageArea')}>
          <ImageViewer
            url={live.previewUrl}
            errorMessage={session.status?.lastError}
            isLive={live.isActive}
            capturedAt={null}
            onClose={() => {}}
          />
        </div>
      </div>
    </div>
  )
}
