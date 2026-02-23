import { useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Typography from "@mui/material/Typography";
import DownloadIcon from "@mui/icons-material/Download";

interface Props {
  blob: Blob | null;
  errorMessage?: string | null;
}

export default function ImageViewer({ blob, errorMessage }: Props) {
  const [url, setUrl] = useState<string | null>(null);

  useEffect(() => {
    if (!blob) {
      setUrl(null);
      return;
    }
    const objectUrl = URL.createObjectURL(blob);
    setUrl(objectUrl);
    return () => URL.revokeObjectURL(objectUrl);
  }, [blob]);

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
      <Button
        size="small"
        startIcon={<DownloadIcon />}
        href={url}
        download={`capture-${Date.now()}.png`}
      >
        Download
      </Button>
    </Box>
  );
}
