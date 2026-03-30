import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { Divider, Stack, Typography } from "@mui/material";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import { Loader } from "../../../../../../Components/Loader/Loader";
import { F1SeasonStatsDto } from "../../../../../../Dto/F1Predictions/F1SeasonStatsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import F1DriverStatsCard from "./F1DriverStatsCard";
import F1LeadGapStaircase from "./F1LeadGapStaircase";
import F1StatsSection from "./F1StatsSection";

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
    <Stack direction="column" spacing={1}>
      {isLoading && <Loader />}
      {!isLoading && !stats && (
        <Typography variant="body1">Сезон {season} ещё не стартовал</Typography>
      )}
      {!isLoading && stats && (
        <>
          <F1StatsSection title="10 место" columns={3}>
            {[
              <F1DriverStatsCard
                title="Сколько очков принесли гонщики за 10 место"
                data={stats.tenthPlacePointsRating}
                scoreLabel={["очко", "очка", "очков"]}
              />,
              <F1DriverStatsCard
                title="Самые популярные предсказания на 10 место"
                data={stats.mostPickedForTenthPlace}
                scoreLabel={["раз", "раза", "раз"]}
              />,
              <F1DriverStatsCard
                title="Выбор 10 места => DNF в гонке"
                data={stats.tenthPickedButDnfed}
                scoreLabel={["раз", "раза", "раз"]}
              />,
            ]}
          </F1StatsSection>

          <Divider />

          <F1StatsSection title="DNF" columns={3}>
            {[
              <F1DriverStatsCard
                title="Самые частые DNF в гонках"
                data={stats.mostDnfDrivers}
                scoreLabel={["раз", "раза", "раз"]}
              />,
              <F1DriverStatsCard
                title="Самые популярные выборы DNF"
                data={stats.mostPickedForDnf}
                scoreLabel={["раз", "раза", "раз"]}
              />,
            ]}
          </F1StatsSection>

          <Divider />

          <F1StatsSection title="Отрыв лидера" columns={2}>
            {[
              <F1LeadGapStaircase
                data={stats.closestLeadGapPredictions}
                members={members}
              />,
            ]}
          </F1StatsSection>
        </>
      )}
    </Stack>
  );
}
