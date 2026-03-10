import React from "react";
import {
  Box,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography,
} from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "../Predictions/F1TeamBadge";

interface Props {
  title: string;
  standings: string[];
  teams: F1TeamDto[];
}

export default function F1ChampionshipCurrentStandings({
  title,
  standings,
  teams,
}: Props) {
  return (
    <Stack direction="column" spacing={1}>
      <Typography variant="h6">{title}</Typography>
      <TableContainer>
        <Table>
          <TableBody>
            {standings.map((driver, index) => (
              <TableRow key={driver}>
                <TableCell sx={{ padding: "1px", width: "10%" }}>
                  <IconButton size="small" sx={{ visibility: "hidden" }}>
                    <DragIndicatorIcon />
                  </IconButton>
                </TableCell>
                <TableCell sx={{ padding: "1px", width: "5%" }}>
                  <Typography>{index + 1}</Typography>
                </TableCell>
                <TableCell sx={{ padding: "1px", width: "100%" }}>
                  <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                    <F1TeamBadge driver={driver} teams={teams} />
                    <Typography>{driver}</Typography>
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Stack>
  );
}
