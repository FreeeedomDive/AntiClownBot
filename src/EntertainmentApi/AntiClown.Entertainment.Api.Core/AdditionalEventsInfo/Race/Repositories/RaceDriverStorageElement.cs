using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;

[Index(nameof(DriverName))]
public class RaceDriverStorageElement : SqlStorageElement
{
    public string DriverName { get; set; }
    public int Points { get; set; }
    public double CorneringSkill { get; set; }
    public double AccelerationSkill { get; set; }
    public double BreakingSkill { get; set; }
}