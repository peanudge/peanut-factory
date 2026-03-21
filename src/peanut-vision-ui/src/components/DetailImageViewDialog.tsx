import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import CloseIcon from "@mui/icons-material/Close";
import ImageActionBar from "./ImageActionBar";
import type { CapturedImage } from "../api/types";
import { formatFilenameTimestamp, formatTime } from "../utils/formatTimestamp";

/** Full-size image dialog opened when a gallery thumbnail is clicked. */
interface Props {
  image: CapturedImage | null;
  errorMessage?: string | null;
  onClose: () => void;
}

export default function DetailImageViewDialog({ image, errorMessage, onClose }: Props) {
  return (
    <Dialog open={image !== null} onClose={onClose} maxWidth="lg" fullWidth>
      <DialogTitle sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", pb: 1 }}>
        <Typography variant="subtitle1" fontWeight={600}>
          {image ? formatTime(image.capturedAt) : ""}
        </Typography>
        <IconButton size="small" onClick={onClose}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </DialogTitle>

      <DialogContent sx={{ p: 1.5, display: "flex", flexDirection: "column", gap: 1 }}>
        <Box
          sx={{
            position: "relative",
            border: "1px solid",
            borderColor: "divider",
            borderRadius: 1,
            overflow: "hidden",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            bgcolor: "background.default",
            minHeight: 400,
          }}
        >
          {image && (
            <img
              src={image.url}
              alt="Captured frame"
              style={{ maxWidth: "100%", maxHeight: "70vh", display: "block", objectFit: "contain" }}
            />
          )}
          {errorMessage && (
            <Box sx={{ position: "absolute", top: 8, left: 8 }}>
              <Chip size="small" label={errorMessage} color="error" sx={{ fontWeight: 600, fontSize: "0.7rem" }} />
            </Box>
          )}
        </Box>

        {image && (
          <ImageActionBar
            url={image.url}
            filename={`capture-${formatFilenameTimestamp(image.capturedAt)}.png`}
            savedPath={image.savedPath}
          />
        )}
      </DialogContent>
    </Dialog>
  );
}
