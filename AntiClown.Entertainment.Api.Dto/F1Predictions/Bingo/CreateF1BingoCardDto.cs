namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;

public class CreateF1BingoCardDto
{
    public int Season { get; set; }
    public string Description { get; set; }
    public string? Explanation { get; set; }
    public F1BingoCardProbabilityDto Probability { get; set; }
    public int TotalRepeats { get; set; }
}