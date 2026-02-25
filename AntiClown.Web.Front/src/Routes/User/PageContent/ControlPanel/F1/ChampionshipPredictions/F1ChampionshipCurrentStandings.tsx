import React from "react";
import {
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography,
} from "@mui/material";

interface Props {
  title: string;
  standings: string[];
}

export default function F1ChampionshipCurrentStandings({
  title,
  standings,
}: Props) {
  return (
    <Stack direction="column" spacing={1}>
      <Typography variant="h6">{title}</Typography>
      <TableContainer>
        <Table>
          <TableBody>
            {standings.map((driver, index) => (
              <TableRow key={driver}>
                <TableCell sx={{ padding: "1px", width: "5%" }}>
                  <Typography>{index + 1}</Typography>
                </TableCell>
                <TableCell sx={{ padding: "1px", width: "100%" }}>
                  <Typography>{driver}</Typography>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Stack>
  );
}
