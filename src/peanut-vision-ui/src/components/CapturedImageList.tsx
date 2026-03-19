import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import type { CapturedImage } from "../api/types";

interface Props {
  images: CapturedImage[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onClear: () => void;
}

function formatTime(d: Date): string {
  return `${String(d.getHours()).padStart(2, "0")}:${String(d.getMinutes()).padStart(2, "0")}:${String(d.getSeconds()).padStart(2, "0")}`;
}

export default function CapturedImageList({ images, selectedId, onSelect, onClear }: Props) {
  return (
    <Box sx={{ mt: 1 }}>
      <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 1 }}>
        <Typography variant="subtitle2" color="text.secondary">
          Captured Images ({images.length})
        </Typography>
        {images.length > 0 && (
          <Button
            size="small"
            color="error"
            startIcon={<DeleteSweepIcon />}
            onClick={onClear}
          >
            Clear All
          </Button>
        )}
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
                  width: 80,
                  height: 60,
                  border: "2px solid",
                  borderColor: img.id === selectedId ? "primary.main" : "divider",
                  borderRadius: 1,
                  overflow: "hidden",
                  transition: "border-color 0.15s",
                }}
              >
                <img
                  src={img.url}
                  alt={`Capture at ${formatTime(img.capturedAt)}`}
                  style={{ width: "100%", height: "100%", objectFit: "cover", display: "block" }}
                />
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
