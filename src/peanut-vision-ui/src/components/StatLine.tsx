import { memo } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

interface Props {
  label: string;
  value: string | number;
}

const StatLine = memo(function StatLine({ label, value }: Props) {
  return (
    <Box sx={{ display: "flex", justifyContent: "space-between" }}>
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="body2" fontFamily="monospace">
        {value}
      </Typography>
    </Box>
  );
});

export default StatLine;
