using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Items;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Api.Core.Inventory.Services;

public class ItemsValidator : IItemsValidator
{
    public ItemsValidator(
        IEconomyService economyService,
        IItemsRepository itemsRepository,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.economyService = economyService;
        this.itemsRepository = itemsRepository;
        this.antiClownDataApiClient = antiClownDataApiClient;
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
        var currentUserItemsOfThisType = await itemsRepository.FindAsync(
            new ItemsFilter
            {
                OwnerId = userId,
                IsActive = true,
                Name = itemName,
            }
        );
        var maxActiveItemsOfOneType = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.Inventory, "MaximumActiveItemsOfOneType");
        if (currentUserItemsOfThisType.Length >= maxActiveItemsOfOneType)
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
        var sellItemPercent = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.Inventory, "SellItemPercent");
        var cost = item.Price * sellItemPercent / 100;
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
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}