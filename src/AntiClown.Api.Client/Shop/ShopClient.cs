/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Shop;

public class ShopClient : IShopClient
{
    public ShopClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Shops.CurrentShopInfoDto> ReadAsync(System.Guid shopId)
    {
        var requestBuilder = new RequestBuilder($"api/shops/{shopId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Shops.CurrentShopInfoDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Shops.ShopItemDto> RevealAsync(System.Guid shopId, System.Guid itemId)
    {
        var requestBuilder = new RequestBuilder($"api/shops/{shopId}/items/{itemId}/reveal", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Shops.ShopItemDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Inventories.BaseItemDto> BuyAsync(System.Guid shopId, System.Guid itemId)
    {
        var requestBuilder = new RequestBuilder($"api/shops/{shopId}/items/{itemId}/buy", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Inventories.BaseItemDto>(requestBuilder.Build());
    }

    public async Task ReRollAsync(System.Guid shopId)
    {
        var requestBuilder = new RequestBuilder($"api/shops/{shopId}/reroll", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task ResetAsync(System.Guid shopId)
    {
        var requestBuilder = new RequestBuilder($"api/shops/{shopId}/reset", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Shops.ShopStatsDto> ReadStatsAsync(System.Guid shopId)
    {
        var requestBuilder = new RequestBuilder($"api/shops/{shopId}/stats", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Shops.ShopStatsDto>(requestBuilder.Build());
    }

    public async Task ResetAllAsync()
    {
        var requestBuilder = new RequestBuilder($"api/shops/resetAll", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
