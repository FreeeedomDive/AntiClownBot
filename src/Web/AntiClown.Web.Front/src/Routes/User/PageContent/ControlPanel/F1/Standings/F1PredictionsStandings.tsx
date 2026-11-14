import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import F1PredictionsApi, {
  getF1ErrorMessage,
} from "../../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { Alert, Stack, Typography } from "@mui/material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1PredictionsStandingsChart from "./F1PredictionsStandingsChart";
import F1PredictionsStandingsTable from "./F1PredictionsStandingsTable";
import {
  F1RaceSummary as F1RaceDto,
  F1StandingsView as F1StandingsDto,
} from "../../../../../../Dto/F1Predictions/F1InternalDto";
import { F1ChartsDto } from "../../../../../../Dto/F1Predictions/F1ChartsDto";

export default function F1PredictionsStandings() {
  const [searchParams] = useSearchParams();
  const currentYear = new Date().getFullYear();
  const season = Number(searchParams.get("season") ?? currentYear);

  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>();
  const [finishedRaces, setFinishedRaces] = useState<F1RaceDto[]>([]);
  const [standings, setStandings] = useState<F1StandingsDto>();
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);
  const [charts, setCharts] = useState<F1ChartsDto>();

  useEffect(() => {
    async function load() {
      setIsLoading(true);
      setError(undefined);
      setStandings(undefined);
      setCharts(undefined);
      const standings = await F1PredictionsApi.getSeasonStandings(season);
      setStandings(standings);
      if (!standings || standings.standings.length === 0) {
        return;
      }

      const members = await DiscordMembersApi.getMembers(
        standings.standings.map((x) => x.userId),
      ).catch(() => []);
      setFinishedRaces(standings.events);
      setCharts(standings.charts);
      setMembers(Array.isArray(members) ? members.filter(Boolean) : []);
    }

    load()
      .catch((error) => setError(getF1ErrorMessage(error)))
      .finally(() => setIsLoading(false));
  }, [season]);

  return (
    <Stack direction={"column"} spacing={1}>
      {isLoading && <Loader />}
      {!isLoading && error && <Alert severity="error">{error}</Alert>}
      {!isLoading && standings && standings.standings.length === 0 && (
        <Typography variant={"body1"}>
          Сезон {season} еще не стартовал
        </Typography>
      )}
      {!isLoading && standings && standings.standings.length > 0 && charts && (
        <Stack direction={"column"} spacing={2}>
          <F1PredictionsStandingsTable
            finishedRaces={finishedRaces}
            members={members}
            standings={standings}
          />
          <F1PredictionsStandingsChart
            key={season}
            standings={standings}
            members={members}
            charts={charts}
            races={finishedRaces}
          />
        </Stack>
      )}
    </Stack>
  );
}
