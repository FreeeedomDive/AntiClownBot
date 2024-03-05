import { F1PredictionDto } from "./F1PredictionDto";

export interface F1RaceDto {
  id: string;
  season: number;
  name: string;
  isActive: boolean;
  isOpened: boolean;
  predictions:
    | {
        $values: F1PredictionDto[];
      }
    | F1PredictionDto[];
}
