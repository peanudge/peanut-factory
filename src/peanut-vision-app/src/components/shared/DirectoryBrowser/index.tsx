import { useState, useEffect } from 'react'
import { ChevronRight, ChevronDown, Folder, FolderOpen, HardDrive } from 'lucide-react'
import { getFilesystemRoots, listDirectory, type DirectoryEntry } from '@/api/client'
import Modal from '@/components/shared/Modal'
import cx from './cx'

interface Props {
  open: boolean
  currentPath: string
  onSelect: (path: string) => void
  onClose: () => void
}

interface TreeNode {
  name: string
  path: string
  hasChildren: boolean
  children?: TreeNode[]
  expanded: boolean
  loading: boolean
}

export default function DirectoryBrowser({ open, currentPath, onSelect, onClose }: Props) {
  const [tree, setTree] = useState<TreeNode[]>([])
  const [selectedPath, setSelectedPath] = useState(currentPath)
  const [error, setError] = useState<string | null>(null)

  // Load roots when modal opens
  useEffect(() => {
    if (!open) return
    setSelectedPath(currentPath)
    setError(null)
    getFilesystemRoots()
      .then(r => {
        const nodes: TreeNode[] = r.map(root => ({
          name: root,
          path: root,
          hasChildren: true,
          expanded: false,
          loading: false,
        }))
        setTree(nodes)
        // Auto-expand to currentPath
        if (currentPath) expandToPath(nodes, currentPath, r)
      })
      .catch(() => setError('Failed to load filesystem roots'))
  }, [open]) // eslint-disable-line react-hooks/exhaustive-deps

  const expandToPath = async (nodes: TreeNode[], targetPath: string, rootPaths: string[]) => {
    const matchingRoot = rootPaths.find(r => targetPath.startsWith(r))
    if (!matchingRoot) return

    const segments = targetPath
      .replace(matchingRoot, '')
      .split(/[/\\]/)
      .filter(Boolean)

    let currentNodes = nodes
    let currentPath = matchingRoot

    for (const segment of segments) {
      const node = currentNodes.find(n => n.path === currentPath || n.name === matchingRoot)
      if (node) {
        await expandNode(node, currentNodes)
        currentPath = [currentPath, segment].join(currentPath.endsWith('/') || currentPath.endsWith('\\') ? '' : '/')
        currentNodes = node.children ?? []
      }
    }
    setTree([...nodes])
  }

  const expandNode = async (node: TreeNode, _siblings: TreeNode[]) => {
    if (node.expanded || node.loading) return
    node.loading = true
    setTree(t => [...t])
    try {
      const entries: DirectoryEntry[] = await listDirectory(node.path)
      node.children = entries.map(e => ({
        name: e.name,
        path: e.path,
        hasChildren: e.hasChildren,
        expanded: false,
        loading: false,
      }))
      node.expanded = true
    } catch {
      node.children = []
      node.expanded = true
    } finally {
      node.loading = false
    }
  }

  const handleToggle = async (node: TreeNode) => {
    if (node.expanded) {
      node.expanded = false
    } else {
      await expandNode(node, tree)
    }
    setTree(t => [...t])
  }

  const handleSelect = (path: string) => setSelectedPath(path)

  const handleConfirm = () => {
    if (selectedPath) {
      onSelect(selectedPath)
      onClose()
    }
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      title="Select Output Directory"
      actions={
        <>
          <button type="button" className={cx('cancelBtn')} onClick={onClose}>Cancel</button>
          <button
            type="button"
            className={cx('selectBtn')}
            onClick={handleConfirm}
            disabled={!selectedPath}
          >
            Select
          </button>
        </>
      }
    >
      <div className={cx('browser')}>
        {error && <p className={cx('error')}>{error}</p>}
        <div className={cx('tree')}>
          {tree.map(node => (
            <TreeItem
              key={node.path}
              node={node}
              selectedPath={selectedPath}
              onToggle={handleToggle}
              onSelect={handleSelect}
              depth={0}
              isRoot
            />
          ))}
        </div>
        {selectedPath && (
          <div className={cx('selectedPath')}>
            <span className={cx('selectedLabel')}>Selected:</span>
            <span className={cx('selectedValue')}>{selectedPath}</span>
          </div>
        )}
      </div>
    </Modal>
  )
}

interface TreeItemProps {
  node: TreeNode
  selectedPath: string
  onToggle: (node: TreeNode) => void
  onSelect: (path: string) => void
  depth: number
  isRoot?: boolean
}

function TreeItem({ node, selectedPath, onToggle, onSelect, depth, isRoot }: TreeItemProps) {
  const isSelected = node.path === selectedPath

  return (
    <div>
      <div
        className={cx('item', { selected: isSelected })}
        style={{ paddingLeft: `${depth * 16 + 4}px` }}
        onClick={() => onSelect(node.path)}
      >
        {node.hasChildren ? (
          <button
            type="button"
            className={cx('chevron')}
            onClick={(e) => { e.stopPropagation(); onToggle(node) }}
          >
            {node.loading
              ? <span className={cx('loader')} />
              : node.expanded
                ? <ChevronDown size={12} />
                : <ChevronRight size={12} />}
          </button>
        ) : (
          <span className={cx('chevron', 'empty')} />
        )}
        {isRoot
          ? <HardDrive size={14} className={cx('icon')} />
          : node.expanded
            ? <FolderOpen size={14} className={cx('icon')} />
            : <Folder size={14} className={cx('icon')} />}
        <span className={cx('name')}>{node.name}</span>
      </div>

      {node.expanded && node.children && (
        <div>
          {node.children.map(child => (
            <TreeItem
              key={child.path}
              node={child}
              selectedPath={selectedPath}
              onToggle={onToggle}
              onSelect={onSelect}
              depth={depth + 1}
            />
          ))}
          {node.children.length === 0 && (
            <div
              className={cx('empty')}
              style={{ paddingLeft: `${(depth + 1) * 16 + 24}px` }}
            >
              Empty
            </div>
          )}
        </div>
      )}
    </div>
  )
}
