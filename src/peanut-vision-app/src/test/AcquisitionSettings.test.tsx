import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import type { AcquisitionFormConfig, AcquisitionConfigPreset, CamFileInfo } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'

vi.mock('@/components/shared/AcquisitionSettings/cx', () => ({
  default: (...args: string[]) => args.filter(Boolean).join(' '),
}))
vi.mock('@/components/shared/DirectoryBrowser', () => ({
  default: () => null,
}))
vi.mock('@/components/shared/Modal', () => ({
  default: ({ open, children, actions, title }: { open: boolean; children: React.ReactNode; actions: React.ReactNode; title: string }) =>
    open ? <div role="dialog" aria-label={title}>{children}{actions}</div> : null,
}))

const makeConfig = (overrides: Partial<AcquisitionFormConfig> = {}): AcquisitionFormConfig => ({
  ...DEFAULT_ACQUISITION_FORM_CONFIG,
  ...overrides,
})

const mockCamera: CamFileInfo = {
  fileName: 'TC-A160K-SEM_freerun_RGB8.cam',
  manufacturer: 'Crevis',
  cameraModel: 'TC-A160K',
  width: 4160,
  height: 3120,
  spectrum: 'Color',
  colorFormat: 'RGB8',
  trigMode: 'IMMEDIATE',
  acquisitionMode: 'VIDEO',
  tapConfiguration: '1X',
}

const mockPreset: AcquisitionConfigPreset = {
  name: 'Test Preset',
  profileId: 'TC-A160K-SEM_freerun_RGB8.cam',
  frameCount: 10,
  intervalMs: 1000,
}

