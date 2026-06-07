import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, act } from '@testing-library/react'
import { useAcquisitionSession } from '@/hooks/useAcquisitionSession'
import { createWrapper } from './helpers'
import type { AcquisitionFormConfig } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'

// Capture what startAcquisition was called with
const mockStartAcquisition = vi.fn().mockResolvedValue({ message: 'ok', profileId: 'cam.cam' })

vi.mock('@/api/client', () => ({
  startAcquisition: (...args: unknown[]) => mockStartAcquisition(...args),
  stopAcquisition: vi.fn().mockResolvedValue({ message: 'stopped' }),
  getAcquisitionStatus: vi.fn().mockResolvedValue({
    isActive: false,
    channelState: 'none',
    allowedActions: ['start'],
  }),
  triggerAndCapture: vi.fn().mockResolvedValue({ blob: new Blob() }),
  ApiError: class ApiError extends Error {},
}))

function makeConfig(overrides: Partial<AcquisitionFormConfig> = {}): AcquisitionFormConfig {
  return {
    ...DEFAULT_ACQUISITION_FORM_CONFIG,
    profileId: 'crevis.cam',
    acquisitionMode: 'auto',
    intervalMs: 1000,   // default: valid auto mode so handleStart() passes validation
    ...overrides,
  }
}

describe('useAcquisitionSession', () => {
  const wrapper = createWrapper()

  beforeEach(() => {
    mockStartAcquisition.mockClear()
  })

  describe('startAcquisition payload', () => {
    it('sends outputDirectory to server', async () => {
      const config = makeConfig({ outputDirectory: '/data/peanut/captures' })
      const { result } = renderHook(() => useAcquisitionSession(config), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).toHaveBeenCalledWith(
        expect.objectContaining({ outputDirectory: '/data/peanut/captures' })
      )
    })

    it('sends format to server', async () => {
      const config = makeConfig({ format: 'bmp' })
      const { result } = renderHook(() => useAcquisitionSession(config), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).toHaveBeenCalledWith(
        expect.objectContaining({ format: 'bmp' })
      )
    })

    it('sends intervalMs=null in manual mode regardless of intervalMs value', async () => {
      const config = makeConfig({ acquisitionMode: 'manual', intervalMs: 500 })
      const { result } = renderHook(() => useAcquisitionSession(config), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).toHaveBeenCalledWith(
        expect.objectContaining({ intervalMs: null })
      )
    })

    it('sends intervalMs in auto mode', async () => {
      const config = makeConfig({ acquisitionMode: 'auto', intervalMs: 1000 })
      const { result } = renderHook(() => useAcquisitionSession(config), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).toHaveBeenCalledWith(
        expect.objectContaining({ intervalMs: 1000 })
      )
    })

    it('sends all fields together when config is fully populated', async () => {
      const config = makeConfig({
        profileId: 'crevis-tc-a160k-freerun-rgb8.cam',
        acquisitionMode: 'auto',
        frameCount: 10,
        intervalMs: 2000,
        outputDirectory: '/custom/output',
        format: 'raw',
      })
      const { result } = renderHook(() => useAcquisitionSession(config), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).toHaveBeenCalledWith({
        profileId: 'crevis-tc-a160k-freerun-rgb8.cam',
        frameCount: 10,
        intervalMs: 2000,
        outputDirectory: '/custom/output',
        format: 'raw',
      })
    })

    it('does not start when auto mode has no intervalMs', async () => {
      const config = makeConfig({ acquisitionMode: 'auto', intervalMs: null })
      const { result } = renderHook(() => useAcquisitionSession(config), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).not.toHaveBeenCalled()
    })
  })

  describe('preset config round-trip via useAcquisitionConfig', () => {
    it('preset outputDirectory flows into startAcquisition payload', async () => {
      // Simulates: loadPreset() → config.outputDirectory → useAcquisitionSession → startAcquisition
      const presetConfig = makeConfig({
        profileId: 'crevis.cam',
        outputDirectory: '/preset/output/path',
        format: 'bmp',
      })
      const { result } = renderHook(() => useAcquisitionSession(presetConfig), { wrapper })

      await act(async () => { result.current.handleStart() })

      expect(mockStartAcquisition).toHaveBeenCalledWith(
        expect.objectContaining({
          outputDirectory: '/preset/output/path',
          format: 'bmp',
        })
      )
    })
  })
})
