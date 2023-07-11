using AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Transfusion;

public interface ITransfusionClient
{
    Task<TransfusionEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}