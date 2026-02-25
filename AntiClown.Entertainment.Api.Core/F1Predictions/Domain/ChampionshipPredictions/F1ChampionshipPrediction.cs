namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;

public class F1ChampionshipPrediction
{
    public int Season { get; set; }
    public Guid UserId { get; set; }
    public string[]? PreSeasonStandings { get; set; }
    public string[]? MidSeasonStandings { get; set; }
}
