import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { getLatestFrame } from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { API_BASE_URL, LIVE_PREVIEW_POLL_INTERVAL_MS } from "../constants";
import type { AcquisitionStatus } from "../api/types";

export function useLivePreview(acquisitionStatus: AcquisitionStatus | null) {
  const { data: latestFrame } = useQuery({
    queryKey: queryKeys.latestFrame,
    queryFn: getLatestFrame,
    refetchInterval:
      acquisitionStatus?.isActive === true && acquisitionStatus?.hasFrame === true
        ? LIVE_PREVIEW_POLL_INTERVAL_MS
        : false,
  });

  const previewUrl = useMemo(
    () =>
      latestFrame
        ? `${API_BASE_URL}/acquisition/latest-frame?_t=${Date.now()}`
        : null,
    [latestFrame] // Date.now() is intentionally excluded — it's not a React value, not a dep
  );

  return {
    previewUrl,
    isActive: acquisitionStatus?.isActive ?? false,
  };
}
