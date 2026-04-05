import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import type { ReactNode } from "react";
import { createElement } from "react";
import { useLivePreview } from "./useLivePreview";
import type { AcquisitionStatus } from "../api/types";
import { getLatestFrame } from "../api/client";

vi.mock("../api/client", () => ({
  getLatestFrame: vi.fn().mockResolvedValue(null),
}));

vi.mock("../constants", async (importOriginal) => {
  const actual = await importOriginal<typeof import("../constants")>();
  return {
    ...actual,
    API_BASE_URL: "http://localhost:5000/api",
  };
});

function wrapper({ children }: { children: ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return createElement(QueryClientProvider, { client: queryClient }, children);
}

const inactive: AcquisitionStatus = { isActive: false };
const active: AcquisitionStatus = { isActive: true, hasFrame: true };

describe("useLivePreview", () => {
  beforeEach(() => vi.clearAllMocks());

  it("isActive is false when acquisitionStatus is null", () => {
    const { result } = renderHook(() => useLivePreview(null), { wrapper });
    expect(result.current.isActive).toBe(false);
  });

  it("isActive is false when acquisitionStatus.isActive is false", () => {
    const { result } = renderHook(() => useLivePreview(inactive), { wrapper });
    expect(result.current.isActive).toBe(false);
  });

  it("isActive is true when acquisitionStatus.isActive is true", () => {
    const { result } = renderHook(() => useLivePreview(active), { wrapper });
    expect(result.current.isActive).toBe(true);
  });

  it("previewUrl is null before any frame is received", () => {
    const { result } = renderHook(() => useLivePreview(null), { wrapper });
    expect(result.current.previewUrl).toBeNull();
  });

  it("previewUrl contains API endpoint with timestamp when frame is received", async () => {
    const mockGetLatestFrame = vi.mocked(getLatestFrame);
    mockGetLatestFrame.mockResolvedValue({ blob: new Blob(), savedPath: "/capture/img.png" });

    const { result } = renderHook(() => useLivePreview(active), { wrapper });

    await act(async () => {
      await new Promise((r) => setTimeout(r, 50));
    });

    expect(result.current.previewUrl).toMatch(
      /^http:\/\/localhost:5000\/api\/acquisition\/latest-frame\?_t=\d+$/
    );
  });
});
