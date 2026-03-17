import { useCallback, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Grid";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import ErrorAlert from "../components/ErrorAlert";
import AcquisitionControls from "../components/AcquisitionControls";
import AcquisitionStats from "../components/AcquisitionStats";
import EventLog from "../components/EventLog";
import ImageViewer from "../components/ImageViewer";
import ContinuousSettings from "../components/ContinuousSettings";
import type { AcquisitionMode, AcquisitionStatus, CamFileInfo } from "../api/types";
import {
  getCameras,
  startAcquisition,
  stopAcquisition,
  getAcquisitionStatus,
  triggerAndCapture,
  snapshot,
  getLatestFrame,
} from "../api/client";
import { useAsyncOperation } from "../hooks/useAsyncOperation";
import { usePolling } from "../hooks/usePolling";
import {
  POLL_INTERVAL_ACTIVE_MS,
  POLL_INTERVAL_IDLE_MS,
} from "../constants";

export default function AcquisitionTab() {
  const [cameras, setCameras] = useState<CamFileInfo[]>([]);
  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [frameCount, setFrameCount] = useState<number | null>(null);
  const [intervalMs, setIntervalMs] = useState<number | null>(null);
  const [status, setStatus] = useState<AcquisitionStatus | null>(null);
  const [capturedBlob, setCapturedBlob] = useState<Blob | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "info" | "warning" | "error";
  } | null>(null);
  const { busy, error, clearError, execute } = useAsyncOperation();

  const hasWarnings = (status?.statistics?.droppedFrameCount ?? 0) > 0
    || (status?.statistics?.clusterUnavailableCount ?? 0) > 0;
  const hasErrors = !!status?.lastError || (status?.statistics?.errorCount ?? 0) > 0;

  const fetchStatus = useCallback(() => {
    getAcquisitionStatus().then(setStatus).catch(() => {});
  }, []);

  const pollInterval = status?.isActive
    ? POLL_INTERVAL_ACTIVE_MS
    : POLL_INTERVAL_IDLE_MS;
  const { refresh, throttled } = usePolling(fetchStatus, pollInterval);

  useEffect(() => {
    getCameras()
      .then((c) => {
        setCameras(c);
        if (c.length > 0) setSelectedProfile(c[0].fileName);
      })
      .catch(() => {});
  }, []);

  // Live preview: poll latest frame while acquisition is active and has frames
  useEffect(() => {
    if (!status?.isActive || !status?.hasFrame) return;
    const t = setInterval(async () => {
      try {
        const blob = await getLatestFrame();
        if (blob) setCapturedBlob(blob);
      } catch {
        /* ignore */
      }
    }, 1000);
    return () => clearInterval(t);
  }, [status?.isActive, status?.hasFrame]);

  const handleStart = () =>
    execute(async () => {
      await startAcquisition(selectedProfile, undefined, frameCount, intervalMs);
      fetchStatus();
      setSnackbar({ message: "촬영이 시작되었습니다", severity: "success" });
    });

  const handleStop = () =>
    execute(async () => {
      await stopAcquisition();
      fetchStatus();
      setSnackbar({ message: "촬영이 중지되었습니다", severity: "info" });
    });

  const handleTrigger = () =>
    execute(async () => {
      setCapturedBlob(await triggerAndCapture());
      fetchStatus();
      setSnackbar({
        message: "프레임이 촬영되었습니다",
        severity: "success",
      });
    });

  const handleCapture = () =>
    execute(async () => {
      setCapturedBlob(await snapshot(selectedProfile));
      fetchStatus();
      setSnackbar({
        message: "스냅샷이 촬영되었습니다",
        severity: "success",
      });
    });

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <ErrorAlert error={error} onClose={clearError} />

      <AcquisitionControls
        cameras={cameras}
        selectedProfile={selectedProfile}
        onProfileChange={setSelectedProfile}
        mode={mode}
        onModeChange={setMode}
        status={status}
        busy={busy}
        onCapture={handleCapture}
        onStart={handleStart}
        onStop={handleStop}
        onTrigger={handleTrigger}
        onRefresh={refresh}
        refreshThrottled={throttled}
        hasWarnings={hasWarnings}
        hasErrors={hasErrors}
      />

      {mode === "continuous" && (
        <ContinuousSettings
          frameCount={frameCount}
          onFrameCountChange={setFrameCount}
          intervalMs={intervalMs}
          onIntervalMsChange={setIntervalMs}
          disabled={status?.isActive}
        />
      )}

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <AcquisitionStats stats={status?.statistics} />
            <EventLog events={status?.recentEvents} />
          </Box>
        </Grid>
        <Grid size={{ xs: 12, md: 8 }}>
          <ImageViewer blob={capturedBlob} errorMessage={status?.lastError} />
        </Grid>
      </Grid>

      <Snackbar
        open={snackbar !== null}
        autoHideDuration={3000}
        onClose={() => setSnackbar(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackbar(null)}
          severity={snackbar?.severity ?? "info"}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackbar?.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
