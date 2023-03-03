using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Services;

public interface IItemsService
{
    Task<BaseItem[]> ReadAllItemsForUserAsync(Guid userId);
    Task<BaseItem[]> ReadAllActiveItemsForUserAsync(Guid userId);
    Task OpenLootBoxAsync(Guid userId);
    Task ChangeItemActiveStatusAsync(Guid userId, Guid itemId, bool isActive);
    Task SellItemAsync(Guid userId, Guid itemId);
}