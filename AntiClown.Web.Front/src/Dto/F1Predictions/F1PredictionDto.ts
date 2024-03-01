import {F1DriverDto} from "./F1DriverDto";
import {F1SafetyCarsPredictionDto} from "./F1SafetyCarsPredictionDto";

export interface F1PredictionDto {
  raceId: string;
  userId: string;
  tenthPlacePickedDriver: F1DriverDto;
  safetyCarsPrediction: F1SafetyCarsPredictionDto;
  teamsPickedDrivers: F1DriverDto[];
  firstPlaceLeadPrediction: number;
}