import { useCallback, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import IconButton from "@mui/material/IconButton";
import RefreshIcon from "@mui/icons-material/Refresh";
import { AreaChart, Area, XAxis, YAxis, ResponsiveContainer, Tooltip } from "recharts";
import type { HistogramData } from "../api/types";
import { getHistogram } from "../api/client";

interface ChartPoint {
  bin: number;
  red: number;
  green: number;
  blue: number;
}

function toChartData(data: HistogramData): ChartPoint[] {
  // Downsample to 64 bins for cleaner rendering
  const factor = 4;
  const result: ChartPoint[] = [];
  for (let i = 0; i < 256; i += factor) {
    let r = 0, g = 0, b = 0;
    for (let j = 0; j < factor && i + j < 256; j++) {
      r += data.red[i + j];
      g += data.green[i + j];
      b += data.blue[i + j];
    }
    result.push({ bin: i, red: r, green: g, blue: b });
  }
  return result;
}

export default function HistogramChart() {
  const [data, setData] = useState<ChartPoint[] | null>(null);

  const refresh = useCallback(async () => {
    try {
      const hist = await getHistogram();
      if (hist) setData(toChartData(hist));
    } catch {
      /* ignore */
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  return (
    <Box sx={{ mt: 1 }}>
      <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 0.5 }}>
        <Typography variant="subtitle2" color="text.secondary">
          Histogram
        </Typography>
        <IconButton size="small" onClick={refresh}>
          <RefreshIcon fontSize="small" />
        </IconButton>
      </Box>

      {data ? (
        <ResponsiveContainer width="100%" height={120}>
          <AreaChart data={data} margin={{ top: 0, right: 0, left: 0, bottom: 0 }}>
            <XAxis dataKey="bin" hide />
            <YAxis hide />
            <Tooltip
              labelFormatter={(v) => `Bin ${v}-${Number(v) + 3}`}
              contentStyle={{ fontSize: 11, padding: "4px 8px" }}
            />
            <Area
              type="monotone"
              dataKey="red"
              stroke="#f44336"
              fill="#f44336"
              fillOpacity={0.3}
              strokeWidth={1}
              dot={false}
              isAnimationActive={false}
            />
            <Area
              type="monotone"
              dataKey="green"
              stroke="#4caf50"
              fill="#4caf50"
              fillOpacity={0.3}
              strokeWidth={1}
              dot={false}
              isAnimationActive={false}
            />
            <Area
              type="monotone"
              dataKey="blue"
              stroke="#2196f3"
              fill="#2196f3"
              fillOpacity={0.3}
              strokeWidth={1}
              dot={false}
              isAnimationActive={false}
            />
          </AreaChart>
        </ResponsiveContainer>
      ) : (
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            height: 80,
            border: "1px dashed",
            borderColor: "divider",
            borderRadius: 1,
          }}
        >
          <Typography variant="caption" color="text.secondary">
            No frame data available
          </Typography>
        </Box>
      )}
    </Box>
  );
}
