using AntiClown.EntertainmentApi.Dto.CommonEvents;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.ActiveEventsIndex;

public interface IActiveEventsIndexClient
{
    Task<Dictionary<CommonEventTypeDto, bool>> ReadAllEventTypesAsync();
    Task<CommonEventTypeDto[]> ReadActiveEventsAsync();
    Task CreateAsync(CommonEventTypeDto eventType, bool isActiveByDefault);
    Task UpdateAsync(CommonEventTypeDto eventType, bool isActive);
    Task ActualizeIndexAsync();
}