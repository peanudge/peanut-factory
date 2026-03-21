import Box from "@mui/material/Box";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import CloseIcon from "@mui/icons-material/Close";
import type { CapturedImage } from "../api/types";
import { formatTime } from "../utils/formatTimestamp";

/** Renders a single image thumbnail with selection highlight and delete button. */
interface Props {
  image: CapturedImage;
  selected: boolean;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
}

export default function ImageThumbnail({ image, selected, onSelect, onDelete }: Props) {
  return (
    <Box
      onClick={() => onSelect(image.id)}
      sx={{
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
          width: "100%",
          height: 60,
          border: "2px solid",
          borderColor: selected ? "primary.main" : "divider",
          borderRadius: 1,
          overflow: "hidden",
          transition: "border-color 0.15s",
          "&:hover .delete-btn": { opacity: 1 },
        }}
      >
        <img
          src={image.url}
          alt={`Capture at ${formatTime(image.capturedAt)}`}
          style={{ width: "100%", height: "100%", objectFit: "cover", display: "block" }}
        />
        <IconButton
          className="delete-btn"
          size="small"
          onClick={(event) => {
            event.stopPropagation();
            onDelete(image.id);
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
        {formatTime(image.capturedAt)}
      </Typography>
    </Box>
  );
}
