using AntiClownApiClient.Dto.Responses.ShopResponses;

namespace AntiClownApiClient.ShopClient;

public interface IShopClient
{
    Task<UserShopResponseDto> GetAsync(ulong userId);
    Task<ItemIdInSlotResponseDto> ItemIdInSlotAsync(ulong userId, int slot);
    Task<ShopItemRevealResponseDto> ItemRevealAsync(ulong userId, Guid itemId);
    Task<BuyItemResponseDto> BuyAsync(ulong userId, Guid itemId);
    Task<ReRollResponseDto> ReRollAsync(ulong userId);
}