namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

public class F1PredictionResult
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int DnfsPoints { get; set; }
    public int SafetyCarsPoints { get; set; }
    public int FirstPlaceLeadPoints { get; set; }
    public int TeamMatesPoints { get; set; }
}