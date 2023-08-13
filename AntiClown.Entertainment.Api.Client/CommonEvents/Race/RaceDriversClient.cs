using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Race;

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

    private readonly RestClient restClient;

    private const string ControllerUrl = "events/common/race/drivers";
}