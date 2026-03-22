import { useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import CircularProgress from "@mui/material/CircularProgress";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Typography from "@mui/material/Typography";
import DeleteIcon from "@mui/icons-material/Delete";
import RefreshIcon from "@mui/icons-material/Refresh";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ReferenceLine,
  ResponsiveContainer,
} from "recharts";
import { clearLatencyRecords, getLatencyRecords, getLatencyStats } from "../api/client";
import type { LatencyRecord, LatencyStats } from "../api/types";

// ── Stat card ──────────────────────────────────────────────────────────────

function StatCard({ label, value, unit = "ms", color }: {
  label: string;
  value: number | undefined;
  unit?: string;
  color?: string;
}) {
  return (
    <Card variant="outlined" sx={{ minWidth: 120 }}>
      <CardContent sx={{ py: 1.5, px: 2, "&:last-child": { pb: 1.5 } }}>
        <Typography variant="caption" color="text.secondary" display="block">
          {label}
        </Typography>
        <Typography variant="h6" fontWeight={600} color={color ?? "text.primary"}>
          {value !== undefined ? value.toFixed(2) : "—"}
          <Typography component="span" variant="caption" color="text.secondary" ml={0.5}>
            {unit}
          </Typography>
        </Typography>
      </CardContent>
    </Card>
  );
}

// ── Chart ──────────────────────────────────────────────────────────────────

