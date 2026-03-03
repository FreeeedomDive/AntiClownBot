/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.RaceDrivers;

public class RaceDriversClient : IRaceDriversClient
{
    public RaceDriversClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto[]> ReadAllAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/drivers/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto> FindAsync(string name)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/drivers/{name}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto>(requestBuilder.Build());
    }

    public async Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/drivers/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(raceDriver);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.Race.RaceDriverDto raceDriver)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/race/drivers/", HttpRequestMethod.PATCH);
        requestBuilder.WithJsonBody(raceDriver);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
