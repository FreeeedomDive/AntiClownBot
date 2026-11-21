export interface F1BingoDetailsDto {
  season: number;
  activeSeason: number;
  canCreate: boolean;
  board: {
    id: string;
    participantId: string;
    bingoAchieved: boolean;
    cards: F1BingoUserCardDto[];
  } | null;
}

export interface F1BingoUserCardDto {
  id: string;
  description: string;
  explanation: string | null;
  language: string;
  probability: "LOW" | "MEDIUM" | "HIGH";
  requiredRepeats: number;
  completedRepeats: number;
  completed: boolean;
}
