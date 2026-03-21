import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { Session } from "../api/types";
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
import {
  getSessions,
  getActiveSession,
  createSession,
  endSession,
  deleteSession,
} from "../api/client";

import { queryKeys } from "../api/queryKeys";

interface SessionSelectorProps {
  onSessionChange?: (name: string | null) => void;
}

export default function SessionSelector({ onSessionChange }: SessionSelectorProps = {}) {
  const queryClient = useQueryClient();
  const [historyOpen, setHistoryOpen] = useState(false);
  const [newOpen, setNewOpen] = useState(false);
  const [newName, setNewName] = useState("");
  const [newNotes, setNewNotes] = useState("");

  const { data: activeSession } = useQuery({
    queryKey: queryKeys.activeSession,
    queryFn: getActiveSession,
  });

  const { data: sessions = [] } = useQuery<Session[]>({
    queryKey: queryKeys.sessions,
    queryFn: () => getSessions(),
  });

  useEffect(() => {
    onSessionChange?.(activeSession?.name ?? null);
  }, [activeSession, onSessionChange]);

  const invalidate = () => {
    queryClient.invalidateQueries({ queryKey: queryKeys.sessions });
    queryClient.invalidateQueries({ queryKey: queryKeys.activeSession });
  };

  const createMutation = useMutation({
    mutationFn: ({ name, notes }: { name: string; notes?: string }) =>
      createSession(name, notes),
    onSuccess: () => {
      setNewName("");
      setNewNotes("");
      setNewOpen(false);
      invalidate();
    },
  });

  const endMutation = useMutation({
    mutationFn: (id: string) => endSession(id),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteSession(id),
    onSuccess: invalidate,
  });

  const busy = createMutation.isPending || endMutation.isPending || deleteMutation.isPending;

  const handleCreate = () => {
    if (!newName.trim()) return;
    createMutation.mutate({ name: newName.trim(), notes: newNotes.trim() || undefined });
  };

  const formatDate = (iso: string) => new Date(iso).toLocaleString();

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
            onClick={() => endMutation.mutate(activeSession.id)}
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
                      onClick={(e) => { e.stopPropagation(); deleteMutation.mutate(s.id); }}
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
