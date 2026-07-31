import { F1PredictionUserResultDto } from "./F1PredictionUserResultDto";
import { F1PodiumDto } from "./F1PodiumDto";

export interface F1StandingsRowDto {
  userId: string;
  totalPoints: number;
  results: (F1PredictionUserResultDto | null)[];
  previousPodiums: F1PodiumDto[];
}
