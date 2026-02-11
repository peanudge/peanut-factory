import { useState, useEffect, useRef } from "react";

export function useApiData<T>(fetcher: () => Promise<T>) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetcherRef = useRef(fetcher);
  fetcherRef.current = fetcher;

  useEffect(() => {
    let cancelled = false;
    fetcherRef.current().then(
      (result) => {
        if (!cancelled) {
          setData(result);
          setLoading(false);
        }
      },
      (e: unknown) => {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Failed");
          setLoading(false);
        }
      },
    );
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return { data, loading, error };
}
