using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses.UserCommandResponses;
using AntiClownApiClient.Extensions;
using RestSharp;

namespace AntiClownApiClient.ItemsClient;

public class ItemsClient : IItemsClient
{
    public ItemsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<List<BaseItem>> AllItemsAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/items");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<List<BaseItem>>();
    }

    public async Task<SetActiveStatusForItemResponseDto> SetActiveStatusForItemAsync(ulong userId, Guid itemId, bool isActive)
    {
        var request = new RestRequest($"api/users/{userId}/items/{itemId}/active/{isActive}");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<SetActiveStatusForItemResponseDto>();
    }

    public async Task<SellItemResponseDto> SellItemAsync(ulong userId, Guid itemId)
    {
        var request = new RestRequest($"api/users/{userId}/items/{itemId}/sell");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<SellItemResponseDto>();
    }

    public async Task<OpenLootBoxResultDto> OpenLootBoxAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/items/lootbox/open");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<OpenLootBoxResultDto>();
    }

    public async Task AddLootBoxAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/items/lootbox/add");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task RemoveLootBoxAsync(ulong userId)
    {
        var request = new RestRequest($"api/users/{userId}/items/lootbox/remove");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
}