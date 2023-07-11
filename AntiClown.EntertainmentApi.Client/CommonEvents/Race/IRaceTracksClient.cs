using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Race;

public interface IRaceTracksClient
{
    Task<RaceTrackDto[]> ReadAllAsync();
    Task CreateAsync(RaceTrackDto raceTrack);
}