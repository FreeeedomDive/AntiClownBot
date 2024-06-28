/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.RaceEvent;

public class RaceEventClient : IRaceEventClient
{
    public RaceEventClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceEventDto> ReadAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/{eventId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceEventDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> StartNewAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/start", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    public async Task AddParticipantAsync(System.Guid eventId, System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/{eventId}/addParticipant", HttpRequestMethod.POST);
        requestBuilder.WithQueryParameter("userId", userId);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task FinishAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/{eventId}/finish", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
