import React from "react";
import {
  Box,
  Chip,
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
  tenthPlaceDriver?: string;
  dnfDrivers?: string[];
  positionDriver?: string;
  predictedPosition?: number | null;
}

export default function F1QualifyingGridView({
  grid,
  teams,
  showTitle = true,
  tenthPlaceDriver,
  dnfDrivers = [],
  positionDriver,
  predictedPosition,
}: Props) {
  return (
    <Stack direction="column" spacing={1}>
      {showTitle && (
        <Typography variant="h6" sx={{ px: 2 }}>
          Стартовая решётка
        </Typography>
      )}
      <TableContainer>
        <Table
          size="small"
          sx={{
            "& .MuiTableCell-root": { borderColor: "rgba(255,255,255,0.08)" },
          }}
        >
          <TableBody>
            {grid.map((driver, index) => (
              <TableRow
                key={driver}
                sx={{
                  bgcolor:
                    driver === tenthPlaceDriver
                      ? "rgba(144,202,249,0.08)"
                      : dnfDrivers.includes(driver)
                        ? "rgba(255,167,38,0.05)"
                        : "transparent",
                }}
              >
                <TableCell sx={{ py: 0.75, px: 2, width: 40 }}>
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ fontVariantNumeric: "tabular-nums" }}
                  >
                    {index + 1}
                  </Typography>
                </TableCell>
                <TableCell sx={{ py: 0.75, px: 1 }}>
                  <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                    <F1TeamBadge driver={driver} teams={teams} />
                    <Typography
                      variant="body2"
                      sx={{
                        color:
                          driver === positionDriver
                            ? "primary.main"
                            : "text.primary",
                        fontWeight: driver === positionDriver ? 600 : 400,
                      }}
                      title={
                        driver === positionDriver
                          ? "Пилот для прогноза позиции"
                          : undefined
                      }
                    >
                      {driver}
                    </Typography>
                  </Box>
                </TableCell>
                <TableCell align="right" sx={{ py: 0.75, pl: 0, pr: 1 }}>
                  <Box
                    sx={{
                      display: "flex",
                      gap: 0.5,
                      justifyContent: "flex-end",
                    }}
                  >
                    {driver === tenthPlaceDriver && (
                      <Chip
                        size="small"
                        label="P10"
                        color="primary"
                        variant="outlined"
                        sx={{
                          height: 20,
                          fontSize: "0.65rem",
                          "& .MuiChip-label": { px: 0.75 },
                        }}
                      />
                    )}
                    {driver === positionDriver && (
                      <Chip
                        size="small"
                        label={
                          predictedPosition ? `P${predictedPosition}` : "P?"
                        }
                        title={
                          predictedPosition
                            ? "Прогноз места на финише"
                            : "Место на финише ещё не выбрано"
                        }
                        color="primary"
                        variant="outlined"
                        sx={{
                          height: 20,
                          fontSize: "0.65rem",
                          "& .MuiChip-label": { px: 0.75 },
                        }}
                      />
                    )}
                    {dnfDrivers.includes(driver) && (
                      <Chip
                        size="small"
                        label="DNF"
                        color="warning"
                        variant="outlined"
                        sx={{
                          height: 20,
                          fontSize: "0.65rem",
                          "& .MuiChip-label": { px: 0.75 },
                        }}
                      />
                    )}
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
