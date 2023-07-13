using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;

public interface IDailyActiveEventsIndexRepository
{
    Task<Dictionary<DailyEventType, bool>> ReadAllEventTypesAsync();
    Task CreateAsync(DailyEventType eventType, bool isActiveByDefault);
    Task UpdateAsync(DailyEventType eventType, bool isActive);
}