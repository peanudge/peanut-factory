import { useCallback, useState } from "react";
import Box from "@mui/material/Box";
import Tabs from "@mui/material/Tabs";
import Tab from "@mui/material/Tab";
import AcquisitionControls from "../components/AcquisitionControls";
import EventLog from "../components/EventLog";
import ImageGallery from "../components/ImageGallery";
import ContinuousSettings from "../components/ContinuousSettings";
import ImageSaveSettingsPanel from "../components/ImageSaveSettingsPanel";
import PresetSelector from "../components/PresetSelector";
import ExposureControl from "../components/ExposureControl";
import ImageViewer from "../components/ImageViewer";
import CollapsiblePanel from "../components/CollapsiblePanel";
import ErrorBoundary from "../components/ErrorBoundary";
import KeyboardShortcutsHelp from "../components/KeyboardShortcutsHelp";
import { useImageGallery } from "../hooks/useImageGallery";
import { useCapture } from "../hooks/useCapture";
import { useExposure } from "../hooks/useExposure";
import { useResizablePanel } from "../hooks/useResizablePanel";
import { useKeyboardShortcuts } from "../hooks/useKeyboardShortcuts";

const SIDEBAR_TAB_KEY = "peanut-vision-sidebar-tab";

function readSidebarTab(): number {
  try {
    const raw = localStorage.getItem(SIDEBAR_TAB_KEY);
    if (raw === null) return 0;
    const parsed = parseInt(raw, 10);
    return Number.isFinite(parsed) && parsed >= 0 ? parsed : 0;
  } catch {
    return 0;
  }
}

export default function AcquisitionTab() {
  const gallery = useImageGallery();

  const acq = useCapture({ onImageCaptured: gallery.invalidate });
  const exp = useExposure();

  const isCalibrationAvailable =
    acq.acquisitionStatus?.channelState === "idle" ||
    acq.acquisitionStatus?.channelState === "active";

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

  const [sidebarTab, setSidebarTab] = useState<number>(readSidebarTab);
  const [helpOpen, setHelpOpen] = useState(false);

  const handleSidebarTabChange = useCallback((_: unknown, v: number) => {
    setSidebarTab(v);
    try { localStorage.setItem(SIDEBAR_TAB_KEY, String(v)); } catch { /* ignore */ }
  }, []);

  // Keyboard shortcut callbacks
  const handlePrevImage = useCallback(() => {
    const images = gallery.images;
    if (images.length === 0) return;
    const idx = images.findIndex((img) => img.id === gallery.selectedId);
    const prev = idx <= 0 ? images[images.length - 1] : images[idx - 1];
    gallery.setSelectedId(prev.id);
  }, [gallery]);

  const handleNextImage = useCallback(() => {
    const images = gallery.images;
    if (images.length === 0) return;
    const idx = images.findIndex((img) => img.id === gallery.selectedId);
    const next = idx < 0 || idx >= images.length - 1 ? images[0] : images[idx + 1];
    gallery.setSelectedId(next.id);
  }, [gallery]);

  const handleDeleteSelected = useCallback(() => {
    if (gallery.selectedId) gallery.handleDelete(gallery.selectedId);
  }, [gallery]);

  const handleToggleContinuous = useCallback(() => {
    if (acq.acquisitionStatus?.isActive) {
      acq.handleStop();
    } else {
      acq.handleStart();
    }
  }, [acq]);

  useKeyboardShortcuts({
    onCapture: acq.handleCapture,
    onToggleContinuous: handleToggleContinuous,
    onDeleteSelected: handleDeleteSelected,
    onPrevImage: handlePrevImage,
    onNextImage: handleNextImage,
    onReturnToLive: () => gallery.setSelectedId(null),
    onShowHelp: () => setHelpOpen(true),
    isActive: acq.acquisitionStatus?.isActive,
  });

  // Show live preview during active acquisition; show selected historical image otherwise
  const isLive = acq.acquisitionStatus?.isActive ?? !gallery.selectedId;
  const viewerUrl = acq.acquisitionStatus?.isActive
    ? acq.previewUrl
    : (gallery.selectedImageUrl ?? acq.previewUrl);

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
        {/* Sticky tab header */}
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
          <Tabs value={sidebarTab} onChange={handleSidebarTabChange} variant="fullWidth">
            <Tab label="Capture" />
            <Tab label="Camera" />
            <Tab label="Settings" />
          </Tabs>
        </Box>

        <ErrorBoundary label="Sidebar">
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
            exposure={exp.exposure}
            exposureValue={exp.exposureValue}
            isActive={acq.acquisitionStatus?.isActive ?? false}
            busy={exp.busy}
            isCalibrationAvailable={isCalibrationAvailable}
            onExposureChange={exp.setExposureValue}
            onLoad={exp.handleLoadExposure}
            onApply={exp.handleApplyExposure}
          />
        </Box>

        {/* Tab 2: Settings */}
        <Box sx={{ display: sidebarTab === 2 ? "flex" : "none", flexDirection: "column", gap: 2 }}>
          <ImageSaveSettingsPanel />
        </Box>
        </ErrorBoundary>
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
          <ErrorBoundary label="Image Viewer">
            <ImageViewer
              url={viewerUrl}
              filename={gallery.selectedImage?.filename}
              errorMessage={acq.acquisitionStatus?.lastError}
              savedPath={gallery.selectedImage?.filePath}
              isLive={isLive}
              capturedAt={gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null}
              imageId={!isLive ? (gallery.selectedId ?? null) : null}
              tags={gallery.selectedImage?.tags}
              notes={gallery.selectedImage?.notes}
              onReturnToLive={() => gallery.setSelectedId(null)}
            />
          </ErrorBoundary>
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
          <ErrorBoundary label="Image Gallery">
            <ImageGallery
              images={gallery.images}
              selectedId={gallery.selectedId}
              onSelect={gallery.setSelectedId}
              onDelete={gallery.handleDelete}
              page={gallery.page}
              totalPages={gallery.totalPages}
              totalCount={gallery.totalCount}
              onPageChange={gallery.setPage}
              filterFromDate={gallery.filterFromDate}
              onFromDateChange={gallery.setFilterFromDate}
              filterToDate={gallery.filterToDate}
              onToDateChange={gallery.setFilterToDate}
              filterFormat={gallery.filterFormat}
              onFormatChange={gallery.setFilterFormat}
              isLoading={gallery.isLoading}
            />
          </ErrorBoundary>
        </CollapsiblePanel>
        <CollapsiblePanel label="Event Log" defaultOpen={false}>
          <ErrorBoundary label="Event Log">
            <EventLog events={acq.acquisitionStatus?.recentEvents} />
          </ErrorBoundary>
        </CollapsiblePanel>
      </Box>

      <KeyboardShortcutsHelp open={helpOpen} onClose={() => setHelpOpen(false)} />
    </Box>
  );
}
