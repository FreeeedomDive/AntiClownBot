using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

public class F1PredictionRaceResult
{
    public Guid RaceId { get; set; }
    public string[] Classification { get; set; }
    public string[] DnfDrivers { get; set; }
    public int SafetyCars { get; set; }
    public decimal FirstPlaceLead { get; set; }
}