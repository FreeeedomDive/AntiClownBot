using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Shops.Domain;

namespace AntiClown.Api.Core.Shops.Services;

public interface IShopsService
{
    Task CreateNewShopForUserAsync(Guid userId);
    Task ResetShop(Guid shopId);
    Task<CurrentShopInfo> ReadCurrentShopAsync(Guid shopId);
    Task<ShopItem> RevealAsync(Guid shopId, Guid itemId);
    Task<BaseItem> BuyAsync(Guid shopId, Guid itemId);
    Task ReRollAsync(Guid shopId);
    Task<ShopStats> ReasStats(Guid shopId);
}