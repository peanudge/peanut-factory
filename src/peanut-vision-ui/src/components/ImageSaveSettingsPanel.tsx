import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import Alert from "@mui/material/Alert";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import FolderIcon from "@mui/icons-material/Folder";
import type { ImageSaveSettings } from "../api/types";
import { getImageSaveSettings, updateImageSaveSettings } from "../api/client";
import { queryKeys } from "../api/queryKeys";

const DEFAULT_SETTINGS: ImageSaveSettings = {
  outputDirectory: "CapturedImages",
};

export default function ImageSaveSettingsPanel() {
  const queryClient = useQueryClient();
  const [localSettings, setLocalSettings] = useState<ImageSaveSettings>(DEFAULT_SETTINGS);
  const [saved, setSaved] = useState(false);

  const { data: serverSettings } = useQuery({
    queryKey: queryKeys.imageSaveSettings,
    queryFn: getImageSaveSettings,
  });

  useEffect(() => {
    if (serverSettings) setLocalSettings(serverSettings);
  }, [serverSettings]);

  const saveMutation = useMutation({
    mutationFn: (settings: ImageSaveSettings) => updateImageSaveSettings(settings),
    onSuccess: (updated) => {
      queryClient.setQueryData(queryKeys.imageSaveSettings, updated);
      setLocalSettings(updated);
      setSaved(true);
      setTimeout(() => setSaved(false), 3000);
    },
  });

  return (
    <Accordion disableGutters variant="outlined">
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <FolderIcon fontSize="small" color="action" />
          <Typography variant="subtitle2">Image Save Settings</Typography>
        </Box>
      </AccordionSummary>
      <AccordionDetails>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            label="Output Directory"
            size="small"
            value={localSettings.outputDirectory}
            onChange={(e) => setLocalSettings({ outputDirectory: e.target.value })}
            helperText="절대 경로 또는 앱 루트 기준 상대 경로"
            fullWidth
          />

          <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
            <Button
              size="small"
              variant="contained"
              onClick={() => saveMutation.mutate(localSettings)}
              disabled={saveMutation.isPending}
            >
              Save Settings
            </Button>
          </Box>

          {saveMutation.isError && (
            <Alert severity="error" sx={{ py: 0 }}>
              {saveMutation.error instanceof Error ? saveMutation.error.message : "Failed to save settings"}
            </Alert>
          )}
          {saved && <Alert severity="success" sx={{ py: 0 }}>Settings saved</Alert>}
        </Box>
      </AccordionDetails>
    </Accordion>
  );
}
