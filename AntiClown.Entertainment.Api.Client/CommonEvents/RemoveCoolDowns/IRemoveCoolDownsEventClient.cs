using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.RemoveCoolDowns;

public interface IRemoveCoolDownsEventClient
{
    Task<RemoveCoolDownsEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
}