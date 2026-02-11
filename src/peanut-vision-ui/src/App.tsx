import { useState } from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import CameraIcon from "@mui/icons-material/CameraAlt";
import SystemTab from "./tabs/SystemTab";
import AcquisitionTab from "./tabs/AcquisitionTab";
import CalibrationTab from "./tabs/CalibrationTab";

export default function App() {
  const [tab, setTab] = useState(0);

  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
      <AppBar position="static" elevation={0}>
        <Toolbar variant="dense">
          <CameraIcon sx={{ mr: 1 }} />
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            PeanutVision
          </Typography>
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
          <Tab label="Calibration" />
        </Tabs>
      </AppBar>

      <Container maxWidth="lg" sx={{ py: 3, flexGrow: 1 }}>
        {tab === 0 && <SystemTab />}
        {tab === 1 && <AcquisitionTab />}
        {tab === 2 && <CalibrationTab />}
      </Container>
    </Box>
  );
}
