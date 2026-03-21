import { useEffect } from "react";
import Alert from "@mui/material/Alert";
import Typography from "@mui/material/Typography";

interface Props {
  error: string;
  errorCode?: string;
  onClose: () => void;
}

export default function ErrorAlert({ error, errorCode, onClose }: Props) {
  useEffect(() => {
    if (!error) return;
    const t = setTimeout(onClose, 5000);
    return () => clearTimeout(t);
  }, [error, onClose]);

  if (!error) return null;
  return (
    <Alert severity="error" onClose={onClose}>
      {error}
      {errorCode && (
        <Typography variant="caption" display="block" sx={{ opacity: 0.7, mt: 0.5 }}>
          [{errorCode}]
        </Typography>
      )}
    </Alert>
  );
}
