import { F1PredictionDto } from "./F1PredictionDto";
import { F1PredictionRaceResultDto } from "./F1PredictionRaceResultDto";
import { F1RacePredictionConditionsDto } from "./F1RacePredictionConditionsDto";

export interface F1RaceDto {
  id: string;
  season: number;
  name: string;
  isActive: boolean;
  isOpened: boolean;
  isSprint: boolean;
  conditions?: F1RacePredictionConditionsDto;
  predictions: F1PredictionDto[];
  result: F1PredictionRaceResultDto;
}
