import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ChevronDown, ChevronUp } from 'lucide-react'
import type { BoardInfo } from '@/api/types'
import { getBoardStatus } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import cx from './cx'

export default function BoardRow({ board }: { board: BoardInfo }) {
  const [open, setOpen] = useState(false)
  const { data: status, isFetching } = useQuery({
    queryKey: queryKeys.boardStatus(board.index),
    queryFn: () => getBoardStatus(board.index),
    enabled: open,
    staleTime: Infinity,
  })

  const rows: [string, string | number][] = status
    ? [
        ['Input Connector', status.inputConnector],
        ['Input State', status.inputState],
        ['Signal Strength', status.signalStrength],
        ['Output State', status.outputState],
        ['Camera Link', status.cameraLinkStatus],
        ['Sync Errors', status.syncErrors],
        ['Clock Errors', status.clockErrors],
        ['Grabber Errors', status.grabberErrors],
        ['Frame Trigger Violations', status.frameTriggerViolations],
        ['Line Trigger Violations', status.lineTriggerViolations],
        ['PCIe Link', status.pcieLinkInfo],
      ]
    : []

  return (
    <>
      <tr onClick={() => setOpen((o) => !o)} style={{ cursor: 'pointer' }}>
        <td>
          <button type="button" className={cx('toggle')}>
            {open ? <ChevronUp size={14} /> : <ChevronDown size={14} />}
          </button>
        </td>
        <td>{board.index}</td>
        <td>{board.boardName}</td>
        <td>{board.boardType}</td>
        <td>{board.serialNumber}</td>
        <td>{board.pciPosition}</td>
      </tr>
      {open && (
        <tr>
          <td colSpan={6} className={cx('detail')}>
            <div className={cx('detailInner')}>
              {isFetching && <span className={cx('spinner')} />}
              {status && (
                <table className={cx('detailTable')}>
                  <tbody>
                    {rows.map(([label, value]) => (
                      <tr key={label}>
                        <td>{label}</td>
                        <td>{String(value)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          </td>
        </tr>
      )}
    </>
  )
}
