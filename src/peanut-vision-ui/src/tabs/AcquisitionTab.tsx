import { useState } from "react";
import Box from "@mui/material/Box";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import AcquisitionControls from "../components/AcquisitionControls";
import EventLog from "../components/EventLog";
import ImageGallery from "../components/ImageGallery";
import ContinuousSettings from "../components/ContinuousSettings";
import ImageSaveSettingsPanel from "../components/ImageSaveSettingsPanel";
import SessionSelector from "../components/SessionSelector";
import PresetSelector from "../components/PresetSelector";
import CalibrationActions from "../components/CalibrationActions";
import ExposureControl from "../components/ExposureControl";
import ImageViewer from "../components/ImageViewer";
import CollapsiblePanel from "../components/CollapsiblePanel";
import { useImageGallery } from "../hooks/useImageGallery";
import { useAcquisitionActions } from "../hooks/useAcquisitionActions";
import { useLivePreview } from "../hooks/useLivePreview";
import { useResizablePanel } from "../hooks/useResizablePanel";

interface Props {
  onSessionChange?: (name: string | null) => void;
}

export default function AcquisitionTab({ onSessionChange }: Props = {}) {
  const gallery = useImageGallery();
  const acq = useAcquisitionActions();
  const live = useLivePreview(acq.acquisitionStatus);

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

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      {/* LEFT SIDEBAR */}
      <Box
        ref={sidebarRef}
        sx={{
          width: 340,
          flexShrink: 0,
          borderRight: "1px solid",
          borderColor: "divider",
          overflowY: "auto",
          overflowX: "hidden",
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
            isActive={acq.acquisitionStatus?.isActive ?? false}
            busy={acq.busy}
            isCalibrationAvailable={acq.isCalibrationAvailable}
            onExposureChange={acq.setExposureValue}
            onLoad={acq.handleLoadExposure}
            onApply={acq.handleApplyExposure}
          />
          <CalibrationActions
            busy={acq.busy}
            isCalibrationAvailable={acq.isCalibrationAvailable}
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
            url={live.previewUrl}
            errorMessage={acq.acquisitionStatus?.lastError}
            isLive={live.isActive}
            capturedAt={null}
            onClose={() => {}}
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
        <CollapsiblePanel label="Captures" count={gallery.totalCount} defaultOpen={true}>
          {gallery.selectedImageUrl && (
            <Box sx={{ mb: 1, maxHeight: 240, overflow: "hidden" }}>
              <ImageViewer
                url={gallery.selectedImageUrl}
                filename={gallery.selectedImage?.filename}
                savedPath={gallery.selectedImage?.filePath}
                isLive={false}
                capturedAt={gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null}
                onClose={() => gallery.setSelectedId(null)}
              />
            </Box>
          )}
          <ImageGallery
            images={gallery.images}
            selectedId={gallery.selectedId}
            onSelect={gallery.setSelectedId}
            onDelete={gallery.handleDelete}
            page={gallery.page}
            totalPages={gallery.totalPages}
            onPageChange={gallery.setPage}
            filterSessionId={gallery.filterSessionId}
            onFilterChange={gallery.setFilterSessionId}
            isLoading={gallery.isLoading}
          />
        </CollapsiblePanel>
        <CollapsiblePanel label="Event Log" defaultOpen={false}>
          <EventLog events={acq.acquisitionStatus?.recentEvents} />
        </CollapsiblePanel>
      </Box>
    </Box>
  );
}
