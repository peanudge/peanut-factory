import { useState } from 'react'
import { Loader2, Square, Trash2, Pencil, FolderSearch } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import AcquisitionSettings from '@/components/shared/AcquisitionSettings'
import StatusChip from '@/components/shared/StatusChip'
import Modal from '@/components/shared/Modal'
import DirectoryBrowser from '@/components/shared/DirectoryBrowser'
import type { AcquisitionConfigPreset, AcquisitionFormConfig, SaveImageFormat, CamFileInfo } from '@/api/types'
import { DEFAULT_ACQUISITION_FORM_CONFIG } from '@/api/types'
import { getPresets, savePreset, deletePreset } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'
import type { UseAcquisitionConfig } from '@/hooks/useAcquisitionConfig'
import type { AcquisitionSession } from '@/hooks/useAcquisitionSession'
import cx from './cx'

function presetToFormConfig(preset: AcquisitionConfigPreset): AcquisitionFormConfig {
  return {
    ...DEFAULT_ACQUISITION_FORM_CONFIG,
    profileId: preset.profileId,
    frameCount: preset.frameCount ?? null,
    intervalMs: preset.intervalMs ?? null,
    acquisitionMode: preset.intervalMs != null ? 'auto' : 'manual',
    outputDirectory: preset.outputDirectory ?? DEFAULT_ACQUISITION_FORM_CONFIG.outputDirectory,
    format: preset.format ?? DEFAULT_ACQUISITION_FORM_CONFIG.format,
  }
}

interface Props {
  acqConfig: UseAcquisitionConfig
  session: AcquisitionSession
}

export default function CaptureTab({ acqConfig, session }: Props) {
  if (session.isActive) {
    return <ActiveView session={session} />
  }
  return <IdleView acqConfig={acqConfig} session={session} />
}

// ── Active: status + stop/trigger ────────────────────────────────────────────

function ActiveView({ session }: { session: AcquisitionSession }) {
  const s = session.status
  return (
    <div className={cx('activeView')}>
      <div className={cx('activeHeader')}>
        <StatusChip
          active
          label={`Active — ${s?.profileId ?? ''}`}
          hasWarnings={session.hasWarnings}
          hasErrors={session.hasErrors}
        />
      </div>
      <dl className={cx('activeInfo')}>
        <div className={cx('infoRow')}><dt>Profile</dt><dd>{s?.profileId ?? '—'}</dd></div>
        <div className={cx('infoRow')}>
          <dt>Frame count</dt>
          <dd>{s?.activeFrameCount != null ? s.activeFrameCount : '∞'}</dd>
        </div>
        <div className={cx('infoRow')}>
          <dt>Interval</dt>
          <dd>{s?.activeIntervalMs != null ? `${s.activeIntervalMs / 1000}s` : 'manual'}</dd>
        </div>
        <div className={cx('infoRow')}>
          <dt>Save to</dt>
          <dd title={s?.outputDirectory}>{s?.outputDirectory ?? '—'}</dd>
        </div>
        {s?.statistics && (
          <>
            <div className={cx('infoRow')}><dt>Frames</dt><dd>{s.statistics.frameCount}</dd></div>
            <div className={cx('infoRow')}><dt>FPS</dt><dd>{s.statistics.averageFps.toFixed(1)}</dd></div>
            {s.statistics.droppedFrameCount > 0 && (
              <div className={cx('infoRow', 'warn')}><dt>Dropped</dt><dd>{s.statistics.droppedFrameCount}</dd></div>
            )}
          </>
        )}
        {s?.lastError && (
          <div className={cx('infoRow', 'error')}><dt>Error</dt><dd>{s.lastError}</dd></div>
        )}
      </dl>

      {s?.activeIntervalMs == null && (
        <button
          type="button"
          className={cx('triggerBtn')}
          onClick={session.handleTrigger}
          disabled={session.busy}
        >
          {session.busy && <Loader2 size={14} className={cx('spin')} />}
          Trigger
        </button>
      )}

      <button
        type="button"
        className={cx('stopBtn')}
        onClick={session.handleStop}
        disabled={session.busy}
      >
        {session.busy
          ? <Loader2 size={14} className={cx('spin')} />
          : <Square size={14} />}
        Stop
      </button>
    </div>
  )
}

// ── Idle: tabbed layout ───────────────────────────────────────────────────────

type Tab = 'settings' | 'presets'

