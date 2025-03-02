namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

public class F1BingoCard
{
    public Guid Id { get; set; }
    public int Season { get; set; }
    public string Description { get; set; }
    public string? Explanation { get; set; }
    public F1BingoCardProbability Probability { get; set; }
    public int TotalRepeats { get; set; }
    public int CompletedRepeats { get; set; }
    public bool IsCompleted { get; set; }
}