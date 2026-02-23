import { useCallback, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Grid";
import ErrorAlert from "../components/ErrorAlert";
import AcquisitionControls from "../components/AcquisitionControls";
import AcquisitionStats from "../components/AcquisitionStats";
import ImageViewer from "../components/ImageViewer";
import type { AcquisitionStatus, CameraProfile } from "../api/types";
import {
  getCameras,
  startAcquisition,
  stopAcquisition,
  getAcquisitionStatus,
  sendTrigger,
  captureFrame,
  snapshot,
} from "../api/client";
import { useAsyncOperation } from "../hooks/useAsyncOperation";
import { usePolling } from "../hooks/usePolling";

export default function AcquisitionTab() {
  const [cameras, setCameras] = useState<CameraProfile[]>([]);
  const [selectedProfile, setSelectedProfile] = useState("");
  const [status, setStatus] = useState<AcquisitionStatus | null>(null);
  const [capturedBlob, setCapturedBlob] = useState<Blob | null>(null);
  const { busy, error, clearError, execute } = useAsyncOperation();

  const fetchStatus = useCallback(() => {
    getAcquisitionStatus().then(setStatus).catch(() => {});
  }, []);

  const { refresh, throttled } = usePolling(fetchStatus);

  useEffect(() => {
    getCameras()
      .then((c) => {
        setCameras(c);
        if (c.length > 0) setSelectedProfile(c[0].id);
      })
      .catch(() => {});
  }, []);

  const handleStart = () =>
    execute(async () => { await startAcquisition(selectedProfile); });

  const handleStop = () =>
    execute(async () => { await stopAcquisition(); });

  const handleTrigger = () =>
    execute(async () => { await sendTrigger(); });

  const handleCapture = () =>
    execute(async () => { setCapturedBlob(await captureFrame()); });

  const handleSnapshot = () =>
    execute(async () => { setCapturedBlob(await snapshot(selectedProfile)); });

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <ErrorAlert error={error} onClose={clearError} />

      <AcquisitionControls
        cameras={cameras}
        selectedProfile={selectedProfile}
        onProfileChange={setSelectedProfile}
        status={status}
        busy={busy}
        onStart={handleStart}
        onStop={handleStop}
        onTrigger={handleTrigger}
        onCapture={handleCapture}
        onSnapshot={handleSnapshot}
        onRefresh={refresh}
        refreshThrottled={throttled}
      />

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <AcquisitionStats stats={status?.statistics} />
        </Grid>
        <Grid size={{ xs: 12, md: 8 }}>
          <ImageViewer blob={capturedBlob} errorMessage={status?.lastError} />
        </Grid>
      </Grid>
    </Box>
  );
}
