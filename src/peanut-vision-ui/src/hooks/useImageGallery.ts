import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { listImages, deleteImage, imageFileUrl } from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { useToast } from "../contexts/ToastContext";
import type { CapturedImageRecord } from "../api/types";

const PAGE_SIZE = 20;

export function useImageGallery() {
  const queryClient = useQueryClient();
  const { toast } = useToast();
  const [page, setPage] = useState(1);
  const [filterSessionId, setFilterSessionId] = useState<string | null>(null);
  const [filterFromDate, setFilterFromDate] = useState<string | null>(null);
  const [filterToDate, setFilterToDate] = useState<string | null>(null);
  const [filterFormat, setFilterFormat] = useState<string | null>(null);
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const queryParams = {
    page,
    pageSize: PAGE_SIZE,
    ...(filterSessionId ? { sessionId: filterSessionId } : {}),
    ...(filterFromDate ? { dateFrom: filterFromDate } : {}),
    ...(filterToDate ? { dateTo: filterToDate } : {}),
    ...(filterFormat ? { format: filterFormat } : {}),
  };

  const { data, isLoading } = useQuery({
    queryKey: queryKeys.images(queryParams),
    queryFn: () => listImages(queryParams),
  });

  // Reset to page 1 when filters change
  useEffect(() => { setPage(1); }, [filterSessionId, filterFromDate, filterToDate, filterFormat]);

  // Auto-select the newest image when nothing is selected
  useEffect(() => {
    if (data?.items.length && selectedId === null) {
      setSelectedId(data.items[0].id);
    }
  }, [data, selectedId]);

  const deleteMutation = useMutation({
    mutationFn: deleteImage,
    onSuccess: (_: void, id: string) => {
      queryClient.invalidateQueries({ queryKey: queryKeys.images() });
      if (selectedId === id) setSelectedId(null);
      toast("이미지가 삭제되었습니다", "info");
    },
    onError: (e: unknown) =>
      toast(e instanceof Error ? e.message : "삭제에 실패했습니다", "error"),
  });

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: queryKeys.images() });
  };

  const images: CapturedImageRecord[] = data?.items ?? [];
  const selectedImage = images.find((i) => i.id === selectedId) ?? null;
  const selectedImageUrl = selectedId ? imageFileUrl(selectedId) : null;

  return {
    images,
    totalCount: data?.totalCount ?? 0,
    totalPages: data?.totalPages ?? 0,
    page,
    setPage,
    isLoading,
    filterSessionId,
    setFilterSessionId,
    filterFromDate,
    setFilterFromDate,
    filterToDate,
    setFilterToDate,
    filterFormat,
    setFilterFormat,
    selectedId,
    setSelectedId,
    selectedImage,
    selectedImageUrl,
    handleDelete: (id: string) => deleteMutation.mutate(id),
    invalidate,
  };
}
