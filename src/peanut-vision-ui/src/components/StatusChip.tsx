import Chip from "@mui/material/Chip";

interface Props {
  active: boolean;
  label?: string;
}

export default function StatusChip({ active, label }: Props) {
  return (
    <Chip
      label={label ?? (active ? "Active" : "Inactive")}
      color={active ? "success" : "default"}
      size="small"
      variant="outlined"
    />
  );
}
