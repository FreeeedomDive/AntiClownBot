using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;

[Index(nameof(EventType))]
public class ActiveEventsIndexStorageElement : SqlStorageElement
{
    public string EventType { get; set; }
    public bool IsActive { get; set; }
}