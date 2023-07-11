using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Race;

public class RaceDriversClient : IRaceDriversClient
{
    public RaceDriversClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<RaceDriverDto[]> ReadAllAsync()
    {
        var request = new RestRequest(ControllerUrl);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RaceDriverDto[]>();
    }

    public async Task<RaceDriverDto> FindAsync(string name)
    {
        var request = new RestRequest($"{ControllerUrl}/{name}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RaceDriverDto>();
    }

    public async Task CreateAsync(RaceDriverDto raceDriver)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(raceDriver);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task UpdateAsync(RaceDriverDto raceDriver)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(raceDriver);
        var response = await restClient.PatchAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private const string ControllerUrl = "events/common/race/drivers";
    private readonly RestClient restClient;
}