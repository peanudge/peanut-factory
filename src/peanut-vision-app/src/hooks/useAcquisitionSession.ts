import { useCallback } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import type { AcquisitionStatus, AcquisitionFormConfig } from '@/api/types'
import {
  startAcquisition,
  stopAcquisition,
  getAcquisitionStatus,
  sendTrigger,
  ApiError,
} from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'

export function useAcquisitionSession(config: AcquisitionFormConfig) {
  const queryClient = useQueryClient()
  const { toast } = useToast()

  const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
    queryKey: queryKeys.acquisitionStatus,
    queryFn: getAcquisitionStatus,
    refetchInterval: (query) => query.state.data?.isActive ? 1000 : false,
  })

  const invalidateStatus = useCallback(() =>
    queryClient.invalidateQueries({ queryKey: queryKeys.acquisitionStatus }),
  [queryClient])

  const handleError = useCallback((e: unknown) => {
    toast(
      e instanceof ApiError ? e.message : e instanceof Error ? e.message : 'Operation failed',
      'error',
    )
  }, [toast])

  const startMutation = useMutation({
    mutationFn: (cfg: AcquisitionFormConfig) =>
      startAcquisition({
        profileId: cfg.profileId,
        frameCount: cfg.frameCount,
        intervalMs: cfg.acquisitionMode === 'auto' ? cfg.intervalMs : null,
        outputDirectory: cfg.outputDirectory,
        format: cfg.format,
      }),
    onSuccess: () => {
      invalidateStatus()
      toast('촬영이 시작되었습니다', 'success')
    },
    onError: handleError,
  })

  const stopMutation = useMutation({
    mutationFn: stopAcquisition,
    onSuccess: () => {
      invalidateStatus()
      toast('촬영이 중지되었습니다', 'info')
    },
    onError: handleError,
  })

  const triggerMutation = useMutation({
    mutationFn: sendTrigger,
    onError: handleError,
  })

  const status = acquisitionStatus ?? null
  const isActive = status?.isActive ?? false
  const busy = startMutation.isPending || stopMutation.isPending || triggerMutation.isPending
  const canStart = (status?.allowedActions?.includes('start') ?? false) && !!config.profileId
  const canStop = status?.allowedActions?.includes('stop') ?? false
  const canTrigger = status?.allowedActions?.includes('trigger') ?? false
  const hasWarnings =
    (status?.statistics?.droppedFrameCount ?? 0) > 0 ||
    (status?.statistics?.clusterUnavailableCount ?? 0) > 0
  const hasErrors =
    !!status?.lastError || (status?.statistics?.errorCount ?? 0) > 0

  const handleStart = () => {
    if (config.acquisitionMode === 'auto' && config.intervalMs === null) {
      toast('Please input interval time', 'warning')
      return
    }
    startMutation.mutate(config)
  }

  const handleStartWithConfig = (cfg: AcquisitionFormConfig) => {
    if (cfg.acquisitionMode === 'auto' && cfg.intervalMs === null) {
      toast('Please input interval time', 'warning')
      return
    }
    startMutation.mutate(cfg)
  }

  return {
    status,
    isActive,
    busy,
    canStart,
    canStop,
    canTrigger,
    hasWarnings,
    hasErrors,
    handleStart,
    handleStartWithConfig,
    handleStop: () => stopMutation.mutate(),
    handleTrigger: () => triggerMutation.mutate(),
  }
}

export type AcquisitionSession = ReturnType<typeof useAcquisitionSession>
