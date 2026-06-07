import { useImageGallery } from '@/hooks/useImageGallery'
import ImageViewer from '@/components/shared/ImageViewer'
import ImageGallery from '@/components/shared/ImageGallery'
import cx from './cx'

export default function Gallery() {
  const gallery = useImageGallery()

  return (
    <div className={cx('Gallery')}>
      <div className={cx('viewer')}>
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={
            gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null
          }
        />
      </div>

      <div className={cx('gridPane')}>
        <ImageGallery
          images={gallery.images}
          selectedId={gallery.selectedId}
          onSelect={gallery.setSelectedId}
          onDelete={gallery.handleDelete}
          dateFrom={gallery.dateFrom}
          dateTo={gallery.dateTo}
          onDateFromChange={gallery.setDateFrom}
          onDateToChange={gallery.setDateTo}
          isLoading={gallery.isLoading}
          onRefresh={gallery.refresh}
        />
      </div>
    </div>
  )
}
