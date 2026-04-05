import Box from "@mui/material/Box";
import ImageViewer from "../components/ImageViewer";
import ImageGallery from "../components/ImageGallery";
import { useImageGallery } from "../hooks/useImageGallery";

export default function GalleryTab() {
  const gallery = useImageGallery();

  return (
    <Box sx={{ display: "flex", flexGrow: 1, overflow: "hidden", height: "100%" }}>
      {/* 좌: 선택 이미지 뷰어 */}
      <Box
        sx={{
          width: 480,
          flexShrink: 0,
          borderRight: "1px solid",
          borderColor: "divider",
          p: 2,
          display: "flex",
          flexDirection: "column",
        }}
      >
        <ImageViewer
          url={gallery.selectedImageUrl}
          filename={gallery.selectedImage?.filename}
          savedPath={gallery.selectedImage?.filePath}
          isLive={false}
          capturedAt={
            gallery.selectedImage ? new Date(gallery.selectedImage.capturedAt) : null
          }
          onClose={() => gallery.setSelectedId(null)}
        />
      </Box>

      {/* 우: 갤러리 그리드 */}
      <Box sx={{ flexGrow: 1, overflow: "hidden", p: 2 }}>
        <ImageGallery
          images={gallery.images}
          selectedId={gallery.selectedId}
          onSelect={gallery.setSelectedId}
          onDelete={gallery.handleDelete}
          page={gallery.page}
          totalPages={gallery.totalPages}
          onPageChange={gallery.setPage}
          filterSessionId={gallery.filterSessionId}
          onFilterChange={gallery.setFilterSessionId}
          isLoading={gallery.isLoading}
        />
      </Box>
    </Box>
  );
}
