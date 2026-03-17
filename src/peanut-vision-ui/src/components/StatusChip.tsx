import Chip from "@mui/material/Chip";

interface Props {
  active: boolean;
  label?: string;
  hasWarnings?: boolean;
  hasErrors?: boolean;
}

export default function StatusChip({ active, label, hasWarnings, hasErrors }: Props) {
  return (
    <Chip
      label={label ?? (active ? "Active" : "Inactive")}
      color={hasErrors ? "error" : hasWarnings ? "warning" : active ? "success" : "default"}
      size="small"
      variant="outlined"
    />
  );
}
