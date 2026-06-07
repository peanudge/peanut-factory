import { useState } from 'react'
import { Download, Save, Copy, Check } from 'lucide-react'
import cx from './cx'

interface Props {
  url: string
  filename?: string
  savedPath?: string
}

export default function ImageActionBar({ url, filename, savedPath }: Props) {
  const [copied, setCopied] = useState(false)

  const handleCopy = () => {
    if (!savedPath) return
    navigator.clipboard.writeText(savedPath).then(() => {
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    })
  }

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
        <div className={cx('savedPathWrap')}>
          <Save size={12} className={cx('saveIcon')} />
          <span className={cx('savedPath')} title={savedPath}>{savedPath}</span>
          <button
            type="button"
            className={cx('copyBtn', { copied })}
            onClick={handleCopy}
            title="Copy path to clipboard"
          >
            {copied ? <Check size={12} /> : <Copy size={12} />}
          </button>
        </div>
      )}
    </div>
  )
}
