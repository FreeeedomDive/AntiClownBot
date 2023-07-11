using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.ActiveEventsIndex;

public interface IActiveEventsIndexService
{
    Task<Dictionary<CommonEventType, bool>> ReadAllEventTypesAsync();
    Task<CommonEventType[]> ReadActiveEventsAsync();
    Task CreateAsync(CommonEventType eventType, bool isActiveByDefault);
    Task UpdateAsync(CommonEventType eventType, bool isActive);
    Task ActualizeIndexAsync();
}