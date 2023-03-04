using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public static class PriceBuilder
{
    private static readonly Dictionary<Rarity, int> Prices = new()
    {
        {Rarity.Common, 1000},
        {Rarity.Rare, 2500},
        {Rarity.Epic, 4500},
        {Rarity.Legendary, 10000},
        {Rarity.BlackMarket, 20000}
    };
    
    public static int Build(Rarity rarity)
    {
        return Prices[rarity];
    }
}