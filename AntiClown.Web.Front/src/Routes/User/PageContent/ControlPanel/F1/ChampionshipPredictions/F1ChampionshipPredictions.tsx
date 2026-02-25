import React, { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Alert, Fab, Grid, Snackbar, Stack, Typography } from "@mui/material";
import { Save } from "@mui/icons-material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1ChampionshipPredictionsApi from "../../../../../../Api/F1ChampionshipPredictionsApi";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { F1ChampionshipResultsDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipResultsDto";
import { F1ChampionshipPredictionTypeDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipPredictionTypeDto";
import { AddPredictionResultDto } from "../../../../../../Dto/F1Predictions/AddPredictionResultDto";
import { getDriversFromTeams } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import F1ChampionshipDriverDnDList from "./F1ChampionshipDriverDnDList";
import F1ChampionshipCurrentStandings from "./F1ChampionshipCurrentStandings";

const fabStyle = {
  position: "absolute",
  bottom: 16,
  right: 16,
};

const SEASON = new Date().getFullYear();

export default function F1ChampionshipPredictions() {
  const { userId } = useParams<"userId">();
  const [results, setResults] = useState<F1ChampionshipResultsDto | undefined>(
    undefined,
  );
  const [preSeasonDrivers, setPreSeasonDrivers] = useState<string[]>([]);
  const [midSeasonDrivers, setMidSeasonDrivers] = useState<string[]>([]);
  const [isSaving, setIsSaving] = useState(false);
  const [saveResult, setSaveResult] = useState<AddPredictionResultDto | null>(
    null,
  );

  useEffect(() => {
    if (!userId) {
      return;
    }

    async function load() {
      const [resultsData, predictionData, teams] = await Promise.all([
        F1ChampionshipPredictionsApi.readResults(SEASON),
        F1ChampionshipPredictionsApi.read(userId!, SEASON),
        F1PredictionsApi.getActiveTeams(),
      ]);

      setResults(resultsData);

      const defaultDrivers = getDriversFromTeams(teams);
      setPreSeasonDrivers(predictionData.preSeasonStandings?.length ? predictionData.preSeasonStandings : defaultDrivers);
      setMidSeasonDrivers(predictionData.midSeasonStandings?.length ? predictionData.midSeasonStandings : defaultDrivers);
    }

    load().catch(console.error);
  }, [userId]);

  const save = useCallback(async () => {
    if (!userId) {
      return;
    }

    setIsSaving(true);
    const result = await F1ChampionshipPredictionsApi.createOrUpdate({
      season: SEASON,
      userId,
      preSeasonStandings: preSeasonDrivers,
      midSeasonStandings: midSeasonDrivers,
    });
    setSaveResult(result);
    setIsSaving(false);
  }, [userId, preSeasonDrivers, midSeasonDrivers]);

  if (results === undefined) {
    return <Loader />;
  }

  if (!results.hasData) {
    return (
      <Typography variant="h5">Сезон {SEASON} еще не стартовал</Typography>
    );
  }

  const preSeasonEnabled =
    results.isOpen && results.stage === F1ChampionshipPredictionTypeDto.PreSeason;
  const midSeasonEnabled =
    results.isOpen && results.stage === F1ChampionshipPredictionTypeDto.MidSeason;

  return (
    <Stack direction="row" width="100%">
      <Grid
        container
        spacing={1}
        sx={{ width: "100%", height: "100%", margin: "auto" }}
      >
        <Grid
          item
          key="championship-pre-season"
          xs={12}
          sm={12}
          md={12}
          lg={4}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipDriverDnDList
              title="Предсезонные предсказания"
              droppableId="championship-pre-season-dnd"
              drivers={preSeasonDrivers}
              setDrivers={setPreSeasonDrivers}
              disabled={!preSeasonEnabled}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key="championship-mid-season"
          xs={12}
          sm={12}
          md={12}
          lg={4}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipDriverDnDList
              title="Предсказания посреди сезона"
              droppableId="championship-mid-season-dnd"
              drivers={midSeasonDrivers}
              setDrivers={setMidSeasonDrivers}
              disabled={!midSeasonEnabled}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key="championship-current-standings"
          xs={12}
          sm={12}
          md={12}
          lg={4}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipCurrentStandings
              title="Текущий чемпионат"
              standings={results.standings}
            />
          </Stack>
        </Grid>
      </Grid>

      <Snackbar
        open={saveResult !== null}
        autoHideDuration={3000}
        onClose={() => setSaveResult(null)}
      >
        <Alert
          severity={
            saveResult === AddPredictionResultDto.Success ? "success" : "warning"
          }
        >
          {saveResult === AddPredictionResultDto.Success
            ? "Предсказания успешно сохранены"
            : "Предсказания на этот сезон уже закрыты"}
        </Alert>
      </Snackbar>

      <Fab
        disabled={isSaving}
        variant="extended"
        sx={fabStyle}
        onClick={save}
        color="success"
      >
        <Save />
        {isSaving ? <>Сохранение...</> : <>Сохранить</>}
      </Fab>
    </Stack>
  );
}
