import { useCallback, useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import type { AcquisitionConfigPreset, ContinuousSubMode, TriggerModeOption } from '@/api/types'
import { getCameras } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'

export function useAcquisitionConfig() {
  const [selectedProfile, setSelectedProfile] = useState('')
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>('auto')
  const [triggerMode, setTriggerMode] = useState<TriggerModeOption>('soft')
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
    setTriggerMode(preset.triggerMode ?? 'soft')
    setFrameCount(preset.frameCount ?? null)
    setIntervalMs(preset.intervalMs ?? null)
  }, [])

  return {
    cameras,
    selectedProfile,
    setSelectedProfile,
    continuousSubMode,
    setContinuousSubMode,
    triggerMode,
    setTriggerMode,
    frameCount,
    setFrameCount,
    intervalMs,
    setIntervalMs,
    handleLoadPreset,
  }
}

export type AcquisitionConfig = ReturnType<typeof useAcquisitionConfig>
