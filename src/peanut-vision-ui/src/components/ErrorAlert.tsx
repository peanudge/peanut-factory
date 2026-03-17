import { useEffect } from "react";
import Alert from "@mui/material/Alert";

interface Props {
  error: string;
  onClose: () => void;
}

export default function ErrorAlert({ error, onClose }: Props) {
  useEffect(() => {
    if (!error) return;
    const t = setTimeout(onClose, 5000);
    return () => clearTimeout(t);
  }, [error, onClose]);

  if (!error) return null;
  return (
    <Alert severity="error" onClose={onClose}>
      {error}
    </Alert>
  );
}
