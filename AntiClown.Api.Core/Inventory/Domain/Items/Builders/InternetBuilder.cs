using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Api.Core.Inventory.Domain.Items.Builders;

public class InternetBuilder : ItemBuilder
{
    public InternetBuilder(Guid id, Rarity rarity, int price)
    {
        Id = id;
        Rarity = rarity;
        Price = price;
        IsActive = false;
    }

    public InternetBuilder WithRandomCooldownReducePercent()
    {
        if (!IsRarityDefined())
        {
            throw new ArgumentException("Item rarity is not defined");
        }

        cooldownReducePercent = CooldownReducePercentDistribution[Rarity];

        return this;
    }

    public InternetBuilder WithRandomCooldownReduceTries()
    {
        if (!IsRarityDefined())
        {
            throw new ArgumentException("Item rarity is not defined");
        }

        cooldownReduceTries = CooldownReduceTriesDistribution[Rarity];

        return this;
    }

    public InternetBuilder WithRandomCooldownReduceChance()
    {
        if (!IsRarityDefined())
        {
            throw new ArgumentException("Item rarity is not defined");
        }

        cooldownReduceChance = CooldownReduceChanceDistribution[Rarity];

        return this;
    }

    public InternetBuilder WithRandomDistributedStats()
    {
        if (!IsRarityDefined())
        {
            throw new InvalidOperationException("Undefined item rarity");
        }

        var randomStats = RandomStatsDistributions[Rarity]();
        for (var i = 0; i < randomStats; i++)
        {
            switch (Randomizer.GetRandomNumberBetween(0, 2))
            {
                case 0:
                    cooldownReducePercent++;
                    break;
                case 1:
                    cooldownReduceTries++;
                    break;
                case 2:
                    cooldownReduceChance++;
                    break;
            }
        }

        return this;
    }

    public Internet Build()
    {
        return new Internet
        {
            Id = Id,
            Price = Price,
            Rarity = Rarity,
            IsActive = false,
            Speed = cooldownReducePercent,
            Gigabytes = cooldownReduceTries,
            Ping = cooldownReduceChance,
        };
    }

    private int cooldownReduceChance;

    private int cooldownReducePercent;
    private int cooldownReduceTries;

    private const int BaseCooldownReducePercent = 8;
    private const int BaseCooldownReduceTries = 1;
    private const int BaseCooldownReduceChance = 3;

    private static readonly Dictionary<Rarity, int> CooldownReducePercentDistribution = new()
    {
        { Rarity.Common, BaseCooldownReducePercent + 0 },
        { Rarity.Rare, BaseCooldownReducePercent + 1 },
        { Rarity.Epic, BaseCooldownReducePercent + 2 },
        { Rarity.Legendary, BaseCooldownReducePercent + 3 },
        { Rarity.BlackMarket, BaseCooldownReducePercent + 4 },
    };

    private static readonly Dictionary<Rarity, int> CooldownReduceTriesDistribution = new()
    {
        { Rarity.Common, BaseCooldownReduceTries + 0 },
        { Rarity.Rare, BaseCooldownReduceTries + 1 },
        { Rarity.Epic, BaseCooldownReduceTries + 2 },
        { Rarity.Legendary, BaseCooldownReduceTries + 3 },
        { Rarity.BlackMarket, BaseCooldownReduceTries + 4 },
    };


    private static readonly Dictionary<Rarity, int> CooldownReduceChanceDistribution = new()
    {
        { Rarity.Common, BaseCooldownReduceChance + 0 },
        { Rarity.Rare, BaseCooldownReduceChance + 1 },
        { Rarity.Epic, BaseCooldownReduceChance + 2 },
        { Rarity.Legendary, BaseCooldownReduceChance + 3 },
        { Rarity.BlackMarket, BaseCooldownReduceChance + 4 },
    };

    private static readonly Dictionary<Rarity, Func<int>> RandomStatsDistributions = new()
    {
        { Rarity.Common, () => Randomizer.GetRandomNumberInclusive(1, 3) },
        { Rarity.Rare, () => Randomizer.GetRandomNumberInclusive(1, 4) },
        { Rarity.Epic, () => Randomizer.GetRandomNumberInclusive(2, 4) },
        { Rarity.Legendary, () => Randomizer.GetRandomNumberInclusive(2, 5) },
        { Rarity.BlackMarket, () => Randomizer.GetRandomNumberInclusive(3, 5) },
    };
}