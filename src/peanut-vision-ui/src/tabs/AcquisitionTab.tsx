import { useCallback, useEffect, useRef, useState } from "react";
import Box from "@mui/material/Box";
import Collapse from "@mui/material/Collapse";
import Chip from "@mui/material/Chip";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import Divider from "@mui/material/Divider";
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
import PresetSelector from "../components/PresetSelector";
import CalibrationActions from "../components/CalibrationActions";
import ExposureControl from "../components/ExposureControl";
import type { AcquisitionMode, AcquisitionPreset, AcquisitionStatus, CamFileInfo, CapturedImage, ContinuousSubMode, ExposureInfo, TriggerModeOption } from "../api/types";
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

function SidebarSection({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
      <Typography variant="overline" color="text.secondary" sx={{ lineHeight: 1 }}>
        {label}
      </Typography>
      {children}
      <Divider />
    </Box>
  );
}

function CollapsiblePanel({
  label,
  count,
  defaultOpen = true,
  children,
}: {
  label: string;
  count?: number;
  defaultOpen?: boolean;
  children: React.ReactNode;
}) {
  const [open, setOpen] = useState(defaultOpen);
  return (
    <Box sx={{ borderTop: "1px solid", borderColor: "divider" }}>
      <Box
        onClick={() => setOpen((o) => !o)}
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          px: 2,
          py: 0.75,
          cursor: "pointer",
          bgcolor: "background.default",
          "&:hover": { bgcolor: "action.hover" },
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <Typography variant="subtitle2">{label}</Typography>
          {count != null && <Chip label={count} size="small" />}
        </Box>
        <ExpandMoreIcon
          sx={{
            fontSize: 20,
            transition: "transform 0.2s",
            transform: open ? "rotate(180deg)" : "rotate(0deg)",
          }}
        />
      </Box>
      <Collapse in={open}>
        <Box sx={{ p: 2 }}>
          {children}
        </Box>
      </Collapse>
    </Box>
  );
}

interface Props {
  onSessionChange?: (name: string | null) => void;
}

const RIGHT_PANEL_MIN = 200;
const RIGHT_PANEL_MAX = 560;
const RIGHT_PANEL_DEFAULT = 280;

