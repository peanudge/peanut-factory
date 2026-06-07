import { useCallback, useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import type { AcquisitionConfigPreset, AcquisitionMode } from '@/api/types'
import { getCameras } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'

export function useAcquisitionConfig() {
  const [selectedProfile, setSelectedProfile] = useState('')
  const [acquisitionMode, setAcquisitionMode] = useState<AcquisitionMode>('auto')
  const [frameCount, setFrameCount] = useState<number | null>(null)
  const [intervalMs, setIntervalMs] = useState<number | null>(null)

  const { data: cameras = [] } = useQuery({
    queryKey: queryKeys.cameras,
    queryFn: getCameras,
  })

  useEffect(() => {
    if (cameras.length > 0 && !selectedProfile) {
      setSelectedProfile(cameras[0].fileName)
    }
  }, [cameras, selectedProfile])

  const handleLoadPreset = useCallback((preset: AcquisitionConfigPreset) => {
    setSelectedProfile(preset.profileId)
    setFrameCount(preset.frameCount ?? null)
    setIntervalMs(preset.intervalMs ?? null)
  }, [])

  return {
    cameras,
    selectedProfile,
    setSelectedProfile,
    acquisitionMode,
    setAcquisitionMode,
    frameCount,
    setFrameCount,
    intervalMs,
    setIntervalMs,
    handleLoadPreset,
  }
}

export type AcquisitionConfig = ReturnType<typeof useAcquisitionConfig>
