using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

public class F1PredictionRaceResult
{
    public Guid RaceId { get; set; }
    public F1Driver[] Classification { get; set; }
    public F1Driver[] DnfDrivers { get; set; }
    public int SafetyCars { get; set; }
    public decimal FirstPlaceLead { get; set; }
}