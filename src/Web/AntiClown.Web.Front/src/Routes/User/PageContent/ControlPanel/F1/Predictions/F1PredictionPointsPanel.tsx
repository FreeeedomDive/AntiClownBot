import { Paper, Stack, Typography } from "@mui/material";
import {
  InternalPredictionDto,
  InternalRaceResultDto,
} from "../../../../../../Dto/F1Predictions/F1InternalDto";

interface Props {
  result: InternalRaceResultDto | null;
  prediction: InternalPredictionDto | null;
}

export default function F1PredictionPointsPanel({ result, prediction }: Props) {
  if (!result) return null;

  const points = result.points;
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
    <Paper
      variant="outlined"
      sx={{
        borderRadius: 2,
        bgcolor: "rgba(255,255,255,0.025)",
        backgroundImage: "none",
        minWidth: 0,
        p: 2,
      }}
    >
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
  );
}
