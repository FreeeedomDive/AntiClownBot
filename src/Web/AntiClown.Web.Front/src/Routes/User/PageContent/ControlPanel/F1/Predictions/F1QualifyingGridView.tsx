import React from "react";
import {
  Box,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography,
} from "@mui/material";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "./F1TeamBadge";

interface Props {
  grid: string[];
  teams: F1TeamDto[];
  showTitle?: boolean;
}

export default function F1QualifyingGridView({ grid, teams, showTitle = true }: Props) {
  return (
    <Stack direction="column" spacing={1}>
      {showTitle && (
        <Typography variant="h6" align="center">
          Стартовая решётка
        </Typography>
      )}
      <TableContainer>
        <Table>
          <TableBody>
            {grid.map((driver, index) => (
              <TableRow key={driver}>
                <TableCell sx={{ padding: "4px 8px", width: "10%" }}>
                  <Typography variant="body1" fontWeight="bold">
                    {index + 1}
                  </Typography>
                </TableCell>
                <TableCell sx={{ padding: "4px 8px", width: "100%" }}>
                  <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                    <F1TeamBadge driver={driver} teams={teams} />
                    <Typography variant="body1">{driver}</Typography>
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
