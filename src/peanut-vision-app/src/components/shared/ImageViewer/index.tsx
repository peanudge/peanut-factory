import ImageActionBar from '@/components/shared/ImageActionBar'
import { formatTime } from '@/utils/formatTimestamp'
import cx from './cx'

interface Props {
  url: string | null
  filename?: string
  errorMessage?: string | null
  savedPath?: string
  isLive: boolean
  capturedAt: Date | null
  onClose?: () => void
}

export default function ImageViewer({
  url,
  filename,
  errorMessage,
  savedPath,
  isLive,
  capturedAt,
  onClose,
}: Props) {
  if (!url) {
    return <div className={cx('empty')}>No captured frame</div>
  }

  return (
    <div className={cx('viewer')}>
      <div className={cx('imageWrap')}>
        <img src={url} alt="Captured frame" />

        {errorMessage && (
          <div className={cx('overlayTL')}>
            <span className={cx('chip', 'error')}>{errorMessage}</span>
          </div>
        )}

        <div className={cx('overlayTR')}>
          {isLive ? (
            <span className={cx('chip', 'live')}>LIVE</span>
          ) : (
            <>
              <span className={cx('chip', 'time')}>
                {capturedAt ? formatTime(capturedAt) : 'Captured'}
              </span>
              {capturedAt !== null && onClose && (
                <span className={cx('closeChip')} onClick={onClose}>
                  Close
                </span>
              )}
            </>
          )}
        </div>
      </div>

      <div className={cx('actionBar')}>
        <ImageActionBar url={url} filename={filename} savedPath={savedPath} />
      </div>
    </div>
  )
}
