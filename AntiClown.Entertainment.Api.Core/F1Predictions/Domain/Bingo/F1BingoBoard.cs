namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

public class F1BingoBoard
{
    public Guid UserId { get; set; }
    public int Season { get; set; }
    public Guid[] Cards { get; set; }
    public bool IsCompleted { get; set; }
}