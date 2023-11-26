namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class UserPointsStats
{
    public Guid UserId { get; set; }
    public int Races { get; set; }
    public int AveragePoints { get; set; }
    public int MedianPoints { get; set; }
}