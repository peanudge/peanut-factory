import { createFileRoute } from '@tanstack/react-router'
import classNames from 'classnames/bind'
import styles from '@/styles/Home.module.scss'

const cx = classNames.bind(styles)
export const Route = createFileRoute('/')({ component: Home })

function Home() {
  return (
    <div className={cx('Home')}>
      <h2>[ Vision Camera Test ]</h2>
      <h3>테스트</h3>
    </div>
  )
}
