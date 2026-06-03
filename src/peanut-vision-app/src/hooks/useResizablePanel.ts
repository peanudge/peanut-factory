import { useCallback, useRef } from "react";

/** Options for configuring a horizontally resizable panel. */
interface ResizablePanelOptions {
  defaultWidth: number;
  min: number;
  max: number;
  /** "right" = panel is on the right (drag handle on left edge); "left" = panel is on the left (drag handle on right edge). Default: "right" */
  direction?: "left" | "right";
}

/** Encapsulates drag-resize logic for a panel, using refs to avoid re-renders during dragging. */
export function useResizablePanel(options: ResizablePanelOptions) {
  const { defaultWidth, min, max, direction = "right" } = options;

  const panelRef = useRef<HTMLDivElement>(null);
  const panelWidth = useRef(defaultWidth);
  const isDragging = useRef(false);
  const dragStartX = useRef(0);
  const dragStartWidth = useRef(0);

  const onResizerMouseDown = useCallback((event: React.MouseEvent) => {
    event.preventDefault();
    isDragging.current = true;
    dragStartX.current = event.clientX;
    dragStartWidth.current = panelWidth.current;
    document.body.style.cursor = "col-resize";
    document.body.style.userSelect = "none";

    const onMouseMove = (moveEvent: MouseEvent) => {
      if (!isDragging.current) return;
      const delta = direction === "left"
        ? moveEvent.clientX - dragStartX.current
        : dragStartX.current - moveEvent.clientX;
      const nextWidth = Math.min(max, Math.max(min, dragStartWidth.current + delta));
      panelWidth.current = nextWidth;
      if (panelRef.current) {
        panelRef.current.style.width = `${nextWidth}px`;
      }
    };

    const onMouseUp = () => {
      isDragging.current = false;
      document.body.style.cursor = "";
      document.body.style.userSelect = "";
      document.removeEventListener("mousemove", onMouseMove);
      document.removeEventListener("mouseup", onMouseUp);
    };

    document.addEventListener("mousemove", onMouseMove);
    document.addEventListener("mouseup", onMouseUp);
  }, [min, max, direction]);

  return { panelRef, onResizerMouseDown, defaultWidth };
}
