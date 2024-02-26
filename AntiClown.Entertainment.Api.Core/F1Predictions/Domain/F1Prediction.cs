namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class F1Prediction
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public F1Driver TenthPlacePickedDriver { get; set; }
    public F1Driver FirstDnfPickedDriver { get; set; }
    public F1DnfPrediction DnfPrediction { get; set; }
}