import { useState } from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import FolderIcon from "@mui/icons-material/Folder";
import CameraIcon from "@mui/icons-material/CameraAlt";
import SystemTab from "./tabs/SystemTab";
import AcquisitionTab from "./tabs/AcquisitionTab";
import GalleryTab from "./tabs/GalleryTab";
import LatencyTab from "./tabs/LatencyTab";

export default function App() {
  const [tab, setTab] = useState(0);
  const [sessionName, setSessionName] = useState<string | null>(null);

  return (
    <Box sx={{ display: "flex", flexDirection: "column", height: "100vh" }}>
      <AppBar position="static" elevation={0} sx={{ borderBottom: "1px solid", borderColor: "divider" }}>
        <Toolbar variant="dense">
          <CameraIcon sx={{ mr: 1 }} />
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            PeanutVision
          </Typography>
          {sessionName && (
            <Chip
              icon={<FolderIcon />}
              label={sessionName}
              size="small"
              color="secondary"
              variant="outlined"
              sx={{ mr: 1 }}
            />
          )}
        </Toolbar>
        <Tabs
          value={tab}
          onChange={(_, v) => setTab(v)}
          sx={{ px: 2 }}
          textColor="inherit"
          indicatorColor="secondary"
        >
          <Tab label="System" />
          <Tab label="Acquisition" />
          <Tab label="Gallery" />
          <Tab label="Latency" />
        </Tabs>
      </AppBar>

      <Container maxWidth="lg" sx={{ py: 3, flexGrow: 1, display: tab === 0 ? undefined : "none" }}>
        <SystemTab />
      </Container>
      <Box sx={{ flexGrow: 1, overflow: "hidden", display: tab === 1 ? "flex" : "none" }}>
        <AcquisitionTab onSessionChange={setSessionName} />
      </Box>
      <Box sx={{ flexGrow: 1, overflow: "hidden", display: tab === 2 ? "flex" : "none" }}>
        <GalleryTab />
      </Box>
      <Box sx={{ flexGrow: 1, overflow: "auto", display: tab === 3 ? "block" : "none" }}>
        <LatencyTab />
      </Box>
    </Box>
  );
}
