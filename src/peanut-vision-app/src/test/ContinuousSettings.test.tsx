import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import ContinuousSettings from '@/components/shared/ContinuousSettings'

// SCSS modules → return className as-is
vi.mock('@/components/shared/ContinuousSettings/cx', () => ({
  default: (...args: string[]) => args.filter(Boolean).join(' '),
}))

const defaultProps = {
  acquisitionMode: 'auto' as const,
  onAcquisitionModeChange: vi.fn(),
  frameCount: null,
  onFrameCountChange: vi.fn(),
  intervalMs: null,
  onIntervalMsChange: vi.fn(),
  disabled: false,
}

describe('ContinuousSettings', () => {
  describe('mode cards', () => {
    it('renders 자동 촬영 and 수동 촬영 cards', () => {
      render(<ContinuousSettings {...defaultProps} />)
      expect(screen.getByText('자동 촬영')).toBeInTheDocument()
      expect(screen.getByText('수동 촬영')).toBeInTheDocument()
    })

    it('calls onAcquisitionModeChange when manual card is clicked', () => {
      const onChange = vi.fn()
      render(<ContinuousSettings {...defaultProps} onAcquisitionModeChange={onChange} />)
      fireEvent.click(screen.getByText('수동 촬영'))
      expect(onChange).toHaveBeenCalledWith('manual')
    })

    it('calls onAcquisitionModeChange when auto card is clicked', () => {
      const onChange = vi.fn()
      render(<ContinuousSettings {...defaultProps} acquisitionMode="manual" onAcquisitionModeChange={onChange} />)
      fireEvent.click(screen.getByText('자동 촬영'))
      expect(onChange).toHaveBeenCalledWith('auto')
    })
  })

  describe('auto mode', () => {
    it('shows Interval and Stop after inputs', () => {
      render(<ContinuousSettings {...defaultProps} acquisitionMode="auto" />)
      expect(screen.getByText('Interval')).toBeInTheDocument()
      expect(screen.getByText('Stop after')).toBeInTheDocument()
      // Two number inputs rendered
      expect(screen.getAllByRole('spinbutton')).toHaveLength(2)
    })

    it('calls onIntervalMsChange when interval input changes', () => {
      const onChange = vi.fn()
      render(<ContinuousSettings {...defaultProps} acquisitionMode="auto" onIntervalMsChange={onChange} />)
      fireEvent.change(screen.getByPlaceholderText('500'), { target: { value: '300' } })
      expect(onChange).toHaveBeenCalledWith(300)
    })

    it('calls onFrameCountChange with null when input is cleared', () => {
      const onChange = vi.fn()
      render(<ContinuousSettings {...defaultProps} acquisitionMode="auto" frameCount={5} onFrameCountChange={onChange} />)
      // Second spinbutton is Stop after (frames)
      const inputs = screen.getAllByRole('spinbutton')
      fireEvent.change(inputs[1], { target: { value: '' } })
      expect(onChange).toHaveBeenCalledWith(null)
    })

    it('shows hint text for interval and stop after', () => {
      render(<ContinuousSettings {...defaultProps} acquisitionMode="auto" />)
      expect(screen.getByText('최소 50ms')).toBeInTheDocument()
      expect(screen.getByText('비우면 제한없음')).toBeInTheDocument()
    })
  })

  describe('manual mode', () => {
    it('shows trigger description text', () => {
      render(<ContinuousSettings {...defaultProps} acquisitionMode="manual" />)
      expect(screen.getByText(/Trigger/)).toBeInTheDocument()
    })

    it('does not show Interval or Stop after inputs', () => {
      render(<ContinuousSettings {...defaultProps} acquisitionMode="manual" />)
      expect(screen.queryByLabelText(/Interval/i)).not.toBeInTheDocument()
      expect(screen.queryByLabelText(/Stop after/i)).not.toBeInTheDocument()
    })
  })

  describe('disabled state', () => {
    it('disables mode cards when disabled', () => {
      render(<ContinuousSettings {...defaultProps} disabled />)
      const cards = screen.getAllByRole('button')
      cards.forEach(card => expect(card).toBeDisabled())
    })

    it('disables inputs in auto mode when disabled', () => {
      render(<ContinuousSettings {...defaultProps} acquisitionMode="auto" disabled />)
      const inputs = screen.getAllByRole('spinbutton')
      inputs.forEach(input => expect(input).toBeDisabled())
    })
  })
})
