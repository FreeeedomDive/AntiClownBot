export interface F1PredictionRaceResultDto {
  raceId: string;
  classification: string[];
  dnfDrivers: string[];
  safetyCars: number;
  firstPlaceLead: number;
}