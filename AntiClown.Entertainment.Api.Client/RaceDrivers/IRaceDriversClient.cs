/* Generated file */
namespace AntiClown.Entertainment.Api.Client.RaceDrivers;

public interface IRaceDriversClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto[]> ReadAllAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto> FindAsync(System.String name);
    System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver);
    System.Threading.Tasks.Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver);
}
