/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.RaceTracks;

public interface IRaceTracksClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto[]> ReadAllAsync();
    Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto raceTrack);
}
