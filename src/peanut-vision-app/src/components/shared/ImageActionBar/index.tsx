import { Download, Save } from 'lucide-react'
import cx from './cx'

interface Props {
  url: string
  filename?: string
  savedPath?: string
}

export default function ImageActionBar({ url, filename, savedPath }: Props) {
  return (
    <div className={cx('bar')}>
      <a
        className={cx('btn')}
        href={url}
        download={filename ?? 'capture.png'}
      >
        <Download size={13} /> Download
      </a>
      {savedPath && (
        <span className={cx('savedPath')} title={savedPath}>
          <Save size={12} /> {savedPath}
        </span>
      )}
    </div>
  )
}
