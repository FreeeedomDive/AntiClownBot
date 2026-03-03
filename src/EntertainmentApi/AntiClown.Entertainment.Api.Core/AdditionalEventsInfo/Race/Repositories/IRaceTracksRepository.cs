using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;

namespace AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;

public interface IRaceTracksRepository
{
    Task<RaceTrack[]> ReadAllAsync();
    Task CreateAsync(RaceTrack raceTrack);
}