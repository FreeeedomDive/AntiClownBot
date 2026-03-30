import { DriverStatisticsDto } from "./DriverStatisticsDto";
import { UserStatisticsDto } from "./UserStatisticsDto";
import { LeadGapPredictionStatsDto } from "./LeadGapPredictionStatsDto";

export interface F1SeasonStatsDto {
  tenthPlacePointsRating: DriverStatisticsDto[];
  mostPickedForTenthPlace: DriverStatisticsDto[];
  tenthPickedButDnfed: DriverStatisticsDto[];
  driverOwnTenthPlacePoints: DriverStatisticsDto[];
  tenthPlacePredictionEfficiency: DriverStatisticsDto[];
  mostDnfDrivers: DriverStatisticsDto[];
  mostPickedForDnf: DriverStatisticsDto[];
  tenthPlaceDnfAntiRating: UserStatisticsDto[];
  closestLeadGapPredictions: LeadGapPredictionStatsDto[];
  safetyCarPickCounts: DriverStatisticsDto[];
  safetyCarActualCounts: DriverStatisticsDto[];
  safetyCarCorrectGuesses: DriverStatisticsDto[];
}
