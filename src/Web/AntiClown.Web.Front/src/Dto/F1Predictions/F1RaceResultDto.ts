import { F1PredictionRaceResultDto } from "./F1PredictionRaceResultDto";

export interface F1RaceResultDto {
  success: boolean;
  result: F1PredictionRaceResultDto;
}
