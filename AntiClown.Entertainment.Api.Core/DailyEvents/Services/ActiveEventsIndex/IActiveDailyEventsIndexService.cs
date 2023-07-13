using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.ActiveEventsIndex;

public interface IActiveDailyEventsIndexService
{
    Task<Dictionary<DailyEventType, bool>> ReadAllEventTypesAsync();
    Task<DailyEventType[]> ReadActiveEventsAsync();
    Task CreateAsync(DailyEventType eventType, bool isActiveByDefault);
    Task UpdateAsync(DailyEventType eventType, bool isActive);
    Task ActualizeIndexAsync();
}