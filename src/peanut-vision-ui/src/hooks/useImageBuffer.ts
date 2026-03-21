import { useCallback, useEffect, useState } from "react";
import type { CapturedImage } from "../api/types";

export function useImageBuffer(maxSize: number) {
  const [capturedFrames, setCapturedFrames] = useState<CapturedImage[]>([]);
  const [selectedFrameId, setSelectedFrameId] = useState<string | null>(null);

  const addFrame = useCallback((blob: Blob, savedPath?: string) => {
    const url = URL.createObjectURL(blob);
    const newFrame: CapturedImage = {
      id: crypto.randomUUID(),
      url,
      blob,
      capturedAt: new Date(),
      savedPath,
    };
    setCapturedFrames((prev) => {
      const next = [newFrame, ...prev];
      if (next.length > maxSize) {
        next.slice(maxSize).forEach((f) => URL.revokeObjectURL(f.url));
        return next.slice(0, maxSize);
      }
      return next;
    });
    setSelectedFrameId(newFrame.id);
  }, [maxSize]);

  const deleteFrame = useCallback((id: string) => {
    setCapturedFrames((prev) => {
      const target = prev.find((f) => f.id === id);
      if (target) URL.revokeObjectURL(target.url);
      return prev.filter((f) => f.id !== id);
    });
    setSelectedFrameId((prev) => (prev === id ? null : prev));
  }, []);

  const clearAllFrames = useCallback(() => {
    setCapturedFrames((prev) => {
      prev.forEach((f) => URL.revokeObjectURL(f.url));
      return [];
    });
    setSelectedFrameId(null);
  }, []);

  useEffect(() => {
    return () => {
      setCapturedFrames((prev) => {
        prev.forEach((f) => URL.revokeObjectURL(f.url));
        return prev;
      });
    };
  }, []);

  return {
    capturedFrames,
    selectedFrameId,
    addFrame,
    deleteFrame,
    clearAllFrames,
    selectFrame: setSelectedFrameId,
  };
}
