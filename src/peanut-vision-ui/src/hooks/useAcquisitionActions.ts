import { useCallback, useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type {
  AcquisitionMode,
  AcquisitionPreset,
  AcquisitionStatus,
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
  ApiError,
} from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { POLL_INTERVAL_ACTIVE_MS, POLL_INTERVAL_IDLE_MS } from "../constants";

interface UseAcquisitionActionsParams {
  onFrameCaptured: (blob: Blob, savedPath?: string) => void;
}

export function useAcquisitionActions({ onFrameCaptured }: UseAcquisitionActionsParams) {
  const queryClient = useQueryClient();

  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>("auto");
  const [triggerMode, setTriggerMode] = useState<TriggerModeOption>("soft");
  const [frameCount, setFrameCount] = useState<number | null>(null);
  const [intervalMs, setIntervalMs] = useState<number | null>(null);
  const [snackbar, setSnackbar] = useState<{
    message: string;
    severity: "success" | "info" | "warning" | "error";
  } | null>(null);
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);
  const [gainValue, setGainValue] = useState(0);
  const [ffcEnabled, setFfcEnabled] = useState(false);
  const [error, setError] = useState("");
  const [errorCode, setErrorCode] = useState("");

  const clearError = useCallback(() => {
    setError("");
    setErrorCode("");
  }, []);

  const handleError = useCallback((e: unknown) => {
    if (e instanceof ApiError) {
      setError(e.message);
      setErrorCode(e.errorCode);
    } else {
      setError(e instanceof Error ? e.message : "Operation failed");
      setErrorCode("UNKNOWN_ERROR");
    }
  }, []);

  // ── Queries ──

  const { data: cameras = [] } = useQuery({
    queryKey: queryKeys.cameras,
    queryFn: getCameras,
  });

  useEffect(() => {
    if (cameras.length > 0 && !selectedProfile) {
      setSelectedProfile(cameras[0].fileName);
    }
  }, [cameras, selectedProfile]);

  const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
    queryKey: queryKeys.acquisitionStatus,
    queryFn: getAcquisitionStatus,
    refetchInterval: (query) =>
      query.state.data?.isActive ? POLL_INTERVAL_ACTIVE_MS : POLL_INTERVAL_IDLE_MS,
  });

  const { data: latestFrame } = useQuery({
    queryKey: queryKeys.latestFrame,
    queryFn: getLatestFrame,
    refetchInterval: acquisitionStatus?.isActive && acquisitionStatus?.hasFrame ? 1000 : false,
  });

  useEffect(() => {
    if (latestFrame) onFrameCaptured(latestFrame.blob, latestFrame.savedPath);
  }, [latestFrame, onFrameCaptured]);

  // ── Mutations ──

  const invalidateStatus = () =>
    queryClient.invalidateQueries({ queryKey: queryKeys.acquisitionStatus });

  const startMutation = useMutation({
    mutationFn: () =>
      startAcquisition(
        selectedProfile,
        triggerMode,
        frameCount,
        continuousSubMode === "auto" ? intervalMs : null,
      ),
    onSuccess: () => {
      invalidateStatus();
      setSnackbar({ message: "촬영이 시작되었습니다", severity: "success" });
    },
    onError: handleError,
  });

  const stopMutation = useMutation({
    mutationFn: stopAcquisition,
    onSuccess: () => {
      invalidateStatus();
      setSnackbar({ message: "촬영이 중지되었습니다", severity: "info" });
    },
    onError: handleError,
  });

  const triggerMutation = useMutation({
    mutationFn: triggerAndCapture,
    onSuccess: (result) => {
      onFrameCaptured(result.blob, result.savedPath);
      invalidateStatus();
      setSnackbar({ message: "프레임이 촬영되었습니다", severity: "success" });
    },
    onError: handleError,
  });

  const snapshotMutation = useMutation({
    mutationFn: () => snapshot(selectedProfile),
    onSuccess: (result) => {
      onFrameCaptured(result.blob, result.savedPath);
      invalidateStatus();
      setSnackbar({ message: "스냅샷이 촬영되었습니다", severity: "success" });
    },
    onError: handleError,
  });

  const loadExposureMutation = useMutation({
    mutationFn: getExposure,
    onSuccess: (info) => {
      setExposureState(info);
      setExposureValue(info.exposureUs);
      setGainValue(info.gainDb);
      setSnackbar({ message: "Exposure settings loaded", severity: "success" });
    },
    onError: handleError,
  });

  const applyExposureMutation = useMutation({
    mutationFn: () => setExposure(exposureValue, gainValue),
    onSuccess: (result) => {
      setSnackbar({ message: result.message, severity: "success" });
    },
    onError: handleError,
  });

  const blackMutation = useMutation({
    mutationFn: blackCalibration,
    onSuccess: (result) => setSnackbar({ message: result.message, severity: "success" }),
    onError: handleError,
  });

  const whiteMutation = useMutation({
    mutationFn: whiteCalibration,
    onSuccess: (result) => setSnackbar({ message: result.message, severity: "success" }),
    onError: handleError,
  });

  const whiteBalanceMutation = useMutation({
    mutationFn: whiteBalance,
    onSuccess: (result) => setSnackbar({ message: result.message, severity: "success" }),
    onError: handleError,
  });

  const ffcMutation = useMutation({
    mutationFn: (enable: boolean) => setFfc(enable),
    onSuccess: (result) => setSnackbar({ message: result.message, severity: "success" }),
    onError: handleError,
  });

  // ── Computed state ──

  const busy =
    startMutation.isPending ||
    stopMutation.isPending ||
    triggerMutation.isPending ||
    snapshotMutation.isPending ||
    loadExposureMutation.isPending ||
    applyExposureMutation.isPending ||
    blackMutation.isPending ||
    whiteMutation.isPending ||
    whiteBalanceMutation.isPending ||
    ffcMutation.isPending;

  const hasWarnings =
    (acquisitionStatus?.statistics?.droppedFrameCount ?? 0) > 0 ||
    (acquisitionStatus?.statistics?.clusterUnavailableCount ?? 0) > 0;
  const hasErrors =
    !!acquisitionStatus?.lastError ||
    (acquisitionStatus?.statistics?.errorCount ?? 0) > 0;

  const refresh = useCallback(() => {
    queryClient.invalidateQueries({ queryKey: queryKeys.acquisitionStatus });
  }, [queryClient]);

  // ── Handlers ──

  const handleStart = () => startMutation.mutate();
  const handleStop = () => stopMutation.mutate();
  const handleTrigger = () => triggerMutation.mutate();
  const handleCapture = () => snapshotMutation.mutate();
  const handleLoadExposure = () => loadExposureMutation.mutate();
  const handleApplyExposure = () => applyExposureMutation.mutate();
  const handleBlack = () => blackMutation.mutate();
  const handleWhite = () => whiteMutation.mutate();
  const handleWhiteBalance = () => whiteBalanceMutation.mutate();

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
    ffcMutation.mutate(checked);
  };

  return {
    cameras,
    selectedProfile,
    setSelectedProfile,
    mode,
    setMode,
    continuousSubMode,
    setContinuousSubMode,
    triggerMode,
    setTriggerMode,
    frameCount,
    setFrameCount,
    intervalMs,
    setIntervalMs,
    acquisitionStatus: acquisitionStatus ?? null,
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
    errorCode,
    clearError,
    hasWarnings,
    hasErrors,
    refresh,
    throttled: false, // React Query handles deduplication; always allow manual refresh
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
