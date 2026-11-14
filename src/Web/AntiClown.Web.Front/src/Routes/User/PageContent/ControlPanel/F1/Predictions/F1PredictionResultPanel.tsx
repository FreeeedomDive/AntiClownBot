import {
  Box,
  Chip,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography,
} from "@mui/material";
import {
  InternalPredictionDto,
  InternalRaceResultDto,
} from "../../../../../../Dto/F1Predictions/F1InternalDto";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "./F1TeamBadge";

interface Props {
  result: InternalRaceResultDto | null;
  prediction: InternalPredictionDto | null;
  teams: F1TeamDto[];
  positionDriver?: string;
}

const PANEL_SX = {
  borderRadius: 2,
  bgcolor: "rgba(255,255,255,0.025)",
  backgroundImage: "none",
  minWidth: 0,
} as const;

export default function F1PredictionResultPanel({
  result,
  prediction,
  teams,
  positionDriver,
}: Props) {
  const points = result?.points;
  const categories = points
    ? [
        { label: "10 место", points: points.tenthPlacePoints },
        {
          label: result.season === 2023 ? "Первый DNF" : "DNF",
          points: points.dnfsPoints,
        },
        { label: "Инциденты", points: points.safetyCarsPoints },
        { label: "Отрыв победителя", points: points.firstPlaceLeadPoints },
        { label: "Команды", points: points.teamMatesPoints },
        { label: "Позиция гонщика", points: points.driverPositionPoints },
      ].filter((category) => category.points !== null)
    : [];

  return (
    <Stack spacing={2} sx={{ minWidth: 0 }}>
      <Paper variant="outlined" sx={PANEL_SX}>
        <Typography fontWeight={600} sx={{ p: 2 }}>
          Результат гонки
        </Typography>
        {!result ? (
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ px: 2, pb: 2 }}
          >
            Результат гонки ещё не опубликован.
          </Typography>
        ) : (
          <>
            <TableContainer>
              <Table
                size="small"
                aria-label="Результат гонки"
                sx={{
                  tableLayout: "fixed",
                  "& td": {
                    py: 0.75,
                    px: 1,
                    borderColor: "rgba(255,255,255,0.08)",
                  },
                  "& tr:last-child td": { borderBottom: 0 },
                }}
              >
                <colgroup>
                  <col style={{ width: 36 }} />
                  <col />
                  <col style={{ width: 64 }} />
                </colgroup>
                <TableBody>
                  {result.result.classification.map((entry, index) => (
                    <TableRow
                      key={entry.driverName}
                      sx={{
                        bgcolor:
                          entry.driverName ===
                          prediction?.tenthPlacePickedDriver
                            ? "rgba(144,202,249,0.08)"
                            : prediction?.dnfPickedDrivers?.includes(
                                  entry.driverName,
                                )
                              ? "rgba(255,167,38,0.05)"
                              : "transparent",
                      }}
                    >
                      <TableCell>
                        <Typography
                          variant="body2"
                          sx={{ fontVariantNumeric: "tabular-nums" }}
                          color={
                            index === 9 ? "primary.main" : "text.secondary"
                          }
                        >
                          {index + 1}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Box
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            gap: 1,
                            minWidth: 0,
                          }}
                        >
                          <F1TeamBadge
                            driver={entry.driverName}
                            teams={teams}
                          />
                          <Typography
                            variant="body2"
                            noWrap
                            title={entry.driverName}
                            color={
                              entry.driverName === positionDriver
                                ? "primary.main"
                                : "text.primary"
                            }
                            fontWeight={
                              entry.driverName === positionDriver ? 600 : 400
                            }
                          >
                            {entry.driverName}
                          </Typography>
                        </Box>
                      </TableCell>
                      <TableCell align="right">
                        {entry.status !== "FINISHED" && (
                          <Chip
                            size="small"
                            variant="outlined"
                            label={entry.status}
                            color={
                              entry.status === "DNF"
                                ? "warning"
                                : entry.status === "DQ"
                                  ? "error"
                                  : "default"
                            }
                            sx={{
                              height: 20,
                              minWidth: 40,
                              fontSize: "0.65rem",
                              "& .MuiChip-label": { px: 0.75 },
                            }}
                          />
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
            <Stack
              spacing={0.5}
              sx={{ p: 2, borderTop: "1px solid rgba(255,255,255,0.08)" }}
            >
              {result.result.safetyCars !== null && (
                <Typography variant="body2">
                  Инциденты: {result.result.safetyCars}
                </Typography>
              )}
              {result.result.firstPlaceLead !== null && (
                <Typography variant="body2">
                  Отрыв: {result.result.firstPlaceLead} сек
                </Typography>
              )}
            </Stack>
          </>
        )}
      </Paper>
      {result && (
        <Paper variant="outlined" sx={{ ...PANEL_SX, p: 2 }}>
          <Stack spacing={1}>
            <Stack
              direction="row"
              justifyContent="space-between"
              alignItems="center"
              gap={1}
            >
              <Typography fontWeight={600}>Ваши очки</Typography>
              {points && (
                <Typography variant="h6" fontWeight={700}>
                  {points.totalPoints}
                </Typography>
              )}
            </Stack>
            {points ? (
              categories.map((category) => (
                <Stack
                  key={category.label}
                  direction="row"
                  justifyContent="space-between"
                  gap={1}
                >
                  <Typography variant="body2" color="text.secondary">
                    {category.label}
                  </Typography>
                  <Typography variant="body2" fontWeight={600}>
                    {category.points}
                  </Typography>
                </Stack>
              ))
            ) : (
              <Typography variant="body2" color="text.secondary">
                {prediction
                  ? "Очки ещё не рассчитаны."
                  : "Вы не оставили предсказание на эту гонку."}
              </Typography>
            )}
          </Stack>
        </Paper>
      )}
    </Stack>
  );
}
