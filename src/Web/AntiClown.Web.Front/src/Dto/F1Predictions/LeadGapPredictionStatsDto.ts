export interface LeadGapPredictionStatsDto {
  userId: string;
  raceName: string;
  predictedGap: number;
  actualGap: number;
  difference: number;
}
