import { useState } from 'react'
import {
  createColumnHelper,
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  useReactTable,
  type SortingState,
} from '@tanstack/react-table'
import { Filter, RefreshCw, Trash2, X, ChevronUp, ChevronDown, ChevronsUpDown } from 'lucide-react'
import { thumbnailUrl } from '@/api/client'
import type { CapturedImageRecord } from '@/api/types'
import { formatTime } from '@/utils/formatTimestamp'
import cx from './cx'

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

const columnHelper = createColumnHelper<CapturedImageRecord>()

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
  const [sorting, setSorting] = useState<SortingState>([
    { id: 'capturedAt', desc: true },
  ])
  const isFiltered = !!(dateFrom || dateTo)

  const columns = [
    columnHelper.display({
      id: 'thumbnail',
      header: '',
      size: 44,
      cell: ({ row }) => {
        const img = row.original
        return img.hasThumbnail ? (
          <img
            src={thumbnailUrl(img.id)}
            alt=""
            className={cx('rowThumb')}
            onError={(e) => { e.currentTarget.style.display = 'none' }}
          />
        ) : (
          <div className={cx('rowThumbPlaceholder')}>{img.format}</div>
        )
      },
    }),
    columnHelper.accessor('filename', {
      header: 'Filename',
      cell: ({ getValue, row }) => (
        <span title={row.original.filePath}>{getValue()}</span>
      ),
    }),
    columnHelper.display({
      id: 'resolution',
      header: 'Resolution',
      cell: ({ row }) => `${row.original.width} × ${row.original.height}`,
    }),
    columnHelper.accessor('fileSizeBytes', {
      header: 'Size',
      cell: ({ getValue }) => formatBytes(getValue()),
    }),
    columnHelper.accessor('format', {
      header: 'Format',
      cell: ({ getValue }) => getValue().toUpperCase(),
    }),
    columnHelper.accessor('capturedAt', {
      header: 'Captured',
      cell: ({ getValue }) => formatTime(new Date(getValue())),
    }),
    columnHelper.display({
      id: 'delete',
      header: '',
      size: 36,
      cell: ({ row }) => (
        <button
          type="button"
          className={cx('rowDeleteBtn')}
          onClick={(e) => { e.stopPropagation(); onDelete(row.original.id) }}
          title="Delete"
        >
          <X size={12} />
        </button>
      ),
    }),
  ]

  const table = useReactTable({
    data: images,
    columns,
    state: { sorting },
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  })

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
              className={cx('clearBtn', 'clearFilter')}
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

      {/* Table */}
      {!isLoading && images.length > 0 && (
        <div className={cx('tableWrap')}>
          <table className={cx('table')}>
            <thead>
              {table.getHeaderGroups().map((hg) => (
                <tr key={hg.id}>
                  {hg.headers.map((header) => {
                    const canSort = header.column.getCanSort()
                    const sortDir = header.column.getIsSorted()
                    return (
                      <th
                        key={header.id}
                        style={{ width: header.getSize() }}
                        className={cx({ sortable: canSort })}
                        onClick={canSort ? header.column.getToggleSortingHandler() : undefined}
                      >
                        <span className={cx('thContent')}>
                          {flexRender(header.column.columnDef.header, header.getContext())}
                          {canSort && (
                            <span className={cx('sortIcon')}>
                              {sortDir === 'asc' ? <ChevronUp size={11} /> :
                               sortDir === 'desc' ? <ChevronDown size={11} /> :
                               <ChevronsUpDown size={11} />}
                            </span>
                          )}
                        </span>
                      </th>
                    )
                  })}
                </tr>
              ))}
            </thead>
            <tbody>
              {table.getRowModel().rows.map((row) => (
                <tr
                  key={row.id}
                  className={cx('row', { selected: row.original.id === selectedId })}
                  onClick={() => onSelect(row.original.id)}
                >
                  {row.getVisibleCells().map((cell) => (
                    <td
                      key={cell.id}
                      className={cx({
                        tdThumb: cell.column.id === 'thumbnail',
                        tdFilename: cell.column.id === 'filename',
                        tdMeta: !['thumbnail', 'filename', 'delete'].includes(cell.column.id),
                      })}
                    >
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Clear all */}
      {images.length > 0 && (
        <div className={cx('actions')}>
          <span className={cx('count')}>{images.length} images</span>
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
