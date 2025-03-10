namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;

public class F1BingoCardDto
{
    public Guid Id { get; set; }
    public int Season { get; set; }
    public string Description { get; set; }
    public string? Explanation { get; set; }
    public F1BingoCardProbabilityDto Probability { get; set; }
    public int TotalRepeats { get; set; }
    public int CompletedRepeats { get; set; }
    public bool IsCompleted { get; set; }
}