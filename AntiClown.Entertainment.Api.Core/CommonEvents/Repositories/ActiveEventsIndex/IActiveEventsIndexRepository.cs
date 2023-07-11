using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;

public interface IActiveEventsIndexRepository
{
    Task<Dictionary<CommonEventType, bool>> ReadAllEventTypesAsync();
    Task CreateAsync(CommonEventType eventType, bool isActiveByDefault);
    Task UpdateAsync(CommonEventType eventType, bool isActive);
}