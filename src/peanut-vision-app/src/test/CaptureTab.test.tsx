import { describe, it, expect, vi, beforeEach } from 'vitest'
import { screen, waitFor, fireEvent } from '@testing-library/react'
import CaptureTab from '@/components/Acquisition/CaptureTab'
import { renderWithProviders } from './helpers'
import type { AcquisitionConfigPreset } from '@/api/types'

vi.mock('@/components/Acquisition/cx', () => ({
  default: (...args: unknown[]) => args.filter(Boolean).join(' '),
}))
vi.mock('@/components/shared/DirectoryBrowser', () => ({ default: () => null }))
vi.mock('@/components/shared/Modal', () => ({
  default: ({ open, children, actions }: {
    open: boolean; children: React.ReactNode; actions: React.ReactNode
  }) => open ? <div role="dialog">{children}{actions}</div> : null,
}))

const mockGetPresets = vi.fn()
vi.mock('@/api/client', () => ({
  getPresets: (...args: unknown[]) => mockGetPresets(...args),
  savePreset: vi.fn(),
  deletePreset: vi.fn(),
  startAcquisition: vi.fn(),
  stopAcquisition: vi.fn(),
  getAcquisitionStatus: vi.fn().mockResolvedValue({ isActive: false, channelState: 'none', allowedActions: ['start'] }),
  sendTrigger: vi.fn(),
  ApiError: class ApiError extends Error {},
}))

const CAMERAS: string[] = ['valid-cam.cam']

function makeSession(overrides = {}) {
  return {
    status: null,
    isActive: false,
    busy: false,
    canStart: true,
    canStop: false,
    canTrigger: false,
    hasWarnings: false,
    hasErrors: false,
    handleStart: vi.fn(),
    handleStartWithConfig: vi.fn(),
    handleStop: vi.fn(),
    handleTrigger: vi.fn(),
    ...overrides,
  }
}

function makeAcqConfig(overrides = {}) {
  return {
    cameras: CAMERAS,
    camerasLoading: false,
    config: {
      profileId: 'valid-cam.cam',
      acquisitionMode: 'manual' as const,
      frameCount: null,
      intervalMs: null,
      outputDirectory: 'CapturedImages',
      format: 'png' as const,
    },
    updateConfig: vi.fn(),
    loadPreset: vi.fn(),
    ...overrides,
  }
}

describe('CaptureTab — preset list stale profile warning', () => {
  const PRESETS: AcquisitionConfigPreset[] = [
    { name: 'valid preset', profileId: 'valid-cam.cam' },
    { name: 'stale preset', profileId: 'deleted-cam.cam' },
  ]

  beforeEach(() => {
    mockGetPresets.mockResolvedValue(PRESETS)
  })

  it('shows a warning title on a preset whose profileId is no longer available', async () => {
    renderWithProviders(
      <CaptureTab acqConfig={makeAcqConfig()} session={makeSession()} />
    )

    // Navigate to preset tab
    const presetTabBtn = await screen.findByRole('button', { name: /저장된 설정/i })
    presetTabBtn.click()

    await waitFor(() => {
      // The stale preset's list item should carry a warning indicator
      expect(screen.getByTestId('stale-profile-warning-stale preset')).toBeDefined()
    })
  })

  it('does not show a warning on a preset with a valid profileId', async () => {
    renderWithProviders(
      <CaptureTab acqConfig={makeAcqConfig()} session={makeSession()} />
    )

    const presetTabBtn = await screen.findByRole('button', { name: /저장된 설정/i })
    presetTabBtn.click()

    await waitFor(() => screen.getByText('valid preset'))

    expect(screen.queryByTestId('stale-profile-warning-valid preset')).toBeNull()
  })

  it('edit dialog keeps the stale profileId selected instead of silently resetting it', async () => {
    renderWithProviders(
      <CaptureTab acqConfig={makeAcqConfig()} session={makeSession()} />
    )

    const presetTabBtn = await screen.findByRole('button', { name: /저장된 설정/i })
    presetTabBtn.click()

    await waitFor(() => screen.getByText('stale preset'))

    // Open the edit dialog for the stale preset (second preset in the list).
    fireEvent.click(screen.getAllByTitle('수정')[1])

    const combo = await screen.findByRole('combobox') as HTMLSelectElement
    expect(combo.value).toBe('deleted-cam.cam')
    expect(screen.getByRole('option', { name: /deleted-cam\.cam/ })).toBeInTheDocument()
  })
})
