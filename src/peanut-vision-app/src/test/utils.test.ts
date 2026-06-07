import { describe, it, expect } from 'vitest'

// formatBytes — extracted from ImageGallery for testing
function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

describe('formatBytes', () => {
  it('formats bytes under 1KB', () => {
    expect(formatBytes(0)).toBe('0 B')
    expect(formatBytes(512)).toBe('512 B')
    expect(formatBytes(1023)).toBe('1023 B')
  })

  it('formats kilobytes', () => {
    expect(formatBytes(1024)).toBe('1.0 KB')
    expect(formatBytes(2048)).toBe('2.0 KB')
    expect(formatBytes(1536)).toBe('1.5 KB')
  })

  it('formats megabytes', () => {
    expect(formatBytes(1024 * 1024)).toBe('1.0 MB')
    expect(formatBytes(1024 * 1024 * 5.5)).toBe('5.5 MB')
  })
})

// formatTime
import { formatTime } from '@/utils/formatTimestamp'

describe('formatTime', () => {
  it('formats time as HH:MM:SS', () => {
    const date = new Date('2026-06-07T14:30:05')
    expect(formatTime(date)).toBe('14:30:05')
  })

  it('pads single digits with zeros', () => {
    const date = new Date('2026-06-07T01:02:03')
    expect(formatTime(date)).toBe('01:02:03')
  })
})
