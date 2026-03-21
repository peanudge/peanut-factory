import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import CircularProgress from "@mui/material/CircularProgress";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import DownloadIcon from "@mui/icons-material/Download";
import ImageThumbnail from "./ImageThumbnail";
import { useImageExport } from "../hooks/useImageExport";
import type { CapturedImage } from "../api/types";

interface Props {
  images: CapturedImage[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  onClear: () => void;
}

export default function CapturedImageList({ images, selectedId, onSelect, onDelete, onClear }: Props) {
  const { exporting, handleExportZip } = useImageExport(images);

  return (
    <Box>
      {images.length > 0 && (
        <Box sx={{ display: "flex", gap: 0.5, mb: 1, justifyContent: "flex-end" }}>
          <Button
            size="small"
            startIcon={exporting ? <CircularProgress size={14} /> : <DownloadIcon />}
            onClick={handleExportZip}
            disabled={exporting}
          >
            Export ZIP
          </Button>
          <Button size="small" color="error" startIcon={<DeleteSweepIcon />} onClick={onClear}>
            Clear All
          </Button>
        </Box>
      )}

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
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(90px, 1fr))",
            gap: 1,
            maxHeight: 220,
            overflowY: "auto",
            pr: 0.5,
            "&::-webkit-scrollbar": { width: 4 },
            "&::-webkit-scrollbar-thumb": { borderRadius: 2, bgcolor: "divider" },
          }}
        >
          {images.map((image) => (
            <ImageThumbnail
              key={image.id}
              image={image}
              selected={image.id === selectedId}
              onSelect={onSelect}
              onDelete={onDelete}
            />
          ))}
        </Box>
      )}
    </Box>
  );
}
