import { useRef, useEffect } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import DeleteSweepIcon from "@mui/icons-material/DeleteSweep";
import CloseIcon from "@mui/icons-material/Close";
import CameraAltIcon from "@mui/icons-material/CameraAlt";
import type { CapturedEvent } from "../api/types";
import { formatTime, getFilename } from "../utils/formatTimestamp";

interface Props {
  events: CapturedEvent[];
  selectedId: string | null;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  onClear: () => void;
}

export default function CaptureLog({ events, selectedId, onSelect, onDelete, onClear }: Props) {
  const scrollRef = useRef<HTMLDivElement>(null);
  const firstId = events[0]?.id;

  useEffect(() => {
    if (scrollRef.current) scrollRef.current.scrollTop = 0;
  }, [firstId]);

  return (
    <Box>
      {events.length > 0 && (
        <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 1 }}>
          <Button size="small" color="error" startIcon={<DeleteSweepIcon />} onClick={onClear}>
            Clear All
          </Button>
        </Box>
      )}

      {events.length === 0 ? (
        <Box sx={{
          display: "flex", alignItems: "center", justifyContent: "center",
          height: 60, border: "1px dashed", borderColor: "divider", borderRadius: 1,
        }}>
          <Typography variant="caption" color="text.secondary">No captures yet</Typography>
        </Box>
      ) : (
        <Box ref={scrollRef} sx={{
          maxHeight: "calc(50vh - 120px)",
          overflowY: "auto",
          "&::-webkit-scrollbar": { width: 4 },
          "&::-webkit-scrollbar-thumb": { borderRadius: 2, bgcolor: "divider" },
        }}>
          {events.map((evt) => (
            <Box
              key={evt.id}
              onClick={() => onSelect(evt.id)}
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 1,
                pl: 1,
                pr: 0.5,
                py: 0.75,
                cursor: "pointer",
                borderLeft: "3px solid",
                borderColor: evt.id === selectedId ? "primary.main" : "transparent",
                borderBottom: "1px solid",
                borderBottomColor: "divider",
                bgcolor: evt.id === selectedId ? "action.selected" : "transparent",
                "&:last-child": { borderBottom: "none" },
                "&:hover": { bgcolor: "action.hover" },
                "&:hover .delete-btn": { opacity: 1 },
              }}
            >
              <CameraAltIcon sx={{ fontSize: 14, color: "text.secondary", flexShrink: 0 }} />
              <Box sx={{ flexGrow: 1, minWidth: 0 }}>
                <Typography
                  variant="caption"
                  display="block"
                  noWrap
                  sx={{ fontFamily: "monospace", fontSize: "0.7rem" }}
                >
                  {getFilename(evt.filePath)}
                </Typography>
                <Typography variant="caption" color="text.secondary" sx={{ fontSize: "0.65rem" }}>
                  {formatTime(evt.capturedAt)}
                </Typography>
              </Box>
              <IconButton
                className="delete-btn"
                size="small"
                onClick={(e) => { e.stopPropagation(); onDelete(evt.id); }}
                sx={{ opacity: 0, transition: "opacity 0.15s", p: 0.25 }}
              >
                <CloseIcon sx={{ fontSize: 12 }} />
              </IconButton>
            </Box>
          ))}
        </Box>
      )}
    </Box>
  );
}
