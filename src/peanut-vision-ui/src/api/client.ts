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
} from "./types";

export interface CaptureResult {
  blob: Blob;
  savedPath?: string;
}

async function handleErrorResponse(res: Response): Promise<never> {
  const body = await res.json().catch(() => ({}));
  throw new Error(
    body.error ?? body.message ?? `HTTP ${res.status}`,
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

export async function snapshot(
  profileId: string,
  triggerMode?: string,
): Promise<CaptureResult> {
  const res = await fetch(`${API_BASE_URL}/acquisition/snapshot`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ profileId, triggerMode }),
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
      if (!res.ok) return res.json().then((b) => { throw new Error(b.error ?? `HTTP ${res.status}`); });
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
      if (!res.ok) return res.json().then((b) => { throw new Error(b.error ?? `HTTP ${res.status}`); });
    });
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
  gainDb?: number,
): Promise<ApiMessage & { exposureUs: number; gainDb: number }> {
  return request("/calibration/exposure", {
    method: "PUT",
    body: JSON.stringify({ exposureUs, gainDb }),
  });
}
