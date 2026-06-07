import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import AcquisitionActionBar from '@/components/shared/AcquisitionActionBar'

vi.mock('@/components/shared/AcquisitionActionBar/cx', () => ({
  default: (...args: string[]) => args.filter(Boolean).join(' '),
}))
vi.mock('@/components/shared/StatusChip', () => ({
  default: ({ label }: { label: string }) => <span>{label}</span>,
}))

const defaultProps = {
  isActive: false,
  profileLabel: undefined,
  canStart: true,
  canStop: false,
  canTrigger: false,
  acquisitionMode: 'auto' as const,
  busy: false,
  onStart: vi.fn(),
  onStop: vi.fn(),
  onTrigger: vi.fn(),
  onRefresh: vi.fn(),
  refreshThrottled: false,
}

describe('AcquisitionActionBar', () => {
  describe('when not active (canStart=true)', () => {
    it('shows Start button', () => {
      render(<AcquisitionActionBar {...defaultProps} />)
      expect(screen.getByText(/Start/)).toBeInTheDocument()
    })

    it('calls onStart when Start is clicked', () => {
      const onStart = vi.fn()
      render(<AcquisitionActionBar {...defaultProps} onStart={onStart} />)
      fireEvent.click(screen.getByText(/Start/))
      expect(onStart).toHaveBeenCalledOnce()
    })

    it('Start button is disabled when canStart=false', () => {
      render(<AcquisitionActionBar {...defaultProps} canStart={false} />)
      expect(screen.getByText(/Start/).closest('button')).toBeDisabled()
    })

    it('Start button is disabled when busy', () => {
      render(<AcquisitionActionBar {...defaultProps} busy />)
      expect(screen.getByText(/Start/).closest('button')).toBeDisabled()
    })
  })

  describe('when active (canStop=true)', () => {
    it('shows Stop button instead of Start', () => {
      render(<AcquisitionActionBar {...defaultProps} canStart={false} canStop={true} />)
      expect(screen.getByText(/Stop/)).toBeInTheDocument()
      expect(screen.queryByText(/Start/)).not.toBeInTheDocument()
    })

    it('calls onStop when Stop is clicked', () => {
      const onStop = vi.fn()
      render(<AcquisitionActionBar {...defaultProps} canStart={false} canStop={true} onStop={onStop} />)
      fireEvent.click(screen.getByText(/Stop/))
      expect(onStop).toHaveBeenCalledOnce()
    })
  })

  describe('Trigger button (manual mode)', () => {
    it('shows Trigger button in manual mode when canTrigger=true', () => {
      render(<AcquisitionActionBar {...defaultProps} canTrigger={true} acquisitionMode="manual" />)
      expect(screen.getByText(/Trigger/)).toBeInTheDocument()
    })

    it('does not show Trigger button in auto mode', () => {
      render(<AcquisitionActionBar {...defaultProps} canTrigger={true} acquisitionMode="auto" />)
      expect(screen.queryByText(/Trigger/)).not.toBeInTheDocument()
    })

    it('does not show Trigger button when canTrigger=false', () => {
      render(<AcquisitionActionBar {...defaultProps} canTrigger={false} acquisitionMode="manual" />)
      expect(screen.queryByText(/Trigger/)).not.toBeInTheDocument()
    })

    it('calls onTrigger when Trigger is clicked', () => {
      const onTrigger = vi.fn()
      render(<AcquisitionActionBar {...defaultProps} canTrigger={true} acquisitionMode="manual" onTrigger={onTrigger} />)
      fireEvent.click(screen.getByText(/Trigger/))
      expect(onTrigger).toHaveBeenCalledOnce()
    })
  })

  describe('Refresh button', () => {
    it('calls onRefresh when clicked', () => {
      const onRefresh = vi.fn()
      render(<AcquisitionActionBar {...defaultProps} onRefresh={onRefresh} />)
      // Refresh is the last button in the bar
      const buttons = screen.getAllByRole('button')
      fireEvent.click(buttons[buttons.length - 1])
      expect(onRefresh).toHaveBeenCalledOnce()
    })

    it('is disabled when refreshThrottled=true', () => {
      render(<AcquisitionActionBar {...defaultProps} refreshThrottled />)
      const buttons = screen.getAllByRole('button')
      expect(buttons[buttons.length - 1]).toBeDisabled()
    })
  })

  describe('StatusChip', () => {
    it('shows Active label when isActive', () => {
      render(<AcquisitionActionBar {...defaultProps} isActive={true} profileLabel="cam.cam" />)
      expect(screen.getByText('Active (cam.cam)')).toBeInTheDocument()
    })

    it('shows Inactive when not active', () => {
      render(<AcquisitionActionBar {...defaultProps} isActive={false} />)
      expect(screen.getByText('Inactive')).toBeInTheDocument()
    })
  })
})