function IdleView({ acqConfig, session }: Props) {
  const [activeTab, setActiveTab] = useState<Tab>('settings')
  const { toast } = useToast()
  const queryClient = useQueryClient()

  const { data: presets = [], isLoading: presetsLoading } = useQuery({
    queryKey: queryKeys.presets,
    queryFn: getPresets,
  })

  const saveMutation = useMutation({
    mutationFn: savePreset,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      toast('설정이 저장되었습니다', 'success')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '저장 실패', 'error'),
  })

  const deleteMutation = useMutation({
    mutationFn: deletePreset,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      toast('설정이 삭제되었습니다', 'info')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '삭제 실패', 'error'),
  })

  const editMutation = useMutation({
    mutationFn: async ({ updated, oldName }: { updated: AcquisitionConfigPreset; oldName: string }) => {
      await savePreset(updated)
      if (oldName !== updated.name) {
        await deletePreset(oldName)
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.presets })
      toast('설정이 수정되었습니다', 'success')
    },
    onError: (e: unknown) => toast(e instanceof Error ? e.message : '수정 실패', 'error'),
  })

  return (
    <div className={cx('idleWrap')}>
      <div className={cx('tabBar')}>
        <button
          type="button"
          className={cx('tabBtn', { tabActive: activeTab === 'settings' })}
          onClick={() => setActiveTab('settings')}
        >
          설정
        </button>
        <button
          type="button"
          className={cx('tabBtn', { tabActive: activeTab === 'presets' })}
          onClick={() => setActiveTab('presets')}
        >
          저장된 설정
          {presets.length > 0 && (
            <span className={cx('tabBadge')}>{presets.length}</span>
          )}
        </button>
      </div>

      <div className={cx('tabContent')}>
        {activeTab === 'settings' ? (
          <AcquisitionSettings
            config={acqConfig.config}
            onChange={acqConfig.updateConfig}
            cameras={acqConfig.cameras}
            camerasLoading={acqConfig.camerasLoading}
            canStart={session.canStart}
            busy={session.busy}
            onStart={session.handleStart}
            onSavePreset={(name) =>
              saveMutation.mutate({
                name,
                profileId: acqConfig.config.profileId,
                frameCount: acqConfig.config.frameCount,
                intervalMs: acqConfig.config.intervalMs,
                outputDirectory: acqConfig.config.outputDirectory,
                format: acqConfig.config.format,
              })
            }
            savingPreset={saveMutation.isPending}
          />
        ) : (
          <PresetTab
            presets={presets}
            presetsLoading={presetsLoading}
            cameras={acqConfig.cameras}
            camerasLoading={acqConfig.camerasLoading}
            busy={session.busy}
            onStart={(preset) => session.handleStartWithConfig(presetToFormConfig(preset))}
            onDelete={(name) => deleteMutation.mutate(name)}
            deleting={deleteMutation.isPending}
            onEdit={(updated, oldName) => editMutation.mutate({ updated, oldName })}
            editing={editMutation.isPending}
          />
        )}
      </div>
    </div>
  )
}

// ── Preset tab ────────────────────────────────────────────────────────────────

interface PresetTabProps {
  presets: AcquisitionConfigPreset[]
  presetsLoading: boolean
  cameras: CamFileInfo[]
  camerasLoading: boolean
  busy: boolean
  onStart: (preset: AcquisitionConfigPreset) => void
  onDelete: (name: string) => void
  deleting: boolean
  onEdit: (updated: AcquisitionConfigPreset, oldName: string) => void
  editing: boolean
}

type EditState = {
  originalName: string
  name: string
  profileId: string
  outputDirectory: string
  format: SaveImageFormat
  frameCount: number | null
  acquisitionMode: 'auto' | 'manual'
  intervalMs: number | null
  intervalRaw: string
  intervalError: string
  browserOpen: boolean
}

const FORMATS: { value: SaveImageFormat; label: string }[] = [
  { value: 'png', label: 'PNG' },
  { value: 'bmp', label: 'BMP' },
  { value: 'raw', label: 'RAW' },
]

