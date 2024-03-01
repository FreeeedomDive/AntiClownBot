import {F1DriverDto} from "./F1DriverDto";

export interface F1DnfPredictionDto{
  noDnfPrediction: boolean;
  dnfPickedDrivers: F1DriverDto[] | null;
}