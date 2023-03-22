using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Shops.Domain;

namespace AntiClown.Api.Core.Shops.Services;

public interface IShopsService
{
    Task CreateNewShopForUserAsync(Guid userId);
    Task ResetShop(Guid userId);
    Task<CurrentShopInfo> ReadCurrentShopAsync(Guid userId);
    Task<ShopItem> RevealAsync(Guid userId, Guid itemId);
    Task<BaseItem> BuyAsync(Guid userId, Guid itemId);
    Task ReRollAsync(Guid userId);
}