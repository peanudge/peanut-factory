import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import type { CamFileInfo } from "../api/types";

/** Dropdown for selecting a camera profile (.cam file). */
interface Props {
  cameras: CamFileInfo[];
  selectedProfile: string;
  onProfileChange: (id: string) => void;
  disabled?: boolean;
}

export default function CameraProfileSelector({ cameras, selectedProfile, onProfileChange, disabled }: Props) {
  return (
    <FormControl size="small" sx={{ minWidth: 280 }}>
      <InputLabel>Camera Profile</InputLabel>
      <Select
        value={selectedProfile}
        label="Camera Profile"
        onChange={(event) => onProfileChange(event.target.value)}
        disabled={disabled}
      >
        {cameras.map((camera) => (
          <MenuItem key={camera.fileName} value={camera.fileName}>
            {camera.fileName}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
