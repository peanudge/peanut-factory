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

export interface CameraProfile {
  id: string;
  displayName: string;
  manufacturer: string;
  model: string;
  connector: string;
  triggerMode: string;
  pixelFormat: string;
  expectedWidth: number;
  expectedHeight: number;
  description?: string;
}

export interface CamFiles {
  directory: string;
  files: string[];
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
}

export interface AcquisitionStatus {
  isActive: boolean;
  profileId?: string;
  hasFrame?: boolean;
  lastError?: string | null;
  statistics?: AcquisitionStatistics;
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
