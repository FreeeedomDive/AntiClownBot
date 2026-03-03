namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionDto
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public string TenthPlacePickedDriver { get; set; }
    public F1DnfPredictionDto DnfPrediction { get; set; }
    public F1SafetyCarsPredictionDto SafetyCarsPrediction { get; set; }
    [Obsolete("2024-2025")] public string[] TeamsPickedDrivers { get; set; }
    public decimal FirstPlaceLeadPrediction { get; set; }
    public int DriverPositionPrediction { get; set; }
}