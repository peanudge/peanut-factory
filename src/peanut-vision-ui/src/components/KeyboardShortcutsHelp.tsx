import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import CloseIcon from "@mui/icons-material/Close";
import KeyboardIcon from "@mui/icons-material/Keyboard";

interface ShortcutRow {
  keys: string[];
  description: string;
}

const SHORTCUTS: ShortcutRow[] = [
  { keys: ["Space"], description: "Snapshot capture (when idle)" },
  { keys: ["Ctrl", "R"], description: "Start / stop continuous capture" },
  { keys: ["Delete"], description: "Delete selected image" },
  { keys: ["←"], description: "Previous image in gallery" },
  { keys: ["→"], description: "Next image in gallery" },
  { keys: ["Esc"], description: "Return to live view" },
  { keys: ["?"], description: "Show this help" },
];

interface Props {
  open: boolean;
  onClose: () => void;
}

export default function KeyboardShortcutsHelp({ open, onClose }: Props) {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ display: "flex", alignItems: "center", gap: 1, pr: 6 }}>
        <KeyboardIcon fontSize="small" />
        Keyboard Shortcuts
        <IconButton
          size="small"
          onClick={onClose}
          sx={{ position: "absolute", right: 8, top: 8 }}
          aria-label="Close"
        >
          <CloseIcon fontSize="small" />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
          {SHORTCUTS.map(({ keys, description }) => (
            <Box
              key={keys.join("+")}
              sx={{ display: "flex", alignItems: "center", justifyContent: "space-between" }}
            >
              <Typography variant="body2" color="text.secondary">
                {description}
              </Typography>
              <Box sx={{ display: "flex", gap: 0.5, flexShrink: 0, ml: 2 }}>
                {keys.map((k) => (
                  <Chip
                    key={k}
                    label={k}
                    size="small"
                    variant="outlined"
                    sx={{ fontFamily: "monospace", fontWeight: 600, fontSize: "0.7rem", height: 22 }}
                  />
                ))}
              </Box>
            </Box>
          ))}
        </Box>
      </DialogContent>
    </Dialog>
  );
}
