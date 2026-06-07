import { useState } from 'react'
import { useLiveStream } from '@/hooks/useLiveStream'
import { useResizablePanel } from '@/hooks/useResizablePanel'
import { useAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import { useAcquisitionSession } from '@/hooks/useAcquisitionSession'
import CaptureTab from './CaptureTab'
import ImageSaveSettingsPanel from '@/components/shared/ImageSaveSettingsPanel'
import ImageViewer from '@/components/shared/ImageViewer'
import cx from './cx'

const TABS = ['Capture', 'Settings'] as const

export default function Acquisition() {
  const config = useAcquisitionConfig()
  const session = useAcquisitionSession(config)
  const live = useLiveStream()
  const { panelRef: sidebarRef, onResizerMouseDown } = useResizablePanel({
    defaultWidth: 340,
    min: 260,
    max: 480,
    direction: 'left',
  })
  const [activeTab, setActiveTab] = useState(0)

  return (
    <div className={cx('Acquisition')}>
      <div ref={sidebarRef} className={cx('sidebar')}>
        <div className={cx('sidebarTabs')}>
          {TABS.map((label, i) => (
            <button
              key={label}
              type="button"
              className={cx('tab', { active: activeTab === i })}
              onClick={() => setActiveTab(i)}
            >
              {label}
            </button>
          ))}
        </div>

        <div className={cx('tabPanel', { hidden: activeTab !== 0 })}>
          <CaptureTab config={config} session={session} />
        </div>

        <div className={cx('tabPanel', { hidden: activeTab !== 1 })}>
          <ImageSaveSettingsPanel />
        </div>
      </div>

      <div className={cx('resizer')} onMouseDown={onResizerMouseDown} />

      <div className={cx('canvas')}>
        <div className={cx('imageArea')}>
          <ImageViewer
            url={live.previewUrl}
            errorMessage={session.status?.lastError}
            isLive={live.isActive}
            capturedAt={null}
            onClose={() => {}}
          />
        </div>
      </div>
    </div>
  )
}
