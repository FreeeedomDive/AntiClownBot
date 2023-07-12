using AntiClown.Api.Client.Extensions;
using AntiClown.Api.Dto.Inventories;
using AntiClown.Api.Dto.Shops;
using RestSharp;

namespace AntiClown.Api.Client.Shop;

public class ShopClient : IShopClient
{
    public ShopClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<CurrentShopInfoDto> ReadAsync(Guid shopId)
    {
        var request = new RestRequest(BuildApiUrl(shopId));
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<CurrentShopInfoDto>();
    }

    public async Task<ShopItemDto> RevealItemAsync(Guid shopId, Guid itemId)
    {
        var request = new RestRequest($"{BuildApiUrl(shopId)}/items/{itemId}/reveal");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<ShopItemDto>();
    }

    public async Task<BaseItemDto> BuyItemAsync(Guid shopId, Guid itemId)
    {
        var request = new RestRequest($"{BuildApiUrl(shopId)}/items/{itemId}/buy");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<BaseItemDto>();
    }

    public async Task ReRollShopAsync(Guid shopId)
    {
        var request = new RestRequest($"{BuildApiUrl(shopId)}/reroll");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task ResetShopAsync(Guid shopId)
    {
        var request = new RestRequest($"{BuildApiUrl(shopId)}/reset");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private static string BuildApiUrl(Guid shopId) => $"shops/{shopId}";

    private readonly RestClient restClient;
}