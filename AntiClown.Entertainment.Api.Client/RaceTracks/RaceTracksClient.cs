/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.RaceTracks;

public class RaceTracksClient : IRaceTracksClient
{
    public RaceTracksClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto[]> ReadAllAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/race/tracks/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto[]>();
    }

    public async System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto raceTrack)
    {
        var request = new RestRequest("entertainmentApi/events/common/race/tracks/", Method.Post);
        request.AddJsonBody(raceTrack);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
