import { useCallback, useState } from "react";
import { Link } from "react-router-dom";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Toolbar from "@mui/material/Toolbar";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import CameraIcon from "@mui/icons-material/CameraAlt";
import PhotoLibraryIcon from "@mui/icons-material/PhotoLibrary";
import SettingsIcon from "@mui/icons-material/Settings";
import AcquisitionControls from "../components/AcquisitionControls";
import ContinuousSettings from "../components/ContinuousSettings";
import ErrorBoundary from "../components/ErrorBoundary";
import EventLog from "../components/EventLog";
import ExposureControl from "../components/ExposureControl";
import CollapsiblePanel from "../components/CollapsiblePanel";
import ImageViewer from "../components/ImageViewer";
import KeyboardShortcutsHelp from "../components/KeyboardShortcutsHelp";
import PresetSelector from "../components/PresetSelector";
import { useCapture } from "../hooks/useCapture";
import { useExposure } from "../hooks/useExposure";
import { useImageGallery } from "../hooks/useImageGallery";
import { useKeyboardShortcuts } from "../hooks/useKeyboardShortcuts";

export default function CapturePage() {
  const gallery = useImageGallery();
  const acq = useCapture({ onImageCaptured: gallery.invalidate });
  const exp = useExposure();

  const [helpOpen, setHelpOpen] = useState(false);

  const isCalibrationAvailable =
    acq.acquisitionStatus?.channelState === "idle" ||
    acq.acquisitionStatus?.channelState === "active";

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
    <Box sx={{ display: "flex", flexDirection: "column", height: "100vh" }}>
      {/* AppBar */}
      <AppBar position="static" elevation={0} sx={{ borderBottom: "1px solid", borderColor: "divider" }}>
        <Toolbar variant="dense">
          <CameraIcon sx={{ mr: 1 }} />
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            PeanutVision
          </Typography>
          <Button
            component={Link}
            to="/gallery"
            color="inherit"
            startIcon={<PhotoLibraryIcon />}
            sx={{ textTransform: "none", mr: 1 }}
          >
            갤러리
          </Button>
          <Tooltip title="Backoffice">
            <IconButton component={Link} to="/backoffice" color="inherit" size="small">
              <SettingsIcon />
            </IconButton>
          </Tooltip>
        </Toolbar>
      </AppBar>

      {/* Main content: left panel + right image viewer */}
      <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden" }}>
        {/* LEFT PANEL */}
        <Box
          sx={{
            width: 320,
            flexShrink: 0,
            borderRight: "1px solid",
            borderColor: "divider",
            overflowY: "auto",
            overflowX: "hidden",
            p: 2,
            display: "flex",
            flexDirection: "column",
            gap: 2,
          }}
        >
          <ErrorBoundary label="Capture Panel">
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
          </ErrorBoundary>
        </Box>

        {/* RIGHT PANEL: Image Viewer + optional Event Log */}
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

          <CollapsiblePanel label="Event Log" defaultOpen={false}>
            <ErrorBoundary label="Event Log">
              <EventLog events={acq.acquisitionStatus?.recentEvents} />
            </ErrorBoundary>
          </CollapsiblePanel>
        </Box>
      </Box>

      <KeyboardShortcutsHelp open={helpOpen} onClose={() => setHelpOpen(false)} />
    </Box>
  );
}
