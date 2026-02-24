namespace AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

public record F1PredictionResult
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int DnfsPoints { get; set; }
    public int SafetyCarsPoints { get; set; }
    public int FirstPlaceLeadPoints { get; set; }
    [Obsolete("2024-2025")] public int TeamMatesPoints { get; set; }
    public int DriverPositionPoints { get; set; }
    public int TotalPoints { get; set; }
}