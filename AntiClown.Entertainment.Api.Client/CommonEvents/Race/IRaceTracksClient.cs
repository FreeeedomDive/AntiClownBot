using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Race;

public interface IRaceTracksClient
{
    Task<RaceTrackDto[]> ReadAllAsync();
    Task CreateAsync(RaceTrackDto raceTrack);
}