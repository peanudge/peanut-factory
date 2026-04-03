import { useCallback, useEffect, useRef, useState } from "react";
import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import IconButton from "@mui/material/IconButton";
import InputBase from "@mui/material/InputBase";
import Typography from "@mui/material/Typography";
import AddIcon from "@mui/icons-material/Add";
import CheckIcon from "@mui/icons-material/Check";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { patchImageAnnotations } from "../api/client";
import { queryKeys } from "../api/queryKeys";

interface Props {
  imageId: string;
  initialTags: string[];
  initialNotes: string;
}

const DEBOUNCE_MS = 500;

export default function AnnotationPanel({ imageId, initialTags, initialNotes }: Props) {
  const queryClient = useQueryClient();

  const [tags, setTags] = useState<string[]>(initialTags);
  const [notes, setNotes] = useState(initialNotes);
  const [tagInput, setTagInput] = useState("");
  const [showTagInput, setShowTagInput] = useState(false);
  const [savedRecently, setSavedRecently] = useState(false);

  const notesDebounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const latestTagsRef = useRef(tags);
  const latestNotesRef = useRef(notes);
  latestTagsRef.current = tags;
  latestNotesRef.current = notes;

  // Sync when a different image is selected
  useEffect(() => {
    setTags(initialTags);
    setNotes(initialNotes);
    setShowTagInput(false);
    setTagInput("");
    setSavedRecently(false);
  }, [imageId, initialTags, initialNotes]);

  const mutation = useMutation({
    mutationFn: ({ t, n }: { t: string[]; n: string }) =>
      patchImageAnnotations(imageId, t, n),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.images() });
      setSavedRecently(true);
      setTimeout(() => setSavedRecently(false), 1500);
    },
  });

  const save = useCallback(
    (t: string[], n: string) => mutation.mutate({ t, n }),
    [mutation],
  );

  const removeTag = (tag: string) => {
    const next = tags.filter((t) => t !== tag);
    setTags(next);
    save(next, latestNotesRef.current);
  };

  const addTag = () => {
    const trimmed = tagInput.trim();
    if (!trimmed || tags.includes(trimmed)) {
      setTagInput("");
      setShowTagInput(false);
      return;
    }
    const next = [...tags, trimmed];
    setTags(next);
    setTagInput("");
    setShowTagInput(false);
    save(next, latestNotesRef.current);
  };

  const handleNotesChange = (value: string) => {
    setNotes(value);
    if (notesDebounceRef.current) clearTimeout(notesDebounceRef.current);
    notesDebounceRef.current = setTimeout(() => {
      save(latestTagsRef.current, value);
    }, DEBOUNCE_MS);
  };

  const handleTagKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") { e.preventDefault(); addTag(); }
    if (e.key === "Escape") { setTagInput(""); setShowTagInput(false); }
  };

  return (
    <Box sx={{ px: 0.5, py: 0.75, display: "flex", flexDirection: "column", gap: 0.75 }}>
      {/* Tags row */}
      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5, alignItems: "center" }}>
        {tags.map((tag) => (
          <Chip
            key={tag}
            label={tag}
            size="small"
            onDelete={() => removeTag(tag)}
            sx={{ fontSize: "0.65rem", height: 20 }}
          />
        ))}
        {showTagInput ? (
          <Box sx={{ display: "flex", alignItems: "center", gap: 0.25 }}>
            <InputBase
              autoFocus
              value={tagInput}
              onChange={(e) => setTagInput(e.target.value)}
              onKeyDown={handleTagKeyDown}
              onBlur={addTag}
              placeholder="tag name"
              sx={{
                fontSize: "0.7rem",
                border: "1px solid",
                borderColor: "primary.main",
                borderRadius: 0.5,
                px: 0.5,
                height: 20,
                width: 80,
              }}
              inputProps={{ style: { padding: 0 } }}
            />
            <IconButton size="small" onClick={addTag} sx={{ p: 0.125 }}>
              <CheckIcon sx={{ fontSize: 12 }} />
            </IconButton>
          </Box>
        ) : (
          <IconButton
            size="small"
            onClick={() => setShowTagInput(true)}
            title="Add tag"
            sx={{ p: 0.125, border: "1px dashed", borderColor: "divider", borderRadius: 0.5 }}
          >
            <AddIcon sx={{ fontSize: 12 }} />
          </IconButton>
        )}
      </Box>

      {/* Notes textarea */}
      <Box
        component="textarea"
        value={notes}
        onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) => handleNotesChange(e.target.value)}
        placeholder="Notes…"
        rows={2}
        sx={{
          width: "100%",
          resize: "vertical",
          fontSize: "0.7rem",
          fontFamily: "inherit",
          border: "1px solid",
          borderColor: "divider",
          borderRadius: 0.5,
          px: 0.75,
          py: 0.5,
          bgcolor: "background.paper",
          color: "text.primary",
          outline: "none",
          boxSizing: "border-box",
          "&:focus": { borderColor: "primary.main" },
        }}
      />

      {/* Saved feedback */}
      {savedRecently && (
        <Typography variant="caption" color="success.main" sx={{ fontSize: "0.65rem", lineHeight: 1 }}>
          Saved
        </Typography>
      )}
    </Box>
  );
}
