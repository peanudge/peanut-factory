import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import type { AcquisitionFormConfig } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'

vi.mock('@/components/shared/AcquisitionSettings/cx', () => ({
  default: (...args: unknown[]) => args.filter(Boolean).join(' '),
}))
vi.mock('@/components/shared/DirectoryBrowser', () => ({ default: () => null }))
vi.mock('@/components/shared/Modal', () => ({
  default: ({ open, children, actions }: {
    open: boolean; children: React.ReactNode; actions: React.ReactNode
  }) => open ? <div role="dialog">{children}{actions}</div> : null,
}))

const BASE_CONFIG: AcquisitionFormConfig = {
  ...DEFAULT_ACQUISITION_FORM_CONFIG,
  profileId: 'cam1.cam',
  acquisitionMode: 'manual',
}

const CAMERAS: string[] = ['cam1.cam', 'cam2.cam']

function renderSettings(overrides: Partial<Parameters<typeof AcquisitionSettings>[0]> = {}) {
  const defaults = {
    config: BASE_CONFIG,
    onChange: vi.fn(),
    cameras: CAMERAS,
    camerasLoading: false,
    canStart: true,
    busy: false,
    onStart: vi.fn(),
    onSavePreset: vi.fn(),
    savingPreset: false,
  }
  return render(<AcquisitionSettings {...defaults} {...overrides} />)
}

// ── Camera select ──

describe('Camera profile select', () => {
  it('renders all camera options', () => {
    renderSettings({ cameras: CAMERAS })
    expect(screen.getByRole('option', { name: 'cam1.cam' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'cam2.cam' })).toBeInTheDocument()
  })

  it('shows loading option and is disabled when camerasLoading', () => {
    renderSettings({ camerasLoading: true })
    expect(screen.getByRole('option', { name: '로딩 중…' })).toBeInTheDocument()
    expect(screen.getByRole('combobox')).toBeDisabled()
  })

  it('calls onChange with selected value', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.change(screen.getByRole('combobox'), { target: { value: 'cam2.cam' } })
    expect(onChange).toHaveBeenCalledWith('profileId', 'cam2.cam')
  })

  it('keeps a stale profileId selectable when it is no longer in the cameras list', () => {
    // CamFile was renamed after this profile was saved; the controlled select
    // must still reflect the saved value instead of silently falling back to
    // the first camera (which would be sent to the server on save).
    renderSettings({
      config: { ...BASE_CONFIG, profileId: 'renamed-away.cam' },
      cameras: ['cam1.cam', 'cam2.cam'],
    })
    const combo = screen.getByRole('combobox') as HTMLSelectElement
    expect(combo.value).toBe('renamed-away.cam')
    expect(screen.getByRole('option', { name: /renamed-away\.cam/ })).toBeInTheDocument()
  })
})

// ── Format radio ──

describe('Format selection', () => {
  it('renders PNG, BMP, RAW radio buttons', () => {
    renderSettings()
    expect(screen.getByRole('radio', { name: 'PNG' })).toBeInTheDocument()
    expect(screen.getByRole('radio', { name: 'BMP' })).toBeInTheDocument()
    expect(screen.getByRole('radio', { name: 'RAW' })).toBeInTheDocument()
  })

  it('calls onChange("format", "bmp") when BMP selected', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.click(screen.getByRole('radio', { name: 'BMP' }))
    expect(onChange).toHaveBeenCalledWith('format', 'bmp')
  })

  it('reflects current format from config', () => {
    renderSettings({ config: { ...BASE_CONFIG, format: 'bmp' } })
    expect(screen.getByRole('radio', { name: 'BMP' })).toBeChecked()
    expect(screen.getByRole('radio', { name: 'PNG' })).not.toBeChecked()
  })
})

// ── Trigger mode ──

describe('Trigger mode', () => {
  it('renders 자동 and 수동 radio buttons', () => {
    renderSettings()
    expect(screen.getByRole('radio', { name: /자동/ })).toBeInTheDocument()
    expect(screen.getByRole('radio', { name: /수동/ })).toBeInTheDocument()
  })

  it('shows interval input when 자동 selected', () => {
    renderSettings({ config: { ...BASE_CONFIG, acquisitionMode: 'auto' } })
    expect(screen.getByPlaceholderText('1000')).toBeInTheDocument()
  })

  it('hides interval input when 수동 selected', () => {
    renderSettings({ config: { ...BASE_CONFIG, acquisitionMode: 'manual' } })
    expect(screen.queryByPlaceholderText('1000')).not.toBeInTheDocument()
  })

  it('calls onChange("acquisitionMode", "auto") when 자동 clicked', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.click(screen.getByRole('radio', { name: /자동/ }))
    expect(onChange).toHaveBeenCalledWith('acquisitionMode', 'auto')
  })

  it('calls onChange("intervalMs", 500) when interval set to 500', () => {
    const onChange = vi.fn()
    renderSettings({ onChange, config: { ...BASE_CONFIG, acquisitionMode: 'auto' } })
    fireEvent.change(screen.getByPlaceholderText('1000'), { target: { value: '500' } })
    expect(onChange).toHaveBeenCalledWith('intervalMs', 500)
  })
})

// ── Start button ──

describe('Start button', () => {
  it('calls onStart when clicked and canStart is true', () => {
    const onStart = vi.fn()
    renderSettings({ onStart, canStart: true })
    fireEvent.click(screen.getByRole('button', { name: /촬영 시작/ }))
    expect(onStart).toHaveBeenCalledTimes(1)
  })

  it('is disabled when canStart is false', () => {
    renderSettings({ canStart: false })
    expect(screen.getByRole('button', { name: /촬영 시작/ })).toBeDisabled()
  })

  it('is disabled when busy is true', () => {
    renderSettings({ busy: true, canStart: true })
    expect(screen.getByRole('button', { name: /촬영 시작/ })).toBeDisabled()
  })
})

// ── Save settings modal ──

describe('Save settings modal', () => {
  it('opens modal when 이 설정 저장하기 clicked', () => {
    renderSettings()
    fireEvent.click(screen.getByRole('button', { name: /이 설정 저장하기/ }))
    expect(screen.getByRole('dialog')).toBeInTheDocument()
  })

  it('calls onSavePreset with trimmed name on 저장 click', () => {
    const onSavePreset = vi.fn()
    renderSettings({ onSavePreset })
    fireEvent.click(screen.getByRole('button', { name: /이 설정 저장하기/ }))
    fireEvent.change(screen.getByPlaceholderText('설정 이름'), { target: { value: '  내 설정  ' } })
    fireEvent.click(screen.getByRole('button', { name: /^저장/ }))
    expect(onSavePreset).toHaveBeenCalledWith('내 설정')
  })

  it('disables 저장 button when savingPreset is true', () => {
    renderSettings({ savingPreset: true })
    fireEvent.click(screen.getByRole('button', { name: /이 설정 저장하기/ }))
    expect(screen.getByRole('button', { name: /^저장/ })).toBeDisabled()
  })
})
