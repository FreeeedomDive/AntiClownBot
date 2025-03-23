import { useEffect, useState } from "react";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import { F1PredictionsStandingsDto } from "../../../../../../Dto/F1Predictions/F1PredictionsStandingsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { countTotalPoints } from "../../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import { Stack, Typography } from "@mui/material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1PredictionsStandingsChart from "./F1PredictionsStandingsChart";
import F1PredictionsStandingsSeasonSelect from "./F1PredictionsStandingsSeasonSelect";
import { F1PredictionUserResultDto } from "../../../../../../Dto/F1Predictions/F1PredictionUserResultDto";
import F1PredictionsStandingsTable from "./F1PredictionsStandingsTable";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";

export default function F1PredictionsStandings() {
  const [isLoading, setIsLoading] = useState(true);
  const [finishedRaces, setFinishedRaces] = useState<F1RaceDto[]>([]);
  const [standings, setStandings] = useState<F1PredictionsStandingsDto>({});
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);
  const [season, setSeason] = useState(new Date().getFullYear());
  const [sortedStandings, setSortedStandings] =
    useState<F1PredictionUserResultDto[][]>();

  useEffect(() => {
    async function load() {
      const allFinishedRaces = await F1PredictionsApi.find({
        isActive: false,
        season,
      });
      const standings = await F1PredictionsApi.getStandings(season);
      const members = await DiscordMembersApi.getMembers(
        Object.keys(standings),
      );
      const sortedStandings = Object.values(standings).sort(
        (a, b) => countTotalPoints(b) - countTotalPoints(a),
      );

      setFinishedRaces(allFinishedRaces);
      setStandings(standings);
      setSortedStandings(sortedStandings);
      setMembers(members);
      setIsLoading(false);
    }

    load().catch(console.error);
  }, [season]);

  return (
    <Stack direction={"column"} spacing={2}>
      {isLoading && <Loader />}
      {!isLoading && (
        <F1PredictionsStandingsSeasonSelect
          season={season}
          setSeason={setSeason}
        />
      )}
      {!isLoading && sortedStandings && sortedStandings.length === 0 && (
        <Typography variant={"h5"}>Сезон {season} еще не стартовал</Typography>
      )}
      {!isLoading && sortedStandings && sortedStandings.length > 0 && (
        <Stack direction={"column"} spacing={4}>
          <F1PredictionsStandingsTable
            finishedRaces={finishedRaces}
            sortedStandings={sortedStandings}
            members={members}
            standings={standings}
          />
          <F1PredictionsStandingsChart
            season={season}
            standings={standings}
            members={members}
          />
        </Stack>
      )}
    </Stack>
  );
}