export default function AcquisitionTab({ onSessionChange }: Props = {}) {
  const [cameras, setCameras] = useState<CamFileInfo[]>([]);

  // Right panel resize — width stored in a ref and applied via DOM ref
  // to avoid re-renders (and layout shifts) during dragging
  const rightPanelRef = useRef<HTMLDivElement>(null);
  const rightPanelWidth = useRef(RIGHT_PANEL_DEFAULT);
  const isDragging = useRef(false);
  const dragStartX = useRef(0);
  const dragStartWidth = useRef(0);

  const onResizerMouseDown = useCallback((e: React.MouseEvent) => {
    e.preventDefault();
    isDragging.current = true;
    dragStartX.current = e.clientX;
    dragStartWidth.current = rightPanelWidth.current;
    document.body.style.cursor = "col-resize";
    document.body.style.userSelect = "none";

    const onMouseMove = (ev: MouseEvent) => {
      if (!isDragging.current) return;
      const delta = dragStartX.current - ev.clientX;
      const next = Math.min(RIGHT_PANEL_MAX, Math.max(RIGHT_PANEL_MIN, dragStartWidth.current + delta));
      rightPanelWidth.current = next;
      if (rightPanelRef.current) rightPanelRef.current.style.width = `${next}px`;
    };

    const onMouseUp = () => {
      isDragging.current = false;
      document.body.style.cursor = "";
      document.body.style.userSelect = "";
      document.removeEventListener("mousemove", onMouseMove);
      document.removeEventListener("mouseup", onMouseUp);
    };

    document.addEventListener("mousemove", onMouseMove);
    document.addEventListener("mouseup", onMouseUp);
  }, []);
  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>("auto");
  const [triggerMode, setTriggerMode] = useState<TriggerModeOption>("soft");
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

  useEffect(() => {
    return () => {
      setImages((prev) => {
        prev.forEach((img) => URL.revokeObjectURL(img.url));
        return prev;
      });
    };
  }, []);

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
        triggerMode,
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
      setSnackbar({ message: "프레임이 촬영되었습니다", severity: "success" });
    });

  const handleCapture = () =>
    execute(async () => {
      const { blob, savedPath } = await snapshot(selectedProfile);
      addImage(blob, savedPath);
      fetchStatus();
      setSnackbar({ message: "스냅샷이 촬영되었습니다", severity: "success" });
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

  const handleLoadPreset = useCallback((preset: AcquisitionPreset) => {
    setSelectedProfile(preset.profileId);
    setTriggerMode((preset.triggerMode as TriggerModeOption) ?? "soft");
    setFrameCount(preset.frameCount ?? null);
    setIntervalMs(preset.intervalMs ?? null);
    if (preset.frameCount != null || preset.intervalMs != null) {
      setMode("continuous");
    }
  }, []);

  const handleFfcToggle = (_: unknown, checked: boolean) => {
    setFfcEnabled(checked);
    execute(async () => {
      setSnackbar({ message: (await setFfc(checked)).message, severity: "success" });
    });
  };

  const selectedImage = images.find((img) => img.id === selectedId) ?? null;

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      <ErrorAlert error={error} onClose={clearError} />

      {/* LEFT SIDEBAR */}
      <Box
        sx={{
          width: 340,
          flexShrink: 0,
          borderRight: "1px solid",
          borderColor: "divider",
          overflowY: "auto",
          p: 3,
          display: "flex",
          flexDirection: "column",
          gap: 2,
        }}
      >
        <SidebarSection label="Acquisition">
          <PresetSelector
            profileId={selectedProfile}
            triggerMode={triggerMode}
            frameCount={frameCount}
            intervalMs={intervalMs}
            onLoadPreset={handleLoadPreset}
            disabled={status?.isActive}
          />
          <AcquisitionControls
            cameras={cameras}
            selectedProfile={selectedProfile}
            onProfileChange={setSelectedProfile}
            mode={mode}
            onModeChange={setMode}
            continuousSubMode={continuousSubMode}
            triggerMode={triggerMode}
            onTriggerModeChange={setTriggerMode}
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
        </SidebarSection>

        <SidebarSection label="Exposure">
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
        </SidebarSection>

        <SidebarSection label="Calibration">
          <CalibrationActions
            busy={busy}
            ffcEnabled={ffcEnabled}
            onBlack={handleBlack}
            onWhite={handleWhite}
            onWhiteBalance={handleWhiteBalance}
            onFfcToggle={handleFfcToggle}
          />
        </SidebarSection>

        <Accordion disableGutters elevation={0} sx={{ border: "1px solid", borderColor: "divider" }}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Typography variant="subtitle2">Save Settings</Typography>
          </AccordionSummary>
          <AccordionDetails sx={{ p: 1 }}>
            <ImageSaveSettingsPanel />
          </AccordionDetails>
        </Accordion>

        <SidebarSection label="Session">
          <SessionSelector onSessionChange={onSessionChange} />
        </SidebarSection>
      </Box>

      {/* MAIN CANVAS */}
      <Box sx={{ flexGrow: 1, display: "flex", flexDirection: "column", overflow: "hidden" }}>
        {/* Image viewer + stats/histogram row */}
        <Box sx={{ flexGrow: 1, p: 2, display: "flex", flexDirection: "column", gap: 2, overflow: "hidden" }}>
          <Box sx={{ flexGrow: 1, minHeight: 0 }}>
            <ImageViewer
              url={selectedImage?.url ?? null}
              filename={selectedImage ? `capture-${formatFilenameTimestamp(selectedImage.capturedAt)}.png` : undefined}
              errorMessage={status?.lastError}
              savedPath={selectedImage?.savedPath}
            />
          </Box>
          <Box sx={{ display: "flex", gap: 2, flexShrink: 0 }}>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <HistogramChart />
            </Box>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <AcquisitionStats stats={status?.statistics} />
            </Box>
          </Box>
        </Box>
      </Box>

      {/* DRAG HANDLE */}
      <Box
        onMouseDown={onResizerMouseDown}
        sx={{
          width: 4,
          flexShrink: 0,
          cursor: "col-resize",
          bgcolor: "divider",
          transition: "background-color 0.15s",
          "&:hover": { bgcolor: "primary.main" },
        }}
      />

      {/* RIGHT PANEL */}
      <Box
        ref={rightPanelRef}
        sx={{
          width: RIGHT_PANEL_DEFAULT,
          flexShrink: 0,
          display: "flex",
          flexDirection: "column",
          overflow: "hidden",
        }}
      >
        <CollapsiblePanel label="Captures" count={images.length} defaultOpen>
          <CapturedImageList
            images={images}
            selectedId={selectedId}
            onSelect={setSelectedId}
            onDelete={handleDeleteImage}
            onClear={handleClearAll}
          />
        </CollapsiblePanel>

        <CollapsiblePanel label="Event Log" defaultOpen={false}>
          <EventLog events={status?.recentEvents} />
        </CollapsiblePanel>
      </Box>

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
