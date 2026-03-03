import {F1PredictionUserResultDto} from "./F1PredictionUserResultDto";

export interface F1PredictionsStandingsDto {
  [userId: string] : F1PredictionUserResultDto[]
}