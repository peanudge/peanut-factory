import { useState, useCallback } from "react";

export function useAsyncOperation() {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  const clearError = useCallback(() => setError(""), []);

  const execute = useCallback(async (fn: () => Promise<void>) => {
    setBusy(true);
    setError("");
    try {
      await fn();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setBusy(false);
    }
  }, []);

  return { busy, error, clearError, execute };
}
