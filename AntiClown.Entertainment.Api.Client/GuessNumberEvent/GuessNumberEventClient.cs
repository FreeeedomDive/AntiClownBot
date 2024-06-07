/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.GuessNumberEvent;

public class GuessNumberEventClient : IGuessNumberEventClient
{
    public GuessNumberEventClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberEventDto> ReadAsync(System.Guid eventId)
    {
        var request = new RestRequest("entertainmentApi/events/common/guessNumber/{eventId}", Method.Get);
        request.AddUrlSegment("eventId", eventId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberEventDto>();
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/guessNumber/start", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    public async System.Threading.Tasks.Task AddPickAsync(System.Guid eventId, AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberUserPickDto guessNumberUserPickDto)
    {
        var request = new RestRequest("entertainmentApi/events/common/guessNumber/{eventId}/addPick", Method.Post);
        request.AddUrlSegment("eventId", eventId);
        request.AddJsonBody(guessNumberUserPickDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task FinishAsync(System.Guid eventId)
    {
        var request = new RestRequest("entertainmentApi/events/common/guessNumber/{eventId}/finish", Method.Post);
        request.AddUrlSegment("eventId", eventId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
