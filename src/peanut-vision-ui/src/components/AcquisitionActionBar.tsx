import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
import PlayArrowIcon from "@mui/icons-material/PlayArrow";
import StopIcon from "@mui/icons-material/Stop";
import AdjustIcon from "@mui/icons-material/Adjust";
import PhotoCameraIcon from "@mui/icons-material/PhotoCamera";
import RefreshIcon from "@mui/icons-material/Refresh";
import StatusChip from "./StatusChip";
import type { AcquisitionAction, AcquisitionMode, AcquisitionStatus, ContinuousSubMode } from "../api/types";

/** Renders capture/start/stop/trigger buttons and status indicator. */
interface Props {
  mode: AcquisitionMode;
  continuousSubMode: ContinuousSubMode;
  selectedProfile: string;
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

export default function AcquisitionActionBar({
  mode,
  continuousSubMode,
  selectedProfile,
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
    <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
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
          {allowed("stop") ? (
            <Tooltip title={busy ? "처리 중..." : "촬영 중지"}>
              <span>
                <Button
                  color="error"
                  startIcon={<StopIcon />}
                  onClick={onStop}
                  disabled={busy}
                >
                  Stop
                </Button>
              </span>
            </Tooltip>
          ) : (
            <Tooltip
              title={
                !selectedProfile
                  ? "프로파일을 선택하세요"
                  : busy
                  ? "처리 중..."
                  : !allowed("start")
                  ? "시작할 수 없습니다"
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
          )}
          {allowed("trigger") && continuousSubMode === "manual" && (
            <Tooltip title={busy ? "처리 중..." : "수동 촬영 (Manual capture)"}>
              <span>
                <Button
                  startIcon={<AdjustIcon />}
                  onClick={onTrigger}
                  disabled={busy}
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
