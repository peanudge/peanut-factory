import type { ReactNode } from 'react'
import classNames from 'classnames/bind'
import styles from './index.module.scss'

const cx = classNames.bind(styles)

interface Props {
  open: boolean
  onClose: () => void
  title: string
  children: ReactNode
  actions?: ReactNode
}

export default function Modal({ open, onClose, title, children, actions }: Props) {
  if (!open) return null
  return (
    <div className={cx('overlay')} onClick={onClose}>
      <div className={cx('dialog')} onClick={(e) => e.stopPropagation()}>
        <div className={cx('header')}>
          <h3>{title}</h3>
          <button type="button" onClick={onClose}>
            ✕
          </button>
        </div>
        <div className={cx('body')}>{children}</div>
        {actions && <div className={cx('footer')}>{actions}</div>}
      </div>
    </div>
  )
}
