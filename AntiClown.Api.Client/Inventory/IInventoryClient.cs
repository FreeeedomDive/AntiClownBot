/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Inventory;

public interface IInventoryClient
{
    Task<AntiClown.Api.Dto.Inventories.InventoryDto> ReadAllAsync(System.Guid userId);
    Task<AntiClown.Api.Dto.Inventories.BaseItemDto> ReadAsync(System.Guid userId, System.Guid itemId);
    Task<AntiClown.Api.Dto.Inventories.LootBoxRewardDto> OpenLootBoxAsync(System.Guid userId);
    Task ChangeItemActiveStatusAsync(System.Guid userId, System.Guid itemId, bool isActive);
    Task SellAsync(System.Guid userId, System.Guid itemId);
}
