import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import DownloadIcon from "@mui/icons-material/Download";
import SaveIcon from "@mui/icons-material/Save";

/** Renders a download button and optional saved-path indicator for a captured image. */
interface Props {
  url: string;
  filename?: string;
  savedPath?: string;
}

export default function ImageActionBar({ url, filename, savedPath }: Props) {
  return (
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
  );
}
