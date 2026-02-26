namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;

public record F1ChampionshipUserPoints
{
    public Guid UserId { get; set; }
    public int[] PreSeasonPoints { get; set; } = [];
    public int[] MidSeasonPoints { get; set; } = [];
}
