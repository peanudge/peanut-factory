import classNames from 'classnames/bind'
import styles from './ToastContainer.module.scss'
import { useToast } from './ToastContext'

const cx = classNames.bind(styles)

export function ToastContainer() {
  const { toasts, dismiss } = useToast()
  return (
    <div className={cx('ToastContainer')}>
      {toasts.map((t) => (
        <div key={t.id} className={cx('toast', t.severity)}>
          <span>{t.message}</span>
          <button type="button" onClick={() => dismiss(t.id)}>✕</button>
        </div>
      ))}
    </div>
  )
}
