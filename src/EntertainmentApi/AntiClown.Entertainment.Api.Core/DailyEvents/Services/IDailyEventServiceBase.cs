using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services;

public interface IDailyEventServiceBase<T> where T : DailyEventBase
{
    Task<T> ReadAsync(Guid eventId);
    Task<Guid> StartNewEventAsync();
}