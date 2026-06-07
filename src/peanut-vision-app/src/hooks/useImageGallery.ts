import { useState } from 'react'
import { imageFileUrl } from '@/api/client'
import type { CapturedImageRecord } from '@/api/types'

export function useImageGallery() {
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [selectedImage, setSelectedImage] = useState<CapturedImageRecord | null>(null)

  const selectedImageUrl = selectedId ? imageFileUrl(selectedId) : null

  const handleRowSelect = (id: string | null, image: CapturedImageRecord | null) => {
    setSelectedId(id)
    setSelectedImage(image)
  }

  return { selectedId, selectedImage, selectedImageUrl, handleRowSelect }
}
