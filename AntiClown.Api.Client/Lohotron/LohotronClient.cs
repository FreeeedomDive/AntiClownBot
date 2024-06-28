/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Lohotron;

public class LohotronClient : ILohotronClient
{
    public LohotronClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Economies.LohotronRewardDto> UseLohotronAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}/lohotron", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Economies.LohotronRewardDto>(requestBuilder.Build());
    }

    public async Task ResetAsync()
    {
        var requestBuilder = new RequestBuilder($"api/economy/lohotron/reset", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
