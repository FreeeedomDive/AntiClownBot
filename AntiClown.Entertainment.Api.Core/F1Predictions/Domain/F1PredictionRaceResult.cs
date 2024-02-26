namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

public class F1PredictionRaceResult
{
    public Guid RaceId { get; set; }
    public F1Driver[] Classification { get; set; }
    public F1Driver? FirstDnf { get; set; }
    public F1Driver[] DnfDrivers { get; set; }
}