namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

public class F1Prediction
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public string TenthPlacePickedDriver { get; set; }
    public F1DnfPrediction DnfPrediction { get; set; }
    public F1SafetyCars SafetyCarsPrediction { get; set; }
    public string[] TeamsPickedDrivers { get; set; }
    public decimal FirstPlaceLeadPrediction { get; set; }
}