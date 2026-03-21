import { useCallback, useEffect, useRef, useState } from "react";
import type { CapturedEvent } from "../api/types";

export function useCaptureLog() {
  const [events, setEvents] = useState<CapturedEvent[]>([]);
  const [selectedEventId, setSelectedEventId] = useState<string | null>(null);
  const prevHeadIdRef = useRef<string | null>(null);

  // Auto-select the newest event only when the user has nothing selected or is already
  // following the live head. Preserves manual selections when new frames arrive.
  useEffect(() => {
    const headId = events[0]?.id ?? null;
    if (!headId || headId === prevHeadIdRef.current) return;
    setSelectedEventId((sel) =>
      sel === null || sel === prevHeadIdRef.current ? headId : sel
    );
    prevHeadIdRef.current = headId;
  }, [events]);

  const addEvent = useCallback((filePath: string) => {
    const newEvent: CapturedEvent = {
      id: crypto.randomUUID(),
      filePath,
      capturedAt: new Date(),
    };
    setEvents((prev) => [newEvent, ...prev]);
  }, []);

  const deleteEvent = useCallback((id: string) => {
    setEvents((prev) => prev.filter((e) => e.id !== id));
    setSelectedEventId((prev) => (prev === id ? null : prev));
  }, []);

  const clearAll = useCallback(() => {
    setEvents([]);
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
