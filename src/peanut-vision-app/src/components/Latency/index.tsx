import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ReferenceLine,
  ResponsiveContainer,
} from 'recharts'
import { RefreshCw, Trash2 } from 'lucide-react'
import { clearLatencyRecords, getLatencyRecords, getLatencyStats } from '@/api/client'
import type { LatencyRecord, LatencyStats } from '@/api/types'
import cx from './cx'

// ── StatCard ──────────────────────────────────────────────────────────────────

function StatCard({
  label,
  value,
  unit = 'ms',
  color,
}: {
  label: string
  value: number | undefined
  unit?: string
  color?: string
}) {
  return (
    <div className={cx('card')}>
      <span className={cx('cardLabel')}>{label}</span>
      <div className={cx('cardValue')} style={color ? { color } : undefined}>
        {value !== undefined ? value.toFixed(2) : '—'}
        <span>{unit}</span>
      </div>
    </div>
  )
}

// ── LatencyChart ──────────────────────────────────────────────────────────────

function LatencyChart({
  records,
  stats,
}: {
  records: LatencyRecord[]
  stats: LatencyStats | null
}) {
  const data = records.map((r) => ({ id: r.id, latency: r.latencyMs }))

  return (
    <ResponsiveContainer width="100%" height={220}>
      <LineChart data={data} margin={{ top: 8, right: 16, left: 0, bottom: 0 }}>
        <CartesianGrid strokeDasharray="3 3" stroke="#333" />
        <XAxis
          dataKey="id"
          tick={{ fontSize: 11 }}
          label={{ value: 'Measurement #', position: 'insideBottom', offset: -2, fontSize: 11 }}
        />
        <YAxis tick={{ fontSize: 11 }} unit="ms" width={56} />
        <Tooltip
          formatter={(v) => [`${Number(v).toFixed(3)} ms`, 'Latency']}
          contentStyle={{ background: '#1e1e2e', border: '1px solid #444', fontSize: 12 }}
        />
        {stats && (
          <ReferenceLine
            y={stats.meanMs}
            stroke="#90caf9"
            strokeDasharray="4 4"
            label={{
              value: `avg ${stats.meanMs.toFixed(1)}ms`,
              fill: '#90caf9',
              fontSize: 11,
              position: 'insideTopRight',
            }}
          />
        )}
        {stats && (
          <ReferenceLine
            y={stats.p95Ms}
            stroke="#ffb74d"
            strokeDasharray="4 4"
            label={{
              value: `p95 ${stats.p95Ms.toFixed(1)}ms`,
              fill: '#ffb74d',
              fontSize: 11,
              position: 'insideTopRight',
            }}
          />
        )}
        <Line
          type="monotone"
          dataKey="latency"
          dot={false}
          strokeWidth={1.5}
          stroke="#82ca9d"
          isAnimationActive={false}
        />
      </LineChart>
    </ResponsiveContainer>
  )
}

// ── LatencyChip ───────────────────────────────────────────────────────────────

function LatencyChip({ value }: { value: number }) {
  const color = value < 50 ? 'success' : value < 150 ? 'warning' : 'error'
  return (
    <span className={cx('chip', color)}>{value.toFixed(2)} ms</span>
  )
}

// ── RecordsTable ──────────────────────────────────────────────────────────────

function RecordsTable({ records }: { records: LatencyRecord[] }) {
  const visible = [...records].reverse().slice(0, 100)
  return (
    <div className={cx('tableWrap')}>
      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Frame</th>
            <th>Trigger Sent At</th>
            <th>Frame Received At</th>
            <th style={{ textAlign: 'right' }}>Latency (ms)</th>
            <th>Profile</th>
          </tr>
        </thead>
        <tbody>
          {visible.length === 0 ? (
            <tr className={cx('emptyRow')}>
              <td colSpan={6}>
                No records yet. Start acquisition and fire triggers to collect data.
              </td>
            </tr>
          ) : (
            visible.map((r) => (
              <tr key={r.id}>
                <td>{r.id}</td>
                <td>{r.frameIndex}</td>
                <td className={cx('mono')}>
                  {new Date(r.triggerSentAt).toISOString().slice(11, 23)}
                </td>
                <td className={cx('mono')}>
                  {new Date(r.frameReceivedAt).toISOString().slice(11, 23)}
                </td>
                <td className={cx('right')}>
                  <LatencyChip value={r.latencyMs} />
                </td>
                <td style={{ color: '#666', fontSize: '0.75rem' }}>
                  {r.profileId ?? '—'}
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  )
}

// ── Main ──────────────────────────────────────────────────────────────────────

export default function Latency() {
  const qc = useQueryClient()
  const [autoRefresh] = useState(true)

  const { data: records = [], isFetching: fetchingRecords } = useQuery({
    queryKey: ['latency', 'records'],
    queryFn: () => getLatencyRecords(500),
    refetchInterval: autoRefresh ? 2000 : false,
  })

  const { data: stats } = useQuery<LatencyStats | null>({
    queryKey: ['latency', 'stats'],
    queryFn: getLatencyStats,
    refetchInterval: autoRefresh ? 2000 : false,
  })

  const clearMutation = useMutation({
    mutationFn: clearLatencyRecords,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['latency'] })
    },
  })

  const refresh = () => qc.invalidateQueries({ queryKey: ['latency'] })

  const statItems = [
    { label: 'Count', value: stats?.count, unit: '' },
    { label: 'Min', value: stats?.minMs },
    { label: 'Max', value: stats?.maxMs, color: '#f44336' },
    { label: 'Mean', value: stats?.meanMs, color: '#58a' },
    { label: 'P50', value: stats?.p50Ms },
    { label: 'P95', value: stats?.p95Ms, color: '#ff9800' },
    { label: 'P99', value: stats?.p99Ms, color: '#f44336' },
    { label: 'Std Dev', value: stats?.stdDevMs },
  ]

  return (
    <div className={cx('Latency')}>
      {/* Header */}
      <div className={cx('header')}>
        <h2 className={cx('title')}>Trigger → Frame Latency Analysis</h2>
        <span className={cx('countChip')}>{records.length} records</span>
        {fetchingRecords && <span className={cx('spinner')} />}
        <div className={cx('spacer')} />
        <button type="button" className={cx('btn')} onClick={refresh}>
          <RefreshCw size={13} /> Refresh
        </button>
        <button
          type="button"
          className={cx('btn', 'danger')}
          onClick={() => clearMutation.mutate()}
          disabled={records.length === 0 || clearMutation.isPending}
        >
          <Trash2 size={13} /> Clear
        </button>
      </div>

      {/* Stats */}
      <div className={cx('statsGrid')}>
        {statItems.map(({ label, value, unit, color }) => (
          <StatCard
            key={label}
            label={label}
            value={value}
            unit={unit ?? 'ms'}
            color={color}
          />
        ))}
      </div>

      {/* Chart */}
      <div className={cx('paper')}>
        <p className={cx('sectionLabel')}>Latency over time</p>
        {records.length > 0 ? (
          <LatencyChart records={records} stats={stats ?? null} />
        ) : (
          <div className={cx('noData')}>
            No data — fire triggers to start collecting measurements
          </div>
        )}
      </div>

      {/* Table */}
      <div>
        <p className={cx('sectionLabel')}>Recent records (most recent first, up to 100 shown)</p>
        <RecordsTable records={records} />
      </div>
    </div>
  )
}
