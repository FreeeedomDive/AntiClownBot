using AntiClown.Api.Core.Common;
using AntiClown.Api.Core.Shops.Domain;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Shops;
using AntiClown.Tools.Utility.Extensions;
using FluentAssertions;
using SqlRepositoryBase.Core.Exceptions;

namespace AntiClown.Api.Core.IntegrationTests.Shops;

public class ShopsServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task EconomySetUp()
    {
        try
        {
            startShop = await ShopsService.ReadCurrentShopAsync(User.Id);
        }
        catch (SqlEntityNotFoundException)
        {
            Assert.Fail($"{nameof(NewUserService)} did not create shop for user {User.Id}");
        }
    }

    [Test]
    public void NewUserService_Should_CreateNewShopWithItems()
    {
        var @default = Shop.Default;
        startShop.Id.Should().Be(User.Id);
        startShop.ReRollPrice.Should().Be(@default.ReRollPrice);
        startShop.FreeReveals.Should().Be(@default.FreeReveals);
        startShop.Items.Length.Should().Be(Constants.MaximumItemsInShop);
    }

    [Test]
    public async Task ShopsService_Reveal_Should_ThrowIfNotEnoughBalance()
    {
        var economy = await EconomyService.ReadEconomyAsync(User.Id);
        var shop = await ShopsRepository.ReadAsync(User.Id);
        shop.FreeReveals = 0;
        await ShopsRepository.UpdateAsync(shop);
        await EconomyService.UpdateScamCoinsAsync(User.Id, -economy.ScamCoins,
            "Тест валидации распознавания предмета из магазина");
        var itemToReveal = startShop.Items.SelectRandomItem();
        var action = () => ShopsService.RevealAsync(User.Id, itemToReveal.Id);
        await action.Should().ThrowAsync<NotEnoughBalanceException>();
        var shopItem = await ShopItemsRepository.TryReadAsync(itemToReveal.Id);
        shopItem.Should().NotBeNull();
        shopItem!.IsRevealed.Should().BeFalse();
    }

    [Test]
    public async Task ShopsService_Reveal_Should_SpendFreeRevealIfItExists()
    {
        var freeReveals = startShop.FreeReveals;
        freeReveals.Should().BePositive();
        var itemToReveal = startShop.Items.SelectRandomItem();
        var shopItem = await ShopsService.RevealAsync(User.Id, itemToReveal.Id);
        shopItem.IsRevealed.Should().BeTrue();
        var updatedShop = await ShopsService.ReadCurrentShopAsync(User.Id);
        updatedShop.FreeReveals.Should().Be(freeReveals - 1);
    }

    [Test]
    public async Task ShopsService_Reveal_Should_SpendAllFreeReveals_ThenSpendBalance()
    {
        var totalReveals = startShop.FreeReveals;
        await EconomyService.UpdateScamCoinsAsync(User.Id, 10000, "Тест распознавания предметов");
        var startEconomy = await EconomyService.ReadEconomyAsync(User.Id);
        for (var i = 0; i < totalReveals + 1; i++)
        {
            var items = await ShopItemsRepository.FindAsync(User.Id);
            var notRevealedItems = items.Where(x => !x.IsRevealed);
            var itemToReveal = notRevealedItems.SelectRandomItem();
            await ShopsService.RevealAsync(User.Id, itemToReveal.Id);
            var shop = await ShopsService.ReadCurrentShopAsync(User.Id);
            shop.FreeReveals.Should().Be(Math.Max(totalReveals - i - 1, 0));
            var shopItem = await ShopItemsRepository.TryReadAsync(itemToReveal.Id);
            shopItem!.IsRevealed.Should().BeTrue();
            if (i != totalReveals)
            {
                continue;
            }

            /* last iteration */
            var revealPrice = itemToReveal.Price * Constants.RevealShopItemPercent / 100;
            var economy = await EconomyService.ReadEconomyAsync(User.Id);
            economy.ScamCoins.Should().Be(startEconomy.ScamCoins - revealPrice);
        }
    }

    [Test]
    public async Task ShopsService_Reveals_Should_ChangeStatistics()
    {
        var totalReveals = startShop.FreeReveals;
        await EconomyService.UpdateScamCoinsAsync(User.Id, 10000, "Тест распознавания предметов");
        var startStats = await ShopStatsRepository.ReadAsync(User.Id);
        for (var i = 0; i < totalReveals; i++)
        {
            var items = await ShopItemsRepository.FindAsync(User.Id);
            var notRevealedItems = items.Where(x => !x.IsRevealed);
            var itemToReveal = notRevealedItems.SelectRandomItem();
            await ShopsService.RevealAsync(User.Id, itemToReveal.Id);
            var stats = await ShopStatsRepository.ReadAsync(User.Id);
            stats.TotalReveals.Should().Be(startStats.TotalReveals + i + 1);
            stats.ScamCoinsLostOnReveals.Should().Be(startStats.ScamCoinsLostOnReveals);
        }

        var items2 = await ShopItemsRepository.FindAsync(User.Id);
        var notRevealedItems2 = items2.Where(x => !x.IsRevealed);
        var itemToReveal2 = notRevealedItems2.SelectRandomItem();
        await ShopsService.RevealAsync(User.Id, itemToReveal2.Id);
        var revealPrice = itemToReveal2.Price * Constants.RevealShopItemPercent / 100;
        var stats2 = await ShopStatsRepository.ReadAsync(User.Id);
        stats2.TotalReveals.Should().Be(startStats.TotalReveals + totalReveals + 1);
        stats2.ScamCoinsLostOnReveals.Should().Be(startStats.ScamCoinsLostOnReveals + revealPrice);
    }

    [Test]
    public async Task ShopsService_Buy_Should_ThrowIfNotEnoughBalance()
    {
        var economy = await EconomyService.ReadEconomyAsync(User.Id);
        await EconomyService.UpdateScamCoinsAsync(User.Id, -economy.ScamCoins,
            "Тест валидации покупки предмета из магазина");
        var item = startShop.Items.SelectRandomItem();
        var action = () => ShopsService.BuyAsync(User.Id, item.Id);
        await action.Should().ThrowAsync<NotEnoughBalanceException>();
    }

    [Test]
    public async Task ShopsService_Buy_Should_ThrowIfItemAlreadyBought()
    {
        await EconomyService.UpdateScamCoinsAsync(User.Id, 20000,
            "Тест валидации на повторную покупку предмета из магазина");
        var item = startShop.Items.SelectRandomItem();
        await ShopsService.BuyAsync(User.Id, item.Id);
        var action = () => ShopsService.BuyAsync(User.Id, item.Id);
        await action.Should().ThrowAsync<ShopItemAlreadyBoughtException>();
    }

    [Test]
    public async Task ShopsService_Buy_Should_AddItemToUserInventory()
    {
        await EconomyService.UpdateScamCoinsAsync(User.Id, 20000,
            "Тест покупки предмета из магазина");
        var item = startShop.Items.SelectRandomItem();
        var boughtItem = await ShopsService.BuyAsync(User.Id, item.Id);
        var inventoryItem = await ItemsService.ReadItemAsync(User.Id, boughtItem.Id);
        boughtItem.Should().BeEquivalentTo(inventoryItem);
    }

    [Test]
    public async Task ShopsService_Buy_Should_GenerateInventoryItemWithSameCharacteristics()
    {
        await EconomyService.UpdateScamCoinsAsync(User.Id, 20000,
            "Тест покупки предмета из магазина");
        var item = startShop.Items.SelectRandomItem();
        var boughtItem = await ShopsService.BuyAsync(User.Id, item.Id);
        boughtItem.ItemName.Should().Be(item.Name);
        boughtItem.Price.Should().Be(item.Price);
        boughtItem.Rarity.Should().Be(item.Rarity);
        boughtItem.OwnerId.Should().Be(item.ShopId);
    }

    [Test]
    public async Task ShopsService_Buy_Should_ThrowWhenRevealAlreadyBoughtItem()
    {
        await EconomyService.UpdateScamCoinsAsync(User.Id, 20000,
            "Тест покупки предмета из магазина");
        var item = startShop.Items.SelectRandomItem();
        await ShopsService.BuyAsync(User.Id, item.Id);
        var boughtShopItem = await ShopItemsRepository.TryReadAsync(item.Id);
        boughtShopItem!.IsRevealed.Should().BeFalse();
        boughtShopItem.IsOwned.Should().BeTrue();
        var action = () => ShopsService.RevealAsync(User.Id, item.Id);
        await action.Should().ThrowAsync<ShopItemAlreadyBoughtException>();
    }

    private CurrentShopInfo startShop = null!;
}