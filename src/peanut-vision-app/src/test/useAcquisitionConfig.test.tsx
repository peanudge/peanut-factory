import { describe, it, expect, vi } from 'vitest'
import { renderHook, act } from '@testing-library/react'
import { useAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import { createWrapper } from './helpers'

vi.mock('@/api/client', () => ({
  getCameras: vi.fn().mockResolvedValue([
    { fileName: 'crevis-tc-a160k-freerun-rgb8.cam', width: 4160, height: 3120 },
    { fileName: 'crevis-tc-a160k-softtrig-rgb8.cam', width: 4160, height: 3120 },
  ]),
  getFilesystemDefaults: vi.fn().mockResolvedValue({ desktopPath: '' }),
}))

describe('useAcquisitionConfig', () => {
  const wrapper = createWrapper()

  describe('initial state', () => {
    it('starts with auto acquisition mode', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      expect(result.current.config.acquisitionMode).toBe('auto')
    })

    it('starts with null frameCount and intervalMs', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      expect(result.current.config.frameCount).toBeNull()
      expect(result.current.config.intervalMs).toBeNull()
    })

    it('starts with default outputDirectory', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      expect(result.current.config.outputDirectory).toBe('CapturedImages')
    })

    it('auto-selects first camera profile after cameras load', async () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      await vi.waitFor(() => {
        expect(result.current.config.profileId).toBe('crevis-tc-a160k-freerun-rgb8.cam')
      })
    })
  })

  describe('updateConfig', () => {
    it('updates a single field by key', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.updateConfig('acquisitionMode', 'manual') })
      expect(result.current.config.acquisitionMode).toBe('manual')
    })

    it('updates frameCount', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.updateConfig('frameCount', 10) })
      expect(result.current.config.frameCount).toBe(10)
    })

    it('updates intervalMs', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.updateConfig('intervalMs', 500) })
      expect(result.current.config.intervalMs).toBe(500)
    })

    it('updates profileId', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.updateConfig('profileId', 'crevis-tc-a160k-softtrig-rgb8.cam') })
      expect(result.current.config.profileId).toBe('crevis-tc-a160k-softtrig-rgb8.cam')
    })

    it('does not overwrite other fields when updating one', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.updateConfig('frameCount', 5) })
      act(() => { result.current.updateConfig('intervalMs', 200) })
      expect(result.current.config.frameCount).toBe(5)
      expect(result.current.config.intervalMs).toBe(200)
    })
  })

  describe('loadPreset', () => {
    it('loads all preset values into config', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => {
        result.current.loadPreset({
          name: 'My Preset',
          profileId: 'crevis-tc-a160k-softtrig-rgb8.cam',
          frameCount: 5,
          intervalMs: 200,
          outputDirectory: '/data/captures',
          format: 'bmp',
        })
      })
      expect(result.current.config.profileId).toBe('crevis-tc-a160k-softtrig-rgb8.cam')
      expect(result.current.config.frameCount).toBe(5)
      expect(result.current.config.intervalMs).toBe(200)
      expect(result.current.config.outputDirectory).toBe('/data/captures')
      expect(result.current.config.format).toBe('bmp')
    })

    it('keeps existing values for fields not in preset', () => {
      const { result } = renderHook(() => useAcquisitionConfig(), { wrapper })
      act(() => { result.current.updateConfig('outputDirectory', '/my/path') })
      act(() => {
        result.current.loadPreset({
          name: 'Minimal',
          profileId: 'crevis-tc-a160k-freerun-rgb8.cam',
        })
      })
      expect(result.current.config.frameCount).toBeNull()
      expect(result.current.config.intervalMs).toBeNull()
      // outputDirectory not in preset → keeps existing value
      expect(result.current.config.outputDirectory).toBe('/my/path')
    })
  })
})
