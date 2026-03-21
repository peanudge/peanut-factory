import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { useCaptureLog } from "./useCaptureLog";

// ── Globals mock ──────────────────────────────────────────────────────────────

const revokeObjectURL = vi.fn();
const createObjectURL = vi.fn((blob: Blob) => `blob:mock/${(blob as unknown as { name?: string }).name ?? "img"}`);

beforeEach(() => {
  vi.clearAllMocks();
  Object.defineProperty(globalThis, "URL", {
    value: { createObjectURL, revokeObjectURL },
    writable: true,
  });
});

// ── Helpers ───────────────────────────────────────────────────────────────────

function addN(result: ReturnType<typeof renderHook<ReturnType<typeof useCaptureLog>, unknown>>["result"], n: number) {
  for (let i = 0; i < n; i++) {
    act(() => result.current.addEvent(`/path/img${i}.png`, `blob:mock/img${i}`));
  }
}

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("useCaptureLog", () => {
  describe("addEvent", () => {
    it("prepends new events to the list", () => {
      const { result } = renderHook(() => useCaptureLog());

      act(() => result.current.addEvent("/a.png", "blob:a"));
      act(() => result.current.addEvent("/b.png", "blob:b"));

      expect(result.current.events[0].filePath).toBe("/b.png");
      expect(result.current.events[1].filePath).toBe("/a.png");
      expect(result.current.events).toHaveLength(2);
    });

    it("stores objectUrl on the event", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/img.png", "blob:mock/img"));
      expect(result.current.events[0].objectUrl).toBe("blob:mock/img");
    });

    it("stores null objectUrl when passed null", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/img.png", null));
      expect(result.current.events[0].objectUrl).toBeNull();
    });

    it("sets capturedAt close to now", () => {
      const before = new Date();
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/img.png", null));
      const after = new Date();
      expect(result.current.events[0].capturedAt.getTime()).toBeGreaterThanOrEqual(before.getTime());
      expect(result.current.events[0].capturedAt.getTime()).toBeLessThanOrEqual(after.getTime());
    });
  });

  describe("FIFO eviction at MAX_CAPTURES=50", () => {
    it("does not evict when at exactly 50 items", () => {
      const { result } = renderHook(() => useCaptureLog());
      addN(result, 50);
      expect(result.current.events).toHaveLength(50);
      expect(revokeObjectURL).not.toHaveBeenCalled();
    });

    it("evicts the oldest event when 51st is added", () => {
      const { result } = renderHook(() => useCaptureLog());
      addN(result, 50);
      // The oldest item (added first) is at the tail
      const oldestUrl = result.current.events[49].objectUrl;

      act(() => result.current.addEvent("/new.png", "blob:mock/new"));

      expect(result.current.events).toHaveLength(50);
      expect(revokeObjectURL).toHaveBeenCalledWith(oldestUrl);
      expect(result.current.events[0].filePath).toBe("/new.png");
    });

    it("does not revoke null objectUrls during eviction", () => {
      const { result } = renderHook(() => useCaptureLog());
      // Add 50 events with null objectUrl
      for (let i = 0; i < 50; i++) {
        act(() => result.current.addEvent(`/img${i}.png`, null));
      }
      act(() => result.current.addEvent("/new.png", "blob:mock/new"));
      // revokeObjectURL should never be called for null URLs
      expect(revokeObjectURL).not.toHaveBeenCalled();
    });
  });

  describe("deleteEvent", () => {
    it("removes the event by id", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      const id = result.current.events[0].id;

      act(() => result.current.deleteEvent(id));

      expect(result.current.events).toHaveLength(0);
    });

    it("calls revokeObjectURL for the deleted event", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      const { id, objectUrl } = result.current.events[0];

      act(() => result.current.deleteEvent(id));

      expect(revokeObjectURL).toHaveBeenCalledWith(objectUrl);
    });

    it("does not call revokeObjectURL when objectUrl is null", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", null));
      const id = result.current.events[0].id;

      act(() => result.current.deleteEvent(id));

      expect(revokeObjectURL).not.toHaveBeenCalled();
    });

    it("clears selectedEventId when selected event is deleted", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      const id = result.current.events[0].id;
      // Auto-select fires via useEffect; manually confirm
      act(() => result.current.selectEvent(id));
      expect(result.current.selectedEventId).toBe(id);

      act(() => result.current.deleteEvent(id));

      expect(result.current.selectedEventId).toBeNull();
    });

    it("preserves selectedEventId when a different event is deleted", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      act(() => result.current.addEvent("/b.png", "blob:b"));
      const [bId, aId] = [result.current.events[0].id, result.current.events[1].id];
      act(() => result.current.selectEvent(bId));

      act(() => result.current.deleteEvent(aId)); // delete a, keep b selected

      expect(result.current.selectedEventId).toBe(bId);
    });
  });

  describe("clearAll", () => {
    it("removes all events", () => {
      const { result } = renderHook(() => useCaptureLog());
      addN(result, 5);
      act(() => result.current.clearAll());
      expect(result.current.events).toHaveLength(0);
    });

    it("revokes all objectUrls", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      act(() => result.current.addEvent("/b.png", "blob:b"));
      act(() => result.current.addEvent("/c.png", null)); // null should be skipped

      act(() => result.current.clearAll());

      expect(revokeObjectURL).toHaveBeenCalledTimes(2);
      expect(revokeObjectURL).toHaveBeenCalledWith("blob:a");
      expect(revokeObjectURL).toHaveBeenCalledWith("blob:b");
    });

    it("resets selectedEventId to null", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      act(() => result.current.selectEvent(result.current.events[0].id));
      expect(result.current.selectedEventId).not.toBeNull();

      act(() => result.current.clearAll());

      expect(result.current.selectedEventId).toBeNull();
    });
  });

  describe("auto-select behaviour", () => {
    it("auto-selects the first event added", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      expect(result.current.selectedEventId).toBe(result.current.events[0].id);
    });

    it("auto-advances selection to the newest event when following live head", async () => {
      const { result } = renderHook(() => useCaptureLog());
      await act(async () => { result.current.addEvent("/a.png", "blob:a"); });
      const firstId = result.current.events[0].id;
      expect(result.current.selectedEventId).toBe(firstId);

      await act(async () => { result.current.addEvent("/b.png", "blob:b"); });
      // Was following head (a) → should advance to b
      expect(result.current.selectedEventId).toBe(result.current.events[0].id);
    });

    it("preserves manual selection when a new event arrives", () => {
      const { result } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      act(() => result.current.addEvent("/b.png", "blob:b"));
      // Manually select the older item (b is at index 1 now — it was added first)
      const olderId = result.current.events[1].id;
      act(() => result.current.selectEvent(olderId));
      expect(result.current.selectedEventId).toBe(olderId);

      // New event arrives
      act(() => result.current.addEvent("/c.png", "blob:c"));
      // Manual selection must be preserved
      expect(result.current.selectedEventId).toBe(olderId);
    });
  });

  describe("unmount cleanup", () => {
    it("revokes all remaining objectUrls on unmount", () => {
      const { result, unmount } = renderHook(() => useCaptureLog());
      act(() => result.current.addEvent("/a.png", "blob:a"));
      act(() => result.current.addEvent("/b.png", "blob:b"));
      act(() => result.current.addEvent("/c.png", null));

      unmount();

      expect(revokeObjectURL).toHaveBeenCalledTimes(2);
      expect(revokeObjectURL).toHaveBeenCalledWith("blob:a");
      expect(revokeObjectURL).toHaveBeenCalledWith("blob:b");
    });
  });
});
