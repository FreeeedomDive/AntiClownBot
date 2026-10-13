import React, { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { RightsWrapper } from "../../../../../../Components/UserRights/RightsWrapper";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
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
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1PredictionAdmin from "./F1PredictionAdmin";

interface RacesData {
  season: number;
  onlyActive: boolean;
  races: F1RaceDto[];
  error?: string;
}

export default function F1PredictionsAdminList() {
  const [searchParams, setSearchParams] = useSearchParams();
  const currentYear = new Date().getFullYear();
  const requestedSeason = Number(searchParams.get("season") ?? currentYear);
  const season =
    Number.isInteger(requestedSeason) &&
    requestedSeason >= 2023 &&
    requestedSeason <= currentYear
      ? requestedSeason
      : currentYear;
  const onlyActive =
    searchParams.get("active") === "true" ||
    (searchParams.get("active") === null && season === currentYear);
  const [data, setData] = useState<RacesData>();
  const [selectedRaceId, setSelectedRaceId] = useState("");

  useEffect(() => {
    let cancelled = false;
    F1PredictionsApi.find({ season, isActive: onlyActive ? true : undefined })
      .then((races) => {
        if (!cancelled) {
          setData({ season, onlyActive, races });
          setSelectedRaceId(
            onlyActive ? races[0]?.id ?? "" : races.at(-1)?.id ?? "",
          );
        }
      })
      .catch(() => {
        if (!cancelled) {
          setData({
            season,
            onlyActive,
            races: [],
            error: "Не удалось загрузить список гонок",
          });
        }
      });
    return () => {
      cancelled = true;
    };
  }, [season, onlyActive]);

  const isLoading =
    !data || data.season !== season || data.onlyActive !== onlyActive;
  const currentRace = !isLoading
    ? data?.races.find((race) => race.id === selectedRaceId)
    : undefined;

  return (
    <RightsWrapper requiredRights={[RightsDto.F1PredictionsAdmin]}>
      <Stack spacing={2}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          spacing={2}
          alignItems={{ sm: "center" }}
        >
          <FormControl
            size="small"
            sx={{ width: { xs: "100%", sm: 360 }, maxWidth: "100%" }}
            disabled={isLoading || !data?.races.length}
          >
            <InputLabel id="admin-race-label">Гонка</InputLabel>
            <Select
              labelId="admin-race-label"
              label="Гонка"
              value={currentRace?.id ?? ""}
              onChange={(event) => setSelectedRaceId(event.target.value)}
            >
              {!isLoading &&
                data?.races.map((race) => (
                  <MenuItem key={race.id} value={race.id}>
                    {race.name}
                    {race.isSprint ? " · Спринт" : ""} · {race.season}
                  </MenuItem>
                ))}
            </Select>
          </FormControl>
          <FormControlLabel
            control={
              <Checkbox
                size="small"
                checked={onlyActive}
                onChange={(_, checked) => {
                  const next = new URLSearchParams(searchParams);
                  next.set("active", String(checked));
                  setSearchParams(next, { replace: true });
                }}
              />
            }
            label="Только текущие гонки"
          />
        </Stack>
        {isLoading ? (
          <Loader />
        ) : data?.error ? (
          <Alert severity="error">{data.error}</Alert>
        ) : currentRace ? (
          <F1PredictionAdmin key={currentRace.id} f1Race={currentRace} />
        ) : (
          <Alert severity="info">
            {onlyActive
              ? `В сезоне ${season} нет текущих гонок. Отключите фильтр, чтобы посмотреть завершённые.`
              : `В сезоне ${season} пока нет гонок.`}
          </Alert>
        )}
      </Stack>
    </RightsWrapper>
  );
}
