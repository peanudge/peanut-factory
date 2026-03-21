import { useQuery } from "@tanstack/react-query";
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
import { getBoards, getCameras } from "../api/client";
import { queryKeys } from "../api/queryKeys";

export default function SystemTab() {
  const { data: boards, isLoading: boardsLoading, error: boardsError } = useQuery({
    queryKey: queryKeys.boards,
    queryFn: getBoards,
  });

  const { data: cameras, isLoading: camsLoading, error: camsError } = useQuery({
    queryKey: queryKeys.cameras,
    queryFn: getCameras,
  });

  const loading = boardsLoading || camsLoading;
  const error = boardsError ?? camsError;

  if (loading) return <CircularProgress sx={{ m: 4 }} />;
  if (error) return <Alert severity="error" sx={{ m: 2 }}>{error instanceof Error ? error.message : "Failed to load"}</Alert>;

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
              {!boards?.length ? (
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
          Camera Files
        </Typography>
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>File Name</TableCell>
                <TableCell>Manufacturer</TableCell>
                <TableCell>Model</TableCell>
                <TableCell>Color Format</TableCell>
                <TableCell>Resolution</TableCell>
                <TableCell>Spectrum</TableCell>
                <TableCell>Trigger</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {!cameras?.length ? (
                <TableRow>
                  <TableCell colSpan={7} align="center">
                    No camera files found
                  </TableCell>
                </TableRow>
              ) : (
                cameras.map((c) => (
                  <TableRow key={c.fileName} hover>
                    <TableCell sx={{ fontFamily: "monospace", fontSize: "0.8rem" }}>
                      {c.fileName}
                    </TableCell>
                    <TableCell>{c.manufacturer}</TableCell>
                    <TableCell>{c.cameraModel}</TableCell>
                    <TableCell>{c.colorFormat}</TableCell>
                    <TableCell>
                      {c.width}&times;{c.height}
                    </TableCell>
                    <TableCell>{c.spectrum}</TableCell>
                    <TableCell>
                      <StatusChip
                        active={c.trigMode === "IMMEDIATE"}
                        label={c.trigMode}
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
