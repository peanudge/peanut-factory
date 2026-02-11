import { useEffect, useState } from "react";
import Box from "@mui/material/Box";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import Typography from "@mui/material/Typography";
import Collapse from "@mui/material/Collapse";
import IconButton from "@mui/material/IconButton";
import Alert from "@mui/material/Alert";
import CircularProgress from "@mui/material/CircularProgress";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import StatusChip from "../components/StatusChip";
import type { BoardInfo, BoardStatus, CameraProfile } from "../api/types";
import { getBoards, getBoardStatus, getCameras } from "../api/client";

function BoardRow({ board }: { board: BoardInfo }) {
  const [open, setOpen] = useState(false);
  const [status, setStatus] = useState<BoardStatus | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleToggle = async () => {
    if (!open && !status) {
      setLoading(true);
      setError("");
      try {
        setStatus(await getBoardStatus(board.index));
      } catch (e) {
        setError(e instanceof Error ? e.message : "Failed to load");
      } finally {
        setLoading(false);
      }
    }
    setOpen(!open);
  };

  return (
    <>
      <TableRow
        hover
        sx={{ cursor: "pointer", "& > *": { borderBottom: "unset" } }}
        onClick={handleToggle}
      >
        <TableCell padding="checkbox">
          <IconButton size="small">
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        <TableCell>{board.index}</TableCell>
        <TableCell>{board.boardName}</TableCell>
        <TableCell>{board.boardType}</TableCell>
        <TableCell>{board.serialNumber}</TableCell>
        <TableCell>{board.pciPosition}</TableCell>
      </TableRow>
      <TableRow>
        <TableCell sx={{ py: 0 }} colSpan={6}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ m: 2 }}>
              {loading && <CircularProgress size={20} />}
              {error && <Alert severity="error">{error}</Alert>}
              {status && (
                <Table size="small">
                  <TableBody>
                    {([
                      ["Input Connector", status.inputConnector],
                      ["Input State", status.inputState],
                      ["Signal Strength", status.signalStrength],
                      ["Output State", status.outputState],
                      ["Camera Link", status.cameraLinkStatus],
                      ["Sync Errors", status.syncErrors],
                      ["Clock Errors", status.clockErrors],
                      ["Grabber Errors", status.grabberErrors],
                      ["Frame Trigger Violations", status.frameTriggerViolations],
                      ["Line Trigger Violations", status.lineTriggerViolations],
                      ["PCIe Link", status.pcieLinkInfo],
                    ] as [string, string | number][]).map(([label, value]) => (
                      <TableRow key={label}>
                        <TableCell sx={{ fontWeight: 500, width: 220 }}>
                          {label}
                        </TableCell>
                        <TableCell>{String(value)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              )}
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
}

export default function SystemTab() {
  const [boards, setBoards] = useState<BoardInfo[]>([]);
  const [cameras, setCameras] = useState<CameraProfile[]>([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const [b, c] = await Promise.all([getBoards(), getCameras()]);
        if (!cancelled) {
          setBoards(b);
          setCameras(c);
        }
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : "Failed");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => { cancelled = true; };
  }, []);

  if (loading) return <CircularProgress sx={{ m: 4 }} />;
  if (error) return <Alert severity="error" sx={{ m: 2 }}>{error}</Alert>;

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
