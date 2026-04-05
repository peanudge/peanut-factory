import { useState } from "react";
import { Link } from "react-router-dom";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import IconButton from "@mui/material/IconButton";
import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import Toolbar from "@mui/material/Toolbar";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AcquisitionStatsTab from "../tabs/AcquisitionStatsTab";
import LatencyTab from "../tabs/LatencyTab";
import SystemTab from "../tabs/SystemTab";

export default function BackofficePage() {
  const [tab, setTab] = useState(0);

  return (
    <Box sx={{ display: "flex", flexDirection: "column", height: "100vh" }}>
      <AppBar position="static" elevation={0} sx={{ borderBottom: "1px solid", borderColor: "divider" }}>
        <Toolbar variant="dense">
          <Tooltip title="돌아가기">
            <IconButton component={Link} to="/" color="inherit" edge="start" size="small" sx={{ mr: 1 }}>
              <ArrowBackIcon />
            </IconButton>
          </Tooltip>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            PeanutVision — Backoffice
          </Typography>
        </Toolbar>
        <Tabs
          value={tab}
          onChange={(_, v) => setTab(v)}
          sx={{ px: 2 }}
          textColor="inherit"
          indicatorColor="secondary"
        >
          <Tab label="Latency" />
          <Tab label="System" />
          <Tab label="취득 통계" />
        </Tabs>
      </AppBar>

      <Box sx={{ flexGrow: 1, overflow: "auto", display: tab === 0 ? "block" : "none" }}>
        <LatencyTab />
      </Box>
      <Container maxWidth="lg" sx={{ py: 3, flexGrow: 1, display: tab === 1 ? undefined : "none" }}>
        <SystemTab />
      </Container>
      <Box sx={{ flexGrow: 1, overflow: "auto", display: tab === 2 ? "block" : "none" }}>
        <AcquisitionStatsTab />
      </Box>
    </Box>
  );
}
