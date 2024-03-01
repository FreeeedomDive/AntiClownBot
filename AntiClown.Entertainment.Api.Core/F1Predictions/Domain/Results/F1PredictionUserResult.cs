namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

public class F1PredictionUserResult
{
    public Guid UserId { get; set; }
    public int[] PointsPerRace { get; set; }
}