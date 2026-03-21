import { useCallback, useRef } from "react";
import Box from "@mui/material/Box";
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
import PresetSelector from "../components/PresetSelector";
import CalibrationActions from "../components/CalibrationActions";
import ExposureControl from "../components/ExposureControl";
import SidebarSection from "../components/SidebarSection";
import CollapsiblePanel from "../components/CollapsiblePanel";
import { useImageBuffer } from "../hooks/useImageBuffer";
import { useAcquisitionActions } from "../hooks/useAcquisitionActions";
import { MAX_CAPTURED_IMAGES } from "../constants";

function formatFilenameTimestamp(d: Date): string {
  return `${d.getFullYear()}${String(d.getMonth() + 1).padStart(2, "0")}${String(d.getDate()).padStart(2, "0")}_${String(d.getHours()).padStart(2, "0")}${String(d.getMinutes()).padStart(2, "0")}${String(d.getSeconds()).padStart(2, "0")}_${String(d.getMilliseconds()).padStart(3, "0")}`;
}

interface Props {
  onSessionChange?: (name: string | null) => void;
}

const RIGHT_PANEL_MIN = 200;
const RIGHT_PANEL_MAX = 560;
const RIGHT_PANEL_DEFAULT = 280;

export default function AcquisitionTab({ onSessionChange }: Props = {}) {
  const { capturedFrames, selectedFrameId, addFrame, deleteFrame, clearAllFrames, selectFrame } =
    useImageBuffer(MAX_CAPTURED_IMAGES);

  const acq = useAcquisitionActions({ onFrameCaptured: addFrame });

  // Right panel resize — width stored in a ref and applied via DOM ref
  // to avoid re-renders during dragging
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

  const selectedFrame = capturedFrames.find((f) => f.id === selectedFrameId) ?? null;

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      <ErrorAlert error={acq.error} onClose={acq.clearError} />

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
            profileId={acq.selectedProfile}
            triggerMode={acq.triggerMode}
            frameCount={acq.frameCount}
            intervalMs={acq.intervalMs}
            onLoadPreset={acq.handleLoadPreset}
            disabled={acq.acquisitionStatus?.isActive}
          />
          <AcquisitionControls
            cameras={acq.cameras}
            selectedProfile={acq.selectedProfile}
            onProfileChange={acq.setSelectedProfile}
            mode={acq.mode}
            onModeChange={acq.setMode}
            continuousSubMode={acq.continuousSubMode}
            triggerMode={acq.triggerMode}
            onTriggerModeChange={acq.setTriggerMode}
            status={acq.acquisitionStatus}
            busy={acq.busy}
            onCapture={acq.handleCapture}
            onStart={acq.handleStart}
            onStop={acq.handleStop}
            onTrigger={acq.handleTrigger}
            onRefresh={acq.refresh}
            refreshThrottled={acq.throttled}
            hasWarnings={acq.hasWarnings}
            hasErrors={acq.hasErrors}
          />
          {acq.mode === "continuous" && (
            <ContinuousSettings
              subMode={acq.continuousSubMode}
              onSubModeChange={acq.setContinuousSubMode}
              frameCount={acq.frameCount}
              onFrameCountChange={acq.setFrameCount}
              intervalMs={acq.intervalMs}
              onIntervalMsChange={acq.setIntervalMs}
              disabled={acq.acquisitionStatus?.isActive}
            />
          )}
        </SidebarSection>

        <SidebarSection label="Exposure">
          <ExposureControl
            exposure={acq.exposure}
            exposureValue={acq.exposureValue}
            gainValue={acq.gainValue}
            busy={acq.busy}
            onExposureChange={acq.setExposureValue}
            onGainChange={acq.setGainValue}
            onLoad={acq.handleLoadExposure}
            onApply={acq.handleApplyExposure}
          />
        </SidebarSection>

        <SidebarSection label="Calibration">
          <CalibrationActions
            busy={acq.busy}
            ffcEnabled={acq.ffcEnabled}
            onBlack={acq.handleBlack}
            onWhite={acq.handleWhite}
            onWhiteBalance={acq.handleWhiteBalance}
            onFfcToggle={acq.handleFfcToggle}
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
        <Box sx={{ flexGrow: 1, p: 2, display: "flex", flexDirection: "column", gap: 2, overflow: "hidden" }}>
          <Box sx={{ flexGrow: 1, minHeight: 0 }}>
            <ImageViewer
              url={selectedFrame?.url ?? null}
              filename={selectedFrame ? `capture-${formatFilenameTimestamp(selectedFrame.capturedAt)}.png` : undefined}
              errorMessage={acq.acquisitionStatus?.lastError}
              savedPath={selectedFrame?.savedPath}
            />
          </Box>
          <Box sx={{ display: "flex", gap: 2, flexShrink: 0 }}>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <HistogramChart />
            </Box>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <AcquisitionStats stats={acq.acquisitionStatus?.statistics} />
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
        <CollapsiblePanel label="Captures" count={capturedFrames.length} defaultOpen>
          <CapturedImageList
            images={capturedFrames}
            selectedId={selectedFrameId}
            onSelect={selectFrame}
            onDelete={deleteFrame}
            onClear={clearAllFrames}
          />
        </CollapsiblePanel>

        <CollapsiblePanel label="Event Log" defaultOpen={false}>
          <EventLog events={acq.acquisitionStatus?.recentEvents} />
        </CollapsiblePanel>
      </Box>

      <Snackbar
        open={acq.snackbar !== null}
        autoHideDuration={3000}
        onClose={() => acq.setSnackbar(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => acq.setSnackbar(null)}
          severity={acq.snackbar?.severity ?? "info"}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {acq.snackbar?.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
