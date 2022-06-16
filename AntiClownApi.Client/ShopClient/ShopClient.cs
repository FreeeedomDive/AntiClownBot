using AntiClownApiClient.Dto.Responses.ShopResponses;
using AntiClownApiClient.Extensions;
using Loggers;
using RestSharp;

namespace AntiClownApiClient.ShopClient;

public class ShopClient : IShopClient
{
    public ShopClient(RestClient restClient, ILogger logger)
    {
        this.restClient = restClient;
        this.logger = logger;
    }

    public async Task<UserShopResponseDto> GetAsync(ulong userId)
    {
        var request = new RestRequest($"api/shop/{userId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<UserShopResponseDto>();
    }

    public async Task<ItemIdInSlotResponseDto> ItemIdInSlotAsync(ulong userId, int slot)
    {
        var request = new RestRequest($"api/shop/{userId}/ExecuteGetAsyncItemIdInSlot/{slot}");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<ItemIdInSlotResponseDto>();
    }

    public async Task<ShopItemRevealResponseDto> ItemRevealAsync(ulong userId, Guid itemId)
    {
        var request = new RestRequest($"api/shop/{userId}/reveal/{itemId}");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<ShopItemRevealResponseDto>();
    }

    public async Task<BuyItemResponseDto> BuyAsync(ulong userId, Guid itemId)
    {
        var request = new RestRequest($"api/shop/{userId}/buy/{itemId}");
        var response = await restClient.ExecutePostAsync(request);
        logger.Info(response.Content!);
        return response.TryDeserialize<BuyItemResponseDto>();
    }

    public async Task<ReRollResponseDto> ReRollAsync(ulong userId)
    {
        var request = new RestRequest($"api/shop/{userId}/reroll");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<ReRollResponseDto>();
    }

    private readonly RestClient restClient;
    private readonly ILogger logger;
}