import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { Box, Grid, Typography } from "@mui/material";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import { Loader } from "../../../../../../Components/Loader/Loader";
import { F1SeasonStatsDto } from "../../../../../../Dto/F1Predictions/F1SeasonStatsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import F1DriverStatsCard from "./F1DriverStatsCard";
import F1LeadGapStaircase from "./F1LeadGapStaircase";

export default function F1PredictionsStats() {
  const [searchParams] = useSearchParams();
  const season = Number(searchParams.get("season") ?? new Date().getFullYear());

  const [isLoading, setIsLoading] = useState(true);
  const [stats, setStats] = useState<F1SeasonStatsDto | null>(null);
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);

  useEffect(() => {
    async function load() {
      setIsLoading(true);
      const loaded = await F1PredictionsApi.getSeasonStats(season);
      setStats(loaded);

      const userIds = loaded.closestLeadGapPredictions.map((x) => x.userId);
      if (userIds.length > 0) {
        const loadedMembers = await DiscordMembersApi.getMembers(userIds);
        setMembers(loadedMembers);
      }
    }

    load()
      .catch(console.error)
      .finally(() => setIsLoading(false));
  }, [season]);

  return (
    <Box>
      {isLoading && <Loader />}
      {!isLoading && !stats && (
        <Typography variant="body1">Сезон {season} ещё не стартовал</Typography>
      )}
      {!isLoading && stats && (
        <Grid container spacing={2}>
          <Grid item xs={4}>
            <F1DriverStatsCard
              title="Рейтинг по полученным очкам за гонщиков за 10 место"
              data={stats.tenthPlacePointsRating}
              scoreLabel="очк."
            />
          </Grid>
          <Grid item xs={4}>
            <F1DriverStatsCard
              title="Сколько раз выбирали на 10 место"
              data={stats.mostPickedForTenthPlace}
              scoreLabel="раз"
            />
          </Grid>
          <Grid item xs={4}>
            <F1DriverStatsCard
              title="Выбор 10 места => DNF в гонке"
              data={stats.tenthPickedButDnfed}
              scoreLabel="раз"
            />
          </Grid>
          <Grid item xs={4}>
            <F1DriverStatsCard
              title="Самые частые DNF в гонке"
              data={stats.mostDnfDrivers}
              scoreLabel="раз"
            />
          </Grid>
          <Grid item xs={4}>
            <F1DriverStatsCard
              title="Самые частые предсказания DNF"
              data={stats.mostPickedForDnf}
              scoreLabel="раз"
            />
          </Grid>
          <Grid item xs={4}>
            <F1LeadGapStaircase
              data={stats.closestLeadGapPredictions}
              members={members}
            />
          </Grid>
        </Grid>
      )}
    </Box>
  );
}
