import { renderHook, act, waitFor } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createElement } from "react";
import type { ReactNode } from "react";

// ── Module mocks ──────────────────────────────────────────────────────────────

vi.mock("../api/client", () => ({
  listImages: vi.fn(),
  deleteImage: vi.fn(),
  imageFileUrl: (id: string) => `http://localhost/images/${id}/file`,
}));

vi.mock("../contexts/ToastContext", () => ({
  useToast: () => ({ toast: vi.fn() }),
}));

import { listImages } from "../api/client";
import { useImageGallery } from "./useImageGallery";
import type { ImagePage } from "../api/types";

// ── Helpers ───────────────────────────────────────────────────────────────────

function makeImagePage(overrides: Partial<ImagePage> = {}): ImagePage {
  return {
    items: [],
    totalCount: 0,
    totalPages: 1,
    page: 1,
    pageSize: 20,
    ...overrides,
  };
}

function makeWrapper() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return function Wrapper({ children }: { children: ReactNode }) {
    return createElement(QueryClientProvider, { client: queryClient }, children);
  };
}

// ── Tests ─────────────────────────────────────────────────────────────────────

beforeEach(() => {
  vi.clearAllMocks();
  vi.mocked(listImages).mockResolvedValue(makeImagePage());
});

describe("useImageGallery", () => {
  describe("initial state", () => {
    it("starts on page 1 with no filters", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      expect(result.current.page).toBe(1);
      expect(result.current.filterSessionId).toBeNull();
      expect(result.current.filterFromDate).toBeNull();
      expect(result.current.filterToDate).toBeNull();
      expect(result.current.filterFormat).toBeNull();
    });

    it("returns empty images and zero counts before data loads", () => {
      vi.mocked(listImages).mockReturnValue(new Promise(() => {})); // never resolves
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      expect(result.current.images).toEqual([]);
      expect(result.current.totalCount).toBe(0);
      expect(result.current.totalPages).toBe(0);
    });

    it("calls listImages with page=1 on mount", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      expect(listImages).toHaveBeenCalledWith(
        expect.objectContaining({ page: 1 }),
      );
    });
  });

  describe("filter resets page to 1", () => {
    it("resets page to 1 when filterSessionId changes", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      // advance to page 2
      act(() => result.current.setPage(2));
      expect(result.current.page).toBe(2);

      // change session filter — page should reset
      act(() => result.current.setFilterSessionId("session-abc"));
      expect(result.current.page).toBe(1);
    });

    it("resets page to 1 when filterFormat changes", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      act(() => result.current.setPage(3));
      expect(result.current.page).toBe(3);

      act(() => result.current.setFilterFormat("png"));
      expect(result.current.page).toBe(1);
    });

    it("resets page to 1 when filterFromDate changes", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      act(() => result.current.setPage(2));
      act(() => result.current.setFilterFromDate("2025-01-01"));
      expect(result.current.page).toBe(1);
    });

    it("resets page to 1 when filterToDate changes", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      act(() => result.current.setPage(2));
      act(() => result.current.setFilterToDate("2025-12-31"));
      expect(result.current.page).toBe(1);
    });
  });

  describe("listImages is called with active filters", () => {
    it("passes sessionId to listImages when filter is set", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      act(() => result.current.setFilterSessionId("sess-xyz"));

      await waitFor(() =>
        expect(listImages).toHaveBeenCalledWith(
          expect.objectContaining({ sessionId: "sess-xyz" }),
        ),
      );
    });

    it("passes format to listImages when format filter is set", async () => {
      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.isLoading).toBe(false));

      act(() => result.current.setFilterFormat("bmp"));

      await waitFor(() =>
        expect(listImages).toHaveBeenCalledWith(
          expect.objectContaining({ format: "bmp" }),
        ),
      );
    });
  });

  describe("selectedId auto-select", () => {
    it("auto-selects the first image when data arrives and nothing is selected", async () => {
      vi.mocked(listImages).mockResolvedValue(
        makeImagePage({
          items: [
            {
              id: "img-1",
              filename: "a.png",
              format: "png",
              capturedAt: new Date().toISOString(),
              width: 100,
              height: 100,
              hasThumbnail: false,
              sessionId: null,
              fileSizeBytes: 1000,
              filePath: "/captures/a.png",
            },
          ],
          totalCount: 1,
        }),
      );

      const { result } = renderHook(() => useImageGallery(), {
        wrapper: makeWrapper(),
      });

      await waitFor(() => expect(result.current.images).toHaveLength(1));

      expect(result.current.selectedId).toBe("img-1");
    });
  });
});
