import React, { useEffect, useState } from "react";
import { useParams, useSearchParams } from "react-router-dom";
import axios from "axios";
import {
  Alert,
  CircularProgress,
  Fab,
  Grid,
  Snackbar,
  Stack,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { Save } from "@mui/icons-material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1PredictionsApi, {
  getF1ErrorMessage,
} from "../../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import {
  F1ChampionshipDetailsDto,
  F1ChampionshipPredictionDto,
  F1ChampionshipStage,
} from "../../../../../../Dto/F1Predictions/F1ChampionshipDetailsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import F1ChampionshipDriverDnDList from "./F1ChampionshipDriverDnDList";
import F1ChampionshipCurrentStandings from "./F1ChampionshipCurrentStandings";
import F1ChampionshipUserStandings from "./F1ChampionshipUserStandings";

const fabStyle = {
  position: "fixed" as const,
  bottom: { xs: "calc(16px + env(safe-area-inset-bottom))", md: 24 },
  right: { xs: 16, md: 24 },
  zIndex: 1000,
};

function editableOrder(
  prediction: F1ChampionshipPredictionDto | null,
  roster: string[],
) {
  const order = prediction?.driverStandings;
  if (
    order?.length === roster.length &&
    roster.every((driver) => order.includes(driver))
  ) {
    return order;
  }
  return roster;
}

export default function F1ChampionshipPredictions() {
  const { userId } = useParams<"userId">();
  const [searchParams] = useSearchParams();
  const currentYear = new Date().getFullYear();
  const requestedSeason = Number(searchParams.get("season") ?? currentYear);
  const season =
    Number.isInteger(requestedSeason) &&
    requestedSeason >= 2023 &&
    requestedSeason <= currentYear
      ? requestedSeason
      : currentYear;

  return (
    <ChampionshipPredictions key={`${userId}:${season}`} season={season} />
  );
}

function ChampionshipPredictions({ season }: { season: number }) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const [details, setDetails] = useState<F1ChampionshipDetailsDto>();
  const [loadError, setLoadError] = useState<string>();
  const [revision, setRevision] = useState(0);
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);
  const [draft, setDraft] = useState<{
    stage: F1ChampionshipStage;
    drivers: string[];
  } | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [notice, setNotice] = useState<{
    message: string;
    error?: boolean;
  } | null>(null);

  useEffect(() => {
    const controller = new AbortController();
    F1PredictionsApi.getChampionship(season, controller.signal)
      .then((data) => {
        if (controller.signal.aborted) return;
        setDetails(data);
        setLoadError(undefined);
        if (data.rows.length === 0) return;
        DiscordMembersApi.getMembers(data.rows.map((row) => row.participantId))
          .then((members) => {
            if (!controller.signal.aborted && Array.isArray(members))
              setMembers(members);
          })
          .catch(console.error);
      })
      .catch((error) => {
        if (!controller.signal.aborted) setLoadError(getF1ErrorMessage(error));
      });
    return () => controller.abort();
  }, [season, revision]);

  if (!details) {
    return loadError ? <Alert severity="error">{loadError}</Alert> : <Loader />;
  }

  if (!details.configured) {
    return <Typography variant="h5">Сезон {season} еще не настроен</Typography>;
  }

  const activeStage = details.predictionsOpen ? details.currentStage : null;
  function stageOrder(
    stage: F1ChampionshipStage,
    prediction: F1ChampionshipPredictionDto | null,
    roster: string[],
  ) {
    if (stage !== activeStage) return prediction?.driverStandings ?? [];
    return draft?.stage === stage
      ? draft.drivers
      : editableOrder(prediction, roster);
  }
  const preSeasonDrivers = stageOrder(
    "PRE_SEASON",
    details.preSeasonPrediction,
    details.preSeasonRoster,
  );
  const midSeasonDrivers = stageOrder(
    "MID_SEASON",
    details.midSeasonPrediction,
    details.midSeasonRoster,
  );

  async function save() {
    if (!activeStage) return;
    const drivers =
      activeStage === "PRE_SEASON" ? preSeasonDrivers : midSeasonDrivers;
    setIsSaving(true);
    try {
      const saved = await F1PredictionsApi.saveChampionshipPrediction(
        season,
        activeStage,
        drivers,
      );
      setDetails(
        (previous) =>
          previous &&
          (activeStage === "PRE_SEASON"
            ? { ...previous, preSeasonPrediction: saved }
            : { ...previous, midSeasonPrediction: saved }),
      );
      setDraft(null);
      setNotice({ message: "Предсказание сохранено" });
      setRevision((value) => value + 1);
    } catch (error) {
      const closed =
        axios.isAxiosError(error) && error.response?.status === 409;
      setNotice({
        message: closed
          ? "Приём предсказаний на этот этап закрыт"
          : getF1ErrorMessage(error),
        error: true,
      });
      if (closed) {
        setDraft(null);
        setRevision((value) => value + 1);
      }
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <Stack width="100%" sx={{ pb: 10 }}>
      {loadError && <Alert severity="warning">{loadError}</Alert>}
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
              description={
                activeStage === "PRE_SEASON"
                  ? "Приём открыт"
                  : details.stageStarted
                    ? "Приём закрыт"
                    : "Приём ещё не открыт"
              }
              droppableId="championship-pre-season-dnd"
              drivers={preSeasonDrivers}
              setDrivers={(drivers) =>
                setDraft({ stage: "PRE_SEASON", drivers })
              }
              disabled={false}
              editable={activeStage === "PRE_SEASON" && !isSaving}
              teams={details.teams}
              points={details.preSeasonPrediction?.points}
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
              description={
                activeStage === "MID_SEASON"
                  ? "Приём открыт"
                  : details.currentStage === "MID_SEASON"
                    ? "Приём закрыт"
                    : "Приём ещё не открыт"
              }
              droppableId="championship-mid-season-dnd"
              drivers={midSeasonDrivers}
              setDrivers={(drivers) =>
                setDraft({ stage: "MID_SEASON", drivers })
              }
              disabled={
                details.currentStage !== "MID_SEASON" &&
                !details.midSeasonPrediction
              }
              editable={activeStage === "MID_SEASON" && !isSaving}
              teams={details.teams}
              points={details.midSeasonPrediction?.points}
            />
            {details.midSeasonPrediction?.predictionSource ===
              "PRE_SEASON_COPY" && (
              <Typography variant="body2" color="text.secondary">
                Скопировано из предсезонного прогноза
              </Typography>
            )}
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
              standings={details.driverStandings}
              teams={details.teams}
              currentDrivers={details.currentDrivers}
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
              allPoints={details.rows}
              members={members}
            />
          </Stack>
        </Grid>
      </Grid>

      <Snackbar
        open={notice !== null}
        autoHideDuration={3000}
        onClose={() => setNotice(null)}
      >
        <Alert severity={notice?.error ? "error" : "success"}>
          {notice?.message}
        </Alert>
      </Snackbar>

      {activeStage && (
        <Fab
          disabled={isSaving}
          variant={isMobile ? "circular" : "extended"}
          sx={fabStyle}
          onClick={save}
          color="success"
          aria-label={isSaving ? "Сохранение..." : "Сохранить предсказания"}
        >
          {isSaving ? (
            <CircularProgress
              size={24}
              color="inherit"
              sx={{ mr: isMobile ? 0 : 1 }}
            />
          ) : (
            <Save />
          )}
          {!isMobile && (isSaving ? "Сохранение..." : "Сохранить")}
        </Fab>
      )}
    </Stack>
  );
}
