using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;

public class DailyEventStorageElement : SqlStorageElement
{
    public string Type { get; set; }
    public DateTime EventDateTime { get; set; }
    public string Details { get; set; }
}