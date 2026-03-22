import { useRef, useEffect } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import Tooltip from "@mui/material/Tooltip";
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

const TILE_SIZE = 68;

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
          overflowY: "auto",
          "&::-webkit-scrollbar": { width: 4 },
          "&::-webkit-scrollbar-thumb": { borderRadius: 2, bgcolor: "divider" },
        }}>
          <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5, p: 0.5 }}>
            {events.map((evt) => {
              const filename = getFilename(evt.filePath);
              const timestamp = formatTime(evt.capturedAt);
              const isSelected = evt.id === selectedId;

              return (
                <Tooltip key={evt.id} title={`${filename} — ${timestamp}`} arrow>
                  <Box
                    onClick={() => onSelect(evt.id)}
                    sx={{
                      width: TILE_SIZE,
                      height: TILE_SIZE,
                      position: "relative",
                      flexShrink: 0,
                      cursor: "pointer",
                      border: "2px solid",
                      borderColor: isSelected ? "primary.main" : "transparent",
                      borderRadius: 0.5,
                      overflow: "hidden",
                      bgcolor: "background.default",
                      "&:hover .del-btn": { opacity: 1 },
                    }}
                  >
                    {evt.objectUrl ? (
                      <img
                        src={evt.objectUrl}
                        alt=""
                        style={{ width: "100%", height: "100%", objectFit: "cover", display: "block" }}
                      />
                    ) : (
                      <Box sx={{
                        width: "100%", height: "100%",
                        display: "flex", alignItems: "center", justifyContent: "center",
                      }}>
                        <CameraAltIcon sx={{ fontSize: 24, color: "text.disabled" }} />
                      </Box>
                    )}
                    <IconButton
                      className="del-btn"
                      size="small"
                      onClick={(e) => { e.stopPropagation(); onDelete(evt.id); }}
                      sx={{
                        position: "absolute",
                        top: 2,
                        right: 2,
                        opacity: 0,
                        transition: "opacity 0.15s",
                        p: 0.25,
                        bgcolor: "rgba(0,0,0,0.5)",
                        color: "common.white",
                        "&:hover": { bgcolor: "rgba(0,0,0,0.7)" },
                      }}
                    >
                      <CloseIcon sx={{ fontSize: 12 }} />
                    </IconButton>
                  </Box>
                </Tooltip>
              );
            })}
          </Box>
        </Box>
      )}
    </Box>
  );
}
