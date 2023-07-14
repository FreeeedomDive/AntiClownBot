using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Race;

public interface IRaceEventClient
{
    Task<RaceEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
    Task AddParticipantAsync(Guid eventId, Guid userId);
    Task FinishAsync(Guid eventId);

    IRaceDriversClient Drivers { get; set; }
    IRaceTracksClient Tracks { get; set; }
}