const defaultProps = {
  config: makeConfig({ profileId: 'TC-A160K-SEM_freerun_RGB8.cam' }),
  onChange: vi.fn(),
  cameras: [mockCamera],
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

describe('AcquisitionSettings', () => {
  describe('settings form', () => {
    it('renders camera profile select with provided cameras', () => {
      render(<AcquisitionSettings {...defaultProps} />)
      expect(screen.getByRole('combobox')).toBeInTheDocument()
      expect(screen.getByText('TC-A160K-SEM_freerun_RGB8.cam')).toBeInTheDocument()
    })

    it('shows loading option when camerasLoading is true', () => {
      render(<AcquisitionSettings {...defaultProps} cameras={[]} camerasLoading={true} />)
      expect(screen.getByText('로딩 중…')).toBeInTheDocument()
    })

    it('calls onChange when profileId select changes', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings {...defaultProps} onChange={onChange} />)
      fireEvent.change(screen.getByRole('combobox'), { target: { value: 'TC-A160K-SEM_freerun_RGB8.cam' } })
      expect(onChange).toHaveBeenCalledWith('profileId', 'TC-A160K-SEM_freerun_RGB8.cam')
    })

    it('renders format radio buttons', () => {
      render(<AcquisitionSettings {...defaultProps} />)
      expect(screen.getByDisplayValue('png')).toBeInTheDocument()
      expect(screen.getByDisplayValue('bmp')).toBeInTheDocument()
      expect(screen.getByDisplayValue('raw')).toBeInTheDocument()
    })

    it('calls onChange("format", "bmp") when bmp radio is clicked', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings {...defaultProps} onChange={onChange} />)
      fireEvent.click(screen.getByDisplayValue('bmp'))
      expect(onChange).toHaveBeenCalledWith('format', 'bmp')
    })

    it('calls onChange("frameCount", 5) when frame count input changes', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings {...defaultProps} onChange={onChange} />)
      fireEvent.change(screen.getByPlaceholderText('∞ (제한없음)'), { target: { value: '5' } })
      expect(onChange).toHaveBeenCalledWith('frameCount', 5)
    })

    it('calls onChange("frameCount", null) when frame count input is cleared', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings {...defaultProps} config={makeConfig({ profileId: 'a.cam', frameCount: 5 })} onChange={onChange} />)
      fireEvent.change(screen.getByPlaceholderText('∞ (제한없음)'), { target: { value: '' } })
      expect(onChange).toHaveBeenCalledWith('frameCount', null)
    })
  })

  describe('trigger mode', () => {
    it('renders auto and manual radio buttons', () => {
      render(<AcquisitionSettings {...defaultProps} />)
      expect(screen.getByDisplayValue('auto')).toBeInTheDocument()
      expect(screen.getByDisplayValue('manual')).toBeInTheDocument()
    })

    it('calls onChange("acquisitionMode", "manual") when manual radio is clicked', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings {...defaultProps} onChange={onChange} />)
      fireEvent.click(screen.getByDisplayValue('manual'))
      expect(onChange).toHaveBeenCalledWith('acquisitionMode', 'manual')
    })

    it('shows interval input when acquisitionMode is auto', () => {
      render(<AcquisitionSettings {...defaultProps} config={makeConfig({ profileId: 'a.cam', acquisitionMode: 'auto' })} />)
      expect(screen.getByPlaceholderText('1')).toBeInTheDocument()
    })

    it('does not show interval input when acquisitionMode is manual', () => {
      render(<AcquisitionSettings {...defaultProps} config={makeConfig({ profileId: 'a.cam', acquisitionMode: 'manual' })} />)
      expect(screen.queryByPlaceholderText('1')).not.toBeInTheDocument()
    })

    it('calls onChange("intervalMs", 300) when interval input changes to 0.3', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings {...defaultProps} config={makeConfig({ profileId: 'a.cam', acquisitionMode: 'auto' })} onChange={onChange} />)
      fireEvent.change(screen.getByPlaceholderText('1'), { target: { value: '0.3' } })
      expect(onChange).toHaveBeenCalledWith('intervalMs', 300)
    })
  })

  describe('start button', () => {
    it('renders 촬영 시작 button', () => {
      render(<AcquisitionSettings {...defaultProps} />)
      expect(screen.getByText('촬영 시작')).toBeInTheDocument()
    })

    it('calls onStart when 촬영 시작 is clicked', () => {
      const onStart = vi.fn()
      render(<AcquisitionSettings {...defaultProps} onStart={onStart} />)
      fireEvent.click(screen.getByText('촬영 시작'))
      expect(onStart).toHaveBeenCalled()
    })

    it('disables 촬영 시작 when canStart is false', () => {
      render(<AcquisitionSettings {...defaultProps} canStart={false} />)
      expect(screen.getByText('촬영 시작').closest('button')).toBeDisabled()
    })

    it('disables 촬영 시작 when busy is true', () => {
      render(<AcquisitionSettings {...defaultProps} busy={true} />)
      expect(screen.getByText('촬영 시작').closest('button')).toBeDisabled()
    })
  })

  describe('quick start presets', () => {
    it('does not render presets section when presets are empty and not loading', () => {
      render(<AcquisitionSettings {...defaultProps} presets={[]} presetsLoading={false} />)
      expect(screen.queryByText('빠른 시작')).not.toBeInTheDocument()
    })

    it('renders skeleton when presetsLoading is true', () => {
      render(<AcquisitionSettings {...defaultProps} presetsLoading={true} />)
      expect(screen.getByText('빠른 시작')).toBeInTheDocument()
    })

    it('renders preset chips when presets are provided', () => {
      render(<AcquisitionSettings {...defaultProps} presets={[mockPreset]} />)
      expect(screen.getByText('Test Preset')).toBeInTheDocument()
    })

    it('calls onQuickStart when a preset chip is clicked', () => {
      const onQuickStart = vi.fn()
      render(<AcquisitionSettings {...defaultProps} presets={[mockPreset]} onQuickStart={onQuickStart} />)
      fireEvent.click(screen.getByText('Test Preset'))
      expect(onQuickStart).toHaveBeenCalledWith(mockPreset)
    })
  })

  describe('save preset modal', () => {
    it('opens save modal when preset save button is clicked', () => {
      render(<AcquisitionSettings {...defaultProps} />)
      fireEvent.click(screen.getByText('+ 프리셋으로 저장'))
      expect(screen.getByRole('dialog')).toBeInTheDocument()
    })

    it('calls onSavePreset with the entered name when 저장 is clicked', () => {
      const onSavePreset = vi.fn()
      render(<AcquisitionSettings {...defaultProps} onSavePreset={onSavePreset} />)
      fireEvent.click(screen.getByText('+ 프리셋으로 저장'))
      fireEvent.change(screen.getByPlaceholderText('프리셋 이름'), { target: { value: 'My Preset' } })
      fireEvent.click(screen.getByText('저장'))
      expect(onSavePreset).toHaveBeenCalledWith('My Preset')
    })
  })
})
