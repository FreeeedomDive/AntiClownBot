/* Generated file */
namespace AntiClown.Entertainment.Api.Client.RaceTracks;

public interface IRaceTracksClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto[]> ReadAllAsync();
    System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto raceTrack);
}
