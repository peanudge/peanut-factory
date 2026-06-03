import { useQuery } from '@tanstack/react-query'
import { getBoards, getCameras } from '@/api/client'
import { queryKeys } from '@/api/queryKeys'
import StatusChip from '@/components/shared/StatusChip'
import BoardRow from '@/components/shared/BoardRow'
import cx from './cx'

const SystemState = () => {
  const {
    data: boards,
    isLoading: boardsLoading,
    error: boardsError,
  } = useQuery({
    queryKey: queryKeys.boards,
    queryFn: getBoards,
  })

  const {
    data: cameras,
    isLoading: camsLoading,
    error: camsError,
  } = useQuery({
    queryKey: queryKeys.cameras,
    queryFn: getCameras,
  })

  const loading = boardsLoading || camsLoading
  const error = boardsError ?? camsError

  if (loading)
    return (
      <div className={cx('loading')}>
        <span className={cx('spinner')} />
      </div>
    )

  if (error)
    return (
      <div className={cx('error')}>
        {error instanceof Error ? error.message : 'Failed to load'}
      </div>
    )

  return (
    <div className={cx('SystemState')}>
      <div className={cx('section')}>
        <h3>Frame Grabber Boards</h3>
        <div className={cx('tableWrap')}>
          <table>
            <thead>
              <tr>
                <th />
                <th>#</th>
                <th>Name</th>
                <th>Type</th>
                <th>Serial</th>
                <th>PCI Position</th>
              </tr>
            </thead>
            <tbody>
              {!boards?.length ? (
                <tr>
                  <td colSpan={6} style={{ textAlign: 'center', padding: '16px' }}>
                    No boards detected
                  </td>
                </tr>
              ) : (
                boards.map((b) => <BoardRow key={b.index} board={b} />)
              )}
            </tbody>
          </table>
        </div>
      </div>

      <div className={cx('section')}>
        <h3>Camera Files</h3>
        <div className={cx('tableWrap')}>
          <table>
            <thead>
              <tr>
                <th>File Name</th>
                <th>Manufacturer</th>
                <th>Model</th>
                <th>Format</th>
                <th>Resolution</th>
                <th>Trigger</th>
              </tr>
            </thead>
            <tbody>
              {!cameras?.length ? (
                <tr>
                  <td colSpan={6} style={{ textAlign: 'center', padding: '16px' }}>
                    No camera files found
                  </td>
                </tr>
              ) : (
                cameras.map((c) => (
                  <tr key={c.fileName}>
                    <td className={cx('mono')}>{c.fileName}</td>
                    <td>{c.manufacturer}</td>
                    <td>{c.cameraModel}</td>
                    <td>{c.colorFormat}</td>
                    <td>
                      {c.width}×{c.height}
                    </td>
                    <td>
                      <StatusChip active={c.trigMode === 'IMMEDIATE'} label={c.trigMode} />
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}

export default SystemState
