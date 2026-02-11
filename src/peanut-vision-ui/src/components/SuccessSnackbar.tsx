import Snackbar from "@mui/material/Snackbar";

interface Props {
  message: string;
  onClose: () => void;
}

export default function SuccessSnackbar({ message, onClose }: Props) {
  return (
    <Snackbar
      open={!!message}
      autoHideDuration={3000}
      onClose={onClose}
      message={message}
    />
  );
}
