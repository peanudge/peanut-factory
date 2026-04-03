import { useCallback, useEffect, useRef, useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import CircularProgress from "@mui/material/CircularProgress";
import IconButton from "@mui/material/IconButton";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import Typography from "@mui/material/Typography";
import Tooltip from "@mui/material/Tooltip";
import Badge from "@mui/material/Badge";
import CloseIcon from "@mui/icons-material/Close";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import CheckBoxOutlineBlankIcon from "@mui/icons-material/CheckBoxOutlineBlank";
import DownloadIcon from "@mui/icons-material/Download";
import { thumbnailUrl, exportImagesZip } from "../api/client";
import type { CapturedImageRecord } from "../api/types";
import { formatTime } from "../utils/formatTimestamp";
import { useToast } from "../contexts/ToastContext";

interface Props {
  images: CapturedImageRecord[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  page: number;
  totalPages: number;
  totalCount: number;
  onPageChange: (p: number) => void;
  filterFromDate: string | null;
  onFromDateChange: (date: string | null) => void;
  filterToDate: string | null;
  onToDateChange: (date: string | null) => void;
  filterFormat: string | null;
  onFormatChange: (format: string | null) => void;
  isLoading: boolean;
}

export default function ImageGallery({
  images,
  selectedId,
  onSelect,
  onDelete,
  page,
  totalPages,
  totalCount,
  onPageChange,
  filterFromDate,
  onFromDateChange,
  filterToDate,
  onToDateChange,
  filterFormat,
  onFormatChange,
  isLoading,
}: Props) {
  const { toast } = useToast();
  const [selectMode, setSelectMode] = useState(false);
  const [checkedIds, setCheckedIds] = useState<Set<string>>(new Set());
  const [isExporting, setIsExporting] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const hasActiveFilters = !!(filterFromDate || filterToDate || filterFormat);

  const exitSelectMode = useCallback(() => {
    setSelectMode(false);
    setCheckedIds(new Set());
  }, []);

  // Exit select mode on Escape
  useEffect(() => {
    if (!selectMode) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") exitSelectMode();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [selectMode, exitSelectMode]);

  // Clear checked set when images list changes (e.g. filter/page change)
  useEffect(() => {
    setCheckedIds(new Set());
  }, [images]);

  const toggleCheck = (id: string) => {
    setCheckedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const selectAll = () => setCheckedIds(new Set(images.map((i) => i.id)));
  const deselectAll = () => setCheckedIds(new Set());

  const handleExport = async () => {
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

  return (
    <Box ref={containerRef}>
      {/* Filters */}
      <Box sx={{ display: "flex", flexDirection: "column", gap: 0.5, mb: 1 }}>
        {/* Format filter */}
        <Select
          size="small"
          value={filterFormat ?? ""}
          onChange={(e) => onFormatChange(e.target.value || null)}
          displayEmpty
          IconComponent={ExpandMoreIcon}
          sx={{ fontSize: "0.75rem", "& .MuiSelect-select": { py: 0.5 } }}
        >
          <MenuItem value=""><em>All formats</em></MenuItem>
          <MenuItem value="png">PNG</MenuItem>
          <MenuItem value="bmp">BMP</MenuItem>
          <MenuItem value="raw">RAW</MenuItem>
        </Select>

        {/* Date range filters */}
        <Box sx={{ display: "flex", gap: 0.5, alignItems: "center" }}>
          <Box
            component="input"
            type="date"
            value={filterFromDate ?? ""}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              onFromDateChange(e.target.value || null)
            }
            title="From date"
            sx={{
              flex: 1,
              fontSize: "0.7rem",
              border: "1px solid",
              borderColor: "divider",
              borderRadius: 0.5,
              px: 0.5,
              py: 0.375,
              bgcolor: "background.paper",
              color: "text.primary",
              outline: "none",
              "&:focus": { borderColor: "primary.main" },
              minWidth: 0,
            }}
          />
          <Typography variant="caption" color="text.secondary" sx={{ flexShrink: 0 }}>–</Typography>
          <Box
            component="input"
            type="date"
            value={filterToDate ?? ""}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              onToDateChange(e.target.value || null)
            }
            title="To date"
            sx={{
              flex: 1,
              fontSize: "0.7rem",
              border: "1px solid",
              borderColor: "divider",
              borderRadius: 0.5,
              px: 0.5,
              py: 0.375,
              bgcolor: "background.paper",
              color: "text.primary",
              outline: "none",
              "&:focus": { borderColor: "primary.main" },
              minWidth: 0,
            }}
          />
        </Box>

        {/* Filter summary + actions row */}
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
          <Typography variant="caption" color="text.secondary">
            {totalCount} {totalCount === 1 ? "image" : "images"}
          </Typography>
          <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
            {hasActiveFilters && (
              <Button
                size="small"
                onClick={() => {
                  onFromDateChange(null);
                  onToDateChange(null);
                  onFormatChange(null);
                }}
                sx={{ fontSize: "0.65rem", py: 0, minWidth: 0 }}
              >
                Clear filters
              </Button>
            )}
            {images.length > 0 && !selectMode && (
              <Tooltip title="Select images to export">
                <IconButton
                  size="small"
                  onClick={() => setSelectMode(true)}
                  sx={{ p: 0.25 }}
                >
                  <CheckBoxOutlineBlankIcon sx={{ fontSize: 14 }} />
                </IconButton>
              </Tooltip>
            )}
            {selectMode && (
              <IconButton size="small" onClick={exitSelectMode} sx={{ p: 0.25 }}>
                <CloseIcon sx={{ fontSize: 14 }} />
              </IconButton>
            )}
          </Box>
        </Box>

        {/* Select mode toolbar */}
        {selectMode && (
          <Box sx={{ display: "flex", alignItems: "center", gap: 0.5, flexWrap: "wrap" }}>
            <Button
              size="small"
              onClick={selectAll}
              sx={{ fontSize: "0.65rem", py: 0, minWidth: 0 }}
            >
              All
            </Button>
            <Button
              size="small"
              onClick={deselectAll}
              sx={{ fontSize: "0.65rem", py: 0, minWidth: 0 }}
            >
              None
            </Button>
            <Badge
              badgeContent={checkedIds.size}
              color="primary"
              sx={{ mx: 0.5 }}
            >
              <Typography variant="caption" color="text.secondary">
                selected
              </Typography>
            </Badge>
            <Button
              size="small"
              variant="contained"
              startIcon={isExporting ? <CircularProgress size={10} color="inherit" /> : <DownloadIcon />}
              disabled={checkedIds.size === 0 || isExporting}
              onClick={handleExport}
              sx={{ fontSize: "0.65rem", py: 0.25, ml: "auto" }}
            >
              Export ZIP
            </Button>
          </Box>
        )}
      </Box>

      {/* Loading state */}
      {isLoading && (
        <Box sx={{ display: "flex", justifyContent: "center", py: 2 }}>
          <CircularProgress size={20} />
        </Box>
      )}

      {/* Empty state */}
      {!isLoading && images.length === 0 && (
        <Box sx={{
          display: "flex", alignItems: "center", justifyContent: "center",
          height: 60, border: "1px dashed", borderColor: "divider", borderRadius: 1,
        }}>
          <Typography variant="caption" color="text.secondary">No captures yet</Typography>
        </Box>
      )}

      {/* Thumbnail grid */}
      {images.length > 0 && (
        <Box
          sx={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(72px, 1fr))",
            gap: "4px",
            maxHeight: "calc(50vh - 140px)",
            overflowY: "auto",
            "&::-webkit-scrollbar": { width: 4 },
            "&::-webkit-scrollbar-thumb": { borderRadius: 2, bgcolor: "divider" },
          }}
        >
          {images.map((img) => (
            <Tooltip
              key={img.id}
              title={
                <Box>
                  <Typography variant="caption" display="block">{img.filename}</Typography>
                  <Typography variant="caption" color="text.secondary" display="block">
                    {formatTime(new Date(img.capturedAt))}
                  </Typography>
                  <Typography variant="caption" color="text.secondary" display="block">
                    {img.width}×{img.height} · {img.format.toUpperCase()}
                  </Typography>
                </Box>
              }
              placement="left"
              arrow
            >
              <Box
                onClick={() => {
                  if (selectMode) {
                    toggleCheck(img.id);
                  } else {
                    onSelect(img.id);
                  }
                }}
                sx={{
                  position: "relative",
                  aspectRatio: "1",
                  cursor: "pointer",
                  borderRadius: 0.5,
                  overflow: "hidden",
                  outline: !selectMode && img.id === selectedId ? "2px solid" : "none",
                  outlineColor: "primary.main",
                  outlineOffset: "-2px",
                  ...(selectMode && checkedIds.has(img.id) && {
                    outline: "2px solid",
                    outlineColor: "secondary.main",
                    outlineOffset: "-2px",
                  }),
                  "&:hover .delete-overlay": { opacity: selectMode ? 0 : 1 },
                }}
              >
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
                      bgcolor: "action.hover",
                    }}
                    onError={(e: React.SyntheticEvent<HTMLImageElement>) => {
                      e.currentTarget.style.display = "none";
                      e.currentTarget.nextElementSibling?.setAttribute(
                        "style", "display: flex",
                      );
                    }}
                  />
                ) : null}

                {/* RAW / fallback placeholder */}
                <Box
                  sx={{
                    display: img.hasThumbnail ? "none" : "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    width: "100%",
                    height: "100%",
                    bgcolor: "action.disabledBackground",
                    position: img.hasThumbnail ? "absolute" : "relative",
                    top: 0,
                    left: 0,
                  }}
                >
                  <Typography
                    variant="caption"
                    sx={{ fontSize: "0.6rem", fontWeight: 700, color: "text.secondary", textTransform: "uppercase" }}
                  >
                    {img.format}
                  </Typography>
                </Box>

                {/* Checkbox overlay (select mode) */}
                {selectMode && (
                  <Box
                    sx={{
                      position: "absolute",
                      top: 1,
                      left: 1,
                      p: 0,
                      bgcolor: "rgba(0,0,0,0.45)",
                      borderRadius: 0.5,
                      display: "flex",
                    }}
                    onClick={(e) => { e.stopPropagation(); toggleCheck(img.id); }}
                  >
                    <Checkbox
                      checked={checkedIds.has(img.id)}
                      size="small"
                      sx={{ p: 0, color: "common.white", "& .MuiSvgIcon-root": { fontSize: 14 } }}
                      tabIndex={-1}
                    />
                  </Box>
                )}

                {/* Delete button overlay */}
                {!selectMode && (
                  <IconButton
                    className="delete-overlay"
                    size="small"
                    onClick={(e) => { e.stopPropagation(); onDelete(img.id); }}
                    sx={{
                      position: "absolute",
                      top: 2,
                      right: 2,
                      p: 0.25,
                      opacity: 0,
                      transition: "opacity 0.15s",
                      bgcolor: "rgba(0,0,0,0.6)",
                      color: "common.white",
                      "&:hover": { bgcolor: "error.main" },
                    }}
                  >
                    <CloseIcon sx={{ fontSize: 10 }} />
                  </IconButton>
                )}
              </Box>
            </Tooltip>
          ))}
        </Box>
      )}

      {/* Load more */}
      {page < totalPages && (
        <Box sx={{ display: "flex", justifyContent: "center", mt: 1 }}>
          <Button
            size="small"
            startIcon={<ExpandMoreIcon />}
            onClick={() => onPageChange(page + 1)}
            sx={{ fontSize: "0.7rem" }}
          >
            Load more
          </Button>
        </Box>
      )}

      {/* Clear all (show when images exist and not in select mode) */}
      {images.length > 0 && page === 1 && !selectMode && (
        <Box sx={{ display: "flex", justifyContent: "flex-end", mt: 1 }}>
          <Button
            size="small"
            color="error"
            startIcon={<DeleteSweepIcon />}
            onClick={() => {
              images.forEach((img) => onDelete(img.id));
            }}
            sx={{ fontSize: "0.7rem" }}
          >
            Clear All
          </Button>
        </Box>
      )}
    </Box>
  );
}
