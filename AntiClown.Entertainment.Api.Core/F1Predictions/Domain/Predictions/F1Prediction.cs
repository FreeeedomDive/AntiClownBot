namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

public class F1Prediction
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public F1Driver TenthPlacePickedDriver { get; set; }
    public F1DnfPrediction DnfPrediction { get; set; }
    public F1SafetyCars SafetyCarsPrediction { get; set; }
    public F1Driver[] TeamsPickedDrivers { get; set; }
    public decimal FirstPlaceLeadPrediction { get; set; }
}