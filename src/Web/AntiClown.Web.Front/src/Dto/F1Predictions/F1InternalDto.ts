import { F1TeamDto } from "./F1TeamDto";
import { F1ChartsDto } from "./F1ChartsDto";
import { F1SeasonStatsDto } from "./F1SeasonStatsDto";
import { F1RacePredictionConditionsDto } from "./F1RacePredictionConditionsDto";

export type RaceStatus = "SCHEDULED" | "OPEN" | "CLOSED" | "FINISHED";
export type SafetyCarsCount = "ZERO" | "ONE" | "TWO" | "THREE_PLUS";

export interface InternalRaceDto {
  id: string;
  name: string;
  type: "RACE" | "SPRINT";
  status: RaceStatus;
  openAt: string;
  closeAt: string;
}

export interface F1RaceSummary extends InternalRaceDto {
  season: number;
  isSprint: boolean;
}

export interface InternalTeamDto {
  name: string;
  badge: string;
  backgroundColor: string;
  textColor: string;
  drivers: string[];
}

export interface InternalPredictionDto {
  id: string;
  leagueId: string;
  participantId: string;
  raceId: string;
  tenthPlacePickedDriver: string | null;
  dnfPickedDrivers: string[] | null;
  safetyCarsPrediction: SafetyCarsCount | null;
  driverPositionPrediction: number | null;
  firstPlaceLeadPrediction: number | null;
  teamsPickedDrivers: string[] | null;
}

export type SaveOwnPredictionDto = Pick<InternalPredictionDto,
  "tenthPlacePickedDriver" | "dnfPickedDrivers" | "safetyCarsPrediction" |
  "driverPositionPrediction" | "firstPlaceLeadPrediction">;

export interface InternalPredictionPointsDto {
  rulesVersion: string;
  totalPoints: number;
  tenthPlacePoints: number | null;
  dnfsPoints: number | null;
  safetyCarsPoints: number | null;
  firstPlaceLeadPoints: number | null;
  teamMatesPoints: number | null;
  driverPositionPoints: number | null;
}

export interface InternalRaceResultDto {
  race: InternalRaceDto;
  season: number;
  rulesVersion: string;
  result: {
    classification: { driverName: string; status: "FINISHED" | "DNF" | "DNS" | "DQ" }[];
    safetyCars: number | null;
    firstPlaceLead: number | null;
  };
  points: InternalPredictionPointsDto | null;
}

export interface InternalUserRaceDto {
  details: {
    race: InternalRaceDto;
    season: number;
    teams: InternalTeamDto[];
    lineupPrepared: boolean;
    conditions: F1RacePredictionConditionsDto | null;
    predictionsOpen: boolean;
    serverTime: string;
    startingGrid: string[] | null;
  };
  prediction: InternalPredictionDto | null;
  result: InternalRaceResultDto | null;
}

export interface F1UserRaceView extends F1RaceSummary {
  teams: F1TeamDto[];
  conditions: F1RacePredictionConditionsDto | null;
  predictionsOpen: boolean;
  serverTime: string;
  qualifyingGrid: string[];
  prediction: InternalPredictionDto | null;
  result: InternalRaceResultDto | null;
}

export interface InternalStandingsDto {
  season: number;
  events: InternalRaceDto[];
  rows: {
    participantId: string;
    participantName: string;
    totalPoints: number;
    maximumTenthPlaceCount: number;
    results: { raceId: string; points: InternalPredictionPointsDto | null }[];
    previousPodiums: { season: number; place: number }[];
  }[];
  currentLeaderParticipantId: string | null;
  currentLeaderPoints: number;
  pointsLeft: number;
  charts: { participantId: string; points: number[] }[];
  championChart: number[];
}

export interface F1RacePointsView extends InternalPredictionPointsDto {
  raceId: string;
  userId: string;
}

export interface F1StandingsRowView {
  userId: string;
  participantName: string;
  totalPoints: number;
  maximumTenthPlaceCount: number;
  results: (F1RacePointsView | null)[];
  previousPodiums: { season: number; place: number }[];
}

export interface F1StandingsView {
  standings: F1StandingsRowView[];
  currentLeaderPoints: number;
  pointsLeft: number;
  events: F1RaceSummary[];
  charts: F1ChartsDto;
}

export interface InternalSeasonStatsDto extends Omit<F1SeasonStatsDto,
  "tenthPlaceDnfAntiRating" | "closestLeadGapPredictions" |
  "safetyCarPickCounts" | "safetyCarActualCounts" | "safetyCarCorrectGuesses"> {
  tenthPlaceDnfAntiRating: { participantId: string; participantName: string; score: number }[];
  closestLeadGapPredictions: {
    participantId: string;
    participantName: string;
    raceName: string;
    predictedGap: number;
    actualGap: number;
    difference: number;
  }[];
  safetyCarPickCounts: { safetyCars: SafetyCarsCount; score: number }[];
  safetyCarActualCounts: { safetyCars: SafetyCarsCount; score: number }[];
  safetyCarCorrectGuesses: { safetyCars: SafetyCarsCount; score: number }[];
}
