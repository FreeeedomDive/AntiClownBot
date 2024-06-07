/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.RaceDrivers;

public class RaceDriversClient : IRaceDriversClient
{
    public RaceDriversClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto[]> ReadAllAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/race/drivers/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto> FindAsync(System.String name)
    {
        var request = new RestRequest("entertainmentApi/events/common/race/drivers/{name}", Method.Get);
        request.AddUrlSegment("name", name);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto>();
    }

    public async System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver)
    {
        var request = new RestRequest("entertainmentApi/events/common/race/drivers/", Method.Post);
        request.AddJsonBody(raceDriver);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver)
    {
        var request = new RestRequest("entertainmentApi/events/common/race/drivers/", Method.Patch);
        request.AddJsonBody(raceDriver);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
