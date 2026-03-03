import { F1StandingsRowDto } from "./F1StandingsRowDto";

export interface F1StandingsDto {
  standings: F1StandingsRowDto[];
  currentLeaderPoints: number;
  pointsLeft: number;
}
