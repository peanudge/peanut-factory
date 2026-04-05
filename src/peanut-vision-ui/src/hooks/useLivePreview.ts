import { useState, useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { getLatestFrame } from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { API_BASE_URL } from "../constants";
import type { AcquisitionStatus } from "../api/types";

export function useLivePreview(acquisitionStatus: AcquisitionStatus | null) {
  const [previewTimestamp, setPreviewTimestamp] = useState(0);

  const { data: latestFrame } = useQuery({
    queryKey: queryKeys.latestFrame,
    queryFn: getLatestFrame,
    refetchInterval:
      acquisitionStatus?.isActive && acquisitionStatus?.hasFrame ? 1000 : false,
  });

  useEffect(() => {
    if (!latestFrame) return;
    setPreviewTimestamp(Date.now());
  }, [latestFrame]);

  const previewUrl =
    previewTimestamp > 0
      ? `${API_BASE_URL}/acquisition/latest-frame?_t=${previewTimestamp}`
      : null;

  return {
    previewUrl,
    isActive: acquisitionStatus?.isActive ?? false,
  };
}
