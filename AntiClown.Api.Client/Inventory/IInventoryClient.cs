using AntiClown.Api.Dto.Inventories;

namespace AntiClown.Api.Client.Inventory;

public interface IInventoryClient
{
    Task<InventoryDto> ReadInventoryAsync(Guid userId);
    Task<BaseItemDto> ReadItemAsync(Guid userId, Guid itemId);
    Task<LootBoxRewardDto> OpenLootBoxAsync(Guid userId);
    Task ChangeItemActiveStatusAsync(Guid userId, Guid itemId, bool isActive);
    Task SellAsync(Guid userId, Guid itemId);
}