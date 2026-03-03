namespace AntiClown.Web.Api.Dto.F1Predictions;

public record F1StandingsDto
{
    public F1StandingsRowDto[] Standings { get; set; }
    public int CurrentLeaderPoints { get; set; }
    public int PointsLeft { get; set; }
}