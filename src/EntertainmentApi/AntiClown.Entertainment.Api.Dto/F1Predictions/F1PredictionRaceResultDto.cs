namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionRaceResultDto
{
    public Guid RaceId { get; set; }
    public string[] Classification { get; set; }
    public string[] DnfDrivers { get; set; }
    public int SafetyCars { get; set; }
    public decimal FirstPlaceLead { get; set; }
}