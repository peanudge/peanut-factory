import { useCallback, useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import type {
  AcquisitionConfigPreset,
  AcquisitionFormConfig,
} from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'
import { getCameras, getFilesystemDefaults } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'

export function useAcquisitionConfig() {
  const [config, setConfig] = useState<AcquisitionFormConfig>(DEFAULT_ACQUISITION_FORM_CONFIG)

  const { data: cameras = [] } = useQuery({
    queryKey: queryKeys.cameras,
    queryFn: getCameras,
  })

  const { data: fsDefaults } = useQuery({
    queryKey: ['filesystem', 'defaults'],
    queryFn: getFilesystemDefaults,
    staleTime: Infinity, // desktop path doesn't change at runtime
  })

  // Auto-select first camera profile
  useEffect(() => {
    if (cameras.length > 0 && !config.profileId) {
      setConfig(prev => ({ ...prev, profileId: cameras[0].fileName }))
    }
  }, [cameras, config.profileId])

  // Set desktop as default outputDirectory once fetched
  useEffect(() => {
    if (fsDefaults?.desktopPath && config.outputDirectory === DEFAULT_ACQUISITION_FORM_CONFIG.outputDirectory) {
      setConfig(prev => ({ ...prev, outputDirectory: fsDefaults.desktopPath }))
    }
  }, [fsDefaults, config.outputDirectory])

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
    }))
  }, [])

  return { cameras, config, updateConfig, loadPreset }
}

export type UseAcquisitionConfig = ReturnType<typeof useAcquisitionConfig>
