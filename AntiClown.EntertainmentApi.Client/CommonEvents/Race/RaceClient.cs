using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Race;

public class RaceClient : IRaceClient
{
    public RaceClient(RestClient restClient)
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
        var response = await restClient.PatchAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task FinishAsync(Guid eventId)
    {
        var request = new RestRequest($"{ControllerUrl}/{eventId}/finish");
        var response = await restClient.PatchAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public IRaceDriversClient Drivers { get; set; }
    public IRaceTracksClient Tracks { get; set; }
    
    private const string ControllerUrl = "events/common/race";
    private readonly RestClient restClient;
}