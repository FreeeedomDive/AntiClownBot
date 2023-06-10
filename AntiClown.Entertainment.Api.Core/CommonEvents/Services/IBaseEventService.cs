using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services;

public interface IBaseEventService<T> where T : CommonEventBase
{
    Task<Guid> StartNewEventAsync();
    Task<T> ReadAsync(Guid eventId);
}