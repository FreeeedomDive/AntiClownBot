using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Transfusion;

public class TransfusionEventClient : ITransfusionEventClient
{
    public TransfusionEventClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<TransfusionEventDto> ReadAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<TransfusionEventDto>();
    }

    public async Task<Guid> StartNewAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/start");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    private readonly RestClient restClient;

    private const string ControllerUrl = "events/common/transfusion";
}