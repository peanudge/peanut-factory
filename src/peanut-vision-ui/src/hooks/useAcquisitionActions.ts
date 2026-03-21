import { useCallback, useEffect, useState } from "react";
import type {
  AcquisitionMode,
  AcquisitionPreset,
  AcquisitionStatus,
  CamFileInfo,
  ContinuousSubMode,
  ExposureInfo,
  TriggerModeOption,
} from "../api/types";
import {
  getCameras,
  startAcquisition,
  stopAcquisition,
  getAcquisitionStatus,
  triggerAndCapture,
  snapshot,
  getLatestFrame,
  blackCalibration,
  whiteCalibration,
  whiteBalance,
  setFfc,
  getExposure,
  setExposure,
} from "../api/client";
import { useAsyncOperation } from "./useAsyncOperation";
import { usePolling } from "./usePolling";
import { DEFAULT_CONTINUOUS_INTERVAL_MS, POLL_INTERVAL_ACTIVE_MS, POLL_INTERVAL_IDLE_MS } from "../constants";

interface UseAcquisitionActionsParams {
  onFrameCaptured: (blob: Blob, savedPath?: string) => void;
}

export function useAcquisitionActions({ onFrameCaptured }: UseAcquisitionActionsParams) {
  const [cameras, setCameras] = useState<CamFileInfo[]>([]);
  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>("auto");
  const [triggerMode, setTriggerMode] = useState<TriggerModeOption>("soft");
  const [frameCount, setFrameCount] = useState<number | null>(null);
  const [intervalMs, setIntervalMs] = useState<number | null>(null);
  const [acquisitionStatus, setAcquisitionStatus] = useState<AcquisitionStatus | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "info" | "warning" | "error";
  } | null>(null);
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);
  const [gainValue, setGainValue] = useState(0);
  const [ffcEnabled, setFfcEnabled] = useState(false);
  const { busy, error, clearError, execute } = useAsyncOperation();

  const hasWarnings =
    (acquisitionStatus?.statistics?.droppedFrameCount ?? 0) > 0 ||
    (acquisitionStatus?.statistics?.clusterUnavailableCount ?? 0) > 0;
  const hasErrors =
    !!acquisitionStatus?.lastError ||
    (acquisitionStatus?.statistics?.errorCount ?? 0) > 0;

  const fetchStatus = useCallback(() => {
    getAcquisitionStatus().then(setAcquisitionStatus).catch(() => {});
  }, []);

  const pollInterval = acquisitionStatus?.isActive
    ? POLL_INTERVAL_ACTIVE_MS
    : POLL_INTERVAL_IDLE_MS;
  const { refresh, throttled } = usePolling(fetchStatus, pollInterval);

  useEffect(() => {
    getCameras()
      .then((c) => {
        setCameras(c);
        if (c.length > 0) setSelectedProfile(c[0].fileName);
      })
      .catch(() => {});
  }, []);

  useEffect(() => {
    if (!acquisitionStatus?.isActive || !acquisitionStatus?.hasFrame) return;
    const t = setInterval(async () => {
      try {
        const result = await getLatestFrame();
        if (result) onFrameCaptured(result.blob, result.savedPath);
      } catch {
        /* ignore */
      }
    }, 1000);
    return () => clearInterval(t);
  }, [acquisitionStatus?.isActive, acquisitionStatus?.hasFrame, onFrameCaptured]);

  const handleStart = () => {
    if (mode === "continuous" && continuousSubMode === "auto" && intervalMs === null) {
      setSnackbar({ message: "Please input interval time for continuous mode", severity: "warning" });
      return;
    }
    execute(async () => {
      await startAcquisition(
        selectedProfile,
        triggerMode,
        frameCount,
        continuousSubMode === "auto" ? intervalMs : null,
      );
      fetchStatus();
      setSnackbar({ message: "촬영이 시작되었습니다", severity: "success" });
    });
  };

  const handleStop = () =>
    execute(async () => {
      await stopAcquisition();
      fetchStatus();
      setSnackbar({ message: "촬영이 중지되었습니다", severity: "info" });
    });

  const handleTrigger = () =>
    execute(async () => {
      const { blob, savedPath } = await triggerAndCapture();
      onFrameCaptured(blob, savedPath);
      fetchStatus();
      setSnackbar({ message: "프레임이 촬영되었습니다", severity: "success" });
    });

  const handleCapture = () =>
    execute(async () => {
      const { blob, savedPath } = await snapshot(selectedProfile);
      onFrameCaptured(blob, savedPath);
      fetchStatus();
      setSnackbar({ message: "스냅샷이 촬영되었습니다", severity: "success" });
    });

  const handleLoadExposure = () =>
    execute(async () => {
      const info = await getExposure();
      setExposureState(info);
      setExposureValue(info.exposureUs);
      setGainValue(info.gainDb);
      setSnackbar({ message: "Exposure settings loaded", severity: "success" });
    });

  const handleApplyExposure = () =>
    execute(async () => {
      const result = await setExposure(exposureValue, gainValue);
      setSnackbar({ message: result.message, severity: "success" });
    });

  const handleBlack = () =>
    execute(async () => {
      setSnackbar({ message: (await blackCalibration()).message, severity: "success" });
    });

  const handleWhite = () =>
    execute(async () => {
      setSnackbar({ message: (await whiteCalibration()).message, severity: "success" });
    });

  const handleWhiteBalance = () =>
    execute(async () => {
      setSnackbar({ message: (await whiteBalance()).message, severity: "success" });
    });

  const handleLoadPreset = useCallback((preset: AcquisitionPreset) => {
    setSelectedProfile(preset.profileId);
    setTriggerMode(preset.triggerMode ?? "soft");
    setFrameCount(preset.frameCount ?? null);
    setIntervalMs(preset.intervalMs ?? null);
    if (preset.frameCount != null || preset.intervalMs != null) {
      setMode("continuous");
    }
  }, []);

  const handleFfcToggle = (_: unknown, checked: boolean) => {
    setFfcEnabled(checked);
    execute(async () => {
      setSnackbar({ message: (await setFfc(checked)).message, severity: "success" });
    });
  };

  const handleSetMode = useCallback(
    (next: AcquisitionMode) => {
      setMode(next);
      if (next === "continuous") {
        setIntervalMs((prev) => prev ?? DEFAULT_CONTINUOUS_INTERVAL_MS);
      }
    },
    []
  );

  return {
    cameras,
    selectedProfile,
    setSelectedProfile,
    mode,
    setMode: handleSetMode,
    continuousSubMode,
    setContinuousSubMode,
    triggerMode,
    setTriggerMode,
    frameCount,
    setFrameCount,
    intervalMs,
    setIntervalMs,
    acquisitionStatus,
    snackbar,
    setSnackbar,
    exposure,
    exposureValue,
    setExposureValue,
    gainValue,
    setGainValue,
    ffcEnabled,
    busy,
    error,
    clearError,
    hasWarnings,
    hasErrors,
    refresh,
    throttled,
    handleStart,
    handleStop,
    handleTrigger,
    handleCapture,
    handleLoadExposure,
    handleApplyExposure,
    handleBlack,
    handleWhite,
    handleWhiteBalance,
    handleLoadPreset,
    handleFfcToggle,
  };
}
