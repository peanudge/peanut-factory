import { useEffect, useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import type { AcquisitionStatus, ExposureInfo } from '@/api/types'
import {
  getAcquisitionStatus,
  getExposure,
  setExposure,
  blackCalibration,
  whiteCalibration,
  whiteBalance,
  setFfc,
  ApiError,
} from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'

export function useCameraCalibration() {
  const { toast } = useToast()
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null)
  const [exposureValue, setExposureValue] = useState(1000)
  const [ffcEnabled, setFfcEnabled] = useState(false)

  // Deduplicated by React Query — no extra network call
  const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
    queryKey: queryKeys.acquisitionStatus,
    queryFn: getAcquisitionStatus,
  })

  const isCalibrationAvailable =
    acquisitionStatus?.channelState === 'idle' ||
    acquisitionStatus?.channelState === 'active'

  useEffect(() => {
    getExposure()
      .then((info) => { setExposureState(info); setExposureValue(info.exposureUs) })
      .catch(() => {})
  }, [])

  const handleError = (e: unknown) => {
    toast(
      e instanceof ApiError ? e.message : e instanceof Error ? e.message : 'Operation failed',
      'error',
    )
  }

  const loadExposureMutation = useMutation({
    mutationFn: getExposure,
    onSuccess: (info) => {
      setExposureState(info)
      setExposureValue(info.exposureUs)
      toast('Exposure settings loaded', 'success')
    },
    onError: handleError,
  })

  const applyExposureMutation = useMutation({
    mutationFn: () => setExposure(exposureValue),
    onSuccess: (result) => toast(result.message, 'success'),
    onError: handleError,
  })

  const blackMutation = useMutation({
    mutationFn: blackCalibration,
    onSuccess: (result) => toast(result.message, 'success'),
    onError: handleError,
  })

  const whiteMutation = useMutation({
    mutationFn: whiteCalibration,
    onSuccess: (result) => toast(result.message, 'success'),
    onError: handleError,
  })

  const whiteBalanceMutation = useMutation({
    mutationFn: whiteBalance,
    onSuccess: (result) => toast(result.message, 'success'),
    onError: handleError,
  })

  const ffcMutation = useMutation({
    mutationFn: (enable: boolean) => setFfc(enable),
    onSuccess: (result) => toast(result.message, 'success'),
    onError: handleError,
  })

  const busy =
    loadExposureMutation.isPending ||
    applyExposureMutation.isPending ||
    blackMutation.isPending ||
    whiteMutation.isPending ||
    whiteBalanceMutation.isPending ||
    ffcMutation.isPending

  return {
    exposure,
    exposureValue,
    setExposureValue,
    ffcEnabled,
    isCalibrationAvailable,
    busy,
    handleLoadExposure: () => loadExposureMutation.mutate(),
    handleApplyExposure: () => applyExposureMutation.mutate(),
    handleBlack: () => blackMutation.mutate(),
    handleWhite: () => whiteMutation.mutate(),
    handleWhiteBalance: () => whiteBalanceMutation.mutate(),
    handleFfcToggle: (_: unknown, checked: boolean) => {
      setFfcEnabled(checked)
      ffcMutation.mutate(checked)
    },
  }
}

export type CameraCalibration = ReturnType<typeof useCameraCalibration>
