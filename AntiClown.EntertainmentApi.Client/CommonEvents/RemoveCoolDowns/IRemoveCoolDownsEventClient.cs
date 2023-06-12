using AntiClown.EntertainmentApi.Dto.CommonEvents.RemoveCoolDowns;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.RemoveCoolDowns;

public interface IRemoveCoolDownsEventClient
{
    Task<RemoveCoolDownsEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}