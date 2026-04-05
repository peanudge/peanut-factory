import { API_BASE_URL } from "../constants";
import type {
  BoardInfo,
  BoardStatus,
  CamFileInfo,
  AcquisitionStatus,
  ExposureInfo,
  ApiMessage,
  ImageSaveSettings,
  HistogramData,
  AcquisitionPreset,
  ImagePage,
  CapturedImageRecord,
  LatencyRecord,
  LatencyStats,
  DiskUsageDto,
  CaptureStat,
  HourlyStatsParams,
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

const DEFAULT_TIMEOUT_MS = 10_000;
const CALIBRATION_TIMEOUT_MS = 30_000;
const MAX_RETRIES = 3;

const CALIBRATION_PATHS = ["/calibration/black", "/calibration/white", "/calibration/white-balance"];

function isCalibrationPath(path: string): boolean {
  return CALIBRATION_PATHS.some((p) => path.startsWith(p));
}

function isRetryable(error: unknown): boolean {
  // Retry on network-level errors (TypeError: Failed to fetch, AbortError from timeout)
  if (error instanceof TypeError) return true;
  if (error instanceof ApiError) return error.statusCode >= 500;
  return false;
}

async function fetchWithTimeout(url: string, options: RequestInit, timeoutMs: number): Promise<Response> {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), timeoutMs);
  try {
    return await fetch(url, { ...options, signal: controller.signal });
  } finally {
    clearTimeout(timer);
  }
}

async function fetchWithRetry(url: string, options: RequestInit, timeoutMs: number): Promise<Response> {
  let lastError: unknown;
  for (let attempt = 0; attempt <= MAX_RETRIES; attempt++) {
    if (attempt > 0) {
      const delayMs = Math.pow(2, attempt - 1) * 1000; // 1s, 2s, 4s
      await new Promise((resolve) => setTimeout(resolve, delayMs));
    }
    try {
      const res = await fetchWithTimeout(url, options, timeoutMs);
      // Don't retry 4xx — only 5xx is retryable
      if (res.ok || (res.status >= 400 && res.status < 500)) return res;
      // 5xx: treat as retryable error unless it's the last attempt
      if (attempt === MAX_RETRIES) return res;
      lastError = new ApiError(`HTTP ${res.status}`, "SERVER_ERROR", res.status);
    } catch (err) {
      lastError = err;
      if (!isRetryable(err) || attempt === MAX_RETRIES) throw err;
    }
  }
  throw lastError;
}

async function request<T>(
  path: string,
  options?: RequestInit,
): Promise<T> {
  const timeoutMs = isCalibrationPath(path) ? CALIBRATION_TIMEOUT_MS : DEFAULT_TIMEOUT_MS;
  const res = await fetchWithRetry(`${API_BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  }, timeoutMs);
  if (!res.ok) await handleErrorResponse(res);
  return res.json();
}

async function rawFetch(path: string, options?: RequestInit): Promise<Response> {
  const timeoutMs = isCalibrationPath(path) ? CALIBRATION_TIMEOUT_MS : DEFAULT_TIMEOUT_MS;
  return fetchWithRetry(`${API_BASE_URL}${path}`, options ?? {}, timeoutMs);
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
  const res = await rawFetch("/acquisition/trigger", { method: "POST" });
  if (!res.ok) await handleErrorResponse(res);
  const savedPath = res.headers.get("X-Image-Path") ?? undefined;
  return { blob: await res.blob(), savedPath };
}

export async function snapshot(
  profileId: string,
  triggerMode?: string,
): Promise<CaptureResult> {
  const res = await rawFetch("/acquisition/snapshot", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ profileId, triggerMode }),
  });
  if (!res.ok) await handleErrorResponse(res);
  const savedPath = res.headers.get("X-Image-Path") ?? undefined;
  return { blob: await res.blob(), savedPath };
}

export async function getLatestFrame(): Promise<CaptureResult | null> {
  const res = await rawFetch("/acquisition/latest-frame");
  if (res.status === 204) return null;
  if (!res.ok) await handleErrorResponse(res);
  const savedPath = res.headers.get("X-Image-Path") ?? undefined;
  return { blob: await res.blob(), savedPath };
}

export async function getHistogram(): Promise<HistogramData | null> {
  const res = await rawFetch("/acquisition/latest-frame/histogram");
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

/** Returns disk usage information for the configured output directory's drive. */
export function getDiskUsage(): Promise<DiskUsageDto> {
  return request("/settings/disk-usage");
}

/** Partially updates OutputDirectory and/or FilenamePrefix without touching other settings. */
export function patchImageSaveSettings(
  patch: { outputDirectory?: string; filenamePrefix?: string },
): Promise<ImageSaveSettings> {
  return request("/settings/image-save", {
    method: "PATCH",
    body: JSON.stringify(patch),
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
  return rawFetch(`/presets/${encodeURIComponent(name)}`, { method: "DELETE" })
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
  format?: string;
} = {}): Promise<ImagePage> {
  const qs = new URLSearchParams();
  if (params.page != null)      qs.set("page", String(params.page));
  if (params.pageSize != null)  qs.set("pageSize", String(params.pageSize));
  if (params.sessionId)         qs.set("sessionId", params.sessionId);
  if (params.dateFrom)          qs.set("dateFrom", params.dateFrom);
  if (params.dateTo)            qs.set("dateTo", params.dateTo);
  if (params.format)            qs.set("format", params.format);
  return request(`/images?${qs}`);
}

export function getImageHistogram(id: string): Promise<HistogramData> {
  return request(`/images/${id}/histogram`);
}

export function deleteImage(id: string): Promise<void> {
  return rawFetch(`/images/${id}`, { method: "DELETE" }).then((res) => {
    if (!res.ok) return res.json().then((b) => {
      throw new ApiError(b.error ?? `HTTP ${res.status}`, b.errorCode ?? "UNKNOWN_ERROR", res.status);
    });
  });
}

export async function exportImagesZip(ids: string[]): Promise<Blob> {
  const res = await rawFetch("/images/export", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ ids }),
  });
  if (!res.ok) await handleErrorResponse(res);
  return res.blob();
}

export function patchImageAnnotations(id: string, tags: string[], notes: string): Promise<CapturedImageRecord> {
  return request(`/images/${id}`, {
    method: "PATCH",
    body: JSON.stringify({ tags, notes }),
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
  return rawFetch("/latency/stats")
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

// ── Hourly Acquisition Statistics ──

/**
 * Fetches today's total acquisition count (UTC midnight to now).
 * Returns `{ count: number }`.
 */
export function getTodayCount(): Promise<{ count: number }> {
  return request("/stats/today");
}

/**
 * Fetches hourly acquisition statistics.
 *
 * - Mode 1 (default): pass `{ hours }` to get the most recent N hours.
 * - Mode 2 (range):   pass `{ from, to? }` for a specific UTC time range.
 *
 * Hours with zero captures are omitted by the server.
 */
export function getHourlyStats(params: HourlyStatsParams = {}): Promise<CaptureStat[]> {
  const qs = new URLSearchParams();
  if (params.from != null) {
    qs.set("from", params.from);
    if (params.to != null) qs.set("to", params.to);
  } else if (params.hours != null) {
    qs.set("hours", String(params.hours));
  }
  const query = qs.toString();
  return request(`/stats/hourly${query ? `?${query}` : ""}`);
}

// ── Calibration ──

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
