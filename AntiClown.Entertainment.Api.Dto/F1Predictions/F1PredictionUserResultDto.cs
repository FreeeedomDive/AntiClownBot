namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public class F1PredictionUserResultDto
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int DnfsPoints { get; set; }
    public int SafetyCarsPoints { get; set; }
    public int FirstPlaceLeadPoints { get; set; }
    public int TeamMatesPoints { get; set; }
    public int TotalPoints { get; set; }
}