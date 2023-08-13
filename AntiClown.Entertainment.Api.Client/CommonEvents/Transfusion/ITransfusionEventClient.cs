using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Transfusion;

public interface ITransfusionEventClient
{
    Task<TransfusionEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}