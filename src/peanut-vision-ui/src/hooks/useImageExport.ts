import { useState } from "react";
import JSZip from "jszip";
import { saveAs } from "file-saver";
import type { CapturedImage } from "../api/types";
import { formatCaptureFilename } from "../utils/formatTimestamp";

/** Encapsulates ZIP export logic for a collection of captured images. */
export function useImageExport(images: CapturedImage[]) {
  const [exporting, setExporting] = useState(false);

  const handleExportZip = async () => {
    if (images.length === 0) return;
    setExporting(true);
    try {
      const zip = new JSZip();
      for (let i = 0; i < images.length; i++) {
        const image = images[i];
        zip.file(formatCaptureFilename(image.capturedAt, i + 1), image.blob);
      }
      const blob = await zip.generateAsync({ type: "blob" });
      saveAs(blob, `captures_${new Date().toISOString().slice(0, 10)}.zip`);
    } finally {
      setExporting(false);
    }
  };

  return { exporting, handleExportZip };
}
