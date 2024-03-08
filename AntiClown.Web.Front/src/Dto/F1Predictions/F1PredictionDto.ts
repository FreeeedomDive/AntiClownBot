import { F1DnfPredictionDto } from "./F1DnfPredictionDto";
import { F1DriverDto } from "./F1DriverDto";
import { F1SafetyCarPredictionDto } from "./F1SafetyCarsPredictionDto";

export interface F1PredictionDto {
  raceId: string;
  userId: string;
  tenthPlacePickedDriver: F1DriverDto;
  safetyCarsPrediction: F1SafetyCarPredictionDto;
  teamsPickedDrivers: F1DriverDto[];
  firstPlaceLeadPrediction: number;
  dnfPrediction: F1DnfPredictionDto;
}
