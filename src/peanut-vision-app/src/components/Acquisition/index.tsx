import { useLiveStream } from '@/hooks/useLiveStream'
import { useAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import { useAcquisitionSession } from '@/hooks/useAcquisitionSession'
import CaptureTab from './CaptureTab'
import ImageViewer from '@/components/shared/ImageViewer'
import cx from './cx'

export default function Acquisition() {
  const acqConfig = useAcquisitionConfig()
  const session = useAcquisitionSession(acqConfig.config)
  const live = useLiveStream()

  return (
    <div className={cx('Acquisition')}>
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

      <div className={cx('sidebar')}>
        <CaptureTab acqConfig={acqConfig} session={session} />
      </div>
    </div>
  )
}
