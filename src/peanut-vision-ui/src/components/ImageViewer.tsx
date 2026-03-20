import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import DownloadIcon from "@mui/icons-material/Download";
import SaveIcon from "@mui/icons-material/Save";

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
          height: 300,
          border: "1px dashed",
          borderColor: "divider",
          borderRadius: 1,
        }}
      >
        <Typography color="text.secondary">
          No captured frame
        </Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box
        sx={{
          position: "relative",
          border: "1px solid",
          borderColor: "divider",
          borderRadius: 1,
          overflow: "hidden",
          mb: 1,
        }}
      >
        <img
          src={url}
          alt="Captured frame"
          style={{ width: "100%", display: "block" }}
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
      <Box sx={{ display: "flex", alignItems: "center", gap: 1, flexWrap: "wrap" }}>
        <Button
          size="small"
          startIcon={<DownloadIcon />}
          href={url}
          download={filename ?? `capture-${Date.now()}.png`}
        >
          Download
        </Button>
        {savedPath && (
          <Tooltip title={savedPath}>
            <Box sx={{ display: "flex", alignItems: "center", gap: 0.5, minWidth: 0 }}>
              <SaveIcon sx={{ fontSize: 14, color: "success.main", flexShrink: 0 }} />
              <Typography
                variant="caption"
                color="success.main"
                noWrap
                sx={{ maxWidth: 300 }}
              >
                {savedPath}
              </Typography>
            </Box>
          </Tooltip>
        )}
      </Box>
    </Box>
  );
}
