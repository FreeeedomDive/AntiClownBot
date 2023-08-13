using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;

namespace AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;

public interface IRaceDriversRepository
{
    Task<RaceDriver[]> ReadAllAsync();
    Task<RaceDriver> FindAsync(string name);
    Task CreateAsync(RaceDriver raceDriver);
    Task UpdateAsync(RaceDriver raceDriver);
}