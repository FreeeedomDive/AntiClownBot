﻿using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Builders;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Services;

public class ItemsService : IItemsService
{
    public ItemsService(
        IItemsValidator validator,
        IItemsRepository itemsRepository,
        IEconomyService economyService,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.validator = validator;
        this.itemsRepository = itemsRepository;
        this.economyService = economyService;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task<BaseItem> ReadItemAsync(Guid userId, Guid itemId)
    {
        return await itemsRepository.ReadAsync(itemId);
    }

    public async Task<BaseItem[]> ReadAllItemsForUserAsync(Guid userId)
    {
        return await itemsRepository.FindAsync(
            new ItemsFilter
            {
                OwnerId = userId,
            }
        );
    }

    public async Task<BaseItem[]> ReadItemsWithNameAsync(ItemName name)
    {
        return await itemsRepository.FindAsync(
            new ItemsFilter
            {
                Name = name.ToString(),
            }
        );
    }

    public async Task<BaseItem[]> ReadAllActiveItemsForUserAsync(Guid userId)
    {
        return await itemsRepository.FindAsync(
            new ItemsFilter
            {
                OwnerId = userId,
                IsActive = true,
            }
        );
    }

    public async Task<LootBoxReward> OpenLootBoxAsync(Guid userId)
    {
        await validator.ValidateOpenLootBoxAsync(userId);
        var itemsRewardRules = new[]
        {
            () => Randomizer.GetRandomNumberBetween(0, 100) < 50,
            () => Randomizer.GetRandomNumberBetween(0, 100) == 0,
        };
        var lootBoxReward = new LootBoxReward
        {
            ScamCoinsReward = Randomizer.GetRandomNumberInclusive(100, 500),
            Items = itemsRewardRules
                    .Where(rule => rule())
                    .Select(_ => ItemBuilder.BuildRandomItem())
                    .ToArray(),
        };
        await economyService.UpdateLootBoxesAsync(userId, -1);
        await economyService.UpdateScamCoinsAsync(userId, lootBoxReward.ScamCoinsReward, "Открытие лутбокса");
        foreach (var item in lootBoxReward.Items)
        {
            await WriteItemAsync(userId, item);
        }

        return lootBoxReward;
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
        var sellItemPercent = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.Inventory, "SellItemPercent");
        var price = sign * item.Price * sellItemPercent / 100;
        await itemsRepository.DeleteAsync(itemId);
        await economyService.UpdateScamCoinsAsync(userId, price, $"Продажа предмета {itemId}");
    }

    public async Task WriteItemAsync(Guid userId, BaseItem item)
    {
        item.OwnerId = userId;
        await itemsRepository.CreateAsync(item);
    }

    private readonly IEconomyService economyService;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IItemsRepository itemsRepository;

    private readonly IItemsValidator validator;
}