function LatencyChart({ records, stats }: { records: LatencyRecord[]; stats: LatencyStats | null }) {
  const data = records.map((r) => ({
    id: r.id,
    latency: r.latencyMs,
  }));

  return (
    <ResponsiveContainer width="100%" height={220}>
      <LineChart data={data} margin={{ top: 8, right: 16, left: 0, bottom: 0 }}>
        <CartesianGrid strokeDasharray="3 3" stroke="#333" />
        <XAxis dataKey="id" tick={{ fontSize: 11 }} label={{ value: "Measurement #", position: "insideBottom", offset: -2, fontSize: 11 }} />
        <YAxis tick={{ fontSize: 11 }} unit="ms" width={56} />
        <Tooltip
          formatter={(v) => [`${Number(v).toFixed(3)} ms`, "Latency"]}
          contentStyle={{ background: "#1e1e2e", border: "1px solid #444", fontSize: 12 }}
        />
        {stats && (
          <ReferenceLine y={stats.meanMs} stroke="#90caf9" strokeDasharray="4 4"
            label={{ value: `avg ${stats.meanMs.toFixed(1)}ms`, fill: "#90caf9", fontSize: 11, position: "insideTopRight" }} />
        )}
        {stats && (
          <ReferenceLine y={stats.p95Ms} stroke="#ffb74d" strokeDasharray="4 4"
            label={{ value: `p95 ${stats.p95Ms.toFixed(1)}ms`, fill: "#ffb74d", fontSize: 11, position: "insideTopRight" }} />
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
  );
}

// ── Records table ──────────────────────────────────────────────────────────

function RecordsTable({ records }: { records: LatencyRecord[] }) {
  const visible = [...records].reverse().slice(0, 100);

  return (
    <TableContainer component={Paper} variant="outlined" sx={{ maxHeight: 320 }}>
      <Table size="small" stickyHeader>
        <TableHead>
          <TableRow>
            <TableCell>#</TableCell>
            <TableCell>Frame</TableCell>
            <TableCell>Trigger Sent At</TableCell>
            <TableCell>Frame Received At</TableCell>
            <TableCell align="right">Latency (ms)</TableCell>
            <TableCell>Profile</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {visible.map((r) => (
            <TableRow key={r.id} hover>
              <TableCell>{r.id}</TableCell>
              <TableCell>{r.frameIndex}</TableCell>
              <TableCell sx={{ fontFamily: "monospace", fontSize: 11 }}>
                {new Date(r.triggerSentAt).toISOString().slice(11, 23)}
              </TableCell>
              <TableCell sx={{ fontFamily: "monospace", fontSize: 11 }}>
                {new Date(r.frameReceivedAt).toISOString().slice(11, 23)}
              </TableCell>
              <TableCell align="right">
                <LatencyChip value={r.latencyMs} />
              </TableCell>
              <TableCell>
                <Typography variant="caption" color="text.secondary">
                  {r.profileId ?? "—"}
                </Typography>
              </TableCell>
            </TableRow>
          ))}
          {visible.length === 0 && (
            <TableRow>
              <TableCell colSpan={6} align="center" sx={{ color: "text.secondary", py: 4 }}>
                No records yet. Start acquisition and fire triggers to collect data.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

function LatencyChip({ value }: { value: number }) {
  const color = value < 50 ? "success" : value < 150 ? "warning" : "error";
  return (
    <Chip
      label={`${value.toFixed(2)} ms`}
      size="small"
      color={color}
      variant="outlined"
      sx={{ fontFamily: "monospace", fontSize: 11 }}
    />
  );
}

// ── Main tab ───────────────────────────────────────────────────────────────

export default function LatencyTab() {
  const qc = useQueryClient();
  const [autoRefresh] = useState(true);

  const { data: records = [], isFetching: fetchingRecords } = useQuery({
    queryKey: ["latency", "records"],
    queryFn: () => getLatencyRecords(500),
    refetchInterval: autoRefresh ? 2000 : false,
  });

  const { data: stats } = useQuery<LatencyStats | null>({
    queryKey: ["latency", "stats"],
    queryFn: getLatencyStats,
    refetchInterval: autoRefresh ? 2000 : false,
  });

  const clearMutation = useMutation({
    mutationFn: clearLatencyRecords,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["latency"] });
    },
  });

  const refresh = () => qc.invalidateQueries({ queryKey: ["latency"] });

  return (
    <Box sx={{ p: 3, display: "flex", flexDirection: "column", gap: 3 }}>
      {/* Header */}
      <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
        <Typography variant="h6" fontWeight={600}>
          Trigger → Frame Latency Analysis
        </Typography>
        <Chip
          label={`${records.length} records`}
          size="small"
          color={records.length > 0 ? "primary" : "default"}
          variant="outlined"
        />
        {fetchingRecords && <CircularProgress size={16} />}
        <Box sx={{ flexGrow: 1 }} />
        <Button
          size="small"
          startIcon={<RefreshIcon />}
          onClick={refresh}
          variant="outlined"
        >
          Refresh
        </Button>
        <Button
          size="small"
          startIcon={<DeleteIcon />}
          onClick={() => clearMutation.mutate()}
          color="error"
          variant="outlined"
          disabled={records.length === 0 || clearMutation.isPending}
        >
          Clear
        </Button>
      </Box>

      {/* Stats row */}
      <Grid container spacing={1.5}>
        {[
          { label: "Count", value: stats?.count, unit: "" },
          { label: "Min", value: stats?.minMs },
          { label: "Max", value: stats?.maxMs, color: "error.main" },
          { label: "Mean", value: stats?.meanMs, color: "primary.main" },
          { label: "P50", value: stats?.p50Ms },
          { label: "P95", value: stats?.p95Ms, color: "warning.main" },
          { label: "P99", value: stats?.p99Ms, color: "error.main" },
          { label: "Std Dev", value: stats?.stdDevMs },
        ].map(({ label, value, unit, color }) => (
          <Grid key={label} size={{ xs: 6, sm: 3, md: "auto" }}>
            <StatCard label={label} value={value} unit={unit ?? "ms"} color={color} />
          </Grid>
        ))}
      </Grid>

      {/* Chart */}
      <Paper variant="outlined" sx={{ p: 2 }}>
        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
          Latency over time
        </Typography>
        {records.length > 0 ? (
          <LatencyChart records={records} stats={stats ?? null} />
        ) : (
          <Box sx={{ height: 220, display: "flex", alignItems: "center", justifyContent: "center" }}>
            <Typography color="text.secondary" variant="body2">
              No data — fire triggers to start collecting measurements
            </Typography>
          </Box>
        )}
      </Paper>

      {/* Table */}
      <Box>
        <Typography variant="subtitle2" color="text.secondary" gutterBottom>
          Recent records (most recent first, up to 100 shown)
        </Typography>
        <RecordsTable records={records} />
      </Box>
    </Box>
  );
}
