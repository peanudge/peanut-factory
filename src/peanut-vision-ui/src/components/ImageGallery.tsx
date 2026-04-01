import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import IconButton from "@mui/material/IconButton";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import Typography from "@mui/material/Typography";
import Tooltip from "@mui/material/Tooltip";
import CloseIcon from "@mui/icons-material/Close";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { useQuery } from "@tanstack/react-query";
import { getSessions, thumbnailUrl } from "../api/client";
import { queryKeys } from "../api/queryKeys";
import type { CapturedImageRecord } from "../api/types";
import { formatTime } from "../utils/formatTimestamp";

interface Props {
  images: CapturedImageRecord[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  page: number;
  totalPages: number;
  totalCount: number;
  onPageChange: (p: number) => void;
  filterSessionId: string | null;
  onFilterChange: (sessionId: string | null) => void;
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
  filterSessionId,
  onFilterChange,
  filterFromDate,
  onFromDateChange,
  filterToDate,
  onToDateChange,
  filterFormat,
  onFormatChange,
  isLoading,
}: Props) {
  const { data: sessions } = useQuery({
    queryKey: queryKeys.sessions,
    queryFn: () => getSessions(),
  });

  const hasActiveFilters = !!(filterSessionId || filterFromDate || filterToDate || filterFormat);

  return (
    <Box>
      {/* Filters */}
      <Box sx={{ display: "flex", flexDirection: "column", gap: 0.5, mb: 1 }}>
        {/* Session filter */}
        <Select
          size="small"
          value={filterSessionId ?? ""}
          onChange={(e) => onFilterChange(e.target.value || null)}
          displayEmpty
          IconComponent={ExpandMoreIcon}
          sx={{ fontSize: "0.75rem", "& .MuiSelect-select": { py: 0.5 } }}
        >
          <MenuItem value=""><em>All sessions</em></MenuItem>
          {sessions?.map((s) => (
            <MenuItem key={s.id} value={s.id}>
              <Typography variant="caption" noWrap>{s.name}</Typography>
            </MenuItem>
          ))}
        </Select>

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

        {/* Filter summary + clear */}
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}>
          <Typography variant="caption" color="text.secondary">
            {totalCount} {totalCount === 1 ? "image" : "images"}
          </Typography>
          {hasActiveFilters && (
            <Button
              size="small"
              onClick={() => {
                onFilterChange(null);
                onFromDateChange(null);
                onToDateChange(null);
                onFormatChange(null);
              }}
              sx={{ fontSize: "0.65rem", py: 0, minWidth: 0, ml: 1 }}
            >
              Clear filters
            </Button>
          )}
        </Box>
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
                onClick={() => onSelect(img.id)}
                sx={{
                  position: "relative",
                  aspectRatio: "1",
                  cursor: "pointer",
                  borderRadius: 0.5,
                  overflow: "hidden",
                  outline: img.id === selectedId ? "2px solid" : "none",
                  outlineColor: "primary.main",
                  outlineOffset: "-2px",
                  "&:hover .delete-overlay": { opacity: 1 },
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

                {/* Delete button overlay */}
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

      {/* Clear all (show when images exist) */}
      {images.length > 0 && page === 1 && (
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
