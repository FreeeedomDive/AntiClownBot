using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;

public interface IDailyEventsRepository
{
    Task<DailyEventBase> ReadAsync(Guid id);
    Task CreateAsync(DailyEventBase commonEvent);
}