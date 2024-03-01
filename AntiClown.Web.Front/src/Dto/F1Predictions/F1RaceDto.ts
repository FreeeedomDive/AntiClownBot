import {F1PredictionDto} from "./F1PredictionDto";

export interface F1RaceDto{
  id: string;
  season: int;
  name: string;
  isActive: boolean;
  isOpened: boolean;
  predictions: F1PredictionDto[];
}