using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Services;

public interface IItemsService
{
    Task<BaseItem> ReadItemAsync(Guid userId, Guid itemId);
    Task<BaseItem[]> ReadAllItemsForUserAsync(Guid userId);
    Task<BaseItem[]> ReadAllActiveItemsForUserAsync(Guid userId);
    Task<LootBoxReward> OpenLootBoxAsync(Guid userId);
    Task ChangeItemActiveStatusAsync(Guid userId, Guid itemId, bool isActive);
    Task SellItemAsync(Guid userId, Guid itemId);
    Task WriteItemAsync(Guid userId, BaseItem item);
}