import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import ErrorBoundary from "./ErrorBoundary";

// ── Helpers ───────────────────────────────────────────────────────────────────

/** A component that throws when `shouldThrow` is true. */
function Bomb({ shouldThrow }: { shouldThrow: boolean }) {
  if (shouldThrow) throw new Error("boom");
  return <span>all good</span>;
}

// Silence React's error output for expected boundary catches
beforeEach(() => {
  vi.spyOn(console, "error").mockImplementation(() => {});
});

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("ErrorBoundary", () => {
  describe("happy path — no error", () => {
    it("renders children normally when no error is thrown", () => {
      render(
        <ErrorBoundary>
          <span>hello world</span>
        </ErrorBoundary>,
      );

      expect(screen.getByText("hello world")).toBeTruthy();
    });

    it("does not render the error UI when no error is thrown", () => {
      render(
        <ErrorBoundary>
          <span>ok</span>
        </ErrorBoundary>,
      );

      expect(screen.queryByText(/failed to load/i)).toBeNull();
      expect(screen.queryByRole("button", { name: /retry/i })).toBeNull();
    });
  });

  describe("error state — child throws", () => {
    it("shows fallback UI when a child throws", () => {
      render(
        <ErrorBoundary>
          <Bomb shouldThrow />
        </ErrorBoundary>,
      );

      expect(screen.getByText(/something went wrong/i)).toBeTruthy();
    });

    it("shows the error message in the fallback", () => {
      render(
        <ErrorBoundary>
          <Bomb shouldThrow />
        </ErrorBoundary>,
      );

      expect(screen.getByText("boom")).toBeTruthy();
    });

    it("shows the label in the title when a label prop is provided", () => {
      render(
        <ErrorBoundary label="Image Gallery">
          <Bomb shouldThrow />
        </ErrorBoundary>,
      );

      expect(screen.getByText("Image Gallery failed to load")).toBeTruthy();
    });

    it("shows the Retry button in the fallback", () => {
      render(
        <ErrorBoundary>
          <Bomb shouldThrow />
        </ErrorBoundary>,
      );

      expect(screen.getByRole("button", { name: /retry/i })).toBeTruthy();
    });

    it("does not render the children when in error state", () => {
      render(
        <ErrorBoundary>
          <Bomb shouldThrow />
        </ErrorBoundary>,
      );

      expect(screen.queryByText("all good")).toBeNull();
    });
  });

  describe("recovery — Retry button resets state", () => {
    it("returns to normal rendering after clicking Retry when the child no longer throws", () => {
      const { rerender } = render(
        <ErrorBoundary>
          <Bomb shouldThrow />
        </ErrorBoundary>,
      );

      // Error boundary is now showing fallback
      expect(screen.getByRole("button", { name: /retry/i })).toBeTruthy();

      // Click retry to reset boundary state
      fireEvent.click(screen.getByRole("button", { name: /retry/i }));

      // Re-render with a non-throwing child
      rerender(
        <ErrorBoundary>
          <Bomb shouldThrow={false} />
        </ErrorBoundary>,
      );

      expect(screen.getByText("all good")).toBeTruthy();
      expect(screen.queryByRole("button", { name: /retry/i })).toBeNull();
    });
  });
});
