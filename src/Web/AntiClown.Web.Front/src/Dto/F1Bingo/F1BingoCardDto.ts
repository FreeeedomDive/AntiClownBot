import { F1BingoCardProbabilityDto } from "./F1BingoCardProbabilityDto";

export interface F1BingoCardDto {
  id: string;
  season: number;
  description: string;
  explanation: string | null;
  probability: F1BingoCardProbabilityDto;
  totalRepeats: number;
  completedRepeats: number;
  isCompleted: boolean;
}
