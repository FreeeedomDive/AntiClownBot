namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class F1PredictionUserResult
{
    public Guid UserId { get; set; }
    public int[] PointsPerRace { get; set; }
}