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
}

export interface Session {
  id: string;
  name: string;
  createdAt: string;
  endedAt: string | null;
  notes: string | null;
  isActive: boolean;
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

export type SaveImageFormat = "png" | "bmp" | "raw";
export type SubfolderStrategy = "none" | "byDate" | "bySession" | "byProfile";

export interface ImageSaveSettings {
  outputDirectory: string;
  format: SaveImageFormat;
  filenamePrefix: string;
  timestampFormat: string;
  includeSequenceNumber: boolean;
  subfolderStrategy: SubfolderStrategy;
  autoSave: boolean;
}
