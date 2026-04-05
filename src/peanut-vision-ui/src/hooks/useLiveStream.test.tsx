import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createElement } from "react";
import type { ReactNode } from "react";
import { useLiveStream } from "./useLiveStream";
import type { AcquisitionStatus } from "../api/types";

// Mock EventSource
const mockEs = {
  addEventListener: vi.fn(),
  close: vi.fn(),
};
vi.stubGlobal("EventSource", vi.fn(function () { return mockEs; }));

vi.mock("../constants", () => ({
  API_BASE_URL: "http://localhost:5000/api",
}));

function wrapper({ children }: { children: ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return createElement(QueryClientProvider, { client: queryClient }, children);
}

describe("useLiveStream", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockEs.addEventListener.mockReset();
    mockEs.close.mockReset();
  });

  it("creates EventSource with correct URL", () => {
    renderHook(() => useLiveStream(), { wrapper });
    expect(EventSource).toHaveBeenCalledWith(
      "http://localhost:5000/api/acquisition/events"
    );
  });

  it("closes EventSource on unmount", () => {
    const { unmount } = renderHook(() => useLiveStream(), { wrapper });
    unmount();
    expect(mockEs.close).toHaveBeenCalled();
  });

  it("previewUrl is null initially", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });
    expect(result.current.previewUrl).toBeNull();
  });

  it("isActive is false initially", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });
    expect(result.current.isActive).toBe(false);
  });

  it("updates previewUrl when frame_ready event is received", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });

    const frameReadyCall = mockEs.addEventListener.mock.calls.find(
      ([event]: [string]) => event === "frame_ready"
    );
    expect(frameReadyCall).toBeDefined();
    const frameReadyHandler = frameReadyCall![1] as () => void;

    act(() => {
      frameReadyHandler();
    });

    expect(result.current.previewUrl).toMatch(/\/acquisition\/latest-frame\?_t=\d+/);
  });

  it("updates isActive when status_changed event is received", () => {
    const { result } = renderHook(() => useLiveStream(), { wrapper });

    const statusCall = mockEs.addEventListener.mock.calls.find(
      ([event]: [string]) => event === "status_changed"
    );
    expect(statusCall).toBeDefined();
    const statusHandler = statusCall![1] as (e: { data: string }) => void;

    const status: Partial<AcquisitionStatus> = { isActive: true };
    act(() => {
      statusHandler({ data: JSON.stringify(status) });
    });

    expect(result.current.isActive).toBe(true);
  });
});
