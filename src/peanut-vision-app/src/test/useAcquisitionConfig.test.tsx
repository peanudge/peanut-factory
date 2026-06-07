import { describe, it, expect, vi } from 'vitest'
import { renderHook, act } from '@testing-library/react'
import { useAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import { createWrapper } from './helpers'

// Mock API — cameras query
vi.mock('@/api/client', () => ({
  getCameras: vi.fn().mockResolvedValue([
    { fileName: 'crevis-tc-a160k-freerun-rgb8.cam', width: 4160, height: 3120 },
    { fileName: 'crevis-tc-a160k-softtrig-rgb8.cam', width: 4160, height: 3120 },
  ]),
}))

describe('useAcquisitionConfig', () => {
  const wrapper = createWrapper()

  describe('initial state', () => {
    it('starts with auto acquisition mode', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      expect(result.current.acquisitionMode).toBe('auto')
    })

    it('starts with null frameCount and intervalMs', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      expect(result.current.frameCount).toBeNull()
      expect(result.current.intervalMs).toBeNull()
    })

    it('auto-selects first camera profile after cameras load', async () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      // After the cameras query resolves, the first camera is auto-selected
      await vi.waitFor(() => {
        expect(result.current.selectedProfile).toBe('crevis-tc-a160k-freerun-rgb8.cam')
      })
    })
  })

  describe('state updates', () => {
    it('setAcquisitionMode updates mode', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.setAcquisitionMode('manual') })
      expect(result.current.acquisitionMode).toBe('manual')
    })

    it('setFrameCount updates frameCount', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.setFrameCount(10) })
      expect(result.current.frameCount).toBe(10)
    })

    it('setIntervalMs updates intervalMs', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.setIntervalMs(500) })
      expect(result.current.intervalMs).toBe(500)
    })

    it('setSelectedProfile updates profile', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.setSelectedProfile('crevis-tc-a160k-softtrig-rgb8.cam') })
      expect(result.current.selectedProfile).toBe('crevis-tc-a160k-softtrig-rgb8.cam')
    })
  })

  describe('handleLoadPreset', () => {
    it('loads preset values into config state', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => {
        result.current.handleLoadPreset({
          name: 'My Preset',
          profileId: 'crevis-tc-a160k-softtrig-rgb8.cam',
          frameCount: 5,
          intervalMs: 200,
        })
      })
      expect(result.current.selectedProfile).toBe('crevis-tc-a160k-softtrig-rgb8.cam')
      expect(result.current.frameCount).toBe(5)
      expect(result.current.intervalMs).toBe(200)
    })

    it('handles preset with null optional fields', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => {
        result.current.handleLoadPreset({
          name: 'Minimal',
          profileId: 'crevis-tc-a160k-freerun-rgb8.cam',
        })
      })
      expect(result.current.frameCount).toBeNull()
      expect(result.current.intervalMs).toBeNull()
    })
  })
})
