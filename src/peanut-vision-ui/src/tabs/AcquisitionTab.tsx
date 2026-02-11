import { useCallback, useEffect, useRef, useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import FormControl from "@mui/material/FormControl";
import IconButton from "@mui/material/IconButton";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import Alert from "@mui/material/Alert";
import Grid from "@mui/material/Grid";
import PlayArrowIcon from "@mui/icons-material/PlayArrow";
import StopIcon from "@mui/icons-material/Stop";
import AdjustIcon from "@mui/icons-material/Adjust";
import CameraAltIcon from "@mui/icons-material/CameraAlt";
import RefreshIcon from "@mui/icons-material/Refresh";
import StatusChip from "../components/StatusChip";
import ImageViewer from "../components/ImageViewer";
import type { AcquisitionStatus, CameraProfile } from "../api/types";
import {
  getCameras,
  startAcquisition,
  stopAcquisition,
  getAcquisitionStatus,
  sendTrigger,
  captureFrame,
} from "../api/client";

export default function AcquisitionTab() {
  const [cameras, setCameras] = useState<CameraProfile[]>([]);
  const [selectedProfile, setSelectedProfile] = useState("");
  const [status, setStatus] = useState<AcquisitionStatus | null>(null);
  const [capturedBlob, setCapturedBlob] = useState<Blob | null>(null);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  const [refreshThrottled, setRefreshThrottled] = useState(false);
  const pollRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const fetchStatus = useCallback(() => {
    getAcquisitionStatus().then(setStatus).catch(() => {});
  }, []);

  // Load camera profiles
  useEffect(() => {
    getCameras()
      .then((c) => {
        setCameras(c);
        if (c.length > 0) setSelectedProfile(c[0].id);
      })
      .catch(() => {});
  }, []);

  // Poll status every 3s
  useEffect(() => {
    fetchStatus();
    pollRef.current = setInterval(fetchStatus, 3000);
    return () => {
      if (pollRef.current) clearInterval(pollRef.current);
    };
  }, [fetchStatus]);

  const handleRefresh = () => {
    if (refreshThrottled) return;
    fetchStatus();
    // Reset polling interval so next auto-poll is a full 3s from now
    if (pollRef.current) clearInterval(pollRef.current);
    pollRef.current = setInterval(fetchStatus, 3000);
    setRefreshThrottled(true);
    setTimeout(() => setRefreshThrottled(false), 3000);
  };

  const wrap = async (fn: () => Promise<void>) => {
    setBusy(true);
    setError("");
    try {
      await fn();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setBusy(false);
    }
  };

  const handleStart = () =>
    wrap(async () => {
      await startAcquisition(selectedProfile);
    });

  const handleStop = () =>
    wrap(async () => {
      await stopAcquisition();
    });

  const handleTrigger = () =>
    wrap(async () => {
      await sendTrigger();
    });

  const handleCapture = () =>
    wrap(async () => {
      const blob = await captureFrame();
      setCapturedBlob(blob);
    });

  const stats = status?.statistics;

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {error && (
        <Alert severity="error" onClose={() => setError("")}>
          {error}
        </Alert>
      )}

      {/* Controls row */}
      <Box sx={{ display: "flex", gap: 2, alignItems: "flex-end", flexWrap: "wrap" }}>
        <FormControl size="small" sx={{ minWidth: 280 }}>
          <InputLabel>Camera Profile</InputLabel>
          <Select
            value={selectedProfile}
            label="Camera Profile"
            onChange={(e) => setSelectedProfile(e.target.value)}
          >
            {cameras.map((c) => (
              <MenuItem key={c.id} value={c.id}>
                {c.displayName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <ButtonGroup variant="contained" disabled={busy}>
          <Button
            color="success"
            startIcon={<PlayArrowIcon />}
            onClick={handleStart}
            disabled={busy || !selectedProfile}
          >
            Start
          </Button>
          <Button
            color="error"
            startIcon={<StopIcon />}
            onClick={handleStop}
          >
            Stop
          </Button>
          <Button
            startIcon={<AdjustIcon />}
            onClick={handleTrigger}
          >
            Trigger
          </Button>
          <Button
            color="info"
            startIcon={<CameraAltIcon />}
            onClick={handleCapture}
          >
            Capture
          </Button>
        </ButtonGroup>

        {status && (
          <StatusChip
            active={status.isActive}
            label={status.isActive ? `Active (${status.profileId ?? ""})` : "Inactive"}
          />
        )}

        <Tooltip title={refreshThrottled ? "Wait..." : "Refresh status"}>
          <span>
            <IconButton
              size="small"
              onClick={handleRefresh}
              disabled={refreshThrottled}
            >
              <RefreshIcon />
            </IconButton>
          </span>
        </Tooltip>
      </Box>

      {/* Stats + Image */}
      <Grid container spacing={3}>
        {/* Statistics */}
        <Grid size={{ xs: 12, md: 4 }}>
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
        </Grid>

        {/* Image viewer */}
        <Grid size={{ xs: 12, md: 8 }}>
          <ImageViewer blob={capturedBlob} />
        </Grid>
      </Grid>
    </Box>
  );
}

function StatLine({ label, value }: { label: string; value: string | number }) {
  return (
    <Box sx={{ display: "flex", justifyContent: "space-between" }}>
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="body2" fontFamily="monospace">
        {value}
      </Typography>
    </Box>
  );
}
