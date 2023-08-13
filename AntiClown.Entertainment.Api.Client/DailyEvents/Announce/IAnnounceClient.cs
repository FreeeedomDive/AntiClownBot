using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;

namespace AntiClown.Entertainment.Api.Client.DailyEvents.Announce;

public interface IAnnounceClient
{
    Task<AnnounceEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}