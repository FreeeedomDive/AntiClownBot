import {F1PredictionUserResultDto} from "../Dto/F1Predictions/F1PredictionUserResultDto";

export function countPointsForRace(raceResult: F1PredictionUserResultDto) {
  return raceResult.tenthPlacePoints
    + raceResult.dnfsPoints
    + raceResult.safetyCarsPoints
    + raceResult.firstPlaceLeadPoints
    + raceResult.teamMatesPoints;
}

export function countTotalPoints(raceResults: (F1PredictionUserResultDto | null)[]) {
  return raceResults
    .map(x => x === null ? 0 : countPointsForRace(x))
    .reduce((acc, curr) => acc + curr, 0);
}