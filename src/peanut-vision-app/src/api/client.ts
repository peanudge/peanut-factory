import { API_BASE_URL } from '@/constants'
import type {
  BoardInfo,
  BoardStatus,
  CamFileInfo,
  AcquisitionStatus,
  ApiMessage,
  HistogramData,
  AcquisitionConfigPreset,
  ImagePage,
  LatencyRecord,
  LatencyStats,
} from "./types";

export interface CaptureResult {
  blob: Blob;
  savedPath?: string;
}

export class ApiError extends Error {
  readonly errorCode: string;
  readonly statusCode: number;

  constructor(message: string, errorCode: string, statusCode: number) {
    super(message);
    this.name = "ApiError";
    this.errorCode = errorCode;
    this.statusCode = statusCode;
  }
}

async function handleErrorResponse(res: Response): Promise<never> {
  const body = await res.json().catch(() => ({}));
  throw new ApiError(
    body.error ?? body.message ?? `HTTP ${res.status}`,
    body.errorCode ?? "UNKNOWN_ERROR",
    res.status,
  );
}

async function request<T>(
  path: string,
  options?: RequestInit,
): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });
  if (!res.ok) await handleErrorResponse(res);
  return res.json();
}

// ── Filesystem ──

export interface DirectoryEntry {
  name: string
  path: string
  hasChildren: boolean
}

export function getFilesystemDefaults(): Promise<{ desktopPath: string }> {
  return request("/filesystem/defaults")
}

export function getFilesystemRoots(): Promise<string[]> {
  return request("/filesystem/roots")
}

export function listDirectory(path: string): Promise<DirectoryEntry[]> {
  return request(`/filesystem/list?path=${encodeURIComponent(path)}`)
}

export function validatePath(path: string): Promise<{ writable: boolean; path: string }> {
  return request(`/filesystem/validate?path=${encodeURIComponent(path)}`)
}

// ── System ──

export function getBoards(): Promise<BoardInfo[]> {
  return request("/system/boards");
}

export function getBoardStatus(index: number): Promise<BoardStatus> {
  return request(`/system/boards/${index}/status`);
}

export function getCameras(): Promise<CamFileInfo[]> {
  return request("/system/cameras");
}

// ── Acquisition ──

export interface StartAcquisitionParams {
  profileId: string
  frameCount?: number | null
  intervalMs?: number | null
  outputDirectory?: string
  format?: string
}

export function startAcquisition(params: StartAcquisitionParams): Promise<ApiMessage & { profileId: string }> {
  return request("/acquisition/start", {
    method: "POST",
    body: JSON.stringify(params),
  });
}

export function stopAcquisition(): Promise<ApiMessage> {
  return request("/acquisition/stop", { method: "POST" });
}

export function getAcquisitionStatus(): Promise<AcquisitionStatus> {
  return request("/acquisition/status");
}

export async function triggerAndCapture(): Promise<CaptureResult> {
  const res = await fetch(`${API_BASE_URL}/acquisition/trigger`, {
    method: "POST",
  });
  if (!res.ok) await handleErrorResponse(res);
  const savedPath = res.headers.get("X-Image-Path") ?? undefined;
  return { blob: await res.blob(), savedPath };
}

export async function getLatestFrame(): Promise<CaptureResult | null> {
  const res = await fetch(`${API_BASE_URL}/acquisition/latest-frame`);
  if (res.status === 204) return null;
  if (!res.ok) await handleErrorResponse(res);
  const savedPath = res.headers.get("X-Image-Path") ?? undefined;
  return { blob: await res.blob(), savedPath };
}

export async function getHistogram(): Promise<HistogramData | null> {
  const res = await fetch(`${API_BASE_URL}/acquisition/latest-frame/histogram`);
  if (res.status === 204) return null;
  if (!res.ok) await handleErrorResponse(res);
  return res.json();
}

// ── Presets ──

export function getPresets(): Promise<AcquisitionConfigPreset[]> {
  return request("/presets");
}

export function savePreset(preset: AcquisitionConfigPreset): Promise<AcquisitionConfigPreset> {
  return request("/presets", {
    method: "PUT",
    body: JSON.stringify(preset),
  });
}

export function deletePreset(name: string): Promise<void> {
  return fetch(`${API_BASE_URL}/presets/${encodeURIComponent(name)}`, { method: "DELETE" })
    .then((res) => {
      if (!res.ok) return res.json().then((b) => {
        throw new ApiError(b.error ?? `HTTP ${res.status}`, b.errorCode ?? "UNKNOWN_ERROR", res.status);
      });
    });
}

// ── Images ──

export function listImages(params: {
  page?: number;
  pageSize?: number;
  dateFrom?: string;
  dateTo?: string;
} = {}): Promise<ImagePage> {
  const qs = new URLSearchParams();
  if (params.page != null)      qs.set("page", String(params.page));
  if (params.pageSize != null)  qs.set("pageSize", String(params.pageSize));
  if (params.dateFrom)          qs.set("dateFrom", params.dateFrom);
  if (params.dateTo)            qs.set("dateTo", params.dateTo);
  return request(`/images?${qs}`);
}

export function deleteImage(id: string): Promise<void> {
  return fetch(`${API_BASE_URL}/images/${id}`, { method: "DELETE" }).then((res) => {
    if (!res.ok) return res.json().then((b) => {
      throw new ApiError(b.error ?? `HTTP ${res.status}`, b.errorCode ?? "UNKNOWN_ERROR", res.status);
    });
  });
}

export function imageFileUrl(id: string): string {
  return `${API_BASE_URL}/images/${id}/file`;
}

// ── Latency Analysis ──

export function getLatencyRecords(limit = 200): Promise<LatencyRecord[]> {
  return request(`/latency/records?limit=${limit}`);
}

export function getLatencyStats(): Promise<LatencyStats | null> {
  return fetch(`${API_BASE_URL}/latency/stats`)
    .then((res) => {
      if (res.status === 204) return null;
      if (!res.ok) return res.json().then((b: Record<string, unknown>) => {
        throw new ApiError(String(b.error ?? `HTTP ${res.status}`), String(b.errorCode ?? "UNKNOWN_ERROR"), res.status);
      });
      return res.json();
    });
}

export function clearLatencyRecords(): Promise<ApiMessage> {
  return request("/latency/records", { method: "DELETE" });
}

