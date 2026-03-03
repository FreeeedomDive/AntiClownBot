import { F1ChampionshipPredictionTypeDto } from "./F1ChampionshipPredictionTypeDto";

export interface F1ChampionshipResultsDto {
  hasData: boolean;
  stage: F1ChampionshipPredictionTypeDto;
  isOpen: boolean;
  standings: string[];
}
