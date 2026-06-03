import cx from './cx'

interface Props {
  active: boolean
  label?: string
  hasWarnings?: boolean
  hasErrors?: boolean
}

export default function StatusChip({ active, label, hasWarnings, hasErrors }: Props) {
  const color = hasErrors ? 'error' : hasWarnings ? 'warning' : active ? 'success' : 'default'
  return (
    <span className={cx('chip', color)}>
      {label ?? (active ? 'Active' : 'Inactive')}
    </span>
  )
}
