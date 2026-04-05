import { useEffect, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import FormControlLabel from "@mui/material/FormControlLabel";
import MenuItem from "@mui/material/MenuItem";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import Alert from "@mui/material/Alert";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import FolderIcon from "@mui/icons-material/Folder";
import type { ImageSaveSettings, SaveImageFormat, SubfolderStrategy } from "../api/types";
import { getImageSaveSettings, updateImageSaveSettings } from "../api/client";
import { queryKeys } from "../api/queryKeys";

const FORMAT_OPTIONS: { value: SaveImageFormat; label: string }[] = [
  { value: "png", label: "PNG" },
  { value: "bmp", label: "BMP" },
  { value: "raw", label: "RAW" },
];

const SUBFOLDER_OPTIONS: { value: SubfolderStrategy; label: string }[] = [
  { value: "none", label: "None" },
  { value: "byDate", label: "By Date (YYYY-MM-DD)" },
  { value: "bySession", label: "By Session" },
  { value: "byProfile", label: "By Profile" },
];

const DEFAULT_SETTINGS: ImageSaveSettings = {
  outputDirectory: "CapturedImages",
  format: "png",
  filenamePrefix: "capture",
  timestampFormat: "yyyyMMdd_HHmmss_fff",
  includeSequenceNumber: false,
  subfolderStrategy: "none",
  autoSave: true,
};

// Characters not allowed in directory paths (Windows-compatible; allows / \ : as path separators)
const INVALID_PATH_CHARS = /[<>"|?*\x00-\x1f]/;
// Characters not allowed in file name prefixes (includes path separators)
const INVALID_FILENAME_CHARS = /[<>:"/\\|?*\x00-\x1f]/;

function validateOutputDirectory(value: string): string {
  if (!value.trim()) return "Output directory is required";
  if (INVALID_PATH_CHARS.test(value)) return 'Path contains invalid characters: < > " | ? *';
  return "";
}

function validateFilenamePrefix(value: string): string {
  if (!value.trim()) return "Filename prefix is required";
  if (INVALID_FILENAME_CHARS.test(value)) return 'Prefix contains invalid characters: < > : " / \\ | ? *';
  return "";
}

export default function ImageSaveSettingsPanel() {
  const queryClient = useQueryClient();
  const [localSettings, setLocalSettings] = useState<ImageSaveSettings>(DEFAULT_SETTINGS);
  const [saved, setSaved] = useState(false);
  const [outputDirError, setOutputDirError] = useState("");
  const [filenamePrefixError, setFilenamePrefixError] = useState("");

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

  const update = <K extends keyof ImageSaveSettings>(key: K, value: ImageSaveSettings[K]) => {
    setLocalSettings((prev) => ({ ...prev, [key]: value }));
    if (key === "outputDirectory") setOutputDirError(validateOutputDirectory(value as string));
    if (key === "filenamePrefix") setFilenamePrefixError(validateFilenamePrefix(value as string));
  };

  const hasValidationErrors = outputDirError !== "" || filenamePrefixError !== "";

  const settings = localSettings;

  const exampleFilename = [
    settings.filenamePrefix || "capture",
    "20260320_143000_123",
    settings.includeSequenceNumber ? "00001" : null,
  ]
    .filter(Boolean)
    .join("_") + `.${settings.format}`;

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
          <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap" }}>
            <TextField
              label="Output Directory"
              size="small"
              value={settings.outputDirectory}
              onChange={(e) => update("outputDirectory", e.target.value)}
              error={outputDirError !== ""}
              helperText={outputDirError || "Relative to app root, or absolute path"}
              sx={{ flexGrow: 1, minWidth: 220 }}
            />
            <TextField
              select
              label="Format"
              size="small"
              value={settings.format}
              onChange={(e) => update("format", e.target.value as SaveImageFormat)}
              sx={{ width: 110 }}
            >
              {FORMAT_OPTIONS.map((o) => (
                <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>
              ))}
            </TextField>
          </Box>

          <Box sx={{ display: "flex", gap: 2, flexWrap: "wrap" }}>
            <TextField
              label="Filename Prefix"
              size="small"
              value={settings.filenamePrefix}
              onChange={(e) => update("filenamePrefix", e.target.value)}
              error={filenamePrefixError !== ""}
              helperText={filenamePrefixError || " "}
              sx={{ width: 160 }}
            />
            <TextField
              label="Timestamp Format"
              size="small"
              value={settings.timestampFormat}
              onChange={(e) => update("timestampFormat", e.target.value)}
              helperText=".NET DateTime format"
              sx={{ width: 200 }}
            />
            <TextField
              select
              label="Subfolder"
              size="small"
              value={settings.subfolderStrategy}
              onChange={(e) => update("subfolderStrategy", e.target.value as SubfolderStrategy)}
              sx={{ width: 200 }}
            >
              {SUBFOLDER_OPTIONS.map((o) => (
                <MenuItem key={o.value} value={o.value}>{o.label}</MenuItem>
              ))}
            </TextField>
          </Box>

          <Box sx={{ display: "flex", gap: 2, alignItems: "center", flexWrap: "wrap" }}>
            <FormControlLabel
              control={
                <Checkbox
                  size="small"
                  checked={settings.autoSave}
                  onChange={(e) => update("autoSave", e.target.checked)}
                />
              }
              label="Auto-save on capture"
            />
            <FormControlLabel
              control={
                <Checkbox
                  size="small"
                  checked={settings.includeSequenceNumber}
                  onChange={(e) => update("includeSequenceNumber", e.target.checked)}
                />
              }
              label="Include sequence number"
            />
          </Box>

          <Box sx={{ display: "flex", alignItems: "center", gap: 2, flexWrap: "wrap" }}>
            <Typography variant="caption" color="text.secondary">
              Example: <strong>{exampleFilename}</strong>
            </Typography>
            <Box sx={{ flexGrow: 1 }} />
            <Button
              size="small"
              variant="contained"
              onClick={() => saveMutation.mutate(localSettings)}
              disabled={saveMutation.isPending || hasValidationErrors}
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
