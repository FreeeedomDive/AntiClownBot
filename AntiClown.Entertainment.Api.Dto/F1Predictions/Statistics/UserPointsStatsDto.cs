namespace AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

public class UserPointsStatsDto
{
    public Guid UserId { get; set; }
    public int Races { get; set; }
    public double AveragePoints { get; set; }
    public double MedianPoints { get; set; }
}