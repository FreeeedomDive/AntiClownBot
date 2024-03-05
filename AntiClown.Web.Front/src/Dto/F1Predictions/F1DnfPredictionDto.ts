import { F1DriverDto } from "./F1DriverDto";

export interface F1DnfPredictionDto {
  noDnfPredicted: boolean;
  dnfPickedDrivers: { $values: F1DriverDto[] | null } | F1DriverDto[] | null;
}
