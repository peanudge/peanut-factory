import Box from "@mui/material/Box";
import CameraProfileSelector from "./CameraProfileSelector";
import AcquisitionModeSelector from "./AcquisitionModeSelector";
import AcquisitionActionBar from "./AcquisitionActionBar";
import type { AcquisitionMode, AcquisitionStatus, CamFileInfo, ContinuousSubMode, TriggerModeOption } from "../api/types";

/** Thin wrapper composing camera profile, mode/trigger selection, and action buttons. */
interface Props {
  cameras: CamFileInfo[];
  selectedProfile: string;
  onProfileChange: (id: string) => void;
  mode: AcquisitionMode;
  onModeChange: (mode: AcquisitionMode) => void;
  continuousSubMode: ContinuousSubMode;
  triggerMode: TriggerModeOption;
  onTriggerModeChange: (mode: TriggerModeOption) => void;
  status: AcquisitionStatus | null;
  busy: boolean;
  onCapture: () => void;
  onStart: () => void;
  onStop: () => void;
  onTrigger: () => void;
  onRefresh: () => void;
  refreshThrottled: boolean;
  hasWarnings?: boolean;
  hasErrors?: boolean;
}

export default function AcquisitionControls({
  cameras,
  selectedProfile,
  onProfileChange,
  mode,
  onModeChange,
  continuousSubMode,
  triggerMode,
  onTriggerModeChange,
  status,
  busy,
  onCapture,
  onStart,
  onStop,
  onTrigger,
  onRefresh,
  refreshThrottled,
  hasWarnings,
  hasErrors,
}: Props) {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
      <CameraProfileSelector
        cameras={cameras}
        selectedProfile={selectedProfile}
        onProfileChange={onProfileChange}
        disabled={status?.isActive}
      />

      <AcquisitionModeSelector
        mode={mode}
        onModeChange={onModeChange}
        triggerMode={triggerMode}
        onTriggerModeChange={onTriggerModeChange}
        disabled={status?.isActive}
      />

      <AcquisitionActionBar
        mode={mode}
        continuousSubMode={continuousSubMode}
        selectedProfile={selectedProfile}
        status={status}
        busy={busy}
        onCapture={onCapture}
        onStart={onStart}
        onStop={onStop}
        onTrigger={onTrigger}
        onRefresh={onRefresh}
        refreshThrottled={refreshThrottled}
        hasWarnings={hasWarnings}
        hasErrors={hasErrors}
      />
    </Box>
  );
}
