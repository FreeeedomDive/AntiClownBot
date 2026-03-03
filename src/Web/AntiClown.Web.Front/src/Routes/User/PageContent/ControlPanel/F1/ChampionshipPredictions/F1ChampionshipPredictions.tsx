import React, { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Alert, Fab, Grid, Snackbar, Stack, Typography } from "@mui/material";
import { Save } from "@mui/icons-material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1ChampionshipPredictionsApi from "../../../../../../Api/F1ChampionshipPredictionsApi";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import { F1ChampionshipResultsDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipResultsDto";
import { F1ChampionshipPredictionTypeDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipPredictionTypeDto";
import { F1ChampionshipUserPointsDto } from "../../../../../../Dto/F1Predictions/F1ChampionshipUserPointsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { AddPredictionResultDto } from "../../../../../../Dto/F1Predictions/AddPredictionResultDto";
import { getDriversFromTeams } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import F1ChampionshipDriverDnDList from "./F1ChampionshipDriverDnDList";
import F1ChampionshipCurrentStandings from "./F1ChampionshipCurrentStandings";
import F1ChampionshipUserStandings from "./F1ChampionshipUserStandings";

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
  const [userPoints, setUserPoints] = useState<
    F1ChampionshipUserPointsDto | undefined
  >(undefined);
  const [allPoints, setAllPoints] = useState<F1ChampionshipUserPointsDto[]>([]);
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);
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
      const [resultsData, predictionData, teams, pointsData] =
        await Promise.all([
          F1ChampionshipPredictionsApi.readResults(SEASON),
          F1ChampionshipPredictionsApi.read(userId!, SEASON),
          F1PredictionsApi.getActiveTeams(),
          F1ChampionshipPredictionsApi.buildPoints(SEASON),
        ]);

      const membersData = await DiscordMembersApi.getMembers(
        pointsData.map((p) => p.userId),
      );

      setResults(resultsData);
      setAllPoints(pointsData);
      setUserPoints(pointsData.find((p) => p.userId === userId));
      setMembers(membersData);

      const defaultDrivers = getDriversFromTeams(teams);
      setPreSeasonDrivers(
        predictionData.preSeasonStandings?.length
          ? predictionData.preSeasonStandings
          : defaultDrivers,
      );
      setMidSeasonDrivers(
        predictionData.midSeasonStandings?.length
          ? predictionData.midSeasonStandings
          : defaultDrivers,
      );
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
    results.isOpen &&
    results.stage === F1ChampionshipPredictionTypeDto.PreSeason;
  const midSeasonEnabled =
    results.isOpen &&
    results.stage === F1ChampionshipPredictionTypeDto.MidSeason;

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
          md={6}
          lg={3}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipDriverDnDList
              title="Предсезонные предсказания"
              description="Активны до старта первой гонки сезона 8 марта"
              droppableId="championship-pre-season-dnd"
              drivers={preSeasonDrivers}
              setDrivers={setPreSeasonDrivers}
              disabled={false}
              editable={preSeasonEnabled}
              points={userPoints?.preSeasonPoints}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key="championship-mid-season"
          xs={12}
          sm={12}
          md={6}
          lg={3}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipDriverDnDList
              title="Предсказания посреди сезона"
              description="Активны в летние каникулы с 27 июля по 23 августа"
              droppableId="championship-mid-season-dnd"
              drivers={midSeasonDrivers}
              setDrivers={setMidSeasonDrivers}
              disabled={
                results.stage === F1ChampionshipPredictionTypeDto.PreSeason
              }
              editable={midSeasonEnabled}
              points={userPoints?.midSeasonPoints}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key="championship-current-standings"
          xs={12}
          sm={12}
          md={6}
          lg={3}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipCurrentStandings
              title="Текущий чемпионат"
              standings={results.standings}
            />
          </Stack>
        </Grid>
        <Grid
          item
          key="championship-user-standings"
          xs={12}
          sm={12}
          md={6}
          lg={3}
          sx={{ display: "flex", flexDirection: "column" }}
        >
          <Stack direction="column" spacing={1} width="100%" height="100%">
            <F1ChampionshipUserStandings
              allPoints={allPoints}
              members={members}
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
            saveResult === AddPredictionResultDto.Success
              ? "success"
              : "warning"
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
