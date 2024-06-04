/* Generated file */
using RestSharp;
using AntiClown.Api.Client.Extensions;

namespace AntiClown.Api.Client.Shop;

public class ShopClient : IShopClient
{
    public ShopClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Shops.CurrentShopInfoDto> ReadAsync(System.Guid shopId)
    {
        var request = new RestRequest("api/shops/{shopId}", Method.Get);
        request.AddUrlSegment("shopId", shopId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Shops.CurrentShopInfoDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Shops.ShopItemDto> RevealAsync(System.Guid shopId, System.Guid itemId)
    {
        var request = new RestRequest("api/shops/{shopId}/items/{itemId}/reveal", Method.Post);
        request.AddUrlSegment("shopId", shopId);
        request.AddUrlSegment("itemId", itemId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Shops.ShopItemDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.BaseItemDto> BuyAsync(System.Guid shopId, System.Guid itemId)
    {
        var request = new RestRequest("api/shops/{shopId}/items/{itemId}/buy", Method.Post);
        request.AddUrlSegment("shopId", shopId);
        request.AddUrlSegment("itemId", itemId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Inventories.BaseItemDto>();
    }

    public async System.Threading.Tasks.Task ReRollAsync(System.Guid shopId)
    {
        var request = new RestRequest("api/shops/{shopId}/reroll", Method.Post);
        request.AddUrlSegment("shopId", shopId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task ResetAsync(System.Guid shopId)
    {
        var request = new RestRequest("api/shops/{shopId}/reset", Method.Post);
        request.AddUrlSegment("shopId", shopId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Shops.ShopStatsDto> ReadStatsAsync(System.Guid shopId)
    {
        var request = new RestRequest("api/shops/{shopId}/stats", Method.Get);
        request.AddUrlSegment("shopId", shopId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Shops.ShopStatsDto>();
    }

    public async System.Threading.Tasks.Task ResetAllAsync()
    {
        var request = new RestRequest("api/shops/resetAll", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
