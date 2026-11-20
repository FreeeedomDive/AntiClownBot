import axios from "axios";
import { F1RaceFilterDto } from "../Dto/F1Predictions/F1RaceFilterDto";
import { F1TeamDto } from "../Dto/F1Predictions/F1TeamDto";
import { F1SeasonStatsDto } from "../Dto/F1Predictions/F1SeasonStatsDto";
import {
  F1ChampionshipDetailsDto,
  F1ChampionshipPredictionDto,
  F1ChampionshipStage,
} from "../Dto/F1Predictions/F1ChampionshipDetailsDto";
import {
  InternalRaceDto,
  InternalTeamDto,
  InternalUserRaceDto,
  InternalStandingsDto,
  InternalSeasonStatsDto,
  InternalPredictionDto,
  SaveOwnPredictionDto,
  F1RaceSummary,
  F1UserRaceView,
  F1StandingsView,
} from "../Dto/F1Predictions/F1InternalDto";

export function getF1ErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (error.response?.status === 401)
      return "Сессия истекла. Войдите заново.";
    return (
      error.response?.data?.detail ??
      error.response?.data?.title ??
      "F1Predictions недоступен. Попробуйте позже."
    );
  }
  return "Не удалось загрузить данные F1Predictions";
}

function raceSummary(race: InternalRaceDto, season: number): F1RaceSummary {
  return { ...race, season, isSprint: race.type === "SPRINT" };
}

function teamView(team: InternalTeamDto): F1TeamDto {
  return {
    ...team,
    firstDriver: team.drivers[0] ?? "",
    secondDriver: team.drivers[1] ?? "",
  };
}

export default class F1PredictionsApi {
  static initV2 = () =>
    axios.create({
      baseURL: "/webApi/v2/f1Predictions/",
      timeout: 30000,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    });

  static findOwnRaces = async (
    filter: F1RaceFilterDto,
  ): Promise<F1RaceSummary[]> => {
    const season = filter.season ?? new Date().getFullYear();
    const response = await F1PredictionsApi.initV2().get<{
      data: InternalRaceDto[];
    }>("races", { params: { season } });
    return response.data.data
      .filter(
        (race) =>
          filter.isActive === undefined ||
          (filter.isActive
            ? race.status === "OPEN" || race.status === "CLOSED"
            : race.status === "FINISHED"),
      )
      .map((race) => raceSummary(race, season));
  };

  static readOwnRace = async (raceId: string): Promise<F1UserRaceView> => {
    const { data } = await F1PredictionsApi.initV2().get<InternalUserRaceDto>(
      `races/${raceId}`,
    );
    return {
      ...raceSummary(data.details.race, data.details.season),
      teams: data.details.teams.map(teamView),
      conditions: data.details.conditions,
      predictionsOpen: data.details.predictionsOpen,
      serverTime: data.details.serverTime,
      qualifyingGrid: data.details.startingGrid ?? [],
      prediction: data.prediction,
      result: data.result,
    };
  };

  static saveOwnPrediction = async (
    raceId: string,
    prediction: SaveOwnPredictionDto,
  ): Promise<InternalPredictionDto> => {
    const { data } = await F1PredictionsApi.initV2().put<InternalPredictionDto>(
      `races/${raceId}/prediction`,
      prediction,
    );
    return data;
  };

  static getChampionship = async (
    season: number,
    signal?: AbortSignal,
  ): Promise<F1ChampionshipDetailsDto> => {
    const { data } = await F1PredictionsApi.initV2().get<
      Omit<F1ChampionshipDetailsDto, "teams"> & { teams: InternalTeamDto[] }
    >(`championship/${season}`, { signal });
    return { ...data, teams: data.teams.map(teamView) };
  };

  static saveChampionshipPrediction = async (
    season: number,
    stage: F1ChampionshipStage,
    driverStandings: string[],
  ): Promise<F1ChampionshipPredictionDto> => {
    const { data } =
      await F1PredictionsApi.initV2().put<F1ChampionshipPredictionDto>(
        `championship/${season}/predictions/${stage}`,
        { driverStandings },
      );
    return data;
  };

  static getSeasonStandings = async (
    season: number,
  ): Promise<F1StandingsView> => {
    const { data } = await F1PredictionsApi.initV2().get<InternalStandingsDto>(
      "standings",
      { params: { season } },
    );
    return {
      standings: data.rows.map((row) => ({
        userId: row.participantId,
        participantName: row.participantName,
        totalPoints: row.totalPoints,
        maximumTenthPlaceCount: row.maximumTenthPlaceCount,
        results: row.results.map((result) =>
          result.points === null
            ? null
            : {
                ...result.points,
                raceId: result.raceId,
                userId: row.participantId,
              },
        ),
        previousPodiums: row.previousPodiums,
      })),
      currentLeaderPoints: data.currentLeaderPoints,
      pointsLeft: data.pointsLeft,
      events: data.events.map((race) => raceSummary(race, data.season)),
      charts: {
        usersCharts: data.charts.map((chart) => ({
          userId: chart.participantId,
          points: chart.points,
        })),
        championChart: {
          userId: data.currentLeaderParticipantId ?? "",
          points: data.championChart,
        },
      },
    };
  };

  static getActiveTeams = async (): Promise<F1TeamDto[]> => {
    const result = await F1PredictionsApi.initV2().get<{
      data: InternalTeamDto[];
    }>(`teams`);
    return result.data.data.map(teamView);
  };

  static getSeasonStats = async (season: number): Promise<F1SeasonStatsDto> => {
    const result = await F1PredictionsApi.initV2().get<InternalSeasonStatsDto>(
      `stats`,
      {
        params: { season },
      },
    );
    const data = result.data;
    const incidents = (items: { safetyCars: string; score: number }[]) =>
      items.map((item) => ({
        driver: (
          { ZERO: "0", ONE: "1", TWO: "2", THREE_PLUS: "3+" } as Record<
            string,
            string
          >
        )[item.safetyCars],
        score: item.score,
      }));
    return {
      ...data,
      tenthPlaceDnfAntiRating: data.tenthPlaceDnfAntiRating.map((row) => ({
        ...row,
        userId: row.participantId,
      })),
      closestLeadGapPredictions: data.closestLeadGapPredictions.map((row) => ({
        ...row,
        userId: row.participantId,
      })),
      safetyCarPickCounts: incidents(data.safetyCarPickCounts),
      safetyCarActualCounts: incidents(data.safetyCarActualCounts),
      safetyCarCorrectGuesses: incidents(data.safetyCarCorrectGuesses),
    };
  };
}
