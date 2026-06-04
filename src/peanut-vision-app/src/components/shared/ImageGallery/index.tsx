import { ChevronDown, Trash2, X } from 'lucide-react'
import { thumbnailUrl } from '@/api/client'
import type { CapturedImageRecord } from '@/api/types'
import { formatTime } from '@/utils/formatTimestamp'
import cx from './cx'

interface Props {
  images: CapturedImageRecord[]
  selectedId: string | null
  onSelect: (id: string) => void
  onDelete: (id: string) => void
  page: number
  totalPages: number
  onPageChange: (p: number) => void
  dateFrom: string | null
  dateTo: string | null
  onDateFromChange: (v: string | null) => void
  onDateToChange: (v: string | null) => void
  isLoading: boolean
}

export default function ImageGallery({
  images,
  selectedId,
  onSelect,
  onDelete,
  page,
  totalPages,
  onPageChange,
  dateFrom,
  dateTo,
  onDateFromChange,
  onDateToChange,
  isLoading,
}: Props) {
  return (
    <div className={cx('wrap')}>
      {/* Date range filter */}
      <div className={cx('filterRow')}>
        <input
          type="date"
          className={cx('dateInput')}
          value={dateFrom ?? ''}
          onChange={(e) => onDateFromChange(e.target.value || null)}
        />
        <span>–</span>
        <input
          type="date"
          className={cx('dateInput')}
          value={dateTo ?? ''}
          onChange={(e) => onDateToChange(e.target.value || null)}
        />
        {(dateFrom || dateTo) && (
          <button
            type="button"
            className={cx('clearBtn')}
            onClick={() => { onDateFromChange(null); onDateToChange(null) }}
          >
            <X size={14} />
          </button>
        )}
      </div>

      {/* Loading */}
      {isLoading && (
        <div className={cx('loadingWrap')}>
          <span className={cx('spinner')} />
        </div>
      )}

      {/* Empty */}
      {!isLoading && images.length === 0 && (
        <div className={cx('empty')}>No captures yet</div>
      )}

      {/* Grid */}
      {images.length > 0 && (
        <div className={cx('grid')}>
          {images.map((img) => (
            <div
              key={img.id}
              className={cx('thumb', { selected: img.id === selectedId })}
              onClick={() => onSelect(img.id)}
              title={`${img.filename}\n${formatTime(new Date(img.capturedAt))}\n${img.width}×${img.height} · ${img.format.toUpperCase()}`}
            >
              {img.hasThumbnail ? (
                <img
                  src={thumbnailUrl(img.id)}
                  alt={img.filename}
                  onError={(e) => {
                    e.currentTarget.style.display = 'none'
                  }}
                />
              ) : (
                <div className={cx('placeholder')}>{img.format}</div>
              )}
              <button
                type="button"
                className={cx('deleteBtn')}
                onClick={(e) => {
                  e.stopPropagation()
                  onDelete(img.id)
                }}
              >
                <X size={10} />
              </button>
            </div>
          ))}
        </div>
      )}

      {/* Bottom actions */}
      <div className={cx('actions')}>
        {page < totalPages ? (
          <button
            type="button"
            className={cx('loadMoreBtn')}
            onClick={() => onPageChange(page + 1)}
          >
            <ChevronDown size={13} /> Load more
          </button>
        ) : (
          <span />
        )}
        {images.length > 0 && page === 1 && (
          <button
            type="button"
            className={cx('clearBtn')}
            onClick={() => images.forEach((img) => onDelete(img.id))}
          >
            <Trash2 size={13} /> Clear All
          </button>
        )}
      </div>
    </div>
  )
}
