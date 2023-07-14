using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.GuessNumber;

public class GuessNumberEventClient : IGuessNumberEventClient
{
    public GuessNumberEventClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<GuessNumberEventDto> ReadAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<GuessNumberEventDto>();
    }

    public async Task<Guid> StartNewAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/start");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    public async Task AddPickAsync(Guid eventId, Guid userId, GuessNumberPickDto pick)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}/addPick");
        request.AddJsonBody(
            new GuessNumberUserPickDto
            {
                UserId = userId,
                Pick = pick,
            }
        );
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

    private const string ControllerUrl = "events/common/guessNumber";
}