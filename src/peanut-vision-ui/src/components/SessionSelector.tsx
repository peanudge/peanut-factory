import { useCallback, useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import IconButton from "@mui/material/IconButton";
import List from "@mui/material/List";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import AddIcon from "@mui/icons-material/Add";
import StopIcon from "@mui/icons-material/Stop";
import HistoryIcon from "@mui/icons-material/History";
import DeleteIcon from "@mui/icons-material/Delete";
import type { Session } from "../api/types";
import {
  getSessions,
  getActiveSession,
  createSession,
  endSession,
  deleteSession,
} from "../api/client";

export default function SessionSelector() {
  const [activeSession, setActiveSession] = useState<Session | null>(null);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [historyOpen, setHistoryOpen] = useState(false);
  const [newOpen, setNewOpen] = useState(false);
  const [newName, setNewName] = useState("");
  const [newNotes, setNewNotes] = useState("");
  const [busy, setBusy] = useState(false);

  const refresh = useCallback(async () => {
    try {
      const [active, all] = await Promise.all([getActiveSession(), getSessions()]);
      setActiveSession(active);
      setSessions(all);
    } catch {
      /* ignore */
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  const handleCreate = async () => {
    if (!newName.trim()) return;
    setBusy(true);
    try {
      await createSession(newName.trim(), newNotes.trim() || undefined);
      setNewName("");
      setNewNotes("");
      setNewOpen(false);
      await refresh();
    } finally {
      setBusy(false);
    }
  };

  const handleEnd = async () => {
    if (!activeSession) return;
    setBusy(true);
    try {
      await endSession(activeSession.id);
      await refresh();
    } finally {
      setBusy(false);
    }
  };

  const handleDelete = async (id: string) => {
    setBusy(true);
    try {
      await deleteSession(id);
      await refresh();
    } finally {
      setBusy(false);
    }
  };

  const formatDate = (iso: string) => {
    const d = new Date(iso);
    return d.toLocaleString();
  };

  return (
    <Box sx={{ display: "flex", alignItems: "center", gap: 1, flexWrap: "wrap" }}>
      <Typography variant="subtitle2" color="text.secondary">
        Session:
      </Typography>

      {activeSession ? (
        <>
          <Chip
            label={activeSession.name}
            color="primary"
            size="small"
            variant="outlined"
          />
          <Button
            size="small"
            color="warning"
            startIcon={<StopIcon />}
            onClick={handleEnd}
            disabled={busy}
          >
            End Session
          </Button>
        </>
      ) : (
        <Typography variant="body2" color="text.secondary">
          No active session
        </Typography>
      )}

      <Button
        size="small"
        startIcon={<AddIcon />}
        onClick={() => setNewOpen(true)}
        disabled={busy}
      >
        New Session
      </Button>

      <IconButton size="small" onClick={() => setHistoryOpen(true)}>
        <HistoryIcon fontSize="small" />
      </IconButton>

      {/* New Session Dialog */}
      <Dialog open={newOpen} onClose={() => setNewOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>New Session</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Session Name"
            fullWidth
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleCreate()}
          />
          <TextField
            margin="dense"
            label="Notes (optional)"
            fullWidth
            multiline
            rows={2}
            value={newNotes}
            onChange={(e) => setNewNotes(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setNewOpen(false)}>Cancel</Button>
          <Button onClick={handleCreate} disabled={busy || !newName.trim()} variant="contained">
            Create
          </Button>
        </DialogActions>
      </Dialog>

      {/* History Dialog */}
      <Dialog open={historyOpen} onClose={() => setHistoryOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Session History</DialogTitle>
        <DialogContent>
          {sessions.length === 0 ? (
            <Typography color="text.secondary" sx={{ py: 2 }}>
              No sessions yet
            </Typography>
          ) : (
            <List dense>
              {sessions.map((s) => (
                <ListItemButton key={s.id} sx={{ borderRadius: 1 }}>
                  <ListItemText
                    primary={
                      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                        {s.name}
                        {s.isActive && <Chip label="Active" size="small" color="success" />}
                      </Box>
                    }
                    secondary={
                      <>
                        {formatDate(s.createdAt)}
                        {s.endedAt && ` — ${formatDate(s.endedAt)}`}
                        {s.notes && ` | ${s.notes}`}
                      </>
                    }
                  />
                  {!s.isActive && (
                    <IconButton
                      edge="end"
                      size="small"
                      onClick={(e) => { e.stopPropagation(); handleDelete(s.id); }}
                      disabled={busy}
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  )}
                </ListItemButton>
              ))}
            </List>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setHistoryOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
