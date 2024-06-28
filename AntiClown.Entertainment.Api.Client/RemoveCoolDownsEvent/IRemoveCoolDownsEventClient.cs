/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.RemoveCoolDownsEvent;

public interface IRemoveCoolDownsEventClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns.RemoveCoolDownsEventDto> ReadAsync(System.Guid eventId);
    Task<System.Guid> StartNewAsync();
}
