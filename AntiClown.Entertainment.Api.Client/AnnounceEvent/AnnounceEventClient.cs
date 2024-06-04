/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.AnnounceEvent;

public class AnnounceEventClient : IAnnounceEventClient
{
    public AnnounceEventClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.DailyEvents.Announce.AnnounceEventDto> ReadAsync(System.Guid eventId)
    {
        var request = new RestRequest("entertainmentApi/events/daily/announce/{eventId}", Method.Get);
        request.AddUrlSegment("eventId", eventId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.DailyEvents.Announce.AnnounceEventDto>();
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewAsync()
    {
        var request = new RestRequest("entertainmentApi/events/daily/announce/start", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    private readonly RestSharp.RestClient restClient;
}
