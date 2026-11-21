namespace AntiClown.F1Predictions.Dto;

public record BingoDetailsDto(int Season, int ActiveSeason, bool CanCreate, BingoBoardDto? Board);

public record BingoBoardDto(Guid Id, Guid ParticipantId, bool BingoAchieved, BingoCardDto[] Cards);

public record BingoCardDto(Guid Id, string Description, string? Explanation, string Language,
    BingoCardProbabilityDto Probability, int RequiredRepeats, int CompletedRepeats, bool Completed);

public enum BingoCardProbabilityDto
{
    LOW,
    MEDIUM,
    HIGH,
}
