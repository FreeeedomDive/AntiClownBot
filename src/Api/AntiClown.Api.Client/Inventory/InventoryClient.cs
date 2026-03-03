/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Inventory;

public class InventoryClient : IInventoryClient
{
    public InventoryClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Inventories.InventoryDto> ReadAllAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/inventories/{userId}/items", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Inventories.InventoryDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Inventories.BaseItemDto> ReadAsync(System.Guid userId, System.Guid itemId)
    {
        var requestBuilder = new RequestBuilder($"api/inventories/{userId}/items/{itemId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Inventories.BaseItemDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Inventories.LootBoxRewardDto> OpenLootBoxAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"api/inventories/{userId}/lootBoxes/open", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Inventories.LootBoxRewardDto>(requestBuilder.Build());
    }

    public async Task ChangeItemActiveStatusAsync(System.Guid userId, System.Guid itemId, bool isActive)
    {
        var requestBuilder = new RequestBuilder($"api/inventories/{userId}/items/{itemId}/active/{isActive}", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task SellAsync(System.Guid userId, System.Guid itemId)
    {
        var requestBuilder = new RequestBuilder($"api/inventories/{userId}/items/{itemId}/sell", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
