namespace AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;

public class F1ChampionshipPredictionDto
{
    public int Season { get; set; }
    public Guid UserId { get; set; }
    public string[]? PreSeasonStandings { get; set; }
    public string[]? MidSeasonStandings { get; set; }
}
