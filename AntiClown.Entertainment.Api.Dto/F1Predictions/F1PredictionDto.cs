namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionDto
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public F1DriverDto TenthPlacePickedDriver { get; set; }
    public F1DnfPredictionDto DnfPrediction { get; set; }
    public F1SafetyCarsPredictionDto SafetyCarsPrediction { get; set; }
    public F1DriverDto[] TeamsPickedDrivers { get; set; }
    public decimal FirstPlaceLeadPrediction { get; set; }
}