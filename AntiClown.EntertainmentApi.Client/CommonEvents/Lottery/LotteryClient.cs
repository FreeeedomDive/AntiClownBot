using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Lottery;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Lottery;

public class LotteryClient : ILotteryClient
{
    public LotteryClient(RestClient restClient)
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
        var response = await restClient.PatchAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task FinishAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}/finish");
        var response = await restClient.PatchAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private const string ControllerUrl = "events/common/lottery";
    private readonly RestClient restClient;
}