using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public static class ItemNameBuilder
{
    public static ItemName Build()
    {
        var goodItemNames = GoodItemNames.SelectMany(x => Enumerable.Repeat(x, goodItemGenerationMultiplier));
        var badItemNames = BadItemNames.SelectMany(x => Enumerable.Repeat(x, badItemGenerationMultiplier));
        var allItemNames = goodItemNames.Concat(badItemNames);
        return allItemNames.SelectRandomItem();
    }

    public static ItemName BuildGoodItemName()
    {
        return GoodItemNames.SelectRandomItem();
    }

    public static ItemName BuildBadItemName()
    {
        return BadItemNames.SelectRandomItem();
    }

    private static readonly ItemName[] GoodItemNames =
    {
        ItemName.CatWife,
        ItemName.DogWife,
        ItemName.Internet,
        ItemName.RiceBowl,
    };

    private static readonly ItemName[] BadItemNames =
    {
        ItemName.JadeRod,
        ItemName.CommunismBanner,
    };
    
    private static int goodItemGenerationMultiplier = 5;
    private static int badItemGenerationMultiplier = 1;
}