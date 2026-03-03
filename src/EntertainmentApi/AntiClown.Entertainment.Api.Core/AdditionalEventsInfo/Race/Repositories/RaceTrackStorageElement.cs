using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;

[Index(nameof(Name))]
public class RaceTrackStorageElement : SqlStorageElement
{
    public string Name { get; set; }
    public int IdealTime { get; set; }
    public int CorneringDifficulty { get; set; }
    public int AccelerationDifficulty { get; set; }
    public int BreakingDifficulty { get; set; }
}