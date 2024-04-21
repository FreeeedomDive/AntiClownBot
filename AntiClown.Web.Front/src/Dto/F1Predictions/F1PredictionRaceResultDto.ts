import {F1DriverDto} from "./F1DriverDto";

export interface F1PredictionRaceResultDto {
  raceId: string;
  classification: F1DriverDto[];
  dnfDrivers: F1DriverDto[];
  safetyCars: number;
  firstPlaceLead: number;
}