namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

public class F1Prediction
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public string TenthPlacePickedDriver { get; set; }
    public DnfPrediction DnfPrediction { get; set; }
    public SafetyCarsCount SafetyCarsPrediction { get; set; }
    [Obsolete("2024-2025")] public string[] TeamsPickedDrivers { get; set; }
    public int DriverPositionPrediction { get; set; }
    public decimal FirstPlaceLeadPrediction { get; set; }
}