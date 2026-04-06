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
  blackCalibration,
  whiteCalibration,
  whiteBalance,
  setFfc,
  getExposure,
  setExposure,
  ApiError,
} from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { useToast } from "../contexts/ToastContext";
import { DEFAULT_CONTINUOUS_INTERVAL_MS, POLL_INTERVAL_ACTIVE_MS, POLL_INTERVAL_IDLE_MS } from "../constants";

export function useAcquisitionActions() {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>("auto");
  const [triggerMode, setTriggerMode] = useState<TriggerModeOption>("soft");
  const [frameCount, setFrameCount] = useState<number | null>(null);
  const [intervalMs, setIntervalMs] = useState<number | null>(null);
  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);
  const [ffcEnabled, setFfcEnabled] = useState(false);

  const handleError = useCallback((e: unknown) => {
    toast(e instanceof ApiError ? e.message : e instanceof Error ? e.message : "Operation failed", "error");
  }, [toast]);

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

  useEffect(() => {
    getExposure()
      .then((info) => { setExposureState(info); setExposureValue(info.exposureUs); })
      .catch(() => {});
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
    queryKey: queryKeys.acquisitionStatus,
    queryFn: getAcquisitionStatus,
    refetchInterval: (query) =>
      query.state.data?.isActive ? POLL_INTERVAL_ACTIVE_MS : POLL_INTERVAL_IDLE_MS,
  });

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
    onSuccess: async () => {
      invalidateStatus();
      const info = await getExposure().catch(() => null);
      if (info) { setExposureState(info); setExposureValue(info.exposureUs); }
      toast("촬영이 시작되었습니다", "success");
    },
    onError: handleError,
  });

  const stopMutation = useMutation({
    mutationFn: stopAcquisition,
    onSuccess: () => {
      invalidateStatus();
      toast("촬영이 중지되었습니다", "info");
    },
    onError: handleError,
  });

  const triggerMutation = useMutation({
    mutationFn: triggerAndCapture,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.latestFrame });
      invalidateStatus();
      toast("프레임이 촬영되었습니다", "success");
    },
    onError: handleError,
  });

  const captureOneMutation = useMutation({
    mutationFn: () => startAcquisition(selectedProfile, triggerMode, 1),
    onSuccess: async () => {
      invalidateStatus();
      const info = await getExposure().catch(() => null);
      if (info) { setExposureState(info); setExposureValue(info.exposureUs); }
      toast("단일 프레임 촬영이 시작되었습니다", "success");
    },
    onError: handleError,
  });

  const loadExposureMutation = useMutation({
    mutationFn: getExposure,
    onSuccess: (info) => {
      setExposureState(info);
      setExposureValue(info.exposureUs);
      toast("Exposure settings loaded", "success");
    },
    onError: handleError,
  });

  const applyExposureMutation = useMutation({
    mutationFn: () => setExposure(exposureValue),
    onSuccess: (result) => {
      toast(result.message, "success");
    },
    onError: handleError,
  });

  const blackMutation = useMutation({
    mutationFn: blackCalibration,
    onSuccess: (result) => toast(result.message, "success"),
    onError: handleError,
  });

  const whiteMutation = useMutation({
    mutationFn: whiteCalibration,
    onSuccess: (result) => toast(result.message, "success"),
    onError: handleError,
  });

  const whiteBalanceMutation = useMutation({
    mutationFn: whiteBalance,
    onSuccess: (result) => toast(result.message, "success"),
    onError: handleError,
  });

  const ffcMutation = useMutation({
    mutationFn: (enable: boolean) => setFfc(enable),
    onSuccess: (result) => toast(result.message, "success"),
    onError: handleError,
  });

  // ── Computed state ──

  const busy =
    startMutation.isPending ||
    stopMutation.isPending ||
    triggerMutation.isPending ||
    captureOneMutation.isPending ||
    loadExposureMutation.isPending ||
    applyExposureMutation.isPending ||
    blackMutation.isPending ||
    whiteMutation.isPending ||
    whiteBalanceMutation.isPending ||
    ffcMutation.isPending;

  const isCalibrationAvailable =
    acquisitionStatus?.channelState === "idle" ||
    acquisitionStatus?.channelState === "active";

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

  const handleStart = () => {
    if (mode === "continuous" && continuousSubMode === "auto" && intervalMs === null) {
      toast("Please input interval time for continuous mode", "warning");
      return;
    }
    startMutation.mutate();
  };
  const handleStop = () => stopMutation.mutate();
  const handleTrigger = () => triggerMutation.mutate();
  const handleCapture = () => captureOneMutation.mutate();
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
    acquisitionStatus: acquisitionStatus ?? null,
    exposure,
    exposureValue,
    setExposureValue,
    ffcEnabled,
    busy,
    hasWarnings,
    hasErrors,
    refresh,
    throttled: false,
    handleStart,
    handleStop,
    handleTrigger,
    handleCapture,
    handleLoadExposure,
    handleApplyExposure,
    isCalibrationAvailable,
    handleBlack,
    handleWhite,
    handleWhiteBalance,
    handleLoadPreset,
    handleFfcToggle,
  };
}
