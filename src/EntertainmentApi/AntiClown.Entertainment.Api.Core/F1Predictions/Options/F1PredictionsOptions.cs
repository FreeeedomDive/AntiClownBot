namespace AntiClown.Entertainment.Api.Core.F1Predictions.Options;

public class F1PredictionsOptions
{
    public TimeSpan ChampionshipPollingInterval { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan QualifyingGridPollingInterval { get; set; } = TimeSpan.FromMinutes(30);
}
