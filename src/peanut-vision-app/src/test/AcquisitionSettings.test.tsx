import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import type { AcquisitionFormConfig, AcquisitionConfigPreset, CamFileInfo } from '@/api/types'
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

const CAMERAS: CamFileInfo[] = [
  { fileName: 'cam1.cam', manufacturer: '', cameraModel: '', width: 1920, height: 1080,
    spectrum: '', colorFormat: '', trigMode: '', acquisitionMode: '', tapConfiguration: '' },
  { fileName: 'cam2.cam', manufacturer: '', cameraModel: '', width: 4160, height: 3120,
    spectrum: '', colorFormat: '', trigMode: '', acquisitionMode: '', tapConfiguration: '' },
]

const PRESETS: AcquisitionConfigPreset[] = [
  { name: '프리셋A', profileId: 'cam1.cam', format: 'png' },
  { name: '프리셋B', profileId: 'cam2.cam', format: 'bmp', frameCount: 10 },
]

function renderSettings(overrides: Partial<Parameters<typeof AcquisitionSettings>[0]> = {}) {
  const defaults = {
    config: BASE_CONFIG,
    onChange: vi.fn(),
    cameras: CAMERAS,
    camerasLoading: false,
    presets: [],
    presetsLoading: false,
    onQuickStart: vi.fn(),
    canStart: true,
    busy: false,
    onStart: vi.fn(),
    onSavePreset: vi.fn(),
    savingPreset: false,
  }
  return render(<AcquisitionSettings {...defaults} {...overrides} />)
}

// ── Presets section ──

describe('Presets quick-start section', () => {
  it('does not render section when presets empty and not loading', () => {
    renderSettings({ presets: [], presetsLoading: false })
    expect(screen.queryByText('빠른 시작')).not.toBeInTheDocument()
  })

  it('renders preset chips when presets provided', () => {
    renderSettings({ presets: PRESETS })
    expect(screen.getByText('프리셋A')).toBeInTheDocument()
    expect(screen.getByText('프리셋B')).toBeInTheDocument()
  })

  it('calls onQuickStart with correct preset when chip clicked', () => {
    const onQuickStart = vi.fn()
    renderSettings({ presets: PRESETS, onQuickStart })
    fireEvent.click(screen.getByText('프리셋A'))
    expect(onQuickStart).toHaveBeenCalledWith(PRESETS[0])
  })

  it('disables all chips when busy', () => {
    renderSettings({ presets: PRESETS, busy: true })
    const buttons = screen.getAllByRole('button', { name: /프리셋/ })
    buttons.forEach(btn => expect(btn).toBeDisabled())
  })

  it('shows skeleton placeholders when presetsLoading', () => {
    const { container } = renderSettings({ presetsLoading: true })
    expect(container.querySelectorAll('.skeleton').length).toBeGreaterThan(0)
    expect(screen.queryByText('프리셋A')).not.toBeInTheDocument()
  })
})

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
    expect(screen.getByPlaceholderText('1')).toBeInTheDocument()
  })

  it('hides interval input when 수동 selected', () => {
    renderSettings({ config: { ...BASE_CONFIG, acquisitionMode: 'manual' } })
    expect(screen.queryByPlaceholderText('1')).not.toBeInTheDocument()
  })

  it('calls onChange("acquisitionMode", "auto") when 자동 clicked', () => {
    const onChange = vi.fn()
    renderSettings({ onChange })
    fireEvent.click(screen.getByRole('radio', { name: /자동/ }))
    expect(onChange).toHaveBeenCalledWith('acquisitionMode', 'auto')
  })

  it('calls onChange("intervalMs", 500) when interval set to 0.5', () => {
    const onChange = vi.fn()
    renderSettings({
      onChange,
      config: { ...BASE_CONFIG, acquisitionMode: 'auto' },
    })
    fireEvent.change(screen.getByPlaceholderText('1'), { target: { value: '0.5' } })
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

// ── Save preset modal ──

describe('Save preset modal', () => {
  it('opens modal when 프리셋으로 저장 clicked', () => {
    renderSettings()
    fireEvent.click(screen.getByRole('button', { name: /프리셋으로 저장/ }))
    expect(screen.getByRole('dialog')).toBeInTheDocument()
  })

  it('calls onSavePreset with trimmed name on 저장 click', () => {
    const onSavePreset = vi.fn()
    renderSettings({ onSavePreset })
    fireEvent.click(screen.getByRole('button', { name: /프리셋으로 저장/ }))
    fireEvent.change(screen.getByPlaceholderText('프리셋 이름'), { target: { value: '  내 프리셋  ' } })
    fireEvent.click(screen.getByRole('button', { name: /^저장/ }))
    expect(onSavePreset).toHaveBeenCalledWith('내 프리셋')
  })

  it('disables 저장 button when savingPreset is true', () => {
    renderSettings({ savingPreset: true })
    fireEvent.click(screen.getByRole('button', { name: /프리셋으로 저장/ }))
    expect(screen.getByRole('button', { name: /^저장/ })).toBeDisabled()
  })
})
