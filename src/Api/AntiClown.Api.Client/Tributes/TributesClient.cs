/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Tributes;

public class TributesClient : ITributesClient
{
    public TributesClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Economies.NextTributeDto> WhenNextTributeAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}/tributes/when", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Economies.NextTributeDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Economies.TributeDto> TributeAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}/tributes/", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Economies.TributeDto>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
