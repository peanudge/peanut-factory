import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import Typography from "@mui/material/Typography";
import ImageActionBar from "./ImageActionBar";
import { formatTime } from "../utils/formatTimestamp";

interface Props {
  url: string | null;
  filename?: string;
  errorMessage?: string | null;
  savedPath?: string;
  isLive: boolean;
  capturedAt: Date | null;
  onReturnToLive: () => void;
}

export default function ImageViewer({ url, filename, errorMessage, savedPath, isLive, capturedAt, onReturnToLive }: Props) {
  if (!url) {
    return (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          height: "100%",
          minHeight: 200,
          border: "1px dashed",
          borderColor: "divider",
          borderRadius: 1,
        }}
      >
        <Typography color="text.secondary">No captured frame</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ display: "flex", flexDirection: "column", height: "100%", gap: 1 }}>
      <Box
        sx={{
          position: "relative",
          border: "1px solid",
          borderColor: "divider",
          borderRadius: 1,
          overflow: "hidden",
          flexGrow: 1,
          minHeight: 0,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          bgcolor: "background.default",
        }}
      >
        <img
          src={url}
          alt="Captured frame"
          style={{ maxWidth: "100%", maxHeight: "100%", display: "block", objectFit: "contain" }}
        />
        {errorMessage && (
          <Box sx={{ position: "absolute", top: 8, left: 8 }}>
            <Chip
              size="small"
              label={errorMessage}
              color="error"
              sx={{ fontWeight: 600, fontSize: "0.7rem" }}
            />
          </Box>
        )}
        <Box sx={{ position: "absolute", top: 8, right: 8, display: "flex", alignItems: "center", gap: 0.5 }}>
          {isLive ? (
            <Chip
              size="small"
              label="LIVE"
              color="success"
              sx={{ fontWeight: 700, fontSize: "0.65rem" }}
            />
          ) : (
            <>
              <Chip
                size="small"
                label={capturedAt ? formatTime(capturedAt) : "Captured"}
                sx={{
                  fontWeight: 600,
                  fontSize: "0.65rem",
                  bgcolor: "rgba(0,0,0,0.55)",
                  color: "#fff",
                }}
              />
              <Chip
                size="small"
                label="Return to Live"
                onClick={onReturnToLive}
                variant="outlined"
                color="primary"
                sx={{ fontWeight: 600, fontSize: "0.65rem", cursor: "pointer", bgcolor: "rgba(0,0,0,0.4)" }}
              />
            </>
          )}
        </Box>
      </Box>
      <Box sx={{ flexShrink: 0 }}>
        <ImageActionBar url={url} filename={filename} savedPath={savedPath} />
      </Box>
    </Box>
  );
}
