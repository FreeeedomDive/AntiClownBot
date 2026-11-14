import { F1DnfPredictionDto } from "./F1DnfPredictionDto";

export function isValidDnfPrediction(prediction: F1DnfPredictionDto): boolean {
  const drivers = prediction.dnfPickedDrivers ?? [];
  return (
    prediction.noDnfPredicted ||
    (drivers.length === 5 &&
      new Set(drivers).size === 5 &&
      drivers.every(Boolean))
  );
}
