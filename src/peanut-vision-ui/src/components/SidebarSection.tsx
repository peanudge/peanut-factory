import Box from "@mui/material/Box";
import Divider from "@mui/material/Divider";
import Typography from "@mui/material/Typography";

interface SidebarSectionProps {
  label: string;
  children: React.ReactNode;
}

export default function SidebarSection({ label, children }: SidebarSectionProps) {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
      <Typography variant="overline" color="text.secondary" sx={{ lineHeight: 1 }}>
        {label}
      </Typography>
      {children}
      <Divider />
    </Box>
  );
}
