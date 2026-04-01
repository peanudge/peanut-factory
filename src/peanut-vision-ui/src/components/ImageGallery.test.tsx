import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createElement } from "react";
import type { ReactNode } from "react";
import ImageGallery from "./ImageGallery";
import type { CapturedImageRecord } from "../api/types";

// ── Module mocks ──────────────────────────────────────────────────────────────

vi.mock("../api/client", () => ({
  getSessions: vi.fn().mockResolvedValue([]),
  thumbnailUrl: (id: string) => `http://localhost/images/${id}/thumbnail`,
  exportImagesZip: vi.fn().mockResolvedValue(new Blob()),
}));

vi.mock("../contexts/ToastContext", () => ({
  useToast: () => ({ toast: vi.fn() }),
}));

// ── Helpers ───────────────────────────────────────────────────────────────────

function makeImage(overrides: Partial<CapturedImageRecord> = {}): CapturedImageRecord {
  return {
    id: "img-1",
    filename: "capture_001.png",
    format: "png",
    capturedAt: "2025-01-15T10:00:00Z",
    width: 4160,
    height: 3120,
    hasThumbnail: true,
    sessionId: null,
    fileSizeBytes: 50000,
    filePath: "/captures/capture_001.png",
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

const defaultProps = {
  images: [] as CapturedImageRecord[],
  selectedId: null,
  onSelect: vi.fn(),
  onDelete: vi.fn(),
  page: 1,
  totalPages: 1,
  totalCount: 0,
  onPageChange: vi.fn(),
  filterSessionId: null,
  onFilterChange: vi.fn(),
  filterFromDate: null,
  onFromDateChange: vi.fn(),
  filterToDate: null,
  onToDateChange: vi.fn(),
  filterFormat: null,
  onFormatChange: vi.fn(),
  isLoading: false,
};

// ── Tests ─────────────────────────────────────────────────────────────────────

describe("ImageGallery", () => {
  describe("thumbnail grid", () => {
    it("renders an img element for each image that has a thumbnail", () => {
      const images = [
        makeImage({ id: "img-1", filename: "a.png", hasThumbnail: true }),
        makeImage({ id: "img-2", filename: "b.png", hasThumbnail: true }),
      ];

      render(
        <ImageGallery {...defaultProps} images={images} totalCount={2} />,
        { wrapper: makeWrapper() },
      );

      const thumbnails = screen.getAllByRole("img");
      expect(thumbnails).toHaveLength(2);
    });

    it("renders a format label placeholder for images without a thumbnail (e.g. RAW)", () => {
      const images = [
        makeImage({ id: "raw-1", filename: "shot.raw", format: "raw", hasThumbnail: false }),
      ];

      render(
        <ImageGallery {...defaultProps} images={images} totalCount={1} />,
        { wrapper: makeWrapper() },
      );

      expect(screen.getByText("raw")).toBeInTheDocument();
      expect(screen.queryByRole("img")).not.toBeInTheDocument();
    });

    it("calls onSelect when a thumbnail is clicked", async () => {
      const onSelect = vi.fn();
      const images = [makeImage({ id: "img-1" })];

      render(
        <ImageGallery {...defaultProps} images={images} totalCount={1} onSelect={onSelect} />,
        { wrapper: makeWrapper() },
      );

      // Click the thumbnail container (the grid item box)
      const imgs = screen.getAllByRole("img");
      await userEvent.click(imgs[0]);

      expect(onSelect).toHaveBeenCalledWith("img-1");
    });

    it("shows 'No captures yet' when image list is empty and not loading", () => {
      render(<ImageGallery {...defaultProps} />, { wrapper: makeWrapper() });

      expect(screen.getByText(/no captures yet/i)).toBeInTheDocument();
    });
  });

  describe("Clear filters button", () => {
    it("does NOT appear when no filters are active", () => {
      render(<ImageGallery {...defaultProps} />, { wrapper: makeWrapper() });

      expect(screen.queryByRole("button", { name: /clear filters/i })).not.toBeInTheDocument();
    });

    it("appears when filterSessionId is set", () => {
      render(
        <ImageGallery {...defaultProps} filterSessionId="sess-123" />,
        { wrapper: makeWrapper() },
      );

      expect(screen.getByRole("button", { name: /clear filters/i })).toBeInTheDocument();
    });

    it("appears when filterFormat is set", () => {
      render(
        <ImageGallery {...defaultProps} filterFormat="png" />,
        { wrapper: makeWrapper() },
      );

      expect(screen.getByRole("button", { name: /clear filters/i })).toBeInTheDocument();
    });

    it("appears when filterFromDate is set", () => {
      render(
        <ImageGallery {...defaultProps} filterFromDate="2025-01-01" />,
        { wrapper: makeWrapper() },
      );

      expect(screen.getByRole("button", { name: /clear filters/i })).toBeInTheDocument();
    });

    it("appears when filterToDate is set", () => {
      render(
        <ImageGallery {...defaultProps} filterToDate="2025-12-31" />,
        { wrapper: makeWrapper() },
      );

      expect(screen.getByRole("button", { name: /clear filters/i })).toBeInTheDocument();
    });

    it("calls all four clear callbacks when clicked", async () => {
      const onFilterChange = vi.fn();
      const onFromDateChange = vi.fn();
      const onToDateChange = vi.fn();
      const onFormatChange = vi.fn();

      render(
        <ImageGallery
          {...defaultProps}
          filterFormat="bmp"
          onFilterChange={onFilterChange}
          onFromDateChange={onFromDateChange}
          onToDateChange={onToDateChange}
          onFormatChange={onFormatChange}
        />,
        { wrapper: makeWrapper() },
      );

      await userEvent.click(screen.getByRole("button", { name: /clear filters/i }));

      expect(onFilterChange).toHaveBeenCalledWith(null);
      expect(onFromDateChange).toHaveBeenCalledWith(null);
      expect(onToDateChange).toHaveBeenCalledWith(null);
      expect(onFormatChange).toHaveBeenCalledWith(null);
    });
  });

  describe("Format dropdown options", () => {
    it("has an 'All formats' option", () => {
      render(<ImageGallery {...defaultProps} />, { wrapper: makeWrapper() });

      // MUI Select renders option text in the DOM
      expect(screen.getByText(/all formats/i)).toBeInTheDocument();
    });

    it("has PNG, BMP, and RAW options", () => {
      render(<ImageGallery {...defaultProps} />, { wrapper: makeWrapper() });

      expect(screen.getByText("PNG")).toBeInTheDocument();
      expect(screen.getByText("BMP")).toBeInTheDocument();
      expect(screen.getByText("RAW")).toBeInTheDocument();
    });
  });

  describe("loading state", () => {
    it("shows a loading spinner while isLoading is true", () => {
      render(
        <ImageGallery {...defaultProps} isLoading />,
        { wrapper: makeWrapper() },
      );

      // MUI CircularProgress renders an svg with role="progressbar"
      expect(screen.getByRole("progressbar")).toBeInTheDocument();
    });
  });
});
