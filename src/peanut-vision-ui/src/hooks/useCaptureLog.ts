import { useCallback, useEffect, useRef, useState } from "react";
import type { CapturedEvent } from "../api/types";

const MAX_CAPTURES = 50;

export function useCaptureLog() {
  const [events, setEvents] = useState<CapturedEvent[]>([]);
  const [selectedEventId, setSelectedEventId] = useState<string | null>(null);
  const prevHeadIdRef = useRef<string | null>(null);
  const eventsRef = useRef<CapturedEvent[]>([]);

  useEffect(() => { eventsRef.current = events; }, [events]);

  // Revoke all remaining blob URLs on unmount
  useEffect(() => {
    return () => {
      eventsRef.current.forEach((e) => {
        if (e.objectUrl) URL.revokeObjectURL(e.objectUrl);
      });
    };
  }, []);

  // Auto-select the newest event only when the user has nothing selected or is already
  // following the live head. Preserves manual selections when new frames arrive.
  useEffect(() => {
    const headId = events[0]?.id ?? null;
    if (!headId || headId === prevHeadIdRef.current) return;
    const prevHeadId = prevHeadIdRef.current; // snapshot before mutation
    prevHeadIdRef.current = headId;
    setSelectedEventId((sel) =>
      sel === null || sel === prevHeadId ? headId : sel
    );
  }, [events]);

  const addEvent = useCallback((filePath: string, objectUrl: string | null) => {
    const newEvent: CapturedEvent = {
      id: crypto.randomUUID(),
      filePath,
      capturedAt: new Date(),
      objectUrl,
    };
    setEvents((prev) => {
      const next = [newEvent, ...prev];
      if (next.length > MAX_CAPTURES) {
        const evicted = next.slice(MAX_CAPTURES);
        evicted.forEach((e) => {
          if (e.objectUrl) URL.revokeObjectURL(e.objectUrl);
        });
        return next.slice(0, MAX_CAPTURES);
      }
      return next;
    });
  }, []);

  const deleteEvent = useCallback((id: string) => {
    setEvents((prev) => {
      const target = prev.find((e) => e.id === id);
      if (target?.objectUrl) URL.revokeObjectURL(target.objectUrl);
      return prev.filter((e) => e.id !== id);
    });
    setSelectedEventId((prev) => (prev === id ? null : prev));
  }, []);

  const clearAll = useCallback(() => {
    setEvents((prev) => {
      prev.forEach((e) => {
        if (e.objectUrl) URL.revokeObjectURL(e.objectUrl);
      });
      return [];
    });
    setSelectedEventId(null);
  }, []);

  return {
    events,
    selectedEventId,
    addEvent,
    deleteEvent,
    clearAll,
    selectEvent: setSelectedEventId,
  };
}
