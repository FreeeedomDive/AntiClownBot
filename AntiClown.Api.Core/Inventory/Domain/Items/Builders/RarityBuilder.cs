using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public static class RarityBuilder
{
    public static Rarity Build()
    {
        return Randomizer.GetRandomNumberBetween(0, 14000) switch
        {
            >= 0 and <= 9500 => Rarity.Common,
            > 9500 and <= 12700 => Rarity.Rare,
            > 12700 and <= 13940 => Rarity.Epic,
            > 13940 and <= 13998 => Rarity.Legendary,
            _ => Rarity.BlackMarket,
        };
    }
}