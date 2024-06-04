/* Generated file */
namespace AntiClown.Entertainment.Api.Client.TransfusionEvent;

public interface ITransfusionEventClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion.TransfusionEventDto> ReadAsync(System.Guid eventId);
    System.Threading.Tasks.Task<System.Guid> StartNewAsync();
}
