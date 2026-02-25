namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;

public class F1ChampionshipResults
{
    public bool HasData { get; set; }
    public F1ChampionshipPredictionType Stage { get; set; }
    public bool IsOpen { get; set; }
    public string[] Standings { get; set; }
}
