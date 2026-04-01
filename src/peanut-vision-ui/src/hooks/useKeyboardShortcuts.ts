import { useEffect } from "react";

/** Returns true if the event target is an interactive input element where
 *  typing should be captured by the element, not by global shortcuts. */
function isInputFocused(): boolean {
  const el = document.activeElement;
  if (!el) return false;
  const tag = el.tagName.toLowerCase();
  return tag === "input" || tag === "textarea" || tag === "select" || (el as HTMLElement).isContentEditable;
}

export interface KeyboardShortcutsConfig {
  /** Space → snapshot capture (when not active) */
  onCapture?: () => void;
  /** Ctrl+R → toggle continuous capture */
  onToggleContinuous?: () => void;
  /** Delete → delete selected image */
  onDeleteSelected?: () => void;
  /** ← previous image */
  onPrevImage?: () => void;
  /** → next image */
  onNextImage?: () => void;
  /** Escape → return to live view */
  onReturnToLive?: () => void;
  /** ? → show help modal */
  onShowHelp?: () => void;
  /** Whether acquisition is currently active (disables some shortcuts) */
  isActive?: boolean;
}

export function useKeyboardShortcuts({
  onCapture,
  onToggleContinuous,
  onDeleteSelected,
  onPrevImage,
  onNextImage,
  onReturnToLive,
  onShowHelp,
  isActive,
}: KeyboardShortcutsConfig) {
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (isInputFocused()) return;

      switch (true) {
        // Space → snapshot capture (only when idle)
        case e.code === "Space" && !isActive: {
          e.preventDefault();
          onCapture?.();
          break;
        }
        // Ctrl+R → toggle continuous
        case e.code === "KeyR" && (e.ctrlKey || e.metaKey): {
          e.preventDefault();
          onToggleContinuous?.();
          break;
        }
        // Delete → delete selected image
        case e.code === "Delete": {
          e.preventDefault();
          onDeleteSelected?.();
          break;
        }
        // ArrowLeft → previous image
        case e.code === "ArrowLeft": {
          e.preventDefault();
          onPrevImage?.();
          break;
        }
        // ArrowRight → next image
        case e.code === "ArrowRight": {
          e.preventDefault();
          onNextImage?.();
          break;
        }
        // Escape → return to live
        case e.code === "Escape": {
          e.preventDefault();
          onReturnToLive?.();
          break;
        }
        // ? → show help
        case e.key === "?" && !e.ctrlKey && !e.metaKey: {
          e.preventDefault();
          onShowHelp?.();
          break;
        }
      }
    };

    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [onCapture, onToggleContinuous, onDeleteSelected, onPrevImage, onNextImage, onReturnToLive, onShowHelp, isActive]);
}