function PresetTab({
  presets, presetsLoading,
  cameras, camerasLoading,
  busy, onStart,
  onDelete, deleting,
  onEdit, editing,
}: PresetTabProps) {
  const [confirmPreset, setConfirmPreset] = useState<AcquisitionConfigPreset | null>(null)
  const [editState, setEditState] = useState<EditState | null>(null)

  const openEdit = (preset: AcquisitionConfigPreset) => {
    setEditState({
      originalName: preset.name,
      name: preset.name,
      profileId: preset.profileId,
      outputDirectory: preset.outputDirectory ?? '',
      format: preset.format ?? 'png',
      frameCount: preset.frameCount ?? null,
      acquisitionMode: preset.intervalMs != null ? 'auto' : 'manual',
      intervalMs: preset.intervalMs ?? null,
      intervalRaw: preset.intervalMs != null ? String(preset.intervalMs) : '',
      intervalError: '',
      browserOpen: false,
    })
  }

  const updateEdit = <K extends keyof EditState>(key: K, value: EditState[K]) =>
    setEditState((s) => s ? { ...s, [key]: value } : s)

  const handleIntervalChange = (raw: string) => {
    updateEdit('intervalRaw', raw)
    if (raw.trim() === '') {
      updateEdit('intervalError', '')
      updateEdit('intervalMs', null)
      return
    }
    const ms = parseInt(raw, 10)
    if (isNaN(ms) || ms < 50) {
      updateEdit('intervalError', '50ms 이상의 정수를 입력하세요')
    } else {
      updateEdit('intervalError', '')
      updateEdit('intervalMs', ms)
    }
  }

  const handleIntervalStep = (delta: number) => {
    const next = Math.max(50, (editState?.intervalMs ?? 1000) + delta)
    setEditState((s) => s ? { ...s, intervalMs: next, intervalRaw: String(next), intervalError: '' } : s)
  }

  const handleEditSave = () => {
    if (!editState) return
    const updated: AcquisitionConfigPreset = {
      name: editState.name.trim(),
      profileId: editState.profileId,
      outputDirectory: editState.outputDirectory || undefined,
      format: editState.format,
      frameCount: editState.frameCount,
      intervalMs: editState.acquisitionMode === 'auto' ? editState.intervalMs : null,
    }
    onEdit(updated, editState.originalName)
    setEditState(null)
  }

  const canSaveEdit = editState
    ? editState.name.trim().length > 0
      && editState.profileId.length > 0
      && (editState.acquisitionMode !== 'auto' || (editState.intervalMs != null && editState.intervalError === ''))
    : false

  if (presetsLoading) {
    return (
      <div className={cx('presetSkeletons')}>
        <div className={cx('presetSkeleton')} />
        <div className={cx('presetSkeleton')} />
      </div>
    )
  }

  if (presets.length === 0) {
    return (
      <p className={cx('presetsEmpty')}>
        저장된 설정이 없습니다.<br />
        설정 탭에서 현재 설정을 저장해보세요.
      </p>
    )
  }

  return (
    <>
      <ul className={cx('presetList')}>
        {presets.map((p) => (
          <li key={p.name} className={cx('presetItem')}>
            <button
              type="button"
              className={cx('presetNameBtn')}
              onClick={() => setConfirmPreset(p)}
              disabled={busy || deleting || editing}
            >
              <span className={cx('presetItemName')}>{p.name}</span>
              <span className={cx('presetItemMeta')}>
                {[
                  p.profileId,
                  p.intervalMs != null ? `${p.intervalMs}ms 간격` : '수동',
                  p.frameCount != null ? `${p.frameCount}장` : null,
                ].filter(Boolean).join(' · ')}
              </span>
            </button>
            <button
              type="button"
              className={cx('presetActionBtn')}
              onClick={() => openEdit(p)}
              disabled={deleting || editing || busy}
              title="수정"
            >
              <Pencil size={13} />
            </button>
            <button
              type="button"
              className={cx('presetDeleteBtn')}
              onClick={() => onDelete(p.name)}
              disabled={deleting || editing || busy}
              title="삭제"
            >
              {deleting
                ? <Loader2 size={13} className={cx('spin')} />
                : <Trash2 size={13} />}
            </button>
          </li>
        ))}
      </ul>

      {/* ── Confirm start dialog ── */}
      <Modal
        open={confirmPreset !== null}
        onClose={() => setConfirmPreset(null)}
        title={confirmPreset?.name ?? ''}
        actions={
          <>
            <button type="button" onClick={() => setConfirmPreset(null)}>취소</button>
            <button
              type="button"
              onClick={() => { if (!confirmPreset) return; onStart(confirmPreset); setConfirmPreset(null) }}
              disabled={busy}
            >
              {busy && <Loader2 size={13} className={cx('spin')} />}
              이 설정으로 촬영 시작
            </button>
          </>
        }
      >
        {confirmPreset && (
          <dl className={cx('presetDetail')}>
            <div><dt>카메라</dt><dd>{confirmPreset.profileId}</dd></div>
            <div><dt>포맷</dt><dd>{(confirmPreset.format ?? 'png').toUpperCase()}</dd></div>
            <div>
              <dt>촬영 방식</dt>
              <dd>{confirmPreset.intervalMs != null ? `자동 (${confirmPreset.intervalMs}ms 간격)` : '수동'}</dd>
            </div>
            <div>
              <dt>프레임 수</dt>
              <dd>{confirmPreset.frameCount != null ? `${confirmPreset.frameCount}장` : '제한 없음'}</dd>
            </div>
            {confirmPreset.outputDirectory && (
              <div><dt>저장 경로</dt><dd>{confirmPreset.outputDirectory}</dd></div>
            )}
          </dl>
        )}
      </Modal>

      {/* ── Edit dialog ── */}
      <Modal
        open={editState !== null}
        onClose={() => setEditState(null)}
        title="설정 수정"
        actions={
          <>
            <button type="button" onClick={() => setEditState(null)}>취소</button>
            <button
              type="button"
              onClick={handleEditSave}
              disabled={editing || !canSaveEdit}
            >
              {editing && <Loader2 size={13} className={cx('spin')} />}
              저장
            </button>
          </>
        }
      >
        {editState && (
          <div className={cx('editForm')}>

            <label className={cx('editLabel')}>
              이름
              <input
                type="text"
                value={editState.name}
                onChange={(e) => updateEdit('name', e.target.value)}
              />
            </label>

            <label className={cx('editLabel')}>
              카메라 프로파일
              <select
                value={editState.profileId}
                onChange={(e) => updateEdit('profileId', e.target.value)}
                disabled={camerasLoading}
              >
                {camerasLoading
                  ? <option>로딩 중…</option>
                  : cameras.map((c) => (
                      <option key={c.fileName} value={c.fileName}>{c.fileName}</option>
                    ))
                }
              </select>
            </label>

            <label className={cx('editLabel')}>
              저장 경로
              <div className={cx('editDirRow')}>
                <input
                  type="text"
                  value={editState.outputDirectory}
                  onChange={(e) => updateEdit('outputDirectory', e.target.value)}
                  placeholder="CapturedImages"
                />
                <button
                  type="button"
                  onClick={() => updateEdit('browserOpen', true)}
                  title="Browse"
                >
                  <FolderSearch size={14} />
                </button>
              </div>
            </label>

            <fieldset className={cx('editFieldset')}>
              <legend>포맷</legend>
              {FORMATS.map(({ value, label }) => (
                <label key={value}>
                  <input
                    type="radio"
                    name="editFormat"
                    value={value}
                    checked={editState.format === value}
                    onChange={() => updateEdit('format', value)}
                  />
                  {label}
                </label>
              ))}
            </fieldset>

            <label className={cx('editLabel')}>
              프레임 수
              <input
                type="number"
                min={1}
                value={editState.frameCount ?? ''}
                placeholder="∞ (제한없음)"
                onChange={(e) => {
                  const v = parseInt(e.target.value, 10)
                  updateEdit('frameCount', isNaN(v) || v < 1 ? null : v)
                }}
              />
            </label>

            <fieldset className={cx('editFieldset')}>
              <legend>촬영 방식</legend>
              <label>
                <input
                  type="radio"
                  name="editMode"
                  checked={editState.acquisitionMode === 'auto'}
                  onChange={() => updateEdit('acquisitionMode', 'auto')}
                />
                자동
              </label>
              <label>
                <input
                  type="radio"
                  name="editMode"
                  checked={editState.acquisitionMode === 'manual'}
                  onChange={() => updateEdit('acquisitionMode', 'manual')}
                />
                수동
              </label>
            </fieldset>

            {editState.acquisitionMode === 'auto' && (
              <label className={cx('editLabel')}>
                간격
                <div className={cx('editIntervalRow')}>
                  <button
                    type="button"
                    onClick={() => handleIntervalStep(-50)}
                    disabled={(editState.intervalMs ?? 1000) <= 50}
                  >−50</button>
                  <input
                    type="text"
                    inputMode="numeric"
                    value={editState.intervalRaw}
                    placeholder="1000"
                    onChange={(e) => handleIntervalChange(e.target.value)}
                    className={editState.intervalError ? cx('inputError') : undefined}
                  />
                  <span>ms</span>
                  <button type="button" onClick={() => handleIntervalStep(50)}>+50</button>
                </div>
                {editState.intervalError && (
                  <span className={cx('editError')}>{editState.intervalError}</span>
                )}
              </label>
            )}

            <DirectoryBrowser
              open={editState.browserOpen}
              currentPath={editState.outputDirectory}
              onSelect={(path) => setEditState((s) => s ? { ...s, outputDirectory: path, browserOpen: false } : s)}
              onClose={() => updateEdit('browserOpen', false)}
            />
          </div>
        )}
      </Modal>
    </>
  )
}
