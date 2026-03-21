import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
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
import SaveIcon from "@mui/icons-material/Save";
import FolderOpenIcon from "@mui/icons-material/FolderOpen";
import DeleteIcon from "@mui/icons-material/Delete";
import type { AcquisitionPreset, TriggerModeOption } from "../api/types";
import { getPresets, savePreset, deletePreset } from "../api/client";
import { queryKeys } from "../api/queryKeys";

interface Props {
  profileId: string;
  triggerMode: TriggerModeOption;
  frameCount: number | null;
  intervalMs: number | null;
  onLoadPreset: (preset: AcquisitionPreset) => void;
  disabled?: boolean;
}

export default function PresetSelector({
  profileId,
  triggerMode,
  frameCount,
  intervalMs,
  onLoadPreset,
  disabled,
}: Props) {
  const queryClient = useQueryClient();
  const [loadOpen, setLoadOpen] = useState(false);
  const [saveOpen, setSaveOpen] = useState(false);
  const [presetName, setPresetName] = useState("");

  const { data: presets = [] } = useQuery({
    queryKey: queryKeys.presets,
    queryFn: getPresets,
  });

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: queryKeys.presets });

  const saveMutation = useMutation({
    mutationFn: (preset: AcquisitionPreset) => savePreset(preset),
    onSuccess: () => {
      setPresetName("");
      setSaveOpen(false);
      invalidate();
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (name: string) => deletePreset(name),
    onSuccess: invalidate,
  });

  const busy = saveMutation.isPending || deleteMutation.isPending;

  const handleSave = () => {
    if (!presetName.trim()) return;
    saveMutation.mutate({ name: presetName.trim(), profileId, triggerMode, frameCount, intervalMs });
  };

  const handleLoad = (preset: AcquisitionPreset) => {
    onLoadPreset(preset);
    setLoadOpen(false);
  };

  return (
    <Box sx={{ display: "flex", gap: 0.5 }}>
      <Button
        size="small"
        startIcon={<SaveIcon />}
        onClick={() => setSaveOpen(true)}
        disabled={disabled || !profileId}
      >
        Save Preset
      </Button>
      <Button
        size="small"
        startIcon={<FolderOpenIcon />}
        onClick={() => setLoadOpen(true)}
        disabled={disabled}
      >
        Load Preset
      </Button>

      {/* Save Dialog */}
      <Dialog open={saveOpen} onClose={() => setSaveOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Save Acquisition Preset</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Preset Name"
            fullWidth
            value={presetName}
            onChange={(e) => setPresetName(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSave()}
          />
          <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: "block" }}>
            Profile: {profileId || "none"} | Trigger: {triggerMode}
            {frameCount != null && ` | Frames: ${frameCount}`}
            {intervalMs != null && ` | Interval: ${intervalMs}ms`}
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSaveOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={busy || !presetName.trim()} variant="contained">
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* Load Dialog */}
      <Dialog open={loadOpen} onClose={() => setLoadOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Load Preset</DialogTitle>
        <DialogContent>
          {presets.length === 0 ? (
            <Typography color="text.secondary" sx={{ py: 2 }}>
              No presets saved yet
            </Typography>
          ) : (
            <List dense>
              {presets.map((p) => (
                <ListItemButton key={p.name} onClick={() => handleLoad(p)} sx={{ borderRadius: 1 }}>
                  <ListItemText
                    primary={p.name}
                    secondary={`${p.profileId} | ${p.triggerMode ?? "soft"}${p.frameCount != null ? ` | ${p.frameCount} frames` : ""}${p.intervalMs != null ? ` | ${p.intervalMs}ms` : ""}`}
                  />
                  <IconButton
                    edge="end"
                    size="small"
                    onClick={(e) => { e.stopPropagation(); deleteMutation.mutate(p.name); }}
                    disabled={busy}
                  >
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </ListItemButton>
              ))}
            </List>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setLoadOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
