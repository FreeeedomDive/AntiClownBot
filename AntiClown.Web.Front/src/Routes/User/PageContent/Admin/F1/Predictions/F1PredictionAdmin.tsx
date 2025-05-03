import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import React, { useCallback, useEffect, useState } from "react";
import {
  Button,
  FormControl,
  InputAdornment,
  OutlinedInput,
  Stack,
  Typography,
} from "@mui/material";
import F1RaceClassifications from "./F1RaceClassifications";
import { LoadingButton } from "@mui/lab";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { getDriversFromTeams } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import {Block, Done, Download, Save} from "@mui/icons-material";
import F1FastApi from "../../../../../../Api/F1FastApi";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionAdmin({ f1Race }: Props) {
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto>(f1Race);

  const [drivers, setDrivers] = useState<string[]>([]);
  const [dnfDrivers, setDnfDrivers] = useState<Set<string>>(new Set());
  const [incidents, setIncidents] = useState<number>(0);
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>();
  const [isClosing, setIsClosing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isLoadingRaceResults, setIsLoadingRaceResults] = useState(false);
  const [isFinishing, setIsFinishing] = useState(false);

  useEffect(() => {
    async function load() {
      const result = await F1PredictionsApi.read(f1Race.id);
      setCurrentF1Race(result);

      const teams = await F1PredictionsApi.getActiveTeams();

      setDrivers(
        result.result?.classification.length === 0
          ? getDriversFromTeams(teams)
          : result.result.classification,
      );
      setDnfDrivers(new Set(result.result?.dnfDrivers ?? []));
      setIncidents(result.result?.safetyCars ?? 0);
      setFirstPlaceLead(String(result.result?.firstPlaceLead ?? ""));
    }

    load();
  }, [f1Race.id]);

  const saveRaceResults = useCallback(async () => {
    setIsSaving(true);
    await F1PredictionsApi.addResult(currentF1Race.id, {
      raceId: currentF1Race.id,
      firstPlaceLead: Number(firstPlaceLead),
      safetyCars: incidents,
      dnfDrivers: Array.from(dnfDrivers.values()),
      classification: drivers,
    });
    setIsSaving(false);
  }, [currentF1Race.id, dnfDrivers, drivers, firstPlaceLead, incidents]);

  const loadRaceResults = useCallback(async () => {
    setIsLoadingRaceResults(true);
    const raceResult = await F1FastApi.getRaceResult(currentF1Race.id);
    if (raceResult.success && !!raceResult.result) {
      setDrivers(raceResult.result.classification,);
      setDnfDrivers(new Set(raceResult.result.dnfDrivers));
      setIncidents(raceResult.result.safetyCars);
      setFirstPlaceLead(String(raceResult.result.firstPlaceLead));
    }
    setIsLoadingRaceResults(false);
  }, [currentF1Race.id]);

  const closePredictions = useCallback(async () => {
    setIsClosing(true);
    await F1PredictionsApi.close(currentF1Race.id);
    setIsClosing(false);
  }, [currentF1Race.id]);

  const finishRace = useCallback(async () => {
    setIsFinishing(true);
    await F1PredictionsApi.finish(currentF1Race.id);
    setIsFinishing(false);
  }, [currentF1Race.id]);

  return (
    <Stack direction={"row"} spacing={16}>
      <F1RaceClassifications
        drivers={drivers}
        setDrivers={setDrivers}
        dnfDrivers={dnfDrivers}
        setDnfDrivers={setDnfDrivers}
      />
      <Stack direction={"column"} spacing={2}>
        <Stack direction={"row"} spacing={4} height={"45px"}>
          <Button
            variant="contained"
            color="error"
            disabled={incidents === 0}
            onClick={() => setIncidents(incidents - 1)}
          >
            <Typography variant="h4">-</Typography>
          </Button>
          <Typography variant="h6">
            Инциденты (VSC, SC, Red Flag): {incidents}
          </Typography>
          <Button
            variant="contained"
            color="success"
            onClick={() => setIncidents(incidents + 1)}
          >
            <Typography variant="h4">+</Typography>
          </Button>
        </Stack>
        <Stack
          spacing={4}
          direction="row"
          alignItems={"flex-start"}
          justifyContent={"space-between"}
        >
          <Typography variant="h6">Отрыв 1 места</Typography>
          <FormControl>
            <OutlinedInput
              id="outlined-adornment-weight"
              endAdornment={
                <InputAdornment position="end">в секундах</InputAdornment>
              }
              placeholder="5.169"
              aria-describedby="outlined-weight-helper-text"
              inputProps={{
                "aria-label": "weight",
              }}
              value={firstPlaceLead}
              onChange={(event) => {
                const fixedValue = event.target.value
                  .replace(/[^\d,.]/g, "")
                  .replace(",", ".");

                const numericValue = Number(fixedValue);

                if (numericValue >= 0) {
                  setFirstPlaceLead(fixedValue);
                }
              }}
            />
          </FormControl>
        </Stack>
        <LoadingButton
          loading={isClosing}
          disabled={isClosing}
          color="error"
          size="large"
          variant="contained"
          startIcon={<Block />}
          onClick={closePredictions}
        >
          Закрыть предсказания
        </LoadingButton>
        <LoadingButton
          loading={isLoadingRaceResults}
          disabled={isLoadingRaceResults}
          color="success"
          size="large"
          variant="contained"
          startIcon={<Download />}
          onClick={loadRaceResults}
        >
          Загрузить результаты гонки
        </LoadingButton>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="success"
          size="large"
          variant="contained"
          startIcon={<Save />}
          onClick={saveRaceResults}
        >
          Сохранить
        </LoadingButton>
        <LoadingButton
          loading={isFinishing}
          disabled={isFinishing}
          color="primary"
          size="large"
          variant="contained"
          startIcon={<Done />}
          onClick={finishRace}
        >
          Завершить гонку и рассчитать результаты
        </LoadingButton>
      </Stack>
    </Stack>
  );
}
