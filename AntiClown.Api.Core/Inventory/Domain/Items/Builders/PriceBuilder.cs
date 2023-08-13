using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public static class PriceBuilder
{
    public static int Build(Rarity rarity)
    {
        return Prices[rarity];
    }

    private static readonly Dictionary<Rarity, int> Prices = new()
    {
        { Rarity.Common, 1000 },
        { Rarity.Rare, 2500 },
        { Rarity.Epic, 4500 },
        { Rarity.Legendary, 10000 },
        { Rarity.BlackMarket, 20000 },
    };
}