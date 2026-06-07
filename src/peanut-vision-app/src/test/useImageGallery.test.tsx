import { describe, it, expect, vi } from 'vitest'
import { renderHook, act } from '@testing-library/react'
import { useImageGallery } from '@/hooks/useImageGallery'
import type { CapturedImageRecord } from '@/api/types'

vi.mock('@/api/client', () => ({
  imageFileUrl: (id: string) => `http://localhost/api/images/${id}/file`,
}))

const makeImage = (id: string): CapturedImageRecord => ({
  id,
  filePath: `/captures/${id}.png`,
  filename: `${id}.png`,
  width: 4160,
  height: 3120,
  fileSizeBytes: 1024 * 500,
  format: 'png',
  capturedAt: new Date().toISOString(),
})

describe('useImageGallery', () => {
  it('starts with no selection', () => {
    const { result } = renderHook(() => useImageGallery())
    expect(result.current.selectedId).toBeNull()
    expect(result.current.selectedImage).toBeNull()
    expect(result.current.selectedImageUrl).toBeNull()
  })

  it('handleRowSelect updates selectedId and selectedImage', () => {
    const { result } = renderHook(() => useImageGallery())
    const image = makeImage('img-1')
    act(() => { result.current.handleRowSelect('img-1', image) })
    expect(result.current.selectedId).toBe('img-1')
    expect(result.current.selectedImage).toEqual(image)
  })

  it('selectedImageUrl is derived from selectedId', () => {
    const { result } = renderHook(() => useImageGallery())
    act(() => { result.current.handleRowSelect('img-1', makeImage('img-1')) })
    expect(result.current.selectedImageUrl).toBe('http://localhost/api/images/img-1/file')
  })

  it('handleRowSelect with null clears selection', () => {
    const { result } = renderHook(() => useImageGallery())
    act(() => { result.current.handleRowSelect('img-1', makeImage('img-1')) })
    act(() => { result.current.handleRowSelect(null, null) })
    expect(result.current.selectedId).toBeNull()
    expect(result.current.selectedImage).toBeNull()
    expect(result.current.selectedImageUrl).toBeNull()
  })

  it('handles multiple selection changes', () => {
    const { result } = renderHook(() => useImageGallery())
    const img1 = makeImage('img-1')
    const img2 = makeImage('img-2')
    act(() => { result.current.handleRowSelect('img-1', img1) })
    act(() => { result.current.handleRowSelect('img-2', img2) })
    expect(result.current.selectedId).toBe('img-2')
    expect(result.current.selectedImage?.id).toBe('img-2')
  })
})
