using AntiClown.Api.Core.Common;
using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Builders;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Items;
using FluentAssertions;
using SqlRepositoryBase.Core.Exceptions;

namespace IntegrationTests.Inventory;

public class ItemsServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task ItemsService_Should_WriteItems()
    {
        var currentItems = await ItemsService.ReadAllItemsForUserAsync(User.Id);
        currentItems.Should().BeEmpty();
        var item = ItemBuilder.BuildRandomItem();
        await ItemsService.WriteItemAsync(User.Id, item);
        currentItems = await ItemsService.ReadAllItemsForUserAsync(User.Id);
        currentItems.Should().BeEquivalentTo(new[] { item });
    }

    [Test]
    public async Task ItemsService_SetInactiveStatusForNegativeItem_Should_ThrowOnValidation()
    {
        var negativeItem = ItemBuilder.BuildRandomItem(config => config.Type = ItemType.Negative);
        negativeItem.IsActive.Should().BeTrue();
        await ItemsService.WriteItemAsync(User.Id, negativeItem);
        negativeItem = await ItemsService.ReadItemAsync(User.Id, negativeItem.Id);
        negativeItem.IsActive.Should().BeTrue();
        var setInactiveStatus = () => ItemsService.ChangeItemActiveStatusAsync(User.Id, negativeItem.Id, false);
        await setInactiveStatus.Should().ThrowAsync<ForbiddenInactiveStatusForNegativeItemException>();
    }

    [Test]
    public async Task ItemsService_SetActiveStatusForMoreThanMaximumActiveItems_Should_ThrowOnValidation()
    {
        var positiveItems = Enumerable
            .Range(0, Constants.MaximumActiveItemsOfOneType + 1)
            .Select(_ => ItemBuilder.BuildRandomItem(config => config.Name = ItemName.CatWife))
            .ToArray();
        for (var i = 0; i < positiveItems.Length; i++)
        {
            var item = positiveItems[i];
            item.IsActive.Should().BeFalse();
            await ItemsService.WriteItemAsync(User.Id, item);
            var setActiveStatus = () => ItemsService.ChangeItemActiveStatusAsync(User.Id, item.Id, true);
            if (i < Constants.MaximumActiveItemsOfOneType)
            {
                await setActiveStatus.Should().NotThrowAsync<TooManyActiveItemsCountException>();
            }
            else
            {
                await setActiveStatus.Should().ThrowAsync<TooManyActiveItemsCountException>();
            }
        }
    }

    [Test]
    public async Task ItemsService_SellPositiveItem_Should_IncreaseMoney()
    {
        var item = ItemBuilder.BuildRandomItem(config => config.Type = ItemType.Positive);
        await ItemsService.WriteItemAsync(User.Id, item);
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);
        await ItemsService.SellItemAsync(User.Id, item.Id);
        var readItem = () => ItemsService.ReadItemAsync(User.Id, item.Id);
        await readItem.Should().ThrowAsync<SqlEntityNotFoundException>();
        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.ScamCoins.Should().Be(economyBefore.ScamCoins + item.Price * Constants.SellItemPercent / 100);
    }

    [Test]
    public async Task ItemsService_SellNegativeItem_Should_DecreaseMoney()
    {
        var item = ItemBuilder.BuildRandomItem(config => config.Type = ItemType.Negative);
        await ItemsService.WriteItemAsync(User.Id, item);
        await EconomyService.UpdateScamCoinsAsync(User.Id, 10000, "Тест продажи предмета");
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);
        await ItemsService.SellItemAsync(User.Id, item.Id);
        var readItem = () => ItemsService.ReadItemAsync(User.Id, item.Id);
        await readItem.Should().ThrowAsync<SqlEntityNotFoundException>();
        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.ScamCoins.Should().Be(economyBefore.ScamCoins - item.Price * Constants.SellItemPercent / 100);
    }

    [Test]
    public async Task ItemsService_SellNegativeItemIfNotEnoughMoney_Should_ThrowOnValidation()
    {
        var item = ItemBuilder.BuildRandomItem(config => config.Type = ItemType.Negative);
        await ItemsService.WriteItemAsync(User.Id, item);
        await EconomyService.UpdateScamCoinsAsync(User.Id, -1499, "Тест валидации продажи предмета");
        var sellItem = () => ItemsService.SellItemAsync(User.Id, item.Id);
        await sellItem.Should().ThrowAsync<NotEnoughBalanceException>();
    }

    [Test]
    public async Task ItemsService_OpenLootBoxIfNotLootBoxesAvailable_Should_ThrowOnValidation()
    {
        var economy = await EconomyService.ReadEconomyAsync(User.Id);
        economy.LootBoxes.Should().Be(0);
        var openLootBox = () => ItemsService.OpenLootBoxAsync(User.Id);
        await openLootBox.Should().ThrowAsync<LootBoxNotFoundException>();
    }

    [Test]
    public async Task ItemsService_OpenLootBox_Should_AddScamCoins()
    {
        await EconomyService.UpdateLootBoxesAsync(User.Id, 1);
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);
        var lootBoxReward = await ItemsService.OpenLootBoxAsync(User.Id);
        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.ScamCoins.Should().Be(economyBefore.ScamCoins + lootBoxReward.ScamCoinsReward);
    }

    [Test]
    public async Task ItemsService_OpenLootBox_Should_AddItems()
    {
        var oneItemWasAwarded = false;
        var twoItemsWereAwarded = false;
        for (var i = 0; i < OpenLootBoxTries; i++)
        {
            await EconomyService.UpdateLootBoxesAsync(User.Id, 1);
            var lootBoxReward = await ItemsService.OpenLootBoxAsync(User.Id);
            oneItemWasAwarded = oneItemWasAwarded || lootBoxReward.Items.Length == 1;
            twoItemsWereAwarded = twoItemsWereAwarded || lootBoxReward.Items.Length == 2;
            foreach (var item in lootBoxReward.Items)
            {
                var itemFromRepository = await ItemsService.ReadItemAsync(User.Id, item.Id);
                itemFromRepository.Should().BeEquivalentTo(item);
            }

            if (oneItemWasAwarded && twoItemsWereAwarded)
            {
                Assert.Pass();
            }
        }

        if (!oneItemWasAwarded)
        {
            Assert.Fail("OpenLootBox should sometimes create lootboxes with one item");
        }

        if (!twoItemsWereAwarded)
        {
            Assert.Fail("OpenLootBox should sometimes create lootboxes with two items");
        }
    }

    private const int OpenLootBoxTries = 1_000;
}