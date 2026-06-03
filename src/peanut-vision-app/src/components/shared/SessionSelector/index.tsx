import { useEffect, useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, Square, History, Trash2 } from 'lucide-react'
import type { Session } from '@/api/types'
import {
  getSessions,
  getActiveSession,
  createSession,
  endSession,
  deleteSession,
} from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import { useToast } from '@/contexts/ToastContext'
import Modal from '@/components/shared/Modal'
import cx from './cx'

interface Props {
  onSessionChange?: (name: string | null) => void
}

export default function SessionSelector({ onSessionChange }: Props = {}) {
  const queryClient = useQueryClient()
  const { toast } = useToast()
  const [historyOpen, setHistoryOpen] = useState(false)
  const [newOpen, setNewOpen] = useState(false)
  const [newName, setNewName] = useState('')
  const [newNotes, setNewNotes] = useState('')

  const { data: activeSession } = useQuery({
    queryKey: queryKeys.activeSession,
    queryFn: getActiveSession,
  })

  const { data: sessions = [] } = useQuery<Session[]>({
    queryKey: queryKeys.sessions,
    queryFn: () => getSessions(),
  })

  useEffect(() => {
    onSessionChange?.(activeSession?.name ?? null)
  }, [activeSession, onSessionChange])

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: queryKeys.sessions })
    queryClient.invalidateQueries({ queryKey: queryKeys.activeSession })
  }

  const handleError = (e: unknown) =>
    toast(e instanceof Error ? e.message : '세션 작업에 실패했습니다', 'error')

  const createMutation = useMutation({
    mutationFn: ({ name, notes }: { name: string; notes?: string }) =>
      createSession(name, notes),
    onSuccess: () => {
      setNewName('')
      setNewNotes('')
      setNewOpen(false)
      invalidate()
    },
    onError: handleError,
  })

  const endMutation = useMutation({
    mutationFn: (id: string) => endSession(id),
    onSuccess: invalidate,
    onError: handleError,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteSession(id),
    onSuccess: invalidate,
    onError: handleError,
  })

  const busy = createMutation.isPending || endMutation.isPending || deleteMutation.isPending

  const handleCreate = () => {
    if (!newName.trim()) return
    createMutation.mutate({ name: newName.trim(), notes: newNotes.trim() || undefined })
  }

  const formatDate = (iso: string) => new Date(iso).toLocaleString()

  return (
    <div className={cx('wrap')}>
      <span className={cx('label')}>Session:</span>

      {activeSession ? (
        <>
          <span className={cx('activeChip')}>{activeSession.name}</span>
          <button
            type="button"
            className={cx('btn', 'warning')}
            onClick={() => endMutation.mutate(activeSession.id)}
            disabled={busy}
          >
            <Square size={13} /> End Session
          </button>
        </>
      ) : (
        <span className={cx('noSession')}>No active session</span>
      )}

      <button
        type="button"
        className={cx('btn')}
        onClick={() => setNewOpen(true)}
        disabled={busy}
      >
        <Plus size={13} /> New Session
      </button>

      <button
        type="button"
        className={cx('iconBtn')}
        onClick={() => setHistoryOpen(true)}
      >
        <History size={15} />
      </button>

      {/* New Session Modal */}
      <Modal
        open={newOpen}
        onClose={() => setNewOpen(false)}
        title="New Session"
        actions={
          <>
            <button type="button" className={cx('btn')} onClick={() => setNewOpen(false)}>
              Cancel
            </button>
            <button
              type="button"
              className={cx('btn', 'primary')}
              onClick={handleCreate}
              disabled={busy || !newName.trim()}
            >
              Create
            </button>
          </>
        }
      >
        <input
          type="text"
          className={cx('field')}
          placeholder="Session Name"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && handleCreate()}
          autoFocus
        />
        <textarea
          className={cx('field')}
          placeholder="Notes (optional)"
          rows={2}
          value={newNotes}
          onChange={(e) => setNewNotes(e.target.value)}
          style={{ resize: 'vertical' }}
        />
      </Modal>

      {/* History Modal */}
      <Modal
        open={historyOpen}
        onClose={() => setHistoryOpen(false)}
        title="Session History"
        actions={
          <button type="button" className={cx('btn')} onClick={() => setHistoryOpen(false)}>
            Close
          </button>
        }
      >
        {sessions.length === 0 ? (
          <p className={cx('empty')}>No sessions yet</p>
        ) : (
          <ul className={cx('list')}>
            {sessions.map((s) => (
              <li key={s.id} className={cx('listItem')}>
                <div>
                  <div className={cx('sessionName')}>
                    {s.name}
                    {s.isActive && <span className={cx('activeBadge')}>Active</span>}
                  </div>
                  <div className={cx('sessionMeta')}>
                    {formatDate(s.createdAt)}
                    {s.endedAt && ` — ${formatDate(s.endedAt)}`}
                    {s.notes && ` | ${s.notes}`}
                  </div>
                </div>
                {!s.isActive && (
                  <button
                    type="button"
                    className={cx('iconBtn2')}
                    onClick={(e) => {
                      e.stopPropagation()
                      deleteMutation.mutate(s.id)
                    }}
                    disabled={busy}
                  >
                    <Trash2 size={14} />
                  </button>
                )}
              </li>
            ))}
          </ul>
        )}
      </Modal>
    </div>
  )
}
