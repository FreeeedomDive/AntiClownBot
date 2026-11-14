import axios from "axios";
import { F1PredictionDto } from "../Dto/F1Predictions/F1PredictionDto";
import { F1RaceDto } from "../Dto/F1Predictions/F1RaceDto";
import { AddPredictionResultDto } from "../Dto/F1Predictions/AddPredictionResultDto";
import { F1PredictionRaceResultDto } from "../Dto/F1Predictions/F1PredictionRaceResultDto";
import { F1RaceFilterDto } from "../Dto/F1Predictions/F1RaceFilterDto";
import { F1TeamDto } from "../Dto/F1Predictions/F1TeamDto";
import { F1ChartsDto } from "../Dto/F1Predictions/F1ChartsDto";
import { F1StandingsDto } from "../Dto/F1Predictions/F1StandingsDto";
import { F1SeasonStatsDto } from "../Dto/F1Predictions/F1SeasonStatsDto";
import {
  InternalRaceDto, InternalTeamDto, InternalUserRaceDto, InternalStandingsDto,
  InternalSeasonStatsDto, InternalPredictionDto, SaveOwnPredictionDto,
  F1RaceSummary, F1UserRaceView, F1StandingsView,
} from "../Dto/F1Predictions/F1InternalDto";

export function getF1ErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (error.response?.status === 401) return "Сессия истекла. Войдите заново.";
    return error.response?.data?.detail ?? error.response?.data?.title ?? "F1Predictions недоступен. Попробуйте позже.";
  }
  return "Не удалось загрузить данные F1Predictions";
}

function raceSummary(race: InternalRaceDto, season: number): F1RaceSummary {
  return { ...race, season, isSprint: race.type === "SPRINT" };
}

function teamView(team: InternalTeamDto): F1TeamDto {
  return { ...team, firstDriver: team.drivers[0] ?? "", secondDriver: team.drivers[1] ?? "" };
}

export default class F1PredictionsApi {
  static initV2 = () => axios.create({
    baseURL: "/webApi/v2/f1Predictions/",
    timeout: 30000,
    headers: { Accept: "application/json", "Content-Type": "application/json" },
  });

  static findOwnRaces = async (filter: F1RaceFilterDto): Promise<F1RaceSummary[]> => {
    const season = filter.season ?? new Date().getFullYear();
    const response = await F1PredictionsApi.initV2().get<{ data: InternalRaceDto[] }>("races", { params: { season } });
    return response.data.data
      .filter((race) => filter.isActive === undefined || (filter.isActive
        ? race.status === "OPEN" || race.status === "CLOSED"
        : race.status === "FINISHED"))
      .map((race) => raceSummary(race, season));
  };

