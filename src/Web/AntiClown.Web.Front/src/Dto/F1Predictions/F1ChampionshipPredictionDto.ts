export interface F1ChampionshipPredictionDto {
  season: number;
  userId: string;
  preSeasonStandings: string[] | null;
  midSeasonStandings: string[] | null;
}
