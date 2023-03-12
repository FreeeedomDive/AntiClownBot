using AntiClown.Api.Core.Common;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Shops.Domain;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Dto.Exceptions.Economy;
using AntiClown.Api.Dto.Exceptions.Shops;

namespace AntiClown.Api.Core.Shops.Services;

public class ShopsValidator : IShopsValidator
{
    public ShopsValidator(
        IEconomyService economyService,
        IShopItemsRepository shopItemsRepository
    )
    {
        this.economyService = economyService;
        this.shopItemsRepository = shopItemsRepository;
    }

    public async Task ValidateRevealAsync(Shop shop, Guid itemId)
    {
        var item = await shopItemsRepository.TryReadAsync(itemId);
        if (item is null)
        {
            throw new ShopItemNotFoundException(shop.Id, itemId);
        }

        if (shop.FreeReveals > 0)
        {
            return;
        }

        await ValidateBalanceAsync(shop.Id, item.Price * Constants.RevealShopItemPercent / 100);
    }

    public async Task ValidateBuyAsync(Shop shop, Guid itemId)
    {
        var item = await shopItemsRepository.TryReadAsync(itemId);
        if (item is null)
        {
            throw new ShopItemNotFoundException(shop.Id, itemId);
        }

        if (item.IsOwned)
        {
            throw new ShopItemAlreadyBoughtException(shop.Id, itemId);
        }

        await ValidateBalanceAsync(shop.Id, item.Price);
    }

    public async Task ValidateReRollAsync(Shop shop)
    {
        await ValidateBalanceAsync(shop.Id, shop.ReRollPrice);
    }

    private async Task ValidateBalanceAsync(Guid userId, int operationPrice)
    {
        var economy = await economyService.ReadEconomyAsync(userId);
        if (economy.ScamCoins < operationPrice)
        {
            throw new NotEnoughBalanceException(userId, economy.ScamCoins, operationPrice);
        }
    }

    private readonly IEconomyService economyService;
    private readonly IShopItemsRepository shopItemsRepository;
}