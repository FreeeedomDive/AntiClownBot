using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Race;

public class RaceEventClient : IRaceEventClient
{
    public RaceEventClient(RestClient restClient)
    {
        this.restClient = restClient;
        Drivers = new RaceDriversClient(restClient);
        Tracks = new RaceTracksClient(restClient);
    }

    public async Task<RaceEventDto> ReadAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RaceEventDto>();
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

    public IRaceDriversClient Drivers { get; set; }
    public IRaceTracksClient Tracks { get; set; }
    private readonly RestClient restClient;

    private const string ControllerUrl = "events/common/race";
}