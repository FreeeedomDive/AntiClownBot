using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public class CommunismBannerBuilder : ItemBuilder
{
    public CommunismBannerBuilder(Guid id, Rarity rarity, int price)
    {
        Id = id;
        Rarity = rarity;
        Price = price;
    }

    public CommunismBannerBuilder WithRandomDistributedStats()
    {
        if (!IsRarityDefined())
        {
            throw new InvalidOperationException("Undefined item rarity");
        }

        var randomStats = RandomStatsDistributions[Rarity]();
        for (var i = 0; i < randomStats; i++)
        {
            switch (Randomizer.CoinFlip())
            {
                case true:
                    tributeDivideChance += 2;
                    break;
                case false:
                    tributeStealChance++;
                    break;
            }
        }

        return this;
    }

    public CommunismBanner Build()
    {
        return new CommunismBanner
        {
            Id = Id,
            Price = Price,
            Rarity = Rarity,
            IsActive = true,
            DivideChance = tributeDivideChance,
            StealChance = tributeStealChance,
        };
    }

    private int tributeDivideChance = BaseTributeDivideChance;
    private int tributeStealChance = BaseTributeStealChance;

    private const int BaseTributeDivideChance = 1;
    private const int BaseTributeStealChance = 1;

    private static readonly Dictionary<Rarity, Func<int>> RandomStatsDistributions = new()
    {
        { Rarity.Common, () => Randomizer.GetRandomNumberInclusive(3, 5) },
        { Rarity.Rare, () => Randomizer.GetRandomNumberInclusive(7, 9) },
        { Rarity.Epic, () => Randomizer.GetRandomNumberInclusive(11, 13) },
        { Rarity.Legendary, () => Randomizer.GetRandomNumberInclusive(15, 17) },
        { Rarity.BlackMarket, () => 20 },
    };
}