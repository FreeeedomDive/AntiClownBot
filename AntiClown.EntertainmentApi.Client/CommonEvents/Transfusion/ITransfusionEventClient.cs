using AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Transfusion;

public interface ITransfusionEventClient
{
    Task<TransfusionEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}