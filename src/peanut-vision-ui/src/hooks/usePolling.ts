import { useEffect, useRef, useCallback, useState } from "react";
import { POLL_INTERVAL_MS, REFRESH_THROTTLE_MS } from "../constants";

export function usePolling(callback: () => void, intervalMs = POLL_INTERVAL_MS) {
  const callbackRef = useRef(callback);
  callbackRef.current = callback;

  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const [throttled, setThrottled] = useState(false);

  const resetInterval = useCallback(() => {
    if (intervalRef.current) clearInterval(intervalRef.current);
    intervalRef.current = setInterval(() => callbackRef.current(), intervalMs);
  }, [intervalMs]);

  useEffect(() => {
    callbackRef.current();
    resetInterval();
    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, [resetInterval]);

  const refresh = useCallback(() => {
    if (throttled) return;
    callbackRef.current();
    resetInterval();
    setThrottled(true);
    setTimeout(() => setThrottled(false), REFRESH_THROTTLE_MS);
  }, [resetInterval, throttled]);

  return { refresh, throttled };
}
