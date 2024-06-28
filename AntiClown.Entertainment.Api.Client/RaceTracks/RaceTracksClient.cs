/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.RaceTracks;

public class RaceTracksClient : IRaceTracksClient
{
    public RaceTracksClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto[]> ReadAllAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/tracks/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto[]>(requestBuilder.Build());
    }

    public async Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceTrackDto raceTrack)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/tracks/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(raceTrack);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
