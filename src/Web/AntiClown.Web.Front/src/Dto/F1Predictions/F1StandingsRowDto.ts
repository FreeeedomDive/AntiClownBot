import { F1PredictionUserResultDto } from "./F1PredictionUserResultDto";

export interface F1StandingsRowDto {
  userId: string;
  totalPoints: number;
  results: F1PredictionUserResultDto[];
}
