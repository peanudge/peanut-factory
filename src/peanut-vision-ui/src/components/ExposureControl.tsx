import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import Slider from "@mui/material/Slider";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import type { ExposureInfo } from "../api/types";
import { DEFAULT_EXPOSURE_MIN, DEFAULT_EXPOSURE_MAX } from "../constants";

interface Props {
  exposure: ExposureInfo | null;
  exposureValue: number;
  isActive: boolean;
  busy: boolean;
  onExposureChange: (value: number) => void;
  onLoad: () => void;
  onApply: () => void;
}

export default function ExposureControl({
  exposure,
  exposureValue,
  isActive,
  busy,
  onExposureChange,
  onLoad,
  onApply,
}: Props) {
  const minExp = exposure?.exposureRange?.min ?? DEFAULT_EXPOSURE_MIN;
  const maxExp = exposure?.exposureRange?.max ?? DEFAULT_EXPOSURE_MAX;

  return (
    <Card variant="outlined">
      <CardContent>
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 1.5 }}>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <Typography variant="subtitle2">Exposure</Typography>
            {isActive ? (
              <Chip label="Live" size="small" color="success" variant="outlined" />
            ) : (
              <Chip label="Pending" size="small" color="default" variant="outlined" />
            )}
          </Box>
          <Tooltip title={isActive ? "Load current values from camera" : "Start acquisition to load camera values"}>
            <span>
              <Button size="small" variant="text" onClick={onLoad} disabled={busy || !isActive}>
                Load Current
              </Button>
            </span>
          </Tooltip>
        </Box>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          <Box>
            <Typography variant="body2" gutterBottom>
              Exposure ({exposureValue.toFixed(0)} &micro;s)
            </Typography>
            <Slider
              value={exposureValue}
              min={minExp}
              max={maxExp}
              step={10}
              onChange={(_, v) => onExposureChange(v as number)}
              valueLabelDisplay="auto"
              valueLabelFormat={(v) => `${v} \u03BCs`}
            />
            {exposure?.exposureRange && (
              <Typography variant="caption" color="text.secondary">
                Range: {minExp} &ndash; {maxExp} &micro;s
              </Typography>
            )}
          </Box>

          <Button variant="contained" onClick={onApply} disabled={busy}>
            {isActive ? "Apply Settings" : "Apply on Start"}
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
