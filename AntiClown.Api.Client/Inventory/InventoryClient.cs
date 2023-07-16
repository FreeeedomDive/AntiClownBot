using AntiClown.Api.Client.Extensions;
using AntiClown.Api.Dto.Inventories;
using RestSharp;

namespace AntiClown.Api.Client.Inventory;

public class InventoryClient : IInventoryClient
{
    public InventoryClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<InventoryDto> ReadAllItemsAsync(Guid userId)
    {
        var request = new RestRequest($"{BuildApiUrl(userId)}/items");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<InventoryDto>();
    }

    public async Task<BaseItemDto> ReadItemAsync(Guid userId, Guid itemId)
    {
        var request = new RestRequest($"{BuildApiUrl(userId)}/items/{itemId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<BaseItemDto>();
    }

    public async Task<LootBoxRewardDto> OpenLootBoxAsync(Guid userId)
    {
        var request = new RestRequest($"{BuildApiUrl(userId)}/lootBoxes/open");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<LootBoxRewardDto>();
    }

    public async Task ChangeItemActiveStatusAsync(Guid userId, Guid itemId, bool isActive)
    {
        var request = new RestRequest($"{BuildApiUrl(userId)}/items/{itemId}/active/{isActive}");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task SellAsync(Guid userId, Guid itemId)
    {
        var request = new RestRequest($"{BuildApiUrl(userId)}/items/{itemId}/sell");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private static string BuildApiUrl(Guid userId)
    {
        return $"inventories/{userId}";
    }

    private readonly RestClient restClient;
}