import { useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import CircularProgress from "@mui/material/CircularProgress";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import CloseIcon from "@mui/icons-material/Close";
import DownloadIcon from "@mui/icons-material/Download";
import JSZip from "jszip";
import { saveAs } from "file-saver";
import type { CapturedImage } from "../api/types";

interface Props {
  images: CapturedImage[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  onClear: () => void;
}

function formatTime(d: Date): string {
  return `${String(d.getHours()).padStart(2, "0")}:${String(d.getMinutes()).padStart(2, "0")}:${String(d.getSeconds()).padStart(2, "0")}`;
}

function formatFilename(d: Date, index: number): string {
  return `capture_${d.getFullYear()}${String(d.getMonth() + 1).padStart(2, "0")}${String(d.getDate()).padStart(2, "0")}_${formatTime(d).replace(/:/g, "")}_${String(index).padStart(3, "0")}.png`;
}

export default function CapturedImageList({ images, selectedId, onSelect, onDelete, onClear }: Props) {
  const [exporting, setExporting] = useState(false);

  const handleExportZip = async () => {
    if (images.length === 0) return;
    setExporting(true);
    try {
      const zip = new JSZip();
      for (let i = 0; i < images.length; i++) {
        const img = images[i];
        zip.file(formatFilename(img.capturedAt, i + 1), img.blob);
      }
      const blob = await zip.generateAsync({ type: "blob" });
      saveAs(blob, `captures_${new Date().toISOString().slice(0, 10)}.zip`);
    } finally {
      setExporting(false);
    }
  };


  return (
    <Box sx={{ mt: 1 }}>
      <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 1 }}>
        <Typography variant="subtitle2" color="text.secondary">
          Captured Images ({images.length})
        </Typography>
        <Box sx={{ display: "flex", gap: 0.5 }}>
          {images.length > 0 && (
            <>
              <Button
                size="small"
                startIcon={exporting ? <CircularProgress size={14} /> : <DownloadIcon />}
                onClick={handleExportZip}
                disabled={exporting}
              >
                Export ZIP
              </Button>
              <Button
                size="small"
                color="error"
                startIcon={<DeleteSweepIcon />}
                onClick={onClear}
              >
                Clear All
              </Button>
            </>
          )}
        </Box>
      </Box>

      {images.length === 0 ? (
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            height: 80,
            border: "1px dashed",
            borderColor: "divider",
            borderRadius: 1,
          }}
        >
          <Typography variant="caption" color="text.secondary">
            No images captured
          </Typography>
        </Box>
      ) : (
        <Box
          sx={{
            display: "flex",
            gap: 1,
            overflowX: "auto",
            pb: 1,
            "&::-webkit-scrollbar": { height: 4 },
            "&::-webkit-scrollbar-thumb": { borderRadius: 2, bgcolor: "divider" },
          }}
        >
          {images.map((img) => (
            <Box
              key={img.id}
              onClick={() => onSelect(img.id)}
              sx={{
                flexShrink: 0,
                cursor: "pointer",
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                gap: 0.5,
              }}
            >
              <Box
                sx={{
                  position: "relative",
                  width: 80,
                  height: 60,
                  border: "2px solid",
                  borderColor: img.id === selectedId ? "primary.main" : "divider",
                  borderRadius: 1,
                  overflow: "hidden",
                  transition: "border-color 0.15s",
                  "&:hover .delete-btn": { opacity: 1 },
                }}
              >
                <img
                  src={img.url}
                  alt={`Capture at ${formatTime(img.capturedAt)}`}
                  style={{ width: "100%", height: "100%", objectFit: "cover", display: "block" }}
                />
                <IconButton
                  className="delete-btn"
                  size="small"
                  onClick={(e) => {
                    e.stopPropagation();
                    onDelete(img.id);
                  }}
                  sx={{
                    position: "absolute",
                    top: -2,
                    right: -2,
                    opacity: 0,
                    transition: "opacity 0.15s",
                    p: 0.25,
                    bgcolor: "rgba(0,0,0,0.6)",
                    color: "white",
                    "&:hover": { bgcolor: "error.main" },
                  }}
                >
                  <CloseIcon sx={{ fontSize: 14 }} />
                </IconButton>
              </Box>
              <Typography variant="caption" color="text.secondary" sx={{ fontSize: "0.65rem" }}>
                {formatTime(img.capturedAt)}
              </Typography>
            </Box>
          ))}
        </Box>
      )}
    </Box>
  );
}
