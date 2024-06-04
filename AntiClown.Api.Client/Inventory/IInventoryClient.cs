/* Generated file */
namespace AntiClown.Api.Client.Inventory;

public interface IInventoryClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.InventoryDto> ReadAllAsync(System.Guid userId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.BaseItemDto> ReadAsync(System.Guid userId, System.Guid itemId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.LootBoxRewardDto> OpenLootBoxAsync(System.Guid userId);
    System.Threading.Tasks.Task ChangeItemActiveStatusAsync(System.Guid userId, System.Guid itemId, System.Boolean isActive);
    System.Threading.Tasks.Task SellAsync(System.Guid userId, System.Guid itemId);
}
