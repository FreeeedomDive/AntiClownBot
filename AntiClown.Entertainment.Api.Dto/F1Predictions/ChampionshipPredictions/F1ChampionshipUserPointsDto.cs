namespace AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;

public class F1ChampionshipUserPointsDto
{
    public Guid UserId { get; set; }
    public int[] PreSeasonPoints { get; set; } = [];
    public int[] MidSeasonPoints { get; set; } = [];
}
