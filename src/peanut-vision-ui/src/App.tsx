import { BrowserRouter, Routes, Route } from "react-router-dom";
import CapturePage from "./pages/CapturePage";
import GalleryPage from "./pages/GalleryPage";
import BackofficePage from "./pages/BackofficePage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<CapturePage />} />
        <Route path="/gallery" element={<GalleryPage />} />
        <Route path="/backoffice" element={<BackofficePage />} />
      </Routes>
    </BrowserRouter>
  );
}
