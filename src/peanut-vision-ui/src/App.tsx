import { BrowserRouter, Routes, Route } from "react-router-dom";
import CapturePage from "./pages/CapturePage";
import GalleryPage from "./pages/GalleryPage";
import BackofficePage from "./pages/BackofficePage";
import { useDiskUsageAlert } from "./hooks/useDiskUsageAlert";

/**
 * Null-rendering sentinel that activates the disk-usage alert side-effect
 * exactly once in the component tree. Placing it here ensures a single toast
 * fires per threshold-crossing event regardless of which page the user is on.
 */
function DiskUsageAlertEffect() {
  useDiskUsageAlert();
  return null;
}

export default function App() {
  return (
    <BrowserRouter>
      {/* Mount the alert effect at the root so it persists across page navigation */}
      <DiskUsageAlertEffect />
      <Routes>
        <Route path="/" element={<CapturePage />} />
        <Route path="/gallery" element={<GalleryPage />} />
        <Route path="/backoffice" element={<BackofficePage />} />
      </Routes>
    </BrowserRouter>
  );
}
