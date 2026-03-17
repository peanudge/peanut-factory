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
import type { AcquisitionStatus, CamFileInfo } from "../api/types";

interface Props {
  cameras: CamFileInfo[];
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
  hasWarnings?: boolean;
  hasErrors?: boolean;
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
  hasWarnings,
  hasErrors,
}: Props) {
  const allowed = (action: string) => status?.allowedActions?.includes(action) ?? false;

  const startDisabled = busy || !allowed("start") || !selectedProfile;
  const stopDisabled = busy || !allowed("stop");
  const triggerDisabled = busy || !allowed("trigger");
  const snapshotDisabled = busy || !allowed("snapshot") || !selectedProfile;

  return (
    <Box sx={{ display: "flex", gap: 2, alignItems: "flex-end", flexWrap: "wrap" }}>
      <FormControl size="small" sx={{ minWidth: 280 }}>
        <InputLabel>Camera Profile</InputLabel>
        <Select
          value={selectedProfile}
          label="Camera Profile"
          onChange={(e) => onProfileChange(e.target.value)}
          disabled={status?.isActive}
        >
          {cameras.map((c) => (
            <MenuItem key={c.fileName} value={c.fileName}>
              {c.fileName}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <ButtonGroup variant="contained">
        <Tooltip title={startDisabled ? "촬영 중에는 시작할 수 없습니다" : "연속 촬영 시작"}>
          <span>
            <Button
              color="success"
              startIcon={<PlayArrowIcon />}
              onClick={onStart}
              disabled={startDisabled}
            >
              Start
            </Button>
          </span>
        </Tooltip>
        <Tooltip title={stopDisabled ? "촬영이 진행 중이지 않습니다" : "촬영 중지"}>
          <span>
            <Button
              color="error"
              startIcon={<StopIcon />}
              onClick={onStop}
              disabled={stopDisabled}
            >
              Stop
            </Button>
          </span>
        </Tooltip>
        <Tooltip title={triggerDisabled ? "촬영을 먼저 시작하세요" : "프레임 촬영"}>
          <span>
            <Button
              startIcon={<AdjustIcon />}
              onClick={onTrigger}
              disabled={triggerDisabled}
            >
              Trigger
            </Button>
          </span>
        </Tooltip>
        <Tooltip title={snapshotDisabled ? "촬영 중에는 스냅샷을 찍을 수 없습니다" : "단일 촬영"}>
          <span>
            <Button
              color="secondary"
              startIcon={<PhotoCameraIcon />}
              onClick={onSnapshot}
              disabled={snapshotDisabled}
            >
              Snapshot
            </Button>
          </span>
        </Tooltip>
      </ButtonGroup>

      {status && (
        <StatusChip
          active={status.isActive}
          label={status.isActive ? `Active (${status.profileId ?? ""})` : "Inactive"}
          hasWarnings={hasWarnings}
          hasErrors={hasErrors}
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
