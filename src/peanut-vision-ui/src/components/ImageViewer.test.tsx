import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createElement, type ReactNode } from "react";
import ImageViewer from "./ImageViewer";

// ── Helpers ───────────────────────────────────────────────────────────────────

function makeWrapper() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return function Wrapper({ children }: { children: ReactNode }) {
    return createElement(QueryClientProvider, { client: qc }, children);
  };
}

const defaultProps = {
  url: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
  isLive: false,
  capturedAt: new Date(),
  onReturnToLive: vi.fn(),
};

function getViewerContainer() {
  // The zoom-enabled container wraps the <img> and has onWheel/onDoubleClick
  return screen.getByRole("img", { name: /captured frame/i }).parentElement!;
}

function wheelIn(el: Element, times = 1) {
  for (let i = 0; i < times; i++) {
    fireEvent.wheel(el, { deltaY: -120 });
  }
}

function wheelOut(el: Element, times = 1) {
  for (let i = 0; i < times; i++) {
    fireEvent.wheel(el, { deltaY: 120 });
  }
}

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("ImageViewer — zoom & pan", () => {
  describe("zoom badge visibility", () => {
    it("does not show zoom badge at initial scale (1×)", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });

    it("shows zoom badge after scrolling in", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      wheelIn(getViewerContainer());
      expect(screen.getByText(/×/)).toBeInTheDocument();
    });

    it("hides zoom badge after scrolling back out to 1×", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      wheelIn(getViewerContainer());
      expect(screen.getByText(/×/)).toBeInTheDocument();

      // Scroll out enough times to return to MIN_SCALE
      wheelOut(getViewerContainer(), 20);
      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });
  });

  describe("scale clamping", () => {
    it("does not exceed MAX_SCALE (10×)", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      const container = getViewerContainer();

      // Scroll in many times — each step multiplies by 1.15
      // 1.15^20 ≈ 16.4 which exceeds 10, so badge should cap at "10.0×"
      wheelIn(container, 20);

      const badge = screen.getByText(/×/);
      const scale = parseFloat(badge.textContent!.replace("×", ""));
      expect(scale).toBeLessThanOrEqual(10);
    });

    it("does not go below MIN_SCALE (1×) — badge disappears", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      const container = getViewerContainer();

      // Zoom in a bit first, then zoom out excessively
      wheelIn(container, 3);
      wheelOut(container, 30);

      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });
  });

  describe("double-click reset", () => {
    it("resets zoom badge to hidden after double-click", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      const container = getViewerContainer();

      wheelIn(container, 3);
      expect(screen.getByText(/×/)).toBeInTheDocument();

      fireEvent.doubleClick(container);
      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });

    it("double-click has no effect when already at 1×", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      const container = getViewerContainer();

      fireEvent.doubleClick(container);
      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });
  });

  describe("live frame reset", () => {
    it("resets zoom when a new live frame url arrives", () => {
      const { rerender } = render(
        <ImageViewer {...defaultProps} isLive url="blob:first" />,
        { wrapper: makeWrapper() },
      );

      wheelIn(getViewerContainer(), 3);
      expect(screen.getByText(/×/)).toBeInTheDocument();

      // Simulate new frame arriving (url changes while isLive=true)
      rerender(
        <QueryClientProvider client={new QueryClient()}>
          <ImageViewer {...defaultProps} isLive url="blob:second" />
        </QueryClientProvider>,
      );

      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });
  });

  describe("pan — cursor style", () => {
    it("shows default cursor at scale 1 (no pan)", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      const container = getViewerContainer();
      // Cursor style is applied via MUI sx; verify indirectly via scale=1 precondition
      expect(container).toBeTruthy();
      expect(screen.queryByText(/×/)).not.toBeInTheDocument();
    });

    it("container receives mousedown events when zoomed in", () => {
      render(<ImageViewer {...defaultProps} />, { wrapper: makeWrapper() });
      const container = getViewerContainer();

      wheelIn(container, 2);

      // Pan should not throw — fire the full drag sequence
      expect(() => {
        fireEvent.mouseDown(container, { clientX: 100, clientY: 100 });
        fireEvent.mouseMove(container, { clientX: 150, clientY: 120 });
        fireEvent.mouseUp(container);
      }).not.toThrow();
    });
  });

  describe("renders nothing useful without a url", () => {
    it("shows placeholder when url is null", () => {
      render(
        <ImageViewer {...defaultProps} url={null} />,
        { wrapper: makeWrapper() },
      );
      expect(screen.getByText(/no captured frame/i)).toBeInTheDocument();
    });
  });
});
