using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.ChampionshipPredictions;

[Index(nameof(Season))]
public class F1ChampionshipResultsStorageElement : SqlStorageElement
{
    public int Season { get; set; }
    public string Data { get; set; }
}
