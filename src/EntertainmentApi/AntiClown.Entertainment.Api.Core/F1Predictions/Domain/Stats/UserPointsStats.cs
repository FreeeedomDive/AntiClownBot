namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;

public class UserPointsStats
{
    public Guid UserId { get; set; }
    public int Races { get; set; }
    public double AveragePoints { get; set; }
    public double MedianPoints { get; set; }
}