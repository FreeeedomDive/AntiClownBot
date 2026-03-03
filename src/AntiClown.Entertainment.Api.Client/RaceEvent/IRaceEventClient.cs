/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.RaceEvent;

public interface IRaceEventClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceEventDto> ReadAsync(System.Guid eventId);
    Task<System.Guid> StartNewAsync();
    Task AddParticipantAsync(System.Guid eventId, System.Guid userId);
    Task FinishAsync(System.Guid eventId);
}
