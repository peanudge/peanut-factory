import { useCallback, useEffect, useRef, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type {
  AcquisitionMode,
  AcquisitionPreset,
  AcquisitionStatus,
  ContinuousSubMode,
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
  ApiError,
} from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { useToast } from "../contexts/ToastContext";
import { API_BASE_URL, DEFAULT_CONTINUOUS_INTERVAL_MS, POLL_INTERVAL_ACTIVE_MS, POLL_INTERVAL_IDLE_MS } from "../constants";

interface UseCaptureParams {
  onImageCaptured: (filePath: string, objectUrl: string | null) => void;
}

export function useCapture({ onImageCaptured }: UseCaptureParams) {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  const [selectedProfile, setSelectedProfile] = useState("");
  const [mode, setMode] = useState<AcquisitionMode>("single");
  const [continuousSubMode, setContinuousSubMode] = useState<ContinuousSubMode>("auto");
  const [triggerMode, setTriggerMode] = useState<TriggerModeOption>("soft");
  const [frameCount, setFrameCount] = useState<number | null>(null);
  const [intervalMs, setIntervalMs] = useState<number | null>(null);
  const [previewTimestamp, setPreviewTimestamp] = useState(0);
  const lastCapturedPathRef = useRef<string | null>(null);

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

  const { data: acquisitionStatus } = useQuery<AcquisitionStatus>({
    queryKey: queryKeys.acquisitionStatus,
    queryFn: getAcquisitionStatus,
    refetchInterval: (query) =>
      query.state.data?.isActive ? POLL_INTERVAL_ACTIVE_MS : POLL_INTERVAL_IDLE_MS,
  });

  const { data: latestFrame } = useQuery({
    queryKey: queryKeys.latestFrame,
    queryFn: getLatestFrame,
    refetchInterval: acquisitionStatus?.isActive && acquisitionStatus?.hasFrame
      ? Math.max(Math.floor((intervalMs ?? POLL_INTERVAL_ACTIVE_MS) / 2), 200)
      : false,
  });

  useEffect(() => {
    if (!latestFrame) return;
    setPreviewTimestamp(Date.now());
    if (latestFrame.savedPath && latestFrame.savedPath !== lastCapturedPathRef.current) {
      lastCapturedPathRef.current = latestFrame.savedPath;
      const objectUrl = latestFrame.blob ? URL.createObjectURL(latestFrame.blob) : null;
      onImageCaptured(latestFrame.savedPath, objectUrl);
    }
  }, [latestFrame, onImageCaptured]);

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
    onSuccess: (result) => {
      setPreviewTimestamp(Date.now());
      const objectUrl = result.blob ? URL.createObjectURL(result.blob) : null;
      if (result.savedPath) {
        onImageCaptured(result.savedPath, objectUrl);
      } else if (objectUrl) {
        URL.revokeObjectURL(objectUrl);
      }
      invalidateStatus();
      toast("프레임이 촬영되었습니다", "success");
    },
    onError: handleError,
  });

  const snapshotMutation = useMutation({
    mutationFn: () => snapshot(selectedProfile),
    onSuccess: (result) => {
      setPreviewTimestamp(Date.now());
      const objectUrl = result.blob ? URL.createObjectURL(result.blob) : null;
      if (result.savedPath) {
        onImageCaptured(result.savedPath, objectUrl);
      } else if (objectUrl) {
        URL.revokeObjectURL(objectUrl);
      }
      invalidateStatus();
      toast("스냅샷이 촬영되었습니다", "success");
    },
    onError: handleError,
  });

  // ── Computed state ──

  const busy =
    startMutation.isPending ||
    stopMutation.isPending ||
    triggerMutation.isPending ||
    snapshotMutation.isPending;

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
  const handleCapture = () => snapshotMutation.mutate();

  const handleLoadPreset = useCallback((preset: AcquisitionPreset) => {
    setSelectedProfile(preset.profileId);
    setTriggerMode(preset.triggerMode ?? "soft");
    setFrameCount(preset.frameCount ?? null);
    setIntervalMs(preset.intervalMs ?? null);
    if (preset.frameCount != null || preset.intervalMs != null) {
      setMode("continuous");
    }
  }, []);

  const handleSetMode = useCallback(
    (next: AcquisitionMode) => {
      setMode(next);
      if (next === "continuous") {
        setIntervalMs((prev) => prev ?? DEFAULT_CONTINUOUS_INTERVAL_MS);
      }
    },
    []
  );

  const previewUrl = previewTimestamp > 0
    ? `${API_BASE_URL}/cameras/cam-1/latest-frame?_t=${previewTimestamp}`
    : null;

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
    busy,
    hasWarnings,
    hasErrors,
    refresh,
    previewUrl,
    throttled: false,
    handleStart,
    handleStop,
    handleTrigger,
    handleCapture,
    handleLoadPreset,
  };
}
