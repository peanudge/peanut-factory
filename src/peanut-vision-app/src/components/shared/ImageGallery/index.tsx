import { useEffect, useState } from 'react'
import {
  createColumnHelper,
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  useReactTable,
  type ColumnFiltersState,
  type RowSelectionState,
  type SortingState,
} from '@tanstack/react-table'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Filter, RefreshCw, Trash2, X, ChevronUp, ChevronDown, ChevronsUpDown, ChevronLeft, ChevronRight } from 'lucide-react'
import { listImages, deleteImage } from '@/api/client'
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
  const [rowSelection, setRowSelection] = useState<RowSelectionState>({})
  const [page, setPage] = useState(1)
  const pageSize = 50

  const dateRange = (columnFilters.find(f => f.id === 'capturedAt')?.value ?? {}) as DateRangeFilter
  const isFiltered = !!(dateRange.from || dateRange.to)

  const { data, isLoading } = useQuery({
    queryKey: queryKeys.images({
      dateFrom: dateRange.from ?? undefined,
      dateTo: dateRange.to ?? undefined,
      page,
    }),
    queryFn: () => listImages({
      page,
      pageSize,
      dateFrom: dateRange.from ?? undefined,
      dateTo: dateRange.to ?? undefined,
    }),
  })

  const totalPages = data?.totalPages ?? 1
  const totalCount = data?.totalCount ?? 0

  const images = data?.items ?? []

  useEffect(() => {
    if (selectedId && images.length > 0 && !images.find(i => i.id === selectedId)) {
      onRowSelect(null, null)
    }
    if (!selectedId && images.length > 0) {
      onRowSelect(images[0].id, images[0])
    }
  }, [images, selectedId]) // eslint-disable-line react-hooks/exhaustive-deps

  // Clear row selection when images list changes (e.g. after delete)
  useEffect(() => {
    setRowSelection({})
  }, [images.length])

  const deleteMutation = useMutation({
    mutationFn: deleteImage,
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.images() })
      if (selectedId === id) onRowSelect(null, null)
    },
    onError: () => toast('삭제에 실패했습니다', 'error'),
  })

  const handleDeleteSelected = async () => {
    const selectedIds = Object.keys(rowSelection)
      .filter(rowIdx => rowSelection[rowIdx])
      .map(rowIdx => images[Number(rowIdx)]?.id)
      .filter(Boolean) as string[]

    if (selectedIds.length === 0) return

    for (const id of selectedIds) {
      await deleteMutation.mutateAsync(id).catch(() => {})
    }

    toast(`${selectedIds.length}개 이미지가 삭제되었습니다`, 'info')
    setRowSelection({})
    queryClient.invalidateQueries({ queryKey: queryKeys.images() })
  }

  const setDateFilter = (range: Partial<DateRangeFilter>) => {
    setPage(1)
    setColumnFilters(prev => {
      const current = (prev.find(f => f.id === 'capturedAt')?.value ?? {}) as DateRangeFilter
      const next = { ...current, ...range }
      const without = prev.filter(f => f.id !== 'capturedAt')
      return (!next.from && !next.to) ? without : [...without, { id: 'capturedAt', value: next }]
    })
  }

  const clearDateFilter = () => {
    setPage(1)
    setColumnFilters(prev => prev.filter(f => f.id !== 'capturedAt'))
  }

  const columns = [
    columnHelper.display({
      id: 'select',
      size: 36,
      header: ({ table }) => (
        <input
          type="checkbox"
          className={cx('checkbox')}
          checked={table.getIsAllRowsSelected()}
          ref={el => {
            if (el) el.indeterminate = table.getIsSomeRowsSelected()
          }}
          onChange={table.getToggleAllRowsSelectedHandler()}
          onClick={e => e.stopPropagation()}
          title="Select all"
        />
      ),
      cell: ({ row }) => (
        <input
          type="checkbox"
          className={cx('checkbox')}
          checked={row.getIsSelected()}
          onChange={row.getToggleSelectedHandler()}
          onClick={e => e.stopPropagation()}
        />
      ),
    }),
    columnHelper.accessor('filename', {
      header: 'Filename',
      cell: ({ getValue, row }) => {
        const filePath = row.original.filePath
        const lastSep = Math.max(filePath.lastIndexOf('/'), filePath.lastIndexOf('\\'))
        const dir = lastSep > 0 ? filePath.substring(0, lastSep) : ''
        return (
          <div title={filePath}>
            <div>{getValue()}</div>
            {dir && <div className={cx('filedir')}>{dir}</div>}
          </div>
        )
      },
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
  ]

  const table = useReactTable({
    data: images,
    columns,
    state: { sorting, columnFilters, rowSelection },
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    onRowSelectionChange: setRowSelection,
    manualFiltering: true,
    enableRowSelection: true,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  })

  const selectedCount = Object.values(rowSelection).filter(Boolean).length

  const handleToggleFilter = () => {
    if (filterOpen && isFiltered) clearDateFilter()
    setFilterOpen(prev => !prev)
  }

  const refresh = () => queryClient.invalidateQueries({ queryKey: queryKeys.images() })

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
            onClick={refresh}
            disabled={isLoading}
            title="Refresh"
          >
            <RefreshCw size={14} />
          </button>
        </div>

        <div className={cx('pagination')}>
          <button
            type="button"
            className={cx('pageBtn')}
            onClick={() => setPage(p => Math.max(1, p - 1))}
            disabled={page <= 1 || isLoading}
            title="Previous page"
          >
            <ChevronLeft size={13} />
          </button>
          <span className={cx('pageInfo')}>
            {totalCount > 0 ? `${page} / ${totalPages}` : '—'}
          </span>
          <button
            type="button"
            className={cx('pageBtn')}
            onClick={() => setPage(p => Math.min(totalPages, p + 1))}
            disabled={page >= totalPages || isLoading}
            title="Next page"
          >
            <ChevronRight size={13} />
          </button>
        </div>

        <div className={cx('toolbarRight')}>
          {selectedCount > 0 && (
            <button
              type="button"
              className={cx('deleteSelectedBtn')}
              onClick={handleDeleteSelected}
              disabled={deleteMutation.isPending}
            >
              <Trash2 size={13} /> Delete {selectedCount} selected
            </button>
          )}
        </div>
      </div>

      {/* Date range filter */}
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
                  className={cx('row', {
                    selected: row.original.id === selectedId,
                    checked: row.getIsSelected(),
                  })}
                  onClick={() => onRowSelect(row.original.id, row.original)}
                >
                  {row.getVisibleCells().map((cell) => (
                    <td
                      key={cell.id}
                      className={cx({
                        tdSelect: cell.column.id === 'select',
                        tdFilename: cell.column.id === 'filename',
                        tdMeta: !['select', 'filename'].includes(cell.column.id),
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
      {totalCount > 0 && (
        <div className={cx('actions')}>
          <span className={cx('count')}>
            {selectedCount > 0
              ? `${selectedCount} selected`
              : `${images.length} / ${totalCount} images`}
          </span>
        </div>
      )}
    </div>
  )
}
