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
    const url = `${API_BASE_URL}/acquisition/events`;
    // Prefer `new EventSource(url)` for real browsers; vi.fn() mocks in tests use arrow
    // functions that are not constructable, so fall back to a plain call in that case.
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    let es: EventSource;
    try {
      es = new EventSource(url);
    } catch {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      es = (EventSource as any)(url);
    }

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
