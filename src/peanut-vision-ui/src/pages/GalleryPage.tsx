import { useCallback, useEffect, useRef, useState } from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import CircularProgress from "@mui/material/CircularProgress";
import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import Divider from "@mui/material/Divider";
import IconButton from "@mui/material/IconButton";
import MenuItem from "@mui/material/MenuItem";
import Pagination from "@mui/material/Pagination";
import Select from "@mui/material/Select";
import Toolbar from "@mui/material/Toolbar";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import ArrowBackIosNewIcon from "@mui/icons-material/ArrowBackIosNew";
import ArrowForwardIosIcon from "@mui/icons-material/ArrowForwardIos";
import CheckBoxOutlineBlankIcon from "@mui/icons-material/CheckBoxOutlineBlank";
import CloseIcon from "@mui/icons-material/Close";
import DownloadIcon from "@mui/icons-material/Download";
import DeleteIcon from "@mui/icons-material/Delete";
import FilterAltOffIcon from "@mui/icons-material/FilterAltOff";

import HistogramDisplay from "../components/HistogramDisplay";
import AnnotationPanel from "../components/AnnotationPanel";
import { useImageGallery } from "../hooks/useImageGallery";
import { exportImagesZip, imageFileUrl, thumbnailUrl } from "../api/client";
import type { CapturedImageRecord } from "../api/types";
import { useToast } from "../contexts/ToastContext";

// ─── Props ──────────────────────────────────────────────────────────────────

interface GalleryPageProps {
  onBack: () => void;
}

// ─── ImageDetailOverlay ──────────────────────────────────────────────────────

interface OverlayProps {
  image: CapturedImageRecord;
  images: CapturedImageRecord[];
  onClose: () => void;
  onDelete: (id: string) => void;
  onNavigate: (id: string) => void;
}

