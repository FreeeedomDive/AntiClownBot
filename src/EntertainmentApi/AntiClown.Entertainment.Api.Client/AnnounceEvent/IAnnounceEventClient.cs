/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.AnnounceEvent;

public interface IAnnounceEventClient
{
    Task<AntiClown.Entertainment.Api.Dto.DailyEvents.Announce.AnnounceEventDto> ReadAsync(System.Guid eventId);
    Task<System.Guid> StartNewAsync();
}
