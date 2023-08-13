using AntiClown.Entertainment.Api.Dto.DailyEvents;

namespace AntiClown.Entertainment.Api.Client.DailyEvents.ActiveEventsIndex;

public interface IActiveDailyEventsIndexClient
{
    Task<Dictionary<DailyEventTypeDto, bool>> ReadAllEventTypesAsync();
    Task<DailyEventTypeDto[]> ReadActiveEventsAsync();
    Task CreateAsync(DailyEventTypeDto eventType, bool isActiveByDefault);
    Task UpdateAsync(DailyEventTypeDto eventType, bool isActive);
    Task ActualizeIndexAsync();
}