function ImageDetailOverlay({ image, images, onClose, onDelete, onNavigate }: OverlayProps) {
  const currentIndex = images.findIndex((img) => img.id === image.id);

  const goPrev = useCallback(() => {
    if (currentIndex > 0) onNavigate(images[currentIndex - 1].id);
  }, [currentIndex, images, onNavigate]);

  const goNext = useCallback(() => {
    if (currentIndex < images.length - 1) onNavigate(images[currentIndex + 1].id);
  }, [currentIndex, images, onNavigate]);

  // Keyboard navigation
  useEffect(() => {
    const handleKey = (e: KeyboardEvent) => {
      if (e.key === "ArrowLeft") goPrev();
      else if (e.key === "ArrowRight") goNext();
      else if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", handleKey);
    return () => window.removeEventListener("keydown", handleKey);
  }, [goPrev, goNext, onClose]);

  const capturedAt = new Date(image.capturedAt);
  const formattedDate = capturedAt.toLocaleString();
  const fileUrl = imageFileUrl(image.id);

  const handleDelete = () => {
    onDelete(image.id);
    onClose();
  };

  return (
    <Dialog
      open
      onClose={onClose}
      fullScreen
      PaperProps={{ sx: { bgcolor: "background.default" } }}
    >
      {/* Dialog Header */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 1,
          px: 2,
          py: 1,
          borderBottom: "1px solid",
          borderColor: "divider",
          bgcolor: "background.paper",
          flexShrink: 0,
        }}
      >
        <IconButton onClick={onClose} size="small">
          <CloseIcon />
        </IconButton>
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Typography variant="subtitle2" noWrap sx={{ fontWeight: 600 }}>
            {image.filename}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {formattedDate} &nbsp;·&nbsp; {image.width}×{image.height} &nbsp;·&nbsp; {image.format.toUpperCase()}
          </Typography>
        </Box>
        <Box sx={{ display: "flex", gap: 0.5, alignItems: "center" }}>
          <Tooltip title="Previous (←)">
            <span>
              <IconButton
                size="small"
                onClick={goPrev}
                disabled={currentIndex <= 0}
              >
                <ArrowBackIosNewIcon fontSize="small" />
              </IconButton>
            </span>
          </Tooltip>
          <Typography variant="caption" color="text.secondary" sx={{ minWidth: 40, textAlign: "center" }}>
            {currentIndex + 1} / {images.length}
          </Typography>
          <Tooltip title="Next (→)">
            <span>
              <IconButton
                size="small"
                onClick={goNext}
                disabled={currentIndex >= images.length - 1}
              >
                <ArrowForwardIosIcon fontSize="small" />
              </IconButton>
            </span>
          </Tooltip>
        </Box>
      </Box>

      {/* Dialog Body */}
      <DialogContent sx={{ p: 0, display: "flex", overflow: "hidden" }}>
        {/* Left: image viewer (60%) */}
        <Box
          sx={{
            flex: "0 0 60%",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            bgcolor: "background.default",
            borderRight: "1px solid",
            borderColor: "divider",
            overflow: "hidden",
            position: "relative",
          }}
        >
          <ZoomableImage url={fileUrl} alt={image.filename} />
        </Box>

        {/* Right: metadata panel (40%) */}
        <Box
          sx={{
            flex: "0 0 40%",
            display: "flex",
            flexDirection: "column",
            overflow: "auto",
            p: 2,
            gap: 2,
          }}
        >
          {/* Histogram */}
          <Box>
            <Typography variant="caption" color="text.secondary" sx={{ display: "block", mb: 0.5, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              Histogram
            </Typography>
            <HistogramDisplay imageId={image.id} />
          </Box>

          <Divider />

          {/* Annotations */}
          <Box>
            <Typography variant="caption" color="text.secondary" sx={{ display: "block", mb: 0.5, fontWeight: 600, textTransform: "uppercase", letterSpacing: "0.05em" }}>
              Tags &amp; Notes
            </Typography>
            <Box sx={{ border: "1px solid", borderColor: "divider", borderRadius: 1 }}>
              <AnnotationPanel
                imageId={image.id}
                initialTags={image.tags}
                initialNotes={image.notes}
              />
            </Box>
          </Box>

          <Divider />

          {/* Actions */}
          <Box sx={{ display: "flex", gap: 1 }}>
            <Button
              variant="outlined"
              startIcon={<DownloadIcon />}
              onClick={() => window.open(fileUrl, "_blank")}
              size="small"
            >
              Download
            </Button>
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteIcon />}
              onClick={handleDelete}
              size="small"
            >
              Delete
            </Button>
          </Box>
        </Box>
      </DialogContent>
    </Dialog>
  );
}

// ─── ZoomableImage ───────────────────────────────────────────────────────────

interface ZoomableImageProps {
  url: string;
  alt: string;
}

function ZoomableImage({ url, alt }: ZoomableImageProps) {
  const [scale, setScale] = useState(1);
  const [translate, setTranslate] = useState({ x: 0, y: 0 });
  const isDragging = useRef(false);
  const dragStart = useRef({ x: 0, y: 0 });
  const translateAtDragStart = useRef({ x: 0, y: 0 });

  // Reset when url changes
  useEffect(() => {
    setScale(1);
    setTranslate({ x: 0, y: 0 });
  }, [url]);

  const handleWheel = useCallback((e: React.WheelEvent) => {
    e.preventDefault();
    const factor = e.deltaY < 0 ? 1.15 : 1 / 1.15;
    setScale((prev) => {
      const next = Math.min(Math.max(prev * factor, 1), 10);
      if (next === 1) setTranslate({ x: 0, y: 0 });
      return next;
    });
  }, []);

  const handleMouseDown = useCallback((e: React.MouseEvent) => {
    if (scale <= 1) return;
    isDragging.current = true;
    dragStart.current = { x: e.clientX, y: e.clientY };
    translateAtDragStart.current = { ...translate };
    e.preventDefault();
  }, [scale, translate]);

  const handleMouseMove = useCallback((e: React.MouseEvent) => {
    if (!isDragging.current) return;
    setTranslate({
      x: translateAtDragStart.current.x + (e.clientX - dragStart.current.x),
      y: translateAtDragStart.current.y + (e.clientY - dragStart.current.y),
    });
  }, []);

  const handleMouseUp = useCallback(() => { isDragging.current = false; }, []);

  return (
    <Box
      onWheel={handleWheel}
      onMouseDown={handleMouseDown}
      onMouseMove={handleMouseMove}
      onMouseUp={handleMouseUp}
      onMouseLeave={handleMouseUp}
      onDoubleClick={() => { setScale(1); setTranslate({ x: 0, y: 0 }); }}
      sx={{
        width: "100%",
        height: "100%",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        overflow: "hidden",
        cursor: scale > 1 ? "grab" : "default",
        userSelect: "none",
      }}
    >
      <img
        src={url}
        alt={alt}
        draggable={false}
        style={{
          maxWidth: "100%",
          maxHeight: "100%",
          objectFit: "contain",
          transform: `scale(${scale}) translate(${translate.x / scale}px, ${translate.y / scale}px)`,
          transformOrigin: "center center",
          transition: isDragging.current ? "none" : "transform 0.05s ease-out",
        }}
      />
    </Box>
  );
}

// ─── ImageGrid ───────────────────────────────────────────────────────────────

interface ImageGridProps {
  images: CapturedImageRecord[];
  selectMode: boolean;
  checkedIds: Set<string>;
  onToggleCheck: (id: string) => void;
  onOpenDetail: (id: string) => void;
}

function ImageGrid({ images, selectMode, checkedIds, onToggleCheck, onOpenDetail }: ImageGridProps) {
  if (images.length === 0) {
    return (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          height: 200,
          border: "1px dashed",
          borderColor: "divider",
          borderRadius: 1,
        }}
      >
        <Typography color="text.secondary">No images found</Typography>
      </Box>
    );
  }

  return (
    <Box
      sx={{
        display: "grid",
        gridTemplateColumns: {
          xs: "repeat(2, 1fr)",
          sm: "repeat(3, 1fr)",
          md: "repeat(4, 1fr)",
          lg: "repeat(5, 1fr)",
          xl: "repeat(6, 1fr)",
        },
        gap: 1,
      }}
    >
      {images.map((img) => {
        const isChecked = checkedIds.has(img.id);
        return (
          <Box
            key={img.id}
            onClick={() => {
              if (selectMode) onToggleCheck(img.id);
              else onOpenDetail(img.id);
            }}
            sx={{
              position: "relative",
              aspectRatio: "1",
              borderRadius: 1,
              overflow: "hidden",
              cursor: "pointer",
              outline: isChecked ? "3px solid" : "none",
              outlineColor: "primary.main",
              outlineOffset: "-3px",
              bgcolor: "action.disabledBackground",
              "&:hover": { opacity: 0.85 },
            }}
          >
            {/* Thumbnail */}
            {img.hasThumbnail ? (
              <Box
                component="img"
                src={thumbnailUrl(img.id)}
                alt={img.filename}
                sx={{
                  width: "100%",
                  height: "100%",
                  objectFit: "cover",
                  display: "block",
                }}
              />
            ) : (
              <Box
                sx={{
                  width: "100%",
                  height: "100%",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                <Typography variant="caption" sx={{ fontWeight: 700, textTransform: "uppercase", color: "text.secondary" }}>
                  {img.format}
                </Typography>
              </Box>
            )}

            {/* Filename label at bottom */}
            <Box
              sx={{
                position: "absolute",
                bottom: 0,
                left: 0,
                right: 0,
                px: 0.5,
                py: 0.25,
                bgcolor: "rgba(0,0,0,0.55)",
              }}
            >
              <Typography
                variant="caption"
                noWrap
                sx={{ fontSize: "0.6rem", color: "#fff", display: "block" }}
              >
                {img.filename}
              </Typography>
            </Box>

            {/* Checkbox overlay in select mode */}
            {selectMode && (
              <Box
                sx={{
                  position: "absolute",
                  top: 4,
                  left: 4,
                  bgcolor: "rgba(0,0,0,0.5)",
                  borderRadius: 0.5,
                }}
                onClick={(e) => { e.stopPropagation(); onToggleCheck(img.id); }}
              >
                <Checkbox
                  checked={isChecked}
                  size="small"
                  sx={{ p: 0.25, color: "common.white", "& .MuiSvgIcon-root": { fontSize: 16 } }}
                  tabIndex={-1}
                />
              </Box>
            )}
          </Box>
        );
      })}
    </Box>
  );
}

// ─── GalleryPage ─────────────────────────────────────────────────────────────

export default function GalleryPage({ onBack }: GalleryPageProps) {
  const { toast } = useToast();
  const gallery = useImageGallery();

  // Overlay state
  const [overlayId, setOverlayId] = useState<string | null>(null);
  const overlayImage = gallery.images.find((img) => img.id === overlayId) ?? null;

  // Select mode state
  const [selectMode, setSelectMode] = useState(false);
  const [checkedIds, setCheckedIds] = useState<Set<string>>(new Set());
  const [isExporting, setIsExporting] = useState(false);

  // Reset checked set when images change (page/filter change)
  useEffect(() => {
    setCheckedIds(new Set());
  }, [gallery.images]);

  const exitSelectMode = useCallback(() => {
    setSelectMode(false);
    setCheckedIds(new Set());
  }, []);

  const toggleCheck = useCallback((id: string) => {
    setCheckedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  }, []);

  const handleExportZip = async () => {
    const ids = Array.from(checkedIds);
    if (ids.length === 0) return;
    setIsExporting(true);
    try {
      const blob = await exportImagesZip(ids);
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = "peanut-vision-export.zip";
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
      toast(`${ids.length}개 이미지를 ZIP으로 내보냈습니다`, "success");
      exitSelectMode();
    } catch (e) {
      toast(e instanceof Error ? e.message : "내보내기에 실패했습니다", "error");
    } finally {
      setIsExporting(false);
    }
  };

  const hasActiveFilters = !!(
    gallery.filterFromDate ||
    gallery.filterToDate ||
    gallery.filterFormat
  );

  const handleClearFilters = () => {
    gallery.setFilterSessionId(null);
    gallery.setFilterFromDate(null);
    gallery.setFilterToDate(null);
    gallery.setFilterFormat(null);
  };

  return (
    <Box sx={{ display: "flex", flexDirection: "column", height: "100vh", bgcolor: "background.default" }}>
      {/* AppBar */}
      <AppBar position="static" color="default" elevation={1}>
        <Toolbar variant="dense" sx={{ gap: 1 }}>
          <IconButton edge="start" onClick={onBack} size="small">
            <ArrowBackIcon />
          </IconButton>

          <Typography variant="h6" sx={{ fontWeight: 700, fontSize: "1rem", flex: 1 }}>
            PeanutVision — Gallery
          </Typography>

          {/* Select mode toggle */}
          {!selectMode ? (
            <Tooltip title="Select images for bulk export">
              <IconButton size="small" onClick={() => setSelectMode(true)}>
                <CheckBoxOutlineBlankIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          ) : (
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              <Typography variant="body2" color="text.secondary">
                {checkedIds.size} selected
              </Typography>
              <Button
                size="small"
                variant="text"
                onClick={() => setCheckedIds(new Set(gallery.images.map((i) => i.id)))}
                sx={{ textTransform: "none", fontSize: "0.75rem" }}
              >
                All
              </Button>
              <Button
                size="small"
                variant="text"
                onClick={() => setCheckedIds(new Set())}
                sx={{ textTransform: "none", fontSize: "0.75rem" }}
              >
                None
              </Button>
              <IconButton size="small" onClick={exitSelectMode}>
                <CloseIcon fontSize="small" />
              </IconButton>
            </Box>
          )}

          {/* Export ZIP button */}
          <Button
            variant="contained"
            size="small"
            startIcon={isExporting ? <CircularProgress size={14} color="inherit" /> : <DownloadIcon />}
            disabled={!selectMode || checkedIds.size === 0 || isExporting}
            onClick={handleExportZip}
            sx={{ textTransform: "none" }}
          >
            Export ZIP
          </Button>
        </Toolbar>
      </AppBar>

      {/* Filter Bar */}
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 1.5,
          px: 2,
          py: 1,
          borderBottom: "1px solid",
          borderColor: "divider",
          bgcolor: "background.paper",
          flexWrap: "wrap",
        }}
      >
        {/* Date from */}
        <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
          <Typography variant="caption" color="text.secondary" sx={{ whiteSpace: "nowrap" }}>
            날짜 from:
          </Typography>
          <Box
            component="input"
            type="date"
            value={gallery.filterFromDate ?? ""}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              gallery.setFilterFromDate(e.target.value || null)
            }
            sx={{
              fontSize: "0.8rem",
              border: "1px solid",
              borderColor: "divider",
              borderRadius: 0.5,
              px: 0.75,
              py: 0.375,
              bgcolor: "background.paper",
              color: "text.primary",
              outline: "none",
              "&:focus": { borderColor: "primary.main" },
            }}
          />
        </Box>

        {/* Date to */}
        <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
          <Typography variant="caption" color="text.secondary" sx={{ whiteSpace: "nowrap" }}>
            to:
          </Typography>
          <Box
            component="input"
            type="date"
            value={gallery.filterToDate ?? ""}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              gallery.setFilterToDate(e.target.value || null)
            }
            sx={{
              fontSize: "0.8rem",
              border: "1px solid",
              borderColor: "divider",
              borderRadius: 0.5,
              px: 0.75,
              py: 0.375,
              bgcolor: "background.paper",
              color: "text.primary",
              outline: "none",
              "&:focus": { borderColor: "primary.main" },
            }}
          />
        </Box>

        {/* Format filter */}
        <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
          <Typography variant="caption" color="text.secondary">
            Format:
          </Typography>
          <Select
            size="small"
            value={gallery.filterFormat ?? ""}
            onChange={(e) => gallery.setFilterFormat(e.target.value || null)}
            displayEmpty
            sx={{ fontSize: "0.8rem", "& .MuiSelect-select": { py: 0.375, pl: 0.75 }, minWidth: 80 }}
          >
            <MenuItem value=""><em>All</em></MenuItem>
            <MenuItem value="png">PNG</MenuItem>
            <MenuItem value="bmp">BMP</MenuItem>
            <MenuItem value="raw">RAW</MenuItem>
          </Select>
        </Box>

        {/* Clear filters */}
        {hasActiveFilters && (
          <Button
            size="small"
            startIcon={<FilterAltOffIcon fontSize="small" />}
            onClick={handleClearFilters}
            sx={{ textTransform: "none", fontSize: "0.75rem" }}
          >
            Clear filters
          </Button>
        )}

        {/* Image count */}
        <Typography variant="body2" color="text.secondary" sx={{ ml: "auto" }}>
          {gallery.totalCount} {gallery.totalCount === 1 ? "image" : "images"}
        </Typography>
      </Box>

      {/* Main content area */}
      <Box sx={{ flex: 1, overflow: "auto", px: 2, py: 2 }}>
        {gallery.isLoading ? (
          <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", height: 200 }}>
            <CircularProgress />
          </Box>
        ) : (
          <ImageGrid
            images={gallery.images}
            selectMode={selectMode}
            checkedIds={checkedIds}
            onToggleCheck={toggleCheck}
            onOpenDetail={(id) => setOverlayId(id)}
          />
        )}
      </Box>

      {/* Footer Pagination */}
      {gallery.totalPages > 1 && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            py: 1.5,
            borderTop: "1px solid",
            borderColor: "divider",
            bgcolor: "background.paper",
          }}
        >
          <Pagination
            count={gallery.totalPages}
            page={gallery.page}
            onChange={(_, p) => gallery.setPage(p)}
            size="small"
            color="primary"
          />
        </Box>
      )}

      {/* Image Detail Overlay */}
      {overlayImage && (
        <ImageDetailOverlay
          image={overlayImage}
          images={gallery.images}
          onClose={() => setOverlayId(null)}
          onDelete={(id) => {
            gallery.handleDelete(id);
            setOverlayId(null);
          }}
          onNavigate={(id) => setOverlayId(id)}
        />
      )}
    </Box>
  );
}
