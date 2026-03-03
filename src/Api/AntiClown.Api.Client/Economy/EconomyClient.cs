/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Economy;

public class EconomyClient : IEconomyClient
{
    public EconomyClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Economies.EconomyDto> ReadAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Economies.EconomyDto>(requestBuilder.Build());
    }

    public async Task UpdateScamCoinsAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateScamCoinsDto updateScamCoinsDto)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}/scamCoins", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(updateScamCoinsDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task UpdateScamCoinsForAllAsync(int scamCoinsDiff, string reason)
    {
        var requestBuilder = new RequestBuilder($"api/economy/scamCoins/updateForAll", HttpRequestMethod.POST);
        requestBuilder.WithQueryParameter("scamCoinsDiff", scamCoinsDiff);
        requestBuilder.WithQueryParameter("reason", reason);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task UpdateLootBoxesAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateLootBoxesDto lootBoxesDto)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}/lootBoxes", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(lootBoxesDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task ResetAllCoolDownsAsync()
    {
        var requestBuilder = new RequestBuilder($"api/economy/resetAllCoolDowns", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
