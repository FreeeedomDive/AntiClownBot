using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Lottery;

public class LotteryEventClient : ILotteryEventClient
{
    public LotteryEventClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<LotteryEventDto> ReadAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<LotteryEventDto>();
    }

    public async Task<Guid> StartNewAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/start");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    public async Task AddParticipantAsync(Guid eventId, Guid userId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}/addParticipant").AddQueryParameter("userId", userId);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task FinishAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}/finish");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;

    private const string ControllerUrl = "events/common/lottery";
}