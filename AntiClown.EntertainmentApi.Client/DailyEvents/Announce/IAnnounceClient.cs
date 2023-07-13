using AntiClown.EntertainmentApi.Dto.DailyEvents.Announce;

namespace AntiClown.EntertainmentApi.Client.DailyEvents.Announce;

public interface IAnnounceClient
{
    Task<AnnounceEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}