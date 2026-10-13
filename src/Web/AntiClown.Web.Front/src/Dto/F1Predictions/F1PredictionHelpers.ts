import { F1DnfPredictionDto } from "./F1DnfPredictionDto";
import { getDriversFromTeams } from "./F1DriversHelpers";
import { F1RaceDto } from "./F1RaceDto";
import { F1TeamDto } from "./F1TeamDto";

export function getPredictionDrivers(
  race: F1RaceDto,
  teams: F1TeamDto[],
): string[] {
  const grid = race.qualifyingGrid?.length
    ? race.qualifyingGrid
    : race.result.classification.length
      ? race.result.classification
      : getDriversFromTeams(teams);

  return Array.from(
    new Set([
      ...grid,
      ...race.result.classification,
      ...race.result.dnfDrivers,
      ...(race.conditions?.positionPredictionDriver
        ? [race.conditions.positionPredictionDriver]
        : []),
      ...race.predictions.flatMap((prediction) => [
        prediction.tenthPlacePickedDriver,
        ...(prediction.dnfPrediction.dnfPickedDrivers ?? []),
        ...(prediction.teamsPickedDrivers ?? []),
      ]),
    ]),
  ).filter(Boolean);
}

export function getPredictionMaxPosition(
  race: F1RaceDto,
  teams: F1TeamDto[],
): number {
  return (
    race.qualifyingGrid?.length ||
    race.result.classification.length ||
    getDriversFromTeams(teams).length
  );
}

export function isValidDnfPrediction(prediction: F1DnfPredictionDto): boolean {
  const drivers = prediction.dnfPickedDrivers ?? [];
  return (
    prediction.noDnfPredicted ||
    (drivers.length === 5 &&
      new Set(drivers).size === 5 &&
      drivers.every(Boolean))
  );
}
