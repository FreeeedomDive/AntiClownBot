using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Repositories;

namespace AntiClown.Api.Core.Inventory.Services;

public class ItemsService : IItemsService
{
    public ItemsService(
        IItemsValidator validator,
        IItemsRepository itemsRepository,
        IEconomyService economyService
    )
    {
        this.validator = validator;
        this.itemsRepository = itemsRepository;
        this.economyService = economyService;
    }

    public async Task<BaseItem[]> ReadAllItemsForUserAsync(Guid userId)
    {
        return await itemsRepository.FindAsync(new ItemsFilter
        {
            OwnerId = userId
        });
    }

    public async Task<BaseItem[]> ReadAllActiveItemsForUserAsync(Guid userId)
    {
        return await itemsRepository.FindAsync(new ItemsFilter
        {
            OwnerId = userId,
            IsActive = true
        });
    }

    public async Task OpenLootBoxAsync(Guid userId)
    {
        await validator.ValidateOpenLootBoxAsync(userId);
        // generate lootbox result
        // decrease lootboxes count and add scam coins => edit economy
        // if result has items, generate items and add to repository
    }

    public async Task ChangeItemActiveStatusAsync(Guid userId, Guid itemId, bool isActive)
    {
        var item = await itemsRepository.ReadAsync(itemId);
        await validator.ValidateEditItemActiveStatusAsync(userId, item, isActive);
        item.IsActive = isActive;
        await itemsRepository.UpdateAsync(item);
    }

    public async Task SellItemAsync(Guid userId, Guid itemId)
    {
        var item = await itemsRepository.ReadAsync(itemId);
        await validator.ValidateSellItemAsync(userId, item);
        var sign = item.ItemType == ItemType.Positive ? 1 : -1;
        var price = sign * item.Price * Constants.SellItemPercent / 100;
        await itemsRepository.DeleteAsync(itemId);
        await economyService.UpdateScamCoinsAsync(userId, price, $"Продажа предмета {itemId}");
    }

    private readonly IItemsValidator validator;
    private readonly IItemsRepository itemsRepository;
    private readonly IEconomyService economyService;
}