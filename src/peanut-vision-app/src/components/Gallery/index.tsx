import { useImageGallery } from '@/hooks/useImageGallery'
import ImageViewer from '@/components/shared/ImageViewer'
import ImageGallery from '@/components/shared/ImageGallery'
import cx from './cx'

export default function Gallery() {
  const gallery = useImageGallery()

  return (
    <div className={cx('Gallery')}>
      {/* Left: selected image viewer */}
      <div className={cx('viewer')}>
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={
            gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null
          }
          onClose={() => gallery.setSelectedId(null)}
        />
      </div>

      {/* Right: gallery grid */}
      <div className={cx('gridPane')}>
        <ImageGallery
          images={gallery.images}
          selectedId={gallery.selectedId}
          onSelect={gallery.setSelectedId}
          onDelete={gallery.handleDelete}
          page={gallery.page}
          totalPages={gallery.totalPages}
          onPageChange={gallery.setPage}
          filterSessionId={gallery.filterSessionId}
          onFilterChange={gallery.setFilterSessionId}
          isLoading={gallery.isLoading}
        />
      </div>
    </div>
  )
}
