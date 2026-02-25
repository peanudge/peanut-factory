import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import FormControl from "@mui/material/FormControl";
import IconButton from "@mui/material/IconButton";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import Tooltip from "@mui/material/Tooltip";
import PlayArrowIcon from "@mui/icons-material/PlayArrow";
import StopIcon from "@mui/icons-material/Stop";
import AdjustIcon from "@mui/icons-material/Adjust";
import PhotoCameraIcon from "@mui/icons-material/PhotoCamera";
import RefreshIcon from "@mui/icons-material/Refresh";
import StatusChip from "./StatusChip";
import type { AcquisitionStatus, CameraProfile } from "../api/types";

interface Props {
  cameras: CameraProfile[];
  selectedProfile: string;
  onProfileChange: (id: string) => void;
  status: AcquisitionStatus | null;
  busy: boolean;
  onStart: () => void;
  onStop: () => void;
  onTrigger: () => void;
  onSnapshot: () => void;
  onRefresh: () => void;
  refreshThrottled: boolean;
}

export default function AcquisitionControls({
  cameras,
  selectedProfile,
  onProfileChange,
  status,
  busy,
  onStart,
  onStop,
  onTrigger,
  onSnapshot,
  onRefresh,
  refreshThrottled,
}: Props) {
  return (
    <Box sx={{ display: "flex", gap: 2, alignItems: "flex-end", flexWrap: "wrap" }}>
      <FormControl size="small" sx={{ minWidth: 280 }}>
        <InputLabel>Camera Profile</InputLabel>
        <Select
          value={selectedProfile}
          label="Camera Profile"
          onChange={(e) => onProfileChange(e.target.value)}
        >
          {cameras.map((c) => (
            <MenuItem key={c.id} value={c.id}>
              {c.displayName}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <ButtonGroup variant="contained" disabled={busy}>
        <Button
          color="success"
          startIcon={<PlayArrowIcon />}
          onClick={onStart}
          disabled={busy || !selectedProfile}
        >
          Start
        </Button>
        <Button
          color="error"
          startIcon={<StopIcon />}
          onClick={onStop}
        >
          Stop
        </Button>
        <Button
          startIcon={<AdjustIcon />}
          onClick={onTrigger}
        >
          Trigger
        </Button>
        <Button
          color="secondary"
          startIcon={<PhotoCameraIcon />}
          onClick={onSnapshot}
          disabled={busy || !selectedProfile}
        >
          Snapshot
        </Button>
      </ButtonGroup>

      {status && (
        <StatusChip
          active={status.isActive}
          label={status.isActive ? `Active (${status.profileId ?? ""})` : "Inactive"}
        />
      )}

      <Tooltip title={refreshThrottled ? "Wait..." : "Refresh status"}>
        <span>
          <IconButton
            size="small"
            onClick={onRefresh}
            disabled={refreshThrottled}
          >
            <RefreshIcon />
          </IconButton>
        </span>
      </Tooltip>
    </Box>
  );
}
