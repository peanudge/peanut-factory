import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Slider from "@mui/material/Slider";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import type { ExposureInfo } from "../api/types";
import { DEFAULT_EXPOSURE_MIN, DEFAULT_EXPOSURE_MAX } from "../constants";

interface Props {
  exposure: ExposureInfo | null;
  exposureValue: number;
  gainValue: number;
  busy: boolean;
  onExposureChange: (value: number) => void;
  onGainChange: (value: number) => void;
  onLoad: () => void;
  onApply: () => void;
}

export default function ExposureControl({
  exposure,
  exposureValue,
  gainValue,
  busy,
  onExposureChange,
  onGainChange,
  onLoad,
  onApply,
}: Props) {
  const minExp = exposure?.exposureRange?.min ?? DEFAULT_EXPOSURE_MIN;
  const maxExp = exposure?.exposureRange?.max ?? DEFAULT_EXPOSURE_MAX;

  return (
    <Card variant="outlined">
      <CardContent>
        <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
          <Typography variant="subtitle2">Exposure &amp; Gain</Typography>
          <Button size="small" variant="text" onClick={onLoad} disabled={busy}>
            Load Current
          </Button>
        </Box>

        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
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

          <TextField
            label="Gain (dB)"
            type="number"
            size="small"
            value={gainValue}
            onChange={(e) => onGainChange(parseFloat(e.target.value) || 0)}
            slotProps={{ htmlInput: { step: 0.5 } }}
          />

          <Button variant="contained" onClick={onApply} disabled={busy}>
            Apply Settings
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
}
