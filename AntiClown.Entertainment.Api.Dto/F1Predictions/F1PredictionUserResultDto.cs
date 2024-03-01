namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionUserResultDto
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int FirstDnfPoints { get; set; }
}