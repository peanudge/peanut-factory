import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import FormControlLabel from "@mui/material/FormControlLabel";
import Switch from "@mui/material/Switch";
import Typography from "@mui/material/Typography";

interface Props {
  busy: boolean;
  ffcEnabled: boolean;
  onBlack: () => void;
  onWhite: () => void;
  onWhiteBalance: () => void;
  onFfcToggle: (_: unknown, checked: boolean) => void;
}

export default function CalibrationActions({
  busy,
  ffcEnabled,
  onBlack,
  onWhite,
  onWhiteBalance,
  onFfcToggle,
}: Props) {
  return (
    <Card variant="outlined">
      <CardContent>
        <Typography variant="subtitle2" gutterBottom>
          Calibration Actions
        </Typography>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5, mt: 1 }}>
          <Box>
            <Button variant="outlined" fullWidth disabled={busy} onClick={onBlack}>
              Black Calibration
            </Button>
            <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 0.5, px: 0.5 }}>
              Cover the lens before executing
            </Typography>
          </Box>

          <Box>
            <Button variant="outlined" fullWidth disabled={busy} onClick={onWhite}>
              White Calibration
            </Button>
            <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 0.5, px: 0.5 }}>
              Ensure uniform ~200DN illumination
            </Typography>
          </Box>

          <Box>
            <Button variant="outlined" fullWidth disabled={busy} onClick={onWhiteBalance}>
              White Balance (Once)
            </Button>
            <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 0.5, px: 0.5 }}>
              Point lens at white target (~200DN)
            </Typography>
          </Box>

          <FormControlLabel
            control={
              <Switch
                checked={ffcEnabled}
                onChange={onFfcToggle}
                disabled={busy}
              />
            }
            label="Flat Field Correction (FFC)"
          />
        </Box>
      </CardContent>
    </Card>
  );
}
