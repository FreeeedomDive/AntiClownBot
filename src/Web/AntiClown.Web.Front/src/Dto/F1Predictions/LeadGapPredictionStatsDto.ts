export interface LeadGapPredictionStatsDto {
  userId: string;
  participantName?: string;
  raceName: string;
  predictedGap: number;
  actualGap: number;
  difference: number;
}
