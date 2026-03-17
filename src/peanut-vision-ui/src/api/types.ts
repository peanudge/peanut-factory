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

export interface AcquisitionStatus {
  isActive: boolean;
  profileId?: string;
  hasFrame?: boolean;
  lastError?: string | null;
  allowedActions?: string[];
  statistics?: AcquisitionStatistics;
  recentEvents?: ChannelEvent[];
}

export interface ExposureInfo {
  exposureUs: number;
  exposureRange?: { min: number; max: number };
  gainDb: number;
}

export interface ApiMessage {
  message: string;
}

export interface ApiError {
  error: string;
}
