using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;

[Index(nameof(EventType))]
public class DailyActiveEventsIndexStorageElement : SqlStorageElement
{
    public string EventType { get; set; }
    public bool IsActive { get; set; }
}