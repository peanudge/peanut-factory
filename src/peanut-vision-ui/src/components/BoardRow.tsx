import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import Box from "@mui/material/Box";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableRow from "@mui/material/TableRow";
import Collapse from "@mui/material/Collapse";
import IconButton from "@mui/material/IconButton";
import CircularProgress from "@mui/material/CircularProgress";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import type { BoardInfo } from "../api/types";
import { getBoardStatus } from "../api/client";
import { queryKeys } from "../api/queryKeys";
import { useToast } from "../contexts/ToastContext";

export default function BoardRow({ board }: { board: BoardInfo }) {
  const [open, setOpen] = useState(false);
  const { toast } = useToast();

  const { data: status, isFetching, error } = useQuery({
    queryKey: queryKeys.boardStatus(board.index),
    queryFn: () => getBoardStatus(board.index),
    enabled: open,
    staleTime: Infinity,
  });

  useEffect(() => {
    if (error) toast(error instanceof Error ? error.message : "Failed to load board status", "error");
  }, [error, toast]);

  return (
    <>
      <TableRow
        hover
        sx={{ cursor: "pointer", "& > *": { borderBottom: "unset" } }}
        onClick={() => setOpen(!open)}
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
              {isFetching && <CircularProgress size={20} />}
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
