import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import React, { useCallback, useEffect, useState } from "react";
import {
  Button,
  FormControl,
  Grid,
  InputAdornment,
  OutlinedInput,
  Stack,
  Typography,
} from "@mui/material";
import F1RaceClassifications from "./F1RaceClassifications";
import { LoadingButton } from "@mui/lab";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { getDriversFromTeams } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import { Block, Done, Save } from "@mui/icons-material";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionAdmin({ f1Race }: Props) {
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto>(f1Race);

  const [teams, setTeams] = useState<F1TeamDto[]>([]);
  const [drivers, setDrivers] = useState<string[]>([]);
  const [qualifyingGrid, setQualifyingGrid] = useState<string[]>([]);
  const [dnfDrivers, setDnfDrivers] = useState<Set<string>>(new Set());
  const [incidents, setIncidents] = useState<number>(0);
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>();
  const [isClosing, setIsClosing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isSavingQualifying, setIsSavingQualifying] = useState(false);
  const [isFinishing, setIsFinishing] = useState(false);
  const [isPredictionsClosed, setIsPredictionsClosed] = useState(false);

  useEffect(() => {
    async function load() {
      const [result, teams] = await Promise.all([
        F1PredictionsApi.read(f1Race.id),
        F1PredictionsApi.getActiveTeams(),
      ]);
      setCurrentF1Race(result);
      setIsPredictionsClosed(!result.isOpened);
      setTeams(teams);

      setDrivers(
        result.result?.classification.length === 0
          ? getDriversFromTeams(teams)
          : result.result.classification,
      );
      setQualifyingGrid(
        result.qualifyingGrid && result.qualifyingGrid.length > 0
          ? result.qualifyingGrid
          : getDriversFromTeams(teams),
      );
      setDnfDrivers(new Set(result.result?.dnfDrivers ?? []));
      setIncidents(result.result?.safetyCars ?? 0);
      setFirstPlaceLead(String(result.result?.firstPlaceLead ?? ""));
    }

    load().catch(console.error);
  }, [f1Race.id]);

  const saveQualifyingResults = useCallback(async () => {
    setIsSavingQualifying(true);
    await F1PredictionsApi.saveQualifyingGrid(currentF1Race.id, qualifyingGrid);
    setIsSavingQualifying(false);
  }, [currentF1Race.id, qualifyingGrid]);

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

  const closePredictions = useCallback(async () => {
    setIsClosing(true);
    await F1PredictionsApi.close(currentF1Race.id);
    setIsPredictionsClosed(true);
    setIsClosing(false);
  }, [currentF1Race.id]);

  const finishRace = useCallback(async () => {
    setIsFinishing(true);
    await F1PredictionsApi.finish(currentF1Race.id);
    setIsFinishing(false);
  }, [currentF1Race.id]);

  return (
    <Stack direction="row" width={"100%"}>
      <Grid
        container
        spacing={1}
        sx={{ width: "100%", height: "100%", margin: "auto" }}
      >
        <Grid
          item
          key={`F1PredictionsAdminColumnQualifying`}
          xs={12}
          sm={12}
          md={12}
          lg={4}
          sx={{ display: "flex", justifyContent: "top", alignItems: "top" }}
        >
          <Stack direction={"column"} spacing={1} width={"100%"}>
            <Typography variant="h6" align="center">Квалификация</Typography>
            <F1RaceClassifications
              drivers={qualifyingGrid}
              setDrivers={setQualifyingGrid}
              dnfDrivers={new Set()}
              setDnfDrivers={() => {}}
              teams={teams}
              showDnf={false}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key={`F1PredictionsAdminColumnRaceResults`}
          xs={12}
          sm={12}
          md={12}
          lg={4}
          sx={{ display: "flex", justifyContent: "top", alignItems: "top" }}
        >
          <Stack direction={"column"} spacing={1} width={"100%"}>
            <Typography variant="h6" align="center">Гонка</Typography>
            <F1RaceClassifications
              drivers={drivers}
              setDrivers={setDrivers}
              dnfDrivers={dnfDrivers}
              setDnfDrivers={setDnfDrivers}
              teams={teams}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key={`F1PredictionsAdminColumnButtons`}
          xs={12}
          sm={12}
          md={12}
          lg={4}
          sx={{ display: "flex", justifyContent: "top", alignItems: "top" }}
        >
          <Stack direction={"column"} spacing={1} width={"100%"}>
            <Stack direction={"row"} spacing={4} height={"40px"}>
              <Button
                variant="contained"
                color="error"
                disabled={incidents === 0}
                onClick={() => setIncidents(incidents - 1)}
              >
                <Typography variant="button">-</Typography>
              </Button>
              <Typography variant="h6">
                Инциденты (VSC, SC, Red Flag): {incidents}
              </Typography>
              <Button
                variant="contained"
                color="success"
                onClick={() => setIncidents(incidents + 1)}
              >
                <Typography variant="button">+</Typography>
              </Button>
            </Stack>
            <Stack
              spacing={4}
              direction="row"
              alignItems={"flex-start"}
              justifyContent={"space-between"}
            >
              <Typography variant="h6">Отрыв 1 места</Typography>
              <FormControl size="small">
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
              disabled={isClosing || isPredictionsClosed}
              color="error"
              size="large"
              variant="contained"
              startIcon={<Block />}
              onClick={closePredictions}
            >
              {isPredictionsClosed
                ? "Предсказания закрыты"
                : "Закрыть предсказания"}
            </LoadingButton>
            <Stack direction="row" spacing={1}>
              <LoadingButton
                loading={isSavingQualifying}
                disabled={isSavingQualifying}
                color="success"
                size="large"
                variant="contained"
                startIcon={<Save />}
                onClick={saveQualifyingResults}
                sx={{ flex: 1 }}
              >
                Сохранить квалификацию
              </LoadingButton>
              <LoadingButton
                loading={isSaving}
                disabled={isSaving}
                color="success"
                size="large"
                variant="contained"
                startIcon={<Save />}
                onClick={saveRaceResults}
                sx={{ flex: 1 }}
              >
                Сохранить гонку
              </LoadingButton>
            </Stack>
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
        </Grid>
      </Grid>
    </Stack>
  );
}
