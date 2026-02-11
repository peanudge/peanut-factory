import { useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Slider from "@mui/material/Slider";
import Switch from "@mui/material/Switch";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import Alert from "@mui/material/Alert";
import FormControlLabel from "@mui/material/FormControlLabel";
import Grid from "@mui/material/Grid";
import Snackbar from "@mui/material/Snackbar";
import {
  blackCalibration,
  whiteCalibration,
  whiteBalance,
  setFfc,
  getExposure,
  setExposure,
} from "../api/client";
import type { ExposureInfo } from "../api/types";

export default function CalibrationTab() {
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);
  const [gainValue, setGainValue] = useState(0);
  const [ffcEnabled, setFfcEnabled] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [busy, setBusy] = useState(false);

  const wrap = async (fn: () => Promise<string>) => {
    setBusy(true);
    setError("");
    try {
      const msg = await fn();
      setSuccess(msg);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setBusy(false);
    }
  };

  const handleLoadExposure = () =>
    wrap(async () => {
      const info = await getExposure();
      setExposureState(info);
      setExposureValue(info.exposureUs);
      setGainValue(info.gainDb);
      return "Exposure settings loaded";
    });

  const handleApplyExposure = () =>
    wrap(async () => {
      const result = await setExposure(exposureValue, gainValue);
      return result.message;
    });

  const handleBlack = () =>
    wrap(async () => (await blackCalibration()).message);

  const handleWhite = () =>
    wrap(async () => (await whiteCalibration()).message);

  const handleWhiteBalance = () =>
    wrap(async () => (await whiteBalance()).message);

  const handleFfcToggle = (_: unknown, checked: boolean) => {
    setFfcEnabled(checked);
    wrap(async () => (await setFfc(checked)).message);
  };

  const minExp = exposure?.exposureRange?.min ?? 100;
  const maxExp = exposure?.exposureRange?.max ?? 10000;

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      {error && (
        <Alert severity="error" onClose={() => setError("")}>
          {error}
        </Alert>
      )}

      <Snackbar
        open={!!success}
        autoHideDuration={3000}
        onClose={() => setSuccess("")}
        message={success}
      />

      <Grid container spacing={3}>
        {/* Calibration actions */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Card variant="outlined">
            <CardContent>
              <Typography variant="subtitle2" gutterBottom>
                Calibration Actions
              </Typography>
              <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
                <Button
                  variant="outlined"
                  disabled={busy}
                  onClick={handleBlack}
                >
                  Black Calibration
                </Button>
                <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
                  Cover the lens before executing
                </Typography>

                <Button
                  variant="outlined"
                  disabled={busy}
                  onClick={handleWhite}
                >
                  White Calibration
                </Button>
                <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
                  Ensure uniform ~200DN illumination
                </Typography>

                <Button
                  variant="outlined"
                  disabled={busy}
                  onClick={handleWhiteBalance}
                >
                  White Balance (Once)
                </Button>
                <Typography variant="caption" color="text.secondary" sx={{ mt: -1 }}>
                  Point lens at white target (~200DN)
                </Typography>

                <FormControlLabel
                  control={
                    <Switch
                      checked={ffcEnabled}
                      onChange={handleFfcToggle}
                      disabled={busy}
                    />
                  }
                  label="Flat Field Correction (FFC)"
                />
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Exposure & Gain */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Card variant="outlined">
            <CardContent>
              <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
                <Typography variant="subtitle2">
                  Exposure &amp; Gain
                </Typography>
                <Button
                  size="small"
                  variant="text"
                  onClick={handleLoadExposure}
                  disabled={busy}
                >
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
                    onChange={(_, v) => setExposureValue(v as number)}
                    valueLabelDisplay="auto"
                    valueLabelFormat={(v) => `${v} μs`}
                  />
                  {exposure?.exposureRange && (
                    <Typography variant="caption" color="text.secondary">
                      Range: {minExp} – {maxExp} &micro;s
                    </Typography>
                  )}
                </Box>

                <TextField
                  label="Gain (dB)"
                  type="number"
                  size="small"
                  value={gainValue}
                  onChange={(e) => setGainValue(parseFloat(e.target.value) || 0)}
                  slotProps={{ htmlInput: { step: 0.5 } }}
                />

                <Button
                  variant="contained"
                  onClick={handleApplyExposure}
                  disabled={busy}
                >
                  Apply Settings
                </Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
