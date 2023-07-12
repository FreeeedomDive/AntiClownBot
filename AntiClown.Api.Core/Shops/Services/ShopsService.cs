using AntiClown.Api.Core.Common;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Builders;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Core.Shops.Domain;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AntiClown.Tools.Utility.Extensions;
using AutoMapper;

namespace AntiClown.Api.Core.Shops.Services;

public class ShopsService : IShopsService
{
    public ShopsService(
        IShopsRepository shopsRepository,
        IShopItemsRepository shopItemsRepository,
        IShopStatsRepository shopStatsRepository,
        IShopsValidator shopsValidator,
        IEconomyService economyService,
        IItemsService itemsService,
        IMapper mapper
    )
    {
        this.shopsRepository = shopsRepository;
        this.shopItemsRepository = shopItemsRepository;
        this.shopStatsRepository = shopStatsRepository;
        this.shopsValidator = shopsValidator;
        this.economyService = economyService;
        this.itemsService = itemsService;
        this.mapper = mapper;
    }

    public async Task CreateNewShopForUserAsync(Guid userId)
    {
        await ResetShop(userId, true);
        await ResetShopItemsAsync(userId);
        await shopStatsRepository.CreateAsync(new ShopStats { Id = userId });
    }

    public async Task ResetShopAsync(Guid shopId)
    {
        await ResetShop(shopId, false);
    }

    public async Task<CurrentShopInfo> ReadCurrentShopAsync(Guid shopId)
    {
        var shop = await shopsRepository.ReadAsync(shopId);
        var items = await shopItemsRepository.FindAsync(shopId);
        return new CurrentShopInfo
        {
            Id = shopId,
            ReRollPrice = shop.ReRollPrice,
            FreeReveals = shop.FreeReveals,
            Items = items,
        };
    }

    public async Task<ShopItem> RevealAsync(Guid shopId, Guid itemId)
    {
        var shop = await shopsRepository.ReadAsync(shopId);
        await shopsValidator.ValidateRevealAsync(shop, itemId);
        var item = (await shopItemsRepository.TryReadAsync(itemId))!;
        if (item.IsRevealed || item.IsOwned)
        {
            return item;
        }

        item.IsRevealed = true;
        await shopItemsRepository.UpdateAsync(item);

        var price = shop.FreeReveals > 0 ? 0 : item.Price * Constants.RevealShopItemPercent / 100;
        if (shop.FreeReveals > 0)
        {
            shop.FreeReveals--;
            await shopsRepository.UpdateAsync(shop);
        }
        else
        {
            await economyService.UpdateScamCoinsAsync(shopId, -price, $"Распознавание предмета {itemId}");
        }

        var stats = await shopStatsRepository.ReadAsync(shopId);
        stats.TotalReveals++;
        stats.ScamCoinsLostOnReveals += price;
        await shopStatsRepository.UpdateAsync(stats);

        return item;
    }

    public async Task<BaseItem> BuyAsync(Guid shopId, Guid itemId)
    {
        var shop = await shopsRepository.ReadAsync(shopId);
        await shopsValidator.ValidateBuyAsync(shop, itemId);
        var shopItem = (await shopItemsRepository.TryReadAsync(itemId))!;
        var newInventoryItem = ItemBuilder.BuildRandomItem(options =>
        {
            options.Name = shopItem.Name;
            options.Rarity = shopItem.Rarity;
        });

        shopItem.IsOwned = true;
        await shopItemsRepository.UpdateAsync(shopItem);

        await itemsService.WriteItemAsync(shopId, newInventoryItem);

        await economyService.UpdateScamCoinsAsync(shopId, -shopItem.Price, $"Покупка предмета {newInventoryItem.Id}");

        var stats = await shopStatsRepository.ReadAsync(shopId);
        stats.ItemsBought++;
        stats.ScamCoinsLostOnPurchases += shopItem.Price;
        await shopStatsRepository.UpdateAsync(stats);

        return newInventoryItem;
    }

    public async Task ReRollAsync(Guid shopId)
    {
        var shop = await shopsRepository.ReadAsync(shopId);
        await shopsValidator.ValidateReRollAsync(shop);

        await ResetShopItemsAsync(shopId);

        var price = shop.ReRollPrice;
        shop.ReRollPrice = Math.Max(
            Constants.DefaultReRollPrice,
            price + Constants.DefaultReRollPriceIncrease
        );
        await shopsRepository.UpdateAsync(shop);

        await economyService.UpdateScamCoinsAsync(shopId, -price, "Реролл магазина");

        var stats = await shopStatsRepository.ReadAsync(shopId);
        stats.TotalReRolls++;
        stats.ScamCoinsLostOnReRolls += price;
        await shopStatsRepository.UpdateAsync(stats);
    }

    public async Task<ShopStats> ReadStatsAsync(Guid shopId)
    {
        return await shopStatsRepository.ReadAsync(shopId);
    }

    private async Task ResetShopItemsAsync(Guid shopId)
    {
        var currentItems = await shopItemsRepository.FindAsync(shopId);
        await shopItemsRepository.DeleteManyAsync(currentItems.Select(x => x.Id).ToArray());
        var newItems = Enumerable.Range(0, Constants.MaximumItemsInShop)
            .Select(_ => ItemBuilder.BuildRandomItem())
            .Select(x => mapper.Map<ShopItem>(x))
            .Pipe(x => x.ShopId = shopId)
            .ToArray();
        await shopItemsRepository.CreateManyAsync(newItems);
    }

    private async Task ResetShop(Guid shopId, bool create)
    {
        var shop = Shop.Default;
        shop.Id = shopId;
        if (create)
        {
            await shopsRepository.CreateAsync(shop);
        }
        else
        {
            await shopsRepository.UpdateAsync(shop);
        }
    }

    private readonly IShopsRepository shopsRepository;
    private readonly IShopItemsRepository shopItemsRepository;
    private readonly IShopStatsRepository shopStatsRepository;
    private readonly IShopsValidator shopsValidator;
    private readonly IEconomyService economyService;
    private readonly IItemsService itemsService;
    private readonly IMapper mapper;
}