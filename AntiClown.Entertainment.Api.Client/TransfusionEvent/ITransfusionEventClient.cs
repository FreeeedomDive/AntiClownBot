/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.TransfusionEvent;

public interface ITransfusionEventClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion.TransfusionEventDto> ReadAsync(System.Guid eventId);
    Task<System.Guid> StartNewAsync();
}
