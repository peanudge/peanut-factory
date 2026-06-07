import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import type { AcquisitionFormConfig } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'

vi.mock('@/components/shared/AcquisitionSettings/cx', () => ({
  default: (...args: string[]) => args.filter(Boolean).join(' '),
}))
vi.mock('@/components/shared/DirectoryBrowser', () => ({
  default: () => null,
}))

const makeConfig = (overrides: Partial<AcquisitionFormConfig> = {}): AcquisitionFormConfig => ({
  ...DEFAULT_ACQUISITION_FORM_CONFIG,
  ...overrides,
})

describe('AcquisitionSettings', () => {
  describe('mode cards', () => {
    it('renders 자동 촬영 and 수동 촬영 cards', () => {
      render(<AcquisitionSettings config={makeConfig()} onChange={vi.fn()} />)
      expect(screen.getByText('자동 촬영')).toBeInTheDocument()
      expect(screen.getByText('수동 촬영')).toBeInTheDocument()
    })

    it('calls onChange("acquisitionMode", "manual") when manual card is clicked', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings config={makeConfig()} onChange={onChange} />)
      fireEvent.click(screen.getByText('수동 촬영'))
      expect(onChange).toHaveBeenCalledWith('acquisitionMode', 'manual')
    })

    it('calls onChange("acquisitionMode", "auto") when auto card is clicked', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'manual' })} onChange={onChange} />)
      fireEvent.click(screen.getByText('자동 촬영'))
      expect(onChange).toHaveBeenCalledWith('acquisitionMode', 'auto')
    })
  })

  describe('auto mode', () => {
    it('shows Interval and Stop after inputs', () => {
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'auto' })} onChange={vi.fn()} />)
      expect(screen.getByText('Interval')).toBeInTheDocument()
      expect(screen.getByText('Stop after')).toBeInTheDocument()
      expect(screen.getAllByRole('spinbutton')).toHaveLength(2)
    })

    it('calls onChange("intervalMs", 300) when interval input changes', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'auto' })} onChange={onChange} />)
      fireEvent.change(screen.getByPlaceholderText('500'), { target: { value: '300' } })
      expect(onChange).toHaveBeenCalledWith('intervalMs', 300)
    })

    it('calls onChange("frameCount", null) when stop-after input is cleared', () => {
      const onChange = vi.fn()
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'auto', frameCount: 5 })} onChange={onChange} />)
      const inputs = screen.getAllByRole('spinbutton')
      fireEvent.change(inputs[1], { target: { value: '' } })
      expect(onChange).toHaveBeenCalledWith('frameCount', null)
    })

    it('shows hint text for interval and stop after', () => {
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'auto' })} onChange={vi.fn()} />)
      expect(screen.getByText('최소 50ms')).toBeInTheDocument()
      expect(screen.getByText('비우면 제한없음')).toBeInTheDocument()
    })
  })

  describe('manual mode', () => {
    it('shows trigger description text', () => {
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'manual' })} onChange={vi.fn()} />)
      expect(screen.getByText(/Trigger/)).toBeInTheDocument()
    })

    it('does not show Interval or Stop after inputs', () => {
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'manual' })} onChange={vi.fn()} />)
      expect(screen.queryByText('Interval')).not.toBeInTheDocument()
      expect(screen.queryByText('Stop after')).not.toBeInTheDocument()
    })
  })

  describe('disabled state', () => {
    it('disables mode cards when disabled', () => {
      render(<AcquisitionSettings config={makeConfig()} onChange={vi.fn()} disabled />)
      const cards = screen.getAllByRole('button')
      cards.forEach(card => expect(card).toBeDisabled())
    })

    it('disables inputs in auto mode when disabled', () => {
      render(<AcquisitionSettings config={makeConfig({ acquisitionMode: 'auto' })} onChange={vi.fn()} disabled />)
      const inputs = screen.getAllByRole('spinbutton')
      inputs.forEach(input => expect(input).toBeDisabled())
    })
  })
})
