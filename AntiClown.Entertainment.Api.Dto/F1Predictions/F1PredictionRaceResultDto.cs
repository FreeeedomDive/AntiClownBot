namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionRaceResultDto
{
    public Guid RaceId { get; set; }
    public F1DriverDto[] Classification { get; set; }
    public F1DriverDto[] DnfDrivers { get; set; }
    public F1SafetyCarsPredictionDto SafetyCars { get; set; }
    public decimal FirstPlaceLead { get; set; }
}