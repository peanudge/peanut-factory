import { useEffect, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../api/queryKeys";
import { API_BASE_URL } from "../constants";
import type { AcquisitionStatus } from "../api/types";

export function useLiveStream() {
  const queryClient = useQueryClient();
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [isActive, setIsActive] = useState(false);

  useEffect(() => {
    const es = new EventSource(`${API_BASE_URL}/acquisition/events`);

    es.addEventListener("frame_ready", () => {
      setPreviewUrl(`${API_BASE_URL}/acquisition/latest-frame?_t=${Date.now()}`);
    });

    es.addEventListener("status_changed", (e: MessageEvent) => {
      const status = JSON.parse(e.data) as AcquisitionStatus;
      queryClient.setQueryData(queryKeys.acquisitionStatus, status);
      setIsActive(status.isActive);
    });

    return () => es.close();
  }, [queryClient]);

  return { previewUrl, isActive };
}
