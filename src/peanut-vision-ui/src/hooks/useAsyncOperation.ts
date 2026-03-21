import { useState, useCallback } from "react";
import { ApiError } from "../api/client";

export function useAsyncOperation() {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [errorCode, setErrorCode] = useState("");

  const clearError = useCallback(() => {
    setError("");
    setErrorCode("");
  }, []);

  const execute = useCallback(async (fn: () => Promise<void>) => {
    setBusy(true);
    setError("");
    setErrorCode("");
    try {
      await fn();
    } catch (e) {
      if (e instanceof ApiError) {
        setError(e.message);
        setErrorCode(e.errorCode);
      } else {
        setError(e instanceof Error ? e.message : "Operation failed");
        setErrorCode("UNKNOWN_ERROR");
      }
    } finally {
      setBusy(false);
    }
  }, []);

  return { busy, error, errorCode, clearError, execute };
}
