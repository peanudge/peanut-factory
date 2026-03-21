import Box from "@mui/material/Box";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import type { AcquisitionMode, TriggerModeOption } from "../api/types";

/** Single/Continuous mode toggle paired with trigger mode selector. */
interface Props {
  mode: AcquisitionMode;
  onModeChange: (mode: AcquisitionMode) => void;
  triggerMode: TriggerModeOption;
  onTriggerModeChange: (mode: TriggerModeOption) => void;
  disabled?: boolean;
}

export default function AcquisitionModeSelector({
  mode,
  onModeChange,
  triggerMode,
  onTriggerModeChange,
  disabled,
}: Props) {
  return (
    <Box sx={{ display: "flex", gap: 1, alignItems: "center", flexWrap: "wrap" }}>
      <ToggleButtonGroup
        value={mode}
        exclusive
        onChange={(_, v) => v && onModeChange(v)}
        size="small"
        disabled={disabled}
      >
        <ToggleButton value="single">Single</ToggleButton>
        <ToggleButton value="continuous">Continuous</ToggleButton>
      </ToggleButtonGroup>

      <FormControl size="small" sx={{ flexGrow: 1 }}>
        <InputLabel>Trigger</InputLabel>
        <Select
          value={triggerMode}
          label="Trigger"
          onChange={(e) => onTriggerModeChange(e.target.value as TriggerModeOption)}
          disabled={disabled}
        >
          <MenuItem value="soft">Soft</MenuItem>
          <MenuItem value="hard">Hard</MenuItem>
          <MenuItem value="combined">Combined</MenuItem>
        </Select>
      </FormControl>
    </Box>
  );
}
