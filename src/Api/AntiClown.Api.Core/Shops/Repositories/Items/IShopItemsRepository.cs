using AntiClown.Api.Core.Shops.Domain;

namespace AntiClown.Api.Core.Shops.Repositories.Items;

public interface IShopItemsRepository
{
    Task<ShopItem?> TryReadAsync(Guid id);
    Task<ShopItem[]> FindAsync(Guid shopId);
    Task CreateManyAsync(ShopItem[] items);
    Task UpdateAsync(ShopItem item);
    Task DeleteManyAsync(Guid[] ids);
}