import React, { useEffect, useState } from "react";
import {
  Alert,
  Checkbox,
  FormControl,
  FormControlLabel,
  InputLabel,
  MenuItem,
  Select,
  Stack,
} from "@mui/material";
import F1PredictionsApi from "../../Api/F1PredictionsApi";
import { Loader } from "../../Components/Loader/Loader";
import { RightsWrapper } from "../../Components/UserRights/RightsWrapper";
import { F1RaceDto } from "../../Dto/F1Predictions/F1RaceDto";
import { RightsDto } from "../../Dto/Rights/RightsDto";
import F1Prediction from "./F1Prediction";

export default function F1PredictionsList() {
  const [onlyActive, setOnlyActive] = useState(true);
  const [selectedRaceId, setSelectedRaceId] = useState("");
  const [data, setData] = useState<{
    onlyActive: boolean;
    races: F1RaceDto[];
    error?: boolean;
  }>();

  useEffect(() => {
    let cancelled = false;
    F1PredictionsApi.find({
      season: new Date().getFullYear(),
      isActive: onlyActive ? true : undefined,
    })
      .then((races) => {
        if (!cancelled) {
          setData({ onlyActive, races });
          setSelectedRaceId(
            onlyActive ? races[0]?.id ?? "" : races.at(-1)?.id ?? "",
          );
        }
      })
      .catch(() => {
        if (!cancelled) setData({ onlyActive, races: [], error: true });
      });
    return () => {
      cancelled = true;
    };
  }, [onlyActive]);

  const isLoading = !data || data.onlyActive !== onlyActive;
  const currentRace = !isLoading
    ? data?.races.find((race) => race.id === selectedRaceId)
    : undefined;

  return (
    <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
      <Stack spacing={2} sx={{ mx: 2, my: 1, minWidth: 0 }}>
        <Stack spacing={0.5}>
          <FormControl
            size="small"
            fullWidth
            disabled={isLoading || !data?.races.length}
          >
            <InputLabel id="mobile-prediction-race-label">Гонка</InputLabel>
            <Select
              labelId="mobile-prediction-race-label"
              label="Гонка"
              value={currentRace?.id ?? ""}
              onChange={(event) => setSelectedRaceId(event.target.value)}
            >
              {!isLoading &&
                data?.races.map((race) => (
                  <MenuItem key={race.id} value={race.id}>
                    {race.name} · {race.season}
                    {race.isSprint ? " · Спринт" : ""}
                  </MenuItem>
                ))}
            </Select>
          </FormControl>
          <FormControlLabel
            control={
              <Checkbox
                size="small"
                checked={onlyActive}
                onChange={(_, checked) => setOnlyActive(checked)}
              />
            }
            label="Только текущие гонки"
          />
        </Stack>
        {isLoading ? (
          <Loader />
        ) : data?.error ? (
          <Alert severity="error">Не удалось загрузить список гонок</Alert>
        ) : currentRace ? (
          <F1Prediction key={currentRace.id} f1Race={currentRace} />
        ) : (
          <Alert severity="info">
            {onlyActive
              ? "Нет текущих гонок. Отключите фильтр, чтобы посмотреть завершённые."
              : "В этом сезоне пока нет гонок."}
          </Alert>
        )}
      </Stack>
    </RightsWrapper>
  );
}
