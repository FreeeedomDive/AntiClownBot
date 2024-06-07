/* Generated file */
namespace AntiClown.Entertainment.Api.Client.AnnounceEvent;

public interface IAnnounceEventClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.DailyEvents.Announce.AnnounceEventDto> ReadAsync(System.Guid eventId);
    System.Threading.Tasks.Task<System.Guid> StartNewAsync();
}
