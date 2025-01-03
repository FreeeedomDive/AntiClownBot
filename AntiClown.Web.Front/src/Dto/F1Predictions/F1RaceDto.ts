import { F1PredictionDto } from "./F1PredictionDto";
import {F1PredictionRaceResultDto} from "./F1PredictionRaceResultDto";

export interface F1RaceDto {
  id: string;
  season: number;
  name: string;
  isActive: boolean;
  isOpened: boolean;
  isSprint: boolean;
  predictions: F1PredictionDto[];
  result: F1PredictionRaceResultDto;
}
