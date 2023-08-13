using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;

public interface ICommonEventsRepository
{
    Task<CommonEventBase> ReadAsync(Guid id);
    Task CreateAsync(CommonEventBase commonEvent);
    Task UpdateAsync(CommonEventBase commonEvent);
}