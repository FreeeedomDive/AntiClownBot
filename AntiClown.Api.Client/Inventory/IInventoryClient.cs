using AntiClown.Api.Dto.Inventories;

namespace AntiClown.Api.Client.Inventory;

public interface IInventoryClient
{
    Task<InventoryDto> ReadAllAsync(Guid userId);
    Task<BaseItemDto> ReadAsync(Guid userId, Guid itemId);
    Task<LootBoxRewardDto> OpenLootBoxAsync(Guid userId);
    Task ChangeItemActiveStatusAsync(Guid userId, Guid itemId, bool isActive);
    Task SellAsync(Guid userId, Guid itemId);
}