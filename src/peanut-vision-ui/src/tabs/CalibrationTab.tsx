import { useState } from "react";
import Box from "@mui/material/Box";
import Grid from "@mui/material/Grid";
import ErrorAlert from "../components/ErrorAlert";
import SuccessSnackbar from "../components/SuccessSnackbar";
import CalibrationActions from "../components/CalibrationActions";
import ExposureControl from "../components/ExposureControl";
import type { ExposureInfo } from "../api/types";
import {
  blackCalibration,
  whiteCalibration,
  whiteBalance,
  setFfc,
  getExposure,
  setExposure,
} from "../api/client";
import { useAsyncOperation } from "../hooks/useAsyncOperation";

export default function CalibrationTab() {
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);
  const [gainValue, setGainValue] = useState(0);
  const [ffcEnabled, setFfcEnabled] = useState(false);
  const [success, setSuccess] = useState("");
  const { busy, error, clearError, execute } = useAsyncOperation();

  const handleLoadExposure = () =>
    execute(async () => {
      const info = await getExposure();
      setExposureState(info);
      setExposureValue(info.exposureUs);
      setGainValue(info.gainDb);
      setSuccess("Exposure settings loaded");
    });

  const handleApplyExposure = () =>
    execute(async () => {
      const result = await setExposure(exposureValue, gainValue);
      setSuccess(result.message);
    });

  const handleBlack = () =>
    execute(async () => { setSuccess((await blackCalibration()).message); });

  const handleWhite = () =>
    execute(async () => { setSuccess((await whiteCalibration()).message); });

  const handleWhiteBalance = () =>
    execute(async () => { setSuccess((await whiteBalance()).message); });

  const handleFfcToggle = (_: unknown, checked: boolean) => {
    setFfcEnabled(checked);
    execute(async () => { setSuccess((await setFfc(checked)).message); });
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <ErrorAlert error={error} onClose={clearError} />
      <SuccessSnackbar message={success} onClose={() => setSuccess("")} />

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 6 }}>
          <CalibrationActions
            busy={busy}
            ffcEnabled={ffcEnabled}
            onBlack={handleBlack}
            onWhite={handleWhite}
            onWhiteBalance={handleWhiteBalance}
            onFfcToggle={handleFfcToggle}
          />
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <ExposureControl
            exposure={exposure}
            exposureValue={exposureValue}
            gainValue={gainValue}
            busy={busy}
            onExposureChange={setExposureValue}
            onGainChange={setGainValue}
            onLoad={handleLoadExposure}
            onApply={handleApplyExposure}
          />
        </Grid>
      </Grid>
    </Box>
  );
}
