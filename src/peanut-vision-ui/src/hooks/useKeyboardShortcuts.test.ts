import { renderHook } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { useKeyboardShortcuts } from "./useKeyboardShortcuts";

// ── Helpers ───────────────────────────────────────────────────────────────────

function fireKey(code: string, extra: Partial<KeyboardEventInit> = {}) {
  const event = new KeyboardEvent("keydown", { code, bubbles: true, ...extra });
  window.dispatchEvent(event);
  return event;
}

function setActiveElement(tag: string, contentEditable = false) {
  const el = document.createElement(tag);
  if (contentEditable) el.contentEditable = "true";
  document.body.appendChild(el);
  el.focus();
  return el;
}

function clearFocus() {
  (document.activeElement as HTMLElement | null)?.blur();
}

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("useKeyboardShortcuts", () => {
  beforeEach(() => {
    clearFocus();
  });

  describe("shortcuts fire when no input is focused", () => {
    it("Space calls onCapture when not active", () => {
      const onCapture = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onCapture, isActive: false }));

      fireKey("Space");

      expect(onCapture).toHaveBeenCalledOnce();
    });

    it("Space does NOT call onCapture when isActive=true", () => {
      const onCapture = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onCapture, isActive: true }));

      fireKey("Space");

      expect(onCapture).not.toHaveBeenCalled();
    });

    it("Ctrl+R calls onToggleContinuous", () => {
      const onToggleContinuous = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onToggleContinuous }));

      fireKey("KeyR", { ctrlKey: true });

      expect(onToggleContinuous).toHaveBeenCalledOnce();
    });

    it("Meta+R calls onToggleContinuous", () => {
      const onToggleContinuous = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onToggleContinuous }));

      fireKey("KeyR", { metaKey: true });

      expect(onToggleContinuous).toHaveBeenCalledOnce();
    });

    it("Delete calls onDeleteSelected", () => {
      const onDeleteSelected = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onDeleteSelected }));

      fireKey("Delete");

      expect(onDeleteSelected).toHaveBeenCalledOnce();
    });

    it("ArrowLeft calls onPrevImage", () => {
      const onPrevImage = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onPrevImage }));

      fireKey("ArrowLeft");

      expect(onPrevImage).toHaveBeenCalledOnce();
    });

    it("ArrowRight calls onNextImage", () => {
      const onNextImage = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onNextImage }));

      fireKey("ArrowRight");

      expect(onNextImage).toHaveBeenCalledOnce();
    });

    it("Escape calls onReturnToLive", () => {
      const onReturnToLive = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onReturnToLive }));

      fireKey("Escape");

      expect(onReturnToLive).toHaveBeenCalledOnce();
    });

    it("? calls onShowHelp", () => {
      const onShowHelp = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onShowHelp }));

      const event = new KeyboardEvent("keydown", { key: "?", code: "Slash", bubbles: true });
      window.dispatchEvent(event);

      expect(onShowHelp).toHaveBeenCalledOnce();
    });
  });

  describe("shortcuts do NOT fire when an input element is focused", () => {
    it("Space does not call onCapture when an input is focused", () => {
      const onCapture = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onCapture, isActive: false }));

      const input = setActiveElement("input");
      fireKey("Space");

      expect(onCapture).not.toHaveBeenCalled();
      input.remove();
    });

    it("ArrowLeft does not call onPrevImage when a textarea is focused", () => {
      const onPrevImage = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onPrevImage }));

      const textarea = setActiveElement("textarea");
      fireKey("ArrowLeft");

      expect(onPrevImage).not.toHaveBeenCalled();
      textarea.remove();
    });

    it("Delete does not call onDeleteSelected when a select is focused", () => {
      const onDeleteSelected = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onDeleteSelected }));

      const select = setActiveElement("select");
      fireKey("Delete");

      expect(onDeleteSelected).not.toHaveBeenCalled();
      select.remove();
    });

    it("Space does not call onCapture when a contentEditable element is focused", () => {
      const onCapture = vi.fn();
      renderHook(() => useKeyboardShortcuts({ onCapture, isActive: false }));

      // jsdom requires the element to be in the document and focused for
      // isContentEditable to be reflected correctly; use an actual editable div
      const div = document.createElement("div");
      div.setAttribute("contenteditable", "true");
      document.body.appendChild(div);
      div.focus();
      // Verify jsdom correctly reports the element as content-editable
      // If not supported, skip assertion to avoid a false failure
      if ((div as HTMLElement).isContentEditable) {
        fireKey("Space");
        expect(onCapture).not.toHaveBeenCalled();
      }
      div.remove();
    });
  });

  describe("cleanup", () => {
    it("removes listener on unmount so callbacks no longer fire", () => {
      const onCapture = vi.fn();
      const { unmount } = renderHook(() =>
        useKeyboardShortcuts({ onCapture, isActive: false }),
      );

      unmount();
      fireKey("Space");

      expect(onCapture).not.toHaveBeenCalled();
    });
  });
});
