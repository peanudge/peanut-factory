import { useImageGallery } from '@/hooks/useImageGallery'
import ImageViewer from '@/components/shared/ImageViewer'
import ImageGallery from '@/components/shared/ImageGallery'
import cx from './cx'

export default function Gallery() {
  const { selectedId, selectedImage, selectedImageUrl, handleRowSelect } = useImageGallery()

  return (
    <div className={cx('Gallery')}>
      <div className={cx('viewer')}>
        <ImageViewer
          url={selectedImageUrl}
          filename={selectedImage?.filename}
          savedPath={selectedImage?.filePath}
          isLive={false}
          capturedAt={selectedImage ? new Date(selectedImage.capturedAt) : null}
        />
      </div>

      <div className={cx('gridPane')}>
        <ImageGallery
          selectedId={selectedId}
          onRowSelect={handleRowSelect}
        />
      </div>
    </div>
  )
}
