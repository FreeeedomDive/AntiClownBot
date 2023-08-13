using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Race;

public interface IRaceDriversClient
{
    Task<RaceDriverDto[]> ReadAllAsync();
    Task<RaceDriverDto> FindAsync(string name);
    Task CreateAsync(RaceDriverDto raceDriver);
    Task UpdateAsync(RaceDriverDto raceDriver);
}