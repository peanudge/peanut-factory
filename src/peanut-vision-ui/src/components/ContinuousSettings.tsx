import { useState } from "react";
import Box from "@mui/material/Box";
import Checkbox from "@mui/material/Checkbox";
import FormControlLabel from "@mui/material/FormControlLabel";
import TextField from "@mui/material/TextField";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import type { ContinuousSubMode } from "../api/types";

interface Props {
  subMode: ContinuousSubMode;
  onSubModeChange: (value: ContinuousSubMode) => void;
  frameCount: number | null;
  onFrameCountChange: (value: number | null) => void;
  intervalMs: number | null;
  onIntervalMsChange: (value: number | null) => void;
  disabled?: boolean;
}

export default function ContinuousSettings({
  subMode,
  onSubModeChange,
  frameCount,
  onFrameCountChange,
  intervalMs,
  onIntervalMsChange,
  disabled,
}: Props) {
  const [infinite, setInfinite] = useState(frameCount === null);

  const handleInfiniteChange = (checked: boolean) => {
    setInfinite(checked);
    onFrameCountChange(checked ? null : 10);
  };

  return (
    <Box sx={{ display: "flex", gap: 2, alignItems: "center", flexWrap: "wrap" }}>
      <ToggleButtonGroup
        value={subMode}
        exclusive
        onChange={(_, v) => v && onSubModeChange(v)}
        size="small"
        disabled={disabled}
      >
        <ToggleButton value="auto">Auto</ToggleButton>
        <ToggleButton value="manual">Manual</ToggleButton>
      </ToggleButtonGroup>

      <TextField
        label="Frame Count"
        type="number"
        size="small"
        value={infinite ? "" : (frameCount ?? "")}
        onChange={(e) => {
          const v = parseInt(e.target.value, 10);
          onFrameCountChange(isNaN(v) || v < 1 ? null : v);
        }}
        disabled={disabled || infinite}
        slotProps={{ htmlInput: { min: 1 } }}
        sx={{ width: 130 }}
      />
      <FormControlLabel
        control={
          <Checkbox
            checked={infinite}
            onChange={(e) => handleInfiniteChange(e.target.checked)}
            size="small"
            disabled={disabled}
          />
        }
        label="Infinite"
      />

      {subMode === "auto" && (
        <TextField
          label="Interval (ms)"
          type="number"
          size="small"
          value={intervalMs ?? ""}
          onChange={(e) => {
            const v = parseInt(e.target.value, 10);
            onIntervalMsChange(isNaN(v) || v < 0 ? null : v);
          }}
          disabled={disabled}
          helperText="최소 50ms"
          slotProps={{ htmlInput: { min: 50 } }}
          sx={{ width: 130 }}
        />
      )}
    </Box>
  );
}
