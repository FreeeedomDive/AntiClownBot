using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.DailyEvents.Announce;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.DailyEvents.Announce;

public class AnnounceClient : IAnnounceClient
{
    public AnnounceClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<AnnounceEventDto> ReadAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<AnnounceEventDto>();
    }

    public async Task<Guid> StartNewAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/start");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    private const string ControllerUrl = "events/daily/announce";
    private readonly RestClient restClient;
}