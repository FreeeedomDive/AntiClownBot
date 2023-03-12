using AntiClown.Api.Core.Common;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Items;

namespace AntiClown.Api.Core.Inventory.Services;

public class ItemsValidator : IItemsValidator
{
    public ItemsValidator(
        IEconomyService economyService,
        IItemsRepository itemsRepository
    )
    {
        this.economyService = economyService;
        this.itemsRepository = itemsRepository;
    }

    public async Task ValidateEditItemActiveStatusAsync(Guid userId, BaseItem item, bool isActive)
    {
        switch (isActive)
        {
            case false when item.ItemType == ItemType.Negative:
                throw new ForbiddenInactiveStatusForNegativeItemException(item.Id);
            case false:
                return;
        }

        var itemName = item.ItemName.ToString();
        var currentUserItemsOfThisType = await itemsRepository.FindAsync(new ItemsFilter
        {
            OwnerId = userId,
            IsActive = true,
            Name = itemName,
        });
        if (currentUserItemsOfThisType.Length >= Constants.MaximumActiveItemsOfOneType)
        {
            throw new TooManyActiveItemsCountException(userId, itemName);
        }
    }

    public async Task ValidateSellItemAsync(Guid userId, BaseItem item)
    {
        if (item.ItemType == ItemType.Positive)
        {
            return;
        }
        var userEconomy = await economyService.ReadEconomyAsync(userId);
        var cost = item.Price * Constants.SellItemPercent / 100;
        if (userEconomy.ScamCoins < cost)
        {
            throw new NotEnoughBalanceException(userId, userEconomy.ScamCoins, cost);
        }
    }

    public async Task ValidateOpenLootBoxAsync(Guid userId)
    {
        var userEconomy = await economyService.ReadEconomyAsync(userId);
        if (userEconomy.LootBoxes == 0)
        {
            throw new LootBoxNotFoundException(userId);
        }
    }

    private readonly IEconomyService economyService;
    private readonly IItemsRepository itemsRepository;
}