using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services;

public interface ICommonEventServiceBase<T> where T : CommonEventBase
{
    Task<T> ReadAsync(Guid eventId);
    Task<Guid> StartNewEventAsync();
}