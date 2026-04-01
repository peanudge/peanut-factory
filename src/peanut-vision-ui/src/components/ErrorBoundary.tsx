import { Component } from "react";
import type { ReactNode, ErrorInfo } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import RefreshIcon from "@mui/icons-material/Refresh";

interface Props {
  children: ReactNode;
  /** Optional label shown in the error UI, e.g. "Image Gallery" */
  label?: string;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

export default class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error("[ErrorBoundary] Uncaught error:", error, info.componentStack);
  }

  handleReset = () => {
    this.setState({ hasError: false, error: null });
  };

  render() {
    if (!this.state.hasError) {
      return this.props.children;
    }

    const { label } = this.props;
    const title = label ? `${label} failed to load` : "Something went wrong";

    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          gap: 1,
          p: 2,
          minHeight: 80,
          border: "1px dashed",
          borderColor: "error.main",
          borderRadius: 1,
          bgcolor: "error.light",
          opacity: 0.85,
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 0.75, color: "error.dark" }}>
          <ErrorOutlineIcon fontSize="small" />
          <Typography variant="body2" fontWeight={600} color="error.dark">
            {title}
          </Typography>
        </Box>
        {this.state.error?.message && (
          <Typography
            variant="caption"
            color="error.dark"
            sx={{ opacity: 0.75, textAlign: "center", maxWidth: 280, wordBreak: "break-word" }}
          >
            {this.state.error.message}
          </Typography>
        )}
        <Button
          size="small"
          startIcon={<RefreshIcon />}
          onClick={this.handleReset}
          variant="outlined"
          color="error"
          sx={{ mt: 0.5, fontSize: "0.72rem" }}
        >
          Retry
        </Button>
      </Box>
    );
  }
}
