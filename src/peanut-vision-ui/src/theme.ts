import { createTheme } from "@mui/material/styles";

const theme = createTheme({
  palette: {
    mode: "light",
    primary:    { main: "#1d4ed8" },
    secondary:  { main: "#7c3aed" },
    success:    { main: "#16a34a" },
    warning:    { main: "#d97706" },
    error:      { main: "#dc2626" },
    background: { default: "#f8fafc", paper: "#ffffff" },
    text:       { primary: "#0f172a", secondary: "#475569" },
    divider:    "#e2e8f0",
  },
  typography: {
    fontFamily: "'Inter', 'Roboto', 'Helvetica', sans-serif",
    h6:   { fontWeight: 600 },
    body2: { fontSize: "0.8125rem" },
  },
  shape: { borderRadius: 8 },
  components: {
    MuiCard:   { defaultProps: { variant: "outlined" } },
    MuiButton: { defaultProps: { disableElevation: true } },
    MuiChip:   { defaultProps: { size: "small" } },
  },
});

export default theme;
