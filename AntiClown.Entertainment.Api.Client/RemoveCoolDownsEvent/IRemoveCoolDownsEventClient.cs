/* Generated file */
namespace AntiClown.Entertainment.Api.Client.RemoveCoolDownsEvent;

public interface IRemoveCoolDownsEventClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns.RemoveCoolDownsEventDto> ReadAsync(System.Guid eventId);
    System.Threading.Tasks.Task<System.Guid> StartNewAsync();
}
