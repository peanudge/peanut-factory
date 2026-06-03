import { useQuery } from '@tanstack/react-query'
import { ChevronDown, Trash2, X } from 'lucide-react'
import { getSessions, thumbnailUrl } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
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
  filterSessionId: string | null
  onFilterChange: (sessionId: string | null) => void
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
  filterSessionId,
  onFilterChange,
  isLoading,
}: Props) {
  const { data: sessions } = useQuery({
    queryKey: queryKeys.sessions,
    queryFn: () => getSessions(),
  })

  return (
    <div className={cx('wrap')}>
      {/* Session filter */}
      <div className={cx('filterRow')}>
        <select
          className={cx('select')}
          value={filterSessionId ?? ''}
          onChange={(e) => onFilterChange(e.target.value || null)}
        >
          <option value="">All sessions</option>
          {sessions?.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name}
            </option>
          ))}
        </select>
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
