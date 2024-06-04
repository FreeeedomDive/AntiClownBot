/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.LotteryEvent;

public class LotteryEventClient : ILotteryEventClient
{
    public LotteryEventClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery.LotteryEventDto> ReadAsync(System.Guid eventId)
    {
        var request = new RestRequest("entertainmentApi/events/common/lottery/{eventId}", Method.Get);
        request.AddUrlSegment("eventId", eventId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery.LotteryEventDto>();
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/lottery/start", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    public async System.Threading.Tasks.Task AddParticipantAsync(System.Guid eventId, System.Guid userId)
    {
        var request = new RestRequest("entertainmentApi/events/common/lottery/{eventId}/addParticipant", Method.Post);
        request.AddUrlSegment("eventId", eventId);
        request.AddQueryParameter("userId", userId.ToString());
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task FinishAsync(System.Guid eventId)
    {
        var request = new RestRequest("entertainmentApi/events/common/lottery/{eventId}/finish", Method.Post);
        request.AddUrlSegment("eventId", eventId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
