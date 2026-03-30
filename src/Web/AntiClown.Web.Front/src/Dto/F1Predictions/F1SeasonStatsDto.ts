import { DriverStatisticsDto } from "./DriverStatisticsDto";
import { LeadGapPredictionStatsDto } from "./LeadGapPredictionStatsDto";

export interface F1SeasonStatsDto {
  tenthPlacePointsRating: DriverStatisticsDto[];
  mostPickedForTenthPlace: DriverStatisticsDto[];
  tenthPickedButDnfed: DriverStatisticsDto[];
  driverOwnTenthPlacePoints: DriverStatisticsDto[];
  tenthPlacePredictionEfficiency: DriverStatisticsDto[];
  mostDnfDrivers: DriverStatisticsDto[];
  mostPickedForDnf: DriverStatisticsDto[];
  closestLeadGapPredictions: LeadGapPredictionStatsDto[];
  safetyCarPickCounts: DriverStatisticsDto[];
  safetyCarActualCounts: DriverStatisticsDto[];
  safetyCarCorrectGuesses: DriverStatisticsDto[];
}
