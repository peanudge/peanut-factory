import { renderHook } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import type { ReactNode } from "react";
import { createElement } from "react";
import { useLivePreview } from "./useLivePreview";
import type { AcquisitionStatus } from "../api/types";

vi.mock("../api/client", () => ({
  getLatestFrame: vi.fn().mockResolvedValue(null),
}));

vi.mock("../constants", () => ({
  API_BASE_URL: "http://localhost:5000/api",
}));

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
});
