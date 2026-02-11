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
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
          <Button variant="outlined" disabled={busy} onClick={onBlack}>
            Black Calibration
          </Button>
          <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
            Cover the lens before executing
          </Typography>

          <Button variant="outlined" disabled={busy} onClick={onWhite}>
            White Calibration
          </Button>
          <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
            Ensure uniform ~200DN illumination
          </Typography>

          <Button variant="outlined" disabled={busy} onClick={onWhiteBalance}>
            White Balance (Once)
          </Button>
          <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
            Point lens at white target (~200DN)
          </Typography>

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
