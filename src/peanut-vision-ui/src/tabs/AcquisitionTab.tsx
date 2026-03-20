import { useCallback, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Grid";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import Typography from "@mui/material/Typography";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ErrorAlert from "../components/ErrorAlert";
import AcquisitionControls from "../components/AcquisitionControls";
import AcquisitionStats from "../components/AcquisitionStats";
import EventLog from "../components/EventLog";
import ImageViewer from "../components/ImageViewer";
import CapturedImageList from "../components/CapturedImageList";
import ContinuousSettings from "../components/ContinuousSettings";
import ImageSaveSettingsPanel from "../components/ImageSaveSettingsPanel";
import SessionSelector from "../components/SessionSelector";
import HistogramChart from "../components/HistogramChart";
import CalibrationActions from "../components/CalibrationActions";
import ExposureControl from "../components/ExposureControl";
import type { AcquisitionMode, AcquisitionStatus, CamFileInfo, CapturedImage, ContinuousSubMode, ExposureInfo } from "../api/types";
import {
  getCameras,
  startAcquisition,
  stopAcquisition,
  getAcquisitionStatus,
  triggerAndCapture,
  snapshot,
  getLatestFrame,
  blackCalibration,
  whiteCalibration,
  whiteBalance,
  setFfc,
  getExposure,
  setExposure,
} from "../api/client";
import { useAsyncOperation } from "../hooks/useAsyncOperation";
import { usePolling } from "../hooks/usePolling";
import {
  POLL_INTERVAL_ACTIVE_MS,
  POLL_INTERVAL_IDLE_MS,
  MAX_CAPTURED_IMAGES,
} from "../constants";

function formatFilenameTimestamp(d: Date): string {
  return `${d.getFullYear()}${String(d.getMonth() + 1).padStart(2, "0")}${String(d.getDate()).padStart(2, "0")}_${String(d.getHours()).padStart(2, "0")}${String(d.getMinutes()).padStart(2, "0")}${String(d.getSeconds()).padStart(2, "0")}_${String(d.getMilliseconds()).padStart(3, "0")}`;
}

export default function AcquisitionTab() {
  const [cameras, setCameras] = useState<CamFileInfo[]>([]);
  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>("auto");
  const [frameCount, setFrameCount] = useState<number | null>(null);
  const [intervalMs, setIntervalMs] = useState<number | null>(null);
  const [status, setStatus] = useState<AcquisitionStatus | null>(null);
  const [images, setImages] = useState<CapturedImage[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "info" | "warning" | "error";
  } | null>(null);
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);
  const [gainValue, setGainValue] = useState(0);
  const [ffcEnabled, setFfcEnabled] = useState(false);
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

  const addImage = useCallback((blob: Blob, savedPath?: string) => {
    const url = URL.createObjectURL(blob);
    const newImage: CapturedImage = { id: crypto.randomUUID(), url, blob, capturedAt: new Date(), savedPath };
    setImages((prev) => {
      const next = [newImage, ...prev];
      if (next.length > MAX_CAPTURED_IMAGES) {
        next.slice(MAX_CAPTURED_IMAGES).forEach((img) => URL.revokeObjectURL(img.url));
        return next.slice(0, MAX_CAPTURED_IMAGES);
      }
      return next;
    });
    setSelectedId(newImage.id);
  }, []);

  const handleDeleteImage = useCallback((id: string) => {
    setImages((prev) => {
      const target = prev.find((img) => img.id === id);
      if (target) URL.revokeObjectURL(target.url);
      return prev.filter((img) => img.id !== id);
    });
    setSelectedId((prev) => (prev === id ? null : prev));
  }, []);

  const handleClearAll = useCallback(() => {
    setImages((prev) => {
      prev.forEach((img) => URL.revokeObjectURL(img.url));
      return [];
    });
    setSelectedId(null);
  }, []);

  // Revoke all URLs on unmount
  useEffect(() => {
    return () => {
      setImages((prev) => {
        prev.forEach((img) => URL.revokeObjectURL(img.url));
        return prev;
      });
    };
  }, []);

  // Live preview: poll latest frame while acquisition is active and has frames
  useEffect(() => {
    if (!status?.isActive || !status?.hasFrame) return;
    const t = setInterval(async () => {
      try {
        const result = await getLatestFrame();
        if (result) addImage(result.blob, result.savedPath);
      } catch {
        /* ignore */
      }
    }, 1000);
    return () => clearInterval(t);
  }, [status?.isActive, status?.hasFrame, addImage]);

  const handleStart = () =>
    execute(async () => {
      await startAcquisition(
        selectedProfile,
        undefined,
        frameCount,
        continuousSubMode === "auto" ? intervalMs : null,
      );
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
      const { blob, savedPath } = await triggerAndCapture();
      addImage(blob, savedPath);
      fetchStatus();
      setSnackbar({
        message: "프레임이 촬영되었습니다",
        severity: "success",
      });
    });

  const handleCapture = () =>
    execute(async () => {
      const { blob, savedPath } = await snapshot(selectedProfile);
      addImage(blob, savedPath);
      fetchStatus();
      setSnackbar({
        message: "스냅샷이 촬영되었습니다",
        severity: "success",
      });
    });

  const handleLoadExposure = () =>
    execute(async () => {
      const info = await getExposure();
      setExposureState(info);
      setExposureValue(info.exposureUs);
      setGainValue(info.gainDb);
      setSnackbar({ message: "Exposure settings loaded", severity: "success" });
    });

  const handleApplyExposure = () =>
    execute(async () => {
      const result = await setExposure(exposureValue, gainValue);
      setSnackbar({ message: result.message, severity: "success" });
    });

  const handleBlack = () =>
    execute(async () => {
      setSnackbar({ message: (await blackCalibration()).message, severity: "success" });
    });

  const handleWhite = () =>
    execute(async () => {
      setSnackbar({ message: (await whiteCalibration()).message, severity: "success" });
    });

  const handleWhiteBalance = () =>
    execute(async () => {
      setSnackbar({ message: (await whiteBalance()).message, severity: "success" });
    });

  const handleFfcToggle = (_: unknown, checked: boolean) => {
    setFfcEnabled(checked);
    execute(async () => {
      setSnackbar({ message: (await setFfc(checked)).message, severity: "success" });
    });
  };

  const selectedImage = images.find((img) => img.id === selectedId) ?? null;

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <ErrorAlert error={error} onClose={clearError} />

      <SessionSelector />

      <AcquisitionControls
        cameras={cameras}
        selectedProfile={selectedProfile}
        onProfileChange={setSelectedProfile}
        mode={mode}
        onModeChange={setMode}
        continuousSubMode={continuousSubMode}
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
          subMode={continuousSubMode}
          onSubModeChange={setContinuousSubMode}
          frameCount={frameCount}
          onFrameCountChange={setFrameCount}
          intervalMs={intervalMs}
          onIntervalMsChange={setIntervalMs}
          disabled={status?.isActive}
        />
      )}

      <ImageSaveSettingsPanel />

      <Accordion disableGutters>
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="subtitle2">Camera Settings</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 6 }}>
              <CalibrationActions
                busy={busy}
                ffcEnabled={ffcEnabled}
                onBlack={handleBlack}
                onWhite={handleWhite}
                onWhiteBalance={handleWhiteBalance}
                onFfcToggle={handleFfcToggle}
              />
            </Grid>
            <Grid size={{ xs: 12, md: 6 }}>
              <ExposureControl
                exposure={exposure}
                exposureValue={exposureValue}
                gainValue={gainValue}
                busy={busy}
                onExposureChange={setExposureValue}
                onGainChange={setGainValue}
                onLoad={handleLoadExposure}
                onApply={handleApplyExposure}
              />
            </Grid>
          </Grid>
        </AccordionDetails>
      </Accordion>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <AcquisitionStats stats={status?.statistics} />
            <EventLog events={status?.recentEvents} />
          </Box>
        </Grid>
        <Grid size={{ xs: 12, md: 8 }}>
          <Box sx={{ display: "flex", flexDirection: "column" }}>
            <ImageViewer
              url={selectedImage?.url ?? null}
              filename={selectedImage ? `capture-${formatFilenameTimestamp(selectedImage.capturedAt)}.png` : undefined}
              errorMessage={status?.lastError}
              savedPath={selectedImage?.savedPath}
            />
            <HistogramChart />
            <CapturedImageList
              images={images}
              selectedId={selectedId}
              onSelect={setSelectedId}
              onDelete={handleDeleteImage}
              onClear={handleClearAll}
            />
          </Box>
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
