import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Collapse from "@mui/material/Collapse";
import Portal from "@mui/material/Portal";
import Slide from "@mui/material/Slide";
import { TransitionGroup } from "react-transition-group";
import { useToast } from "../contexts/ToastContext";

export default function ToastContainer() {
  const { toasts, dismiss } = useToast();

  return (
    <Portal>
      <Box
        sx={{
          position: "fixed",
          bottom: 24,
          right: 24,
          zIndex: "snackbar",
          width: 360,
          maxWidth: "calc(100vw - 48px)",
          pointerEvents: "none",
        }}
      >
        <TransitionGroup component={null}>
          {toasts.map((t) => (
            <Collapse key={t.id} timeout={250}>
              <Box sx={{ mb: 1, pointerEvents: "all" }}>
                <Slide in direction="left" timeout={200}>
                  <Alert
                    severity={t.severity}
                    variant="filled"
                    onClose={() => dismiss(t.id)}
                    sx={{ width: "100%", boxShadow: 3, alignItems: "center" }}
                  >
                    {t.message}
                  </Alert>
                </Slide>
              </Box>
            </Collapse>
          ))}
        </TransitionGroup>
      </Box>
    </Portal>
  );
}
