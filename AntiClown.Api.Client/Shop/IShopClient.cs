using AntiClown.Api.Dto.Inventories;
using AntiClown.Api.Dto.Shops;

namespace AntiClown.Api.Client.Shop;

public interface IShopClient
{
    Task<CurrentShopInfoDto> ReadAsync(Guid shopId);
    Task<ShopItemDto> RevealItemAsync(Guid shopId, Guid itemId);
    Task<BaseItemDto> BuyItemAsync(Guid shopId, Guid itemId);
    Task ReRollShopAsync(Guid shopId);
}