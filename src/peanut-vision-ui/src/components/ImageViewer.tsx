import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import Typography from "@mui/material/Typography";
import ImageActionBar from "./ImageActionBar";
import HistogramDisplay from "./HistogramDisplay";
import AnnotationPanel from "./AnnotationPanel";
import { formatTime } from "../utils/formatTimestamp";
import { useCallback, useEffect, useRef, useState } from "react";

interface Props {
  url: string | null;
  filename?: string;
  errorMessage?: string | null;
  savedPath?: string;
  isLive: boolean;
  capturedAt: Date | null;
  imageId?: string | null;
  tags?: string[];
  notes?: string;
  onReturnToLive: () => void;
}

const MIN_SCALE = 1;
const MAX_SCALE = 10;

function clamp(value: number, min: number, max: number): number {
  return Math.min(Math.max(value, min), max);
}

export default function ImageViewer({ url, filename, errorMessage, savedPath, isLive, capturedAt, imageId, tags, notes, onReturnToLive }: Props) {
  const [scale, setScale] = useState(1);
  const [translate, setTranslate] = useState({ x: 0, y: 0 });
  const isDragging = useRef(false);
  const dragStart = useRef({ x: 0, y: 0 });
  const translateAtDragStart = useRef({ x: 0, y: 0 });
  const containerRef = useRef<HTMLDivElement>(null);

  // Reset zoom/pan when a new live frame arrives
  useEffect(() => {
    if (isLive) {
      setScale(1);
      setTranslate({ x: 0, y: 0 });
    }
  }, [isLive, url]);

  const resetView = useCallback(() => {
    setScale(1);
    setTranslate({ x: 0, y: 0 });
  }, []);

  const handleWheel = useCallback((e: React.WheelEvent<HTMLDivElement>) => {
    e.preventDefault();
    const delta = e.deltaY < 0 ? 1.15 : 1 / 1.15;
    setScale((prev) => {
      const next = clamp(prev * delta, MIN_SCALE, MAX_SCALE);
      // If zooming back to 1, also reset translation
      if (next === MIN_SCALE) {
        setTranslate({ x: 0, y: 0 });
      }
      return next;
    });
  }, []);

  const handleMouseDown = useCallback(
    (e: React.MouseEvent<HTMLDivElement>) => {
      if (scale <= 1) return;
      isDragging.current = true;
      dragStart.current = { x: e.clientX, y: e.clientY };
      translateAtDragStart.current = { ...translate };
      e.preventDefault();
    },
    [scale, translate]
  );

  const handleMouseMove = useCallback((e: React.MouseEvent<HTMLDivElement>) => {
    if (!isDragging.current) return;
    const dx = e.clientX - dragStart.current.x;
    const dy = e.clientY - dragStart.current.y;
    setTranslate({
      x: translateAtDragStart.current.x + dx,
      y: translateAtDragStart.current.y + dy,
    });
  }, []);

  const handleMouseUp = useCallback(() => {
    isDragging.current = false;
  }, []);

  const handleDblClick = useCallback(() => {
    resetView();
  }, [resetView]);

  if (!url) {
    return (
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          height: "100%",
          minHeight: 200,
          border: "1px dashed",
          borderColor: "divider",
          borderRadius: 1,
        }}
      >
        <Typography color="text.secondary">No captured frame</Typography>
      </Box>
    );
  }

  const showZoomBadge = scale > 1;

  return (
    <Box sx={{ display: "flex", flexDirection: "column", height: "100%", gap: 1 }}>
      <Box
        ref={containerRef}
        onWheel={handleWheel}
        onMouseDown={handleMouseDown}
        onMouseMove={handleMouseMove}
        onMouseUp={handleMouseUp}
        onMouseLeave={handleMouseUp}
        onDoubleClick={handleDblClick}
        sx={{
          position: "relative",
          border: "1px solid",
          borderColor: "divider",
          borderRadius: 1,
          overflow: "hidden",
          flexGrow: 1,
          minHeight: 0,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          bgcolor: "background.default",
          cursor: scale > 1 ? (isDragging.current ? "grabbing" : "grab") : "default",
          userSelect: "none",
        }}
      >
        <img
          src={url}
          alt="Captured frame"
          draggable={false}
          style={{
            maxWidth: "100%",
            maxHeight: "100%",
            display: "block",
            objectFit: "contain",
            transform: `scale(${scale}) translate(${translate.x / scale}px, ${translate.y / scale}px)`,
            transformOrigin: "center center",
            transition: isDragging.current ? "none" : "transform 0.05s ease-out",
          }}
        />
        {errorMessage && (
          <Box sx={{ position: "absolute", top: 8, left: 8 }}>
            <Chip
              size="small"
              label={errorMessage}
              color="error"
              sx={{ fontWeight: 600, fontSize: "0.7rem" }}
            />
          </Box>
        )}
        <Box sx={{ position: "absolute", top: 8, right: 8, display: "flex", alignItems: "center", gap: 0.5 }}>
          {showZoomBadge && (
            <Chip
              size="small"
              label={`${scale.toFixed(1)}×`}
              sx={{
                fontWeight: 700,
                fontSize: "0.65rem",
                bgcolor: "rgba(0,0,0,0.6)",
                color: "#fff",
              }}
            />
          )}
          {isLive ? (
            <Chip
              size="small"
              label="LIVE"
              color="success"
              sx={{ fontWeight: 700, fontSize: "0.65rem" }}
            />
          ) : (
            <>
              <Chip
                size="small"
                label={capturedAt ? formatTime(capturedAt) : "Captured"}
                sx={{
                  fontWeight: 600,
                  fontSize: "0.65rem",
                  bgcolor: "rgba(0,0,0,0.55)",
                  color: "#fff",
                }}
              />
              <Chip
                size="small"
                label="Return to Live"
                onClick={onReturnToLive}
                variant="outlined"
                color="primary"
                sx={{ fontWeight: 600, fontSize: "0.65rem", cursor: "pointer", bgcolor: "rgba(0,0,0,0.4)" }}
              />
            </>
          )}
        </Box>
      </Box>
      <Box sx={{ flexShrink: 0 }}>
        <ImageActionBar url={url} filename={filename} savedPath={savedPath} />
      </Box>
      {!isLive && imageId && (
        <Box sx={{ flexShrink: 0, px: 0.5 }}>
          <HistogramDisplay imageId={imageId} />
        </Box>
      )}
      {!isLive && imageId && (
        <Box sx={{ flexShrink: 0, border: "1px solid", borderColor: "divider", borderRadius: 1 }}>
          <AnnotationPanel
            imageId={imageId}
            initialTags={tags ?? []}
            initialNotes={notes ?? ""}
          />
        </Box>
      )}
    </Box>
  );
}
