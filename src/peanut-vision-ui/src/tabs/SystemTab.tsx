import Box from "@mui/material/Box";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import Alert from "@mui/material/Alert";
import CircularProgress from "@mui/material/CircularProgress";
import StatusChip from "../components/StatusChip";
import BoardRow from "../components/BoardRow";
import type { BoardInfo, CameraProfile } from "../api/types";
import { getBoards, getCameras } from "../api/client";
import { useApiData } from "../hooks/useApiData";

export default function SystemTab() {
  const { data, loading, error } = useApiData(
    () => Promise.all([getBoards(), getCameras()]),
  );

  if (loading) return <CircularProgress sx={{ m: 4 }} />;
  if (error) return <Alert severity="error" sx={{ m: 2 }}>{error}</Alert>;

  const [boards, cameras] = data as [BoardInfo[], CameraProfile[]];

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 4 }}>
      {/* Boards */}
      <Box>
        <Typography variant="h6" gutterBottom>
          Frame Grabber Boards
        </Typography>
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell padding="checkbox" />
                <TableCell>#</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Serial</TableCell>
                <TableCell>PCI Position</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {boards.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} align="center">
                    No boards detected
                  </TableCell>
                </TableRow>
              ) : (
                boards.map((b) => <BoardRow key={b.index} board={b} />)
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>

      {/* Cameras */}
      <Box>
        <Typography variant="h6" gutterBottom>
          Camera Profiles
        </Typography>
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>ID</TableCell>
                <TableCell>Display Name</TableCell>
                <TableCell>Manufacturer</TableCell>
                <TableCell>Model</TableCell>
                <TableCell>Pixel Format</TableCell>
                <TableCell>Resolution</TableCell>
                <TableCell>Trigger</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {cameras.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center">
                    No camera profiles found
                  </TableCell>
                </TableRow>
              ) : (
                cameras.map((c) => (
                  <TableRow key={c.id} hover>
                    <TableCell sx={{ fontFamily: "monospace", fontSize: "0.8rem" }}>
                      {c.id}
                    </TableCell>
                    <TableCell>{c.displayName}</TableCell>
                    <TableCell>{c.manufacturer}</TableCell>
                    <TableCell>{c.model}</TableCell>
                    <TableCell>{c.pixelFormat}</TableCell>
                    <TableCell>
                      {c.expectedWidth}&times;{c.expectedHeight}
                    </TableCell>
                    <TableCell>
                      <StatusChip
                        active={c.triggerMode.includes("FREE")}
                        label={c.triggerMode.replace("MC_TrigMode_", "")}
                      />
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>
    </Box>
  );
}
