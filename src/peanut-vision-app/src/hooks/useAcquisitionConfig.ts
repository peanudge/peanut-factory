import { useCallback, useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import type {
  AcquisitionConfigPreset,
  AcquisitionFormConfig,
} from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'
import { getCameras } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'

export function useAcquisitionConfig() {
  const [config, setConfig] = useState<AcquisitionFormConfig>(DEFAULT_ACQUISITION_FORM_CONFIG)

  const { data: cameras = [] } = useQuery({
    queryKey: queryKeys.cameras,
    queryFn: getCameras,
  })

  useEffect(() => {
    if (cameras.length > 0 && !config.profileId) {
      setConfig(prev => ({ ...prev, profileId: cameras[0].fileName }))
    }
  }, [cameras, config.profileId])

  const updateConfig = useCallback(<K extends keyof AcquisitionFormConfig>(
    key: K,
    value: AcquisitionFormConfig[K],
  ) => setConfig(prev => ({ ...prev, [key]: value })), [])

  const loadPreset = useCallback((preset: AcquisitionConfigPreset) => {
    setConfig(prev => ({
      ...prev,
      profileId: preset.profileId,
      frameCount: preset.frameCount ?? null,
      intervalMs: preset.intervalMs ?? null,
      ...(preset.outputDirectory !== undefined && { outputDirectory: preset.outputDirectory }),
      ...(preset.format !== undefined && { format: preset.format }),
      ...(preset.autoSave !== undefined && { autoSave: preset.autoSave }),
    }))
  }, [])

  return { cameras, config, updateConfig, loadPreset }
}

export type UseAcquisitionConfig = ReturnType<typeof useAcquisitionConfig>
