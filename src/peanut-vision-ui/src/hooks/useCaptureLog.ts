import { useCallback, useState } from "react";
import type { CapturedEvent } from "../api/types";

export function useCaptureLog() {
  const [events, setEvents] = useState<CapturedEvent[]>([]);
  const [selectedEventId, setSelectedEventId] = useState<string | null>(null);

  const addEvent = useCallback((filePath: string) => {
    const newEvent: CapturedEvent = {
      id: crypto.randomUUID(),
      filePath,
      capturedAt: new Date(),
    };
    setEvents((prev) => {
      // Auto-select only when no event is selected or the newest event is currently selected
      setSelectedEventId((prevSelected) =>
        prevSelected === null || prevSelected === prev[0]?.id ? newEvent.id : prevSelected
      );
      return [newEvent, ...prev];
    });
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
