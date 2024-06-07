/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Api.Client.Inventory;

public class InventoryClient : IInventoryClient
{
    public InventoryClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.InventoryDto> ReadAllAsync(System.Guid userId)
    {
        var request = new RestRequest("api/inventories/{userId}/items", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Inventories.InventoryDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.BaseItemDto> ReadAsync(System.Guid userId, System.Guid itemId)
    {
        var request = new RestRequest("api/inventories/{userId}/items/{itemId}", Method.Get);
        request.AddUrlSegment("userId", userId);
        request.AddUrlSegment("itemId", itemId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Inventories.BaseItemDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.LootBoxRewardDto> OpenLootBoxAsync(System.Guid userId)
    {
        var request = new RestRequest("api/inventories/{userId}/lootBoxes/open", Method.Post);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Inventories.LootBoxRewardDto>();
    }

    public async System.Threading.Tasks.Task ChangeItemActiveStatusAsync(System.Guid userId, System.Guid itemId, System.Boolean isActive)
    {
        var request = new RestRequest("api/inventories/{userId}/items/{itemId}/active/{isActive}", Method.Post);
        request.AddUrlSegment("userId", userId);
        request.AddUrlSegment("itemId", itemId);
        request.AddUrlSegment("isActive", isActive);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task SellAsync(System.Guid userId, System.Guid itemId)
    {
        var request = new RestRequest("api/inventories/{userId}/items/{itemId}/sell", Method.Post);
        request.AddUrlSegment("userId", userId);
        request.AddUrlSegment("itemId", itemId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
