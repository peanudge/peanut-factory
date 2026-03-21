import { useState } from "react";
import Box from "@mui/material/Box";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import ErrorAlert from "../components/ErrorAlert";
import AcquisitionControls from "../components/AcquisitionControls";
import EventLog from "../components/EventLog";
import CapturedImageList from "../components/CapturedImageList";
import DetailImageViewDialog from "../components/DetailImageViewDialog";
import ContinuousSettings from "../components/ContinuousSettings";
import ImageSaveSettingsPanel from "../components/ImageSaveSettingsPanel";
import SessionSelector from "../components/SessionSelector";
import PresetSelector from "../components/PresetSelector";
import CalibrationActions from "../components/CalibrationActions";
import ExposureControl from "../components/ExposureControl";
import ImageViewer from "../components/ImageViewer";
import CollapsiblePanel from "../components/CollapsiblePanel";
import { useImageBuffer } from "../hooks/useImageBuffer";
import { useAcquisitionActions } from "../hooks/useAcquisitionActions";
import { useResizablePanel } from "../hooks/useResizablePanel";
import { formatFilenameTimestamp } from "../utils/formatTimestamp";
import { MAX_CAPTURED_IMAGES } from "../constants";

interface Props {
  onSessionChange?: (name: string | null) => void;
}

export default function AcquisitionTab({ onSessionChange }: Props = {}) {
  const { capturedFrames, selectedFrameId, addFrame, deleteFrame, clearAllFrames, selectFrame } =
    useImageBuffer(MAX_CAPTURED_IMAGES);

  const acq = useAcquisitionActions({ onFrameCaptured: addFrame });

  const { panelRef: sidebarRef, onResizerMouseDown: onSidebarResizerMouseDown } = useResizablePanel({
    defaultWidth: 340,
    min: 260,
    max: 480,
    direction: "left",
  });

  const { panelRef: rightPanelRef, onResizerMouseDown: onRightResizerMouseDown } = useResizablePanel({
    defaultWidth: 280,
    min: 200,
    max: 560,
  });

  const [sidebarTab, setSidebarTab] = useState(0);
  const [dialogImageId, setDialogImageId] = useState<string | null>(null);
  const dialogImage = capturedFrames.find((f) => f.id === dialogImageId) ?? null;
  const selectedFrame = capturedFrames.find((f) => f.id === selectedFrameId) ?? null;

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      <ErrorAlert error={acq.error} onClose={acq.clearError} />

      {/* LEFT SIDEBAR */}
      <Box
        ref={sidebarRef}
        sx={{
          width: 340,
          flexShrink: 0,
          borderRight: "1px solid",
          borderColor: "divider",
          overflowY: "auto",
          p: 3,
          display: "flex",
          flexDirection: "column",
        }}
      >
        {/* Sticky tab header — bleeds past p:3 with mx:-3 */}
        <Box
          sx={{
            position: "sticky",
            top: 0,
            zIndex: 1,
            bgcolor: "background.paper",
            mx: -3,
            borderBottom: "1px solid",
            borderColor: "divider",
            mb: 2,
          }}
        >
          <Tabs value={sidebarTab} onChange={(_, v) => setSidebarTab(v)} variant="fullWidth">
            <Tab label="Capture" />
            <Tab label="Camera" />
            <Tab label="Settings" />
          </Tabs>
        </Box>

        {/* Tab 0: Capture */}
        <Box sx={{ display: sidebarTab === 0 ? "flex" : "none", flexDirection: "column", gap: 2 }}>
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
        </Box>

        {/* Tab 1: Camera */}
        <Box sx={{ display: sidebarTab === 1 ? "flex" : "none", flexDirection: "column", gap: 2 }}>
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
          <CalibrationActions
            busy={acq.busy}
            ffcEnabled={acq.ffcEnabled}
            onBlack={acq.handleBlack}
            onWhite={acq.handleWhite}
            onWhiteBalance={acq.handleWhiteBalance}
            onFfcToggle={acq.handleFfcToggle}
          />
        </Box>

        {/* Tab 2: Settings */}
        <Box sx={{ display: sidebarTab === 2 ? "flex" : "none", flexDirection: "column", gap: 2 }}>
          <ImageSaveSettingsPanel />
          <SessionSelector onSessionChange={onSessionChange} />
        </Box>
      </Box>

      {/* LEFT SIDEBAR DRAG HANDLE */}
      <Box
        onMouseDown={onSidebarResizerMouseDown}
        sx={{
          width: 4,
          flexShrink: 0,
          cursor: "col-resize",
          bgcolor: "divider",
          transition: "background-color 0.15s",
          "&:hover": { bgcolor: "primary.main" },
        }}
      />

      {/* MAIN CANVAS */}
      <Box sx={{ flexGrow: 1, display: "flex", flexDirection: "column", overflow: "hidden" }}>
        <Box sx={{ flexGrow: 1, minHeight: 0, p: 1.5, display: "flex", flexDirection: "column" }}>
          <ImageViewer
            url={selectedFrame?.url ?? null}
            filename={
              selectedFrame
                ? `capture_${formatFilenameTimestamp(selectedFrame.capturedAt)}.png`
                : undefined
            }
            errorMessage={acq.acquisitionStatus?.lastError}
            savedPath={selectedFrame?.savedPath}
          />
        </Box>
        <Box sx={{ flexShrink: 0, p: 1.5, pt: 0 }}>
          <CapturedImageList
            images={capturedFrames}
            selectedId={selectedFrameId}
            onSelect={(id) => {
              selectFrame(id);
              setDialogImageId(id);
            }}
            onDelete={deleteFrame}
            onClear={clearAllFrames}
          />
        </Box>
      </Box>

      {/* RIGHT PANEL DRAG HANDLE */}
      <Box
        onMouseDown={onRightResizerMouseDown}
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
          width: 280,
          flexShrink: 0,
          display: "flex",
          flexDirection: "column",
          overflow: "hidden",
        }}
      >
        <CollapsiblePanel label="Event Log" defaultOpen={false}>
          <EventLog events={acq.acquisitionStatus?.recentEvents} />
        </CollapsiblePanel>
      </Box>

      <DetailImageViewDialog
        image={dialogImage}
        errorMessage={acq.acquisitionStatus?.lastError}
        onClose={() => setDialogImageId(null)}
      />

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