  static readOwnRace = async (raceId: string): Promise<F1UserRaceView> => {
    const { data } = await F1PredictionsApi.initV2().get<InternalUserRaceDto>(`races/${raceId}`);
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

  static saveOwnPrediction = async (raceId: string, prediction: SaveOwnPredictionDto): Promise<InternalPredictionDto> => {
    const { data } = await F1PredictionsApi.initV2().put<InternalPredictionDto>(`races/${raceId}/prediction`, prediction);
    return data;
  };

  static getSeasonStandings = async (season: number): Promise<F1StandingsView> => {
    const { data } = await F1PredictionsApi.initV2().get<InternalStandingsDto>("standings", { params: { season } });
    return {
      standings: data.rows.map((row) => ({
        userId: row.participantId,
        participantName: row.participantName,
        totalPoints: row.totalPoints,
        maximumTenthPlaceCount: row.maximumTenthPlaceCount,
        results: row.results.map((result) => result.points === null ? null : {
          ...result.points, raceId: result.raceId, userId: row.participantId,
        }),
        previousPodiums: row.previousPodiums,
      })),
      currentLeaderPoints: data.currentLeaderPoints,
      pointsLeft: data.pointsLeft,
      events: data.events.map((race) => raceSummary(race, data.season)),
      charts: {
        usersCharts: data.charts.map((chart) => ({ userId: chart.participantId, points: chart.points })),
        championChart: { userId: data.currentLeaderParticipantId ?? "", points: data.championChart },
      },
    };
  };

  static init = () => {
    return axios.create({
      baseURL: `/webApi/f1predictions/`,
      timeout: 10000,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      validateStatus: function (status) {
        return status >= 200 && status < 300;
      },
    });
  };

  static find = async (filter: F1RaceFilterDto): Promise<F1RaceDto[]> => {
    const result = await F1PredictionsApi.init().post<F1RaceDto[]>(
      `find`,
      filter,
    );
    return result.data;
  };

  static read = async (raceId: string, userId?: string): Promise<F1RaceDto> => {
    const result = await F1PredictionsApi.init().get<F1RaceDto>(raceId, {
      params: userId ? { userId } : undefined,
    });
    return result.data;
  };

  static addPrediction = async (
    raceId: string,
    prediction: F1PredictionDto,
    isAdmin = false,
  ): Promise<AddPredictionResultDto> => {
    const result = await F1PredictionsApi.init().post<AddPredictionResultDto>(
      `${raceId}/addPrediction`,
      prediction,
      { params: { isAdmin } },
    );
    return result.data;
  };

  static close = async (raceId: string): Promise<void> => {
    await F1PredictionsApi.init().post(`${raceId}/close`);
  };

  static addResult = async (
    raceId: string,
    result: F1PredictionRaceResultDto,
  ): Promise<void> => {
    await F1PredictionsApi.init().post(`${raceId}/addResult`, result);
  };

  static finish = async (raceId: string): Promise<void> => {
    await F1PredictionsApi.init().post(`${raceId}/finish`);
  };

  static getStandings = async (season?: number): Promise<F1StandingsDto> => {
    const result = await F1PredictionsApi.init().get(`standingsV2`, {
      params: { season },
    });
    return result.data;
  };

  static getCharts = async (season?: number): Promise<F1ChartsDto> => {
    const result = await F1PredictionsApi.init().get(`charts`, {
      params: { season },
    });
    return result.data;
  };

  static getActiveTeams = async (): Promise<F1TeamDto[]> => {
    const result = await F1PredictionsApi.initV2().get<{ data: InternalTeamDto[] }>(`teams`);
    return result.data.data.map(teamView);
  };

  static getLegacyTeams = async (): Promise<F1TeamDto[]> => {
    const result = await F1PredictionsApi.init().get<F1TeamDto[]>("teams");
    return result.data;
  };

  static saveQualifyingGrid = async (
    raceId: string,
    grid: string[],
  ): Promise<void> => {
    await F1PredictionsApi.init().post(`${raceId}/saveQualifyingGrid`, grid);
  };

  static getSeasonStats = async (season: number): Promise<F1SeasonStatsDto> => {
    const result = await F1PredictionsApi.initV2().get<InternalSeasonStatsDto>(
      `stats`,
      {
        params: { season },
      },
    );
    const data = result.data;
    const incidents = (items: { safetyCars: string; score: number }[]) => items.map((item) => ({
      driver: ({ ZERO: "0", ONE: "1", TWO: "2", THREE_PLUS: "3+" } as Record<string, string>)[item.safetyCars],
      score: item.score,
    }));
    return {
      ...data,
      tenthPlaceDnfAntiRating: data.tenthPlaceDnfAntiRating.map((row) => ({ ...row, userId: row.participantId })),
      closestLeadGapPredictions: data.closestLeadGapPredictions.map((row) => ({ ...row, userId: row.participantId })),
      safetyCarPickCounts: incidents(data.safetyCarPickCounts),
      safetyCarActualCounts: incidents(data.safetyCarActualCounts),
      safetyCarCorrectGuesses: incidents(data.safetyCarCorrectGuesses),
    };
  };

  static createOrUpdateTeam = async (
    teamName: string,
    firstDriver: string,
    secondDriver: string,
  ): Promise<void> => {
    const team = {
      name: teamName,
      firstDriver: firstDriver,
      secondDriver: secondDriver,
    };
    const result = await F1PredictionsApi.init().post(`teams`, team);
    return result.data;
  };
}
