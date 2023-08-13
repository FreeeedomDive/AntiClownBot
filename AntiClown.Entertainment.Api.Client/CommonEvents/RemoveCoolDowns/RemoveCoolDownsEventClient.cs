using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.RemoveCoolDowns;

public class RemoveCoolDownsEventClient : IRemoveCoolDownsEventClient
{
    public RemoveCoolDownsEventClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<RemoveCoolDownsEventDto> ReadAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RemoveCoolDownsEventDto>();
    }

    public async Task<Guid> StartNewAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/start");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    private readonly RestClient restClient;

    private const string ControllerUrl = "events/common/removeCoolDowns";
}