import { useState } from 'react'
import { useAcquisitionActions } from '@/hooks/useAcquisitionActions'
import { useLiveStream } from '@/hooks/useLiveStream'
import { useResizablePanel } from '@/hooks/useResizablePanel'
import AcquisitionControls from '@/components/shared/AcquisitionControls'
import ContinuousSettings from '@/components/shared/ContinuousSettings'
import ExposureControl from '@/components/shared/ExposureControl'
import CalibrationActions from '@/components/shared/CalibrationActions'
import PresetSelector from '@/components/shared/PresetSelector'
import SessionSelector from '@/components/shared/SessionSelector'
import ImageSaveSettingsPanel from '@/components/shared/ImageSaveSettingsPanel'
import ImageViewer from '@/components/shared/ImageViewer'
import cx from './cx'

export default function Acquisition() {
  const acq = useAcquisitionActions()
  const live = useLiveStream()
  const { panelRef: sidebarRef, onResizerMouseDown } = useResizablePanel({
    defaultWidth: 340,
    min: 260,
    max: 480,
    direction: 'left',
  })

  const [sidebarTab, setSidebarTab] = useState(0)

  return (
    <div className={cx('Acquisition')}>
      {/* Sidebar */}
      <div ref={sidebarRef} className={cx('sidebar')}>
        <div className={cx('sidebarTabs')}>
          {['Capture', 'Camera', 'Settings'].map((label, i) => (
            <button
              key={label}
              type="button"
              className={cx('tab', { active: sidebarTab === i })}
              onClick={() => setSidebarTab(i)}
            >
              {label}
            </button>
          ))}
        </div>

        {/* Tab 0: Capture */}
        <div className={cx('tabPanel', { hidden: sidebarTab !== 0 })}>
          <PresetSelector
            profileId={acq.selectedProfile}
            triggerMode={acq.triggerMode}
            frameCount={acq.frameCount}
            intervalMs={acq.intervalMs}
            onLoadPreset={acq.handleLoadPreset}
            disabled={acq.acquisitionStatus?.isActive}
          />
          <AcquisitionControls
            cameras={acq.cameras}
            selectedProfile={acq.selectedProfile}
            onProfileChange={acq.setSelectedProfile}
            continuousSubMode={acq.continuousSubMode}
            triggerMode={acq.triggerMode}
            onTriggerModeChange={acq.setTriggerMode}
            status={acq.acquisitionStatus}
            busy={acq.busy}
            onStart={acq.handleStart}
            onStop={acq.handleStop}
            onTrigger={acq.handleTrigger}
            onRefresh={acq.refresh}
            refreshThrottled={acq.throttled}
            hasWarnings={acq.hasWarnings}
            hasErrors={acq.hasErrors}
          />
          <ContinuousSettings
            subMode={acq.continuousSubMode}
            onSubModeChange={acq.setContinuousSubMode}
            frameCount={acq.frameCount}
            onFrameCountChange={acq.setFrameCount}
            intervalMs={acq.intervalMs}
            onIntervalMsChange={acq.setIntervalMs}
            disabled={acq.acquisitionStatus?.isActive}
          />
        </div>

        {/* Tab 1: Camera */}
        <div className={cx('tabPanel', { hidden: sidebarTab !== 1 })}>
          <ExposureControl
            exposure={acq.exposure}
            exposureValue={acq.exposureValue}
            isActive={acq.acquisitionStatus?.isActive ?? false}
            busy={acq.busy}
            isCalibrationAvailable={acq.isCalibrationAvailable}
            onExposureChange={acq.setExposureValue}
            onLoad={acq.handleLoadExposure}
            onApply={acq.handleApplyExposure}
          />
          <CalibrationActions
            busy={acq.busy}
            isCalibrationAvailable={acq.isCalibrationAvailable}
            ffcEnabled={acq.ffcEnabled}
            onBlack={acq.handleBlack}
            onWhite={acq.handleWhite}
            onWhiteBalance={acq.handleWhiteBalance}
            onFfcToggle={acq.handleFfcToggle}
          />
        </div>

        {/* Tab 2: Settings */}
        <div className={cx('tabPanel', { hidden: sidebarTab !== 2 })}>
          <ImageSaveSettingsPanel />
          <SessionSelector />
        </div>
      </div>

      {/* Resize handle */}
      <div className={cx('resizer')} onMouseDown={onResizerMouseDown} />

      {/* Main canvas */}
      <div className={cx('canvas')}>
        <div className={cx('imageArea')}>
          <ImageViewer
            url={live.previewUrl}
            errorMessage={acq.acquisitionStatus?.lastError}
            isLive={live.isActive}
            capturedAt={null}
            onClose={() => {}}
          />
        </div>
      </div>
    </div>
  )
}
