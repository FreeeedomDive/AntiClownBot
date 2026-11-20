import { F1TeamDto } from "./F1TeamDto";

export type F1ChampionshipStage = "PRE_SEASON" | "MID_SEASON";

export interface F1ChampionshipPredictionDto {
  stage: F1ChampionshipStage;
  driverStandings: string[];
  predictionSource: "USER" | "PRE_SEASON_COPY";
  points: number[];
}

export interface F1ChampionshipParticipantPointsDto {
  participantId: string;
  displayName: string;
  totalPoints: number;
}

export interface F1ChampionshipDetailsDto {
  configured: boolean;
  currentStage: F1ChampionshipStage | null;
  stageStarted: boolean;
  predictionsOpen: boolean;
  preSeasonRoster: string[];
  midSeasonRoster: string[];
  driverStandings: string[];
  preSeasonPrediction: F1ChampionshipPredictionDto | null;
  midSeasonPrediction: F1ChampionshipPredictionDto | null;
  rows: F1ChampionshipParticipantPointsDto[];
  teams: F1TeamDto[];
  currentDrivers: string[];
}
