import { useState } from 'react'
import { Square } from 'lucide-react'
import { useAcquisitionActions } from '@/hooks/useAcquisitionActions'
import { useLiveStream } from '@/hooks/useLiveStream'
import { useResizablePanel } from '@/hooks/useResizablePanel'
import AcquisitionControls from '@/components/shared/AcquisitionControls'
import ContinuousSettings from '@/components/shared/ContinuousSettings'
import ExposureControl from '@/components/shared/ExposureControl'
import CalibrationActions from '@/components/shared/CalibrationActions'
import PresetSelector from '@/components/shared/PresetSelector'
import ImageSaveSettingsPanel from '@/components/shared/ImageSaveSettingsPanel'
import ImageViewer from '@/components/shared/ImageViewer'
import StatusChip from '@/components/shared/StatusChip'
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
  const isActive = acq.acquisitionStatus?.isActive ?? false
  const status = acq.acquisitionStatus

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
          {isActive ? (
            /* ── 촬영 중: 현재 상태 readonly 표시 ── */
            <div className={cx('activeView')}>
              <div className={cx('activeHeader')}>
                <StatusChip
                  active
                  label={`Active — ${status?.profileId ?? ''}`}
                  hasWarnings={acq.hasWarnings}
                  hasErrors={acq.hasErrors}
                />
              </div>

              <dl className={cx('activeInfo')}>
                <div className={cx('infoRow')}>
                  <dt>Profile</dt>
                  <dd>{status?.profileId ?? '—'}</dd>
                </div>
                <div className={cx('infoRow')}>
                  <dt>Trigger</dt>
                  <dd>{status?.triggerMode ?? '—'}</dd>
                </div>
                <div className={cx('infoRow')}>
                  <dt>Frame count</dt>
                  <dd>{status?.activeFrameCount != null ? status.activeFrameCount : '∞'}</dd>
                </div>
                <div className={cx('infoRow')}>
                  <dt>Interval</dt>
                  <dd>{status?.activeIntervalMs != null ? `${status.activeIntervalMs} ms` : 'manual'}</dd>
                </div>
                {status?.statistics && (
                  <>
                    <div className={cx('infoRow')}>
                      <dt>Frames</dt>
                      <dd>{status.statistics.frameCount}</dd>
                    </div>
                    <div className={cx('infoRow')}>
                      <dt>FPS</dt>
                      <dd>{status.statistics.averageFps.toFixed(1)}</dd>
                    </div>
                    {status.statistics.droppedFrameCount > 0 && (
                      <div className={cx('infoRow', 'warn')}>
                        <dt>Dropped</dt>
                        <dd>{status.statistics.droppedFrameCount}</dd>
                      </div>
                    )}
                  </>
                )}
                {status?.lastError && (
                  <div className={cx('infoRow', 'error')}>
                    <dt>Error</dt>
                    <dd>{status.lastError}</dd>
                  </div>
                )}
              </dl>

              {/* Trigger button when manual mode */}
              {status?.activeIntervalMs == null && (
                <button
                  type="button"
                  className={cx('triggerBtn')}
                  onClick={acq.handleTrigger}
                  disabled={acq.busy}
                >
                  Trigger
                </button>
              )}

              <button
                type="button"
                className={cx('stopBtn')}
                onClick={acq.handleStop}
                disabled={acq.busy}
              >
                <Square size={14} /> Stop
              </button>
            </div>
          ) : (
            /* ── 촬영 중 아님: 설정 폼 ── */
            <>
              <PresetSelector
                profileId={acq.selectedProfile}
                triggerMode={acq.triggerMode}
                frameCount={acq.frameCount}
                intervalMs={acq.intervalMs}
                onLoadPreset={acq.handleLoadPreset}
                disabled={false}
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
                disabled={false}
              />
            </>
          )}
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
