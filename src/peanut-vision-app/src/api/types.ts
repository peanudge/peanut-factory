export interface BoardInfo {
  index: number;
  boardType: string;
  boardName: string;
  serialNumber: string;
  pciPosition: string;
}

export interface BoardStatus {
  index: number;
  boardName: string;
  boardType: string;
  serialNumber: string;
  pciPosition: string;
  inputConnector: string;
  inputState: string;
  signalStrength: string;
  outputState: string;
  cameraLinkStatus: string;
  syncErrors: number;
  clockErrors: number;
  grabberErrors: number;
  frameTriggerViolations: number;
  lineTriggerViolations: number;
  pcieLinkInfo: string;
}


export interface AcquisitionStatistics {
  frameCount: number;
  droppedFrameCount: number;
  errorCount: number;
  elapsedMs: number;
  averageFps: number;
  minFrameIntervalMs: number;
  maxFrameIntervalMs: number;
  averageFrameIntervalMs: number;
  copyDropCount: number;
  clusterUnavailableCount: number;
}

export interface ChannelEvent {
  timestamp: string;
  type: string;
  message: string;
}

export type AcquisitionAction = "start" | "stop" | "trigger";

export type ChannelState = "none" | "idle" | "active";

export type AcquisitionMode = "auto" | "manual";

/** The full form configuration a user fills in before starting acquisition. */
export interface AcquisitionFormConfig {
  profileId: string
  acquisitionMode: AcquisitionMode
  frameCount: number | null
  intervalMs: number | null
  outputDirectory: string
  format: SaveImageFormat
}

export const DEFAULT_ACQUISITION_FORM_CONFIG: AcquisitionFormConfig = {
  profileId: '',
  acquisitionMode: 'auto',
  frameCount: null,
  intervalMs: null,
  outputDirectory: 'CapturedImages',
  format: 'png',
}

export interface AcquisitionStatus {
  isActive: boolean;
  channelState?: ChannelState;
  profileId?: string;
  activeFrameCount?: number | null;
  activeIntervalMs?: number | null;
  outputDirectory?: string;
  format?: SaveImageFormat;
  hasFrame?: boolean;
  lastError?: string | null;
  allowedActions?: AcquisitionAction[];
  statistics?: AcquisitionStatistics;
  recentEvents?: ChannelEvent[];
}

export interface ApiMessage {
  message: string;
}

export interface ApiError {
  error: string;
}

export interface CapturedEvent {
  id: string;
  filePath: string;
  capturedAt: Date;
  objectUrl: string | null;
}

export interface HistogramData {
  red: number[];
  green: number[];
  blue: number[];
  bins: number;
}


export type ColorFormat =
  | "Y8" | "Y10" | "Y12" | "Y16"
  | "BAYER8" | "BAYER10" | "BAYER12"
  | "RGB24" | "RGB8" | "RGB32" | "RGBa8"
  | "RGB24PL" | "RGB30PL" | "RGB36PL" | "RGB48PL"
  | (string & Record<never, never>); // allow unknown formats from server

/** Named snapshot of AcquisitionConfig for reuse across sessions. */
export interface AcquisitionConfigPreset {
  name: string;
  profileId: string;
  frameCount?: number | null;
  intervalMs?: number | null;
  outputDirectory?: string;
  format?: SaveImageFormat;
}

// ── Latency Analysis ──

export interface LatencyRecord {
  id: number;
  triggerSentAt: string;
  frameReceivedAt: string;
  latencyMs: number;
  frameIndex: number;
  profileId: string | null;
}

export interface LatencyStats {
  count: number;
  minMs: number;
  maxMs: number;
  meanMs: number;
  p50Ms: number;
  p95Ms: number;
  p99Ms: number;
  stdDevMs: number;
}

export type SaveImageFormat = "png" | "bmp" | "raw";

export interface CapturedImageRecord {
  id: string;
  filePath: string;
  filename: string;
  width: number;
  height: number;
  fileSizeBytes: number;
  format: string;
  capturedAt: string;
}

export interface ImagePage {
  items: CapturedImageRecord[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

