import CameraProfileSelector from '@/components/shared/CameraProfileSelector'
import AcquisitionModeSelector from '@/components/shared/AcquisitionModeSelector'
import AcquisitionActionBar from '@/components/shared/AcquisitionActionBar'
import type {
  AcquisitionStatus,
  CamFileInfo,
  ContinuousSubMode,
  TriggerModeOption,
} from '@/api/types'
import cx from './cx'

interface Props {
  cameras: CamFileInfo[]
  selectedProfile: string
  onProfileChange: (id: string) => void
  continuousSubMode: ContinuousSubMode
  triggerMode: TriggerModeOption
  onTriggerModeChange: (mode: TriggerModeOption) => void
  status: AcquisitionStatus | null
  busy: boolean
  onStart: () => void
  onStop: () => void
  onTrigger: () => void
  onRefresh: () => void
  refreshThrottled: boolean
  hasWarnings?: boolean
  hasErrors?: boolean
}

export default function AcquisitionControls({
  cameras,
  selectedProfile,
  onProfileChange,
  continuousSubMode,
  triggerMode,
  onTriggerModeChange,
  status,
  busy,
  onStart,
  onStop,
  onTrigger,
  onRefresh,
  refreshThrottled,
  hasWarnings,
  hasErrors,
}: Props) {
  return (
    <div className={cx('wrap')}>
      <CameraProfileSelector
        cameras={cameras}
        selectedProfile={selectedProfile}
        onProfileChange={onProfileChange}
        disabled={status?.isActive}
      />
      <AcquisitionModeSelector
        triggerMode={triggerMode}
        onTriggerModeChange={onTriggerModeChange}
        disabled={status?.isActive}
      />
      <AcquisitionActionBar
        continuousSubMode={continuousSubMode}
        selectedProfile={selectedProfile}
        status={status}
        busy={busy}
        onStart={onStart}
        onStop={onStop}
        onTrigger={onTrigger}
        onRefresh={onRefresh}
        refreshThrottled={refreshThrottled}
        hasWarnings={hasWarnings}
        hasErrors={hasErrors}
      />
    </div>
  )
}
