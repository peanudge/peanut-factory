import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { ChannelState } from "../api/types";

interface Props {
  state: ChannelState;
}

const stateConfig: Record<ChannelState, { color: string; label: string; pulse: boolean }> = {
  none:   { color: "#9e9e9e", label: "No Channel",           pulse: false },
  idle:   { color: "#ffc107", label: "Channel Idle (IDLE)",  pulse: false },
  active: { color: "#4caf50", label: "Channel Active (ACTIVE)", pulse: true },
};

export default function ChannelStateIndicator({ state }: Props) {
  const { color, label, pulse } = stateConfig[state];

  return (
    <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
      <Box
        sx={{
          width: 12,
          height: 12,
          borderRadius: "50%",
          backgroundColor: color,
          flexShrink: 0,
          ...(pulse && {
            animation: "pulse 1.5s ease-in-out infinite",
            "@keyframes pulse": {
              "0%, 100%": { opacity: 1, transform: "scale(1)" },
              "50%":       { opacity: 0.6, transform: "scale(1.3)" },
            },
          }),
        }}
      />
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
    </Box>
  );
}
