import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import Typography from "@mui/material/Typography";
import ImageActionBar from "./ImageActionBar";

interface Props {
  url: string | null;
  filename?: string;
  errorMessage?: string | null;
  savedPath?: string;
}

export default function ImageViewer({ url, filename, errorMessage, savedPath }: Props) {
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
      </Box>
      <Box sx={{ flexShrink: 0 }}>
        <ImageActionBar url={url} filename={filename} savedPath={savedPath} />
      </Box>
    </Box>
  );
}
