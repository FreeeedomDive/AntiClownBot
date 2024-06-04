/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.TransfusionEvent;

public class TransfusionEventClient : ITransfusionEventClient
{
    public TransfusionEventClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion.TransfusionEventDto> ReadAsync(System.Guid eventId)
    {
        var request = new RestRequest("entertainmentApi/events/common/transfusion/{eventId}", Method.Get);
        request.AddUrlSegment("eventId", eventId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion.TransfusionEventDto>();
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/transfusion/start", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    private readonly RestSharp.RestClient restClient;
}
