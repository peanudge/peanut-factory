import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import React from "react";
import { useImageGallery } from "./useImageGallery";

vi.mock("../api/client", () => ({
  listImages: vi.fn(() => Promise.resolve({ items: [], totalCount: 0, page: 1, pageSize: 20, totalPages: 0 })),
  deleteImage: vi.fn(),
  imageFileUrl: vi.fn((id: string) => `/api/images/${id}/file`),
}));

vi.mock("../contexts/ToastContext", () => ({
  useToast: () => ({ toast: vi.fn() }),
}));

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe("useImageGallery", () => {
  beforeEach(() => vi.clearAllMocks());

  it("starts with filterDate null", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    expect(result.current.filterDate).toBeNull();
  });

  it("setFilterDate updates filterDate", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    act(() => { result.current.setFilterDate("2026-04-06"); });
    expect(result.current.filterDate).toBe("2026-04-06");
  });

  it("setFilterDate null clears the filter", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    act(() => { result.current.setFilterDate("2026-04-06"); });
    act(() => { result.current.setFilterDate(null); });
    expect(result.current.filterDate).toBeNull();
  });

  it("setFilterDate resets page to 1", () => {
    const { result } = renderHook(() => useImageGallery(), { wrapper });
    act(() => { result.current.setPage(3); });
    act(() => { result.current.setFilterDate("2026-04-06"); });
    expect(result.current.page).toBe(1);
  });
});
