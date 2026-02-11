import { API_BASE_URL } from "../constants";
import type {
  BoardInfo,
  BoardStatus,
  CameraProfile,
  AcquisitionStatus,
  ExposureInfo,
  ApiMessage,
} from "./types";

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

export function getCameras(): Promise<CameraProfile[]> {
  return request("/system/cameras");
}

// ── Acquisition ──

export function startAcquisition(
  profileId: string,
  triggerMode?: string,
): Promise<ApiMessage & { profileId: string }> {
  return request("/acquisition/start", {
    method: "POST",
    body: JSON.stringify({ profileId, triggerMode }),
  });
}

export function stopAcquisition(): Promise<ApiMessage> {
  return request("/acquisition/stop", { method: "POST" });
}

export function getAcquisitionStatus(): Promise<AcquisitionStatus> {
  return request("/acquisition/status");
}

export function sendTrigger(): Promise<ApiMessage> {
  return request("/acquisition/trigger", { method: "POST" });
}

export async function captureFrame(): Promise<Blob> {
  const res = await fetch(`${API_BASE_URL}/acquisition/capture`, {
    method: "POST",
  });
  if (!res.ok) await handleErrorResponse(res);
  return res.blob();
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
