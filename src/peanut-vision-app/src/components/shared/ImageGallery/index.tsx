import { useState } from 'react'
import { Filter, LayoutGrid, List, RefreshCw, Trash2, X } from 'lucide-react'
import { thumbnailUrl } from '@/api/client'
import type { CapturedImageRecord } from '@/api/types'
import { formatTime } from '@/utils/formatTimestamp'
import cx from './cx'

type ViewMode = 'grid' | 'table'

interface Props {
  images: CapturedImageRecord[]
  selectedId: string | null
  onSelect: (id: string) => void
  onDelete: (id: string) => void
  dateFrom: string | null
  dateTo: string | null
  onDateFromChange: (v: string | null) => void
  onDateToChange: (v: string | null) => void
  isLoading: boolean
  onRefresh: () => void
}

function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

export default function ImageGallery({
  images,
  selectedId,
  onSelect,
  onDelete,
  dateFrom,
  dateTo,
  onDateFromChange,
  onDateToChange,
  isLoading,
  onRefresh,
}: Props) {
  const [filterOpen, setFilterOpen] = useState(false)
  const [viewMode, setViewMode] = useState<ViewMode>('grid')
  const isFiltered = !!(dateFrom || dateTo)

  const handleToggleFilter = () => {
    if (filterOpen && isFiltered) {
      onDateFromChange(null)
      onDateToChange(null)
    }
    setFilterOpen((prev) => !prev)
  }

  return (
    <div className={cx('wrap')}>
      {/* Toolbar */}
      <div className={cx('toolbar')}>
        <div className={cx('toolbarLeft')}>
          <button
            type="button"
            className={cx('iconBtn', { active: isFiltered })}
            onClick={handleToggleFilter}
            title={isFiltered ? 'Clear date filter' : 'Filter by date'}
          >
            <Filter size={14} />
            {isFiltered && <span className={cx('badge')} />}
          </button>
          <button
            type="button"
            className={cx('iconBtn')}
            onClick={onRefresh}
            disabled={isLoading}
            title="Refresh"
          >
            <RefreshCw size={14} />
          </button>
        </div>

        <div className={cx('viewToggle')}>
          <button
            type="button"
            className={cx('iconBtn', { active: viewMode === 'grid' })}
            onClick={() => setViewMode('grid')}
            title="Grid view"
          >
            <LayoutGrid size={14} />
          </button>
          <button
            type="button"
            className={cx('iconBtn', { active: viewMode === 'table' })}
            onClick={() => setViewMode('table')}
            title="Table view"
          >
            <List size={14} />
          </button>
        </div>
      </div>

      {/* Date range inputs */}
      {filterOpen && (
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
          {isFiltered && (
            <button
              type="button"
              className={cx('clearBtn')}
              onClick={() => { onDateFromChange(null); onDateToChange(null) }}
              title="Clear filter"
            >
              <X size={14} />
            </button>
          )}
        </div>
      )}

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

      {/* Grid view */}
      {!isLoading && images.length > 0 && viewMode === 'grid' && (
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
                  onError={(e) => { e.currentTarget.style.display = 'none' }}
                />
              ) : (
                <div className={cx('placeholder')}>{img.format}</div>
              )}
              <button
                type="button"
                className={cx('deleteBtn')}
                onClick={(e) => { e.stopPropagation(); onDelete(img.id) }}
              >
                <X size={10} />
              </button>
            </div>
          ))}
        </div>
      )}

      {/* Table view */}
      {!isLoading && images.length > 0 && viewMode === 'table' && (
        <div className={cx('tableWrap')}>
          <table className={cx('table')}>
            <thead>
              <tr>
                <th className={cx('thThumb')}></th>
                <th>Filename</th>
                <th>Resolution</th>
                <th>Size</th>
                <th>Format</th>
                <th>Captured</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {images.map((img) => (
                <tr
                  key={img.id}
                  className={cx('row', { selected: img.id === selectedId })}
                  onClick={() => onSelect(img.id)}
                >
                  <td className={cx('tdThumb')}>
                    {img.hasThumbnail ? (
                      <img
                        src={thumbnailUrl(img.id)}
                        alt=""
                        className={cx('rowThumb')}
                        onError={(e) => { e.currentTarget.style.display = 'none' }}
                      />
                    ) : (
                      <div className={cx('rowThumbPlaceholder')}>{img.format}</div>
                    )}
                  </td>
                  <td className={cx('tdFilename')} title={img.filePath}>{img.filename}</td>
                  <td className={cx('tdMeta')}>{img.width} × {img.height}</td>
                  <td className={cx('tdMeta')}>{formatBytes(img.fileSizeBytes)}</td>
                  <td className={cx('tdMeta')}>{img.format.toUpperCase()}</td>
                  <td className={cx('tdMeta')}>{formatTime(new Date(img.capturedAt))}</td>
                  <td>
                    <button
                      type="button"
                      className={cx('rowDeleteBtn')}
                      onClick={(e) => { e.stopPropagation(); onDelete(img.id) }}
                      title="Delete"
                    >
                      <X size={12} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Clear all */}
      {images.length > 0 && (
        <div className={cx('actions')}>
          <span />
          <button
            type="button"
            className={cx('clearBtn')}
            onClick={() => images.forEach((img) => onDelete(img.id))}
          >
            <Trash2 size={13} /> Clear All
          </button>
        </div>
      )}
    </div>
  )
}
