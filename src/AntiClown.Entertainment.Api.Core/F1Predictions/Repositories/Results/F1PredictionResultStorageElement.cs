using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;

[Index(nameof(RaceId))]
[Index(nameof(UserId))]
public class F1PredictionResultStorageElement : SqlStorageElement
{
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }
    public int TenthPlacePoints { get; set; }
    public int DnfPoints { get; set; }
    public int SafetyCarsPoints { get; set; }
    public int FirstPlaceLeadPoints { get; set; }
    [Obsolete("2024-2025")] public int TeamMatesPoints { get; set; }
    public int DriverPositionPoints { get; set; }
    public int TotalPoints { get; set; }
}