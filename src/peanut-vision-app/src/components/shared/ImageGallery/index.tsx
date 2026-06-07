import { useEffect, useState } from 'react'
import {
  createColumnHelper,
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  useReactTable,
  type ColumnFiltersState,
  type SortingState,
} from '@tanstack/react-table'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Filter, RefreshCw, Trash2, X, ChevronUp, ChevronDown, ChevronsUpDown } from 'lucide-react'
import { listImages, deleteImage, thumbnailUrl } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'
import type { CapturedImageRecord } from '@/api/types'
import { formatTime } from '@/utils/formatTimestamp'
import cx from './cx'

interface Props {
  selectedId: string | null
  onRowSelect: (id: string | null, image: CapturedImageRecord | null) => void
}

type DateRangeFilter = { from: string | null; to: string | null }

function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

const columnHelper = createColumnHelper<CapturedImageRecord>()

export default function ImageGallery({ selectedId, onRowSelect }: Props) {
  const queryClient = useQueryClient()
  const { toast } = useToast()

  const [filterOpen, setFilterOpen] = useState(false)
  const [sorting, setSorting] = useState<SortingState>([{ id: 'capturedAt', desc: true }])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])

  // Derive server query params from TanStack Table column filter state
  const dateRange = (columnFilters.find(f => f.id === 'capturedAt')?.value ?? {}) as DateRangeFilter
  const isFiltered = !!(dateRange.from || dateRange.to)

  const { data, isLoading } = useQuery({
    queryKey: queryKeys.images({
      dateFrom: dateRange.from ?? undefined,
      dateTo: dateRange.to ?? undefined,
    }),
    queryFn: () => listImages({
      page: 1,
      pageSize: 100,
      dateFrom: dateRange.from ?? undefined,
      dateTo: dateRange.to ?? undefined,
    }),
  })

  const images = data?.items ?? []

  // Keep selection in sync: clear if selected image disappears after filter
  useEffect(() => {
    if (selectedId && images.length > 0 && !images.find(i => i.id === selectedId)) {
      onRowSelect(null, null)
    }
    // Auto-select first row when data arrives and nothing is selected
    if (!selectedId && images.length > 0) {
      onRowSelect(images[0].id, images[0])
    }
  }, [images, selectedId]) // eslint-disable-line react-hooks/exhaustive-deps

  const deleteMutation = useMutation({
    mutationFn: deleteImage,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.images() })
      if (selectedId === id) onRowSelect(null, null)
      toast('이미지가 삭제되었습니다', 'info')
    },
    onError: () => toast('삭제에 실패했습니다', 'error'),
  })

  const setDateFilter = (range: Partial<DateRangeFilter>) => {
    setColumnFilters(prev => {
      const current = (prev.find(f => f.id === 'capturedAt')?.value ?? {}) as DateRangeFilter
      const next = { ...current, ...range }
      const without = prev.filter(f => f.id !== 'capturedAt')
      return (!next.from && !next.to) ? without : [...without, { id: 'capturedAt', value: next }]
    })
  }

  const clearDateFilter = () => {
    setColumnFilters(prev => prev.filter(f => f.id !== 'capturedAt'))
  }

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
      enableSorting: false,
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
      enableSorting: false,
      cell: ({ row }) => (
        <button
          type="button"
          className={cx('rowDeleteBtn')}
          onClick={(e) => { e.stopPropagation(); deleteMutation.mutate(row.original.id) }}
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
    state: { sorting, columnFilters },
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    manualFiltering: true, // server handles filtering
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  })

  const handleToggleFilter = () => {
    if (filterOpen && isFiltered) clearDateFilter()
    setFilterOpen(prev => !prev)
  }

  const refresh = () => queryClient.invalidateQueries({ queryKey: queryKeys.images() })

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
          onClick={refresh}
          disabled={isLoading}
          title="Refresh"
        >
          <RefreshCw size={14} />
        </button>
      </div>

      {/* Date range filter — driven by TanStack Table columnFilters */}
      {filterOpen && (
        <div className={cx('filterRow')}>
          <input
            type="date"
            className={cx('dateInput')}
            value={dateRange.from ?? ''}
            onChange={(e) => setDateFilter({ from: e.target.value || null })}
          />
          <span>–</span>
          <input
            type="date"
            className={cx('dateInput')}
            value={dateRange.to ?? ''}
            onChange={(e) => setDateFilter({ to: e.target.value || null })}
          />
          {isFiltered && (
            <button
              type="button"
              className={cx('clearBtn', 'clearFilter')}
              onClick={clearDateFilter}
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
                  onClick={() => onRowSelect(row.original.id, row.original)}
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

      {/* Footer */}
      {images.length > 0 && (
        <div className={cx('actions')}>
          <span className={cx('count')}>{images.length} images</span>
          <button
            type="button"
            className={cx('clearBtn')}
            onClick={() => images.forEach((img) => deleteMutation.mutate(img.id))}
          >
            <Trash2 size={13} /> Clear All
          </button>
        </div>
      )}
    </div>
  )
}
