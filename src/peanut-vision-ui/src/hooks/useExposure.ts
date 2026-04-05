import { useEffect, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import type { ExposureInfo } from "../api/types";
import { getExposure, setExposure, ApiError } from "../api/client";
import { useToast } from "../contexts/ToastContext";

interface UseExposureParams {
  onStartSuccess?: () => void;
}

export function useExposure({ onStartSuccess: _onStartSuccess }: UseExposureParams = {}) {
  const { toast } = useToast();

  const [exposure, setExposureState] = useState<ExposureInfo | null>(null);
  const [exposureValue, setExposureValue] = useState(1000);

  const handleError = (e: unknown) => {
    toast(e instanceof ApiError ? e.message : e instanceof Error ? e.message : "Operation failed", "error");
  };

  useEffect(() => {
    getExposure()
      .then((info) => { setExposureState(info); setExposureValue(info.exposureUs); })
      .catch(() => {});
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

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
      setExposureState(result);
      setExposureValue(result.exposureUs);
      toast("노출이 적용되었습니다", "success");
    },
    onError: handleError,
  });

  const handleLoadExposure = () => loadExposureMutation.mutate();
  const handleApplyExposure = () => applyExposureMutation.mutate();

  const busy = loadExposureMutation.isPending || applyExposureMutation.isPending;

  return {
    exposure,
    exposureValue,
    setExposureValue,
    busy,
    handleLoadExposure,
    handleApplyExposure,
  };
}
