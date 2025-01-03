using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

[Index(nameof(Name))]
public class F1PredictionTeamStorageElement : SqlStorageElement
{
    public string Name { get; set; }
    public string FirstDriver { get; set; }
    public string SecondDriver { get; set; }
}