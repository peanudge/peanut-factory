import { useEffect, useRef } from "react";
import Box from "@mui/material/Box";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import PlayCircleOutlineIcon from "@mui/icons-material/PlayCircleOutline";
import StopCircleIcon from "@mui/icons-material/StopCircle";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import type { ChannelEvent } from "../api/types";

const typeColor: Record<string, "success" | "warning" | "error" | "default"> = {
  AcquisitionStarted: "success",
  AcquisitionStopped: "default",
  FrameDropped: "warning",
  BufferUnavailable: "warning",
  AcquisitionError: "error",
};

const chipLabel: Record<string, string> = {
  AcquisitionStarted: "START",
  AcquisitionStopped: "STOP",
  FrameDropped: "DROP",
  BufferUnavailable: "BUF",
  AcquisitionError: "ERROR",
};

const borderAccent: Record<string, string> = {
  AcquisitionStarted: "success.main",
  AcquisitionStopped: "text.disabled",
  FrameDropped: "warning.main",
  BufferUnavailable: "warning.main",
  AcquisitionError: "error.main",
};

const typeIcon: Record<string, React.ReactElement> = {
  AcquisitionStarted: <PlayCircleOutlineIcon sx={{ fontSize: 14 }} />,
  AcquisitionStopped: <StopCircleIcon sx={{ fontSize: 14 }} />,
  FrameDropped: <WarningAmberIcon sx={{ fontSize: 14 }} />,
  BufferUnavailable: <WarningAmberIcon sx={{ fontSize: 14 }} />,
  AcquisitionError: <ErrorOutlineIcon sx={{ fontSize: 14 }} />,
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
  const scrollRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const el = scrollRef.current;
    if (!el) return;
    const threshold = 80;
    const nearBottom = el.scrollHeight - el.scrollTop - el.clientHeight < threshold;
    if (nearBottom) {
      el.scrollTop = el.scrollHeight;
    }
  }, [events]);

  const reversed = events ? [...events].reverse() : [];

  return (
    <Card variant="outlined">
      <CardContent>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 1,
          }}
        >
          <Typography variant="subtitle2">Event Log</Typography>
          {events && events.length > 0 && (
            <Typography variant="caption" color="text.secondary">
              {events.length} events
            </Typography>
          )}
        </Box>

        {reversed.length > 0 ? (
          <Box
            ref={scrollRef}
            sx={{
              maxHeight: { xs: 240, sm: 320, md: "calc(100vh - 500px)" },
              minHeight: 120,
              overflowY: "auto",
            }}
          >
            {reversed.map((evt, i) => (
              <Box
                key={i}
                sx={{
                  pl: 1,
                  pr: 0.5,
                  py: 0.75,
                  borderLeft: "3px solid",
                  borderColor: borderAccent[evt.type] ?? "divider",
                  borderBottom: "1px solid",
                  borderBottomColor: "divider",
                  "&:last-child": { borderBottom: "none" },
                  mb: 0.25,
                }}
              >
                {/* Row 1: chip + timestamp */}
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    mb: 0.25,
                  }}
                >
                  <Chip
                    icon={typeIcon[evt.type]}
                    label={chipLabel[evt.type] ?? evt.type}
                    size="small"
                    color={typeColor[evt.type] ?? "default"}
                    sx={{ fontSize: "0.65rem", height: 20 }}
                  />
                  <Tooltip title={evt.timestamp} placement="left">
                    <Typography
                      variant="caption"
                      fontFamily="monospace"
                      color="text.secondary"
                      sx={{ flexShrink: 0, cursor: "default" }}
                    >
                      {formatTime(evt.timestamp)}
                    </Typography>
                  </Tooltip>
                </Box>

                {/* Row 2: full message, wraps freely */}
                <Typography
                  variant="caption"
                  color="text.primary"
                  sx={{ display: "block", lineHeight: 1.4, wordBreak: "break-word" }}
                >
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
