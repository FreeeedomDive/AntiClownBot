/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.AnnounceEvent;

public class AnnounceEventClient : IAnnounceEventClient
{
    public AnnounceEventClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.DailyEvents.Announce.AnnounceEventDto> ReadAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/announce/{eventId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.DailyEvents.Announce.AnnounceEventDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> StartNewAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/announce/start", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
