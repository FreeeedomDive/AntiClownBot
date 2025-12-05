using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Web.Api.Dto.F1Predictions;

public record F1StandingsRowDto
{
    public Guid UserId { get; set; }
    public int TotalPoints { get; set; }
    public F1PredictionUserResultDto?[] Results { get; set; }
}