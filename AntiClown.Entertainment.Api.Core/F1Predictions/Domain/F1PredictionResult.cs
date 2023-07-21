namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class F1PredictionResult
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int FirstDnfPoints { get; set; }
}