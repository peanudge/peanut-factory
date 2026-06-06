import ExposureControl from '@/components/shared/ExposureControl'
import CalibrationActions from '@/components/shared/CalibrationActions'
import type { CameraCalibration } from '@/hooks/useCameraCalibration'

interface Props {
  calibration: CameraCalibration
}

export default function CameraTab({ calibration }: Props) {
  return (
    <>
      <ExposureControl
        exposure={calibration.exposure}
        exposureValue={calibration.exposureValue}
        isActive={calibration.isCalibrationAvailable}
        busy={calibration.busy}
        isCalibrationAvailable={calibration.isCalibrationAvailable}
        onExposureChange={calibration.setExposureValue}
        onLoad={calibration.handleLoadExposure}
        onApply={calibration.handleApplyExposure}
      />
      <CalibrationActions
        busy={calibration.busy}
        isCalibrationAvailable={calibration.isCalibrationAvailable}
        ffcEnabled={calibration.ffcEnabled}
        onBlack={calibration.handleBlack}
        onWhite={calibration.handleWhite}
        onWhiteBalance={calibration.handleWhiteBalance}
        onFfcToggle={calibration.handleFfcToggle}
      />
    </>
  )
}
