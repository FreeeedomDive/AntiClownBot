/* Generated file */
namespace AntiClown.Entertainment.Api.Client.RaceEvent;

public interface IRaceEventClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceEventDto> ReadAsync(System.Guid eventId);
    System.Threading.Tasks.Task<System.Guid> StartNewAsync();
    System.Threading.Tasks.Task AddParticipantAsync(System.Guid eventId, System.Guid userId);
    System.Threading.Tasks.Task FinishAsync(System.Guid eventId);
}
