using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Transfusion;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Transfusion;

public class TransfusionClient : ITransfusionClient
{
    public TransfusionClient(RestClient restClient)
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

    private const string ControllerUrl = "events/common/transfusion";
    private readonly RestClient restClient;
}