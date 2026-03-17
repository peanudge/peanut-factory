import Box from "@mui/material/Box";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import Typography from "@mui/material/Typography";
import type { ChannelEvent } from "../api/types";

const typeColor: Record<string, "info" | "warning" | "error" | "default"> = {
  AcquisitionStarted: "info",
  AcquisitionStopped: "info",
  FrameDropped: "warning",
  BufferUnavailable: "warning",
  AcquisitionError: "error",
};

function formatTime(timestamp: string): string {
  const d = new Date(timestamp);
  return d.toLocaleTimeString("en-GB", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    fractionalSecondDigits: 3,
  });
}

interface Props {
  events: ChannelEvent[] | undefined;
}

export default function EventLog({ events }: Props) {
  return (
    <Card variant="outlined">
      <CardContent>
        <Typography variant="subtitle2" gutterBottom>
          Event Log
        </Typography>
        {events && events.length > 0 ? (
          <Box sx={{ maxHeight: 300, overflowY: "auto" }}>
            {events.map((evt, i) => (
              <Box
                key={i}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 1,
                  py: 0.5,
                  borderBottom: "1px solid",
                  borderColor: "divider",
                  "&:last-child": { borderBottom: "none" },
                }}
              >
                <Typography
                  variant="caption"
                  fontFamily="monospace"
                  sx={{ flexShrink: 0 }}
                >
                  {formatTime(evt.timestamp)}
                </Typography>
                <Chip
                  label={evt.type}
                  size="small"
                  color={typeColor[evt.type] ?? "default"}
                  sx={{ fontSize: "0.7rem", height: 20 }}
                />
                <Typography variant="caption" noWrap sx={{ minWidth: 0 }}>
                  {evt.message}
                </Typography>
              </Box>
            ))}
          </Box>
        ) : (
          <Typography variant="body2" color="text.secondary">
            No events recorded
          </Typography>
        )}
      </CardContent>
    </Card>
  );
}
