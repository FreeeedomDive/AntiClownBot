/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.RaceDrivers;

public interface IRaceDriversClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto[]> ReadAllAsync();
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto> FindAsync(string name);
    Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver);
    Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver);
}
