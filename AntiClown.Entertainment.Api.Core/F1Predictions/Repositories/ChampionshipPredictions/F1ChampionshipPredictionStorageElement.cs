using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.ChampionshipPredictions;

[Index(nameof(Season))]
[Index(nameof(UserId))]
[Index(nameof(Season), nameof(UserId))]
public class F1ChampionshipPredictionStorageElement : SqlStorageElement
{
    public int Season { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; }
    public string[] DriverStandings { get; set; }
}
