import Alert from "@mui/material/Alert";

interface Props {
  error: string;
  onClose: () => void;
}

export default function ErrorAlert({ error, onClose }: Props) {
  if (!error) return null;
  return (
    <Alert severity="error" onClose={onClose}>
      {error}
    </Alert>
  );
}
