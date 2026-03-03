namespace AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;

public record F1ChampionshipResultsDto
{
    public bool HasData { get; set; }
    public F1ChampionshipPredictionTypeDto Stage { get; set; }
    public bool IsOpen { get; set; }
    public string[] Standings { get; set; }
}
