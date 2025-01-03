import {F1PredictionUserResultDto} from "../Dto/F1Predictions/F1PredictionUserResultDto";

export function countTotalPoints(raceResults: (F1PredictionUserResultDto | null)[]) {
  return raceResults
    .map(x => x === null ? 0 : x.totalPoints)
    .reduce((acc, curr) => acc + curr, 0);
}