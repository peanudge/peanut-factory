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

export interface CamFileInfo {
  fileName: string;
  manufacturer: string;
  cameraModel: string;
  width: number;
  height: number;
  spectrum: string;
  colorFormat: string;
  trigMode: string;
  acquisitionMode: string;
  tapConfiguration: string;
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

export type AcquisitionAction = "start" | "stop" | "trigger" | "snapshot";

export type ChannelState = "none" | "idle" | "active";

export type AcquisitionMode = "single" | "continuous";
export type ContinuousSubMode = "auto" | "manual";

export interface AcquisitionStatus {
  isActive: boolean;
  channelState?: ChannelState;
  profileId?: string;
  hasFrame?: boolean;
  lastError?: string | null;
  allowedActions?: AcquisitionAction[];
  statistics?: AcquisitionStatistics;
  recentEvents?: ChannelEvent[];
}

export interface ExposureInfo {
  exposureUs: number;
  exposureRange?: { min: number; max: number };
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

export type TriggerModeOption = "soft" | "hard" | "combined";

export type ColorFormat =
  | "Y8" | "Y10" | "Y12" | "Y16"
  | "BAYER8" | "BAYER10" | "BAYER12"
  | "RGB24" | "RGB8" | "RGB32" | "RGBa8"
  | "RGB24PL" | "RGB30PL" | "RGB36PL" | "RGB48PL"
  | (string & Record<never, never>); // allow unknown formats from server

export interface AcquisitionPreset {
  name: string;
  profileId: string;
  triggerMode?: TriggerModeOption | null;
  frameCount?: number | null;
  intervalMs?: number | null;
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
export type SubfolderStrategy = "none" | "byDate" | "bySession" | "byProfile";

export interface CapturedImageRecord {
  id: string;
  filePath: string;
  filename: string;
  hasThumbnail: boolean;
  width: number;
  height: number;
  fileSizeBytes: number;
  format: string;
  capturedAt: string;
  tags: string[];
  notes: string;
}

export interface ImagePage {
  items: CapturedImageRecord[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ImageSaveSettings {
  outputDirectory: string;
  format: SaveImageFormat;
  filenamePrefix: string;
  timestampFormat: string;
  includeSequenceNumber: boolean;
  subfolderStrategy: SubfolderStrategy;
  autoSave: boolean;
}
