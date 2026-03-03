using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Services;

public interface IItemsValidator
{
    Task ValidateEditItemActiveStatusAsync(Guid userId, BaseItem item, bool isActive);
    Task ValidateSellItemAsync(Guid userId, BaseItem item);
    Task ValidateOpenLootBoxAsync(Guid userId);
}