import { API_BASE_URL } from "../constants";
import type {
  BoardInfo,
  BoardStatus,
  CamFileInfo,
  AcquisitionStatus,
  ExposureInfo,
  ApiMessage,
  ImageSaveSettings,
  Session,
  HistogramData,
  AcquisitionPreset,
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

export function startAcquisition(
  profileId: string,
  triggerMode?: string,
  frameCount?: number | null,
  intervalMs?: number | null,
): Promise<ApiMessage & { profileId: string }> {
  return request("/acquisition/start", {
    method: "POST",
    body: JSON.stringify({ profileId, triggerMode, frameCount, intervalMs }),
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

// ── Settings ──

export function getImageSaveSettings(): Promise<ImageSaveSettings> {
  return request("/settings/image-save");
}

export function updateImageSaveSettings(
  settings: ImageSaveSettings,
): Promise<ImageSaveSettings> {
  return request("/settings/image-save", {
    method: "PUT",
    body: JSON.stringify(settings),
  });
}

// ── Sessions ──

export function getSessions(limit = 50): Promise<Session[]> {
  return request(`/sessions?limit=${limit}`);
}

export function getActiveSession(): Promise<Session | null> {
  return fetch(`${API_BASE_URL}/sessions/active`)
    .then((res) => {
      if (res.status === 204) return null;
      if (!res.ok) return res.json().then((b) => {
        throw new ApiError(b.error ?? `HTTP ${res.status}`, b.errorCode ?? "UNKNOWN_ERROR", res.status);
      });
      return res.json();
    });
}

export function createSession(name: string, notes?: string): Promise<Session> {
  return request("/sessions", {
    method: "POST",
    body: JSON.stringify({ name, notes }),
  });
}

export function endSession(id: string): Promise<Session> {
  return request(`/sessions/${id}/end`, { method: "POST" });
}

export function deleteSession(id: string): Promise<void> {
  return fetch(`${API_BASE_URL}/sessions/${id}`, { method: "DELETE" })
    .then((res) => {
      if (!res.ok) return res.json().then((b) => {
        throw new ApiError(b.error ?? `HTTP ${res.status}`, b.errorCode ?? "UNKNOWN_ERROR", res.status);
      });
    });
}

// ── Presets ──

export function getPresets(): Promise<AcquisitionPreset[]> {
  return request("/presets");
}

export function savePreset(preset: AcquisitionPreset): Promise<AcquisitionPreset> {
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
  sessionId?: string;
  dateFrom?: string;
  dateTo?: string;
} = {}): Promise<ImagePage> {
  const qs = new URLSearchParams();
  if (params.page != null)      qs.set("page", String(params.page));
  if (params.pageSize != null)  qs.set("pageSize", String(params.pageSize));
  if (params.sessionId)         qs.set("sessionId", params.sessionId);
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

export function thumbnailUrl(id: string): string {
  return `${API_BASE_URL}/images/${id}/thumbnail`;
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

// ── Calibration ──

export function blackCalibration(): Promise<ApiMessage> {
  return request("/calibration/black", { method: "POST" });
}

export function whiteCalibration(): Promise<ApiMessage> {
  return request("/calibration/white", { method: "POST" });
}

export function whiteBalance(): Promise<ApiMessage> {
  return request("/calibration/white-balance", { method: "POST" });
}

export function setFfc(enable: boolean): Promise<ApiMessage> {
  return request("/calibration/ffc", {
    method: "POST",
    body: JSON.stringify({ enable }),
  });
}

export function getExposure(): Promise<ExposureInfo> {
  return request("/calibration/exposure");
}

export function setExposure(
  exposureUs?: number,
): Promise<ApiMessage & { exposureUs: number }> {
  return request("/calibration/exposure", {
    method: "PUT",
    body: JSON.stringify({ exposureUs }),
  });
}
