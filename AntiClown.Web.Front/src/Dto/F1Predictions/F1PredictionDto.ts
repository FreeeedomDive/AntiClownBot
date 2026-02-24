import { F1DnfPredictionDto } from "./F1DnfPredictionDto";
import { F1SafetyCarPredictionDto } from "./F1SafetyCarsPredictionDto";

export interface F1PredictionDto {
  raceId: string;
  userId: string;
  tenthPlacePickedDriver: string;
  safetyCarsPrediction: F1SafetyCarPredictionDto;
  teamsPickedDrivers?: string[];
  firstPlaceLeadPrediction: number;
  dnfPrediction: F1DnfPredictionDto;
  driverPositionPrediction: number;
}
