import Box from "@mui/material/Box";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import StatLine from "./StatLine";
import type { AcquisitionStatistics } from "../api/types";

interface Props {
  stats: AcquisitionStatistics | undefined;
}

export default function AcquisitionStats({ stats }: Props) {
  return (
    <Card variant="outlined">
      <CardContent>
        <Typography variant="subtitle2" gutterBottom>
          Statistics
        </Typography>
        {stats ? (
          <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
            <StatLine label="FPS" value={stats.averageFps.toFixed(1)} />
            <StatLine label="Frames" value={stats.frameCount} />
            <StatLine label="Dropped" value={stats.droppedFrameCount} />
            <StatLine label="Errors" value={stats.errorCount} />
            <StatLine
              label="Avg Interval"
              value={`${stats.averageFrameIntervalMs.toFixed(1)} ms`}
            />
            <StatLine
              label="Min / Max"
              value={`${stats.minFrameIntervalMs.toFixed(1)} / ${stats.maxFrameIntervalMs.toFixed(1)} ms`}
            />
          </Box>
        ) : (
          <Typography variant="body2" color="text.secondary">
            No acquisition running
          </Typography>
        )}
      </CardContent>
    </Card>
  );
}
