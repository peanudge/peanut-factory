import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import FormControl from "@mui/material/FormControl";
import IconButton from "@mui/material/IconButton";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import Tooltip from "@mui/material/Tooltip";
import PlayArrowIcon from "@mui/icons-material/PlayArrow";
import StopIcon from "@mui/icons-material/Stop";
import AdjustIcon from "@mui/icons-material/Adjust";
import PhotoCameraIcon from "@mui/icons-material/PhotoCamera";
import RefreshIcon from "@mui/icons-material/Refresh";
import StatusChip from "./StatusChip";
import type { AcquisitionAction, AcquisitionMode, AcquisitionStatus, CamFileInfo } from "../api/types";

interface Props {
  cameras: CamFileInfo[];
  selectedProfile: string;
  onProfileChange: (id: string) => void;
  mode: AcquisitionMode;
  onModeChange: (mode: AcquisitionMode) => void;
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
  const allowed = (action: AcquisitionAction) =>
    status?.allowedActions?.includes(action) ?? false;

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

      <ToggleButtonGroup
        value={mode}
        exclusive
        onChange={(_, v) => v && onModeChange(v)}
        size="small"
        disabled={status?.isActive}
      >
        <ToggleButton value="single">Single</ToggleButton>
        <ToggleButton value="continuous">Continuous</ToggleButton>
      </ToggleButtonGroup>

      {mode === "single" ? (
        <Tooltip
          title={
            busy || !allowed("snapshot") || !selectedProfile
              ? "촬영 중에는 스냅샷을 찍을 수 없습니다"
              : "단일 촬영"
          }
        >
          <span>
            <Button
              variant="contained"
              startIcon={<PhotoCameraIcon />}
              onClick={onCapture}
              disabled={busy || !allowed("snapshot") || !selectedProfile}
            >
              Capture
            </Button>
          </span>
        </Tooltip>
      ) : (
        <ButtonGroup variant="contained">
          {!status?.isActive ? (
            <Tooltip
              title={
                busy || !allowed("start") || !selectedProfile
                  ? "프로파일을 선택하세요"
                  : "연속 촬영 시작"
              }
            >
              <span>
                <Button
                  color="success"
                  startIcon={<PlayArrowIcon />}
                  onClick={onStart}
                  disabled={busy || !allowed("start") || !selectedProfile}
                >
                  Start
                </Button>
              </span>
            </Tooltip>
          ) : (
            <Tooltip title={busy ? "처리 중..." : "촬영 중지"}>
              <span>
                <Button
                  color="error"
                  startIcon={<StopIcon />}
                  onClick={onStop}
                  disabled={busy || !allowed("stop")}
                >
                  Stop
                </Button>
              </span>
            </Tooltip>
          )}
          {status?.isActive && (
            <Tooltip
              title={busy || !allowed("trigger") ? "처리 중..." : "프레임 촬영"}
            >
              <span>
                <Button
                  startIcon={<AdjustIcon />}
                  onClick={onTrigger}
                  disabled={busy || !allowed("trigger")}
                >
                  Trigger
                </Button>
              </span>
            </Tooltip>
          )}
        </ButtonGroup>
      )}

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
